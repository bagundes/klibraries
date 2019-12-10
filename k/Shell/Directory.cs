using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace k.Shell
{
    public static class Directory
    {
        public enum SpecialFolder
        {
            AppData = 26,
            TempData = 99,
        }


        public static System.IO.DirectoryInfo GetSpecialFolder(SpecialFolder sfolder)
        {
            System.IO.DirectoryInfo di;
            switch (sfolder)
            {
                case SpecialFolder.AppData:
                    di = new DirectoryInfo(R.App.AppData); break;
                case SpecialFolder.TempData:
                    di = new DirectoryInfo(R.App.AppData); break;
                default:
                    throw new NotImplementedException($"Cannot defined {sfolder.ToString()} folder in Directory.DelTree");

            }

            if (di.Exists)
                Folder(di.FullName);

            return di;
        }


        public static string TempDataFolder(G.Projects project, params string[] folders)
        {
            var path = new string[2];

            path[0] = R.App.AppTemp;
            path[1] = project.ToString().ToLower();            

            return Folder(Path.Combine(path), folders);
        }

        public static string AppDataFolder(G.Projects project, params string[] folders)
        {
            var path = new string[2];
            var b = R.App.AppData;
            path[0] = R.App.AppData;
            path[1] = project.ToString();

            return Folder(Path.Combine(path), folders);
        }

        /// <summary>
        /// Delete files, folder and subfolder
        /// </summary>
        /// <param name="project">Project</param>
        /// <param name="folders">Folders to delete</param>
        public static void DelTree(G.Projects project, SpecialFolder special, params string[] folders)
        {
            folders = folders ?? new string[0];
            var path = new string[folders.Length + 2];
            folders.CopyTo(path, 2);

            path[0] = GetSpecialFolder(special).FullName;
            path[1] = project.ToString();

            var folder = Path.Combine(path);

            //foreach (var file in File.Find(folder))
            //    System.IO.File.Delete(file.FullName);

            System.IO.Directory.Delete(folder, true);
        }

        private static string Folder(string folder, params string[] folders)
        {
            try
            {
                folders = folders ?? new string[0];
                var path = new string[folders.Length + 1];

                path[0] = folder;
                folders.CopyTo(path, 1);

                string specificFolder = Path.Combine(path);

                if (!System.IO.Directory.Exists(specificFolder))
                {
                    System.IO.Directory.CreateDirectory(specificFolder);
                    k.Diagnostic.Debug("Folder", R.Project, "Created folder:{0}", specificFolder);
                }

                return specificFolder;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
