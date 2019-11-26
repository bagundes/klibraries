using System;
using System.Collections.Generic;
using System.Text;

namespace k.Lists
{
    /// <summary>
    /// List of specific type of value
    /// </summary>
    /// <typeparam name="TValue">Value</typeparam>
    class SelectList<TValue> where TValue : class, IBaseList
    {
        private Dictionary<string, TValue> Select = new Dictionary<string, TValue>();

        public void Set(string key, TValue value)
        {
            if (Select.ContainsKey(key))
                Select[key] = value;
            else
                Select.Add(key, value);
        }

        public bool Get(string key, out TValue value)
        {
            if (!Contains(key))
            {
                value = null;
                return false;
            }
            else
            {
                value = Select[key];
                return true;
            }
        }

        public bool Contains(string key)
        {
            return Select.ContainsKey(key);
        }
    }
}
