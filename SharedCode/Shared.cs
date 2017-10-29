using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SharedCode
{
    public static class Ext
    {
        #region "13.3 List what processes an assembly is loaded in" 
        public static ProcessModuleCollection SafeGetModules(this Process process)
        {
            List<ProcessModule> listModules = new List<ProcessModule>();
            ProcessModuleCollection modules = new ProcessModuleCollection(listModules.ToArray());
            try
            {
                modules = process.Modules;
            }
            catch (InvalidOperationException) { }
            catch (PlatformNotSupportedException) { }
            catch (NotSupportedException) { }
            catch (Win32Exception wex)
            {
                Console.WriteLine($"Couldn't get modules for {process.ProcessName}: {wex.Message}");
            }
            // return either the modules or an empty collection
            return modules;
        }
        #endregion  
    }

    public class Shared
    {
        #region "13.3 List what processes an assembly is loaded in"
        public static void TestAssemblyInProcesses()
        {
            string searchAssm = "mscoree.dll";
            var processes = GetProcessesAssemblyIsLoadedIn(searchAssm);
            foreach (Process p in processes)
                Console.WriteLine($"Found {searchAssm} in {p.MainModule.ModuleName}");
        }

        public static IEnumerable<Process> GetProcessesAssemblyIsLoadedIn(string assemblyFileName)
        {
            // System and Idle are not actually processes so there are not modules associated so we skip those.

            //A first chance exception of type 'System.ComponentModel.Win32Exception' occurred in System.dll
            //Additional information: Access is denied

            // Under normal context
            //Couldn't get modules for dasHost: Access is denied
            //Couldn't get modules for WUDFHost: Access is denied
            //Couldn't get modules for StandardCollector.Service: Access is denied
            //Couldn't get modules for winlogon: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for FcsSas: Access is denied
            //Couldn't get modules for VBCSCompiler: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for coherence: Access is denied
            //Couldn't get modules for coherence: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for MOMService: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for csrss: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for vmms: Access is denied
            //Couldn't get modules for dwm: Access is denied
            //Found mscoree.dll in Microsoft.VsHub.Server.HttpHostx64.exe
            //Couldn't get modules for wininit: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for prl_tools: Access is denied
            //Couldn't get modules for coherence: Access is denied
            //Couldn't get modules for MpCmdRun: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for audiodg: Access is denied
            //Couldn't get modules for mqsvc: Access is denied
            //Couldn't get modules for WmiApSrv: Access is denied
            //Couldn't get modules for conhost: Access is denied
            //Couldn't get modules for sqlwriter: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Found mscoree.dll in CSharpRecipes.exe
            //Couldn't get modules for WmiPrvSE: Access is denied
            //Couldn't get modules for spoolsv: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for WmiPrvSE: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Found mscoree.dll in msvsmon.exe
            //Couldn't get modules for csrss: Access is denied
            //Couldn't get modules for dllhost: Access is denied
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for SearchIndexer: Access is denied
            //Couldn't get modules for WmiPrvSE: Access is denied
            //Found mscoree.dll in VBCSCompiler.exe
            //Couldn't get modules for svchost: Access is denied
            //Couldn't get modules for OSPPSVC: Access is denied
            //Couldn't get modules for WmiPrvSE: Access is denied
            //Couldn't get modules for smss: Access is denied
            //Couldn't get modules for IpOverUsbSvc: Access is denied
            //Couldn't get modules for lsass: Access is denied
            //Couldn't get modules for services: Access is denied
            //Couldn't get modules for MsMpEng: Access is denied
            //Couldn't get modules for msdtc: Access is denied
            //Couldn't get modules for prl_tools_service: Access is denied
            //Couldn't get modules for inetinfo: Access is denied
            //Couldn't get modules for sppsvc: Access is denied

            //Under admin context
            //Found mscoree.dll in VBCSCompiler.exe
            //Found mscoree.dll in Microsoft.VsHub.Server.HttpHostx64.exe
            //Found mscoree.dll in msvsmon.exe
            //Found mscoree.dll in VBCSCompiler.exe
            //Couldn't get modules for audiodg: Access is denied
            //Found mscoree.dll in ElevatedPrivilegeActions.vshost.exe
            //Couldn't get modules for sppsvc: Access is denied

            var processes = from process in Process.GetProcesses()
                            where process.ProcessName != "System" &&
                                    process.ProcessName != "Idle"
                            from ProcessModule processModule in process.SafeGetModules()
                            where processModule.ModuleName.Equals(assemblyFileName, StringComparison.OrdinalIgnoreCase)
                            select process;
            return processes;
        }
        #endregion
    }
}
