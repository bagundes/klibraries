﻿using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    public class Init : IInit
    {
        private string control = DateTime.Now.ToString("ffffff");

        public void Init10_Dependency()
        {
            k.StartInit.Starting(new k.Init());
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
