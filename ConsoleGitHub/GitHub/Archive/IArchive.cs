using System;
using System.Threading.Tasks;

namespace GitHub.Archive
{
    public interface IArchive
    {
        string Path { get; }
        byte[] GetBytes();
        void SaveTo(string path);
        long SizeOfArchive();
        long SizeOfDirectory();
        void UnpackTo(string path, Action<int> statusChangeAction = null);
        Task UnpackToAsync(string path, Action<int> statusChangeAction = null);
    }
}