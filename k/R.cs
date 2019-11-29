using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace k
{
    public class R
    {
        private static System.Reflection.Assembly Assembly => System.Reflection.Assembly.GetExecutingAssembly();
        public static string CompanyName => Content.ConfigGlobal.CompanyName;
        public static k.G.Projects Project => k.G.Projects.KCore;

#if DEBUG
        public static bool DebugMode => true;
#else
        public bool DebugMode => k.Content.ConfGlobal.DebugMode == "1";
#endif
        public class Security
        {
            public static string MasterKey => Content.ConfigGlobal.MasterKey;
        }

        public class App
        {
            public static string Name => "K Library Core";
            public static string Namespace => "KC";
            public static Version Version => Assembly.GetName().Version;
            public static int ID => Version.Major;
            public static string Path => System.Environment.CurrentDirectory;

            public static CultureInfo Culture = CultureInfo.CreateSpecificCulture(Content.ConfigGlobal.CultureLanguage);

            public static string[] Resources => Assembly.GetManifestResourceNames();

            public static string MessageLangFile => R.App.Resources
                .Where(t => t.Contains($"Content.Language.{R.App.Culture.Name}.resource"))
                .FirstOrDefault()
                .Replace(".resources", "");

        }
    }
}
