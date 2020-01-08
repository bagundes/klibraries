using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public static class Scripts
    {
        private static string LOG => typeof(Scripts).FullName;        

        public static string FormatQuery(string sql, params object[] values)
        {
            sql = FormatValues(sql, values);
            var clientType = Factory.Connection.GetClient(R.CredID).ClientType;
            Namespace(ref sql);
            SpecificLine(ref sql, clientType);

            return sql;
        }

        public static string FormatValues(string sql, params object[] values)
        {
            for(int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                string valueFormated;

                TypeCode type = Type.GetTypeCode(value.GetType());

                switch (type)
                {
                    case TypeCode.Object:
                    case TypeCode.String:
                    case TypeCode.Int32:
                        valueFormated = value.ToString(); break;
                    case TypeCode.DateTime:
                        valueFormated = ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss"); break;
                    case TypeCode.Boolean:
                            valueFormated = ((bool)value) ? "Y" : "N";
                            break;
                    default:
                        k.Diagnostic.Error(LOG, null, "Error to format {0} to {1} in position {2} in the {3} query", value, type.ToString(), i, sql);
                        throw new KDBException(LOG, E.Message.InvalidFormat_2, value, type.ToString());
                }

                sql = sql.Replace($"{{{i}}}", $"'{valueFormated}'");
            }

            return sql;
        }

        /// <summary>
        /// Replace !! tag to R.Namespace
        /// </summary>
        /// <param name="sql">Query or values to change</param>
        /// <returns></returns>
        public static bool Namespace(ref string sql)
        {
            var nameSpace = G.DataBase.Tags.Namespace;

            var res = sql.Contains(nameSpace);
            sql = sql.Replace(nameSpace, $"{R.Namespace}");
            return res;
        }

        public static string Namespace(string val)
        {
            var nameSpace = G.DataBase.Tags.Namespace;

            var res = val.Contains(nameSpace);
            val = val.Replace(nameSpace, $"{R.Namespace}");
            return val;
        }

        private static string GetTag(string tagname, string sql)
        {
            if (!sql.Contains(tagname))
                return null;

            var index = sql.IndexOf(tagname);

            return null;
        }

        public static bool SpecificLine(ref string sql, G.DataBase.TypeOfClient typeOfClient)
        {
            var specificLine = GetTag(G.DataBase.Tags.SpecificLineHeader, sql) ?? G.DataBase.Tags.SpecificLine;
            var dbType = typeOfClient.ToString().ToLower();
            
            if (!sql.Contains(specificLine))
                return false;
            else
            {
                sql = sql.Replace($"{specificLine}{dbType}", null);
                return true;
            }
        }
    }
}
