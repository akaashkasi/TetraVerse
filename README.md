# Project6

Name | Main Role | Responsibilities 
--- | --- | ---
Madhavi | Networking, Agile Manager | Work heavily on back end: networking (coding) for game components, Unity asset management
Evan | Sprint Release, Testing, Environment | Managing the sprint release, project presentation, environment and sound, play testing
Akaash | Game Design and Creation | backend game logic (tetris blocks- grab interactions), point system, tech support for team
Tian | Github Manager, Locomotion system, UI | Locomotion system coding, UI design, managing the spring release (sprint branch)

Sprint 4 Submission 

Usability Testing Instructions

- Clone the branch labeled Sprint-4.

- The "3 2 1" countdown blocks spawn to indicate the start of the game. 

- At the end of the countdown, blocks will fall from the ceiling of the game room and from there you can be begin searching for blocks/positioning yourself in a spac to grab them. 

- You can move around the room using the joysticks and blocks can be rotated by using one or both of your hands. 

- Once the block touches the floor, it will snap to the nearest position and this will prevent further repositioning and rotation attempts.

- The X button on the left controller can be utilized to jump up instead of waiting for the block to fall naturally.

- You will receive five points if you simply allow the block to fall onto the group and it snaps successfully. 

- If you grab the block and rotate it as you find necessary before placing it, you will be awarded with ten points. (Unfortunately, this aspect of the game has an error at the moment and will not occur as intended, but this will be rectified in the final sprint)

Documentation Video Link

- https://www.kapwing.com/videos/654466c6f693277ec87cbcf2

Individual Member Contributions

- Evan: Wrote/edited Sprint submission on the README document, found and provided the original code for the outlines on the blocks, found and adjusted the speed of the audio used throughout the game to indicate "level" change, created the demonstration video for the sprint, and created the voice-over for the demonstration video.

- Madhavi: Debugged the snapping system more, set the  behavior for unsuccessful snapping when trying to occupy same position as another block or outside the bounds of room, worked on tracking for occupied squares on floor grid via GridManager class which assists in the clearing of a layer, added audio to indicate "level" change within the game, created a wall around the "3 2 1" countdown blocks, added a visual indication of the blocks being frozen/snapped through a change in color, and performing networking fixes with Tian.

- Tian: Worked on creating the point system that allocates points for a user based on snapping success and interaction, worked on an initial version of the GridManager, worked on the creation of a point allocation system point based on collaboration with other users (work in progress), and performed networking fixes (work in progress). Work in progress items are not shown in this sprint but can be seen on Tian's branch. 

- Akaash: Worked on GridManager logic and glow manager, worked with Madhavi to develop the logic of what happens when a layer is completed, used hashmap and tuples to map out each layer of the grid to check to understand what layer is filled to clear the layer and if a layer can in the future be cleared, and in process of implementing a bloom post-processing system for the block to glow when it is grabbed.

Additions

- Audio with various speeds now plays throughout the game in order to indicate a "level" change for the players.

- A wall has been added around the "3 2 1" countdown blocks to prevent user interaction and collision with the blocks.

- A visual indication of the blocks being frozen/snapped can now be seen through a change in color. (A lighter shade of the original color of the block)

- An outline system has been implemented that allows the user to see the individual subsections of each block as they fall and snap into  place.

- A scoring system has been implemented that allows the user to keep track of how many points they have gathered throughout the game.

Bug Fixes

- The snapping system has been corrected for the most part and now block transform positions can be properly associated with an occupied position on the floor. (This was the biggest issue that we fixed)

- Snapping is no longer marked successful when two blocks attempt to occupy the same space or when blocks go outside the bounds of the game room.

- The user can no longer collide or interfere with the "3 2 1" countdown blocks while they fall.

- The game no longer glitches and causes multiple blocks to fall at the same time.

Final Sprint Goals

- We intend to find a way to empty/clear a layer of blocks once the requirements are met by the user(s).

- We plan to patch several networking issues throughout the game.

- We aim to finish the revised point allocation system that operates based on collaboration with other users.

- We will find a solution to the multiuser client issue where strange behaviors occur within the game from the perspective of anyone except the master client.

- We wish to adjust the height of the character collider in order to make game feel more natural from the perspective of the user.

- We intend to adjust the speed at which the blocks fall down in all 3 levels in order to indicate a difficulty shift.

- We plan to determine how the game will properly end.

Issues/Current Errors (Fixing these issues will be our primary focus for the final sprint as most other aspects of our game are completed)

- Our game is not completely networked as we were focused on game logic during this sprint, but we will finish the networking system as we approach the final sprint. (Initial progress can be seen on Tian's branch where the "3 2 1" block's spawn and dissolve feature has been networked)

- When more than 1 player joins the game, we have issues with the way the block is seen by anyone except the master client. Additional clients within the game may see odd behaviors such as the blocks appearing "jumpy" as they fall down.

- When a user interacts with a block and then places it on the ground, the snapping is marked as "unsuccessful" and points aren't awarded even though it is technically successful. We will also ensure that this is fixed for the final sprint.
