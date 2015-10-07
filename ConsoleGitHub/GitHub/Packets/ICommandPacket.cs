using ConsoleGitHub.Network;
using GitHub.Network;

namespace GitHub.Packets
{
    public interface ICommandPacket
    {
        CommandType Command { get;}
        string[] Args { get;}
        byte[] Bytes { get; }
        int Error { get; }
        string ErrorInfo { get; }
    }
}
