﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GitHub.Network;
using GitHub.Packets;
using GitHubConsoleServer.Data;

namespace GitHubConsoleServer.Auth
{
    class BaseAuth: IAuth
    {
        private object lockDictAuthFile = new object();
        private string dictAuthPath = "auth.bin";
        private Dictionary<string, string> dictAuth; 
        private Dictionary<string, string> stockDictAuth = 
            new Dictionary<string, string> { {"Admin","1234qwer"} };
        private AdvancedSocket socket;
        private BinaryFormatter formatter = new BinaryFormatter();
        public BaseAuth(AdvancedSocket advancedSocket)
        {
            socket = advancedSocket;
            Load();
        }
        public string Login(ICommandPacket packet)
        {

            Load();
            if (packet.IsInvalidArguments(2))
            {
                socket.SendPacket(packet);
                return string.Empty;
            }
            var login = packet.Args.First().Trim().ToLower();
            if (dictAuth.ContainsKey(login) &&
                dictAuth[login] == packet.Args.Last().Trim())
            {
                socket.SendPacket(packet);
                Console.WriteLine($"[{Logger.Time()}] User {login} logined");
                return login;
            }
            packet.ErrorInfo = "Wrong login or password";
            socket.SendPacket(packet);
            return string.Empty;
        }

        public string Registration(ICommandPacket packet)
        {
            Load();
            string login = "";
            if (packet.IsInvalidArguments(2))
            { }
            else
            {
                login = packet.Args.First().Trim().ToLower();
                if (dictAuth.ContainsKey(login))
                packet.ErrorInfo = $"Name '{login}' is busy";
            }
            
            socket.SendPacket(packet);

            if (packet.Error != 0)
                return string.Empty;
            dictAuth.Add(login, packet.Args.Last());
            Console.WriteLine($"[{Logger.Time()}] User {login} registration end");
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
