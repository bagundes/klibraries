using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    public class Init : IInit
    {
        public void Init10_Dependencies()
        {
            k.StartInit.Register(new k.Init());
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
