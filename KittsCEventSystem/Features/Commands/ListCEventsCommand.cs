using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using LabApi.Features.Permissions;
using System;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class ListCEventsCommand : ICommand
{
    public string Command { get; } = "listcevents";
    public string[] Aliases { get; } = ["lces"];
    public string Description { get; } = "List registered CEvents";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasPermissions(KittsCEventSystem.Config.ListCEventsPermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        if (CEventManager.RegisteredCEvents.Count == 0)
        {
            response = "<color=yellow>No registered events.</color>";
            return true;
        }

        response = "<color=green>Registered Events:</color>\n";
        foreach (CEvent ev in CEventManager.RegisteredCEvents)
        {
            response += $"<color=green>{ev.Id}</color>: <color=#00FFFF>{ev.Name}</color>\n";
            if (!ev.Description.IsEmpty())
                response += $"<color=yellow>{ev.Description}</color>\n";
        }

        return true;
    }
}

