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
        private string clientPath;
        private Logger logger;
        public FolderProvider(string clientName, string ip="Unknown IP")
        {
            clientPath = Path.Combine(projectsPath, clientName);
            if (!Directory.Exists(clientPath)) Directory.CreateDirectory(clientPath);
            logger = new Logger(clientPath, ip);
        }
        private string FullPath(string name)
        {
            var path = Path.Combine(clientPath, name);
            if (!Directory.Exists(path))
                throw new GitHubException("Project do not exists");
            return path;
        }

        public void PushProject(string name)
        {
            var path = Path.Combine(clientPath, name);
            if (Directory.Exists(path))
                throw new GitHubException("Project allready exists");
            Directory.CreateDirectory(path);
            var fileName = new BaseVersion().Zero().ToString();
            fileName = fileName + ".zip";
            fileName = Path.Combine(path, fileName);
            var emptyArchive = ArchiveZip.CreateEmptyArchive();
            emptyArchive.SaveTo(fileName);
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
            if (version != null)
                throw new GitHubException("Method JoinArchive with version NotImplemented", new NotImplementedException());
            var path = FullPath(name);
            version = LastVersion(path).AddVersion(1);
            archive.SaveTo(Path.Combine(path, version+".zip"));
            logger.Log(name, $"Version {version} added");
        }

        public IArchive TakeVersion(string name, IVersion version = null)
        {
            var path = FullPath(name);
            if (version == null)
                version = LastVersion(path);
            path = Path.Combine(path, version +".zip");
            if (!File.Exists(path))
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

        public bool Contain(string name)
        {
            var path = FullPath(name);
            return Directory.Exists(path);
        }

        private static IVersion LastVersion(string path)
        {
            var last = Directory.GetFiles(path, "*.zip").OrderBy(a => a).LastOrDefault();

            if (last == null)
                return new BaseVersion().Zero();
            var version = Path.GetFileNameWithoutExtension(last);
            return VersionFarm.Parse(version);
        }
    }
}
