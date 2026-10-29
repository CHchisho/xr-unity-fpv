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
