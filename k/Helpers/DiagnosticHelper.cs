using System;
using System.Collections.Generic;
using System.Text;

namespace k.Helpers
{
    public class DiagnosticHelper
    {
        public static string PathLog => Shell.Directory.TempDataFolder(R.Project, "log");
        public static string PathTrack => Shell.Directory.TempDataFolder(R.Project, "log", "track");

        internal static void Clear()
        {

            var zippath = Shell.Directory.TempDataFolder(R.Project);

            var lastAccessLog = R.DebugMode ? DateTime.Now.AddMinutes(-10) : DateTime.Now.AddDays(-P.ClearLogThanNDays);
            var lastAccessZip = R.DebugMode ? DateTime.Now.AddMinutes(-30) : DateTime.Now.AddDays(-30);

            var files_log = Shell.File.Find(PathLog, "*", System.IO.SearchOption.AllDirectories, lastAccessLog);

            Shell.File.ZipFiles(files_log, $"{zippath}\\log_{DateTime.Now.ToString("yyyyMMdd_hhmm")}.zip");
            Shell.File.Delete(files_log);

            var files_zip = Shell.File.Find(zippath, "*.zip", System.IO.SearchOption.TopDirectoryOnly, lastAccessZip);
            Shell.File.Delete(files_zip);
        }
    }
}
