using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public static class Diagnostic
    {
        public static void Warning(string instance, E.Projects prj, string message, params object[] values)
        {

        }

        public static void Debug(string instance, E.Projects prj, string message, params object[] values)
        {
            if (R.DebugMode)
                return;
        }

        private static int Track(string message, params object[] values)
        {
            return 1;
        }

        private static void Register() { }

        private static void OnLine() { }
    }
}
