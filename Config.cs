using Exiled.API.Interfaces;

namespace ModerationToolbox
{
    public sealed class Config : IConfig
    {
	    public bool IsEnabled { get; set; } = true;
    }
}
