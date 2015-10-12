using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.Hosting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GitHub.Network;
using GitHub.Packets;

namespace GitHubConsoleClient
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0}Hello client{0}", new string('-', 10));
            socket =
               new AdvancedSocket(
                   new Socket(
                       AddressFamily.InterNetwork,
                       SocketType.Stream,
                       ProtocolType.Tcp));
            try
            {
                //HELLO
                if (!Hello()) return;
                Console.WriteLine("{0}GitStart{0}", new string('-', 10));
                //AUTH
                while (Auth()) ;
                Console.WriteLine("{0}WellDone{0}", new string('-', 10));
                //WORK
                Work();
            }
            catch (SocketException)
            {
                Console.WriteLine("{0}Connection lost{0}", new string('-', 10));
            }
            catch (GitHub.GitHubException)
            {
                
            }
            Console.ReadKey();
        }
        private static string yn = "[y/n]";
        private static AdvancedSocket socket;

        static bool Hello()
        {
            string ip;
            //Console.WriteLine("{0Input ip{0}", new string('-', 10));
            //ip = Console.ReadLine();
            ip = "127.0.0.1";
            socket.Connect(ip, AdvancedSocket.Port);
            var packet = socket.SendAndRecivePacket(CommandType.Hello, "");
            if (packet.Error == 0)
            {
                Console.WriteLine($"Connect to {ip}:{AdvancedSocket.Port} OK");
                return true;
            }
            Console.WriteLine(packet.ErrorInfo);
            return false;
        }

        static bool Auth()
        {
            Console.WriteLine($"Do you have account? {yn}");
            var answer = Console.ReadLine().Trim().ToLower();
            switch (answer)
            {
                case "y":
                case "n":
                    Console.WriteLine((answer == "y" ? "Authorization" : "Registration") + " start.\nInput Login and password in one line");
                    var packet = socket.SendAndRecivePacket(
                        answer == "y" ?
                        CommandType.Login :
                        CommandType.Registration,
                        Console.ReadLine());
                    if (packet.Error == 0)
                        return false;
                    Console.WriteLine(packet.ErrorInfo);
                    break;
                default:
                    Console.WriteLine("Uncknown answer");
                    break;
            }
            return true;
        }


        private static BinaryFormatter formatter = new BinaryFormatter();
        private static void Load()
        {
           
            if (File.Exists("list.bin"))
                using (var fs = new FileStream("list.bin", FileMode.Open, FileAccess.Read))
                    namesList = (HashSet<string>)formatter.Deserialize(fs);
            else
                namesList = new HashSet<string>();
        }

        private static void Dump()
        {
            using (var fs = new FileStream("list.bin", FileMode.OpenOrCreate, FileAccess.Write))
                formatter.Serialize(fs, namesList);
        }
        private static Dictionary<string, Action<string[]>> commandTranslation =
            new Dictionary<string, Action<string[]>>
            {
                {"help", Help},
                {"set", Set},
                {"project", Project},
                {"list", CList},
                {"add", Add},
                {"clone", Clone},
                {"update", Update},
                {"commit", Commit},
                {"revert", Revert},
                {"log", Log}
            };

        private static HashSet<string> namesList = new HashSet<string>();
        private static string currentProject = "";
        private static ICommandPacket packet;
        static void Work()
        {
            Load();
            try
            {
                while (true)
                {
                    var line = Console.ReadLine().Split(' ').Select(a=>a.Trim());
                    if (!commandTranslation.ContainsKey(line.First()))
                    {
                        Console.WriteLine("Unknown input");
                        continue;
                    }
                    commandTranslation[line.First()].Invoke(line.Skip(1).ToArray());
                    Console.WriteLine(packet.ErrorInfo);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Connection lost");
            }
            Dump();
            Console.ReadKey();
        }

        static void Help(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("List of commands:");
                foreach (var key in commandTranslation.Keys)
                {
                    Console.WriteLine($" - {key}");
                }
                Console.WriteLine("Use 'help X' where X in list of commands");
            }
            else
            {
                Console.WriteLine("Command 'help X' where X in list of commands unwork");
            }
        }
        static void Set(string[] args)
        {
            if (!args.Any() || !namesList.Contains(args.First()))
            {
                Console.WriteLine("Wrong arguments");
                return;
            }
            Console.WriteLine("OK");
        }
        static void Project(string[] args)
        {
            Console.WriteLine(currentProject == string.Empty ? 
                "Project unset" :
                currentProject);
        }

        static void CList(string[] args)
        {
            if (!namesList.Any())
                Console.WriteLine("List empty");
            else
                foreach (var name in namesList)
                    Console.WriteLine($" - {name}");
        }
        static void Add(string[] args)
        {
            packet = socket.SendAndRecivePacket(CommandType.Add, args.FirstOrDefault());
        }
        static void Clone(string[] args)
        {
            var isFlag = args.Last().Equals(".");
            packet = socket.SendAndRecivePacket(CommandType.Clone, args.FirstOrDefault());
            if (packet.Error != 0) return;
            var archive = socket.RecieveArchive();
            archive.UnpackTo(args[1]);



            packet = socket.RecivePacket();
        }
        static void Update(string[] args)
        {

        }
        static void Commit(string[] args)
        {

        }
        static void Revert(string[] args)
        {

        }
        static void Log(string[] args)
        {

        }
    }
}
