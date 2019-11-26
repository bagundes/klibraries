using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace k.Shell
{
    public static class Directory
    {
        private static string LOG => typeof(Directory).Name;
        public enum SpecialFolder
        {
            AppData = 26,
            TempData = 99,
        }

        public static string TempDataFolder(E.Projects project, params string[] folders)
        {
            var path = new string[2];

            path[0] = $"{System.IO.Path.GetTempPath()}{R.CompanyName}";
            path[1] = project.ToString().ToLower();            

            return Folder(Path.Combine(path), folders);
        }

        public static string AppDataFolder(E.Projects project, params string[] folders)
        {
            var path = new string[2];

            path[0] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path[1] = project.ToString();

            return Folder(Path.Combine(path), folders);
        }

        /// <summary>
        /// Delete files, folder and subfolder
        /// </summary>
        /// <param name="project">Project</param>
        /// <param name="folders">Folders to delete</param>
        public static void DelTree(E.Projects project, SpecialFolder special, params string[] folders)
        {
            folders = folders ?? new string[0];
            var path = new string[folders.Length + 2];
            folders.CopyTo(path, 2);

            switch(special)
            {
                case SpecialFolder.AppData:
                    path[0] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); break;
                case SpecialFolder.TempData:
                    path[0] = $"{System.IO.Path.GetTempPath()}{R.CompanyName}"; break;
                default:
                    throw new NotImplementedException($"Cannot defined {special.ToString()} folder in Directory.DelTree");

            }

            path[0] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
                    k.Diagnostic.Debug("Folder", E.Projects.KCore, "Created folder:{0}", specificFolder);
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
