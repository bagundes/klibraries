using k.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public static class Connection
    {
        public static string LOG => typeof(Connection).FullName;

        private static Type _client;// { get; internal set;}

        /// <summary>
        /// Get the client database connection.
        /// </summary>
        /// <param name="dbase">Database name</param>
        /// <returns>Client connection</returns>
        public static IFactory GetClient(string id)
        {
            id = id ?? R.CredID;

            if (_client == null)
                throw new KDBException(LOG, E.Message.ClientIsNotDefined_0);

            var client = (IFactory)Activator.CreateInstance(_client, new object[] { });
            client.Connect(id);

            return client;
        }

        public static void SetClient<T>(T client) where T : k.Interfaces.IFactory
        {
            _client = client.GetType();
            R.CredID = client.Id;
            k.Diagnostic.Debug(LOG, null, "Defined as {0} default", _client.Name);
        }
    }
}
