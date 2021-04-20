using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace ModerationToolbox.Handlers
{
    class Player
    {
        public void OnBan(BanningEventArgs ev)
        {
            Log.Debug($"Target: {ev.Target.Nickname}");
            Log.Debug($"Issuer: {ev.Issuer.Nickname}");
            Log.Debug($"Duration: {ev.Duration}");
            Log.Debug($"Reason: {ev.Reason}");
            Log.Debug($"Allowed: {ev.IsAllowed}");
            if (ev.IsAllowed)
            {
                var punishment = new Db.Punishment();
                punishment.Username = ev.Target.Nickname;
                punishment.Type = "ban";
                punishment.UserId = ev.Target.UserId;
                punishment.Reason = ev.Reason;
                punishment.IssuerId = ev.Issuer.UserId;
                punishment.IssuerIp = ev.Issuer.IPAddress;
                punishment.Ip = ev.Target.IPAddress;
                punishment.Length = ev.Duration / 60;
                punishment.Issued = DateTime.Now;
                Db.AddPunishment(punishment);
            }
        }
    }
}
