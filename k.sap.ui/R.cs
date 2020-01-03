using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace k.sap.ui
{
    public class R
    {
        internal static System.Reflection.Assembly Assembly => System.Reflection.Assembly.GetExecutingAssembly();
        public static string CompanyName => k.R.CompanyName;
        public static string Namespace => k.R.Namespace;
        public static k.G.Projects Project => k.G.Projects.KDI;

        public static string CredID => k.db.R.CredID;
        public static bool DebugMode => k.R.DebugMode;

        public class Security
        {
            public static string MasterKey => k.R.Security.MasterKey;
        }

        public class App
        {
            public static string Name => "K Library for SAP UIAPI";
            public static string Namespace => "KUI";
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
