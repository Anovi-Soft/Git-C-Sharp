using System;
using System.Threading;
using ConsoleGitHub.Archive;

namespace ConsoleGitHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            while (true)
            {
                Thread.Sleep(10000);
                Console.WriteLine("|||||||||||||||||||||||||||||||||||||");   
            }
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
