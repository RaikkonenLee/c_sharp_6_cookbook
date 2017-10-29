using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS4014 // await...

namespace ConsoleTCPServer
{
    class Program
    {
        private static MyTcpServer _server;
        private static CancellationTokenSource _cts;

        static void Main()
        {
            _cts = new CancellationTokenSource();
            try
            {
                // We don't await this call as we want to continue so
                // that the Console UI can process keystrokes

                // The method is not marked with a void in the signature per Best Practice recommendations
                // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
                RunServer(_cts.Token);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            string msg = "Press Esc to stop the server...";
            Console.WriteLine(msg);
            ConsoleKeyInfo cki;
            while (true)
            {
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                {
                    _cts.Cancel();
                    _server.StopListening();
                    break; // allow exit
                }
            }
            Console.WriteLine("");
            Console.WriteLine("All done listening");
        }

        private static async Task RunServer(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(async() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _server = new MyTcpServer(IPAddress.Loopback, 55555);
                    // Uncomment to use SSL for the server
                    //_server = new MyTcpServer(IPAddress.Loopback, 55555, "CSharpCookBook.net"); 
                    await _server.ListenAsync(cancellationToken);
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancelled.");
            }
        }
    }
}
