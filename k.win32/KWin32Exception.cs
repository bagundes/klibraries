using System;
using System.Collections.Generic;
using System.Text;

namespace k.win32
{
    public class KWin32Exception : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        internal KWin32Exception(string log, Exception ex) : base(R.Project, log, ex)
        {
        }

        internal KWin32Exception(string log, E.Message code, params object[] values) : base(R.Project, log, code, values)
        {
        }

        
    }
}
