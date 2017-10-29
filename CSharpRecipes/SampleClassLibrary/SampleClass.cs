using System;

namespace SampleClassLibrary
{
    public class SampleClass
    {
        public SampleClass()
        {
        }

        public string LastMessage { get; set; } = "Not set yet";
        public bool TestMethod1(string info)
        {
            LastMessage = info;
            Console.WriteLine(info);
            return true;
        }

        public bool TestMethod2(string info, int n)
        {
            LastMessage = info;
            Console.WriteLine($"{info} invoked with {n}");
            return true;
        }
    }
}
