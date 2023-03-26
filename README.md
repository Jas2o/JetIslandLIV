# JetIslandLIV
Adds LIV support to Jet Island, allowing for mixed reality capture or avatars in third person.

This mod comes in two flavours:
- IPA is compatible with the mods at https://mods.jet-is.land
- MelonLoader makes it easy to use UnityExplorer.

# Installation and use
Pick a mod loader:
- IPA: follow the instructions at https://mods.jet-is.land/how-to-install/
- ML: download MelonLoader Installer and install 0.5.7 in Jet Island folder.

Then
- Download from [JetIslandLIV releases](https://github.com/Jas2o/JetIslandLIV/releases)
- (IPA only) Extract the DLL and LIVAssets folder into the Plugins folder.
- (ML only) Extract the DLL and LIVAssets folder into the Mods folder.
- Start the game from Steam.
- From LIV, change Capture tab to Manual and select JetIsland.exe.

# Build notes
Unlike the other LIV mods, this one has camera projection matrix commented out in LIV SDK to allow camera to see further, unsure if it will cause any issues for non-avatar users.
