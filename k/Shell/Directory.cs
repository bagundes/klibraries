using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace k.Shell
{
    public static class Directory
    {
        public static string AppDataFolder(E.Projects project, params string[] folders)
        {
            folders = folders ?? new string[0];
            var path = new string[folders.Length + 2];

            path[0] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path[1] = project.ToString();
            folders.CopyTo(path, 2);

            string specificFolder = Path.Combine(path);
            System.IO.Directory.CreateDirectory(specificFolder);

            return specificFolder;

        }
    }
}
