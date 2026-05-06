# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Test

The solution is .NET 6, three projects under `Pawelsberg.GeneticNeuralNetwork/`:

```bash
# Build everything
dotnet build Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetwork.sln

# Run all tests (xUnit)
dotnet test Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetwork.sln

# Run a single test class
dotnet test Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetwork.sln --filter "FullyQualifiedName~TestCasesContainerQualityMeterTests"

# Run one specific test method
dotnet test Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetwork.sln --filter "FullyQualifiedName=Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests.ChildrenTests.SomeTestName"

# Run the console app
dotnet run --project Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetworkConsole
```

User data (test cases, networks, mutator/quality-meter configs, saved specimens) lives in `%appdata%\Pawelsberg.GeneticNeuralNetwork\`. The first-time user must run `>init` inside the console app to extract embedded sample data from `Pawelsberg.GeneticNeuralNetworkConsole/Resources/InitialData/` (XML test cases, .Network.txt, .Mutators.txt, .QualityMeters.txt) into AppData.

## Architecture

Three projects:
- `Pawelsberg.GeneticNeuralNetwork` — library, no entry point.
- `Pawelsberg.GeneticNeuralNetworkConsole` — console app (`Program.cs` → `MainMenu` → `Commands` registry).
- `Pawelsberg.GeneticNeuralNetwork.Tests` — xUnit tests.

### Layering inside the library `Model/` (strict — do not cross)

The Model directory layers domains so each can be reused without dragging the others in. Do not add references that violate this layering.

```
Model/
  Genetics/                            # Generic GA framework, specimen-agnostic
                                       # (ISpecimen, Mutator, Mutators, QualityMeter,
                                       # QualityMeasurement, Generation, Simulation, ParentQueuer)
  NeuralNetworking/                    # Network/Node/Neuron/Bias/Synapse/ActivationFunction
                                       # Knows nothing about genetics or tests
  UnitTesting/                         # TestCase, TestCaseList — domain only
  NeuralNetworkingGenetics/            # GA applied to networks, NO knowledge of test cases
                                       # (network mutators, structural quality meters)
  NeuralNetworkingUnitTesting/         # Network ↔ test cases, NO genetics
                                       # (NetworkExtension running test cases, BackPropagating/)
  NeuralNetworkingGeneticsUnitTesting/ # The full stack: GA + networks + test cases
                                       # (NetworkSimulation, test-case-aware mutators &
                                       # quality meters, mutator/QM disk persistence)
```

`NetworkSimulation` (in `NeuralNetworkingGeneticsUnitTesting/`) is the central orchestrator the console wires up. It owns max-nodes/synapses/propagations limits, the active `TestCaseList`, the `ParentQueuerType`, the `NetworkMutators`, and a `QualityMeterFactory`. Setters fan out to keep mutators and the generation meter in sync — when changing how a property updates downstream state, look at `UpdateMutators`, `UpdateGenerationMeterPropagations`, and `UpdateGenerationMeterTestCaseList` together.

`Simulation<TSpecimen>` runs on a `SimulationTimer` tick: each tick locks parameters, drains a queue of pending specimens, advances `GenerationMultiplier` generations, and updates `BestEver` when generation quality OR (tiebreak) `SimulationMeter` quality improves. Generation meter measures fitness for selection; simulation meter is the secondary tiebreaker preferring smaller networks (fewer nodes/synapses).

### Quality meters — the tree and the registry

Quality meters are composed in a tree (`QualityMeter<Network>` with `Children`). Disk format is a custom text format under `Resources/InitialData/QualityMeters/*.QualityMeters.txt`. To add a new meter type:

1. Add the class (in `NeuralNetworkingGenetics/QualityMeasuring/` if structural, or `NeuralNetworkingGeneticsUnitTesting/QualityMeasuring/` if it consumes test cases). Each meter declares its own `TextName` and static `Parse`.
2. Register it in `QualityMeterTypeRegistry` ([QualityMeterTypeDescriptor.cs](Pawelsberg.GeneticNeuralNetwork/Pawelsberg.GeneticNeuralNetwork/Model/NeuralNetworkingGeneticsUnitTesting/QualityMeasuring/DiskStoring/QualityMeterTypeDescriptor.cs)) with the right `MeterDataSource` flags (`PerTestCase`, `FromTestCases`, `FromNetwork`). Container meters (multi-line parse) go in `_containerDescriptors`. The registry validates that meters appear in the section their data source allows.

Mutators follow a parallel pattern under `NeuralNetworkingGeneticsUnitTesting/Mutating/` with their own disk format under `Resources/InitialData/Mutators/`.

### Console layer

`MainMenu` reads input (`CommandInput` handles tab completion + history) and dispatches to a `Command` looked up in the `Commands` static registry. Adding a console command = new `Command` subclass + entry in `Commands.NameCommands`. Adding a settable simulation parameter = entry in `Variables` (used by the `set` command).

## Conventions

A detailed style guide lives in [.github/copilot-instructions.md](.github/copilot-instructions.md). Critical rules that bite when ignored:

- **Don't duplicate data or maintain different representations of the same data** — prefer on-the-fly calculation.
- **No `var`** — explicit types everywhere.
- **No `ref`/`out`** parameters, no `region`s, no curly braces around single-line `if`/`else`/`for`/`while`.
- **Never re-assign parameters or variables** — introduce new ones instead (loop counters excluded).
- **Null over magic values** — no `""` or sentinel ints to mean "absent"; use nullable types.
- **Don't change code to make it testable.** Tests adapt to the code, not the other way around.
- **Solution-wide consistency** — when you change a concept (rename, signature, contract), find and update every call site rather than leaving the codebase in two styles.
- **When the codebase already has a name for the concept you're extending, use it.
- **Disambiguate when a name could mean two things.
- **Names are verbose and built from concept fragments.
- **Mark methods and classes static when they don't touch instance state.

Tests:
- One test class per tested method.
- Folder layout mirrors the FQTN of the tested class with `Pawelsberg.GeneticNeuralNetwork.Model` stripped and `Tests` appended at the leaf. E.g. tests for `Model/.../QualityMeasuring/TestCasesContainerQualityMeter.Children` live in `Tests/QualityMeasuring/TestCasesContainerQualityMeterTests/ChildrenTests.cs`.

Nullable-warning suppressions are configured in [.editorconfig](Pawelsberg.GeneticNeuralNetwork/.editorconfig) — CS8600/8602/8604/8605/8618/8622/8625 are downgraded to suggestions. Don't promote them back without solution-wide review.