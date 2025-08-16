# Highway-Racer
A lightweight 2D lane-switch racing/runner built in Unity. Drive down a multi‑lane highway, dodge cars, and survive as long as possible while the speed ramps up.

## How to Run (Build folder)
If you just want to play the prebuilt version:

- Open the `Build/` folder in the repo (some projects use `Builds/`).
- **Windows:** run `HighwayRacer.exe`.
- **macOS:** open `HighwayRacer.app` (If a Popup shows saying its not verified, You need to go to Settings > Privacy & Security > Scroll down and Allow Open).

If the `Build` folder is missing, open the project in Unity and create a build via **File → Build Settings…** for your target platform.

## Preview
- **Platform:** Mobile + Desktop
- **Input:** A/D or ←/→ keys; swipe left/right on touch
- **Core Loop:** Survive, score ticks up every second, speed increases over time

### Requirements
- **Unity**: 2021.3 LTS or newer (2022/2023 LTS recommended)
- **Packages/Deps**:
- [**DOTween**](http://dotween.demigiant.com/)
- **TextMeshPro** (Unity built‑in, for HUD text)

## Controls
- **Keyboard**: `A` / `←` to move left; `D` / `→` to move right.

## How It Works (Code Overview)

- **GameManager**
  - Starts/stops the run, updates score each second, raises events.
  - Drives background scrolling via `RawImage.uvRect`.
  - Ramps `gameSpeed` over time (capped by `maxGameSpeed`).

- **CarController**
  - Reads input (keys + swipe), clamps to lane indexes, and tween‑moves to the target lane.
  - On trigger with an `Obstacle`, plays SFX, sets VFX active, and calls `GameOver`.

- **ObstacleManager**
  - Spawns **waves** with lane safety: always keeps at least one free lane; adds grace rules at start.
  - Computes wave interval based on `minWaveGapY` and current speed to avoid unavoidable stacks.
  - Keeps per‑lane cooldowns to reduce overlaps.

- **ObstaclePoolManager**
  - Classic queue‑based pool; reuses inactive obstacle cars.
  - `Spawn/Despawn` manage activation and bookkeeping.

- **DisableZone**
  - Trigger below the screen; sends cars back to the pool when they exit the play area.

- **UIManager**
  - Shows/hides **Main Menu**, **HUD**, and **Game Over** panels.
  - Updates the on-screen **score** text.
  - Called by `GameManager` on start/over to switch panels.

- **SoundManager**
  - Manages two `AudioSource`s: **SFX** and **Music**.
  - Uses a **key → AudioClip** map (e.g., `bgMusic`, `gameover`, `explosion`, `click`).


## AI-Generated Assets
- Some images and sprites in this project were generated with ChatGPT (OpenAI) and may have been lightly edited for integration. These assets are included for prototyping and demonstration.+

## Credits
- DOTween by Demigiant
- TextMeshPro by Unity
- Sound Effect by Universfield from Pixabay
- Sound Effect by Praz Khanal from Pixabay
- Sound Effect by Jurij from Pixabay

