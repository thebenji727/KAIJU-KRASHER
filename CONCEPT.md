# KAIJU KRASHER!

**A growing rampage simulator in VR**  
**Course:** CIST 1423 — Virtual Reality Programming and Technology  
**Format:** One-page concept · Unity + C# · XR Interaction Toolkit  
**Targets:** Meta Quest 2 and HTC Vive Pro 2 (VR Lab, Room 113)

## Pitch

You are a kaiju that does not start as a skyline. You hatch in an alley. You are the size of a stray dog. The city is generated, dense, and indifferent. Everything you smash feeds you. Everything you eat makes you taller. By the end of a run you are standing in the same streets you once hid in, peeling roofs off towers with both hands.

The fantasy is not “be Godzilla.” It is *becoming* Godzilla in first person, in a city built at human scale that then fails against you. *KAIJU KRASHER!* is the Saturday-matinee title on a game that still takes the body seriously.

## Loop

1. **Wake small.** Spawn in a procedural district. Reach, grab, bite, and swipe. Mailboxes, cars, storefronts, and crowds are the first food.
2. **Feed the meter.** Destruction and consumption fill a Growth bar. Thresholds trigger a short, embodied molt: the world shrinks, reach lengthens, a new ability unlocks.
3. **Rampage at the new scale.** A building becomes a box. A highway becomes a ribbon. Response scales with you (drones → tanks → gunships).
4. **Pick a branch.** Each molt offers two abilities (brute vs. ranged, armored vs. agile, roar vs. breath). A run is 10–15 minutes, one district, not an open metro.
5. **End state.** Collapse the district landmark, get contained, or outgrow the block.

Failure is containment or starvation. Success is a skyline you personally unmade.

## Growth

| Stage | Height | Toys | Unlocks |
| --- | --- | --- | --- |
| Pest | ~1 m | trash, stalls, props | bite, pounce, 1:1 grab |
| Menace | ~4 m | cars, buses, facades | dual-grab throw, tail sweep |
| Titan | ~15 m | brownstones, overpasses | roar stagger, climb + peel |
| Apex *(stretch)* | ~40 m | towers | breath *or* seismic slam |

Growth is continuous between thresholds so the street recedes in the headset, not in a cutscene.

## Why this belongs in CIST 1423

The final project must be an original virtual environment that uses the course stack: Unity scenes, GameObjects and prefabs, physics and colliders, C# scripts, and XR Interaction Toolkit locomotion plus object manipulation. *KAIJU KRASHER!* is that stack pointed at one idea — **player scale as the mechanic**.

- **Hands are the kaiju.** XRI grab interactables on destructible props. Two-handed tear on building facades. Throw cars as rigidbodies. No “press X to smash.”
- **Locomotion changes with size.** Pest stage: snap/teleport and physical lean around corners (Quest 2 and Vive Pro 2 both support this). Titan stage: look-to-stomp, large-arc arm sweeps, optional continuous move with a comfort vignette.
- **Scale done carefully.** Changing world-to-player scale can break stereo comfort. The project treats that as the hard problem: blend or snap molts, keep the horizon stable, support seated and standing, test on both lab headsets.
- **Generated city as prey.** A seeded grid of road tiles and building prefabs keeps art scope inside a term and gives GitHub something real to version: generators, prefab variants, and interaction scripts.

## Technical approach (Unity / C# / XRI)

- Scene built from prefab tiles: road, parcel, building shell, interior volume, landmark. A C# `CityGenerator` places them from a seed.
- Player is an XR Origin. Growth is a tracked scale factor on the origin *or* an inverse scale on the city root. Prototype both in week one of production; keep the one that does not induce sway on Quest 2 and Vive Pro 2.
- Destruction: pre-fractured building prefabs + `Rigidbody` impulse on grab/release. Particle and audio layers sell mass. Colliders swap LOD as the player outgrows a block.
- Interaction: XR Interaction Toolkit grab, socket-free two-hand pull, and a custom `KaijuMouth` / stomp trigger. Locomotion Providers swap at each molt.
- AI is thin: a flee field on crowd agents, panicked vehicle rigidbodies, then a director that spends a unit budget (drones, then tanks).
- Performance: one active district, impostors past mid-range, 90 Hz target on Vive Pro 2, comfortable frame time on Quest 2.

GitHub is the submission and the workflow: `main` stays playable, features land on branches (`city-gen`, `growth`, `xri-grab`, `molt-comfort`), pull requests before merge.

## Scope for the term

**Must ship:** one generated district, three growth stages, XRI grab / throw / stomp, one ultimate (breath or slam), a ~10-minute run that ends in landmark collapse or containment. Playable on Quest 2 and Vive Pro 2.

**Cut without guilt:** rival kaiju, multiplayer, full open metro, photoreal city, voice-driven roar as a required path.

**Stretch:** Apex stage, ability draft between molts, photo-mode from Titan height, wireless Vive Pro 2 event build for the VR Lab.

## Tone

Drive-in monster movie from inside the monster: wet streets, neon on wet scales, sirens that drop in pitch as you grow past them. The name is loud. The body is heavy. The city is a toy box that still had people in it.

---

*KAIJU KRASHER! — One run. One city. One body that will not fit it by the end.*
