using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

[assembly: ObfuscateAssembly(true, StripAfterObfuscation = true)]

namespace CSharpRecipes
{
    public static class ReflectionExt
    {
        #region 6.2 Extension method
        public static IEnumerable<MemberInfo> GetMembersInAssembly(this Assembly asm, string memberName) => 
            from type in asm.GetTypes()
                from ms in type.GetMember(memberName, MemberTypes.All,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Static | BindingFlags.Instance)
                select ms;

        public static IEnumerable<Type> GetSerializableTypes(this Assembly asm) => 
            from type in asm.GetTypes()
            where type.IsSerializable && !type.IsNestedPrivate // filters out anonymous types
            select type;

        public static IEnumerable<Type> GetSubclassesForType(this Assembly asm,
                                                                Type baseClassType) => 
            from type in asm.GetTypes()
            where type.IsSubclassOf(baseClassType)
            select type;

        public static IEnumerable<Type> GetNestedTypes(this Assembly asm) => 
            from t in asm.GetTypes()
                from t2 in t.GetNestedTypes(BindingFlags.Instance |
                            BindingFlags.Static |
                            BindingFlags.Public |
                            BindingFlags.NonPublic)
                where !t2.IsEnum && !t2.IsInterface && !t2.IsNestedPrivate // filters out anonymous types
                select t2;
        #endregion

        #region 6.3 Extension methods
        public class TypeHierarchy
        {
            public Type DerivedType { get; set; }
            public IEnumerable<Type> InheritanceChain { get; set; }
        }
        public static IEnumerable<TypeHierarchy> GetTypeHierarchies(this Assembly asm) => 
            from Type type in asm.GetTypes()
            select new TypeHierarchy
            {
                DerivedType = type,
                InheritanceChain = GetInheritanceChain(type)
            };

        public static IEnumerable<Type> GetInheritanceChain(this Type derivedType) => 
            (from t in derivedType.GetBaseTypes()
            select t).Reverse();

        private static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            Type current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
        public static IEnumerable<MemberInfo> GetMethodOverrides(this Type type) => 
            from ms in type.GetMethods(BindingFlags.Instance |
                                    BindingFlags.NonPublic | BindingFlags.Public |
                                    BindingFlags.Static | BindingFlags.DeclaredOnly)
            where ms != ms.GetBaseDefinition()
            select ms.GetBaseDefinition();

        public static MethodInfo GetBaseMethodOverridden(this Type type,
                                                string methodName, Type[] paramTypes)
        {
            MethodInfo method = type.GetMethod(methodName, paramTypes);
            MethodInfo baseDef = method?.GetBaseDefinition();
            if (baseDef != method)
            {
                bool foundMatch = (from p in baseDef.GetParameters()
                            join op in paramTypes
                                on p.ParameterType.UnderlyingSystemType equals op.UnderlyingSystemType
                            select p).Any();

                if (foundMatch)
                    return baseDef;
            }
            return null;
        }
        #endregion
    }
    
    
	public class ReflectionAndDynamicProgramming
	{
        #region Common Code
        private static string GetProcessPath()
        {
            // fix the path so that if running under the debugger we get the original file
            string processName = Process.GetCurrentProcess().MainModule.FileName;
            int index = processName.IndexOf("vshost", StringComparison.Ordinal);
            if (index != -1)
            {
                string first = processName.Substring(0, index);
                int numChars = processName.Length - (index + 7);
                string second = processName.Substring(index + 7, numChars);

                processName = first + second;
            }
            return processName;
        }
        #endregion

        #region "6.1 Listing Referenced Assemblies"	
        public static void ListImportedAssemblies()
		{
            string file = GetProcessPath();
            StringCollection assemblies = new StringCollection();
            ReflectionAndDynamicProgramming.BuildDependentAssemblyList(file,assemblies);
            Console.WriteLine($"Assembly {file} has a dependency tree of these assemblies:{Environment.NewLine}");
            foreach(string name in assemblies)
            {
	            Console.WriteLine($"\t{name}{Environment.NewLine}");
            }
		}

        public static void BuildDependentAssemblyList(string path, 
	        StringCollection assemblies)
        {
	        // maintain a list of assemblies the original one needs
	        if(assemblies == null)
		        assemblies = new StringCollection();

	        // have we already seen this one?
	        if(assemblies.Contains(path)==true)
		        return;

            try
            {
                Assembly asm = null;
                
                // look for common path delimiters in the string 
                // to see if it is a name or a path
                if ((path.IndexOf(@"\", 0, path.Length, StringComparison.Ordinal) != -1) ||
                    (path.IndexOf("/", 0, path.Length, StringComparison.Ordinal) != -1))
                {
                    // load the assembly from a path
                    asm = Assembly.LoadFrom(path);
                }
                else
                {
                    // try as assembly name
                    asm = Assembly.Load(path);
                }
                
                // add the assembly to the list
                if (asm != null)
                    assemblies.Add(path);
                
                // get the referenced assemblies
                AssemblyName[] imports = asm.GetReferencedAssemblies();
                
                // iterate
                foreach (AssemblyName asmName in imports)
                {
                    // now recursively call this assembly to get the new modules 
                    // it references
                    BuildDependentAssemblyList(asmName.FullName, assemblies);
                }
            }
            catch (FileLoadException fle)
            {
                // just let this one go...
                Console.WriteLine(fle);
            }
        }

        #endregion

        #region "6.2 Determining Type Characteristics in Assemblies"	
        public static void DetermineTypeCharacteristics()
		{
            string file = GetProcessPath();
            Assembly asm = Assembly.LoadFrom(file);

            //Find type by name
            string memberSearchName = "DetermineTypeCharacteristics";
            var members = asm.GetMembersInAssembly(memberSearchName);
            Console.WriteLine($"Assembly: {asm.FullName} has members with member name {memberSearchName}:");
            foreach (MemberInfo member in members)
            {
                Console.WriteLine($"Found {member.MemberType} : " +
                    $"{member.ToString()} IN " +
                    $"{member.DeclaringType.FullName}");
            }

            //Types available outside the assembly
            Console.WriteLine($"Assembly: {asm.FullName} has exported types:");
            var types = asm.GetExportedTypes();
            foreach (Type t in types)
            {
                Console.WriteLine($"\t{t.FullName}");
            }

            //Serializable Types
            var serializeableTypes = asm.GetSerializableTypes();
            Console.WriteLine($"{asm.FullName} has serializable types:");
            foreach (var serializeableType in serializeableTypes)
            {
                Console.WriteLine($"\t{serializeableType.Name}");
            }

            //Subclasses of a given Type
            Type type = Type.GetType("CSharpRecipes.ReflectionAndDynamicProgramming+BaseOverrides");
            var subClasses = asm.GetSubclassesForType(type);
            Console.WriteLine($"{type.FullName} is subclassed by:");
            foreach (Type t in subClasses)
            {
                Console.WriteLine($"\t{t.FullName}");
            }

            //Nested Types
            var nestedTypes = asm.GetNestedTypes();
            Console.WriteLine($"{asm.FullName} has nested types:");
            foreach (var nestedType in nestedTypes)
            {
                Console.WriteLine($"\t{nestedType.Name}");
            }
        }
        #endregion

        #region "6.3 Determining Inheritance Characteristics"
        public abstract class BaseOverrides
        {
            public abstract void Foo(string str, int i);

            public abstract void Foo(long l, double d, byte[] bytes);
        }

        public class DerivedOverrides : BaseOverrides
        {
            public override void Foo(string str, int i)
            {
            }

            public override void Foo(long l, double d, byte[] bytes)
            {
            }
        }

        public static void DetermineInheritanceCharacteristics()
        {
            string path = GetProcessPath();
            Assembly asm = Assembly.LoadFrom(path);

            Type derivedType = asm.GetType("CSharpRecipes.ReflectionAndDynamicProgramming+DerivedOverrides", true, true);

            // a specific type
            var chain = derivedType.GetInheritanceChain();
            Console.WriteLine($"Derived Type: {derivedType.FullName}");
            DisplayInheritanceChain(chain);
            Console.WriteLine();

            // all types in the assembly
            var typeHierarchies = asm.GetTypeHierarchies();
            foreach (var th in typeHierarchies)
            {
                // Recurse over all base types
                Console.WriteLine($"Derived Type: {th.DerivedType.FullName}");
                DisplayInheritanceChain(th.InheritanceChain);
                Console.WriteLine();
            }


            var methodOverrides = derivedType.GetMethodOverrides();
            foreach (MethodInfo mi in methodOverrides)
            {
                Console.WriteLine();
                Console.WriteLine($"Current Method: {mi.ToString()}");
                Console.WriteLine($"Base Type FullName:  {mi.DeclaringType.FullName}");
                Console.WriteLine($"Base Method:  {mi.ToString()}");
                // list the types of this method
                foreach (ParameterInfo pi in mi.GetParameters())
                {
                    Console.WriteLine($"\tParam {pi.Name} : {pi.ParameterType.ToString()}");
                }
            }

            // try the signature findmethodoverrides
            string methodName = "Foo";
            var baseTypeMethodInfo = derivedType.GetBaseMethodOverridden(methodName,
                new Type[3] { typeof(long), typeof(double), typeof(byte[]) });
            Console.WriteLine($"{Environment.NewLine}For [Type] Method: [{derivedType.Name}] {methodName}");
            Console.WriteLine($"Base Type FullName: {baseTypeMethodInfo.ReflectedType.FullName}");
            Console.WriteLine($"Base Method: {baseTypeMethodInfo}");
            foreach (ParameterInfo pi in baseTypeMethodInfo.GetParameters())
            {
                // list the params so we can see which one we got
                Console.WriteLine($"\tParam {pi.Name} : {pi.ParameterType.ToString()}");
            }
        }

        private static void DisplayInheritanceChain(IEnumerable<Type> chain)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var type in chain)
            {
                if (builder.Length == 0)
                    builder.Append(type.Name);
                else
                    builder.AppendFormat($"<-{type.Name}");
            }
            Console.WriteLine($"Base Type List: {builder.ToString()}");
        }


        #endregion


        #region "6.4 Invoking Members Using Reflection"
        public static void ReflectionInvocation()
        {
            TestReflectionInvocation();
        }

        public static void TestReflectionInvocation()
        {
            XDocument xdoc = XDocument.Load(@"..\..\SampleClassLibrary\SampleClassLibraryTests.xml");
            ReflectionInvoke(xdoc, @"SampleClassLibrary.dll");
        }

        public static void ReflectionInvoke(XDocument xdoc, string asmPath)
        {
            var test = from t in xdoc.Root.Elements("Test")
                        select new
                        {
                            typeName = (string)t.Attribute("className").Value,
                            methodName = (string)t.Attribute("methodName").Value,
                            parameter = from p in t.Elements("Parameter")
                                        select new { arg = p.Value }
                        };

            // Load the assembly
            Assembly asm = Assembly.LoadFrom(asmPath);

            foreach (var elem in test)
            {
                // create the actual type
                Type reflClassType = asm.GetType(elem.typeName, true, false);

                // Create an instance of this type and verify that it exists
                object reflObj = Activator.CreateInstance(reflClassType);
                if (reflObj != null)
                {
                    // Verify that the method exists and get its MethodInfo obj
                    MethodInfo invokedMethod = reflClassType.GetMethod(elem.methodName);
                    if (invokedMethod != null)
                    {
                        // Create the argument list for the dynamically invoked methods
                        object[] arguments = new object[elem.parameter.Count()];
                        int index = 0;

                        // for each parameter, add it to the list
                        foreach (var arg in elem.parameter)
                        {
                            // get the type of the parameter
                            Type paramType =
                                invokedMethod.GetParameters()[index].ParameterType;

                            // change the value to that type and assign it
                            arguments[index] =
                                Convert.ChangeType(arg.arg, paramType);
                            index++;
                        }

                        // Invoke the method with the parameters
                        object retObj = invokedMethod.Invoke(reflObj, arguments);

                        Console.WriteLine($"\tReturned object: {retObj}");
                        Console.WriteLine($"\tReturned object: {retObj.GetType().FullName}");
                    }
                }
            }
        }
        #endregion

        #region "6.5 Access Local Variables Information"
        public static void TestGetLocalVars()
        {
            string file = GetProcessPath();

	        // Get all local var info for the CSharpRecipes.Reflection.GetLocalVars method
	        System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> vars = 
                GetLocalVars(file, "CSharpRecipes.ReflectionAndDynamicProgramming", "GetLocalVars");
        }

        public static System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> GetLocalVars(string asmPath, string typeName, string methodName)
        {
	        Assembly asm = Assembly.LoadFrom(asmPath);
	        Type asmType = asm.GetType(typeName);
	        MethodInfo mi = asmType.GetMethod(methodName);
	        MethodBody mb = mi.GetMethodBody();

	        System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> vars = 
                (System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo>)mb.LocalVariables;
			
	        // Display information about each local variable
	        foreach (LocalVariableInfo lvi in vars)
	        {
		        Console.WriteLine($"IsPinned: {lvi.IsPinned}");
		        Console.WriteLine($"LocalIndex: {lvi.LocalIndex}");
		        Console.WriteLine($"LocalType.Module: {lvi.LocalType.Module}");
		        Console.WriteLine($"LocalType.FullName: {lvi.LocalType.FullName}");
		        Console.WriteLine($"ToString(): {lvi.ToString()}");
	        }
			
	        return (vars);
        }
		#endregion

		#region "6.6 Creating a Generic Type"
        public static void CreateDictionary()
        {
	        // Get the type we want to construct
	        Type typeToConstruct = typeof(Dictionary<,>); 
            // Get the type arguments we want to construct our type with
	        Type[] typeArguments = {typeof(int), typeof(string)};
	        // Bind these type arguments to our generic type
	        Type newType = typeToConstruct.MakeGenericType(typeArguments);

            // Construct our type
            Dictionary<int, string> dict = (Dictionary<int, string>)Activator.CreateInstance(newType);
			
	        // Test our newly constructed type
	        Console.WriteLine($"Count == {dict.Count}");
            dict.Add(1, "test1");
	        Console.WriteLine($"Count == {dict.Count}");
        }
        #endregion

        #region "6.7 Dynamic vs Object"
        public static void TestDynamicVsObject()
        {
            // Load the assembly
            Assembly asm = Assembly.LoadFrom(@"SampleClassLibrary.dll");
            
            // Get the SampleClass type
            Type reflClassType = asm?.GetType("SampleClassLibrary.SampleClass", true, false);

            if (reflClassType != null)
            {
                // Create our sample class instance
                dynamic sampleClass = Activator.CreateInstance(reflClassType);
                Console.WriteLine($"LastMessage: {sampleClass.LastMessage}");
                Console.WriteLine("Calling TestMethod1");
                sampleClass.TestMethod1("Running TestMethod1");
                Console.WriteLine($"LastMessage: {sampleClass.LastMessage}");
                Console.WriteLine("Calling TestMethod2");
                sampleClass.TestMethod2("Running TestMethod2", 27);
                Console.WriteLine($"LastMessage: {sampleClass.LastMessage}");

                //object objSampleClass = Activator.CreateInstance(reflClassType);
                //Console.WriteLine($"LastMessage: {objSampleClass.LastMessage}");
                //Console.WriteLine("Calling TestMethod1");
                //objSampleClass.TestMethod1("Running TestMethod1");
                //Console.WriteLine($"LastMessage: {objSampleClass.LastMessage}");
                //Console.WriteLine("Calling TestMethod2");
                //objSampleClass.TestMethod2("Running TestMethod2", 27);
                //Console.WriteLine($"LastMessage: {objSampleClass.LastMessage}");

                //Error CS1061  'object' does not contain a definition for 'LastMessage' and no extension method 'LastMessage' accepting a first argument of type 'object' could be found(are you missing a using directive or an assembly reference ?)	CSharpRecipes C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs  482
                //Error CS1061  'object' does not contain a definition for 'TestMethod1' and no extension method 'TestMethod1' accepting a first argument of type 'object' could be found(are you missing a using directive or an assembly reference ?)	CSharpRecipes C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs  484
                //Error CS1061  'object' does not contain a definition for 'LastMessage' and no extension method 'LastMessage' accepting a first argument of type 'object' could be found(are you missing a using directive or an assembly reference ?)	CSharpRecipes C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs  485
                //Error CS1061  'object' does not contain a definition for 'TestMethod2' and no extension method 'TestMethod2' accepting a first argument of type 'object' could be found(are you missing a using directive or an assembly reference ?)	CSharpRecipes C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs  487
                //Error CS1061  'object' does not contain a definition for 'LastMessage' and no extension method 'LastMessage' accepting a first argument of type 'object' could be found(are you missing a using directive or an assembly reference ?)	CSharpRecipes C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs  488
            }

        }



        #endregion

        #region "6.8 Building Objects Dynamically"
        public static void TestBuildingObjectsDynamically()
        {
            dynamic expando = new ExpandoObject();
            expando.Name = "Brian";
            expando.Country = "USA";

            // Add properties dynamically to expando
            AddProperty(expando, "Language", "English");

            // Add method to expando
            expando.IsValid = (Func<bool>)(() =>
            {
                // Check that they supplied a name
                if(string.IsNullOrWhiteSpace(expando.Name))
                    return false;
                return true;
            });

            if(!expando.IsValid())
            {
                // Don't continue...
            }

            // You can also add event handlers to expando objects
            var eventHandler =
                new Action<object, EventArgs>((sender, eventArgs) =>
                {
                    dynamic exp = sender as ExpandoObject;
                    var langArgs = eventArgs as LanguageChangedEventArgs;
                    // Have to cast here and not in signature or we get
                    //Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: Delegate 'System.Action<System.Dynamic.ExpandoObject,string, System.Action < object,System.EventArgs >> ' has
                    //some invalid arguments
                    //   at CallSite.Target(Closure, CallSite, Object, Object, String, Action`2)
                    //   at System.Dynamic.UpdateDelegates.UpdateAndExecute4[T0, T1, T2, T3, TRet](CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
                    //   at CallSite.Target(Closure, CallSite, Object, Object, String, Action`2)
                    //   at System.Dynamic.UpdateDelegates.UpdateAndExecuteVoid4[T0, T1, T2, T3](CallSite site, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
                    //   at CSharpRecipes.ReflectionAndDynamicProgramming.TestExpandoObject() in C:\CS60_Cookbook\CSCB6\CSharpRecipes\06_ReflectionAndDynamicProgramming.cs:line 564
                    //   at CSharpRecipes.MainTester.CH06_TestReflectionAndDynamicProgramming() in C:\CS60_Cookbook\CSCB6\CSharpRecipes\00_MainTester.cs:line 154
                    //   at CSharpRecipes.MainTester.Main() in C:\CS60_Cookbook\CSCB6\CSharpRecipes\00
                    //_MainTester.cs:line 308
                    Console.WriteLine($"Setting Language to : {langArgs?.Language}");
                    exp.Language = langArgs?.Language;
                });

            // Add a LanguageChanged event and predefined event handler
            AddEvent(expando, "LanguageChanged", eventHandler);

            // Add a CountryChanged event and an inline event handler
            AddEvent(expando, "CountryChanged", 
                new Action<object, EventArgs>((sender, eventArgs) =>
            {
                dynamic exp = sender as ExpandoObject;
                var ctryArgs = eventArgs as CountryChangedEventArgs;
                string newLanguage = string.Empty;
                switch (ctryArgs?.Country)
                {
                    case "France":
                        newLanguage = "French";
                        break;
                    case "China":
                        newLanguage = "Mandarin";
                        break;
                    case "Spain":
                        newLanguage = "Spanish";
                        break;
                }
                Console.WriteLine($"Country changed to {ctryArgs?.Country}, " + 
                    $"changing Language to {newLanguage}");
                exp?.LanguageChanged(sender, 
                    new LanguageChangedEventArgs() { Language = newLanguage });
            }));

            // Hook up for the PropertyChanged notification (as ExpandoObject supports it), handy for databinding scenarios...
            ((INotifyPropertyChanged)expando).PropertyChanged += 
                new PropertyChangedEventHandler((sender, ea) =>
            {
                dynamic exp = sender as dynamic;
                var pcea = ea as PropertyChangedEventArgs;
                if(pcea?.PropertyName == "Country")
                    exp.CountryChanged(exp, new CountryChangedEventArgs() { Country = exp.Country });
            });

            // Current state
            Console.WriteLine($"expando contains: {expando.Name}, {expando.Country}, {expando.Language}");
            Console.WriteLine();

            Console.WriteLine("Changing Country to France...");
            expando.Country = "France";
            Console.WriteLine($"expando contains: {expando.Name}, {expando.Country}, {expando.Language}");
            Console.WriteLine();

            Console.WriteLine("Changing Country to China...");
            expando.Country = "China";
            Console.WriteLine($"expando contains: {expando.Name}, {expando.Country}, {expando.Language}");
            Console.WriteLine();

            Console.WriteLine("Changing Country to Spain...");
            expando.Country = "Spain";
            Console.WriteLine($"expando contains: {expando.Name}, {expando.Country}, {expando.Language}");
            Console.WriteLine();

            //expando contains: Brian, USA, English

            //Changing Country to France...
            //Country changed to France, changing Language to French
            //Setting Language to: French
            //expando contains: Brian, France, French

            //Changing Country to China...
            //Country changed to China, changing Language to Mandarin
            //Setting Language to: Mandarin
            //expando contains: Brian, China, Mandarin

            //Changing Country to Spain...
            //Country changed to Spain, changing Language to Spanish
            //Setting Language to: Spanish
            //expando contains: Brian, Spain, Spanish

        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static void AddEvent(ExpandoObject expando, string eventName, Action<object, EventArgs> handler)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(eventName))
                expandoDict[eventName] = handler;
            else
                expandoDict.Add(eventName, handler);
        }
        public class LanguageChangedEventArgs : EventArgs
        {
            public string Language { get; set; }
        }

        public class CountryChangedEventArgs : EventArgs
        {
            public string Country { get; set; }
        }

        #endregion

        #region "6.9 Make Your Objects Extensible"
        public static void TestMakeYourObjectsExtensible()
        {
            // Create a set of information on athletes
            // Note that the service receiving these doesn't have Position as a 
            // property on the Athlete object
            dynamic initialAthletes = new[]
            {
                new
                {
                    Name = "Tom Brady",
                    Sport = "Football",
                    Position = "Quarterback"
                },
                new
                {
                    Name = "Derek Jeter",
                    Sport = "Baseball",
                    Position = "Shortstop"
                },
                new
                {
                    Name = "Michael Jordan",
                    Sport = "Basketball",
                    Position = "Small Forward"
                },
                new
                {
                    Name = "Lionel Messi",
                    Sport = "Soccer",
                    Position = "Forward"
                }
            };

            // serialize the JSON to send to a web service about Athletes...
            string serializedAthletes = JsonNetSerialize(initialAthletes);

            // deserialize the JSON we were sent
            var athletes = JsonNetDeserialize<DynamicAthlete[]>(serializedAthletes);

            dynamic da = athletes[0];
            Console.WriteLine($"Position of first athlete: {da.Position}");
            // Inspect the Athletes and see that we not only got the Position 
            // information, but that we can add an operation to work on the 
            // entity and invoke that as part of the dynamic entity
            foreach(var athlete in athletes)
            {
                dynamic dynamicAthlete = (dynamic)athlete;
                dynamicAthlete.GetUppercaseName =
                    (Func<string>)(() =>
                    {
                        return ((string)dynamicAthlete.Name).ToUpper();
                    });
                Console.WriteLine($"Athlete:");
                Console.WriteLine(athlete);
                Console.WriteLine($"Uppercase Name: {dynamicAthlete.GetUppercaseName()}");
                Console.WriteLine();
                Console.WriteLine();
            }


            //Wrap an existing athlete
            StaticAthlete staticAthlete = new StaticAthlete()
            {
                Sport = "Hockey"
            };

            dynamic extendedAthlete = new DynamicBase<StaticAthlete>(staticAthlete);
            extendedAthlete.Name = "Bobby Orr";
            extendedAthlete.Position = "Defenseman";
            extendedAthlete.GetUppercaseName =
                    (Func<string>)(() =>
                    {
                        return ((string)extendedAthlete.Name).ToUpper();
                    });
            Console.WriteLine($"Static Athlete (extended):");
            Console.WriteLine(extendedAthlete);
            Console.WriteLine($"Uppercase Name: {extendedAthlete.GetUppercaseName()}");
            Console.WriteLine();
            Console.WriteLine();
        }

        public static string JsonNetSerialize<T>(T obj)
        {
            List<JsonConverter> cvt = new List<JsonConverter>();
            cvt.Add(new IsoDateTimeConverter());
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                Converters = cvt,
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }

        public static T JsonNetDeserialize<T>(string json)
        {
            List<JsonConverter> cvt = new List<JsonConverter>();
            // Create Json.Net formatter serializing DateTime using the ISO 8601 format
            cvt.Add(new IsoDateTimeConverter());
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                Converters = cvt,
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
        #endregion
    }


    public class DynamicAthlete : DynamicBase<DynamicAthlete>
    {
        public string Name { get; set; }
        public string Sport { get; set; }
    }

    public class StaticAthlete
    {
        public string Name { get; set; }
        public string Sport { get; set; }
    }

    public class DynamicBase<T> : DynamicObject
        where T : new()
    {
        private T _containedObject = default(T);

        [JsonExtensionData] //JSON.NET 5.0 and above
        private Dictionary<string, object> _dynamicMembers = new Dictionary<string, object>();

        private List<PropertyInfo> _propertyInfos = new List<PropertyInfo>(typeof(T).GetProperties());

        public DynamicBase()
        {
        }
        public DynamicBase(T containedObject)
        {
            _containedObject = containedObject;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (_dynamicMembers.ContainsKey(binder.Name) && _dynamicMembers[binder.Name] is Delegate)
            {
                result = (_dynamicMembers[binder.Name] as Delegate).DynamicInvoke(args);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames() => _dynamicMembers.Keys;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            var propertyInfo = _propertyInfos.Where(pi => pi.Name == binder.Name).FirstOrDefault();
            // Make sure this member isn't a property on the object yet
            if (propertyInfo == null)
            {
                // look in the additional items collection for it
                if (_dynamicMembers.Keys.Contains(binder.Name))
                {
                    // return the dynamic item
                    result = _dynamicMembers[binder.Name];
                    return true;
                }
            }
            else
            {
                // get it from the contained object
                if (_containedObject != null)
                {
                    result = propertyInfo.GetValue(_containedObject);
                    return true;
                }
            }
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = _propertyInfos.Where(pi => pi.Name == binder.Name).FirstOrDefault();
            // Make sure this member isn't a property on the object yet
            if (propertyInfo == null)
            {
                // look in the additional items collection for it
                if (_dynamicMembers.Keys.Contains(binder.Name))
                {
                    // set the dynamic item
                    _dynamicMembers[binder.Name] = value;
                    return true;
                }
                else
                {
                    _dynamicMembers.Add(binder.Name, value);
                    return true;
                }
            }
            else
            {
                // put it in the contained object
                if (_containedObject != null)
                {
                    propertyInfo.SetValue(_containedObject, value);
                    return true;
                }
            }
            return base.TrySetMember(binder, value);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var propInfo in _propertyInfos)
            {
                if(_containedObject != null)
                    builder.AppendFormat("{0}:{1}{2}", propInfo.Name, propInfo.GetValue(_containedObject), Environment.NewLine);
                else
                    builder.AppendFormat("{0}:{1}{2}", propInfo.Name, propInfo.GetValue(this), Environment.NewLine);
            }
            foreach (var addlItem in _dynamicMembers)
            {
                // exclude methods that are added from the description
                Type itemType = addlItem.Value.GetType();
                Type genericType = 
                    itemType.IsGenericType ? itemType.GetGenericTypeDefinition() : null;
                if (genericType != null)
                {
                    if (genericType != typeof(Func<>) &&
                        genericType != typeof(Action<>))
                        builder.AppendFormat("{0}:{1}{2}", addlItem.Key, addlItem.Value, Environment.NewLine);
                }
                else
                    builder.AppendFormat("{0}:{1}{2}", addlItem.Key, addlItem.Value, Environment.NewLine);
            }
            return builder.ToString();
        }
    }
}
