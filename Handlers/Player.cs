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
                string reason = $"You have been banned\nReason: {result.Reason}";
                if (ModerationToolbox.Instance.Config.AppealUrl != null)
                    reason = reason + $"\nAppeal at: {ModerationToolbox.Instance.Config.AppealUrl}";

                ev.Player.Disconnect(reason);
            }
        }

        public void OnBan(BanningEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                var punishment = new Db.Punishment();
                punishment.Username = ev.Target.Nickname;
                punishment.Type = "ban";
                punishment.UserId = ev.Target.UserId;
                punishment.Reason = ev.Reason;
                if (ev.Issuer.UserId == null)
                {
                    punishment.IssuerId = "Console";
                }
                else
                {
                    punishment.IssuerId = ev.Issuer.UserId;
                }
                punishment.IssuerIp = ev.Issuer.IPAddress;
                punishment.Ip = ev.Target.IPAddress;
                punishment.Length = ev.Duration / 60;
                punishment.Issued = DateTime.Now;
                Db.AddPunishment(punishment);
            }
        }
    }
}
