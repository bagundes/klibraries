using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace k.Shell
{
    public static class File
    {
        public static void Write(string text, string name,string path)
        {
            System.IO.File.WriteAllText(Path.Combine(path, name), text);
        }


        #region Loading file
        public static string Load(string name, string path)
        {
            return Path.Combine(path, name);
        }

        public static string Load(FileInfo fileInfo)
        {
            return Load(fileInfo.FullName);
        }

        public static string Load(string fullName)
        {
            return System.IO.File.ReadAllText(fullName);
        }
        #endregion


        public static FileInfo[] Find(string path, string searchPattern = "*", DateTime? lastAccess = null)
        {
            var access = lastAccess ?? DateTime.Now;

            return System.IO.Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastAccessTime < access)
                .ToList()
                .ToArray();
        }
    }
}
