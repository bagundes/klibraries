using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using k.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace k.Lists
{
    public partial class MyList
    {
        private string LOG => this.GetType().Name;
        private const int DFLTSIZE = 10;

        private List<Models.Entry> bucket;

        public Dynamic this[int index, string key]
        {
            get => bucket.Where(t => t.Key.Equals(key.ToLower()) && t.Index == index).Select(t => t.Value).FirstOrDefault();
            set => Set(key, value, index);
        }

        public Dynamic this[string key]
        {
            get => bucket.Where(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.Value).FirstOrDefault();
            set => Set(key, value);
        }

        /// <summary>
        /// Return line
        /// </summary>
        /// <param name="index">Line number</param>
        /// <returns></returns>
        public Dictionary<string, Dynamic> this[int index] => bucket.Where(t => t.Index == index).ToDictionary(t => t.Key,t => t.Value);

        #region Quantity
        /// <summary>
        /// Count quantity rows
        /// </summary>
        public int CountRows => bucket == null ? 0 : bucket.GroupBy(t => t.Index).Count();



        /// <summary>
        /// Count quantity columns
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int CountColumns(int index) => bucket == null ? 0 : bucket.Where(t => t.Index == index).Count();

        public bool HasValues => bucket == null ? false : CountRows > 0;
        #endregion

        #region capacity
        private int capacity = -1;
        public int? Capacity => capacity != -1 ? (int?)capacity : null;
        #endregion

        #region list
        public IEnumerable<string> Keys => bucket == null ? null :  bucket.Select(t => t.Key);

        public IEnumerable<Dynamic> Values => bucket == null ? null : bucket.Select(t => t.Value);

        #endregion

        #region size
        private int size = -1;

        public int? Size => size == -1 ? null: (int?)size;

        #endregion

        public bool IsFixedSize { get; internal set; } = false;
        public bool IsReadOnly => false;

        #region construct
        public MyList() 
        {
            bucket = new List<Entry>();
        }

        public MyList(Dictionary<string, Dynamic> dictionary) 
        {
            capacity = 1;
            size = dictionary.Count;

            Set(dictionary, 0);
        }

        public MyList(Dictionary<string, object> dictionary) 
        {
            bucket = new List<Entry>();

            foreach (var val in dictionary)
                bucket.Add(new Entry { Index = 0, Key = val.Key, Value = new Dynamic(val.Value) });
        }

        public MyList(int size, int capacity)
        {
            this.capacity = capacity;
            this.size = size;
            bucket = new List<Entry>(size * capacity);
        }

        public MyList(int capacity)
        {
            this.capacity = capacity;
            bucket = new List<Entry>();
        }
        #endregion

        #region Bucket
        private void AddForceBucket(string key, object value, int index)
        {
            var i = 0;
            while (Contains(index, key))
                key += $"_{i++}";

            AddBucket(key, value, index);
        }

        private bool AddBucket(string key, object value, int index)
        {
            key = key.ToLower();
            if (Contains(index, key))
                return false;
            else
            {
                HasSpace(index);
                bucket.Add(new Entry { Index = index, Key = key, Value = new Dynamic(value) });
                return true;
            }                                
        }

        private void SetBucket(string key, object value, int index)
        {
            key = key.ToLower();
            if (Contains(index, key))
            {
                var i = GetRealIndex(index, key);
                var foo = bucket[i];
                foo.Value = new Dynamic(value);
                bucket[i] = foo;
            }
            else
            {
                HasSpace(index);
                bucket.Add(new Entry { Index = index, Key = key, Value = new Dynamic(value) });
            }
        }

        private int GetRealIndex(int index, string key)
        {
            return bucket.FindIndex(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) && t.Index == index);
        }
        private bool HasSpace(int index)
        {
            if(bucket == null)
                bucket = new List<Entry>();
            
            if (Size.HasValue && Size >= CountColumns(index))
                throw new KException(LOG, E.Message.BucketSizeLimite_1, Size.Value);

            if (Capacity.HasValue && Capacity >= CountRows)
                throw new KException(LOG, E.Message.BucketCapacityLimite_1, Capacity.Value);

            return true;
        }
        #endregion

        #region Add
        public void AddForce(string key, object value, int index = 0)
        {
            AddForceBucket(key, value, index);
        }

        public bool Add(string key, object value, int index = 0)
        {
            return AddBucket(key, value, index);
        }

        #endregion

        #region Set
        public void Set(MyList myList, int position)
        {

            for (int r = 0; r < myList.CountRows; r++)
            {
                foreach (var entry in myList[0])
                    Set(entry.Key, entry.Value, position);
                position++;
            }
        }

        public void Set(Dictionary<string, Dynamic> dictionary,int index)
        {
            foreach (var val in dictionary)
                bucket.Add(new Entry { Index = index, Key = val.Key, Value = val.Value });
        }

        public void Set(string key, object value, int index = 0)
        {
            SetBucket(key, value, index);
        }

        /// <summary>
        /// Add row
        /// </summary>
        /// <param name="myList"></param>
        
        #endregion
        public bool Get(string key, out Dynamic val)
        {
            return Get(0, key, out val);
        }

        public bool Get(int index, string key, out Dynamic val)
        {
            if (Contains(index, key))
            {
                val = bucket.Where(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) && t.Index == index).Select(t => t.Value).First();
                return true;
            } else
            {
                val = Dynamic.Empty;
                return false;
            }
        }

        /// <summary>
        /// Verify if exists key in the rows
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return bucket.Where(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Any();
        }

        public bool Contains(int index, string key)
        {
            key = key.ToLower();
            return bucket != null && bucket.Where(t => t.Index == index && t.Key.Equals(key)).Any();
        }

        public void Clear()
        {
            bucket = new List<Entry>();
        }

        internal string ToJson()
        {

            if(CountRows == 1)
            {
                var formatting = Formatting.Indented;

                var dic = bucket.Where(t => t.Index == 0).ToDictionary(t => t.Key, t => t.Value.ToObject());


                var json = JsonConvert.SerializeObject(dic,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Formatting = formatting
                    });

                return json;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

    }
}
