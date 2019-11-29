using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Clients
{
    /// <summary>
    /// SQL Server credentials
    /// </summary>
    public class SqlCredential : DBCredential
    {
        public SqlCredential() : base(k.db.R.Project)
        {
        }


        public SqlCredential(string server, string schema, string user, string passwd, int port = 1433) : base(k.G.Projects.KDB)
        {
            base.Host = server;
            base.Schema = schema;
            base.User = user;
            base.Password = passwd;
            base.Parameters.Set("port", port);
        }

        public override string ToString()
        {
            return String.Format("server={0};initial catalog={1};user id={2};password={3};", DbServer, Schema, User, Password);
        }

        /// <summary>
        /// Change the database in string connection.
        /// </summary>
        /// <param name="schema">Database name</param>
        /// <returns></returns>
        public string ToString(string schema)
        {
            return String.Format("server={0};initial catalog={1};user id={2};password={3};", DbServer, schema, User, Password);
        }
    }
}
