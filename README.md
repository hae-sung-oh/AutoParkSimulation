Simulation Program

Env setup
    git clone
    open project
    add ML-Agent
        Window -> Package Manager -> Unity Registry -> ML Agent Install

    Download ML-Agent: 
        Ubuntu: https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md
        Windows: https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation-Anaconda-Windows.md

        conda create -n autopark python=3.10
        conda activate autopark
        pip install tensorflow-gpu
        python -m pip install mlagents==0.30.0

Scenes
    Front
    Diagonal
    Parallel
    Master

Training
    mlagents-learn trainer_config.yaml
    mlagents-learn trainer_config.yaml --resume
    mlagents-learn trainer_config.yaml --force
    mlagents-learn trainer_config.yaml --run-id={}

Test
    Check created Neural Network Model (Behavior Parameter)
    Heuristic Mode

