using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using ConsoleGitHub.Data.Version;
using GitHub;
using GitHub.Util;
using GitHub.Archive;

namespace GitHubConsoleServer.Data
{
    class FolderProvider : IVersionDataProvider
    {
        private string projectsPath = "Projects";
        private Logger logger;
        public FolderProvider(string clientName, string ip="Unknown IP")
        {
            clientPath = clientName;
            logger = new Logger(clientPath, ip);
        }

        private string clientPath;
        private string FullPath(string name)
        {
            var path = Path.Combine(clientPath, name);
            if (!Directory.Exists(path))
                throw new GitHubException("Project do not exists");
            return path;
        }

        public void PushProject(string name)
        {
            var path = FullPath(name);
            if(Directory.Exists(path))
                throw new GitHubException("Project allready exists");
            Directory.CreateDirectory(path);
            logger.Log(name, "Project pushed");

        }

        public void DeleteProject(string name)
        {
            var path = FullPath(name);
            logger.Log(name, "Project delete");
            Directory.Delete(path, true);
        }

        public void JoinArchive(string name, IArchive archive, IVersion version = null)
        {
            if (version == null)
                throw new GitHubException("Method JoinArchive with version NotImplemented", new NotImplementedException());
            var path = FullPath(name);
            version = LastVersion(path).AddVersion();
            archive.SaveTo(Path.Combine(path, version.ToString()));
            logger.Log(name, $"Version {version} added");
        }

        public IArchive TakeVersion(string name, IVersion version)
        {
            var path = FullPath(name);
            path = Path.Combine(path, version.ToString());
            if (!Directory.Exists(path))
                throw new GitHubException($"Project {name} do not contain {version}");
            logger.Log(name, $"Version {version} revert");
            return ArchiveFarm.Open(path);
        }

        //public IArchive TakeAllVersions(string name)
        //{
        //    var path = FullPath(name);
        //    var archive = ArchiveZip.DirToZip(path);
        //    logger.Log(name, "Clone project");
        //    return archive;
        //}

        public string Info(string name=null)
        {
            if (name != null)
                FullPath(name);
            return logger.Info(name);
        }
        private static IVersion LastVersion(string path)
        {
            var last = Directory.GetFiles(path, "*.zip").OrderBy(a => a).LastOrDefault();
            var version = last == null ? new BaseVersion().Zero() : VersionFarm.Parse(last);
            return version;
        }
    }
}
