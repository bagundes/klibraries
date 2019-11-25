using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace k.Tests
{
    [TestClass]
    public class UnitTest1
    {

        public static void Main()
        {
            // KCore
            KCore.Security();
            KCore.Credential();
        }



        public class KCore
        {
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
                Dynamic a = "test";
            }
        }

    }

    public class Credential : k.Models.Credential
    {
        public void Load(string id)
        {
            base.Load<Credential>(id);
        }
        public Credential() : base(E.Projects.Tests)
        {
           


        }
    }
}
