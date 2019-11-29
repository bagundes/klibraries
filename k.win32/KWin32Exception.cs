using System;
using System.Collections.Generic;
using System.Text;

namespace k.win32
{
    internal class KWin32Exception : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        public KWin32Exception(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        public KWin32Exception(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
