using System;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CSharpRecipes
{
    public static class ClassesExtensionMethods
    {
        public static IEnumerable<T> EveryOther<T>(this IEnumerable<T> enumerable)
        {
            bool retNext = true;
            foreach (T t in enumerable)
            {
                if (retNext) yield return t;
                retNext = !retNext;
            }
        }

        public static void DisplayStocks(this IEnumerable<ClassesAndGenerics.Stock> stocks)
        {
            var gainLoss = from stock in stocks
                           select new
                           {
                               Result = stock.GainLoss < 0 ? "lost" : "gained",
                               stock.Ticker,
                               stock.GainLoss
                           };

            foreach (var s in gainLoss)
            {
                Console.WriteLine("  ({0}) {1} {2}%", s.Ticker, s.Result,
                    System.Math.Abs(s.GainLoss));
            }
        }

        public static void Repeat<T>(this List<T> list, T obj, int count)
        {
            if (count < 0)
            {
                throw (new ArgumentException("The count parameter must be greater or equal to zero."));
            }

            for (int index = 0; index < count; index++)
            {
                list.Add(obj);
            }
        }
    }

    public class ClassesAndGenerics
    {
        #region "1.1 Creating Union Type Structures"
        public static void TestUnions()
        {
            Console.WriteLine("\r\n\r\n");
            SignedNumber sNum = new SignedNumber();
            sNum.Num1 = sbyte.MaxValue;
            Console.WriteLine("Num1 = " + sNum.Num1);
            Console.WriteLine("Num2 = " + sNum.Num2);
            Console.WriteLine("Num3 = " + sNum.Num3);
            Console.WriteLine("Num4 = " + sNum.Num4);

            sNum.Num4 = long.MaxValue;
            Console.WriteLine("\r\nNum1 = " + sNum.Num1);
            Console.WriteLine("Num2 = " + sNum.Num2);
            Console.WriteLine("Num3 = " + sNum.Num3);
            Console.WriteLine("Num4 = " + sNum.Num4);
            Console.WriteLine("Num5 = " + sNum.Num5);
            Console.WriteLine("Num6 = " + sNum.Num6);
            // ????           Console.WriteLine("Num7 = " + sNum.Num7);


            Console.WriteLine("\r\n\r\n");
            SignedNumberWithText sNumWithText = new SignedNumberWithText();
            sNumWithText.Num1 = sbyte.MaxValue;
            sNumWithText.Num2 = short.MaxValue;
            sNumWithText.Num3 = int.MaxValue;
            sNumWithText.Num5 = float.MaxValue;
            sNumWithText.Num6 = double.MaxValue;
            sNumWithText.Num4 = long.MaxValue;
            sNumWithText.Text1 = "ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";
            //sNumWithText.Text2 = "asdfasdf";

            sNumWithText.Num1 = sbyte.MaxValue;
            Console.WriteLine("Num1 = " + sNumWithText.Num1);
            Console.WriteLine("Num2 = " + sNumWithText.Num2);
            Console.WriteLine("Num3 = " + sNumWithText.Num3);
            Console.WriteLine("Num4 = " + sNumWithText.Num4);
            Console.WriteLine("Text1 = " + sNumWithText.Text1);
            //Console.WriteLine("Text2 = " + sNumWithText.Text2);

            sNumWithText.Num4 = long.MaxValue;
            Console.WriteLine("\r\nNum1 = " + sNumWithText.Num1);
            Console.WriteLine("Num2 = " + sNumWithText.Num2);
            Console.WriteLine("Num3 = " + sNumWithText.Num3);
            Console.WriteLine("Num4 = " + sNumWithText.Num4);
            Console.WriteLine("Num5 = " + sNumWithText.Num5);
            Console.WriteLine("Num6 = " + sNumWithText.Num6);
            // ????           Console.WriteLine("Num7 = " + sNumWithText.Num7);
            Console.WriteLine("Text1 = " + sNumWithText.Text1);
            //Console.WriteLine("Text2 = " + sNumWithText.Text2);
        }


        [StructLayoutAttribute(LayoutKind.Explicit)]
        struct SignedNumber
        {
            [FieldOffsetAttribute(0)]
            public sbyte Num1;

            [FieldOffsetAttribute(0)]
            public short Num2;

            [FieldOffsetAttribute(0)]
            public int Num3;

            [FieldOffsetAttribute(0)]
            public long Num4;

            [FieldOffsetAttribute(0)]
            public float Num5;

            [FieldOffsetAttribute(0)]
            public double Num6;

            // ????           [FieldOffsetAttribute(0)]
            //            public decimal Num7;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Text1")]
        [StructLayoutAttribute(LayoutKind.Explicit)]
        struct SignedNumberWithText
        {
            [FieldOffsetAttribute(0)]
            public sbyte Num1;

            [FieldOffsetAttribute(0)]
            public short Num2;

            [FieldOffsetAttribute(0)]
            public int Num3;

            [FieldOffsetAttribute(0)]
            public long Num4;

            [FieldOffsetAttribute(0)]
            public float Num5;

            [FieldOffsetAttribute(0)]
            public double Num6;

            // ????           [FieldOffsetAttribute(0)]
            //            public decimal Num7;

            [FieldOffsetAttribute(16)]
            public string Text1;
        }
        #endregion

        #region "1.2 Making a Type Sortable"
        public static void TestSort()
        {
            List<Square> listOfSquares = new List<Square>(){
                                        new Square(1,3),
                                        new Square(4,3),
                                        new Square(2,1),
                                        new Square(6,1)};

            // Test a List<String>
            Console.WriteLine("List<String>");
            Console.WriteLine("Original list");
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }


            Console.WriteLine();
            IComparer<Square> heightCompare = new CompareHeight();
            listOfSquares.Sort(heightCompare);
            Console.WriteLine("Sorted list using IComparer<Square>=heightCompare");
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Sorted list using IComparable<Square>");
            listOfSquares.Sort();
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }


            // Test a SORTEDLIST
            var sortedListOfSquares = new SortedList<int, Square>(){
                                    { 0, new Square(1,3)},
                                    { 2, new Square(3,3)},
                                    { 1, new Square(2,1)},
                                    { 3, new Square(6,1)}};

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SortedList<Square>");
            foreach (KeyValuePair<int, Square> kvp in sortedListOfSquares)
            {
                Console.WriteLine($"{kvp.Key} : {kvp.Value}");
            }
        }


        public class Square : IComparable<Square>
        {
            public Square() { }

            public Square(int height, int width)
            {
                this.Height = height;
                this.Width = width;
            }

            public int Height { get; set; }

            public int Width { get; set; }

            public int CompareTo(object obj)
            {
                Square square = obj as Square;
                if (square != null)
                    return CompareTo(square);
                throw (new ArgumentException("Both objects being compared must be of type Square."));
            }

            public override string ToString() => ($"Height: {this.Height}     Width: {this.Width}");

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Square square = obj as Square;
                if (square != null)
                    return this.Height == square.Height;
                return false;
            }

            public override int GetHashCode() => this.Height.GetHashCode() | this.Width.GetHashCode();

            public static bool operator ==(Square x, Square y) => x.Equals(y);
            public static bool operator !=(Square x, Square y) => !(x == y);
            public static bool operator <(Square x, Square y) => (x.CompareTo(y) < 0);
            public static bool operator >(Square x, Square y) => (x.CompareTo(y) > 0);

            #region IComparable<Square> Members

            public int CompareTo(Square other)
            {
                long area1 = this.Height * this.Width;
                long area2 = other.Height * other.Width;

                if (area1 == area2)
                    return 0;
                else if (area1 > area2)
                    return 1;
                else if (area1 < area2)
                    return -1;
                else
                    return -1;
            }

            #endregion
        }

        public class CompareHeight : IComparer<Square>
        {
            public int Compare(object firstSquare, object secondSquare)
            {
                Square square1 = firstSquare as Square;
                Square square2 = secondSquare as Square;
                if (square1 == null || square2 == null)
                    throw (new ArgumentException("Both parameters must be of type Square."));
                else
                    return Compare(firstSquare, secondSquare);
            }

            #region IComparer<Square> Members

            public int Compare(Square x, Square y)
            {
                if (x.Height == y.Height)
                    return 0;
                else if (x.Height > y.Height)
                    return 1;
                else if (x.Height < y.Height)
                    return -1;
                else
                    return -1;
            }

            #endregion
        }
        #endregion

        #region "1.3 Making a Type Searchable"
        // See the Square type in the previous code region.

        public static void TestSearch()
        {
            List<Square> listOfSquares = new List<Square> {new Square(1,3),
                                                        new Square(4,3),
                                                        new Square(2,1),
                                                        new Square(6,1)};

            IComparer<Square> heightCompare = new CompareHeight();

            // Test a List<Square>
            Console.WriteLine("List<Square>");
            Console.WriteLine("Original list");
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Sorted list using IComparer<Square>=heightCompare");
            listOfSquares.Sort(heightCompare);
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Search using IComparer<Square>=heightCompare");
            int found = listOfSquares.BinarySearch(new Square(1, 3), heightCompare);
            Console.WriteLine($"Found (1,3): {found}");

            Console.WriteLine();
            Console.WriteLine("Sorted list using IComparable<Square>");
            listOfSquares.Sort();
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }

            Console.WriteLine("Search using IComparable<Square>");
            found = listOfSquares.BinarySearch(new Square(6, 1));  // Use IComparable
            Console.WriteLine($"Found (6,1): {found}");


            // Test a SortedList<Square>
            var sortedListOfSquares = new SortedList<int, Square>(){
                                            {0, new Square(1,3)},
                                            {2, new Square(4,3)},
                                            {1, new Square(2,1)},
                                            {4, new Square(6,1)}};

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SortedList<Square>");
            foreach (KeyValuePair<int, Square> kvp in sortedListOfSquares)
            {
                Console.WriteLine($"{kvp.Key} : ${kvp.Value}");
            }

            Console.WriteLine();
            bool foundItem = sortedListOfSquares.ContainsKey(2);
            Console.WriteLine($"sortedListOfSquares.ContainsKey(2): {foundItem}");

            // Does not use IComparer or IComparable
            // -- uses a linear search along with the Equals method
            //    which has not been overloaded
            Square value = new Square(6, 1);
            foundItem = sortedListOfSquares.ContainsValue(value);
            Console.WriteLine($"sortedListOfSquares.ContainsValue(new Square(6,1)): {foundItem}");
        }
        #endregion

        #region "1.4 Returning Multiple Items From A Method"
        public void ReturnDimensions(int inputShape,
            out int height,
            out int width,
            out int depth)
        {
            height = 0;
            width = 0;
            depth = 0;

            // Calculate height, width, depth from the inputShape value
        }

        public Dimensions ReturnDimensions(int inputShape)
        {
            // The default ctor automatically defaults this structure’s members to 0
            Dimensions objDim = new Dimensions();

            // Calculate objDim.Height, objDim.Width, objDim.Depth from the inputShape value

            return (objDim);
        }

        public struct Dimensions
        {
            public int Height;
            public int Width;
            public int Depth;
        }

        public Tuple<int, int, int> ReturnDimensionsAsTuple(int inputShape)
        {
            // Calculate objDim.Height, objDim.Width, objDim.Depth from the inputShape value
            // e.g. {5, 10, 15}

            // Create a Tuple with calculated values
            var objDim = Tuple.Create<int, int, int>(5, 10, 15);

            return (objDim);
        }
        #endregion

        #region "1.5 Parsing Command Line Parameters"
        public static void TestParser(string[] argumentStrings)
        {
            //Important point: why am I immediately converting the parsed arguments to an array?  
            //Because query results are CALCULATED LAZILY and RECALCULATED ON DEMAND.  
            //If we just did the transformation without forcing it to an array, then EVERY SINGLE TIME 
            //we iterated the collection it would reparse.  Remember, the query logic does not know that 
            //the argumentStrings collection isn’t changing!  It is not an immutable object, so every time 
            //we iterate the collection, we run the query AGAIN, and that reparses everything.  
            //Since we only want to parse everything once, we iterate it once and store the results in an array.
            //Now that we’ve got our parsed arguments, we’ll do an error checking pass:
            var arguments = (from argument in argumentStrings
                             select new Argument(argument)).ToArray();

            Console.Write("Command line: ");
            foreach (Argument a in arguments)
            {
                Console.Write($"{a.Original} ");
            }
            Console.WriteLine("");

            ArgumentSemanticAnalyzer analyzer = new ArgumentSemanticAnalyzer();
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("output",
                    "/output:[path to output]",
                    "Specifies the location of the output file.",
                    x => x.IsCompoundSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("trialMode",
                    "/trialmode",
                    "If this is specified it places the product into trial mode",
                    x => x.IsSimpleSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("DEBUGOUTPUT",
                    "/debugoutput:[value1];[value2];[value3]",
                    "A listing of the files the debug output information will be written to",
                    x => x.IsComplexSwitch));
            analyzer.AddArgumentVerifier(
                new ArgumentDefinition("",
                    "[literal value]",
                    "A literal value",
                    x => x.IsSimple));

            if (!analyzer.VerifyArguments(arguments))
            {
                string invalidArguments = analyzer.InvalidArgumentsDisplay();
                Console.WriteLine(invalidArguments);
                ShowUsage(analyzer);
                return;
            }

            //We’ll come back to that.  Assuming that our error checking pass gave the thumbs up, 
            //we’ll extract the information out of the parsed arguments that we need to run our program. 
            //Here’s the information we need:
            string output = string.Empty;
            bool trialmode = false;
            IEnumerable<string> debugOutput = null;
            List<string> literals = new List<string>();

            //For each parsed argument we want to apply an action, 
            // so add them to the analyzer .  
            analyzer.AddArgumentAction("OUTPUT", x => { output = x.SubArguments[0]; });
            analyzer.AddArgumentAction("TRIALMODE", x => { trialmode = true; });
            analyzer.AddArgumentAction("DEBUGOUTPUT", x => { debugOutput = x.SubArguments; });
            analyzer.AddArgumentAction("", x => { literals.Add(x.Original); });

            // check the arguments and run the actions
            analyzer.EvaluateArguments(arguments);

            // display the results
            Console.WriteLine("");
            Console.WriteLine($"OUTPUT: {output}");
            Console.WriteLine($"TRIALMODE: {trialmode}");
            if (debugOutput != null)
            {
                foreach (string item in debugOutput)
                {
                    Console.WriteLine($"DEBUGOUTPUT: {item}");
                }
            }
            foreach (string literal in literals)
            {
                Console.WriteLine($"LITERAL: {literal}");
            }

            //and we are ready to run our program:
            //Program program = new Program(output, trialmode, debugOutput, literals);
            //program.Run();
        }

        public static void ShowUsage(ArgumentSemanticAnalyzer analyzer)
        {
            Console.WriteLine("Program.exe allows the following arguments:");
            foreach (ArgumentDefinition definition in analyzer.ArgumentDefinitions)
            {
                Console.WriteLine($"\t{definition.ArgumentSwitch}: ({definition.Description}){Environment.NewLine}\tSyntax: {definition.Syntax}");
            }
        }

        public sealed class Argument
        {
            public string Original { get; }
            public string Switch { get; private set; }
            public ReadOnlyCollection<string> SubArguments { get; }
            private List<string> subArguments;
            public Argument(string original)
            {
                Original = original;
                Switch = string.Empty;
                subArguments = new List<string>();
                SubArguments = new ReadOnlyCollection<string>(subArguments);
                Parse();
            }

            private void Parse()
            {
                if (string.IsNullOrEmpty(Original))
                {
                    return;
                }
                char[] switchChars = { '/', '-' };
                if (!switchChars.Contains(Original[0]))
                {
                    return;
                }
                string switchString = Original.Substring(1);
                string subArgsString = string.Empty;
                int colon = switchString.IndexOf(':');
                if (colon >= 0)
                {
                    subArgsString = switchString.Substring(colon + 1);
                    switchString = switchString.Substring(0, colon);
                }
                Switch = switchString;
                if (!string.IsNullOrEmpty(subArgsString))
                    subArguments.AddRange(subArgsString.Split(';'));
            }

            // A set of predicates that provide useful information about itself
            //   Implemented using lambdas
            public bool IsSimple => SubArguments.Count == 0;
            public bool IsSimpleSwitch => !string.IsNullOrEmpty(Switch) && SubArguments.Count == 0;
            public bool IsCompoundSwitch => !string.IsNullOrEmpty(Switch) && SubArguments.Count == 1;
            public bool IsComplexSwitch => !string.IsNullOrEmpty(Switch) && SubArguments.Count > 0;
        }

        public sealed class ArgumentDefinition
        {
            public string ArgumentSwitch { get;  }
            public string Syntax { get;  }
            public string Description { get;  }
            public Func<Argument, bool> Verifier { get;  }

            public ArgumentDefinition(string argumentSwitch,
                                      string syntax,
                                      string description,
                                      Func<Argument, bool> verifier)
            {
                ArgumentSwitch = argumentSwitch.ToUpper();
                Syntax = syntax;
                Description = description;
                Verifier = verifier;
            }

            public bool Verify(Argument arg) => Verifier(arg);
        }

        public sealed class ArgumentSemanticAnalyzer
        {
            private List<ArgumentDefinition> argumentDefinitions =
                new List<ArgumentDefinition>();
            private Dictionary<string, Action<Argument>> argumentActions =
                new Dictionary<string, Action<Argument>>();

            public ReadOnlyCollection<Argument> UnrecognizedArguments { get; private set; }
            public ReadOnlyCollection<Argument> MalformedArguments { get; private set; }
            public ReadOnlyCollection<Argument> RepeatedArguments { get; private set; }

            public ReadOnlyCollection<ArgumentDefinition> ArgumentDefinitions => new ReadOnlyCollection<ArgumentDefinition>(argumentDefinitions);

            public IEnumerable<string> DefinedSwitches => from argumentDefinition in argumentDefinitions
                           select argumentDefinition.ArgumentSwitch;

            public void AddArgumentVerifier(ArgumentDefinition verifier) => argumentDefinitions.Add(verifier);

            public void RemoveArgumentVerifier(ArgumentDefinition verifier)
            {
                var verifiersToRemove = from v in argumentDefinitions
                                        where v.ArgumentSwitch == verifier.ArgumentSwitch
                                        select v;
                foreach (var v in verifiersToRemove)
                    argumentDefinitions.Remove(v);
            }

            public void AddArgumentAction(string argumentSwitch, Action<Argument> action) => argumentActions.Add(argumentSwitch, action);

            public void RemoveArgumentAction(string argumentSwitch)
            {
                if (argumentActions.Keys.Contains(argumentSwitch))
                    argumentActions.Remove(argumentSwitch);
            }

            public bool VerifyArguments(IEnumerable<Argument> arguments)
            {
                // no parameter to verify with, fail.
                if (!argumentDefinitions.Any())
                    return false;

                // Identify if any of the arguments are not defined
                this.UnrecognizedArguments = (from argument in arguments
                                              where !DefinedSwitches.Contains(argument.Switch.ToUpper())
                                              select argument).ToList().AsReadOnly();


                //Check for all the arguments where the switch matches a known switch, 
                //but our well-formedness predicate is false. 
                this.MalformedArguments = (from argument in arguments
                                           join argumentDefinition in argumentDefinitions
                                           on argument.Switch.ToUpper() equals
                                               argumentDefinition.ArgumentSwitch
                                           where !argumentDefinition.Verify(argument)
                                           select argument).ToList().AsReadOnly();

                //Sort the arguments into “groups?by their switch, count every group, 
                //and select any groups that contain more than one element, 
                //We then get a read only list of the items.
                this.RepeatedArguments =
                        (from argumentGroup in
                             from argument in arguments
                             where !argument.IsSimple
                             group argument by argument.Switch.ToUpper()
                         where argumentGroup.Count() > 1
                         select argumentGroup).SelectMany(ag => ag).ToList().AsReadOnly();

                if (this.UnrecognizedArguments.Any() ||
                    this.MalformedArguments.Any() ||
                    this.RepeatedArguments.Any())
                    return false;

                return true;
            }

            public void EvaluateArguments(IEnumerable<Argument> arguments)
            {
                //Now we just apply each action:
                foreach (Argument argument in arguments)
                    argumentActions[argument.Switch.ToUpper()](argument);
            }

            public string InvalidArgumentsDisplay()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat($"Invalid arguments: {Environment.NewLine}");
                // Add the unrecognized arguments
                FormatInvalidArguments(builder, this.UnrecognizedArguments,
                    "Unrecognized argument: {0}{1}");

                // Add the malformed arguments
                FormatInvalidArguments(builder, this.MalformedArguments,
                    "Malformed argument: {0}{1}");

                // For the repeated arguments, we want to group them for the display
                // so group by switch and then add it to the string being built.
                var argumentGroups = from argument in this.RepeatedArguments
                                     group argument by argument.Switch.ToUpper() into ag
                                     select new { Switch = ag.Key, Instances = ag };

                foreach (var argumentGroup in argumentGroups)
                {
                    builder.AppendFormat($"Repeated argument: {argumentGroup.Switch}{Environment.NewLine}");
                    FormatInvalidArguments(builder, argumentGroup.Instances.ToList(),
                        "\t{0}{1}");
                }
                return builder.ToString();
            }

            private void FormatInvalidArguments(StringBuilder builder,
                IEnumerable<Argument> invalidArguments, string errorFormat)
            {
                if (invalidArguments != null)
                {
                    foreach (Argument argument in invalidArguments)
                    {
                        builder.AppendFormat(errorFormat,
                            argument.Original, Environment.NewLine);
                    }
                }
            }
        }
        #endregion

        #region "1.6 Initializing A Constant Field at Runtime"
        public class Foo
        {
            public readonly int x;
            public const int y = 1;

            public Foo() { }
            public Foo(int roInitValue)
            {
                x = roInitValue;
            }

            // Rest of class...
        }

        public class Foo2
        {
            public readonly int Bar;

            public Foo2() { }

            public Foo2(int constInitValue)
            {
                Bar = constInitValue;
            }

            // Rest of class...
        }

        public class Foo22
        {
            public const int Bar = 100;

            public Foo22() { }

            public Foo22(int constInitValue)
            {
                //Bar = constInitValue;    // This line causes a compile-time error
            }

            // Rest of class...
        }
        #endregion

        #region "1.7 Building Cloneable Classes"
        public static void TestCloning()
        {
            ShallowClone sc = new ShallowClone();
            sc.ListData.Add("asdf");
            ShallowClone scCloned = sc.ShallowCopy();
            Console.WriteLine($"scCloned.ListData.Remove(\"asdf\") == {scCloned.ListData.Remove("asdf")}");

            DeepClone dc = new DeepClone();
            dc.ListData.Add("asdf");
            DeepClone dcCloned = dc.DeepCopy();
            dcCloned.ListData.Remove("asdf");
            Console.WriteLine($"dc.ListData.Contains(\"asdf\") == {dc.ListData.Contains("asdf")}");
            Console.WriteLine($"dcCloned.ListData.Contains(\"asdf\") == {dcCloned.ListData.Contains("asdf")}");

            MultiClone mc = new MultiClone();
            mc.ListData.Add("asdf");
            MultiClone mcCloned = mc.DeepCopy();
            Console.WriteLine($"mcCloned.ListData.Contains(\"asdf\") == {mcCloned.ListData.Contains("asdf")}");
            Console.WriteLine($"mc.ListData.Contains(\"asdf\") == {mc.ListData.Contains("asdf")}");
        }

        public interface IShallowCopy<T>
        {
            T ShallowCopy();
        }
        public interface IDeepCopy<T>
        {
            T DeepCopy();
        }

        public class ShallowClone : IShallowCopy<ShallowClone>
        {
            public int Data = 1;
            public List<string> ListData = new List<string>();
            public object ObjData = new object();

            public ShallowClone ShallowCopy() => (ShallowClone)this.MemberwiseClone();
        }

        [Serializable]
        public class DeepClone : IDeepCopy<DeepClone>
        {
            public int data = 1;
            public List<string> ListData = new List<string>();
            public object objData = new object();

            public DeepClone DeepCopy()
            {
                BinaryFormatter BF = new BinaryFormatter();
                MemoryStream memStream = new MemoryStream();

                BF.Serialize(memStream, this);
                memStream.Flush();
                memStream.Position = 0;

                return (DeepClone)BF.Deserialize(memStream);
            }
        }

        [Serializable]
        public class MultiClone : IShallowCopy<MultiClone>,
                                  IDeepCopy<MultiClone>
        {
            public int data = 1;
            public List<string> ListData = new List<string>();
            public object objData = new object();

            public MultiClone ShallowCopy() => (MultiClone)this.MemberwiseClone();

            public MultiClone DeepCopy()
            {
                BinaryFormatter BF = new BinaryFormatter();
                MemoryStream memStream = new MemoryStream();

                BF.Serialize(memStream, this);
                memStream.Flush();
                memStream.Position = 0;

                return (MultiClone)BF.Deserialize(memStream);
            }
        }

        #endregion

        #region "1.8 Assuring an Object's Disposal"
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void DisposeObj1()
        {
            using (FileStream FS = new FileStream("Test.txt", FileMode.Create))
            {
                FS.WriteByte((byte)1);
                FS.WriteByte((byte)2);
                FS.WriteByte((byte)3);

                using (StreamWriter SW = new StreamWriter(FS))
                {
                    SW.WriteLine("some text.");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void DisposeObj2()
        {
            using (FileStream FS = new FileStream("Test.txt", FileMode.Create))
            {
                FS.WriteByte((byte)1);
                FS.WriteByte((byte)2);
                FS.WriteByte((byte)3);

                using (StreamWriter SW = new StreamWriter(FS))
                {
                    SW.WriteLine("some text.");
                }
            }
        }
        #endregion

        #region "1.9 Deciding When and Where to Use Generics "
        // See this recipe in the book for more information
        #endregion

        #region "1.10 Understanding generic class types"
        public static void TestGenericClassInstanceCounter()
        {
            // regular class
            FixedSizeCollection A = new FixedSizeCollection(5);
            Console.WriteLine(A);
            FixedSizeCollection B = new FixedSizeCollection(5);
            Console.WriteLine(B);
            FixedSizeCollection C = new FixedSizeCollection(5);
            Console.WriteLine(C);

            // generic class
            FixedSizeCollection<bool> gA = new FixedSizeCollection<bool>(5);
            Console.WriteLine(gA);
            FixedSizeCollection<int> gB = new FixedSizeCollection<int>(5);
            Console.WriteLine(gB);
            FixedSizeCollection<string> gC = new FixedSizeCollection<string>(5);
            Console.WriteLine(gC);
            FixedSizeCollection<string> gD = new FixedSizeCollection<string>(5);
            Console.WriteLine(gD);

            bool b1 = true;
            bool b2 = false;
            bool bHolder = false;

            // add to the standard class (as object)
            A.AddItem(b1);
            A.AddItem(b2);
            // add to the generic class (as bool)
            gA.AddItem(b1);
            gA.AddItem(b2);

            Console.WriteLine(A);
            Console.WriteLine(gA);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'bool'...
            bHolder = (bool)A.GetItem(1);
            // no cast necessary
            bHolder = gA.GetItem(1);

            int i1 = 1;
            int i2 = 2;
            int i3 = 3;
            int iHolder = 0;

            // add to the standard class (as object)
            B.AddItem(i1);
            B.AddItem(i2);
            B.AddItem(i3);
            // add to the generic class (as int)
            gB.AddItem(i1);
            gB.AddItem(i2);
            gB.AddItem(i3);

            Console.WriteLine(B);
            Console.WriteLine(gB);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'int'...
            iHolder = (int)B.GetItem(1);
            // no cast necessary
            iHolder = gB.GetItem(1);

            string s1 = "s1";
            string s2 = "s2";
            string s3 = "s3";
            string sHolder = "";

            // add to the standard class (as object)
            C.AddItem(s1);
            C.AddItem(s2);
            C.AddItem(s3);
            // add an int to the string instance, perfectly OK
            C.AddItem(i1);

            // add to the generic class (as string)
            gC.AddItem(s1);
            gC.AddItem(s2);
            gC.AddItem(s3);
            // try to add an int to the string instance, denied by compiler
            // error CS1503: Argument '1': cannot convert from 'int' to 'string'
            //gC.AddItem(i1);

            Console.WriteLine(C);
            Console.WriteLine(gC);

            // have to cast or get error CS0266: 
            // Cannot implicitly convert type 'object' to 'string'...
            sHolder = (string)C.GetItem(1);
            // no cast necessary
            sHolder = gC.GetItem(1);
            // try to get a string into an int, error
            // error CS0029: Cannot implicitly convert type 'string' to 'int'
            //iHolder = gC.GetItem(1);
        }

        public class FixedSizeCollection
        {
            /// <summary>
            /// Constructor that increments static counter
            /// and sets the maximum number of items
            /// </summary>
            /// <param name="maxItems"></param>
            public FixedSizeCollection(int maxItems)
            {
                FixedSizeCollection.InstanceCount++;
                this.Items = new object[maxItems];
            }

            /// <summary>
            /// Add an item to the class whose type 
            /// is unknown as only object can hold any type
            /// </summary>
            /// <param name="item">item to add</param>
            /// <returns>the index of the item added</returns>
            public int AddItem(object item)
            {
                if (this.ItemCount < this.Items.Length)
                {
                    this.Items[this.ItemCount] = item;
                    return this.ItemCount++;
                }
                else
                    throw new Exception("Item queue is full");
            }

            /// <summary>
            /// Get an item from the class
            /// </summary>
            /// <param name="index">the index of the item to get</param>
            /// <returns>an item of type object</returns>
            public object GetItem(int index)
            {
                if (index >= this.Items.Length &&
                    index >= 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return this.Items[index];
            }

            #region Properties
            /// <summary>
            /// Static instance counter hangs off of the Type for 
            /// StandardClass 
            /// </summary>
            public static int InstanceCount { get; set; }

            /// <summary>
            /// The count of the items the class holds
            /// </summary>
            public int ItemCount { get; private set; }

            /// <summary>
            /// The items in the class
            /// </summary>
#pragma warning disable CSE0002 // Use getter-only auto properties
            private object[] Items { get; set; }
#pragma warning restore CSE0002 // Use getter-only auto properties

            #endregion // Properties

            /// <summary>
            /// ToString override to provide class detail
            /// </summary>
            /// <returns>formatted string with class details</returns>
            public override string ToString() => $"There are {FixedSizeCollection.InstanceCount.ToString()} instances of {this.GetType().ToString()} and this instance contains {this.ItemCount} items...";
        }

        /// <summary>
        /// A generic class to show instance counting
        /// </summary>
        /// <typeparam name="T">the type parameter used for the array storage</typeparam>
        public class FixedSizeCollection<T>
        {
            /// <summary>
            /// Constructor that increments static counter and sets up internal storage
            /// </summary>
            /// <param name="items"></param>
            public FixedSizeCollection(int items)
            {
                FixedSizeCollection<T>.InstanceCount++;
                this.Items = new T[items];
            }

            /// <summary>
            /// Add an item to the class whose type 
            /// is determined by the instantiating type
            /// </summary>
            /// <param name="item">item to add</param>
            /// <returns>the zero-based index of the item added</returns>
            public int AddItem(T item)
            {
                if (this.ItemCount < this.Items.Length)
                {
                    this.Items[this.ItemCount] = item;
                    return this.ItemCount++;
                }
                else
                    throw new Exception("Item queue is full");
            }

            /// <summary>
            /// Get an item from the class
            /// </summary>
            /// <param name="index">the zero-based index of the item to get</param>
            /// <returns>an item of the instantiating type</returns>
            public T GetItem(int index)
            {
                if (index >= this.Items.Length &&
                    index >= 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return this.Items[index];
            }

            #region Properties
            /// <summary>
            /// Static instance counter hangs off of the 
            /// instantiated Type for 
            /// GenericClass
            /// </summary>
            public static int InstanceCount { get; set; }

            /// <summary>
            /// The count of the items the class holds
            /// </summary>
            public int ItemCount { get; private set; }

            /// <summary>
            /// The items in the class
            /// </summary>
#pragma warning disable CSE0002 // Use getter-only auto properties
            private T[] Items { get; set; }
#pragma warning restore CSE0002 // Use getter-only auto properties
            #endregion // Properties

            /// <summary>
            /// ToString override to provide class detail
            /// </summary>
            /// <returns>formatted string with class details</returns>
            public override string ToString() => $"There are {FixedSizeCollection<T>.InstanceCount.ToString()} instances of {this.GetType().ToString()} and this instance contains {this.ItemCount} items...";
        }

        #endregion

        #region "1.11 Reversing the Contents of a Sorted List"

        public static void TestReversibleSortedList()
        {
            SortedList<int, string> data = new SortedList<int, string>() {[2] = "two",[5] = "five",[3] = "three",[1] = "one" };

            foreach (KeyValuePair<int, string> kvp in data)
            {
                Console.WriteLine($"\t {kvp.Key}\t{kvp.Value}");
            }
            Console.WriteLine("");

            // query ordering by descending
            var query = from d in data
                        orderby d.Key descending
                        select d;

            foreach (KeyValuePair<int, string> kvp in query)
            {
                Console.WriteLine($"\t {kvp.Key}\t{kvp.Value}");
            }
            Console.WriteLine("");


            data.Add(4, "four");

            // requery ordering by descending
            query = from d in data
                    orderby d.Key descending
                    select d;

            foreach (KeyValuePair<int, string> kvp in query)
            {
                Console.WriteLine($"\t {kvp.Key}\t{kvp.Value}");
            }
            Console.WriteLine("");

            // Just go against the original list for ascending
            foreach (KeyValuePair<int, string> kvp in data)
            {
                Console.WriteLine($"\t {kvp.Key}\t{kvp.Value}");
            }
        }
        #endregion

        #region "1.12 Constraining Type Arguments"
        public static void TestConversionCls()
        {
            Console.WriteLine("\r\n\r\n");

            Conversion<long> c = new Conversion<long>();
            //Console.WriteLine($"long.MinValue:  {c.ShowAsInt(long.MinValue)}");
            Console.WriteLine($"-100:  {c.ShowAsInt(-100)}");
            Console.WriteLine($"0:  {c.ShowAsInt(0)}");
            Console.WriteLine($"100:  {c.ShowAsInt(100)}");
            //Console.WriteLine($"long.MaxValue:  {c.ShowAsInt(long.MaxValue)}");
        }

        public static void TestComparableListCls()
        {
            Console.WriteLine("\r\n\r\n");

            ComparableList<int> cp =
                new ComparableList<int>() { 100, 10 };

            Console.WriteLine($"0 compare 1 == {cp.Compare(0, 1)}");
            Console.WriteLine($"1 compare 0 == {cp.Compare(1, 0)}");
            Console.WriteLine($"1 compare 1 == {cp.Compare(1, 1)}");
        }

        public static void TestDisposableListCls()
        {
            Console.WriteLine("\r\n\r\n");

            DisposableList<StreamReader> dl = new DisposableList<StreamReader>();

            // Create a few test objects
            StreamReader tr1 = new StreamReader("C:\\Windows\\system.ini");
            StreamReader tr2 = new StreamReader("c:\\Windows\\vmgcoinstall.log");
            StreamReader tr3 = new StreamReader("c:\\Windows\\Starter.xml");

            // Add the test object to the DisposableList
            dl.Add(tr1);
            dl.Insert(0, tr2);

            Console.WriteLine($"dl.IndexOf(tr3) == {dl.IndexOf(tr3)}");

            dl.Add(tr3);

            Console.WriteLine($"dl.Contains(tr1) == {dl.Contains(tr1)}");

            StreamReader[] srArray = new StreamReader[3];
            dl.CopyTo(srArray, 0);
            Console.WriteLine($"srArray[1].ReadLine() == {srArray[1].ReadLine()}");

            Console.WriteLine($"dl.Count == {dl.Count}");

            foreach (StreamReader sr in dl)
            {
                Console.WriteLine($"sr.ReadLine() == {sr.ReadLine()}");
            }

            Console.WriteLine($"dl.IndexOf(tr3) == {dl.IndexOf(tr3)}");

            Console.WriteLine($"dl.IsReadOnly == {dl.IsReadOnly}");

            // Call Dispose before any of the disposable objects are removed from the DisposableList
            dl.RemoveAt(0);
            Console.WriteLine($"dl.Count == {dl.Count}");

            dl.Remove(tr1);
            Console.WriteLine($"dl.Count == {dl.Count}");

            dl.Clear();
            Console.WriteLine($"dl.Count == {dl.Count}");
        }


        public class Conversion<T>
            where T : struct, IConvertible
        {
            public int ShowAsInt(T value) => value.ToInt32(NumberFormatInfo.CurrentInfo);
        }

        public class ComparableList<T> : List<T>
            where T : IComparable<T>
        {
            public int Compare(int index1, int index2) => index1.CompareTo(index2);
        }

        public class DisposableList<T> : IList<T>
            where T : class, IDisposable
        {
            private List<T> _items = new List<T>();

            // Private method that will dispose of items in the list
            private void Delete(T item) => item.Dispose();

            // IList<T> Members
            public int IndexOf(T item) => _items.IndexOf(item);

            public void Insert(int index, T item) => _items.Insert(index, item);

            public T this[int index]
            {
                get { return (_items[index]); }
                set { _items[index] = value; }
            }

            public void RemoveAt(int index)
            {
                Delete(this[index]);
                _items.RemoveAt(index);
            }

            // ICollection<T> Members
            public void Add(T item) => _items.Add(item);

            public bool Contains(T item) => _items.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            // IEnumerable<T> Members
            public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

            // IEnumerable Members
            IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

            // Other members
            public void Clear()
            {
                for (int index = 0; index < _items.Count; index++)
                {
                    Delete(_items[index]);
                }

                _items.Clear();
            }

            public bool Remove(T item)
            {
                int index = _items.IndexOf(item);

                if (index >= 0)
                {
                    Delete(_items[index]);
                    _items.RemoveAt(index);

                    return (true);
                }
                else
                {
                    return (false);
                }
            }
        }
        #endregion

        #region "1.13 Initializing Generic Variables to their Default Value"
        public static void ShowSettingFieldsToDefaults()
        {
            Console.WriteLine("\r\n\r\n");

            DefaultValueExample<int> dv = new DefaultValueExample<int>();

            // Check if the data is set to its defalut value, true is returned
            bool isDefault = dv.IsDefaultData();
            Console.WriteLine($"Initial data: {isDefault}");

            // Set data
            dv.SetData(100);

            // Check again, this time a false is returned
            isDefault = dv.IsDefaultData();
            Console.WriteLine($"Set data: {isDefault}");
        }

        public class DefaultValueExample<T>
        {
            T data = default(T);

            public bool IsDefaultData()
            {
                T temp = default(T);

                if (temp.Equals(data))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }

            public void SetData(T value) => data = value;
        }
        #endregion

        #region "1.14 Adding hooks to generated entities"
        public static void TestPartialMethods()
        {
            Console.WriteLine("Start entity work");
            GeneratedEntity entity = new GeneratedEntity("FirstEntity");
            entity.FirstName = "Bob";
            entity.State = "NH";
            GeneratedEntity secondEntity = new GeneratedEntity("SecondEntity");
            entity.FirstName = "Jay";
            secondEntity.FirstName = "Steve";
            secondEntity.State = "MA";
            entity.FirstName = "Barry";
            secondEntity.State = "WA";
            secondEntity.FirstName = "Matt";
            Console.WriteLine("End entity work");
        }

        //OUTPUT
        //Start entity work
        //Changed property (FirstName) for entity FirstEntity from  to Bob
        //Changed property (State) for entity FirstEntity from  to NH
        //Changed property (FirstName) for entity FirstEntity from Bob to Jay
        //Changed property (FirstName) for entity SecondEntity from  to Steve
        //Changed property (State) for entity SecondEntity from  to MA
        //Changed property (FirstName) for entity FirstEntity from Jay to Barry
        //Changed property (State) for entity SecondEntity from MA to WA
        //Changed property (FirstName) for entity SecondEntity from Steve to Matt
        //End entity work

        public partial class GeneratedEntity
        {
            public GeneratedEntity(string entityName)
            {
                this.EntityName = entityName;
            }

            partial void ChangingProperty(string name, string originalValue, string newValue);

            public string EntityName { get; }

            private string _FirstName;
            public string FirstName
            {
                get { return _FirstName; }
                set
                {
                    ChangingProperty("FirstName", _FirstName, value);
                    _FirstName = value;
                }
            }

            private string _State;
            public string State
            {
                get { return _State; }
                set
                {
                    ChangingProperty("State", _State, value);
                    _State = value;
                }
            }
        }

        public partial class GeneratedEntity
        {
            partial void ChangingProperty(string name, string originalValue, string newValue)
            {
                Console.WriteLine($"Changed property (]{name}) for entity {this.EntityName} from {originalValue} to {newValue}");
            }
        }
        #endregion 

        #region "1.15 Controlling How a Delegate Fires Within a Multicast Delegate"

        public static void InvokeInReverse()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Console.WriteLine("Fire delegates in reverse");
            Delegate[] delegateList = allInstances.GetInvocationList();
            foreach (Func<int> instance in delegateList.Reverse())
            {
                instance();
            }
        }


        public static void InvokeEveryOtherOperation()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances = myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Delegate[] delegateList = allInstances.GetInvocationList();
            Console.WriteLine("Invoke every other delegate");
            foreach (Func<int> instance in delegateList.EveryOther())
            {
                // invoke the delegate
                int retVal = instance();
                Console.WriteLine($"Delegate returned {retVal}");
            }
        }

        public class TestInvokeIntReturn
        {
            public static int Method1()
            {
                Console.WriteLine("Invoked Method1");
                return 1;
            }

            public static int Method2()
            {
                Console.WriteLine("Invoked Method2");
                return 2;
            }

            public static int Method3()
            {
                //throw (new Exception("Method1"));
                //throw (new SecurityException("Method3"));
                Console.WriteLine("Invoked Method3");
                return 3;
            }
        }

        public static void InvokeWithTest()
        {
            Func<bool> myDelegateInstanceBool1 = TestInvokeBoolReturn.Method1;
            Func<bool> myDelegateInstanceBool2 = TestInvokeBoolReturn.Method2;
            Func<bool> myDelegateInstanceBool3 = TestInvokeBoolReturn.Method3;

            Func<bool> allInstancesBool =
                    myDelegateInstanceBool1 +
                    myDelegateInstanceBool2 +
                    myDelegateInstanceBool3;

            Console.WriteLine(
                "Invoke individually (Call based on previous return value):");
            foreach (Func<bool> instance in allInstancesBool.GetInvocationList())
            {
                if (!instance())
                    break;
            }
        }

        public class TestInvokeBoolReturn
        {
            public static bool Method1()
            {
                Console.WriteLine("Invoked Method1");
                return true;
            }

            public static bool Method2()
            {
                Console.WriteLine("Invoked Method2");
                return false;
            }

            public static bool Method3()
            {
                Console.WriteLine("Invoked Method3");
                return true;
            }
        }


        public static void TestIndividualInvokesReturnValue()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Console.WriteLine("Invoke individually (Obtain each return value):");
            foreach (Func<int> instance in allInstances.GetInvocationList())
            {
                int retVal = instance();
                Console.WriteLine($"\tOutput: {retVal}");
            }
        }


        [Serializable]
        public class MulticastInvocationException : Exception
        {
            private List<Exception> _invocationExceptions;

            public MulticastInvocationException()
                : base()
            {
            }

            public MulticastInvocationException(IEnumerable<Exception> invocationExceptions)
            {
                _invocationExceptions = new List<Exception>(invocationExceptions);
            }

            public MulticastInvocationException(string message)
                : base(message)
            {
            }

            public MulticastInvocationException(string message, Exception innerException) :
                base(message, innerException)
            {
            }

            protected MulticastInvocationException(SerializationInfo info, StreamingContext context) :
                base(info, context)
            {
                _invocationExceptions =
                    (List<Exception>)info.GetValue("InvocationExceptions",
                        typeof(List<Exception>));
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            public override void GetObjectData(
               SerializationInfo info, StreamingContext context)
            {
                info.AddValue("InvocationExceptions", this.InvocationExceptions);
                base.GetObjectData(info, context);
            }

            public ReadOnlyCollection<Exception> InvocationExceptions => new ReadOnlyCollection<Exception>(_invocationExceptions);
        }

        public static void TestIndividualInvokesExceptions()
        {
            Func<int> myDelegateInstance1 = TestInvokeIntReturn.Method1;
            Func<int> myDelegateInstance2 = TestInvokeIntReturn.Method2;
            Func<int> myDelegateInstance3 = TestInvokeIntReturn.Method3;

            Func<int> allInstances =
                    myDelegateInstance1 +
                    myDelegateInstance2 +
                    myDelegateInstance3;

            Console.WriteLine("Invoke individually (handle exceptions):");

            // Create an instance of a wrapper exception to hold any exceptions
            // encountered during the invocations of the delegate instances
            List<Exception> invocationExceptions = new List<Exception>();

            foreach (Func<int> instance in allInstances.GetInvocationList())
            {
                try
                {
                    int retVal = instance();
                    Console.WriteLine($"\tOutput: {retVal}");
                }
                catch (Exception ex)
                {
                    // Display and log the exception and continue
                    Console.WriteLine(ex.ToString());
                    EventLog myLog = new EventLog();
                    myLog.Source = "MyApplicationSource";
                    myLog.WriteEntry($"Failure invoking {instance.Method.Name} with error {ex.ToString()}",
                        EventLogEntryType.Error);
                    // add this exception to the list
                    invocationExceptions.Add(ex);
                }
            }
            // if we caught any exceptions along the way, throw our
            // wrapper exception with all of them in it.
            if (invocationExceptions.Count > 0)
            {
                throw new MulticastInvocationException(invocationExceptions);
            }
        }
        #endregion

        #region "1.16 Using closures in C#"
        /// <summary>
        /// A class to represent the sales people of the world...
        /// </summary>
        class SalesPerson
        {
            #region CTOR
            public SalesPerson()
            {
            }

            public SalesPerson(string name,
                                decimal annualQuota,
                                decimal commissionRate)
            {
                this.Name = name;
                this.AnnualQuota = annualQuota;
                this.CommissionRate = commissionRate;
            }
            #endregion //CTOR

            #region Private Members
            decimal _commission;
            #endregion Private Members

            #region Properties
            public string Name { get; set; }

            public decimal AnnualQuota { get; set; }

            public decimal CommissionRate { get; set; }

            public decimal Commission
            {
                get { return _commission; }
                set
                {
                    _commission = value;
                    this.TotalCommission += _commission;
                }
            }

            public decimal TotalCommission { get; private set; }
            #endregion // Properties
        }

        delegate void CalculateEarnings(SalesPerson sp);

#pragma warning disable CSE0003 // Use expression-bodied members
        static CalculateEarnings GetEarningsCalculator(decimal quarterlySales,
                                                        decimal bonusRate)
        {
            return salesPerson =>
            {
                // figure out the sales person's quota for the quarter
                decimal quarterlyQuota = (salesPerson.AnnualQuota / 4);
                // did they make quota for the quarter?
                if (quarterlySales < quarterlyQuota)
                {
                    // didn't make quota, no commission
                    salesPerson.Commission = 0;
                }
                // check for bonus level performance (200% of quota)
                else if (quarterlySales > (quarterlyQuota * 2.0m))
                {
                    decimal baseCommission = quarterlyQuota * salesPerson.CommissionRate;
                    salesPerson.Commission = (baseCommission +
                            ((quarterlySales - quarterlyQuota) *
                            (salesPerson.CommissionRate * (1 + bonusRate))));
                }
                else // just regular commission
                {
                    salesPerson.Commission = salesPerson.CommissionRate * quarterlySales;
                }
            };
        }
#pragma warning restore CSE0003 // Use expression-bodied members


        public class QuarterlyEarning
        {
            public string Name { get; set; }
            public decimal Earnings { get; set; }
            public decimal Rate { get; set; }
        }

        public static void TestClosure()
        {
            // set up the sales people...
            SalesPerson[] salesPeople = {
                new SalesPerson { Name="Chas", AnnualQuota=100000m, CommissionRate=0.10m },
                new SalesPerson { Name="Ray", AnnualQuota=200000m, CommissionRate=0.025m },
                new SalesPerson { Name="Biff", AnnualQuota=50000m, CommissionRate=0.001m }};


            QuarterlyEarning[] quarterlyEarnings =
                           { new QuarterlyEarning(){ Name="Q1", Earnings = 65000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q2", Earnings = 20000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q3", Earnings = 37000m, Rate = 0.1m },
                             new QuarterlyEarning(){ Name="Q4", Earnings = 110000m, Rate = 0.15m } };

            var calculators = from e in quarterlyEarnings
                              select new
                              {
                                  Calculator =
                                      GetEarningsCalculator(e.Earnings, e.Rate),
                                  QuarterlyEarning = e
                              };

            decimal annualEarnings = 0;
            foreach (var c in calculators)
            {
                WriteQuarterlyReport(c.QuarterlyEarning.Name,
                    c.QuarterlyEarning.Earnings, c.Calculator, salesPeople);
                annualEarnings += c.QuarterlyEarning.Earnings;
            }

            // Let's see who is worth keeping...
            WriteCommissionReport(annualEarnings, salesPeople);

            //Console.ReadLine();
        }

        static void WriteQuarterlyReport(string quarter,
                                        decimal quarterlySales,
                                        CalculateEarnings eCalc,
                                        SalesPerson[] salesPeople)
        {
            Console.WriteLine($"{quarter} Sales Earnings on Quarterly Sales of {quarterlySales.ToString("C")}:");
            foreach (SalesPerson salesPerson in salesPeople)
            {
                // calc commission
                eCalc(salesPerson);
                // report
                Console.WriteLine($"\tSales person {salesPerson.Name} made a commission of : {salesPerson.Commission.ToString("C")}");
            }
        }

        static void WriteCommissionReport(decimal annualEarnings,
                                        SalesPerson[] salesPeople)
        {
            decimal revenueProduced = ((annualEarnings) / salesPeople.Length);
            Console.WriteLine("");
            Console.WriteLine($"Annual Earnings were {annualEarnings.ToString("C")}");
            Console.WriteLine("");
            var whoToCan = from salesPerson in salesPeople
                           select new
                           {
                               // if his commission is more than 20% 
                               // of what he produced can him
                               CanThem = (revenueProduced * 0.2m) <
                                           salesPerson.TotalCommission,
                               salesPerson.Name,
                               salesPerson.TotalCommission
                           };

            foreach (var salesPersonInfo in whoToCan)
            {
                Console.WriteLine($"\t\tPaid {salesPersonInfo.Name} {salesPersonInfo.TotalCommission.ToString("C")} to produce {revenueProduced.ToString("C")}");
                if (salesPersonInfo.CanThem)
                {
                    Console.WriteLine($"\t\t\tFIRE {salesPersonInfo.Name}!");
                }
            }
        }
        #endregion

        #region "1.17 Performing Multiple Operations on a List using Functors"
        public static void TestFunctors()
        {
            // No, none of these are real tickers...
            // OU81
            // C#4VR
            // PCKD
            // BTML
            // NOVB
            // MGDCD
            // GNRCS
            // FNCTR
            // LMBDA
            // PCLS
            StockPortfolio tech = new StockPortfolio() {
                {"OU81", -10.5},
                {"C#6VR", 2.0},
                {"PCKD", 12.3},
                {"BTML", 0.5},
                {"NOVB", -35.2},
                {"MGDCD", 15.7},
                {"GNRCS", 4.0},
                {"FNCTR", 9.16},
                {"LMBDA", 9.12},
                {"PCLS", 6.11}};

            tech.PrintPortfolio("Starting Portfolio");
            // sell the worst 3 performers
            var worstPerformers = tech.GetWorstPerformers(3);
            Console.WriteLine("Selling the worst performers:");
            worstPerformers.DisplayStocks();
            tech.SellStocks(worstPerformers);
            tech.PrintPortfolio("After Selling Worst 3 Performers");

            //Output:
            //Starting Portfolio
            //  (OU81) lost 10.5%
            //  (C#6VR) gained 2%
            //  (PCKD) gained 12.3%
            //  (BTML) gained 0.5%
            //  (NOVB) lost 35.2%
            //  (MGDCD) gained 15.7%
            //  (GNRCS) gained 4%
            //  (FNCTR) gained 9.16%
            //  (LMBDA) gained 9.12%
            //  (PCLS) gained 6.11%
            //Selling the worst performers:
            //  (NOVB) lost 35.2%
            //  (OU81) lost 10.5%
            //  (BTML) gained 0.5%
            //After Selling Worst 3 Performers
            //  (C#6VR) gained 2%
            //  (PCKD) gained 12.3%
            //  (MGDCD) gained 15.7%
            //  (GNRCS) gained 4%
            //  (FNCTR) gained 9.16%
            //  (LMBDA) gained 9.12%
            //  (PCLS) gained 6.11%        
        }

        public class Stock
        {
            public double GainLoss { get; set; }
            public string Ticker { get; set; }
        }
        
        public class StockPortfolio : IEnumerable
        {
            List<Stock> _stocks;

            public StockPortfolio()
            {
                _stocks = new List<Stock>();
            }

            public void Add(string ticker, double gainLoss)
            {
                _stocks.Add(new Stock() { Ticker = ticker, GainLoss = gainLoss });
            }

            public IEnumerable<Stock> GetWorstPerformers(int topNumber) => _stocks.OrderBy(
                            (Stock stock) => stock.GainLoss).Take(topNumber);

            public void SellStocks(IEnumerable<Stock> stocks)
            {
                foreach (Stock s in stocks)
                    _stocks.Remove(s);
            }

            public void PrintPortfolio(string title)
            {
                Console.WriteLine(title);
                _stocks.DisplayStocks();
            }

            #region IEnumerable<Stock> Members

            public IEnumerator<Stock> GetEnumerator() => _stocks.GetEnumerator();

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            #endregion
        }
        #endregion

        #region "1.18 Variations on Struct Initialization"
        public struct Data
        {
            public Data(int intData, float floatData = 1.1f, string strData = "a", char charData = 'a', bool boolData = true) : this()
            {
                IntData = intData;
                FloatData = floatData;
                StrData = strData;
                CharData = charData;
                BoolData = boolData;
            }

            public void Init()
            {
                IntData = 2;
                FloatData = 1.1f;
                StrData = "AA";
                CharData = 'A';
                BoolData = true;
            }

            //public Data(int intData, float floatData, string strData, char charData, bool boolData)
            //{
            //    IntData = intData;
            //    FloatData = floatData;
            //    StrData = strData;
            //    CharData = charData;
            //    BoolData = boolData;
            //}

            public int IntData { get; private set; }
            public float FloatData { get; private set; }
            public string StrData { get; private set; }
            public char CharData { get; private set; }
            public bool BoolData { get; private set; }

            public override string ToString() => IntData + " :: " + FloatData + " :: " + StrData + " :: " + CharData + " :: " + BoolData;
        }

        public class TestInitializeStructs
        {
            public static void UseNewInitialization()
            {
                Console.WriteLine("Using Init method to initialize");
                Data dat = new Data();
                dat.Init();
                Console.WriteLine(dat.ToString());
                Console.WriteLine();
                Console.WriteLine();


                Console.WriteLine("Using new to initialize");
                Data dat0 = new Data();
                Console.WriteLine(dat0.ToString());
                Console.WriteLine();
                Console.WriteLine();


                Console.WriteLine("Using new to initialize an array of Data structs");
                Data[] dat1 = new Data[4];
                foreach (Data d in dat1)
                {
                    Console.WriteLine(d.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();


                Console.WriteLine("Using new to initialize an ArrayList of new Data structs");
                ArrayList dat2 = new ArrayList();
                dat2.Add(new Data());
                dat2.Add(new Data());
                dat2.Add(new Data());
                dat2.Add(new Data());
                foreach (Data d in dat2)
                {
                    Console.WriteLine(d.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();


                Console.WriteLine("Using new to initialize an array of new Data structs");
                Data[] dat3 = new Data[4];
                dat3[0] = new Data();
                dat3[1] = new Data();
                dat3[2] = new Data();
                dat3[3] = new Data();
                foreach (Data d in dat3)
                {
                    Console.WriteLine(d.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();


                Console.WriteLine("Using new and LINQ to initialize an array of new Data structs");
                Data[] dataList = new Data[3];
                dataList = (from d in dataList   
                            select new Data()).ToArray();
                foreach (Data d in dataList)
                {
                    Console.WriteLine(d.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();


                Console.WriteLine("Using new and LINQ to initialize an array of new Data structs--using overloaded opt args ctor");
                Data[] dataList2 = new Data[3];
                dataList2 = (from d in dataList2
                            select new Data(2)).ToArray();
                foreach (Data d in dataList2)
                {
                    Console.WriteLine(d.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            public static void UseDefaultInitialization()
            {
                Console.WriteLine("Using default initialization");
                Data dat = default(Data);
                Console.WriteLine(dat.ToString());
                Console.WriteLine();
                Console.WriteLine();
            }

            public static void UseOverloadedctorInitialization()
            {
                Console.WriteLine("Using overloaded ctor to initialize");
                Data dat3 = new Data(2, 2.2f, "blank", 'a');
                Console.WriteLine(dat3.ToString());
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Using overloaded default ctor to initialize");
                Data dat4 = new Data(2);
                Console.WriteLine(dat4.ToString());
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        #endregion

        #region "1.19 A More Concise Way to Check For Null"
        public static bool TestForNull()
        {
            Console.WriteLine($"return value for null:  {CallMethodWithPossibleNull(null)}");
            //Null Exception:    Console.WriteLine($"return value for null.ToLower():  {CallMethodWithPossibleNull(null).ToLower()}");
            Console.WriteLine($"return value for null?.ToLower():  {CallMethodWithPossibleNull(null)?.ToLower()}");
            Console.WriteLine($"return value for non-null:  {CallMethodWithPossibleNull("not null")}");

            Console.WriteLine($"return value for non-null[null]:  {CallMethodWithPossibleNullArray(new string[1] { null })}");
            Console.WriteLine($"return value for null[]:  {CallMethodWithPossibleNullArray(null)}");
            //Null Exception:    Console.WriteLine($"return value for null[].ToLower():  {CallMethodWithPossibleNullArray(null).ToLower()}");
            Console.WriteLine($"return value for null[]?.ToLower():  {CallMethodWithPossibleNullArray(null)?.ToLower()}");
            Console.WriteLine($"return value for non-null[non-null]:  {CallMethodWithPossibleNullArray(new string[1] { "a" })}");

            return true;
        }

        public static string CallMethodWithPossibleNull(string val)
        {
            if (val?.Length > 0)
                Console.WriteLine("val.length > 0");
            else if (val?.Length == 0)
                Console.WriteLine("val.length = 0");
            else
                Console.WriteLine("val.length = null");  // Reached on val == null

            switch (val?.Length)
            {
                case 0:
                    Console.WriteLine("val.length = 0");
                    break;
                case 8:
                    Console.WriteLine("val.length = 8");
                    break;
                default:
                    Console.WriteLine("val.length = null");  // Reached on val == null
                    break;
            }

            string s = val?.ToLower();
            //byte[] b = new byte[val?.Length];   // Error: Cannot convert int? to int
            byte[] b = new byte[(val?.Length).GetValueOrDefault()];

            val?.Trim();
            val?.Trim().ToUpper();
            //Null Exception:    val.Trim()?.ToUpper();
            val?.Trim()?.ToUpper();

            int? len = val?.Length;

            return val;
        }
        public static string CallMethodWithPossibleNullArray(string[] val)
        {
            string retval = "";

            //Null Exception:    retval = val.GetLength(0).ToString();
            retval = val?.GetLength(0).ToString();
            retval = val?.GetLength(0).ToString()?.Trim();
            retval = val?.ToString()?.Trim();
            //Null Exception:    retval = val.ToString()?.Trim();

            //Null Exception:    retval = val[0].ToUpper();
            //Null Exception:    retval = val[0]?.ToUpper();
            //Null Exception:    retval = val[0]?.ToUpper()?.Trim();
            retval = val?[0]?.ToUpper();
            retval = val?[0]?.ToUpper()?.Trim();
            //Null Exception:    retval = val?[0].ToUpper();
            //Null Exception:    retval = val?[0].ToUpper()?.Trim();
            //Null Exception:    retval = val[0].ToUpper()?.Trim();

            int? len = val?.GetLength(0);

            return retval;
        }

        public static void CallMethodWithPossibleNullInt(int? val)
        {
            if (val?.ToString() == "1")
                Console.WriteLine();

        }

        public delegate bool Approval();

        public static void TestForNullDelegate()
        {
            Approval approvalDelegate = () => { return true; };
            Console.WriteLine($"Non-null delegate return value:  {approvalDelegate?.Invoke()}");
            Console.WriteLine($"Non-null delegate return value:  {approvalDelegate.Invoke()}");

            approvalDelegate = null;
            Console.WriteLine($"Null delegate return value:  {approvalDelegate?.Invoke()}");
            Console.WriteLine($"Null delegate return value:  {approvalDelegate?.Invoke().ToString()}");
            //Null Exception:    approvalDelegate.Invoke();
        }
        #endregion
    }
}
