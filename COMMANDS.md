# Description of the commands
## create
Creates a neural network that fits test case list. The neural network generated will perfectly work for the current test case list. It is highly probable that the network generated will have many times more synapses and nodes that are needed.
Further evolution can improve the solution.
## help
Shows the list of commands and a brief description. To get more details about a speciffic command type:
`>help (command name)`
## load          
Loads specific neural network as additional specimen in generation
## loadall
Loads all neural networks as additional specimens
## loadclear     
Loads specific neural network as the only specimen in generation. Use 'pass' network as a parameter to load the simplest network (and restart algorithm).
## loadppmtcl    
Loads a test case list in ppm format
## loadsudokutcl 
Loads a test case list in sudoku format
## loadtcl       
Loads a test case list
## pause         
Pause genetic algorithm
## quit          
Exit the program without saving
## run           
Start genetic algorithm for specific number of generations
## save          
Save the best neural network
## saveppm       
Save the best neural network in ppm format
## set           
Show/set general settings of genetic algorithm
## show          
Show the results of the genetic algorithm simulation.
 Use 'test' as a parameter to show also test results.
 Use 'tests' to show test results in more concise form.
## start         
Start genetic algorithm simulation
