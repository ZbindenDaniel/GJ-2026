
# AGENT instructions

## Project Overview

This is a Unity game jam project. Details on can be found int [OVERVIEW](OVERVIEW.md). 
Current progress is tracked in [PLANNING](PLANNING.md)

Constraints:

* Mechanics are not fully defined yet.
* Development speed > architectural elegance.
* Code must be understandable and easy to modify under time pressure.
* No overengineering.

Primary goal:
Deliver a playable, stable prototype quickly and iterate.

---

## Core Principles

### 1. Simplicity Over Purity

* Prefer readable MonoBehaviours over complex abstraction layers.
* Avoid premature architecture (no heavy patterns unless clearly needed).
* Avoid deep inheritance hierarchies.
* Composition > abstraction.

### 2. Fast Iteration

* Code should be easy to tweak.
* Public serialized fields for quick inspector tuning.
* Avoid magic numbers in code.
* Use `[SerializeField]` instead of hardcoded constants where balancing might change.

### 3. Contained Complexity

* One script = one clear responsibility.
* Avoid 500-line mega scripts.
* If a file grows beyond ~300 lines, consider splitting it logically.

### 4. Playable > Perfect

* If a system works reliably, do not refactor it mid-jam unless it blocks progress.
* Avoid risky refactors close to deadline.

---

## Technical Guidelines

### Unity Version

Use a stable LTS version unless the jam requires otherwise.

### Project Structure

Keep folder structure minimal:

```
Assets/
 ├── Scripts/
 │    ├── Core/
 │    ├── Player/
 │    ├── Enemies/
 │    ├── Systems/
 │    └── UI/
 ├── Prefabs/
 ├── Scenes/
 ├── Art/
 └── Audio/
```

Do not create deep nested folder trees.

---

## Code Style Guidelines

### Naming

* PascalCase for classes and methods.
* camelCase for private fields.
* `_camelCase` for private serialized fields.
* Clear names over short names.

Example:

```csharp
[SerializeField] private float moveSpeed = 5f;
private Rigidbody rb;
```

---

### MonoBehaviour Pattern

Keep scripts straightforward:

```csharp
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
```

Avoid over-abstracting input systems unless necessary.

---

## 2D vs 3D Flexibility

Since direction is unknown:

* Avoid hardcoding `Vector2` unless clearly 2D.
* Prefer `Vector3` for movement logic unless confirmed 2D.
* Separate movement logic from camera logic.
* Avoid physics-specific assumptions early (Rigidbody vs Rigidbody2D).

If switching between 2D and 3D:

* Abstract movement input gathering.
* Keep physics calls localized in one place.

---

## Systems Philosophy

### Input

* Use Unity Input System only if already familiar.
* Otherwise use classic Input Manager for speed.
* Wrap input reading in small methods to allow quick swapping.

### Game State

Avoid complex state machines.

If needed, use a simple enum:

```csharp
public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}
```

Keep state management centralized in one `GameManager`.

---

### GameManager Rules

* Only one GameManager.
* Handles:

  * Game state
  * Restart logic
  * Score tracking (if needed)
* Avoid turning it into a god-object.

---

## Prefab Guidelines

* Prefabs must work immediately when dropped into a scene.
* No hidden dependencies.
* Required references should be serialized fields.
* Avoid runtime `FindObjectOfType` unless jam time pressure demands it.

---

## Debugging Strategy

* Use `Debug.Log` freely during development.
* Remove excessive logs before submission.
* Prefer quick visual debugging (Gizmos, debug UI).

---

## Performance Rules

For a jam:

* Do not prematurely optimize.
* Avoid obvious performance killers (nested Find calls in Update).
* Object pooling only if spawning is heavy and clearly problematic.

---

## Scope Control

If a feature:

* Takes more than 2–3 hours to implement
* Requires architectural redesign
* Adds new complex systems

It should be simplified or cut.

---

## Decision Making Framework

When unsure:

1. Is it necessary for core gameplay?
2. Is there a simpler version?
3. Can we fake it visually instead of engineering it?
4. Can we hardcode it temporarily?

Prefer hacks that are readable over clever systems.

---

## AI Behavior (for Codex Agent)

When generating code:

* Prefer simple MonoBehaviour-based solutions.
* Avoid design patterns unless explicitly requested.
* Avoid dependency injection frameworks.
* Avoid generic-heavy abstractions.
* Keep functions short and readable.
* Include comments only where logic is not obvious.
* Do not introduce external packages unless explicitly required.
* Assume single-developer workflow unless stated otherwise.

When unsure about 2D vs 3D:

* Write logic in a way that can be easily adapted.
* Highlight small adjustment points in comments.

---

## End-of-Jam Stabilization

Before submission:

* Remove obvious debug logs.
* Test restart flow.
* Test build once.
* Check null references.
* Verify all scenes are added to Build Settings.

No refactors on final day unless critical bug.

---

## Final Reminder

This is a game jam project.

Clarity > cleverness
Playable > scalable
Finished > perfect
