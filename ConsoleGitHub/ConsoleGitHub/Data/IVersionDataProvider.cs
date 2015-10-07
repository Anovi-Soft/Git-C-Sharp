using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Data.Version;

namespace ConsoleGitHub.Data
{
    interface IVersionDataProvider
    {
        void PushProject(string name, IVersion version);
        void DeleteProject(string name);
        void JoinArchive(string name, IVersion version = null);
        IArchive TakeVersion(string name, IVersion version);
        IArchive TakeAllVersions(string name);
        string ProjectInfo(string name);
    }
}
