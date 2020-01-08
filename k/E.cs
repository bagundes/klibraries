using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace k
{
    public class G
    {
        public enum Projects
        {
            UnitTests = 9999,
            KCore = 1000,
            KDB = 1001,
            KWin32 = 1002,
            
            KSAP = 2000,
            KDI = 2001,
            KDIServer = 2002,
            KUI = 2002,

            AddonBase = 5000,
            GoodsReceiptScheduler = 5001,

            Others = 9998,
        }

        public class DataBase
        {
            public class SAPTables
            {
                private static string LOG => typeof(SAPTables).FullName;

                public enum TableType
                {
                    System = -1,
                    bott_NoObject = 0,
                    bott_MasterData = 1,
                    bott_MasterDataLines = 2,
                    bott_Document = 3,
                    bott_DocumentLines = 4,
                    bott_NoObjectAutoIncrement = 5,

                }

                public enum ColumnsRuler
                {
                    Unique,
                    PrimaryKey,
                    None,
                }
                public enum ColumnsType
                {
                    AlphaNumeric_Regular,
                    AlphaNumeric_Address,
                    AlphaNumeric_Phone,
                    AlphaNumeric_Text,

                    Numeric,
                    DateTime_Date,
                    DateTime_Hour,
                    UnitsAndTotals_Rate,
                    UnitsAndTotals_Amount,
                    UnitsAndTotals_Price,
                    UnitsAndTotals_Quantity,
                    UnitsAndTotals_Percents,
                    UnitsAndTotals_Measure,

                    General_Link,
                    General_Image,

                    Special_MD5,
                    Special_YesOrNo,
                    Special_StatusQueue,
                }
                public static string GetTypeID(ColumnsType col, out int boFieldTypes, out int boFldSubTypes)
                {
                    boFldSubTypes = 0; //None

                    switch (col)
                    {
                        case SAPTables.ColumnsType.Special_MD5:
                        case SAPTables.ColumnsType.Special_YesOrNo:
                        case SAPTables.ColumnsType.Special_StatusQueue:
                        case ColumnsType.AlphaNumeric_Regular:
                            boFieldTypes = 0;
                            return "A";
                        case ColumnsType.AlphaNumeric_Address:
                            boFieldTypes = 0;
                            boFldSubTypes = 63;
                            return "A?";
                        case ColumnsType.AlphaNumeric_Phone:
                            boFieldTypes = 0;
                            boFldSubTypes = 35;
                            return "A#";
                        case ColumnsType.AlphaNumeric_Text:
                            boFieldTypes = 1;
                            return "M";
                        case ColumnsType.Numeric:
                            boFieldTypes = 2;
                            return "N";
                        case ColumnsType.DateTime_Date:
                            boFieldTypes = 3;
                            return "D";
                        case ColumnsType.DateTime_Hour:
                            boFieldTypes = 3;
                            boFldSubTypes = 84;
                            return "NT";

                        //case ColumnsType.UnitsAndTotals_Rate: return "BR";
                        //case ColumnsType.UnitsAndTotals_Amount: return "BS";
                        //case ColumnsType.UnitsAndTotals_Price: return "BP";
                        //case ColumnsType.UnitsAndTotals_Quantity: return "BQ";
                        //case ColumnsType.UnitsAndTotals_Percents: return "B%";
                        //case ColumnsType.UnitsAndTotals_Measure: return "BM";
                        //case ColumnsType.General_Link: return "MB";
                        //case ColumnsType.General_Image: return "AI";
                        //case ColumnsType.Special_MD5: return "A#";
                        //case ColumnsType.Special_YesOrNo: return "A";
                        //case ColumnsType.Special_StatusQueue: return "A";
                        default: throw new NotImplementedException(col.ToString());


                    }
                }

            }

            public enum TypeOfClient
            {
                MSQL,
                Hana,
            }

            public class Tags
            {
                public static string DefPrefixUserField => "U_";
                public static string Namespace => "!!";
                public static string NameSpaceHeader => "--NAMESPACE:";

                public static string SpecificLine = "--#";
                public static string SpecificLineHeader = "--SPECIFICLINE:";


            }
        }
    }

    internal class E
    {
        public class Extensions
        {
            public const string CREDENTIAL = "credential";
        }

        public enum Message
        {
            TestMessage = -1,
            GenerelError_1 = 0,
            CredentialId_0 = 1,
            CredentialExpired_0 = 2,
            BucketSizeLimite_1 = 3,
            BucketCapacityLimite_1 = 4,
            CredPasswordError_0 = 5
        }
    }
}
