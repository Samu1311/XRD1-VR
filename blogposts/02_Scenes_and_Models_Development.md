# 🗺️ Blog Post 02 – Importing Assets & Setting the Stage

## 🎯 Week Focus
After our initial concept development, Week 2 was dedicated to something far more practical: **finding 3D models**, organizing them, and figuring out how to properly import them into Unity without destroying performance or aesthetics.

## 🧱 Understanding Models, Materials & Textures
Most of our time was spent learning how differently assets behave depending on:
- File format (.fbx, .glb, .gltf…)
- Texture workflows (albedo, normals, metallic, roughness)
- UV maps (or lack of them…)
- Mesh structure (single piece vs. dozens of small objects)

Everyone on the team had the same experience: importing a model rarely looks “right” at first. Many objects arrived with:
- missing textures  
- weird shading  
- inverted normals  
- dozens of mesh pieces  

We spent most of the week experimenting with how to fix this, including:
- organizing the project into a clean folder hierarchy  
- creating materials manually  
- reassigning textures  
- resizing or repositioning assets  
- understanding how Unity handles FBX imports  

By the end of the week, we had **4 environment** fully imported in correct scale, with proper textures and lighting.

## 👥 Team Reflections
We realized early on that each room would require a different set of assets and styling, so working in parallel made sense. Still, we exchanged tips constantly — especially about how to make models look good in VR without heavy performance costs.

This week made us appreciate how crucial asset preparation is. A wrong material setup can make even the best model look awful in VR.

## 🔭 Next Steps
Next week we’ll focus on building the **Main Scene**: the central hub that will load each room.

Author: Alejandro
