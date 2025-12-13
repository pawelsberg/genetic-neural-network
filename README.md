# Genetic Neural Network

Evolve neural networks using genetic algorithms to solve pattern recognition tasks.

## What is this?

This CLI application uses genetic algorithms to evolve neural network structure and weights. 
You define test cases (inputs → expected outputs), and the program evolves networks to match them.

## Quick Start

1. Run the application
2. Initialize sample data: `>init`
3. Load a test case: `>loadtcl isit5`
4. Start evolution: `>start`
5. Check progress: `>show`

## How it Works

1. **Define the problem** — Create test cases with inputs and expected outputs
2. **Evolve solutions** — Genetic algorithm mutates neural networks over generations
3. **Measure quality** — Networks are scored on how well they match expected outputs
4. **Select the best** — Higher quality specimens survive and produce offspring

## Tutorial: "Is it 5?" Network

Train a network to output `1` when input is `5`, otherwise `0`.

| Input | Expected Output |
|-------|-----------------|
| -1    | 0               |
| 0     | 0               |
| 4     | 0               |
| 5     | 1               |
| 6     | 0               |
| 100   | 0               |

### Step 1: Initialize and Load
```
>init
>loadtcl isit5
```

### Step 2: Run Simulation
```
>start
```

### Step 3: Monitor Progress
```
>show
```
This displays the best network found so far, its quality score, and generation count.

Use `>show test` to see how the network performs on each test case. Wait until `Output` matches `ExpOut` for all cases.

### Step 4: Save Your Network
```
>pause
>save isit5_solved
```

Later, reload with `>load isit5_solved`.

## Creating Custom Test Cases

Create XML files in `%appdata%\Pawelsberg.GeneticNeuralNetwork\TestCases` with the naming pattern `name.TestCases.XML`:

```xml
<TestCaseList>
  <TestCase>
    <Input>5</Input>
    <Output>1</Output>
  </TestCase>
  <TestCase>
    <Input>0</Input>
    <Output>0</Output>
  </TestCase>
</TestCaseList>
```

Then load with `>loadtcl name`.

## Documentation

- [COMMANDS.md](COMMANDS.md) — Full command reference
- [GLOSSARY.md](GLOSSARY.md) — Terminology (specimen, mutator, quality, etc.)

## Data Location

All data is stored in `%appdata%\Pawelsberg.GeneticNeuralNetwork`

