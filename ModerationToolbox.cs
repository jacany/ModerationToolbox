using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events;
using Exiled.Loader;
using HarmonyLib;
using UnityEngine;
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
        private int _patches;
        public Harmony Harmony { get; private set; }

	    // Gets the existing instance of the plugin
	    public static ModerationToolbox Instance => singleton;

        public override void OnEnabled()
        {
            base.OnEnabled();
            UnpatchExiledEvents();
            Patch();
            RegisterEvents();
            Db.SyncDb();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            UnregisterEvents();
            Unpatch();
        }

        private void RegisterEvents()
        {
            _player = new Handlers.Player();

            Player.Verified += _player.OnVerified;
        }

        private void UnregisterEvents()
        {
            Player.Verified -= _player.OnVerified;

            _player = null;
        }

        private void Patch()
        {
            try
            {
                Harmony = new Harmony($"ModerationToolbox.{++_patches}");

                var lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;

                Harmony.PatchAll();

                Harmony.DEBUG = lastDebugStatus;

                Log.Debug("Patches applied Successfully!", Loader.ShouldDebugBeShown);
            }
            catch (Exception e)
            {
                Log.Error($"Patching Failed {e}");
            }
        }

        private void Unpatch()
        {
            Harmony.UnpatchAll();

            Log.Debug("Patches have been undone!", Loader.ShouldDebugBeShown);
        }

        private void UnpatchExiledEvents()
        {
            Events.DisabledPatchesHashSet.Add(AccessTools.Method(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(int), typeof(string), typeof(string), typeof(bool) }));

            Events.Instance.ReloadDisabledPatches();
        }
    }
}
