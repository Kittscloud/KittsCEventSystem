namespace KittsCEventSystem.Features.Models;

/// <summary>
/// Base config for CEvents.
/// </summary>
public abstract record CEventConfig
{
    /// <summary>
    /// Defines how many arguments the event expects when running the command.
    /// </summary>
    public abstract int ExpectedArgs { get; }

    /// <summary>
    /// Shows what arguments the config expects for the command.
    /// </summary>
    public abstract string Usage { get; }
}
