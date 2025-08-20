using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nambaya.AEGnordNotifier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {


#if DEBUG
            var hostService = new HostService();
            bool success = hostService.StartService();
            string message = success ? @"Close Nambaya AEGNord Notifier" : "Error at startup!";
            MessageBox.Show(message);
            hostService.StopService();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HostService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
