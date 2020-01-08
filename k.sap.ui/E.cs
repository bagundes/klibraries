using k.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui
{
    public class E
    {
#if DEBUG
        public static string ConnString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
#else
        public static string ConnString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));
#endif

        internal enum Message
        {
            GenerelError_0 = 0,
            ErrorConnectUI_1 = 1,
            FileFormsNotExists_1 = 2,
            ErrorValue_2 = 3,
            InvalidActiveForm_2 = 4,
        }
    }
}
