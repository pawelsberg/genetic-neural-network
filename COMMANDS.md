# Description of the Commands
## `create`
Creates a neural network that fits test case list. 
The neural network generated will perfectly work for the current test case list. 
Output of the neural network will match expected output for all test cases.
It is highly probable that the network generated will have many times more synapses and nodes that are needed.
Further evolution can improve the solution.

Usage: `>create`
## `help`
Shows the list of commands and a brief description. 
To get more details about a specific command add the command name after `help`.

Usages:
`>help`
`>help (command name)`
## `load`          
Loads specific neural network as additional specimen in generation.

Usage: `>load (network name)`
## `loadall`
Loads all neural networks as additional specimens.

Usage: `>loadall`
## `loadclear`     
Loads specific neural network as the only specimen in generation. 
This command will remove all other specimens from the generation.
The simplest possible network (containing one input and one output with one node) is named 'pass'.
You can load it ussing this command to restart simulation. For this run `>loadclear pass` command.

Usage: `>loadclear (network name)`
## `loadppmtcl`    
Loads a test case list in ppm format.

Usage: `>loadppmtcl (ppm test case list name)`
## `loadsudokutcl`
Loads a test case list in sudoku format.

Usage: `>loadsudokutcl (sudoku test case list name)`
## `loadtcl`       
Loads a test case list (standard type described in TestCases.XML files).

Usage: `>loadtcl (test case list name)`
## `pause`         
Pause genetic algorithm.

Usage: `>pause`
## `quit`          
Exit the program without saving.

Usage: `>quit`
## `run`           
Start genetic algorithm for specific number of generations.

Usage: `>run (number of generations)`
## `save`          
Save the best neural network. Existing network with the same name will be overwritten.

Usage: `>save (network name)`
## `saveppm`       
Save the best neural network in ppm format.

Usage: `>saveppm (ppm name)`
## `set`           
Show/set general settings of genetic algorithm.

Usages:
`>set`
`>set (variable name)`
`>set (variable name) (new value)`

There are the following variables to be changed:
### `maxNodes`
  Is the maximum number of nodes up to which genetic algorithm will try to create them. 
  If current number of nodes is greater then this number then genetic algorithm will not try to add any new nodes beyond this number.
  Note that this limit doesn't apply to situations like loading neural network or the `>create` command. 
  Only to the situation when a child neural network specimen is generated.
  
  Default value: `10`
### `maxSynapses`
  Maximum number of synapses to which genetic algorithm will try to create them.
  If the current number of synapses is greater then this number then genetic algorithm will not try to add any further synapses.
  Note that this limit doesn't apply to situations like loading neural network or the `>create` command. 
  Only to the situation when a child neural network specimen is generated.
  
  Default value: `30`
### `propagations`
  Number of times a neural network is processed to calculate output. 
  This matters if the neural network have loops. 
  If the neural network shouldn't have loops it can be set to 1. 

  Default value: `4`
### `successfulMutationsLength`
  This is the number of latest mutations saved and displayed when running `>show` command.

  Default value: `5`
### `maxSpecimens`
  Maximum number of specimens created per generation by genetic algorithm.
  Note that this limit doesn't apply to situations like loading neural networks using `>load ...` and `>loadall` commands. 
  In this case number of specimens per generation can be exceeded.
  
  Default value: `4`
### `delayTimeMs`
  Period of time for UI/background simulator synchronisation.
  
  Default value: `5`
### `generationMultiplier`
  Maximum number of generations to be processed in `delayTimeMs` period.

  Default value: `5`
### `meterType`
  Type of meter to measure quality of the neural network. 
  Each meter type consists of a number of measurements summed together to give a final value of quality.
  Possible values:
  #### `Normal`
  Standard set of measurements
  #### `LowestMultipliers`
  Network with lowest sum of node multipliers is prefered.
  #### `Sequential`
  Measures subsequent test cases only if previous were satissfied.
  #### `Aggregate`
  Checks only the test cases.
  #### `PropagationsAgnostic`
  Runs network for a range of propagations to make sure network is number of propagations agnostic.
    
  Default value: `Normal`
### `parentQueuer`
  Defines how specimens are being selected for the next generation.
  Possible values are: `Normal`,`Unique`,`RandomEnd`

  Default value: `Normal`
### `mutatorsType`
  Defines the way mutations are introduced to the specimens to create new specimens for a next generation.
  Possible values are: `None`,`Normal`,`Cleaner`,`BackpropagationOnly`,`NormalWithBackpropagation`.
  
  Default value: `Normal`
### `seed`
  Seed number for random operations. 
  If provided will result in a repetable result of the simulation. 
  If the value is `x`, the seed is generated based on the current time, leading to non-deterministic results.

  Default value: `x`
## `show`         
Show the results of the genetic algorithm simulation.
Use 'test' as a parameter to show also test results.
Use 'tests' to show test results in more concise form.

Usages:
`>show`
`>show test`
`>show tests`
## `start`         
Start genetic algorithm simulation.

Usage: `>start`
