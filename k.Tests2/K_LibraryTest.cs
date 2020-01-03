using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using k;
using System.Collections.Generic;

namespace k.Tests2
{
    [TestClass]
    public class K_LibraryTest
    {
        private string LOG => this.GetType().Name;

        [TestMethod]
        public void _Init()
        {
            k.StartInit.Starting(new k.Init());
        }

        [TestMethod]
        public void Credential()
        {
            var pass = k.Security.RandomChars(8, false);
            var cred = new TestCredential();
            cred.SetPassword(pass);
            if (!cred.IsValidPassword(pass))
                throw new Exception("the password is invalid");

            cred.User = "user1";

            if (cred.IsValidPassword(pass))
                throw new Exception("the password cannot be equals");

            var epass = cred.EPassword;
            cred.SetPassword(pass);
            if (cred.EPassword == epass)
                throw new Exception("the password cannot be equals");

            var id = cred.Save();
            var credClone = new TestCredential(id);

            if(cred.User != credClone.User || cred.EPassword != credClone.EPassword)
                throw new Exception("Credential cannot load by id");

        }

        [TestMethod]
        public void Diagnostic()
        {
            #region Track
            var tracks = new List<k.Structs.Track>();

            tracks.Add(k.Diagnostic.TrackMessages("track log test", "message 1"));
            tracks.Add(k.Diagnostic.TrackObject(tracks));
            tracks.Add(k.Diagnostic.TrackException(new Exception("exception test", new Exception("inner join test"))));
            tracks.Add(k.Diagnostic.TrackMessages(null));
            tracks.Add(k.Diagnostic.TrackObject(null));

            // Test if duplicate the tracks
            tracks.Add(k.Diagnostic.TrackMessages("track log test", "message 1"));
            tracks.Add(k.Diagnostic.TrackObject(tracks));
            tracks.Add(k.Diagnostic.TrackException(new Exception("exception test", new Exception("inner join test"))));

            foreach (var track in tracks)
                if (!System.IO.File.Exists(track.File) && track.Id != k.Structs.Track.Null)
                    throw new System.IO.FileNotFoundException($"Track ID {track.Id}", track.File);
            #endregion

            #region Diagnostic message
            k.Diagnostic.Debug(this.GetHashCode(), null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Debug(LOG, tracks[0], G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Debug(new TestCredential(), null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Debug(new TestCredential(), tracks[0], G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Debug(this, null, G.Projects.Tests, "Test with hash code");
            
            k.Diagnostic.Error(this.GetHashCode(), null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Error(this, null, G.Projects.Tests, null);
            k.Diagnostic.Error(null, null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Error(LOG, G.Projects.Tests, new Exception("Created exception object", new Exception("Created inner exception object")));

            k.Diagnostic.Warning(this.GetHashCode(), null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Warning(null, null, G.Projects.Tests, "Test with hash code");
            k.Diagnostic.Warning(this, null, G.Projects.Tests, null);
            k.Diagnostic.Warning(LOG, G.Projects.Tests, new Exception("Created exception object", new Exception("Created inner exception object")));
            #endregion
        }

        [TestMethod]
        public void Security()
        {
            var length = DateTime.Now.Millisecond;
            var random = k.Security.RandomChars(length, true);
            if(random.Length != length)
                throw new Exception($"random is not right size");

            var key = k.Security.RandomChars(512, true);
            if (key.Contains(random))
                throw new Exception($"random contains in the key");

            var a = k.Security.Encrypt(random);
            if(random != k.Security.Decrypt(a))
                throw new Exception($"error when tried decrypt");

            var b = k.Security.Encrypt(random, key);
            if (random != k.Security.Decrypt(b, key))
                throw new Exception($"error when tried decrypt");

            if((_= k.Security.Hash(random, key)).Length < 5)
                throw new Exception($"error create the hash");

            if ((_ = k.Security.Id(random, key)).Length < 1)
                throw new Exception($"error create the id");

            if ((_ = k.Security.Token(random, key)).Length < 5)
                throw new Exception($"error create the token");

        }

        [TestMethod]
        public void Dynamic()
        {
            Dynamic @string = "test";
            Dynamic @date = DateTime.Now;
            Dynamic @bool = true;
            Dynamic @double = 1.678;
            Dynamic @int = 1234567;
            Dynamic @char = 'f';
        }

        [TestMethod]
        public void List()
        {

            var myList2x2 = new k.Lists.MyList(2, 2);
            myList2x2.Add("a", 1, 0);
            myList2x2.Add("b", 2, 0);
            try
            {
                myList2x2.Add("c", 3, 0); // Creating error
            }catch(k.KException ex)
            {
                if (ex.Code != 3)
                    throw new Exception("Test error", ex);
            }

            myList2x2.Add("a", 1, 1);
            myList2x2.Add("b", 2, 1);
            try
            {
                myList2x2.Add("b", 2, 2); // Creating error
            }
            catch (k.KException ex)
            {
                if (ex.Code != 4)
                    throw new Exception("Test error", ex);
            }

            var newval = "hello!";
            myList2x2[0, "a"] = newval;

            if(myList2x2[0, "a"] != newval)
                throw new Exception("Test error");
            if(myList2x2[0].Count != 2)
                throw new Exception("Test error");
        }

        
        //[TestMethod]
        //public void GlobalConfig()
        //{
        //    var foo = Stored.ConfigFile.GetGlobalValue("ClearLogThanNDays");
        //    Stored.ConfigFile.SetGlobal("Updated", DateTime.Now);
        //}


        class TestCredential : k.Models.Credential
        {
            public TestCredential() : base()
            {
                User = "user";
                this.SetPassword("passwd");

                SetParameter("Password", "tango");
            }

            public TestCredential(string id) : base()
            {
                Load(id);
            }
        }
    }
}
