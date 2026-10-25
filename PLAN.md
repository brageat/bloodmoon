# Bloodmoon: Plan

A black and red mod menu for **PEAK** (BepInEx 5, C#).
Repo: https://github.com/brageat/bloodmoon-peak (public since v0.13.0)

**Current version:** v1.3.0 (TP to Peak goes to the real summit, Reset all). It's installed and loading in PEAK, but not yet tested or released.

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
- [x] v0.11.1: **TP on E removed** at your request (the v0.10.2 camera fix was never tried in-game) (built)
- [x] v0.11.1 installed
- [x] v0.11.2: empty tabs (Visuals, Misc) are hidden, and come back automatically when a mod uses them (built, released)
- [x] Items tested in-game
- [x] Settings save when the menu closes

- [x] v0.12.0: Items tab grouped (Scout statue, Endgame, Healing, Climbing, Utility) with the 3 statue amulets and Scout's Honor added (built)
- [x] v0.12.0 + v0.12.1 installed (in v0.13.0)
- [x] Grouped Items, statue amulets and noclip tested in-game: all work
- [x] Bug found: flying drifts upward. Fixed in v1.0.1 with a height hold (built)
- [x] v1.0.1 installed (it loads in PEAK)
- [x] Bug found: update check got 403 (GitHub's 60-an-hour limit, used up on a shared VPN address). Fixed: it falls back to the releases page (built)
- [x] Fly drift fix tested in-game: works
- [x] Release 1.0.1 published
- [x] v0.12.1: Noclip toggle under Fly, using the game's `ragdoll.ToggleCollision` (built)
- [x] v0.13.0: Update checker (Misc tab, a notice at startup, a line in the menu header) (built and installed)
- [x] Repo made public, so the update check works and anyone can download
- [x] Release v0.13.0 published
- [x] **v1.0.0** released, the first stable version

- [x] v1.1.0: Respawn at campfire + Respawn where you died, using the game's own `RPCA_ReviveAtPosition`; only when dead or passed out (built and installed)
- [x] Respawn tested in-game: works
- [x] Release 1.1.0 published (zip only, labeled)

- [x] v1.2.0: TP to players, bring player / everyone to me, Hide mountain TPs toggle (on by default) (built and installed)
- [x] Bug found: TP to Peak lands you in the air and you fall (the game warps you before the Peak's ground has loaded)
- [x] v1.2.1: after the jump, wait for the Peak to load, then re-warp onto `MapHandler.respawnThePeak`, with fall protection (built)
- [x] v1.2.1 tested: the re-warp ran, but `respawnThePeak` is at the foot of the final climb (in front of the Kiln), not the top
- [x] v1.3.0: TP to Peak targets the summit flare box instead; Reset all settings button in Misc (built)
- [x] v1.3.0 installed
- [x] TP to Peak tested: works (it takes a second to place you on top, so the button's description now says so)
- [x] Reset all settings tested: works
- [x] On-screen message while TP to Peak works: "At the Kiln for a sec... moving you up to the summit!", then "You're on the summit!" (built and installed)
- [x] On-screen TP to Peak message tested: works (made bigger and moved to the upper middle so it is easy to see)
- [ ] ⏰ **REMINDER: test the player TPs with Dad** (TP to Dad, Bring Dad to me, Bring everyone to me)
- [ ] Then publish the release

### Docs
- [x] Releases ship as **Bloodmoon.zip** (folders already set up, plus a HOW TO INSTALL.txt) because browsers warn less about zips. Download button and README point at the zip.
- [x] Clear step-by-step download and install instructions in the README, on the page and in the zip
- [x] Repo renamed to `brageat/bloodmoon-peak`, with a PEAK description and 10 search topics so people can find it. Old links redirect.
- [x] GitHub Release v0.11.1 with `Bloodmoon.dll` to download
- [x] changes.html updated to v0.11.1, with a download button
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
| 0.11.0 | `e04a73f` | **Items** tab: 17 practical items to spawn |
| 0.11.1 | `90a50c8` | TP on E removed |
| 0.11.2 | `a2d36c9` | Empty tabs are hidden until a mod adds a row to them |
| 0.12.0 | `956a1e2` | Items tab grouped under headings; added the 3 scout statue amulets and Scout's Honor; menus can have section headings |
| 0.12.1 | `42509d9` | Noclip (while flying) |
| 0.13.0 | `537d890` | Update checker: GitHub latest release, startup notice, Misc tab |
| **1.0.0** | `fc82cdd` | First stable version (same features as 0.13.0) |
| 1.0.1 | `ddc7b4f` | Fix: flying no longer drifts upward (height hold). Update check falls back to the releases page when GitHub's API rate limit is hit. |
| 1.1.0 | see git log | Respawn at campfire / where you died (Player tab) |
| 1.2.0 | see git log | Teleport tab: TP to each player, bring a player or everyone to you; Hide mountain TPs (on by default) |
| 1.2.1 | see git log | Fix: TP to Peak no longer drops you. After the jump it re-places everyone on the game's Peak landing spot once the Peak has loaded, with no fall damage. |
| 1.3.0 | see git log | TP to Peak lands at the real summit (next to the rescue flare box, `PeakHandler.flareBox`). New: Reset all settings (Misc). |

## Mods in the menu

| Tab | Mod | How it works | Tested in-game |
|-----|-----|--------------|----------------|
| Movement | Speed multiplier (0.5–5x) | Sets `CharacterMovement.movementModifier` | ✅ v0.1 |
| Movement | Reset speed | Sets speed back to 1x | ✅ v0.7 |
| Movement | Climb speed (0.5–5x) | Sets `CharacterClimbing.climbSpeedMod`, and puts the game's value back at 1x | ✅ v0.7 |
| Movement | Reset climb speed | Sets climb speed back to 1x | ✅ v0.7 |
| Movement | Fly | Turns gravity off on every ragdoll body part (`Bodypart.Rig`) and sets their velocity from WASD, Space, Ctrl and Shift each physics step. Resets `sinceGrounded` and `fallSeconds` so there's no fall damage. | ✅ v0.6 |
| Movement | Fly speed (2–40 m/s) | Shift doubles it | ✅ v0.7 |
| World | Instant campfire | `Campfire.DebugLight()` on the nearest unlit campfire (the same as a normal light) | ✅ v0.10 |
| Teleport | TP to biome ×6 | Calls `LightWithoutReveal()` on the skipped campfires, then the game's `MapHandler.JumpToSegment(segment)`, which moves everyone and updates the fog | ✅ v0.10 |
| Items | 17 spawn buttons | Finds the item by name (ignoring case) in `ItemDatabase.Instance.itemLookup`, then `ItemDatabase.Add(item, point)`, which spawns it over the network and picks it up | ✅ v0.11 |
| World | Stop rising fog | Holds `OrbFogHandler.currentWaitTime` at 0 and sets `speed` to 0. Also freezes the height-based `Fog` (`fogSpeed`, plus its private `sinceStop` timer). Only the host decides when fog moves. | ✅ v0.7 |
| Player | Infinite stamina | Turns on the game's own `Character.infiniteStam` switch and refills stamina to max | ✅ v0.7 |
| Player | Infinite health | Turns on the private `CharacterData.isInvincible` flag (through reflection) and clears every effect | ✅ v0.7 |
| Player | Set health (10–100%) | Sets Injury to `1 - health`. Other effects stay (v0.8). | ✅ v0.8 |
| Player | Heal fully | Sets Injury to 0. Other effects stay (v0.8.1). | ✅ v0.11 |
| Effects | Block injury / hunger / cold / poison / heat / drowsy / curse / spores | Clears only that effect whenever it appears | ✅ v0.8 |
| Effects | Block all effects | Clears every effect whenever it appears (no invincibility) | ✅ v0.11 |
| Effects | Clear all effects | Clears every effect once | ✅ v0.11 |
| Effects | Show each effect | Hides or shows the 8 Block rows | ✅ v0.11 |

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
- [x] Each spawn button gives you the item (check the log for any "couldn't find" lines)

## Test v0.10.3
- [x] TP to the biome you're in moves only you, and the campfire stays unlit
- [x] Settings are saved (the log shows "Settings saved" when you close the menu)

## Test v0.10.0: Teleport tab
- [x] Instant campfire lights the nearest campfire
- [x] TP to each biome lands at that biome and lights the campfires before it

## Test v0.8.1 + v0.9.0
- [x] Heal fully only heals injury; other effects stay
- [x] Block all effects keeps every effect away
- [x] Clear all effects removes everything once
- [x] Show each effect OFF hides the 8 Block rows, and ON brings them back

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
