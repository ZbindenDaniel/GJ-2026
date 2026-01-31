
## npcs

- Will have different masks
- each mask has some attributes
- Mask and number of players depends on the level (higher the level, the more players / different masks)
- When entering the level, the new npcs will be spawned, when leaving, they will be removed. Entering a new room, will generate them.
- You are leaving in elevaters -> after opening elevator, new npcs are already there.
- there is a max amount of players

## elevator directions

- There are 4 elevators.
- The elevator the player is currently in is always direction +2.
- The other elevators are randomly assigned -1 (down), 0 (same room), or +1 (up) by the LevelDesigner.
- LevelDesigner also tracks which elevator the player starts in (index).
