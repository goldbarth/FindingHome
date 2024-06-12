# Finding Home
### *2D Platformer / Prototype / Unity*

My first Unity project.
A 2D platformer prototype with a focus on simple player movement and controls.
Story: The game is about a young girl who is lost in a mysterious world and tries to find her way back home.

##### Load/Save System

You can save and load your progress in the game in the pause menu.
The progress is visible in the load menu,
where you can see the collected level coins percentage and the death count.

It automatically saves the game when you quit the game, or it crashes.

It is possible to have 4 save slots, which you can reach over the load menu.
You can overwrite a save slot by starting a new game and selecting the save slot you want to overwrite.


The save files are stored in the `C:\Users\[Username]\AppData\LocalLow\Goldbarth Prod\Finding Home` folder.


##### Controls

- `A`/`D`: Move left/right
- `A`/`D` + hold `Shift`: Run
- `W`/`S`: Climb up/down (only in later levels. levels not ready implemented yet.)
- `Space`: Jump
- `Shift`: Dash (only in later levels)
- `E`: Interact
- `ESC`: Pause

##### Features

- Player Controller
	- Jump
		- Multijump
		- Walljump (only in later levels)
	- Walk/Run
	- Wallgrab (only in later levels)
	- Dash (only in later levels)
	
- Menu System
	- Main
	- Options
	- Credits
	- Save/Load
    - Pause
	
- Load/Save System
- Mono-/(NPC)Dialogue 
- Mini Puzzle
- Obstacles/Traps
- Collectible (Coins)
- Szene Animation