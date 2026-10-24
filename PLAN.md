# Bloodmoon: Plan

A black and red mod menu for **PEAK** (BepInEx 5, C#).
Repo: https://github.com/brageat/bloodmoon (private)

**Current version:** v0.11.0 (Items tab). It's built but not yet installed. Everything works except TP on E (the fix is waiting to be tested).

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
- [x] v0.8.0 installed (log: loaded)
- [x] v0.8.0 tested in-game: Effects tab, Set health and scrolling work
- [x] v0.8.1: Heal fully only heals injury; new Clear all effects button in the Effects tab (built)
- [x] v0.8.1 installed
- [x] v0.9.0: Block all effects toggle, Show each effect toggle (hides the 8 Block rows), rows can hide themselves (built)
- [x] v0.9.0 installed
- [x] v0.9.0 Effects toggles work in-game
- [x] v0.9.1: Show each effect is off by default (built)
- [x] v0.9.1 installed
- [x] v0.10.0: Teleport tab with TP on E, Instant campfire and TP to each biome (built and installed)
- [x] v0.10.1: menu fits as many rows as there's room for (Movement shows all 6); bigger ▲/▼ "more" text (built)
- [x] v0.10.0 tested: Instant campfire and TP to biomes work; **TP on E didn't work**
- [x] v0.10.2: TP on E fix (PEAK's camera isn't `Camera.main`, so it now uses `MainCamera.instance` + look direction), TP-to-biome campfire guard, settings-save logging (built)
- [x] v0.10.3: TP to a current or earlier biome moves only you to its campfire and lights nothing, so friends aren't left in the fog. Only later biomes do the full lobby jump. (built)
- [x] v0.10.4: Instant campfire moved to the World tab (built)
- [x] v0.11.0: Items tab, spawning 17 practical items with the game's own `ItemDatabase.Add` (built)
- [ ] v0.11.0 installed
- [ ] Items tested in-game
- [ ] TP on E tested in-game
- [ ] Settings save when the menu closes (they haven't saved since 4:42 PM)
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
| 0.8.0 | `bfb5bef` | **Effects** tab: a Block toggle per effect, so players can pick which effects to stop. Set health only changes injury now. Long tabs scroll. |
| 0.8.1 | `ef5436b` | Heal fully only heals injury. Clearing everything moved to a new **Clear all effects** button (Effects). |
| 0.9.0 | `7c07799` | **Block all effects** toggle, plus a **Show each effect** toggle that hides the 8 separate Block rows. Menu rows can now hide themselves (`MenuItem.IsShown`). |
| 0.9.1 | `64e9a03` | Show each effect is off by default |
| 0.10.0 | `3c98029` | **Teleport** tab: TP on E, Instant campfire, TP to each biome |
| 0.10.1 | `f06d1ec` | Rows fill the available height, measured from real row sizes, instead of a fixed 5. Bigger ▲/▼ "more" hints. |
| 0.10.2 | `58e3b73` | TP on E fix (camera), guard for each campfire in TP to biome, logging for TP and settings saves |
| 0.10.3 | `8fb1560` | TP to a current or earlier biome only moves you and lights nothing, so friends are safe |
| 0.10.4 | `c4af401` | Instant campfire moved to the World tab |
| 0.11.0 | see git log | **Items** tab: 17 practical items to spawn |

## Mods in the menu

| Tab | Mod | How it works | Tested in-game |
|-----|-----|--------------|----------------|
| Movement | Speed multiplier (0.5–5x) | Sets `CharacterMovement.movementModifier` | ✅ v0.1 |
| Movement | Reset speed | Sets speed back to 1x | ✅ v0.7 |
| Movement | Climb speed (0.5–5x) | Sets `CharacterClimbing.climbSpeedMod`, and puts the game's value back at 1x | ✅ v0.7 |
| Movement | Reset climb speed | Sets climb speed back to 1x | ✅ v0.7 |
| Movement | Fly | Turns gravity off on every ragdoll body part (`Bodypart.Rig`) and sets their velocity from WASD, Space, Ctrl and Shift each physics step. Resets `sinceGrounded` and `fallSeconds` so there's no fall damage. | ✅ v0.6 |
| Movement | Fly speed (2–40 m/s) | Shift doubles it | ✅ v0.7 |
| Teleport | TP on E | Raycasts from the camera (skipping your own body) and calls the game's `WarpPlayerRPC` for everyone | ❌ v0.10, fix in v0.10.2 |
| World | Instant campfire | `Campfire.DebugLight()` on the nearest unlit campfire (the same as a normal light) | ✅ v0.10 |
| Teleport | TP to biome ×6 | Calls `LightWithoutReveal()` on the skipped campfires, then the game's `MapHandler.JumpToSegment(segment)`, which moves everyone and updates the fog | ✅ v0.10 |
| Items | 17 spawn buttons | Finds the item by name (ignoring case) in `ItemDatabase.Instance.itemLookup`, then `ItemDatabase.Add(item, point)`, which spawns it over the network and picks it up | ⬜ |
| World | Stop rising fog | Holds `OrbFogHandler.currentWaitTime` at 0 and sets `speed` to 0. Also freezes the height-based `Fog` (`fogSpeed`, plus its private `sinceStop` timer). Only the host decides when fog moves. | ✅ v0.7 |
| Player | Infinite stamina | Turns on the game's own `Character.infiniteStam` switch and refills stamina to max | ✅ v0.7 |
| Player | Infinite health | Turns on the private `CharacterData.isInvincible` flag (through reflection) and clears every effect | ✅ v0.7 |
| Player | Set health (10–100%) | Sets Injury to `1 - health`. Other effects stay (v0.8). | ✅ v0.8 |
| Player | Heal fully | Sets Injury to 0. Other effects stay (v0.8.1). | ⬜ v0.8.1 change |
| Effects | Block injury / hunger / cold / poison / heat / drowsy / curse / spores | Clears only that effect whenever it appears | ✅ v0.8 |
| Effects | Block all effects | Clears every effect whenever it appears (no invincibility) | ⬜ |
| Effects | Clear all effects | Clears every effect once | ⬜ |
| Effects | Show each effect | Hides or shows the 8 Block rows | ⬜ |

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

## Test v0.11.0: Items
- [ ] Each spawn button gives you the item (check the log for any "couldn't find" lines)

## Test v0.10.3
- [ ] TP on E teleports you to where you look
- [ ] TP to the biome you're in moves only you, and the campfire stays unlit
- [ ] Settings are saved (the log shows "Settings saved" when you close the menu)

## Test v0.10.0: Teleport tab
- [ ] TP on E: look somewhere, press E, and you teleport there
- [ ] Instant campfire lights the nearest campfire
- [ ] TP to each biome lands at that biome and lights the campfires before it

## Test v0.8.1 + v0.9.0
- [ ] Heal fully only heals injury; other effects stay
- [ ] Block all effects keeps every effect away
- [ ] Clear all effects removes everything once
- [ ] Show each effect OFF hides the 8 Block rows, and ON brings them back

## Test v0.8.0: Effects tab
- [x] Each Block toggle stops only its own effect
- [x] Set health changes injury without clearing other effects
- [x] Effects tab scrolls (8 rows, 5 shown) with "more above/below" hints

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
