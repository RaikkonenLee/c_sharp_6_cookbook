using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace CSharpRecipes
{
    #region Extension Methods
    static class DataTypeExtMethods
    {
        #region "3.1 Encoding a Binary as Base64"
        public static string Base64EncodeBytes(this byte[] inputBytes) => 
            (Convert.ToBase64String(inputBytes));
        #endregion

        #region "3.2 Decoding a Base64-encoded Binary"
        public static byte[] Base64DecodeString(this string inputStr)
        {
            byte[] decodedByteArray =
                    Convert.FromBase64String(inputStr);
            return (decodedByteArray);
        }
        #endregion

        #region 3.8 Safely Performing a Narrowing Numeric Cast
        public static int AddNarrowingChecked(this long lhs, long rhs) => 
            checked((int)(lhs + rhs));
        #endregion
    }
    #endregion

    public static class DataTypes
	{
        #region "3.1 Encoding a Binary as Base64"
        public static string EncodeBitmapToString(string bitmapFilePath)
        {
            byte[] image = null;
            FileStream fstrm = 
                new FileStream(bitmapFilePath,
                                FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(fstrm))
            {
                image = new byte[reader.BaseStream.Length];
                for (int i = 0; i < reader.BaseStream.Length; i++)
                    image[i] = reader.ReadByte();
            }
            return image.Base64EncodeBytes();
        }

        public static string MakeBase64EncodedStringForMime(string base64Encoded)
        {
            // This is the code to embed CRLF after every 76 chars in the Base64 encoded
            //   string so that the entire encoded string may be sent as an embedded MIME
            //   attachment in an email (MIME attachements have a limit of 76 chars per line)
            StringBuilder originalStr = new StringBuilder(base64Encoded);
            StringBuilder newStr = new StringBuilder();
            const int mimeBoundary = 76;
            int cntr = 1;
            while ((cntr * mimeBoundary) < (originalStr.Length - 1))
            {
                newStr.AppendLine(originalStr.ToString(((cntr - 1) * mimeBoundary), mimeBoundary));
                cntr++;
            }
            if (((cntr - 1) * mimeBoundary) < (originalStr.Length - 1))
            {
                newStr.AppendLine(originalStr.ToString(((cntr - 1) * mimeBoundary), 
                    ((originalStr.Length) - ((cntr - 1) * mimeBoundary))));
            }
            return newStr.ToString();
        }

        public static void TestEncodingBinaryBase64()
        {
            byte[] ba = new byte[5] { 32, 33, 34, 35, 36 };
            Console.WriteLine(ba.Base64EncodeBytes());
            foreach (byte b in ba.Base64EncodeBytes().Base64DecodeString())
                Console.WriteLine(b);

            string output = EncodeBitmapToString(@"CSCBCover.bmp");
            Console.WriteLine(output);

            string mimeReady = MakeBase64EncodedStringForMime(output);
            Console.WriteLine(mimeReady);
        }
        #endregion

        #region "3.2 Decoding a Base64-encoded Binary"
        public static void TestDecodingBinaryBase64()
        {
            // Use the encoding method from 3.1 to get the encoded byte array
            string bmpAsString = EncodeBitmapToString(@"CSCBCover.bmp");
            //Get a temp file name and path to write to
            string bmpFile = Path.GetTempFileName() + ".bmp";

            // decode the image with the extension method
            byte[] imageBytes = bmpAsString.Base64DecodeString();
            FileStream fstrm = new FileStream(bmpFile,
                                FileMode.CreateNew, FileAccess.Write);
            using (BinaryWriter writer = new BinaryWriter(fstrm))
            {
                writer.Write(imageBytes);
            }

            // Comment out this line to actually see the file
            File.Delete(bmpFile);
        }

        #endregion

        #region "3.3 Converting a String Returned as a Byte[] Back Into a String"
        public static void TestConvertingStringAsByteArrayToString()
        {
            byte[] asciiCharacterArray = {128, 83, 111, 117, 114, 99, 101,
                                        32, 83, 116, 114, 105, 110, 103, 128};
            string asciiCharacters = Encoding.ASCII.GetString(asciiCharacterArray);
            Console.WriteLine(asciiCharacters);

            byte[] unicodeCharacterArray = {128, 0, 83, 0, 111, 0, 117, 0, 114, 0, 99, 0,
                                        101, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110,
                                        0, 103, 0, 128, 0};
            string unicodeCharacters = Encoding.Unicode.GetString(unicodeCharacterArray);
            Console.WriteLine(unicodeCharacters);
        }
        #endregion

        #region "3.4 Passing a String to a Method that Accepts only a Byte[]"
        public static void TestConvertingStringToByteArray()
        {
            byte[] asciiCharacterArray = {128, 83, 111, 117, 114, 99, 101,
                                        32, 83, 116, 114, 105, 110, 103, 128};
            string asciiCharacters = Encoding.ASCII.GetString(asciiCharacterArray);

            byte[] asciiBytes = Encoding.ASCII.GetBytes(asciiCharacters);

            foreach (byte b in asciiBytes)
                Console.WriteLine(b);

            byte[] unicodeCharacterArray = {128, 0, 83, 0, 111, 0, 117, 0, 114, 0, 99, 0,
                                        101, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110,
                                        0, 103, 0, 128, 0};
            string unicodeCharacters = Encoding.Unicode.GetString(unicodeCharacterArray);

            byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodeCharacters);
            foreach (byte b in unicodeBytes)
                Console.WriteLine(b);

            //Discussion code
            byte[] sourceArray = { 83 };
            byte[] sourceArray2 = { 83, 0 };
        }
        #endregion

        #region "3.5 Determining if a String is a Valid Number"
        public static void TestDetermineIfStringIsNumber()
        {
            string IsNotNumber = "111west";
            string IsNum = "  +111  ";
            string IsFloat = "  23.11  ";
            string IsExp = "  +23 e+11  ";

            int i = 0;
            float f = 0;

            Console.WriteLine(int.TryParse(IsNum, out i));      // 111		// 1.1 will not work here
            Console.WriteLine(float.TryParse(IsNum, out f));        // 111
            Console.WriteLine(float.TryParse(IsFloat, out f));  // 23.11
                                                                //Console.WriteLine(float.Parse(IsExp));	// throws

            Console.WriteLine(IsInt(IsNum));		// True
            Console.WriteLine(IsInt(IsNotNumber));		// False
            Console.WriteLine(IsInt(IsFloat));		// False
            Console.WriteLine(IsInt(IsExp));        // False
            Console.WriteLine();


            Console.WriteLine(IsDoubleFromTryParse(IsNum));		// True
            Console.WriteLine(IsDoubleFromTryParse(IsNotNumber));		// False
            Console.WriteLine(IsDoubleFromTryParse(IsFloat));		// True
            Console.WriteLine(IsDoubleFromTryParse(IsExp));     // False
            Console.WriteLine();

            string str = "12.5";
            double result = 0;
            if (double.TryParse(str,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.NumberFormatInfo.CurrentInfo,
                    out result))
            {
                // Is a double!
            }

        }


        public static bool IsInt(string value)
        {
            int foo;
            return int.TryParse(value, out foo);
        }
        public static bool IsDoubleFromTryParse(string value)
        {
            double result = 0;
            return double.TryParse(value, System.Globalization.NumberStyles.Float, 
                System.Globalization.NumberFormatInfo.CurrentInfo, out result);
        }
        #endregion

        #region "3.6 Rounding a Floating Point Value"		
        public static void TestRound()
        {
            int i = (int)Math.Round(2.5555); // i == 3
            Console.WriteLine(i);
            Console.WriteLine(Math.Round(2.5555, 2));
            double dbl = Math.Round(2.5555, 2); // dbl == 2.56
            Console.WriteLine(Math.Round(2.444444, 3));
            Console.WriteLine(Math.Round(2.555555555555555555555555555550001));
            Console.WriteLine(Math.Round(.5));
            double dbl1 = Math.Round(1.5); // dbl1 == 2
            Console.WriteLine(Math.Round(1.5));
            double dbl2 = Math.Round(2.5); // dbl2 == 2
            Console.WriteLine(Math.Round(2.5));
            Console.WriteLine(Math.Round(3.5));
            Console.WriteLine();

            Console.WriteLine(Math.Floor(.5));
            Console.WriteLine(Math.Floor(1.5));
            Console.WriteLine(Math.Floor(2.5));
            Console.WriteLine(Math.Floor(3.5));
            Console.WriteLine();

            Console.WriteLine(Math.Ceiling(.5));
            Console.WriteLine(Math.Ceiling(1.5));
            Console.WriteLine(Math.Ceiling(2.5));
            Console.WriteLine(Math.Ceiling(3.5));
            Console.WriteLine();

            Console.WriteLine(RoundUp(.4));
            Console.WriteLine(RoundUp(.5));
            Console.WriteLine(RoundUp(.6));
            Console.WriteLine(RoundUp(1.4));
            Console.WriteLine(RoundUp(1.5));
            Console.WriteLine(RoundUp(1.6));
            Console.WriteLine(RoundUp(2.4));
            Console.WriteLine(RoundUp(2.5));
            Console.WriteLine(RoundUp(2.6));
            Console.WriteLine(RoundUp(3.4));
            Console.WriteLine(RoundUp(3.5));
            Console.WriteLine(RoundUp(3.6));
            Console.WriteLine();

            Console.WriteLine(RoundDown(.4));
            Console.WriteLine(RoundDown(.5));
            Console.WriteLine(RoundDown(.6));
            Console.WriteLine(RoundDown(1.4));
            Console.WriteLine(RoundDown(1.5));
            Console.WriteLine(RoundDown(1.6));
            Console.WriteLine(RoundDown(2.4));
            Console.WriteLine(RoundDown(2.5));
            Console.WriteLine(RoundDown(2.6));
            Console.WriteLine(RoundDown(3.4));
            Console.WriteLine(RoundDown(3.5));
            Console.WriteLine(RoundDown(3.6));
        }
        #endregion

        #region "3.7 Choosing a Rounding Algorithm"
        public static double RoundUp(double valueToRound) => Math.Floor(valueToRound + 0.5);

        public static double RoundDown(double valueToRound)
        {
            double floorValue = Math.Floor(valueToRound);
            if ((valueToRound - floorValue) > .5)
                return (floorValue + 1);
            else
                return (floorValue);
        }
        #endregion

        #region "3.8 Safely Performing a Narrowing Numeric Cast"
        public static void TestNarrowing()
        {
            long lhs = 34000;
            long rhs = long.MaxValue;
            try
            {
                int result = lhs.AddNarrowingChecked(rhs);
            }
            catch(OverflowException)
            {
                // could not be added
            }

            // Our two variables are declared and initialized
            int sourceValue = 34000;
            short destinationValue = 0;

            // Determine if sourceValue will lose information in a cast to a short
            if (sourceValue <= short.MaxValue && sourceValue >= short.MinValue)
                destinationValue = (short)sourceValue;
            else
            {
                // Inform the application that a loss of information will occur
            }
        }


        #endregion

        #region "3.9 Testing For A Valid Enumeration Value"

        public enum Language
        {
            Other = 0, CSharp = 1, VBNET = 2, VB6 = 3,
            All = (Other | CSharp | VBNET | VB6)
        }
        public static void TestValidEnumValue()
        {
            HandleEnum(Language.CSharp);
            HandleEnum((Language)1); // 1 is CSharp

            HandleEnum((Language)100);
            int someVar = 42;
            HandleEnum((Language)someVar);

        }
        public static void HandleEnum(Language language)
        {
            if (CheckLanguageEnumValue(language))
            {
                // Use language here
                Console.WriteLine($"{language} is an OK enum value");
            }
            else
            {
                // Deal with the invalid enum value here
                Console.WriteLine($"{language} is not an OK enum value");
            }
        }

        public static bool CheckLanguageEnumValue(Language language)
        {
            switch (language)
            {
                // all valid types for the enum listed here
                // this means only the ones we specify are valid 
                // not any enum value for this enum
                case Language.CSharp:
                case Language.Other:
                case Language.VB6:
                case Language.VBNET:
                    break;
                default:
                    Debug.Assert(false, $"{language} is not a valid enumeration value to pass.");
                    return false;
            }
            return true;
        }
        #endregion

        #region "3.10 Using Enumerated Members in a Bitmask"
        // See recipe 3.10 in book for explanation.
        [Flags]
        public enum RecycleItems
        {
            None = 0x00,
            Glass = 0x01,
            AluminumCans = 0x02,
            MixedPaper = 0x04,
            Newspaper = 0x08
        }

        [Flags]
        public enum RecycleItemsMore
        {
            None = 0x00,
            Glass = 0x01,
            AluminumCans = 0x02,
            MixedPaper = 0x04,
            Newspaper = 0x08,
            TinCans = 0x10,
            Cardboard = 0x20,
            ClearPlastic = 0x40,
        }

        [Flags]
        public enum RecycleItemsAll
        {
            None = 0x00,
            Glass = 0x01,
            AluminumCans = 0x02,
            MixedPaper = 0x04,
            Newspaper = 0x08,
            TinCans = 0x10,
            Cardboard = 0x20,
            ClearPlastic = 0x40,
            All = (None | Glass | AluminumCans | MixedPaper | Newspaper | TinCans |
                   Cardboard | ClearPlastic)
        }

        [Flags]
        enum LanguageAllGroups
        {
            CSharp = 0x0001, VBNET = 0x0002, VB6 = 0x0004, Cpp = 0x0008,
            CobolNET = 0x000F, FortranNET = 0x0010, JSharp = 0x0020,
            MSIL = 0x0080,
            All = (CSharp | VBNET | VB6 | Cpp | FortranNET | JSharp | MSIL),
            VBOnly = (VBNET | VB6),
            NonVB = (CSharp | Cpp | FortranNET | JSharp | MSIL)
        }

        public static void TestEnumBitmask()
        {
            // Enum                      Bits
            //RecycleItems.Glass         0001
            //RecycleItems.AluminumCans  0010
            //ORed bit values            0011

            RecycleItems items = RecycleItems.Glass | RecycleItems.Newspaper;
            if ((items & RecycleItems.Glass) == RecycleItems.Glass)
                Console.WriteLine("The enum contains the C# enumeration value");
            else
                Console.WriteLine("The enum does NOT contain the C# value");

            // Enum                                          Bits
            //RecycleItems.Glass | RecycleItems.AluminumCans 0011
            //RecycleItems.Glass                             0001
            //ANDed bit values                               0001

            items = RecycleItems.Glass | RecycleItems.AluminumCans |
                     RecycleItems.MixedPaper;

        }
        #endregion

        #region "3.11 Determining If One or More Enumeration Flags are Set"
        // See recipe 3.11 in book for explanation.

        [Flags]
        public enum LanguageFlags
        {
            CSharp = 0x0001, VBNET = 0x0002, VB6 = 0x0004, Cpp = 0x0008,
            AllLanguagesExceptCSharp = VBNET | VB6 | Cpp
        }

        public static void TestEnumFlags()
        {
            LanguageFlags lang = LanguageFlags.CSharp | LanguageFlags.VBNET;

            if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
            {
                Console.WriteLine("lang contains at least Language.CSharp");
            }

            if (lang == LanguageFlags.CSharp)
            {
                Console.WriteLine("lang contains only the Language.CSharp");
            }

            if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                Console.WriteLine("lang contains at least Language.CSharp and Language.VBNET");
            }

            if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                Console.WriteLine("lang contains only the Language.CSharp and Language.VBNET");
            }

            lang = LanguageFlags.CSharp;
            if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
            {
                //Language_1_21.CSharp      0001
                //lang                 0001
                //ANDed bit values     0001

                Console.WriteLine("CSharp found in AND comparison (CSharp value)");
            }

            lang = LanguageFlags.CSharp;
            if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
            {
                // CSharp is found using OR logic
            }

            lang = LanguageFlags.CSharp | LanguageFlags.VB6 | LanguageFlags.Cpp;
            if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
            {
                // CSharp is found using OR logic
            }

            lang = LanguageFlags.VBNET | LanguageFlags.VB6 | LanguageFlags.Cpp;
            if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
            {
                // CSharp is found using OR logic
            }

            lang = LanguageFlags.VBNET;
            if ((lang & LanguageFlags.CSharp) == LanguageFlags.CSharp)
            {
                //Language_1_21.CSharp      0001
                //lang                 0010
                //ANDed bit values     0000
                Console.WriteLine("CSharp found in AND comparison (VBNET value)");
            }

            lang = LanguageFlags.CSharp;

            if (lang == LanguageFlags.CSharp)
            {
                //Language_1_21.CSharp      0001
                //lang                 0001
                //ORed bit values      0001

            }

            if ((lang > 0) && (LanguageFlags.CSharp == (lang | LanguageFlags.CSharp)))
            {
            }

            lang = LanguageFlags.CSharp | LanguageFlags.Cpp | LanguageFlags.VB6;

            if (lang == LanguageFlags.CSharp)
            {
                //Language_1_21.CSharp      0001
                //lang                 1101
                //ORed bit values      1101
            }

            lang = LanguageFlags.CSharp | LanguageFlags.VBNET;
            if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                //we can test multiple bits to determine whether they are both on and all other bits are off.
                Console.WriteLine("Found just CSharp and VBNET");
            }

            // now check with Cpp added
            lang = LanguageFlags.CSharp | LanguageFlags.VBNET | LanguageFlags.Cpp;
            if ((lang > 0) && ((lang & (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                //we can test multiple bits to determine whether they are at least both on regardless of what else is in there.
                Console.WriteLine("Found at least CSharp and VBNET ");
            }

            lang = LanguageFlags.CSharp | LanguageFlags.VBNET;
            if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                //we can determine whether at least these bits are turned on.
                Console.WriteLine("Found CSharp or VBNET");
            }

            lang = LanguageFlags.CSharp | LanguageFlags.VBNET | LanguageFlags.Cpp;
            if ((lang > 0) && ((lang | (LanguageFlags.CSharp | LanguageFlags.VBNET)) ==
               (LanguageFlags.CSharp | LanguageFlags.VBNET)))
            {
                //we can determine whether at least these bits are turned on.
                Console.WriteLine("Found CSharp or VBNET");
            }

            if ((lang > 0) && (lang | LanguageFlags.AllLanguagesExceptCSharp) ==
                LanguageFlags.AllLanguagesExceptCSharp)
            {
                Console.WriteLine("Only CSharp is not specified");
            }

        }
        #endregion
    }

}

