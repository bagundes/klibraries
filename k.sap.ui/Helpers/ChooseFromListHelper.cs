using k.Lists;
using k.sap.ui.Forms;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPForm = SAPbouiCOM.Form;

namespace k.sap.ui.Helpers
{
    public static class ChooseFromListHelper
    {
        private static SelectFromListForm SFListForm;

        /// <summary>
        /// Create a new Choose From List
        /// </summary>
        /// <param name="objType">Object Type https://blogs.sap.com/2017/04/27/list-of-object-types/ </param>
        /// <param name="uniqueID"></param>
        /// <param name="multiSelection"></param>
        /// <returns></returns>
        public static ChooseFromList Create(ref SAPForm oForm,  string objType, string uniqueID, bool multiSelection = false)
        {
            var oCFLs = oForm.ChooseFromLists;
            var oCFLCreationParams = UI.Conn.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams) as ChooseFromListCreationParams;

            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = objType;
            oCFLCreationParams.UniqueID = uniqueID;

            try
            {
                return oCFLs.Add(oCFLCreationParams);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"{uniqueID} choose from list exists.", ex);
            }
        }

        /// <summary>
        /// Create equal conditions
        /// </summary>
        /// <param name="oCFL">Choose from list in the form</param>
        /// <param name="bucket">Result query</param>
        public static void CreateConditionsEquals(ref ChooseFromList oCFL, Bucket bucket)
        {
            var oCons = oCFL.GetConditions();
            // Clear conditions
            oCons = new SAPbouiCOM.Conditions();


            for(int i = 0; i < bucket.CountRows; i++)
            {
                foreach (var field in bucket[i])
                {
                    var oCon = oCons.Add();

                    oCon.Alias = field.Key;
                    oCon.Operation = BoConditionOperation.co_EQUAL;
                    oCon.CondVal = field.Value;

                    if(bucket.CountRows > (i + 1))
                        oCon.Relationship = BoConditionRelationship.cr_OR;

                    
                }
            }

            oCFL.SetConditions(oCons);
        }

        public static Bucket GetValues(ref SBOItemEventArg pVal, params dynamic[] columns)
        {
            var oCFLEvent = pVal as SAPbouiCOM.ISBOChooseFromListEventArg;
            var oDataTable = oCFLEvent.SelectedObjects;
            

            if (oDataTable == null)
                return null;

            var bucket = new Bucket(oDataTable.Rows.Count);
            for (int i = 0; i < oDataTable.Rows.Count; i++)
                foreach (var column in columns)
                    bucket.Add(column, oDataTable.GetValue(column, i), i);

            return bucket;
        }

        public static void SelectFromList(Action<Bucket> call, string title, bool multi, string sql, params object[] values)
        {
            SFListForm = new Forms.SelectFromListForm(call, title, multi);
            SFListForm.Load(sql, values);
        }
    }
}
