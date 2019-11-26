using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
        public k.Lists.ParametersList Parameters { get; set; } = new Lists.ParametersList();
        public  List<string> Roles { get; set; } = new List<string>();
        public Credential(E.Projects project) 
        {
            path = Shell.Directory.AppDataFolder(project, LOG.ToLower());
        }

        protected void Load<T>(string id) where T : Credential
        {
            var file = Shell.File.Find(path, $"{id}.{LOG.ToLower()}").FirstOrDefault();
            var json = Shell.File.Load(file);

            if (!R.DebugMode)
                json = Security.Decrypt(json, null);

            var foo = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            foreach (var p in k.Reflection.GetProperties(foo))
                k.Reflection.SetValue(this, p.Name, p.GetValue(foo));
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

        public virtual object GetParameter(string key, string msgerror)
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

    }
}
