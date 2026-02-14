# KittsCEventSystem
*LabAPI Custom Events Framework*

[![License](https://img.shields.io/badge/License-AGPL%20v3.0-blue?style=for-the-badge)](https://github.com/Kittscloud/KittsCEventSystem/blob/main/LICENSE) [![Downloads](https://img.shields.io/github/downloads/Kittscloud/KittsCEventSystem/total?style=for-the-badge)](https://github.com/Kittscloud/ServerSpecificsSyncer/releases/latest) [![GitHub release](https://img.shields.io/github/v/release/Kittscloud/KittsCEventSystem?style=for-the-badge)](https://github.com/Kittscloud/KittsCEventSystem/releases/latest) [![](https://img.shields.io/badge/.NET-4.8.1-512BD4?logo=dotnet&logoColor=fff&style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481) [![GitHub stars](https://img.shields.io/github/stars/Kittscloud/KittsCEventSystem?style=for-the-badge)](https://github.com/Kittscloud/KittsCEventSystem/stargazers) [![GitHub issues](https://img.shields.io/github/issues/Kittscloud/KittsCEventSystem?style=for-the-badge)](https://github.com/Kittscloud/KittsCEventSystem/issues)

`KittsCEventSystem` is a framework that makes adding event easy for `SCP Secret Laboratory` using `LabAPI`.

## Consider Supporting?
If you enjoy this project and would like to support future development, I would greatly appreciate it if you considered donating via my [Ko-Fi](https://ko-fi.com/kittscloud).

## How to use KittsCEventSystem:
To install `KittsCEventSystem` on your server, you will need:
- `KittsCEventSystem` latest version.

All of these files can be found in the [latest release](https://github.com/Kittscloud/KittsCEventSystem/releases/latest).

Once you have these:
- Place `KittsCEventSystem.dll` in the `plugins` folder.

Run the server and you're set!

### Configurations:
| Parameter                      | Type     | Description                                               | Default Value              |
|--------------------------------|----------|-----------------------------------------------------------|----------------------------|
| `IsEnabled`                    | `bool`   | Is plugin enabled.                                        | `true`                     |
| `Debug`                        | `bool`   | Sends debug logs to console.                              | `false`                    |
| `EnableExamples`               | `bool`   | Whether example menus in-built to the plugin are enabled. | `true`                     |
| `ClearCEventQueuePermission`   | `string` | Permission for clear cevent queue command.                | `"kts.clearceventqueue"`   |
| `ListCEventsPermission`        | `string` | Permission for list cevents command.                      | `"kts.listcevents"`        |
| `QueueCEventPermission`        | `string` | Permission for queue cevent command.                      | `"kts.queuecevent"`        |
| `RemoveQueuedCEventPermission` | `string` | Permission for remove queued cevent command.              | `"kts.removequeuedcevent"` |
| `StopCurrentCEventPermission`  | `string` | Permission for stop current cevent command.               | `"kts.stopcurrentcevent"`  |
| `ViewCEventQueuePermission`    | `string` | Permission for view queued cevent command.                | `"kts.viewqueuedcevent"`   |

### Default YML Config File:
```yml
# Is plugin enabled
is_enabled: true
# Sends debug logs to console
debug: false
# Whether in-built example events are enabled
enable_examples: true
# Permission for clear cevent queue command
clear_c_event_queue: kts.clearceventqueue
# Permission for list cevents command
list_c_events: kts.listcevents
# Permission for queue cevent command
queue_c_event: kts.queuecevent
# Permission for remove queued cevent command
remove_queued_c_event: kts.removequeuedcevent
# Permission for stop current cevent command
stop_current_c_event: kts.stopcurrentcevent
# Permission for view queued cevent command
view_c_event_queue: kts.viewqueuedcevent
```

### Want to use in your own project?
To install in your project, simply reference the `KittsCEventSystem.dll` file, found in the [latest release](https://github.com/Kittscloud/KittsCEventSystem/releases/latest).

Create a new class inheriting the `CEvent` class.

### Example CEvent
```csharp
public class Test : CEvent
{
    public override string Name { get; } = "Name Here";

	public override string Description { get; } = "Description Here";

	public override int Id { get; } = 1;
	
	// Your custom events below
}
```

### CEvent Class
| Parameter / Method                                       | Type / Return Type | Description                                                              |
|----------------------------------------------------------|--------------------|--------------------------------------------------------------------------|
| `Name`                                                   | `string`           | Gets the name of `CEvent`.                                               |
| `Description`                                            | `string`           | Gets the description of `CEvent`.                                        |
| `Id`                                                     | `int`              | ID of the menu. Must be greater than 0.                                  |
| `PlayersRequired`                                        | `int`              | Gets or sets the amount of player required to run the `CEvent`.          |
| `RegisterEvent`                                          | `bool`             | Should the `CEvent` be registered in `CEventManager.RegisteredCEvents`.  |
| `Config`                                                 | `CEventConfig`     | Gets or sets the config for this `CEvent`.                               |
| `CreateConfigFromArgs(ArraySegment<string>, out string)` | `CEventConfig`     | Create a config based on command arguments.                              |

### GlobalCEvent Class
Works the same as a `CEvent` however the parameters are already set and configs don't get used if defined, they are only for puttint in override of events.

### CEventManager Class
| Parameter / Method                                | Type / Return Type            | Description                                                                 |
|---------------------------------------------------|-------------------------------|-----------------------------------------------------------------------------|
| `RegisteredCEvents`                               | `IReadOnlyList<CEvent>`       | Contains all registered `CEvents`.                                          |
| `RegisteredGlobalCEvents`                         | `IReadOnlyList<GlobalCEvent>` | Contains all registered `GlobalCEvents`.                                    |
| `QueuedCEvents`                                   | `Queue<CEvent>`               | Contains queued `CEvents`.                                                  |
| `CurrentCEvent`                                   | `CEvent`                      | Current running `CEvents`.                                                  |
| `RegisterAllEvents()`                             | `void`                        | Register events in `Assembly.GetCallingAssembly`.                           |
| `Register(Assembly)`                              | `void`                        | Register all events in target `Assembly`.                                   |
| `Register(CEvent)`                                | `void`                        | Register a `CEvent`.                                                        |
| `Unregister(CEvent)`                              | `void`                        | Unregister a `CEvent`.                                                      |
| `UnregisterAllEvents()`                           | `void`                        | Unregister all `CEvents`.                                                   |
| `QueueCEvent(CEvent, int, int)`                   | `Menu`                        | Queue a new `CEvent` to run.                                                |
| `StopCurrentEvent()`                              | `void`                        | Stop the current `CEvent`.                                                  |
| `SwitchCEvent(CEvent)`                            | `void`                        | Switches the `CurrentCEvent` with a new `CEvent`.                           |
| `EventSetup()`                                    | `void`                        | Locks warhead, disables decon, pauses waves, locks doors, and cleans items. |
| `EventCleanup()`                                  | `void`                        | Stop the current `CEvent`.                                                  |
| `CleanupItems()`                                  | `void`                        | Removes all ragdolls and items from the scene.                              |
| `TeleportToDoor(IReadOnlyList<Player>, DoorName)` | `void`                        | Teleports multiple players to a specific door.                              |
| `TeleportToDoor(Player, DoorName)`                | `void`                        | Teleports a single player to a specific door.                               |
| `LockAllDoors()`                                  | `void`                        | Locks all doors and elevators.                                              |
| `UnlockAllDoors()`                                | `void`                        | Unlocks all doors and elevators.                                            |
| `Unlock(FacilityZone)`                            | `void`                        | Unlocks all doors in a given zone.                                          |
| `SetMaxAmmo(Player)`                              | `void`                        | Sets the player's ammo to max values.                                       |
| `SpawnWave(SpawnWaveType)`                        | `void`                        | Spawns a wave of the specified type.                                        |
| `PauseAllWaves()`                                 | `void`                        | Pauses all waves.                                                           |
| `UnpauseAllWaves()`                               | `void`                        | Resets timers and unpauses all waves.                                       |

You can look at the [Examples](https://github.com/Kittscloud/KittsCEventSystem/tree/main/KittsCEventSystem/Examples) folder to get a better idea of how to create a `CEvent`, `CEvent` with a config and a `GlobalCEvent`.

When you enable your plugin, simply run:
```csharp
CEventManager.RegisterAllEvents();
```
This will register all `CEvents` in your assembly. It is important to keep `KittsCEventSystem.dll` in the `plugins` folder, as it must run as a plugin in order to register menus from all assemblies with `KittsCEventSystem.dll`.

## Found a bug or have feedback?
If you have found a bug please make an issue on GitHub or the quickest way is to message me on discord at `kittscloud`.

Also message me on discord if you have feedback for me, I'd appreciate it very much. Thank you!
