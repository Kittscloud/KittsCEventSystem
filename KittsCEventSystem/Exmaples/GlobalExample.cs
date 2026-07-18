using KittsCEventSystem.Features.CEvents;
using LabApi.Events.Arguments.PlayerEvents;

namespace KittsCEventSystem.Exmaples;

internal class GlobalExample : GlobalCEvent
{
    // This is an example of a GlobalCEvent, pretty much whatever event you override in this
    // will always run in any CEvent, this can be really good if you wanted to do things such as below
    public override void OnPlayerEscaping(PlayerEscapingEventArgs ev)
    {
        // Stops the player from escaping in any event
        ev.IsAllowed = false;

        // If you wanted this to run on all events excluding one or some then just make an if statement
        if (CEventManager.CurrentCEvent is NukeRunConfigExample) // Will allow escaping on NukeRunConfigExmaple but nothing else
            ev.IsAllowed = true;
    }

    // You should only have one class inheriting the GlobalCEvent but you can have as many as you want if you want to split it up
}
