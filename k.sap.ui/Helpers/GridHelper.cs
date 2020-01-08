using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class GridHelper
    {
        private static string LOG => typeof(MatrixHelper).FullName;

        public static void ReadOnly(ref SAPbouiCOM.Grid grid)
        {
            for(int i = 0; i < grid.Columns.Count; i++)
                grid.Columns.Item(i).Editable = false;
        }

        public static void Linkto(ref SAPbouiCOM.Grid grid, BoLinkedObject boLinkedObject)
        {
            Linkto(ref grid, ((int)boLinkedObject).ToString());
        }
        public static void Linkto(ref SAPbouiCOM.Grid grid, string boLinkedObject)
        {
            var oColumn = grid.Columns.Item(0) as EditTextColumn;
            oColumn.Type = BoGridColumnType.gct_EditText;
            oColumn.LinkedObjectType = boLinkedObject;
        }

    }
}
