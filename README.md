# OrbitoonZ

A 2D arcade space shooter, and my first complete game in Unity.

**▶ [Play it in your browser on itch.io](https://antreask12.itch.io/orbitoonz)** (WebGL, no download)

---

## The game

Survive waves of asteroids and enemy spaceship formations while keeping an eye on the bombs. Bombs grow larger every second, and if any of them reaches full size, it detonates and the game is over. Each one takes 30 hits to destroy, so you constantly have to choose between dodging what's coming at you and stopping the next bomb before it's too late.

### Controls

| Action | Input |
| :--- | :--- |
| Move | `WASD` / Arrow keys |
| Shoot | Left mouse button (one shot per click) |
| Restart after game over | `R` |

### Features

- **Escalating waves:** each asteroid wave spawns more enemies than the last.
- **Spaceship formations:** ships spawn in randomly generated grid patterns.
- **Growing bombs:** a pressure mechanic that forces you to split your attention.
- **Power-ups**
  - *Triple shot* (30 s). Pick up a second one while it's active and it upgrades to a **five-way shot**.
  - *Shield* (5 s), which absorbs one hit.
- **Lives, score and game over**, with music switching between gameplay and game over.
- **Game feel:** camera shake on damage, particle explosions, a scrolling background and an animated color-shifting nebula.

---

## How it works

All gameplay code is in [`Assets/Scripts`](Assets/Scripts) (Unity 2021.3, C#).

| Script | Responsibility |
| :--- | :--- |
| `PlayerController` | Movement within screen bounds, shooting, lives, power-up timers |
| `SpawnManager` | Coroutine-based spawning of waves, power-ups, bombs and planets; moves spaceship formations |
| `PoolManager` + `BombScale` | Bombs are **object-pooled**: reused instead of destroyed, and reset when taken from the pool |
| `Enemy` / `EnemySpaceShip` | Collision handling, scoring and death effects for each enemy type |
| `ShootingScript` | Laser movement and cleanup, including multi-laser spreads |
| `GameManager` | Singleton holding game state, UI, score, music and restart |

---

## Looking back

This was the project where I learned Unity's fundamentals, and I've deliberately kept the code exactly as I originally wrote it. I've since moved on to 3D work, and looking back I can spot mistakes I'd now catch straight away:

- **Asteroids are moved by two scripts at once:** `Enemy.cs` moves them, and `SpawnManager` also moves every object tagged `Enemy`.
- **Power-up timers aren't tracked:** picking up a shield while one is active lets the older timer switch off the new shield early.
- **Laser spreads:** when one laser of a triple/five shot hits, the whole spread is destroyed, and a laser without a parent can cause a null reference.
- **Bomb growth timing** uses `Time.time % interval`, which can skip or repeat steps depending on frame rate.
- `FindGameObjectsWithTag` runs every frame, and `GameManager` polls the player's lives instead of reacting to an event.

If I built it again today, I would:

- Use **events** instead of `FindObjectOfType` and direct references between managers.
- Move tuning values (speeds, durations, health) into **ScriptableObjects** so they can be balanced without touching code.
- Use the **new Input System**, and **pool** lasers and enemies as well, not just bombs.
- Split `SpawnManager`, which handles several unrelated jobs, into smaller spawners.

---

## Running the project

1. Clone the repository and open it with **Unity 2021.3.16f1** (or a later 2021.3 LTS).
2. Import these two free packs from the Unity Asset Store:
   - [2D Space Kit](https://assetstore.unity.com/search#q=2D%20Space%20Kit) (backgrounds and effects)
   - [2D Casual UI](https://assetstore.unity.com/search#q=2D%20Casual%20UI) (interface sprites)

   They aren't included in this repository because their license doesn't allow redistributing the raw files. Without them, some sprites will appear as missing; after importing, Unity reconnects them automatically.
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

---

## Credits

- **2D Space Kit** and **2D Casual UI**: Unity Asset Store, used under the Standard Unity Asset Store EULA.

Game design and code by **Andrewk12** ([GitHub](https://github.com/antrewk12) · [itch.io](https://antreask12.itch.io)).
The code is released under the MIT License (see [`LICENSE`](LICENSE)); third-party assets keep their own licenses.
