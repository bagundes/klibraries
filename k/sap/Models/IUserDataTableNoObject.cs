using System;
using System.Collections.Generic;
using System.Text;

namespace k.sap.Models
{
    public interface IUserDataTableNoObject : IDataTable
    {
        
        string Code { get; }
        string Name { get; }
    }
}
