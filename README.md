# Bloodmoon

A blood-red mod menu for **PEAK**, written as a BepInEx 5 plugin.

## Features
- Draggable themed menu. **Insert** opens and closes it (you can change the key in the config).
- **Speed multiplier**, from 0.5x to 5x, saved between sessions.

## Setup (one time)
1. Install the **.NET SDK** (8 or newer): https://dotnet.microsoft.com/download
2. Install **BepInEx 5** for PEAK. The easiest way is the `BepInExPack_PEAK` package from Thunderstore: extract it into the PEAK folder, then run the game once so BepInEx creates its folders.
3. If PEAK isn't in `C:\Program Files (x86)\Steam\steamapps\common\PEAK`, change `GameDir` in `Bloodmoon.csproj`.

## Build
```
dotnet build -c Release
```
The build copies `Bloodmoon.dll` into `PEAK\BepInEx\plugins\Bloodmoon\` automatically.

## Config
`PEAK\BepInEx\config\com.brageat.bloodmoon.cfg`, which is created on first launch.

## Notes
- The speed multiplier sets `Character.localCharacter.refs.movement.movementModifier`. If a game update renames it, the build will fail with an error on that line.
- At 1x the mod doesn't touch movement at all.
