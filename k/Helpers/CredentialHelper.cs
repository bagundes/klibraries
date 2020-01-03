using System;
using System.Collections.Generic;
using System.Text;

namespace k.Helpers
{

    internal static class CredentialHelper
    {
        public static string Path => k.Shell.Directory.AppDataFolder(G.Projects.KCore, "autentications");
        public static string ConvertToEPassword(string password, string user)
        {
            return k.Security.Encrypt(password, user);
        }

        public static string ConvertToPassword(string epassword, string user)
        {
            return k.Security.Decrypt(epassword, user);
        }

        public static void Clear()
        {

            var path = R.App.AppData;
#if DEBUG
            var files = Shell.File.Find(Path, $"*.{E.Extensions.CREDENTIAL}", System.IO.SearchOption.AllDirectories, DateTime.Now.AddDays(-1));
#else
            var files = Shell.File.Find(Path, $"*.{E.Extensions.CREDENTIAL}", System.IO.SearchOption.AllDirectories, DateTime.Now.AddDays(-30));
#endif

            Shell.File.Delete(files);
        }
    }
}
