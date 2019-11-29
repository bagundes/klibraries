using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    internal class KException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        public KException(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        public KException(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
