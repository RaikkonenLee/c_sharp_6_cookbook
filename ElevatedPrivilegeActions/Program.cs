using System;
using System.ServiceProcess;

namespace ElevatedPrivilegeActions
{

    class Program
    {
        static void Main(string[] args)
        {
            #region "13.2 Controlling a Service"
            TestServiceManipulation();
            #endregion

            #region "13.3 List what processes an assembly is loaded in"
            SharedCode.Shared.TestAssemblyInProcesses();
            #endregion
        }

        #region "13.2 Controlling a Service"
        public static void TestServiceManipulation()
        {
            ServiceController scStateService = new ServiceController("COM+ Event System");
            Console.WriteLine($"Service Type: {scStateService.ServiceType.ToString()}");
            Console.WriteLine($"Service Name: {scStateService.ServiceName}");
            Console.WriteLine($"Display Name: {scStateService.DisplayName}");

            foreach (ServiceController sc in scStateService.DependentServices)
                Console.WriteLine($"{scStateService.DisplayName} is depended on by: {sc.DisplayName}");

            foreach (ServiceController sc in scStateService.ServicesDependedOn)
                Console.WriteLine($"{scStateService.DisplayName} depends on: {sc.DisplayName}");

            Console.WriteLine($"Status: {scStateService.Status}");
            // save original state
            ServiceControllerStatus originalState = scStateService.Status;

            TimeSpan serviceTimeout = TimeSpan.FromSeconds(60);
            // if it is stopped, start it
            if (scStateService.Status == ServiceControllerStatus.Stopped)
            {
                scStateService.Start();
                // wait up to 60 seconds for start
                scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
            }
            Console.WriteLine($"Status: {scStateService.Status}");

            // if it is paused, continue
            if (scStateService.Status == ServiceControllerStatus.Paused)
            {
                if (scStateService.CanPauseAndContinue)
                {
                    scStateService.Continue();
                    // wait up to 60 seconds for start
                    scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                }
            }
            Console.WriteLine($"Status: {scStateService.Status}");

            // should be running at this point 

            // can we stop it?
            if (scStateService.CanStop)
            {
                // In order to manipulate services, you need administrator access on the machine through User Account Control (UAC)
                // We can request these rights for our code in the app.manifest file associated with the assembly.
                // The default requested execution level is to run as the person invoking the code
                // <requestedExecutionLevel level="asInvoker" uiAccess="false" />
                //If we had not added the following to the app.manifest file:
                //< requestedExecutionLevel level = "requireAdministrator" uiAccess = "false" />
                //You get the following error even though CanStop is true because you are not in an administrative context even if 
                // your account has admin rights:
                //  A first chance exception of type 'System.InvalidOperationException' occurred in System.ServiceProcess.dll
                //  Additional information: Cannot open EventSystem service on computer '.'.
                scStateService.Stop();
                // wait up to 60 seconds for stop
                scStateService.WaitForStatus(ServiceControllerStatus.Stopped, serviceTimeout);
            }
            Console.WriteLine($"Status: {scStateService.Status}");

            // set it back to the original state
            switch (originalState)
            {
                case ServiceControllerStatus.Stopped:
                    if (scStateService.CanStop)
                        scStateService.Stop();
                    break;
                case ServiceControllerStatus.Running:
                    scStateService.Start();
                    // wait up to 60 seconds for start
                    scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                    break;
                case ServiceControllerStatus.Paused:
                    // if it was paused and is stopped, need to restart so we can pause
                    if (scStateService.Status == ServiceControllerStatus.Stopped)
                    {
                        scStateService.Start();
                        // wait up to 60 seconds for start
                        scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                    }
                    // now pause
                    if (scStateService.CanPauseAndContinue)
                    {
                        scStateService.Pause();
                        // wait up to 60 seconds for paused
                        scStateService.WaitForStatus(ServiceControllerStatus.Paused, serviceTimeout);
                    }
                    break;
            }
            scStateService.Refresh();
            Console.WriteLine($"Status: {scStateService.Status.ToString()}");

            // close it
            scStateService.Close();
        }
        #endregion
    }
}
