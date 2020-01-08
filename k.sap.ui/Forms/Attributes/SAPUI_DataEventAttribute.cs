using SAPbouiCOM;
using System;

namespace k.sap.ui.Forms.Attributes
{
    public class SAPUI_DataEventAttribute : Attribute
    {
        private BoEventTypes boEventTypes;

        public SAPUI_DataEventAttribute(BoEventTypes boEventTypes)
        {
            this.boEventTypes = boEventTypes;
        }
    }
}