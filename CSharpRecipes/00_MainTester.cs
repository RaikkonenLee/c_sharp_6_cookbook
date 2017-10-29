using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharpRecipes
{
    class MainTester
    {
        #region "(1) CLASSES AND GENERICS CHAPTER TEST CODE"
        static void CH01_TestClassesAndGenerics()
        {
            PrintHeader("Classes and Generics Chapter Tests");
            ClassesAndGenerics.TestUnions();
            ClassesAndGenerics.TestSort();
            ClassesAndGenerics.TestSearch();
            ClassesAndGenerics.TestParser(new string[] { "c:\\input\\infile.txt", "-output:d:\\outfile.txt", "-trialmode" });
            ClassesAndGenerics.TestParser(new string[] { });
            ClassesAndGenerics.TestParser(new string[] { "c:\\input\\infile.txt", "-output:", "-trialmode" });
            ClassesAndGenerics.TestCloning();
            ClassesAndGenerics.TestGenericClassInstanceCounter();
            ClassesAndGenerics.ShowSettingFieldsToDefaults();
            ClassesAndGenerics.TestDisposableListCls();
            ClassesAndGenerics.TestComparableListCls();
            ClassesAndGenerics.TestConversionCls();
            ClassesAndGenerics.TestReversibleSortedList();

            ClassesAndGenerics.InvokeInReverse();
            ClassesAndGenerics.InvokeEveryOtherOperation();
            ClassesAndGenerics.InvokeWithTest();
            ClassesAndGenerics.TestIndividualInvokesReturnValue();
            ClassesAndGenerics.TestIndividualInvokesExceptions();
            ClassesAndGenerics.TestClosure();
            ClassesAndGenerics.TestFunctors();

            ClassesAndGenerics.TestPartialMethods();

            ClassesAndGenerics.TestInitializeStructs.UseNewInitialization();
            ClassesAndGenerics.TestInitializeStructs.UseDefaultInitialization();
            ClassesAndGenerics.TestInitializeStructs.UseOverloadedctorInitialization();
            ClassesAndGenerics.TestForNull();
            ClassesAndGenerics.TestForNullDelegate();
        }
        #endregion

        #region "(2) COLLECTIONS, ENUMERATIONS, AND ITERATORS CHAPTER TEST CODE"
        static async Task CH02_TestCollectionsEnumsAndIterators()
        {
            PrintHeader("Collections, Enumerations and Iterators Chapter Tests");
            CollectionsEnumerationsAndIterators.TestDuplicateItemsListT();
            CollectionsEnumerationsAndIterators.TestSortedList();
            CollectionsEnumerationsAndIterators.TestSortKeyValues();
            CollectionsEnumerationsAndIterators.TestMaxMinValueDictionary();
            CollectionsEnumerationsAndIterators.TestSerialization();
            CollectionsEnumerationsAndIterators.TestArrayForNulls();
            CollectionsEnumerationsAndIterators.TestIterators();
            CollectionsEnumerationsAndIterators.TestFinallyAndIterators();
            CollectionsEnumerationsAndIterators.CreateNestedObjects();
            await CollectionsEnumerationsAndIterators.TestConcurrentDictionary();
        }
        #endregion

        #region "(3) DATATYPES CHAPTER TEST CODE"


        static void CH03_TestDataTypes()
        {
            PrintHeader("Data Types Chapter Tests");

            DataTypes.TestEncodingBinaryBase64();
            DataTypes.TestDecodingBinaryBase64();
            DataTypes.TestConvertingStringAsByteArrayToString();
            DataTypes.TestDetermineIfStringIsNumber();
            DataTypes.TestRound();
            DataTypes.TestNarrowing();
            DataTypes.TestValidEnumValue();
            DataTypes.TestEnumBitmask();
            DataTypes.TestEnumFlags();
        }
        #endregion

        #region "(4) LINQ AND LAMBDA EXPRESSIONS CHAPTER TEST CODE"
        static void CH04_TestLINQAndLambda()
        {
            LINQAndLambda.TestLinqMessageQueue();
            LINQAndLambda.TestSetSemantics();
            LINQAndLambda.TestCompiledQuery();
            LINQAndLambda.TestLinqForCulture();
            LINQAndLambda.TestWeightedMovingAverage();
            LINQAndLambda.TestQueryConfig();
            LINQAndLambda.TestLinqToDataSet();
            LINQAndLambda.TestXmlFromDatabase();
            LINQAndLambda.TestTakeSkipWhile();
            LINQAndLambda.TestUsingNonIEnumT();
            LINQAndLambda.FindSpecificInterfaces();
            LINQAndLambda.TestUsingLambdaExpressions();
            LINQAndLambda.TestParameterModifiers();
            LINQAndLambda.TestPLINQ();
        }
        #endregion 

		#region "(5) DEBUGGING AND EXCEPTION HANDLING CHAPTER TEST CODE"
        static async Task CH05_TestDebuggingAndExceptionHandling()
        {
            PrintHeader("Debugging and Exception Handling Chapter Tests");
            DebuggingAndExceptionHandling instance = new DebuggingAndExceptionHandling();
            DebuggingAndExceptionHandling.ReflectionException();

            DebuggingAndExceptionHandling.TestSpecializedException();
            try
            {
                DebuggingAndExceptionHandling.TestPollingAsyncDelegate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            DebuggingAndExceptionHandling.TestPollingAsyncDelegate();
            DebuggingAndExceptionHandling.TestExceptionData();
            DebuggingAndExceptionHandling.ProcessRespondingState state;
            foreach (Process P in Process.GetProcesses())
            {
                state = DebuggingAndExceptionHandling.GetProcessState(P);
                if (state == DebuggingAndExceptionHandling.ProcessRespondingState.NotResponding)
                {
                    Console.WriteLine("{0} is not responding.", P.ProcessName);
                }
            }
            DebuggingAndExceptionHandling.TestEventLogClass();
            DebuggingAndExceptionHandling.WatchForAppEvent((EventLog.GetEventLogs())[0]);
            DebuggingAndExceptionHandling.TestCreateSimpleCounter();
            DebuggingAndExceptionHandling.TestCustomDebuggerDisplay();
            instance.TestCallerInfoAttribs();
            await instance.TestHandlingAsyncExceptionsAsync();
            instance.TestExceptionFilters();
        }
        #endregion

        #region "(6) REFLECTION AND DYNAMIC PROGRAMMING CHAPTER TEST CODE"
        static void CH06_TestReflectionAndDynamicProgramming()
        {
            PrintHeader("Reflection and Dynamic Programming Chapter Tests");

            //ReflectionAndDynamicProgramming.ListImportedAssemblies();
            //ReflectionAndDynamicProgramming.DetermineTypeCharacteristics();
            //ReflectionAndDynamicProgramming.DetermineInheritanceCharacteristics();
            //ReflectionAndDynamicProgramming.ReflectionInvocation();
            //ReflectionAndDynamicProgramming.TestGetLocalVars();
            //ReflectionAndDynamicProgramming.CreateDictionary();
            //ReflectionAndDynamicProgramming.TestDynamicVsObject();
            //ReflectionAndDynamicProgramming.TestBuildingObjectsDynamically();
            //ReflectionAndDynamicProgramming.TestMakeYourObjectsExtensible();
        }
        #endregion

        #region "(7) REGULAR EXPRESSIONS CHAPTER TEST CODE"
        static void CH07_TestRegularExpressions()
        {
			PrintHeader("Regular Expressions Chapter Tests");
			RegEx.TestExtractGroupings();
			RegEx.TestUserInputRegEx("");
			RegEx.TestUserInputRegEx(@"");
			RegEx.TestUserInputRegEx("foo");
			RegEx.TestUserInputRegEx(@"\\\");
			RegEx.TestUserInputRegEx(@"\\\\");
            RegEx.TestUserInputRegEx(null);
            RegEx.TestComplexReplace();
			RegEx.TestTokenize();
			RegEx.TestGetLine();
			RegEx.TestOccurrencesOf();
        }
        #endregion
        
        #region "(8) FILE SYSTEM IO CHAPTER TEST CODE"
        static async Task CH08_TestFileSystemIO()
        {
            PrintHeader("File System IO Chapter Tests");

            FileSystemIO.SearchDirFileWildcards();
            FileSystemIO.ObtainDirTree();
            FileSystemIO.ParsePath();
            FileSystemIO.LaunchInteractConsoleUtilities();
            await FileSystemIO.LockSubsectionsOfAFile();
            FileSystemIO.WaitFileSystemAction();
            FileSystemIO.CompareVersionInfo();
            FileSystemIO.TestAllDriveInfo();
            FileSystemIO.TestCompressNewFile();
        }
        #endregion
        
		#region "(9) NETWORKING AND WEB CHAPTER TEST CODE"
		static async Task CH09_TestNetworkingAndWeb()
		{
            await NetworkingAndWeb.HandlingWebServerErrorsAsync();
            await NetworkingAndWeb.CommunicatingWithWebServerAsync();
            NetworkingAndWeb.GoingThroughProxy();
            NetworkingAndWeb.ObtainingHtmlFromUrl();
            NetworkingAndWeb.TestBuildAspNetPages();
            NetworkingAndWeb.TestEscapeUnescape();
            NetworkingAndWeb.GetCustomErrorPageLocations();
            NetworkingAndWeb.GetCustomErrorPageLocationsLinq();
            await NetworkingAndWeb.SimulatingFormExecution();
            await NetworkingAndWeb.DownloadingDataFromServerAsync();
            await NetworkingAndWeb.UploadingDataToServerAsync();
            await NetworkingAndWeb.TestPing();
            await NetworkingAndWeb.TestSendMailAsync();
            await NetworkingAndWeb.TestPortScanner();
            NetworkingAndWeb.GetInternetSettings();
            await NetworkingAndWeb.TestFtpAsync();
        }
        #endregion

        #region "(10) XML CHAPTER TEST CODE"
        static void CH10_TestXML()
		{
			PrintHeader("XML Chapter Tests");

            XML.AccessXml();
            XML.QueryXml();
            XML.ValidateXml();
            XML.DetectXmlChanges();
            XML.HandleInvalidChars();
            XML.TransformXml();
            XML.TestContinualValidation();
            XML.TestExtendingTransformations();
            XML.TestBulkSchema();
            XML.TestXsltParameters();
		}
		#endregion

		#region "(11) SECURITY CHAPTER TEST CODE"
        static void CH11_TestSecurity()
        {
            PrintHeader("Security Chapter Tests");
            Security.EncDecString();
            Security.EncDecFile();
            Security.CleanUpCrypto();
            Security.VerifyStringIntegrity();
            Security.SafeAssert();
            Security.VerifyAssemblyPerms();
            Security.MinimizeAttackSurface();
            Security.TestViewFileRegRights();
            Security.TestGrantRevokeFileRights();
            Security.TestSecureString();
            Security.TestEncryptDecryptWebConfigSection();
            Security.TestPasswordHashing();

        }
        #endregion

        #region "(12) THREADING, SYNCHRONIZATION, AND CONCURRENCY CHAPTER TEST CODE"
        static async Task CH12_TestThreadingSyncAndConcurrency()
        {
            PrintHeader("Threading, Synchronization, and Concurrency Chapter Tests");

            ThreadingSyncAndConcurrency.PerThreadStatic();
            ThreadingSyncAndConcurrency.ThreadSafeAccess();
            ThreadingSyncAndConcurrency.CompletionAsyncDelegate();
            ThreadingSyncAndConcurrency.PreventSilentTermination();
            ThreadingSyncAndConcurrency.StoreThreadDataPrivately();
            ThreadingSyncAndConcurrency.Halo5Session.Play();
            ThreadingSyncAndConcurrency.TestResetEvent();
            ThreadingSyncAndConcurrency.TestInterlocked();
            ThreadingSyncAndConcurrency.TestReaderWriterLockSlim();
            await ThreadingSyncAndConcurrency.TestAsyncDatabase();
            ThreadingSyncAndConcurrency.TestTaskContinuation();
        }
        #endregion
        
		#region "(13) TOOLBOX CHAPTER TEST CODE"
		static void CH13_TestToolbox()
		{
            Toolbox.PreventBadShutdown();
            SharedCode.Shared.TestAssemblyInProcesses();
            Toolbox.TestMessageQueue();
            Toolbox.TestRedirectOutput();
            Toolbox.TestCaptureOutput();
            Toolbox.RunCodeInNewAppDomain();
            Console.WriteLine(Toolbox.GetOSAndServicePack()); 
        }
		#endregion

		static void Main()
        {
            try
            {
                //CH01_TestClassesAndGenerics();
                //Task ch02 = CH02_TestCollectionsEnumsAndIterators();
                //ch02.Wait();
                //CH03_TestDataTypes();
                //CH04_TestLINQAndLambda();
                //try
                //{
                //    Task ch05 = CH05_TestDebuggingAndExceptionHandling();
                //    ch05.Wait();
                //}
                //catch (System.Exception e)o
                //{
                //    Console.WriteLine(e);
                //}
                CH06_TestReflectionAndDynamicProgramming();
                CH07_TestRegularExpressions();
                Task ch08 = CH08_TestFileSystemIO();
                ch08.Wait();
                Task ch09 = CH09_TestNetworkingAndWeb();
                ch09.Wait();
                CH10_TestXML();
                CH11_TestSecurity();
                Task ch12 = CH12_TestThreadingSyncAndConcurrency();
                ch12.Wait();
                CH13_TestToolbox();
            }
            catch(Exception e)
            {
               Console.WriteLine(e.ToString());
            }

            // wait for enter to be pressed
            Console.WriteLine("Press ENTER to finish...");
            Console.ReadLine();
        }

         
        #region UtilityCode
        static void PrintHeader(string testtype)
        {
            Console.WriteLine("************************************************************");
            Console.WriteLine("**    Running " + testtype + "...");
            Console.WriteLine("************************************************************");
        }
        #endregion
    }
}
