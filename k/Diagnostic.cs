﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace k
{
    public static class Diagnostic
    {
        private static string Path => Shell.Directory.TempDataFolder(R.Project, "log");

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
            var lastAccessZip = R.DebugMode ? DateTime.Now.AddMinutes(-30) : DateTime.Now.AddDays(-P.ClearLogThanNDays);

            var files_log = Shell.File.Find(Path, "*", System.IO.SearchOption.AllDirectories, lastAccessLog);

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
        /// /// <param name="track">Track file</param>
        /// <param name="prj">Project name</param>
        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Error(string log, string track, G.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Error, log, track, prj, message, values);
        }

        public static void Error(string log, G.Projects prj, Exception ex)
        {
            var track = Track(ex);
            Register(DiagnosticType.Error, log, track, prj, ex.Message);

        }

        /// <summary>
        /// Create a error message log
        /// </summary>
        /// <param name="instance">Class hash code or class name</param>
        /// <param name="prj">Project name</param>
        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Error(object instance, G.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Error, instance, null, prj, message, values);
        }
        #endregion

        #region Warning

        /// <summary>
        /// Create a warning message log
        /// </summary>
        /// <param name="instance">Class hash code or class name</param>
        /// <param name="track">Track id</param>
        /// <param name="prj">Project name</param>

        /// <param name="message">Message</param>
        /// <param name="values">Values to string format</param>
        public static void Warning(object instance, string track, G.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Warning, instance, track, prj, message, values); 
        }

        public static void Warning(object instance, G.Projects prj, string message, params object[] values)
        {
            Register(DiagnosticType.Warning, instance, null, prj, message, values);
        }
        #endregion

        #region Debug
        public static void Debug(string log, string track, G.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, log, track, prj, message, values);
        }

        public static void Debug(string log, G.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, log, null, prj, message, values);
        }

        public static void Debug(int hashCode, G.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, $"Instance:{hashCode.ToString("000000")}", null, prj, message, values);
        }

        public static void Debug(int hashCode, string track, G.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, $"Instance:{hashCode.ToString("000000")}", track, prj, message, values);
        }

        public static void Debug(KModel model, G.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                Register(DiagnosticType.Debug, $"Instance:{model.GetHashCode()}", null, prj, message, values);
        }
        #endregion

        #region Track
        public static string TrackObj(object value)
        {

            var formatting = Formatting.Indented;

            var json = JsonConvert.SerializeObject(value,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = formatting
                });

            return Track(json);
        }


        public static string Track(params object[] values)
        {
            var track = k.Security.Id(values);
            track += new string('0', track.Length < 6 ? 6 - track.Length : 0);
            var file = $"{Path}\\track\\{track}.track";

            if(System.IO.File.Exists(file))
            {
                var text = $"Track replay date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                Shell.File.Save(text, file, false, true);
            }
            else
            {
                var list = new List<object>
                {
                    $"Track date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"
                };

                foreach (var value in values)
                {
                    if (value.GetType().BaseType == typeof(Array))
                    {
                        int i = 0;
                        foreach (var value1 in (object[])value)
                            list.Add($"{i++.ToString("0")}. " + value1);
                    }
                    else
                        list.Add(value);
                }

                Shell.File.Save(Array.ConvertAll(list.ToArray(), x => x.ToString()), file, false, true);
            }

            return track;
        }

        public static string Track(Exception ex)
        {
            var inner = ex;
            var list = new List<object>();

            var limit = 0;
            while (inner != null)
            {
                if(!String.IsNullOrEmpty(ex.Message))
                {
                    if(limit > 0)
                        list.Add("\n>>>>> INNER EXCEPTION <<<<<");
                    else
                        list.Add(">>>>> EXCEPTION <<<<<");

                    list.Add(ex.Message);
                    list.Add(ex.StackTrace ?? "> No StackTrace");
                    if (R.DebugMode) list.Add(ex.Source ?? "> No Source");
                }
                
                inner = inner.InnerException;

                if (++limit > 10)
                {
                    list.Add("Inner exception was limited 10 levels");
                    break;
                }
            }

            list.Add("\n");
            return Track(list.ToArray());
        }
        #endregion

        private static void Register(DiagnosticType dtype, object log, string track, G.Projects prj, string message, params object[] values)
        {
#if DEBUG 
            var file = $"{Path}\\{DateTime.Now.ToString("yyMMdd")}_0000.log";
#else
            var file = $"{path}\\{DateTime.Now.ToString("yyMMdd_HH")}00.log";
#endif

            var time = DateTime.Now.ToString("hh:mm:ss");
            var type = dtype.ToString() + new String(' ', 7 - dtype.ToString().Length);
            var trck = !String.IsNullOrEmpty(track) ? $"Track:{track} " : null;
            var msg = Dynamic.StringFormat(message, values);

            var line = $"{time} {type} - {trck}{prj}.{log}: {msg}";

            Shell.File.Save(line, file, false, true);
        }
    }
}
