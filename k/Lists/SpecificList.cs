using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k.Lists
{
    /// <summary>
    /// List of specific type of value
    /// </summary>
    /// <typeparam name="TValue">Value</typeparam>
    public class SpecificList<TValue> where TValue : class
    {

        
        private Dictionary<string, TValue> Select = new Dictionary<string, TValue>();

        public TValue this[string key]
        {
            get
            {
                key = key.ToLower();

                if (Contains(key))
                    return Select[key];
                else
                    return null;
            }
        }

        public TValue this[int index]
        {
            get
            {
                var key = Select.Keys.ToList()[index];

                return Select[key];
            }
        }

        /// <summary>
        /// Add new value, if key exists the method will add number to the name (keyN).
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value</param>
        /// <returns>Key name</returns>
        public string Add(string key, TValue value)
        {
            if (Select.ContainsKey(key))
            {
                for(int i = 1; ; i++)
                {
                    key = $"{key}{i}";
                    if (!Select.ContainsKey(key))
                        break;
                }
            }
            
            Select.Add(key, value);

            return key;
        }

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

        public TValue Get(string key)
        {
            if (!Contains(key))
                return null;
            else
                return Select[key];
        }

        public bool Contains(string key)
        {
            return Select.ContainsKey(key);
        }
    }
}
