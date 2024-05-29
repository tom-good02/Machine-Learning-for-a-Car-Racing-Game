# Machine Learning for a Car Racing Game

Built in Unity, this project provides the ability to create and train your own Neuroevolution Agents through an interactive UI and gamified experience. Alongside this, it utilises [ML-Agents](https://github.com/Unity-Technologies/ml-agents), a Unity toolkit, to train Reinforcement Learning Agents. A demo video of the project can be seen [here](https://www.youtube.com/watch?v=_l_QiZRoJls).

## Basic Installation

This will guide you through the steps to simply download and run the application. This will enable you to train Neuroevolution Agents. If you wish to train Reinforcement Learning Agents, please follow the advanced installation instructions.

- Note: The application has only been built for Windows and only tested on Windows 11
- Download the "ApplicationBuild" directory from this git
- Locate the Project.exe file and run it
- The application will now load and you are able to create and train Neuroevolution Agents

## Advanced Installation

This will guide you through the process of installing if you wish to open the project in the Unity Editor or if you wish to run Reinforcement Learning.

- Note: This has only been tested on Unity Editor version: 2022.3.10f1. Other versions of the editor may cause issues.
- Note: ML-Agents release 20 is the only tested version. Other versions of ML-Agents may not work.
- Download the repository
- (Optional, only necessary if you wish to edit the project) Add the project to Unity Hub. Click Add -> From Disk -> Select the "Project" folder
  - An error concerning ML-Agents will appear, click continue
  - An error concerning compilation will appear, click ignore
  - The project should now load into the Unity editor with compilation errors related to mlagents. This is because we have not yet installed ML-Agents
- Follow the installation instructions for [ML-Agents](https://github.com/Unity-Technologies/ml-agents). Only version 20 has been tested
  - This will involve installing Python, only Python version 3.9.13 has been tested
- Once ML-Agents is installed, in the terminal navigate to the Project directory
- Activate your Python environment
- The following command will begin training using the pre-built environments: `mlagents-learn config/car_config.yml --env=trackRL --time-scale=15 --width=1280 --height=720 --run-id=MyRunID`
- Important: Depending on which track difficulty you would like to train on, replace `track` in the command with either `Easy`, `Medium`, `Hard` or `Silverstone`. Also note:
  - Replace '`MyRunID`' with the name of your Agent.
  - `width` and `height` specify the size of the environment window. Feel free to change this depending on your screen size or preference.
  - `time-scale` specifies the speed at which the Unity environment will run at. A higher time-scale will result in faster training, although it is not advised to go above a value of 20 as Unity's physics engine can become unstable.
  - This command assumes you are in the 'Project' directory. If you are not, you may have to edit the path to the config file and the env.
  - The config file is located within the Project folder at `Project/config/car_config.yml`. This file contains the parameters used in training. Feel free to edit the parameters. For further detail on these or other available parameters visit the ML-Agents GitLab or web docs page.
