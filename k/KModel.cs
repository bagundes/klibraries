using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
