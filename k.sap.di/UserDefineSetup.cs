using k.Lists;
using k.sap.Attributes;
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
        private string LOG => this.GetType().FullName; 

        public List<TableStruct> Tables = new List<TableStruct>();

        /// <summary>
        /// Convert model to TableStruct and add in the list to create.
        /// </summary>
        /// <typeparam name="T">DataTable</typeparam>
        /// <param name="model">Model</param>
        /// <param name="form">Create type of form</param>
        public void Add<T>(T model, Form form) where T : k.sap.Models.DataTable
        {
            TableStruct table;

            #region Table struct
            BoUTBTableType boUTBTableType;
            if (model.Properties.TableType == G.DataBase.SAPTables.TableType.System)
                boUTBTableType = (BoUTBTableType)G.DataBase.SAPTables.TableType.bott_NoObject;
            else
                boUTBTableType = (BoUTBTableType)model.Properties.TableType;

            table = new TableStruct(
                model.Properties.Name,
                model.Properties.Description,
                model.Properties.TableName,
                boUTBTableType,
                model.Properties.Level,
                model.Properties.IsSystem,
                form);
            #endregion

            #region Column struct
            foreach (var field in model.GetType().GetFields())
            {
                var col = new ColumnStruct();

                foreach (object attr in field.GetCustomAttributes(true))
                {
                    if (attr != null && attr is SAPColumnAttribute)
                    {
                        var sapcol = attr as SAPColumnAttribute;
                        if (!sapcol.SapColumn)
                        {
                            var customer_tag =  k.G.DataBase.Tags.Namespace;
                            var def_userfield = k.G.DataBase.Tags.DefPrefixUserField;

                            // Removing SAP and Customer tag in the column name
                            col.Name = field.Name.StartsWith(def_userfield) ? field.Name.Replace(def_userfield, null) : field.Name;

                            if(model.Properties.IsSystem)
                                col.Name = col.Name.StartsWith(customer_tag) ? col.Name : k.db.Factory.Scripts.Namespace($"!!{col.Name}");
                            
                            col.Description = k.db.Factory.Scripts.Namespace($"!!: {sapcol.Description}");
                            col.Size = sapcol.Size;
                            col.SetFieldType(sapcol.Type);

                            #region Valid Values
                            if (attr != null && attr is SAPColumnValidValuesAttribute)
                            {
                                var sapVValues = attr as SAPColumnValidValuesAttribute;
                                col.ValidValues = sapVValues.GetValidValues();
                            }
                            #endregion
                        }
                    }

                    
                }

                if (!String.IsNullOrEmpty(col.Name))
                    table.Columns.Add(col);
            }

            Tables.Add(table);
            #endregion
        }

        

        private bool CreateTable(TableStruct table, bool test)
        {
            if (table.IsSystem)
                return false;

            // Add or update only if necessary
            var tableNameWithoutAt = k.db.Factory.Scripts.Namespace(table.Table.Replace("@", ""));

            var foo = k.db.Factory.ResultSet.GetRow(R.CredID, Content.Queries.OUTB.Details_1, tableNameWithoutAt);
            if(foo != null)
            {
                if (foo["Descr"].ToString() == table.Description)
                {
                    if (foo["ObjectType"].ToIntOrNull() == (int)table.TableType)
                        return false;
                    else if (test)
                        return true;
                }

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


            var oMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables) as SAPbobsCOM.UserTablesMD;

            try
            {
                var update = oMD.GetByKey(tableNameWithoutAt);

                if (!update)
                {
                    oMD.TableName = tableNameWithoutAt;
                    oMD.TableType = table.TableType;
                }
                oMD.TableDescription = table.Description;

                int res;
                if (update)
                    res = oMD.Update();
                else
                    res = oMD.Add();


                var track = k.Diagnostic.TrackMessages("Model:", table.ToJson(), oMD.GetType().Name, oMD.GetAsXML());
                
                if (res != 0)
                {
                    var error = k.sap.DI.GetLastErrorDescription();
                    k.Diagnostic.Error(LOG, track, $"({res}) {error}.");
                    throw new Exception($"Error to create {tableNameWithoutAt} table. Error code {res}", new Exception(error));
                }
                else
                {
                    var bar = update ? "updated" : "created";
                    k.Diagnostic.Debug(this, track, $"{tableNameWithoutAt} table has been {bar} as {table.TableType.ToString()}");

                    return true;
                }

            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oMD);
                oMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }            


        private bool CreateColumn(string tableName, ColumnStruct col, bool test)
        {

            var customer_tag = k.G.DataBase.Tags.Namespace;

            var colName = col.Name;

            if (colName.StartsWith("U_"))
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
                    return false;
                else if (test)
                    return !equals;
            }
            #endregion

            var oMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;

            try
            {
                #region Error -1120 : Error: Ref count for this object is higher then 0
                // Solution : https://archive.sap.com/discussions/thread/1958196
                var oRS = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
                oRS = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                #endregion

                var update = false;
                var res = 0;

                if (fieldInfo != null)
                    update = oMD.GetByKey(tableName, fieldInfo["FieldID"].ToInt());

                oMD.TableName = tableName;
                oMD.Name = colName;
                oMD.Description = col.Description;

                oMD.Type = col.Type;
                oMD.SubType = col.SubType;

                if (update && oMD.EditSize > col.Size)
                    Diagnostic.Warning(this, null, $"It's not possible reduce size on {tableName}.{colName} column.");
                else
                    oMD.EditSize = col.Size;




                oMD.EditSize = oMD.EditSize < col.Size ? col.Size : oMD.EditSize;
                oMD.Mandatory = col.Mandatory;

                if (!String.IsNullOrEmpty(col.DefaultValue))
                    oMD.DefaultValue = col.DefaultValue;

                //if (column.likedTable != null)
                //    oUserFieldsMD.LinkedTable = column.likedTable.TableName;


                #region Loading valid values
                if (col.ValidValues.HasValues)
                {
                    // Creating index of valid values.
                    var indexValidValues = new Dictionary<int, string>(oMD.ValidValues.Count);
                    for (int i = 0; i < oMD.ValidValues.Count; i++)
                    {
                        oMD.ValidValues.SetCurrentLine(i);
                        if(!String.IsNullOrEmpty(oMD.ValidValues.Value))
                            indexValidValues.Add(i/*current line */, oMD.ValidValues.Value);
                    }


                    // Add or update the valid values
                    foreach (var value in col.ValidValues[0])
                    {
                        var indexValidValue = -1;

                        if (indexValidValues.Where(t => t.Value.Equals(value.Value.ToString())).Select(t => t.Key).Any())
                            indexValidValue = indexValidValues.Where(t => t.Value.Equals(value.Value.ToString())).Select(t => t.Key).FirstOrDefault();
                                

                        if (indexValidValue > -1)
                        {
                            oMD.ValidValues.SetCurrentLine(indexValidValue);
                        }
                        else if (!String.IsNullOrEmpty(oMD.ValidValues.Value))
                        {
                            oMD.ValidValues.Add();
                            oMD.ValidValues.SetCurrentLine(oMD.ValidValues.Count - 1);
                        }

                        oMD.ValidValues.Value = value.Key;
                        oMD.ValidValues.Description = value.Value;
                    }
                } else
                {
                    if(oMD.ValidValues.Count > 0)
                        oMD.ValidValues.Delete();
                }

                #endregion

                if (update)
                    res = oMD.Update();
                else
                    res = oMD.Add();

                var track = k.Diagnostic.TrackMessages("Model:", col.ToJson(), oMD.GetType().Name, oMD.GetAsXML());
                if (res != 0)
                {
                    var error = k.sap.DI.GetLastErrorDescription();

                    switch (res)
                    {
                        case 0: break;
                        case -2035: // Ignored beacause the column already exist.
                        case -1029:
                        case -5002: // 10000558 - Field length cannot be decreased.
                            k.Diagnostic.Warning(this.GetHashCode(), track, $"({res}) {error}");
                            return false;
                        default:
                            k.Diagnostic.Error(LOG, track, $"({res}) {error}.");
                            throw new Exception($"Error to create {tableName}.{col.Name} column. Error code {res}", new Exception(error));
                    }  
                }
                else
                {
                    k.Diagnostic.Debug(this, track, $"{tableName}.{col.Name} column has been successfully created");
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
                k.Diagnostic.Error(this, null, $"Error to create the {tableName}.{colName} field");
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oMD);
                oMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void CreateUDOs()
        {
            // TODO: Create ruler to check if it need to add or update the UDO
            var dsources = Tables.Where(t => t.IsSystem == false
                                    && (t.TableType == BoUTBTableType.bott_Document
                                        || t.TableType == BoUTBTableType.bott_MasterData)
                                    && t.Level == 0).ToList();

            foreach (var dsource in dsources)
            {

                int res = 0;
                var oMD = k.sap.DI.Conn.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD) as SAPbobsCOM.UserObjectsMD;

                try
                {
                    var update = oMD.GetByKey(dsource.Name);

                    oMD.Code = dsource.Name;
                    oMD.Name = dsource.Name;
                    oMD.ObjectType = (BoUDOObjType)dsource.TableType;
                    oMD.TableName = dsource.Table;
                    oMD.CanArchive = BoYesNoEnum.tNO;
                    oMD.CanCancel = BoYesNoEnum.tNO;
                    oMD.CanClose = BoYesNoEnum.tNO;

                    if (dsource.Form != Form.None)
                    {
                        oMD.CanCreateDefaultForm = BoYesNoEnum.tYES;
                        if (dsource.Form == Form.Matrix)
                            oMD.EnableEnhancedForm = BoYesNoEnum.tNO; // Create a type of matrix form
                    }

                    oMD.CanDelete = BoYesNoEnum.tYES;
                    oMD.CanFind = BoYesNoEnum.tNO;
                    oMD.CanLog = BoYesNoEnum.tYES;
                    oMD.CanYearTransfer = BoYesNoEnum.tNO;
                    oMD.ManageSeries = BoYesNoEnum.tNO;


                    var tables = Tables.Where(t => t.Name == dsource.Name && t.Level > 0).OrderBy(t => t.Level).ToList();

                    foreach (var table in Tables)
                    {
                        var line = oMD.ChildTables.Count - 1;
                        oMD.ChildTables.SetCurrentLine(line);
                        oMD.ChildTables.TableName = table.Table;
                    }

                    if (update)
                        res = oMD.Update();
                    else
                        res = oMD.Add();

                    var track = k.Diagnostic.TrackMessages("Model:", dsource.ToJson(), oMD.GetType().Name, oMD.GetAsXML());
                    if (res != 0)
                    {
                        var error = k.sap.DI.GetLastErrorDescription();

                        switch (res)
                        {
                            case 0: break;
                            case -2035:
                            case -1029: //Not possible update the UDO.
                            case -5002:
                                k.Diagnostic.Warning(this.GetHashCode(), track, $"({res}) {error}");
                                return;
                            default:
                                k.Diagnostic.Error(LOG, track, $"({res}) {error}.");
                                throw new Exception($"Error to create {dsource.Name} UDO. Error code {res}", new Exception(error));
                        }
                    }
                    else
                    {
                        k.Diagnostic.Debug(this.GetHashCode(), null, $"{dsource.Name} UDO has been successfully created");
                    }
                }
                catch (Exception ex)
                {
                    k.Diagnostic.Error(LOG, ex);
                    throw ex;
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oMD);
                    oMD = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

        }

        /// <summary>
        /// Create or update the user data tables
        /// </summary>
        /// <param name="test">Execute the test mode</param>
        /// <returns></returns>
        public bool Save(bool test)
        {
            // It return true if any method return true
            var res = false;
            
            foreach(var table in Tables)
            {
                res = CreateTable(table, test) || res;
                foreach (var col in table.Columns)
                    res = CreateColumn($"{table.Table}", col, test) || res;
            }

            CreateUDOs();

            return res;
        }
    }

    /// <summary>
    /// Table structure k library format
    /// </summary>
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

        /// <summary>
        /// Table struct
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="table"></param>
        /// <param name="tableType"></param>
        /// <param name="level"></param>
        /// <param name="isSystem"></param>
        /// <param name="form"></param>
        public TableStruct(string name, string description, string table, BoUTBTableType tableType, int level, bool isSystem, UserDefineSetup.Form form = UserDefineSetup.Form.None)
        {
            Name = name;
            Description = $"{R.Namespace}: {description}";
            Table = k.db.Factory.Scripts.Namespace(table);
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

        public Bucket ValidValues = new Bucket();
        
        public void SetFieldType(G.DataBase.SAPTables.ColumnsType col)
        {
            int boFieldTypes;
            int boFldSubTypes;
            MyTypeID = G.DataBase.SAPTables.GetTypeID(col, out boFieldTypes, out boFldSubTypes);

            Type = (BoFieldTypes)boFieldTypes;
            SubType = (BoFldSubTypes)boFldSubTypes;


            //TODO : Create only a method (SetFieldType => TypeOfColumn)
            TypeOfColumn(col);
        }


        private void TypeOfColumn(G.DataBase.SAPTables.ColumnsType sapCol)
        {
            switch (sapCol)
            {
                case G.DataBase.SAPTables.ColumnsType.Special_MD5:
                    this.Type = BoFieldTypes.db_Alpha;
                    this.SubType = BoFldSubTypes.st_None;
                    this.Size = 32; break;
                case G.DataBase.SAPTables.ColumnsType.Special_YesOrNo:
                    this.Type = BoFieldTypes.db_Alpha;
                    this.SubType = BoFldSubTypes.st_None;
                    this.Size = 1;
                    this.ValidValues.Add("Y", "Yes");
                    this.ValidValues.Add("N", "No"); break;
                case G.DataBase.SAPTables.ColumnsType.Special_StatusQueue:
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
