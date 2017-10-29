using System;
using System.Net;
using System.Threading.Tasks;

namespace ConsoleTCPClient
{
    class Program
    {
        static void Main()
        {
            Task serverChat = TalkToServerAsync();
            serverChat.Wait();
            Console.WriteLine(@"Press the ENTER key to continue...");
            Console.Read();
        }

        private static async Task MakeClientCallToServerAsync(string msg)
        {
            MyTcpClient client = new MyTcpClient(IPAddress.Loopback, 55555);
            // Uncomment to use SSL to talk to the server
            //MyTcpClient client = new MyTcpClient(IPAddress.Loopback, 55555, "CSharpCookBook.net"); 
            await client.ConnectToServerAsync(msg);
        }

        private static async Task TalkToServerAsync()
        {
            await MakeClientCallToServerAsync("Just wanted to say hi");
            await MakeClientCallToServerAsync("Just wanted to say hi again");
            await MakeClientCallToServerAsync("Are you ignoring me?");

            // now send a bunch of messages...
            string msg;
            for (int i = 0; i < 100; i++)
            {
                msg = $"I'll not be ignored! (round {i})";
                RunClientCallAsTask(msg);
            }
        }

        private static void RunClientCallAsTask(string msg)
        {
            Task.Run(async () =>
            {
                await MakeClientCallToServerAsync(msg);
            });
        }
    }

}
