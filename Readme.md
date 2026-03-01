# Architectural Patterns

Patterns observed across multiple scripts in `Assets/Scripts/`.

---

## 1. MonoBehaviour Component Composition

All scripts inherit `MonoBehaviour` and attach to GameObjects. Cross-object references are resolved at `Start()` via `GetComponent<T>()` rather than singletons or dependency injection frameworks.

- `PlayerMovement.cs:14–17` — caches `rb`, `anim`, `sr` via GetComponent at Start
- `PlayerLife.cs:9–11` — same pattern for Animator and Rigidbody2D
- `GpaCollector.cs:9` — gets Text component reference

## 2. Serialized Field Inspector Wiring

Dependencies are injected through Unity's inspector using `[SerializeField]` private fields. This keeps fields encapsulated while allowing editor-time assignment — the project's primary dependency mechanism.

- `PlayerMovement.cs:6–11` — jumpableGround LayerMask, sounds, joystick reference
- `GpaCollector.cs:6–7` — gpaText Text reference
- `cameraController.cs:5–6` — player Transform reference
- `PointFollower.cs:5–7` — points array, speed

## 3. Physics-Based Movement (Rigidbody2D Velocity)

Movement is implemented by directly setting `Rigidbody2D.velocity` each frame, not by applying forces. Jump is a one-shot Y-velocity assignment guarded by a ground check.

- `PlayerMovement.cs:35–42` — horizontal: `rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y)`
- `PlayerMovement.cs:44–48` — jump: `rb.velocity = new Vector2(rb.velocity.x, jumpForce)`
- `PlayerLife.cs:21` — death: `rb.bodyType = RigidbodyType2D.Static` to freeze

Ground detection uses `Physics2D.BoxCast` downward against a `jumpableGround` LayerMask (`PlayerMovement.cs:67–71`).

## 4. Trigger/Collision Event Pattern

Game logic is triggered through Unity physics callbacks, not polling. The pattern is consistent: check tag → act → (optionally) destroy.

**OnTriggerEnter2D** (non-solid zones):
- `GpaCollector.cs:14–20` — `CompareTag("GPA")` → increment score, Destroy item, update UI
- `Finish.cs:8–11` — `CompareTag("Player")` → Invoke delayed level load
- `StickyPlatform.cs:7–9` — parent player to platform transform

**OnCollisionEnter2D** (solid contacts):
- `PlayerLife.cs:14–20` — `CompareTag("Trap")` or `"Water"` → trigger death sequence

## 5. Animation State Machine (Enum-to-Integer)

Player animation uses a C# enum mapped to an Animator integer parameter. One `UpdateAnimationState()` method owns all transitions, called each Update.

- `PlayerMovement.cs:18` — `private enum MovementState { idle, running, jumping, falling }`
- `PlayerMovement.cs:57–83` — state logic based on velocity and grounded status
- `PlayerMovement.cs:84` — `anim.SetInteger("state", (int)state)`

The Animator Controller at `Assets/Animations/Player.controller` mirrors this with integer-driven transitions.

## 6. Scene Management

All scene navigation uses `UnityEngine.SceneManagement.SceneManager`. Two patterns are used:

- **Sequential:** `SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1)` — used in `Finish.cs:15` and `PlayerLife.cs:28`
- **Named:** `SceneManager.LoadScene("Start Screen")` — used in `PauseMenu.cs` and `startMenu.cs`

## 7. Delayed Invocation via `Invoke()`

Actions that need a timed delay (e.g., wait for animation to finish) use `Invoke(methodName, seconds)` rather than coroutines.

- `Finish.cs:11` — `Invoke("CompleteLevel", 2f)` after player reaches finish
- `PlayerLife.cs:22` — `Invoke("RestartLevel", 1f)` after death animation

## 8. Waypoint Path Following

Moving platforms follow an ordered array of Transform waypoints using `Vector2.MoveTowards`. On reaching a point, the index cycles (with reversal logic).

- `PointFollower.cs:5–26` — `points[]` array, `currentPoint` index, `MoveTowards` in Update

## 9. Transform Parenting for Platform Riding

StickyPlatform solves the "player slides off moving platform" problem by reparenting the player's transform to the platform on enter and null on exit.

- `StickyPlatform.cs:7–14` — `SetParent(transform)` / `SetParent(null)`

## 10. Global Pause State

Pause is implemented via `Time.timeScale = 0/1` combined with a `public static bool Paused` flag. No event system is used; other scripts can read the static directly.

- `PauseMenu.cs:5` — `public static bool Paused = false`
- `PauseMenu.cs:16–24` — toggle timeScale and bool, show/hide pause UI panel

## 11. Audio Playback

Audio sources are serialized references played on-demand with `.Play()`. No audio manager singleton; each script owns the sources it needs.

- `PlayerMovement.cs:9–10` — `jumpSound`, `walkSound` AudioSource fields
- `PlayerMovement.cs:47,49` — `jumpSound.Play()` on jump
- `SettingsMenu.cs` — `AudioMixer.SetFloat()` for global volume from UI slider
