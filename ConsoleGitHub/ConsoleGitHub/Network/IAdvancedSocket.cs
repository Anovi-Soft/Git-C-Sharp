using ConsoleGitHub.Network.Packets;

namespace ConsoleGitHub.Network
{
    public interface IAdvancedSocket
    {
        void SendPacket(ICommandPacket packet);
        ICommandPacket 
    }
}