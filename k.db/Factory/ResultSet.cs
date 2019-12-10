using k.Lists;
using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public class ResultSet
    {


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
        /// <returns>Generic list</returns>
        public static MyList GetRow(string id, string sql, params object[] values)
        {
            using (var client = Connection.GetClient(id))
            {
                if (client.DoQuery(sql, values))
                    return client.Fields();
                else
                    return null;
            }
        }

        public static MyList GetLines(string id, string sql, params object[] values)
        {
            var list = new MyList();

            using (var client = Connection.GetClient(id))
            {
                if (client.DoQuery(sql, values))
                {
                    while (client.Next())
                        list.Set(client.Fields(), client.Position);
                    return list;
                }
                else
                    return null;                
            }
        }
    }
}
