using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.Tests2
{
    [TestClass]
    public class KDBTest
    {
        [TestMethod]
        public void SQlConnection()
        {
            var cred = new k.db.Clients.SqlCredential("DESKTOP-SPOLPCU", "SBODemoGB_Script", "sa", "easy1234");
            var id = cred.Save();
            using (var client = new k.db.Clients.SqlClient())
            {
                client.Connect(cred);
                k.db.Factory.Connection.SetClient(client);
            }

            using (var client = new k.db.Clients.SqlClient())
                client.Connect(id);

        }
    }
}
