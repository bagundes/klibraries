using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Clients
{
    public interface IDBCredential
    {
            string DbServer { get; }
            string DbDatabase { get; }
    }
}
