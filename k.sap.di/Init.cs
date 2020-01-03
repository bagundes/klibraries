using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace k.sap.di
{
    public class Init : IInit
    {
        public void Init10_Dependencies()
        {
            k.StartInit.Register(new k.sap.Init());
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
