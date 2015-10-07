using ConsoleGitHub.Data.Version;
using GitHub.Archive;

namespace GitHubConsoleServer.Data
{
    interface IVersionDataProvider
    {
        void PushProject(string name);
        void DeleteProject(string name);
        void JoinArchive(string name, IArchive archive, IVersion version = null);
        IArchive TakeVersion(string name, IVersion version = null);
        string Info(string name=null);
    }
}
