using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public class R
    {


#if DEBUG
        public static bool DebugMode => true;
#else
        public bool DebugMode => k.Content.ConfGlobal.DebugMode == "1";
#endif
        public class Security
        {
            public static string MasterKey => k.Content.ConfGlobal.MasterKey;
        }
    }
}
