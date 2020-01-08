using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using k.Lists;
using k.db.Factory;
using k.sap.Models;

namespace k.db.Clients
{
    public class SqlClient : k.db.IFactory
    {
        private string LOG => this.GetType().FullName;

        private string StringConn;
        private SqlConnection Conn;
        private SqlCommand Command;
        private SqlDataReader DataReader;
        private int Cursor;
        private string Alias;
        private bool HasLine;
        private k.Structs.Track Track;

        public bool IsFirstLine { get; internal set; }

        public int FieldCount { get; internal set; }

        public string LastCommand { get; internal set; }

        public string Id { get; internal set; }

        public G.DataBase.TypeOfClient ClientType => G.DataBase.TypeOfClient.MSQL;

        public int Position { get; internal set; }

        G.DataBase.TypeOfClient IFactory.ClientType => throw new NotImplementedException();

        public T[] Column<T>(object index)
        {
            throw new NotImplementedException();
        }

        public void Connect(IDBCredential cred)
        {
            Dispose();
            var sqlCred = cred as SqlCredential;
            StringConn = cred.ToString();
            Conn = new SqlConnection(StringConn);
            Conn.Open();
            Alias = sqlCred.Info2();
            Id = sqlCred.Save();
            k.Diagnostic.Debug(this.GetHashCode(), null, $"Connected on SQL Server: {StringConn}");
        }

        //TODO: Remove this constructor
        public void Connect(SqlCredential cred)
        {
            Dispose();

            StringConn = cred.ToString();
            Conn = new SqlConnection(StringConn);
            Conn.Open();
            Alias = cred.Info2();
            Id = cred.Save();
            k.Diagnostic.Debug(this.GetHashCode(), null, $"Connected on SQL Server: {StringConn}");
        }

        /// <summary>
        /// Connect using credentail ID
        /// </summary>
        /// <param name="id"></param>
        public void Connect(string id)
        {
            Connect(new SqlCredential(id));
        }

        public void Connect(DBCredential cred)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (Conn != null && Conn.State == System.Data.ConnectionState.Open)
            {
                Conn.Close();
                k.Diagnostic.Debug(this.GetHashCode(), null, $"Desconnected on SQL Server: {Alias}");
            }
        }

        public bool DoQuery(string sql, params dynamic[] values)
        {
            try
            {
                LastCommand = Factory.Scripts.FormatQuery(sql, values);
                Track = k.Diagnostic.TrackMessages(sql, "Values:", values, "\nSQL Server: Query formated", LastCommand);
                Command = new SqlCommand(LastCommand, Conn);
                DataReader = Command.ExecuteReader();

                if(DataReader.HasRows)
                {
                    DataReader.Read();
                    FieldCount = DataReader.FieldCount;
                    Position = 0;
                }
                else
                {
                    Position = -1;
                }

                IsFirstLine = DataReader.HasRows;
                HasLine = DataReader.HasRows;
                return HasLine;
            }
            catch(Exception ex)
            {
                k.Diagnostic.Error(LOG, Track, "Error to execute the query.");
                k.Diagnostic.Error(LOG, ex);

                throw ex;
            }
        }

        public Dynamic Field(object index)
        {
            throw new NotImplementedException();
        }

        public Bucket Fields()
        {
            var res = new Bucket();

            for (int c = 0; c < FieldCount; c++)
                res.Add(DataReader.GetName(c), DataReader.GetValue(c));

            return res;
        }

        public bool HasDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasTable(string database, string table)
        {
            throw new NotImplementedException();
        }

        public Dynamic[] Line1()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Dynamic> Line2()
        {
            throw new NotImplementedException();
        }

        public bool Next(int limit = -1)
        {
            

            if (!HasLine)
                return false;

            Cursor++;
            IsFirstLine = Cursor == 1;

            if (IsFirstLine)
                Position = 0;
            else
                Position++;

            if (Cursor > limit && limit > 0)
            {
                k.Diagnostic.Warning(this.GetHashCode(), Track, "The query is limited to show {0} lines.", limit);

                DataReader.Close();
            }

            if (DataReader.IsClosed)
                return false;

            if (Cursor == 1)
                return true;

            if (DataReader.Read())
                return true;
            else
            {
                DataReader.Close();
                return false;
            }
        }

        public bool NoQuery(string sql, params dynamic[] values)
        {
            throw new NotImplementedException();
        }

        public bool Procedure(string name, params object[] values)
        {
            throw new NotImplementedException();
        }

        public int Version()
        {
            throw new NotImplementedException();
        }

        public void Save<T>(T model) where T : IUserDataTableNoObject
        {
            throw new NotImplementedException();
        }
    }
}
