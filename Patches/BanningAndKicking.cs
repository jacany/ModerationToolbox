using System;
using Exiled.Events.EventArgs;
using Exiled.Events.Handlers;
using GameCore;
using HarmonyLib;
using Mirror;
using UnityEngine;
namespace PlayerManager.Patches
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(int), typeof(string), typeof(string), typeof(bool) })]
    internal static class BanningAndKicking
    {
        private static bool Prefix(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {
            try
            {
                if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
                {
                    duration = int.MaxValue;
                }

                string userId = null;
                string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;

                Exiled.API.Features.Player targetPlayer = Exiled.API.Features.Player.Get(user);
                Exiled.API.Features.Player issuerPlayer = Exiled.API.Features.Player.Get(issuer) ?? Exiled.API.Features.Server.Host;

                try
                {
                    if (ConfigFile.ServerConfig.GetBool("online_mode", false))
                        userId = targetPlayer.UserId;
                }
                catch
                {
                    ServerConsole.AddLog("Failed during issue of User ID ban (1)!");
                    return false;
                }

                string message = $"You have been {((duration > 0) ? "banned" : "kicked")}. ";
                if (!string.IsNullOrEmpty(reason))
                    message = message + "\nReason: " + reason;
                if (!string.IsNullOrEmpty(PlayerManager.Instance.Config.AppealUrl) && duration > 0)
                    message = message + $"\nAppeal at: {PlayerManager.Instance.Config.AppealUrl}";

                if (!ServerStatic.GetPermissionsHandler().IsVerified || !targetPlayer.IsStaffBypassEnabled)
                {
                    if (duration > 0)
                    {
                        var ev = new BanningEventArgs(targetPlayer, issuerPlayer, duration, reason, message);

                        Player.OnBanning(ev);

                        duration = ev.Duration;
                        reason = ev.Reason;
                        message = ev.FullMessage;

                        if (!ev.IsAllowed)
                            return false;

                        string originalName = string.IsNullOrEmpty(targetPlayer.Nickname)
                            ? "(no nick)"
                            : targetPlayer.Nickname;
                        long issuanceTime = TimeBehaviour.CurrentTimestamp();
                        long banExpiryTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
                        try
                        {
                            if (userId != null)
                            {
                                if (issuer == "Administrator")
                                {
                                    Db.AddPunishment(new Db.Punishment
                                    {
                                        Username = originalName,
                                        Type = "ban",
                                        UserId = userId,
                                        Reason = reason,
                                        IssuerId = issuer,
                                        IssuerIp = issuerPlayer.IPAddress,
                                        Ip = targetPlayer.IPAddress,
                                        Length = duration / 60,
                                        Issued = new DateTime(issuanceTime)
                                    });
                                }
                                else
                                {
                                    Db.AddPunishment(new Db.Punishment
                                    {
                                        Username = originalName,
                                        Type = "ban",
                                        UserId = userId,
                                        Reason = reason,
                                        IssuerId = issuerPlayer.UserId,
                                        IssuerIp = issuerPlayer.IPAddress,
                                        Ip = targetPlayer.IPAddress,
                                        Length = duration / 60,
                                        Issued = new DateTime(issuanceTime)
                                    });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ServerConsole.AddLog($"Failed during issue of User ban (2)! {e}");
                            return false;
                        }
                    }
                    else if (duration == 0)
                    {
                        var ev = new KickingEventArgs(targetPlayer, issuerPlayer, reason, message);

                        Player.OnKicking(ev);

                        reason = ev.Reason;
                        message = ev.FullMessage;

                        if (!ev.IsAllowed)
                            return false;
                    }
                }

                ServerConsole.Disconnect(targetPlayer.ReferenceHub.gameObject, message);

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"PlayerManager.Patches.BanningAndKicking: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
