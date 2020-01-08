using k.Attributes;
using k.sap.ui.Helpers;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Forms.SystemHelpers.Crm
{
    public static class ActivitySystemHelper
    {
        private static string LOG => typeof(ActivitySystemHelper).FullName;
        public static class UniqueID
        {
            public const string Modules_MenuUID = "43684";
            public const string FormTypeEx = "651";
            public const string ObjectType = "33";

            public const string TXTClgCodeUID = "5";
            public const string TXTPreviousActivityIUD = "148";
            public const string LABPreviousActivityIUD = "149";
            public const string CMBDocumentTypeUID = "42";
        }

        /// <summary>
        /// OCLG.ClgCode
        /// </summary>
        /// <param name="oForm"></param>
        /// <returns></returns>
        public static Dynamic GetObjectKey(ref SAPbouiCOM.Form oForm)
        {
            return FormHelper.GetValue(ref oForm, UniqueID.TXTClgCodeUID, false);
        }

        /// <summary>
        /// OCLG.DocType
        /// </summary>
        /// <param name="oForm"></param>
        /// <returns></returns>
        public static Dynamic GetLinkedDocumentType(ref SAPbouiCOM.Form oForm)
        {
            return FormHelper.GetValue(ref oForm, UniqueID.CMBDocumentTypeUID, false);
        }
    }
}
