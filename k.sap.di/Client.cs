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
        private static string LOG => typeof(DI).FullName;
        private static int transactionNumber;

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

        public static void Connect(dynamic diCompany)
        {
            
            if (IsConnected())
                conn.Disconnect();

            conn = diCompany as SAPbobsCOM.Company;
            k.Diagnostic.Debug(LOG, null, "SAP DI connected using ui api on {0} customer as {1}", conn.CompanyName, conn.UserName);
            LinkClient();
        }
        
        /// <summary>
        /// Connect SAP using DI API and replace string connection default.
        /// </summary>
        /// <param name="sapCredential"></param>
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
                var track = k.Diagnostic.TrackMessages(sapCredential.Info1());
                k.Diagnostic.Error(LOG, track, $"({res}) {error}.");
                throw new Exception($"Error to connect SAP using DI api. Error code {res}", new Exception(error));
            }
            else
            {
                k.Diagnostic.Debug(LOG, null, "SAP DI connected on {0} customer as {1}", conn.CompanyName, conn.UserName);
                LinkClient();
            }
        }

        private static void LinkClient()
        {
            var client = new k.db.Clients.SAPClient();
            k.db.Factory.Connection.SetClient(client);
            k.Diagnostic.Debug(LOG, null, $"Linked the client as SAPClient on {conn.Server}.{conn.CompanyDB}.");
        }

        public static void Disconnect()
        {
            if (IsConnected())
            {
                var name = conn.CompanyName;
                var user = conn.UserName;
                conn.Disconnect();
                k.Diagnostic.Debug(LOG, null, "SAP DI diconnected on {0} customer as {1}", name, user);
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

        public static int GetUserId()
        {
            return k.db.Factory.ResultSet.Get(null, k.sap.Content.Queries.OUSR.UserId_1, Conn.UserName);
        }

        public static Dynamic GetNewObjectKey()
        {
            return new Dynamic(DI.Conn.GetNewObjectKey());
        }

        /// <summary>
        /// Starting a transaction
        /// </summary>
        /// <returns>Number of transaction:
        /// -1) Transaction was created mannually
        /// </returns>
        public static int StartTransaction()
        {
            if (transactionNumber == 0)
            {
                if (DI.Conn.InTransaction)
                    transactionNumber = - 1;
                else
                {
                    DI.Conn.StartTransaction();
                    transactionNumber = k.Security.IdNumber();
                }
                
                return transactionNumber;
            }
            else
            {
                return 0;
            }

            
        }

        /// <summary>
        /// Close the transaction
        /// </summary>
        /// <param name="number">Number of transaction</param>
        /// <param name="boWfTransOpt">Action</param>
        public static void CommitTransaction(int number)
        {
            if (DI.Conn.InTransaction && number > 0 && number == transactionNumber)
            {
                DI.Conn.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                transactionNumber = 0;
            }
        }

        /// <summary>
        /// Close the transaction
        /// </summary>
        /// <param name="number">Number of transaction</param>
        /// <param name="boWfTransOpt">Action</param>
        public static void RollBackTransaction(int number)
        {
            if (DI.Conn.InTransaction && number > 0 && number == transactionNumber)
            {
                DI.Conn.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                transactionNumber = 0;
            }
        }
    }
}
