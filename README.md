# Collaborative-Museum-Heist-with-RL
This project uses Unity version 2020.3.27f1 and the ML-Agents 19 Package.

ML-Agents Toolkit: https://github.com/Unity-Technologies/ml-agents

To see results:
1. Open the scene Scenes/Complex Environment/CE_Group_Training_Scene in Unity and hit play. 
2. To asign different policy, add a policy from the folder Brain_CE to the agents. The option to change the policy of TechGuy and Locksmith is in the Editor in the gameobject Robbers>TechGuy>RobberAgent and Robbers>Locksmith>RobberAgent respectively under the section Model.

To train agents:
1. Create a new scene.
2. Add prefabs "vanilla I + curr I" or "curr II" depending on the environment you want. Environment Type I and Type II are described in the paper. 
3. Read file CreateEnvironment.pdf to create a virtual environment and train agents. Use the correct configuration files. For example if you want to train agents using the curriculum II method, add the prefabs "curr II" in the scene, and use the configuration "config/CurrII.yaml". 


Free assets from Unity Asset Store: https://assetstore.unity.com/packages/3d/environments/polytex-home-168285


