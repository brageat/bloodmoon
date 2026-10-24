# Bloodmoon: Plan

A black and red mod menu for **PEAK** (BepInEx 5, C#).
Repo: https://github.com/brageat/bloodmoon (private)

**Current version:** v0.8.0 (Effects tab). It's built but not yet installed. Everything up to v0.7.0 is tested and working.

---

## Checkpoints
Checked off as each one is completed.

### Setup
- [x] .NET SDK 8 installed
- [x] PEAK installed, and BepInEx 5.4.75301 installed into it
- [x] Project scaffolded and saved to the private GitHub repo
- [x] README with requirements, setup steps and troubleshooting

### Menu
- [x] v0.1.0: first menu and speed slider, working in-game
- [x] v0.2.0: keyboard-only controls (no hand cursor)
- [x] v0.3.0: large categorised menu, module system, black and red theme

### Mods
- [x] Speed multiplier (tested in-game)
- [x] v0.4.0: Infinite stamina (built)
- [x] v0.5.0: Infinite health, Set health, Heal fully (built)
- [x] v0.5.0: Climb speed (built)
- [x] v0.5.0 installed and loading in PEAK (the log shows 4 modules)
- [x] v0.5.0 tested in-game: climb speed, stamina and all health mods work
- [x] v0.6.0: Flying (built)
- [x] v0.6.0 installed (loading in PEAK)
- [x] v0.6.0 tested in-game: flying works
- [x] v0.7.0: Stop rising fog (built)
- [x] v0.7.0 installed (log: 6 modules loaded)
- [x] v0.7.0 tested in-game: fog stays still (the "fog rises" message still shows)
- [x] v0.8.0: Effects tab with one Block toggle per effect, Set health only changes injury, menu scrolling (built)
- [ ] v0.8.0 installed and tested in-game
- [ ] More mods from [Ideas for later](#ideas-for-later)

### Docs
- [x] PLAN.md written
- [x] changes.html visual overview
- [x] Plan and overview committed and pushed

---

## Done

| Version | Commit | What changed |
|---------|--------|--------------|
| 0.1.0 | `d626df5`, `0ed04a8` | First menu (blood moon theme, Insert to open), speed multiplier slider. Fixed the build by adding the Photon reference. |
| 0.1.0 | `a816b11` | README: requirements, step-by-step setup, troubleshooting |
| 0.2.0 | `907a235` | **Keyboard-only menu.** PEAK's hand cursor no longer appears. Errors are logged through BepInEx, because Unity's log writer fails in PEAK. Building while the game is running now gives a warning instead of failing. |
| 0.3.0 | `2bfbbe9` | **Large menu**: categories sidebar, description footer, **module system**, black and red theme |
| 0.4.0 | `eaaea00` | Infinite stamina (Player) |
| 0.5.0 | `0ad725e` | Infinite health, Set health, Heal fully (Player). Climb speed and its reset (Movement). |
| 0.6.0 | `2985201` | Flying and fly speed (Movement). Modules can now run code on every physics step (`FixedTick`). |
| 0.7.0 | `1e93ec1` | Stop rising fog (World) |
| 0.8.0 | see git log | **Effects** tab: a Block toggle per effect, so players can pick which effects to stop. Set health only changes injury now. Long tabs scroll. |

## Mods in the menu

| Tab | Mod | How it works | Tested in-game |
|-----|-----|--------------|----------------|
| Movement | Speed multiplier (0.5–5x) | Sets `CharacterMovement.movementModifier` | ✅ v0.1 |
| Movement | Reset speed | Sets speed back to 1x | ✅ v0.7 |
| Movement | Climb speed (0.5–5x) | Sets `CharacterClimbing.climbSpeedMod`, and puts the game's value back at 1x | ✅ v0.7 |
| Movement | Reset climb speed | Sets climb speed back to 1x | ✅ v0.7 |
| Movement | Fly | Turns gravity off on every ragdoll body part (`Bodypart.Rig`) and sets their velocity from WASD, Space, Ctrl and Shift each physics step. Resets `sinceGrounded` and `fallSeconds` so there's no fall damage. | ✅ v0.6 |
| Movement | Fly speed (2–40 m/s) | Shift doubles it | ✅ v0.7 |
| World | Stop rising fog | Holds `OrbFogHandler.currentWaitTime` at 0 and sets `speed` to 0. Also freezes the height-based `Fog` (`fogSpeed`, plus its private `sinceStop` timer). Only the host decides when fog moves. | ✅ v0.7 |
| Player | Infinite stamina | Turns on the game's own `Character.infiniteStam` switch and refills stamina to max | ✅ v0.7 |
| Player | Infinite health | Turns on the private `CharacterData.isInvincible` flag (through reflection) and clears every effect | ✅ v0.7 |
| Player | Set health (10–100%) | Sets Injury to `1 - health`. Other effects stay (v0.8). | ⬜ v0.8 change |
| Player | Heal fully | Clears every effect once | ✅ v0.7 |
| Effects | Block injury / hunger / cold / poison / heat / drowsy / curse / spores | Clears only that effect whenever it appears | ⬜ |

## Controls

| Key | Action |
|-----|--------|
| Insert | Open / close |
| Tab / Shift+Tab | Next / previous category |
| Up / Down | Select a row |
| Left / Right | Adjust a slider, or turn a toggle off/on |
| Enter | Press a button, or flip a toggle |

---

## Now: test v0.5.0 in-game
- [x] Speed and climb speed both change how fast you move
- [x] Infinite stamina: sprint and climb without the bar draining
- [x] Infinite health: hunger, cold and falls don't hurt. Check whether a huge fall still knocks you out for a moment.
- [x] Set health: ← and → change the stamina bar right away
- [x] Heal fully clears everything

If anything fails, check `PEAK\BepInEx\LogOutput.log` for `[Error : Bloodmoon]` lines.

## Test v0.6.0: flying
- [x] Turning Fly on makes you float instead of falling
- [x] WASD follows the camera, Space goes up, Ctrl goes down, Shift is faster
- [x] Letting go of all keys makes you hover
- [x] Turning Fly off drops you to the ground with no fall damage

## Test v0.8.0: Effects tab
- [ ] Each Block toggle stops only its own effect
- [ ] Set health changes injury without clearing other effects
- [ ] Effects tab scrolls (8 rows, 5 shown) with "more above/below" hints

## Test v0.7.0: stop rising fog
- [x] With it on, the fog countdown never runs out and the fog doesn't close in
- [x] Turning it off makes the fog move again after a full countdown

## Flying: how it was built (Movement tab)
How the game works (checked in `Assembly-CSharp`):
- The character is a physics ragdoll. Its body parts are in `character.refs.ragdoll.partList`, and each has a `Bodypart.rig` (a `Rigidbody`).
- Player input is in `character.input`: `movementInput`, `jumpIsPressed` and `crouchIsPressed`.
- Fall damage is handled in `CharacterMovement.CheckFallDamage()`, and there is a `CapFallDamage()` helper.

Plan:
1. **Fly** toggle and **Fly speed** slider.
2. While flying: turn off gravity on every body part, and set each part's velocity from input. WASD moves relative to the camera, Space goes up and Ctrl goes down. With no input, you hover.
3. Keep the character counted as on the ground (`sinceGrounded`), so landing doesn't do fall damage.
4. When flying is turned off, turn gravity back on.

## Ideas for later
| Tab | Idea |
|-----|------|
| Player | No fall damage (separate from infinite health), super jump |
| World | Time of day, fullbright |
| Visuals | Show teammates through walls, FOV slider |
| Misc | Teleport to a teammate, unlock all badges |

---

## Workflow notes
- **Close PEAK before building.** The game locks `Bloodmoon.dll` while it's running.
- Build: `dotnet build -c Release`. This also copies the DLL into `PEAK\BepInEx\plugins\Bloodmoon\`.
- Launch: `steam://rungameid/3527290`
- **Adding a mod:** create a `Modules/XModule.cs` that extends `Module`, then add it to `Plugin.Modules`.
- To find game field names, read `Assembly-CSharp.dll` with reflection. Private fields need `System.Reflection`.
- See [changes.html](changes.html) for a visual overview.
