using System;

namespace k.sap.ui.Forms.Attributes
{
    public class SAPUI_ActionAttribute : Attribute
    {

        public readonly string TypeEx;
        public SAPUI_ActionAttribute(string formTypeEx)
        {
            TypeEx = formTypeEx;
        }
    }
}