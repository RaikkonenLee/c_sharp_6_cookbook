using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.Remoting;
using Microsoft.Win32;
using System.Messaging;

namespace CSharpRecipes
{
	public static class Toolbox
	{
		#region "13.1 Dealing with Operating System Shutdown, Power Management, or User Session Changes"
        public static void PreventBadShutdown()
        {
            RegisterForSystemEvents();

            // change power mode
            // change session stuff

            UnregisterFromSystemEvents();
        }

        public static void RegisterForSystemEvents()
        {
            // always get the final notification when the event thread is shutting down 
            // so we can unregister
            SystemEvents.EventsThreadShutdown += 
                new EventHandler(OnEventsThreadShutdown);
            SystemEvents.PowerModeChanged +=
                new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch +=
                new SessionSwitchEventHandler(OnSessionSwitch);
            SystemEvents.SessionEnding +=
                new SessionEndingEventHandler(OnSessionEnding);
            SystemEvents.SessionEnded +=
                new SessionEndedEventHandler(OnSessionEnded);
        }

        private static void UnregisterFromSystemEvents()
        {
            SystemEvents.EventsThreadShutdown -= 
                new EventHandler(OnEventsThreadShutdown);
            SystemEvents.PowerModeChanged -=
                new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch -=
                new SessionSwitchEventHandler(OnSessionSwitch);
            SystemEvents.SessionEnding -=
                new SessionEndingEventHandler(OnSessionEnding);
            SystemEvents.SessionEnded -=
                new SessionEndedEventHandler(OnSessionEnded);
        }

        private static void OnEventsThreadShutdown(object sender, EventArgs e)
        {
            Debug.WriteLine("System event thread is shutting down, no more notifications.");

            // Unregister all our events as the notification thread is going away
            UnregisterFromSystemEvents();
        }

        private static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            // power mode is changing
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    Debug.WriteLine("PowerMode: OS is resuming from suspended state");
                    break;
                case PowerModes.StatusChange:
                    Debug.WriteLine("PowerMode: There was a change relating to the power" + 
                        " supply (weak battery, unplug, etc..)");
                    break;
                case PowerModes.Suspend:
                    Debug.WriteLine("PowerMode: OS is about to be suspended");
                    break;
            }
        }

        private static void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // check reason
            switch (e.Reason)
            {
                case SessionSwitchReason.ConsoleConnect:
                    Debug.WriteLine("Session connected from the console");
                    break;
                case SessionSwitchReason.ConsoleDisconnect:
                    Debug.WriteLine("Session disconnected from the console");
                    break;
                case SessionSwitchReason.RemoteConnect:
                    Debug.WriteLine("Remote session connected");
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    Debug.WriteLine("Remote session disconnected");
                    break;
                case SessionSwitchReason.SessionLock:
                    Debug.WriteLine("Session has been locked");
                    break;
                case SessionSwitchReason.SessionLogoff:
                    Debug.WriteLine("User was logged off from a session");
                    break;
                case SessionSwitchReason.SessionLogon:
                    Debug.WriteLine("User has logged on to a session");
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    Debug.WriteLine("Session changed to or from remote status");
                    break;
                case SessionSwitchReason.SessionUnlock:
                    Debug.WriteLine("Session has been unlocked");
                    break;
            }
        }

        private static void OnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            // true to cancel the user request to end the session, false otherwise
            e.Cancel = false;
            // check reason
            switch(e.Reason)
            {
                case SessionEndReasons.Logoff:
                    Debug.WriteLine("Session ending as the user is logging off");
                    break;
                case SessionEndReasons.SystemShutdown:
                    Debug.WriteLine("Session ending as the OS is shutting down");
                    break;
            }
        }

        private static void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    Debug.WriteLine("Session ended as the user is logging off");
                    break;
                case SessionEndReasons.SystemShutdown:
                    Debug.WriteLine("Session ended as the OS is shutting down");
                    break;
            }
        }

        #endregion

        #region "13.2 Controlling a Service"
        // See Recipe 13.2 in the ElevatedPrivilegeActions assembly
        #endregion

        #region "13.3 List what processes an assembly is loaded in"
        // See Shared.TestAssemblyInProcesses();
        #endregion

        #region "13.4 Using Message Queues on a local workstation"
        public static void TestMessageQueue()
        {
            // NOTE: Message Queue services must be set up for this to work
            // This can be added in Add/Remove Windows Components
            try
            {
                using (MQWorker mqw = new MQWorker(@".\private$\MQWorkerQ")) // workstation syntax
                //using (MQWorker mqw = new MQWorker(@".\MQWorkerQ")) // server syntax
                {
                    string xml = "<MyXml><InnerXml location=\"inside\"/></MyXml>";
                    Console.WriteLine("Sending message to message queue: " + xml);
                    mqw.SendMessage("Label for message", xml);
                    string retXml = mqw.ReadMessage();
                    Console.WriteLine("Read message from message queue: " + retXml);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #region MQWorker class
        class MQWorker : IDisposable
        {
            private bool _disposed;
            private string _mqPathName;
            MessageQueue _queue;

            public MQWorker(string queuePathName)
            {
                if (string.IsNullOrEmpty(queuePathName))
                    throw new ArgumentNullException(nameof(queuePathName));

                _mqPathName = queuePathName;

                SetUpQueue();
            }

            private void SetUpQueue()
            {
                // See if the queue exists, create it if not
                if (!MessageQueue.Exists(_mqPathName))
                {
                    try
                    {
                        // If you are seeing:
                        //A first chance exception of type 'System.Messaging.MessageQueueException' occurred in System.Messaging.dll
                        //Additional information: External component has thrown an exception.
                        // Go back to TestMessageQueue and use Workstation Syntax, not Server Syntax if you are running locally on 
                        // a non server OS
                        _queue = MessageQueue.Create(_mqPathName);
                    }
                    catch (MessageQueueException mqex)
                    {
                        // see if we are running on a workgroup computer
                        if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.UnsupportedOperation)
                        {
                            string origPath = _mqPathName;
                            // must be a private queue in workstation mode
                            int index = _mqPathName.ToLowerInvariant().
                                            IndexOf("private$", StringComparison.OrdinalIgnoreCase);
                            if (index == -1)
                            {
                                // get the first \
                                index = _mqPathName.IndexOf(@"\", StringComparison.OrdinalIgnoreCase);
                                // insert private$\ after server entry
                                _mqPathName = _mqPathName.Insert(index + 1, @"private$\");
                                // try try again
                                try
                                {
                                    if (!MessageQueue.Exists(_mqPathName))
                                        _queue = MessageQueue.Create(_mqPathName);
                                    else
                                        _queue = new MessageQueue(_mqPathName);
                                }
                                catch (Exception)
                                {
                                    // set original as inner exception
                                    throw new Exception($"Failed to create message queue with {origPath}" +
                                        $" or {_mqPathName}", mqex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _queue = new MessageQueue(_mqPathName);
                }
            }

            public void SendMessage(string label, string body)
            {
                Message msg = new Message();
                // label our message
                msg.Label = label;
                // override the default XML formatting with binary
                // as it is faster (at the expense of legibility while debugging)
                msg.Formatter = new BinaryMessageFormatter();
                // make this message persist (causes message to be written
                // to disk)
                msg.Recoverable = true;
                msg.Body = body;
                _queue?.Send(msg);
            }

            public string ReadMessage()
            {
                Message msg = null;
                msg = _queue.Receive();
                msg.Formatter = new BinaryMessageFormatter();
                return (string)msg.Body;
            }


            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!this._disposed)
                {
                    if (disposing)
                        _queue.Dispose();

                    _disposed = true;
                }
            }
            #endregion
        }
        #endregion

        #endregion

        #region "13.5 Capturing Output from the Standard Output Stream"
        public static void TestRedirectOutput()
        {
            try 
            {
                Console.WriteLine("Stealing standard output!");
                string logfile = Path.GetTempFileName();
                Console.WriteLine($"Logging to: {logfile}");
                using (StreamWriter writer = new StreamWriter(logfile))
                {
                    // steal stdout for our own purposes...
                    Console.SetOut(writer);

                    Console.WriteLine("Writing to the console... NOT!");

                    for (int i = 0; i < 10; i++)
                        Console.WriteLine(i);
                }
            }
            catch(IOException e) 
            {
                Debug.WriteLine(e.ToString());
                return ;            
            }

            // Recover the standard output stream so that a 
            // completion message can be displayed.
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("Back to standard output!");
        }
        #endregion

        #region "13.6 Capture output from a running process"
        public static void TestCaptureOutput()
        {
            Process application = new Process();
            // run the command shell
            application.StartInfo.FileName = @"cmd.exe";

            // get a directory listing from the current directory
            application.StartInfo.Arguments = @"/Cdir " + Environment.CurrentDirectory;
            Console.WriteLine($"Running cmd.exe with arguments: {application.StartInfo.Arguments}");

            // redirect standard output so we can read it
            application.StartInfo.RedirectStandardOutput = true;
            application.StartInfo.UseShellExecute = false;
            
            // Create a log file to hold the results in the current EXE directory
            string logfile = Path.GetTempFileName();
            Console.WriteLine($"Logging to: {logfile}");
            using (StreamWriter logger = new StreamWriter(logfile))
            {
                // start it up
                application.Start();

                application.WaitForExit();

                string output = application.StandardOutput.ReadToEnd();

                logger.Write(output);
            }

            // close the process object
            application.Close();

        }
        #endregion

        #region "13.7 Running Code in Its Own AppDomain"
        public class RunMe : MarshalByRefObject
        {
            public RunMe()
            {
                PrintCurrentAppDomainName();
            }

            public void PrintCurrentAppDomainName()
            {
                string name = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Hello from {name}!");
            }
        }
        public static void RunCodeInNewAppDomain()
        {
            AppDomain myOwnAppDomain = AppDomain.CreateDomain("MyOwnAppDomain");
            // print out our current AppDomain name
            RunMe rm = new RunMe();
            rm.PrintCurrentAppDomainName();

            // Create our RunMe class in the new appdomain
            Type adType = typeof(RunMe);
            ObjectHandle objHdl =
                myOwnAppDomain.CreateInstance(adType.Module.Assembly.FullName, adType.FullName);
            // unwrap the reference
            RunMe adRunMe = (RunMe)objHdl.Unwrap();

            // make a call on the toolbox
            adRunMe.PrintCurrentAppDomainName();

            // now unload the appdomain
            AppDomain.Unload(myOwnAppDomain);
        }

		#endregion

		#region "13.8 Determining the Operating System and Service Pack Version of the current OS"
        public static string GetOSAndServicePack()
        {
            // Get the current OS info
            OperatingSystem os = Environment.OSVersion;
            RegistryKey rk = 
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string osText = (string)rk?.GetValue("ProductName");
            if (string.IsNullOrWhiteSpace(osText))
                osText = os.VersionString;
            else
                osText = ($"{osText} {os.Version.Major}.{os.Version.Minor}.{os.Version.Build}");
            if (!string.IsNullOrWhiteSpace(os.ServicePack))
                osText = $"{osText} {os.ServicePack}";
            return osText;
        }

        #endregion
    }
}
