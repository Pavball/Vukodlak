# Vukodlak

A moderator's companion app for a real-life, Town of Salem-style social deduction party game. One person (the narrator) runs it on a phone: it assigns secret roles, tracks who is alive or dead, and times the day, voting and explanation phases. Players don't use the app, and there is no networking, AI, or automatic night-action logic. The narrator resolves all role abilities by hand.

## Project facts

- **Engine:** Unity 6000.6.8f1, Built-in Render Pipeline, legacy Input Manager (`StandaloneInputModule`).
- **Target platform:** Android (`com.ribcageharmony.vukodlak`, min SDK 26, target SDK 36, IL2CPP). The project uses Unity's default Gradle templates and manifest, so there is no `Assets/Plugins/Android` folder.
- **UI:** uGUI with TextMesh Pro.
- **Scene:** only `Assets/Scenes/SampleScene.unity`. All UI lives here as panels that are switched on and off.
- **Remote:** `https://github.com/Pavball/Vukodlak.git`, default branch `main`.
- **Tests:** none exist. The Test Framework package is installed, but there are no test assemblies and no `.asmdef` files, so all game code compiles into `Assembly-CSharp`.

## Code layout

All scripts are in `Assets/Scripts`, in the global namespace. Each manager is a `MonoBehaviour` on its own scene object.

| Script | Scene object | Tag | Responsibility |
|---|---|---|---|
| `GameManager` | `GameManager` | `GameController` | Switches UI panels through `ChangeToGamemodeUI(int)` |
| `PhaseManager` | `PhaseManager` | `PhaseManager` | Day → Voting → Explanation → Night state machine, timers, votes needed |
| `RoleDistribution` | `RoleDistributionManager` | `RoleDistribution` | Classic mode: uses presets for 10–20 players |
| `CustomRoleManager` | `CustomRoleDistributionManager` | `CustomRoleManager` | Custom mode: narrator picks role counts for 3 or more players |
| `Role` | `Role` | `Role` | Role data, the static `roleDictionary`, presets by player count, shuffle |
| `RoleManager` | `RoleDescriptionManager` | Untagged | Role encyclopedia screen |
| `SettingsManager` | `SettingsManager` | `SettingsManager` | Timer dropdowns, saved to `PlayerPrefs` |
| `PopupManager` | `PopupManager` | `PopupManager` | Animated error dialogs |
| `Utils/DiskRoleLoader` | None (static) | | Loads extra roles from `Assets/Resources/roles.txt` |
| `Utils/PlayerNames` | None (static) | | Parses the name list and finds duplicates for both game modes |

## How things connect

- **Managers find each other by tag:** they call `GameObject.FindGameObjectWithTag(...)` in `Start()`. Renaming or removing a tag breaks this at runtime with no compile error.
- **Most button behaviour is in the scene, not in code:** handlers are persistent `onClick` calls set in the Inspector. Renaming a public method silently breaks them, so search `SampleScene.unity` for the method name first. Code-wired exceptions are `GeneratePlayerCountBTN` (`CustomRoleManager.Start`) and the runtime-created role and player entries.
- **`ChangeToGamemodeUI` codes are also set in the Inspector:**

| Code | Result |
|---|---|
| 0 | Main menu (also clears role tables) |
| 1 | Classic setup |
| 2 | Custom setup |
| 3 | Role descriptions |
| 4 | Custom role picker |
| 5 | Back from role picker to custom setup |
| 8 | Game table |
| 10 / 11 | Game info open / close |
| 12 | Settings |

- **Game start:** both modes open the game table from code, with `ChangeToGamemodeUI(8)` then `PhaseManager.InitializeGame`. Classic does this in `RoleDistribution.GenerateRoles`, Custom in `CustomRoleManager.RandomizeRoles`, which is called by `GeneratePlayerTableBTN`.
- **Player names:** kept as typed, apart from surrounding spaces. Duplicates are matched ignoring case.
- **The alive and dead lists are the game state:** a player is an instantiated `V2NameRolePrefab` parented to the alive or graveyard scroll view content. Counts and votes needed come from `childCount`. Votes needed is `ceil(alive / 2)`.
- **Timers use `Invoke`:** `_tick` and `_explanationTick` reschedule themselves. They are called by string name, so renaming them needs the string updated as well.

## Adding roles

Roles come from two places:

1. The hardcoded dictionary in `Role.LoadRoles`. New built-in roles go here.
2. `Assets/Resources/roles.txt`, loaded by `DiskRoleLoader`. Each entry is separated by `##` and has 7 lines: Name, Faction, Description, Goal, Type (`Good`/`Evil`/`Neutral`), Color (`R,G,B,A`), IsDead. The first block is a description of the format and is skipped.

The file ships inside the build, so players can't edit it on their phones. A malformed entry or a duplicate role name is skipped with a console warning. Any line ending works.

Roles load the first time `Role.roleDictionary` is read, not in a static constructor, because `Resources.Load` isn't allowed during scene load. `RoleSelectorDropdown` is filled from this dictionary at runtime. To add a built-in role to Classic mode, also update the preset lists in `Role.FetchPresetByPlayerCount`.

## Known issues

Found by reading the code, not yet reproduced:

- **`Role` is a `MonoBehaviour` created with `new`.** Unity warns on every instance, and it should become a plain class or `ScriptableObject`. These objects compare equal to `null` in Unity, so never null-check a `Role`.
- **`PhaseManager.InitializeGame` sets `dayCounter = 1`, while `ResetGame` sets it to 0.** Check both paths when changing day numbering.

Naming oddities to keep as they are unless deliberately migrating:

- The `PlayerPrefs` key `NightDurationDropdownIndex` actually stores the voting duration. Renaming it loses saved settings.
- The timer dropdowns' `5 sec` option looks like a leftover test value, and it's the default.

## Repository state

- **`Assets/TextMesh Pro/Examples & Extras` isn't in git.** Nothing in the project uses it, so keep it out of commits.
- **Android build status:** a debug-signed APK builds on Unity 6 in about 13 minutes on a clean build. It hasn't been run on a phone, and a signed build hasn't been tried.
- **`user.keystore` is tracked in git.** Never print, move, or modify it.
- **`.roles.txt` at the repo root is an empty tracked file.** The loader doesn't use it.

## Working in this project

- **Use the Unity MCP connection** to read the console, inspect the scene, and run editor commands. The editor must be open with this project loaded.
- **Don't hand-edit `.unity`, `.prefab`, or `.asset` YAML.** Change them through the editor or Unity MCP so references stay intact.
- **Keep `.meta` files with their assets** when adding, moving, or deleting files. Let Unity generate them.
- **Editing a script drops the Unity MCP connection** while the editor recompiles. Wait for the editor to finish before calling Unity tools again.
- **Roles are cached after first use,** so `roles.txt` changes need a domain reload to show up.
- **Builds are done from the Unity Editor** (Build Profiles). There is no CLI build or CI. Android signing needs the keystore passwords, which aren't stored in the project.
