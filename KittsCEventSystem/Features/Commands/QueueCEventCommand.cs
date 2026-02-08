using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using KittsCEventSystem.Features.Models;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using System;
using System.Linq;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class QueueCEventCommand : ICommand
{
    public string Command { get; } = "queuecevent";
    public string[] Aliases { get; } = ["qec"];
    public string Description { get; } = "Queue a CEvent";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Player.TryGet(sender, out Player player))
        {
            response = "<color=red>You must be a player to run this command.</color>";
            return false;
        }

        if (!player.HasPermissions(KittsCEventSystem.Config.QueueCEventPermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "<color=orange>Usage: ceventqueue <eventId|null> [config...] [runIn] [position]</color>";
            return false;
        }

        int runIn = 1;
        int position = -1;

        if (arguments.At(0).Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            if (arguments.Count > 1)
            {
                response = "<color=red>No additional arguments allowed for null event.</color>";
                return false;
            }

            CEventManager.QueueCEvent(null, runIn, position);
            response = "<color=green>Queued normal round.</color>";
            return true;
        }

        if (!int.TryParse(arguments.At(0), out int eventId))
        {
            response = "<color=red>Invalid event ID.</color>";
            return false;
        }

        CEvent baseEvent = CEventManager.RegisteredCEvents.FirstOrDefault(e => e.Id == eventId);
        if (baseEvent == null)
        {
            response = "<color=red>Event not found.</color>";
            return false;
        }

        int index = 1;
        int expectedConfigArgs = baseEvent.Config.ExpectedArgs;

        if (arguments.Count < index + expectedConfigArgs)
        {
            response =
                $"<color=orange>Usage: ceventqueue <eventId|null> {baseEvent.Config.Usage} [runIn] [position]</color>";
            return false;
        }

        ArraySegment<string> configArgs = expectedConfigArgs > 0
            ? new ArraySegment<string>(arguments.Array, arguments.Offset + index, expectedConfigArgs)
            : default;

        index += expectedConfigArgs;

        if (arguments.Count > index)
        {
            if (!int.TryParse(arguments.At(index), out runIn) || runIn < 1)
            {
                response = "<color=red>runIn must be an integer >= 1.</color>";
                return false;
            }

            index++;
        }

        if (arguments.Count > index)
        {
            if (!int.TryParse(arguments.At(index), out position) || (position != -1 && position < 1))
            {
                response = "<color=red>position must be -1 or >= 1.</color>";
                return false;
            }

            index++;
        }

        if (arguments.Count > index)
        {
            response = "<color=red>Too many arguments.</color>";
            return false;
        }

        CEventConfig config = baseEvent.CreateConfigFromArgs(configArgs, out string error);
        if (!string.IsNullOrEmpty(error))
        {
            response =
                $"<color=orange>Usage: ceventqueue <eventId|null> {baseEvent.Config.Usage} [runIn] [position]</color>\n{error}";
            return false;
        }

        CEvent instance = (CEvent)Activator.CreateInstance(baseEvent.GetType());
        instance.Config = config ?? baseEvent.Config;

        CEventManager.QueueCEvent(instance, runIn, position);

        response = $"<color=green>Queued event {instance.Name}.</color>";
        return true;
    }
}
