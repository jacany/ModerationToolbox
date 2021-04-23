using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;

namespace ModerationToolbox
{
    public class ModerationToolbox : Plugin<Config>
    {
        private static ModerationToolbox singleton = new ModerationToolbox();

        private ModerationToolbox() { }

        public override string Name { get; } = "ModerationToolbox";
        public override string Author { get; } = "jacany";
        public override string Prefix { get; } = "ModerationToolbox";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 8, 0);

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Player _player;

	// Gets the existing instance of the plugin
	public static ModerationToolbox Instance => singleton;

        public override void OnEnabled()
        {
            Log.Info("Enabled :)");
            RegisterEvents();
            Db.SyncDb();
        }

        public override void OnDisabled()
        {
            Log.Info("Disabled :(");
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            _player = new Handlers.Player();

            Player.Banning += _player.OnBan;
            Player.Verified += _player.OnVerified;
        }

        private void UnregisterEvents()
        {
            Player.Banning -= _player.OnBan;
            Player.Verified -= _player.OnVerified;

            _player = null;
        }
    }
}
