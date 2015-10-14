using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GitHub.Network;

namespace GitHub.Packets
{
    [Serializable]
    public class CommandPacket:ICommandPacket
    {
        private readonly Encoding _encoding = Encoding.Unicode;
        public CommandPacket(CommandType command, string args)
        {
            Command = command;
            _args = args;
            ErrorInfo = "OK";
            Error = 0;
        }

        public CommandPacket(int error, string errorInfo)
        {
            Error = error;
            ErrorInfo = errorInfo;
        }

        public static CommandPacket FromBytes(byte[] bytes)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return (CommandPacket)formatter.Deserialize(stream);
            }
        }
        public CommandType Command { get; }
        private readonly string _args;

        public string[] Args => _args?.Split(' ')
            .Select(a => a)
            .Where(a => a.Any())
            .ToArray() ??
            new string[0];

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

        public int Error { get; set; }

        private string _errorInfo;
        public string ErrorInfo
        {
            get { return _errorInfo; }
            set
            {
                _errorInfo = value;
                if (value != "OK")
                    Error = 1;
            }
        }

        public void SetAsInvalidArgument()
        {
            ErrorInfo = "Bad list of arguments";
        }

        /// <summary>
        /// Check is there invalid list of arguments and if is invalid invoke <see cref="SetAsInvalidArgument"/>
        /// </summary>
        public bool IsInvalidArguments(int count)
        {
            if (Args.Count(a => a.Trim().Any()) == count) return false;
            SetAsInvalidArgument();
            return true;
        }
    }
}
