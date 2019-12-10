using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    internal class KDBException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        public KDBException(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        public KDBException(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
