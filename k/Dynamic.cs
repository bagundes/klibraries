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

        public static implicit operator string(Dynamic v)
        {
            return v.ToString();
        }


        public static implicit operator Dynamic(int v)
        {
            return new Dynamic(v);
        }

        public static implicit operator int(Dynamic v)
        {
            return v.ToInt();
        }

        public static implicit operator Dynamic(DateTime v)
        {
            return new Dynamic(v);
        }

        public static implicit operator DateTime(Dynamic v)
        {
            return v.ToDateTime();
        }
        #endregion

        #region Convert to
        public T ToEnum<T>() where T : Enum
        {
            throw new NotImplementedException();
            //return (T)ToInt();
        }
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
            {
                //Diagnostic.Debug(this.GetHashCode)
                return def;
            }
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
        
        public DateTime ToDateTime(string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (Value is DateTime) //if (Value.GetType() == typeof(DateTime))
                return (DateTime)Value;
            else
                return DateTime.ParseExact(Value.ToString(), format,
                                       System.Globalization.CultureInfo.InvariantCulture);
        }
        #endregion

        #region Format to
        public string StringFormat(params object[] values)
        {
            var val = Value.ToString();
            return Dynamic.StringFormat(val, values);
        }

        public string Decrypt()
        {
            return k.Security.Decrypt(ToString());
        }
        #endregion

        #region Validation

        #endregion

        #region Static
        public static Dynamic Empty => new Dynamic(String.Empty);

        public static string StringFormat(string format, params object[] values)
        {
            var val1 = format.Clone().ToString();

            values = values ?? new object[0];
            int sf = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (format.Contains($"{{{i}}}"))
                {
                    sf++;
                    format = format.Replace($"{{{i}}}", values[i].ToString());
                }
            }

            if (sf != values.Length)
            {
                var list = new List<object>();
                list.Add($"Value : {val1}");
                list.Add($"Format: {format}");
                for (int i = 0; i < values.Length; i++)
                    list.Add($"{String.Format("{0, 6}", i)}: {values[i]}");

                var track = k.Diagnostic.Track(list.ToArray());
                k.Diagnostic.Warning("StringFormat", track, R.Project, "The string is not contains all values of parameters");

                return $"!{format}";
            }

            return format;
        }
        #endregion
    }


}
