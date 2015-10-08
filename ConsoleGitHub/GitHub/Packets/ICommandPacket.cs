using GitHub.Network;

namespace GitHub.Packets
{
    public interface ICommandPacket
    {
        CommandType Command { get;}
        string[] Args { get;}
        byte[] Bytes { get; }
        int Error { get; set; }
        string ErrorInfo { get; set; }
        void SetAsInvalidArgument();
        bool IsValidArguments(int count);
    }
}
