using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using k.Structs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace k
{
    public static class Diagnostic
    {
        

        private enum DiagnosticType
        {
            Warning,
            Debug,
            Error,
        }

        #region Error
        public static void Error(object instance, Track? track, string message, params object[] values)
        {
            object log;
            Details(instance, track, out track, out log);
            Register(DiagnosticType.Error, log, track,  message, values);
        }

        public static void Error<TEx>(object instance, TEx ex) where TEx : Exception
        {
            Track? track = TrackException(ex);
            object log;
            Details(instance, track, out track, out log);
            Register(DiagnosticType.Error, log, track, $"{ex.GetType().Name}: {ex.Message.Replace(System.Environment.NewLine, " ")}");
        }

        #endregion

        #region Warning
        public static void Warning(object instance, Track? track, string message, params object[] values)
        {
            object log;
            Details(instance, track, out track, out log);
            Register(DiagnosticType.Warning, log, null, message, values);
        }

        public static void Warning<TEx>(object instance, TEx ex) where TEx : Exception
        {
            Track? track = TrackException(ex);
            object log;
            Details(instance, track, out track, out log);
            Register(DiagnosticType.Warning, log, track, $"{ex.GetType().Name}: {ex.Message}");
        }
        #endregion

        #region Debug

        public static void Debug(object instance, Track? track, string message, params object[] values)
        {
            if (R.DebugMode)
            {
                object log;
                Details(instance, track, out track, out log);

                Register(DiagnosticType.Debug, log, track, message, values);
            }
        }
        #endregion

        #region Track

        public static Track TrackMessages(params object[] values)
        {
            if (values is null)
                return Track.TrackNull();

            var track = new Track(Helpers.DiagnosticHelper.PathTrack);
            
            track.CreateId(values);
            var text = $"Track date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
           
            

            // if file exists, it will only mark as replay track.
            if (System.IO.File.Exists(track.File))
            {
                Shell.File.Save(text, track.File, false, true);
            }
            else
            {
                var list = new List<object>();

                foreach (var value in values)
                {
                    if (value.GetType().BaseType == typeof(Array))
                    {
                        int i = 0;
                        foreach (var value1 in (object[])value)
                            list.Add($"\t{i++.ToString("0")}. " + value1);
                    }
                    else
                        list.Add(value);
                }

                list.Add(">>>>>>>>>>>>>>>>>>>>>>>>\n" + text);

                Shell.File.Save(Array.ConvertAll(list.ToArray(), x => x.ToString()), track.File, false, true);
            }

            return track;
        }

        public static Track TrackObject(object value)
        {
            if (value is null)
                return Track.TrackNull();

            string json;

            if (value is KModel)
                json = ((KModel)value).ToJson();
            else
            {
                try
                {

                    var formatting = Formatting.Indented;

                    json = JsonConvert.SerializeObject(value,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            Formatting = formatting
                        });
                }catch(Exception ex)
                {
                    var track = TrackException(ex);
                    json = $"Sorry! K library cannot serialize {value.GetType().FullName} object. You can check track id {track.Id}";
                }
            }
            return TrackMessages("Class:" + value.GetType().FullName, json);
        }

        public static Track TrackException<TEx>(TEx ex) where TEx : Exception
        {
            Exception inner = ex;
            var list = new List<object>();

            var limit = 0;
            //list.Add($"{ ex.GetType().Name.ToUpper()}");

            while (inner != null)
            {
                if (!String.IsNullOrEmpty(ex.Message))
                {
                    if(limit > 0)
                        list.Add("\n>>>>> INNER EXCEPTION <<<<<");
                    else
                        list.Add($">>>>> { ex.GetType().Name.ToUpper()} <<<<<");

                    list.Add(ex.Message);

                    if (ex.StackTrace != null)
                    {
                        list.Add("> STACKTRACE");
                        list.Add(ex.StackTrace);
                    }else
                    {
                        list.Add("> No StackTrace");
                    }

                    if (R.DebugMode)
                    {
                        if (ex.Source != null)
                        {
                            list.Add("> SOURCE");
                            list.Add(ex.Source);
                        }
                        else
                        {
                            list.Add("> No Source");
                        }
                    }
                }
                
                inner = inner.InnerException;

                if (++limit > 10)
                {
                    list.Add("Inner exception was limited 10 levels");
                    break;
                }
            }

            //list.Add("\n");

            return TrackMessages(list.ToArray());
        }
        #endregion

        #region Private methods
        private static void Register(DiagnosticType dtype, object log, Track? track, string message, params object[] values)
        {
#if DEBUG 
            var file = $"{Helpers.DiagnosticHelper.PathLog}\\{DateTime.Now.ToString("yyMMdd")}_0000.log";
#else
            var file = $"{Helpers.DiagnosticHelper.PathLog}\\{DateTime.Now.ToString("yyMMdd_HH")}00.log";
#endif

            var time = DateTime.Now.ToString("HH:mm:ss");
            var type = dtype.ToString() + new String(' ', 7 - dtype.ToString().Length);
            var trck = track;
            if (message is null)
                message = "Sorry! No messages logged.";
            var msg = Dynamic.StringFormat(message, values);

            var line = $"{time} {type} - {log} {trck}: {msg}";

            Shell.File.Save(line, file, false, true);
        }

        private static void Details(object instance, Track? trackin, out Track? track, out object log)
        {
            if(instance is null)
            {
                track = trackin;
                log = Track.Null;
            }
            else if (instance is int)
            {
                track = trackin;
                log = $"C{(int)instance}";
            }
            else if (instance is string)
            {
                track = trackin;
                log = (string)instance;
            }
            else
            {
                log = $"{instance.GetType().FullName}:C{instance.GetHashCode()}";
                track = trackin ?? TrackObject(instance);
            }
        }
        #endregion
    }
}
