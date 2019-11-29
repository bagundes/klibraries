using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace k
{
    public class Init : IInit
    {
        private string control = DateTime.Now.ToString("ffffff");
        private Thread logFileThread;

        public void Init10_Dependency()
        {
            k.Diagnostic.Debug(control, R.Project, "Debug mode is enabled");
            Content.Language.en_GB.Culture = new CultureInfo("en-GB");
        }


        public void Init20_Config()
        {
            
        }

        public void Init50_Threads()
        {
            // Log file monitor
            logFileThread = new Thread(new ThreadStart(Diagnostic.ClearLogSchedule));
            logFileThread.Start();

        }

        public void Init60_End()
        {
            logFileThread.Abort();
        }
    }
}
