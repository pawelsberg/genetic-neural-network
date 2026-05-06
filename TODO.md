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
10. Change test cases to use decimal instead of integer
11. Additional refactorings:
    1. Meters that have children. Consider a separate class/interface for those. This is to avoid hiding Children for some containers.
    2. Interfaces/names/parameters of INetworkQualityMeterContainerTextConvertible and INetworkQualityMeterTextConvertible need to be clearer. Need to digg deeper to understand the difference.
    3. Investigate AI's code
    4. RandomNumberOfTimes, MultipleTimes and NothingDoer
        can parsing be placed for each of those mutators in dedicated location?
    5. List all the methods that just are used in tests and delete them - together with those tests.
12. AI reported bugs - investigate:
    1. TestCasesSequentialContainerQualityMeter does not serialize _goodDifference. TestCasesSequentialContainerQualityMeter.cs:92 writes TestCasesSequential() with empty parens; the parser at line 213 hardcodes 0.001d. Save→load is lossy for any non-default value.

