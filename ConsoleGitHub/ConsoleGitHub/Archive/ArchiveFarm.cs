using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Archive
{
    public class ArchiveFarm
    {
        static public IArchive Open(string path)
        {
            switch (path.Split('.').Last())
            {
                case "zip":
                    return new ArchiveZip(path);
                default:
                    throw new FormatException($"Unknown format {path.Split('.').Last()}");
            }
        }
    }
}
