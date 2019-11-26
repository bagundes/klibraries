using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public class KException : BaseException
    {
        protected override string MessageLangFile => k.R.App.MessageLangFile;

        public KException(string log, Exception ex) : base(E.Projects.KCore, log, ex)
        {
        }

        public KException(string log, E.Message code, params object[] values) : base(E.Projects.KCore, log, code, values)
        {
        }

        
    }
}
