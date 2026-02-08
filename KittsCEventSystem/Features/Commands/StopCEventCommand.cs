using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using LabApi.Features.Permissions;
using System;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class StopCEventCommand : ICommand
{
    public string Command { get; } = "stopcevent";
    public string[] Aliases { get; } = ["sce"];
    public string Description { get; } = "Stop the current CEvent";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasPermissions(KittsCEventSystem.Config.StopCurrentCEventPermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        if (CEventManager.CurrentCEvent == null)
        {
            response = "<color=yellow>No event is currently running.</color>";
            return false;
        }

        CEventManager.StopCurrentEvent();
        response = "<color=green>Current event stopped.</color>";
        return true;
    }
}

