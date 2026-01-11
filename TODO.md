# TODO
1. Mutators:
    2. allow modifying from console
    3. back propagation doing all weights changes in one go.
2. Meter types:
    5. allow modifying from console
3. Create UI
4. Trace mutations better in the history
5. Test cases:
    1. Allow modifying
    3. Allow saving
6. Networks:
    1. Allow modifying
7. Show:
    1. PPM
    2. Sudoku
8. Add top command - shows live simulation information.
9. Running on GPU
    Variable maxGpuSpecimens will control how many specimens will run on GPU. If 0 - no running on GPU.
    Variable maxSpecimens will still control how many specimens will run on CPU. If both maxGpuSpecimens > 0 and maxSpecimens > 0 then simulation will run on both CPU and GPU.
    In each generation the following will be done on GPU:
        - quality measurement of neural networks which includes networks propagation.
    The rest will run on CPU. Including back propagation for now.