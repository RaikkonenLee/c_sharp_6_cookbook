using System;
using System.Text;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace NamedPipes
{
    class NamedPipeServerConsole
    {
        static void Main()
        {
            Task server = RunServer();
            server.Wait();

            // make our server hang around so you can see the messages sent
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static async Task RunServer()
        {
            Console.WriteLine("Initiating server, waiting for client...");
            // Start up our named pipe in message mode and close the pipe
            // when done.
            using (NamedPipeServerStream serverPipe = new
                    NamedPipeServerStream("mypipe", PipeDirection.InOut, 1,
                    PipeTransmissionMode.Message, PipeOptions.None))
            {
                // wait for a client...
                serverPipe.WaitForConnection();

                // process messages until the client goes away
                while (serverPipe.IsConnected)
                {
                    int bytesRead = 0;
                    byte[] messageBytes = new byte[256];
                    // read until we have the message then respond
                    do
                    {
                        // build up the client message
                        StringBuilder message = new StringBuilder();

                        // check that we can read the pipe
                        if (serverPipe.CanRead)
                        {
                            // loop until the entire message is read
                            do
                            {
                                bytesRead =
                                    await serverPipe.ReadAsync(messageBytes, 0, 
                                                    messageBytes.Length);

                                // got bytes from the stream so add them to the message
                                if (bytesRead > 0)
                                {
                                    message.Append(
                                        Encoding.Unicode.GetString(messageBytes, 0, bytesRead));
                                    Array.Clear(messageBytes, 0, messageBytes.Length);
                                }
                            }
                            while (!serverPipe.IsMessageComplete);
                        }

                        // if we got a message, write it out and respond
                        if (message.Length > 0)
                        {
                            // set to zero as we have read the whole message
                            bytesRead = 0;
                            Console.WriteLine($"Received message: {message.ToString()}");

                            // return the message text we got from the 
                            // client in reverse
                            char[] messageChars =
                                message.ToString().Trim().ToCharArray();
                            Array.Reverse(messageChars);
                            string reversedMessageText = new string(messageChars);

                            // show the return message
                            Console.WriteLine($"    Returning Message: {reversedMessageText}");

                            // write the response
                            messageBytes = Encoding.Unicode.GetBytes(messageChars);
                            if (serverPipe.CanWrite)
                            {
                                // write the message
                                await serverPipe.WriteAsync(messageBytes, 0, messageBytes.Length);
                                // flush the buffer
                                await serverPipe.FlushAsync();
                                // wait till read by client
                                serverPipe.WaitForPipeDrain();
                            }
                        }
                    }
                    while (bytesRead != 0);
                }
            }

        }
    }
}
