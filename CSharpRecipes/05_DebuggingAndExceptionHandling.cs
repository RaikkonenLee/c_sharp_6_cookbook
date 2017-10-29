#define TRACE
#define TRACE_INSTANTIATION
#define TRACE_BEHAVIOR

using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Specialized;
using System.Linq;
using System.Data;

//EventLogInstaller
using System.ComponentModel;
using System.Threading.Tasks;
using System.Data.Common;
using System.Runtime.CompilerServices;


//Done deliberately in 5.3 to show overload of ==
#pragma warning disable 1718 //Comparison made to same variable; did you mean to compare something else? 

namespace CSharpRecipes
{

    public static class DebuggingAndExceptionHandlingExtensions
    {
        public static IEnumerable<Exception> GetNestedExceptionList(this Exception exception)
        {
            Exception current = exception;
            do
            {
                current = current.InnerException;
                if (current != null)
                    yield return current;
            }
            while (current != null);
        }

        public static string ToShortDisplayString(this Exception ex)
        {
            StringBuilder displayText = new StringBuilder();
            WriteExceptionShortDetail(displayText, ex);
            foreach (Exception inner in ex.GetNestedExceptionList()) 
            {
                displayText.AppendFormat("**** INNEREXCEPTION START ****{0}", Environment.NewLine);
                WriteExceptionShortDetail(displayText, inner);
                displayText.AppendFormat("**** INNEREXCEPTION END ****{0}{0}", Environment.NewLine);
            }
            return displayText.ToString();
        }

        public static void WriteExceptionShortDetail(StringBuilder builder, Exception ex)
        {
            builder.AppendFormat("Message: {0}{1}", ex.Message, Environment.NewLine);
            builder.AppendFormat("Type: {0}{1}", ex.GetType(), Environment.NewLine);
            builder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            builder.AppendFormat("TargetSite: {0}{1}", ex.TargetSite, Environment.NewLine);
        }

        public static string ToFullDisplayString(this Exception ex)
        {
            StringBuilder displayText = new StringBuilder();
            WriteExceptionDetail(displayText, ex);
            foreach (Exception inner in ex.GetNestedExceptionList()) 
            {
                displayText.AppendFormat("**** INNEREXCEPTION START ****{0}", Environment.NewLine);
                WriteExceptionDetail(displayText, inner);
                displayText.AppendFormat("**** INNEREXCEPTION END ****{0}{0}", Environment.NewLine);
            }
            return displayText.ToString();
        }

        public static void WriteExceptionDetail(StringBuilder builder, Exception ex)
        {
            builder.AppendFormat("Message: {0}{1}", ex.Message, Environment.NewLine);
            builder.AppendFormat("Type: {0}{1}", ex.GetType(), Environment.NewLine);
            builder.AppendFormat("HelpLink: {0}{1}", ex.HelpLink, Environment.NewLine);
            builder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            builder.AppendFormat("TargetSite: {0}{1}", ex.TargetSite, Environment.NewLine);
            builder.AppendFormat("Data:{0}", Environment.NewLine);
            foreach (DictionaryEntry de in ex.Data)
            {
                builder.AppendFormat("\t{0} : {1}{2}",
                    de.Key, de.Value, Environment.NewLine);
            }
            builder.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
        }
    }


	public class DebuggingAndExceptionHandling
    {
        #region "5.0 Introduction"
        public static void SetValue(object value)
        {
            try
            {
                //myObj.Property1 = value;
            }
            catch (NullReferenceException)
            {
                // Handle potential exceptions arising from this call here. 
            }
        }

        public static void CallCOMMethod1()
        {
            try
            {
                // Call a method on a COM object. 
                //myCOMObj.Method1();
            }
            catch
            {
                //Handle potential exceptions arising from this call here.
            }
        }


        public static void CallCOMMethod2()
        {
            try
            {
                // Call a method on a COM object.       
                //myCOMObj.Method1();
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                // Handle potential COM exceptions arising from this call here.
            }
            catch (InvalidOperationException)
            {
                // Handle any potential method calls to the COM object which are
                // not valid in its current state.
            }
        }

        public static void CallCOMMethod3()
        {
            try
            {
                // Call a method on a COM object.
                //myCOMObj.Method1();
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                // Handle potential COM exceptions arising from this call here.
            }
            finally
            {
                // Clean up and free any resources here. 	    
                // For example, there could be a method on myCOMObj to allow us to clean
                // up after using the Method1 method.    	    
            }
        }

        public static int GetAuthorCount(string connectionString)
        {
            SqlConnection sqlConn = null;
            SqlCommand sqlComm = null;

            using(sqlConn = new SqlConnection(connectionString))
            {
                using (sqlComm = new SqlCommand())
                {
                    sqlComm.Connection = sqlConn;
                    sqlComm.Parameters.Add("@pubName", 
                        SqlDbType.NChar).Value = "O''Reilly";
                    sqlComm.CommandText = "SELECT COUNT(*) FROM Authors " +
                        "WHERE Publisher=@pubName";

                    sqlConn.Open();
                    object authorCount = sqlComm.ExecuteScalar();
                    return (int)authorCount;
                }
            }
        }

        // Just here to help the sample code compile
        public static Stream GetAnyAvailableStream() => null;

        public static void SomeMethod1()
        {
            try
            {
                Stream s = GetAnyAvailableStream();
                Console.WriteLine("This stream has a length of " + s.Length);
            }
            catch (NullReferenceException)
            {
                // Handle a null stream here.
            }
        }

        public static void SomeMethod2()
        {
            Stream s = GetAnyAvailableStream();

            if (s != null)
            {
                Console.WriteLine("This stream has a length of " + s.Length);
            }
            else
            {
                // Handle a null stream here.      
            }
        }

        public static void SomeMethod3()
        {
            Stream s = null;
            using (s = GetAnyAvailableStream())
            {

                if (s != null)
                {
                    Console.WriteLine("This stream has a length of " + s.Length);
                }
                else
                {
                    // Handle a null stream here.
                }
            }
        }



        #endregion 

        #region "5.1 Knowing When to Catch and Rethrow Exceptions"
        public static void TestExceptionCode()
        {
            try
            {
                // *********sample code start
                try
                {
                    Console.WriteLine("In try");
                    int z2 = 9999999;
                    checked { z2 *= 999999999; }
                }
                catch (OverflowException oe)
                {
                    // Record the fact that the overflow exception occurred.
                    EventLog.WriteEntry("MyApplication", oe.Message, EventLogEntryType.Error);
                    throw;
                }
                // *********sample code end
            }
            catch (Exception)
            {
                // just here to help the sample code run
            }
        }


		#endregion

        #region "5.2 Handling Exceptions Thrown from Methods Invoked Via Reflection"
        public static void ReflectionException()
        {
            Type reflectedClass = typeof(DebuggingAndExceptionHandling);

            try
            {
	            MethodInfo methodToInvoke = reflectedClass.GetMethod("TestInvoke");
		        methodToInvoke?.Invoke(null, null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToShortDisplayString());
            }
        }

        // Output
        //Message: Exception has been thrown by the target of an invocation.
        //Type: System.Reflection.TargetInvocationException
        //Source: mscorlib
        //TargetSite: System.Object InvokeMethod(System.Object, System.Object[], System.Si
        //gnature, Boolean)
        //**** INNEREXCEPTION START ****
        //Message: Thrown from invoked method.
        //Type: System.Exception
        //Source: CSharpRecipes
        //TargetSite: Void TestInvoke()
        //**** INNEREXCEPTION END ****

        public static void TestInvoke()
        {
            throw (new Exception("Thrown from invoked method."));
        }

        // See the DebuggingAndExceptionHandlingExtensions class for the other methods used here
        		
        #endregion
        
        #region "5.3 Creating a New Exception Type"		
		public static void TestSpecializedException()
		{
			// Generic inner exception used to test the RemoteComponentException's inner exception
			Exception inner = new Exception("The Inner Exception");
    
			// Test each ctor
			Console.WriteLine(Environment.NewLine + Environment.NewLine + "TEST EACH CTOR");
			RemoteComponentException se1 = new RemoteComponentException ();
			RemoteComponentException se2 = new RemoteComponentException ("A Test Message for se2");
			RemoteComponentException se3 = new RemoteComponentException ("A Test Message for se3", inner);
			RemoteComponentException se4 = new RemoteComponentException ("A Test Message for se4", 
				"MyServer");
			RemoteComponentException se5 = new RemoteComponentException ("A Test Message for se5", inner, 
				"MyServer");

			// Test new ServerName property
			Console.WriteLine(Environment.NewLine + "TEST NEW SERVERNAME PROPERTY");
			Console.WriteLine("se1.ServerName == " + se1.ServerName);
			Console.WriteLine("se2.ServerName == " + se2.ServerName);
			Console.WriteLine("se3.ServerName == " + se3.ServerName);
			Console.WriteLine("se4.ServerName == " + se4.ServerName);
			Console.WriteLine("se5.ServerName == " + se5.ServerName);

			// Test overridden Message property
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- MESSAGE PROPERTY");
			Console.WriteLine("se1.Message == " + se1.Message);
			Console.WriteLine("se2.Message == " + se2.Message);
			Console.WriteLine("se3.Message == " + se3.Message);
			Console.WriteLine("se4.Message == " + se4.Message);
			Console.WriteLine("se5.Message == " + se5.Message);

			// Test -overridden- ToString method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- TOSTRING METHOD");
			Console.WriteLine("se1.ToString() == " + se1.ToString());
			Console.WriteLine("se2.ToString() == " + se2.ToString());
			Console.WriteLine("se3.ToString() == " + se3.ToString());
			Console.WriteLine("se4.ToString() == " + se4.ToString());
			Console.WriteLine("se5.ToString() == " + se5.ToString());

			// Test ToBaseString method
			Console.WriteLine(Environment.NewLine + "TEST TOBASESTRING METHOD");
			Console.WriteLine("se1.ToBaseString() == " + se1.ToBaseString());
			Console.WriteLine("se2.ToBaseString() == " + se2.ToBaseString());
			Console.WriteLine("se3.ToBaseString() == " + se3.ToBaseString());
			Console.WriteLine("se4.ToBaseString() == " + se4.ToBaseString());
			Console.WriteLine("se5.ToBaseString() == " + se5.ToBaseString());

			// Test -overridden- == operator
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- == OPERATOR");
            Console.WriteLine("se1 == se1 == " + (se1 == se1));
			Console.WriteLine("se2 == se1 == " + (se2 == se1));
			Console.WriteLine("se3 == se1 == " + (se3 == se1));
			Console.WriteLine("se4 == se1 == " + (se4 == se1));
			Console.WriteLine("se5 == se1 == " + (se5 == se1));
			Console.WriteLine("se5 == se4 == " + (se5 == se4));

			// Test -overridden- != operator
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- != OPERATOR");
			Console.WriteLine("se1 != se1 == " + (se1 != se1));
			Console.WriteLine("se2 != se1 == " + (se2 != se1));
			Console.WriteLine("se3 != se1 == " + (se3 != se1));
			Console.WriteLine("se4 != se1 == " + (se4 != se1));
			Console.WriteLine("se5 != se1 == " + (se5 != se1));
			Console.WriteLine("se5 != se4 == " + (se5 != se4));

			// Test -overridden- GetBaseException method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- GETBASEEXCEPTION METHOD");
			Console.WriteLine("se1.GetBaseException() == " + se1.GetBaseException());
			Console.WriteLine("se2.GetBaseException() == " + se2.GetBaseException());
			Console.WriteLine("se3.GetBaseException() == " + se3.GetBaseException());
			Console.WriteLine("se4.GetBaseException() == " + se4.GetBaseException());
			Console.WriteLine("se5.GetBaseException() == " + se5.GetBaseException());

			// Test -overridden- GetHashCode method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- GETHASHCODE METHOD");
			Console.WriteLine("se1.GetHashCode() == " + se1.GetHashCode());
			Console.WriteLine("se2.GetHashCode() == " + se2.GetHashCode());
			Console.WriteLine("se3.GetHashCode() == " + se3.GetHashCode());
			Console.WriteLine("se4.GetHashCode() == " + se4.GetHashCode());
			Console.WriteLine("se5.GetHashCode() == " + se5.GetHashCode());

			// Test serialization
			Console.WriteLine(Environment.NewLine + "TEST SERIALIZATION/DESERIALIZATION");
			BinaryFormatter binaryWrite = new BinaryFormatter();
			Stream ObjectFile = File.Create("se1.object");
			binaryWrite.Serialize(ObjectFile, se1);
			ObjectFile.Close();
			ObjectFile = File.Create("se2.object");
			binaryWrite.Serialize(ObjectFile, se2);
			ObjectFile.Close();
			ObjectFile = File.Create("se3.object");
			binaryWrite.Serialize(ObjectFile, se3);
			ObjectFile.Close();
			ObjectFile = File.Create("se4.object");
			binaryWrite.Serialize(ObjectFile, se4);
			ObjectFile.Close();
			ObjectFile = File.Create("se5.object");
			binaryWrite.Serialize(ObjectFile, se5);
			ObjectFile.Close();

			BinaryFormatter binaryRead = new BinaryFormatter();
			ObjectFile = File.OpenRead("se1.object");
			object Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se2.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se3.object");
			Data = binaryRead.Deserialize(ObjectFile);    
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();    
			ObjectFile = File.OpenRead("se4.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se5.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data + Environment.NewLine 
				+ "----------");
			ObjectFile.Close();

			Console.WriteLine(Environment.NewLine + "END TEST" + Environment.NewLine);
		}

        // SAMPLE OUTPUT
//        TEST -OVERRIDDEN- MESSAGE PROPERTY
//se1.Message == Exception of type 'CSharpRecipes.DebuggingAndExceptionHandling+Re
//moteComponentException' was thrown.
//The server(Unknown)has encountered an error.
//se2.Message == A Test Message for se2
//The server (Unknown)has encountered an error.
//se3.Message == A Test Message for se3
//The server (Unknown)has encountered an error.
//se4.Message == A Test Message for se4
//The server (MyServer)has encountered an error.
//se5.Message == A Test Message for se5
//The server (MyServer)has encountered an error.

//TEST -OVERRIDDEN- TOSTRING METHOD
//se1.ToString() == An error has occured in a server component of this client.
//Server Name:
//Message: Exception of type 'CSharpRecipes.DebuggingAndExceptionHandling+RemoteCo
//mponentException' was thrown.
//The server(Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se2.ToString() == An error has occured in a server component of this client.
//Server Name:
//Message: A Test Message for se2
//The server(Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se3.ToString() == An error has occured in a server component of this client.
//Server Name:
//Message: A Test Message for se3
//The server(Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION START ****
//Message: The Inner Exception
//Type: System.Exception
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION END ****


//se4.ToString() == An error has occured in a server component of this client.
//Server Name: MyServer
//Message: A Test Message for se4
//The server(MyServer)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se5.ToString() == An error has occured in a server component of this client.
//Server Name: MyServer
//Message: A Test Message for se5
//The server(MyServer)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION START ****
//Message: The Inner Exception
//Type: System.Exception
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION END ****



//TEST TOBASESTRING METHOD
//se1.ToBaseString() == CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponen
//tException: Exception of type 'CSharpRecipes.DebuggingAndExceptionHandling+Remot
//eComponentException' was thrown.
//The server(Unknown)has encountered an error.
//se2.ToBaseString() == CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponen
//tException: A Test Message for se2
//The server(Unknown)has encountered an error.
//se3.ToBaseString() == CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponen
//tException: A Test Message for se3
//The server(Unknown)has encountered an error. ---> System.Exception: The Inner E
//xception
//   --- End of inner exception stack trace ---
//se4.ToBaseString() == CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponen
//tException: A Test Message for se4
//The server(MyServer)has encountered an error.
//se5.ToBaseString() == CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponen
//tException: A Test Message for se5
//The server(MyServer)has encountered an error. ---> System.Exception: The Inner
//Exception
//   --- End of inner exception stack trace ---

//TEST -OVERRIDDEN- == OPERATOR
//se1 == se1 == True
//se2 == se1 == False
//se3 == se1 == False
//se4 == se1 == False
//se5 == se1 == False
//se5 == se4 == False

//TEST -OVERRIDDEN- != OPERATOR
//se1 != se1 == False
//se2 != se1 == True
//se3 != se1 == True
//se4 != se1 == True
//se5 != se1 == True
//se5 != se4 == True

//TEST -OVERRIDDEN- GETBASEEXCEPTION METHOD
//se1.GetBaseException() == An error has occured in a server component of this cli
//ent.
//Server Name:
//Message: Exception of type 'CSharpRecipes.DebuggingAndExceptionHandling+RemoteCo
//mponentException' was thrown.
//The server (Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se2.GetBaseException() == An error has occured in a server component of this cli
//ent.
//Server Name:
//Message: A Test Message for se2
//The server (Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se3.GetBaseException() == System.Exception: The Inner Exception
//se4.GetBaseException() == An error has occured in a server component of this cli
//ent.
//Server Name: MyServer
//Message: A Test Message for se4
//The server (MyServer)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//se5.GetBaseException() == System.Exception: The Inner Exception

//TEST -OVERRIDDEN- GETHASHCODE METHOD
//se1.GetHashCode() == 46104728
//se2.GetHashCode() == 12289376
//se3.GetHashCode() == 43495525
//se4.GetHashCode() == 55915408
//se5.GetHashCode() == 33476626

//TEST SERIALIZATION/DESERIALIZATION
//----------
//An error has occured in a server component of this client.
//Server Name:
//Message: Exception of type 'CSharpRecipes.DebuggingAndExceptionHandling+RemoteCo
//mponentException' was thrown.
//The server(Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//----------
//An error has occured in a server component of this client.
//Server Name:
//Message: A Test Message for se2
//The server (Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//----------
//An error has occured in a server component of this client.
//Server Name:
//Message: A Test Message for se3
//The server (Unknown)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION START ****
//Message: The Inner Exception
//Type: System.Exception
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION END ****


//----------
//An error has occured in a server component of this client.
//Server Name: MyServer
//Message: A Test Message for se4
//The server (MyServer)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:

//----------
//An error has occured in a server component of this client.
//Server Name: MyServer
//Message: A Test Message for se5
//The server (MyServer)has encountered an error.
//Type: CSharpRecipes.DebuggingAndExceptionHandling+RemoteComponentException
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION START ****
//Message: The Inner Exception
//Type: System.Exception
//HelpLink:
//Source:
//TargetSite:
//Data:
//StackTrace:
//**** INNEREXCEPTION END ****


//----------

//END TEST



        [Serializable]
        public class RemoteComponentException : Exception, ISerializable
        {
            #region Constructors
            // Normal exception ctor's
	        public RemoteComponentException() : base()
	        {
	        }

	        public RemoteComponentException(string message) : base(message)
	        {
	        }

	        public RemoteComponentException(string message, Exception innerException) 
		        : base(message, innerException)
	        {
	        }

	        // Exception ctor's that accept the new ServerName parameter
	        public RemoteComponentException(string message, string serverName) : base(message)
	        {
		        this.ServerName = serverName;
	        }

	        public RemoteComponentException(string message, Exception innerException, string serverName) 
		        : base(message, innerException)
	        {
		        this.ServerName = serverName;
            }

            // Serialization ctor
            protected RemoteComponentException(SerializationInfo exceptionInfo,
                StreamingContext exceptionContext)
                : base(exceptionInfo, exceptionContext)
            {
                this.ServerName = exceptionInfo.GetString("ServerName");
            }
            #endregion // Constructors

            #region Properties
            // Read-only property for server name
            public string ServerName { get; }

            public override string Message => $"{base.Message}{Environment.NewLine}" +
                        $"The server ({this.ServerName ?? "Unknown"})" +
                        "has encountered an error.";
            #endregion // Properties

            #region Overridden methods
            // ToString method
            public override string ToString() => "An error has occured in a server component of this client." +
                    $"{Environment.NewLine}Server Name: " +
                    $"{this.ServerName}{Environment.NewLine}" +
                    $"{this.ToFullDisplayString()}";

            // Used during serialization to capture information about extra fields
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info,
                StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("ServerName", this.ServerName);
            }
            #endregion // Overridden methods

            // Call base.ToString method
            public string ToBaseString() => (base.ToString());
        }
        #endregion

        #region "5.4 Breaking on a First Chance Exception"
        // See recipe 5.4 in book for explanation.
        #endregion

        #region "5.5 Handling Exceptions Thrown from an Asynchronous Delegate"

        public static void TestPollingAsyncDelegate()
		{
			AsyncInvoke MI = new AsyncInvoke(TestAsyncInvoke.Method1);
			IAsyncResult AR = MI.BeginInvoke(null, null);
			
			while (!AR.IsCompleted)
			{
				System.Threading.Thread.Sleep(100);
				Console.Write('.');
			}
			Console.WriteLine("\r\n\r\nDONE Polling...");

            try
            {
                int RetVal = MI.EndInvoke(AR);
                Console.WriteLine("RetVal (Polling): " + RetVal);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
		}
		
		
		public delegate int AsyncInvoke();
		public class TestAsyncInvoke
		{
			public static int Method1()
			{
				//throw (new Exception("Method1"));
							Console.WriteLine("Invoked Method1");
							return (1);
			}
		}
        #endregion

        #region "5.6 Giving Exceptions the Extra Info They Need with Exception.Data"
        public static void TestExceptionData()
        {
            try
            {
                try
                {
                    try
                    {
                        try
                        {
                            ArgumentException irritable =
                                new ArgumentException("I'm irritable!");
                            irritable.Data["Cause"]="Computer crashed";
                            irritable.Data["Length"]=10;
                            throw irritable;
                        }
                        catch (Exception e)
                        {
                            // see if I can help...
                            if(e.Data.Contains("Cause"))
                                e.Data["Cause"]="Fixed computer";
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        e.Data["Comment"]="Always grumpy you are";
                        throw;
                    }
                }
                catch (Exception e)
                {
                    e.Data["Reassurance"]="Error Handled";
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception supporting data:");
                foreach(DictionaryEntry de in e.Data)
                {
                    Console.WriteLine("\t{0} : {1}",de.Key,de.Value);
                }
            }
            TestExceptionDataSerializable();
        }

        public static void TestExceptionDataSerializable()
        {
            Exception badMonkey =
                new Exception("You are a bad monkey!");
            try
            {
                badMonkey.Data["Details"] = new Monkey();
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        [Serializable]
        public class Monkey
        {
            public string Name { get; } = "George";
        }
        #endregion

        #region "5.7 Dealing with Unhandled Exceptions in a WinForms Application"
        // see the UnhandledThreadExceptions project
        #endregion

        #region "5.8 Dealing with Unhandled Exceptions in a Windows Presentation Foundation (WPF) Application"
        // see the UnhandledWPFExceptions project
        #endregion

        #region "5.9 Determining Whether a Process Has Stopped Responding"
        public static void TestProcessResponding()
        {
            var processes = Process.GetProcesses().ToArray();
            Array.ForEach(processes, p =>
                {
                    var processState = GetProcessState(p);
                    switch (processState)
                    {
                        case ProcessRespondingState.NotResponding:
                            Console.WriteLine($"{p.ProcessName} is not responding.");
                            break;
                        case ProcessRespondingState.Responding:
                            Console.WriteLine($"{p.ProcessName} is responding.");
                            break;
                        case ProcessRespondingState.Unknown:
                            Console.WriteLine($"{p.ProcessName}'s state could not be determined.");
                            break;
                    }
                });
        }
        public enum ProcessRespondingState
        {
            Responding,
            NotResponding,
            Unknown
        }

        public static ProcessRespondingState GetProcessState(Process p)
        {
            if (p.MainWindowHandle == IntPtr.Zero)
            {
                Trace.WriteLine($"{p.ProcessName} does not have a MainWindowHandle");
                return ProcessRespondingState.Unknown;
            }
            else
            {
                // This process has a MainWindowHandle
                if (!p.Responding)
                    return ProcessRespondingState.NotResponding;
                else
                    return ProcessRespondingState.Responding;
            }
        }
        #endregion

        #region "5.10 Using Event Logs in Your Application"
        public static void TestEventLogClass()
        {
            // See the AppEventsEventLogInstallerApp for getting the sources set up


            // Causes an exception
            //AppEvents AppEventLog1 = new AppEvents("AppLog", "AppLocal");
            //AppEvents GlobalEventLog1 = new AppEvents("Application", "AppLocal");
            //GlobalEventLog1.WriteToLog("",EventLogEntryType.Information, CategoryType.AppStartUp, EventIDType.ExceptionThrown);

            AppEvents appEvents = null;
            try
            {
                appEvents = new AppEvents("", "APPEVENTSOURCE");
                appEvents.WriteToLog("MESSAGE", EventLogEntryType.Information,
                    CategoryType.AppStartUp, EventIDType.ExceptionThrown,
                    new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
                appEvents.WriteToLog("MESSAGE", EventLogEntryType.Information,
                    CategoryType.ReadFromDB, EventIDType.Read);

                System.Threading.Thread.Sleep(250);

                var entries = appEvents.GetEntries()?.Cast<EventLogEntry>().ToArray();
                if (entries != null)
                    Array.ForEach(entries, evt =>
                    {
                        Console.WriteLine($"\r\nMessage:        {evt.Message}");
                        Console.WriteLine($"Category:       {evt.Category}");
                        Console.WriteLine($"CategoryNumber: {evt.CategoryNumber}");
                        Console.WriteLine($"EntryType:      {evt.EntryType.ToString()}");
                        Console.WriteLine($"InstanceId:     {evt.InstanceId}");
                        Console.WriteLine($"Index:          {evt.Index}");
                        Console.WriteLine($"MachineName:    {evt.MachineName}");
                        Console.WriteLine($"Source:         {evt.Source}");
                        Console.WriteLine($"TimeGenerated:  {evt.TimeGenerated}");
                        Console.WriteLine($"TimeWritten:    {evt.TimeWritten}");
                        Console.WriteLine($"UserName:       {evt.UserName}");

                        foreach (byte data in evt.Data)
                            Console.WriteLine($"\tData: {data}");
                    });

                appEvents.ClearLog();
                appEvents.CloseLog();
                appEvents.DeleteLog();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            // CreateMultipleLogs
            AppEvents AppEventLog = new AppEvents("AppLog", "AppLocal");
            AppEvents GlobalEventLog = new AppEvents("AppSystemLog", "AppGlobal");

            ListDictionary LogList = new ListDictionary();
            LogList.Add(AppEventLog.LogName, AppEventLog);
            LogList.Add(GlobalEventLog.LogName, GlobalEventLog);

            ((AppEvents)LogList[AppEventLog.LogName]).WriteToLog("App startup",
                EventLogEntryType.Information, CategoryType.AppStartUp,
                EventIDType.ExceptionThrown);

            ((AppEvents)LogList[GlobalEventLog.LogName]).WriteToLog("App startup security check",
                EventLogEntryType.Information, CategoryType.AppStartUp,
                EventIDType.BufferOverflowCondition);

            foreach (DictionaryEntry Log in LogList)
            {
                ((AppEvents)Log.Value).WriteToLog("App startup",
                    EventLogEntryType.FailureAudit,
                    CategoryType.AppStartUp, EventIDType.SecurityFailure);
            }

            foreach (DictionaryEntry Log in LogList)
            {
                ((AppEvents)Log.Value).DeleteLog();
            }
            LogList.Clear();

        }



        public enum EventIDType
        {
            NA = 0,
            Read = 1,
            Write = 2,
            ExceptionThrown = 3,
            BufferOverflowCondition = 4,
            SecurityFailure = 5,
            SecurityPotentiallyCompromised = 6
        }

        public enum CategoryType : short
        {
            None = 0,
            WriteToDB = 1,
            ReadFromDB = 2,
            WriteToFile = 3,
            ReadFromFile = 4,
            AppStartUp = 5,
            AppShutDown = 6,
            UserInput = 7
        }

        public class AppEvents
        {
            // If you encounter a SecurityException trying to read the registry (Security log)
            // follow these instructions:
            // 1) Open the Registry Editor (search for regedit or type regedit at the Run prompt)
            // 2) Navigate to the following key:
            // 3) HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security
            // 4) Right click on this entry and select Permissions
            // 5) Add the user you are logged in as and give the user the Read permission

            // If you encounter a SecurityException trying to write to the event log
            // "Requested registry access is not allowed.", then the event source has not been
            // created.  Try re-running the EventLogInstaller for your custom event or for this 
            // sample code, run %WINDOWS%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe AppEventsEventLogInstallerApp.dll"
            // If you just ran it, you may need to wait a bit until Windows catches up and 
            // recognizes the log that was added.

            const string localMachine = ".";
            // Constructors
            public AppEvents(string logName) :
                this(logName, Process.GetCurrentProcess().ProcessName)
            { }

            public AppEvents(string logName, string source) :
                this(logName, source, localMachine)
            { }

            public AppEvents(string logName, string source,
                string machineName = localMachine)
            {
                this.LogName = logName;
                this.SourceName = source;
                this.MachineName = machineName;

                Log = new EventLog(LogName, MachineName, SourceName);
            }


            private EventLog Log { get; set; } = null;

            public string LogName { get; set; }

            public string SourceName { get; set; }

            public string MachineName { get; set; } = localMachine;


            // Methods
            public void WriteToLog(string message, EventLogEntryType type,
                CategoryType category, EventIDType eventID)
            {
                if (Log == null)
                    throw (new ArgumentNullException(nameof(Log),
                        "This Event Log has not been opened or has been closed."));

                EventLogPermission evtPermission =
                    new EventLogPermission(EventLogPermissionAccess.Write, MachineName);
                evtPermission.Demand();

                // If you get a SecurityException here, see the notes at the 
                // top of the class
                Log.WriteEntry(message, type, (int)eventID, (short)category);
            }

            public void WriteToLog(string message, EventLogEntryType type,
                CategoryType category, EventIDType eventID, byte[] rawData)
            {
                if (Log == null)
                    throw (new ArgumentNullException(nameof(Log),
                        "This Event Log has not been opened or has been closed."));

                EventLogPermission evtPermission =
                    new EventLogPermission(EventLogPermissionAccess.Write, MachineName);
                evtPermission.Demand();

                // If you get a SecurityException here, see the notes at the 
                // top of the class
                Log.WriteEntry(message, type, (int)eventID, (short)category, rawData);
            }

            public IEnumerable<EventLogEntry> GetEntries()
            {
                EventLogPermission evtPermission =
                    new EventLogPermission(EventLogPermissionAccess.Administer, MachineName);
                evtPermission.Demand();
                return Log?.Entries.Cast<EventLogEntry>().Where(evt => evt.Source == SourceName);
            }

            public void ClearLog()
            {
                EventLogPermission evtPermission =
                    new EventLogPermission(EventLogPermissionAccess.Administer, MachineName);
                evtPermission.Demand();
                if (!IsNonCustomLog())
                    Log?.Clear();
            }

            public void CloseLog()
            {
                Log?.Close();
                Log = null;
            }

            public void DeleteLog()
            {
                if (!IsNonCustomLog())
                    if (EventLog.Exists(LogName, MachineName))
                        EventLog.Delete(LogName, MachineName);

                CloseLog();
            }

            public bool IsNonCustomLog()
            {
                // Because Application, Setup, Security, System, and other non-custom logs 
                // can contain crucial information  you can't just delete or clear them
                if (LogName == string.Empty || // same as application
                    LogName == "Application" ||
                    LogName == "Security" ||
                    LogName == "Setup" ||
                    LogName == "System")
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region "5.11 Watching the Event Log for a Specific Entry"
        public static void WatchForAppEvent(EventLog log)
        {
            log.EnableRaisingEvents = true;
            log.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);

            log.Source = "APPEVENTSSOURCE"; // See 5.10 for how to establish this event source
            log.WriteEntry("Anyone paying attention?", EventLogEntryType.Information);
            log.WriteEntry("Well maybe you'll see this!", EventLogEntryType.Error);
        }

        public static void OnEntryWritten(object source, EntryWrittenEventArgs entryArg)
        {
            if (entryArg.Entry.EntryType == EventLogEntryType.Error)
            {
                // Do further actions here as necessary
                Console.WriteLine(entryArg.Entry.Category);
                Console.WriteLine(entryArg.Entry.EntryType.ToString());
                Console.WriteLine(entryArg.Entry.Message);
                Console.WriteLine("Entry written");
            }
        }
        #endregion

        #region "5.12 Implementing a Simple Performance Counter"
        public static void TestCreateSimpleCounter()
        {
            PerformanceCounter AppCounter = 
                CreateSimpleCounter("AppCounter", "", 
                    PerformanceCounterType.CounterTimer, "AppCategory", "");
            AppCounter.RawValue = 10;
            for (int i = 0; i <= 10; i++)
            {
                CounterSample CounterSampleValue = AppCounter.NextSample();
                Console.WriteLine($"\r\n--> Sample RawValue  = {CounterSampleValue.RawValue}");

                long value = AppCounter.IncrementBy(i * 2);
                System.Threading.Thread.Sleep(10 * i);

                CounterSample CounterNextSampleValue = AppCounter.NextSample();
                Console.WriteLine($"--> NextValue RawValue = {CounterNextSampleValue.RawValue}");

                Console.WriteLine($"Time delta = {(CounterNextSampleValue.TimeStamp - CounterSampleValue.TimeStamp)}");
                Console.WriteLine($"Time 100ns delta = {(CounterNextSampleValue.TimeStamp100nSec - CounterSampleValue.TimeStamp100nSec)}");
                Console.WriteLine($"CounterTimeStamp delta = {(CounterNextSampleValue.CounterTimeStamp - CounterSampleValue.CounterTimeStamp)}");

                float sample1 = CounterSample.Calculate(CounterSampleValue, CounterNextSampleValue);
                Console.WriteLine($"--> Calculated Sample1 = {sample1}");
            }
        }

        public static PerformanceCounter CreateSimpleCounter(string counterName, string counterHelp,
            PerformanceCounterType counterType, string categoryName, string categoryHelp)
        {
            CounterCreationDataCollection counterCollection =
                new CounterCreationDataCollection();

            // Create the custom counter object and add it to the collection of counters
            CounterCreationData counter = 
                new CounterCreationData(counterName, counterHelp, counterType);
            counterCollection.Add(counter);

            // Create category
            if (PerformanceCounterCategory.Exists(categoryName))
                PerformanceCounterCategory.Delete(categoryName);

            PerformanceCounterCategory appCategory =
                PerformanceCounterCategory.Create(categoryName, categoryHelp,
                    PerformanceCounterCategoryType.SingleInstance, counterCollection);

            // Create the counter and initialize it
            PerformanceCounter appCounter =
                new PerformanceCounter(categoryName, counterName, false);

            appCounter.RawValue = 0;

            return (appCounter);
        }
        #endregion

        #region "5.13 Create custom debugging displays for your classes"
        [DebuggerDisplay("Citizen Full Name = {Honorific}{First}{Middle}{Last}")]
        public class Citizen
        {
            public string Honorific { get; set; }
            public string First { get; set; }
            public string Middle { get; set; }
            public string Last { get; set; }
        }
        public static void TestCustomDebuggerDisplay()
        {
            Citizen mrsJones = new Citizen()
            {
                Honorific = "Mrs.",
                First = "Alice",
                Middle = "G.",
                Last = "Jones"
            };
            Citizen mrJones = new Citizen()
            {
                Honorific = "Mr.",
                First = "Robert",
                Middle = "Frederick",
                Last = "Jones"
            };
        }
        #endregion

        #region "5.14 Tracking where exceptions come from"
        public void TestCallerInfoAttribs()
        {
            try
            {
                LibraryMethod();
            }
            catch(Exception ex)
            {
                RecordCatchBlock(ex);
                Console.WriteLine("");
                Console.WriteLine(ex.ToString());
            }
        }

        public void LibraryMethod(
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string sourceFilePath = "",
                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                // Do some library action
                // had a problem
                throw new NullReferenceException();
            }
            catch(Exception ex)
            {
                // Wrap the exception and capture the source of where the
                // library method was called from
                throw new LibraryException(ex)
                {
                    CallerMemberName = memberName,
                    CallerFilePath = sourceFilePath,
                    CallerLineNumber = sourceLineNumber
                };
            }
        }

        public void RecordCatchBlock(Exception ex,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            string catchDetails =
                $"{ex.GetType().Name} caught in member \"{memberName}\" " +
                $"in catch block encompassing line {sourceLineNumber} " +
                $"in file {sourceFilePath} " +
                $"with message \"{ex.Message}\"";
            Console.WriteLine(catchDetails);
        }

        [Serializable]
        public class LibraryException : Exception
        {
            public LibraryException(Exception inner) : base(inner.Message,inner)
            {
            }
            public string CallerMemberName { get; set; }
            public string CallerFilePath { get; set; }
            public int CallerLineNumber { get; set; }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("CallerMemberName", this.CallerMemberName);
                info.AddValue("CallerFilePath", this.CallerFilePath);
                info.AddValue("CallerLineNumber", this.CallerLineNumber);
            }
            public override string ToString() => "LibraryException originated in " +
                $"member \"{CallerMemberName}\" " +
                $"on line {CallerLineNumber} " +
                $"in file {CallerFilePath} " +
                $"with exception details: {Environment.NewLine}" +
                $"{InnerException.ToString()}";
        }
        #endregion

        #region "5.15 Handling Exceptions in Asynchronous Scenarios"
        public async Task TestHandlingAsyncExceptionsAsync()
        {
            // Team producing software
            // Manager sends Steve to create code, exception"DefectCreatedException" thrown
            // Manager sends Jay, Tom, Seth to write code, all throw DefectCreatedExceptions

            // Single async method call
            try
            {
                // Steve, get that project done!
                await SteveCreateSomeCodeAsync();
            }
            catch (DefectCreatedException dce)
            {
                Console.WriteLine($"Steve introduced a Defect: {dce.Message}");
            }

            // Multiple async methods (WaitAll)

            // OK Team, make that new thing this weekend! You guys better hurry up with that!
            Task jayCode = JayCreateSomeCodeAsync();
            Task tomCode = TomCreateSomeCodeAsync();
            Task sethCode = SethCreateSomeCodeAsync();

            Task teamComplete = Task.WhenAll(new Task[] { jayCode, tomCode, sethCode });
            try
            {
                await teamComplete;
            }
            catch
            {
                // Get the messages from the exceptions thrown from 
                // the set of actions
                var defectMessages = 
                    teamComplete.Exception?.InnerExceptions.Select(e => 
                        e.Message).ToList();
                defectMessages?.ForEach(m => 
                    Console.WriteLine($"{m}"));
            }

            // Background context resumption (ConfigureAwait) in Discussion
            // Speak to this in the discussion, in UI /ASP.NET|WebApi if you want to include details 
            // about the request in your logging, DONT use ConfigureAwait on the actions as the returned
            // exceptions will not be on the thread with the "context" and you can't access them (like URI/Action/UI Context)

            // awaiting an action in an exception handler
            // discuss how the original throw location is preserved via 
            // System.Runtime.ExceptionServices.ExceptionDispatchInfo
            try
            {
                try
                {
                    await SteveCreateSomeCodeAsync();
                }
                catch (DefectCreatedException dce)
                {
                    Console.WriteLine(dce.ToString());
                    await WriteEventLogEntryAsync("ManagerApplication", dce.Message, EventLogEntryType.Error);
                    throw;
                }
            }
            catch(DefectCreatedException dce)
            {
                Console.WriteLine(dce.ToString());
            }
        }

        public async Task WriteEventLogEntryAsync(string source, string message, EventLogEntryType type)
        {
            await Task.Factory.StartNew(() => EventLog.WriteEntry(source, message, type));
        }

        public async Task SteveCreateSomeCodeAsync()
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(100, 1000));
            throw new DefectCreatedException("Null Reference",42);
        }

        public async Task JayCreateSomeCodeAsync()
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(100, 1000));
            throw new DefectCreatedException("Ambiguous Match",2);
        }

        public async Task TomCreateSomeCodeAsync()
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(100, 1000));
            throw new DefectCreatedException("Quota Exceeded",11);
        }
        public async Task SethCreateSomeCodeAsync()
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(100, 1000));
            throw new DefectCreatedException("Out Of Memory", 8);
        }

        [Serializable]
        public class DefectCreatedException : Exception
        {
            #region Constructors
            // Normal exception ctor's
            public DefectCreatedException() : base()
	        {
            }

            public DefectCreatedException(string message) : base(message)
	        {
            }

            public DefectCreatedException(string message, Exception innerException) 
		        : base(message, innerException)
	        {
            }

            // Exception ctor's that accept the new parameters
            public DefectCreatedException(string defect, int line) : base(string.Empty)
	        {
                this.Defect = defect;
                this.Line = line;
            }

            public DefectCreatedException(string defect, int line, Exception innerException) 
		        : base(string.Empty, innerException)
	        {
                this.Defect = defect;
                this.Line = line;
            }


            // Serialization ctor
            protected DefectCreatedException(SerializationInfo exceptionInfo,
                StreamingContext exceptionContext)
                : base(exceptionInfo, exceptionContext)
            {
            }
            #endregion // Constructors

            #region Properties
            public string Defect { get; }
            public int Line { get; }

            public override string Message => 
                $"A defect was introduced: ({this.Defect ?? "Unknown"} on line {this.Line})";
            #endregion // Properties

            #region Overridden methods
            // ToString method
            public override string ToString() => 
                $"{Environment.NewLine}{this.ToFullDisplayString()}";

            // Used during serialization to capture information about extra fields
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info,
                StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("Defect", this.Defect);
                info.AddValue("Line", this.Line);
            }
            #endregion // Overridden methods

            // Call base.ToString method
            public string ToBaseString() => (base.ToString());
        }
        #endregion

        #region "5.16 Being selective about exception processing"
        public void TestExceptionFilters()
        {
            Console.WriteLine("Simulating database call timeout");
            try
            {
                ProtectedCallTheDatabase("timeout");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception catch caught a database exception: {ex.Message}");
            }
            Console.WriteLine("");

            Console.WriteLine("Simulating database call login failure");
            try
            {
                ProtectedCallTheDatabase("loginfail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception catch caught a database exception: {ex.Message}");
            }
            Console.WriteLine("");

            Console.WriteLine("Simulating successful database call");
            try
            {
                ProtectedCallTheDatabase("noerror");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception catch caught a database exception: {ex.Message}");
            }
            Console.WriteLine("");
        }

        private void ProtectedCallTheDatabase(string problem)
        {
            try
            {
                CallTheDatabase(problem);
                Console.WriteLine("No error on database call");
            }
            catch (DatabaseException dex) when (dex.Number == -2) // watch for timeouts
            {
                Console.WriteLine($"DatabaseException catch caught a database exception: {dex.Message}");
            }
       }

        private void CallTheDatabase(string problem)
        {
            switch (problem)
            {
                case "timeout":
                    throw new DatabaseException(
                        "Timeout expired. The timeout period elapsed prior to " +
                        "completion of the operation or the server is not " +
                        "responding. (Microsoft SQL Server, Error: -2).")
                    {
                        Number = -2,
                        Class = 11
                    };
                case "loginfail":
                    throw new DatabaseException("Login failed for user")
                    {
                        Number = 18456,
                    };
            }
        }
        /// <summary>
        /// Imagine if you will, a database exception that is not allowed to be 
        /// constructed, thrown or used by anyone but it's maker....
        /// Yes I'm looking at you SqlException...
        /// </summary>
        [Serializable]
        public class DatabaseException : DbException
        {
            public DatabaseException(string message) : base(message) { }
            public byte Class { get; set; }
            public Guid ClientConnectionId { get; set; }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public SqlErrorCollection Errors { get; set; }
            public int LineNumber { get; set; }
            public int Number { get; set; }
            public string Procedure { get; set; }
            public string Server { get; set; }
            public override string Source => base.Source;
            public byte State { get; set; }
            public override void GetObjectData(SerializationInfo si, StreamingContext context)
            {
                base.GetObjectData(si, context);
            }
        }
        #endregion
    }
}
