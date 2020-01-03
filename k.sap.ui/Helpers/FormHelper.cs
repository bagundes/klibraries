using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class FormHelper
    {
        private static string LOG => typeof(FormHelper).FullName;

        public static SAPbouiCOM.Form Load(string srf_xml, bool visible)
        {            
            var oFormCreationParams =  k.sap.UI.Conn.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;
            oFormCreationParams.XmlData = srf_xml;

            if (oFormCreationParams.UniqueID == String.Empty)
                oFormCreationParams.UniqueID = $"{k.R.Namespace}_{k.Security.RandomChars(5,false)}";

            var oForm = k.sap.UI.Conn.Forms.AddEx(oFormCreationParams);
            oForm.Visible = visible;

            return oForm;
        }

        /// <summary>
        /// Get value of control componete
        /// </summary>
        /// <param name="control">Object control</param>
        /// <param name="require">Require field (it is not null or empty)</param>
        /// <param name="nullif"></param>
        /// <returns></returns>
        public static Dynamic GetValue(SAPbouiCOM.Item oItem, bool require, string nullif = null)
        {

            var value = String.Empty;
            try
            {
                switch (oItem.Type)
                {
                    case SAPbouiCOM.BoFormItemTypes.it_EDIT:
                    case SAPbouiCOM.BoFormItemTypes.it_EXTEDIT:
                        value = ((SAPbouiCOM.EditText)oItem.Specific).Value; break;
                    case SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX:
                        value = ((SAPbouiCOM.ComboBox)oItem.Specific).Selected.Value; break;
                    case SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX:
                        value = ((SAPbouiCOM.CheckBox)oItem.Specific).Checked.ToString(); break;
                    case SAPbouiCOM.BoFormItemTypes.it_OPTION_BUTTON:
                        var opt = (SAPbouiCOM.OptionBtn)oItem.Specific;
                        if (opt.Selected)
                            value = opt.ValOn; break;
                    default:
                        throw new NotImplementedException($"UniqueID:{oItem.UniqueID}, Type:{oItem.Type}");
                }

                if (require)
                {
                    if ((string.IsNullOrEmpty(value) 
                        || (nullif != null 
                            && nullif.Equals(value, StringComparison.InvariantCultureIgnoreCase))))
                        throw new KUIException(LOG, oItem);
                }

                return new Dynamic(value);
            }

            finally

            {
                // oItem.Enabled = enabled;
            }

        }

        public static Dynamic GetValue(SAPbouiCOM.Cell oCell, bool require, string nullif = null)
        {
            var oItem = (oCell.Specific as SAPbouiCOM.EditText).Item;
            return GetValue(oItem, require, nullif);
        }
        public static void SetUDSValue(ref SAPbouiCOM.Form oForm, string uniqueID, string value)
        {
            oForm.DataSources.UserDataSources.Item(uniqueID).ValueEx = value;
        }

        /// <summary>
        /// Get srf embbeded file
        /// </summary>
        /// <param name="name">File or class name</param>
        /// <param name="Assembly">Assembly to find the file</param>
        /// <returns></returns>
        public static string GetSRFFile(string name, System.Reflection.Assembly Assembly)
        {
            name = name.Replace("Form", "").ToLower();

            var res = Assembly.GetManifestResourceNames()
            .Where(t => t.Contains($"Content.forms.{name}.srf"))
            .FirstOrDefault();

            if (res == null)
                throw new Exception("The srf file is not saved in content\\forms or it is not embbeded");

            using (var stream = Assembly.GetManifestResourceStream(res))
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
