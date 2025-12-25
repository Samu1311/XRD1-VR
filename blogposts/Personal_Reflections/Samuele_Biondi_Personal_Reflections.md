# Individual Reflection – VR Project (Forward to the Past)

## Personal Contributions

- Design and full implementation of the **Mesoamerica room**, including environment setup, interaction logic, audio, and visual feedback.
- Design and development of a **pressure plate puzzle system**, implemented as reusable prefabs with scripted logic, visual glow feedback, and sound effects.
- Decision-making and implementation regarding **interaction style**, using ray-based interactions combined with XR Grab Interactables and Rigidbody physics.
- Manual creation and assignment of **materials and textures**, as well as optimization through selective collider placement to reduce unnecessary physics calculations.
- Contribution to the **initial Main Scene setup**, including XR Origin configuration, additive scene loading strategy, and early portal prototype design.
- Proposal of the project’s **folder structure**, later adopted and maintained collaboratively by the team.
- Active participation in testing, debugging, and integration phases, including workarounds for Quest Link compatibility issues.

---

## Reflection on Design and VR Theory

The goal of *Forward to the Past* was to create an immersive and engaging VR experience that allows players to explore historical environments through interaction and puzzle solving. From a VR theory perspective, this project helped me better understand how concepts such as immersion, presence, embodiment, and interaction design translate from theory into concrete implementation decisions.

In my Mesoamerica room, I focused on creating a sense of spatial presence by combining a large-scale environment, ambient audio inspired by the historical setting, and world-locked interactions. According to the course material, immersion in VR is not only about visual fidelity, but also about how consistently the virtual world responds to the user’s actions. This directly influenced my decision to rely on physical metaphors, such as pressure plates and weighted stone boxes, rather than abstract UI-based puzzles.

A conscious design choice I made was to avoid explicit UI instructions. Instead, I relied on affordances and feedback: pressure plates visually react when activated, and audio cues reinforce the player’s actions. This aligns with interaction design principles discussed in the course, where feedback and signifiers guide the user without breaking immersion. As a player myself, I find overly explicit UI disruptive, and this personal perspective influenced my design approach while still remaining consistent with VR usability theory.

### Interaction Choices and Technical Decisions

Initially, I attempted to implement a rotating Mayan calendar puzzle composed of multiple rings. However, this approach revealed several VR-specific usability issues, such as unreliable targeting, overlapping interactable objects, and rotation constraints that were difficult to control intuitively. From a theoretical standpoint, this highlighted how interaction fidelity and precision are critical in VR, especially when relying on ray-based selection.

Due to these issues and time constraints, I decided to pivot to a pressure plate puzzle. This decision reflects an iterative prototyping mindset, emphasized throughout the course: discarding an idea is sometimes necessary when it negatively affects usability or player experience. The pressure plate solution proved more intuitive, encouraged exploration of the environment, and allowed for clearer feedback mechanisms.

The use of ray interactors instead of direct hand interaction was another deliberate choice. Given the scale of the environment, ray-based interaction reduced physical reach limitations and improved accessibility. Combined with XR Grab Interactables and Rigidbody physics, this provided a balance between realism and usability.

### Implementation Challenges and Learning Outcomes

One of the main technical challenges I faced was managing colliders and triggers. Initially, incorrect collider sizing caused objects to float above the pressure plates. By separating trigger volumes from physical colliders, I achieved both reliable detection and visually correct object placement. This reinforced the importance of aligning physics behavior with visual expectations in VR.

Another challenge involved material reuse and texture orientation, particularly when applying existing stone materials to new objects such as the weighted boxes. Solving this required a deeper understanding of UV mapping and texture alignment, which strengthened my idea of how visual consistency affects immersion.

Hardware-related issues also played a role. Due to Quest Link compatibility problems on my own machine, I relied on teammates’ setups to test VR functionality. While inconvenient, this experience emphasized the practical constraints of VR development and the importance of collaborative testing.

### Testing, Feedback, and Final Reflection

During the XR Lab expo day, user feedback confirmed that the visual quality and ambience of the environments were strong, while also revealing that some rooms felt overwhelming without guidance. Although my room did not include UI instructions, observing how players naturally interacted with the pressure plates validated my design choices.

Overall, this project significantly deepened my understanding of VR development. Concepts such as immersion, interaction design, locomotion, and performance optimization became real through implementation. More importantly, I learned how theoretical principles must often be balanced against technical limitations and usability concerns. The experience reinforced the importance of iteration, player-centered design, and pragmatic decision-making in VR development.

---

*Author: Samuele Biondi (316357)*
