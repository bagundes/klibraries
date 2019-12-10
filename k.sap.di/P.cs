using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.di
{
    internal class P
    {
        public static bool CreateTableAndFields => k.Stored.ConfigFile.GetGlobalValue("CreateTablesAndFields").ToBool();
    }
}
