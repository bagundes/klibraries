using System;
using System.Collections.Generic;
using System.Text;

namespace k.Interfaces
{
    public interface IUserDataTableNoObject : IDataTable
    {
        
        string Code { get; }
        string Name { get; }
    }
}
