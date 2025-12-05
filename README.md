# Fog Game

## Team Members:
- Nyle Huntley - Gameplay design and fog system GDExtension lead
- Hilbert Lee - UI, map design and battery system lead
- Sean Weston - Monster behavior and design lead

## GDExtension Description:
The DynamicFog2D node extends Godot's Node2D to render a simple, dynamic fog system. It draws a low resolution mask of black pixels (displayed as white with a shader active) onto a child Sprite2D node, covering the entire display as set in the parameters. Any Sprite2D in the specified light group acts as a "light" - it must have a white texture that is then rotated, scaled, and blended onto the fog image to erase the pixels beneath it. The black fog pixels gradually regenerate over time when not in contact with the lights, which carve out visible areas in real time. Editable parameters include display width and height, fog regeneration rate (how quickly the fog fade back in), and the name of the group containing the Sprite2D "light" nodes. Multiple Sprite2D light sources can be used simultaneously, though for the purposes of our game this was not used.

To have a visual fog effect rather than a flat white texture over the screen, a GDShader is applied to the child Sprite2D node used for displaying fog. The shader applies a user specified fog texture on top of the fog pixels and visually animates an endless scrolling effect. The shader also exposes properties for the fog scrolling direction and speed as well as visible opacity of the fog, meaning the fog can be fully opaque or just barely visible over the screen if desired.

## Features:
*In case of gif attachment issues, gifs will be permanently stored [here](Feature%20Gifs).*
### Fog and light system
Fog system with and without shader texture for demonstration - static green background also for demonstration.

![Fog with texture](https://github.com/nah353/IMG420-Final-Project/blob/0de1cacba204560f4aaab0366b9d112884b6abed/Feature%20Gifs/Fog%20with%20texture.gif) ![Fog without texture](https://github.com/nah353/IMG420-Final-Project/blob/0de1cacba204560f4aaab0366b9d112884b6abed/Feature%20Gifs/Fog%20without%20texture.gif)

### Flashlight battery scaling
As the battery level decreases, the flashlight's size also decreases. Battery pickups increase battery level and flashlight size. Sped up greatly for demonstration.

![Flashlight scaling](https://github.com/nah353/IMG420-Final-Project/blob/0de1cacba204560f4aaab0366b9d112884b6abed/Feature%20Gifs/Flashlight%20scaling.gif)

### Endless map generation
Camera zoomed out greatly and fog removed for demonstration. One map chunk is repeatedly generated ahead of the player as they move to create endless map effect.

![Map generation](https://github.com/nah353/IMG420-Final-Project/blob/0de1cacba204560f4aaab0366b9d112884b6abed/Feature%20Gifs/Map%20generation.gif)

### Battery spawning and arrow indicators


### Monster AI


## Installation, Build and Running Instructions
1. Clone this repository
2. Enter IMG420-Final-Project directory in command line
3. Run "git submodule update --init" to retrieve godot-cpp file content
4. Run "scons" (with other parameters if necessary) to build gdextension
5. Open project in Godot or relaunch to use DynamicFog2D node properly
6. Run the project through 'Run Project' in the Godot editor or press F5 to start the main menu scene, or run that specific scene for intended game flow

## Controls and Gameplay Instructions
Use WASD or arrow keys to move the player around and use your mouse to aim the flashlight. Try to survive as long as possible while the AI enemies slowly stalk you, but be careful not to anger them by pointing your flashlight for too long. Use the arrow indicators to find your way to the next battery and recharge your flashlight before its battery runs out and you're left defenseless.

## Known Issues and Future Improvements
- Sprite2D light textures are scaled up 4x to reduce resolution and greatly improve performance of fog, so light sprites are an inaccurate visualization of the fog cutout effect they have.
- Light sprites must have their position updated directly rather than as children of some other node such as the player to accurately affect the fog.

## Demo Video
[Watch the demo on YouTube](https://youtu.be/B7XJEbcCctE)
