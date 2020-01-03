using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Forms
{
    public abstract class SystemFormLoad : Form
    {

        public SystemFormLoad(string typeEx)
        {
            TypeEx = typeEx;
        }

        public virtual void Load()
        {
            this.oForm = k.sap.UI.GetActiveForm();
            if (oForm.TypeEx != TypeEx)
                throw new KUIException(LOG, E.Message.InvalidActiveForm_2, TypeEx, oForm.TypeEx);

            InitComponents();
            LoadEvents();
        }

        public override void InitComponents()
        {
            #region System Items
            //MtxCalendar_sys10Item
            foreach (var f in k.Reflection.GetPrivateFields(this)
                .Where(t => t.Name.Contains("_sys") && t.Name.EndsWith("Item")))
            {
                var foo = f.Name.Split('_');
                var name = foo[1].Replace("sys", "").Replace("Item", "");

                try
                {
                    f.SetValue(this, oForm.Items.Item(name).Specific);
                }
                catch (Exception ex)
                {
                    Diagnostic.Error(this, Diagnostic.TrackException(ex), 
                        $"Error to instance the {f.Name} field with {name} item. {ex.Message}");
                    throw ex;
                }
            }
            #endregion

            base.InitComponents();
        }
    }
}
