using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.di
{
    internal class KDIException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        public KDIException(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        public KDIException(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
