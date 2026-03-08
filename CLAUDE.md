# Diverse -- Fuzzer pico library for .NET

## Project overview

Diverse is a **zero-dependency .NET fuzzing library** (netstandard2.0) that generates random but contextually coherent test data. It is **not** a business application -- it is a library consumed via NuGet by test projects.

Key value propositions:
- Random but **deterministic** (seed-based reproducibility)
- **Diverse** data (names, addresses from multiple cultures/continents)
- **Extensible** via extension methods on `IFuzz`
- **NoDuplication** mode with memoization

## Architecture

### Core: Composition + Delegation

```
Fuzzer (orchestrator)
├── holds single Random instance (seeded)
├── composes 9 internal sub-fuzzers:
│   NumberFuzzer, StringFuzzer, LoremFuzzer, PersonFuzzer,
│   AddressFuzzer, DateTimeFuzzer, TypeFuzzer, GuidFuzzer, CollectionFuzzer
├── each sub-fuzzer receives IFuzz parent (accesses shared Random)
└── NoDuplication logic lives in Fuzzer, not in sub-fuzzers
```

### Key interfaces

- `IFuzz` = composite interface inheriting 9 sub-interfaces (`IFuzzNumbers`, `IFuzzPersons`, `IFuzzStrings`, etc.)
- `IFuzz.Random` is **explicit interface implementation** -- hidden from casual users, exposed for extension methods
- Extension point for users: write extension methods on `IFuzz`

### NoDuplication mechanism

1. `MemoizerKey` = `(MethodBase, argumentsHashCode)` captured via `MethodCapture` + `ArgumentHasher`
2. Try standard generation up to `MaxFailingAttemptsForNoDuplication` times (default: 100)
3. If exhausted, call domain-specific `lastChanceGenerationFunction` (enumerates remaining candidates)
4. Uses a separate `SideEffectFreeFuzzerWithDuplicationAllowed` to avoid StackOverflow in fallback lambdas
5. Throws `DuplicationException` if truly exhausted

## Build & Test

```bash
# Build
dotnet build Diverse.sln

# Run tests
dotnet test Diverse.Tests/Diverse.Tests.csproj
```

- Library: `Diverse/Diverse.csproj` -- multi-targets **netstandard2.0** + **net8.0**
- Tests: `Diverse.Tests/Diverse.Tests.csproj` -- targets **net8.0**
- Test framework: **NUnit** + **NFluent** (assertions)
- Test framework detection: `IsATestMethod()` recognizes **NUnit**, **xUnit**, and **MSTest** attributes (zero-dependency, string-based matching)
- No external runtime dependencies in the library itself

## Coding conventions

### General style

- C# with XML doc comments on all public API members
- `#region` blocks to group related methods in `Fuzzer.cs` (by domain: NumberFuzzer, PersonFuzzer, etc.)
- Internal sub-fuzzers are **not public** -- only `Fuzzer` and `IFuzz*` interfaces are the public API
- `sealed` is NOT systematically used on classes in this codebase
- No primary constructors (codebase predates C# 12)

### Naming

- Test classes: `{Subject}Should` (e.g., `NumberFuzzerShould`, `PersonFuzzerShould`)
- Test methods: `Snake_case_descriptive` (e.g., `Be_able_to_provide_always_different_values_of_integers`)
- Fuzzer methods: `Generate{Thing}` or `Pick{Thing}From` (e.g., `GenerateInteger`, `PickOneFrom`)
- Sub-fuzzer implementations: `{Domain}Fuzzer` (e.g., `NumberFuzzer`, `PersonFuzzer`)

### Key patterns to preserve

1. **Every `Generate*` method on `Fuzzer.cs`** follows the same structure:
   - If `NoDuplication` -> call `GenerateWithoutDuplication(...)` with method capture + argument hash + standard function + optional last-chance function
   - Else -> delegate to sub-fuzzer
2. **Sub-fuzzers** always receive `IFuzz` (not `Fuzzer`) and use `_fuzzer.Random` for randomness
3. **Data diversity**: person generation uses continent-aware name pools (first name origin determines last name pool)
4. **Determinism contract**: never use `new Random()` for data generation inside sub-fuzzers -- always use the shared `IFuzz.Random`

### CRITICAL: Static data is immutable (determinism guarantee)

All static data arrays/dictionaries used for generation (`LastNames._perContinent`, `Male.ContextualizedFirstNames`, `Female.ContextualizedFirstNames`, `Latin.Words`, `Adjectives.PerFeeling`, `Tech.EmailDomainNames`, street/city data in `Geography/`, etc.) **must NEVER be modified** -- no reordering, no renaming, no removing, no inserting entries in the middle.

**Why**: Users who fix a seed in their tests (e.g., `new Fuzzer(seed: 1248680008)`) rely on the exact same sequence of generated values across library upgrades. The `Random` instance indexed into these arrays produces deterministic results only if the arrays remain identical. Changing the content or order of any static data array is a **breaking change** for every consumer test that uses a fixed seed.

**Only safe operation**: appending new entries at the end of an existing array (existing indices remain stable).

### What NOT to do

- Do NOT modify static data arrays/dictionaries (see above -- breaks determinism for all consumers)
- Do NOT add external dependencies (this is a zero-dependency pico library)
- Do NOT make sub-fuzzer implementations public
- Do NOT use `new Random()` for data generation (breaks determinism). The only exceptions are `Fuzzer` constructor (seed generation) and `GenerateFuzzerName()` (cosmetic)
- Do NOT change the `IFuzz` interface signature lightly -- it's the extension point for all users
- Do NOT use MediatR, DI containers, or application-level patterns -- this is a library, not an app
- Do NOT introduce async -- fuzzing is CPU-bound and synchronous by design

## Test conventions

- Setup: `AllTestFixtures.cs` registers `Fuzzer.Log = TestContext.WriteLine` (mandatory)
- Tests must be **fast** (sub-millisecond to ~400ms max)
- Probabilistic tests use `[Repeat(200)]` to catch rare failures
- Assertions use **NFluent** (`Check.That(x).IsEqualTo(y)`)
- Each test creates its own `Fuzzer` instance (no shared state, no `[SetUp]` fields)

## File organization

| Directory | Contents |
|-----------|----------|
| `Diverse/` | Library source code |
| `Diverse/Numbers/` | `NumberFuzzer`, `IFuzzNumbers`, `NumberExtensions` |
| `Diverse/Strings/` | `StringFuzzer`, `LoremFuzzer`, `Adjectives`, `Latin`, `Feeling` |
| `Diverse/Persons/` | `PersonFuzzer`, `Person`, `Gender`, `Male`, `Female`, `LastNames`, `Locations` |
| `Diverse/Address/` | `AddressFuzzer`, `Address`, `Country` |
| `Diverse/Address/Geography/` | `GeographyExpert`, country-specific data classes |
| `Diverse/DateTimes/` | `DateTimeFuzzer` |
| `Diverse/Types/` | `TypeFuzzer`, `ReflectionExtensions` |
| `Diverse/Guid/` | `GuidFuzzer` |
| `Diverse/Collections/` | `CollectionFuzzer` |
| `Diverse/Helpers/` | `Maybe<T>`, `Memoizer`, `MemoizerKey`, `NumberExtensions` |
| `Diverse.Tests/` | NUnit test project |
