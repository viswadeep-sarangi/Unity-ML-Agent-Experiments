# Unity ML Agents - Experiments
#### This repository contains my experiments with Reinforcement Learning and Imitation Learning using the ML Agents toolkit in the Unity engine
### All projects shown here are my own original work and have been written from scratch
`@author: Viswadeep Sarangi`

## Projects

### 1. Replica of the basic 'Ball Balancing on Cube' AI included in ML Agents example projects
This was my first try at understanding the inner mechanisms of the ML Agents toolkit, to later modify and adapt to my own ideas. The project can be found in the Scenes folder named "Cube Ball Balancing AI Env".
Below is a demo of the ML Brain that was trained with the Proximal Policy Optimization and a Multilayer Perceptron based Neural Network. 

This animated gif demonstrates 9 cubes, all having the same 'ML Brain' but initialized with random rotations.
Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder. 

![Ball Balancing AI on Cube](_github_readme_resources/cube_ball_balancing_ai.gif)



### 2. Ball Balancing on a Bigger Ball
After understanding the internal mechanisms of the ML Agents toolkit, this project was implemented to train the RL AI on a 'harder' problem which adhered to the previous objective of balancing a ball, but on a spherical surface. This project tests the ML-Agents toolkit in being able to optimize the agent in a limited 'tight space' of movement with an initially randomly placed ball and spherical bigger ball. 

This animated gif demonstrates 9 sphere, all having the same 'ML Brain' but initialized with random positions. The redder the balls, the worse they are able to balance.
Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder.

![Sphere Ball Balancing AI on Cube](_github_readme_resources/sphere_ball_balancing_ai.gif)
