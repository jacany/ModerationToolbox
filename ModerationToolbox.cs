using System;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace ModerationToolbox
{
    public class ModerationToolbox : Plugin<Config>
    {
	public static ModerationToolbox Instance { get; } = new ModerationToolbox();
	private ModerationToolbox() { }

	public override string Name { get; } = "ModerationToolbox";
	public override string Author { get; } = "jacany";
	public override string Prefix { get; } = "MT";
	public override Version Version { get; } = new Version(1, 0, 0);
	public override Version RequiredExiledVersion { get; } = new Version(2, 8, 0);

	public override PluginPriority Priority { get; } = PluginPriority.Medium;

	public override void OnEnabled()
	{
	    if (!Config.IsEnabled) return;
	    Log.Info("Enabled :)");
	}

	public override void OnDisabled()
	{
	    Log.Info("Disabled :(");
	}
    }
}
