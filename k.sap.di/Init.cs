using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace k.sap.di
{
    public class Init : IInit
    {
        private string control = DateTime.Now.ToString("ffffff");

        public void Init10_Dependency()
        {
            k.StartInit.Starting(new k.Init());
            k.StartInit.Starting(new k.db.Init());
            k.StartInit.Starting(new k.sap.Init());
        }


        public void Init20_Config()
        {
            
        }

        public void Init50_Threads()
        {

        }

        public void Init60_End()
        {
        }
    }
}
