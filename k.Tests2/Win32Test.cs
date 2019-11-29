using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace k.Tests2
{

    [TestClass]
    public class Win32Test
    {
        [TestMethod]
        public void WinService()
        {
            var result = true;
            var serviceName = "SBODI_Server";

            
            result = k.win32.WinService.Exists(serviceName);
            Assert.IsTrue(result);
            result = k.win32.WinService.Exists(serviceName + "!");
            Assert.IsFalse(result);

            try
            {
                result = k.win32.WinService.IsRunning(serviceName);
                k.win32.WinService.Start(serviceName);
                k.win32.WinService.Stop(serviceName);
                k.win32.WinService.Reboot(serviceName);

                result = k.win32.WinService.IsRunning(serviceName);
                Assert.IsTrue(result);
            }
            catch(k.BaseException be)
            {
                Assert.AreEqual(be.Code, 1);
            }

            
        }
    }
}
