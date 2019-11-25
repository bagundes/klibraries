using System;
using System.Collections.Generic;
using System.Text;

namespace k
{
    public partial class Dynamic
    {
        public readonly object Value;

        public Dynamic(object value)
        {
            Value = value;
        }

        #region Implicit conversion
        public static implicit operator Dynamic(string v)
        {
            return new Dynamic(v); 
        }
        #endregion

        #region Convert to
        public override string ToString()
        {
            return Value.ToString();
        }

        public int ToInt(int def = 0)
        {
            int val;
            if (int.TryParse(this.ToString(), out val))
                return val;
            else
                return def;
        }

        public bool ToBool()
        {
            var val = Value.ToString().ToUpper();
            switch(val)
            {
                case "Y":
                case "YES":
                case "S":
                case "SI":
                case "SIM":
                case "1":
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Format to
        public string StringFormat(params object[] values)
        {
            var val = Value.ToString();
            for (int i = 0; i < values.Length; i++)
                val = val.Replace($"{{{i}}}", values[i].ToString());

            return val;
        }
        #endregion

        #region Validation

        #endregion

        #region Static
        public static Dynamic Empty => new Dynamic(String.Empty);
        #endregion
    }


}
