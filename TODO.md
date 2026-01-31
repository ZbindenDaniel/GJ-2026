
# Todo

## Programming

### GameControl (Dani & Tobi)

- master control: sync and control the manager classes. This could be done using an event system or some sort of state machine
- progress tracking

The flow of the game is like this:
- Player starts inside the elevator 
- elevator sounds play, background music gets louder and louder
- elevator arrives at destination, door opens
- Player can turn around inside the elevator and have a glimps outside.
- player can then select a mask from inside the elevator and then leaves it
- NPCs react to the player (head movements, noises)
- based on the mask and the players movements the NPC either guide them, ignore the or push them back into the elevator.

### EnvironmentManager (Tobi)

this class manages the environment in the scene

- masken laden: load a selection of masks into the elevator
- Räume/props laden
- NPCs laden

### PlayerManager (Tobi)

- walking, looking around
- mask selection
- (emotes)
- player effects: player gets drunk which effects steering (done by creating a fluctuating inertia on the player body)

### NPCManager (Dani)

- body animations
- face animations (schauen, hints)

### ElevatorManager (Dani, bugs: Tobi)

- door animations
- triggering sound


### UiManager (Tobi)

- start game. startscreen
- pause menu: exit, restart

## Design (Alex)

- Masken
- NPCs
- rooms
- elevator

## Sounds

- Elevator effects
- People noises (walking, party sounds, emotes)
- Party Music
