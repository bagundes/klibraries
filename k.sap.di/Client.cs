using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace k.sap
{
    public class DI : IClient
    {
        private static string LOG => typeof(DI).Name;

        private static SAPbobsCOM.Company conn;
        public static SAPbobsCOM.ICompany Conn
        {
            get
            {
                if (conn == null || !conn.Connected)
                    throw new Exception("Application is not connected SAP DI");
                else
                    return conn;
            }
        }

        public static bool IsConnected()
        {
            return (conn != null && conn.Connected);
        }

        public static void Connect(k.sap.SAPCredential sapCredential)
        {
            if (IsConnected())
                conn.Disconnect();

           

            conn = new SAPbobsCOM.Company();
            conn.DbServerType =   (SAPbobsCOM.BoDataServerTypes) sapCredential.SapBoDataServerTypes;
            conn.Server = sapCredential.DbServer;
            conn.language = (SAPbobsCOM.BoSuppLangs) sapCredential.SapBoSuppLangs;
            conn.CompanyDB = sapCredential.SapCompanyDb;
            conn.DbUserName = sapCredential.SapDbUserName;
            conn.DbPassword = sapCredential.SapDbPassword;
            conn.UserName = sapCredential.SapUserName;
            conn.Password = sapCredential.SapUserPassword;
            var res = conn.Connect();

            if (res != 0)
            {
                var error = conn.GetLastErrorDescription();
                var track = k.Diagnostic.Track(sapCredential.DetailsFull());
                k.Diagnostic.Error(LOG, R.Project, track, $"({res}) {error}.");
                throw new Exception($"Error to connect SAP using DI api. Error code {res}", new Exception(error));
            }
            else
                k.Diagnostic.Debug(LOG, R.Project, "SAP DI connected on {0} customer as {1}", conn.CompanyName, conn.UserName);
        }

        public static void Disconnect()
        {
            if (IsConnected())
            {
                var name = conn.CompanyName;
                var user = conn.UserName;
                conn.Disconnect();
                k.Diagnostic.Debug(LOG, R.Project, "SAP DI diconnected on {0} customer as {1}", name, user);
            }

            conn = null;
        }

        public static string GetLastErrorDescription()
        {
            return Conn.GetLastErrorDescription();
        }

        public static int GetLastErrorCode()
        {
            return Conn.GetLastErrorCode();
        }

    }
}
