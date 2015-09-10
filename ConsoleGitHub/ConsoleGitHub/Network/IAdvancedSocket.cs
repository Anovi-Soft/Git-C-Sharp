using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Network.Packets;

namespace ConsoleGitHub.Network
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