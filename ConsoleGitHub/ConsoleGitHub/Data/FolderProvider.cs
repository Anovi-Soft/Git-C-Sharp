using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Archive;
using ConsoleGitHub.Data;

namespace ConsoleGitHub.Data
{
    /// <summary>
    /// unused
    /// </summary>
    class FolderProvider : IVersionDataProvider
    {
        public void Delete(string directory)
        {
            FileHelper.Delete(directory);
        }

        public void InsertArcive(IArchive archive, string directory)
        {
            FileHelper.CreateAllFolders(directory);
            archive.UnpackTo(directory);
        }

        public IArchive TakeArchive(string directory)
        {
            return ArchiveFarm.Open(directory);
        }
    }
}
