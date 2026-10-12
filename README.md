# The Count Is Down

A small atmospheric 2D survival/crafting game built with Godot 4 and C#.

The project blends exploration, timed pressure, and recipe-based resolution. The player moves through multiple scenes, collects items, and reaches a final crafting decision before the game world becomes fully dark.

## Build status

This project does compile successfully outside the Godot editor via the .NET toolchain.

Verification run:

```bash
dotnet build "Canon Kmicic.csproj"
```

Result: successful build, exit code 0.

## Build vs runtime

This project can be compiled with .NET, but it is not a standalone .NET application in the usual sense. The game still requires the Godot runtime or an exported desktop build to actually run.

In practice:

- Build without the editor: yes
- Launch the game without the editor: yes, via Godot or a packaged build
- Run it as a normal console program: no

The repository already includes export presets in `export_presets.cfg`, so a desktop build can be produced through the Godot export pipeline.

## Technical overview

### Engine and runtime

- Godot 4.7.1 is configured in the project file
- The .NET target is `net8.0`
- The project is set up as a C# Godot game using the Godot.NET.Sdk package
- `project.godot` registers the main scene and autoloaded global nodes

### Core architecture

The project follows a scene-driven Godot architecture with a small set of singleton-style controller nodes and resource-backed content.

#### Global game controller

`GameMaster` acts as the central runtime state machine for the game session.

It is responsible for:

- the main countdown timer
- world darkening over time
- fade transitions between scenes and endings
- resetting the session state
- invoking the crafting/endgame flow
- registering UI nodes such as the crafting panel and game-over menu

This node is effectively the “session orchestrator” for the whole playthrough.

#### Player controller

`Kmicic` is the player character, implemented as a `CharacterBody2D`.

Its responsibilities include:

- axis-based horizontal movement
- gravity and collision response via `MoveAndSlide()`
- animation playback tied to movement state
- locking the player during death or transition sequences

This makes the player a reusable actor that can be carried across room changes without being recreated.

#### Level and scene flow

`LevelContainer` is the transition manager between map scenes.

It handles:

- loading new `Level` scenes by path
- preserving the player instance between level changes
- positioning the player at named spawn points
- applying camera bounds after scene changes
- fading in and out while swaps occur
- enabling/disabling transition triggers during load transitions

This is the core architectural mechanism that lets the game behave like a connected world rather than isolated screens.

#### Inventory and crafting logic

`ItemManager` stores the player’s collected resources and tracks which pickups have already been marked as acquired.

`Crafting` then checks the selected items against recipe definitions in the `resources/recepies` folder. The matching logic is intentionally lightweight: it compares required ingredient IDs against the chosen inventory set and resolves the result animation and outcome.

This approach keeps game balance and narrative results data-driven while the actual game logic remains simple and fast to adjust.

#### Resource-driven content

The project uses Godot `.tres` resources for data-heavy content:

- `resources/items/*.tres` for item metadata
- `resources/recepies/*.tres` for recipes and outcomes
- `default_bus_layout.tres` for audio mixing
- scenes for assembled gameplay objects and UI layout

This pattern keeps gameplay content editable without needing to recompile the C# code for simple data changes.

## Gameplay loop

The game loop follows this sequence:

1. Player enters the scene and begins exploring
2. Items are collected and tracked in `ItemManager`
3. The global timer starts and gradually darkens the world
4. When the timer reaches its limit, the game enters the crafting phase
5. The player attempts to match a recipe from the collected items
6. Depending on the result, the game plays a result animation and opens the end screen
7. The run can be restarted through `GameMaster.ResetGame()`

## Project structure

```text
.
├── assets/                         # art, fonts, sound, and imported media
├── resources/
│   ├── item_manager.tres
│   ├── items/                     # item definitions
│   └── recepies/                  # recipe definitions and outcomes
├── scenes/                        # level scenes and UI scenes
├── scripts/                       # C# gameplay logic and systems
├── .editorconfig
├── .gitattributes
├── .gitignore
├── Canon Kmicic.csproj
├── Canon Kmicic.sln
├── default_bus_layout.tres
├── export_presets.cfg
├── project.godot
├── icon.svg
└── README.md
```

## Requirements

To develop or build the project, you need:

- Godot 4.x with C# support enabled
- .NET 8 SDK
- a working Godot C# development setup

## Local build commands

Compile the C# project from the command line:

```bash
dotnet build "Canon Kmicic.csproj"
```

If you want to run the project through Godot without the editor UI, use a local Godot installation:

```bash
godot --path .
```

or export a build using the preset configuration in `export_presets.cfg`.

## Notes

- `project.godot` declares autoloads for `AudioManager` and `GameMaster`.
- `GameMaster.Instance.RegisterCrafting(...)` and `RegisterGameOver(...)` are used to wire UI elements into the global flow.
- The project mixes deterministic configuration data in resources with imperative scene logic in C#.
- The end-state flow is intentionally exploit-sensitive: recipe outcome depends on inventory selection and the final timing state.

## License

No explicit license file is present in the repository yet. If you plan to redistribute or publish the project, confirm the intended licensing before doing so.
