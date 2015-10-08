using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GitHub.Network;

namespace GitHubConsoleServer.Auth
{
    class BaseAuth: IAuth
    {
        private object lockDictAuthFile = new object();
        private string dictAuthPath = "auth.bin";
        private Dictionary<string, string> dictAuth; 
        private Dictionary<string, string> stockDictAuth = 
            new Dictionary<string, string>{ {"Admin","1234qwer"} };
        private AdvancedSocket socket;
        private BinaryFormatter formatter = new BinaryFormatter();
        public BaseAuth(AdvancedSocket advancedSocket)
        {
            socket = advancedSocket;
            bool initEnd = false;
            Load();
        }
        public string Login()
        {
            Load();
            var packet = socket.RecivePacket(CommandType.Login);
            if (!packet.IsValidArguments(2))
            {
                socket.SendPacket(packet);
                return string.Empty;
            }
            if (dictAuth.ContainsKey(packet.Args.First()) &&
                dictAuth[packet.Args.First()] == packet.Args.Last())
            {
                socket.SendPacket(packet);
                return packet.Args.First();
            }
            packet.ErrorInfo = "Wrong login or password";
            socket.SendPacket(packet);
            return string.Empty;
        }

        public string Registration()
        {
            Load();
            var packet = socket.RecivePacket(CommandType.Registration);
            if (!packet.IsValidArguments(2)) {}
            else if (dictAuth.ContainsKey(packet.Args.First()))
                packet.ErrorInfo = $"Name '{packet.Args.First()}' is busy";
            
            socket.SendPacket(packet);

            if (packet.Error != 0)
                return string.Empty;
            
            Dump();
            return packet.Args.First();
        }

        public void Load()
        {
            lock (lockDictAuthFile)
                if (File.Exists(dictAuthPath))
                    using (var fs = new FileStream(dictAuthPath, FileMode.Open, FileAccess.Read))
                        dictAuth = (Dictionary<string, string>) formatter.Deserialize(fs);
                else
                    dictAuth = stockDictAuth;
        }

        public void Dump()
        {
            lock (lockDictAuthFile)
                using (var fs = new FileStream(dictAuthPath, FileMode.OpenOrCreate, FileAccess.Write))
                    formatter.Serialize(fs,dictAuth);
        }

    }
}
