using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.diserver
{
    internal class KDIServerException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        public KDIServerException(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        public KDIServerException(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
