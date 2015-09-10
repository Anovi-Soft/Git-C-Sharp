using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Version;

namespace ConsoleGitHub.Data
{
    interface IVersionDataProvider:IDataProvider
    {
        IArchive TakeArchive(string diectory, IVersion version);
        IArchive TakeAll(string diectory);
    }
}
