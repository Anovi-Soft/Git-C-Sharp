using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Data;
using ConsoleGitHub.Network.Packets;

namespace ConsoleGitHub.Network
{
    public class AdvancedSocket:Socket, IAdvancedSocket
    {
        private const int PacketSize = 1000;
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
            var archSize = archive.SizeOfArchive();
            Send(BitConverter.GetBytes(archSize));
            using (var br = new BinaryReader(File.Open(archive.Path, FileMode.Open, FileAccess.Read)))
                for (var i =0; i < archSize; i+=PacketSize)
                    Send(br.ReadBytes(PacketSize));
        }

        public IArchive ReceiveArchive()
        {
            var path = FileHelper.GetFreeTmpName(".zip");
            var bytes = new byte[4];
            Receive(bytes);
            var fileSize = BitConverter.ToInt32(bytes, 0);
            using (var bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                bytes = new byte[PacketSize];
                for (var i = 0; i < fileSize; i += PacketSize)
                {
                    
                }

            }
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
