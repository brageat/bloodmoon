# Bloodmoon

A blood-red mod menu for **PEAK**, written as a BepInEx 5 plugin in C#.

## Features
- Draggable themed menu with a procedurally drawn blood moon. **Insert** opens and closes it.
- **Speed multiplier** slider, from 0.5x to 5x, saved between sessions.

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
Launch PEAK, join a game and press **Insert**.

---

## Troubleshooting
| Problem | Fix |
|---------|-----|
| `No .NET SDKs were found` | Do step 1, then open a **new** terminal. |
| `Could not find ... BepInEx.dll` or `Assembly-CSharp.dll` | BepInEx isn't installed, the game hasn't been launched once, or `GameDir` is wrong (steps 2 and 3). |
| Build error mentioning `movementModifier` or `refs` | A PEAK update renamed something the speed code uses. Check `SpeedHack.cs` against the current game code. |
| Menu doesn't appear | Check `PEAK\BepInEx\LogOutput.log` for a "Bloodmoon ... has risen" line. If it isn't there, the plugin didn't load. |

## Config
`PEAK\BepInEx\config\com.brageat.bloodmoon.cfg` is created on first launch. You can change the toggle key and the default speed there.

## Uninstall
Delete `PEAK\BepInEx\plugins\Bloodmoon\`. To remove all mods, also delete `BepInEx`, `winhttp.dll`, `doorstop_config.ini` and `.doorstop_version` from the PEAK folder.

## Project layout
| File | What it does |
|------|--------------|
| `Plugin.cs` | Entry point: settings, the toggle key and freeing the cursor while the menu is open |
| `BloodmoonMenu.cs` | The menu layout and theme, including the procedural textures |
| `SpeedHack.cs` | Applies the speed multiplier to your character |
| `Bloodmoon.csproj` | Build settings, references to the game's DLLs and the auto-copy into plugins |

## Notes
- Speed works by setting `Character.localCharacter.refs.movement.movementModifier`. At 1x the mod doesn't touch it.
- This is a personal fun project and isn't affiliated with the makers of PEAK.
