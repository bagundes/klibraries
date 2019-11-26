using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace k
{
    public static class Diagnostic
    {
        private static string path => Shell.Directory.TempDataFolder(R.Project, "log");

        private enum DiagnosticType
        {
            Warning,
            Debug,
            Error,
        }


        internal static void ClearLogSchedule()
        {
            var wait = R.DebugMode ? 600000 /* 10 minutes*/ : 86400000 /* per day */;

            do
            {
                ClearLog();

                Thread.Sleep(wait); 
            } while (true);
        }

        public static void ClearLog()
        {
            
            var zippath = Shell.Directory.TempDataFolder(R.Project);

            var lastAccessLog = R.DebugMode ? DateTime.Now.AddMinutes(-10) : DateTime.Now.AddDays(-1);
            var lastAccessZip = R.DebugMode ? DateTime.Now.AddMinutes(-30) : DateTime.Now.AddDays(-int.Parse(Content.ConfigGlobal.ClearLogThanNDays));

            var files_log = Shell.File.Find(path, "*", System.IO.SearchOption.AllDirectories, lastAccessLog);

            Shell.File.ZipFiles(files_log, $"{zippath}\\log_{DateTime.Now.ToString("yyyyMMdd_hhmm")}.zip");
            Shell.File.Delete(files_log);

            var files_zip = Shell.File.Find(zippath, "*.zip", System.IO.SearchOption.TopDirectoryOnly, lastAccessZip);

            Shell.File.Delete(files_zip);
        }

        #region Error
        /// <summary>
        /// Create a error message log
        /// </summary>
        /// <param name="instance">Class hash code or class name</param>
        /// <param name="prj">Project name</param>
        /// <param name="track">Track file</param>
        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Error(string instance, E.Projects prj, int track, string message, params object[] values)
        {
            Register(DiagnosticType.Error, instance, prj, track, message, values);
        }

        public static void Error(string log, E.Projects prj, Exception ex)
        {
            var track = Track(ex);
            Register(DiagnosticType.Error, instance, prj, track, message, values);

        }

        /// <summary>
        /// Create a error message log
        /// </summary>
        /// <param name="instance">Class hash code or class name</param>
        /// <param name="prj">Project name</param>
        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Error(string instance, E.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Error, instance, prj, -1, message, values);
        }
        #endregion

        #region Warning

        /// <summary>
        /// Create a warning message log
        /// </summary>
        /// <param name="instance">Class hash code or class name</param>
        /// <param name="prj">Project name</param>
        /// <param name="track">Track file</param>
        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Warning(string instance, E.Projects prj, int track, string message, params object[] values)
        {
            Register(DiagnosticType.Warning, instance, prj, track, message, values);
            
        }

        public static void Warning(string instance, E.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Warning, instance, prj, -1, message, values);
        }
        #endregion

        #region Debug
        public static void Debug(string instance, E.Projects prj, int track, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, instance, prj, track, message, values);
        }

        public static void Debug(string instance, E.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, instance, prj, -1, message, values);
        }
        #endregion

        #region Track
        public static int Track(params object[] values)
        {
            var track = k.Security.Id(values);
            var file = $"{path}\\{track}.track";
           
            Shell.File.Save(Array.ConvertAll(values, x => x.ToString()), file, true, true);

            return track;
        }

        public static int Track(Exception ex)
        {
            var inner = ex;
            var list = new List<object>();

            while (inner != null)
            {
                list.Add(ex.Message == null ? "- No Message" : ex.Message);
                list.Add(ex.StackTrace == null? "- No StackTrace" : ex.StackTrace);
                if (R.DebugMode) list.Add(ex.Source == null ? "No Source" : ex.Source);

                list.Add(ex.InnerException == null ? "- No InnerException" : "InnerException\n");

                inner = ex.InnerException;
            }

            return Track(list.ToArray());
        }
        #endregion

        private static void Register(DiagnosticType dtype, string log, E.Projects prj, int track, string message, params object[] values)
        {
            var file = $"{path}\\{DateTime.Now.ToString("yyMMdd_hh")}00.log";

            var time = DateTime.Now.ToString("hh:mm:ss");
            var type = dtype.ToString() + new String(' ', 7 - dtype.ToString().Length);
            var trck = track >= 0 ? $"Track:{track.ToString("00000")}. " : null;
            var msg = Dynamic.StringFormat(message, values);

            var line = $"{time} {type} - {trck}{msg}";

            Shell.File.Save(line, file, false, true);
        }

        private static void OnLine() { }
    }
}
