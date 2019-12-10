using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public static class Connection
    {
        public static string LOG => typeof(Connection).Name;

        private static Type _client;// { get; internal set;}

        /// <summary>
        /// Get the client database connection.
        /// </summary>
        /// <param name="dbase">Database name</param>
        /// <returns>Client connection</returns>
        internal static IFactory GetClient(string id)
        {
            if (_client == null)
                throw new KDBException(LOG, E.Message.ClientIsNotDefined_0);

            var client = (IFactory)Activator.CreateInstance(_client, new object[] { });
            client.Connect(id);

            return client;
        }

        public static void SetClient<T>(T client) where T : Factory.IFactory
        {
            _client = client.GetType();
            R.CredID = client.Id;
            k.Diagnostic.Debug(LOG, R.Project, "Defined as {0} default", _client.Name);
        }
    }
}
