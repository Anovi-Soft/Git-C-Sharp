using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Hosting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GitHub.Archive;
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
            finally
            {
                socket.Close();
            }

            Console.ReadKey();
        }
        private static string yn = "[y/n]";
        private static AdvancedSocket socket;

        static bool Hello()
        {
            IPAddress ip;
            string input;
            do
            {
                Console.WriteLine("{0}Input ip{0}", new string('-', 10));
                input = Console.ReadLine().Trim();
                IPAddress.TryParse(input, out ip);
            } while (ip==null);

            //ip = "127.0.0.1";
            socket.Connect(input, AdvancedSocket.Port);
            
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
                    Console.WriteLine((answer == "y" ? "Authorization" : "Registration") + " start.\nInput Login and password");
                    Console.Write("LOGIN: ");
                    currentUser = Console.ReadLine().Trim().ToLower();
                    Console.Write("PASSWORD: ");
                    var pass = Console.ReadLine().Trim();
                    var packet = socket.SendAndRecivePacket(
                        answer == "y" ?
                        CommandType.Login :
                        CommandType.Registration,
                        currentUser + " " + pass);
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

        private static string currentUser = String.Empty;
        private static Dictionary<string, Dictionary<string,string>> projectsDict = new Dictionary<string, Dictionary<string, string>>();

        private static Dictionary<string, string> projectPath
        {
            get
            {
                if (!projectsDict.ContainsKey(currentUser))
                    projectsDict.Add(currentUser, new Dictionary<string, string>());
                return projectsDict[currentUser];
            }
        }
        private static List<string> namesList {
            get
            {
                if (!projectsDict.ContainsKey(currentUser))
                    projectsDict.Add(currentUser, new Dictionary<string, string>());
                return projectsDict[currentUser].Keys.ToList();
            }
        }

        private static BinaryFormatter formatter = new BinaryFormatter();
        private static void Load()
        {
           
            if (File.Exists("list.bin"))
                try
                {
                    using (var fs = new FileStream("list.bin", FileMode.Open, FileAccess.Read))
                        projectsDict = (Dictionary<string, Dictionary<string, string>>)formatter.Deserialize(fs);
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }

            projectsDict = new Dictionary<string, Dictionary<string, string>>();
        }

        private static void Dump()
        {
            using (var fs = new FileStream("list.bin", FileMode.OpenOrCreate, FileAccess.Write))
                formatter.Serialize(fs, projectsDict);
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
                {"log", Log},
                {"goodbuy", a =>
                {
                    socket.SendPacket(CommandType.GoodBy, "");
                    throw new SystemException();
                }}
            };

        private static string currentProject = "";
        private static ICommandPacket packet;
        static void Work()
        {
            Load();
            try
            {
                while (true)
                {
                    var line = Console.ReadLine().Split(' ').Select(a => a.Trim().ToLower());
                    if (!commandTranslation.ContainsKey(line.First()))
                    {
                        Console.WriteLine("Unknown input");
                        continue;
                    }
                    commandTranslation[line.First()].Invoke(line.Skip(1).ToArray());
                    if ("set,list,help,project".Split(',').Contains(line.First())) continue;
                    Console.WriteLine(packet.ErrorInfo);
                }
            }
            catch (SystemException)
            {
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
            currentProject = args.First();
            Console.WriteLine("OK");
        }
        static void Project(string[] args)
        {
            Console.WriteLine(currentProject == string.Empty ? 
                "Project unset" :
                $"{currentProject} at {projectPath[currentProject]}");
        }

        static void CList(string[] args)
        {
            if (!namesList.Any())
                Console.WriteLine("List empty");
            else
                foreach (var name in namesList)
                    Console.WriteLine($" - {name} at {projectPath[name]}");
        }
        static void Add(string[] args)
        {
            packet = socket.SendAndRecivePacket(CommandType.Add, args.FirstOrDefault());
        }
        static void Clone(string[] args)
        {
            if (namesList.Contains(args[0].Trim().ToLower()))
            {
                Console.WriteLine($"Project already exist, clone of {args[0]} at {projectPath[args[0]]}");
                return;
            }
            var isFlag = args.Last().Equals(".");
            packet = socket.SendAndRecivePacket(CommandType.Clone, args.FirstOrDefault());
            if (packet.Error != 0) return;
            var archive = socket.RecieveArchive();
            var path = isFlag ? Path.Combine(args[1], args[0]) : args[1];
            archive.UnpackTo(path);
            packet = socket.RecivePacket();
            projectPath.Add(args[0].Trim().ToLower(), path.Replace("/","\\"));
            Dump();
        }
        static void Update(string[] args)
        {
            if (currentProject == string.Empty)
            {
                Console.WriteLine("Set current project");
                return;
            }
            packet = socket.SendAndRecivePacket(CommandType.Update, currentProject);
            if (packet.Error != 0) return;
            var archive = socket.RecieveArchive();
            archive.UnpackTo(projectPath[currentProject]);
            packet = socket.RecivePacket();
        }
        static void Commit(string[] args)
        {
            if (currentProject == string.Empty)
            {
                Console.WriteLine("Set current project");
                return;
            }
            packet = socket.SendAndRecivePacket(CommandType.Commit, currentProject);
            if (packet.Error != 0) return;
            var archive = ArchiveZip.DirToZip(projectPath[currentProject]);
            socket.SendArchive(archive);
            packet = socket.RecivePacket();
        }
        static void Revert(string[] args)
        {
            if (currentProject == string.Empty)
            {
                packet = new CommandPacket(CommandType.Revert, "") {ErrorInfo = "Set current project"};
                return;
            }
            bool hard = args.Count() == 2 && args[1] == "-hard";
            hard = args.Count() == 1 || hard;
            packet = socket.SendAndRecivePacket(CommandType.Revert, currentProject+" "+ args.FirstOrDefault());
            if (packet.Error != 0) return;
            var archive = socket.RecieveArchive();
            archive.UnpackTo(projectPath[currentProject], hard);
            packet = socket.RecivePacket();

        }
        static void Log(string[] args)
        {
            packet = socket.SendAndRecivePacket(CommandType.Log, args.FirstOrDefault());
        }
    }
}
