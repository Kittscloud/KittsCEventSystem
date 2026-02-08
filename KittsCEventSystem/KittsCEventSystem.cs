using KittsCEventSystem.Features;
using KittsCEventSystem.Features.CEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using System;

namespace KittsCEventSystem;

public class KittsCEventSystem : Plugin
{
    public static Plugin Instance { get; private set; }

    public override string Name { get; } = "KittsCEventSystem";
    public override string Author { get; } = "Kittscloud";
    public override string Description { get; } = "";
    public override LoadPriority Priority { get; } = LoadPriority.Lowest;

    public override Version Version { get; } = new Version(0, 1, 0);
    public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

    public static Config Config { get; set; }
    private bool _errorLoadingConfig = false;

    private readonly CEventsHandler _cEventsHandler = new();

    public override void Enable()
    {
        Instance = this;

        if (_errorLoadingConfig)
            Log.Error("There was an error loading the config files, please check them or generate new ones");

        if (!Config.IsEnabled)
            return;

        CEventManager.RegisterAllEvents();

        Instance = this;

        CustomHandlersManager.RegisterEventsHandler(_cEventsHandler);

        CEventManager.RegisterQueuedAssemblies();

        Log.Info($"Successfully Enabled {Name}@{Version}");
    }

    public override void Disable()
    {
        this.SaveConfig(Config, "config.yml");

        CEventManager.UnregisterAllEvents();

        CustomHandlersManager.UnregisterEventsHandler(_cEventsHandler);

        Log.Info($"Successfully Disabled {Name}@{Version}");
    }

    public override void LoadConfigs()
    {
        _errorLoadingConfig = !this.TryLoadConfig("config.yml", out Config config);
        Config = config ?? new Config();

        base.LoadConfigs();
    }
}
