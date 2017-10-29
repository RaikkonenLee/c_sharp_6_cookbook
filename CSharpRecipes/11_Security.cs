using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using System.Configuration;
using Microsoft.Win32;
using System.Reflection;
using System.Linq;

namespace CSharpRecipes
{
    public class Security
	{
		#region "11.1 Encrypting/Decrypting a String"	
		public static void EncDecString()
		{
			string encryptedString = CryptoString.Encrypt("MyPassword");
			Console.WriteLine($"encryptedString: {encryptedString}");
			// get the key and IV used so you can decrypt it later
			byte[] key = CryptoString.Key;
			byte[] IV = CryptoString.IV;

			CryptoString.Key = key;
			CryptoString.IV = IV;
			string decryptedString = CryptoString.Decrypt(encryptedString);
			Console.WriteLine($"decryptedString: {decryptedString}");
		}

		public sealed class CryptoString
		{
			private CryptoString() { }

			private static byte[] savedKey = null;
			private static byte[] savedIV = null;

			public static byte[] Key
			{
				get;
				set;
			}

			public static byte[] IV
			{
				get;
				set;
			}

			private static void RdGenerateSecretKey(RijndaelManaged rdProvider)
			{
				if (savedKey == null)
				{
					rdProvider.KeySize = 256;
					rdProvider.GenerateKey();
					savedKey = rdProvider.Key;
				}
			}

			private static void RdGenerateSecretInitVector(RijndaelManaged rdProvider)
			{
				if (savedIV == null)
				{
					rdProvider.GenerateIV();
					savedIV = rdProvider.IV;
				}
			}

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
            public static string Encrypt(string originalStr)
            {
                // Encode data string to be stored in memory
                byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
                byte[] originalBytes = { };

                // Create MemoryStream to contain output
                using (MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length))
                {
                    using (RijndaelManaged rijndael = new RijndaelManaged())
                    {
                        // Generate and save secret key and init vector
                        RdGenerateSecretKey(rijndael);
                        RdGenerateSecretInitVector(rijndael);

                        if (savedKey == null || savedIV == null)
                        {
                            throw (new NullReferenceException(
                                "savedKey and savedIV must be non-null."));
                        }

                        // Create encryptor, and stream objects
                        using (ICryptoTransform rdTransform =
                            rijndael.CreateEncryptor((byte[])savedKey.Clone(),
                                                    (byte[])savedIV.Clone()))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform,
                                CryptoStreamMode.Write))
                            {
                                // Write encrypted data to the MemoryStream
                                cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                originalBytes = memStream.ToArray();
                            }
                        }
                    }
                }

				// Convert encrypted string
				string encryptedStr = Convert.ToBase64String(originalBytes);
				return (encryptedStr);
			}

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
            public static string Decrypt(string encryptedStr)
			{
				// Unconvert encrypted string
				byte[] encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
				byte[] initialText = new Byte[encryptedStrAsBytes.Length];

                using (RijndaelManaged rijndael = new RijndaelManaged())
                {
                    using (MemoryStream memStream = new MemoryStream(encryptedStrAsBytes))
                    {
                        if (savedKey == null || savedIV == null)
                        {
                            throw (new NullReferenceException(
                                "savedKey and savedIV must be non-null."));
                        }

                        // Create decryptor, and stream objects
                        using (ICryptoTransform rdTransform =
                               rijndael.CreateDecryptor((byte[])savedKey.Clone(),
                                                        (byte[])savedIV.Clone()))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform,
                                   CryptoStreamMode.Read))
                            {
                                // Read in decrypted string as a byte[]
                                cryptoStream.Read(initialText, 0, initialText.Length);
                            }
                        }
                    }
                }

				// Convert byte[] to string
				string decryptedStr = Encoding.ASCII.GetString(initialText);
				return (decryptedStr);
			}
		}

		#endregion

		#region "11.2 Encrypting and Decrypting a File"	
		public static void EncDecFile()
		{
            string encrypt = "My TDES Secret Data!";

            // Use TripleDES
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                SecretFile secretTDESFile = new SecretFile(tdes, "tdestext.secret");

                Console.WriteLine($"Writing secret data: {encrypt}");
                secretTDESFile.SaveSensitiveData(encrypt);
                // save for storage to read file
                byte[] key = secretTDESFile.Key;
                byte[] IV = secretTDESFile.IV;

                string decrypt = secretTDESFile.ReadSensitiveData();
                Console.WriteLine($"Read secret data: {decrypt}");
            }

            // Use Rijndael
            using (RijndaelManaged rdProvider = new RijndaelManaged())
            {
                SecretFile secretRDFile = new SecretFile(rdProvider, "rdtext.secret");

                encrypt = "My Rijndael Secret Data!";

                Console.WriteLine($"Writing secret data: {encrypt}");
                secretRDFile.SaveSensitiveData(encrypt);
                // save for storage to read file
                byte[] key = secretRDFile.Key;
                byte[] IV = secretRDFile.IV;

                string decrypt = secretRDFile.ReadSensitiveData();
                Console.WriteLine($"Read secret data: {decrypt}");
            }
        }

		public class SecretFile
		{
			private byte[] savedKey = null;
			private byte[] savedIV = null;
			private SymmetricAlgorithm symmetricAlgorithm;
			string path;

			public byte[] Key
			{
				get;
				set;
			}

			public byte[] IV
			{
				get;
				set;
			}

			public SecretFile(SymmetricAlgorithm algorithm, string fileName)
			{
				symmetricAlgorithm = algorithm;
				path = fileName;
			}

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
            public void SaveSensitiveData(string sensitiveData)
			{
				// Encode data string to be stored in encrypted file
				byte[] encodedData = Encoding.Unicode.GetBytes(sensitiveData);

                // Create FileStream and crypto service provider objects
                using (FileStream fileStream = new FileStream(path,
                       FileMode.Create,
                       FileAccess.Write))
                {
                    // Generate and save secret key and init vector
                    GenerateSecretKey();
                    GenerateSecretInitVector();

                    // Create crypto transform and stream objects
                    using (ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor(savedKey,
                           savedIV))
                    {
                        using (CryptoStream cryptoStream =
                               new CryptoStream(fileStream, transform, CryptoStreamMode.Write))
                        {
                            // Write encrypted data to the file 
                            cryptoStream.Write(encodedData, 0, encodedData.Length);
                        }
                    }
                }
			}

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
            public string ReadSensitiveData()
			{
                string decrypted = null;

                // Create file stream to read encrypted file back
                using (FileStream fileStream = new FileStream(path,
                       FileMode.Open,
                       FileAccess.Read))
                {
                    //print out the contents of the encrypted file
                    using (BinaryReader binReader = new BinaryReader(fileStream))
                    {
                        Console.WriteLine("---------- Encrypted Data ---------");
                        int count = (Convert.ToInt32(binReader.BaseStream.Length));
                        byte[] bytes = binReader.ReadBytes(count);
                        char[] array = Encoding.Unicode.GetChars(bytes);
                        string encdata = new string(array);
                        Console.WriteLine(encdata);
                        Console.WriteLine($"---------- Encrypted Data ---------{Environment.NewLine}");

                        // reset the file stream
                        fileStream.Seek(0, SeekOrigin.Begin);

                        // Create Decryptor
                        using (ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(savedKey, savedIV))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(fileStream, transform, CryptoStreamMode.Read))
                            {
                                //print out the contents of the decrypted file
                                using (StreamReader srDecrypted = new StreamReader(cryptoStream, new UnicodeEncoding()))
                                {
                                    Console.WriteLine("---------- Decrypted Data ---------");
                                    decrypted = srDecrypted.ReadToEnd();
                                    Console.WriteLine(decrypted);
                                    Console.WriteLine($"---------- Decrypted Data ---------{Environment.NewLine}");
                                }
                            }
                        }
                    }
                }

                return decrypted;
			}

			private void GenerateSecretKey()
			{
				if (null != (symmetricAlgorithm as TripleDESCryptoServiceProvider))
				{
					TripleDESCryptoServiceProvider tdes;
					tdes = symmetricAlgorithm as TripleDESCryptoServiceProvider;
					tdes.KeySize = 192;	//  Maximum key size
					tdes.GenerateKey();
					savedKey = tdes.Key;
				}
				else if (null != (symmetricAlgorithm as RijndaelManaged))
				{
					RijndaelManaged rdProvider;
					rdProvider = symmetricAlgorithm as RijndaelManaged;
					rdProvider.KeySize = 256; // Maximum key size
					rdProvider.GenerateKey();
					savedKey = rdProvider.Key;
				}
			}

			private void GenerateSecretInitVector()
			{
				if (null != (symmetricAlgorithm as TripleDESCryptoServiceProvider))
				{
					TripleDESCryptoServiceProvider tdes;
					tdes = symmetricAlgorithm as TripleDESCryptoServiceProvider;
					tdes.GenerateIV();
					savedIV = tdes.IV;
				}
				else if (null != (symmetricAlgorithm as RijndaelManaged))
				{
					RijndaelManaged rdProvider;
					rdProvider = symmetricAlgorithm as RijndaelManaged;
					rdProvider.GenerateIV();
					savedIV = rdProvider.IV;
				}
			}
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]

        #endregion

        #region "11.3 Cleaning Up Cryptography Information"	
        public static void CleanUpCrypto()
		{
			string originalStr = "SuperSecret information";
			// Encode data string to be stored in memory
			byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
			byte[] originalBytes = { };

			// Create MemoryStream to contain output
			MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length);

			RijndaelManaged rijndael = new RijndaelManaged();

			// Generate secret key and init vector
			rijndael.KeySize = 256;
			rijndael.GenerateKey();
			rijndael.GenerateIV();

			// save off the key and IV for later decryption
			byte[] key = rijndael.Key;
			byte[] IV = rijndael.IV;

			// Create encryptor, and stream objects
			ICryptoTransform transform = rijndael.CreateEncryptor(rijndael.Key,
				rijndael.IV);
			CryptoStream cryptoStream = new CryptoStream(memStream, transform,
				CryptoStreamMode.Write);

			// Write encrypted data to the MemoryStream
			cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
			cryptoStream.FlushFinalBlock();

			// Release all resources as soon as we are done with them
			// to prevent retaining any information in memory
			memStream.Close();
			memStream = null;
			cryptoStream.Close();
			cryptoStream = null;
			transform.Dispose();
			transform = null;
			// this clear statement regens both the key and the init vector so that
			// what is left in memory is no longer the values you used to encrypt with
			rijndael.Clear();
			// make this eligible for GC as soon as possible
			rijndael = null;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void CleanUpCryptoWithUsing()
        {
            string originalStr = "SuperSecret information";
            // Encode data string to be stored in memory.
            byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
            byte[] originalBytes = { };

            // Create MemoryStream to contain output.
            using (MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length))
            {
                using (RijndaelManaged rijndael = new RijndaelManaged())
                {
                    // Generate secret key and init vector.
                    rijndael.KeySize = 256;
                    rijndael.GenerateKey();
                    rijndael.GenerateIV();

                    // Save off the key and IV for later decryption.
                    byte[] key = rijndael.Key;
                    byte[] IV = rijndael.IV;

                    // Create encryptor and stream objects.
                    using (ICryptoTransform transform =
                            rijndael.CreateEncryptor(rijndael.Key, rijndael.IV))
                    {
                        using (CryptoStream cryptoStream = new
                               CryptoStream(memStream, transform,
                                CryptoStreamMode.Write))
                        {
                            // Write encrypted data to the MemoryStream.
                            cryptoStream.Write(originalStrAsBytes, 0,
                                        originalStrAsBytes.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                    }
                }
            }
        }

        #endregion

        #region "11.4 Verifying that a String Remains Uncorrupted Following Transmission"	
        public static void VerifyStringIntegrity()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("TESTING:  VerifyStringIntegrity()");

            string originalString = "This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing. This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.This is the string that we'll be testing.";

            // Create a hash value from the original string value we need to protect and sign the hash value
            string rsaPublicKey;
            byte[] signature = SendData(originalString, out rsaPublicKey);

            //      Uncomment the code below to quickly test handling a tampered string:
            //          originalString += "a";
            //      Uncomment the code below to quickly test handling a tampered signature:
            //          signature[1] = 100;


            // Now, verify that the string has not been corrupted, nor tampered with
            if (ReceiveData(originalString, signature, rsaPublicKey))
            {
                Console.WriteLine("The original string was NOT corrupted or tampered with.");
            }
            else
            {
                Console.WriteLine("ALERT:  The original string was corrupted and/or tampered with.");
            }
        }

        private static byte[] SendData(string originalString, out string rsaPublicKey)
        {
            // Digitally sign the string data
            byte[] signature = AntiTamper.SignString(originalString, out rsaPublicKey);

            // Send the data and the signature to its destination...

            return signature;
        }

        #pragma warning restore CSE0003 // Consider using an expression-bodied member
        private static bool ReceiveData(string originalString, byte[] signature, string rsaPublicKey)
        {
            // Receive the data and signature from the sender...

            // Verify the digital signature
            return (AntiTamper.VerifySignature(originalString, signature, rsaPublicKey));
        }


        public class AntiTamper
        {
            static private readonly int RSA_KEY_SIZE = 2048;


            public static byte[] SignString(string clearText, out string rsaPublicKey)
            {
                byte[] signature = null;
                rsaPublicKey = null;

                byte[] encodedClearText = Encoding.Unicode.GetBytes(clearText);

                using (SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider())
                {
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
                    {
                        signature = rsa.SignData(encodedClearText, sha512);

                        //rsaPublicParams = rsa.ExportParameters(false);
                        rsaPublicKey = rsa.ToXmlString(false);
                    }
                }

                return signature;
            }

            public static bool VerifySignature(string clearText, byte[] signature, string rsaPublicKey)
            {
                bool verified = false;
                byte[] encodedClearText = Encoding.Unicode.GetBytes(clearText);

                using (SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider())
                {
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
                    {
                        //rsa.ImportParameters(rsaPublicParams);
                        rsa.FromXmlString(rsaPublicKey);

                        verified = rsa.VerifyData(encodedClearText, sha512, signature);
                    }
                }

                return verified;
            }
        }

        #region Deprecated:  Integrity check without tampering check
        public static void VerifyStringIntegrityWithoutTamperProtection()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("TESTING:  VerifyStringIntegrity()");

            string testString = "This is the string that we'll be testing.";
            string unhashedString;
            string hashedString = HashOps.CreateStringHash(testString);

            bool result = HashOps.IsStringCorrupted(hashedString, out unhashedString);
            Console.WriteLine(result);
            if (!result)
                Console.WriteLine($"The string sent is: {unhashedString}");
            else
                Console.WriteLine($"The string {unhashedString} has become corrupted.");
        }

        public class HashOps
        {
            public static string CreateStringHash(string unHashedString)
            {
                byte[] encodedUnHashedString = Encoding.Unicode.GetBytes(unHashedString);

                SHA256Managed hashingObj = new SHA256Managed();
                byte[] hashCode = hashingObj.ComputeHash(encodedUnHashedString);

                string hashBase64 = Convert.ToBase64String(hashCode);
                string stringWithHash = unHashedString + hashBase64;

                hashingObj.Clear();

                return (stringWithHash);
            }

            public static bool IsStringCorrupted(string stringWithHash,
                out string originalStr)
            {
                // Code to quickly test the handling of a tampered string
                //stringWithHash = stringWithHash.Replace('a', 'b');

                if (stringWithHash.Length < 45)
                {
                    originalStr = null;
                    return (true);
                }

                string hashCodeString =
                    stringWithHash.Substring(stringWithHash.Length - 44);
                string unHashedString =
                    stringWithHash.Substring(0, stringWithHash.Length - 44);

                byte[] hashCode = Convert.FromBase64String(hashCodeString);

                byte[] encodedUnHashedString = Encoding.Unicode.GetBytes(unHashedString);

                SHA256Managed hashingObj = new SHA256Managed();
                byte[] receivedHashCode = hashingObj.ComputeHash(encodedUnHashedString);

                bool hasBeenTamperedWith = false;
                for (int counter = 0; counter < receivedHashCode.Length; counter++)
                {
                    if (receivedHashCode[counter] != hashCode[counter])
                    {
                        hasBeenTamperedWith = true;
                        break;
                    }
                }

                if (!hasBeenTamperedWith)
                {
                    originalStr = unHashedString;
                }
                else
                {
                    originalStr = null;
                }

                hashingObj.Clear();

                return (hasBeenTamperedWith);
            }
        }
        #endregion
        #endregion

        #region "11.5 Making a Security Assert Safe"	
        public static void SafeAssert()
		{
			CallSecureFunctionSafelyAndEfficiently();
		}

		public static void CallSecureFunctionSafelyAndEfficiently()
		{

			// set up a permission to be able to access non-public members 
			// via reflection
			ReflectionPermission perm =
				new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

			// Demand the permission set we have compiled before using Assert
			// to make sure we have the right before we Assert it.  We do
			// the Demand to insure that we have checked for this permission
			// before using Assert to short-circuit stackwalking for it which
			// helps us stay secure, while performing better.
			perm.Demand();

			// Assert this right before calling into the function that
			// would also perform the Demand to short-circuit the stack walk
			// each call would generate.  The Assert helps us to optimize
			// out use of SecureFunction
			perm.Assert();

			// we call the secure function 100 times but only generate
			// the stackwalk from the function to this calling function
			// instead of walking the whole stack 100 times.
			for (int i = 0; i < 100; i++)
			{
				SecureFunction();
			}
		}

		public static void SecureFunction()
		{
			// set up a permission to be able to access non-public members 
			// via reflection
			ReflectionPermission perm =
				new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

			// Demand the right to do this and cause a stack walk
			perm.Demand();

			// Perform the action here...
		}

		#endregion

		#region "11.6 Verifying if an Assembly has been Granted Specific Permissions"	
		public static void VerifyAssemblyPerms()
		{
			// This would set up a Regex for the O’Reilly web site then use it to create a 
			//   WebPermission for connecting to that site and all sites containing 
			//   the www.oreilly.com string.  We would then check the WebPermission against 
			//   the SecurityManager to see if we have the permission to do this.
			Regex regex = new Regex(@"http://www\.oreilly\.com/.*");
			WebPermission webConnectPerm = new WebPermission(NetworkAccess.Connect, regex);

			// Deprecated way of checking permissions:
			//if (SecurityManager.IsGranted(webConnectPerm))
			//{
			//    // connect to the oreilly site
			//}

			// New way of checking
			PermissionSet pSet = new PermissionSet(PermissionState.None);
			pSet.AddPermission(webConnectPerm);
			bool isGranted = pSet.IsSubsetOf(Assembly.GetExecutingAssembly().PermissionSet);	  // can also use:   AppDomain.CurrentDomain.PermissionSet   if you want to check only the current appdomain
			Console.WriteLine($"\r\n\r\nPermissions in first permission set?  {isGranted.ToString()}\r\n\r\n");
		}
		#endregion

		#region "11.7 Minimizing the Attack Surface of an Assembly"	
		public static void MinimizeAttackSurface()
		{
			Console.WriteLine("See the text about how to implement SecurityAction.RequestRefuse");
		}
		#endregion

		#region "11.8 Obtaining Security/Audit Information"
		public static void TestViewFileRegRights()
		{
			// Get security information from a file
			string file = @"C:\Windows\win.ini";
			FileSecurity fileSec = File.GetAccessControl(file);
			DisplayFileSecurityInfo(fileSec);

			// Get security information from a registry key
			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\14.0");
			RegistrySecurity regSecurity = regKey.GetAccessControl();
			DisplayRegKeySecurityInfo(regSecurity);
		}


		public static void DisplayFileSecurityInfo(FileSecurity fileSec)
		{
			Console.WriteLine($"GetSecurityDescriptorSddlForm:  {fileSec.GetSecurityDescriptorSddlForm(AccessControlSections.All)}");

			foreach (FileSystemAccessRule ace in fileSec.GetAccessRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine($"\tIdentityReference.Value: {ace.IdentityReference.Value}");
				Console.WriteLine($"\tAccessControlType: {ace.AccessControlType}");
				Console.WriteLine($"\tFileSystemRights: {ace.FileSystemRights}");
				Console.WriteLine($"\tInheritanceFlags: {ace.InheritanceFlags}");
				Console.WriteLine($"\tIsInherited: {ace.IsInherited}");
				Console.WriteLine($"\tPropagationFlags: {ace.PropagationFlags}");

				Console.WriteLine("-----------------\r\n\r\n");
			}


			foreach (FileSystemAuditRule ace in fileSec.GetAuditRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine($"\tIdentityReference.Value: {ace.IdentityReference.Value}");
				Console.WriteLine($"\tAuditFlags: {ace.AuditFlags}");
				Console.WriteLine($"\tFileSystemRights: {ace.FileSystemRights}");
				Console.WriteLine($"\tInheritanceFlags: {ace.InheritanceFlags}");
				Console.WriteLine($"\tIsInherited: {ace.IsInherited}");
				Console.WriteLine($"\tPropagationFlags: {ace.PropagationFlags}");

				Console.WriteLine("-----------------\r\n\r\n");
			}

			Console.WriteLine($"GetGroup(typeof(NTAccount)).Value: {fileSec.GetGroup(typeof(NTAccount)).Value}");
			Console.WriteLine($"GetOwner(typeof(NTAccount)).Value: {fileSec.GetOwner(typeof(NTAccount)).Value}");

			Console.WriteLine("---------------------------------------\r\n\r\n\r\n");
		}



		public static void DisplayRegKeySecurityInfo(RegistrySecurity regSec)
		{
			Console.WriteLine($"GetSecurityDescriptorSddlForm:  {regSec.GetSecurityDescriptorSddlForm(AccessControlSections.All)}");

			foreach (RegistryAccessRule ace in regSec.GetAccessRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine($"\tIdentityReference.Value: {ace.IdentityReference.Value}");
				Console.WriteLine($"\tAccessControlType: {ace.AccessControlType}");
				Console.WriteLine($"\tRegistryRights: {ace.RegistryRights.ToString()}");
				Console.WriteLine($"\tInheritanceFlags: {ace.InheritanceFlags}");
				Console.WriteLine($"\tIsInherited: {ace.IsInherited}");
				Console.WriteLine($"\tPropagationFlags: {ace.PropagationFlags}");

				//NTAccount name = (NTAccount)ace.IdentityReference;
				//Console.WriteLine("\tname: \{name);
				// Change NTAccount to SecurityIdentifier to get the non-human readable version of the SID

				Console.WriteLine("-----------------\r\n\r\n");
			}


			foreach (RegistryAuditRule ace in regSec.GetAuditRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine($"\tIdentityReference.Value: {ace.IdentityReference.Value}");
				Console.WriteLine($"\tAuditFlags: {ace.AuditFlags}");
				Console.WriteLine($"\tRegistryRights: {ace.RegistryRights.ToString()}");
				Console.WriteLine($"\tInheritanceFlags: {ace.InheritanceFlags}");
				Console.WriteLine($"\tIsInherited: {ace.IsInherited}");
				Console.WriteLine($"\tPropagationFlags: {ace.PropagationFlags}");

				Console.WriteLine("-----------------\r\n\r\n");
			}

			Console.WriteLine($"GetGroup(typeof(NTAccount)).Value: {regSec.GetGroup(typeof(NTAccount)).Value}");
			Console.WriteLine($"GetOwner(typeof(NTAccount)).Value: {regSec.GetOwner(typeof(NTAccount)).Value}");

			Console.WriteLine("---------------------------------------\r\n\r\n\r\n");
		}
		#endregion

		#region "11.9 Granting/Revoking Access to a File or Registry Key"
		public static void TestGrantRevokeFileRights()
		{
            try
            {
                NTAccount user = new NTAccount(@"BUILTIN\Users");     // BUILTIN\Administrators

                string file = @"..\..\FOO.TXT";
                GrantFileRights(file, user, FileSystemRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);
                RevokeFileRights(file, user, FileSystemRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);

                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MyCompany\MyApp"))
                {
                    GrantRegKeyRights(regKey, user, RegistryRights.Notify, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny);
                    RevokeRegKeyRights(regKey, user, RegistryRights.Notify, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("You will need to have the FOO.TXT file and add the HKLM\\SOFTWARE\\MyCompany\\MyApp registry key if it does not already exist to run the test: " + ex.Message);
            }
		}



		public static void GrantRegKeyRights(RegistryKey regKey, NTAccount user, RegistryRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			RegistrySecurity regSecurity = regKey.GetAccessControl();

			DisplayRegKeySecurityInfo(regSecurity);

			RegistryAccessRule rule = new RegistryAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			regSecurity.AddAccessRule(rule);
			regKey.SetAccessControl(regSecurity);

			DisplayRegKeySecurityInfo(regSecurity);
		}

		public static void RevokeRegKeyRights(RegistryKey regKey, NTAccount user, RegistryRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			RegistrySecurity regSecurity = regKey.GetAccessControl();

			DisplayRegKeySecurityInfo(regSecurity);

			RegistryAccessRule rule = new RegistryAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			regSecurity.AddAccessRule(rule);

			//System.Security.Principal.IdentityNotMappedException was unhandled
			//      Message="Some or all identity references could not be translated."
			//--Only when account is DISABLED

			regKey.SetAccessControl(regSecurity);

			DisplayRegKeySecurityInfo(regSecurity);
		}




		public static void GrantFileRights(string file, NTAccount user, FileSystemRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			FileSecurity fileSecurity = File.GetAccessControl(file);

			DisplayFileSecurityInfo(fileSecurity);

			FileSystemAccessRule rule = new FileSystemAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			fileSecurity.AddAccessRule(rule);
			File.SetAccessControl(file, fileSecurity);

			DisplayFileSecurityInfo(fileSecurity);
		}

		public static void RevokeFileRights(string file, NTAccount user, FileSystemRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			FileSecurity fileSecurity = File.GetAccessControl(file);

			DisplayFileSecurityInfo(fileSecurity);

			FileSystemAccessRule rule = new FileSystemAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			fileSecurity.RemoveAccessRuleSpecific(rule);
			File.SetAccessControl(file, fileSecurity);

			DisplayFileSecurityInfo(fileSecurity);
		}
		#endregion

		#region "11.10 Protecting String Data with Secure Strings"
		public static void TestSecureString()
		{
			StreamReader sr = new StreamReader(@"..\..\data.txt");
			using (SecureString secretStr = CreateSecureString(sr))
			{
				string nonSecureStr = ReadSecureString(secretStr);

				Console.WriteLine($"secretStr = {secretStr.ToString()}");
				Console.WriteLine($"nonSecureStr = {nonSecureStr}");
			}
		}

		public static string ReadSecureString(SecureString secretStr)
		{
			// Cannot modify this string:  secretStr.AppendChar('x');

			// In order to read back the string you need to use some special methods
			IntPtr secretStrPtr = Marshal.SecureStringToBSTR(secretStr);
			string nonSecureStr = Marshal.PtrToStringBSTR(secretStrPtr);

			Marshal.ZeroFreeBSTR(secretStrPtr);

			// Can't clear a read-only SecureString obj
			if (!secretStr.IsReadOnly())
				secretStr.Clear();

			return nonSecureStr;
		}

		public static SecureString CreateSecureString(StreamReader secretStream)
		{
			SecureString secretStr = new SecureString();
			char[] buf = new char[1];

			while (secretStream.Peek() >= 0)
			{
				int nextByte = secretStream.Read(buf, 0, 1);
				secretStr.AppendChar(buf[0]);
			}

			// Make the secretStr object read-only
			secretStr.MakeReadOnly();

			return (secretStr);
		}

		public static SecureString CreateSecureString(string secret)
		{
			SecureString secretStr = new SecureString();
			char[] buf = new char[1];

			foreach (char c in secret)
			{
				secretStr.AppendChar(c);
			}

			// Make the secretStr object read-only
			secretStr.MakeReadOnly();

			return (secretStr);
		}
		#endregion

		#region "11.11 Securing Stream Data (see AuthenticatedStream, NegotiateStream, and SslStream)"
		//      http://www.inventec.ch/chdh/notes/14.htm
		#endregion

		#region "11.12 Encrypting web.config Information"
		public static void TestEncryptDecryptWebConfigSection()
		{
            try
            {
                DisplayWebConfigInfo();

                EncryptWebConfigSection(null, "appSettings", "DataProtectionConfigurationProvider");

                DisplayWebConfigInfo();

                DecryptWebConfigSection(null, "appSettings");

                DisplayWebConfigInfo();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Please check the config sections specified: " + ex.Message);
            }
		}

		public static void EncryptWebConfigSection(string appPath, string protectedSection, string dataProtectionProvider)
		{
			System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(appPath);
			ConfigurationSection webConfigSection = webConfig.GetSection(protectedSection);

			if (!webConfigSection.SectionInformation.IsProtected)
			{
				webConfigSection.SectionInformation.ProtectSection(dataProtectionProvider);
				webConfig.Save();
			}
		}

		public static void DecryptWebConfigSection(string appPath, string protectedSection)
		{
			System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(appPath);
			ConfigurationSection webConfigSection = webConfig.GetSection(protectedSection);

			if (webConfigSection.SectionInformation.IsProtected)
			{
				webConfigSection.SectionInformation.UnprotectSection();
				webConfig.Save();
			}
		}

		public static void DisplayWebConfigInfo()
		{
			//Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None, @"");
			System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(null);	//"http://localhost/WebApplication1/web.config");   //WebForm1.aspx

			Console.WriteLine("\r\n\r\n------------------------------------");
			Console.WriteLine($"FilePath: {webConfig.FilePath}");
			Console.WriteLine("APP_SETTINGS");
			foreach (KeyValueConfigurationElement o in webConfig.AppSettings.Settings)
			{
				Console.WriteLine($"\tKEY: {o.Key}");
				Console.WriteLine($"\tVALUE: {o.Value}");
			}
			Console.WriteLine("APP_CONN_STRS");
			foreach (ConnectionStringSettings css in webConfig.ConnectionStrings.ConnectionStrings)
			{
				Console.WriteLine($"\tNAME: {css.Name}");
				Console.WriteLine($"\tCONN_STR: {css.ConnectionString}");
			}
			Console.WriteLine("APP_SECTION_GROUP");
			foreach (ConfigurationSectionGroup sg in webConfig.SectionGroups)
			{
				Console.WriteLine($"\tNAME: {sg.Name}");
				//Console.WriteLine($"\tGROUP_NAME: {sg.SectionGroupName}");
				//Console.WriteLine($"\tTYPE: {sg.Type}");
			}
			Console.WriteLine("SECTIONS");
			foreach (ConfigurationSection cs in webConfig.Sections)
			{
				Console.WriteLine($"\tNAME: {cs.SectionInformation.Name}");
				//Console.WriteLine($"\tSECTION_NAME: {cs.SectionInformation.SectionName}");
				//Console.WriteLine($"\tSOURCE: {cs.ElementInformation.Source}");
			}
		}
		#endregion

		#region "11.13 Obtaining a Safer File Handle"
        // See text
		#endregion

		#region "11.14 Storing Passwords"

		const int HASH_ITERATIONS = 43;
		const string HASH_ALGORITHM = "SHA-512";
		const int SALT_LENGTH = 64;


		public static void TestPasswordHashing()
		{
			// User ("customer") registers with a specific password, in this case: "MyClearPwd"
			string userName = "customer";
			string srClearTextPwdGood = "MyClearPwd";
			string srClearTextPwdBad1 = "MyClearPwd1";
			string srClearTextPwdBad2 = "MyClearpwd";

			// Generate a salt and hash from the password
			SecureString salt;
			SecureString pwdHash = GeneratePasswordHashAndSalt(CreateSecureString(srClearTextPwdGood), out salt);

			// Store hash and salt together in the DB along with the username.
			SaveHashedPassword(userName, pwdHash, salt);

			// When comparing use the username to retrieve the hash and salt
			//    use this salt when re-hashing the entered pwd.
			//    Then compare the 2 hashes.
			SecureString hashPart;
			SecureString saltPart;
            RetrieveHashedPasswordAndSalt(userName, out hashPart, out saltPart);

			//----TEST CODE
			//string hashPlusSalt = ReadSecureString(pwdHash) + ReadSecureString(salt);   //BROKEN 
			//SecureString hashPart = CreateSecureString(hashPlusSalt.Substring(0, 64));
			//SecureString saltPart = CreateSecureString(hashPlusSalt.Substring(64));
			hashPart = pwdHash;
			saltPart = salt;

			//Console.WriteLine($"hashPlusSalt Length == {hashPlusSalt.Length}");
			Console.WriteLine($"Secondary  generated salt: {ReadSecureString(saltPart)}");
			Console.WriteLine($"Secondary  generated salt Length: {ReadSecureString(saltPart).Length}{Environment.NewLine}");

			Console.WriteLine($"Secondary  hashed password: {ReadSecureString(hashPart)}");
			Console.WriteLine($"Secondary  hashed password Length: {ReadSecureString(hashPart).Length}{Environment.NewLine}{Environment.NewLine}");
			//----TEST CODE

			// Compare hashed pwds
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd\". Result: {ComparePasswords(pwdHash, salt, CreateSecureString(srClearTextPwdGood))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd1\". Result: {ComparePasswords(pwdHash, salt, CreateSecureString(srClearTextPwdBad1))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearpwd\". Result: {ComparePasswords(pwdHash, salt, CreateSecureString(srClearTextPwdBad2))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd1\" and NULL salt. Result: {ComparePasswords(pwdHash, null, CreateSecureString(srClearTextPwdBad1))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd\" and NULL salt. Result: {ComparePasswords(pwdHash, null, CreateSecureString(srClearTextPwdGood))}");

			Console.WriteLine();

			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd\". Result: {ComparePasswords(hashPart, saltPart, CreateSecureString(srClearTextPwdGood))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd1\". Result: {ComparePasswords(hashPart, saltPart, CreateSecureString(srClearTextPwdBad1))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearpwd\". Result: {ComparePasswords(hashPart, saltPart, CreateSecureString(srClearTextPwdBad2))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd1\" and NULL salt. Result: {ComparePasswords(hashPart, null, CreateSecureString(srClearTextPwdBad1))}");
			Console.WriteLine($"Compare originally hashed password with \"MyClearPwd\" and NULL salt. Result: {ComparePasswords(hashPart, null, CreateSecureString(srClearTextPwdGood))}");
		}

		public static SecureString GeneratePasswordHashAndSalt(SecureString passwd, out SecureString salt)
		{
			// First generate the unique salt we will use to hash with
			salt = GenerateSalt();
			Console.WriteLine($"Originally generated salt: {ReadSecureString(salt)}{Environment.NewLine}");
			Console.WriteLine($"Originally generated salt Length: {ReadSecureString(salt).Length}{Environment.NewLine}");

			// Create salted hash
			string hashedPwd = GenerateHash(passwd, salt);
			Console.WriteLine($"Originally hashed password: {hashedPwd}{Environment.NewLine}{Environment.NewLine}");
			Console.WriteLine($"Originally hashed password Length: {hashedPwd.Length}{Environment.NewLine}{Environment.NewLine}");

			return CreateSecureString(hashedPwd);
		}


		private static SecureString GenerateSalt()
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

			byte[] salt = new byte[SALT_LENGTH];
			rng.GetBytes(salt);

			return CreateSecureString(Convert.ToBase64String(salt));
		}


		private static string GenerateHash(SecureString clearTextData, SecureString salt)
		{
			if (salt?.Length > 0)
			{
				// Combine password and salt before hashing
				byte[] clearTextDataArray = Encoding.UTF8.GetBytes(ReadSecureString(clearTextData));
				byte[] clearTextSaltArray = Convert.FromBase64String(ReadSecureString(salt));

				byte[] clearTextDataSaltArray = new byte[clearTextDataArray.Length + clearTextSaltArray.Length];
				Array.Copy(clearTextDataArray, 0, clearTextDataSaltArray, 0, clearTextDataArray.Length);
				Array.Copy(clearTextSaltArray, 0, clearTextDataSaltArray, clearTextDataArray.Length, clearTextSaltArray.Length);

				// Use a secure hashing algorithm
				HashAlgorithm alg = HashAlgorithm.Create(HASH_ALGORITHM);

				byte[] hashedPwd = null;

				for (int index = 0; index < HASH_ITERATIONS; index++)
				{
					if (hashedPwd == null)
					{
						// Initial hash of the cleartext password
						hashedPwd = alg.ComputeHash(clearTextDataSaltArray);
					}
					else
					{
						// Re-hash the hash for added entropy
						hashedPwd = alg.ComputeHash(hashedPwd);
					}
				}

				return Convert.ToBase64String(hashedPwd);
			}
			else
			{
				throw new ArgumentException($"Salt parameter {nameof(salt)} cannot be empty or null. This is a security violation.");
			}
		}


		public static void SaveHashedPassword(string userName, SecureString pwdHash, SecureString salt)
		{
			string base64PwdHash = ReadSecureString(pwdHash);
			string base64Salt = ReadSecureString(salt);

			// Store in DB
			//   INSERT users ('user', 'pwd', 'salt', ...) (userName, base64PwdHash, base64Salt, ...)
		}


		public static void RetrieveHashedPasswordAndSalt(string userName, out SecureString storedHashedPwd, out SecureString storedSalt)
		{
			// Get from DB
			//    SELECT pwd, salt FROM users WHERE user = ?
			//    SetString(userName);

			storedHashedPwd = new SecureString();
			storedSalt = new SecureString();
        }


		public static bool ComparePasswords(SecureString storedHashedPwd, SecureString storedSalt, SecureString clearTextPwd)
        {
            try
            {
                // First hash the clear text pwd using the same technique
                byte[] userEnteredHashedPwd = Convert.FromBase64String(GenerateHash(clearTextPwd, storedSalt));

				// Get the stored hashed pwd/salt
				byte[] originalHashedPwd = Convert.FromBase64String(ReadSecureString(storedHashedPwd));

				// Now compare the two hashes
				//    If true, the user entered password is correct
				if (userEnteredHashedPwd.SequenceEqual(originalHashedPwd))
					return true;
            }
            catch(ArgumentException ae)
            {
                // You should log this error and return false here
                Console.WriteLine(ae.Message);
                return false;
            }

            return false;
        }
        #endregion
    }
}
