# STYLE.md — The Spirit and Style of This Codebase

This document captures how the original developer writes code, so that AI (or any contributor)
can produce changes indistinguishable from the original. [CLAUDE.md](CLAUDE.md) and
[.github/copilot-instructions.md](.github/copilot-instructions.md) state the rules; this file
explains the spirit behind them and the idioms the rules don't spell out. When in doubt, open a
neighbouring file and imitate it — the codebase is small and consistent enough that imitation works.

## 1. The spirit

- **This is a long-lived personal research project, not a product.** Clarity and conceptual purity
  win over cleverness, performance micro-optimisation, and framework fashion. The code reads like a
  textbook implementation of its domain (Code Complete is the explicit reference).
- **The domain model is the architecture.** Everything meaningful lives under `Model/`. There is no
  DI container, no mocking framework, no NuGet dependency beyond xUnit. Plain C# objects wired by
  hand in constructors and static registries. Do not introduce frameworks, packages, or
  "infrastructure" — the absence of them is deliberate.
- **Domains stay pure; integration lives in dedicated layers.** `NeuralNetworking/` knows nothing of
  genetics or test cases; `Genetics/` is specimen-agnostic; combinations get their own folder
  (`NeuralNetworkingGeneticsUnitTesting/` is literally "all three combined"). A change that makes a
  lower layer reference a higher concept is wrong even if it compiles and is convenient.
- **Compute, don't cache.** State that can be derived is derived on access
  (`TestCasesContainerQualityMeter.Children` builds child meters on every get; `SynapseCount()`
  counts on the fly). Never store a second representation of data that already exists — the sync
  bugs are considered worse than the CPU cost.
- **Null means "absent" and is meaningful.** `Children == null` means "meter not configured yet",
  `int?`/`double?` mean "no value". No `""`, no `-1`, no `0` sentinels.
- **Self-explanatory code over comments.** If a comment would explain *what*, rename something
  instead. Comments that exist are short lowercase phase markers or genuine contract notes (see §5).

## 2. Naming — compound names built from concept fragments

Names are long, verbose, and assembled from the project's fixed vocabulary
([GLOSSARY.md](GLOSSARY.md)): *Network, Node, Neuron, Bias, Synapse, Specimen, Mutator, QualityMeter,
QualityMeasurement, TestCase, TestCaseList, Generation, Simulation, ParentQueuer, Propagation*.
Reuse these fragments; never invent a synonym for an existing concept ("weight" is `Multiplier`,
"fitness" is `Quality`).

Patterns to follow exactly:

| Kind | Pattern | Examples |
|---|---|---|
| Network mutator | `<Thing><Action-er>NetworkMutator` | `NeuronAdderNetworkMutator`, `SynapseReconnectorNetworkMutator`, `NothingDoerMutator` |
| Quality meter | `<Scope><Aspect>NetworkQualityMeter` | `TestCaseDifferenceNetworkQualityMeter`, `TotalNodesNetworkQualityMeter` |
| Extension class | `<TargetClass><Aspect?>Extension` (singular) | `NetworkExtension`, `NetworkTextExtension`, `BiasExtension` |
| Marker/capability interface | `I<Capability><Role>` | `IMaxNodesLimitedMutator`, `INetworkQualityMeterWithPropagations`, `IPerTestCaseNetworkQualityMeter` |
| Static registry | plural noun | `Commands`, `Variables`, `Mutators`, `NetworkQualityMeters`, `QualityMeterTypeRegistry` |
| Console command | `<Verb><Noun?>Command` | `LoadQualityMetersCommand`, `SaveMutatorsCommand` |
| Event | past-tense + classic pattern | `NextGenerationCreated`, `NextGenerationCreatedEventArgs<T>`, handler `OnNextGenerationCreated` |

Classes are nouns, methods are verbs. Local variables are spelled out too: `qualityMeasurement`,
`resultSynapse`, `nextGenerationCreatedEventArgs` — never `qm`, `rs`, `args` (abbreviation appears
only in cramped lambdas: `nQ`, `varble`).

**Existing misspellings are established names.** `AvrageQuality`, `WiteChars`,
`TestCaseSubstractErrorIfGoodNetworkQualityMeter` are known and intentional-by-now. Do not fix one
in passing; renaming is its own solution-wide change (code, tests, disk formats, docs) done only
when asked.

## 3. Layering idioms

- **Cross-domain behaviour attaches via static extension classes in the higher layer.** The
  `Network` class has no idea it can run test cases; `NeuralNetworkingUnitTesting/NetworkExtension`
  adds that. Disk persistence for networks lives in `NeuralNetworking/DiskStoring/NetworkExtension`.
  When you need `A` to do something with `B` from another domain, write `static class AExtension`
  in the folder that is allowed to know both — never add the method to `A` itself.
- **A folder = a domain aspect.** Sub-aspects get subfolders (`Mutating/`, `QualityMeasuring/`,
  `QueuingParents/`, `Simulating/`, `DiskStoring/`, `BackPropagating/`). New files go where their
  knowledge requirements permit, using the most restrictive folder possible.
- **One public class per file, file named after the class.** Small companion records may share a
  file (`QualityMeterTypeDescriptor.cs` holds two records and the registry — they form one concept).

## 4. C# dialect

The mechanical rules are in CLAUDE.md (no `var`, no `ref`/`out`, no regions, no reassignment, no
braces on single-line blocks). Beyond those:

- **File-scoped namespaces** (`namespace X;`), .NET 6, nullable annotations used where they carry
  meaning (`List<QualityMeter<Network>>?`), CS86xx warnings are downgraded — don't add `!` or null
  checks just to silence tooling.
- **Explicit types even when painful.** Tests spell out
  `List<QualityMeter<Network>>` in full rather than use `var`. Target-typed `new()` appears only
  rarely in newest code for private field initialisers; explicit `new List<X>()` is the default.
- **Property style:** classic bodies dominate — `get { return _x; }` — with backing fields prefixed
  `_camelCase`. Expression bodies (`=>`) are acceptable only for true one-liners
  (`public string ToText() => TextName;`). Setters that must keep other state in sync call explicit
  `Update...`/`Rebuild...` private methods (see `NetworkSimulation`).
- **Literals:** doubles carry the `d` suffix (`0d`, `1d`, `-1d`).
- **Single-statement loops go on one line** when trivial:
  `while (Index < Text.Length && WiteChars.Contains(Text[Index])) Index++;`
- **Strings:** new code uses interpolation (`$"Expected '(' (character number {Index})"`); older
  `string.Format` survives untouched. Anything written to or parsed from disk uses
  `CultureInfo.InvariantCulture` explicitly.
- **LINQ in moderation:** fine for querying (`Select`, `Where`, `FirstOrDefault`, `Any`,
  `Distinct`), but stateful algorithms are written as plain `foreach`/`for` loops with named locals
  (see `Mutators.Mutate`, `Generation.CreateNextGeneration`).
- **Exceptions:** `FormatException` with position info for text-parsing errors;
  `InvalidOperationException` with an instructive message for misconfiguration or format violations
  ("Format requires explicit sections."); plain `Exception` is tolerated in console commands where
  the message is user-facing. No custom exception types.
- **Events:** classic .NET pattern — `event EventHandler<TEventArgs>`, an `EventArgs` subclass
  carrying data, raised via a private `On...` method. No `Action` events, no reactive libraries.
- **Threading:** one coarse `object _parametersLock`. Setters lock and mutate; the timer tick takes
  a snapshot of every parameter inside one `lock`, then does all work outside it. Follow that exact
  shape when touching `Simulation`.
- **Records** are reserved for small immutable descriptors (`QualityMeterTypeDescriptor`); domain
  objects are mutable classes.

## 5. Comments

Three kinds exist; write nothing else:

1. **Lowercase phase markers** inside longer methods, marking algorithm steps:
   `// measure quality of specimens`, `// add inputs`, `// two children for each good network - if there is space left`.
2. **XML `<summary>`** on members whose *contract* is not obvious from the signature — caller
   preconditions, what null means, when to call it
   (`CodedText.TryReadName`, `Simulation.RecalculateMaxPossibleQuality`). Most members have none;
   don't add doc comments to self-explanatory code.
3. **Rare warnings about ordering/pitfalls:**
   `// TestCasesSequential must be checked before TestCases since it starts with "TestCases"`.

`// TODO - ...` is used for acknowledged design debt (`// TODO - separate children policy`). Never
write comments that narrate the next line, justify a change, or reference the change process.

## 6. The serialization pattern (meters, mutators, networks)

Everything persisted to disk uses the same hand-rolled text format machinery. When adding a
serializable type, follow this checklist — it is the most codified ritual in the repo:

1. `public static string TextName = "ShortName";` (or `const`) — the disk keyword.
2. Instance `public string ToText()` producing `TextName(param1,param2)` with
   `CultureInfo.InvariantCulture`; parameterless types return just `TextName`.
3. `public static <Type> Parse(string parameters, QualityMeter<Network> parent)` (shape varies by
   family) using `CodedText.SplitParams` / `CodedText` reads. Parsing is strict — malformed input
   throws with a message saying what was expected.
4. Implement the family's marker interfaces (`INetworkMutatorTextConvertible`,
   `INetworkQualityMeterTextConvertible`, data-source markers like `IPerTestCaseNetworkQualityMeter`).
5. Register in the family registry (`QualityMeterTypeRegistry`, mutators' equivalent) — registries
   are static classes with static-constructor-built lists plus `ByTextName` dictionaries.
6. Add/extend sample data under `Resources/InitialData/` if users should see it after `>init`.

All parsing goes through `CodedText` (index-based scanner: `TrySkip`/`Skip`/`ReadInt`/
`ReadParenthesesContent`/`CurrentIndent`/`AdvanceLine`). Indentation is 2 spaces per level and is
structural in multi-line formats. Never reach for XML/JSON serializers for these formats — XML is
used only for the pre-existing test-case files.

## 7. Tests

- **xUnit only, no mocks, no fakes.** Tests build real object graphs; shared setup lives in a
  `TestHelper` static class *inside the test folder that needs it*, exposing `Create...` factory
  methods. Production code is never modified to accommodate a test ("Don't change code to make it
  testable").
- **Structure is law:** one test class per tested member, named `<Member>Tests`
  (`ToTextTests`, `ChildrenTests`, `ConstructorTests`); folder mirrors the tested class's namespace
  with `Pawelsberg.GeneticNeuralNetwork.Model` stripped and `Tests` appended at the leaf.
- **Interface conformance gets its own test class:** `Implements<InterfaceName>Tests` asserting
  `Assert.IsAssignableFrom<I...>(instance)`.
- **Fact names state observable behaviour** in PascalCase: `FormatsCorrectly`,
  `ReturnsNullWhenNotConfigured`, `UsesInvariantCulture`; a leading condition joins with an
  underscore: `WithPropagationRange_CreatesMetersForEachPropagation`.
- **Arrange/act/assert separated by blank lines, never by comments.** Culture-sensitive tests set
  `CultureInfo.CurrentCulture` in a `try/finally` that restores the original.
- A new serializable type customarily gets at least `TextNameTests`, `ToTextTests`, and
  `ConstructorTests` (round-tripping the format), mirroring the existing meter test folders.

## 8. Console app

- New command = subclass of `Command` with `public static string Name = "cmdname";` (lowercase),
  overriding `LoadParameters(CodedText)`, `Run(NetworkSimulation)`, `ShortDescription`, and
  `GetParameterCompletions` when tab completion makes sense; then register in
  `Commands.NameCommands`. Parameter errors throw plain `Exception` with a
  "<Command> command syntax exception - ..." style message.
- New tunable simulation parameter = a `Variable` entry in `Variables.List` (camelCase console
  name, `Getter`/`Setter`/`Parse` lambdas) so `set` picks it up automatically.
- User-visible behaviour changes belong in [COMMANDS.md](COMMANDS.md); new domain concepts belong
  in [GLOSSARY.md](GLOSSARY.md). Keep both in step with the code.

## 9. How to make a change here

1. **Find the existing name for the concept and extend it**; grep before inventing anything.
2. **Change solution-wide.** A rename, signature change, or contract change updates every call
   site, every test, every disk format and every doc in the same change — the codebase is never
   left in two styles.
3. **Respect the layering before convenience.** If the "easy" fix needs a forbidden reference, the
   fix is in the wrong layer.
4. **Prefer deleting to deprecating.** Unused members are removed outright (see recent commits:
   "Remove CodedText.GetIndentLevel - used only for tests"), not kept "just in case".
5. **Small, focused commits** with plain imperative one-line messages describing the change, e.g.
   "Refactoring of quality meters", "Use TryReadName instead of static ExtractTextName in CodedText".
6. **When your new code looks different from the file around it, your new code is wrong.**
