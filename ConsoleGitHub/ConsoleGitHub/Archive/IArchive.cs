using System;
using System.Threading.Tasks;

namespace ConsoleGitHub.Archive
{
    interface IArchive
    {
        byte[] GetBytes();
        void SaveTo(string path);
        long SizeOfArchive();
        long SizeOfDirectory();
        void UnpackTo(string path, Action<int> statusChangeAction = null);
        Task UnpackToAsync(string path, Action<int> statusChangeAction = null);
    }
}