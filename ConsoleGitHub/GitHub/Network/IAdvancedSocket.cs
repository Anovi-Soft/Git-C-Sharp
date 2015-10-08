using System.Threading.Tasks;
using GitHub.Archive;
using GitHub.Packets;

namespace GitHub.Network
{
    public interface IAdvancedSocket
    {
        void SendPacket(ICommandPacket packet);
        void SendPacket(CommandType commandType, string args);
        ICommandPacket RecivePacket();
        void SendArchive(IArchive archive);
        IArchive RecieveArchive();
        Task SendArchiveAsync(IArchive arhcive);
        Task<IArchive> RecieveArchiveAsync();
        ICommandPacket RecivePacket(CommandType commandType);
        ICommandPacket SendAndRecivePacket(ICommandPacket packet);
        ICommandPacket SendAndRecivePacket(CommandType commandType, string args);
    }
}