using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace CSharpRecipes
{
    #region Extension Methods
    static class CollectionExtMethods
    {

        #region 2.1 Looking for Specific Items in an List<T>

        // The method to retrieve all matching objects in a 
        //  sorted or unsorted List<T>
        public static IEnumerable<T> GetAll<T>(this List<T> myList, T searchValue) => 
            myList.Where(t => t.Equals(searchValue));

        // The method to retrieve all matching objects in a sorted ListEx<T>
        public static T[] BinarySearchGetAll<T>(this List<T> myList, T searchValue)
        {
            List<T> retObjs = new List<T>();

            // Search for first item.
            int center = myList.BinarySearch(searchValue);
            if (center > 0)
            {
                retObjs.Add(myList[center]);

                int left = center;
                while (left > 0 && myList[left - 1].Equals(searchValue))
                {
                    left -= 1;
                    retObjs.Add(myList[left]);
                }

                int right = center;
                while (right < (myList.Count - 1) &&
                    myList[right + 1].Equals(searchValue))
                {
                    right += 1;
                    retObjs.Add(myList[right]);
                }
            }

            return (retObjs.ToArray());
        }
        // Count the number of times an item appears in this 
        //   unsorted or sorted List<T>
        public static int CountAll<T>(this List<T> myList, T searchValue) => 
            myList.GetAll(searchValue).Count();

        // Count the number of times an item appears in this sorted List<T>
        public static int BinarySearchCountAll<T>(this List<T> myList, T searchValue) => 
            BinarySearchGetAll(myList, searchValue).Count();

        #endregion // 2.1

        #region 2.7 Creating Custom Enumerators
        public static IEnumerable<T> EveryNthItem<T>(this IEnumerable<T> enumerable, int step)
        {
            int current = 0;
            foreach (T item in enumerable)
            {
                ++current;
                if (current % step == 0)
                    yield return item;
            }
        }
        #endregion
    }
    #endregion

    public class CollectionsEnumerationsAndIterators
    {
        #region "2.1 Looking for Multiple Items in an List<T>"
        public static void TestDuplicateItemsListT()
        {
            // Retrieval
            List<int> listRetrieval = new List<int>() { -1, -1, 1, 2, 2, 2, 2, 3, 100, 4, 5 };

            Console.WriteLine("--GET All--");
            IEnumerable<int> items = listRetrieval.GetAll(2);
            foreach (var item in items)
                Console.WriteLine($"item: {item}");

            Console.WriteLine();
            items = listRetrieval.GetAll(-2);
            foreach (var item in items)
                Console.WriteLine($"item-2: {item}");

            Console.WriteLine();
            items = listRetrieval.GetAll(5);
            foreach (var item in items)
                Console.WriteLine($"item5: {item}");

            Console.WriteLine("\r\n--BINARY SEARCH GET ALL--");
            listRetrieval.Sort();
            int[] listItems = listRetrieval.BinarySearchGetAll(-2);
            foreach (var item in listItems)
                Console.WriteLine($"item-2: {item}");

            Console.WriteLine();
            listItems = listRetrieval.BinarySearchGetAll(2);
            foreach (var item in listItems)
                Console.WriteLine($"item2: {item}");

            Console.WriteLine();
            listItems = listRetrieval.BinarySearchGetAll(5);
            foreach (var item in listItems)
                Console.WriteLine($"item5: {item}");

            //Counting

            List<int> list = new List<int>() { -2, -2, -1, -1, 1, 2, 2, 2, 2, 3, 100, 4, 5 };

            Console.WriteLine("--CONTAINS TOTAL--");
            int count = list.CountAll(2);
            Console.WriteLine($"Count2: {count}");

            count = list.CountAll(3);
            Console.WriteLine($"Count3: {count}");

            count = list.CountAll(1);
            Console.WriteLine($"Count1: {count}");

            Console.WriteLine("\r\n--BINARY SEARCH COUNT ALL--");
            list.Sort();
            count = list.BinarySearchCountAll(2);
            Console.WriteLine($"Count2: {count}");

            count = list.BinarySearchCountAll(3);
            Console.WriteLine($"Count3: {count}");

            count = list.BinarySearchCountAll(1);
            Console.WriteLine($"Count1: {count}");


        }
        #endregion

        #region "2.2 Keeping Your List<T> Sorted"
        public static void TestSortedList()
        {
            // Create a SortedList and populate it with 
            //    randomly choosen numbers
            SortedList<int> sortedList = new SortedList<int>();
            sortedList.Add(200);
            sortedList.Add(20);
            sortedList.Add(2);
            sortedList.Add(7);
            sortedList.Add(10);
            sortedList.Add(0);
            sortedList.Add(100);
            sortedList.Add(-20);
            sortedList.Add(56);
            sortedList.Add(55);
            sortedList.Add(57);
            sortedList.Add(200);
            sortedList.Add(-2);
            sortedList.Add(-20);
            sortedList.Add(55);
            sortedList.Add(55);

            // Display it
            foreach (var i in sortedList)
                Console.WriteLine(i);

            // Now modify a value at a particular index
            sortedList.ModifySorted(0, 5);
            sortedList.ModifySorted(1, 10);
            sortedList.ModifySorted(2, 11);
            sortedList.ModifySorted(3, 7);
            sortedList.ModifySorted(4, 2);
            sortedList.ModifySorted(2, 4);
            sortedList.ModifySorted(15, 0);
            sortedList.ModifySorted(0, 15);
            sortedList.ModifySorted(223, 15);

            // Display it
            Console.WriteLine();
            foreach (var i in sortedList)
                Console.WriteLine(i);

            // Doing it the hard way
            List<int> testList = new List<int>();
            testList.Add(200);
            testList.Sort();
            testList.Add(20);
            testList.Sort();
            testList.Add(2);
            testList.Sort();
            testList.Add(7);
            testList.Sort();
            testList.Add(10);
            testList.Sort();
            testList.Add(0);
            testList.Sort();
            testList.Add(100);
            testList.Sort();
            testList.Add(-20);
            testList.Sort();
            testList.Add(56);
            testList.Sort();
            testList.Add(55);
            testList.Sort();
            testList.Add(57);
            testList.Sort();
            testList.Add(200);
            testList.Sort();
        }
        /*  ORIGINAL DATA
        -20
        -20
        -2
        0
        2
        7
        10
        20
        55
        55
        55
        56
        57
        100
        200
        200

        -20
        0
        0
        0
        2
        2
        3
        4
        10
        15
        20
        55
        55
        57
        100
        223
        */

        public class SortedList<T> : List<T>
        {
            public new void Add(T item)
            {
                int position = this.BinarySearch(item);
                if (position < 0)
                    position = ~position;

                this.Insert(position, item);
            }

            public void ModifySorted(T item, int index)
            {
                this.RemoveAt(index);

                int position = this.BinarySearch(item);
                if (position < 0)
                    position = ~position;

                this.Insert(position, item);
            }
        }
        #endregion

        #region "2.3 Sorting a Dictionary’s Keys and/or Values"
        public static void TestSortKeyValues()
        {
            // Define a Dictionary<T,U> object
            Dictionary<string, string> hash = new Dictionary<string, string>()
            {
                ["2"] = "two",
                ["1"] = "one",
                ["5"] = "five",
                ["4"] = "four",
                ["3"] = "three"
            };

            var x = from k in hash.Keys orderby k ascending select k;
            foreach (string s in x)
                Console.WriteLine($"Key: {s}  Value: {hash[s]}");

            Console.WriteLine();

            x = from k in hash.Keys orderby k descending select k;
            foreach (string s in x)
                Console.WriteLine($"Key: {s}  Value: {hash[s]}");


            x = from k in hash.Values orderby k ascending select k;
            foreach (string s in x)
                Console.WriteLine($"Value: {s}");

            Console.WriteLine();

            x = from k in hash.Values orderby k descending select k;
            foreach (string s in x)
                Console.WriteLine($"Value: {s}");

            SortedDictionary<string, string> sortedHash = new SortedDictionary<string, string>()
            {
                ["2"] = "two",
                ["1"] = "one",
                ["5"] = "five",
                ["4"] = "four",
                ["3"] = "three"
            };
            foreach (string key in sortedHash.Keys)
                Console.WriteLine($"Key: {key}  Value: {sortedHash[key]}");
            foreach (string key in sortedHash.OrderByDescending(item => item.Key).Select(item => item.Key))
                Console.WriteLine($"Key: {key}  Value: {sortedHash[key]}");

        }
        #endregion

        #region "2.4 Creating a Dictionary with Max and Min Value Boundaries"
        public static void TestMaxMinValueDictionary()
        {
            MinMaxValueDictionary<int, int> table = new MinMaxValueDictionary<int, int>(100, 200);
            table.Add(1, 100);
            table.Add(2, 200);
            table.Add(3, 150);
            table.Add(4, 200);
            table[2] = 100;
            try
            {
                table[2] = 500;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            table.Add(5, 200);
            try
            {
                table.Add(6, 20);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                table.Add(7, 2000);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Table Contents:");
            foreach (var key in table.Keys)
                Console.WriteLine($"Key: {key}  Value: {table[key]}");

            table.Remove(1);
            table.Remove(2);
            table.Remove(3);

            Console.WriteLine("Table Contents after removing items 1-3:");
            foreach (var key in table.Keys)
                Console.WriteLine($"Key: {key}  Value: {table[key]}");

            table.Clear();
            Console.WriteLine("Table Contents after clearing:");
            foreach (var key in table.Keys)
                Console.WriteLine($"Key: {key}  Value: {table[key]}");
        }



        [Serializable]
        public class MinMaxValueDictionary<T, U>
            where U : IComparable<U>
        {
            protected Dictionary<T, U> internalDictionary = null;

            public MinMaxValueDictionary(U minValue, U maxValue)
            {
                this.MinValue = minValue;
                this.MaxValue = maxValue;
                internalDictionary = new Dictionary<T, U>();
            }

#pragma warning disable CSE0002 // Use getter-only auto properties
            public U MinValue { get; private set; } = default(U);
            public U MaxValue { get; private set; } = default(U);
#pragma warning restore CSE0002 // Use getter-only auto properties

            public int Count => (internalDictionary.Count);

            public Dictionary<T, U>.KeyCollection Keys => (internalDictionary.Keys);

            public Dictionary<T, U>.ValueCollection Values => (internalDictionary.Values);

            public U this[T key]
            {
                get { return (internalDictionary[key]); }
                set
                {
                    if (value.CompareTo(MinValue) >= 0 &&
                        value.CompareTo(MaxValue) <= 0)
                        internalDictionary[key] = value;
                    else
                        throw new ArgumentOutOfRangeException(nameof(value), value,
                            $"Value must be within the range {MinValue} to {MaxValue}");
                }
            }

            public void Add(T key, U value)
            {
                if (value.CompareTo(MinValue) >= 0 &&
                    value.CompareTo(MaxValue) <= 0)
                    internalDictionary.Add(key, value);
                else
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"Value must be within the range {MinValue} to {MaxValue}");
            }

            public bool ContainsKey(T key) => (internalDictionary.ContainsKey(key));

            public bool ContainsValue(U value) => (internalDictionary.ContainsValue(value));

            public override bool Equals(object obj) => (internalDictionary.Equals(obj));

            public IEnumerator GetEnumerator() => (internalDictionary.GetEnumerator());

            public override int GetHashCode() => (internalDictionary.GetHashCode());

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                internalDictionary.GetObjectData(info, context);
            }

            public void OnDeserialization(object sender)
            {
                internalDictionary.OnDeserialization(sender);
            }

            public override string ToString() => (internalDictionary.ToString());

            public bool TryGetValue(T key, out U value) => 
                (internalDictionary.TryGetValue(key, out value));

            public void Remove(T key)
            {
                internalDictionary.Remove(key);
            }

            public void Clear()
            {
                internalDictionary.Clear();
            }
        }
        #endregion

        #region "2.5 Persisting a Collection Between Application Sessions"
        public static void TestSerialization()
        {
            ArrayList HT = new ArrayList() { "Zero", "One", "Two" };

            foreach (object O in HT)
                Console.WriteLine(O.ToString());
            SerializeToFile<ArrayList>(HT, "HT.data");

            ArrayList HTNew = new ArrayList();
            HTNew = DeserializeFromFile<ArrayList>("HT.data");
            foreach (object O in HTNew)
                Console.WriteLine(O.ToString());

            if (HT == HTNew)
                Console.WriteLine("Same reference");
            else
                Console.WriteLine("Different reference");

            if (HT[0] == HTNew[0])
                Console.WriteLine("Same [0] reference");
            else
                Console.WriteLine("Different [0] reference");


            Console.WriteLine();
            List<int> test = new List<int>() { 1, 2 };
            foreach (int i in test)
                Console.WriteLine(i.ToString());
            SerializeToFile<List<int>>(test, "TEST.DATA");
            List<int> testNew = new List<int>();
            testNew = DeserializeFromFile<List<int>>("TEST.DATA");
            foreach (int i in testNew)
                Console.WriteLine(i.ToString());

            Console.WriteLine();
            Dictionary<int, int> testD = new Dictionary<int, int>()
            {
                [1] = 1,
                [2] = 2
            };
            foreach (KeyValuePair<int, int> kvp in testD)
                Console.WriteLine($"{kvp.Key} : {kvp.Value}");
            SerializeToFile<Dictionary<int, int>>(testD, "TEST.DATA");
            Dictionary<int, int> testDNew = new Dictionary<int, int>();
            testDNew = DeserializeFromFile<Dictionary<int, int>>("TEST.DATA");
            foreach (KeyValuePair<int, int> kvp in testDNew)
                Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            byte[] serializedDict = Serialize<Dictionary<int, int>>(testD);
            Dictionary<int, int> deserializedDict = Deserialize<Dictionary<int, int>>(serializedDict);
            foreach (KeyValuePair<int, int> kvp in deserializedDict)
                Console.WriteLine($"{kvp.Key} : {kvp.Value}");


        }


        public static void SerializeToFile<T>(T obj, string dataFile)
        {
            using (FileStream fileStream = File.Create(dataFile))
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                binSerializer.Serialize(fileStream, obj);
            }
        }

        public static T DeserializeFromFile<T>(string dataFile)
        {
            T obj = default(T);
            using (FileStream fileStream = File.OpenRead(dataFile))
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                obj = (T)binSerializer.Deserialize(fileStream);
            }
            return obj;
        }

        public static byte[] Serialize<T>(T obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                binSerializer.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] serializedObj)
        {
            T obj = default(T);
            using (MemoryStream memStream = new MemoryStream(serializedObj))
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                obj = (T)binSerializer.Deserialize(memStream);
            }
            return obj;
        }

        #endregion

        #region "2.6 Testing Every Element In An Array or List<T>"
        public static void TestArrayForNulls()
        {
            // Create a List of strings
            List<string> strings = new List<string>() { "one", null, "three", "four" };

            // Determine if there are no null values in the List
            string str = strings.TrueForAll(delegate (string val)
            {
                if (val == null)
                    return false;
                else
                    return true;
            }).ToString();

            // Display the results
            Console.WriteLine(str);

            List<string> nulls = new List<string>() { null, null, null, null };
            // Determine if there are all null values in the List
            string result = nulls.TrueForAll(delegate (string val)
            {
                if (val == null)
                    return true;
                else
                    return false;
            }).ToString();

            // Display the results
            Console.WriteLine(result);

        }

        #endregion

        #region "2.7 Creating Custom Enumerators"
        public static void TestIterators()
        {
            Container<int> container = new Container<int>();
            // Create test data
            List<int> testData = new List<int>(){
                -1,1,2,3,4,5,6,7,8,9,10,200,500};

            // Add test data to Container object
            container.Clear();
            container.AddRange(testData);
            // Iterate over Container object
            foreach (int i in container)
                Console.WriteLine(i);

            Console.WriteLine();
            foreach (int i in container.GetReverseOrderEnumerator())
                Console.WriteLine(i);

            Console.WriteLine();
            foreach (int i in container.GetForwardStepEnumerator(2))
                Console.WriteLine(i);

            Console.WriteLine();
            foreach (int i in container.GetReverseStepEnumerator(3))
                Console.WriteLine(i);

            Console.WriteLine();
            foreach (int i in GetValues())
                Console.WriteLine(i);

            Container<int> intContainer = new Container<int>();
            intContainer.Add(1);
            intContainer.Add(3);
            intContainer.Add(2);
            foreach (int i in intContainer)
                Console.WriteLine(i);

        }

        public static IEnumerable<int> GetValues()
        {
            yield return 10;
            yield return 20;
            yield return 30;
            yield return 100;
        }
        
        public class Container<T> : IEnumerable<T>
        {
            public Container() { }

            private List<T> _internalList = new List<T>();

            // This iterator iterates over each element from first to last
            public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

            // This iterator iterates over each element from last to first
            public IEnumerable<T> GetReverseOrderEnumerator()
            {
                foreach (T item in ((IEnumerable<T>)_internalList).Reverse())
                    yield return item;
            }

            // This iterator iterates over each element from first to last stepping 
            // over a predefined number of elements
            public IEnumerable<T> GetForwardStepEnumerator(int step)
            {
                foreach (T item in _internalList.EveryNthItem(step))
                    yield return item;
            }

            // This iterator iterates over each element from last to first stepping 
            // over a predefined number of elements
            public IEnumerable<T> GetReverseStepEnumerator(int step)
            {
                foreach (T item in ((IEnumerable<T>)_internalList).Reverse().EveryNthItem(step))
                    yield return item;
            }

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion

            public void Clear()
            {
                _internalList.Clear();
            }

            public void Add(T item)
            {
                _internalList.Add(item);
            }

            public void AddRange(ICollection<T> collection)
            {
                _internalList.AddRange(collection);
            }
        }


        #endregion

        #region "2.8 Dealing with Finally Blocks and Iterators"
        public static void TestFinallyAndIterators()
        {
            //Create a StringSet object and fill it with data
            StringSet strSet =
                new StringSet()
                    {"item1",
                        "item2",
                        "item3",
                        "item4",
                        "item5"};

            // Use the GetEnumerator iterator.
            foreach (string s in strSet)
                Console.WriteLine(s);


            // Display all data in StringSet object
            try
            {
                foreach (string s in strSet)
                {
                    try
                    {
                        Console.WriteLine(s);
                        // Force an exception here
                        //throw new Exception();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("In foreach catch block");
                    }
                    finally
                    {
                        // Executed on each iteration
                        Console.WriteLine("In foreach finally block");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("In outer catch block");
            }
            finally
            {
                // Executed on each iteration
                Console.WriteLine("In outer finally block");
            }

            /*
			This code is executed in this fashion when an exception occurs in the iterator:
			 - In iterator finally block
			 - In outer catch block
			 - In outer finally block

			This code is executed in this fashion when NO exception occurs in the iterator:
			 - item1
			 - In foreach finally block
			 - item2
			 - In foreach finally block
			 - item3
			 - In foreach finally block
			 - item4
			 - In foreach finally block
			 - item5
			 - In foreach finally block
			 - item6
			 - In foreach finally block
			 - In iterator finally block
			 - In outer finally block

			This code is executed in this fashion when an exception occurs in the foreach loop:
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In foreach catch block
			 - In foreach finally block
			 - In iterator finally block
			 - In outer finally block
			*/
        }


        public class StringSet : IEnumerable<string>
        {
            private List<string> _items = new List<string>();

            public void Add(string value)
            {
                _items.Add(value);
            }

            public IEnumerator<string> GetEnumerator()
            {
                try
                {
                    for (int index = 0; index < _items.Count; index++)
                    {
                        // Force an exception here
                        //if (index == 1) throw new Exception();
                        yield return (_items[index]);
                    }
                }
                // Cannot use catch blocks in an iterator
                finally
                {
                    // Only executed at end of foreach loop (including on yield break)
                    Console.WriteLine("In iterator finally block");
                }
            }

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }
        #endregion

        #region "2.9 Implementing Nested foreach Functionality In A Class"
        public static void CreateNestedObjects()
        {
            Group<Group<Item>> hierarchy =
                new Group<Group<Item>>("root") {
                    new Group<Item>("subgroup1"){
                        new Item("item1",100),
                        new Item("item2",200)},
                    new Group<Item>("subgroup2"){
                        new Item("item3",300),
                        new Item("item4",400)}};

            IEnumerator enumerator = ((IEnumerable)hierarchy).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(((Group<Item>)enumerator.Current).Name);
                foreach (Item i in ((Group<Item>)enumerator.Current))
                {
                    Console.WriteLine(i.Name);
                }
            }

            // Read back the data
            DisplayNestedObjects(hierarchy);
        }

        //topLevelGroup.Count: 2
        //topLevelGroupName:  root
        //        subGroup.SubGroupName:  subgroup1
        //        subGroup.Count: 2
        //                item.Name:     item1
        //                item.Location: 100
        //                item.Name:     item2
        //                item.Location: 200
        //        subGroup.SubGroupName:  subgroup2
        //        subGroup.Count: 2
        //                item.Name:     item3
        //                item.Location: 300
        //                item.Name:     item4
        //                item.Location: 400

        private static void DisplayNestedObjects(Group<Group<Item>> topLevelGroup)
        {
            Console.WriteLine($"topLevelGroup.Count: {topLevelGroup.Count}");
            Console.WriteLine($"topLevelGroupName:  {topLevelGroup.Name}");

            // Outer foreach to iterate over all objects in the 
            // topLevelGroup object
            foreach (Group<Item> subGroup in topLevelGroup)
            {
                Console.WriteLine($"\tsubGroup.SubGroupName:  {subGroup.Name}");
                Console.WriteLine($"\tsubGroup.Count: {subGroup.Count}");

                // Inner foreach to iterate over all Item objects in the 
                // current SubGroup object
                foreach (Item item in subGroup)
                {
                    Console.WriteLine($"\t\titem.Name:     {item.Name}");
                    Console.WriteLine($"\t\titem.Location: {item.Location}");
                }
            }
        }


        public class Group<T> : IEnumerable<T>
        {
            public Group(string name)
            {
                this.Name = name;
            }

            private List<T> _groupList = new List<T>();

            public string Name { get; set; }

            public int Count => _groupList.Count;

            public void Add(T group)
            {
                _groupList.Add(group);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            //IEnumerator IEnumerable.GetEnumerator() => new GroupEnumerator<T>(_groupList.ToArray());

            public IEnumerator<T> GetEnumerator() => _groupList.GetEnumerator();
        }


        public class Item
        {
            public Item(string name, int location)
            {
                this.Name = name;
                this.Location = location;
            }
            public string Name { get; set; }
            public int Location { get; set; }
        }

        public class GroupEnumerator<T> : IEnumerator
        {
            public T[] _items;

            int position = -1;

            public GroupEnumerator(T[] list)
            {
                _items = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < _items.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return _items[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }


        #endregion

        #region "2.10 Recipe about using collections asynchronously and safely"
        public class Fan
        {
            public string Name { get; set; }
            public DateTime Admitted { get; set; }
            public int AdmittanceGateNumber { get; set; }
        }

        private static ConcurrentDictionary<int, Fan> stadiumGates =
            new ConcurrentDictionary<int, Fan>();
        private static bool monitorGates = true;
        public static async Task TestConcurrentDictionary()
        {
            // set up a list of fans attending the event
            List<Fan> fansAttending = new List<Fan>();
            for (int i = 0; i < 100; i++)
                fansAttending.Add(new Fan() { Name = "Fan" + i });
            Fan[] fans = fansAttending.ToArray();

            int gateCount = 10;
            Task[] entryGates = new Task[gateCount];
            Task[] securityMonitors = new Task[gateCount];

            for (int gateNumber = 0; gateNumber < gateCount; gateNumber++)
            {
                //FUN FACT:
                //You might think that as gateNumber changes in the for loop that the creation of the Task
                //admitting the Fan would capture the value (0,1,2,etc.) at the point in time that the Task
                //was created.  As it turns out, the Task will use the CURRENT value in the gateNumber
                //variable when it runs.  This means that even though you launched a Task for gate 0, it might 
                //get a gateNumber variable with 9 in it as the loop has progressed since the Task was
                //created.
                //To deal with this, we assign the values to a local variable which fixes the scope and the
                //value you wanted can be captured by the Task appropriately.
                int GateNum = gateNumber;
                int GateCount = gateCount;
                Action action = delegate () { AdmitFans(fans, GateNum, GateCount); };
                entryGates[gateNumber] = Task.Run(action);
            }

            for (int gateNumber = 0; gateNumber < gateCount; gateNumber++)
            {
                int GateNum = gateNumber;
                Action action = delegate () { MonitorGate(GateNum); };
                securityMonitors[gateNumber] = Task.Run(action);
            }

            await Task.WhenAll(entryGates);

            // Shut down monitoring
            monitorGates = false;

            // These are all zero at this point as all gates are empty, code examples...
            var keys = stadiumGates.Select(gate => gate.Key);
            var values = stadiumGates.Select(gate => gate.Value);
            var count = stadiumGates.Select(gate => gate).Count();
        }

        private static void AdmitFans(Fan[] fans, int gateNumber, int gateCount)
        {
            Random rnd = new Random();
            int fansPerGate = fans.Length / gateCount;
            int start = gateNumber * fansPerGate;
            int end = start + fansPerGate - 1;
            for (int f = start; f <= end; f++)
            {
                Console.WriteLine($"Admitting {fans[f].Name} through gate {gateNumber}");
                var fanAtGate =
                    stadiumGates.AddOrUpdate(gateNumber, fans[f], 
                        (key, fanInGate) =>
                        {
                            Console.WriteLine($"{fanInGate.Name} was replaced by " +
                                $"{fans[f].Name} in gate {gateNumber}");
                            return fans[f];
                        });
                // Perform patdown check and check ticket
                Thread.Sleep(rnd.Next(500, 2000));
                // Let them through the gate
                fans[f].Admitted = DateTime.Now;
                fans[f].AdmittanceGateNumber = gateNumber;
                Fan fanAdmitted;
                if(stadiumGates.TryRemove(gateNumber, out fanAdmitted))
                    Console.WriteLine($"{fanAdmitted.Name} entering event from gate " +
                        $"{fanAdmitted.AdmittanceGateNumber} on " +
                        $"{fanAdmitted.Admitted.ToShortTimeString()}");
                else // if we couldn't admit them, security must have gotten them...
                    Console.WriteLine($"{fanAdmitted.Name} held by security " + 
                        $"at gate {fanAdmitted.AdmittanceGateNumber}");
            }
        }

        private static void MonitorGate(int gateNumber)
        {
            Random rnd = new Random();
            while (monitorGates)
            {
                Fan currentFanInGate;
                if (stadiumGates.TryGetValue(gateNumber, out currentFanInGate))
                    Console.WriteLine($"Monitor: {currentFanInGate.Name} is in Gate {gateNumber}");
                else
                    Console.WriteLine($"Monitor: No fan is in Gate {gateNumber}");

                // Wait and then check gate again
                Thread.Sleep(rnd.Next(500, 5000));
            }
        }
    }
    #endregion


    #region "Defined enumerations"
    [Flags]
    public enum Shapes
    {
        Square = 0, Circle = 1, Cylinder = 2, Octagon = 4
    }

    [Flags]
    public enum IceCreamToppings
    {
        Sprinkles = 0,
        HotFudge = 1,
        Cherry = 2,
        WhippedCream = 4,
        All = Sprinkles | HotFudge | Cherry | WhippedCream
    }



    [Flags]
    public enum RecycleItems
    {
        Glass = 0x01,
        AluminumCans = 0x02,
        MixedPaper = 0x04,
        Newspaper = 0x08,
        TinCans = 0x10,
        Cardboard = 0x20,
        ClearPlastic = 0x40,
        All = (Glass | AluminumCans | MixedPaper | Newspaper | TinCans | Cardboard | ClearPlastic)
    }
    #endregion

}
