# Bloodmoon

A black-and-red mod menu for **PEAK**, written as a BepInEx 5 plugin in C#.

## ⬇ Download and install

### **[⬇ Download Bloodmoon.zip](https://github.com/brageat/bloodmoon-peak/releases/latest/download/Bloodmoon.zip)**
Always the newest version, about 23 KB. [All versions](https://github.com/brageat/bloodmoon-peak/releases)

> **Browser warning?** Chrome or Edge may say the file "isn't commonly downloaded". That only means the mod is new and not many people have downloaded it yet. Click **Keep** (in Chrome, open the downloads list, then **⋯ → Keep**).

### Step 1: Install BepInEx (one time only)
BepInEx is the mod loader that lets PEAK load mods.
1. Go to **[BepInExPack_PEAK on Thunderstore](https://thunderstore.io/c/peak/p/BepInEx/BepInExPack_PEAK/)** and click **Manual Download**.
2. Open your **PEAK folder**: in Steam, right-click **PEAK → Manage → Browse local files**.
3. Open the BepInEx zip, go into its `BepInExPack_PEAK` folder, and copy **everything inside it** into your PEAK folder (next to `PEAK.exe`).
4. Start PEAK once, then close it. BepInEx finishes setting itself up.

### Step 2: Install Bloodmoon
1. Download **Bloodmoon.zip** (the link above).
2. Open the zip and drag its **`BepInEx`** folder into your PEAK folder. If Windows asks to merge or replace, click **Yes**.
3. Check that this file exists: `PEAK\BepInEx\plugins\Bloodmoon\Bloodmoon.dll`

### Step 3: Play
Start PEAK, join a game and press **Insert** to open the menu.

### Updating
When a new version is out, Bloodmoon shows a red notice at the top of the screen. **Close PEAK**, download the new zip, and do Step 2 again. Click **Yes** to replace the old file.

### Uninstalling
Delete the folder `PEAK\BepInEx\plugins\Bloodmoon\`.

No need to build anything to just play. The build steps below are only for changing the code.

## Features
- Large menu with a procedurally drawn blood moon, a sidebar of categories (Movement, Player, Effects, Teleport, Items, World, Misc; empty tabs are hidden until a mod uses them) and a description of whichever row is selected. Long tabs scroll to follow your selection.
- **Keyboard only**, so the mouse cursor never appears.
- Each mod is its own module, so adding new ones is easy (see [Adding a new mod](#adding-a-new-mod)).

### Mods
| Category | Mod | What it does |
|----------|-----|--------------|
| Movement | Speed multiplier | 0.5x to 5x in 0.25 steps, saved between sessions |
| Movement | Reset speed | Puts speed back to 1x |
| Movement | Climb speed | 0.5x to 5x climbing speed in 0.25 steps |
| Movement | Reset climb speed | Puts climbing speed back to 1x |
| Movement | Fly | WASD moves where you look, Space goes up, Ctrl goes down, Shift doubles speed, and you hover when you let go. No fall damage while flying or for 2 s after. Always starts off. |
| Movement | Fly speed | 2 to 40 m/s |
| Movement | Noclip (while flying) | Fly through walls, rocks and the ground. Collision comes back when you stop flying. |
| World | Instant campfire | Lights the nearest unlit campfire instantly, with no holding and no waiting for everyone |
| Misc | Update status | Checks GitHub's latest release when the game starts. If there's a newer version, a red notice shows at the top of the screen and in the menu header. Enter opens the download page. |
| Misc | Check for updates | Asks GitHub again |
| Misc | Reset all settings | Puts every mod back to default (speeds 1x, every toggle off). Keeps your menu key and update settings. |
| Teleport | TP to <player> | A button for each other player in your lobby; teleports you to them |
| Teleport | Bring <player> to me / Bring everyone to me | Teleports other players to you, spread out in front of you (dead players are skipped) |
| Teleport | Hide mountain TPs | **On by default.** Hides the TP-to-biome buttons so nobody jumps the lobby by accident |
| Teleport | TP to Beach / Tropics / Alpine-Mesa / Caldera / The Kiln / Peak | **Current or earlier biome:** teleports only you to that biome's campfire and lights nothing, so friends aren't affected. **Later biome:** lights the skipped campfires and uses the game's segment jump, which **moves everyone in the lobby** (host only). |
| Items | Spawn, grouped: **Scout statue** (Healing, Infinite stamina and Super jump amulets, Scout's Honor), **Endgame** (Strange gem, Dynamite, Scout effigy), **Healing** (First aid kit, Cure-all, Energy drink), **Climbing** (Rope spool, Rope cannon, Chain launcher, Climbing spike, Rescue claw, Magic bean, Cloud fungus), **Utility** (Flare, Lantern, Portable stove, Backpack) | Uses the game's own `ItemDatabase.Add`, so everyone sees the item, and puts it in your hand (or in front of you if your hands are full) |
| World | Stop rising fog | Freezes the rising fog and holds its countdown at 0. Works solo or as host. Turning it off restarts the countdown. |
| Player | Infinite stamina | Never run out of stamina while sprinting, jumping or climbing |
| Player | Infinite health | Blocks all damage and clears every effect (the all-in-one switch) |
| Player | Set health | 10% to 100%. Only changes injury; other effects stay |
| Player | Heal fully | Heals all injury once. Other effects stay. |
| Player | Respawn at campfire | When you're dead or passed out: revives you next to the lit campfire closest to where you died (the same revive the Scout effigy uses; you drop your items) |
| Player | Respawn where you died | The same, but at the spot you died |
| Effects | Block injury / hunger / cold / poison / heat / drowsy / curse / spores | A separate toggle for each effect. It stops that one effect and leaves the others alone. |
| Effects | Block all effects | One switch that keeps every effect away (lighter than Infinite health) |
| Effects | Clear all effects | Clears every effect once |
| Effects | Show each effect | Shows or hides the separate Block toggles. Off by default, so the tab stays short. |

PEAK has no HP bar. Damage comes as afflictions that fill up the stamina bar, and you pass out when it's full, so the health mods work on those.

### Controls
| Key | Action |
|-----|--------|
| **Insert** | Open / close the menu |
| **Tab** / **Shift+Tab** | Next / previous category |
| **Up / Down** | Select a row |
| **Left / Right** | Change a slider (hold to repeat), or turn a toggle off/on |
| **Enter** | Press a button, or flip a toggle |

---

## What you need

| # | What | Where to get it | How to check it's working |
|---|------|-----------------|---------------------------|
| 1 | **Windows PC** | – | – |
| 2 | **PEAK** (Steam) | Steam store | The game launches |
| 3 | **.NET SDK 8** or newer | `winget install Microsoft.DotNet.SDK.8` or https://dotnet.microsoft.com/download | `dotnet --list-sdks` shows `8.x` |
| 4 | **BepInEx 5 for PEAK** (`BepInExPack_PEAK`) | https://thunderstore.io/c/peak/p/BepInEx/BepInExPack_PEAK/ | A `BepInEx\plugins` folder exists in the PEAK folder after you launch the game |

## Setup, step by step

### 1. Install the .NET SDK
```
winget install Microsoft.DotNet.SDK.8
```
Accept the Windows admin prompt when it appears. Then **open a new terminal** and check it worked:
```
dotnet --list-sdks
```

### 2. Install BepInEx into PEAK
1. Download `BepInExPack_PEAK` from the Thunderstore link above using **Manual Download**.
2. Open the zip and go into its `BepInExPack_PEAK` folder.
3. Copy everything in there (`BepInEx`, `winhttp.dll`, `doorstop_config.ini` and `.doorstop_version`) into the PEAK folder, next to `PEAK.exe`.
   The default location is `C:\Program Files (x86)\Steam\steamapps\common\PEAK`, or in Steam: right-click PEAK → Manage → Browse local files.
4. **Launch PEAK once, then close it.** BepInEx finishes setting itself up on that launch.

### 3. Tell the project where PEAK is (only if it isn't in the default location)
In `Bloodmoon.csproj`, change this line to your PEAK folder:
```xml
<GameDir Condition="'$(GameDir)' == ''">C:\Program Files (x86)\Steam\steamapps\common\PEAK</GameDir>
```
Or pass the folder when you build:
```
dotnet build -c Release -p:GameDir="D:\SteamLibrary\steamapps\common\PEAK"
```

### 4. Build
From this folder:
```
dotnet build -c Release
```
This builds against the game's own DLLs, then **copies `Bloodmoon.dll` into `PEAK\BepInEx\plugins\Bloodmoon\` automatically**.

### 5. Play
Launch PEAK, join a game and press **Insert**. See [Controls](#controls).

---

## Troubleshooting
| Problem | Fix |
|---------|-----|
| `No .NET SDKs were found` | Do step 1, then open a **new** terminal. |
| `Could not find ... BepInEx.dll` or `Assembly-CSharp.dll` | BepInEx isn't installed, the game hasn't been launched once, or `GameDir` is wrong (steps 2 and 3). |
| Build error mentioning `movementModifier` or `refs` | A PEAK update renamed something the speed code uses. Check `Modules/SpeedModule.cs` against the current game code. |
| `Couldn't install Bloodmoon.dll. Close PEAK first` | The game locks the DLL while it's running. Close PEAK and build again. |
| Menu doesn't appear | Check `PEAK\BepInEx\LogOutput.log` for a "Bloodmoon ... has risen" line. If it isn't there, the plugin didn't load. |

## Config
**Update check:** when the game starts it reads `ReleaseApiUrl` (GitHub's latest-release API for this repo). Turn it off with `CheckOnStart = false`.

`PEAK\BepInEx\config\com.brageat.bloodmoon.cfg` is created on first launch. You can change the toggle key and the default speed there.

## Uninstall
Delete `PEAK\BepInEx\plugins\Bloodmoon\`. To remove all mods, also delete `BepInEx`, `winhttp.dll`, `doorstop_config.ini` and `.doorstop_version` from the PEAK folder.

## Project layout
| File | What it does |
|------|--------------|
| `Plugin.cs` | Entry point: the list of modules, the toggle key and error logging |
| `Menu/BloodmoonMenu.cs` | Keyboard navigation and the menu layout |
| `Menu/MenuItems.cs` | Categories and row types: slider, toggle, button |
| `Menu/Theme.cs` | Black and red colours plus the procedural textures (moon, bars, highlights) |
| `Modules/Module.cs` | Base class every mod builds on |
| `Modules/SpeedModule.cs` | The speed multiplier mod |
| `Bloodmoon.csproj` | Build settings, references to the game's DLLs and the auto-copy into plugins |

## Adding a new mod
1. Create a file in `Modules/`, e.g. `Modules/FlyModule.cs`, with a class that extends `Module`:
   - In `Init()`, bind its config values and add its rows to the menu (`SliderItem`, `ToggleItem` or `ButtonItem`), each with a `Category`.
   - In `Tick()`, do the actual work. It runs every frame.
2. Add `new FlyModule(),` to the `Modules` list in `Plugin.cs`.
3. Build. The rows appear in their category automatically.

`Modules/SpeedModule.cs` is a complete example to copy from.

## Notes
- Speed works by setting `Character.localCharacter.refs.movement.movementModifier`. At 1x the mod doesn't touch it.
- This is a personal fun project and isn't affiliated with the makers of PEAK.
