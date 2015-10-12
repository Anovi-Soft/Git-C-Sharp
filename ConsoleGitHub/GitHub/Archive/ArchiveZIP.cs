using System;
using System.IO;
using System.Threading.Tasks;
using GitHub.Util;
using Ionic.Zip;

namespace GitHub.Archive
{
    public class ArchiveZip : IArchive
    {
        public string Path { get; }
    
        private readonly long _folderSize;
        public ArchiveZip(string path, long folderSize = -1)
        {
            _folderSize = folderSize;
            Path = path;
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

        public void UnpackTo(string path, bool hard = true, Action<int> statusChangeAction=null)
        {
            FileHelper.CreateAllFolders(path);
            if (hard)
            {
                Directory.Delete(path, true);
                FileHelper.CreateAllFolders(path);
            }
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
            return Task.Run(() => 
            UnpackTo(path, true,statusChangeAction));
        }

        public byte[] GetBytes()
        {
            return File.ReadAllBytes(Path);
        }

        public static IArchive CreateEmptyArchive()
        {
            var path = FileHelper.GetFreeTmpName("zip");
            FileHelper.CreateAllFolders(path);
            using (var zipFile = new ZipFile())
                zipFile.Save(path);
                return new ArchiveZip(path,0);
        }
        
    }
}
