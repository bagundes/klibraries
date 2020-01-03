using k.sap.ui.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class OptionsHelper
    {
        private static string LOG => typeof(OptionsHelper).FullName;
        public static Dynamic GetValueOf(ref SAPbouiCOM.Form oForm,params string[] items)
        {
            try
            {
                foreach (var item in items)
                {
                    var oItem = oForm.Items.Item(item);
                    var v = FormHelper.GetValue(oItem, false);
                    if (!v.IsNullOrEmpty())
                        return v;
                }
            }catch(Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
            }

            return null;
        }
    }
}
