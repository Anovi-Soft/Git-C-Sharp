using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Network.Packets
{
    [Serializable]
    public class CPacket:ICommandPacket
    {
        private readonly Encoding _encoding = Encoding.Unicode;
        public CPacket(CommandType command, string args)
        {
            Command = command;
            _args = args;
            Error = 0;
            ErrorInfo = "OK";
        }

        public CPacket(int error, string errorInfo)
        {
            Error = error;
            ErrorInfo = errorInfo;
        }

        public static CPacket FromBytes(byte[] bytes)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return (CPacket)formatter.Deserialize(stream);
            }
        }
        public CommandType Command { get; }
        private readonly string _args;
        public string[] Args => _args.Split(' ');

        public byte[] Bytes
        {
            get
            {
                IFormatter formatter = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, this);
                    return stream.ToArray();
                }
            }
        }

        public int Error { get; }
        public string ErrorInfo { get; }
    }
}
