using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;


namespace CSharpRecipes
{
    public class RegEx
    {
        #region "7.1 Extracting Groups from a MatchCollection"
        public static void TestExtractGroupings()
        {
            string source = @"Path = ""\\MyServer\MyService\MyPath;
                              \\MyServer2\MyService2\MyPath2\""";
            string matchPattern = @"\\\\					# \\
									(?<TheServer>\w*)		# Server name
									\\						# \
									(?<TheService>\w*)\\	# Service name";

            //			foreach(Match m in theMatches)
            //			{
            //				for (int counter = 0; counter < m.Groups.Count; counter++)
            //				{
            //					Console.WriteLine(m.Groups[0].GroupNameFromNumber(counter), m.Groups[counter]);
            //				}
            //			}


            foreach (Dictionary<string, Group> grouping in ExtractGroupings(source, matchPattern, true))
            {
                foreach (KeyValuePair<string, Group> kvp in grouping)
                    Console.WriteLine($"Key/Value = {kvp.Key} / {kvp.Value}"); 
                Console.WriteLine("");
            }
        }

        public static List<Dictionary<string, Group>> ExtractGroupings(string source, string matchPattern,
                                                       bool wantInitialMatch)
        {
            List<Dictionary<string, Group>> keyedMatches = new List<Dictionary<string, Group>>();
            int startingElement = 1;
            if (wantInitialMatch)
            {
                startingElement = 0;
            }

            Regex RE = new Regex(matchPattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            MatchCollection theMatches = RE.Matches(source);

            //			return (theMatches);


            foreach (Match m in theMatches)
            {
                Dictionary<string, Group> groupings = new Dictionary<string, Group>();

                for (int counter = startingElement; counter < m.Groups.Count; counter++)
                {
                    groupings.Add(RE.GroupNameFromNumber(counter), m.Groups[counter]);
                }

                keyedMatches.Add(groupings);
            }

            return (keyedMatches);
        }
        #endregion

        #region "7.2 Verifying the Syntax of a Regular Expression"

        public static void TestUserInputRegEx(string regEx)
        {
            if (VerifyRegEx(regEx))
                Console.WriteLine("This is a valid regular expression.");
            else
                Console.WriteLine("This is not a valid regular expression.");
        }

        public static bool VerifyRegEx(string testPattern)
        {
            bool isValid = true;

            if ((testPattern?.Length ?? 0) > 0)
            {
                try
                {
                    Regex.Match("", testPattern);
                }
                catch (ArgumentException ae)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                    Console.WriteLine(ae);
                }
            }
            else
            {
                //BAD PATTERN: Pattern is null or empty
                isValid = false;
            }

            return (isValid);
        }

        #endregion

        #region "7.3 Augmenting the Basic String Replacement Function"
        public static string MatchHandler(Match theMatch)
        {
            // Handle Top attributes of the Property tag
            if (theMatch.Value.StartsWith("<Property", StringComparison.Ordinal))
            {
                long controlValue = 0;

                // Obtain the numeric value of the Top attribute
                Match topAttributeMatch = Regex.Match(theMatch.Value, "Top=\"([-]*\\d*)");
                if (topAttributeMatch.Success)
                {
                    if (string.IsNullOrEmpty(topAttributeMatch.Groups[1].Value.Trim()))
                    {
                        // If blank, set to zero
                        return (theMatch.Value.Replace(topAttributeMatch.Groups[0].Value.Trim(), "Top=\"0"));
                    }
                    else if (topAttributeMatch.Groups[1].Value.Trim().StartsWith("-"))
                    {
                        // If only a negative sign (syntax error), set to zero
                        return (theMatch.Value.Replace(topAttributeMatch.Groups[0].Value.Trim(), "Top=\"0"));
                    }
                    else
                    {
                        // We have a valid number
                        // Convert the matched string to a numeric value
                        controlValue = long.Parse(topAttributeMatch.Groups[1].Value,
                            System.Globalization.NumberStyles.Any);

                        // If the Top attribute is out of the specified range, set it to zero
                        if (controlValue < 0 || controlValue > 5000)
                        {
                            return (theMatch.Value.Replace(topAttributeMatch.Groups[0].Value.Trim(),
                                "Top=\"0"));
                        }
                    }
                }
            }

            return (theMatch.Value);
        }

        public static void ComplexReplace(string matchPattern, string source)
        {
            MatchEvaluator replaceCallback = new MatchEvaluator(MatchHandler);
            string newString = Regex.Replace(source, matchPattern, replaceCallback);

            Console.WriteLine($"Replaced String = {newString}");
        }

        public static void TestComplexReplace()
        {
            string matchPattern = "<.*>";
            string source = @"<?xml version=""1.0\"" encoding=\""UTF-8\""?>
        <Window ID=""Main"">
            <Control ID=""TextBox"">
                <Property Top=""100"" Left=""0"" Text=""BLANK""/>
            </Control>
            <Control ID=""Label"">
                <Property Top=""99990"" Left=""0"" Caption=""Enter Name Here""/>
            </Control>
        </Window>";

            ComplexReplace(matchPattern, source);
        }
        #endregion

        #region "7.4 Implementing a Better Tokenizer"
        public static string[] Tokenize(string equation)
        {
            Regex RE = new Regex(@"([\+\-\*\(\)\^\\])");
            return (RE.Split(equation));
        }

        public static void TestTokenize()
        {
            foreach (string token in Tokenize("(y - 3)(3111*x^21 + x + 320)"))
                Console.WriteLine($"String token = {token.Trim()}");
        }
        #endregion

        #region "7.5 Returning the Entire Line in Which a Match is Found"
        public static List<string> GetLines2(string source, string pattern, bool isFileName)
        {
            // Start Timer Code...
            Counter c = new Counter();
            c.Clear();
            c.Start();
            // Start Timer Code...


            string text = source;
            List<string> matchedLines = new List<string>();

            // If this is a file, get the entire file's text
            if (isFileName)
            {
                FileStream FS = new FileStream(source, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                StreamReader SR = new StreamReader(FS);

                while (text != null)
                {
                    text = SR.ReadLine();

                    if (text != null)
                    {
                        // Run the regex on each line in the string
                        Regex RE = new Regex(pattern, RegexOptions.Multiline);
                        MatchCollection theMatches = RE.Matches(text);

                        if (theMatches.Count > 0)
                        {
                            // Get the line if a match was found
                            matchedLines.Add(text);
                        }
                    }
                }
            }
            else
            {
                // Run the regex once on the entire string
                Regex RE = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection theMatches = RE.Matches(text);

                // Get the line for each match
                foreach (Match m in theMatches)
                {
                    int lineStartPos = GetBeginningOfLine(text, m.Index);
                    int lineEndPos = GetEndOfLine(text, (m.Index + m.Length - 1));
                    string line = text.Substring(lineStartPos, lineEndPos - lineStartPos);
                    matchedLines.Add(line);
                }
            }


            // End Timer Code...
            c.Stop();
            Console.WriteLine("Seconds: " + c.Seconds.ToString());
            // End Timer Code...


            return (matchedLines);
        }



        //public static List<string> GetLines(string source, string pattern, bool isFileName)
        //{
        //	// Start Timer Code...
        //	Counter c = new Counter();
        //	c.Clear();
        //	c.Start();
        //	// Start Timer Code...


        //	string text = source;
        //	List<string> matchedLines = new List<string>();

        //	// If this is a file, get the entire file's text
        //	if (isFileName)
        //	{
        //		FileStream FS = new FileStream(source, FileMode.Open, 
        //			FileAccess.Read, FileShare.Read);
        //		StreamReader SR = new StreamReader(FS);
        //		text = SR.ReadToEnd();
        //		SR.Close();
        //		FS.Close();
        //	}

        //	// Run the regex once on the entire string
        //	Regex RE = new Regex(pattern, RegexOptions.Multiline);
        //	MatchCollection theMatches = RE.Matches(text);

        //	// Get the line for each match
        //	foreach (Match m in theMatches)
        //	{
        //		int lineStartPos = GetBeginningOfLine(text, m.Index);
        //		int lineEndPos = GetEndOfLine(text, (m.Index + m.Length - 1));
        //		string line = text.Substring(lineStartPos, lineEndPos - lineStartPos);
        //		matchedLines.Add(line);
        //	}


        //	// End Timer Code...
        //	c.Stop();
        //	Console.WriteLine("Seconds: \{c.Seconds.ToString()}");
        //	// End Timer Code...


        //	return (matchedLines);
        //}
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static List<string> GetLines(string source, string pattern, bool isFileName)
        {
            List<string> matchedLines = new List<string>();

            // If this is a file, get the entire file's text.
            if (isFileName)
            {
                using (FileStream FS = new FileStream(source, FileMode.Open,
                       FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader SR = new StreamReader(FS))
                    {
                        Regex RE = new Regex(pattern, RegexOptions.Multiline);
                        string text = "";
                        while (text != null)
                        {
                            text = SR.ReadLine();
                            if (text != null)
                            {
                                // Run the regex on each line in the string.
                                //    It is necessary to add CRLF chars
                                //    since Readline() strips off these chars
                                if (RE.IsMatch(text + Environment.NewLine))
                                {
                                    // Get the line if a match was found.
                                    matchedLines.Add(text);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Run the regex once on the entire string.
                Regex RE = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection theMatches = RE.Matches(source);

                // Use these vars to remember the last line added to matchedLines
                // so that we do not add duplicate lines.
                int lastLineStartPos = -1;
                int lastLineEndPos = -1;

                // Get the line for each match.
                foreach (Match m in theMatches)
                {
                    int lineStartPos = GetBeginningOfLine(source, m.Index);
                    int lineEndPos = GetEndOfLine(source, (m.Index + m.Length - 1));

                    // If this is not a duplicate line, add it.
                    if (lastLineStartPos != lineStartPos &&
                        lastLineEndPos != lineEndPos)
                    {
                        string line = source.Substring(lineStartPos,
                                        lineEndPos - lineStartPos);
                        matchedLines.Add(line);

                        // Reset line positions.
                        lastLineStartPos = lineStartPos;
                        lastLineEndPos = lineEndPos;
                    }
                }
            }
            return (matchedLines);
        }

        public static int GetBeginningOfLine(string text, int startPointOfMatch)
        {
            if (startPointOfMatch > 0)
            {
                --startPointOfMatch;
            }

            if (startPointOfMatch >= 0 && startPointOfMatch < text?.Length)
            {
                // Move to the left until the first '\n char is found
                for (int index = startPointOfMatch; index >= 0; index--)
                {
                    if (text?[index] == '\n')
                    {
                        return (index + 1);
                    }
                }

                return (0);
            }

            return (startPointOfMatch);
        }

        public static int GetEndOfLine(string text, int endPointOfMatch)
        {
            if (endPointOfMatch >= 0 && endPointOfMatch < text?.Length)
            {
                // Move to the right until the first '\n char is found
                for (int index = endPointOfMatch; index < text.Length; index++)
                {
                    if (text?[index] == '\n')
                    {
                        return (index);
                    }
                }

                return (text.Length);
            }

            return (endPointOfMatch);
        }

        public static void TestGetLine()
        {
            // Get the matching lines within the file TestFile.txt
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            List<string> lines = GetLines(@"..\..\TestFile.txt", "\n", true);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the string TestString
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines("Line1\r\nLine2\r\nLine3\nLine4", "Line", false);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the string TestString
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines(@"Line1
                Line2
                Line3
                Line4", "Line", false);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the string TestString
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines("\rLLLLLL", "L", false);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the file TestFile.txt
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines2(@"..\..\TestFile.txt", "\n", true);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the string TestString
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines2("Line1\r\nLine2\r\nLine3\nLine4", "Line", false);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");

            // Get the matching lines within the string TestString
            Console.WriteLine("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
            lines = GetLines2("\rLLLLLL", "L", false);
            foreach (string s in lines)
                Console.WriteLine($"MatchedLine: {s}");
        }
        #endregion

        #region "7.6 Finding a Particular Occurrence of a Match"
        public static Match FindOccurrenceOf(string source, string pattern, int occurrence)
        {
            if (occurrence < 1)
            {
                throw (new ArgumentException("Cannot be less than 1", nameof(occurrence)));
            }

            // Make occurrence zero-based
            --occurrence;

            // Run the regex once on the source string
            Regex RE = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            if (occurrence >= theMatches.Count)
            {
                return (null);
            }
            else
            {
                return (theMatches[occurrence]);
            }
        }

        public static List<Match> FindEachOccurrenceOf(string source, string pattern, int occurrence)
        {
            if (occurrence < 1)
            {
                throw (new ArgumentException("Cannot be less than 1", nameof(occurrence)));
            }

            List<Match> occurrences = new List<Match>();

            // Run the regex once on the source string
            Regex RE = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection theMatches = RE.Matches(source);

            for (int index = (occurrence - 1); index < theMatches.Count; index += occurrence)
            {
                occurrences.Add(theMatches[index]);
            }

            return (occurrences);
        }

        public static void TestOccurrencesOf()
        {
            Console.WriteLine();
            Console.WriteLine();
            Match matchResult = FindOccurrenceOf("one two three one two three one two three one"
                + " two three one two three one two three", "two", 2);
            Console.WriteLine($"{matchResult?.ToString()}\t{matchResult?.Index}");

            Console.WriteLine();
            List<Match> results = FindEachOccurrenceOf("one one two three one two three one two" +
                " three one two three", "one", 2);
            foreach (Match m in results)
                Console.WriteLine($"{m.ToString()}\t{m.Index}");
        }
        #endregion

        #region "7.7 Using Common Patterns"
        /*
Match only alphanumeric characters along with the characters ?, +, ., and any whitespace:
	    ^([\w\.\+\-]|\s)*$

Be careful using the - character within a character class—a regular expression enclosed within [ and ]. That character is also used to specify a range of characters, as in a-z for a through z inclusive. If you want to use a literal - character, either escape it with \ or put it at the end of the expression, as shown in the previous and next examples.

Match only alphanumeric characters along with the characters ?, +, ., and any whitespace, with the stipulation that there is at least one of these characters and no more than 10 of these characters:
	    ^([\w\.\+\-]|\s){1,10}$

Match a person’s name, up to 55 characters:
	    ^[a-zA-Z\'\-\s]{1,55}$

Match a positive or negative integer:
	    ^((\+|\?)\d)?\d+*$

Match a positive or negative floating point number only, this pattern does not match integers:
	    ^(\+|\?)?(\d*\.\d+)$

Match a floating point or integer number that can have a positive or negative value
	    ^(\+|\?)?(\d*\.)?\d+|\d+)$

Match a date in the form ##/##/####, where the day and month can be a one- or two-digit value and year can either only be a two- or four-digit value:
	    ^\d{1,2}\/\d{1,2}\/\d{2,4}$

Match a time to be entered in the form ##:## with an optional am or pm extension (note that this regular expression also handles military time):
	    ^\d{1,2}:\d{2}\s?([ap]m)?$

Verify if the input is a Ssocial sSecurity number of the form ###-##-####:
	    ^\d{3}-\d{2}-\d{4}$

Match an IPv4 address:
	    ^([0-2]?[0-95]?[0-95]\.){3}[0-2]?[0-95]?[0-95]$

Verify that an email address is in the form name@address where address is not an IP address:
	    ^[A-Za-z0-9_\-\.]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+$

Verify that an email address is in the form name@address where address is an IP address:
	    ^[A-Za-z0-9_\-\.]+@([0-2]?[0-95]?[0-95]\.){3}[0-2]?[0-59]?[0-95]$

Match or verify a URL that uses either the HTTP, HTTPS, or FTP protocol. Note that this regular expression will not match relative URLs:.
	    ^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-
	    9\-\._\?\,\'/\\\+&%\$#\=~])*$

Match only a dollar amount with the optional $ and + or -preceding characters (note that any number of decimal places may be added):
	    ^\$?[+-]?[\d,]*(\.\d*)?$

This is similar to the previous regular expression, except that no more than two decimal places are allowed
	    ^\$?[+-]?[\d,]*\.?\d{0,2}$

Match a credit card number to be entered as four sets of four digits separated with a space, -, or no character at all:
	    ^((\d{4}[- ]?){3}\d{4})$

Match a zip code to be entered as five digits with an optional four-digit extension:
	    ^\d{5}(-\d{4})?$

Match a North American phone number with an optional area code and an optional - character to be used in the phone number and no extension:
	    ^(\(?[0-9]{3}\)?)?\-?[0-9]{3}\-?[0-9]{4}$

Match a phone number similar to the previous regular expression but allow an optional five-digit extension prefixed with either ext or extension:
	    ^(\(?[0-9]{3}\)?)?\-?[0-9]{3}\-?[0-9]{4}(\s*ext(ension)?[0-9]{5})?$

Match a full path beginning with the drive letter and optionally match a filename with a three-character extension (note that no .. characters signifying to move up the directory hierarchy are allowed, nor is a directory name with a . followed by an extension):
	    ^[a-zA-Z]:[\\/]([_a-zA-Z0-9]+[\\/]?)*([_a-zA-Z0-9]+\.[_a-zA-Z0-9]{0,3})?$

Verify if the input password string matches some specific rules for entering a password (i.e., the password is between 6 and 25 characters in length and contains alphanumeric characters):
	    ^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,25}$

Determine if any malicious characters were input by the user. Note that this regular expression will not prevent all malicious input, and it also prevents some valid input, such as last names that contain a single quote:.
	    ^([^\)\(\<\>\"\'\%\&\+\;][(-{2})])*$

Extract a tag from an XHTML, HTML, or XML string. This regular expression will return the beginning tag and ending tag, including any attributes of the tag. Note that you will need to replace TAGNAME with the real tag name you want to search for:. 
	    <TAGNAME.*?>(.*?)</TAGNAME>

Extract a comment line from code. The following regular expression extracts HTML comments from a web page. This can be useful in determining if any HTML comments that are leaking sensitive information need to be removed from your code base before it goes into production:. 
	    <!--.*?-->

Match a C# single line comment:
	    //.*$

Match a C# multi-line comment:
	    /\*.*?\*/

        #endregion
    }



    #region Timing Code Helper Class
    // Downloaded code...
    public class Counter 
	{
		long elapsedCount = 0;
		long startCount = 0;

		public void Start()
		{
			startCount = 0;
            NativeMethods.QueryPerformanceCounter(ref startCount);
		}
		
		public void Stop()
		{
			long stopCount = 0;
            NativeMethods.QueryPerformanceCounter(ref stopCount);

			elapsedCount += (stopCount - startCount);
		}

		public void Clear() => elapsedCount = 0;

		public float Seconds
		{
			get
			{
				long freq = 0;
                NativeMethods.QueryPerformanceFrequency(ref freq);
				return((float) elapsedCount / (float) freq);
			}
		}

		public override string ToString() => $"{Seconds} seconds";

		static long Frequency 
		{
			get 
			{
				long freq = 0;
                NativeMethods.QueryPerformanceFrequency(ref freq);
				return freq;
			}
		}
		static long Value 
		{
			get 
			{
				long count = 0;
                NativeMethods.QueryPerformanceCounter(ref count);
				return count;
			}
		}
	}

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("KERNEL32")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [System.Runtime.InteropServices.DllImport("KERNEL32")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceFrequency(ref long lpFrequency);
    }
    #endregion
}
