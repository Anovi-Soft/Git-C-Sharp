using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Network.Packets;

namespace ConsoleGitHub.Network
{
    public class AdvancedSocket:Socket, IAdvancedSocket
    {
        public AdvancedSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        {
        }

        public AdvancedSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }

        public AdvancedSocket(SocketInformation socketInformation) : base(socketInformation)
        {
        }

        public void SendPacket(ICommandPacket packet)
        {
            //var bytes = packet.Bytes;
            //Send(BitConverter.GetBytes(bytes.Length));
            Send(packet.Bytes);
        }

        public ICommandPacket ReceivePacket()
        {
            var bytes = new byte[200];
            Receive(bytes);
            return PacketFarm.Get(bytes);
        }

        public void SendArchive(IArchive archive)
        {
            Send(BitConverter.GetBytes(archive.SizeOfArchive()));
            using (var sr = new StreamReader(archive.Path))
            using (var br = new BinaryReader(sr))
        }

        public IArchive ReceiveArchive()
        {
            throw new NotImplementedException();
        }

        public Task SendArchiveAsync(IArchive arhcive)
        {
            throw new NotImplementedException();
        }

        public Task<IArchive> ReciveArchiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
