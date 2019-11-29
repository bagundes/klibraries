using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Factory
{
    public class ResultSet
    {
        public Dynamic Get(string sql, params object[] values)
        {

            return Dynamic.Empty;
        }

        public k.Lists.GenericList GetLine(string sql, params object[] values)
        {
            return new Lists.GenericList();
        }
    }
}
