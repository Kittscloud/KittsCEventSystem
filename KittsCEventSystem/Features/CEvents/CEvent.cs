using KittsCEventSystem.Features.Models;
using LabApi.Events.CustomHandlers;
using System;

namespace KittsCEventSystem.Features.CEvents;

public abstract class CEvent : CustomEventsHandler
{
    /// <summary>
    /// Gets the name of <see cref="CEvent"/>.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the description of <see cref="CEvent"/>.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets the id of <see cref="CEvent"/>.
    /// </summary>
    public abstract int Id { get; }

    /// <summary>
    /// Gets or sets the amount of player required to run the <see cref="CEvent"/>.
    /// </summary>
    public virtual int PlayersRequired { get; } = -1;

    /// <summary>
    /// Should the <see cref="CEvent"/> be registered in <see cref="CEventManager.RegisteredCEvents"/>.
    /// </summary>
    public virtual bool RegisterEvent { get; } = true;

    /// <summary>
    /// Gets or sets the config for this <see cref="CEvent"/>.
    /// </summary>
    public virtual CEventConfig Config { get; set; }

    /// <summary>
    /// Create a config based on command arguments.
    /// </summary>
    /// <param name="args">Command arguments after eventId.</param>
    /// <param name="error">Displays the error to the executer.</param>
    /// <returns>A new config instance or null to use default.</returns>
    public virtual CEventConfig CreateConfigFromArgs(ArraySegment<string> args, out string error)
    {
        error = null;
        return null;
    }
}
