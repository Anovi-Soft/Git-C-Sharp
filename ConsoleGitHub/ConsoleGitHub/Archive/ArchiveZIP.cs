using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using ConsoleGitHub.Data;
using Ionic.Zip;

namespace ConsoleGitHub.Archive
{
    public class ArchiveZip : IArchive
    {
        public string Path { get; }
    
        private readonly long _folderSize;
        public ArchiveZip(string path, long folderSize = -1)
        {
            _folderSize = folderSize;
            Path = path;
            FileHelper.SetReadOnlyHidden(path);
        }
        ~ArchiveZip()
        {
            if (!File.Exists(Path)) return;
            FileHelper.UnSetReadOnlyHidden(Path);
            File.Delete(Path);
        }
        public static ArchiveZip DirToZip(string directory, Action<int> statusChangeAction = null)
        {
            var path = FileHelper.GetFreeTmpName("zip");
            if (path == null) throw new ArgumentNullException(nameof(path));
            FileHelper.CreateAllFolders(path);

            using (var zipFile = new ZipFile())
            {
                if (statusChangeAction != null)
                    zipFile.SaveProgress += (o, args) => 
                        statusChangeAction((int)(1.0d / args.TotalBytesToTransfer * args.BytesTransferred * 100.0d));
                zipFile.AddDirectory(directory);
                zipFile.Save(path);
            }

            return new ArchiveZip(path, FileHelper.Size(directory));
        }

        public static Task<ArchiveZip> DirToZipAsync(string directory, Action<int> statusChangeAction = null)
        {
            return Task.Run(() => DirToZip(directory, statusChangeAction));
        }


        public void SaveTo(string path)
        {
            FileHelper.CreateAllFolders(path);
            File.Copy(Path, path, true);
            FileHelper.UnSetReadOnlyHidden(path);
        }

        public long SizeOfArchive()
        {
            return new FileInfo(Path).Length;
        }

        public long SizeOfDirectory()
        {
            return _folderSize;
        }

        public void UnpackTo(string path, Action<int> statusChangeAction=null)
        {
            FileHelper.CreateAllFolders(path);
            if (path == null) throw new ArgumentNullException(nameof(path));
            using (var zipFile = ZipFile.Read(Path))
            {
                if (statusChangeAction != null)
                    zipFile.ExtractProgress += (o, args) =>
                        statusChangeAction((int)(1.0d / args.TotalBytesToTransfer * args.BytesTransferred * 100.0d));
                zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public Task UnpackToAsync(string path, Action<int> statusChangeAction=null)
        {
            return Task.Run(() => ((IArchive) this).UnpackTo(path, statusChangeAction));
        }

        public byte[] GetBytes()
        {
            return File.ReadAllBytes(Path);
        }
        
    }
}
