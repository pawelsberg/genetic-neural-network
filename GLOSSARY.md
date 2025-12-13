# GLOSSARY.md

## Terms and Definitions

### Activation Function
A mathematical function applied to a neuron's output to determine its final value. Available activation functions include:
- **Linear (L)**: Returns the input value unchanged.
- **Threshold (T)**: Limits output to the range from -1 to 1.
- **Squashing (S)**: Limits output to the range from 0 to 1.
- **Sigmoid (Σ)**: Applies the sigmoid function, limiting output to the range from 0 to 1.
- **Tanh (H)**: Applies the hyperbolic tangent function, limiting output to the range from -1 to 1.

### Backpropagation
A supervised learning algorithm used to adjust the weights of synapses in a neural network based on the error between expected and actual outputs. In this program, backpropagation can be used as part of the mutation process to fine-tune network weights.

### Bias
A special type of node in a neural network that provides a constant output value. Biases can be added to networks through mutations to help the network learn patterns that require an offset.

### Generation
A group of specimens that are evaluated at the same time by the genetic algorithm.

### Genetic Algorithm
A search heuristic inspired by the process of natural selection. It is used here to find a neural network that fits best provided criteria.

### Multiplier (Weight)
A numerical value associated with a synapse that scales the signal passing through it. Multipliers determine the strength and influence of connections between nodes.

### Mutation
A change introduced to a specimen to create a new specimen for the next generation. In this case neural network is changed by modifying its structure or weights.

### Mutator
A component responsible for introducing specific types of changes (mutations) to neural network specimens. Different mutators perform different modifications such as adding neurons, modifying weights, removing synapses, or applying backpropagation.

### Mutators
A collection of mutators with associated weights that determine how frequently each type of mutation is applied during the genetic algorithm. The weights influence the probability of selecting each mutator.

### Neural Network
A computational model inspired by the way biological neural networks in the human brain work. It is designed to recognize patterns and consists of nodes connected by synapses. 
In this program nodes are connected in any order and loops are possible. For that reason network may need more than one pass to generate output.

### Neuron
A type of node that processes inputs by summing weighted signals and applying an activation function. Each neuron has input synapses with multipliers and output synapses.

### Node
An individual processing unit in a neural network. Can be a Neuron (with inputs and activation function) or a Bias (constant output). Nodes have output synapses connecting them to other nodes or network outputs.

### Parent Queuer
A component that determines how specimens are selected from one generation to create the next. Different queuing strategies affect the diversity and convergence of the genetic algorithm. Available types: `Normal`, `Unique`, `RandomEnd`.

### Propagation
The process of processing input through a neural network. Network may need many propagations to produce the output. The number of propagations is configurable.

### Quality
A measure of how well a neural network performs against a test case list. The quality measurement can also include depending on configuration other metrics like: size of the network, time of evaluation.

### Quality Meter
A component that measures the quality of a specimen based on various criteria. Different meter types (`Normal`, `LowestMultipliers`, `Sequential`, `Aggregate`, `PropagationsAgnostic`) use different measurement strategies.

### Seed
A starting point for random operations in the genetic algorithm. Providing a specific seed will result in repeatable results of the simulation.

### Simulation
The process of running the genetic algorithm to evolve neural networks over multiple generations. A simulation can be started, paused, and monitored.

### Specimen
An individual solution (in this context, a neural network) within a generation that the genetic algorithm evaluates.

### Synapse
A connection between input and node, node and another node or node and output in a neural network that transmits signals. Each synapse connected to a neuron has an associated multiplier (weight).

### Test Case
A single input-output pair used to evaluate neural network performance. Contains input values and expected output values.

### Test Case List
A set of test cases (inputs and expected outputs) used to evaluate the performance of a neural network.
