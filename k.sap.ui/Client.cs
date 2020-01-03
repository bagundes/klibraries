using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace k.sap
{
    public class UI : IClient
    {
        private static string LOG => typeof(UI).FullName;

        private static SAPbouiCOM.Application conn;
        public static SAPbouiCOM.Application Conn
        {
            get
            {
                if (conn == null)
                    throw new Exception("Application is not connected SAP UI");
                else
                    return conn;
            }
        }

        public static bool IsConnected()
        {
            return (conn != null);
        }

        /// <summary>
        /// Connect sap using UI API.
        /// </summary>
        /// <param name="connString">If you not define the string connection, the string connection will be default</param>
        public static void Connect(string connString = null)
        {
            try
            {

                SAPbouiCOM.SboGuiApi SboGuiApi = null;
                string sConnectionString = null;

                SboGuiApi = new SAPbouiCOM.SboGuiApi();

                sConnectionString = connString ?? sap.ui.E.ConnString;
                SboGuiApi.Connect(sConnectionString);
                conn = SboGuiApi.GetApplication(-1);

                var debug = false;
#if DEBUG
                debug = true; 
#endif

                if (R.DebugMode && !debug)
                    conn.MessageBox("Addon is connected as debug mode.");
                else
                    conn.SetStatusBarMessage($"{R.CompanyName} addon is connected.", SAPbouiCOM.BoMessageTime.bmt_Short, false);

            }
            catch(Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
                throw new Exception($"Error to connect SAP using DI UIAPI.", ex);
            }

        }

        public static void Disconnect()
        {
            throw new NotImplementedException();
        }

        public static SAPbouiCOM.Form GetActiveForm() => Conn.Forms.ActiveForm;
        public static dynamic GetIdCompany()
        {
            return Conn.Company.GetDICompany();
        }
        public static string GetLastErrorDescription()
        {
            throw new NotImplementedException();
        }

        public static int GetLastErrorCode()
        {
            throw new NotImplementedException();
        }

    }
}
