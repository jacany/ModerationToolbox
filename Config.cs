using System.ComponentModel;
using Exiled.API.Interfaces;

namespace ModerationToolbox
{
    public sealed class Config : IConfig
    {
	    public bool IsEnabled { get; set; } = true;

        [Description("Database settings; The database should be either MySQL or MariaDB")]
        public string DbHost { get; set; }
        public int DbPort { get; set; } = 1234;
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public string DbSchema { get; set; }
    }
}
