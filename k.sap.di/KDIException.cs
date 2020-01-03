using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.di
{
    public class KDIException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        internal KDIException(string log, Exception ex) : base(log, ex)
        {
        }

        internal KDIException(string log, E.Message code, params object[] values) : base(log, code, values)
        {
        }

        
    }
}
