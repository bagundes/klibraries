using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class MessageHelper
    {
        public static void ErrorInfo(string message, params object[] values)
        {
            k.sap.UI.Conn.StatusBar.SetText(Dynamic.StringFormat(message, values),
                SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        }

        public static void ErrorNotification(string message, params object[] values)
        {
            k.sap.UI.Conn.StatusBar.SetText(Dynamic.StringFormat(message, values),
                SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        }
    }
}
