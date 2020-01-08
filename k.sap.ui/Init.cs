using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace k.sap.ui
{
    public class Init : IInit
    {
        public void Init10_Dependencies()
        {
            k.StartInit.Register(new k.sap.Init());
        }


        public void Init20_Config()
        {
            //k.sap.UI.Conn.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(Forms.SelectFromListForm.UI_FormDataEvent);
        }

        public void Init50_Threads()
        {

        }

        public void Init60_End()
        {
        }
    }
}
