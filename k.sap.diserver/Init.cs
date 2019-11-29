using System;


namespace k.sap.diserver
{
    public class Init : IInit
    {
        private string control = DateTime.Now.ToString("ffffff");
        private string LOG => this.GetType().Name;
        public void Init10_Dependency()
        {
            k.StartInit.Starting(new k.Init());
            k.StartInit.Starting(new k.sap.Init());


        }


        public void Init20_Config()
        {
            #region Check DIServer
            if (!k.win32.WinService.Exists(R.ServiceName))
                throw new KDIServerException(LOG, E.Message.DIServerIsNotExists_1, R.ServiceName);

            if (!k.win32.WinService.IsRunning(R.ServiceName))
                k.win32.WinService.Start(R.ServiceName);
            #endregion
        }

        public void Init50_Threads()
        {

        }

        public void Init60_End()
        {
        }
    }
}
