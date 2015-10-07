using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ConsoleGitHub.Network.Packets;
using GitHub.Archive;
using GitHub.Packets;
using GitHub.Util;

namespace GitHub.Network
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
            var bytes = packet.Bytes;
            Send(BitConverter.GetBytes(bytes.Length));
            Send(packet.Bytes);
        }
        public void SendPacket(CommandType commandType, string args)
        {
            SendPacket(new CPacket(commandType, args));
        }
        public void SendPacket(int error, string errorInfo)
        {
            SendPacket(new CPacket(error, errorInfo));
        }

        public ICommandPacket RecivePacket()
        {
            var bytes = new byte[4];
            Receive(bytes);
            bytes = new byte[BitConverter.ToInt32(bytes,0)];
            Receive(bytes);
            return PacketFarm.GetFromBytes(bytes);
        }
        
        public void SendArchive(IArchive archive)
        {
            var archSize = archive.SizeOfArchive();
            Send(BitConverter.GetBytes(archSize));
            using (var br = new BinaryReader(File.Open(archive.Path, FileMode.Open, FileAccess.Read)))
                for (var i =0; i < archSize; i+=PacketSize)
                    Send(br.ReadBytes(PacketSize));
        }

        public IArchive RecieveArchive()
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
                    if (fileSize-i < PacketSize)
                        bytes = new byte[fileSize - i];
                    Receive(bytes);
                    bw.Write(bytes);
                }
            }
            return new ArchiveZip(path);
        }
        

        public Task SendArchiveAsync(IArchive arhcive)
        {
            return Task.Run(() => SendArchive(arhcive));
        }

        public Task<IArchive> RecieveArchiveAsync()
        {
            return Task.Run(() => RecieveArchive());
        }
    }
}
