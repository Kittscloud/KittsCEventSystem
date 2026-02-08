using System.ComponentModel;

namespace KittsCEventSystem;

public class Config
{
    /// <summary>
    /// Is plugin enabled.
    /// </summary>
    [Description("Is plugin enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Sends debug logs to console.
    /// </summary>
    [Description("Sends debug logs to console")]
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Whether in-built example events are enabled.
    /// </summary>
    [Description("Whether in-built example events are enabled")]
    public bool EnableExamples { get; set; } = true;

    /// <summary>
    /// Permission for clear cevent queue command.
    /// </summary>
    [Description("Permission for clear cevent queue command")]
    public string ClearCEventQueuePermission { get; set; } = "kts.clearceventqueue";

    /// <summary>
    /// Permission for list cevents command.
    /// </summary>
    [Description("Permission for list cevents command")]
    public string ListCEventsPermission { get; set; } = "kts.listcevents";

    /// <summary>
    /// Permission for queue cevent command.
    /// </summary>
    [Description("Permission for queue cevent command")]
    public string QueueCEventPermission { get; set; } = "kts.queuecevent";

    /// <summary>
    /// Permission for remove queued cevent command.
    /// </summary>
    [Description("Permission for remove queued cevent command")]
    public string RemoveQueuedCEventPermission { get; set; } = "kts.removequeuedcevent";

    /// <summary>
    /// Permission for stop current cevent command.
    /// </summary>
    [Description("Permission for stop current cevent command")]
    public string StopCurrentCEventPermission { get; set; } = "kts.stopcurrentcevent";

    /// <summary>
    /// Permission for view queued cevent command.
    /// </summary>
    [Description("Permission for view queued cevent command")]
    public string ViewCEventQueuePermission { get; set; } = "kts.viewqueuedcevent";
}
