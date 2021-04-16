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
            public string Reason { get; set; }
            public string Issuer { get; set; }
            public string IssuerId { get; set; } = null;
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
    Id       int auto_increment
        primary key,
    Username longtext    null,
    Type     longtext    null,
    UserId   longtext    null,
    Reason   longtext    null,
    Issuer   longtext    null,
    IssuerId longtext    null,
    Ip       longtext    null,
    Length   int         not null,
    Issued   datetime(6) not null
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
