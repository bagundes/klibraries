using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public abstract class KModel
    {
        public string Log => this.GetType().Name;
                
        protected virtual Object Clone() 
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(json, this.GetType());
        }

        public virtual string ToJson()
        {
            var formatting = Formatting.None;

            if (R.DebugMode) formatting = Formatting.Indented;

            var json = JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = formatting
                });

            return json;
        }

        public virtual string ToXml()
        {
            throw new NotImplementedException();
        }

        public virtual string Serialize()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object model)
        {
            if (this.GetType() != model.GetType())
                return false;

            var fields1 = k.Reflection.GetFields(this);
            foreach (var f in k.Reflection.GetFields(model))
            {
                if (!fields1.Any(t => t.Name.Equals(f.Name)))
                    return false;

                var field1 = Reflection.GetValue(this, f.Name);
                var field2 = Reflection.GetValue(this, f.Name);
                if (!field1.Equals(field2))
                    return false;
            }

            var props1 = k.Reflection.GetProperties(this);
            foreach (var f in k.Reflection.GetProperties(model))
            {
                if (!props1.Any(t => t.Name.Equals(f.Name)))
                    return false;

                var field1 = Reflection.GetValue(this, f.Name);
                var field2 = Reflection.GetValue(this, f.Name);
                if (!field1.Equals(field2))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var id = 0;
            foreach(var p in k.Reflection.GetProperties(this))
            {
                var v = k.Reflection.GetValue(this, p.Name).ToString();
                for (int i = 0; i < v.Length; i++)
                    id += (int)$"{p.Name}:{v}"[i]; 
            }

            foreach (var p in k.Reflection.GetFields(this))
            {
                var v = k.Reflection.GetValue(this, p.Name).ToString();
                for (int i = 0; i < v.Length; i++)
                    id += (int)$"{p.Name}:{v}"[i];
            }

            return id;
        }
    }
}
