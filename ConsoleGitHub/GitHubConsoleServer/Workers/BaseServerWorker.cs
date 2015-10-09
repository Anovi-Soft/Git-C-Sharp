using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GitHub;
using GitHub.Network;
using GitHub.Packets;
using GitHubConsoleServer.Auth;
using GitHubConsoleServer.Data;
using static System.String;

namespace GitHubConsoleServer.Workers
{
    class BaseServerWorker:IWorker
    {
        private AdvancedSocket socket;
        private string userName = Empty;
        private ICommandPacket packet;
        private IVersionDataProvider provider;
        public void Run(object arg = null)
        {
            socket = arg as AdvancedSocket;
            if (socket == null) return;
            if (StepAuth()) return;
            provider = new FolderProvider(userName, 
                ((IPEndPoint)socket.socket.RemoteEndPoint).Address.ToString());


            while (true)
                try
                {
                    packet = socket.RecivePacket(CommandType.WorkerCommands);
                    switch (packet.Command)
                    {
                        case CommandType.Add:
                            Add();
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
                }
                catch (SocketException)
                {
                    Console.WriteLine($"[{Logger.Time()}] Connection lost");
                    return;
                }
                catch (Exception e)
                {
                    packet.ErrorInfo = "Unknown server exception";
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    try{socket.SendPacket(packet);}
                    catch (Exception){/*ignored*/}
                    Console.WriteLine($"{packet.Command} {Join(" ",packet.Args)} {packet.ErrorInfo}");
                }
            
        }

        private bool StepAuth()
        {
            var auth = new BaseAuth(socket);

            try
            {
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
                            return true;
                    }

                } while (userName == Empty);
            }
            catch (SocketException)
            {
                Console.WriteLine($"[{Logger.Time()}] Connection lost");
                return true;
            }
            return false;
        }

        private void Add()
        {
            if (packet.IsInvalidArguments(1)) return;
            provider.PushProject(packet.Args.First());
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
