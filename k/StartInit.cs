using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k
{
    public class StartInit
    {

        public static List<string> Ran { get; internal set; } = new List<string>();


        public static void Starting(IInit init)
        {
            var name = init.GetType().FullName;
            if (Ran.Where(t => t == name).Any())
                return;


            init.Init10_Dependency();
            init.Init20_Config();
            init.Init50_Threads();

            k.Diagnostic.Debug("StartInit", E.Projects.KCore, "Registred the library {0}", name);
            Ran.Add(name);
        }

    }

    public interface IInit
    {
        void Init10_Dependency();

        void Init20_Config();

        void Init50_Threads();
        void Init60_End();

    }
}
