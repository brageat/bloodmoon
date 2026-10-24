"""Build Bloodmoon.zip for a GitHub release: unzip it into the PEAK folder and the DLL lands in the right place.

Usage: python tools/make_release_zip.py bin/Release/netstandard2.1/Bloodmoon.dll Bloodmoon.zip 1.0.1
"""
import pathlib
import sys
import zipfile

dll = pathlib.Path(sys.argv[1])
out = pathlib.Path(sys.argv[2])
version = sys.argv[3]

install = f"""Bloodmoon {version}: a black and red mod menu for PEAK

HOW TO INSTALL
1. You need BepInEx for PEAK first (BepInExPack_PEAK on Thunderstore):
   https://thunderstore.io/c/peak/p/BepInEx/BepInExPack_PEAK/
   Copy it into your PEAK folder and start the game once.
2. Unzip THIS zip into your PEAK folder (the one with PEAK.exe).
   Bloodmoon.dll ends up in PEAK\\BepInEx\\plugins\\Bloodmoon\\
3. Start PEAK, join a game and press INSERT.

Find your PEAK folder in Steam: right-click PEAK > Manage > Browse local files.

More info and updates: https://github.com/brageat/bloodmoon-peak
"""

with zipfile.ZipFile(out, "w", zipfile.ZIP_DEFLATED) as z:
    z.write(dll, "BepInEx/plugins/Bloodmoon/Bloodmoon.dll")
    z.writestr("Bloodmoon - HOW TO INSTALL.txt", install.replace("\n", "\r\n"))

with zipfile.ZipFile(out) as z:
    for info in z.infolist():
        print(f"  {info.filename}  ({info.file_size} bytes)")
print(f"{out.name}: {out.stat().st_size} bytes")
