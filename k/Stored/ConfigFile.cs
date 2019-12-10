using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k.Stored
{
    public static class ConfigFile
    {
        public const string CONFIGGlobalFileName = "config.json";
        private static string CONFIGGlobalDevFileName
        {
            get
            {
                var foo = CONFIGGlobalFileName.Split('.');
                var configDevFile = foo[0] + "-dev." + foo[1];
                return configDevFile;
            }
        }
        private static string PATH => k.R.App.Path;

        private static k.Lists.MyList bucket;

        private static void LoadingGlobal()
        {
            if(bucket == null)
            {
                var configfile = k.Shell.File.Find(PATH, CONFIGGlobalFileName, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();

#if DEBUG
                if (k.Shell.File.Find(PATH, CONFIGGlobalDevFileName, System.IO.SearchOption.TopDirectoryOnly).Length < 1)
                {                    
                    System.IO.File.Copy(configfile.FullName, configfile.DirectoryName + "\\" + CONFIGGlobalDevFileName);
                    configfile = new System.IO.FileInfo(configfile.DirectoryName + "\\" + CONFIGGlobalDevFileName);

                }
#endif
                var json = System.IO.File.ReadAllText(configfile.FullName);

                var dic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

                var a = new object();
                bucket = new k.Lists.MyList(dic);
            }
        }

        public static Dynamic GetGlobalValue(string key)
        {
            LoadingGlobal();
#if DEBUG
            if (!bucket.Contains(key))
                SetGlobal(key, null);
#endif

            return bucket[key];

        }

        public static void SetGlobal(string key, object value)
        {
            LoadingGlobal();
            bucket.Set(key, value);
            var json = bucket.ToJson();

            var configfile = k.Shell.File.Find(PATH, CONFIGGlobalFileName, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();

#if DEBUG
            
            configfile = new System.IO.FileInfo(PATH + "\\" + CONFIGGlobalDevFileName);
#endif

            k.Shell.File.Save(bucket.ToJson(), configfile.FullName, true);
        }
    }
}
