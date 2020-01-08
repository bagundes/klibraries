using k.db.Clients;
using k.Lists;
using System;
using System.Collections.Generic;
using System.Text;

namespace k.db
{
    public interface IFactory : IDisposable
    {
        #region Properties
        bool IsFirstLine { get; }
        int FieldCount { get; }
        string LastCommand { get; }
        string Id { get; }
        G.DataBase.TypeOfClient ClientType { get; }

        int Position { get; }
        #endregion

        void Connect(IDBCredential cred);

        void Connect(string credId);


        #region Execute
        bool DoQuery(string sql, params dynamic[] values);
        bool NoQuery(string sql, params dynamic[] values);
        bool Procedure(string name, params object[] values);
        bool Next(int limit = -1);
        #endregion

        #region Result
        Dynamic Field(object index);
        Bucket Fields();

        #endregion

        #region Check
        bool HasDatabase(string name);
        bool HasTable(string database, string table);
        #endregion

        #region Properties
        int Version();
        //ColumnStruct[] Columns(string table);
        #endregion

        void Save<T>(T model) where T : k.sap.Models.IUserDataTableNoObject;
    }
}
