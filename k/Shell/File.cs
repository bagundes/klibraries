using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace k.Shell
{
    public static class File
    {
        private static string LOG => typeof(File).FullName;
        public static string Write(string text, string name,string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var file = Path.Combine(path, name);
            System.IO.File.WriteAllText(file, text);
            return file;
        }


        #region Loading file
        public static string Load(string name, string path)
        {
            return Load(Path.Combine(path, name));
            
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

        #region Save  file
        public static void Save(string[] lines, string filename, bool ovride = false, bool wait = false)
        {
            var tried = 0;
            var fileInfo = new FileInfo(filename);
            System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

            var exists = System.IO.File.Exists(fileInfo.FullName);

            while (wait == true && IsLocked(fileInfo.FullName) && exists)
            {
                System.Threading.Thread.Sleep(2000);

                //TODO: Fix bug when log/track file is being used
                if (tried++ > 20 
                    && fileInfo.Directory.ToString().Contains(Helpers.DiagnosticHelper.PathLog))                   
                {
                    Save($"The {fileInfo.Name} file is locked a long time.", fileInfo.FullName, false);
                    fileInfo = new FileInfo(fileInfo.FullName + "_locked");
                }
            }      

            if (ovride)
            {
                if (WaitUnlocked(fileInfo.FullName))
                    System.IO.File.WriteAllLines(fileInfo.FullName, lines);
            }
            else
            {
                using (var file = new System.IO.StreamWriter(fileInfo.FullName, true))
                {
                    for (int i = 0; i < lines.Length; i++)
                        file.WriteLine(lines[i]);
                }
            }
        }

        public static void Save(string line, string filename, bool ovride = false, bool wait = false)
        {
            Save(new string[] { line }, filename, ovride, wait);
        }

        public static string Save(Stream stream, string filename, bool replace = true)
        {
            var fileInfo = new FileInfo(filename);

            if (stream == null)
                throw new Exception("The stream is not to be empty");


            if (fileInfo.Exists && !replace)
                return fileInfo.FullName;

            using (var output = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(output);
                stream.Close();
            }

            return fileInfo.FullName;
        }
        #endregion

        #region Compact files
        public static void ZipFiles(FileInfo[] files, string destination)
        {
            ZipFiles(Array.ConvertAll(files, t => t.FullName), destination);
        }

        public static void ZipFiles(string[] files, string destination)
        {
            using(var zip = new ZipFile())
            {
                foreach(var file in files)
                {
                    if(System.IO.File.Exists(file))
                        zip.AddFile(file);

                    zip.Save(destination);
                }

                if (R.DebugMode && files.Length > 0)
                {
                    var track = Diagnostic.TrackMessages(files);
                    Diagnostic.Debug(typeof(File).Name, track, "Compacted {0} files to {1}", files.Length, destination);
                }
            }
        }
        #endregion

        #region Status
        public static bool WaitUnlocked(string filename)
        {
            while (IsLocked(filename))
                System.Threading.Thread.Sleep(1000);

            return true;
        }
        public static bool IsLocked(string file)
        {
            return IsLocked(new FileInfo(file));
        }

        public static bool IsLocked(FileInfo file)
        {
            file.Refresh();
            FileStream stream = null;

            try
            {
                if (file.Exists)
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);

                //arquivo está disponível
                return false;
            }
            catch (IOException)
            {
                //o arquivo está indisponível pelas seguintes causas:
                //está sendo escrito
                //utilizado por uma outra thread
                //não existe ou sendo criado
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        #endregion
        public static void Delete(FileInfo file)
        {
            System.IO.File.Delete(file.FullName);

            if (R.DebugMode)
                Diagnostic.Debug(LOG, null, "File deleted: {0}", file.FullName);            
        }

        public static void Delete(FileInfo[] files)
        {
            foreach(var file in files)
                System.IO.File.Delete(file.FullName);

            if (R.DebugMode && files.Length > 0)
            {
                var track = Diagnostic.TrackMessages(Array.ConvertAll(files, t => t.FullName));
                Diagnostic.Debug(LOG, null, "Deleted {0} files", files.Length);
            }
        }

        public static FileInfo[] Find(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories, DateTime? lastAccess = null)
        {
            var access = lastAccess ?? DateTime.Now;
            try
            {
                return System.IO.Directory.GetFiles(path, searchPattern, searchOption)
                    .Select(f => new FileInfo(f))
                    .Where(f => f.LastAccessTime < access)
                    .ToList()
                    .ToArray();
            
            }catch(Exception ex)
            {
                var track = Diagnostic.TrackObject(new { path, searchPattern, searchOption, lastAccess });
                Diagnostic.Error(LOG, track, $"Error to find the files: {searchOption}. {ex.Message}");
                Diagnostic.Error(LOG, ex);
                throw ex;
            }
        }
    }
}
