# Planning Notes

## Current Constraints & Decisions
- **Game type:** 3D only.
- **Scenes:** Single gameplay scene; "levels" swap props/objects in the same scene.
- **Masks:** Hardcoded attributes per mask (manually set in inspector). Start with enum fields for `shape`, `eyes`, `mouth`, `color`.
- **Mask selection:** Raycast-based interaction with a configurable max distance + keypress.
- **Room reaction:** On entry, music/NPCs pause briefly (configurable), then react.
- **Aggressive NPC behavior:** Not defined yet.
- **Elevator doors:** Local coordinate movement; prefab-based.
- **Fail/reset:** Full reset to initial state (subject to change later).

## Open Questions (Need Design Clarification)
1. **Aggressive reaction definition:** Emote only or movement toward player? Does it block progress or trigger fail?
2. **Mask reaction rules:** Exact mapping between mask attributes and NPC reactions/hints.
3. **Room entry timing:** What constitutes "player enters room" (trigger volume? door crossing?) and how long is the pause before NPC reactions?
4. **Hint delivery:** How do NPCs convey hints (look direction, animations, UI indicators)?

## Data Structures (Keep Minimal & Editable)
- **MaskAttributes** (class or struct) with enum fields for `Shape`, `Eyes`, `Mouth`, `Color`.
  - Values are set per mask prefab or component in the inspector.
  - Avoid global registries unless needed.
- **MaskDefinition** (MonoBehaviour) to hold `MaskAttributes` + references (mesh/material), if needed.

## Proposed Work (Draft Tasks)
> Note: Each task includes a reason + higher-level goal, and keeps code additions minimal.

### 1) Add MaskAttributes enums + MaskDefinition component
- **Reason:** We need a consistent, inspectable data model for mask selection and NPC evaluation.
- **Higher-level goal:** Enable fast iteration on mask design without new systems.
- **Details:** Add a small MonoBehaviour with enums and serialized fields; no registries or managers yet.
- **Data structure check:** Ensure enums are explicitly named and can be extended without breaking serialized data.

### 2) Raycast-based mask selection on Player
- **Reason:** Player must select masks in the elevator quickly via look + keypress.
- **Higher-level goal:** Establish the core interaction loop with minimal UI overhead.
- **Details:** Add a simple raycast in player controller (or a small new component) with max distance; use a LayerMask for masks.
- **Logging/try-catch:** Log selection changes; guard raycast hits and input reads.

### 3) Room entry trigger and reaction timing
- **Reason:** We need a deterministic moment to pause audio/NPCs and then react.
- **Higher-level goal:** Make the room flow feel readable and consistent.
- **Details:** Add a trigger volume (or existing doorway collider) that starts a timer; after delay, notify NPCManager.
- **Logging/try-catch:** Log room entry and reaction start; guard missing references.

### 4) NPC reaction state stub
- **Reason:** We need a placeholder for idle/happy/aggressive behaviors even before final logic.
- **Higher-level goal:** Enable designers to test the loop visually.
- **Details:** Add a simple enum state on NPC; switch animations or placeholder Debug logs.
- **Logging/try-catch:** Log state changes; avoid throwing if animator is missing.

### 5) Reset flow baseline
- **Reason:** Full reset is required on bad mask; needs a clear start state.
- **Higher-level goal:** Guarantee a stable loop for playtesting.
- **Details:** Define reset entry point in GameControl; reset player position, mask selection, and NPC state.
- **Logging/try-catch:** Log reset events; guard missing components.

## Implementation Notes
- Prefer serialized fields for easy tuning.
- Avoid extra managers unless absolutely needed.
- Keep MonoBehaviours single-responsibility and under ~300 lines.
- Add logging + try/catch where meaningful (scene loads, manager init, critical interactions).
