using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GitHub.Network;

namespace GitHubConsoleClient
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            socket =
               new AdvancedSocket(
                   new Socket(
                       AddressFamily.InterNetwork,
                       SocketType.Stream,
                       ProtocolType.Tcp));
            //HELLO
            if (!Hello()) return;
            //AUTH
            while (Auth());
            //WORK
            Work();
        }
        private static string yn = "[y/n]";
        private static AdvancedSocket socket;

        static bool Hello()
        {
            string ip;
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

        static void Work()
        {
            
        }
    }
}
