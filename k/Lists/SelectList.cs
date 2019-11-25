using System;
using System.Collections.Generic;
using System.Text;

namespace k.Lists
{
    class SelectList<T> where T : class, IBaseList
    {
        private Dictionary<string, T> Select = new Dictionary<string, T>();

        public void Set(string key, T value)
        {
            if (Select.ContainsKey(key))
                Select[key] = value;
            else
                Select.Add(key, value);
        }

        public bool Get(string key, out T value)
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
