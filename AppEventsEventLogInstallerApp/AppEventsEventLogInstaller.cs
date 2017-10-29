using System.ComponentModel;
using System.Diagnostics;
using System.Configuration.Install;

namespace AppEventsEventLogInstallerApp
{
    /// <summary>
    /// To INSTALL: C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\CSCB6\AppEventsEventLogInstallerApp\bin\Debug\AppEventsEventLogInstallerApp.dll
    /// To UNINSTALL: C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u C:\CSCB6\AppEventsEventLogInstallerApp\bin\Debug\AppEventsEventLogInstallerApp.dll
    /// </summary>
    [RunInstaller(true)]
    public class AppEventsEventLogInstaller : Installer
    {
        private EventLogInstaller evtLogInstaller;

        public AppEventsEventLogInstaller()
        {
            evtLogInstaller = new EventLogInstaller();
            evtLogInstaller.Source = "APPEVENTSSOURCE";
            evtLogInstaller.Log = ""; // Default to Application 
            Installers.Add(evtLogInstaller);

            evtLogInstaller = new EventLogInstaller();
            evtLogInstaller.Source = "AppLocal";
            evtLogInstaller.Log = "AppLog"; 
            Installers.Add(evtLogInstaller);

            evtLogInstaller = new EventLogInstaller();
            evtLogInstaller.Source = "AppGlobal";
            evtLogInstaller.Log = "AppSystemLog"; 
            Installers.Add(evtLogInstaller);
        }
        public static void Main()
        {
            AppEventsEventLogInstaller appEventsEventLogInstaller = 
                new AppEventsEventLogInstaller();
        }
    }

}