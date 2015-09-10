using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;

namespace ConsoleGitHub
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            a.Bind(new IPEndPoint(IPAddress.Any, 24001));
            a.Listen(100);
        }

        static async void Test()
        {
            IArchive zip =  await ArchiveZip.DirToZipAsync(@"C:\temp",Out);
            Console.WriteLine("End Archivate");
            Console.ReadLine();
            await zip.UnpackToAsync(@"c:\test\1", Out);
            Console.WriteLine("End Extract");
        }

        static void Out(int i)
        {
            Console.WriteLine(i);
        }
    }
}
