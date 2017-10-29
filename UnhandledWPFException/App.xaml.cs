using System;
using System.Windows;
using System.Diagnostics;

namespace UnhandledWPFException
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // indicate we handled it
            e.Handled = true;
            ReportUnhandledException(e.Exception);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // WPF UI Exceptions
            this.DispatcherUnhandledException += 
                new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(
                    Application_DispatcherUnhandledException);

            // Those dirty thread exceptions
            AppDomain.CurrentDomain.UnhandledException += 
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException); 
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportUnhandledException(e.ExceptionObject as Exception);
        }

        private void ReportUnhandledException(Exception ex)
        {
            // Log the exception information in the event log
            EventLog.WriteEntry("UnhandledWPFException Application",
                ex.ToString(), EventLogEntryType.Error);
            // Let the user know what happenned
            MessageBox.Show("Unhandled Exception: " + ex.ToString());
            // shut down the application
            this.Shutdown();
        }
    }
}
