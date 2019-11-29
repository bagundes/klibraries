using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k.Lists
{
    /// <summary>
    /// List of parameters, the value can be any object.
    /// </summary>
    public class GenericList : IBaseList
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        public string[] GetKeys() => Parameters.Select(t => t.Key).ToArray();

        /// <summary>
        /// Get parameter value.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns>Value.  If key is not exists the value will be empty.</returns>
        public Dynamic this[string key]
        {
            get
            {
                key = key.ToLower();

                if (Contains(key))
                    return new Dynamic(Parameters[GetKey(key)]);
                else
                    return Dynamic.Empty;
            }
        }

        public Dynamic this[int index]
        {
            get
            {
                var key = Parameters.Keys.ToList()[index];

                return new Dynamic(Parameters[key]);
            }
        }

        /// <summary>
        /// Add new value, if key exists the method will add number to the name (keyN).
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value</param>
        /// <returns>Key name</returns>
        public string Add(string key, object value)
        {
            key = key.ToLower();

            if (Contains(key))
            {
                for (int i = 1; ; i++)
                {
                    key = $"{key}{i}";
                    if (!Parameters.ContainsKey(key))
                        break;
                }
            }

            Parameters.Add(key, value);

            return key;
        }

        /// <summary>
        /// Set key and value. If key exists the method will overwrite the value.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value</param>
        public void Set(string key, object value)
        {
            key = key.ToLower();

            if (Contains(key))
                Parameters[GetKey(key)] = value;
            else
                Parameters.Add(key, value);
        }

        /// <summary>
        /// Get value.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value to return</param>
        /// <returns>If key exists.</returns>
        public bool Get(string key, out Dynamic value)
        {
            key = key.ToLower();

            if(!Contains(key))
            {
                value = null;
                return false;
            } else
            {
                value = new Dynamic(Parameters[GetKey(key)]);
                return true;
            }
        }

        public Dynamic Get(string key)
        {
            key = key.ToLower();

            if (!Contains(key))
            {
                return Dynamic.Empty;
            }
            else
            {
                return new Dynamic(Parameters[GetKey(key)]);
            }
        }

        /// <summary>
        /// Check if key contains in the parameters.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Parameters.Where(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Any();
        }

        private string GetKey(string key)
        {
            return Parameters.Where(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.Key).FirstOrDefault();
        }
    }
}
