using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SAPbobsCOM;


namespace k.Tests
{
    [TestClass]
    public class Win32Test
    {

        public static void Main()
        {
            k.StartInit.Starting(new k.Init());

            // KCore
            KCore.Dynamic1();
            KCore.Security();
            KCore.Credential();
            KCore.List();
            KCore.Shell();
            KCore.KExceptionTest();

            // SAPDI
            KCore.SAPDI();
            KCore.SAPDIServer();

            return;
        }



        public class KCore
        {
            public static void KExceptionTest()
            {
                //try
                //{
                //    throw new k.KException("Test", E.Message.GenerelError_1, "test");
                //}
                //catch(BaseException be)
                //{
                //    var msg = be.Message;
                //}

                //try
                //{
                //    throw new k.KException("Test", new System.Exception("ex"));
                //}
                //catch (BaseException be)
                //{
                //    var msg = be.Message;
                //}

                //try
                //{
                //    throw new k.KException("Test", E.Message.TestMessage, "test");
                //}
                //catch (BaseException be)
                //{
                //    var msg = be.Message;
                //}

            }

            public static void Security()
            {
                var value = k.Security.RandomChars(99999, true);
                var key = k.Security.RandomChars(15, true);

                var a = k.Security.Encrypt(value, key);
                var b = k.Security.Decrypt(a, key);
                var c = k.Security.Token(a);
                var d = k.Security.Token(a,b);
                var e = k.Security.Hash(a, b, c, d);
            }

            public static void List()
            {
                

                var par = new k.Lists.GenericList();
                par.Set("a", "test1");
                par.Set("A", "test2");
                if (par["a"] != "test2")
                    throw new InternalTestFailureException();

                par.Set("b", "test1");

                Dynamic foo;
                bool t;

                t = par.Get("B", out foo); // true
                if (!t)
                    throw new InternalTestFailureException();

                t = par.Get("c", out foo); // false
                if (t)
                    throw new InternalTestFailureException();

            }

            public static void Credential()
            {
                var cred = new Credential();
                cred.Host = "pinkmannew";
                cred.Schema = "sbodemogb";
                cred.User = "sa";
                cred.Password = "12345";
                cred.SetParameter("sap", "9.2PL4");
                cred.SetRole("customer");

                var epsw = cred.GetEncryptPassword();
                cred.SetEncryptPassword(epsw);

                var id = cred.Save();

                var cred1 = new Credential();
                cred1.Load(id);

            }

            public static void Dynamic1()
            {
                Dynamic s1 = "test {0}, {1}";
                var hash = s1.GetHashCode();
                string s2 = s1;

                Dynamic i1 = 1;
                hash = i1.GetHashCode();
                int i2 = i1;
                hash = i2.GetHashCode();

                Dynamic d1 = DateTime.Now;
                DateTime d2 = d1;

                int i3 = s1.ToInt();

                var d = Dynamic.StringFormat(s1, "a1", "b1");



            }

            public static void SAPDI()
            {
                k.StartInit.Starting(new k.sap.di.Init());

                var dbcred = new k.db.Clients.SqlCredential("DESKTOP-SPOLPCU", "SBODemoGB", "sa", "easy1234");
                var sapcred = new k.sap.SAPCredential(BoDataServerTypes.dst_MSSQL2017, dbcred, BoSuppLangs.ln_English_Gb, "manager", "manager");

                sap.DI.Connect(sapcred);
            }

            public static void SAPDIServer()
            {
                k.StartInit.Starting(new k.sap.diserver.Init());

                var dbcred = new k.db.Clients.SqlCredential("DESKTOP-SPOLPCU", "SBODemoGB", "sa", "easy1234");
                var sapcred = new k.sap.SAPCredential(BoDataServerTypes.dst_MSSQL2017, dbcred, BoSuppLangs.ln_English_Gb, "manager", "manager");


            }

            public static void Shell()
            {
                // Directory
                var folder = k.Shell.Directory.AppDataFolder(G.Projects.Tests);
                if(!System.IO.Directory.Exists(folder))
                    throw new InternalTestFailureException();
                folder = k.Shell.Directory.AppDataFolder(G.Projects.Tests, "a");
                if (!System.IO.Directory.Exists(folder))
                    throw new InternalTestFailureException();

                k.Shell.Directory.DelTree(G.Projects.Tests, k.Shell.Directory.SpecialFolder.AppData, "a");
                folder = k.Shell.Directory.AppDataFolder(G.Projects.Tests, "a", "b");
                if (!System.IO.Directory.Exists(folder))
                    throw new InternalTestFailureException();

              
                folder = k.Shell.Directory.AppDataFolder(G.Projects.Tests, "a", "b");
                if (!System.IO.Directory.Exists(folder))
                    throw new InternalTestFailureException();



                // File
                var text = "test";
                k.Shell.File.Write(text, "test1.txt", folder);
                string foo;
                if (!System.IO.File.Exists(folder + "\\test1.txt"))
                    throw new InternalTestFailureException();

                foo = k.Shell.File.Load("test1.txt", folder);
                if (foo != text)
                    throw new InternalTestFailureException();

                foo = k.Shell.File.Load("test1.txt", folder);
                if (foo != text)
                    throw new InternalTestFailureException();

                foo = k.Shell.File.Load("test1.txt", folder);
                if (foo != text)
                    throw new InternalTestFailureException();


            }
        }

    }

    public class Credential : k.Models.Credential
    {
        public void Load(string id)
        {
            base.Load(id);
        }
        public Credential() : base(G.Projects.Tests)
        {
           


        }
    }
}
