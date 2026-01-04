# Alejandro Bautista – Individual Reflection
## Main Contributions

- Designed and implemented the **Ancient Egypt room**, including environment layout and interactive puzzles  
- Created and integrated **VR interactions** using the Unity XR Interaction Toolkit  
- **Modified and divided an existing Sphinx model** so it could be properly rigged and animated  
- **Animated the Sphinx in Blender**, then imported and integrated the animation into Unity  
- Designed **audio-guided storytelling**, where the Sphinx guides the player through riddles  
- Implemented tools such as an **auto-grabbable system** for interactive objects  
- Used a **mesh combine script** to improve performance for large environment objects  
- Handled the **majority of project merges**, integrating all team scenes into one project  
- Fixed **merge bugs and inconsistencies** across scenes (colliders, interactions, lighting, textures)  
- Solved **texture and material issues** caused by using different render pipelines  

---

## Reflection

### Learning by Doing in XR Development

Before this course, I had **no prior experience with Unity**, which made the project challenging from the beginning. The course mainly introduced us to the **different parts that compose a VR application in Unity**, such as scenes, interactions, physics, animation, audio, and performance considerations.

Rather than focusing on very specific implementations, the course encouraged learning how these parts work together by actively developing an application. As a result, most of my learning happened while building, testing, debugging, and fixing issues. Through this process, I developed an understanding of how Unity and VR systems interact, especially in the context of interaction and scene management.

---

### Applying Course Concepts in Practice

While the course explained the technical building blocks of a VR application, it was up to us to decide how to use them in practice. In the Ancient Egypt room, I applied this knowledge by combining interaction, audio, animation, and environment design.

Instead of using UI elements or text instructions, I guided the player using **audio narration**. The Sphinx speaks to the player and presents riddles that encourage exploration of the environment. This approach allowed the player to stay immersed while still understanding what to do.

The riddles were designed to naturally lead the player toward interacting with objects in the space. This helped connect interaction systems with level design and encouraged learning through exploration rather than explicit instructions.

---

### Design Decisions and Ownership

To make interactions easier to manage and more consistent, I worked on an **auto-grabbable system** that allowed objects to be picked up without complex individual setup. This simplified development and ensured similar behavior across interactive objects.

For larger environment elements, I used a **mesh combine script** to reduce the number of separate meshes. This helped improve performance and stability, which is especially important in VR to maintain smooth movement and avoid discomfort.

I also took responsibility for **merging the project**, integrating scenes created by different team members. This involved resolving broken references, fixing interaction and physics issues, and correcting visual inconsistencies. Handling merges required understanding how all parts of the VR application were connected, not just my own scene.

---

### Technical Challenges and Problem-Solving

One recurring challenge was dealing with **assets that used different render pipelines**, which caused textures and materials to appear incorrectly after merging scenes. I resolved this by reassigning materials and adjusting shaders so the visuals were consistent across the project.

Another challenge was ensuring that interactions continued to work correctly after merges. Issues such as incorrect colliders, missing components, or misplaced objects could easily break functionality. Fixing these problems required careful testing and reinforced the importance of consistency across scenes. With each merge, many required assets disappeared or broke, which meant they had to be fixed again.

---

### Animation and Content Integration

Although the Sphinx model already existed, it was not suitable for animation in its original form. I modified and divided the model so it could be properly rigged and animated in **Blender**. After animating it, I imported the animation into Unity and connected it to the Egypt room’s interaction and audio flow.

This process helped me understand how external content creation tools and Unity work together, and how animation and audio can be used to guide the player.

---

### Reflection and Future Improvements

The project itself was already quite **modular**, with different rooms separated into individual scenes. However, one of the main issues was that we **did not merge often enough early on**. Because of this, many bugs and conflicts accumulated and only became visible late in development, which made the merging process very stressful and time-consuming.

If I were to work on a similar project again, I would **merge earlier and more frequently**, even if features are not fully finished. Regular integration would have helped us detect issues sooner and reduce the amount of time spent fixing compounded bugs near deadlines. Probaably even research how to set up everything so the process is easier for everyone.

This experience showed me that even in modular projects, frequent merging and testing are essential to keep development manageable.

---

## Conclusion

This project was a major learning experience for me. Starting with no prior Unity knowledge, I was able to contribute to multiple aspects of the VR application, including interaction systems, animation, audio, performance, and project integration. By working hands-on with the different components of a VR app, I gained a clearer understanding of how they come together to form a complete experience.

Overall, the project helped me build both technical confidence and a broader understanding of VR development in Unity.
