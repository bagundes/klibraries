using k.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace k
{
    public partial class Dynamic : IEquatable<Dynamic>
    {
        public readonly object Value;

        public Dynamic()
        {
            Value = null;
        }
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

        public static explicit operator Dynamic(Int64 v)
        {
            return new Dynamic(v);
        }

        public static implicit operator int(Dynamic v)
        {
            return v.ToInt(0);
        }

        public static implicit operator Dynamic(DateTime v)
        {
            return new Dynamic(v);
        }

        public static implicit operator DateTime(Dynamic v)
        {
            return v.ToDateTime();
        }

        public static implicit operator Dynamic(bool v)
        {
            return new Dynamic(v);
        }

        public static implicit operator Dynamic(double v)
        {
            return new Dynamic(v);
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
            return Value == null ? String.Empty : Value.ToString().Trim();
        }

        public object ToObject()
        {
            return Value;
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

        public int? ToIntOrNull()
        {
            int val;
            if (int.TryParse(this.ToString(), out val))
                return val;
            else
                return null;
        }

        public TimeSpan ToTime()
        {
            // TODO: change code
            var foo = TimeSpan.ParseExact(Value.ToString(), "hmm", null);
            return foo;
            //var foo = ToString().Split(':');
            //return new TimeSpan(int.Parse(foo[0]), int.Parse(foo[1]), 0);
        }

        public DateTime ToTime(DateTime date)
        {
            return date.Add(ToTime());
        }

        public bool ToBool()
        {
            var val = Value.ToString().ToUpper();
            switch(val)
            {
                case "T":
                case "TRUE":
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

        #region SAP formats
        public DateTime Sap_ToDate()
        {
            return ToDateTime("yyyyMMdd");
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
        public bool Equals(Dynamic other) => Value.ToString().Equals(other.ToString());

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<object>.Default.GetHashCode(Value);
        }

        public bool IsNullOrEmpty()
        {
            return Value == null || String.IsNullOrEmpty(Value.ToString());
        }
        #endregion
    }

    public partial class Dynamic
    {
        #region Static
        private static string LOG => typeof(Dynamic).FullName;
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
                    format = format.Replace($"{{{i}}}", String.Format("{0}", values[i]));
                }
            }

            if (sf != values.Length)
            {
                var list = new List<object>();
                list.Add($"Value : {val1}");
                list.Add($"Format: {format}");
                for (int i = 0; i < values.Length; i++)
                    list.Add($"{String.Format("{0, 6}", i)}: {values[i]}");

                var track = k.Diagnostic.TrackMessages(list.ToArray());
                k.Diagnostic.Warning("StringFormat", track, "The string is not contains all values of parameters");

                return $"!{format}";
            }

            return format;
        }

        public static string RemoveDuplicateChars(string val)
        {
            var list = new List<char>();

            foreach (var c in val)
            {
                if (!list.Contains(c))
                    list.Add(c);
            }

            return String.Join("", list.ToArray());

        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static string GetEnumAlias(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                var attributes = fi.GetCustomAttributes(typeof(AliasAttribute), false) as AliasAttribute[];

                if (attributes != null && attributes.Any())
                {
                    return attributes.First().Alias;
                }

                return value.ToString();
            }
            else
                return String.Empty;
        }

        public static T GetValueFromAlias<T>(string alias) where T : Enum
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                typeof(AliasAttribute)) as AliasAttribute;
                if (attribute != null)
                {
                    if (attribute.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Equals(alias, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
            }

            // If alias is not exists, return value default
            try
            {
                return (T)System.Enum.Parse(typeof(T), "None");
            }
            catch (Exception ex)
            {
                k.Diagnostic.Error(LOG, ex);
                throw ex;
            }


        }

        public static Dynamic From(object val)
        {
            return new Dynamic(val);
        }
        #endregion
    }
}
