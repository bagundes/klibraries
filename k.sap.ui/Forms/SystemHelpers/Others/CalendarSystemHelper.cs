using k.Attributes;
using k.sap.ui.Helpers;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Forms.SystemHelpers.Others
{
    public static class CalendarSystemHelper
    {
        private static string LOG => typeof(CalendarSystemHelper).FullName;
        public static class UniqueID
        {
            public const string MenuUID = "10770";
            public const string MtxCalendarUID = "10";
            public const string TxtDateUID = "99";
            public const string FormTypeEx = "40005";
            public const string optMonthUID = "65";
            public const string optWeekUID = "56";
            public const string optWorkingWeekUID = "55";
            public const string optDayUID = "54";
        }

        public enum TypeOfCalendar
        {
            [Description("Month")]
            [Alias("M")]
            Month,
            [Description("Week")]
            [Alias("W")]
            Week,
            [Description("Working Week")]
            [Alias("R")]
            WorkingWeek,
            [Description("Day")]
            [Alias("D")]
            Day,
            None
        }

        public static void ReloadCalendar(ref SAPbouiCOM.Form oForm)
        {
            //var foo = DI.Conn.InTransaction;
            //TODO: Static values
            var butGo = oForm.Items.Item("100");
            butGo.Click();
            //var matrix = oForm.Items.Item("134").Specific as Matrix;
            //MatrixHelper.Refresh(ref matrix);
            

            //oForm.Refresh();

            //oForm.Close();
            //System.Threading.Thread.Sleep(3000);
            //UI.Conn.Menus.Item(UniqueID.MenuUID).Activate();
            //System.Threading.Thread.Sleep(3000);
            //UI.Conn.Menus.Item(UniqueID.MenuUID).Activate();
            //TODO: Static values
            //var matrix = oForm.Items.Item(UniqueID.MtxCalendarUID).Specific as Matrix;
            //MatrixHelper.Refresh(ref matrix);
            //var foo = matrix.GetCellSpecific("0", 10) as Cell;
            //var bar = foo.Specific as EditText;
            ////oForm.Items.Item("100").Click();



        }

        public static TypeOfCalendar GetTypeOfCalendar(ref SAPbouiCOM.Form oForm)
        {
            //TODO: Method return fixed value
            if (oForm.TypeEx == UniqueID.FormTypeEx)
            {
                var val = OptionsHelper.GetValueOf(ref oForm, UniqueID.optMonthUID, UniqueID.optWeekUID, UniqueID.optWorkingWeekUID, UniqueID.optDayUID);
                return k.Dynamic.GetValueFromAlias<TypeOfCalendar>(val);

            } else
                return TypeOfCalendar.None;
        }

        public static DateTime GetDate(ref SAPbouiCOM.Form oForm)
        {
            return k.sap.ui.Helpers.FormHelper.GetValue(oForm.Items.Item(UniqueID.TxtDateUID), true).Sap_ToDate();
        }

        /// <summary>
        /// GetDatails of row.
        /// TODO: The Details return only calendar type day.
        /// </summary>
        /// <param name="oForm"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DateTime GetDetailsDate(ref SAPbouiCOM.Form oForm, int row)
        {
            var typeOfCalendar = GetTypeOfCalendar(ref oForm);
            var date = GetDate(ref oForm);
            if (typeOfCalendar == TypeOfCalendar.Day)
            {
                if (row == -1) row = 1;

                var time = TimeSpan.FromMinutes(30 * (row - 1));
                date = date.Add(time);
            }
            else
                k.Diagnostic.Error(LOG, null, $"GetDetailsDate is not implemented {typeOfCalendar} type");


            return date;
        }
    }
}
