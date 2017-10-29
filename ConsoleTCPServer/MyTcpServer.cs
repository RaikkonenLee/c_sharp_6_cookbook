using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleTCPServer
{
    class MyTcpServer
    {
        #region Private Members
        private TcpListener _listener;
        private object _syncRoot = new object();
        #endregion

        #region CTORs

        public MyTcpServer(IPAddress address, int port, string sslServerName = null)
        {
            Port = port;
            Address = address;
            SSLServerName = sslServerName;
        }
        #endregion // CTORs

        #region Properties
        public IPAddress Address { get;  }

        public int Port { get; }

        public bool Listening { get; private set; }

        public string SSLServerName { get; }
        #endregion

        #region Public Methods
        public async Task ListenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                lock (_syncRoot)
                {
                    _listener = new TcpListener(Address, Port);

                    // fire up the server
                    _listener.Start();

                    // set listening bit
                    Listening = true;
                }

                // Enter the listening loop.
                do
                {
                    Console.Write("Looking for someone to talk to... ");
                    // Wait for connection
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Run(async () =>
                        {
                            TcpClient newClient = await _listener.AcceptTcpClientAsync();
                            Console.WriteLine("Connected to new client");
                            await ProcessClientAsync(newClient, cancellationToken);
                        },cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // the user cancelled
                        Listening = false;
                    }
                }
                while (Listening);
            }
            catch (SocketException se)
            {
                Console.WriteLine($"SocketException: {se}");
            }
            finally
            {
                // shut it down
                StopListening();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        public void StopListening()
        {
            if (Listening)
            {
                lock (_syncRoot)
                {
                    // set listening bit
                    Listening = false;
                    try
                    {
                        // shut it down if it is listening
                        if (_listener.Server.IsBound)
                            _listener.Stop();
                    }
                    catch (ObjectDisposedException)
                    {
                        // if we try to stop listening while waiting
                        // for a connection in AcceptTcpClientAsync (since it blocks)
                        // it will throw an ObjectDisposedException here
                        // Since we know in this case we are shutting down anyway
                        // just note that we cancelled
                        Console.WriteLine("Cancelled the listener");
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private async Task ProcessClientAsync(TcpClient client, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                // Buffer for reading data
                byte[] bytes = new byte[1024];
                StringBuilder clientData = new StringBuilder();

                Stream stream = null;
                if (!string.IsNullOrWhiteSpace(SSLServerName))
                {
                    Console.WriteLine($"Talking to client over SSL using {SSLServerName}");
                    SslStream sslStream = new SslStream(client.GetStream());
                    sslStream.AuthenticateAsServer(GetServerCert(SSLServerName), false, SslProtocols.Default, true);
                    stream = sslStream;
                }
                else
                {
                    Console.WriteLine("Talking to client over regular HTTP");
                    stream = client.GetStream();
                }
                // get the stream to talk to the client over
                using (stream)
                {
                    // set initial read timeout to 1 minute to allow for connection
                    stream.ReadTimeout = 60000;
                    // Loop to receive all the data sent by the client.
                    int bytesRead = 0;
                    do
                    {
                        // THIS SEEMS LIKE A BUG, but it apparently isnt...
                        // When we use Read, the first time it works fine, and then on the second
                        // read when there is no data the IOException is thrown for the timeout 
                        // resulting from the 1 second timeout set on the NetworkStream.
                        // If we use ReadAsync, it just hangs forever when there is no data on the 
                        // second read.  This is because timeouts are ignored on the Socket class when
                        // Async is used
                        try
                        {
                            // We use Read here and not ReadAsync as if you call ReadAsync
                            // it will not timeout as you might expect (see note above)
                            bytesRead = stream.Read(bytes, 0, bytes.Length);
                            if (bytesRead > 0)
                            {
                                // Translate data bytes to an ASCII string and append
                                clientData.Append(
                                    Encoding.ASCII.GetString(bytes, 0, bytesRead));
                                // decrease read timeout to 1/2 second now that data is coming in
                                stream.ReadTimeout = 1000;
                            }
                        }
                        catch (IOException ioe)
                        {
                            // read timed out, all data has been retrieved
                            Trace.WriteLine($"Read timed out: {ioe}");
                            bytesRead = 0;
                        }
                    }
                    while (bytesRead > 0);

                    Console.WriteLine($"Client says: {clientData}");

                    // Thank them for their input
                    bytes = Encoding.ASCII.GetBytes("Thanks call again!");

                    // Send back a response.
                    await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                }
            }
            finally
            {
                // stop talking to client
                client?.Close();
            }
        }

        private static X509Certificate GetServerCert(string subjectName)
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                X509CertificateCollection certificate = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

                if (certificate.Count > 0)
                    return (certificate[0]);
                else
                    return (null);
            }
        }
        #endregion
    }
}
