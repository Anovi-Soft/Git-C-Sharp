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
        IArchive ReceiveArchive();
        Task SendArchiveAsync(IArchive arhcive);
        Task<IArchive> ReceiveArchiveAsync();

    }
}