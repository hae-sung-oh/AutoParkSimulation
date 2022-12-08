# Autonomous Parking Simulation
This project is about an autonomous parking simulation program with Unity ML-Agent.

[Program Download Link](https://drive.google.com/file/d/1nP-ikZiklXb_1zPpwrG-GXffE2ixzpMr/view?usp=sharing)

<br/>

## Installation
* Clone the git repository
```
git clone https://github.com/hae-sung-oh/AutoParkSimulation.git
```

* Open the project with Unity Hub

* Add ML-Agent package in the project
    * Window -> Package Manager -> Unity Registry -> ML Agent Install

* Install ML-Agent
    * For Ubuntu: [Instructions](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md)
        ```
        conda create -n autopark python=3.10
        conda activate autopark
        pip install tensorflow-gpu
        python -m pip install mlagents==0.30.0
        ```

    * For Windows: [Instructions](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation-Anaconda-Windows.md)


<br/> 

## Scenes
There are 3 Unity scenes for corresponding parking situations. The `Master` scene is for the `.exe` program.
* Front
* Diagonal
* Parallel
* Master

<br/>

## Training
Now, if you run following commands and hit the 'Play' button in the Unity Editor, you can train the agent with Reinforcement Learning.

* Default command
    ```
    mlagents-learn trainer_config.yaml
    ```

* Specify RUN_ID (You can label the custom training ID)
    ```
    mlagents-learn trainer_config.yaml --run-id={RUN_ID}
    ```

* Resume existing training with RUN_ID
    ```
    mlagents-learn trainer_config.yaml --run-id={RUN_ID} --resume
    ```

* Run existing training forcefully and overwrite it
    ```
    mlagents-learn trainer_config.yaml --run-id={RUN_ID} --force
    ```


<br/>

## Test
After the training, you can test the result with the following methods.

* Check created Neural Network Model (Behavior Parameter)


* Heuristic Mode (Manual Control)

