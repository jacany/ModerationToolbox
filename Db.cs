using System;
using MySqlConnector;
using Exiled.API.Features;

namespace ModerationToolbox
{
    public class Db
    {
        public class Punishment
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Type { get; set; }
            public string UserId { get; set; }
            public string Reason { get; set; } = null;
            public bool Unpunished { get; set; } = false;
            public string IssuerId { get; set; }
            public string IssuerIp { get; set; }
            public string Ip { get; set; }
            public int Length { get; set; }
            public DateTime Issued { get; set; }
        }

        public static async void SyncDb()
        {
            Log.Info("Syncing Database Tables");

            try
            {
                using (var db = new MySql())
                {
                    await db.Connection.OpenAsync();
                    
                    // Create our punishments table if it doesn't already exist
                    using (var command = new MySqlCommand())
                    {
                        command.Connection = db.Connection;
                        command.CommandText = @"
create table if not exists Punishments
(
    Id         int auto_increment
        primary key,
    Username   longtext             null,
    Type       longtext             null,
    UserId     longtext             null,
    Reason     longtext             null,
    Unpunished tinyint(1) default 0 not null,
    IssuerId   longtext             not null,
    IssuerIp   longtext             not null,
    Ip         longtext             null,
    Length     int                  not null,
    Issued     datetime(6)          not null
);";
                        await command.ExecuteNonQueryAsync();
                    }


                    db.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async void AddPunishment(Punishment punishment)
        {
            try
            {
                using (var db = new MySql())
                {
                    await db.Connection.OpenAsync();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = db.Connection;
                        command.CommandText = "INSERT INTO Punishments (Username, Type, UserId, Reason, IssuerId, IssuerIp, Ip, Length, Issued) VALUES (@Username, @Type, @UserId, @Reason, @IssuerId, @IssuerIp, @Ip, @Length, @Issued)";
                        command.Parameters.AddWithValue("@Username", punishment.Username);
                        command.Parameters.AddWithValue("@Type", punishment.Type);
                        command.Parameters.AddWithValue("@UserId", punishment.UserId);
                        command.Parameters.AddWithValue("@Reason", punishment.Reason);
                        command.Parameters.AddWithValue("@IssuerId", punishment.IssuerId);
                        command.Parameters.AddWithValue("@IssuerIp", punishment.IssuerIp);
                        command.Parameters.AddWithValue("@Ip", punishment.Ip);
                        command.Parameters.AddWithValue("@Length", punishment.Length);
                        command.Parameters.AddWithValue("@Issued", punishment.Issued.ToString("yyyy-MM-dd HH:mm:ss"));
                        await command.ExecuteNonQueryAsync();
                    }

                    db.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public class MySql : IDisposable
    {
        public string Server = ModerationToolbox.Instance.Config.DbHost;
        public int Port = ModerationToolbox.Instance.Config.DbPort;
        public string Username = ModerationToolbox.Instance.Config.DbUser;
        public string Password = ModerationToolbox.Instance.Config.DbPassword;
        public string Database = ModerationToolbox.Instance.Config.DbSchema;

        public MySqlConnection Connection { get; set; }

        public MySql()
        {
            Connection = new MySqlConnection($"Server={Server};Port={Port};Uid={Username};Password={Password};Database={Database}");
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
