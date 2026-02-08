using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using LabApi.Features.Permissions;
using System;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class ClearCEventQueueCommand : ICommand
{
    public string Command { get; } = "clearceventqueue";
    public string[] Aliases { get; } = ["cceq"];
    public string Description { get; } = "Clear the CEvent queue";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasPermissions(KittsCEventSystem.Config.ClearCEventQueuePermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        CEventManager.QueuedCEvents.Clear();
        response = "<color=green>CEvent queue cleared.</color>";
        return true;
    }
}
