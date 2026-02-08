using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using KittsCEventSystem.Exmaple;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using MapGeneration;
using Mirror;
using PlayerRoles.Ragdolls;
using Respawning;
using Respawning.Waves;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KittsCEventSystem.Features.CEvents;

public static class CEventManager
{
    #region Registering
    private readonly static List<CEvent> _registeredCEvents = [];

    private readonly static List<GlobalCEvent> _registeredGlobalCEvents = [];

    /// <summary>
    /// Contains all registered <see cref="CEvent"/>s.
    /// </summary>
    public static IReadOnlyList<CEvent> RegisteredCEvents => _registeredCEvents;

    /// <summary>
    /// Contains all registered <see cref="GlobalCEvent"/>s.
    /// </summary>
    public static IReadOnlyList<GlobalCEvent> RegisteredGlobalCEvents => _registeredGlobalCEvents;

    private static readonly Queue<Assembly> _waitingAssemblies = new();

    /// <summary>
    /// Contains queued <see cref="CEvent"/>s.
    /// </summary>
    /// <remarks>Null being a normal round.</remarks>
    public static Queue<CEvent> QueuedCEvents { get; set; } = new();

    /// <summary>
    /// Current running <see cref="CEvent"/>.
    /// Null being a normal round.
    /// </summary>
#nullable enable
    public static CEvent? CurrentCEvent { get; internal set; }
#nullable disable

    /// <summary>
    /// Register waiting assemblies when plugin is loaded.
    /// </summary>
    internal static void RegisterQueuedAssemblies()
    {
        while (_waitingAssemblies.TryDequeue(out Assembly assembly))
            assembly.Register();
    }

    /// <summary>
    /// Register events in <see cref="Assembly.GetCallingAssembly"/>.
    /// </summary>
    public static void RegisterAllEvents() => Assembly.GetCallingAssembly().Register();

    /// <summary>
    /// Register all events in target <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">Target <see cref="Assembly"/>.</param>
    private static void Register(this Assembly assembly)
    {
        if (KittsCEventSystem.Config == null)
        {
            if (!_waitingAssemblies.Contains(assembly))
                _waitingAssemblies.Enqueue(assembly);

            return;
        }

        try
        {
            Log.Debug("CEventManager.Register", $"Loading assembly {assembly.GetName().Name}..");

            List<CEvent> allCEvents = [.. assembly.GetTypes()
                .Where(t =>
                    typeof(CEvent).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t != typeof(CEvent) &&
                    !typeof(GlobalCEvent).IsAssignableFrom(t) &&
                    (t != typeof(NukeRunExample) || KittsCEventSystem.Config.EnableExamples)
                )
                .Select(t => Activator.CreateInstance(t) as CEvent)
                .Where(e => e != null)
            ];

            List<GlobalCEvent> allGlobalCEvents = [.. assembly.GetTypes()
                .Where(t =>
                    typeof(GlobalCEvent).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t != typeof(GlobalCEvent) &&
                    (t != typeof(GlobalExample) || KittsCEventSystem.Config.EnableExamples)
                )
                .Select(t => Activator.CreateInstance(t) as GlobalCEvent)
                .Where(e => e != null)
            ];

            foreach (CEvent cEvent in allCEvents)
                try
                {
                    cEvent.Register();
                }
                catch (Exception e)
                {
                    Log.Error("CEventManager.Register", $"Error loading CEvent {cEvent.Name}: {e.Message}");
                    Log.Debug("CEventManager.Register", e.ToString());
                }

            foreach (GlobalCEvent globalEvent in allGlobalCEvents)
                try
                {
                    globalEvent.Register();
                }
                catch (Exception e)
                {
                    Log.Error("CEventManager.Register", $"Error loading GlobalCEvent {globalEvent.Name}: {e.Message}");
                    Log.Debug("CEventManager.Register", e.ToString());
                }

            Log.Info("CEventManager.Register", $"Loaded assembly {assembly.GetName().Name}: {_registeredCEvents.Count}/{allCEvents.Count} CEvents registered, {_registeredGlobalCEvents.Count}/{allGlobalCEvents.Count} GlobalCEvents registered");
        }
        catch (Exception e)
        {
            Log.Error("CEventManager.Register", $"Failed to load assembly {assembly.GetName().Name}: {e.Message}");
            Log.Debug("CEventManager.Register", e.ToString());
        }
    }

    /// <summary>
    /// Register a <see cref="CEvent"/>.
    /// </summary>
    /// <param name="cEvent"><see cref="CEvent"/> to register.</param>
    /// <exception cref="ArgumentException">Thrown if event is invalid.</exception>
    public static void Register(this CEvent cEvent)
    {
        try
        {
            if (cEvent == null || !cEvent.RegisterEvent || ((cEvent.GetType() == typeof(NukeRunExample) || cEvent.GetType() == typeof(GlobalExample)) && !KittsCEventSystem.Config.EnableExamples))
                return;

            Log.Debug("CEventManager.Register", $"Loading CEvent {cEvent.Name}...");

            if (cEvent is GlobalCEvent globalCEvent)
            {
                _registeredGlobalCEvents.Add(globalCEvent);
                Log.Debug("CEventManager.Register", $"A GlobalCEvent was registered successfully");
                return;
            }

            if (cEvent.Id == 0)
                throw new ArgumentException("CEvent Id cannot be 0 (reserved for GlobalCEvent)");
            if (_registeredCEvents.Any(c => c.Id == cEvent.Id))
                throw new ArgumentException($"CEvent Id {cEvent.Id} already registered");

            _registeredCEvents.Add(cEvent);
            Log.Debug("CEventManager.Register", $"CEvent {cEvent.Name} registered successfully");
        }
        catch (Exception e)
        {
            Log.Error("CEventManager.Register", $"Failed to load CEvent {cEvent.Name}: {e.Message}");
            Log.Debug("CEventManager.Register", e.ToString());
        }
    }

    /// <summary>
    /// Unregister a <see cref="CEvent"/>.
    /// </summary>
    /// <param name="cEvent">The <see cref="CEvent"/>.</param>
    public static void Unregister(this CEvent cEvent)
    {
        if (_registeredCEvents.Contains(cEvent))
            _registeredCEvents.Remove(cEvent);
    }

    /// <summary>
    /// Unregister all <see cref="CEvent"/>s.
    /// </summary>
    public static void UnregisterAllEvents()
    {
        foreach (CEvent cEvent in _registeredCEvents)
            cEvent.Unregister();

        _registeredCEvents.Clear();
    }

    /// <summary>
    /// Register all <see cref="GlobalCEvent"/>s.
    /// </summary>
    internal static void RegisterAllGlobalEvents()
    {
        foreach (GlobalCEvent globalCEvent in _registeredGlobalCEvents)
            CustomHandlersManager.RegisterEventsHandler(globalCEvent);
    }

    /// <summary>
    /// Unregister all <see cref="GlobalCEvent"/>s.
    /// </summary>
    internal static void UnregisterAllGlobalEvents()
    {
        foreach (GlobalCEvent globalCEvent in _registeredGlobalCEvents)
            CustomHandlersManager.UnregisterEventsHandler(globalCEvent);
    }
    #endregion

    #region Running Events
    /// <summary>
    /// Queue a new <see cref="CEvent"/> to run.
    /// </summary>
    /// <param name="cEvent">Target <see cref="CEvent"/> (or null for normal rounds).</param>
    /// <param name="runIn">Number of rounds to wait before running (must be >= 1).</param>
    /// <param name="position">Where to insert in the queue (1 = front, higher = later, -1 = end).</param>
    /// <remarks>Null entries in the queue represent normal rounds.</remarks>
    public static void QueueCEvent(CEvent cEvent, int runIn = 1, int position = -1)
    {
        if (runIn < 1)
        {
            Log.Error("CEventManager.QueueCEvent", $"Invalid runIn value: {runIn}. Must be >= 1.");
            return;
        }

        if (position == 0 || position < -1)
        {
            Log.Error("CEventManager.QueueCEvent", $"Invalid position value: {position}. Must be >= 1 or -1 (default).");
            return;
        }

        List<CEvent> entries = [.. Enumerable.Repeat<CEvent>(null, runIn - 1)];
        if (cEvent != null) entries.Add(cEvent);

        if (position == -1 || position > QueuedCEvents.Count)
            foreach (var ev in entries)
                QueuedCEvents.Enqueue(ev);
        else
        {
            List<CEvent> tempList = [.. QueuedCEvents];
            tempList.InsertRange(position - 1, entries);
            QueuedCEvents.Clear();
            foreach (CEvent ev in tempList)
                QueuedCEvents.Enqueue(ev);
        }

        Log.Debug("CEventManager.QueueCEvent", $"Queued {cEvent.Name} in {runIn} round(s) at position {position}");
    }

    /// <summary>
    /// Stop the current <see cref="CEvent"/>.
    /// </summary>
    public static void StopCurrentEvent()
    {
        if (CurrentCEvent != null)
        {
            EventCleanup();

            Log.Debug("CEventManager.StopCurrentEvent", null);
        }
    }

    /// <summary>
    /// Switches the <see cref="CurrentCEvent"/> with a new <see cref="CEvent"/>.
    /// </summary>
    /// <param name="new">New <see cref="CEvent"/> to switch to.</param>
    public static void SwitchCEvent(CEvent @new)
    {
        UnregisterAllGlobalEvents();
        CustomHandlersManager.UnregisterEventsHandler(CurrentCEvent);
        CurrentCEvent = null;

        CurrentCEvent = @new;
        CustomHandlersManager.RegisterEventsHandler(@new);
        RegisterAllGlobalEvents();
    }
    #endregion

    #region Misc Utils
    public enum SpawnWaveType
    {
        MTFWave,
        ChaosWave,
        MTFMiniWave,
        ChaosMiniWave
    }

    /// <summary>
    /// 
    /// Locks warhead, disables decon, pauses waves, locks doors, and cleans items.
    /// </summary>
    /// <param name="lockWarhead">Should setup lock the warhead.</param>
    public static void EventSetup(bool lockWarhead = false)
    {
        if (lockWarhead) AlphaWarheadController.Singleton.IsLocked = true;
        Round.IsLocked = true;
        DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
        ServerConsole.FriendlyFire = false;
        ServerConfigSynchronizer.RefreshAllConfigs();

        foreach (SpawnableWaveBase wave in WaveManager.Waves)
            if (wave is TimeBasedWave timeBasedWave)
                timeBasedWave.Timer.Pause(float.PositiveInfinity);

        LockAllDoors();

        CleanupItems();

        Log.Debug("CEventManager.EventSetup", null);
    }

    /// <summary>
    /// Stop the current <see cref="CEvent"/>.
    /// </summary>
    public static void EventCleanup()
    {
        RoundRestart.InitiateRoundRestart();

        Log.Debug("CEventManager.EventCleanup", null);
    }

    /// <summary>
    /// Removes all ragdolls and items from the scene.
    /// </summary>
    public static void CleanupItems()
    {
        BasicRagdoll[] ragdollsArray = UnityEngine.Object.FindObjectsByType<BasicRagdoll>(FindObjectsSortMode.None);
        foreach (BasicRagdoll ragdoll in ragdollsArray)
            NetworkServer.Destroy(ragdoll.gameObject);

        ItemPickupBase[] itemsArray = UnityEngine.Object.FindObjectsByType<ItemPickupBase>(FindObjectsSortMode.None);
        foreach (ItemPickupBase item in itemsArray)
            NetworkServer.Destroy(item.gameObject);

        Log.Debug("CEventManager.CleanupItems", null);
    }

    /// <summary>
    /// Teleports multiple players to a specific door.
    /// </summary>
    /// <param name="players">Players to teleport.</param>
    /// <param name="doorName">Target door.</param>
    public static void TeleportToDoor(this IReadOnlyList<Player> players, DoorName doorName)
    {
        foreach (Player player in players)
            player.TeleportToDoor(doorName);

        Log.Debug("CEventManager.TeleportPlayersToDoor", $"{players.Count} {doorName}");
    }

    /// <summary>
    /// Teleports a single player to a specific door.
    /// </summary>
    /// <param name="player">Player to teleport.</param>
    /// <param name="doorName">Target door.</param>
    public static void TeleportToDoor(this Player player, DoorName doorName)
    {
        Transform tranform = Door.List.FirstOrDefault(d => d.DoorName == doorName).Transform;

        Vector3 vector = tranform.position + Vector3.up;
        vector += tranform.forward * 0.35f;

        player.Position = vector;

        Log.Debug("CEventManager.TeleportToDoor", $"{player} {doorName}");
    }

    /// <summary>
    /// Locks all doors and elevators.
    /// </summary>
    public static void LockAllDoors()
    {
        foreach (Door door in Door.List)
        {
            door.IsOpened = false;
            door.IsLocked = true;
            door.Base.Update();
        }

        Elevator.LockAll();
        Log.Debug("CEventManager.LockAllDoors", null);
    }

    /// <summary>
    /// Unlocks all doors and elevators.
    /// </summary>
    public static void UnlockAllDoors()
    {
        foreach (Door door in Door.List)
        {
            door.IsLocked = false;
            door.Base.Update();
        }

        Elevator.UnlockAll();
        Log.Debug("CEventManager.UnlockAllDoors", null);
    }

    /// <summary>
    /// Unlocks all doors in a given zone.
    /// </summary>
    /// <param name="zone">Zone to unlock.</param>
    public static void Unlock(this FacilityZone zone)
    {
        switch (zone)
        {
            case FacilityZone.None: UnlockAllDoors(); break;
            case FacilityZone.Surface:
                foreach (Door door in Door.List)
                    if (door.DoorName is DoorName.SurfaceGate or
                            DoorName.SurfaceNuke or
                            DoorName.SurfaceEscapePrimary or
                            DoorName.None)
                        door.IsLocked = false;
                break;
            case FacilityZone.Entrance:
                foreach (Door door in Door.List)
                {
                    if (door.DoorName is DoorName.None)
                        door.IsLocked = false;

                    if (door.DoorName is DoorName.EzGateA or
                            DoorName.EzGateB or
                            DoorName.EzIntercom)
                        door.IsOpened = true;
                }
                break;
            case FacilityZone.HeavyContainment:
                foreach (Door door in Door.List)
                {
                    if (door.DoorName is DoorName.None or
                            DoorName.HczHidLower or
                            DoorName.HczHidUpper)
                        door.IsLocked = false;

                    if (door.DoorName is DoorName.HczHidChamber or
                            DoorName.HczArmory or
                            DoorName.Hcz049Armory or
                            DoorName.Hcz079FirstGate or
                            DoorName.Hcz096 or
                            DoorName.Hcz049Armory)
                        door.IsOpened = true;

                    Elevator.UnlockAll();
                }
                break;
            case FacilityZone.LightContainment:
                foreach (Door door in Door.List)
                {
                    if (door.DoorName is DoorName.None or
                            DoorName.LczGr18Gate or
                            DoorName.LczGr18Inner or
                            DoorName.Lcz173Armory or
                            DoorName.Lcz173Gate or
                            DoorName.Lcz173Connector)
                        door.IsLocked = false;

                    if (door.DoorName is DoorName.LczArmory)
                        door.IsOpened = true;
                }
                break;
        }

        Log.Debug("CEventManager.Unlock", zone);
    }

    /// <summary>
    /// Sets the player's ammo to max values.
    /// </summary>
    /// <param name="player">Target player.</param>
    public static void SetMaxAmmo(this Player player)
    {
        Dictionary<ItemType, ushort> maxAmmo = new()
        {
            { ItemType.Ammo9x19, 40 },
            { ItemType.Ammo556x45, 40 },
            { ItemType.Ammo762x39, 40 },
            { ItemType.Ammo44cal, 18 },
            { ItemType.Ammo12gauge, 14 }
        };

        foreach (ItemBase item in player.ReferenceHub.inventory.UserInventory.Items.Values.ToList())
            if (item is InventorySystem.Items.Armor.BodyArmor armor)
            {
                switch (armor.ItemTypeId)
                {
                    case ItemType.ArmorLight:
                        maxAmmo[ItemType.Ammo9x19] = 70;
                        maxAmmo[ItemType.Ammo556x45] = 40;
                        maxAmmo[ItemType.Ammo762x39] = 40;
                        maxAmmo[ItemType.Ammo44cal] = 18;
                        maxAmmo[ItemType.Ammo12gauge] = 14;
                        break;
                    case ItemType.ArmorCombat:
                        maxAmmo[ItemType.Ammo9x19] = 70;
                        maxAmmo[ItemType.Ammo556x45] = 120;
                        maxAmmo[ItemType.Ammo762x39] = 120;
                        maxAmmo[ItemType.Ammo44cal] = 48;
                        maxAmmo[ItemType.Ammo12gauge] = 54;
                        break;
                    case ItemType.ArmorHeavy:
                        maxAmmo[ItemType.Ammo9x19] = 210;
                        maxAmmo[ItemType.Ammo556x45] = 200;
                        maxAmmo[ItemType.Ammo762x39] = 200;
                        maxAmmo[ItemType.Ammo44cal] = 68;
                        maxAmmo[ItemType.Ammo12gauge] = 74;
                        break;
                }
                break;
            }

        foreach (KeyValuePair<ItemType, ushort> ammo in maxAmmo)
            player.SetAmmo(ammo.Key, ammo.Value);

        Log.Debug("PlayerUtils.SetMaxAmmo", player.DisplayName);
    }

    /// <summary>
    /// Spawns a wave of the specified type.
    /// </summary>
    /// <param name="waveType">Type of wave.</param>
    public static void SpawnWave(SpawnWaveType waveType)
    {
        SpawnableWaveBase wave = waveType switch
        {
            SpawnWaveType.MTFWave => WaveManager.Waves.ElementAt(0),
            SpawnWaveType.ChaosWave => WaveManager.Waves.ElementAt(1),
            SpawnWaveType.MTFMiniWave => WaveManager.Waves.ElementAt(2),
            SpawnWaveType.ChaosMiniWave => WaveManager.Waves.ElementAt(3),
            _ => throw new ArgumentOutOfRangeException(nameof(waveType), waveType, null)
        };

        WaveManager.Spawn(wave);
        Log.Debug("CEventManager.SpawnWave", null);
    }

    /// <summary>
    /// Pauses all waves.
    /// </summary>
    public static void PauseAllWaves()
    {
        foreach (TimeBasedWave wave in WaveManager.Waves.Cast<TimeBasedWave>())
            wave.Timer.Pause(float.PositiveInfinity);

        Log.Debug("CEventManager.PauseAllWaves", null);
    }

    /// <summary>
    /// Resets timers and unpauses all waves.
    /// </summary>
    public static void UnpauseAllWaves()
    {
        foreach (SpawnableWaveBase wave in WaveManager.Waves)
            if (wave is TimeBasedWave timeWave)
                timeWave.Timer.Reset();

        Log.Debug("CEventManager.UnpauseAllWaves", null);
    }
    #endregion
}
