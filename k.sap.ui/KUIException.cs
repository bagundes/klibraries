using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.ui
{
    public class KUIException : BaseException
    {
        protected override string MessageLangFile => R.App.MessageLangFile;

        internal KUIException(string log, Exception ex) : base(log, ex)
        {
        }

        internal KUIException(string log, E.Message code, params object[] values) : base(log, code, values)
        {
        }

        internal KUIException(string log, SAPbouiCOM.IItem item) : base(log, E.Message.ErrorValue_2, null)
        {
            var list = new List<object>();
            list.Add(item.UniqueID);
            list.Add(item.Description);
            Values = list.ToArray();
        }


    }
}
