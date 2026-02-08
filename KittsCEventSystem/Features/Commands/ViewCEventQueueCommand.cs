using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using LabApi.Features.Permissions;
using System;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class ViewCEventQueueCommand : ICommand
{
    public string Command { get; } = "viewceventqueue";
    public string[] Aliases { get; } = ["vceq"];
    public string Description { get; } = "View the CEvent queue";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasPermissions(KittsCEventSystem.Config.ViewCEventQueuePermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        if (CEventManager.QueuedCEvents.Count == 0)
        {
            response = "<color=yellow>The event queue is empty.</color>";
            return true;
        }

        int index = 1;
        response = "<color=green>CEvent Queue:</color>\n";

        if (CEventManager.CurrentCEvent != null)
            response += $"<color=green>Current Event: {CEventManager.CurrentCEvent.Name}</color>\n";

        foreach (CEvent ev in CEventManager.QueuedCEvents)
        {
            response += $"{index}. {(ev == null ? "Normal Round" : ev.Name)}\n";
            index++;
        }

        return true;
    }
}
