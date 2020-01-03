using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Helpers
{
    public static class MatrixHelper
    {
        private static string LOG => typeof(MatrixHelper).FullName;

        /// <summary>
        /// Read value in the matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="require"></param>
        /// <param name="nullif"></param>
        /// <returns></returns>
        public static Dynamic GetValue(ref SAPbouiCOM.Matrix matrix, int row, dynamic col, bool require, string nullif = null)
        {
            if (row == -1)
                return Dynamic.Empty;

            try
            {



                var value = FormHelper.GetValue(matrix.Columns.Item(col).Cells.Item(row), require, nullif);

                return value;
            }
            catch(Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
                throw ex;
            }
        }

        public static void DelRowWithoutDSource(ref SAPbouiCOM.Matrix matrix)
        {
            try
            {
                var lineSelected = matrix.GetCellFocus().rowIndex;
                matrix.DeleteRow(lineSelected);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int? GetLineFocus(ref SAPbouiCOM.Matrix matrix)
        {
            try
            {
                var cell = matrix.GetCellFocus();
                if (cell != null)
                    return cell.rowIndex;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
