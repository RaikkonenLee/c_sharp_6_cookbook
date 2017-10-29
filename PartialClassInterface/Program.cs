using System;

namespace PartialClassInterfaces
{
    class Program
    {
        static void Main()
        {
            TriValue tv = new TriValue(3, 4, 5);
            Console.WriteLine("Average: {0}",tv.Average);
            Console.WriteLine("Sum: {0}", tv.Sum);
            Console.WriteLine("Product: {0}", tv.Product);
            Console.WriteLine("Boolean: {0}", Convert.ToBoolean(tv));
            Console.WriteLine("Byte: {0}", Convert.ToByte(tv));
            Console.WriteLine("Char: {0}", Convert.ToChar(tv));
            Console.WriteLine("Decimal: {0}", Convert.ToDecimal(tv));
            Console.WriteLine("Double: {0}", Convert.ToDouble(tv));
            Console.WriteLine("Int16: {0}", Convert.ToInt16(tv));
            Console.WriteLine("Int32: {0}", Convert.ToInt32(tv));
            Console.WriteLine("Int64: {0}", Convert.ToInt64(tv));
            Console.WriteLine("SByte: {0}", Convert.ToSByte(tv));
            Console.WriteLine("Single: {0}", Convert.ToSingle(tv));
            Console.WriteLine("String: {0}", Convert.ToString(tv));
            Console.WriteLine("Type: {0}", Convert.GetTypeCode(tv));
            Console.WriteLine("UInt16: {0}", Convert.ToUInt16(tv));
            Console.WriteLine("UInt32: {0}", Convert.ToUInt32(tv));
            Console.WriteLine("UInt64: {0}", Convert.ToUInt64(tv));
        }
    }
}
