using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class ComboboxHelper
    {
        public static string LOG => typeof(ComboboxHelper).FullName;
        public static void Load(ref SAPbouiCOM.ComboBox comboBox, k.Lists.Bucket bucket, bool clear = true)
        {
            if (clear)
                for (int i = comboBox.ValidValues.Count - 1; i >= 0; i--)
                    comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);

            foreach (var value in bucket)
                comboBox.ValidValues.Add(value.ElementAt(0).Value, value.ElementAt(1).Value);
        }

        public static void Select(ref SAPbouiCOM.ComboBox comboBox, object val, SAPbouiCOM.BoSearchKey boSearchKey)
        {
            if(val != null)
                comboBox.Select(val, boSearchKey);
        }
    }

}
