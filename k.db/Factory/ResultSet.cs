using k.Lists;
using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public class ResultSet
    {
        /// <summary>
        /// Get specific field
        /// </summary>
        /// <param name="id">CredID</param>
        /// <param name="sql">query</param>
        /// <param name="values">values</param>
        /// <returns></returns>
        public static Dynamic Get(string id, string sql, params object[] values)
        {
            using(var client = Connection.GetClient(id))
            {
                if (client.DoQuery(sql, values))
                    return client.Field(0);
                else
                    return Dynamic.Empty;

            }
        }

        /// <summary>
        /// Return first result row.
        /// </summary>
        /// <param name="id">Credential ID</param>
        /// <param name="sql">Query</param>
        /// <param name="values">Values of query</param>
        /// <returns>Generic list or null</returns>
        public static Bucket GetRow(string id, string sql, params object[] values)
        {
            using (var client = Connection.GetClient(id))
            {
                if (client.DoQuery(sql, values))
                    return client.Fields();
                else
                    return null; 
            }
        }

        public static Bucket GetLines(string id, string sql, params object[] values)
        {
            var bucket = new Bucket();

            using (var client = Connection.GetClient(id))
            {
                if (client.DoQuery(sql, values))
                {
                    while (client.Next())
                        bucket.Set(client.Fields(), client.Position);
                    return bucket;
                }
                else
                    return null;                
            }
        }
    }
}
