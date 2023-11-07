# GenPred

A Markov decision process (MDP) reachability probability generation tool for NUS [CS4211 Formal Methods for Software Engineering](https://nusmods.com/courses/CS4211/formal-methods-for-software-engineering) course project: _Applying Probabilistic Model Checking in Tennis Match Analytics_.

## Usage

```cmd
GenPred <input> <output>
```

* `input`: A directory containing several `.pcsp` files.
* `output`: A file in `.csv` format.

`GenPred` will verify all PCSP models in the `input` directory by calling the dynamic link libraries (DLLs) of [Process Analysis Toolkit (PAT)](https://pat.comp.nus.edu.sg/) and store the minimal and maximal reachability probabilities of winning states to `output`, which will then be evaluated by the betting simulator.