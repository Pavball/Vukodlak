# Vukodlak

A moderator's companion app for a real-life, Town of Salem-style social deduction party game. One person (the narrator) runs it on a phone: it assigns secret roles, tracks who is alive or dead, and times the day, voting and explanation phases. Players don't use the app, and there is no networking, AI, or automatic night-action logic. The narrator resolves all role abilities by hand.

## Project facts

- **Engine:** Unity 6000.6.8f1, Built-in Render Pipeline, legacy Input Manager (`StandaloneInputModule`).
- **Target platform:** Android (`com.ribcageharmony.vukodlak`, min SDK 26, IL2CPP). The editor's active build target is currently StandaloneWindows64.
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
| `CustomRoleManager` | `CustomRoleDistributionManager` | `CustomRoleManager` | Custom mode: narrator picks role counts for any player count |
| `Role` | `Role` | `Role` | Role data, the static `roleDictionary`, presets by player count, shuffle |
| `RoleManager` | `RoleDescriptionManager` | Untagged | Role encyclopedia screen |
| `SettingsManager` | `SettingsManager` | `SettingsManager` | Timer dropdowns, saved to `PlayerPrefs` |
| `PopupManager` | `PopupManager` | `PopupManager` | Animated error dialogs |
| `Utils/DiskRoleLoader` | None (static) | | Loads extra roles from `Assets/roles.txt` |

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

- **Custom game start:** `GeneratePlayerTableBTN` runs three Inspector calls in order: `CustomRoleManager.RandomizeRoles`, `GameManager.ChangeToGamemodeUI(8)`, `PhaseManager.InitializeGame`.
- **The alive and dead lists are the game state:** a player is an instantiated `V2NameRolePrefab` parented to the alive or graveyard scroll view content. Counts and votes needed come from `childCount`. Votes needed is `ceil(alive / 2)`.
- **Timers use `Invoke`:** `_tick` and `_explanationTick` reschedule themselves. They are called by string name, so renaming them needs the string updated as well.

## Adding roles

Roles come from two places:

1. The hardcoded dictionary in the static `Role` constructor. New built-in roles go here.
2. `Assets/roles.txt`, loaded by `DiskRoleLoader`. Each entry is separated by `##` and has 7 lines: Name, Faction, Description, Goal, Type (`Good`/`Evil`/`Neutral`), Color (`R,G,B,A`), IsDead. The first block is a description of the format and is skipped.

To add a built-in role, also update the preset lists in `Role.FetchPresetByPlayerCount` if it belongs in Classic mode, and the options of `RoleSelectorDropdown` in the scene. That dropdown is hardcoded, so `roles.txt` roles never appear in it.

## Known issues

Found by reading the code, not yet reproduced:

- **`roles.txt` on Android:** the loader reads from `Application.dataPath`, which is inside the APK on Android, and the file isn't packaged into builds. Creating it there is likely to throw in the same static constructor. Use `Resources`, `StreamingAssets`, or `persistentDataPath` instead.
- **`Role.CheckDuplicateRoles` removes items from the list it is looping over**, which throws if `roles.txt` contains a duplicate of a built-in role.
- **`Role` is a `MonoBehaviour` created with `new`.** Unity warns on every instance, and it should become a plain class or `ScriptableObject`.
- **`RoleDistribution.GenerateRoles` clears the alive list twice and never clears the graveyard.** It's a copy-paste slip, currently masked because the Back button clears both.
- **Custom mode has no player count validation**, unlike Classic mode's 10–20 limit.
- **Classic mode lowercases player names; Custom mode doesn't.**
- **`ButtonToTestDictionary` (inactive) points at `CustomRoleManager.ReadDictionary`**, which is commented out.
- **`PhaseManager.InitializeGame` sets `dayCounter = 1`, while `ResetGame` sets it to 0.** Check both paths when changing day numbering.

Naming oddities to keep as they are unless deliberately migrating:

- The `PlayerPrefs` key `NightDurationDropdownIndex` actually stores the voting duration. Renaming it loses saved settings.
- The timer dropdowns' `5 sec` option looks like a leftover test value, and it's the default.

## Repository state

- **Unity upgrade in progress:** `main` is on 2022.3.10f1, and the working tree holds an uncommitted upgrade to 6000.6.8f1 (`Packages/`, `ProjectSettings/`, many TextMesh Pro files). Keep unrelated changes out of that diff.
- **Android Gradle templates predate the upgrade:** `Assets/Plugins/Android/*.gradle` hardcode compile SDK 33 and target SDK 33, while Player Settings says target 32. Android builds on Unity 6 haven't been verified.
- **`user.keystore` is tracked in git.** Never print, move, or modify it.
- **`.roles.txt` at the repo root is an empty tracked file.** The loader doesn't use it.

## Working in this project

- **Use the Unity MCP connection** to read the console, inspect the scene, and run editor commands. The editor must be open with this project loaded.
- **Don't hand-edit `.unity`, `.prefab`, or `.asset` YAML.** Change them through the editor or Unity MCP so references stay intact.
- **Keep `.meta` files with their assets** when adding, moving, or deleting files. Let Unity generate them.
- **Static initialisation failures poison the type** until the next domain reload, so one bad `roles.txt` entry disables every role until scripts reload.
- **Builds are done from the Unity Editor** (Build Profiles). There is no CLI build or CI. Android signing needs the keystore passwords, which aren't stored in the project.
