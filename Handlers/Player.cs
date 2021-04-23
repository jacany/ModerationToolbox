using System;
using Exiled.Events.EventArgs;

namespace ModerationToolbox.Handlers
{
    class Player
    {
        public void OnVerified(VerifiedEventArgs ev)
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
        }
    }
}
