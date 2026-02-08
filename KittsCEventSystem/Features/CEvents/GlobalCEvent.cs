namespace KittsCEventSystem.Features.CEvents;

public abstract class GlobalCEvent : CEvent
{
    /// <inheritdoc/>
    public sealed override string Name => "GlobalCEvent";

    /// <inheritdoc/>
    public sealed override string Description => "GlobalCEvent";

    /// <inheritdoc/>
    public sealed override int Id => 0;
}
