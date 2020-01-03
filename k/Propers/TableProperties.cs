using System;
using System.Collections.Generic;
using System.Text;

namespace k.Propers
{
    public class TableProperties
    {
        /// <summary>
        /// Name of object
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Name of table
        /// </summary>
        public string TableName { get; set; }
        public string Description { get; set; }
        public G.DataBase.SAPTables.TableType TableType { get; set; }
        public bool IsSystem { get; set; }
        public int Level { get; set; }
    }
}
