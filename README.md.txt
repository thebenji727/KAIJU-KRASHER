# KAIJU KRASHER!

A growing kaiju rampage simulator in VR for CIST 1423.

## Concept
See [CONCEPT.md](CONCEPT.md) for the full design.

## Requirements
- Unity 2022.3 LTS (or the version you're using)
- XR Interaction Toolkit
- Targets: Meta Quest 2 and HTC Vive Pro 2

## How to open
1. Clone the repo
2. Open the Unity project at `Krash!/Kaiju Krasher`
3. Let packages import
4. Open the main scene and press Play

## Current scripts
- `CityGenerator.cs` – procedural city generation
- `KaijuGrowth.cs` – stage-based growth and scaling
- `KaijuLocomotionManager.cs` – switches locomotion by growth stage