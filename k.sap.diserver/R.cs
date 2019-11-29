using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace k.sap.diserver
{
    public class R
    {
        private static System.Reflection.Assembly Assembly => System.Reflection.Assembly.GetExecutingAssembly();
        public static string CompanyName => k.R.CompanyName;
        public static k.G.Projects Project => k.G.Projects.KDIServer;
        public static bool DebugMode => k.R.DebugMode;
        public static string ServiceName => "SBODI_Server";

        public class Security
        {
            public static string MasterKey => k.R.Security.MasterKey;
        }

        public class App
        {
            public static string Name => "K Library for SAP DIServer";
            public static string Namespace => "KDIServer";
            public static Version Version => Assembly.GetName().Version;
            public static int ID => Version.Major;
            public static string Path => k.R.App.Path;

            public static CultureInfo Culture = k.R.App.Culture;

            public static string[] Resources => Assembly.GetManifestResourceNames();

            public static string MessageLangFile => R.App.Resources
                .Where(t => t.Contains($"Content.Language.{R.App.Culture.Name}.resource"))
                .FirstOrDefault()
                .Replace(".resources", "");
        }
    }
}
