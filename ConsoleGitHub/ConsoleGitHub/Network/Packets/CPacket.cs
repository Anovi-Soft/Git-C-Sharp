using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        public CPacket(byte[] bytes)
        {
            Command = (CommandType)BitConverter.ToInt16(bytes, 0);
            _args = _encoding.GetString(bytes.Skip(2).ToArray());
        }
        public CommandType Command { get; }
        private readonly string _args;
        public string[] Args => _args.Split(' ');

        public byte[] Bytes
        {
            get
            {
                var a = BitConverter.GetBytes((short) Command).ToList();
                a.AddRange(_encoding.GetBytes(_args));
                return a.ToArray();
            }
        }
    }
}
