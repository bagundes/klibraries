using k.Propers;
using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.Models
{
    public interface IDataTable
    {
        TableProperties Properties { get; }
        bool PrimaryKey(out string col, out dynamic value);
        IReadOnlyDictionary<string, dynamic> GetTablesField();
    }
}
