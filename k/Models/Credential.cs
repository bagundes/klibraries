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
        protected string LOG => this.GetType().FullName;

        public string Host { get; set; }
        public string Schema { get; set; }
        public string User { get; set; }

        /// <summary>
        /// Encrypted password
        /// </summary>
        public string EPassword { get; set; }
        public DateTime DueDate { get; set; }
        public k.Lists.Bucket Parameters { get; set; } = new k.Lists.Bucket();
        public  List<string> Roles { get; set; } = new List<string>();
        public Credential() {}

        protected void Load(string id)
        {
            var file = Shell.File.Find(Helpers.CredentialHelper.Path, $"{id}.{LOG.ToLower()}").FirstOrDefault();
            if (file == null)
            {
                k.Diagnostic.Error(LOG, null, "Cannot possible to load {0} id in {1}", id, Helpers.CredentialHelper.Path);
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
            
            Shell.File.Write(json, name, Helpers.CredentialHelper.Path);

            return id;
        }

        #region Parameters
        public virtual void SetParameter(string key, object value)
        {
            if (key.ToLower().Contains("passw"))
                value = Helpers.CredentialHelper.ConvertToEPassword(value.ToString(), User);

            Parameters.Set(key, value);
        }



        public virtual Dynamic GetParameter(string key, string msgerror = null)
        {
            Dynamic val;
            if (!Parameters.Get(key, out val) && !String.IsNullOrEmpty(msgerror))
                throw new KeyNotFoundException(msgerror);
            else
                if (key.Contains("passw"))
                    return Helpers.CredentialHelper.ConvertToPassword(val.ToString(), User);
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
        /// <param name="passowrd"></param>
        public void SetPassword(string passowrd)
        {
            EPassword = Helpers.CredentialHelper.ConvertToEPassword(passowrd, User);
        }

        public string GetPassword()
        {
            try
            {
                return k.Security.Decrypt(EPassword, User);
            }catch(Exception ex)
            {
                var track = Diagnostic.TrackObject(this);
                Diagnostic.Debug(this, track, $"The invalid epassword, may be you added password before add user");
                Diagnostic.Error(LOG, ex);
                throw new KException(LOG, E.Message.CredPasswordError_0);
                throw ex;
            }
        }

        public bool IsValidPassword(string password)
        {
            try
            {
                return GetPassword() == password;
            }catch
            {
                return false;
            }
        }

        /// <summary>
        /// Details about the data information
        /// </summary>
        /// <returns>Complete information about this credential</returns>
        public string Info1()
        {
            var cred = (Credential)Clone();
            if (!R.DebugMode)
            {
                var passwd = String.IsNullOrEmpty(cred.EPassword) ? "null" : cred.EPassword.Substring(0, 2) + new string('*', cred.EPassword.Length - 2);
                cred.EPassword = passwd;

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
        /// Details about the data information
        /// </summary>
        /// <returns>Basic information about this credential</returns>
        public string Info2()
        {
            return $"{User}@{Host}.{Schema}";
        }
    }
}
