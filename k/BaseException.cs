using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace k
{
    public abstract class BaseException : Exception
    {
        public readonly int Code;
        public readonly string LogName;
        public readonly string Tag;
        protected object[] Values;
        protected int AppId;

        public override string Message => CreateMessage();
        protected abstract string MessageLangFile { get; }

        public BaseException(E.Projects appId, string log, Enum code, params object[] values)
        {
            LogName = log;
            Code = Convert.ToInt32(code);
            Tag = code.ToString();
            Values = values ?? new object[0];
            AppId = Convert.ToInt32(appId);
        }

        public BaseException(E.Projects appId, string log, Exception ex)
        {
            LogName = log;
            Code = 0;
            Tag = "FatalError";
            Values = new object[1] { ex.Message } ;
            AppId = Convert.ToInt32(appId);

            var track = Diagnostic.Track(ex);
            k.Diagnostic.Error(log, appId, track, ex.Message);
        }

        protected string CreateMessage()
        {
            var code = Code.ToString("00000");

            
            var resx = new ResourceManager(MessageLangFile, typeof(Init).Assembly);
            var name = $"M{code}_{Values.Length}";

            try
            {
                var msg = resx.GetString(name);

                if (String.IsNullOrEmpty(msg))
                {
                    k.Diagnostic.Warning(this.GetType().Name, R.Project, "The code {0} ({1}) is not MessageLangFile file", Code, Tag);
                    return $"[{code} - {Tag}] {String.Join(',', Values)}";
                }
                return Dynamic.StringFormat($"[{code}] {msg}", Values);
            }
            catch(Exception ex)
            {
                var track = Diagnostic.Track(ex);
                k.Diagnostic.Error(this.GetType().Name, R.Project, track, "The code {0} ({1}) return exception {2}", Code, Tag, ex.Message);
                return $"[{code} - {Tag}] {String.Join(',', Values)}";
            }
        }

        

    }
}
