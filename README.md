# Project6

Name | Main Role | Responsibilities 
--- | --- | ---
Madhavi | Networking, Agile Manager | Work heavily on back end: networking (coding) for game components, Unity asset management
Evan | Sprint Release, Testing, Environment | Managing the sprint release, project presentation, environment and sound, play testing
Akaash | Game Design and Creation | backend game logic (tetris blocks- grab interactions), point system, tech support for team
Tian | Github Manager, Locomotion system, UI | Locomotion system coding, UI design, managing the spring release (sprint branch)

Sprint 5 Submission 

Usability Testing Instructions

- Clone the branch labeled Sprint-5.

- The "3 2 1" countdown blocks spawn to indicate the start of the game.

- At the end of the countdown, blocks will fall from the ceiling of the game room and from there you can be begin searching for blocks/positioning yourself in a space to grab them.

- You can move around the room using the joysticks and blocks can be rotated by using one or both of your hands.

- Once the block touches the floor, it will snap to the nearest position and this will prevent further repositioning and rotation attempts.

- The X button on the left controller can be utilized to jump up instead of waiting for the block to fall naturally.

- The player will receive five points if they simply allow the block to fall onto the group and it snaps successfully.

- If the player grabs the block and rotates it as they find necessary before placing it, they will be awarded with ten points.

Demonstration Video

- https://www.kapwing.com/videos/6556fcc340222bfe5fd11d26

Individual Member Contributions

Evan: Assisted in writing/editing the Sprint submission on the README document, created the voice-over for the demonstration video, and performed user testing.

Madhavi: Aided in implementing RPC callbacks for all parts of the game, made fixes that allowed the game to better operate as a single-player game, partially solved the issue where blocks refuse to snap correctly due to user interaction, added the parent’s transform position with the child’s initial local position in order to allow blocks to properly snap, assisted in writing/editing the Sprint submission on the README document, and assisted in the creation of the Sprint submission on the README document.

Tian: Aided in implementing RPC callbacks for all parts of the game, developed the end-game logic, resolved the issue where players were able to peek through wall gaps, created a script that stops spawning new blocks when the point maximum is reached, created the demonstration video for the Sprint, and implemented a script that allows for the erasue of layers once all grid spaces are occupied by blocks.

Akaash: Fixed bugs regarding glow (Bloom, Outline, Material options) and helped with developing code that allows for the clearing of blocks when they are all snapped to the floor.

Feature Additions

- Additional points are now awarded for user interaction. (Madhavi)

- Blocks will now stop spawning once the point maximum of 600 is reached. (Tian)

- Layers are now deleted once all grid spaces are occupied by blocks. (Tian/Akaash)

- Blocks can now properly snap onto the floor in accordance with the grid. (Temporary addition that is being adjusted by Madhavi)

Sprint 5 Bug Fixes

- Blocks are now able to snap to floor after being interacted with by user. (Madhavi)

- Users can no longer peek outside the walls of the game room. (Tian)

- Bugs regarding glow such as bloom, outlines, and material options have been resolved. (Akaash)

Post-Thanksgiving Bug Resolutions

- Correct grid positions need to be marked as occupied to ensure that e.g. 2 blocks don’t occupy same square and to identify when the entire floor has been filled and the level can be cleared. 

- Currently, you will see that blocks sometimes occupy the same spot because we’re not able to identify exactly which squares are empty vs which ones are not.

- Child block rotations are modified in an unexpected way for some reason. Madhavi has been testing ways in which the block can be reconstructed to its intended rotation and position after it has been selected by a user. The blocks can then simply retrieve the transforms of each of the children to identify occupied squares on the floor upon collision with the floor. (The debugText.text modification statements in BlockSnapGrab.cs can be uncommented to better understand this)

Post-Thanksgiving Testing

- The speed at which the blocks fall in each of the three levels. Currently, our game is programmed to decrease the linear drag (speed up the pace at which the blocks fall) and reduce the time interval between subsequent blocks falling at each of the levels. We want to make sure these are comfortable from a user POV.

- User testing of the velocity of a jump. We want to fine-tune the velocity of the jump both while going up and falling back down.
Post bug-fix, thorough testing of clearing a layer.

Additional Note

- We are not going to do networking for Tetraverse because of the issues that we've had so far with its implementation. We have discussed this issue with the professors and it seems that a lack of networking features is fine.
