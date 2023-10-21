# genetic-neural-network
Genetic algorithm evolving structure and weights of the neural networks to perform some tasks
# Examples of usage:
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

