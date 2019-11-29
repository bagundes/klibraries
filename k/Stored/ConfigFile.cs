using System;
using System.Collections.Generic;
using System.Text;

namespace k.Stored
{
    public static class ConfigFile
    {
        private static string LOG => typeof(ConfigFile).Name;

        private static Lists.GenericList Params = new Lists.GenericList();

        //internal static void Loading()
        //{
        //    //if (k.Shell.File.Find())


        //    //    var json = k.Shell.File.Load();
        //}



        //public PropertiesList_v1(string file)
        //{
        //    var json = System.IO.File.ReadAllText(file);
        //    properties = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
        //}

        ///// <summary>
        ///// Get the parameter value
        ///// </summary>
        ///// <param name="key">Key</param>
        ///// <param name="msgerror">Create exception when the Key is not exists</param>
        ///// <returns></returns>
        //public Dynamic Get(string key, string msgerror = null)
        //{

        //    if (properties != null && properties.Where(t => t.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)).Any())
        //        return new Dynamic(properties.Where(t => t.Key.ToUpper() == key.ToUpper()).Select(t => t.Value).FirstOrDefault());
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(msgerror))
        //            throw new BaseException(LOG, C.MessageEx.StoredCacheError10_1, msgerror);
        //        else
        //            return Dynamic.Empty;
        //    }
        //}

        //public Dictionary<string, dynamic> GetList()
        //{
        //    return new Dictionary<string, dynamic>(properties);
        //}

        //public void Set(string key, dynamic value)
        //{
        //    if (properties == null)
        //        properties = new Dictionary<string, dynamic>();

        //    if (properties.Where(t => t.Key.ToUpper() == key.ToUpper()).Any())
        //        properties[key] = value;
        //    else
        //        properties.Add(key, value);
        //}

        //public bool ContainsKey(string key)
        //{
        //    return properties.ContainsKey(key);
        //}
    }
}
