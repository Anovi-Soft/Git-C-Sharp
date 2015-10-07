using System;
using System.Linq;

namespace GitHub.Archive
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
