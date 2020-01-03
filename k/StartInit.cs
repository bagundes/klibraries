using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k
{
    public class StartInit
    {
        private static string LOG => typeof(StartInit).FullName;

        public static List<string> Ran { get; internal set; } = new List<string>();

        [Obsolete("Use StartInit.Register", true)]
        public static void Starting(IInit init){}

        public static void Register(IInit init)
        {
            try
            {
                var name = init.GetType().FullName;
                if (Ran.Where(t => t == name).Any())
                    return;

                k.Diagnostic.Debug(LOG, null, "Starting {0} dependencies", name);
                init.Init10_Dependencies();
                k.Diagnostic.Debug(LOG, null, "Starting {0} configs", name);
                init.Init20_Config();
                k.Diagnostic.Debug(LOG, null, "Starting {0} threads ", name);
                init.Init50_Threads();
                k.Diagnostic.Debug(LOG, null, "Registred the {0} library", name);
                Ran.Add(name);
            }catch(Exception ex)
            {
                k.Diagnostic.Error(init, ex);
                throw ex;
            }
        }

    }

    public interface IInit
    {
        void Init10_Dependencies();

        void Init20_Config();

        void Init50_Threads();
        void Init60_End();

    }
}
