using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public class Init
    {
        private string control = DateTime.Now.ToString("ffffff");

        public void Init10_Before()
        {
            if (R.DebugMode)
                k.Diagnostic.Debug(control, E.Projects.KCore, "Debug mode is enabled");
        }

        public void Init90_After()
        {

        }
    }
}
