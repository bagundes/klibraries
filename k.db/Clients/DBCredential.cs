using System;
using System.Collections.Generic;
using System.Text;

namespace k.db.Clients
{
    public abstract class DBCredential : k.Models.Credential
    {
        public virtual string DbServer => Host;
        public virtual string DbDatabase => Schema;

        public DBCredential() : base()
        {
        }        

    }
}
