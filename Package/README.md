> **Disclaimer:**
> Most of mods I work with are some old and outdated ones that their authors haven't updated so far to match 1.0. I am not a professional modder myself, but I am good with coding and gaming. If you experience any problems with the mods I published, you can find me in <a href="https://discord.com/channels/1522110224947871817/1522118606937133136">Hexium</a> discord server by typing DMT.

# BreedingUpgrades

A Valheim mod that gives offspring of your tamed creatures a rare chance to spawn with an extra star.
[Fork/update of Dumba's BreedingUpgrades](https://thunderstore.io/c/valheim/p/Dumba/BreedingUpgrades/), maintained for Valheim 1.0.x.

**GitHub:** <img height="18" src="https://github.githubassets.com/favicons/favicon-dark.svg"></img> [DaiMinhTri/BreedingUpgrades](https://github.com/DaiMinhTri/BreedingUpgrades)

## Features

### Star Upgrades
Offspring have a configurable percentage chance (default 5%) to gain a star level when born.

### Egg Support
Works for both mammals (Wolves, Lox, Boars, Moose) and egg-layers (Chickens, Asksvin). Eggs are intercepted during procreation and upgraded in the same roll.

### Moose (Deep North)
Moose uses the standard Tameable/Procreation components, so star upgrades apply to calves and carry over when they grow up.

### Server-Side Control
Configuration is locked by default and synced from the server using embedded ServerSync. Clients cannot change settings unless the server allows it.

### Breeding Limits (Optional)
Configurable population cap per species. When enabled, breeding stops when the species limit is reached nearby. Supports Boars, Wolves, Lox, Chickens, Moose, and Asksvin.

### Config Hot-Reload
Edit the `.cfg` file while the game is running — changes are picked up automatically without restarting.

### Console Commands
| Command | Description |
|---------|-------------|
| `breeding_info` | Display mod status and current configuration |
| `breeding_simulate [count]` | Simulate breeding rolls (default 100) |
| `breeding_debug` | Toggle debug logging on/off |

## Configuration

A configuration file is generated at `BepInEx/config/DMT.breedingupgrades.cfg` after the first launch.

| Section | Setting | Default | Description |
|---------|---------|---------|-------------|
| General | Lock Configuration | On | Lock config to server admins only |
| General | Enable Mod | On | Master toggle for mod functionality |
| Breeding | Upgrade Chance | 5.0% | Percentage chance for offspring to gain +1 star |
| Breeding | Max Star Level | 2 | Maximum star level for offspring (0-10). Values above 2 require CLLC or similar mod |
| Breeding Limits | Enable Breeding Limit | On | Enable custom population cap per species |
| Breeding Limits | Max Boars | 5 | Max boars nearby before breeding stops (3-20) |
| Breeding Limits | Max Wolves | 5 | Max wolves nearby before breeding stops (3-20) |
| Breeding Limits | Max Lox | 5 | Max lox nearby before breeding stops (3-20) |
| Breeding Limits | Max Chickens | 5 | Max chickens nearby before breeding stops (3-20) |
| Breeding Limits | Max Moose | 5 | Max moose nearby before breeding stops (3-20) |
| Breeding Limits | Max Asksvin | 5 | Max asksvin nearby before breeding stops (3-20) |
| Debug | Enable Debug Logging | Off | Verbose logging for troubleshooting (not synced) |

> **Note:** Max Star Level above 2 requires [CLLC](https://valheim.thunderstore.io/package/JavaJarred/Creature_Level_And_Loot_Control/) or a similar creature level mod. Without it, upgrades will cap at 2 stars (vanilla limit).

## Compatibility

### Creature Level Mods
- **CLLC (CreatureLevelAndLootControl)** - Did not check with the latest updates.
- **Star Level Systems** - Was not compatible in the past, and I haven't tested, because I don't use it.

### Creature Mods
- **Moose (Deep North)** - Fully supported. Moose uses standard Tameable/Procreation components.
- **Therzie's Monstrum** - Used to work in the past. Haven't tested, because I don't use it.
- Should work with all mods that add standard tamable creatures or eggs.

## Installation

1. Install [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download and extract `BreedingUpgrades.dll` into your `BepInEx/plugins/` folder
3. Launch the game to generate the config file

## Future Plans

### Mutation System
Rare traits (speed, health, damage) beyond stars. Creatures can inherit mutations from parents with configurable chance per trait.

## Credits
- Original mod by **Dumba** (Discord: dumba7435)
- ServerSync library by **Blitz**

## Buy Me a Coffee

If you enjoy this mod, consider buying me a coffee: [![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-daiminhtri-yellow)](https://buymeacoffee.com/daiminhtri)

## Changelog

### 1.0.4
- Added configurable breeding limits per species (Boars, Wolves, Lox, Chickens, Moose, Asksvin)
- Added config hot-reload — edit `.cfg` file while the game is running
- Cleaned the informative part of the mod

### 1.0.3
- Bumped BepInExPack dependency to 5.4.2350
- Confirmed Moose (Deep North) support
- No functional changes - patch targets verified compatible with 1.0.12

### 1.0.2
- Improved egg upgrade reliability when multiple animals breed at the same time
- Enhanced CLLC detection
- Added console commands: `breeding_info`, `breeding_simulate`, `breeding_debug`
- Added configuration validation
- Reorganized code for better maintainability

### 1.0.1
- Added automatic detection for CLLC (CreatureLevelAndLootControl)
- Fixed config incompatibility that sometimes caused upgrade failures
- Updated config descriptions with compatibility information
- Documented Star Level Systems incompatibility

### 1.0.0
- Initial release
- Added ServerSync for config enforcement
