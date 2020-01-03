using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public class KException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        internal KException(string log, Exception ex) : base(log, ex)
        {
        }

        internal KException(string log, E.Message code, params object[] values) : base(log, code, values)
        {
        }

        
    }
}
