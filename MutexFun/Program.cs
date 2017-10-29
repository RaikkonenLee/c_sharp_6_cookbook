using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
//using System.IO;

namespace MutexFun
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable()]
    public struct Contact
    {
	    public string _name;
	    public int _age;
    }

	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
                // create the initial shared memory manager to get things set up
                using(SharedMemoryManager<Contact> sm =
	                new SharedMemoryManager<Contact>("Contacts",8092))
                {
	                // this is the sender process 

	                // launch the second process to get going
	                string processName = Process.GetCurrentProcess().MainModule.FileName;
	                int index = processName.IndexOf("vshost");
	                if (index != -1)
	                {
		                string first = processName.Substring(0, index);
		                int numChars = processName.Length - (index + 7);
		                string second = processName.Substring(index + 7, numChars);

		                processName = first + second;
	                }
                    Process receiver = Process.Start(
                        new ProcessStartInfo(
                            processName,
                            "Receiver"));

                    // give it 5 seconds to spin up
                    Thread.Sleep(5000);

	                // make up a contact
	                Contact man;
	                man._age = 23;
	                man._name = "Dirk Daring";

	                // send it to the other process via shared memory
	                sm.SendObject(man);
                }
			}
			else
			{
				// this is the receiver process

                // create the initial shared memory manager to get things set up
                using(SharedMemoryManager<Contact> sm =
	                new SharedMemoryManager<Contact>("Contacts",8092))
                {

	                // get the contact once it has been sent
	                Contact c = (Contact)sm.ReceiveObject();

	                // Write it out (or to a database...)
	                Console.WriteLine("Contact {0} is {1} years old.",
						                c._name, c._age);

                    // show for 5 seconds
                    Thread.Sleep(5000);
                }
			}

			// Uncomment this if you want to see the console window...
			//Console.ReadLine();
		}
	}
}
