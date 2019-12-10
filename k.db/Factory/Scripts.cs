using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public static class Scripts
    {
        private static string LOG => typeof(Scripts).Name;        

        public static string FormatQuery(string sql, params object[] values)
        {
            var clientType = Factory.Connection.GetClient(R.CredID).ClientType;
            Namespace(ref sql);
            SpecificLine(ref sql, clientType);

            sql = FormatValues(sql, values);
            
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
                    case TypeCode.String:
                    case TypeCode.Int32:
                        valueFormated = value.ToString(); break;
                    case TypeCode.DateTime:
                        valueFormated = ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss"); break;
                    case TypeCode.Boolean:
                            valueFormated = ((bool)value) ? "Y" : "N";
                            break;
                    default:
                        k.Diagnostic.Error(LOG, R.Project, "Error to format {0} to {1} in position {2} in the {3} query", value, type.ToString(), i, sql);
                        throw new KDBException(LOG, E.Message.InvalidFormat_2, value, type.ToString());
                }

                sql = sql.Replace($"{{{i}}}", $"'{valueFormated}'");
            }

            return sql;
        }

        /// <summary>
        /// Replace !!_ tag to R.Namespace
        /// </summary>
        /// <param name="sql">Query or values to change</param>
        /// <returns></returns>
        public static bool Namespace(ref string sql)
        {
            var nameSpace = E.DataBase.Tags.Namespace;

            var res = sql.Contains(nameSpace);
            sql = sql.Replace(nameSpace, $"{R.Namespace}_");
            return res;
        }

        private static string GetTag(string tagname, string sql)
        {
            if (!sql.Contains(tagname))
                return null;

            var index = sql.IndexOf(tagname);

            return null;
        }

        public static bool SpecificLine(ref string sql, E.DataBase.TypeOfClient typeOfClient)
        {
            var specificLine = GetTag(E.DataBase.Tags.SpecificLineHeader, sql) ?? E.DataBase.Tags.SpecificLine;
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
