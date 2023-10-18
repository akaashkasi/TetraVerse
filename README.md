# Project6

Name | Main Role | Responsibilities 
--- | --- | ---
Madhavi | Networking, Agile Manager | Work heavily on back end: networking (coding) for game components, Unity asset management
Evan | Sprint Release, Testing, Environment | Managing the sprint release, project presentation, environment and sound, play testing
Akaash | Game Design and Creation | backend game logic (tetris blocks- grab interactions), point system, tech support for team
Tian | Github Manager, Locomotion system, UI | Locomotion system coding, UI design, managing the spring release (sprint branch)

Sprint 3 Submission 

Build/Playtest Instructions

- Clone the branch labeled "sprint-3".

- Open the GameRoom scene from the Scenes folder and make sure SnapManager is unselected.

- Go to build settings and select only the "GameRoom," to build and run. This will then open into the game room. 

Documentation Video Link

https://clipchamp.com/watch/qK4mnLjqFmg

Individual Member Contributions

- Evan: Wrote/edited Sprint submission on the README document, added background audio to the game room, added sound effects for when the blocks collide with the floor, and created the voice-over for the demonstration video.

- Madhavi: Added the "3 2 1" blocks that spawn to signal the beginning of the game, implemented the "levels" in the game that directly result in the blocks falling at different speeds, worked on basic block snapping mechanics, and created the demonstration video.

- Tian: Added the jump functionality and corrected various bugs such as blocks breaking the game room and the player being able to move outside of the game room.

- Akaash: Added a glow functionality to the blocks when grabbed, assisted in the correction of the bug involving the block being able to go through the side walls, and enabled the detection of a complete row to then delete or remove a set of blocks on the grid.

Additions

- A vertical jumping system has been implemented within the game and it is binded to the trigger buttons on the Oculus controllers.

- A "3,2,1" countdown sequence has been added to the game and it is utilized to signal the spawning of blocks within the game room.

- Text indicating the current level of the game is now displayed on the wall.

- Background music that continuously plays during gameplay has been added.

- A sound effect indicating the collision of a block with the floor of the game room has been added.

- A basic block snapping system has been added that allows the blocks to align themselves with the grid spaces on the floor.

- A sound effect indicating that a block has been grabbed by a player has been implemented.

Bug Fixes

- Falling blocks that are too close to the walls no longer cause the game room to fall apart as a result.

- Player hands can no longer go through the walls of the game room.

- Players can no longer utilize the continuous motion system to move outside of the game room.

- Blocks can no longer go through the walls of the game room.

Unity Issues

- We were not able to determine how the adjust the lighting in the room in order to prevent the walls from appearing different.

- We attempted to rectify this issue by having Unity precompute the lighting data automatically, but that method didn't work.

- In the next Sprint, we hope to have this lighting issue solved by editing the properties of walls and having them replicate a material that doesn't produce as many shadows.

- Block snapping is slightly buggy and will be adjusted thoroughly during the next Sprint.
