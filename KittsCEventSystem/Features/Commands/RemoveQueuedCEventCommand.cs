using CommandSystem;
using KittsCEventSystem.Features.CEvents;
using LabApi.Features.Permissions;
using System;
using System.Collections.Generic;

namespace KittsCEventSystem.Features.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal sealed class RemoveQueuedCEventCommand : ICommand
{
    public string Command { get; } = "removequeuedcevent";
    public string[] Aliases { get; } = ["rqce"];
    public string Description { get; } = "Remove an entry from the CEvent queue";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasPermissions(KittsCEventSystem.Config.RemoveQueuedCEventPermission))
        {
            response = "<color=red>You do not have permission.</color>";
            return false;
        }

        if (arguments.Count != 1 || !int.TryParse(arguments.At(0), out int position))
        {
            response = "<color=orange>Usage: ceventqueueremove <position></color>";
            return false;
        }

        if (position < 1 || position > CEventManager.QueuedCEvents.Count)
        {
            response = "<color=red>Invalid queue position.</color>";
            return false;
        }

        List<CEvent> temp = [.. CEventManager.QueuedCEvents];
        temp.RemoveAt(position - 1);

        CEventManager.QueuedCEvents.Clear();
        foreach (CEvent ev in temp)
            CEventManager.QueuedCEvents.Enqueue(ev);

        response = "<color=green>Removed queued entry.</color>";
        return true;
    }
}

