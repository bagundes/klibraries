using System;

namespace k.sap.ui.Forms.Attributes
{
    public class SAPUI_MenuEventAttribute : Attribute
    {
        public readonly bool Before;

        public SAPUI_MenuEventAttribute(bool before)
        {
            this.Before = before;
        }
    }
}