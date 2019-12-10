using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;


namespace k.Tests2
{
    [TestClass]
    public class KTest
    {
        private string LOG => this.GetType().Name;

        [TestMethod]
        public void Diagnostic()
        {

            var ex = new Exception($"Test exception", new Exception($"Inner Test exception"));
            var text = $"Test in text exception {DateTime.Now}";
            
            var track1 = k.Diagnostic.Track(ex);
            var track2 = k.Diagnostic.Track(text);


            k.Diagnostic.Error(LOG, R.Project, ex);
            k.Diagnostic.Error(LOG, R.Project, text, "Format string");
            k.Diagnostic.Error(LOG, track1, R.Project, text + "{0} ", "Format string");

            k.Diagnostic.Warning(LOG, R.Project, text, "Format string");
            k.Diagnostic.Warning(LOG, R.Project, text + "{0}", "Format string");

            k.Diagnostic.Debug(this.GetHashCode(), R.Project, text, "Format string");
            k.Diagnostic.Debug(LOG, R.Project, text, "Format string");
        }

        [TestMethod]
        public void Lists()
        {
            k.StartInit.Starting(new k.Init());
            var myList = new k.Lists.MyList();
            myList.Add("test", "value");
            myList.Set("test", "value1");
            myList.AddForce("test", "value");
            myList.Add("test", "value");

            //var gl = new k.Lists.GenericList();
            //gl.Add("val1", new object());
            //gl.Add("Val", "test2");
            //gl.Add("Val", "test1");
            //gl.Set("vAl1", "test");
            //var foo = gl.Get("val1");

            //if(!foo.ToString().Equals("test"))
            //    throw new ArgumentOutOfRangeException("lists");

            //var sl = new k.Lists.SpecificList<string>();
            //sl.Add("val1", "$$");
            //sl.Add("Val1", "test");
            //var bar = sl.Get("val1");

            //if (bar.Equals("test"))
            //    throw new ArgumentOutOfRangeException("lists");
        }

        [TestMethod]
        public void Models()
        {
            #region credentials
            var credv3 = new Credential
            {
                Host = "test",
                Schema = "test",
                User = "test",
                Password = "test"
            };

            var id = credv3.Save();

            var cred2 = new Credential(id);
            if (!credv3.Equals(cred2))
                throw new ArgumentOutOfRangeException();

            var foo = credv3.GetHashCode();
            #endregion

        }

        [TestMethod]
        public void GlobalConfig()
        {
            var foo = Stored.ConfigFile.GetGlobalValue("ClearLogThanNDays");
            Stored.ConfigFile.SetGlobal("Updated", DateTime.Now);
        }


        class Credential : k.Models.Credential
        {
            public Credential() : base(R.Project)
            {
            }

            public Credential(string id) : base(R.Project)
            {
                Load(id);
            }
        }
    }
}
