using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace k
{
    public abstract class KCoreException : Exception
    {
        public readonly int Code;
        public readonly string LogName;
        public readonly string Tag;
        protected object[] Values;
        protected int AppId;

        public override string Message => CreateMessage();
        protected abstract ResourceManager Resx { get; }

        public KCoreException(E.Projects appId, string log, Enum code, params object[] values)
        {
            LogName = log;
            Code = Convert.ToInt32(code);
            Tag = code.ToString();
            Values = values;
            AppId = Convert.ToInt32(appId);
        }

        public KCoreException(E.Projects proj, string log, Exception ex)
        {

        }

        protected string CreateMessage()
        {

        }

        protected void CreateTrackFile()
        {

        }

    }
}
