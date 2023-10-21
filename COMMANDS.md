# Description of the Commands
## create
Creates a neural network that fits test case list. 
The neural network generated will perfectly work for the current test case list. 
Output of the neural network will match expected output for all test cases.
It is highly probable that the network generated will have many times more synapses and nodes that are needed.
Further evolution can improve the solution.

Usage: `>create`
## help
Shows the list of commands and a brief description. 
To get more details about a specific command add the command name after `help`.

Usages:
`>help`
`>help (command name)`
## load          
Loads specific neural network as additional specimen in generation.

Usage: `>load (network name)`
## loadall
Loads all neural networks as additional specimens.

Usage: `>loadall`
## loadclear     
Loads specific neural network as the only specimen in generation. 
This command will remove all other specimens from the generation.
The simplest possible network (containing one input and one output with one node) is named 'pass'.
You can load it ussing this command to restart simulation. For this run `>loadclear pass` command.

Usage: `>loadclear (network name)`
## loadppmtcl    
Loads a test case list in ppm format.

Usage: `>loadppmtcl (ppm test case list name)`
## loadsudokutcl 
Loads a test case list in sudoku format.

Usage: `>loadsudokutcl (sudoku test case list name)`
## loadtcl       
Loads a test case list (standard type described in TestCases.XML files).

Usage: `>loadtcl (test case list name)`
## pause         
Pause genetic algorithm.

Usage: `>pause`
## quit          
Exit the program without saving.

Usage: `>quit`
## run           
Start genetic algorithm for specific number of generations.

Usage: `>run (number of generations)`
## save          
Save the best neural network. Existing network with the same name will be overwritten.

Usage: `>save (network name)`
## saveppm       
Save the best neural network in ppm format.

Usage: `>saveppm (ppm name)`
## set           
Show/set general settings of genetic algorithm.

Usages:
`>set`
`>set (variable name)`
`>set (variable name) (new value)`
## show          
Show the results of the genetic algorithm simulation.
Use 'test' as a parameter to show also test results.
Use 'tests' to show test results in more concise form.

Usages:
`>show`
`>show test`
`>show tests`
## start         
Start genetic algorithm simulation.

Usage: `>start`
