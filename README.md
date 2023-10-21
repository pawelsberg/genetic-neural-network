# genetic-neural-network
Genetic algorithm evolving structure and weights of the neural networks to perform some tasks. 
This program uses CLI (command-line interface). For a brief description of the commands use `>help` command or for a full description see COMMANDS.md 

# Tutorial:
## 1) 'Is it 5' neural network
This example shows how to use this program to generate a neural network that realises function checking if the input value is equal to 5.
Function (neural network) will return 1 if the input value is equal 5 otherwise function will return 0.
The following table describes behaviour:
| Input Value | Output Value |
| ----------- | ------------ |
| -1          | 0            |
| 0           | 0            |
| 4           | 0            |
| 5           | 1            |
| 6           | 0            |
| 100         | 0            |


### a) Prepare test case list
First we need to prepare a list of test cases that will describe wanted behaviour.
In the `%appdata%/Pawelsberg.GeneticNeuralNetwork\TestCases` folder create a file with test cases named `isit5.TestCases.XML`:
```xml
<TestCaseList>
	<TestCase>
		<Input>-1</Input>
		<Output>0</Output>
	</TestCase>
	<TestCase>
		<Input>0</Input>
		<Output>0</Output>
	</TestCase>
	<TestCase>
		<Input>4</Input>
		<Output>0</Output>
	</TestCase>
	<TestCase>
		<Input>5</Input>
		<Output>1</Output>
	</TestCase>
	<TestCase>
		<Input>6</Input>
		<Output>0</Output>
	</TestCase>
	<TestCase>
		<Input>100</Input>
		<Output>0</Output>
	</TestCase>
</TestCaseList>
```
### b) Start simulation
Open application. Load test case list by command:
```
>loadtcl isit5
```
Start simulation in background by typing the following command:
```
>start
```
Simulation will run in the background.
To check the progress of the simulation run the following command:
```
>show
Best Ever Network:
Input 0 * 0.0000000000000 -> Neuron(L) 0
Neuron(L) 0 -> Output 0

Best Ever Quality: 112.01
Max Possible Quality: 167.0001
Generation: 10820
Best Ever Network (Nodes:1,Synapses:2)
Last Successful Mutations:
```
To check how well the current best network is peforming in solving test cases use command:
```
>show test
Best Ever Quality: 112.01
Inputs:  -1.0
ExpOut:   0.0
Output:   0.0

Inputs:   0.0
ExpOut:   0.0
Output:   0.0

Inputs:   4.0
ExpOut:   0.0
Output:   0.0

Inputs:   5.0
ExpOut:   1.0
Output:   0.0

Inputs:   6.0
ExpOut:   0.0
Output:   0.0

Inputs: 100.0
ExpOut:   0.0
Output:   0.0
```
### c) save results
After some time program will generate a neural network that works well with the set of test cases:
```
>show test
Best Ever Quality: 159.50000843507664
Inputs:  -1.0
ExpOut:   0.0
Output:   0.0

Inputs:   0.0
ExpOut:   0.0
Output:   0.0

Inputs:   4.0
ExpOut:   0.0
Output:   0.0

Inputs:   5.0
ExpOut:   1.0
Output:   1.0

Inputs:   6.0
ExpOut:   0.0
Output:   0.0

Inputs: 100.0
ExpOut:   0.0
Output:   0.0
```
Expected values (ExpOut) equal actual values returned from the network (Output).
The network generated has 3 nodes and 6 synapses:
```
>show
Best Ever Network:
Input 0 * 1.4272446499341 -> Neuron(L) 0
Neuron(T) 1 * -5.7204412862237 -> Neuron(L) 0
Neuron(L) 0 * -0.7671565038895 -> Neuron(S) 2
Neuron(L) 0 * 0.6759530660428 -> Neuron(T) 1
Neuron(T) 1 * 2.1800208000247 -> Neuron(S) 2
Neuron(S) 2 -> Output 0

Best Ever Quality: 159.5000084955875
Max Possible Quality: 167.0001
Generation: 191570
Best Ever Network (Nodes:3,Synapses:6)
Last Successful Mutations:
Gen:186244 Qual: (159.5000084949367 => 159.5000084950384 - incr 0.0000000001017)
 Mutation: RandomNumberOfTimes(1):(NeuronModifier)
Gen:189815 Qual: (159.5000084950384 => 159.5000084953047 - incr 0.0000000002664)
 Mutation: RandomNumberOfTimes(1):(NeuronModifier)
Gen:190046 Qual: (159.5000084953047 => 159.5000084954067 - incr 0.0000000001019)
 Mutation: RandomNumberOfTimes(1):(NeuronModifier)
Gen:191112 Qual: (159.5000084954067 => 159.5000084955482 - incr 0.0000000001415)
 Mutation: RandomNumberOfTimes(1):(NeuronModifier)
Gen:191178 Qual: (159.5000084955482 => 159.5000084955875 - incr 0.0000000000393)
 Mutation: RandomNumberOfTimes(1):(NeuronModifier)
```
To save the result neural network use the command:
```
>save isit5_v1
```
This result can be loaded later on by using:
```
>load isit5_v1
```
Note that loading a neural network will add it as a specimen to the current generation. If the network loaded is not good enough it can be eliminated by other specimens in the generation (of higher quality).
Use the following for the full list of commands:
```
>help
```


