using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpRecipes
{
    public static class FileExtensions
    {
        #region FileSystemInfo methods
        public static string ToDisplayString(this FileSystemInfo fileSystemInfo)
        {
            string type = fileSystemInfo.GetType().ToString();
            if (fileSystemInfo is DirectoryInfo)
                type = "DIRECTORY";
            else if (fileSystemInfo is FileInfo)
                type = "FILE";
            return $"{type}: {fileSystemInfo.Name}";
        }
        #endregion // FileSystemInfo methods
    }

    public class FileSystemIO
    {
        #region "8.1 Searching for Directories or Files Using Wildcards"
        public static void SearchDirFileWildcards()
        {
            try
            {
                // set up initial paths
                string tempPath = Path.GetTempPath();
                string tempDir = tempPath + @"\MyTempDir";
                string tempDir2 = tempDir + @"\MyNestedTempDir";
                string tempDir3 = tempDir + @"\MyNestedTempDirPattern";
                string tempFile = tempDir + @"\MyTempFile.PDB";
                string tempFile2 = tempDir2 + @"\MyNestedTempFile.PDB";
                string tempFile3 = tempDir + @"\MyTempFile.TXT";

                // create temp dirs and files
                Directory.CreateDirectory(tempDir);
                Directory.CreateDirectory(tempDir2);
                Directory.CreateDirectory(tempDir3);
                FileStream stream = File.Create(tempFile);
                FileStream stream2 = File.Create(tempFile2);
                FileStream stream3 = File.Create(tempFile3);
                // close files before using
                stream.Close();
                stream2.Close();
                stream3.Close();

                // print out all of the items in the temp dir
                DisplayFilesAndSubDirectories(tempDir);

                // print out the items matching a pattern
                string pattern = "*.PDB";
                DisplayFilesWithPattern(tempDir, pattern);

                // print out the directories in the path
                DisplayDirectoriesFromInfo(tempDir);

                // print out directories matching a pattern
                pattern = "*Pattern*";
                DisplayDirectoriesWithPattern(tempDir, pattern);

                // print out all files
                DisplayFiles(tempDir);

                // print out all files matching a pattern
                pattern = "*.PDB";
                DisplayFilesWithGetFiles(tempDir, pattern);

                // Now use methods that return actual objects instead of just the string path

                // get the items
                DisplayDirectoryContents(tempDir);

                // get the items that match the pattern
                pattern = "*.PDB";
                DisplayDirectoryContentsWithPattern(tempDir, pattern);

                // get the directory infos
                DisplayDirectoriesFromInfo(tempDir);

                // get the directory infos that match a pattern
                pattern = "*Pattern*";
                DisplayDirectoriesWithPattern(tempDir, pattern);

                // get the file infos
                DisplayFilesFromInfo(tempDir);

                // get the file infos that match a pattern
                pattern = "*.PDB";
                DisplayFilesWithInstanceGetFiles(tempDir, pattern);

                // clean up temp stuff we looked at
                File.Delete(tempFile3);
                File.Delete(tempFile2);
                File.Delete(tempFile);
                Directory.Delete(tempDir3);
                Directory.Delete(tempDir2);
                Directory.Delete(tempDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void DisplayFilesAndSubDirectories(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string[] items = Directory.GetFileSystemEntries(path);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        public static void DisplaySubDirectories(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string[] items = Directory.GetDirectories(path);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        public static void DisplayFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string[] items = Directory.GetFiles(path);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        public static void DisplayDirectoryContents(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            var fileSystemDisplayInfos =
                (from fsi in mainDir.GetFileSystemInfos()
                 where fsi is FileSystemInfo || fsi is DirectoryInfo
                 select fsi.ToDisplayString()).ToArray();

            Array.ForEach(fileSystemDisplayInfos, s =>
            {
                Console.WriteLine(s);
            });
        }

        public static void DisplayDirectoriesFromInfo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            DirectoryInfo[] items = mainDir.GetDirectories();
            Array.ForEach(items, item =>
            {
                Console.WriteLine($"DIRECTORY: {item.Name}");
            });
        }

        public static void DisplayFilesFromInfo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            FileInfo[] items = mainDir.GetFiles();
            Array.ForEach(items, item =>
            {
                Console.WriteLine($"FILE: {item.Name}");
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayFilesWithPattern(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            string[] items = Directory.GetFileSystemEntries(path, pattern);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayDirectoriesWithPattern(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            string[] items = Directory.GetDirectories(path, pattern);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayFilesWithGetFiles(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            string[] items = Directory.GetFiles(path, pattern);
            Array.ForEach(items, item =>
            {
                Console.WriteLine(item);
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayDirectoryContentsWithPattern(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            var fileSystemDisplayInfos =
                (from fsi in mainDir.GetFileSystemInfos(pattern)
                 where fsi is FileSystemInfo || fsi is DirectoryInfo
                 select fsi.ToDisplayString()).ToArray();

            Array.ForEach(fileSystemDisplayInfos, s =>
            {
                Console.WriteLine(s);
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayDirectoriesWithPatternFromInfo(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            DirectoryInfo[] items = mainDir.GetDirectories(pattern);
            Array.ForEach(items, item =>
            {
                Console.WriteLine($"DIRECTORY: {item.Name}");
            });
        }

        // The pattern passed in is a string equal to "*.PDB"
        public static void DisplayFilesWithInstanceGetFiles(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            DirectoryInfo mainDir = new DirectoryInfo(path);
            FileInfo[] items = mainDir.GetFiles(pattern);
            Array.ForEach(items, item =>
            {
                Console.WriteLine($"FILE: {item.Name}");
            });
        }

        #endregion

        #region "8.2 Obtaining the Directory Tree"
        public static void ObtainDirTree()
        {
            try
            {
                // set up items to find...
                string tempPath = Path.GetTempPath();
                string tempDir1 = tempPath + @"\MyTempDir";
                string tempDir2 = tempDir1 + @"\Chapter 1 - The Beginning";
                string tempDir3 = tempDir2 + @"\Chapter 1a - The Rest Of The Beginning";
                string tempDir4 = tempDir2 + @"\IHaveAPDBFile";
                string tempFile1 = tempDir1 + @"\MyTempFile.PDB";
                string tempFile2 = tempDir2 + @"\MyNestedTempFile.PDB";
                string tempFile3 = tempDir3 + @"\Chapter 1 - MyDeepNestedTempFile.PDB";
                string tempFile4 = tempDir4 + @"\PDBFile.PDB";
                // Tree looks like this
                // TEMP\
                //		MyTempDir\
                //				Chapter 1 - The Beginning\
                //								Chapter 1a - The Rest Of The Beginning\
                //								IHaveAPDBFile
                //

                // create temp dirs and files
                Directory.CreateDirectory(tempDir1);
                Directory.CreateDirectory(tempDir2);
                Directory.CreateDirectory(tempDir3);
                Directory.CreateDirectory(tempDir4);
                FileStream stream1 = File.Create(tempFile1);
                FileStream stream2 = File.Create(tempFile2);
                FileStream stream3 = File.Create(tempFile3);
                FileStream stream4 = File.Create(tempFile4);
                // close files before using
                stream1.Close();
                stream2.Close();
                stream3.Close();
                stream4.Close();

                //tempDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                // list all of the files without recursion
                Stopwatch watch1 = Stopwatch.StartNew();
                DisplayAllFilesAndDirectories(tempDir1);
                watch1.Stop();
                Console.WriteLine("*************************");

                // list all of the files using recursion
                Stopwatch watch2 = Stopwatch.StartNew();
                DisplayAllFilesAndDirectoriesWithRecursion(tempDir1);
                watch2.Stop();
                Console.WriteLine("*************************");
                Console.WriteLine($"Non-Recursive method time elapsed {watch1.Elapsed.ToString()}");
                Console.WriteLine($"Recursive method time elapsed {watch2.Elapsed.ToString()}");

                //// obtain a listing of all files with the extension of PDB  
                DisplayAllFilesWithExtension(tempDir1, ".PDB");

                // clean up temp stuff we looked at
                File.Delete(tempFile4);
                File.Delete(tempFile3);
                File.Delete(tempFile2);
                File.Delete(tempFile1);
                Directory.Delete(tempDir4);
                Directory.Delete(tempDir3);
                Directory.Delete(tempDir2);
                Directory.Delete(tempDir1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region Iterative (correct way)
        public static IEnumerable<FileSystemInfo> GetAllFilesAndDirectories(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            Stack<FileSystemInfo> stack = new Stack<FileSystemInfo>();

            stack.Push(dirInfo);
            while (dirInfo != null || stack.Count > 0)
            {
                FileSystemInfo fileSystemInfo = stack.Pop();
                DirectoryInfo subDirectoryInfo = fileSystemInfo as DirectoryInfo;
                if (subDirectoryInfo != null)
                {
                    yield return subDirectoryInfo;
                    foreach (FileSystemInfo fsi in subDirectoryInfo.GetFileSystemInfos())
                        stack.Push(fsi);
                    dirInfo = subDirectoryInfo;
                }
                else
                {
                    yield return fileSystemInfo;
                    dirInfo = null;
                }
            }
        }

        public static void DisplayAllFilesAndDirectories(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));

            var strings = (from fileSystemInfo in GetAllFilesAndDirectories(dir)
                           select fileSystemInfo.ToDisplayString()).ToArray();

            Array.ForEach(strings, s => { Console.WriteLine(s); });
        }

        public static void DisplayAllFilesWithExtension(string dir, string extension)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentNullException(nameof(extension));

            var strings = (from fileSystemInfo in GetAllFilesAndDirectories(dir)
                           where fileSystemInfo is FileInfo &&
                                 fileSystemInfo.FullName.Contains("Chapter 1") &&
                                 (string.Compare(fileSystemInfo.Extension, extension,
                                                 StringComparison.OrdinalIgnoreCase) == 0)
                           select fileSystemInfo.ToDisplayString()).ToArray();

            Array.ForEach(strings, s => { Console.WriteLine(s); });
        }


        #endregion // Iterative

        #region Recursive (incorrect way)
        public static IEnumerable<FileSystemInfo> GetAllFilesAndDirectoriesWithRecursion(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileSystemInfo[] fileSystemInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                yield return fileSystemInfo;
                if (fileSystemInfo is DirectoryInfo)
                {
                    foreach (FileSystemInfo fsi in GetAllFilesAndDirectoriesWithRecursion(fileSystemInfo.FullName))
                        yield return fsi;
                }
            }
        }

        public static void DisplayAllFilesAndDirectoriesWithRecursion(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));

            var strings = (from fileSystemInfo in GetAllFilesAndDirectoriesWithRecursion(dir)
                           select fileSystemInfo.ToDisplayString()).ToArray();

            Array.ForEach(strings, s => { Console.WriteLine(s); });
        }

        public static void DisplayAllFilesWithExtensionwithRecursion(string dir, string extension)
        {
            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentNullException(nameof(extension));

            var strings = (from fileSystemInfo in GetAllFilesAndDirectoriesWithRecursion(dir)
                           where fileSystemInfo is FileInfo &&
                                   fileSystemInfo.FullName.Contains("Chapter 1") &&
                                   (string.Compare(fileSystemInfo.Extension, extension,
                                                   StringComparison.OrdinalIgnoreCase) == 0)
                           select fileSystemInfo.ToDisplayString()).ToArray();

            Array.ForEach(strings, s => { Console.WriteLine(s); });
        }
        #endregion // Recursive


        public static void DisplayAllFilesAndDirectoriesWithoutRecursion(string dir)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectoriesWithoutRecursion(dir)
                            select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static void DisplayAllFilesWithExtensionWithoutRecursion(string dir, string extension)
        {
            var strings = from fileSystemInfo in GetAllFilesAndDirectoriesWithoutRecursion(dir)
                            where fileSystemInfo is FileInfo &&
                                fileSystemInfo.FullName.Contains("Chapter 1") &&
                                (string.Compare(fileSystemInfo.Extension, extension,
                                                StringComparison.OrdinalIgnoreCase) == 0)
                            select fileSystemInfo.ToDisplayString();

            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static IEnumerable<FileSystemInfo> GetAllFilesAndDirectoriesWithoutRecursion(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            Stack<FileSystemInfo> stack = new Stack<FileSystemInfo>();

            stack.Push(dirInfo);
            while (dirInfo != null || stack.Count > 0)
            {
                FileSystemInfo fileSystemInfo = stack.Pop();
                DirectoryInfo subDirectoryInfo = fileSystemInfo as DirectoryInfo;
                if (subDirectoryInfo != null)
                {
                    yield return subDirectoryInfo;
                    foreach (FileSystemInfo fsi in subDirectoryInfo.GetFileSystemInfos())
                        stack.Push(fsi);
                    dirInfo = subDirectoryInfo;
                }
                else
                {
                    yield return fileSystemInfo;
                    dirInfo = null;
                }
            }
        }

        #endregion

        #region "8.3 Parsing a Path"
        public static void ParsePath()
        {
            DisplayPathParts(@"c:\test\tempfile.txt");
        }

        public static void DisplayPathParts(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string root = Path.GetPathRoot(path);
            string dirName = Path.GetDirectoryName(path);
            string fullFileName = Path.GetFileName(path);
            string fileExt = Path.GetExtension(path);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
            StringBuilder format = new StringBuilder();
            format.Append($"ParsePath of {path} breaks up into the following pieces:{Environment.NewLine}");
            format.Append($"\tRoot: {root}{Environment.NewLine}");
            format.Append($"\tDirectory Name: {dirName}{Environment.NewLine}");
            format.Append($"\tFull File Name: {fullFileName}{Environment.NewLine}");
            format.Append($"\tFile Extension: {fileExt}{Environment.NewLine}");
            format.Append($"\tFile Name Without Extension: {fileNameWithoutExt}{Environment.NewLine}");
            Console.WriteLine(format.ToString());
        }

        #endregion

        #region "8.4 Launching and Interacting with Console Utilities "
        public static void LaunchInteractConsoleUtilities()
        {
            RunProcessToReadStdIn();
        }

        public static void RunProcessToReadStdIn()
        {
            Process application = new Process();
            // run the command shell
            application.StartInfo.FileName = @"cmd.exe";

            // turn on standard extensions
            application.StartInfo.Arguments = "/E:ON";
            application.StartInfo.RedirectStandardInput = true;
            application.StartInfo.UseShellExecute = false;

            // start it up
            application.Start();

            // get stdin
            StreamWriter input = application.StandardInput;

            // run the command to display the time
            input.WriteLine("TIME /T");

            // stop the application we launched
            input.WriteLine("exit");
        }

        #endregion

        #region "8.5 Locking Subsections of a File"
        public static async Task LockSubsectionsOfAFile()
        {
            string path = Path.GetTempFileName();
            await CreateLockedFileAsync(path);
            File.Delete(path);
        }

        public static async Task CreateLockedFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(fileName,
                        FileMode.Create,
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite, 4096, useAsync: true);

                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync("The First Line");
                    await writer.WriteLineAsync("The Second Line");
                    await writer.FlushAsync();

                    try
                    {
                        // Lock all of the file.
                        fileStream.Lock(0, fileStream.Length);

                        // Do some lengthy processing here…
                        Thread.Sleep(1000);
                    }
                    finally
                    {
                        // Make sure we unlock the file.
                        // If a process terminates with part of a file locked or closes a file
                        // that has outstanding locks, the behavior is undefined which is MS
                        // speak for bad things….
                        fileStream.Unlock(0, fileStream.Length);
                    }

                    await writer.WriteLineAsync("The Third Line");
                    fileStream = null;
                }
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }

        public static async Task CreateLockedFileWithExceptionAsync(string fileName)
        {

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(fileName,
                        FileMode.Create,
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite, 4096, useAsync: true);
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteLineAsync("The First Line");
                    await streamWriter.WriteLineAsync("The Second Line");
                    await streamWriter.FlushAsync();

                    // Lock all of the file.
                    fileStream.Lock(0, fileStream.Length);

                    FileStream writeFileStream = null;
                    try
                    {
                        writeFileStream = new FileStream(fileName,
                                                    FileMode.Open,
                                                    FileAccess.Write,
                                                    FileShare.ReadWrite, 4096, useAsync: true);
                        using (StreamWriter streamWriter2 = new StreamWriter(writeFileStream))
                        {
                            await streamWriter2.WriteAsync("foo ");
                            try
                            {
                                streamWriter2.Close(); // --> Exception occurs here!
                            }
                            catch
                            {
                                Console.WriteLine(
                                "The streamWriter2.Close call generated an exception.");
                            }
                            streamWriter.WriteLine("The Third Line");
                        }
                        writeFileStream = null;
                    }
                    finally
                    {
                        if (writeFileStream != null)
                            writeFileStream.Dispose();
                    }
                }
                fileStream = null;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }

        public static async Task CreateLockedFileWithUnlockAsync(string fileName)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(fileName,
                                            FileMode.Create,
                                            FileAccess.ReadWrite,
                                            FileShare.ReadWrite, 4096, useAsync: true);
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteLineAsync("The First Line");
                    await streamWriter.WriteLineAsync("The Second Line");
                    await streamWriter.FlushAsync();

                    // Lock all of the file. 	            
                    fileStream.Lock(0, fileStream.Length);

                    // Try to access the locked file...
                    FileStream writeFileStream = null;
                    try
                    {
                        writeFileStream = new FileStream(fileName,
                                                    FileMode.Open,
                                                    FileAccess.Write,
                                                    FileShare.ReadWrite, 4096, useAsync: true);
                        using (StreamWriter streamWriter2 = new StreamWriter(writeFileStream))
                        {
                            await streamWriter2.WriteAsync("foo");
                            fileStream.Unlock(0, fileStream.Length);
                            await streamWriter2.FlushAsync();
                        }
                        writeFileStream = null;
                    }
                    finally
                    {
                        if (writeFileStream != null)
                            writeFileStream.Dispose();
                    }
                }
                fileStream = null;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }



        #endregion

        #region "8.6 Waiting for an Action to Occur in the File System"
        public static void WaitFileSystemAction()
		{
			string tempPath = Path.GetTempPath();
			string fileName = "temp.zip";
            string fullPath = tempPath + fileName;
			File.Delete(fullPath);
			
			WaitForZipCreation(tempPath,fileName);
		}

        public static void WaitForZipCreation(string path, string fileName)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            FileSystemWatcher fsw = null;
	        try
	        {
		        fsw = new FileSystemWatcher();
		        string [] data = new string[] {path,fileName};
		        fsw.Path = path;
		        fsw.Filter = fileName;
		        fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
			        | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Run the code to generate the file we are looking for
                // Normally you wouldn’t do this as another source is creating
                // this file
                Task work = Task.Run(() =>
                {
                    try
                    {
                        // wait a sec...
                        Thread.Sleep(1000);
                        // create a file in the temp directory
                        if (data.Length == 2)
                        {
                            string dataPath = data[0];
                            string dataFile = path + data[1];
                            Console.WriteLine($"Creating {dataFile} in task...");
                            FileStream fileStream = File.Create(dataFile);
                            fileStream.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                });

                // Don't await the work task finish as we detect that
                // through the FileSystemWatcher
                WaitForChangedResult result =
                    fsw.WaitForChanged(WatcherChangeTypes.Created);
                Console.WriteLine($"{result.Name} created at {path}.");
	        }
	        catch(Exception e)
	        {
		        Console.WriteLine(e.ToString());
	        }
            finally
            {
                // clean it up
                File.Delete(fileName);
                fsw?.Dispose();
            }
        }

		#endregion

		#region "8.7 Comparing Version Information of Two Executable Modules"
		public static void CompareVersionInfo()
		{
			// Version comparison
			string file1 = Path.GetTempFileName();
			string file2 = Path.GetTempFileName();
			FileSystemIO.FileComparison result = 
				FileSystemIO.CompareFileVersions(file1,file2);
			// cleanup
			File.Delete(file1);
			File.Delete(file2);
		}

        public enum FileComparison
        {
	        Error = 0,
	        Newer = 1,
	        Older = 2,
	        Same = 3
        }


        private static FileComparison ComparePart(int p1, int p2) => 
            p1 > p2 ? FileComparison.Newer :
                (p1 < p2 ? FileComparison.Older : FileComparison.Same);

        public static FileComparison CompareFileVersions(string file1, string file2)
        {
            if (string.IsNullOrWhiteSpace(file1))
                throw new ArgumentNullException(nameof(file1));
            if (string.IsNullOrWhiteSpace(file2))
                throw new ArgumentNullException(nameof(file2));

            FileComparison retValue = FileComparison.Error;
	        // get the version information
	        FileVersionInfo file1Version = FileVersionInfo.GetVersionInfo(file1);
	        FileVersionInfo file2Version = FileVersionInfo.GetVersionInfo(file2);

            retValue = ComparePart(file1Version.FileMajorPart, file2Version.FileMajorPart);
            if (retValue != FileComparison.Same)
            {
                retValue = ComparePart(file1Version.FileMinorPart, file2Version.FileMinorPart);
                if (retValue != FileComparison.Same)
                {
                    retValue = ComparePart(file1Version.FileBuildPart, file2Version.FileBuildPart);
                    if (retValue != FileComparison.Same)
                        retValue = ComparePart(file1Version.FilePrivatePart, 
                                file2Version.FilePrivatePart);
                }
            }
	        return retValue;
        }

		#endregion

		#region "8.8 Querying Information for All Drives on a System"
		public static void TestAllDriveInfo()
		{
			DisplayOneDrivesInfo();
			DisplayAllDriveInfo();
		}
		
		public static void DisplayOneDrivesInfo()
		{
            DriveInfo drive = new DriveInfo("D");
            if (drive.IsReady)
	            Console.WriteLine($"The space available on the D:\\ drive: {drive.AvailableFreeSpace}");
            else
	            Console.WriteLine("Drive D:\\ is not ready.");
	
			// If the drive is not ready you will get a:
			//   System.IO.IOException: The device is not ready.	
			Console.WriteLine();
		}
				
        public static void DisplayAllDriveInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Array.ForEach(drives, drive =>
            {
                if (drive.IsReady)
                {
                    Console.WriteLine($"Drive {drive.Name} is ready.");
                    Console.WriteLine($"AvailableFreeSpace: {drive.AvailableFreeSpace}");
                    Console.WriteLine($"DriveFormat: {drive.DriveFormat}");
                    Console.WriteLine($"DriveType: {drive.DriveType}");
                    Console.WriteLine($"Name: {drive.Name}");
                    Console.WriteLine($"RootDirectory.FullName: {drive.RootDirectory.FullName}");
                    Console.WriteLine($"TotalFreeSpace: {drive.TotalFreeSpace}");
                    Console.WriteLine($"TotalSize: {drive.TotalSize}");
                    Console.WriteLine($"VolumeLabel: {drive.VolumeLabel}");
                }
                else
                {
                    Console.WriteLine($"Drive {drive.Name} is not ready.");
                }
                Console.WriteLine();
            });
        }
		#endregion

		#region "8.9 Compressing and Decompressing Your Files"
        public static async void TestCompressNewFileAsync()
        {
	        byte[] data = new byte[10000000];
	        for (int i = 0; i < 10000000; i++)
		        data[i] = (byte)i;


            string normalFilePath = Path.GetTempPath() + "NewNormalFile.txt";
            string deflateCompressedFilePath = Path.GetTempPath() + "NewCompressedFile.txt";
            string deflateDecompressedFilePath = Path.GetTempPath() + "NewDecompressedFile.txt";
            string gzCompressedFilePath = Path.GetTempPath() + "NewGZCompressedFile.txt";
            string gzDecompressedFilePath = Path.GetTempPath() + "NewGZDecompressedFile.txt";

            Console.WriteLine($"Base file: {normalFilePath}");
            Console.WriteLine($"Deflate compressed file: {deflateCompressedFilePath}");
            Console.WriteLine($"Defalte decompressed file: {deflateDecompressedFilePath}");
            Console.WriteLine($"GZip compressed file: {gzCompressedFilePath}");
            Console.WriteLine($"GZip decompressed file: {gzDecompressedFilePath}");

            using (FileStream fs =
                new FileStream(normalFilePath,
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                    4096, useAsync:true))
            {
                await fs.WriteAsync(data, 0, data.Length);
            }

            await CompressFileAsync(normalFilePath, deflateCompressedFilePath, 
                CompressionType.Deflate);

            await DecompressFileAsync(deflateCompressedFilePath, deflateDecompressedFilePath,
                CompressionType.Deflate);

            await CompressFileAsync(normalFilePath, gzCompressedFilePath, 
                CompressionType.GZip);

            await DecompressFileAsync(gzCompressedFilePath, gzDecompressedFilePath,
                CompressionType.GZip);

            //Normal file size == 10,000,000 bytes
            //GZipped file size == 84,362
            //Deflated file size == 42,145
            //Pre .NET 4.5 GZipped file size == 155,204
            //Pre .NET 4.5 Deflated file size == 155,168

            // 36 bytes are related to the GZip CRC
        }

        /// <summary>
        /// Compress the source file to the destination file.
        /// This is done in 1MB chunks to not overwhelm the memory usage.
        /// </summary>
        /// <param name="sourceFile">the uncompressed file</param>
        /// <param name="destinationFile">the compressed file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static async Task CompressFileAsync(string sourceFile, 
                                        string destinationFile, 
                                        CompressionType compressionType)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
                throw new ArgumentNullException(nameof(sourceFile));

            if (string.IsNullOrWhiteSpace(destinationFile))
                throw new ArgumentNullException(nameof(destinationFile));

            FileStream streamSource = null;
            FileStream streamDestination = null;
            Stream streamCompressed = null;

            int bufferSize = 4096;
            using (streamSource = new FileStream(sourceFile,
                    FileMode.OpenOrCreate, FileAccess.Read, FileShare.None,
                    bufferSize, useAsync: true))
            {
                using (streamDestination = new FileStream(destinationFile,
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                    bufferSize, useAsync: true))
                {
                    // read 1MB chunks and compress them
                    long fileLength = streamSource.Length;

                    // write out the fileLength size
                    byte[] size = BitConverter.GetBytes(fileLength);
                    await streamDestination.WriteAsync(size, 0, size.Length);

                    long chunkSize = 1048576; // 1MB
                    while (fileLength > 0)
                    {
                        // read the chunk
                        byte[] data = new byte[chunkSize];
                        await streamSource.ReadAsync(data, 0, data.Length);

                        // compress the chunk
                        MemoryStream compressedDataStream =
                            new MemoryStream();

                        if (compressionType == CompressionType.Deflate)
                            streamCompressed =
                                new DeflateStream(compressedDataStream,
                                    CompressionMode.Compress);
                        else
                            streamCompressed =
                                new GZipStream(compressedDataStream,
                                    CompressionMode.Compress);

                        using (streamCompressed)
                        {
                            // write the chunk in the compressed stream
                            await streamCompressed.WriteAsync(data, 0, data.Length);
                            await streamCompressed.FlushAsync();
                        }
                        // get the bytes for the compressed chunk
                        byte[] compressedData =
                            compressedDataStream.GetBuffer();

                        // write out the chunk size
                        size = BitConverter.GetBytes(chunkSize);
                        await streamDestination.WriteAsync(size, 0, size.Length);

                        // write out the compressed size
                        size = BitConverter.GetBytes(compressedData.Length);
                        await streamDestination.WriteAsync(size, 0, size.Length);

                        // write out the compressed chunk
                        await streamDestination.WriteAsync(compressedData, 0,
                            compressedData.Length);

                        await streamDestination.FlushAsync();

                        // subtract the chunk size from the file size
                        fileLength -= chunkSize;

                        // if chunk is less than remaining file use
                        // remaining file
                        if (fileLength < chunkSize)
                            chunkSize = fileLength;
                    }
                }
            }
        }

        /// <summary>
        /// This function will decompress the chunked compressed file
        /// created by the CompressFile function.
        /// </summary>
        /// <param name="sourceFile">the compressed file</param>
        /// <param name="destinationFile">the destination file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static async Task DecompressFileAsync(string sourceFile, 
                                        string destinationFile,
                                        CompressionType compressionType)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
                throw new ArgumentNullException(nameof(sourceFile));
            if (string.IsNullOrWhiteSpace(destinationFile))
                throw new ArgumentNullException(nameof(destinationFile));

            FileStream streamSource = null;
            FileStream streamDestination = null;
            Stream streamUncompressed = null;

            int bufferSize = 4096;
            using (streamSource = new FileStream(sourceFile,
                    FileMode.OpenOrCreate, FileAccess.Read, FileShare.None,
                    bufferSize, useAsync: true))
            {
                using (streamDestination = new FileStream(destinationFile,
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                    bufferSize, useAsync: true))
                {
                    // read the fileLength size
                    // read the chunk size
                    byte[] size = new byte[sizeof(long)];
                    await streamSource.ReadAsync(size, 0, size.Length);
                    // convert the size back to a number
                    long fileLength = BitConverter.ToInt64(size, 0);
                    long chunkSize = 0;
                    int storedSize = 0;
                    long workingSet = Process.GetCurrentProcess().WorkingSet64;
                    while (fileLength > 0)
                    {
                        // read the chunk size
                        size = new byte[sizeof(long)];
                        await streamSource.ReadAsync(size, 0, size.Length);
                        // convert the size back to a number
                        chunkSize = BitConverter.ToInt64(size, 0);
                        if (chunkSize > fileLength ||
                            chunkSize > workingSet)
                            throw new InvalidDataException();

                        // read the compressed size
                        size = new byte[sizeof(int)];
                        await streamSource.ReadAsync(size, 0, size.Length);
                        // convert the size back to a number
                        storedSize = BitConverter.ToInt32(size, 0);
                        if (storedSize > fileLength ||
                            storedSize > workingSet)
                            throw new InvalidDataException();

                        if (storedSize > chunkSize)
                            throw new InvalidDataException();

                        byte[] uncompressedData = new byte[chunkSize];
                        byte[] compressedData = new byte[storedSize];
                        await streamSource.ReadAsync(compressedData, 0,
                            compressedData.Length);

                        // uncompress the chunk
                        MemoryStream uncompressedDataStream =
                            new MemoryStream(compressedData);

                        if (compressionType == CompressionType.Deflate)
                            streamUncompressed =
                                new DeflateStream(uncompressedDataStream,
                                    CompressionMode.Decompress);
                        else
                            streamUncompressed =
                                new GZipStream(uncompressedDataStream,
                                    CompressionMode.Decompress);

                        using (streamUncompressed)
                        {
                            // read the chunk in the compressed stream
                            await streamUncompressed.ReadAsync(uncompressedData, 0,
                                uncompressedData.Length);
                        }

                        // write out the uncompressed chunk
                        await streamDestination.WriteAsync(uncompressedData, 0,
                            uncompressedData.Length);

                        // subtract the chunk size from the file size
                        fileLength -= chunkSize;

                        // if chunk is less than remaining file use remaining file
                        if (fileLength < chunkSize)
                            chunkSize = fileLength;
                    }
                }
            }
        }

		public enum CompressionType
		{
			Deflate,
			GZip
		}

        #region Pre-.NET 4.5 version
        public static void TestCompressNewFile()
        {
            byte[] data = new byte[10000000];
            for (int i = 0; i < 10000000; i++)
                data[i] = (byte)i;


            string normalFilePath = Path.GetTempPath() + "NewNormalFile.txt";
            string deflateCompressedFilePath = Path.GetTempPath() + "NewCompressedFile.txt";
            string deflateDecompressedFilePath = Path.GetTempPath() + "NewDecompressedFile.txt";
            string gzCompressedFilePath = Path.GetTempPath() + "NewGZCompressedFile.txt";
            string gzDecompressedFilePath = Path.GetTempPath() + "NewGZDecompressedFile.txt";

            Console.WriteLine($"Base file: {normalFilePath}");
            Console.WriteLine($"Deflate compressed file: {deflateCompressedFilePath}");
            Console.WriteLine($"Defalte decompressed file: {deflateDecompressedFilePath}");
            Console.WriteLine($"GZip compressed file: {gzCompressedFilePath}");
            Console.WriteLine($"GZip decompressed file: {gzDecompressedFilePath}");

            FileStream fs =
                new FileStream(normalFilePath,
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            using (fs)
            {
                fs.Write(data, 0, data.Length);
            }

            CompressFile(normalFilePath, deflateCompressedFilePath,
                CompressionType.Deflate);

            DecompressFile(deflateCompressedFilePath, deflateDecompressedFilePath,
                CompressionType.Deflate);

            CompressFile(normalFilePath, gzCompressedFilePath,
                CompressionType.GZip);

            DecompressFile(gzCompressedFilePath, gzDecompressedFilePath,
                CompressionType.GZip);

            //Normal file size == 10,000,000 bytes
            //GZipped file size == 155,204
            //Deflated file size == 155,168
            // 36 bytes are related to the GZip CRC
        }

        /// <summary>
        /// Compress the source file to the destination file.
        /// This is done in 1MB chunks to not overwhelm the memory usage.
        /// </summary>
        /// <param name="sourceFile">the uncompressed file</param>
        /// <param name="destinationFile">the compressed file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static void CompressFile(string sourceFile,
                                        string destinationFile,
                                        CompressionType compressionType)
        {
            if (sourceFile != null)
            {
                FileStream streamSource = null;
                FileStream streamDestination = null;
                Stream streamCompressed = null;

                using (streamSource = File.OpenRead(sourceFile))
                {
                    using (streamDestination = File.OpenWrite(destinationFile))
                    {
                        // read 1MB chunks and compress them
                        long fileLength = streamSource.Length;

                        // write out the fileLength size
                        byte[] size = BitConverter.GetBytes(fileLength);
                        streamDestination.Write(size, 0, size.Length);

                        long chunkSize = 1048576; // 1MB
                        while (fileLength > 0)
                        {
                            // read the chunk
                            byte[] data = new byte[chunkSize];
                            streamSource.Read(data, 0, data.Length);

                            // compress the chunk
                            MemoryStream compressedDataStream =
                                new MemoryStream();

                            if (compressionType == CompressionType.Deflate)
                                streamCompressed =
                                    new DeflateStream(compressedDataStream,
                                        CompressionMode.Compress);
                            else
                                streamCompressed =
                                    new GZipStream(compressedDataStream,
                                        CompressionMode.Compress);

                            using (streamCompressed)
                            {
                                // write the chunk in the compressed stream
                                streamCompressed.Write(data, 0, data.Length);
                            }
                            // get the bytes for the compressed chunk
                            byte[] compressedData =
                                compressedDataStream.GetBuffer();

                            // write out the chunk size
                            size = BitConverter.GetBytes(chunkSize);
                            streamDestination.Write(size, 0, size.Length);

                            // write out the compressed size
                            size = BitConverter.GetBytes(compressedData.Length);
                            streamDestination.Write(size, 0, size.Length);

                            // write out the compressed chunk
                            streamDestination.Write(compressedData, 0,
                                compressedData.Length);

                            // subtract the chunk size from the file size
                            fileLength -= chunkSize;

                            // if chunk is less than remaining file use
                            // remaining file
                            if (fileLength < chunkSize)
                                chunkSize = fileLength;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function will decompress the chunked compressed file
        /// created by the CompressFile function.
        /// </summary>
        /// <param name="sourceFile">the compressed file</param>
        /// <param name="destinationFile">the destination file</param>
        /// <param name="compressionType">the type of compression to use</param>
        public static void DecompressFile(string sourceFile,
                                        string destinationFile,
                                        CompressionType compressionType)
        {
            FileStream streamSource = null;
            FileStream streamDestination = null;
            Stream streamUncompressed = null;

            using (streamSource = File.OpenRead(sourceFile))
            {
                using (streamDestination = File.OpenWrite(destinationFile))
                {
                    // read the fileLength size
                    // read the chunk size
                    byte[] size = new byte[sizeof(long)];
                    streamSource.Read(size, 0, size.Length);
                    // convert the size back to a number
                    long fileLength = BitConverter.ToInt64(size, 0);
                    long chunkSize = 0;
                    int storedSize = 0;
                    long workingSet = Process.GetCurrentProcess().WorkingSet64;
                    while (fileLength > 0)
                    {
                        // read the chunk size
                        size = new byte[sizeof(long)];
                        streamSource.Read(size, 0, size.Length);
                        // convert the size back to a number
                        chunkSize = BitConverter.ToInt64(size, 0);
                        if (chunkSize > fileLength ||
                            chunkSize > workingSet)
                            throw new InvalidDataException();

                        // read the compressed size
                        size = new byte[sizeof(int)];
                        streamSource.Read(size, 0, size.Length);
                        // convert the size back to a number
                        storedSize = BitConverter.ToInt32(size, 0);
                        if (storedSize > fileLength ||
                            storedSize > workingSet)
                            throw new InvalidDataException();

                        if (storedSize > chunkSize)
                            throw new InvalidDataException();

                        byte[] uncompressedData = new byte[chunkSize];
                        byte[] compressedData = new byte[storedSize];
                        streamSource.Read(compressedData, 0,
                            compressedData.Length);

                        // uncompress the chunk
                        MemoryStream uncompressedDataStream =
                            new MemoryStream(compressedData);

                        if (compressionType == CompressionType.Deflate)
                            streamUncompressed =
                                new DeflateStream(uncompressedDataStream,
                                    CompressionMode.Decompress);
                        else
                            streamUncompressed =
                                new GZipStream(uncompressedDataStream,
                                    CompressionMode.Decompress);

                        using (streamUncompressed)
                        {
                            // read the chunk in the compressed stream
                            streamUncompressed.Read(uncompressedData, 0,
                                uncompressedData.Length);
                        }

                        // write out the uncompressed chunk
                        streamDestination.Write(uncompressedData, 0,
                            uncompressedData.Length);

                        // subtract the chunk size from the file size
                        fileLength -= chunkSize;

                        // if chunk is less than remaining file use remaining file
                        if (fileLength < chunkSize)
                            chunkSize = fileLength;
                    }
                }
            }
        }
        #endregion
        #endregion

    }
}
