﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace k.db
{
    public class R
    {
        private static System.Reflection.Assembly Assembly => System.Reflection.Assembly.GetExecutingAssembly();
        public static string CompanyName => k.R.CompanyName;

        public static bool DebugMode => k.R.DebugMode;

        public class Security
        {
            public static string MasterKey => k.R.Security.MasterKey;
        }

        public class App
        {
            public static string Name => "KCoreDB";
            public static string Namespace => "KDB";
            public static Version Version => Assembly.GetName().Version;
            public static int ID => Version.Major;
            public static string Path => k.R.App.Path;

            public static CultureInfo Culture = k.R.App.Culture;
        }
    }
}