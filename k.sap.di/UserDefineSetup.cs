using k.Lists;
using k.sap.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.di
{
    public class UserDefineSetup : k.sap.IUserDefineSetup
    {
        public enum Form
        {
            Default,
            Matrix,
            None
        }
        private string LOG => this.GetType().Name; 

        public List<TableStruct> Tables = new List<TableStruct>();

        public void Add<T>(T dt, Form form) where T : k.sap.Models.DataTable
        {
            var table = new TableStruct(
                dt.Properties.Name,
                dt.Properties.Description,
                dt.Properties.TableName,
                (SAPbobsCOM.BoUTBTableType)dt.Properties.ObjectType,
                dt.Properties.Level,
                dt.Properties.IsSystem,
                form);

           
           
            foreach (var field in dt.GetType().GetFields())
            {
                foreach(object attr in field.GetCustomAttributes(true))
                {
                    if (attr != null && attr is SAPColumnAttribute)
                    {
                        var sapcol = attr as SAPColumnAttribute;
                        var col = new ColumnStruct
                        {
                            Name = field.Name.StartsWith("U_") ? field.Name.Replace("U_", "") : field.Name,
                            Description = sapcol.Description
                        };

                        col.SetFieldType(sapcol.Type);

                        table.Columns.Add(col);
                    }
                }
            }

            Tables.Add(table);
        }

        

        private void CreateTable(TableStruct table)
        {
            if (table.IsSystem)
                return;
            // Add or update only if necessary
            var tableNameWithoutAt = table.Table.Replace("@", "");

            var foo = k.db.Factory.ResultSet.GetRow(R.CredID, Content.Queries.OUTB.Details_1, tableNameWithoutAt);
            if(foo.HasValues)
            {
                if (foo["Descr"].ToString() == table.Description)
                    if (foo["ObjectType"].ToIntOrNull() == (int)table.TableType)
                        return;
            }

            if (!P.CreateTableAndFields)
                throw new KDIException(LOG, E.Message.CreateTableFields_0);


            #region Error -1120 : Error: Ref count for this object is higher then 0
            // Solution : https://archive.sap.com/discussions/thread/1958196
            var oRS = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
            oRS = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion


            var oUserTableMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables) as SAPbobsCOM.UserTablesMD;

            try
            {
                var update = oUserTableMD.GetByKey(tableNameWithoutAt);

                if (!update)
                {
                    oUserTableMD.TableName = tableNameWithoutAt;
                    oUserTableMD.TableType = table.TableType;
                }
                oUserTableMD.TableDescription = table.Description;

                int res;
                if (update)
                    res = oUserTableMD.Update();
                else
                    res = oUserTableMD.Add();

                if (res != 0)
                {
                    var error = k.sap.DI.GetLastErrorDescription();
                    var track = k.Diagnostic.Track(table.ToJson());
                    k.Diagnostic.Error(LOG, R.Project, track, $"({res}) {error}.");
                    throw new Exception($"Error to create {tableNameWithoutAt} table. Error code {res}", new Exception(error));
                }
                else
                {
                    var bar = update ? "updated" : "created";
                    var track = k.Diagnostic.Track(oUserTableMD.GetAsXML());
                    k.Diagnostic.Debug(this.GetHashCode(), track, R.Project, $"{tableNameWithoutAt} table has been {bar} as {table.TableType.ToString()}");
                }

            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, R.Project, ex);
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTableMD);
                oUserTableMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void CreateColumn(string tableName, ColumnStruct col)
        {
            #region Error -1120 : Error: Ref count for this object is higher then 0
            // Solution : https://archive.sap.com/discussions/thread/1958196
            var oRS = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
            oRS = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion

            var colName = col.Name;

            if (colName.Substring(0, 2) == "U_")
                colName = colName.Substring(2);



            var fieldInfo = k.db.Factory.ResultSet.GetRow(R.CredID, Content.Queries.CUFD.FieldIDInfor_2
                , tableName
                , colName);

            #region Check if need to add or update the column.
            if (fieldInfo != null)
            {
                var equals = true;

                equals = equals && fieldInfo["Descr"].Equals(col.Description);
                equals = equals && fieldInfo["Dflt"].Equals(col.DefaultValue);
                equals = equals && fieldInfo["NotNull"].ToBool() == ((int)col.Mandatory == 1 /* Yes */);
                equals = equals && fieldInfo["MyTypeID"].ToString().Equals(col.MyTypeID);

                // Check col structure
                if (equals)
                {
                    var lines = k.db.Factory.ResultSet.GetLines(R.CredID, Content.Queries.CUFD.ValidValues_2
                        , tableName
                        , colName);
                    
                    if(lines != null)
                        foreach (var line in lines[0])
                        {
                            if(!line.Equals(col.ValidValues))
                            {
                                equals = false;
                                break;
                            }
                        }
                }


                if (equals)
                    return;
            }
            #endregion

            var oUserFieldsMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;

            try
            {
                var update = false;
                var res = 0;

                if (fieldInfo != null)
                    update = oUserFieldsMD.GetByKey(tableName, fieldInfo["FieldID"].ToInt());

                

                oUserFieldsMD.TableName = tableName;
                oUserFieldsMD.Name = colName;
                oUserFieldsMD.Description = col.Description;

                oUserFieldsMD.Type = col.Type;
                oUserFieldsMD.SubType = col.SubType;
                oUserFieldsMD.EditSize = col.Size;
                oUserFieldsMD.Mandatory = col.Mandatory;

                if (!String.IsNullOrEmpty(col.DefaultValue))
                    oUserFieldsMD.DefaultValue = col.DefaultValue;

                //if (column.likedTable != null)
                //    oUserFieldsMD.LinkedTable = column.likedTable.TableName;


                #region Loading valid values
                if (col.ValidValues.HasValues)
                {
                    // Creating index of valid values.
                    var indexValidValues = new Dictionary<int, string>(oUserFieldsMD.ValidValues.Count);
                    for (int i = 0; i < oUserFieldsMD.ValidValues.Count; i++)
                    {
                        oUserFieldsMD.ValidValues.SetCurrentLine(i);
                        indexValidValues.Add(i/*current line */, oUserFieldsMD.ValidValues.Value);
                    }


                    // Add or update the valid values
                    foreach (var value in col.ValidValues[0])
                    {
                        var indexValidValue = indexValidValues.Where(t => t.Value.Equals(value.Value.ToString())).Select(t => t.Key).FirstOrDefault();
                        if (indexValidValue > -1)
                        {
                            oUserFieldsMD.ValidValues.SetCurrentLine(indexValidValue);
                        }
                        else if (!String.IsNullOrEmpty(oUserFieldsMD.ValidValues.Value))
                        {
                            oUserFieldsMD.ValidValues.Add();
                            oUserFieldsMD.ValidValues.SetCurrentLine(oUserFieldsMD.ValidValues.Count - 1);
                        }

                        oUserFieldsMD.ValidValues.Value = value.Key;
                        oUserFieldsMD.ValidValues.Description = value.Value;
                    }
                }

                #endregion

                if (update)
                    res = oUserFieldsMD.Update();
                else
                    res = oUserFieldsMD.Add();


                if (res != 0)
                {
                    var error = k.sap.DI.GetLastErrorDescription();
                    var track = k.Diagnostic.Track(col.ToJson());

                    switch (res)
                    {
                        case 0: break;
                        case -2035: // Ignored beacause the column already exist.
                        case -1029:
                        case -5002: // 10000558 - Field length cannot be decreased.
                            k.Diagnostic.Warning(this.GetHashCode(), track, R.Project, $"({res}) {error}");
                            return;
                        default:
                            k.Diagnostic.Error(LOG, R.Project, track, $"({res}) {error}.");
                            throw new Exception($"Error to create {tableName}.{col.Name} column. Error code {res}", new Exception(error));
                    }  
                }
                else
                {
                    k.Diagnostic.Debug(this.GetHashCode(), R.Project, $"{tableName}.{col.Name} column has been successfully created");
                }                
            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, R.Project, ex);
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                oUserFieldsMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void CreateUDOs()
        {
            var dsources = Tables.Where(t => t.IsSystem == false
                                    && (t.TableType == BoUTBTableType.bott_Document
                                        || t.TableType == BoUTBTableType.bott_MasterData)
                                    && t.Level == 0).ToList();

            foreach (var dsource in dsources)
            {

                int res = 0;
                var oUserObjectsMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD) as SAPbobsCOM.UserObjectsMD;

                try
                {
                    var update = oUserObjectsMD.GetByKey(dsource.Name);

                    oUserObjectsMD.Code = dsource.Name;
                    oUserObjectsMD.Name = dsource.Name;
                    oUserObjectsMD.ObjectType = (BoUDOObjType)dsource.TableType;
                    oUserObjectsMD.TableName = dsource.Table;
                    oUserObjectsMD.CanArchive = BoYesNoEnum.tNO;
                    oUserObjectsMD.CanCancel = BoYesNoEnum.tNO;
                    oUserObjectsMD.CanClose = BoYesNoEnum.tNO;

                    if (dsource.Form != Form.None)
                    {
                        oUserObjectsMD.CanCreateDefaultForm = BoYesNoEnum.tYES;
                        if (dsource.Form == Form.Matrix)
                            oUserObjectsMD.EnableEnhancedForm = BoYesNoEnum.tNO; // Create a type of matrix form
                    }

                    oUserObjectsMD.CanDelete = BoYesNoEnum.tYES;
                    oUserObjectsMD.CanFind = BoYesNoEnum.tNO;
                    oUserObjectsMD.CanLog = BoYesNoEnum.tYES;
                    oUserObjectsMD.CanYearTransfer = BoYesNoEnum.tNO;
                    oUserObjectsMD.ManageSeries = BoYesNoEnum.tNO;


                    var tables = Tables.Where(t => t.Name == dsource.Name && t.Level > 0).OrderBy(t => t.Level).ToList();

                    foreach (var table in Tables)
                    {
                        var line = oUserObjectsMD.ChildTables.Count - 1;
                        oUserObjectsMD.ChildTables.SetCurrentLine(line);
                        oUserObjectsMD.ChildTables.TableName = table.Table;
                    }

                    if (update)
                        res = oUserObjectsMD.Update();
                    else
                        res = oUserObjectsMD.Add();

                    if (res != 0)
                    {
                        var error = k.sap.DI.GetLastErrorDescription();
                        var track = k.Diagnostic.TrackObj(oUserObjectsMD);

                        switch (res)
                        {
                            case 0: break;
                            case -2035:
                            case -1029: //Not possible update the UDO.
                            case -5002:
                                k.Diagnostic.Warning(this.GetHashCode(), track, R.Project, $"({res}) {error}");
                                return;
                            default:
                                k.Diagnostic.Error(LOG, R.Project, track, $"({res}) {error}.");
                                throw new Exception($"Error to create {dsource.Name} UDO. Error code {res}", new Exception(error));
                        }
                    }
                    else
                    {
                        k.Diagnostic.Debug(this.GetHashCode(), R.Project, $"{dsource.Name} UDO has been successfully created");
                    }
                }
                catch (Exception ex)
                {
                    k.Diagnostic.Error(LOG, R.Project, ex);
                    throw ex;
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                    oUserObjectsMD = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

        }

        public void Save()
        {
            foreach(var table in Tables)
            {
                CreateTable(table);
                foreach (var col in table.Columns)
                    CreateColumn($"{table.Table}", col);
            }

            CreateUDOs();
        }
    }


    public class TableStruct : k.KModel
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Table;
        public readonly SAPbobsCOM.BoUTBTableType TableType;
        public readonly bool IsSystem;
        public readonly int Level;
        public readonly UserDefineSetup.Form Form;
        public List<ColumnStruct> Columns = new List<ColumnStruct>();

        public TableStruct(string name, string description, string table, BoUTBTableType tableType, int level, bool isSystem, UserDefineSetup.Form form = UserDefineSetup.Form.None)
        {
            Name = name;
            Description = $"{R.Namespace}: {description}";
            Table = table;
            TableType = tableType;
            IsSystem = isSystem;
            Level = level;
            Form = form;
        }
    }
    public class ColumnStruct : k.KModel
    {
        public string Name;
        public string Description;
        public BoFieldTypes Type { get; internal set; }
        public BoFldSubTypes SubType { get; internal set; }
        public string  MyTypeID { get; internal set; }

        public int Size;
        public SAPbobsCOM.BoYesNoEnum Mandatory;
        public string DefaultValue;

        public MyList ValidValues = new MyList();
        
        public void SetFieldType(sap.E.SAPTables.ColumnsType col)
        {
            int boFieldTypes;
            int boFldSubTypes;
            MyTypeID = sap.E.SAPTables.GetTypeID(col, out boFieldTypes, out boFldSubTypes);

            Type = (BoFieldTypes)boFieldTypes;
            SubType = (BoFldSubTypes)boFldSubTypes;


            //TODO : Create only a method (SetFieldType => TypeOfColumn)
            TypeOfColumn(col);
        }


        private void TypeOfColumn(sap.E.SAPTables.ColumnsType sapCol)
        {
            switch (sapCol)
            {
                case sap.E.SAPTables.ColumnsType.Special_MD5:
                    this.Type = BoFieldTypes.db_Alpha;
                    this.SubType = BoFldSubTypes.st_None;
                    this.Size = 32; break;
                case sap.E.SAPTables.ColumnsType.Special_YesOrNo:
                    this.Type = BoFieldTypes.db_Alpha;
                    this.SubType = BoFldSubTypes.st_None;
                    this.Size = 1;
                    this.ValidValues.Add("Y", "Yes");
                    this.ValidValues.Add("N", "No"); break;
                case sap.E.SAPTables.ColumnsType.Special_StatusQueue:
                    this.Type = BoFieldTypes.db_Alpha;
                    this.SubType = BoFldSubTypes.st_None;
                    this.Size = 1;
                    this.ValidValues.Add("E", "Error");
                    this.ValidValues.Add("D", "Done");
                    this.ValidValues.Add("Q", "Queue");
                    this.ValidValues.Add("W", "Waiting");
                    this.ValidValues.Add("C", "Canceled"); break;
                default:
                    if (sapCol.ToString().StartsWith("Special_"))
                        throw new NotImplementedException();
                    else
                        return;
            }


        }
    }
}
