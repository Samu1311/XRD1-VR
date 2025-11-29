# Blog Post 06 – The Great Merge

## 🔗 Integrating Everything
Week 6 was easily the most technically demanding. After all rooms were mostly complete, we needed to merge:
- 4 rooms  
- 1 Main scene  
- portals  
- puzzle logic  
- audio systems  
- prefabs  

Predictably, Git decided to test our patience.

## 🛠️ Merge Conflicts Everywhere
A few issues we ran into:
- multiple XR Origins from different branches  
- duplicate Interaction Managers  
- mismatched scene names  
- broken prefab links  
- objects disappearing after merge
- missing scripts and/or references

We were forced to create **Main-v2**, a clean rebuild of the main scene, so we could re-link everything manually. That took a lot of time, with stuff still breaking sometimes even when we switched to working simultaneously only on main-v2, committing directly and pulling updates in there. 

## 🧪 Testing the MVP
Once the scenes were finally integrated:
- teleportation worked  
- most of the puzzles triggered properly  
- portals opened as expected  
- performance remained acceptable  

We had a mostly working MVP, even if the process wasn’t smooth... at ALL!

## 🧠 Reflection
This was the most “real software engineering” week so far.  
Merging multiple independent XR projects is messy — but facing this together taught us a lot about organization and communication.
Additionally, we learned an important lesson - to do environments in smaller scales using less assets and focusing more on the interactions, as well as to **never again** leave merging for projects this large for the last few days before the presentation... At least we had fun? Not counting the merging..._shudders in PTSD_.

## 🔭 Next Steps
Final presentation week + final bug fixes after the presentation.


Authors: Alejandro & Ginta
