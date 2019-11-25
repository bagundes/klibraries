using System;
using System.Collections.Generic;
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
                if (Contains(key))
                    return new Dynamic(Parameters[key]);
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
            if (Parameters.ContainsKey(key))
                Parameters[key] = value;
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
            if(!Contains(key))
            {
                value = null;
                return false;
            } else
            {
                value = new Dynamic(Parameters[key]);
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
            return Parameters.ContainsKey(key);
        }
    }
}
