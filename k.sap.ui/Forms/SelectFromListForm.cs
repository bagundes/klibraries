using k.Lists;
using k.sap.ui.Helpers;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace k.sap.ui.Forms
{
    // Loading form and properties
    public partial class SelectFromListForm : UserForm
    {
        #region Items
        private EditText txtFindItem;
        private Button butSearchItem, butChooseItem;
        private DataTable udtBucket;
        private Grid gridBucketItem;
        #endregion

        #region Labels
        private string BucketDataTableXMLUniqueID => "DT_List";
        private string ColCheckBoxUniqueID => "chkCol";

        public static string FormTypeEx => "CFLV2";
        #endregion

        private string OriginalDataXml;
        private string Title;
        public readonly bool Multi;
        private bool Selected = false;
        private Action<Bucket> Call;

        public SelectFromListForm(Action<Bucket> call, string title, bool multi) : base(FormHelper.GetSRFFile(typeof(SelectFromListForm).Name, R.Assembly))
        {
            Title = title;
            Multi = multi;
            Call = call;
        }

        /// <summary>
        /// Load Select From List
        /// </summary>
        /// <param name="multi">Select multiple lines</param>
        /// <param name="sql">Query</param>
        /// <param name="values">Values to query</param>
        public void Load(string sql, params object[] values)
        {
            try
            {

            
            base.Load(true);
            oForm.Title = $"{k.R.Namespace}: {Title}";
            udtBucket = oForm.DataSources.DataTables.Item(BucketDataTableXMLUniqueID);

            LoadEvents();

            LoadGridBucket(Multi, sql, values);
            } finally
            {
                oForm.Freeze(false);
            }


        }

        protected override void LoadEvents()
        {
            butSearchItem.ClickAfter += butSearch_ClickAfter;
            butChooseItem.ClickAfter += butChoose_ClickAfter;
        }
    }

    // Events
    public partial class SelectFromListForm
    {
        private void butSearch_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            var filterText = FormHelper.GetValue(txtFindItem.Item, false);
            //if (!filterText.IsNullOrEmpty())
            Filter(filterText);
        }
        private void butChoose_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            oForm.Freeze(true);
            var bucket = GetSelectedValues();
            Call(bucket);
            oForm.Close();
        }
    }

    // Actions
    public partial class SelectFromListForm
    {
        private void LoadGridBucket(bool multi, string sql, params object[] values)
        {
            try 
            {
                oForm.Freeze(true);
                #region Prepare ListDataTable
                sql = db.Factory.Scripts.FormatQuery(sql, values);

                udtBucket.ExecuteQuery(sql);
                this.OriginalDataXml = udtBucket.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly);

                if (multi)
                {
                    PrepareCheckBoxListDataTable();
                    gridBucketItem.SelectionMode = BoMatrixSelect.ms_None;
                    gridBucketItem.Columns.Item(ColCheckBoxUniqueID).Type = BoGridColumnType.gct_CheckBox;
                    gridBucketItem.Columns.Item(ColCheckBoxUniqueID).TitleObject.Caption = "Sel";
                    gridBucketItem.Columns.Item(ColCheckBoxUniqueID).Width = 25;
                }
                else
                {
                    gridBucketItem.SelectionMode = BoMatrixSelect.ms_Single;
                }
                #endregion
                
                for (int i = 0; i < gridBucketItem.Columns.Count; i++)
                {
                    var col = gridBucketItem.Columns.Item(i);

                    if (col.UniqueID != ColCheckBoxUniqueID)
                        col.Editable = false;

                    gridBucketItem.Columns.Item(ColCheckBoxUniqueID).TitleObject.Sortable = true;
                }

                gridBucketItem.AutoResizeColumns();

            }
            finally
            {
                oForm.Freeze(false);
            }
            
        }

        private void Filter(string filter)
        {
            if (String.IsNullOrEmpty(filter))
            {
                udtBucket.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, OriginalDataXml);
                return;
            }

            var filtedDataXml = OriginalDataXml.Clone() as string;

            byte[] byteArray = Encoding.Unicode.GetBytes(OriginalDataXml);
            MemoryStream originalXML = new MemoryStream(byteArray);

            var xdoc = XDocument.Load(originalXML);

            #region Filter data
            foreach (var root in xdoc.Elements())
                foreach (var rows in root.Elements())
                    foreach (var row in rows.Elements())
                    {
                        var exists = false;
                        foreach (var cells in row.Elements())
                        {
                            foreach (var cell in cells.Elements())
                            {
                                exists = cell.Element("Value").Value.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase);
                                if (exists)
                                    break;
                            }
                        }

                        if (!exists)
                            filtedDataXml = filtedDataXml.Replace(row.ToString(SaveOptions.DisableFormatting), "");
                    }
            #endregion

            udtBucket.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, filtedDataXml);
        }

        private void PrepareCheckBoxListDataTable()
        {
            byte[] byteArray;

            var newMetaDataXml = String.Empty;
            #region Metadata ColCheckBoxUniqueID
            var metaDataXml = udtBucket.SerializeAsXML(BoDataTableXmlSelect.dxs_MetaData);
            byteArray = Encoding.Unicode.GetBytes(metaDataXml);

            using (MemoryStream originalXML = new MemoryStream(byteArray))
            {

                var xMetaData = XDocument.Load(originalXML);

                var colTag = new XElement("Column");
                colTag.SetAttributeValue("Uid", ColCheckBoxUniqueID);
                colTag.SetAttributeValue("Type", 1);
                colTag.SetAttributeValue("MaxLength", 1);

                foreach (var root in xMetaData.Elements())
                    foreach (var columns in root.Elements())
                        columns.AddFirst(colTag);

                newMetaDataXml = xMetaData.ToString(SaveOptions.DisableFormatting);
            }
            #endregion

            var newDataXml = String.Empty;
            #region Serialize values of ColCheckBoxUniqueID

            var dataXml = udtBucket.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly);
            byteArray = Encoding.Unicode.GetBytes(dataXml);

            using (MemoryStream originalXML = new MemoryStream(byteArray))
            {

                var xdoc = XDocument.Load(originalXML);
                var selChkTag = new XElement("ColumnUid", ColCheckBoxUniqueID);
                var selChkTagValue = new XElement("Value", "N");
                var cellTag = new XElement("Cell", selChkTag, selChkTagValue);

                foreach (var root in xdoc.Elements())
                    foreach (var rows in root.Elements())
                        foreach (var row in rows.Elements())
                            foreach (var cells in row.Elements())
                                cells.AddFirst(cellTag);


                newDataXml = xdoc.ToString(SaveOptions.DisableFormatting);
            }
            #endregion

            udtBucket.Clear();
            var foo = "<?xml version=\"1.0\" encoding=\"UTF-16\" ?>";
            udtBucket.LoadSerializedXML(BoDataTableXmlSelect.dxs_MetaData, foo + newMetaDataXml);
            udtBucket.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, foo + newDataXml);
            
            // Update data cache
            this.OriginalDataXml = newDataXml;
        }

        public Bucket GetSelectedValues()
        {
            var bucket = new Bucket();
            if(Multi)
            {
                var foo = 0;
                for(int l = 0; l < udtBucket.Rows.Count; l++)
                    if(udtBucket.GetValue(ColCheckBoxUniqueID, l) == "Y")
                    {
                        for (int c = 0; c < udtBucket.Columns.Count; c++)
                        {
                            bucket.Add(udtBucket.Columns.Item(c).Name, udtBucket.GetValue(c, l), foo);
                        }
                        foo++;
                    }
            }
            else
            {
                throw new NotImplementedException();
            }

            return bucket;
        }
    }
}
