using System;
using System.Linq;
using System.ServiceProcess;


namespace k.win32
{
    public static class WinService
    {
        private static string LOG => typeof(WinService).Name;

        /// <summary>
        /// Verify if a service exists
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <returns></returns>
        public static bool Exists(string serviceName)
        {
            var foo = ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));
            k.Diagnostic.Debug(LOG, R.Project, foo ? "The {0} service exists." : "The {0} service is not exists", serviceName);
            return foo;
        }

        /// <summary>
        /// Start a service by it's name
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Start(string serviceName)
        {
            k.Diagnostic.Debug(LOG, R.Project, "Trying start the {0} service.", serviceName);

            ServiceController sc = new ServiceController();
            sc.ServiceName = serviceName;

            k.Diagnostic.Debug(LOG, R.Project, "The {0} service is {1}.", sc.DisplayName, sc.Status.ToString());

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    k.Diagnostic.Debug(LOG, R.Project, "The {0} service status is now set to {1}", sc.DisplayName, sc.Status.ToString());
                }
                catch (InvalidOperationException e)
                {
                    var track = k.Diagnostic.Track(e);
                    k.Diagnostic.Error(LOG, track, R.Project, "Could not start the {0} service.", sc.DisplayName);

                    throw new KWin32Exception(LOG, E.Message.CannotStartStopService_2, "start", sc.DisplayName);
                }
            }
            
        }

        /// <summary>
        /// Stop a service that is active
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Stop(string serviceName)
        {
            k.Diagnostic.Debug(LOG, R.Project, "Trying stop the {0} service.", serviceName);

            ServiceController sc = new ServiceController();
            sc.ServiceName = serviceName;

            k.Diagnostic.Debug(LOG, R.Project, "The {0} service is {1}.", sc.DisplayName, sc.Status.ToString());

            if (sc.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    // Display the current service status.
                    k.Diagnostic.Debug(LOG, R.Project, "The {0} service status is now set to {1}", sc.DisplayName, sc.Status.ToString());
                }
                catch (InvalidOperationException e)
                {
                    var track = k.Diagnostic.Track(e);
                    k.Diagnostic.Error(LOG, R.Project, track, "Could not stop the {0} service.", sc.DisplayName);

                    throw new KWin32Exception(LOG, E.Message.CannotStartStopService_2,"stop", sc.DisplayName);
                }
            }            
        }

        /// <summary>
        ///  Verify if a service is running.
        /// </summary>
        /// <param name="serviceName"></param>
        public static bool IsRunning(string serviceName)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = serviceName;

            k.Diagnostic.Debug(LOG, R.Project, "The {0} service is {1}.", sc.DisplayName, sc.Status.ToString());

            return sc.Status == ServiceControllerStatus.Running;
        }

        /// <summary>
        /// Reboots a service
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Reboot(string serviceName)
        {
            k.Diagnostic.Debug(LOG, R.Project, "Trying restart the {0} service.", serviceName);

            if (Exists(serviceName))
            {
                if (IsRunning(serviceName))
                    Stop(serviceName);
                else
                    Start(serviceName);
            }            
        }
    }
}
