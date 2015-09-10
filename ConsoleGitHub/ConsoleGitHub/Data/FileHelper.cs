using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleGitHub.Data
{
    static class FileHelper
    {
        public static void SetReadOnlyHidden(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found" + path);
            var atribute = File.GetAttributes(path);
            if ((atribute & FileAttributes.ReadOnly) == 0)
                atribute |= FileAttributes.ReadOnly;
            if ((atribute & FileAttributes.Hidden) == 0)
                atribute |= FileAttributes.Hidden;
            File.SetAttributes(path, atribute);
        }
        public static void UnSetReadOnlyHidden(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found" + path);
            var atribute = File.GetAttributes(path);
            if ((atribute & FileAttributes.ReadOnly) != 0)
                atribute ^= FileAttributes.ReadOnly;
            if ((atribute & FileAttributes.Hidden) != 0)
                atribute ^= FileAttributes.Hidden;
            File.SetAttributes(path, atribute);
        }
        public static long Size(string path)
        {
            long size = 0;
            if (File.Exists(path))
                return new FileInfo(path).Length;
            if (!Directory.Exists(path)) throw new FileNotFoundException();
            foreach (var file in new DirectoryInfo(path).GetFiles())
                size += file.Length;
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
                size += Size(dir.FullName);
            return size;
        }
        public static string GetFreeTmpName(string extension, string folder = "temp")
        {
            while (extension.StartsWith("."))
                extension = extension.Substring(1);
            for (var i = 0UL; i < ulong.MaxValue; i++)
            {
                string name = $"{folder}/tmp{i}.{extension}";
                if (!File.Exists(name))
                    return name;
            }
            throw new IndexOutOfRangeException();
        }
        public static void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            else if (Directory.Exists(path))
                Directory.Delete(path);
        }
        public static void CreateAllFolders(string path)
        {
            var n = 0;
            var splt = path.Replace("/", "\\").Split('\\');
            if (splt.Last().IndexOf('.') != -1)
                n = 1;
            var subPath = "";
            for (var i = 0; i < splt.Length - n; i++)
            {
                subPath += splt[i] + "\\";
                if (!Directory.Exists(subPath))
                    Directory.CreateDirectory(subPath);
            }
        }
    }
}
