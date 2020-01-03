using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Forms
{
    public interface IForm
    {
        void InitComponents();
    }
    public abstract class Form
    {
        protected string LOG => this.GetType().FullName;

        protected SAPbouiCOM.Form oForm;
        protected string TypeEx;
        protected abstract void LoadEvents();

        public virtual void InitComponents()
        {
            #region User Items
            foreach (var f in k.Reflection.GetPrivateFields(this).Where(t => t.Name.EndsWith("Item") && !t.Name.Contains("_sys")))
            {
                var name = f.Name.Replace("Item", "");
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
        }
    }
}
