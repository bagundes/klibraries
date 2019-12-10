using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace k.Models
{
    public abstract class Credential : k.KModel
    {
        protected string LOG => this.GetType().Name;
        public readonly string path;


        public string Host { get; set; }
        public string Schema { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public DateTime DueDate { get; set; }
        public k.Lists.MyList Parameters { get; set; } = new k.Lists.MyList();
        public  List<string> Roles { get; set; } = new List<string>();
        public Credential(G.Projects project) 
        {
            path = Shell.Directory.AppDataFolder(project, LOG.ToLower());
        }

        protected void Load(string id)
        {
            var file = Shell.File.Find(path, $"{id}.{LOG.ToLower()}").FirstOrDefault();
            if (file == null)
            {
                k.Diagnostic.Error(LOG, R.Project, "Cannot possible to load {0} id in {1}", id, path);
                throw new KException(LOG, E.Message.CredentialId_0);
            }
            
            var json = Shell.File.Load(file);

            if (!R.DebugMode)
                json = Security.Decrypt(json, null);

            var foo = Newtonsoft.Json.JsonConvert.DeserializeObject(json, this.GetType());

            foreach (var p in k.Reflection.GetProperties(foo))
                k.Reflection.SetValue(this, p.Name, p.GetValue(foo));

            if (DueDate != (new DateTime()) && DueDate < DateTime.Now)
            {
                k.Shell.File.Delete(file);
                throw new KException(LOG, E.Message.CredentialExpired_0);
            }
        }

        /// <summary>
        /// Save credential and return credential ID
        /// </summary>
        /// <returns>ID</returns>
        public virtual string Save()
        {

            var json = ToJson();
            var id = Security.Hash(json);
            var name = $"{id}.{LOG.ToLower()}";
            
            Shell.File.Write(json, name, path);

            return id;
        }

        #region Parameters
        public virtual void SetParameter(string key, object value)
        {
            Parameters.Set(key, value);
        }

        public virtual Dynamic GetParameter(string key, string msgerror = null)
        {
            Dynamic val;
            if (!Parameters.Get(key, out val) && !String.IsNullOrEmpty(msgerror))
                throw new KeyNotFoundException(msgerror);
            else
                return val;

        }
        #endregion

        #region Roles
        public virtual void SetRole(string role)
        {
            if (!Roles.Where(t => t.Equals(role, StringComparison.InvariantCultureIgnoreCase)).Any())
                Roles.Add(role);
        }

        public virtual bool ExistsRole(string role)
        {
            return Roles.Where(t => t.Equals(role, StringComparison.InvariantCultureIgnoreCase)).Any();
        }
        #endregion

        /// <summary>
        /// Set the encrypted password in the password field.
        /// </summary>
        /// <param name="epasswd"></param>
        public void SetEncryptPassword(string epasswd)
        {
            Password = k.Security.Decrypt(epasswd, User);
        }

        public string GetEncryptPassword()
        {
            return k.Security.Encrypt(Password, User);
        }

        /// <summary>
        /// Details about the data information
        /// </summary>
        /// <returns></returns>
        public string DetailsFull()
        {
            var cred = (Credential)Clone();
            if (!R.DebugMode)
            {
                var passwd = String.IsNullOrEmpty(cred.Password) ? "null" : cred.Password.Substring(0, 2) + new string('*', cred.Password.Length - 2);
                cred.Password = passwd;

                foreach(var key in cred.Parameters.Keys)
                {
                    if(key.Contains("pass"))
                    {
                        var pass = cred.GetParameter(key).ToString();
                        var passwd1 = String.IsNullOrEmpty(pass) ? "null" : pass.Substring(0, 2) + new string('*', pass.Length - 2);
                        cred.SetParameter(key, passwd1);
                    }
                }
            }


            return JsonConvert.SerializeObject(cred,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                });

        }

        /// <summary>
        /// Return information about the credential
        /// </summary>
        /// <returns></returns>
        public string DetailsSimple()
        {
            return $"{User}@{Host}.{Schema}";
        }
    }

    internal static class CredentialControl
    {
        internal static void ClearSchedule()
        {
            var wait = R.DebugMode ? TimeSpan.FromDays(1) : TimeSpan.FromHours(1);

            do
            {
                ClearCredentials();

                Thread.Sleep((int)wait.TotalMilliseconds);
            } while (true);
        }

        public static void ClearCredentials()
        {

            var path = Shell.Directory.GetSpecialFolder(Shell.Directory.SpecialFolder.AppData);

            var files = Shell.File.Find(path.FullName, "*.credential", System.IO.SearchOption.AllDirectories, DateTime.Now.AddDays(-30));
            
            Shell.File.Delete(files);           
        }
    }
}
