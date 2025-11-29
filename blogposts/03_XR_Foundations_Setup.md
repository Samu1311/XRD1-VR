# 🌀 Blog Post 03 – Building the Main Hub

## 🏛️ Establishing a Shared Starting Point
Week 3 was all about creating the **Main Scene** — the central hub where the XR Origin, camera, UI systems, and portal logic all live.

Instead of every team member having their own rig, we decided early on to use a **single shared XR setup** in the Main scene. This allowed:
- consistent locomotion  
- consistent player height  
- consistent input actions  
- unified lighting & audio baseline  

## 🚪 Loading Rooms From the Main Scene
We implemented (and debugged…) the logic for loading rooms **additively**. This means:
- The Main scene remains active  
- Each civilization room loads on top  
- The camera and player rig never change scenes  

This gave us a clean structure for the entire project.

## 🧪 Testing Teleportation & Movement
With the camera anchored in Main and rooms loading around it:
- We teleported into the first room  
- Walked across the temple environment  
- Identified collider issues and scaling inconsistencies  
- Verified interaction systems still worked after scene loading  

## 🔧 Challenges
We quickly learned:
- XR rigs break VERY easily when duplicated  
- Collisions between multiple rooms must be isolated  
- Everything needs to remain modular to avoid merge conflicts  

## ✍️ Reflection
This week made the entire project feel “real” for the first time. Being able to move from Main to a civilization room gave us the structure we needed for future puzzle work.

## 🔭 Next Steps
Each team member will begin implementing **their room’s main puzzle mechanics**.


Author: Ginta
