using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using CSharpRecipes.Config;
using System.Data.Linq;
using CSharpRecipes.ProductsTableAdapters;
using System.IO;

namespace CSharpRecipes
{
    public static class LinqExtensions
    {
        #region 4.5 Adding Functional Extensions for Use with LINQ

        public static decimal? WeightedMovingAverage(this IEnumerable<decimal?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            decimal aggregate = 0.0M;
            decimal weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new decimal?(aggregate / count);
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<double?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new double?(aggregate / count);
            return null;
        }

        public static float? WeightedMovingAverage(this IEnumerable<float?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            float aggregate = 0.0F;
            float weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new float?(aggregate / count);
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<short?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new double?(aggregate / count);
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<int?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new double?(aggregate / count);
            return null;
        }

        public static double? WeightedMovingAverage(this IEnumerable<long?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            double weight;
            int item = 1;
            // count how many items are not null and use that
            // as the weighting factor
            int count = source.Count(val => val.HasValue);
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    weight = item / count;
                    aggregate += nullable.GetValueOrDefault() * weight;
                    count++;
                }
            }
            if (count > 0)
                return new double?(aggregate / count);
            return null;
        }

        public static decimal WeightedMovingAverage(this IEnumerable<decimal> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            decimal weight;
            decimal aggregate = 0.0M;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0M;
        }

        public static double WeightedMovingAverage(this IEnumerable<double> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0;
        }

        public static float WeightedMovingAverage(this IEnumerable<float> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            float weight;
            float aggregate = 0.0F;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0F;
        }

        public static double WeightedMovingAverage(this IEnumerable<short> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0;
        }

        public static double WeightedMovingAverage(this IEnumerable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0;
        }

        public static double WeightedMovingAverage(this IEnumerable<long> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double weight;
            double aggregate = 0.0;
            int item = 1;
            // use the count of the items from the source
            // as the weighting factor
            int count = source.Count();
            foreach (var value in source)
            {
                weight = item / count;
                aggregate += value * weight;
                item++;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0;
        }

        public static decimal? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, decimal?> selector) => 
                source.Select<TSource, decimal?>(selector).WeightedMovingAverage();

        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, double?> selector) => 
                source.Select<TSource, double?>(selector).WeightedMovingAverage();

        public static float? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, float?> selector) => 
                source.Select<TSource, float?>(selector).WeightedMovingAverage();

        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, short?> selector) => 
                source.Select<TSource, short?>(selector).WeightedMovingAverage();

        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source,
            Func<TSource, int?> selector) => 
                source.Select<TSource, int?>(selector).WeightedMovingAverage();

        public static double? WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, long?> selector) => 
                source.Select<TSource, long?>(selector).WeightedMovingAverage();

        public static decimal WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, decimal> selector) => 
                source.Select<TSource, decimal>(selector).WeightedMovingAverage();

        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, double> selector) => 
                source.Select<TSource, double>(selector).WeightedMovingAverage();

        public static float WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, float> selector) => 
                source.Select<TSource, float>(selector).WeightedMovingAverage();

        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, short> selector) => 
                source.Select<TSource, short>(selector).WeightedMovingAverage();

        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, int> selector) => 
                source.Select<TSource, int>(selector).WeightedMovingAverage();

        public static double WeightedMovingAverage<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, long> selector) => 
                source.Select<TSource, long>(selector).WeightedMovingAverage();


        #region Extend Average
        public static double? Average(this IEnumerable<short?> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            int count = 0;
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    aggregate += nullable.GetValueOrDefault();
                    count++;
                }
            }
            if (count > 0)
                return new double?(aggregate / count);
            return null;
        }
        public static double Average(this IEnumerable<short> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            double aggregate = 0.0;
            // use the count of the items from the source
            int count = source.Count();
            foreach (var value in source)
            {
                aggregate += value;
            }
            if (count > 0)
                return aggregate / count;
            else
                return 0.0;
        }
        public static double? Average<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, short?> selector) => 
                source.Select(selector).Average();

        public static double Average<TSource>(this IEnumerable<TSource> source,
            Func<TSource, short> selector) => 
                source.Select<TSource, short>(selector).Average();

        #endregion // Extend Average
        #endregion // 4.5
    }

    public static class LINQAndLambda
    {
        #region "4.0 Introduction"
        // declare the delegate
        public delegate int IncreaseByANumber(int j);
        // Defines a delegate for multiple
        public delegate int MultipleIncreaseByANumber(int j, int k, int l);

        // set up a method to implement the delegate functionality
        static public int MultiplyByANumber(int j) => j * 42;

        public static void ExecuteCSharp1_0()
        {
            // create the delegate instance
            IncreaseByANumber increase =
               new IncreaseByANumber(
                   LINQAndLambda.MultiplyByANumber);

            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));
        }

        public static void ExecuteCSharp2_0()
        {
            // create the delegate instance
            IncreaseByANumber increase =
               new IncreaseByANumber(
                delegate (int j)
                {
                    return j * 42;
                });

            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));
        }

        public static void ExecuteCSharp3_0()
        {
            // declare the lambda expression
            IncreaseByANumber increase = j => j * 42;
            // invoke the method and print 420 to the console
            Console.WriteLine(increase(10));

            MultipleIncreaseByANumber multiple = (j, k, l) => ((j * 42) / k) % l;
            Console.WriteLine(multiple(10, 11, 12));
        }
        #endregion

        #region "4.1 Query a Message Queue"

        public class EnumerableMessageQueue : MessageQueue, IEnumerable<Message>
        {
            public EnumerableMessageQueue() :
                base()
            { }
            public EnumerableMessageQueue(string path) : base(path) { }
            public EnumerableMessageQueue(string path, bool sharedModeDenyReceive) :
                base(path, sharedModeDenyReceive)
            { }
            public EnumerableMessageQueue(string path, QueueAccessMode accessMode) :
                base(path, accessMode)
            { }
            public EnumerableMessageQueue(string path, bool sharedModeDenyReceive, bool enableCache) :
                base(path, sharedModeDenyReceive, enableCache)
            { }
            public EnumerableMessageQueue(string path, bool sharedModeDenyReceive,
                bool enableCache, QueueAccessMode accessMode) :
                    base(path, sharedModeDenyReceive, enableCache, accessMode)
            { }

            public static new EnumerableMessageQueue Create(string path) => Create(path, false);

            public static new EnumerableMessageQueue Create(string path, bool transactional)
            {
                // Use MessageQueue directly to make sure the queue exists
                if (!MessageQueue.Exists(path))
                    MessageQueue.Create(path, transactional);
                // create the enumerable queue once we know it is there
                return new EnumerableMessageQueue(path);
            }

            public new MessageEnumerator GetMessageEnumerator()
            {
                throw new NotSupportedException("Please use GetEnumerator");
            }

            public new MessageEnumerator GetMessageEnumerator2()
            {
                throw new NotSupportedException("Please use GetEnumerator");
            }

            IEnumerator<Message> IEnumerable<Message>.GetEnumerator()
            {
                //NOTE: In .NET 3.5, you used to be able to call "GetEnumerator" on MessageQueue
                //via normal LINQ semantics and have it work.  Now we have to call GetMessageEnumerator2 
                //as GetEnumerator has been deprecated.  
                //Now we use EnumerableMessageQueue which deals with this for us...
                MessageEnumerator messageEnumerator = base.GetMessageEnumerator2();
                while (messageEnumerator.MoveNext())
                {
                    yield return messageEnumerator.Current;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                //NOTE: In .NET 3.5, you used to be able to call "GetEnumerator" on MessageQueue
                //via normal LINQ semantics and have it work.  Now we have to call GetMessageEnumerator2 
                //as GetEnumerator has been deprecated.  
                //Now we use EnumerableMessageQueue which deals with this for us...
                MessageEnumerator messageEnumerator = base.GetMessageEnumerator2();
                while (messageEnumerator.MoveNext())
                {
                    yield return messageEnumerator.Current;
                }
            }
        }

        public static void TestLinqMessageQueue()
        {
            try
            {
                // create and populate a queue
                string queuePath = @".\private$\LINQMQ";
                EnumerableMessageQueue messageQueue = null;
                if (!EnumerableMessageQueue.Exists(queuePath))
                    messageQueue = EnumerableMessageQueue.Create(queuePath);
                else
                    messageQueue = new EnumerableMessageQueue(queuePath);

                using (messageQueue)
                {
                    BinaryMessageFormatter messageFormatter = new BinaryMessageFormatter();
                    Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.Contains("D")).ToArray();
                    for (int i = 0; i < 10; i++)
                    {
                        Message msg = new Message();
                        // label our message
                        msg.Label = i.ToString();
                        // override the default XML formatting with binary
                        // as it is faster (at the expense of legibility while debugging)
                        msg.Formatter = messageFormatter;
                        // make this message persist (causes message to be written
                        // to disk)
                        msg.Recoverable = true;
                        if (i < types.Length)
                            msg.Body = types[i].ToString();
                        else
                            msg.Body = types[i - types.Length].ToString();
                        messageQueue.Send(msg);
                    }

                    // Query the message queue for specific messages with the following criteria:
                    // 1) the label must be less than 5
                    // 2) the name of the type in the message body must contain 'CSharpRecipes.D'
                    // 3) the results should be in descending order by type name (from the body)

                    var query = from Message msg in messageQueue
                                    // The first assignment to msg.Formatter is so that we can touch the
                                    // Message object.  It assigns the BinaryMessageFormatter  to each message
                                    // instance so that it can be read to determine if it matches the criteria.
                                    // This is done and then checks that the formatter was correctly assigned 
                                    // by performing an equality check which satisfies the Where clause's need
                                    // for a boolean result while still executing the assignment of the formatter.
                                where ((msg.Formatter = messageFormatter) == messageFormatter) &&
                                    int.Parse(msg.Label) < 5 &&
                                    msg.Body.ToString().Contains("CSharpRecipes.D")
                                orderby msg.Body.ToString() descending
                                select msg;

                    // check our results for messages with a label > 5 and containing a 'D' in the name
                    foreach (var msg in query)
                        Console.WriteLine($"Label: {msg.Label}" +
                            $" Body: {msg.Body}");

                    // clean up the queue
                    messageQueue.Purge();
                }
                // remove the queue
                EnumerableMessageQueue.Delete(queuePath);
            }
            catch(InvalidOperationException ex)
            {
                // If you get this you may not have the message queue infrastructure installed
                // You can install the Message Queuing items in Add/Remove Programs under Windows components
                Console.WriteLine("Check to see if message queuing is installed: " + ex.Message);
            }
            // RESULTS look like this:
            //Label: 2 Body: CSharpRecipes.DebuggingAndExceptionHandlingExtensions
            //Label: 3 Body: CSharpRecipes.DebuggingAndExceptionHandling
            //Label: 1 Body: CSharpRecipes.DataTypes
            //Label: 0 Body: CSharpRecipes.DataTypeExtMethods
        }
        #endregion

        #region "4.2 Using Set Semantics with Data"
        public class Employee
        {
            public string Name { get; set; }
            public override string ToString() => this.Name;
            public override bool Equals(object obj) => 
                this.GetHashCode().Equals(obj.GetHashCode());
            public override int GetHashCode() => this.Name.GetHashCode();
        }

        public static void TestSetSemantics()
        {
            // Distinct
            string[] dailySecurityLog = {
                    "Rakshit logged in",
                    "Aaron logged in",
                    "Rakshit logged out",
                    "Ken logged in",
                    "Rakshit logged in",
                    "Mahesh logged in",
                    "Jesse logged in",
                    "Jason logged in",
                    "Josh logged in",
                    "Melissa logged in",
                    "Rakshit logged out",
                    "Mary-Ellen logged out",
                    "Mahesh logged in",
                    "Alex logged in",
                    "Scott logged in",
                    "Aaron logged out",
                    "Jesse logged out",
                    "Scott logged out",
                    "Dave logged in",
                    "Ken logged out",
                    "Alex logged out",
                    "Rakshit logged in",
                    "Dave logged out",
                    "Josh logged out",
                    "Jason logged out"};

            // Distinct
            IEnumerable<string> whoLoggedIn =
                dailySecurityLog.Where(logEntry => logEntry.Contains("logged in")).Distinct();
            Console.WriteLine("Everyone who logged in today:");
            foreach (string who in whoLoggedIn)
                Console.WriteLine(who);


            Employee[] project1 = {
                        new Employee(){ Name = "Rakshit" },
                        new Employee(){ Name = "Jason" },
                        new Employee(){ Name = "Josh" },
                        new Employee(){ Name = "Melissa" },
                        new Employee(){ Name = "Aaron" },
                        new Employee() { Name = "Dave" },
                        new Employee() {Name = "Alex" } };
            Employee[] project2 = {
                        new Employee(){ Name = "Mahesh" },
                        new Employee() {Name = "Ken" },
                        new Employee() {Name = "Jesse" },
                        new Employee(){ Name = "Melissa" },
                        new Employee(){ Name = "Aaron" },
                        new Employee(){ Name = "Alex" },
                        new Employee(){ Name = "Mary-Ellen" } };
            Employee[] project3 = {
                        new Employee(){ Name = "Mike" },
                        new Employee(){ Name = "Scott" },
                        new Employee(){ Name = "Melissa" },
                        new Employee(){ Name = "Aaron" },
                        new Employee(){ Name = "Alex" },
                        new Employee(){ Name = "Jon" } };


            // Union
            Console.WriteLine("Employees for all projects");
            var allProjectEmployees = project1.Union(project2.Union(project3));
            foreach (Employee employee in allProjectEmployees)
                Console.WriteLine(employee);

            // Intersect
            Console.WriteLine("Employees on every project");
            var everyProjectEmployees = project1.Intersect(project2.Intersect(project3));
            foreach (Employee employee in everyProjectEmployees)
                Console.WriteLine(employee);

            // Except
            var intersect1_3 = project1.Intersect(project3);
            var intersect1_2 = project1.Intersect(project2);
            var intersect2_3 = project2.Intersect(project3);
            var unionIntersect = intersect1_2.Union(intersect1_3).Union(intersect2_3);

            Console.WriteLine("Employees on only one project");
            var onlyProjectEmployees = allProjectEmployees.Except(unionIntersect);
            foreach (Employee employee in onlyProjectEmployees)
                Console.WriteLine(employee);
        }
        #endregion

        #region "4.3 Reuse Parameterized Queries with LINQ to SQL"
        public static void TestCompiledQuery()
        {
            //NOTE: Using CompiledQuery is not possible with EF5 and EF6 as it was redone to use DbContext, not ObjectContext and 
            // CompiledQuery.Compile requires an ObjectContext.  If you are using EF below 5 this will still work but if not
            // and you are using LINQ to SQL, you can still use the LINQ to SQL data context  
            // Microsoft recommends using a DbContext in new development, but for people who have existing code on prior data
            // access mechanisms, CompiledQuery can still help!
            //http://blogs.msdn.com/b/adonet/archive/2012/02/14/sneak-preview-entity-framework-5-0-performance-improvements.aspx
            var GetEmployees =
                CompiledQuery.Compile((NorthwindLinq2Sql.NorthwindLinq2SqlDataContext nwdc, string ac, string ttl) =>
                        from employee in nwdc.Employees
                            //where employee.HomePhone.Contains(string.Format("(\{ac})")) &&  //THROWS NOT SUPPORTED EXCEPTION BELOW
                        where employee.HomePhone.Contains(ac) &&
                                employee.Title == ttl
                        select employee);

            //System.NotSupportedException: Method 'System.String Format(System.String, System.Object)' has no supported translation to SQL.
            //   at System.Data.Linq.SqlClient.SqlMethodCallConverter.VisitMethodCall(SqlMethodCall mc)

            // This LINQ to SQL type is in another assembly or we get "The mapping of CLR type to EDM type is ambiguous because multiple CLR types match the EDM type"
            // as it conflicts with the names in NorthwindEntitiy and EF only goes by Name, not the full type name
            var northwindDataContext = new NorthwindLinq2Sql.NorthwindLinq2SqlDataContext();
            northwindDataContext.Log = Console.Out;

            foreach (var employee in GetEmployees(northwindDataContext, "(206)", "Sales Representative"))
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");

            foreach (var employee in GetEmployees(northwindDataContext, "(71)", "Sales Manager"))
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
        }

        #endregion

        #region "4.4 Sort Results in a Culture-Sensitive Manner"	
        public static void TestLinqForCulture()
        {

            // The Danish language treats the character "Æ" as an individual letter, 
            // sorting it after "Z" in the alphabet. 
            // The English language treats the character "Æ" as a 
            // special symbol, sorting it before the letter "A" in the alphabet.
            string[] names = { "Jello", "Apple", "Bar", "Æble", "Forsooth", "Orange", "Zanzibar" };

            // Create CultureInfo for Danish in Denmark.
            CultureInfo danish = new CultureInfo("da-DK");
            // Create CultureInfo for English in the U.S.
            CultureInfo american = new CultureInfo("en-US");

            CultureStringComparer comparer = new CultureStringComparer(danish, CompareOptions.None);
            var query = names.OrderBy(n => n, comparer);
            Console.WriteLine($"Ordered by specific culture : {comparer.CurrentCultureInfo.Name}");
            foreach (string name in query)
                Console.WriteLine(name);

            comparer.CurrentCultureInfo = american;
            query = names.OrderBy(n => n, comparer);
            Console.WriteLine($"Ordered by specific culture : {comparer.CurrentCultureInfo.Name}");
            foreach (string name in query)
                Console.WriteLine(name);


            query = from n in names
                    orderby n
                    select n;
            Console.WriteLine("Ordered by Thread.CurrentThread.CurrentCulture : " +
                $"{ Thread.CurrentThread.CurrentCulture.Name}");
            foreach (string name in query)
                Console.WriteLine(name);


            // RESULTS look like this:
            //Ordered by specific culture : da-DK
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
            //Æble
            //Ordered by specific culture : en-US
            //Æble
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
            //Ordered by Thread.CurrentThread.CurrentCulture : en-US
            //Æble
            //Apple
            //Bar
            //Forsooth
            //Jello
            //Orange
            //Zanzibar
        }

        public class CultureStringComparer : IComparer<string>
        {
            private CultureStringComparer()
            {
            }

            public CultureStringComparer(CultureInfo cultureInfo, CompareOptions options)
            {
                if (cultureInfo == null)
                    throw new ArgumentNullException(nameof(cultureInfo));

                CurrentCultureInfo = cultureInfo;
                Options = options;
            }

            public int Compare(string x, string y) => 
                CurrentCultureInfo.CompareInfo.Compare(x, y, Options);

            public CultureInfo CurrentCultureInfo { get; set; }

            public CompareOptions Options { get; set; }
        }

        #endregion

        #region "4.5 Adding Functional Extensions for Use with LINQ"
        public static void TestWeightedMovingAverage()
        {
            decimal[] prices = new decimal[10] { 13.5M, 17.8M, 92.3M, 0.1M, 15.7M, 19.99M, 9.08M, 6.33M, 2.1M, 14.88M };
            Console.WriteLine(prices.WeightedMovingAverage());
            Console.WriteLine(prices.Average());

            double[] dprices = new double[10] { 13.5, 17.8, 92.3, 0.1, 15.7, 19.99, 9.08, 6.33, 2.1, 14.88 };
            Console.WriteLine(dprices.WeightedMovingAverage());
            Console.WriteLine(dprices.Average());

            float[] fprices = new float[10] { 13.5F, 17.8F, 92.3F, 0.1F, 15.7F, 19.99F, 9.08F, 6.33F, 2.1F, 14.88F };
            Console.WriteLine(fprices.WeightedMovingAverage());
            Console.WriteLine(fprices.Average());

            int[] iprices = new int[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(iprices.WeightedMovingAverage());
            Console.WriteLine(iprices.Average());

            long[] lprices = new long[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(lprices.WeightedMovingAverage());
            Console.WriteLine(lprices.Average());

            short?[] sprices = new short?[10] { 13, 17, 92, 0, 15, 19, 9, 6, 2, 14 };
            Console.WriteLine(sprices.WeightedMovingAverage());
            // System.Linq.Extensions doesn't implement Average for short but we do for them!
            Console.WriteLine(sprices.Average());
        }
        
        #endregion

        #region "4.6 Query and Join Across Data Repositories"
        public static void TestLinqToDataSet()
        {
            // LINQ to SQL
            var dataContext = new NorthwindLinq2Sql.NorthwindLinq2SqlDataContext();
            var categoryData = (from c in dataContext.Categories
                                select new
                                {
                                    CategoryId = c.CategoryID,
                                    Name = c.CategoryName,
                                    Description = c.Description
                                }).ToList(); // Without ToList you get "Only parameterless constructors and initializers are supported in LINQ to Entities."
            var categories = new XElement("Categories",
                                from cd in categoryData
                                select new XElement("Category",
                                   new XAttribute("Id", cd.CategoryId),
                                   new XAttribute("Name", cd.Name),
                                   new XAttribute("Description", cd.Description)));

            using (XmlWriter writer = XmlWriter.Create("Categories.xml"))
            {
                categories.WriteTo(writer);
            }


            XElement xmlCategories = XElement.Load("Categories.xml");


            ProductsTableAdapter adapter = new ProductsTableAdapter();
            Products products = new Products();
            adapter.Fill(products._Products);

            var expr = from product in products._Products
                       where product.UnitsInStock > 100
                       join xc in xmlCategories.Elements("Category")
                       on product.CategoryID equals int.Parse(xc.Attribute("Id").Value)
                       select new
                       {
                           ProductName = product.ProductName,
                           Category = xc.Attribute("Name").Value,
                           CategoryDescription = xc.Attribute("Description").Value
                       };

            foreach (var productInfo in expr)
            {
                Console.WriteLine($"ProductName: {productInfo.ProductName}" +
                    $" Category: {productInfo.Category}" +
                    $" Category Description: {productInfo.CategoryDescription}");
            }

            //OUTPUT
            //ProductName: Grandma's Boysenberry Spread Category: Condiments Category Description: Sweet and savory sauces, relishes, spreads, and seasonings
            //ProductName: Gustaf's Knäckebröd Category: Grains/Cereals Category Description: Breads, crackers, pasta, and cereal
            //ProductName: Geitost Category: Dairy Products Category Description: Cheeses
            //ProductName: Sasquatch Ale Category: Beverages Category Description: Soft drinks, coffees, teas, beer, and ale
            //ProductName: Inlagd Sill Category: Seafood Category Description: Seaweed and fish
            //ProductName: Boston Crab Meat Category: Seafood Category Description: Seaweed and fish
            //ProductName: Pâté chinois Category: Meat/Poultry Category Description: Prepared meats
            //ProductName: Sirop d'érable Category: Condiments Category Description: Sweet and savory sauces, relishes, spreads, and seasonings
            //ProductName: Röd Kaviar Category: Seafood Category Description: Seaweed and fish
            //ProductName: Rhönbräu Klosterbier Category: Beverages Category Description: Soft drinks, coffees, teas, beer, and ale
        }
        #endregion 

        #region "4.7 Querying Configuration Files with LINQ"
        public static void TestQueryConfig()
        {
            CSharpRecipesConfigurationSection recipeConfig =
                ConfigurationManager.GetSection("CSharpRecipesConfiguration") as CSharpRecipesConfigurationSection;

            var expr = from ChapterConfigurationElement chapter in recipeConfig.Chapters.OfType<ChapterConfigurationElement>()
                       where (chapter.Title.Contains("and")) && ((int.Parse(chapter.Number) % 2) == 0)
                       select new
                       {
                           ChapterNumber = $"Chapter {chapter.Number}",
                           chapter.Title
                       };

            foreach (var chapterInfo in expr)
                Console.WriteLine($"{chapterInfo.ChapterNumber} : {chapterInfo.Title}");


            System.Configuration.Configuration machineConfig =
                ConfigurationManager.OpenMachineConfiguration();

            var query = from ConfigurationSection section in machineConfig.Sections.OfType<ConfigurationSection>()
                        where section.SectionInformation.RequirePermission
                        select section;

            foreach (ConfigurationSection section in query)
                Console.WriteLine(section.SectionInformation.Name);

        }
        #endregion

        #region "4.8 Creating XML Straight from a Database"
        public static void TestXmlFromDatabase()
        {
            NorthwindEntities dataContext = new NorthwindEntities();

            // Log the generated SQL to the console
            dataContext.Database.Log = Console.WriteLine;

            // select the top 5 customers whose contact is the owner and
            // those owners placed orders spending more than $10000 this year 

            // Generated SQL for query - output via DataContext.Log
            //SELECT [t10].[CompanyName], [t10].[ContactName], [t10].[Phone], [t10].[TotalSpend]
            //FROM (
            //    SELECT TOP (5) [t0].[Company Name] AS [CompanyName], [t0].[Contact Name] AS
            //[ContactName], [t0].[Phone], [t9].[value] AS [TotalSpend]
            //    FROM [Customers] AS [t0]
            //    OUTER APPLY (
            //        SELECT COUNT(*) AS [value]
            //        FROM [Orders] AS [t1]
            //        WHERE [t1].[Customer ID] = [t0].[Customer ID]
            //        ) AS [t2]
            //    OUTER APPLY (
            //        SELECT SUM([t8].[value]) AS [value]
            //        FROM (
            //            SELECT [t3].[Customer ID], [t6].[Order ID], 
            //                ([t7].[Unit Price] * 
            //                (CONVERT(Decimal(29,4),[t7].[Quantity]))) - ([t7].[Unit Price] * 
            //                    (CONVERT(Decimal(29,4),[t7].[Quantity])) * 
            //                        (CONVERT(Decimal(29,4),[t7].[Discount]))) AS [value], 
            //                [t7].[Order ID] AS [Order ID2], 
            //                [t3].[Contact Title] AS [ContactTitle], 
            //                [t5].[value] AS [value2], 
            //                [t6].[Customer ID] AS [CustomerID]
            //            FROM [Customers] AS [t3]
            //            OUTER APPLY (
            //                SELECT COUNT(*) AS [value]
            //                FROM [Orders] AS [t4]
            //                WHERE [t4].[Customer ID] = [t3].[Customer ID]
            //                ) AS [t5]
            //            CROSS JOIN [Orders] AS [t6]
            //            CROSS JOIN [Order Details] AS [t7]
            //            ) AS [t8]
            //        WHERE ([t0].[Customer ID] = [t8].[Customer ID]) AND ([t8].[Order ID] = [
            //t8].[Order ID2]) AND ([t8].[ContactTitle] LIKE @p0) AND ([t8].[value2] > @p1) AN
            //D ([t8].[CustomerID] = [t8].[Customer ID])
            //        ) AS [t9]
            //    WHERE ([t9].[value] > @p2) AND ([t0].[Contact Title] LIKE @p3) AND ([t2].[va
            //lue] > @p4)
            //    ORDER BY [t9].[value] DESC
            //    ) AS [t10]
            //ORDER BY [t10].[TotalSpend] DESC
            //-- @p0: Input String (Size = 0; Prec = 0; Scale = 0) [%Owner%]
            //-- @p1: Input Int32 (Size = 0; Prec = 0; Scale = 0) [0]
            //-- @p2: Input Decimal (Size = 0; Prec = 29; Scale = 4) [10000]
            //-- @p3: Input String (Size = 0; Prec = 0; Scale = 0) [%Owner%]
            //-- @p4: Input Int32 (Size = 0; Prec = 0; Scale = 0) [0]
            //-- Context: SqlProvider(SqlCE) Model: AttributedMetaModel Build: 3.5.20706.1

            var bigSpenders = new XElement("BigSpenders",
                        from top5 in
                            (
                                (from customer in
                                        (
                                            from c in dataContext.Customers
                                                // get the customers where the contact is the owner 
                                                // and they placed orders
                                            where c.ContactTitle.Contains("Owner")
                                            && c.Orders.Count > 0
                                            join orderData in
                                                (
                                                    from c in dataContext.Customers
                                                        // get the customers where the contact is the owner 
                                                        // and they placed orders
                                                    where c.ContactTitle.Contains("Owner")
                                                    && c.Orders.Count > 0
                                                    from o in c.Orders
                                                        // get the order details
                                                    join od in dataContext.Order_Details
                                                        on o.OrderID equals od.OrderID
                                                    select new
                                                    {
                                                        c.CompanyName,
                                                        c.CustomerID,
                                                        o.OrderID,
                                                        // have to calc order value from orderdetails
                                                        //(UnitPrice*Quantity as Total)- (Total*Discount) 
                                                        // as NetOrderTotal
                                                        NetOrderTotal = (
                                                            (((double)od.UnitPrice) * od.Quantity) -
                                                            ((((double)od.UnitPrice) * od.Quantity) * od.Discount))
                                                    }
                                                )
                                            on c.CustomerID equals orderData.CustomerID
                                            into customerOrders
                                            select new
                                            {
                                                c.CompanyName,
                                                c.ContactName,
                                                c.Phone,
                                                // Get the total amount spent by the customer
                                                TotalSpend = customerOrders.Sum(order => order.NetOrderTotal)
                                            }
                                        )
                                     // only worry about customers that spent > 10000
                                 where customer.TotalSpend > 10000
                                 orderby customer.TotalSpend descending
                                 // only take the top 5 spenders
                                 select new
                                 {
                                     CompanyName = customer.CompanyName,
                                     ContactName = customer.ContactName,
                                     Phone = customer.Phone,
                                     TotalSpend = customer.TotalSpend
                                 }).Take(5)
                            ).ToList()  // Without ToList you get "Only parameterless constructors and initializers are supported in LINQ to Entities."
                                        // format the data as XML
                        select new XElement("Customer",
                                    new XAttribute("companyName", top5.CompanyName),
                                    new XAttribute("contactName", top5.ContactName),
                                    new XAttribute("phoneNumber", top5.Phone),
                                    new XAttribute("amountSpent", top5.TotalSpend)));

            //var bigSpenders =
            //            (from customer in
            //                 (
            //                     from c in dataContext.Customers
            //                     where c.ContactTitle.Contains("Owner") && c.Orders.Count > 0
            //                     join orderData in
            //                         (
            //                             from c in dataContext.Customers
            //                             where c.ContactTitle.Contains("Owner") && c.Orders.Count > 0
            //                             from o in c.Orders
            //                             join od in dataContext.Order_Details
            //                                 on o.OrderID equals od.OrderID
            //                             select new
            //                             {
            //                                 c.CompanyName,
            //                                 c.CustomerID,
            //                                 o.OrderID,
            //                                 NetOrderTotal = (
            //                                     (od.UnitPrice * (decimal)od.Quantity) -
            //                                     ((od.UnitPrice * (decimal)od.Quantity) *
            //                                     (decimal)od.Discount))
            //                             }
            //                         )
            //                     on c.CustomerID equals orderData.CustomerID
            //                     into customerOrders
            //                     select new
            //                     {
            //                         c.CompanyName,
            //                         c.ContactName,
            //                         c.Phone,
            //                         TotalNetOrders = customerOrders.Sum(order => order.NetOrderTotal)
            //                     }
            //                 )
            //             where customer.TotalNetOrders > 10000
            //             orderby customer.TotalNetOrders descending
            //             select customer).Take(5);

            using (XmlWriter writer = XmlWriter.Create("BigSpenders.xml"))
            {
                bigSpenders.WriteTo(writer);
            }

            //<BigSpenders>
            //  <Customer companyName="Folk och fä HB" contactName="Maria Larsson" 
            //            phoneNumber="0695-34 67 21" amountSpent="29567.562475292383" />
            //  <Customer companyName="White Clover Markets" contactName="Karl Jablonski" 
            //            phoneNumber="(206) 555-4112" amountSpent="27363.604972146455" />
            //  <Customer companyName="Bon app'" contactName="Laurence Lebihan" 
            //            phoneNumber="91.24.45.40" amountSpent="21963.252474311179" />
            //  <Customer companyName="Simons bistro" contactName="Jytte Petersen" 
            //            phoneNumber="31 12 34 56" amountSpent="16817.097494864836" />
            //  <Customer companyName="LINO-Delicateses" contactName="Felipe Izquierdo" 
            //            phoneNumber="(8) 34-56-12" amountSpent="16476.564986549318" />
            //</BigSpenders>

            Console.WriteLine(bigSpenders.ToString());
        }
        #endregion

        #region "4.9 Being Selective About Your Query Results"
        public static void TestTakeSkipWhile()
        {
            //Problem: You want to be able to get a dynamic subset of a query result

            //NOTE: TakeWhile and SkipWhile are not available in LINQ to SQL
            NorthwindEntities dataContext = new NorthwindEntities();

            // find the products for all suppliers
            var query =
                dataContext.Suppliers.GroupJoin(dataContext.Products,
                    s => s.SupplierID, p => p.SupplierID,
                    (s, products) => new
                    {
                        s.CompanyName,
                        s.ContactName,
                        s.Phone,
                        Products = products
                    }).OrderByDescending(supplierData => supplierData.Products.Count());

            var results = query.AsEnumerable().TakeWhile(supplierData => supplierData.Products.Count() > 3);
            //.SkipWhile(supplierData =>
            Console.WriteLine($"Suppliers that provide more than three products: {results.Count()}");
            foreach (var supplierData in results)
            {
                Console.WriteLine($"    Company Name : {supplierData.CompanyName}");
                Console.WriteLine($"    Contact Name : {supplierData.ContactName}");
                Console.WriteLine($"    Contact Phone : {supplierData.Phone}");
                Console.WriteLine($"    Products Supplied : {supplierData.Products.Count()}");
                foreach (var productData in supplierData.Products)
                    Console.WriteLine($"        Product: {productData.ProductName}");
            }

            //OUTPUT
            //Suppliers that provide more than three products: 4
            //    Company Name : Pavlova, Ltd.
            //    Contact Name : Ian Devling
            //    Contact Phone : (03) 444-2343
            //    Products Supplied : 5
            //        Product: Pavlova
            //        Product: Alice Mutton
            //        Product: Carnarvon Tigers
            //        Product: Vegie-spread
            //        Product: Outback Lager
            //    Company Name : Plutzer Lebensmittelgroßmärkte AG
            //    Contact Name : Martin Bein
            //    Contact Phone : (069) 992755
            //    Products Supplied : 5
            //        Product: Rössle Sauerkraut
            //        Product: Thüringer Rostbratwurst
            //        Product: Wimmers gute Semmelknödel
            //        Product: Rhönbräu Klosterbier
            //        Product: Original Frankfurter grüne Soße
            //    Company Name : New Orleans Cajun Delights
            //    Contact Name : Shelley Burke
            //    Contact Phone : (100) 555-4822
            //    Products Supplied : 4
            //        Product: Chef Anton's Cajun Seasoning
            //        Product: Chef Anton's Gumbo Mix
            //        Product: Louisiana Fiery Hot Pepper Sauce
            //        Product: Louisiana Hot Spiced Okra
            //    Company Name : Specialty Biscuits, Ltd.
            //    Contact Name : Peter Wilson
            //    Contact Phone : (161) 555-4448
            //    Products Supplied : 4
            //        Product: Teatime Chocolate Biscuits
            //        Product: Sir Rodney's Marmalade
            //        Product: Sir Rodney's Scones
            //        Product: Scottish Longbreads
        }
        #endregion

        #region "4.10 Using LINQ with Collections that Don’t Support IEnumerable<T>"	
        public static void TestUsingNonIEnumT()
        {
            //IEnumerable
            //    ArrayList
            //    BitArray
            //    Hashtable
            //    Queue
            //    SortedList
            //    Stack
            //    System.Net.CredentialCache
            //    XmlNodeList
            //    XPathNodeIterator

            //ICollection
            //    System.Diagnostics.Eventlogentrycollection
            //    System.Net.CookieCollection
            //    System.Security.AccessControl.GenericAcl
            //    System.Security.PermissionSet

            // Make some XML with some types that you can use with LINQ 
            // that don't support IEnumerable<T> directly
            XElement xmlFragment = new XElement("NonGenericLinqableTypes",
                                    new XElement("IEnumerable",
                                        new XElement("System.Collections",
                                            new XElement("ArrayList"),
                                            new XElement("BitArray"),
                                            new XElement("Hashtable"),
                                            new XElement("Queue"),
                                            new XElement("SortedList"),
                                            new XElement("Stack")),
                                        new XElement("System.Net",
                                            new XElement("CredentialCache")),
                                        new XElement("System.Xml",
                                            new XElement("XmlNodeList")),
                                        new XElement("System.Xml.XPath",
                                            new XElement("XPathNodeIterator"))),
                                    new XElement("ICollection",
                                        new XElement("System.Diagnostics",
                                            new XElement("EventLogEntryCollection")),
                                        new XElement("System.Net",
                                            new XElement("CookieCollection")),
                                        new XElement("System.Security.AccessControl",
                                            new XElement("GenericAcl")),
                                        new XElement("System.Security",
                                            new XElement("PermissionSet"))));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFragment.ToString());

            // Select the names of the nodes under IEnumerable that have children and are
            // named System.Collections and contain a capital S and return that list in descending order
            var query = from node in doc.SelectNodes("/NonGenericLinqableTypes/IEnumerable/*").Cast<XmlNode>()
                        where node.HasChildNodes &&
                            node.Name == "System.Collections"
                        from XmlNode xmlNode in node.ChildNodes
                        where xmlNode.Name.Contains('S')
                        orderby xmlNode.Name descending
                        select xmlNode.Name;

            foreach (string name in query)
                Console.WriteLine(name);

            EventLog log = new EventLog("Application");
            query = from EventLogEntry entry in log.Entries
                    where entry.EntryType == EventLogEntryType.Error &&
                        entry.TimeGenerated > DateTime.Now.Subtract(new TimeSpan(6, 0, 0))
                    select entry.Message;

            Console.WriteLine($"There were {query.Count<string>()}" +
                " Application Event Log error messages in the last 6 hours!");
            foreach (string message in query)
                Console.WriteLine(message);

            ArrayList stuff = new ArrayList();
            stuff.Add(DateTime.Now);
            stuff.Add(DateTime.Now);
            stuff.Add(1);
            stuff.Add(DateTime.Now);

            // Throws exception because not all items are DateTimes
            //var expr = from item in stuff.Cast<DateTime>()
            //           select item;
            //foreach (DateTime item in expr)
            //    Console.WriteLine(item);

            var expr = from item in stuff.OfType<DateTime>()
                       select item;
            foreach (DateTime item in expr)
                Console.WriteLine(item);
        }
        #endregion 

        #region "4.11 An Advanced Interface Search Mechanism"
        public static void FindSpecificInterfaces()
        {
            // set up the interfaces to search for
            Type[] interfaces = {
                typeof(System.ICloneable),
                typeof(System.Collections.ICollection),
                typeof(System.IAppDomainSetup) };

            // set up the type to examine
            Type searchType = typeof(System.Collections.ArrayList);

            var matches = from t in searchType.GetInterfaces()
                          join s in interfaces on t equals s
                          select s;

            Console.WriteLine("Matches found:");
            foreach (Type match in matches)
                Console.WriteLine(match.ToString());

            // A filter to search for all implemented interfaces that are defined 
            // within a particular namespace (in this case the System.Collections namespace):
            var collectionsInterfaces = from type in searchType.GetInterfaces()
                                        where type.Namespace == "System.Collections"
                                        select type;
            foreach (Type t in collectionsInterfaces)
                Console.WriteLine($"Implemented interface in System.Collections: {t}");

            // A filter to search for all implemented interfaces that contain a method called Add, 
            // which returns an Int32 value:
            var addInterfaces = from type in searchType.GetInterfaces()
                                from method in type.GetMethods()
                                where (method.Name == "Add") &&
                                        (method.ReturnType == typeof(int))
                                select type;
            foreach (Type t in addInterfaces)
                Console.WriteLine($"Implemented interface with int Add() method: {t}");

            // A filter to search for all implemented interfaces that are loaded from the 
            // Global Assembly Cache (GAC):
            var gacInterfaces = from type in searchType.GetInterfaces()
                                where type.Assembly.GlobalAssemblyCache
                                select type;
            foreach (Type t in gacInterfaces)
                Console.WriteLine($"Implemented interface loaded from GAC: {t}");

            // A filter to search for all implemented interfaces that are defined within an 
            // assembly with the version number 4.0.0.0:
            var versionInterfaces = from type in searchType.GetInterfaces()
                                    where type.Assembly.GlobalAssemblyCache &&
                                        type.Assembly.GetName().Version.Major == 4 &&
                                        type.Assembly.GetName().Version.Minor == 0 &&
                                        type.Assembly.GetName().Version.Build == 0 &&
                                        type.Assembly.GetName().Version.Revision == 0
                                    select type;
            foreach (Type t in versionInterfaces)
                Console.WriteLine($"Implemented interface from assembly with version 4.0.0.0: {t}");
        }

        #endregion

        #region "4.12 Using Lambda Expressions"
        public static void TestUsingLambdaExpressions()
        {
            OldWay ow = new OldWay();
            ow.WorkItOut();

            LambdaWay iw = new LambdaWay();
            iw.WorkItOut();

            DirectAssignmentWay diw = new DirectAssignmentWay();
            diw.WorkItOut();

            GenericWay gw = new GenericWay();
            gw.WorkItOut();

            GenericEventConsumer gec = new GenericEventConsumer();
            gec.Test();

            OuterVars ov = new OuterVars();
            ov.SeeOuterWork();

            Func<int, int> dwInt = i =>
            {
                Console.WriteLine(i);
                return i;
            };

        }

        class OuterVars
        {
            public void SeeOuterWork()
            {
                int count = 0;
                int total = 0;
                Func<int> countUp = () => count++;
                for (int i = 0; i < 10; i++)
                    total += countUp();
                Debug.WriteLine($"Total = {total}");
            }
        }

        class OldWay
        {
            // declare delegate
            delegate int DoWork(string work);

            // have a method to create an instance of and call the delegate
            public void WorkItOut()
            {
                // declare instance
                DoWork dw = new DoWork(DoWorkMethodImpl);
                // invoke delegate
                int i = dw("Do work the old way");
            }

            // Have a method that the delegate is tied to with a matching signature 
            // so that it is invoked when the delegate is called
            public int DoWorkMethodImpl(string s)
            {
                Console.WriteLine(s);
                return s.GetHashCode();
            }
        }

        class LambdaWay
        {
            // declare delegate
            delegate int DoWork(string work);

            // have a method to create an instance of and call the delegate
            public void WorkItOut()
            {
                // declare instance
                DoWork dw = s =>
                {
                    Console.WriteLine(s);
                    return s.GetHashCode();
                };
                // invoke delegate
                int i = dw("Do some inline work");
            }
        }

        class DirectAssignmentWay
        {
            // declare delegate
            delegate int DoWork(string work);

            // have a method to create an instance of and call the delegate
            public void WorkItOut()
            {
                // declare instance and assign method
                DoWork dw = DoWorkMethodImpl;
                // invoke delegate
                int i = dw("Do some direct assignment work");
            }
            // Have a method that the delegate is tied to with a matching signature 
            // so that it is invoked when the delegate is called
            public int DoWorkMethodImpl(string s)
            {
                Console.WriteLine(s);
                return s.GetHashCode();
            }
        }

        class GenericWay
        {
            // have a method to create two instances of and call the delegates
            public void WorkItOut()
            {
                Func<string, string> dwString = s =>
                {
                    Console.WriteLine(s);
                    return s;
                };

                // invoke string delegate
                string retStr = dwString("Do some generic work");

                Func<int, int> dwInt = i =>
                {
                    Console.WriteLine(i);
                    return i;
                };

                // invoke int delegate
                int j = dwInt(5);

            }
        }

        public class GenericEventArgs<T> : EventArgs
        {
            public GenericEventArgs(T value)
            {
                this.Value = value;
            }

            public T Value { get; set; }
        }

        public class GenericEvent
        {
            // declare generic events
            public event EventHandler<GenericEventArgs<string>> DoingStringWork;
            public event EventHandler<GenericEventArgs<int>> DoingIntWork;

            public void WorkItOut()
            {
                DoingStringWork(this, new GenericEventArgs<string>("String work"));
                DoingIntWork(this, new GenericEventArgs<int>(5));
            }
        }

        public class GenericEventConsumer
        {
            public void Test()
            {
                GenericEvent ge = new GenericEvent();
                ge.DoingStringWork += new EventHandler<GenericEventArgs<string>>(ge_DoingStringWork);
                ge.DoingIntWork += new EventHandler<GenericEventArgs<int>>(ge_DoingIntWork);
            }
            void ge_DoingIntWork(object sender, GenericEventArgs<int> workArgs)
            {
                throw new NotImplementedException();
            }
            void ge_DoingStringWork(object sender, GenericEventArgs<string> workArgs)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region "4.13 Using Different Parameter Modifiers in Lambda Expressions"
        public static void TestParameterModifiers()
        {
            ParameterMods pm = new ParameterMods();
            pm.WorkItOut();

            OldParams op = new OldParams();
            op.WorkItOut();

            OuterVariablesParameterModifiers ovpm = new OuterVariablesParameterModifiers();
            ovpm.TestParams();
        }

        class ParameterMods
        {
            // declare out delegate
            delegate int DoOutWork(out string work);
            // declare ref delegate
            delegate int DoRefWork(ref string work);
            // declare params delegate
            delegate int DoParamsWork(params object[] workItems);
            // declare simulated params delegate
            delegate int DoNonParamsWork(object[] workItems);

            // have a method to create an instance of and call the delegate
            public void WorkItOut()
            {
                // declare instance and assign method
                DoOutWork dow = (out string s) =>
                {
                    s = "WorkFinished";
                    Console.WriteLine(s);
                    return s.GetHashCode();
                };
                // invoke delegate
                string work;
                int i = dow(out work);
                Console.WriteLine(work);

                // declare instance and assign method
                DoRefWork drw = (ref string s) =>
                {
                    Console.WriteLine(s);
                    s = "WorkFinished";
                    return s.GetHashCode();
                };
                // invoke delegate
                work = "WorkStarted";
                i = drw(ref work);
                Console.WriteLine(work);

                ////Done as an lambda expression you also get CS1670 "params is not valid in this context"
                //DoParamsWork dpwl = (params object[] workItems) =>
                //{
                //    foreach (object o in workItems)
                //    {
                //        Console.WriteLine(o.ToString());
                //    }
                //    return workItems.GetHashCode();
                //};
                ////Done as an anonymous method you get CS1670 "params is not valid in this context"
                //DoParamsWork dpwa = delegate (params object[] workItems)
                //{
                //    foreach (object o in workItems)
                //    {
                //        Console.WriteLine(o.ToString());
                //    }
                //    return workItems.GetHashCode();
                //};


                // All we have to do is omit the params keyword.
                DoParamsWork dpw = workItems =>
                {
                    foreach (object o in workItems)
                        Console.WriteLine(o.ToString());
                    return workItems.GetHashCode();
                };

                i = dpw("Hello", "42", "bar");

                // Work around params not being valid by using object[]
                // as it gives an unbounded number of method parameters
                // we just don't get the benefit of having the compiler 
                // create the object array for us implicitly
                DoNonParamsWork dnpw = (object[] items) =>
                {
                    foreach (object o in items)
                        Console.WriteLine(o.ToString());
                    if (items.Length > 0)
                        return items[0].GetHashCode();
                    else
                        return -1;
                };
                // invoke delegate
                i = dnpw(new object[] { "WorkItem1", 5, 65.99, true });
            }
        }

        class OldParams
        {
            // declare delegate
            delegate int DoWork(params string[] workItems);

            // have a method to create an instance of and call the delegate
            public void WorkItOut()
            {
                // declare instance
                DoWork dw = new DoWork(DoWorkMethodImpl);
                string[] items = new string[3];
                items[0] = "item 0";
                items[1] = "item 1";
                items[2] = "item 2";
                // invoke delegate
                int i = dw(items);

                items = new string[1];
                items[0] = "item 0";
                // invoke delegate
                i = dw(items);

            }

            // Have a method that the delegate is tied to with a matching signature 
            // so that it is invoked when the delegate is called
            public int DoWorkMethodImpl(params string[] items)
            {
                foreach (string s in items)
                    Console.WriteLine(s);
                return items.GetHashCode();
            }
        }

        class OuterVariablesParameterModifiers
        {
            // declare delegate
            delegate int DoWork(string work);

            public void TestParams(params string[] items)
            {
                // declare instance
                DoWork dw = s =>
                {
                    Console.WriteLine(s);
                    foreach (string item in items)
                        Console.WriteLine(item);
                    return s.GetHashCode();
                };
                // invoke delegate
                int i = dw("DoWorkMethodImpl1");
            }

            //public void TestOut(out string outStr)
            //{
            //    // declare instance
            //    DoWork dw = s =>
            //    {
            //        Console.WriteLine(s);
            //        // Causes error CS1628: 
            //        // "Cannot use ref or out parameter 'outStr' inside an 
            //        // anonymous method, lambda expression, or query expression"
            //        outStr = s;
            //        return s.GetHashCode();
            //    };
            //    // invoke delegate
            //    int i = dw("DoWorkMethodImpl1");
            //}

            //public void TestRef(ref string refStr)
            //{
            //    // declare instance
            //    DoWork dw = s =>
            //    {
            //        Console.WriteLine(s);
            //        // Causes error CS1628: 
            //        // "Cannot use ref or out parameter 'refStr' inside an 
            //        // anonymous method, lambda expression, or query expression"
            //        refStr = s;
            //        return s.GetHashCode();
            //    };
            //    // invoke delegate
            //    int i = dw("DoWorkMethodImpl1");
            //}
        }

        #endregion

        #region "4.14 Speeding up LINQ operations with Parallelism"
        public static void TestPLINQ()
        {
            List<string> ingredients = new List<string>();
            using (StreamReader file = new StreamReader("IngredientList.txt"))
            {
                string line = string.Empty;
                while ((line = file.ReadLine()) != null)
                    ingredients.Add(line);
            }

            List<string> chapterTitles = new List<string>();
            using (StreamReader file = new StreamReader("RecipeChapters.txt"))
            {
                string line = string.Empty;
                while ((line = file.ReadLine()) != null)
                    chapterTitles.Add(line);
            }

            // Generate the recipe set
            Random rnd = new Random();
            List<RecipeChapter> chapters = new List<RecipeChapter>();
            for(int i = 0; i< chapterTitles.Count; i++)
            {
                RecipeChapter chapter = new RecipeChapter()
                {
                    Number = i + 1,
                    Title = chapterTitles[i],
                };
                List<Recipe> recipes = new List<Recipe>();
                for (int r = 1; r <= 10; r++)
                {
                    recipes.Add(new Recipe()
                    {
                        Chapter = chapter,
                        MainIngredient = ingredients[rnd.Next(0,ingredients.Count)],
                        Number = r,
                        Rank = rnd.Next(1, 5),
                        TextApproved = false,
                        IngredientsApproved = false,
                        RecipeEvaluated = 0,
                        FinalEditingComplete = false,
                    });
                }
                chapter.Recipes = recipes;
                chapters.Add(chapter);
            }

            chapters = chapters.OrderBy(c => c.Number).ToList();

            TimeSpan elapsed;
            Stopwatch watch = new Stopwatch();

            LogOutput("Running Cookbook Evaluation");

            // See how long it takes to make the pass in normal LINQ
            try
            {
                watch.Start();
                chapters.Select(c => TimedEvaluateChapter(c, rnd)).ToList();
            }
            finally
            {
                watch.Stop();
                elapsed = watch.Elapsed;
            }
            LogOutput("***********************************************");
            LogOutput($"Full Chapter Evaluation with LINQ took: {elapsed}");
            LogOutput("***********************************************");

            // reset the chapter collection for the PLINQ pass
            chapters = ResetCategories(chapters);
            watch.Reset();

            // See how long it takes to make the pass in PLINQ
            try
            {
                watch.Start();
                chapters.AsParallel().Select(c => TimedEvaluateChapter(c, rnd)).ToList();
            }
            finally
            {
                watch.Stop();
                elapsed = watch.Elapsed;
            }
            LogOutput("***********************************************");
            LogOutput($"Full Chapter Evaluation with PLINQ took: {elapsed}");
            LogOutput("***********************************************");

            LogOutput($"Cookbook Evaluation Complete");

        }

        private static void LogOutput(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        private static RecipeChapter TimedEvaluateChapter(RecipeChapter rc, Random rnd)
        {
            Stopwatch watch = new Stopwatch();
            LogOutput($"Evaluating Chapter {rc}");
            watch.Start();
            foreach (var r in rc.Recipes)
                EvaluateRecipe(r, rnd);
            watch.Stop();
            LogOutput($"Finished Evaluating Chapter {rc}");
            return rc;
        }
        private static Recipe EvaluateRecipe(Recipe r, Random rnd)
        {
            //Recipe Editing steps
            if (!r.TextApproved)
            {
                //Read the recipe to make sure it makes sense
                Thread.Sleep(50);
                int evaluation = rnd.Next(1, 10);
                // 7 means it didn't make sense so don't approve it, send it back for rework
                if (evaluation == 7)
                {
                   LogOutput($"{r} failed the readthrough! Reworking...");
                }
                else
                    r.TextApproved = true;
                return EvaluateRecipe(r, rnd);
            }
            else if (!r.IngredientsApproved)
            {
                //Check the ingredients and measurements
                Thread.Sleep(100);
                int evaluation = rnd.Next(1, 10);
                // 3 means it the ingredients or measurements are incorrect, send it back for rework
                if (evaluation == 3)
                {
                    LogOutput($"{r} had incorrect measurements! Reworking...");
                }
                else
                    r.IngredientsApproved = true;
                return EvaluateRecipe(r, rnd);
            }
            else if (r.RecipeEvaluated != r.Rank)
            {
                //Prepare recipe and taste
                Thread.Sleep(50 * r.Rank);
                int evaluation = rnd.Next(1, 10);
                // 4 means it didn't taste right, send it back for rework
                if (evaluation == 4)
                {
                    r.TextApproved = false;
                    r.IngredientsApproved = false;
                    r.RecipeEvaluated = 0;
                    LogOutput($"{r} tasted bad!  Reworking...");
                }
                else
                    r.RecipeEvaluated++;
                return EvaluateRecipe(r, rnd);
            }
            else
            {
                //Final editing pass(Brooke or Katie)
                Thread.Sleep(50 * r.Rank);
                int evaluation = rnd.Next(1, 10);
                // 1 means it just wasn't quite ready, send it back for rework
                if (evaluation == 1)
                {
                    r.TextApproved = false;
                    r.IngredientsApproved = false;
                    r.RecipeEvaluated = 0;
                    LogOutput($"{r} failed final editing!  Reworking...");
                    return EvaluateRecipe(r, rnd);
                }
                else
                {
                    r.FinalEditingComplete = true;
                    LogOutput($"{r} is ready for release!");
                }
            }
            return r;
        }

        private static List<RecipeChapter> ResetCategories(List<RecipeChapter> categories)
        {
            for(int i=0; i < categories.Count; i++)
                categories[i].Recipes = ResetRecipes(categories[i].Recipes);
            return categories.OrderBy(c => c.Number).ToList();
        }
        private static List<Recipe> ResetRecipes(List<Recipe> recipes)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                recipes[i].FinalEditingComplete = false;
                recipes[i].IngredientsApproved = false;
                recipes[i].RecipeEvaluated = 0;
                recipes[i].TextApproved = false;
            }
            return recipes.OrderBy(r => r.Number).ToList();
        }

        #endregion
    }

    public class RecipeChapter
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public List<Recipe> Recipes { get; set; }

        public override string ToString() => $"{Number} - {Title}";
    }

    public class Recipe
    {
        public RecipeChapter Chapter { get; set; }
        public string MainIngredient { get; set; }
        public int Number { get; set; }
        public bool TextApproved { get; set; }
        public bool IngredientsApproved { get; set; }

        /// <summary>
        /// Recipe should be evaluated as many times as the Rank of the recipe
        /// </summary>
        public int RecipeEvaluated { get; set; }

        public bool FinalEditingComplete { get; set; }

        public int Rank { get; set; }

        public override string ToString() => 
            $"{Chapter.Number}.{Number} ({Chapter.Title}:{MainIngredient})";
    }
}
