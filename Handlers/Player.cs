using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace ModerationToolbox.Handlers
{
    class Player
    {
        public async void OnVerified(VerifiedEventArgs ev)
        {
            var result = Db.CheckPlayerPunishments(ev.Player.UserId, ev.Player.IPAddress);

            if (result.Type == "mute")
            {
                ev.Player.IsMuted = true;
            }
            else if (result.Type == "ban")
            {
                string reason = $"You have been banned.";
                if (!string.IsNullOrEmpty(result.Reason))
                    reason = reason + $"\nReason: {result.Reason}";
                if (!string.IsNullOrEmpty(ModerationToolbox.Instance.Config.AppealUrl))
                    reason = reason + $"\nAppeal at: {ModerationToolbox.Instance.Config.AppealUrl}";

                ev.Player.Disconnect(reason);
            }

            string userGroup = await Db.GetPlayer(ev.Player.UserId);
            Log.Info(userGroup);

            // idk why vs is saying this doesn't exist, but it compiles so idk wtf is happening
            UserGroup grp = ServerStatic.GetPermissionsHandler()._groups[userGroup];

            ev.Player.Group = grp;
            ev.Player.GroupName = "admin";
        }
    }
}
