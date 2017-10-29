using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Web;
using System.Web.Compilation;
using System.CodeDom.Compiler;
using System.DirectoryServices;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CSharpRecipes
{
	public class NetworkingAndWeb
	{
		private static string GetWebAppPath()
		{
			string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

			#region Code to make it work in debug...
			// figure out where the web code is, should be in an adjacent directory
			// to the console app code
			string cscbWebPath = path;
			int index = -1;
			// keep backing up directories till we find it
			while (!Directory.Exists(cscbWebPath + @"\CSCBWeb"))
			{
				index = cscbWebPath.LastIndexOf('\\');
				if (index == -1)
				{
					cscbWebPath = "";
					break;
				}
				cscbWebPath = cscbWebPath.Substring(0, index);
			}
			#endregion

			// make sure we have a web path
			if (cscbWebPath.Length > 0)
			{
				// append webdir name
				cscbWebPath += @"\CSCBWeb";
			}
			return cscbWebPath;
		}

		#region "9.1 Handling Web Server Errors"
        public enum ResponseCategories
        {
	        Unknown,        // unknown code  ( < 100 or > 599)
	        Informational,  // informational codes (100 <= 199)
	        Success,        // success codes (200 <= 299)
	        Redirected,     // redirection code (300 <= 399)
	        ClientError,    // client error code (400 <= 499)
	        ServerError     // server error code (500 <= 599)
        }

        public static async Task HandlingWebServerErrorsAsync()
        {
	        HttpWebRequest httpRequest = null;
	        // get a URI object
	        Uri uri = new Uri("http://localhost");
	        // create the initial request
	        httpRequest = (HttpWebRequest)WebRequest.Create(uri);
	        HttpWebResponse httpResponse = null;
	        try
	        {
                httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync();
	        }
	        catch (WebException we)
	        {
		        Console.WriteLine(we.ToString());
		        return;
	        }
	        switch (CategorizeResponse(httpResponse))
	        {
		        case ResponseCategories.Unknown:
			        Console.WriteLine("Unknown");
			        break;
		        case ResponseCategories.Informational:
			        Console.WriteLine("Informational");
			        break;
		        case ResponseCategories.Success:
			        Console.WriteLine("Success");
			        break;
		        case ResponseCategories.Redirected:
			        Console.WriteLine("Redirected");
			        break;
		        case ResponseCategories.ClientError:
			        Console.WriteLine("ClientError");
			        break;
		        case ResponseCategories.ServerError:
			        Console.WriteLine("ServerError");
			        break;

	        }
        }

        public static ResponseCategories CategorizeResponse(HttpWebResponse httpResponse)
        {
	        // Just in case there are more success codes defined in the future
	        // by HttpStatusCode, we will check here for the "success" ranges
	        // instead of using the HttpStatusCode enum as it overloads some
	        // values
	        int statusCode = (int)httpResponse.StatusCode;
	        if ((statusCode >= 100) && (statusCode <= 199))
	        {
		        return ResponseCategories.Informational;
	        }
	        else if ((statusCode >= 200) && (statusCode <= 299))
	        {
		        return ResponseCategories.Success;
	        }
	        else if ((statusCode >= 300) && (statusCode <= 399))
	        {
		        return ResponseCategories.Redirected;
	        }
	        else if ((statusCode >= 400) && (statusCode <= 499))
	        {
		        return ResponseCategories.ClientError;
	        }
	        else if ((statusCode >= 500) && (statusCode <= 599))
	        {
		        return ResponseCategories.ServerError;
	        }
	        return ResponseCategories.Unknown;
        }

		#endregion

		#region "9.2 Communicating with a Web Server"
        public static async Task CommunicatingWithWebServerAsync()
        {
            try
            {
                // Working version once CSCBWeb is published locally
                HttpWebRequest request =
                    GenerateHttpWebRequest(new Uri("http://localhost/CSCBWeb/index.aspx"));
                // Book version
                //HttpWebRequest request =
                //    GenerateHttpWebRequest(new Uri("http://localhost/mysite/index.aspx"));

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    // This next line uses CategorizeResponse from Recipe 9.1
                    if (CategorizeResponse(response) == ResponseCategories.Success)
                    {
                        Console.WriteLine("Request succeeded");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Try launching the CSCBWeb project before running this test: " + ex.Message);
            }
        }

        // GET overload
        public static HttpWebRequest GenerateHttpWebRequest(Uri uri)
        {
            // create the initial request
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri);
            // return the request
            return httpRequest;
        }

        // POST overload
        public static HttpWebRequest GenerateHttpWebRequest(Uri uri,
	        string postData,
            string contentType)
        {
	        // create the initial request
            HttpWebRequest httpRequest = GenerateHttpWebRequest(uri);

	        // Get the bytes for the request, should be pre-escaped
	        byte[] bytes = Encoding.UTF8.GetBytes(postData);

	        // Set the content type of the data being posted.
            httpRequest.ContentType = contentType;
		        //"application/x-www-form-urlencoded"; for forms
                //"application/json" for json data 
                //"application/xml" for xml data

	        // Set the content length of the string being posted.
	        httpRequest.ContentLength = postData.Length;

	        // Get the request stream and write the post data in
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
	        // return the request
	        return httpRequest;
        }

		#endregion

		#region "9.3 Going Through a Proxy"
		public static void GoingThroughProxy()
		{
			HttpWebRequest request =
				GenerateHttpWebRequest(new Uri("http://internethost/mysite/index.aspx"));

			// add the proxy info
			AddProxyInfoToRequest(request, 
                                new Uri("http://webproxy:80"), 
                                "user", 
                                "pwd", 
                                "domain");

            // Set it up to go through the same proxy for all requests to this Uri
            Uri proxyURI = new Uri("http://webproxy:80");

            // in 1.1 you used to do this:
            //GlobalProxySelection.Select = new WebProxy(proxyURI);

            // Now in 2.0 and above you do this:
            WebRequest.DefaultWebProxy = new WebProxy(proxyURI);

            // for the current user Internet Explorer proxy info use this
            WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
		}

        public static HttpWebRequest AddProxyInfoToRequest(HttpWebRequest httpRequest,
	        Uri proxyUri,
	        string proxyId,
	        string proxyPassword,
	        string proxyDomain)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

	        // create the proxy object
	        WebProxy proxyInfo = new WebProxy();
	        // add the address of the proxy server to use
	        proxyInfo.Address = proxyUri;
	        // tell it to bypass the proxy server for local addresses
	        proxyInfo.BypassProxyOnLocal = true;
	        // add any credential information to present to the proxy server
	        proxyInfo.Credentials = new NetworkCredential(proxyId,
                proxyPassword,
		        proxyDomain);
	        // assign the proxy information to the request
	        httpRequest.Proxy = proxyInfo;

	        // return the request
	        return httpRequest;
        }

		#endregion

		#region "9.4 Obtaining the HTML from a URL"
		public static void ObtainingHtmlFromUrl()
		{
            try
            {
                string html = GetHtmlFromUrlAsync(new Uri("http://www.oreilly.com")).Result;
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
		}

        public static async Task<string> GetHtmlFromUrlAsync(Uri url)
        {
	        string html = string.Empty;
	        HttpWebRequest request = GenerateHttpWebRequest(url);
	        using(HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            {
		        if (CategorizeResponse(response) == ResponseCategories.Success)
		        {
			        // get the response stream.
			        Stream responseStream = response.GetResponseStream();
                    // use a stream reader that understands UTF8
                    using(StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
	                    html = reader.ReadToEnd();
                    }
		        }
	        }
	        return html;
        }

		#endregion

		#region "9.5 Using the Web Browser Control"
		// See the WebBrowser project 
		#endregion 

		#region "9.6 Pre-building an ASP.NET web site programmatically"
        public class MyClientBuildManagerCallback : ClientBuildManagerCallback
        {
	        public MyClientBuildManagerCallback()
		        : base()
	        {
	        }

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
	        public override void ReportCompilerError(CompilerError error)
	        {
		        string msg = $"Report Compiler Error: {error.ToString()}";
		        Debug.WriteLine(msg);
		        Console.WriteLine(msg);
	        }

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
	        public override void ReportParseError(ParserError error)
	        {
		        string msg = $"Report Parse Error: {error.ToString()}";
		        Debug.WriteLine(msg);
		        Console.WriteLine(msg);
	        }

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
            public override void ReportProgress(string message)
	        {
		        string msg = $"Report Progress: {message}";
		        Debug.WriteLine(msg);
		        Console.WriteLine(msg);
	        }
        }

        public static void TestBuildAspNetPages()
        {
	        try
	        {
		        // get the path to the web app shipping with this code...
		        string cscbWebPath = GetWebAppPath();

		        // make sure we have a web path
		        if(cscbWebPath.Length>0)
		        {
			        string appVirtualDir = @"CSCBWeb";
			        string appPhysicalSourceDir = cscbWebPath;
			        // make the target an adjacent directory as it cannot be in the same tree
			        // or the buildmanager screams...
			        string appPhysicalTargetDir = Path.GetDirectoryName(cscbWebPath) + @"\BuildCSCB"; 

			        // check flags again in Beta2, more options...
			        //AllowPartiallyTrustedCallers   
			        //Clean   
			        //CodeAnalysis   
			        //Default   
			        //DelaySign   
			        //FixedNames   
			        //ForceDebug   
			        //OverwriteTarget   
			        //Updatable 

        //                    Report Progress: Building directory '/CSCBWeb/App_Data'.
        //Report Progress: Building directory '/CSCBWeb/Role_Database'.
        //Report Progress: Building directory '/CSCBWeb'.
        //Report Compiler Error: c:\PRJ32\Book_2_0\C#Cookbook2\Code\CSCBWeb\Default.aspx.cs(14,7) : warning CS0105: The using directive for 'System.Configuration' appeared previously in this namespace


        //                    Report Progress: Building directory '/CSCBWeb/App_Data'.
        //Report Progress: Building directory '/CSCBWeb/Role_Database'.
        //Report Progress: Building directory '/CSCBWeb'.

			        PrecompilationFlags flags = PrecompilationFlags.ForceDebug |
										        PrecompilationFlags.OverwriteTarget;

                    ClientBuildManagerParameter cbmp = new ClientBuildManagerParameter();
                    cbmp.PrecompilationFlags = flags;
			        ClientBuildManager cbm =
					        new ClientBuildManager(appVirtualDir, 
											        appPhysicalSourceDir, 
											        appPhysicalTargetDir,
                                                    cbmp);
			        MyClientBuildManagerCallback myCallback = new MyClientBuildManagerCallback();
			        cbm.PrecompileApplication(myCallback);
		        }
	        }
	        catch (Exception e)
	        {
		        Debug.WriteLine(e.ToString());
	        }
        }
		#endregion

		#region "9.7 Escaping and Unescaping data for the web"
		public static void TestEscapeUnescape()
		{
            string data = "<H1>My html</H1>";
            Console.WriteLine($"Original Data: {data}");
            Console.WriteLine();

            string escapedData = Uri.EscapeDataString(data);
            Console.WriteLine($"Escaped Data: {escapedData}");
            Console.WriteLine();

            string unescapedData = Uri.UnescapeDataString(escapedData);
            Console.WriteLine($"Unescaped Data: {unescapedData}");
            Console.WriteLine();

            string uriString = "http://user:password@localhost:8080/www.abc.com/" + 
	            "home page.htm?item=1233;html=<h1>Heading</h1>#stuff";
            Console.WriteLine($"Original Uri string: {uriString}");
            Console.WriteLine();

            string escapedUriString = Uri.EscapeUriString(uriString);
            Console.WriteLine($"Escaped Uri string: {escapedUriString}");
            Console.WriteLine();

            // Why not just use EscapeDataString to escape a Uri?  It's not picky enough...
            string escapedUriData = Uri.EscapeDataString(uriString);
            Console.WriteLine($"Escaped Uri data: {escapedUriData}");
            Console.WriteLine();

            Console.WriteLine(Uri.UnescapeDataString(escapedUriString));
		}
        #endregion

        #region "9.8 Checking out a web server's custom error pages
        public static void GetCustomErrorPageLocations()
        {
            try
            {
                // MetaEdit can be gotten here: http://support.microsoft.com/kb/q232068/
                // Find metabase properties here
                // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/iissdk/html/4930f687-3a39-4f7d-9bc7-56dbb013d2b5.asp

                // Sometimes the COM world is a rough place...
                UInt32 E_RPC_SERVER_UNAVAILABLE = 0x800706BA;

                try
                {
                    // This is a case sensitive entry in the metabase
                    // You'd think it was misspelled but you would be mistaken...
                    const string WebServerSchema = "IIsWebServer";

                    // set up to talk to the local IIS server
                    string server = "localhost";

                    // Create a dictionary entry for the IIS server with a fake
                    // user and password.  Credentials would have to be provided
                    // if you are running as a regular user
                    using (DirectoryEntry w3svc =
                        new DirectoryEntry($"IIS://{server}/w3svc",
                                "Domain/UserCode", "Password"))
                    {
                        foreach (DirectoryEntry site in w3svc?.Children)
                        {
                            if (site != null)
                            {
                                using (site)
                                {
                                    // check all web servers on this box
                                    if (site.SchemaClassName == WebServerSchema)
                                    {
                                        // get the metabase entry for this server
                                        string metabaseDir = $"/w3svc/{site.Name}/ROOT";

                                        if (site.Children != null)
                                        {
                                            // find the ROOT directory for each server
                                            foreach (DirectoryEntry root in site.Children)
                                            {
                                                using (root)
                                                {
                                                    // did we find the root dir for this site?
                                                    if (root?.Name.Equals("ROOT",
                                                            StringComparison.OrdinalIgnoreCase) ?? false)
                                                    {
                                                        // get the HttpErrors
                                                        if (root?.Properties.Contains("HttpErrors") == true)
                                                        {
                                                            // write them out
                                                            PropertyValueCollection httpErrors = root?.Properties["HttpErrors"];
                                                            for (int i = 0; i < httpErrors?.Count; i++)
                                                            {
                                                                //400,*,FILE,C:\WINDOWS\help\iisHelp\common\400.htm
                                                                string[] errorParts = httpErrors?[i].ToString().Split(',');
                                                                Console.WriteLine("Error Mapping Entry:");
                                                                Console.WriteLine($"\tHTTP error code: {errorParts[0]}");
                                                                Console.WriteLine($"\tHTTP sub-error code: {errorParts[1]}");
                                                                Console.WriteLine($"\tMessage Type: {errorParts[2]}");
                                                                Console.WriteLine($"\tPath to error HTML file: {errorParts[3]}");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (COMException e)
                {
                    // apparently it won't talk to us right now
                    // this could be set up in a loop to retry....
                    if (e.ErrorCode != (Int32)E_RPC_SERVER_UNAVAILABLE)
                    {
                        throw;
                    }
                }
            }
            catch (COMException ex)
            {
                Console.WriteLine("You may need to install IIS and the metabase based on the error for this test: " + ex.Message);
            }
        }

        public static void GetCustomErrorPageLocationsLinq()
        {
            // MetaEdit can be gotten here: http://support.microsoft.com/kb/q232068/
            // Find metabase properties here
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/iissdk/html/4930f687-3a39-4f7d-9bc7-56dbb013d2b5.asp

            try
            {
                // Sometimes the COM world is a rough place...
                UInt32 E_RPC_SERVER_UNAVAILABLE = 0x800706BA;

                try
                {
                    // This is a case sensitive entry in the metabase
                    // You'd think it was misspelled but you would be mistaken...
                    const string WebServerSchema = "IIsWebServer";

                    // set up to talk to the local IIS server
                    string server = "localhost";

                    // Create a dictionary entry for the IIS server with a fake
                    // user and password.  Credentials would have to be provided
                    // if you are running as a regular user
                    using (DirectoryEntry w3svc =
                        new DirectoryEntry($"IIS://{server}/w3svc",
                                "Domain/UserCode", "Password"))
                    {
                        // Break up the query using Explicit dot notation into getting the site, then the http error property values
                        //var sites = w3svc?.Children.OfType<DirectoryEntry>()
                        //            .Where(child => child.SchemaClassName == WebServerSchema)
                        //            .SelectMany(child => child.Children.OfType<DirectoryEntry>());
                        //var httpErrors = sites
                        //               .Where(site => site.Name == "ROOT")
                        //               .SelectMany<DirectoryEntry,string>(site => site.Properties["HttpErrors"].OfType<string>());

                        // Combine the query using Explicit dot notation
                        //var httpErrors = w3svc?.Children.OfType<DirectoryEntry>()
                        //                    .Where(site => site.SchemaClassName == WebServerSchema)
                        //                    .SelectMany(siteDir => siteDir.Children.OfType<DirectoryEntry>())
                        //                    .Where(siteDir => siteDir.Name == "ROOT")
                        //                    .SelectMany<DirectoryEntry, string>(siteDir => siteDir.Properties["HttpErrors"].OfType<string>());

                        // Use a regular query expression to
                        // select the http errors for all web sites on the machine
                        var httpErrors = from site in
                                                w3svc?.Children.OfType<DirectoryEntry>()
                                         where site.SchemaClassName == WebServerSchema
                                         from siteDir in
                                             site.Children.OfType<DirectoryEntry>()
                                         where siteDir.Name == "ROOT"
                                         from httpError in siteDir.Properties["HttpErrors"].OfType<string>()
                                         select httpError;

                        // use eager evaluation to convert this to the array
                        // so that we don't requery on each iteration.  We would miss
                        // updates to the metabase that occur during execution but
                        // that is a small price to pay vs. the requery cost.
                        // This will force the evaluation of the query now once.
                        string[] errors = httpErrors.ToArray();
                        foreach (var httpError in errors)
                        {
                            //400,*,FILE,C:\WINDOWS\help\iisHelp\common\400.htm
                            string[] errorParts = httpError.ToString().Split(',');
                            Console.WriteLine("Error Mapping Entry:");
                            Console.WriteLine($"\tHTTP error code: {errorParts[0]}");
                            Console.WriteLine($"\tHTTP sub-error code: {errorParts[1]}");
                            Console.WriteLine($"\tMessage Type: {errorParts[2]}");
                            Console.WriteLine($"\tPath to error HTML file: {errorParts[3]}");
                        }
                    }
                }
                catch (COMException e)
                {
                    // apparently it won't talk to us right now
                    // this could be set up in a loop to retry....
                    if (e.ErrorCode != (Int32)E_RPC_SERVER_UNAVAILABLE)
                    {
                        throw;
                    }
                }
            }
            catch(COMException ex)
            {
                Console.WriteLine("You may need to install IIS and the metabase based on the error for this test: " + ex.Message);
            }
        }

        #endregion

        #region "9.9 Writing a TCP Server"
        // See the ConsoleTCPServer project

        #endregion

        #region "9.10 Writing a TCP Client"
        // See the ConsoleTCPClient project
        #endregion

        #region "9.11 Simulating Form Execution"
        public static async Task SimulatingFormExecution()
        {
            // In order to use this, you need to run the CSCBWeb project first.
            Uri uri = new Uri("http://localhost:4088/WebForm1.aspx");
            WebClient client = new WebClient();

            // Create a series of name/value pairs to send
            // Add necessary parameter/value pairs to the name/value container.
            NameValueCollection collection = new NameValueCollection()
                { {"Item", "WebParts"},
                    {"Identity", "foo@bar.com"},
                    {"Quantity", "5"} };

            Console.WriteLine(
                $"Uploading name/value pairs to URI {uri.AbsoluteUri} ...");

            // Upload the NameValueCollection.
            byte[] responseArray =
                await client.UploadValuesTaskAsync(uri, "POST", collection);

            // Decode and display the response.
            Console.WriteLine(
                $"\nResponse received was {Encoding.UTF8.GetString(responseArray)}");

        }
        #endregion

        #region "9.12 Transferring Data via HTTP"	
        public static async Task DownloadingDataFromServerAsync()
        {
            Uri uri = new Uri("http://localhost:4088/DownloadData.aspx");

            // make a client
            using (WebClient client = new WebClient())
            {
                // get the contents of the file
                Console.WriteLine($"Downloading {uri.AbsoluteUri}");
                // download the page and store the bytes
                byte[] bytes;
                try
                {
                    // NOTE: There is also a DownloadDataAsync which is used in the older
                    // EAP pattern which we do not use here.
                    bytes = await client.DownloadDataTaskAsync(uri);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                    return;
                }
                // Write the HTML out
                string page = Encoding.UTF8.GetString(bytes);
                Console.WriteLine(page);

                // go get the file
                Console.WriteLine($"Retrieving file from {uri}...{Environment.NewLine}");
                // get file and put it in a temp file
                string tempFile = Path.GetTempFileName();
                try
                {
                    // NOTE: There is also a DownloadFileAsync which is used in the older
                    // EAP pattern which we do not use here.
                    await client.DownloadFileTaskAsync(uri, tempFile);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                    return;
                }
                Console.WriteLine($"Downloaded {uri} to {tempFile}");
            }
        }

        public static async Task UploadingDataToServerAsync()
        {
            Uri uri = new Uri("http://localhost:4088/UploadData.aspx");
            // make a client
            using (WebClient client = new WebClient())
            {
                Console.WriteLine($"Uploading to {uri.AbsoluteUri}");
                try
                {
                    // NOTE: There is also a UploadFileAsync which is used in the older
                    // EAP pattern which we do not use here.
                    await client.UploadFileTaskAsync(uri, "SampleClassLibrary.dll");
                    Console.WriteLine($"Uploaded successfully to {uri.AbsoluteUri}");
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                }
            }
        }
        #endregion

        #region "9.13 Using Named Pipes to Communicate"
        // See the NamedPipeClientConsole and NamedPipeServerConsole projects
        #endregion

        #region  "9.14 Pinging programmatically"
        public static async Task TestPing()
        {
            System.Net.NetworkInformation.Ping pinger =
                new System.Net.NetworkInformation.Ping();
            PingReply reply = await pinger.SendPingAsync("www.oreilly.com");
            DisplayPingReplyInfo(reply);

            pinger.PingCompleted += pinger_PingCompleted;
            pinger.SendAsync("www.oreilly.com", "oreilly ping");

        }

        private static void DisplayPingReplyInfo(PingReply reply)
        {
            Console.WriteLine("Results from pinging " + reply.Address);
            Console.WriteLine($"\tFragmentation allowed?: {!reply.Options.DontFragment}");
            Console.WriteLine($"\tTime to live: {reply.Options.Ttl}");
            Console.WriteLine($"\tRoundtrip took: {reply.RoundtripTime}");
            Console.WriteLine($"\tStatus: {reply.Status.ToString()}");
        }

        private static void pinger_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;
            DisplayPingReplyInfo(reply);

            if (e.Cancelled)
                Console.WriteLine($"Ping for {e.UserState.ToString()} was cancelled");
            else 
                Console.WriteLine($"Exception thrown during ping: {e.Error?.ToString()}");
        }

        #endregion

        #region  "9.15 Send SMTP mail using the SMTP service"
        public static async Task TestSendMailAsync()
        {
            try
            {
                // send a message with attachments
                string from = "authors@oreilly.com";
                string to = "authors@oreilly.com";
                MailMessage attachmentMessage = new MailMessage(from, to);
                attachmentMessage.Subject = "Hi there!";
                attachmentMessage.Body = "Check out this cool code!";
                // many systems filter out HTML mail that is relayed
                attachmentMessage.IsBodyHtml = false;
                // set up the attachment
                string pathToCode = @"..\..\09_NetworkingAndWeb.cs";
                Attachment attachment =
                    new Attachment(pathToCode,
                        MediaTypeNames.Application.Octet);
                attachmentMessage.Attachments.Add(attachment);

                // If you have one, you can bounce this off the local SMTP service.  
                // The local SMTP service needs to have relaying set up to go through 
                // a real email server like you used to be able to do in IIS6...
                //SmtpClient client = new SmtpClient("localhost");

                // Since we live a more security conscious time, we would provide the
                // correct parameters to connect to the SMTP server with the host name, 
                // port, SSL enabled and your credentials.  
                // NOTE: If you don't replace the current values you will get a
                // XXX exception like this:
                // System.Net.Mail.SmtpException: The SMTP host was not found. --->
                //    System.Net.WebException: The remote name could not be resolved:'YOURSMTPSERVERHERE'
                using (SmtpClient client = new SmtpClient("YOURSMTPSERVERHERE", 999))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("YOURSMTPUSERNAME", "YOURSMTPPASSWORD");
                    await client.SendMailAsync(attachmentMessage);
                    // or just send text
                    MailMessage textMessage = new MailMessage("authors@oreilly.com",
                                        "authors@oreilly.com",
                                        "Me again",
                                        "You need therapy, talking to yourself is one thing but writing code to send email is a whole other thing...");
                    await client.SendMailAsync(textMessage);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Console.WriteLine(e);
            }
        }
        #endregion

        #region  "9.16 Use Sockets to Scan the Ports on a Machine"
        public static async Task TestPortScanner()
        {
            // do a specific range
            Console.WriteLine("Checking ports 75-85 on localhost...");
            CheapoPortScanner cps = 
                new CheapoPortScanner("127.0.0.1", 75, 85);
            var progress = new Progress<CheapoPortScanner.PortScanResult>();
            progress.ProgressChanged += (sender, args) =>
            {
                Console.WriteLine(
                    $"Port {args.PortNum} is " +
                    $"{(args.IsPortOpen ? "open" : "closed")}");
            };
            await cps.ScanAsync(progress);
            cps.LastPortScanSummary();

            // do the local machine, whole port range 1-65535
            //cps = new CheapoPortScanner();
            //await cps.Scan(progress);
            //cps.LastPortScanSummary();
        }


        public class CheapoPortScanner
        {
            #region Private consts and members
            private const int PORT_MIN_VALUE = 1;
            private const int PORT_MAX_VALUE = 65535;
            private List<int> _openPorts;
            private List<int> _closedPorts;
            #endregion

            #region Properties
            public ReadOnlyCollection<int> OpenPorts => new ReadOnlyCollection<int>(_openPorts);
            public ReadOnlyCollection<int> ClosedPorts => new ReadOnlyCollection<int>(_closedPorts);

            public int MinPort { get; } = PORT_MIN_VALUE;
            public int MaxPort { get; } = PORT_MAX_VALUE;
            public string Host { get; } = "127.0.0.1"; // localhost

            #endregion // Properties
            #region CTORs & Init code
            public CheapoPortScanner()
            {
                // defaults are already set for ports & localhost
                SetupLists();
            }

            public CheapoPortScanner(string host, int minPort, int maxPort)
            {
                if (minPort > maxPort)
                    throw new ArgumentException("Min port cannot be greater than max port");
                if (minPort < PORT_MIN_VALUE || minPort > PORT_MAX_VALUE)
                    throw new ArgumentOutOfRangeException(
                        $"Min port cannot be less than {PORT_MIN_VALUE} " +
                        $"or greater than {PORT_MAX_VALUE}");
                if (maxPort < PORT_MIN_VALUE || maxPort > PORT_MAX_VALUE)
                    throw new ArgumentOutOfRangeException(
                        $"Max port cannot be less than {PORT_MIN_VALUE} " +
                        $"or greater than {PORT_MAX_VALUE}");

                this.Host = host;
                this.MinPort = minPort;
                this.MaxPort = maxPort;
                SetupLists();
            }

            private void SetupLists()
            {
                // set up lists with capacity to hold half of range
                // since we can't know how many ports are going to be open
                // so we compromise and allocate enough for half

                // rangeCount is max - min + 1
                int rangeCount = (this.MaxPort - this.MinPort) + 1;
                // if there are an odd number, bump by one to get one extra slot
                if (rangeCount % 2 != 0)
                    rangeCount += 1;
                // reserve half the ports in the range for each
                _openPorts = new List<int>(rangeCount / 2);
                _closedPorts = new List<int>(rangeCount / 2);
            }
            #endregion // CTORs & Init code

            #region Progress Result 
            public class PortScanResult
            {
                public int PortNum { get; set; }

                public bool IsPortOpen { get; set; }
            }

            #endregion // Progress Result

            #region Private Methods
            private async Task CheckPortAsync(int port, IProgress<PortScanResult> progress)
            {
                if (await IsPortOpenAsync(port))
                {
                    // if we got here it is open
                    _openPorts.Add(port);

                    // notify anyone paying attention
                    progress?.Report(
                        new PortScanResult() { PortNum = port, IsPortOpen = true });
                }
                else
                {
                    // server doesn't have that port open
                    _closedPorts.Add(port);
                    progress?.Report(
                        new PortScanResult() { PortNum = port, IsPortOpen = false });
                }
            }

            private async Task<bool> IsPortOpenAsync(int port)
            {
                Socket sock = null;
                try
                {
                    // make a TCP based socket
                    sock = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);
                    // connect 
                    await Task.Run(() => sock.Connect(this.Host, port));
                    return true;
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionRefused)
                        return false;
                    else
                    {
                        //An error occurred when attempting to access the socket. 
                        Debug.WriteLine(se.ToString());
                        Console.WriteLine(se.ToString());
                    }
                }
                finally
                {
                    if (sock?.Connected ?? false)
                        sock?.Disconnect(false);
                    sock?.Close();
                }
                return false;
            }
            #endregion

            #region Public Methods
            public async Task ScanAsync(IProgress<PortScanResult> progress)
            {
                for (int port = this.MinPort; port <= this.MaxPort; port++)
                    await CheckPortAsync(port, progress);
            }

            public void LastPortScanSummary()
            {
                Console.WriteLine($"Port Scan for host at {this.Host}");
                Console.WriteLine($"\tStarting Port: {this.MinPort}");
                Console.WriteLine($"\tEnding Port: {this.MaxPort}");
                Console.WriteLine($"\tOpen ports: {string.Join(",", _openPorts)}");
                Console.WriteLine($"\tClosed ports: {string.Join(",", _closedPorts)}");
            }

            #endregion // Public Methods
        }

        #endregion

        #region "9.17 Use the Current Internet Connection Settings"
        public static void GetInternetSettings()
        {
            try
            {
                Console.WriteLine("");
                Console.WriteLine("Reading current internet connection settings");
                InternetSettingsReader isr = new InternetSettingsReader();
                Console.WriteLine($"Current Proxy Address: {isr.ProxyAddress}");
                Console.WriteLine($"Current Proxy Port: {isr.ProxyPort}");
                Console.WriteLine($"Current ByPass Local Address setting: {isr.BypassLocalAddresses}");
                Console.WriteLine($"Exception addresses for proxy (bypass):");
                string exceptions;
                if (isr.ProxyExceptions?.Count > 0)
                    exceptions = "\t" + (string.Join(",", isr.ProxyExceptions?.ToArray()));
                else
                    exceptions = "\tNone";
                Console.WriteLine(exceptions);
                Console.WriteLine($"Proxy connection type: {isr.ConnectionType.ToString()}");
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]

        #region WinInet structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetPerConnOptionList
        {
            public int dwSize;                  // size of the INTERNET_PER_CONN_OPTION_LIST struct
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
            public IntPtr szConnection;			// connection name to set/query options
            public int dwOptionCount;			// number of options to set/query
            public int dwOptionError;               // on error, which option failed
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
            public IntPtr options;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetConnectionOption
        {
            static int Size { get; } = Marshal.SizeOf(typeof(InternetConnectionOption));
            public PerConnOption m_Option;
            public InternetConnectionOptionValue m_Value;

            // Nested Types
            [StructLayout(LayoutKind.Explicit)]
            public struct InternetConnectionOptionValue
            {
                // Fields
                [FieldOffset(0)]
                public System.Runtime.InteropServices.ComTypes.FILETIME m_FileTime;
                [FieldOffset(0)]
                public int m_Int;
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
                [FieldOffset(0)]
                public IntPtr m_StringPtr;
            }
        }
        #endregion

        #region WinInet enums
        //
        // options manifests for Internet{Query|Set}Option
        //
        //public enum InternetOption
        //{
        //    INTERNET_OPTION_CALLBACK = 1,
        //    INTERNET_OPTION_CONNECT_TIMEOUT = 2,
        //    INTERNET_OPTION_CONNECT_RETRIES = 3,
        //    INTERNET_OPTION_CONNECT_BACKOFF = 4,
        //    INTERNET_OPTION_SEND_TIMEOUT = 5,
        //    INTERNET_OPTION_CONTROL_SEND_TIMEOUT = INTERNET_OPTION_SEND_TIMEOUT,
        //    INTERNET_OPTION_RECEIVE_TIMEOUT = 6,
        //    INTERNET_OPTION_CONTROL_RECEIVE_TIMEOUT = INTERNET_OPTION_RECEIVE_TIMEOUT,
        //    INTERNET_OPTION_DATA_SEND_TIMEOUT = 7,
        //    INTERNET_OPTION_DATA_RECEIVE_TIMEOUT = 8,
        //    INTERNET_OPTION_HANDLE_TYPE = 9,
        //    INTERNET_OPTION_LISTEN_TIMEOUT = 11,
        //    INTERNET_OPTION_READ_BUFFER_SIZE = 12,
        //    INTERNET_OPTION_WRITE_BUFFER_SIZE = 13,

        //    INTERNET_OPTION_ASYNC_ID = 15,
        //    INTERNET_OPTION_ASYNC_PRIORITY = 16,

        //    INTERNET_OPTION_PARENT_HANDLE = 21,
        //    INTERNET_OPTION_KEEP_CONNECTION = 22,
        //    INTERNET_OPTION_REQUEST_FLAGS = 23,
        //    INTERNET_OPTION_EXTENDED_ERROR = 24,

        //    INTERNET_OPTION_OFFLINE_MODE = 26,
        //    INTERNET_OPTION_CACHE_STREAM_HANDLE = 27,
        //    INTERNET_OPTION_USERNAME = 28,
        //    INTERNET_OPTION_PASSWORD = 29,
        //    INTERNET_OPTION_ASYNC = 30,
        //    INTERNET_OPTION_SECURITY_FLAGS = 31,
        //    INTERNET_OPTION_SECURITY_CERTIFICATE_STRUCT = 32,
        //    INTERNET_OPTION_DATAFILE_NAME = 33,
        //    INTERNET_OPTION_URL = 34,
        //    INTERNET_OPTION_SECURITY_CERTIFICATE = 35,
        //    INTERNET_OPTION_SECURITY_KEY_BITNESS = 36,
        //    INTERNET_OPTION_REFRESH = 37,
        //    INTERNET_OPTION_PROXY = 38,
        //    INTERNET_OPTION_SETTINGS_CHANGED = 39,
        //    INTERNET_OPTION_VERSION = 40,
        //    INTERNET_OPTION_USER_AGENT = 41,
        //    INTERNET_OPTION_END_BROWSER_SESSION = 42,
        //    INTERNET_OPTION_PROXY_USERNAME = 43,
        //    INTERNET_OPTION_PROXY_PASSWORD = 44,
        //    INTERNET_OPTION_CONTEXT_VALUE = 45,
        //    INTERNET_OPTION_CONNECT_LIMIT = 46,
        //    INTERNET_OPTION_SECURITY_SELECT_CLIENT_CERT = 47,
        //    INTERNET_OPTION_POLICY = 48,
        //    INTERNET_OPTION_DISCONNECTED_TIMEOUT = 49,
        //    INTERNET_OPTION_CONNECTED_STATE = 50,
        //    INTERNET_OPTION_IDLE_STATE = 51,
        //    INTERNET_OPTION_OFFLINE_SEMANTICS = 52,
        //    INTERNET_OPTION_SECONDARY_CACHE_KEY = 53,
        //    INTERNET_OPTION_CALLBACK_FILTER = 54,
        //    INTERNET_OPTION_CONNECT_TIME = 55,
        //    INTERNET_OPTION_SEND_THROUGHPUT = 56,
        //    INTERNET_OPTION_RECEIVE_THROUGHPUT = 57,
        //    INTERNET_OPTION_REQUEST_PRIORITY = 58,
        //    INTERNET_OPTION_HTTP_VERSION = 59,
        //    INTERNET_OPTION_RESET_URLCACHE_SESSION = 60,
        //    INTERNET_OPTION_ERROR_MASK = 62,
        //    INTERNET_OPTION_FROM_CACHE_TIMEOUT = 63,
        //    INTERNET_OPTION_BYPASS_EDITED_ENTRY = 64,
        //    INTERNET_OPTION_DIAGNOSTIC_SOCKET_INFO = 67,
        //    INTERNET_OPTION_CODEPAGE = 68,
        //    INTERNET_OPTION_CACHE_TIMESTAMPS = 69,
        //    INTERNET_OPTION_DISABLE_AUTODIAL = 70,
        //    INTERNET_OPTION_MAX_CONNS_PER_SERVER = 73,
        //    INTERNET_OPTION_MAX_CONNS_PER_1_0_SERVER = 74,
        //    INTERNET_OPTION_PER_CONNECTION_OPTION = 75
        //    INTERNET_OPTION_DIGEST_AUTH_UNLOAD = 76,
        //    INTERNET_OPTION_IGNORE_OFFLINE = 77,
        //    INTERNET_OPTION_IDENTITY = 78,
        //    INTERNET_OPTION_REMOVE_IDENTITY = 79,
        //    INTERNET_OPTION_ALTER_IDENTITY = 80,
        //    INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81,
        //    INTERNET_OPTION_AUTODIAL_MODE = 82,
        //    INTERNET_OPTION_AUTODIAL_CONNECTION = 83,
        //    INTERNET_OPTION_CLIENT_CERT_CONTEXT = 84,
        //    INTERNET_OPTION_AUTH_FLAGS = 85,
        //    INTERNET_OPTION_COOKIES_3RD_PARTY = 86,
        //    INTERNET_OPTION_DISABLE_PASSPORT_AUTH = 87,
        //    INTERNET_OPTION_SEND_UTF8_SERVERNAME_TO_PROXY = 88,
        //    INTERNET_OPTION_EXEMPT_CONNECTION_LIMIT = 89,
        //    INTERNET_OPTION_ENABLE_PASSPORT_AUTH = 90,

        //    INTERNET_OPTION_HIBERNATE_INACTIVE_WORKER_THREADS = 91,
        //    INTERNET_OPTION_ACTIVATE_WORKER_THREADS = 92,
        //    INTERNET_OPTION_RESTORE_WORKER_THREAD_DEFAULTS = 93,
        //    INTERNET_OPTION_SOCKET_SEND_BUFFER_LENGTH = 94,
        //    INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95,

        //    INTERNET_OPTION_DATAFILE_EXT = 96,

        //    INTERNET_FIRST_OPTION = INTERNET_OPTION_CALLBACK,
        //    INTERNET_LAST_OPTION = INTERNET_OPTION_DATAFILE_EXT
        //}

        //
        // Options used in INTERNET_PER_CONN_OPTON struct
        //
        public enum PerConnOption
        {
            INTERNET_PER_CONN_FLAGS = 1, // Sets or retrieves the connection type. The Value member will contain one or more of the values from PerConnFlags 
            INTERNET_PER_CONN_PROXY_SERVER = 2, // Sets or retrieves a string containing the proxy servers.  
            INTERNET_PER_CONN_PROXY_BYPASS = 3, // Sets or retrieves a string containing the URLs that do not use the proxy server.  
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4//, // Sets or retrieves a string containing the URL to the automatic configuration script.  
            //INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5, // Sets or retrieves the automatic discovery settings. The Value member will contain one or more of the values from PerConnAutoDiscoveryFlags 
            //INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6, // Chained autoconfig URL. Used when the primary autoconfig URL points to an INS file that sets a second autoconfig URL for proxy information.  
            //INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7, // Minutes until automatic refresh of autoconfig URL by autodiscovery.  
            //INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8, // Read only option. Returns the time the last known good autoconfig URL was found using autodiscovery.  
            //INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9  // Read only option. Returns the last known good URL found using autodiscovery.  
        }

        //
        // PER_CONN_FLAGS
        //
        [Flags]
        public enum PerConnFlags
        {
            PROXY_TYPE_DIRECT = 0x00000001,  // direct to net
            PROXY_TYPE_PROXY = 0x00000002,  // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,  // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }

        ////
        //// PER_CONN_AUTODISCOVERY_FLAGS
        ////
        //[Flags]
        //public enum PerConnAutoDiscoveryFlags
        //{
        //    AUTO_PROXY_FLAG_USER_SET = 0x00000001,   // user changed this setting
        //    AUTO_PROXY_FLAG_ALWAYS_DETECT = 0x00000002,   // force detection even when its not needed
        //    AUTO_PROXY_FLAG_DETECTION_RUN = 0x00000004,   // detection has been run
        //    AUTO_PROXY_FLAG_MIGRATED = 0x00000008,   // migration has just been done
        //    AUTO_PROXY_FLAG_DONT_CACHE_PROXY_RESULT = 0x00000010,   // don't cache result of host=proxy name
        //    AUTO_PROXY_FLAG_CACHE_INIT_RUN = 0x00000020,   // don't initalize and run unless URL expired
        //    AUTO_PROXY_FLAG_DETECTION_SUSPECT = 0x00000040   // if we're on a LAN & Modem, with only one IP, bad?!?
        //}
        #endregion

        private static class NativeMethods
        {
            #region P/Invoke defs
            [DllImport("WinInet.dll", EntryPoint = "InternetQueryOptionW", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool InternetQueryOption(
                IntPtr hInternet,
                int dwOption,
                ref InternetPerConnOptionList optionsList,
                ref int bufferLength
                );
            #endregion
        }

        public class InternetSettingsReader
        {
            #region Private Members
            string _proxyAddr;
            int _proxyPort = -1;
            bool _bypassLocal;
            string _autoConfigAddr;
            List<string> _proxyExceptions;
            PerConnFlags _flags;
            #endregion

            #region CTOR
            public InternetSettingsReader()
            {
            }
            #endregion

            #region Properties
            public string ProxyAddress
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_SERVER);
                    // parse out the addr and port
                    string proxyInfo = Marshal.PtrToStringUni(
                                            ico.m_Value.m_StringPtr);
                    ParseProxyInfo(proxyInfo);
                    return _proxyAddr;
                }
            }
            public int ProxyPort
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_SERVER);
                    // parse out the addr and port
                    string proxyInfo = Marshal.PtrToStringUni(
                                            ico.m_Value.m_StringPtr);
                    ParseProxyInfo(proxyInfo);
                    return _proxyPort;
                }
            }
            public bool BypassLocalAddresses
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS);
                    // bypass is listed as <local> in the exceptions list
                    string exceptions =
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);

                    if (exceptions.IndexOf("<local>") != -1)
                        _bypassLocal = true;
                    else
                        _bypassLocal = false;
                    return _bypassLocal;
                }
            }
            public string AutoConfigurationAddress
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL);
                    // get these straight
                    _autoConfigAddr =
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);
                    if (_autoConfigAddr == null)
                        _autoConfigAddr = "";
                    return _autoConfigAddr;
                }
            }
            public IList<string> ProxyExceptions
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS);
                    // exceptions are seperated by semi colon
                    string exceptions =
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);
                    if (!string.IsNullOrEmpty(exceptions))
                    {
                        _proxyExceptions = new List<string>(exceptions.Split(';'));
                    }
                    return _proxyExceptions;
                }
            }
            public PerConnFlags ConnectionType
            {
                get
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_FLAGS);
                    _flags = (PerConnFlags)ico.m_Value.m_Int;

                    return _flags;
                }
            }

            #endregion

            #region Private Methods
            private void ParseProxyInfo(string proxyInfo)
            {
                if (!string.IsNullOrEmpty(proxyInfo))
                {
                    string[] parts = proxyInfo.Split(':');
                    if (parts.Length == 2)
                    {
                        _proxyAddr = parts[0];
                        try
                        {
                            _proxyPort = Convert.ToInt32(parts[1]);
                        }
                        catch (FormatException)
                        {
                            // no port
                            _proxyPort = -1;
                        }
                    }
                    else
                    {
                        _proxyAddr = parts[0];
                        _proxyPort = -1;
                    }
                }
            }

            private static InternetConnectionOption GetInternetConnectionOption(PerConnOption pco)
            {
                //Allocate the list and option.
                InternetPerConnOptionList perConnOptList = new InternetPerConnOptionList();
                InternetConnectionOption ico = new InternetConnectionOption();
                //pin the option structure
                GCHandle gch = GCHandle.Alloc(ico, GCHandleType.Pinned);
                //initialize the option for the data we want
                ico.m_Option = pco;
                //Initialize the option list for the default connection or LAN.
                int listSize = Marshal.SizeOf(perConnOptList);
                perConnOptList.dwSize = listSize;
                perConnOptList.szConnection = IntPtr.Zero;
                perConnOptList.dwOptionCount = 1;
                perConnOptList.dwOptionError = 0;
                // figure out sizes & offsets
                int icoSize = Marshal.SizeOf(ico);
                // alloc enough memory for the option (native memory not .NET heap)
                perConnOptList.options =
                    Marshal.AllocCoTaskMem(icoSize);

                // Make pointer from the structure
                IntPtr optionListPtr = perConnOptList.options;
                Marshal.StructureToPtr(ico, optionListPtr, false);

                //Make the query
                if (NativeMethods.InternetQueryOption(
                    IntPtr.Zero,
                    75, //(int)InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
                    ref perConnOptList,
                    ref listSize) == true)
                {
                    //retrieve the value
                    ico =
                        (InternetConnectionOption)Marshal.PtrToStructure(perConnOptList.options,
                                                typeof(InternetConnectionOption));
                }
                // free the COM memory
                Marshal.FreeCoTaskMem(perConnOptList.options);
                //unpin the structs
                gch.Free();

                return ico;
            }
            #endregion
        }

        #endregion

        #region "9.18 Transferring Files Using FTP"
        public static async Task TestFtpAsync()
        {
            Uri downloadFtpSite =
                new Uri("ftp://ftp.oreilly.com/pub/examples/csharpckbk/CSharpCookbook.zip");
            string targetPath = "CSharpCookbook.zip";
            await NetworkingAndWeb.FtpDownloadAsync(downloadFtpSite, targetPath);
            string uploadFile = "SampleClassLibrary.dll";
            Uri uploadFtpSite =
                new Uri($"ftp://localhost/{uploadFile}");
            await NetworkingAndWeb.FtpUploadAsync(uploadFtpSite, uploadFile);
        }
        public static async Task FtpDownloadAsync(Uri ftpSite, string targetPath)
        {
            try
            {
                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(
                    ftpSite);

                request.Credentials = new NetworkCredential("anonymous", "authors@oreilly.com");
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Stream data = response.GetResponseStream();
                    File.Delete(targetPath);
                    Console.WriteLine($"Downloading {ftpSite.AbsoluteUri} to {targetPath}...");

                    byte[] byteBuffer = new byte[4096];
                    using (FileStream output = new FileStream(targetPath, FileMode.CreateNew, 
                        FileAccess.ReadWrite,FileShare.ReadWrite, 4096, useAsync: true))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead > 0)
                                await output.WriteAsync(byteBuffer, 0, bytesRead);
                        }
                        while (bytesRead > 0);
                    }
                    Console.WriteLine($"Downloaded {ftpSite.AbsoluteUri} to {targetPath}");
                }
            }
            catch (WebException e)
            {
                Console.WriteLine($"Failed to download {ftpSite.AbsoluteUri} to {targetPath}");
                Console.WriteLine(e);
            }
        }

        public static async Task FtpUploadAsync(Uri ftpSite, string uploadFile)
        {
            Console.WriteLine($"Uploading {uploadFile} to {ftpSite.AbsoluteUri}...");
            try
            {
                FileInfo fileInfo = new FileInfo(uploadFile);
                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(
                    ftpSite);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                //if working with text files and going across operating system platforms, 
                //you might want to set this value to false to avoid line ending problems
                request.UseBinary = true;
                request.ContentLength = fileInfo.Length;
                request.Credentials = new NetworkCredential("anonymous", "authors@oreilly.com");
                byte[] byteBuffer = new byte[4096];
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    using (FileStream fileStream = 
                        new FileStream(uploadFile, FileMode.Open, FileAccess.Read, 
                        FileShare.Read, 4096, useAsync: true))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = await fileStream.ReadAsync(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead > 0)
                                await requestStream.WriteAsync(byteBuffer, 0, bytesRead);
                        }
                        while (bytesRead > 0);
                    }
                }
                using (FtpWebResponse response = (FtpWebResponse) await request.GetResponseAsync())
                {
                    Console.WriteLine(response.StatusDescription);
                }
                Console.WriteLine($"Uploaded {uploadFile} to {ftpSite.AbsoluteUri}...");
            }
            catch (WebException e)
            {
                Console.WriteLine($"Failed to upload {uploadFile} to {ftpSite.AbsoluteUri}.");
                Console.WriteLine(((FtpWebResponse)e.Response).StatusDescription);
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}
