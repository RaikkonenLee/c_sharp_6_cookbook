using System;
using System.Windows.Forms;
using System.Threading;

namespace UnhandledThreadException
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Adds the event handler to catch any exceptions that happen in the main UI thread.
            Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);

            // add the event handler for all threads in the appdomain except for the main UI thread
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.Run(new Form1());
        }

        // Handles the exception event for all other threads
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // just show the exception details
            MessageBox.Show("CurrentDomain_UnhandledException: " + e.ExceptionObject.ToString());
        }

        // Handles the exception event from a UI thread.
        static void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            // just show the exception details
            MessageBox.Show("OnThreadException: " + t.Exception.ToString());
        }
    }
}