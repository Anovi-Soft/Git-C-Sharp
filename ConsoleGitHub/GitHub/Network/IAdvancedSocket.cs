using System.Threading.Tasks;
using GitHub.Archive;
using GitHub.Packets;

namespace GitHub.Network
{
    public interface IAdvancedSocket
    {
        void SendPacket(ICommandPacket packet);
        ICommandPacket RecivePacket();
        void SendArchive(IArchive archive);
        IArchive RecieveArchive();
        Task SendArchiveAsync(IArchive arhcive);
        Task<IArchive> RecieveArchiveAsync();

    }
}