using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GitHub;
using GitHub.Network;
using GitHub.Packets;
using GitHubConsoleServer.Auth;
using static System.String;

namespace GitHubConsoleServer.Workers
{
    class BaseServerWorker:IWorker
    {
        private AdvancedSocket socket;
        private string userName = Empty;
        private ICommandPacket packet;
        public void Run(object arg = null)
        {
            socket = arg as AdvancedSocket;
            if (socket == null) return;
            
            var auth = new BaseAuth(socket);
            do
            {
                packet = socket.RecivePacket(CommandType.Auth);
                switch (packet.Command)
                {
                    case CommandType.Login:
                        userName = auth.Login(packet);
                        break;
                    case CommandType.Registration:
                        userName = auth.Registration(packet);
                        break;
                    default:
                        return;
                }
            } while (userName == Empty);


            while (true)
                try
                {
                    packet = socket.RecivePacket(CommandType.WorkerCommands);
                    switch (packet.Command)
                    {
                        case CommandType.Add:

                            break;
                        case CommandType.Clone:

                            break;
                        case CommandType.Commit:

                            break;
                        case CommandType.Update:

                            break;
                        case CommandType.Revert:

                            break;
                        case CommandType.Log:

                            break;
                        case CommandType.GoodBy:
                            Console.WriteLine($"[{DateTime.Now}] client say goodby");
                            return;
                        default:
                            return;
                    }
                }
                catch (GitHubException e)
                {
                    packet.ErrorInfo = e.Message;
                    Console.WriteLine(e.Message);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Connection lost");
                }
                catch (Exception e)
                {
                    packet.ErrorInfo = "Unknown server exception";
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    socket.SendPacket(packet);
                }

            
        }
        private void Add()
        {

        }
        private void Clone()
        {

        }
        private void Update()
        {

        }
        private void Commit()
        {

        }
        private void Revert()
        {

        }
        private void Log()
        {

        }
    }
}
