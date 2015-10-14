using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ConsoleGitHub.Data.Version;
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
                            Clone();
                            break;
                        case CommandType.Commit:
                            Commit();
                            break;
                        case CommandType.Update:
                            Update();
                            break;
                        case CommandType.Revert:
                            Revert();
                            break;
                        case CommandType.Log:
                            Log();
                            break;
                        case CommandType.GoodBy:
                            Console.WriteLine($"[{Logger.Time()}] client say goodby");
                            return;
                        default:
                            return;
                    }
                }
                catch (SocketException)
                {
                    packet.ErrorInfo = "Connection lost";
                    return;
                }
                catch (GitHubException e)
                {
                    packet.ErrorInfo = e.Message;
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
                    if(packet.Command != CommandType.Log)
                        Console.WriteLine($"[{Logger.Time()}][{userName}] {packet.Command} {Join(" ",packet.Args)} {packet.ErrorInfo}");
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
            provider.Log(packet.Args.First(), $"Project {packet.Args.First()} added");
        }
        private void Clone()
        {
            if (packet.IsInvalidArguments(1)) return;
            var archive = provider.TakeVersion(packet.Args.First());
            socket.SendPacket(packet);
            socket.SendArchive(archive);
            provider.Log(packet.Args.First(), $"Project {packet.Args.First()} cloned");
        }
        private void Update()
        {
            if (packet.IsInvalidArguments(1)) return;
            if (!provider.Contain(packet.Args.First()))
            {
                packet.ErrorInfo = "Project do not exist";
                return;
            }
            var archive = provider.TakeVersion(packet.Args.First());
            socket.SendPacket(packet);
            socket.SendArchive(archive);
            provider.Log(packet.Args.First(), $"Update project {packet.Args.First()} to user");

        }
        private void Commit()
        {
            if (packet.IsInvalidArguments(1)) return;
            if (!provider.Contain(packet.Args.First()))
            {
                packet.ErrorInfo = "Project do not exist";
                return;
            }
            socket.SendPacket(packet);
            var archive = socket.RecieveArchive();
            provider.JoinArchive(packet.Args.First(), archive);
            provider.Log(packet.Args.First(), $"Project {packet.Args.First()} commit to server");
        }
        private void Revert()
        {
            if (packet.Args.Count() > 2)
            {
                packet.SetAsInvalidArgument();
                return;
            }
            if (!provider.Contain(packet.Args.First()))
            {
                packet.ErrorInfo = "Project do not exist";
                return;
            }
            var version = packet.Args.Count() == 2 ?
                new BaseVersion().Parse(packet.Args[1]) :
                null;
            var archive = provider.TakeVersion(packet.Args.First(), version);
            socket.SendPacket(packet);
            socket.SendArchive(archive);
            provider.Log(packet.Args.First(), $"Project {packet.Args.First()} revert to user");
        }
        private void Log()
        {
            if (packet.Args.Count() > 1)
            {
                packet.SetAsInvalidArgument();
                return;
            }
            packet.ErrorInfo = packet.Args.Any()
                ? provider.Info(packet.Args.First())
                : provider.Info();
        }
    }
}
