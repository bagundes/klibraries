using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap
{
    public class DIServer : IClient
    {
        private static string LOG => typeof(DIServer).Name;

        private static k.Lists.SpecificList<String> Tokens = new Lists.SpecificList<string>();

        public static bool IsConnected(string key)
        {
            string token;
            if (!Tokens.Get(key, out token))
            {
                var diServer = new SBODI_Server.Node() as SBODI_Server.INode;

                return true;
            }
            else
                return false;
        }

        

        //public static bool IsDIAPIConnected()
        //{
        //    return (di1 != null && !di1.Connected);
        //}

        //public static void Connect(k.sap.SAPCredential sapCredential)
        //{
        //    if (IsDIAPIConnected())
        //        DI.di1.Disconnect();

        //    DI.di1 = new SAPbobsCOM.Company();

        //    di1 = new SAPbobsCOM.Company();
        //    di1.DbServerType = sapCredential.SapBoDataServerTypes;
        //    di1.Server = sapCredential.DbServer;
        //    di1.language = sapCredential.SapBoSuppLangs;
        //    di1.CompanyDB = sapCredential.SapCompanyDb;
        //    di1.DbUserName = sapCredential.SapDbUserName;
        //    di1.DbPassword = sapCredential.SapDbPassword;
        //    di1.UserName = sapCredential.SapUserName;
        //    di1.Password = sapCredential.SapUserPassword;
        //    var res = di1.Connect();

        //    if (res != 0)
        //    {
        //        var error = di1.GetLastErrorDescription();
        //        var track = k.Diagnostic.Track(sapCredential.DataInfo());
        //        k.Diagnostic.Error(LOG, R.Project, track, $"({res}) {error}.");
        //        throw new k.sap.di.KDIException(LOG, k.sap.di.E.Message.ErrorConnectDI_2, di1.Server, error);
        //    }
        //    else
        //        k.Diagnostic.Debug(LOG, R.Project, "SAP DI connected on {0} customer as {1}", di1.CompanyName, di1.UserName);
        //}

    }
}
