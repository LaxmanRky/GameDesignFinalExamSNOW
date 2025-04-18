# Timeline-Based Spawning System

This document explains how to set up and use the Timeline-based spawning system for falling objects in your game.

## Overview

The system uses Unity's Timeline feature to precisely control when and where objects spawn. This gives you complete control over the pacing and difficulty of your game.

## Setup Instructions

### 1. Configure Your Prefabs

- Make sure all your falling object prefabs (snow, asteroids, etc.) have the `FallingObjectController` component attached
- Configure each prefab with appropriate settings:
  - Set the `objectTag` to identify the type of object ("Snow", "Debris", etc.)
  - Adjust `fallSpeed`, `rotationSpeed`, and other parameters as needed

### 2. Set Up Spawn Points

- Make sure you have a GameObject named "SpawnPoints" with child objects representing each spawn point
- Position these spawn points where you want objects to appear (typically at the top of the screen)

### 3. Configure the FallingObjectSpawner

- Add the `FallingObjectSpawner` component to your TimelineManager GameObject
- Drag your spawn points into the "Spawn Points" list
- Add all your falling object prefabs to the "Falling Object Prefabs" list
- Note the index of each prefab (0, 1, 2, etc.) as you'll need this for Timeline signals

### 4. Set Up Timeline

1. Add a Playable Director component to your TimelineManager GameObject
2. Create a new Timeline asset and assign it to the Playable Director
3. Add Signal Tracks to your Timeline
4. Add Signal Emitters at specific time points
5. For each Signal Emitter:
   - Create a Signal Asset if prompted
   - Click "Add Receiver" and select your TimelineManager GameObject
   - Choose one of these methods:
     - `FallingObjectSpawner.SpawnObject(prefabIndex, spawnPointIndex)`
     - `FallingObjectSpawner.SpawnObjectRandom(prefabIndex)`
     - `FallingObjectSpawner.SpawnRandomObject(spawnPointIndex)`
     - `FallingObjectSpawner.SpawnRandomObjectAtRandomPoint()`

## Using the System

### Spawning Specific Objects

To spawn a specific prefab at a specific spawn point:
- Use `SpawnObject(prefabIndex, spawnPointIndex)`
- Example: `SpawnObject(0, 2)` spawns the first prefab at the third spawn point

### Random Spawning

For more variety, you can use:
- `SpawnObjectRandom(prefabIndex)` - Specific object at random location
- `SpawnRandomObject(spawnPointIndex)` - Random object at specific location
- `SpawnRandomObjectAtRandomPoint()` - Completely random

### Creating Patterns

- Place signal emitters at regular intervals for rhythmic patterns
- Use different spawn points to create interesting spatial patterns
- Mix different types of objects for gameplay variety

## Troubleshooting

If signals show "None (Signal Receiver)" in Timeline:
1. Make sure the FallingObjectSpawner component is on the same GameObject as your PlayableDirector
2. For each signal emitter, click "Add Receiver" in the Inspector
3. Select your GameObject with the FallingObjectSpawner
4. Choose one of the spawn methods from the dropdown
5. Add the required parameters (prefab index and/or spawn point index)

## Example Timeline Setup

1. Create a Signal Track named "Regular Spawns"
2. Add Signal Emitters every 2 seconds
3. Connect each to `SpawnRandomObjectAtRandomPoint()`
4. Create another Signal Track named "Special Spawns"
5. Add Signal Emitters at key moments
6. Connect these to `SpawnObject(0, 0)` to spawn specific objects at specific locations
