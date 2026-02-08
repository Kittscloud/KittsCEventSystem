using KittsCEventSystem.Features;
using KittsCEventSystem.Features.CEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace KittsCEventSystem.Exmaple;

internal class NukeRunExample : CEvent
{
    // Here is the name of the event, it is required
    public override string Name { get; } = "Nuke Run Exmaple";

    // Here you can define the description of the plugin, or you can leave it blank or remove it.
    public override string Description { get; } = "Players will run to the exit from the d-class cells while nuke goes off.";

    // The id must be unique and is used to run an event
    public override int Id { get; } = 1;

    // Setting the required players to -1 or removing it means it does not matter
    public override int PlayersRequired { get; } = -1;

    // So, to explain it in an easy way, you inherit the CEvent class which then gives you access
    // to all event in the game, simply type public override and see all the events
    // This CEvent is for nuke run, so below are the events to override to start that event
    public override void OnServerRoundStarted()
    {
        // Use the event utils to make setup easier
        // This function here will lock warhead, disable decontamination, pause waves, lock doors, and clean items
        CEventManager.EventSetup();

        // Set the player's role and give them the items they need
        foreach (Player p in Player.ReadyList)
        {
            p.SetRole(RoleTypeId.ClassD, flags: RoleSpawnFlags.UseSpawnpoint);
            p.ClearInventory();

            for (int i = 0; i < 3; i++)
                p.AddItem(ItemType.SCP207);

            for (int i = 0; i < 5; i++)
                p.AddItem(ItemType.Medkit);
        }

        Elevator.UnlockAll(); // Unlock as warhead doesn't do it automatically

        // Start the nuke
        AlphaWarheadController.Singleton.IsLocked = false;
        AlphaWarheadController.Singleton.StartDetonation(true);

        Log.Debug("NukeRunEvent.OnServerRoundStarted", null);
    }

    public override void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
    {
        List<Player> alivePlayers = [.. Player.ReadyList.Where(p => p.IsAlive && p.IsHuman && !p.IsTutorial)];

        List<Player> deadPlayers = [.. Player.ReadyList.Where(p => !p.IsAlive || p.IsSCP)];

        if (alivePlayers.Count == 0)
        {
            foreach (Player p in Player.ReadyList)
                p.SendHint("Nobody won the event!", 5f);

            // Run the event cleanup function, which pretty much just restarts the round
            Timing.CallDelayed(5f, CEventManager.EventCleanup);
        }
        else
        {
            foreach (Player p in Player.ReadyList)
                p.SendHint($"{string.Join(", ", alivePlayers.Select(p => p.DisplayName))} won the event!", 5f);

            // Run the event cleanup function, which pretty much just restarts the round
            Timing.CallDelayed(5f, CEventManager.EventCleanup);
        }

        Log.Debug("NukeRunEvent.OnWarheadDetonated", null);
    }
}
