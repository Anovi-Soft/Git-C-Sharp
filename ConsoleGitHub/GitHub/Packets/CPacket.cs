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

        public string[] Args => _args.Split(' ')
            .Select(a => a)
            .Where(a => a.Any())
            .ToArray();

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
                Error = 1;
            }
        }

        public void SetAsInvalidArgument()
        {
            ErrorInfo = "Bad list of arguments";
        }

        public bool IsValidArguments(int count)
        {
            if (Args.Count() != count)
            {
                SetAsInvalidArgument();
                return false;
            }
            return true;
        }
    }
}
