using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace k
{
    public partial class R
    {
        private static System.Reflection.Assembly Assembly => System.Reflection.Assembly.GetExecutingAssembly();
    }

    public partial class R
    {
        #region ConfigGlobal

        public static string CompanyName => Content.ConfigGlobal.CompanyName;
        public static string Namespace => Content.ConfigGlobal.Namespace;
        public static k.G.Projects Project => k.G.Projects.KCore;

#if DEBUG
        public static bool DebugMode = true;
#else
        public static bool DebugMode = false;
#endif
        #endregion

        private static string projectName;
        public static string ProjectName
        {
            get
            {
                if (String.IsNullOrEmpty(projectName))
                    if (!DebugMode)
                        throw new Exception("It's need add project name in k.R.ProjectName");
                    else
                        return "KTests";
                else
                    return projectName;
                
            }
            set
            {
                if (String.IsNullOrEmpty(projectName))
                {
                    projectName = value;
                    Diagnostic.Debug("ProjectName", null, $"Project name: {projectName}");
                }else
                    Diagnostic.Warning("ProjectName", null, $"It's not possible to change {projectName} project name");
            }            
        }

        public class Security
        {
            public static string MasterKey => P.MasterKey;
        }

        public class App
        {

            public static string AppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{CompanyName}\\{ProjectName}";
            public static string AppTemp => $"{System.IO.Path.GetTempPath()}{CompanyName}\\{ProjectName}";
            public static string Name => "K Library Core";
            public static string Namespace => "KC";
            public static Version Version => Assembly.GetName().Version;
            public static int ID => Version.Major;
            public static string Path = System.Environment.CurrentDirectory;

            public static CultureInfo Culture => CultureInfo.CreateSpecificCulture(Stored.ConfigFile.GetGlobalValue("CultureLanguage"));

            public static string[] Resources => Assembly.GetManifestResourceNames();

            public static string MessageLangFile => R.App.Resources
                .Where(t => t.Contains($"Content.Language.{R.App.Culture.Name}.resource"))
                .FirstOrDefault()
                .Replace(".resources", "");

        }
    }
}
