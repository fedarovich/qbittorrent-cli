using System.Runtime.Serialization;

namespace QBittorrent.Client
{
    public enum ConnectionStatus
    {
        [EnumMember(Value = "connected")]
        Connected,

        [EnumMember(Value = "firewalled")]
        Firewalled,

        [EnumMember(Value = "disconnected")]
        Disconnected
    }
}
