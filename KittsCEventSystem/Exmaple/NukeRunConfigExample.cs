using KittsCEventSystem.Features;
using KittsCEventSystem.Features.CEvents;
using KittsCEventSystem.Features.Models;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KittsCEventSystem.Exmaple;

internal class NukeRunConfigExample : CEvent
{
    // See NukeRunExmaple.cs first.

    // But wait, what if you want to parse certain configurations to your event?
    // Well, that can be done very easily, this example shows how to make a config for your events

    public override string Name { get; } = "Nuke Run Config Exmaple";
    public override string Description { get; } = "Players will run to the exit from the d-class cells while nuke goes off. And you can config what they get.";
    public override int Id { get; } = 2;
    public override int PlayersRequired { get; } = -1;

    // First you must create you config of what you want to be dynamic
    // In this case we will be making the amount of medkits and colas dynamic
    // You config must also inherit the CEventConfig record
    public sealed record NukeRunConfig(
        int Medkits,
        int Colas
    ) : CEventConfig
    {
        // Here you put how many args the command is expecting, args are seperated by a space
        public override int ExpectedArgs => 2;

        // In here you will put what will be appended to the end of the command usage
        // This is so people using the command know what arguments they must parse in to run the event
        public override string Usage => "<medkits> <colas>";
    }

    // Basically, when running a command it will call this function, this function uses the config arguments from the command
    // Using those args it must create a config to use for the event
    // In this function below we take in two args, make sure they're ints and create a new config from those args
    // This is a basic run down of how to make configs for your events
    public override CEventConfig CreateConfigFromArgs(ArraySegment<string> args, out string error)
    {
        error = null;

        // Pretty much if there is something wrong with the args you change the error to your message
        // And then you return the default config
        // How this function works, if you return an error message (not null) then it means there was an error running the command
        // If there was an error then it will display the usage and error and the event wont run
        // It is important you include these checks such as making sure it has the right amount of args and that they're ints if using ints
        // If you dont then you will most likely get errors
        if (!int.TryParse(args.At(0), out int medkits))
        {
            error = "<color=red>Medkits must be an integer.</color>";
            return Config;
        }

        if (!int.TryParse(args.At(1), out int colas))
        {
            error = "<color=red>Colas must be an integer.</color>";
            return Config;
        }

        // I would put a check in to make sure that there aren't more items than inventrory slots
        // But you should understand the idea of CEventConfig

        // If the arguments all check out then create a new config and return it
        return new NukeRunConfig(medkits, colas);
    }

    // This is used to define a default config
    // If you plan to use config I would always have a default so it doesn't throw errors
    // Also, if you do not enter anything in the config section of the command then it will just be the default
    public override CEventConfig Config { get; set; } = new NukeRunConfig(5, 3);

    // Below is the same as the Nuke Run Exmaple execpt when giving colas and medkits we use the Config.Colas and Config.Medkits
    public override void OnServerRoundStarted()
    {
        CEventManager.EventSetup();

        foreach (Player p in Player.ReadyList)
        {
            p.SetRole(RoleTypeId.ClassD, flags: RoleSpawnFlags.UseSpawnpoint);
            p.ClearInventory();

            for (int i = 0; i < (Config as NukeRunConfig).Colas; i++)
                p.AddItem(ItemType.SCP207);

            for (int i = 0; i < (Config as NukeRunConfig).Medkits; i++)
                p.AddItem(ItemType.Medkit);
        }

        Elevator.UnlockAll();

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

            Timing.CallDelayed(5f, CEventManager.EventCleanup);
        }
        else
        {
            foreach (Player p in Player.ReadyList)
                p.SendHint($"{string.Join(", ", alivePlayers.Select(p => p.DisplayName))} won the event!", 5f);

            Timing.CallDelayed(5f, CEventManager.EventCleanup);
        }

        Log.Debug("NukeRunEvent.OnWarheadDetonated", null);
    }
}
