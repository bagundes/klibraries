using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using k.Lists;
using k.db.Factory;

namespace k.db.Clients
{
    public class SqlClient : Factory.IFactory
    {
        private string LOG => this.GetType().Name;

        private string StringConn;
        private SqlConnection Conn;
        private SqlCommand Command;
        private SqlDataReader DataReader;
        private int Cursor;
        private string Alias;
        private bool HasLine;
        private string Track;

        public bool IsFirstLine { get; internal set; }

        public int FieldCount { get; internal set; }

        public string LastCommand { get; internal set; }

        public string Id { get; internal set; }

        public E.DataBase.TypeOfClient ClientType => E.DataBase.TypeOfClient.MSQL;

        public int Position { get; internal set; }

        public T[] Column<T>(object index)
        {
            throw new NotImplementedException();
        }


        public void Connect(SqlCredential cred)
        {
            Dispose();

            StringConn = cred.ToString();
            Conn = new SqlConnection(StringConn);
            Conn.Open();
            Alias = cred.DetailsSimple();
            Id = cred.Save();
            k.Diagnostic.Debug(this.GetHashCode(), R.Project, $"Connected on SQL Server: {StringConn}");
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
                k.Diagnostic.Debug(this.GetHashCode(), R.Project, $"Desconnected on SQL Server: {Alias}");
            }
        }

        public bool DoQuery(string sql, params dynamic[] values)
        {
            try
            {
                LastCommand = Factory.Scripts.FormatQuery(sql, values);
                Track = k.Diagnostic.Track(sql, "Values:", values, "\nSQL Server: Query formated", LastCommand);
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
                k.Diagnostic.Error(LOG, R.Project, ex);
                k.Diagnostic.Error(LOG, Track, R.Project, "Error to execute the query. {0}", ex.Message);

                throw ex;
            }
        }

        public Dynamic Field(object index)
        {
            throw new NotImplementedException();
        }

        public MyList Fields()
        {
            var res = new MyList();

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
            Position++;

            if (!HasLine)
                return false;

            Cursor++;
            IsFirstLine = Cursor == 1;

            if (Cursor > limit && limit > 0)
            {
                k.Diagnostic.Warning(this.GetHashCode(), Track, R.Project, "The query is limited to show {0} lines.", limit);

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
    }
}
