# Blog Post 05 – Polishing the Puzzles

## 🎨 From Prototype to Real Puzzle
Week 5 was about locking down our final puzzle idea and polishing it so it fit naturally into the environment.

For Mesoamerica:
- Three pressure plates were created as **reusable prefabs**  
- Each plate reacts to weight using colliders and events  
- A “stone box” prefab was made so players know what to place on top  
- A glowing animation appears when activated  
- When all three plates are triggered → a calendar behind them glows  

The calendar no longer rotates, but now acts as a **reward highlight**.

## 🧰 Interaction Logic
The system uses:
- XRGrabInteractables (for the boxes)  
- Rigidbody physics  
- Trigger events  
- A central manager tracking puzzle completion  

Once solved, the Mesoamerican room gives clear audio & visual feedback.

## 👥 Team Progress
Everyone reached a similar level of polish:
- Egypt’s Sphinx logic works reliably  
- China’s gong triggers the exit door  
- Greece’s vases show depictions of different greek myths, and the Oracle is able to answer yes/no questions 

## 🧠 Reflection
This week felt productive and satisfying — seeing interactions run smoothly in VR was extremely motivating.

## 🔭 Next Steps
Integrating all rooms into Main, solving inevitable merge conflicts.


Author: Samuele
