# Fog Game

## Team Members:
- Nyle Huntley - Gameplay design and fog system GDExtension lead
- Sean Weston - Monster behavior and design lead
- Hilbert Lee - UI, map design and battery system lead

## GDExtension Description:
The DynamicFog2D node extends Godot's Node2D to render a simple, dynamic fog system. It draws a low resolution mask of black pixels (displayed as white with a shader active) onto a child Sprite2D node, covering the entire display as set in the parameters. Any Sprite2D in the specified light group acts as a "light" - it must have a white texture that is then rotated, scaled, and blended onto the fog image to erase the pixels beneath it. The black fog pixels gradually regenerate over time when not in contact with the lights, which carve out visible areas in real time. Editable parameters include display width and height, fog regeneration rate (how quickly the fog fade back in), and the name of the group containing the Sprite2D "light" nodes. Multiple Sprite2D light sources can be used simultaneously, though for the purposes of our game this was not used.

To have a visual fog effect rather than a flat white texture over the screen, a GDShader is applied to the child Sprite2D node used for displaying fog. The shader applies a user specified fog texture on top of the fog pixels and visually animates an endless scrolling effect. The shader also exposes properties for the fog scrolling direction and speed as well as visible opacity of the fog, meaning the fog can be fully opaque or just barely visible over the screen if desired.

## Features:
### Fog and light system
Fog system with and without shader texture for demonstration - static green background also for demonstration.
![Fog with texture](https://github.com/user-attachments/assets/93441090-bffe-4b6d-8111-9b199c4e5308) ![Fog without texture](https://github.com/user-attachments/assets/8d541add-c86a-4fbc-8a4e-e1f2886b6086)

### Flashlight battery scaling
As the battery level decreases, the flashlight's size also decreases. Battery pickups increase battery level and flashlight size. Sped up greatly for demonstration.


### Endless map generation

### Battery spawning and arrow indicators


### Monster AI


To run and compile project:

1. Enter IMG420-Final-Project directory in command line
2. Run "git submodule update --init" to retrieve godot-cpp file content
3. Run "scons" (with other parameters if necessary) to build gdextension
4. Open project in Godot or relaunch to use DynamicFog2D node properly
