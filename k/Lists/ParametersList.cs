using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k.Lists
{
    /// <summary>
    /// List of parameters, the value can be any object.
    /// </summary>
    public class ParametersList : IBaseList
    {
        private Dictionary<string, object> Parameters = new Dictionary<string, object>();

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
