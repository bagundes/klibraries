using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace k
{
    public class Init : IInit
    {
        public void Init10_Dependencies()
        {
            k.Diagnostic.Debug(this, null, "Debug mode is enabled");
            Content.Language.en_GB.Culture = new CultureInfo("en-GB");

        }


        public void Init20_Config()
        {
            // Loading config file
            
        }

        public void Init50_Threads()
        {
            Threads.ControlHelper.Add(Helpers.DiagnosticHelper.Clear, "Clear old log files", 60);
            Threads.ControlHelper.Add(Helpers.CredentialHelper.Clear, "Clear old credentials files", 60);
            Threads.ControlHelper.Start();
        }

        public void Init60_End()
        {
            Threads.ControlHelper.End();
        }
    }
}
