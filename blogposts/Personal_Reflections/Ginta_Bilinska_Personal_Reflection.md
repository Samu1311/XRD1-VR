## Main Contributions
- Designed and implemented the Greek room interactions: interactive vases and the Oracle of Delphi
- Researched, selected, and adapted environment and prop assets for the Greek room
- Implemented a room-level progression system for Greece that tracks player interactions and activates the portal once the necessary amount of interactions have been completed
- Co-responsible (with Alejandro) for resolving major merge conflicts which involved missing references, broken prefabs, and XR setup inconsistencies
- Began prototyping an archery training interaction, later cut due to scope and time constraints
- Came up with the title *Forward to the Past* (again, hehe)

### Introduction

Unlike with our earlier project SARA, our knowledge here did not begin entirely from scratch. 
We already had gained some experience working with Unity, structuring XR projects, and thinking in terms of tangible interaction and user experience. 

However, *Forward to the Past* quickly revealed that Virtual Reality introduces a much higher level of complexity and considerations. 
While the general workflow felt familiar, VR was significantly more time-consuming and fragile in practice.

One major difference was how much we relied on and learned to configure existing XR scripts, such as XR Interactable and XR Grabbable ones from the XR Interaction Toolkit, in addition to writing our own scripts. 
Understanding how these components work, how they depend on the XR Origin, and how easily they can break when duplicated or misconfigured was a whole learning experience in itself. 

It could be due to our scope and what we set to accomplish, but compared to our AR project, VR interactions felt even more sensitive to scale, colliders, physics, and timing, which meant that even small changes often required repeated testing inside the headset. This soon became another source of frustration due to the amount of time it would take to load scenes, but we tried to treat it as a lesson in patience as well as planning.

This project was also my first time using a VR headset at all, which made the experience especially exciting. Being fully immersed inside an environment we had built ourselves felt exhilarating, simultaneously changing how I approached interaction design and immersion. 
Sometimes things that seemed fine in the Unity editor felt uncomfortable, confusing, or even overwhelming in VR, reinforcing how critical real-device testing is when developing VR applications.

Although we used pre-made, free assets from the Unity asset store, a significant amount of time still went into adapting them. This involved searching for many different suitable models, adjusting scale and placement, redesigning layouts, and building terrains so the environment felt realistic and appealing. 
Sometimes, finding assets that fit together both visually and thematically felt a bit like playing The Sims, with slightly more pressure and value coming from placing, rotating, and replacing objects until the scene finally felt believable.

### Contributions and Technical Work

My primary responsibility in this project was designing and implementing the Greek room interactions that aligned with our goal, and felt intuitive in VR while encouraging exploration.

One of the core mechanics was the interactive Greek vases, which players could select to reveal mythological stories. 
Each vase was implemented as an XR interactable and connected to a shared myth database using a ScriptableObject. 

This separation between interaction logic and content allowed myths to be reused and easily extended without changing the interaction code:

`[SerializeField] private GreekMythDatabase mythDatabase;`

`[SerializeField] private int mythIndex = 0;`


When a vase is interacted with, it notifies the room controller and spawns a floating text panel positioned above the object, with the panel automatically rotating to face the player’s camera. 
This became a valuable UX principle and consideration, which originated from another round of trial and error when working with VR: information should orient itself to the user, not force the user to adjust.

Another central interaction was the Oracle of Delphi, who responds to the player after a short delay with a randomized answer in a Yes/No style (e.g. The gods look favourably upon you). 
While technically simple, this interaction focused on pacing and atmosphere rather than mechanical difficulty. Coroutines were used to create a sense of anticipation:

` yield return new WaitForSeconds(thinkingTime);`

`string answer = GenerateAnswer();`

`ShowDialogue(answer);`

This reinforced how timing, audio, and feedback play a major role in immersion, even when the interaction itself is minimal. I will admit to wanting to animate the Oracle at a later point, but alas, since it was not critical functionality, I found myself needing to prioritise other aspects of our VR experience.


To manage progression, each of us implemented a room controller that tracks user interactions. In my case, once the user has interacted with enough the vases and/or the oracle, the portal activates and prompts the user to go even further into the past, while still granting the choice to keep exploring the current world they are in:

`if (totalInteractions >= requiredInteractions && !isCompleted)`

`{
    StartCoroutine(CompleteRoom());
}`


This design choice allows players to explore the room freely rather than follow a fixed sequence, granting them more freedom and creating an experience fueled by curiosity rather than instructions.
Ultimately, this was also what we tried to achieve with *Forward to The Past*; while currently the experience feels more game-like, since we wanted to focus on the fun aspect, one of the main goals eventually is to add more educational value, in order to make learning a fun, engaging experience. Personally, learning is always most interesting when driven by curiosity, rather than another set of instructions.

### Learning Outcomes, Challenges, and Reflection

Developing a Virtual Reality experience highlighted the need for even higher demands on both technical precision and design decisions than in the development approaches we were already familiar with.
Since the user must be fully immersed, developers must ensure that their senses are convincingly engaged at all times, which made theoretical concepts such as spatial scale, locomotion and overall performance far more noticeable in practice. 
Even small inconsistencies in the scale, collider placements or interaction timings become immediately noticable when experienced through the headset, which can quickly break immersion or lead to discomfort.

Unlike desktop projects, where interaction and testing often happen in the editor with mouse and keyboard inputs, VR requires frequent testing on the actual hardware, as performance constraints, physical movement and user comfort can only be properly evaluated inside the headset. 
While we utilised the XR Interaction Toolkit to handle many standard interactions, e.g. grabbing and selecitng objects, this also meant that correct setup and consistency were crucial across all scenes, since small misconfigurations could easily cause unexpected behaviour. For example, duplicated XR Origins, mismatched interaction managers, or broken references could easily render a scene unusable, often without providing helpful error feedback.

This was also made prominent by another challenge, which came from coordinating our work across quite varied schedules. 
Since we were often unable to work on the project simultaneously, large parts of the development had to be done individually, which, as always, had both advantages and disadvantages.
While this did allow each team member to progress faster and focus on their own room and features more in-depth, it also led to differences in how code and assets were structured, how certain interactions, e.g. text visuals, were implemented, and how scenes were organised.
When these parts later had to be merged, the inconsistencies became more apparent and occassionally required additional time to resolve, especially in a VR project where structural consistency across scenes is particularly important.

I also now realise that we pushed the scope of the immersion aspect and model use in the project a bit further than was realistically managable with our experience. 
In our goal to create large, detailed, immersive environments, we ended up lacking time for more complex feature development, and creating extra difficulties in optimisation, merging and debugging.
While I am still mostly satisfied with our work, I also recognise the significance of still needing to learn how to balance creative ambition with technical feasibility and time constraints.

Despite some difficulties, the project clearly demonstrated how VR enables experiences that go beyond what is possible on a flat screen, creating a world of new possibilities for interaction, learning and immersion.
At the same time, it is interesting to imagine how the technology will evolve. Working with the current hardware highlighted areas where future improvements are likely to have a strong impact, such as more efficient rendering techniques, higher and more stable frame rates, improved display resolution, better optics etc. 
The reduction of discomfort would also allow developers to focus more on interaction and experience design rather than constant performance optimisation.

### Conclusion

*Forward to the Past* allowed me to learn VR application development, and gain a deeper theoretical understanding of XR technologies, interaction paradigms, and challenges specific to this domain. Additionally, it helped to improve my skills and familiarity with C#.
Some of the main takeaways will definitely be the importance of good structure and early alignment within the team, thorough planning and consistent merging, as well how crucial yet time-consuming it is to achieve true immersion, clarity, and comfort.

Although the project was demanding, it was vital in further developing my problem-solving skills in a context that required a different mindset, with a more spatial way of thinking than previous projects.
What is more, just like our AR project SARA, this opened up a new set of interests for me within video game and XR development, highlighting the creative freedom and potential that these technologies offer.
At the same time, it made me more aware of the practical constraints and responsibilities that come with designing immersive interactive experiences, and the careful balance required between ambition, technical feasibility, and user experience.
