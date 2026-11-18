# 3D First Person View

Scene: `Assets/Scenes/FirstPersonCourse.unity`.

## Assignment

- Player object with a Rigidbody (Interpolate, Continuous, drag), a capsule model, and `Orientation` / `CameraPosition` children.
- Mouse look on the main camera (`CameraController`) and WASD movement relative to facing (`PlayerController`, speed 50).
- Extra scene objects: walls, crates, and trees, each with its own material.

## Added

- Jump (Space) while grounded.
- A border of solid walls so the player cannot fall off the plane.
- Five crystals to collect. They spin and disappear on touch.
- A golden beacon that finishes the leve only after every crystal is collected. The beacon stays locked otherwise.
- A sliding gate that blocks the path and can be jumped over.
- An on-screen counter and status text.
- Escape releases the cursor; a mouse click locks it again.

## Added for V2

- The photogrammetry sneaker turns while you look at it, so the scan can be inspected from all sides.
- The center reticle turns blue on an info hotspot and gold on a teleport portal.
- Escape releases the cursor; a mouse click locks it again.

## VR bowling

Scene: `Assets/Scenes/BowlingAlley.unity`.

- Left stick walks, right stick snap-turns. Point and press the trigger to teleport onto the approach or the side decks.
- Grip grabs the ball. Let go to throw it. Knocked pins buzz the controllers.
- The red button resets the pins and the ball. The scoreboard counts how many pins have fallen.
- The pin, ball, and alley are low-poly models made for this scene.
