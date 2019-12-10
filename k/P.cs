using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    internal class P
    {
        public static int ClearLogThanNDays => k.Stored.ConfigFile.GetGlobalValue("ClearLogThanNDays").ToInt();

        public static string MasterKey => k.Stored.ConfigFile.GetGlobalValue("MasterKey");
    }
}
