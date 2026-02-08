using KittsCEventSystem.Features.CEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using System.Linq;

namespace KittsCEventSystem.Features;

internal class CEventsHandler : CustomEventsHandler
{
    public override void OnServerRoundRestarted()
    {
        if (CEventManager.CurrentCEvent != null)
        {
            CEventManager.SwitchCEvent(null);
            Log.Debug("CEventsHandler.OnServerRoundRestarted", "Unregistered events");
        }

        if (!CEventManager.QueuedCEvents.IsEmpty())
        {
            CEvent nextEvent = CEventManager.QueuedCEvents.Dequeue();
            if (nextEvent != null)
            {
                CEventManager.SwitchCEvent(nextEvent);
                Log.Debug("CEventsHandler.OnServerRoundRestarted", $"Registered event {nextEvent.Name} ({nextEvent.Id})");
            }
        }
    }

    public override void OnServerRoundStarted()
    {
        CEvent current = CEventManager.CurrentCEvent;

        if (current != null && current.PlayersRequired > Player.ReadyList.Count())
        {
            CEventManager.SwitchCEvent(null);

            Log.Warn("CEventsHandler.OnServerRoundStarted", $"{current.Name} ({current.Id}) could not run because there were not enough players ({Player.ReadyList.Count()}/{current.PlayersRequired})");
        }
    }
}
