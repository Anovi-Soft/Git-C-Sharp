using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;

namespace ConsoleGitHub.Data
{
    interface IDataProvider
    {
        void Delete(string directory);
        void InsertArcive(IArchive archive, string directory);
        IArchive TakeArchive(string path);

    }
}
