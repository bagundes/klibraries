using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    public class KDBException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        internal KDBException(string log, Exception ex) : base(log, ex)
        {
        }

        internal KDBException(string log, E.Message code, params object[] values) : base(log, code, values)
        {
        }

        
    }
}
