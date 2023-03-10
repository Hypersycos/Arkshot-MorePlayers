# Arkshot-MorePlayers
Allows more than 4 players to join an instance of Arkshot (Steam ID 468800). Requires BepinEx and Arkshot-MultiplayerSync, and must be installed on the host and any clients that join after the 4th.

# Installation
## Prerequisites
### BepInEx
The plugin was developed with [5.4.21](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21/), so if in doubt use that. Make sure to download the 32-bit / x86 edition. Further instructions can be found [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html). Arkshot also requires the alternative endpoint, detailed [here](https://docs.bepinex.dev/articles/user_guide/troubleshooting.html#change-the-entry-point-1)
### Arkshot-MultiplayerSync
[MultiplayerSync](https://github.com/Hypersycos/Arkshot-MultiplayerSync) must be installed in the plugins folder, or the plugin will not load.

## Plugin
Download the [latest dll](https://github.com/Hypersycos/Arkshot-MorePlayers/releases/latest/download/MorePlayers.dll) and put it in Arkshot/BepInEx/plugins. After an initial run, the config file should be accessible in BepInEx/config. The value MaxPlayerCount is only relevant on the host, and can be changed to any value. Arkshot uses photon for its networking, and I do not know which tier it uses (though I would imagine it's the free one). This may limit the max number of players per room, but even the free tier allows 16 which I would argue is more than enough.
