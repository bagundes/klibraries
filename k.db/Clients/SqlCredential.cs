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
        public SqlCredential() : base()
        {
        }

        public SqlCredential(string id) : base()
        {
            base.Load(id);
        }
        public SqlCredential(string server, string schema, string user, string passwd, int port = 1433) : base()
        {
            base.Host = server;
            base.Schema = schema;
            base.User = user;
            base.EPassword = passwd;
            base.Parameters.Set("port", port);
        }

        public override string ToString()
        {
            return String.Format("server={0};initial catalog={1};user id={2};password={3};", DbServer, Schema, User, EPassword);
        }

        /// <summary>
        /// Change the database in string connection.
        /// </summary>
        /// <param name="schema">Database name</param>
        /// <returns></returns>
        public string ToString(string schema)
        {
            return String.Format("server={0};initial catalog={1};user id={2};password={3};", DbServer, schema, User, EPassword);
        }
    }
}
