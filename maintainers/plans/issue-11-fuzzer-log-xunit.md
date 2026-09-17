# Fix issue #11 — `Fuzzer.Log` static + xUnit `ITestOutputHelper`

## 📍 STATE

- **Where it stands**: PR [#12](https://github.com/tpierrain/Diverse/pull/12) is open, **CI green on
  the fixes** (commit `8ccbeca`), **225 tests passing**, Release clean on both TFMs. The **18 review
  findings of 2026-09-17 are fixed, bar 4** (see below), each fix driven by a test seen red first.
- **Next step**: Thomas reviews and merges. Nothing else is in flight.
- **What was deliberately NOT fixed here**, and why:
  - **F13 and F14 predate this branch** (culture-sensitive seed formatting; out-of-range integers
    under NoDuplication). Thomas's call, 2026-09-17: **their own PR**, not this one.
  - **F8 is half-fixed**: reading `IFuzz.Random` still consumes the one-shot banner, but the banner
    it emits is now correct (the test name is resolved in the constructor). Closing it entirely
    would mean scattering the seam across the 25 `Generate*` methods, which this plan forbids.
  - **F17 stays open by construction**: a wiring check in the constructor would break the very
    pattern the README recommends (`new Fuzzer().WithLogger(...)` hands the logger over *after*
    construction). F1's fix restores the loud failure on every path that draws randomness.
- **This file is the door**: this repo has no `plans/ACTIVE.md`; resume from this STATE block.

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
- [x] **Step 6 — Documentation** _(2026-08-15)_
  - [x] Rewrite the xUnit block of `BuildErrorMessageForMissingLogRegistration` (done in Step 5, driven by T15)
  - [x] `README.md` — registration split per framework (NUnit / xUnit / MSTest), "When is the seed traced?", "Running your tests in parallel"
  - [x] `CLAUDE.md` — the `Fuzzer.Log` save/restore rule for fixtures + the two-seams invariant as pattern #5
- [x] **Step 7 — Verification & release** _(2026-08-15)_
  - [x] Full suite green (216 tests), determinism guards untouched, Release build clean on **both** TFMs (only the 12 pre-existing CS1591 warnings on `FuzzerExtensions`/`MethodCapture` remain)
  - [x] Version bump 1.0.1 → **1.1.0** + `<PackageReleaseNotes>` filled

- [x] **Step 8 — Ship** _(2026-08-15)_
  - [x] Branch pushed: `fix/11-defer-seed-logging-and-per-instance-logger`
  - [x] **PR open: https://github.com/tpierrain/Diverse/pull/12** — CI `build (ubuntu-latest)` **green** (1m01s)
  - [x] Deep review run on 2026-09-17 → **18 findings, merge held** (see below)
  - [x] Triage: Thomas's call on 2026-09-17, *"oui, corrige"* → fix everything but the two
        pre-existing defects (F13, F14), which get their own PR

- [x] **Step 9 — Fix the review findings** _(2026-09-17)_
  - [x] 🔴 **9 tests written first and seen red on their own assertion** (one per defect, plus the
        rewritten concurrency test), then implemented → 🟢 **225 tests green**
  - [x] **Emission mechanics** (F2, F4, F5, F12): a `_parentFuzzer` **reference** instead of a copy
        of its logger, the static `Log` captured at construction, `WithLogger(...)` re-arming the
        trace, and the emission claimed with `Interlocked.CompareExchange`
  - [x] **Banner content** (F6, F9, F10): the test name resolved in the constructor (retried at
        emission when there was none), the wording `--- first used by the test:` for a `Fuzzer`
        built outside any test, and `_seedWasProvided` inherited from the parent
  - [x] **Robustness** (F1, F3, F11): `TypeFuzzer` rethrows `FuzzerException` before its two
        catch-alls, the Console fallback guarded by its own try/catch, and only the **undelivered**
        lines replayed
  - [x] **Documentation** (F7, F16, F18): the error message leads with the two wiring options and
        its MSTest snippet now matches the README; the README says every `Fuzzer` needs its own
        logger under the per-instance wiring, and that **xUnit does not capture the Console**;
        the false C# rationale on the private ctor is gone
  - [x] **Test hygiene** (F15 + cleanup): `Utils/Concurrently.cs` brings back what a thread threw,
        a `LogMutatingFixture` base collapses the triplicated scaffolding, the tautological name
        expectations are pinned by shape, and the concurrency test now races **one** `Fuzzer`
        (`[Repeat(200)]`)
  - [x] Re-verified by probe against the rebuilt assembly: F1 throws, F2/F4/F5 behave, F6 keeps its
        hint, F3 survives a dead Console, F11 stops repeating, F12 **0 duplicated banners in 2000
        runs** (was 32)
  - [ ] Review + merge by Thomas (**the only thing waiting on him**)
  - [ ] Once merged: reply to `Poubone` on issue #11 with the nuance below (xUnit 2.x + parallelism is what actually reproduces), and tag `v1.1.0` to trigger the NuGet release workflow
  - [ ] Its own PR, afterwards: **F13 + F14**, the two pre-existing defects

## Review findings (2026-09-17) — to triage before merge

Reviewed `main...HEAD` (5 commits, 11 files). **F1, F4, F5, F6 and F14 were reproduced locally**
by compiling a console app against the built branch assembly, and **F18 by compiling the variant it
declares impossible** (probes kept out of the repo). F3, F8, F9, F11, F12, F17 were reproduced the
same way by the review; the rest were established by reading the code against the promises made in
`README.md` and `<PackageReleaseNotes>`.

### Regressions this branch introduces

- [x] **F1 — a missing log sink now fails SILENTLY on `GenerateInstanceOf<T>()`.** Moving the
      `FuzzerException` out of the constructor drops it inside `TypeFuzzer`'s pre-existing
      catch-all (`TypeFuzzer.cs:110` and `:222`), so `Fuzzer.Log = null;
      new Fuzzer(42).GenerateInstanceOf<Poco>()` **returns null** instead of telling the user to
      register a sink. Reproduced. On `main` it threw with the full guidance.
- [x] **F2 — the static `Fuzzer.Log` is now resolved at first draw, not at construction.** For
      users staying on the documented static pattern under parallel xUnit, class A's banner is
      emitted with whatever sink class B installed in the meantime: the crash is replaced by seeds
      landing in the wrong test's output.
- [x] **F3 — the `Console` fallback is itself unguarded.** If `Console.Out` is dead too (the same
      runner teardown invalidates both), the exception escapes the `IFuzz.Random` getter and fails
      the user's test, against the release note that promises the opposite.
- [x] **F4 — `WithLogger(...)` after `GenerateNoDuplicationFuzzer()` never reaches the child.** The
      logger is snapshotted at derive time; the child then throws `FuzzerException` asking for a
      sink the user registered one line earlier, and `IFuzz` exposes no `WithLogger` to repair it.
      Reproduced. `README.md:271` and the release notes state the inheritance unconditionally.
- [x] **F5 — `WithLogger(...)` after the first draw is a permanent silent no-op** (the banner has
      already fired) and still returns `this` as if it had worked. Reproduced: static sink 4 lines,
      late instance sink 0. The shape that loses the trace is exactly the per-test
      `_fuzzer.WithLogger(output)` an xUnit user will write for a fixture-scoped Fuzzer.
- [x] **F6 — a derived NoDuplication fuzzer lies about its seed and drops the reproduction hint.**
      Reproduced on a seedless parent: the banner reads *"instantiated from a provided seed"* and
      the *"you can instantiate another Fuzzer with that very same seed"* line is gone — and with
      lazy tracing, a parent that is never drawn from emits nothing at all, so that hint can vanish
      from a whole run. Two tests pin the truncated 4-line shape as expected.
- [ ] **F8 — reading `IFuzz.Random` now has a side effect.** The documented extension seam emits
      the one-shot banner, so a debugger auto-evaluating properties (the default in Visual Studio
      and Rider) destroys the seed trace of the very test being stepped through — and can throw.
      → **Half-fixed, knowingly.** The banner is no longer *wrong* when it fires early (the test
      name comes from the constructor now, so a debugger read still names the right test), but the
      read does still consume it. Closing it completely means either scattering the seam across the
      25 `Generate*` methods (which this plan forbids: a new method would forget it) or handing the
      sub-fuzzers a `Random` through another door, i.e. touching all 9 of them. Left open on
      purpose, with its cost written down rather than discovered again.
- [x] **F9 — the test name is resolved at first draw, so it is lost off the test's own stack.**
      `async` tests after an `await`, `Task.Run`, `Parallel.ForEach` all banner `(not found)()`,
      where `main` resolved them from the constructor's stack. `FuzzerWithItsOwnLoggerShould.cs:94`
      freezes `(not found)` as expected.
- [x] **F10 — a Fuzzer shared by several tests attributes its seed to whichever test drew first**,
      and the others get no seed line at all, against `README.md:276` ("the seed used for every
      test ran").

### Weaknesses of the new code (not regressions)

- [x] **F7 — the docs and the error message now disagree with each other.** The new xUnit section
      wires the logger per instance only, which makes README step 3 (`new Fuzzer(seed: 1248680008)`
      to reproduce a failure) throw; and `BuildErrorMessageForMissingLogRegistration` still opens
      by recommending the static `Log` that causes issue #11, with `WithLogger` buried 25 lines
      below. Its MSTest snippet also contradicts `README.md:243-255`.
- [x] **F11 — a partially failing sink gets its lines replayed.** `Emit` re-prints the whole banner
      to the Console, so already-delivered lines appear twice, in two places, one copy truncated.
      Untested: the stub throws on its first call.
- [x] **F12 — `_seedHasBeenLogged` and `_instanceLogger` need no lock, but do need `volatile`.** The
      double banner is knowingly accepted (measured at 1.6% over 2000 runs); the unconsidered half
      is `_instanceLogger` published without a barrier, which can read null on a thread-pool thread
      and throw at a user who did register a logger. `volatile` costs nothing on the hot path.
- [x] **F16 — the Console fallback is invisible to the very audience it was added for.** xUnit
      deliberately does not capture `Console.Out` into a test's report (Test Explorer shows nothing
      at all), so when an xUnit user's `ITestOutputHelper` throws, the seed is not lost loudly, it
      is lost silently — while `README.md:265` and the release notes promise the opposite.
- [ ] **F13 — the seed is formatted with the ambient culture.** Under `sv-SE`, `fi-FI`, `lt-LT`,
      `et-EE`, a negative seed prints with U+2212 MINUS SIGN and cannot be pasted back into
      `new Fuzzer(seed: ...)`. Fix: `ToString(CultureInfo.InvariantCulture)`. **Pre-existing on
      `main`**, but this diff rewrites those exact lines, so it is in reach here — a scope call,
      like F14. Auto-generated seeds are never negative (`Random.Next()`), so only a user-supplied
      negative seed hits it.
- [x] **F15 — the only concurrency test cannot fail for the race that exists, and can kill the
      host for another.** It uses **two separate** Fuzzers, each with its own sink; since both
      `_instanceLogger` and `_seedHasBeenLogged` are per-instance, the assertion holds by
      construction and no interleaving can make it red. The real hazard (two threads on **one**
      Fuzzer, measured at 1.6%) has no test at all. On top of that it runs generation in raw
      foreground threads with no `try/catch`, so a regression in logger resolution throws on an
      unguarded thread and **kills the test host**, losing the verdicts of all 216 tests instead of
      reporting one red. `[Repeat(20)]` is also the lowest in the suite, against `CLAUDE.md:107`
      which states `[Repeat(200)]` for probabilistic tests.
- [ ] **F17 — a project that never registers a sink can now go entirely undetected.** With
      `Fuzzer.Log = null`, `new Fuzzer(42).GenerateStringFromPattern("FR-2024")` returns its value
      with no banner and no exception: the call reaches neither seam. This is **distinct** from the
      accepted "no banner on draw-free paths" (there, nothing is reproducible anyway): what is lost
      here is the detection of the **wiring mistake itself**, which `main` caught on the very first
      `new Fuzzer()`. Worth considering: validate the sink once at construction *without* emitting.
      → **Cannot be closed without breaking the recommended pattern**, verified: the whole point of
      `new Fuzzer().WithLogger(output.WriteLine)` is that the logger arrives **after** the
      constructor, so a constructor-time check would throw at every xUnit user following the
      README. F1's fix restores the loud failure on every path that draws randomness, which leaves
      only the draw-free calls silent. Deliberate residual.
- [x] **F18 — the private constructor's stated rationale is not a C# rule.** Its XML remark
      (`Fuzzer.cs:141`) claims the 5 required parameters keep it out of the public ctor's overload
      resolution. **Verified false**: the variant with the last two parameters optional compiles
      with 0 errors. The plan repeated the claim and is corrected above. Cost: an awkward signature
      and `null, false` at every internal call site, upheld for nothing.

### Pre-existing, found in a callee of the touched code

- [ ] **F14 — `GenerateInteger(min, max)` returns out-of-range values in NoDuplication mode.**
      `Fuzzer.cs:547` passes `maxValue` where `Enumerable.Range` expects a **count**. Reproduced:
      `GenerateInteger(10, 15)` returned `24, 17, 20`; `GenerateInteger(-5, -1)` threw
      `ArgumentOutOfRangeException` on the 6th draw. The sibling `LastChanceToFindAge` (line 748)
      gets it right with `maxAge - minAge`. Existing coverage misses it because its test uses
      `min = 0`, where count and bound coincide. **Predates this branch** — fixing it here is a
      scope call.

### Cleanup, no behaviour attached

- [x] `CLAUDE.md` contradicted itself: a mandatory `[SetUp]`/`[TearDown]` rule three lines above a
      "no `[SetUp]` fields" one, with the three new fixtures obeying the first and breaking the
      second. → The carve-out is now explicit, and two rules the review paid for were added
      beside it (bring back what a thread threw; never re-read an expectation from the object
      under test).
- [x] `SeparatorLine` duplicated 4 times and the save/restore triplicated. → `LogMutatingFixture`
      in `Diverse.Tests/Utils/`, which the three fixtures now derive from.
- [x] The derived-fuzzer tests built their expected name from the object under test, which made
      that half of the assertion unfailable. → Pinned by shape instead
      (`Matches(@"^--- Fuzzer \(""fuzzer\d+""\)...")`).
- [ ] `Diverse/Diverse/Diverse.xml` is a tracked Release-only build artifact that drifts silently.
      **Left tracked**: untracking it is a repo-wide call for Thomas, not a fix for this PR.

### Deliberately NOT findings

Three candidates were dropped because this plan pre-decides them: no banner on paths that draw no
randomness, the `FuzzerException` re-throwing on every draw, and keeping `WithLogger` off `IFuzz`.
The review also confirmed what the branch gets right: the core fix works (out-of-repo xUnit 2.9.2
repro, 3 of 5 runs failing on 1.0.1, 69/69 green here), the new check in the `Random` getter costs
nothing (NoDuplication is 2.4x faster), and determinism is genuinely unchanged.

## Verification against real xUnit (out-of-repo, scratchpad)

The stub in `Diverse.Tests` only approximates xUnit, so the fix was also checked against the real
thing, in throwaway projects outside this repo.

- [x] **The reported crash reproduces on 1.0.1 — but only with xUnit 2.x and only under
      parallelism.** With `xunit 2.9.2` + `Diverse 1.0.1`, 12 test classes each doing
      `Fuzzer.Log = output.WriteLine; _fuzzer = new Fuzzer();` in their constructor:
      **3 runs out of 5 failed** (0 to 3 failures each) with
      `System.InvalidOperationException : There is no currently active test.` at
      `Diverse.Fuzzer.LogSeedAndTestInformations` — the exact stack trace of the issue.
- [x] **Same repro, 5 runs against this branch: 69/69 green every time.**
- [x] The issue's *minimal* snippet (a single test class) does **not** crash on 1.0.1, and does not
      crash under xUnit v3 either. So the dominant root cause in the wild is **(b) the shared
      mutable static under parallel execution**, more than the constructor timing on its own. Worth
      saying in the issue reply: someone hitting this on xUnit v3 or on a single test class is
      hitting something else.
- [x] The recommended `new Fuzzer().WithLogger(output.WriteLine)` pattern verified green, and the
      banner now names the real `[Fact]`/`[Theory]` (e.g. `--- from the test: SampleTests.Example()`)
      instead of `(not found)`.

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
private readonly bool _seedWasProvided;                 // was a ctor local, now needed at first-use time
private readonly bool _isASilentInternalFuzzer;
private readonly Fuzzer _parentFuzzer;                  // a reference, NOT a copy of its logger (F4)
private readonly Action<string> _staticLoggerAtConstructionTime;   // the static Log as it was (F2)
private readonly string _testNameAtConstructionTime;    // null when no test was on the stack (F9)
private volatile Action<string> _instanceLogger;        // null => walk the resolution order below
private int _seedHasBeenLogged;                         // an int, to be claimed atomically (F12)

public Fuzzer(int? seed = null, string name = null, bool? noDuplication = false)
    : this(seed, name, noDuplication, parentFuzzer: null, isASilentInternalFuzzer: false) { }

private Fuzzer(int? seed, string name, bool? noDuplication,
               Fuzzer parentFuzzer, bool isASilentInternalFuzzer) { ... }
```

The public signature stays byte-for-byte identical (no binary break).

> ⚠️ **Correction (2026-09-17).** This section used to claim that the private ctor's 5 **required**
> parameters are what keep `new Fuzzer()` / `new Fuzzer(42)` unambiguous inside the assembly. That
> is not a C# rule, and the claim is also carried in the XML remark on the private ctor
> (`Fuzzer.cs:141`). **Verified by compiling the "forbidden" variant**: making the last two
> parameters optional produces **0 errors** — the candidate with fewer declared parameters wins the
> tie-break, so the public ctor is selected regardless. The awkward `null, false` spelled out at
> every internal call site satisfies a constraint that does not exist. See F18 below.

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
    if (_seedHasBeenLogged != 0 || _isASilentInternalFuzzer) { return; }

    var logger = ResolveTheLogger();
    if (logger == null)
    {
        throw new FuzzerException(BuildErrorMessageForMissingLogRegistration());
    }

    // Claimed BEFORE emitting, and atomically: a throwing sink must not be retried forever, and
    // threads racing on the first generated value must not each emit a banner.
    if (Interlocked.CompareExchange(ref _seedHasBeenLogged, 1, 0) != 0) { return; }

    Emit(logger, BuildSeedAndTestInformationLines(Seed, _seedWasProvided, Name,
                                                  _testNameAtConstructionTime));
}

private Action<string> ResolveTheLogger()
    => _instanceLogger                       // ours, whenever it is set (even after a first draw)
       ?? _parentFuzzer?.ResolveTheLogger()  // our parent's, however late it was given to it
       ?? _staticLoggerAtConstructionTime    // the static one AS IT WAS when we were built
       ?? Log;                               // and only then, the current static one
```

**Late for what can still change, early for what a parallel test can steal.** Reading the static
`Log` at emission time is what let a test overwriting it receive the seed of another test's
`Fuzzer` (F2); keeping a *reference* to the parent instead of a copy of its logger is what makes a
`WithLogger(...)` call landing after the derivation still reach the child (F4).

`Emit` builds the whole banner first, then `try { foreach (var line in lines) { logger(line);
delivered++; } } catch (Exception exception) { FallBackOnTheConsole(exception, lines, delivered); }`
— catching `Exception`, not just `InvalidOperationException` (sinks are arbitrary user delegates:
`ObjectDisposedException`, stale-helper NREs…). The fallback writes one warning line naming the
exception type and message, then **only the lines the sink did not take** (F11), and is **itself
wrapped in a try/catch that swallows** (F3): the very teardown that kills a test output helper is
what closes the writer the runner redirected the Console to, and tracing a seed is never a reason
to fail an end-user's test.

The `FuzzerException` is thrown **outside** the try/catch, and `_seedHasBeenLogged` is not set in
that branch, so a missing registration keeps throwing on every subsequent call — matching today's
"every `new Fuzzer()` throws" behaviour. ⚠️ And it must never be swallowed by a catch-all on a
generation path: `TypeFuzzer` rethrows it explicitly before its two `catch (Exception)`, or
`GenerateInstanceOf<T>()` hands back a silent `null` instead (F1).

**One atomic claim, no lock.** `_seedHasBeenLogged` is an `int` claimed with
`Interlocked.CompareExchange`: the plain `bool` check-then-set duplicated the banner in **1.6% of
2000 runs** with 8 threads racing (measured), which is not the "pathological" case this section
used to assume. The cost is one interlocked operation on the **first** draw only — every later call
returns on a plain read. `_instanceLogger` is `volatile` for the same reason: written by
`WithLogger(...)` on one thread, read on another. Sharing one `Fuzzer` across threads stays unsound
for *generation* (`System.Random` is not thread-safe); that is no reason for the **tracing** to
misbehave on top.

### Derived fuzzers

```csharp
private IFuzz SideEffectFreeFuzzerWithDuplicationAllowed
    => _sideEffectFreeFuzzer ?? (_sideEffectFreeFuzzer =
        new Fuzzer(Seed, null, false, this, isASilentInternalFuzzer: true));

public IFuzz GenerateNoDuplicationFuzzer()
    => new Fuzzer(Seed, null, true, this, isASilentInternalFuzzer: false);
```

A derived `Fuzzer` receives `this` rather than a snapshot of its logger, and inherits
`_seedWasProvided` from it: whether a **human** provided the seed is the parent's fact, so a child
of a seedless parent must not claim a provided seed, nor lose the line saying how to reproduce the
run (F6) — all the more so since a parent that is never drawn from now traces nothing at all.

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
