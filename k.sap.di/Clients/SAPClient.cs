using k.db.Factory;
using k.Lists;
using k.sap.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI = k.sap.DI;

namespace k.db.Clients
{
    public class SAPClient : IFactory
    {
        private string LOG => this.GetType().FullName;

        private bool HasLine;
        private SAPbobsCOM.Recordset RS;
        private k.Structs.Track Track;

        public bool IsFirstLine { get; internal set;} = false;

        public int FieldCount { get; internal set; }

        public string LastCommand { get; internal set; }

        public string Id { get; internal set; }

        public G.DataBase.TypeOfClient ClientType => G.DataBase.TypeOfClient.MSQL;

        public int Position { get; internal set; }

        public T[] Column<T>(object index)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {

        }

        public void Connect(IDBCredential cred)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connect using credentail ID
        /// </summary>
        /// <param name="id"></param>
        public void Connect(string id)
        {
            return;
            //throw new NotSupportedException();
        }

        public void Dispose()
        {
            if (RS != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(RS);
                RS = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public bool DoQuery(string sql, params dynamic[] values)
        {
            

            try
            {
                LastCommand = Factory.Scripts.FormatQuery(sql, values);
                Track = k.Diagnostic.TrackMessages(sql, "Values:", values, "\nSQL Server: Query formated", LastCommand);
                RS = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                RS.DoQuery(LastCommand);

                HasLine = !RS.EoF;

                if (HasLine)
                {
                    FieldCount = RS.Fields.Count;
                    Position = 0;
                    IsFirstLine = true;
                }
                else
                {
                    Position = -1;
                    IsFirstLine = false;
                }

                return HasLine;
            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, Track, "Error to execute the query.");
                k.Diagnostic.Error(LOG, ex);

                throw ex;
            }
        }

        public Dynamic Field(object index)
        {
            return new Dynamic(RS.Fields.Item(index).Value);
        }

        public Bucket Fields()
        {
            var res = new Bucket();

            for (int c = 0; c < FieldCount; c++)
                res.Add(RS.Fields.Item(c).Name, RS.Fields.Item(c).Value);

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
            // If it not has line
            if (!HasLine)
                return false;


            if (IsFirstLine)
            {
                IsFirstLine = false;
                return true;
            }
            
            if (++Position >= limit && limit > 0)
            {
                k.Diagnostic.Warning(this.GetHashCode(), Track, "The query is limited to show {0} lines.", limit);

                RS = null;
                return false;
            }

            RS.MoveNext();

            if (!RS.EoF)
                return true;
            else
            {
                RS = null;
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
            //if (model.Properties.IsSystem)
            //    throw new NotImplementedException();

            var numTransaction = DI.StartTransaction();

            if(model.Properties.TableType == G.DataBase.SAPTables.TableType.bott_NoObject)
            {
                var foo = k.db.Factory.Scripts.Namespace(model.Properties.Name);
                var oTable = DI.Conn.UserTables.Item(foo);
                oTable.Code = model.Code;
                oTable.Name = model.Name;

                foreach (var field in model.GetTablesField())
                    oTable.UserFields.Fields.Item(field.Key).Value = Dynamic.From(field.Value).ToString();

                var res = oTable.Add();
            }


            //try
            //{
            //    var oCompService = DI.Conn.GetCompanyService();

            //    var oGeneralService = oCompService.GetGeneralService(model.Properties.Name);
            //    var oGeneralData = oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData) as SAPbobsCOM.GeneralData;

            //    var update = model.PrimaryKey(out string col, out dynamic value);

            //    if (update)
            //    {
            //        var oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
            //        oGeneralParams.SetProperty(col, value);
            //        oGeneralData = oGeneralService.GetByParams(oGeneralParams);
            //    }

            //    foreach (var field in model.GetSAPFields())
            //        oGeneralData.SetProperty(field.Key, field.Value);

            //    if (update)
            //        oGeneralService.Update(oGeneralData);
            //    else
            //        oGeneralService.Add(oGeneralData);
            //}catch(Exception ex)
            //{
            //    k.Diagnostic.Error(this, ex);
            //    DI.RollBackTransaction(numTransaction);
            //    throw ex;
            //}
            //finally
            //{
            //    DI.CommitTransaction(numTransaction);
            //}

        }

        public string FormatQuery(string sql, params dynamic[] values)
        {
            throw new NotImplementedException();
        }
    }
}
