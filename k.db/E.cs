using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    public class E
    {
        public enum Message
        {
            GenerelError_1 = 0,
            ClientIsNotDefined_0 = 1,
            InvalidFormat_2 = 2,
        }

        public class DataBase
        {
            public enum TypeOfClient
            {
                MSQL,
                Hana,
            }

            public class Tags
            {
                public static string Namespace => "!!_";
                public static string NameSpaceHeader => "--NAMESPACE:";

                public static string SpecificLine = "--#";
                public static string SpecificLineHeader = "--SPECIFICLINE:";


            }
        }
    }
}
