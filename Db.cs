using System;
using System.Threading.Tasks;
using MySqlConnector;
using Exiled.API.Features;

namespace PlayerManager
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

        public class PlayerCheckValues
        {
            public string Type { get; set; }
            public int Length { get; set; }
            public string Reason { get; set; } = null;
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
);

create table if not exists Users
(
    Id     text not null,
    `Rank` text null,
    constraint Users_id_uindex
        unique (Id) using hash
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

        public static PlayerCheckValues CheckPlayerPunishments(string id, string ip)
        {
            PlayerCheckValues result = new PlayerCheckValues();

            result.Length = 0;
            result.Type = "none";

            try
            {
                using (var db = new MySql())
                {
                    db.Connection.Open();
                    using (var command = new MySqlCommand($"SELECT Type,Reason,Length,Issued FROM Punishments WHERE Unpunished LIKE false AND (UserId LIKE '{id}' OR Ip LIKE '{ip}') AND NOW() <= DATE_ADD(Issued, INTERVAL Length MINUTE);", db.Connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int length = reader.GetInt32("Length");
                            DateTime issued = reader.GetDateTime("Issued");
                            DateTime expires = issued.AddMinutes(length);
                            string pType = reader.GetString("Type");

                            if (length != null && issued != null && pType == "ban")
                            {
                                string reason = reader.GetString("Reason");
                                PlayerCheckValues returnValue = new PlayerCheckValues();

                                returnValue.Reason = reason;
                                returnValue.Length = Convert.ToInt32((expires - DateTime.Now).TotalSeconds);
                                returnValue.Type = "ban";

                                return returnValue;
                            }
                            else if (length != null && issued != null && pType == "mute")
                            {
                                result.Length = Convert.ToInt32((expires - DateTime.Now).TotalSeconds);
                                result.Type = "mute";
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return result;
            }
        }

        public static async Task<string> GetPlayer(string id)
        {
            try
            {
                using (var db = new MySql())
                {
                    await db.Connection.OpenAsync();
                    using (var cmd = new MySqlCommand($"SELECT Rank FROM Users WHERE Id LIKE '{id}'", db.Connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            return reader.GetString("Rank");
                }

            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return "";
        }
    }

    public class MySql : IDisposable
    {
        public string Server = PlayerManager.Instance.Config.DbHost;
        public int Port = PlayerManager.Instance.Config.DbPort;
        public string Username = PlayerManager.Instance.Config.DbUser;
        public string Password = PlayerManager.Instance.Config.DbPassword;
        public string Database = PlayerManager.Instance.Config.DbSchema;

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
