# Blog Post 05 – Polishing the Puzzles

## 🎨 From Prototype to Real Puzzle

Week 5 focused on finalizing our puzzle ideas and polishing them so they fit naturally into their environments.

### **Mesoamerica – Samuele**
- Built three **pressure plates** as reusable prefabs  
- Each plate reacts to weight using colliders, events, and a glowing activation effect  
- Created a **stone box** prefab to guide players on what to place on the plates  
- When all three plates are triggered, the **calendar activates with a glow effect** as the reward  

This room now has a clean interaction flow supported by XR physics and a central puzzle manager.

### **Egypt – Alejandro**
- Animated the **Sphinx** to react during puzzle interactions separatimng the different parts in blender to allow movement.  
- Added multiple audio cues that trigger when items are placed on the table near the Sphinx
- Made the items collected be placed in the correct position, once the user drop them in the table.  
- Ensured the puzzle feels lively, responsive, and integrated with the room’s atmosphere  

### **China – Eliza**
- Set up the animations for both the **gong** and the **exit door**  
- When the player interacts with the gong, the door opens in a smooth animated sequence  
- Added clear visual feedback to make the puzzle solution feel rewarding  

### **Greece – Ginta**
- Built an interaction system using canvas elements so players can explore **multiple Greek myths** on the vases  
- Added a **randomizer** that allows the Oracle to provide yes/no answers in an unpredictable, mysterious way  
- Enhanced the room’s narrative feel through varied mythological content  

---

## 🏛️ Main Scene Improvements

Several shared tools and optimizations were developed this week:

- **Mesh Combine Script** (Alejandro):  
  Merges child meshes of large models to reduce draw calls and boost performance.

- **Auto-Grabbable Tool** (Alejandro):  
  Automatically configures objects to be grabbable, speeding up scene setup.

- **Instruction System** (Ginta):  
  A flexible system for creating and updating room-specific instructions when needed.

These additions help unify the workflow and improve performance across all rooms.

---

## 🧠 Reflection

This week was extremely motivating — with each room reaching a similar level of polish, the project finally feels cohesive and smooth in VR.

---

## 🔭 Next Steps

- Integrating all rooms into the Main scene  
- Handling merge conflicts  
- Preparing for full puzzle flow testing  

**Author: Alejandro & Samuele**
