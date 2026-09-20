# Changelog


## 1.0.4
- Added configurable breeding limits per species (Boars, Wolves, Lox, Chickens, Moose, Asksvin)
- Switched config sync to embedded ServerSync (v1.0.12)
- Breeding limits now enabled by default, max range changed to 3-20
- Cleaned the informative part of the mod

## 1.0.3
- Bumped BepInExPack dependency to 5.4.2350
- Confirmed Moose (Deep North) support
- No functional changes - patch targets verified compatible with 1.0.12

## 1.0.2
- Improved egg upgrade reliability when multiple animals breed at the same time
- Enhanced CLLC detection
- Added console commands: `breeding_info`, `breeding_simulate`, `breeding_debug`
- Added configuration validation
- Reorganized code for better maintainability

## 1.0.1
- Added automatic detection for CLLC (CreatureLevelAndLootControl)
- Fixed config incompatibility that sometimes caused upgrade failures
- Updated config descriptions with compatibility information
- Documented Star Level Systems incompatibility

## 1.0.0
- Initial release
- Added ServerSync for config enforcement
