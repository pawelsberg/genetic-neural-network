# GLOSSARY.md

## Terms and Definitions

### Neural Network
A computational model inspired by the way biological neural networks in the human brain work. It is designed to recognize patterns and consists of nodes connected by synapses. 
In this program nodes are connected in any order and loops are possible. For that reason network may need more than one pass to generate output.

### Genetic Algorithm
A search heuristic inspired by the process of natural selection. It is used here to find a neural network that fits best provided criteria.

### Specimen
An individual solution (in this context, a neural network) within a generation that the genetic algorithm evaluates.

### Generation
A group of specimens that are evaluated at the same time by the genetic algorithm.

### Test Case List
A set of inputs and expected outputs used to evaluate the performance of a neural network.

### Quality
A measure of how well a neural network performs against a test case list. The quality measurement can also include depending on configuration other metrics like: size of the network, time of evaluation.

### Synapse
A connection between input and node, node and another node or node and output in a neural network that transmits signals.

### Node
An individual processing unit in a neural network. Represents a sumator with possibly many inputs and possibly many outputs (synapses). It consists of a specific activation function.

### Propagation
The process of processing input through a neural network. Network may need many propagations to produce the output. 

### Mutation
A change introduced to a specimen to create a new specimen for the next generation. In this case neural network is changed by modifying it's structure.

### Seed
A starting point for random operations in the genetic algorithm. Providing a specific seed will result in repeatable results of the simulation.
