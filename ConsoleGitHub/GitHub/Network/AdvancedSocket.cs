using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using GitHub.Archive;
using GitHub.Packets;
using GitHub.Util;

namespace GitHub.Network
{
    public class AdvancedSocket: IAdvancedSocket
    {
        private const int PacketSize = 1024;
        public static int Port = 23232;
        private Socket socket;
        //public AdvancedSocket(Socket socket) : base(socket.AddressFamily, socket.SocketType, socket.ProtocolType)
        //{

        //}
        public AdvancedSocket(Socket socket)
        {
            this.socket = socket;
        }

        private void Send(byte[] bytes) => socket.Send(bytes);
        private void Receive(byte[] bytes) => socket.Receive(bytes);
        public void Connect(string ip, int port) => socket.Connect(ip, port);
        public void Close() => socket.Close();


        public void SendPacket(ICommandPacket packet)
        {
            var bytes = packet.Bytes;
            Send(BitConverter.GetBytes(bytes.Length));
            Send(packet.Bytes);
        }
        public void SendPacket(CommandType commandType, string args)
        {
            SendPacket(new CommandPacket(commandType, args));
        }
        //public void SendPacket(int error, string errorInfo)
        //{
        //    SendPacket(new CommandPacket(error, errorInfo));
        //}

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

        public ICommandPacket RecivePacket(CommandType commandType)
        {
            var packet = RecivePacket();
            while ((packet.Command & commandType)==0)
            {
                packet.ErrorInfo = $"The server is waiting for the {commandType} command";
                SendPacket(packet);
                packet = RecivePacket();
            }
            return packet;
        }

        public ICommandPacket SendAndRecivePacket(ICommandPacket packet)
        {
            SendPacket(packet);
            return RecivePacket();
        }

        public ICommandPacket SendAndRecivePacket(CommandType commandType, string args)
        {
            SendPacket(commandType, args);
            return RecivePacket();
        }
    }
}
