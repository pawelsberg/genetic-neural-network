## General Guidelines
Main idea: do not repeat yourself - unless it means making it much more complicated.
Code should be self-explanatory; prefer clear naming and structure over comments.
Follow ideas of Code Complete book by Steve McConnell where applicable.
### C# Coding Conventions
Don't use "var" - specify explicit types for all variable declarations.
Don't use ref nor out parameters.
Do not use regions.
Do not use magic int values instead of using nullable ints representing lack of value.
Do not use magic string value ("" for example) instead of using null to represent lack of value.
Don't use curly braces for single-line if/else/for/while blocks.
Reduce dependency between classes as much as possible.



## Pawelsberg.GeneticNeuralNetwork.Tests
Use a test class per tested method.
Folder structure should resemble FQTN of tested class with removed Pawelsberg.GeneticNeuralNetwork.Model part and added Tests suffix for the deepest folder - equvalent to tested class.

## Pawelsberg.GeneticNeuralNetwork library
Structure:
```
     Model/
        files directly there                     # Code not related to genetics, neural networking, or unit testing
        ├── Genetics/                            # Generic genetic algorithm framework - specimen-agnostic
        ├── NeuralNetworking/                    # Neural network implementation - without genetics
        ├── NeuralNetworkingGenetics/            # Neural network code specific to genetics - no knowledge of test cases
        ├── UnitTesting/                         # Test case domain model - without neural network, genetics
        ├── NeuralNetworkingUnitTesting/         # Network + test case integration
        └── NeuralNetworkingGeneticsUnitTesting/ # Network + genetics + test case integration
```

### Key Classes in Model
`RandomGenerator`
 Utility class for random number generation. Supports seeding for reproducible results.
`Log`
 Thread-safe queue of messages.
`DataDirectory`
 Represents a directory for application data. 
`CodedText`
 Represents text that can be parsed.

### Key Classes/Interfaces in Genetics
#### In Pawelsberg.GeneticNeuralNetwork.Model.Genetics
`ISpecimen`
 Interface that needs to be implemented by a specimen to be used in the genetic algorithm.
`Mutator`
 Base class for a mutator that modifies a specimen. It uses `RandomGenerator` when needed.
`Mutators`
 A list of mutators that can be used by the genetic algorithm. Each mutator has a weight that influences its selection probability.
`MutationDescription`
 Describes a mutation that happened during mutation.
`QualityMeter`
 Base class for a meter - meter is measuring quality of a specimen.
`QualityMeasurement`
 Result of quality measurement by a `QualityMeter`. 
`Generation`
 Represents a generation of specimens. Generates a new generation.
#### In Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating
There are defined here some generic mutators:
`MultipleTimesMutator`
 A mutator that applies another mutator multiple times.
`NothingDoerMutator`
 A mutator that does nothing.
`RandomNumberOfTimesMutator`
 A mutator that applies another mutator a random number of times within specified bounds.
#### In Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents
There are defined here parent queuers that select parents for reproduction:
`ParentQueuer`
 Base class for queuing parents for selection in the genetic algorithm.
#### In Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating
`Simulation`
 Represents a sequence of generations evolving specimens.

### Key Classes/Enums in NeuralNetworking
`Node`
 Base class for a neural network node.
`Neuron`
 Is a `Node` that represents a neuron in the neural network.
`Bias`
 Is a `Node` that represents a constant value.
`Synapse`
 Represents a connection in the neural network. This could be between two nodes or between a node and an input/output of a neural network.
`ActivationFunction`
 An enum representing a type of activation function used by a neuron.
`Network`
 Represents a neural network consisting of nodes and synapses. Inputs and outputs of a neural network are represented by designated synapses.
#### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring
Rutines for saving/loading neural networks to/from disk.

### Key Classes in UnitTesting
`TestCase`
 Represents a single test case with input and expected output.
`TestCaseList`
  Represents a collection of test cases representing a single algorithm.
#### In Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring
Various routines for saving/loading test cases and test case lists to/from disk.

### Key Classes in NeuralNetworkingGenetics
Genetic algorithm logic for neural networks without knowledge of test cases.
#### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
Mutators of neural networks - not related to test cases.
#### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring
Quality measurement of neural networks - not related to test cases.

### Key Classes in NeuralNetworkingUnitTesting
Neural network unit testing logic without knowledge of genetic algorithm.
`NetworkExtension`
 Extension methods for neural networks to run test cases.
#### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating
Backpropagation logic for neural networks to fulfill test cases.
##### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math
Math for backpropagation.

### Key Classes in NeuralNetworkingGeneticsUnitTesting
Genetic algorithm logic for neural networks related to test cases.
`NetworkSimulation`
 Represents a simulation of neural networks being evolved to fulfill test cases.
##### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating
Code related to mutating neural networks with usage of test cases.
##### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring
Code related to serialisation/deserialisation, saving/loading neural network mutators that use test cases to/from disk.
##### In Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring
Code related to measuring quality of neural networks with usage of test cases.


## Pawelsberg.GeneticNeuralNetworkConsole application
### In Pawelsberg.GeneticNeuralNetworkConsole.Model
`Command`
  Base class for a console command.
`CommandDispatcher`
  A collection of commands with their names.
`MainMenu`
  Represents application's main menu of the commands of the console application.
`Variable`
  Represents a variable that can be used in the application.
  


 TODO: finish


