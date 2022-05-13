using System;
using Exiled.Events.EventArgs;
using Exiled.Events.Handlers;
using HarmonyLib;

namespace PlayerManager.Patches {

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus
    {
        private static bool Prefix(CharacterClassManager __instance, bool value)
        {
            try
            {
                ChangingMuteStatusEventArgs ev = new ChangingMuteStatusEventArgs(Exiled.API.Features.Player.Get(__instance._hub), value, true);

                Exiled.Events.Handlers.Player.OnChangingMuteStatus(ev);

                if (!ev.IsAllowed)
                {
                    if (value == true)
                    {
                        MuteHandler.RevokePersistentMute(__instance.UserId);
                    }
                    else
                    {
                        MuteHandler.IssuePersistentMute(__instance.UserId);
                    }

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"{typeof(ChangingMuteStatus).FullName}.{nameof(Prefix)}:\n{e}");
                return true;
            }
        }
    }
}
