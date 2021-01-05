# Unity ML Agents - Experiments
#### This repository contains my experiments with Reinforcement Learning and Imitation Learning using the ML Agents toolkit in the Unity engine
### All projects shown here are my own original work and have been written from scratch
`@author: Viswadeep Sarangi`

## Highlighted Projects

### Christmas Bug
This was a fun project made for Christmas 2020 showing an animation of an excited bug endlessly chasing Christmas presents.
The bug was designed from scratch, comprising of native Unity gameobjects linked together using Configurable Joints with appropriate limits. The movements of the bug (agent) were trained using RL AI based on PPO and rewarded on (1) looking towards the present, (2) moving towards the present, and finally (3) getting the present. The agent was penalized for (1) head touching the ground, and (2) taking too long to get to the present.  
Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder. The project can be found in the Scenes folder named "4. Ant Mover (RL)"   

[You can try it out live in your browser by clicking here](https://viswadeep-sarangi.github.io/christmas-spider-unity-webgl-build/)

***This animated gif demonstrates a bug model created with native Unity gameobjects and Configurable Joints, trained to crawl towards the presents***

![Christmas Bug Hunt](_github_readme_resources/christmas_bug_crawl.gif)   

**Keywords:** `Reinforcement Learning` `Promixal Policy Optimization` `Multilayer Perceptron Neural Network` `Walking Model`



## Other Projects

### 1. Replica of the basic 'Ball Balancing on Cube' AI included in ML Agents example projects
This was my first try at understanding the inner mechanisms of the ML Agents toolkit, to later modify and adapt to my own ideas. The project can be found in the Scenes folder named "1. Cube Ball Balancing AI Env (RL)".

***This animated gif demonstrates 9 cubes, all having the same 'ML Brain' but initialized with random rotations.***   
Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder. 

![Ball Balancing AI on Cube](_github_readme_resources/cube_ball_balancing_ai.gif)   
   
**Keywords:** `Reinforcement Learning` `Promixal Policy Optimization` `Multilayer Perceptron Neural Network`
      
      
### 2. Ball Balancing on a Bigger Ball
After understanding the internal mechanisms of the ML Agents toolkit, this project was implemented to train the RL AI on a 'harder' problem which adhered to the previous objective of balancing a ball, but on a spherical surface. The project can be found in the Scenes folder named "2. Sphere Ball Balancing AI Env (RL)". This project tests the ML-Agents toolkit in being able to optimize the agent in a limited tight space of movement with an initially randomly placed ball and spherical bigger ball. 

***This animated gif demonstrates 9 spheres, all having the same 'ML Brain' but initialized with random positions. The redder the balls, the worse they are at balancing.***   
Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder.

![Sphere Ball Balancing AI on Cube](_github_readme_resources/sphere_ball_balancing_ai.gif)     

**Keywords:** `Reinforcement Learning` `Promixal Policy Optimization` `Multilayer Perceptron Neural Network`

### 3. Dribbling a Ball on a Plate
Combining the learnings from the above projects, this project expanded the output space to be able to control both position as well as rotation of a plate surface with the aim of bouncing a ball, while making sure it doesn't go outside the bounds or drop to the ground. The twist is that, the ball bounces higher everytime it touches the surface, making it harder to contain it in the bounds with every bounce. The project can be found in the Scenes folder named "3. Bouncy Ball on Plate AI Env (RL)".   

***This animated gif demonstrates 9 plates, all having the same 'ML Brain' but initialized with random positions and rotations***    

Details of the training parameters of the ML-Agents Academy and the RL AI network are in `trainer_config.yaml` in the `ML Brain Training Results` folder.
   
![Bouncy Ball on Surface AI](_github_readme_resources/bouncy_ball_balancing_ai.gif)  

**Keywords:** `Reinforcement Learning` `Promixal Policy Optimization` `Multilayer Perceptron Neural Network`
   
  
-----  
### Feel free to ask me about any of the projects at <viswadeep.sarangi@gmail.com>
