using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using k.sap.Models;
//using k.db.Clients;
//using k.sap;

namespace k.Tests2
{
    [TestClass]
    public class _SapDiTest
    {

        

        //readonly SAPCredential creddi = new k.sap.SAPCredential((int)SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017,
        //    new k.db.Clients.SqlCredential("DESKTOP-SPOLPCU", "SBODemoGB_Script", "sa", "easy1234"),
        //    (int)SAPbobsCOM.BoSuppLangs.ln_English_Gb,
        //    "manager",
        //    "manager");


        //[TestMethod]
        //public void Init()
        //{
        //    k.StartInit.Starting(new k.sap.di.Init());
        //}

        //[TestMethod]
        //public void Connection()
        //{
        //    //var credsql = new k.db.Clients.SqlCredential("DESKTOP-SPOLPCU", "SBODemoGB_Script", "sa", "easy1234");

        //    //var creddi = new k.sap.SAPCredential((int)SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017,
        //    //    credsql,
        //    //    (int)SAPbobsCOM.BoSuppLangs.ln_English_Gb,
        //    //    "manager",
        //    //    "manager") ;

        //    k.sap.DI.Connect(creddi);
        //    k.sap.DI.Disconnect();

        //}

        //[TestMethod]
        //public void CreateTable()
        //{
        //    k.sap.DI.Connect(creddi);
        //    var t1 = new Test1Table();

        //    var a = new k.sap.di.UserDefineSetup();
        //    a.Add(t1, sap.di.UserDefineSetup.Form.None);
        //    a.Save();
        //    k.sap.DI.Disconnect();

        //}

        //public class Test1Table : k.sap.Models.UserDataTableNoObject
        //{
        //    [SAPColumn(k.sap.E.SAPTables.ColumnsType.Numeric, "Number", 0)]
        //    public int U_Number;

        //    [SAPColumn(k.sap.E.SAPTables.ColumnsType.AlphaNumeric_Text, "Text")]
        //    public string U_Text;

        //    [SAPColumn(k.sap.E.SAPTables.ColumnsType.Special_MD5, "Description")]
        //    public string MD5;

        //    [SAPColumn(E.SAPTables.ColumnsType.AlphaNumeric_Regular, "ValidValues", 2)]
        //    [SAPColumnValidValues(true, "BR","Brazil", "IE","Ireland", "UK","United Kindom")]
        //    public string VValues;

        //    public Test1Table() : base("TEST", "Test table")
        //    {
        //    }
        //}
    }
}
