using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class EditTextHelper
    {
        private static string LOG => typeof(EditTextHelper).FullName;

        public static void SetValue(ref SAPbouiCOM.Form oForm, ref SAPbouiCOM.EditText editText, string value)
        {
            if(editText.DataBind.DataBound)
            {
                FormHelper.SetUDSValue(ref oForm, editText.DataBind.Alias, value);
            } else
            {
                editText.Value = value;
            }
        }
    }
}
