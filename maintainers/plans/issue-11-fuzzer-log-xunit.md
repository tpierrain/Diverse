# Fix issue #11 — `Fuzzer.Log` static + xUnit `ITestOutputHelper`

## Context

[Issue #11](https://github.com/tpierrain/Diverse/issues/11) reports that following the README
literally with xUnit crashes:

```csharp
public SampleTests(ITestOutputHelper output)
{
    Fuzzer.Log = output.WriteLine;
    _fuzzer = new Fuzzer();   // System.InvalidOperationException: There is no currently active test.
}
```

Three root causes, all in `Diverse/Fuzzer.cs`:

- **(a) The constructor logs eagerly.** `Fuzzer..ctor` (line 133) always calls
  `LogSeedAndTestInformations`. xUnit's `ITestOutputHelper.WriteLine` throws when no test is
  active — which is exactly the case in a test-class constructor. A logging concern kills a
  passing test.
- **(b) `Fuzzer.Log` is a shared mutable static** (line 109). Under
  `parallelizeTestCollections: true`, concurrent test classes overwrite each other's sink →
  races, output attached to the wrong test.
- **(c) The test name resolves to `(not found)`.** `FindTheNameOfTheTestInvolved()` (line 226)
  walks the stack for a `[Fact]`/`[Test]` frame; from a test-class constructor there is none yet.

Intended outcome: the exact snippet above works, parallel xUnit runs stop cross-contaminating,
the banner names the real test, and a broken sink can never take a test down — with **zero**
change to the determinism contract and **no** breaking change to the public API.

Decisions taken with Thomas (2026-08-15): deferred logging **+** a per-instance `WithLogger(...)`;
a throwing sink falls back to `Console` with a warning; a **null** `Fuzzer.Log` keeps throwing
`FuzzerException` (the throw simply moves to the first `Generate*`).

## Tracking

- [x] **Step 0 — Test helpers** (no assertions, nothing to test-drive) _(2026-08-15)_
  - [x] `Diverse.Tests/Utils/LogSpy.cs` — `IReadOnlyList<string> Lines` + a single cached `Action<string> Sink`
  - [x] `Diverse.Tests/Utils/TestOutputHelperOutsideOfAnActiveTestStub.cs` — cached `Action<string> WriteLine` that increments `CallCount` then throws `InvalidOperationException("There is no currently active test.")` (faithful xUnit behaviour, zero dependency)
- [x] **Step 1 — Defer the banner to first use** (fixes (a) and (c)) _(2026-08-15)_
  - [x] 🔴 T1 `Not_log_anything_at_construction_time` — seen red on "4 lines logged in the ctor"
  - [x] Extract `LogSeedAndTestInformations` into an **instance** `EnsureTheSeedHasBeenLogged()`, guarded by a `_seedHasBeenLogged` bool; call it from the `Random IFuzz.Random` getter
  - [x] 🟢 T1 + regression guards T2, T3, T4 green (they pin the exact banner text through the refactoring)
- [x] **Step 2 — Test name resolved from the real test** _(2026-08-15)_
  - [x] 🔴 T5 `Resolve_the_name_of_the_test_even_when_the_Fuzzer_was_built_outside_of_the_test_method` — verified red against the pre-fix library (`--- from the test: (not found)()`) by stashing the Step 1 implementation
  - [x] 🟢 Green as a consequence of Step 1, as expected
- [x] **Step 3 — Close the NoDuplication hole + silence the duplicate banner** _(2026-08-15)_
  - [x] 🔴 T6, 🔴 T7 — both red on the *internal* fuzzer's name being traced instead of the user's one, which confirms `IFuzz.Random` alone is not a complete seam
  - [x] `EnsureTheSeedHasBeenLogged()` as the first statement of `GenerateWithoutDuplication<T>`
  - [x] Private 4-param ctor + `_isASilentInternalFuzzer` flag for `SideEffectFreeFuzzerWithDuplicationAllowed` only (the `instanceLogger` parameter is added to that private ctor in Step 4, when a test demands it)
  - [x] 🟢 T6, T7 — full suite green, 208 tests
- [x] **Step 4 — Per-instance logger `WithLogger(...)`** (fixes (b)) _(2026-08-15)_
  - [x] Stub `public Fuzzer WithLogger(Action<string> logger) => this;` so the tests fail on the *assertion*, not on compilation
  - [x] 🔴 T8, 🔴 T12 (T9 green: it pins the fallback on the static `Log`) → implement `_instanceLogger` and the `_instanceLogger ?? Log` resolution → 🟢
  - [x] 🔴 T11 (`FuzzerException`: the derived fuzzer had no logger to fall back on) → propagate `_instanceLogger` to both derived-fuzzer creation sites → 🟢. **T10 was green on arrival** and is a property guard, not a driver: it cannot be expressed at all without `WithLogger`, so there was no pre-existing red to see.
- [x] **Step 5 — A throwing sink must never break a test** _(2026-08-15)_
  - [x] 🔴 T13, T14 (the `InvalidOperationException` propagated) → split into a pure `BuildSeedAndTestInformationLines()` + an `Emit()` with try/catch and Console fallback → 🟢
  - [x] 🔴 T15 (the message lacked `WithLogger`) → `FuzzerException` raised **outside** the try/catch, flag not set in that branch, and the xUnit guidance in `BuildErrorMessageForMissingLogRegistration` rewritten → 🟢
  - [x] Note: NFluent 2.8's `AndWhichMessage()` lives in the `NFluent.ApiChecks` namespace; `WhichMember(e => e.Message)` is the one available from `NFluent`
- [ ] **Step 6 — Documentation**
  - [ ] Rewrite the xUnit block of `BuildErrorMessageForMissingLogRegistration` (it currently advises the very pattern that causes #11)
  - [ ] `README.md` — split the registration section per framework, document *when* the seed is traced, add a "Running your tests in parallel" subsection
  - [ ] `CLAUDE.md` — the `Fuzzer.Log` save/restore rule for fixtures + the two-seams invariant
- [ ] **Step 7 — Verification & release**
  - [ ] Full suite green on both TFMs, determinism guards untouched
  - [ ] Version bump 1.0.1 → **1.1.0** + fill `<PackageReleaseNotes>`

## Design

### The seam: two insertion points, not one

`Random IFuzz.Random` alone is **not** a complete choke point (verified against all 9 sub-fuzzers):
the whole NoDuplication mode routes through `GenerateWithoutDuplication` →
`generationFunction(SideEffectFreeFuzzerWithDuplicationAllowed)`, i.e. randomness is drawn from a
**different** `Fuzzer` instance, so the outer one's getter is never touched.

Exactly two seams, both in `Fuzzer.cs`, both funnelling into one idempotent method:

1. the `Random IFuzz.Random` getter (`Fuzzer.cs:99`)
2. the first statement of `GenerateWithoutDuplication<T>`

Knowingly out of scope: calls that draw nothing at all (`GenerateInstanceOf<T>()` on an
interface, `GenerateWords(0)`, argument-validation throws) emit no banner — there is no random
sequence to reproduce. Do **not** scatter the call across the 25 public `Generate*` methods: a
future method would forget it.

### New state and constructors (`Diverse/Fuzzer.cs`)

```csharp
private readonly bool _seedWasProvided;          // was a ctor local, now needed at first-use time
private readonly bool _isASilentInternalFuzzer;
private Action<string> _instanceLogger;          // null => fall back to the static Log
private bool _seedHasBeenLogged;

public Fuzzer(int? seed = null, string name = null, bool? noDuplication = false)
    : this(seed, name, noDuplication, instanceLogger: null, isASilentInternalFuzzer: false) { }

private Fuzzer(int? seed, string name, bool? noDuplication,
               Action<string> instanceLogger, bool isASilentInternalFuzzer) { ... }
```

The public signature stays byte-for-byte identical (no binary break). The private ctor takes 5
**required** parameters, so `new Fuzzer()` / `new Fuzzer(42)` stay unambiguous inside the assembly.

> A public ctor overload `Fuzzer(Action<string> logger, int? seed = null, ...)` was rejected: it
> makes `new Fuzzer()`, `new Fuzzer(null)`, `new Fuzzer(seed: 42)` and `new Fuzzer(name: "x")`
> **CS0121 ambiguous** — it would break the compile of every consumer plus ~30 call sites in
> `Diverse.Tests`.
>
> `AsyncLocal<Action<string>>` was also rejected: `ExecutionContext` flows downward only, so a
> value set in NUnit's `[OneTimeSetUp]` (the documented pattern) or MSTest's
> `[AssemblyInitialize]` would be discarded before the tests run — it would silently break every
> currently-green NUnit suite.

### The public opt-in

```csharp
public Fuzzer WithLogger(Action<string> logger)
{
    _instanceLogger = logger ?? throw new ArgumentNullException(nameof(logger));
    return this;
}
```

Returns `Fuzzer` (not `IFuzz`) so `Seed`, `Name`, `MaxFailingAttemptsForNoDuplication` stay
reachable after chaining. Do **not** add it to `IFuzz` — that interface is the user extension
point (CLAUDE.md), adding a member breaks every implementer.

### Emission

```csharp
private void EnsureTheSeedHasBeenLogged()
{
    if (_seedHasBeenLogged || _isASilentInternalFuzzer) { return; }

    var logger = _instanceLogger ?? Log;
    if (logger == null)
    {
        throw new FuzzerException(BuildErrorMessageForMissingLogRegistration());
    }

    _seedHasBeenLogged = true;   // set BEFORE emitting: a throwing sink must not be retried forever
    Emit(logger, BuildSeedAndTestInformationLines());
}
```

`Emit` builds the whole banner first, then `try { foreach (var line in lines) logger(line); }
catch (Exception exception) { ... }` — catching `Exception`, not just `InvalidOperationException`
(sinks are arbitrary user delegates: `ObjectDisposedException`, stale-helper NREs…). The fallback
writes one warning line naming the exception type and message, then the **complete** banner to
`Console.WriteLine` (available on netstandard2.0).

The `FuzzerException` is thrown **outside** the try/catch, and `_seedHasBeenLogged` is not set in
that branch, so a missing registration keeps throwing on every subsequent call — matching today's
"every `new Fuzzer()` throws" behaviour.

**No thread-safety primitive**: `_seedHasBeenLogged` stays a plain `bool`. Sharing one `Fuzzer`
across threads is already unsound (`System.Random` is not thread-safe), CLAUDE.md forbids
introducing concurrency primitives, and the getter is hot (`GeneratePassword` hits it ~10 times
per call). Worst case in a pathological race: the banner prints twice.

### Derived fuzzers

```csharp
private IFuzz SideEffectFreeFuzzerWithDuplicationAllowed
    => _sideEffectFreeFuzzer ?? (_sideEffectFreeFuzzer =
        new Fuzzer(Seed, null, false, _instanceLogger, isASilentInternalFuzzer: true));

public IFuzz GenerateNoDuplicationFuzzer()
    => new Fuzzer(Seed, null, true, _instanceLogger, isASilentInternalFuzzer: false);
```

`SideEffectFreeFuzzerWithDuplicationAllowed` is private, same-seeded, and only ever created
*after* its owner's banner has fired → silencing it loses zero information.
`GenerateNoDuplicationFuzzer()` is **public** and its result can be used alone, so it must keep
speaking (silencing it could produce a whole run with no seed traced at all) — it just becomes
lazy like everything else. Both stay **lazy** and same-seeded; the flag never changes a seed or a
creation time.

## Tests to write first (NUnit + NFluent, no xUnit dependency)

All three new fixtures must save and restore the global `Fuzzer.Log` (set once for the whole
assembly by `Diverse.Tests/AllTestFixtures.cs:11`) via `[SetUp]`/`[TearDown]`, and must **not** be
`[Parallelizable]` — the assembly has no `[assembly: Parallelizable]`, so NUnit's default serial
execution is what protects the static mutation.

Assertion quality: assert the **whole** sequence of lines with `ContainsExactly`, never
`IsNotEmpty`; never interpolate `fuzzer.Seed` into an expectation built from the code under test
(use a literal seed, or a regex for the random-seed case); a matcher on every expected throw.

### `Diverse.Tests/FuzzerLoggingShould.cs`

| # | Test | Assertion | Today |
|---|---|---|---|
| T1 | `Not_log_anything_at_construction_time` | `Check.That(spy.Lines).IsEmpty()` right after `new Fuzzer(seed: 42, name: "fuzzer1")` | 🔴 4 lines logged in the ctor |
| T2 | `Log_the_whole_seed_banner_once_the_first_value_has_been_generated` | `ContainsExactly` the 4 literal lines, incl. `--- from the test: FuzzerLoggingShould.Log_the_whole_...()` | guard |
| T3 | `Log_the_reproduction_hint_when_no_seed_was_provided` | 5 lines; literals for 0/2/3/4, regex `^--- Fuzzer \("fuzzer1"\) instantiated with the seed \(-?\d+\)$` for line 1 | guard |
| T4 | `Log_the_seed_banner_only_once_whatever_the_number_of_generated_values` | 3 successive calls → still the same 4 lines (kills the "log on every `Random` access" mutant) | guard |
| T5 | `Resolve_the_name_of_the_test_even_when_the_Fuzzer_was_built_outside_of_the_test_method` | build the Fuzzer on a `new Thread(...)` + `Join()` (the xUnit-free analogue of a test-class ctor), generate on the test thread → the banner names the real `[Test]` | 🔴 `(not found)` |
| T6 | `Not_log_any_extra_banner_when_using_the_NoDuplication_mode` | `new Fuzzer(seed: 42, name: "fuzzer1", noDuplication: true).GenerateInteger(1, 5)` → exactly fuzzer1's 4 lines | 🔴 8 lines |
| T7 | `Log_the_banner_of_a_derived_NoDuplication_Fuzzer_only_once_it_is_used` | `IsEmpty()` after `GenerateNoDuplicationFuzzer()`, then `HasSize(4)` + seed-42 regex after generating | 🔴 8 lines before any generation |

### `Diverse.Tests/FuzzerWithItsOwnLoggerShould.cs`

| # | Test | Assertion | Today |
|---|---|---|---|
| T8 | `Send_its_seed_banner_to_its_own_logger_instead_of_the_static_one` | `instanceSpy.Lines` = the 4 lines **and** `Check.That(staticSpy.Lines).IsEmpty()` | 🔴 |
| T9 | `Fall_back_on_the_static_Log_when_no_instance_logger_was_provided` | `staticSpy.Lines` = the 4 lines (pins the resolution order, paired with T8) | guard |
| T10 | `Keep_the_logs_of_two_concurrent_Fuzzers_separated_when_each_has_its_own_logger` | `Fuzzer.Log = null`; fuzzers A(seed 1)/B(seed 2), each with its own logger, generating on two `Thread`s → each spy gets exactly its own 4 lines. `[Repeat(20)]` | 🔴 |
| T11 | `Pass_its_own_logger_down_to_the_Fuzzer_it_derives_for_the_NoDuplication_mode` | `Fuzzer.Log = null` → missing propagation means `FuzzerException`, i.e. red for the right reason | 🔴 |
| T12 | `Refuse_a_null_logger` | `Check.ThatCode(() => new Fuzzer().WithLogger(null)).Throws<ArgumentNullException>()` | 🔴 |

### `Diverse.Tests/FuzzerWithAFailingLogSinkShould.cs`

| # | Test | Assertion | Today |
|---|---|---|---|
| T13 | `Not_break_the_test_when_the_registered_log_sink_throws` | `DoesNotThrow()` **and** `CallCount == 1` (proves we really tried the user's sink), still `1` after 3 more generations | 🔴 the `InvalidOperationException` escapes |
| T14 | `Fall_back_on_the_Console_when_the_registered_log_sink_throws` | capture `Console.Out` into a `StringWriter` (restored in a `finally`) → `ContainsExactly` the warning line **plus** the 4 banner lines | 🔴 |
| T15 | `Throw_an_explicit_FuzzerException_only_when_a_value_is_generated_and_no_log_sink_was_registered` | `new Fuzzer(seed: 42)` `DoesNotThrow()`, then `GenerateInteger()` `Throws<FuzzerException>()` with a message mentioning `WithLogger`, `ITestOutputHelper`, `OneTimeSetUp` | 🔴 the ctor throws today |

### Determinism guards — must stay green and **unmodified**

`FuzzerWithItsOwnDeterministicCapabilitiesShould` (the 10 hard-coded integers for seed
`1226354269`, and `33828652`), the whole `NoDuplicationFuzzersShould` fixture, and
`FuzzerShould.Indicate_what_to_do_in_the_DuplicationException_message...` (full-message
assertion). Run them at every step, from the first 🔴 onward.

**Determinism is provably untouched**: `_internalRandom = new Random(seed)` stays at the same
place in the ctor and nothing draws from it during construction (verified: all 9 sub-fuzzer ctors
only store the `IFuzz` reference). `EnsureTheSeedHasBeenLogged()` reads `Seed`, `Name`,
`_seedWasProvided` and walks a `StackTrace` — it never calls `.Next()`. `GenerateFuzzerName()`
keeps its own `new Random()` and stays **eagerly** called in the ctor, in the same position.

## Documentation

- **`Fuzzer.cs` — `BuildErrorMessageForMissingLogRegistration` (lines 214-216).** The current
  xUnit advice `Fuzzer.Log = testOutputHelper.WriteLine;` *is* the cause of #11. Replace it with
  the test-class-constructor shape using `new Fuzzer().WithLogger(output.WriteLine)`. Keep the
  NUnit and MSTest blocks; add one sentence stating that an instance logger wins over the static
  `Fuzzer.Log` and is the safe option under parallel execution.
- **`README.md` (lines 189-210).** Split the registration section into NUnit (unchanged), xUnit
  (`WithLogger`, showing the exact shape from the issue), MSTest. Add "### When is the seed
  traced?" (on the first generated value, once per instance, Console fallback if your sink
  throws) and "### Running your tests in parallel" (linking issue #11). Lines 211-223, the sample
  banner output, stay accurate as-is.
- **`CLAUDE.md`.** Line 103 → note that any fixture mutating `Fuzzer.Log` must save/restore it and
  must not be `[Parallelizable]`. Add to "Key patterns to preserve": the banner is emitted lazily,
  once per instance, through exactly **two** seams (`IFuzz.Random` getter and
  `GenerateWithoutDuplication`) — any new randomness path must go through one of them.

## Verification

```bash
dotnet build Diverse.sln
dotnet test Diverse.Tests/Diverse.Tests.csproj
```

- The 15 new tests green, the determinism guards green and unmodified.
- Build must be clean on **both** TFMs (netstandard2.0 + net8.0) — `Console.WriteLine` and
  everything else used here is available on netstandard2.0.
- Manual cross-check against the issue: the exact repro snippet compiles and runs with the new
  `WithLogger` form; the banner names the `[Fact]`, not `(not found)`.

## Release

`Diverse/Diverse.csproj` lines 19-21 → `1.1.0.0` / `1.1.0.0` / `1.1.0` (additive public API ⇒ not
a patch; nothing removed or changed ⇒ not a major). Fill the empty `<PackageReleaseNotes>` with
the five observable behaviour changes: banner timing moved to first use; `FuzzerException` timing
moved with it; the internal side-effect-free fuzzer no longer emits a banner; a throwing sink no
longer propagates (Console fallback); new per-instance logger.
