using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitHub.Network;
using GitHubConsoleServer.Workers;

namespace GitHubConsoleServer
{
    class ServerProgram
    {
        private const int maxClientsCount = 100;
        private static Socket socket = 
            new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        private static int currentTasksCount;
        private static List<Task> listTasks = new List<Task>(); 

        private static void MainLoop(IWorker currentWorker)
        {
            socket.Bind(
                new IPEndPoint(
                    IPAddress.Any,
                    AdvancedSocket.Port));
            socket.Listen(maxClientsCount + 1);
            while (true)
                try
                {
                    AdvancedSocket _socket = new AdvancedSocket(socket.Accept());
                    var packet = _socket.RecivePacket(CommandType.Hello);

                    if (currentTasksCount == maxClientsCount)
                        packet.ErrorInfo = "Server is bussy, please try again later";
                    _socket.SendPacket(packet);

                    if (packet.Error != 0)
                    {
                        _socket.Close();
                        continue;
                    }

                    var task = Task.Factory.StartNew(() =>
                    {
                        currentTasksCount++;
                        int numOfTask = listTasks.Count;
                        Console.WriteLine($"[{DateTime.Now}]Task№{numOfTask} start working");
                        new BaseServerWorker().Run(_socket);
                        _socket.Close();
                        return numOfTask;
                    });

                    listTasks.Add(task);

                    task.ContinueWith(numOfTask =>
                    {
                        currentTasksCount--;
                        Console.WriteLine($"[{DateTime.Now}]Task№{numOfTask} end working");
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }




        static void Main(string[] args)
        {
            Console.WriteLine("Hello Client");
            var task = Task.Factory.StartNew(() => MainLoop(new BaseServerWorker()));
            while (!task.IsCompleted)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
