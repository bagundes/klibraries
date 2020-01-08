using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.sap.ui.Forms
{
    
    public abstract class UserForm : Form
    {
        private string srf;
        public UserForm(string srf)
        {
            this.srf = srf;
        }


        protected void Load(bool visible)
        {
            oForm = k.sap.ui.Helpers.FormHelper.Load(srf, visible);

            InitComponents();
        }
    }
}
