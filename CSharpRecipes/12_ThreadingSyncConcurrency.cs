using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CSharpRecipes.Properties;
using NorthwindLinq2Sql;
using System.Data.Entity;

namespace CSharpRecipes
{
    public class ThreadingSyncAndConcurrency
	{
        #region "12.1 Creating Per-Thread Static Fields"
        public static void PerThreadStatic()
        {
            TestStaticField();
        }

        public class Foo
        {
            [ThreadStaticAttribute()]
            public static string bar = "Initialized string";
        }

        private static void TestStaticField()
        {
            ThreadStaticField.DisplayStaticFieldValue();

            Thread newStaticFieldThread = 
                new Thread(ThreadStaticField.DisplayStaticFieldValue);

            newStaticFieldThread.Start();

            ThreadStaticField.DisplayStaticFieldValue();
        }

        private class ThreadStaticField
        {
            [ThreadStaticAttribute()]
            public static string bar = "Initialized string";

            public static void DisplayStaticFieldValue()
            {
                string msg = $"{Thread.CurrentThread.GetHashCode()} contains static field value of: {ThreadStaticField.bar} ";
                Console.WriteLine(msg);
            }
        }


        #endregion

        #region "12.2 Providing Thread Safe Access To Class Members"
        public static void ThreadSafeAccess()
        {
            DeadLock deadLock = new DeadLock();
            lock(deadLock)
            {
                Thread thread = new Thread(deadLock.Method1);
                thread.Start();

                // Do some time consuming task here
            }

            int num = 0;
            if(Monitor.TryEnter(MonitorMethodAccess.SyncRoot, 250))
            {
                MonitorMethodAccess.ModifyNumericField(10);
                num = MonitorMethodAccess.ReadNumericField();
                Monitor.Exit(MonitorMethodAccess.SyncRoot);
            }
            Console.WriteLine(num);

        }

        public static class NoSafeMemberAccess
        {
            private static int numericField = 1;

            public static void IncrementNumericField() 
            {
                ++numericField;
            }

            public static void ModifyNumericField(int newValue) 
            {
                numericField = newValue;
            }

            public static int ReadNumericField() => (numericField);
        }

        public static class SaferMemberAccess
        {
            private static int numericField = 1;
            private static object syncObj = new object();

            public static void IncrementNumericField() 
            {
                lock(syncObj)
                {
                    ++numericField;
                }
            }

            public static void ModifyNumericField(int newValue) 
            {
                lock (syncObj)
                {
                    numericField = newValue;
                }
            }

            public static int ReadNumericField()
            {
                lock (syncObj)
                {
                    return (numericField);
                }
            }
        }

        public class DeadLock
        {
            private object syncObj = new object();

            public void Method1()
            {
                lock(syncObj)
                {
                    // Do something
                }
            }
        }

        public static class MonitorMethodAccess
        {
            private static int numericField = 1;
            private static object syncObj = new object();
            public static object SyncRoot => syncObj;

            public static void IncrementNumericField() 
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        ++numericField;
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }
            }

            public static void ModifyNumericField(int newValue) 
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        numericField = newValue;
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }
            }

            public static int ReadNumericField()
            {
                if (Monitor.TryEnter(syncObj, 250))
                {
                    try
                    {
                        return (numericField);
                    }
                    finally
                    {
                        Monitor.Exit(syncObj);
                    }
                }

                return (-1);
            }
            [MethodImpl (MethodImplOptions.Synchronized)]
            public static void MySynchronizedMethod()
            {
            }

        }

        
        #endregion

        #region "12.3 Preventing Silent Thread Termination"
        public static void PreventSilentTermination()
        {
            MainThread mt = new MainThread();
            mt.CreateNewThread();
        }

        public class MainThread
        {
            public void CreateNewThread()
            {
                // Spawn new thread to do concurrent work
                Thread newWorkerThread = new Thread(Worker.DoWork);
                newWorkerThread.Start();
            }
        }

        public class Worker
        {
            // Method called by ThreadStart delegate to do concurrent work
            public static void DoWork ()
            {
                try
                {
                    // Do thread work here
                    throw new Exception("Boom!");
                }
                catch(Exception e) 
                {
                    // Handle thread exception here
                    Console.WriteLine(e.ToString());
                    // Do not re-throw exception
                }
                finally
                {
                    // Do thread cleanup here
                }
            }
        }
        #endregion

        #region "12.4 Being Notified of the Completion of an Asynchronous Delegate"
        public static void CompletionAsyncDelegate()
        {
            AsyncAction2 aa2 = new AsyncAction2();
            aa2.CallbackAsyncDelegate();
        }

        public delegate int AsyncInvoke();
        
        public class TestAsyncInvoke
        {
            public static int Method1()
            {
                Console.WriteLine(
                    $"Invoked Method1 on Thread {Thread.CurrentThread.ManagedThreadId}");
                return (1);
            }
        }

        public class AsyncAction2
        {
            public void CallbackAsyncDelegate()
            {
                AsyncCallback callBack = DelegateCallback;

                AsyncInvoke method1 = TestAsyncInvoke.Method1;
                Console.WriteLine(
                    $"Calling BeginInvoke on Thread {Thread.CurrentThread.ManagedThreadId}");
                IAsyncResult asyncResult = method1.BeginInvoke(callBack, method1);

                // No need to poll or use the WaitOne method here, so return to the calling method.
                return;
            }

            private static void DelegateCallback(IAsyncResult iresult)
            {
                Console.WriteLine(
                    $"Getting callback on Thread {Thread.CurrentThread.ManagedThreadId}");
                AsyncResult asyncResult = (AsyncResult)iresult;
                AsyncInvoke method1 = (AsyncInvoke)asyncResult.AsyncDelegate;

                int retVal = method1.EndInvoke(asyncResult);
                Console.WriteLine($"retVal (Callback): {retVal}");
            }
        }

        public delegate int AsyncInvoke2();

        public class TestAsyncInvoke2
        {
            public static int Method1()
            {
                Console.WriteLine("Invoked Method1 on Thread {0}",
                    Thread.CurrentThread.ManagedThreadId);
                return (1);
            }
        }

        #endregion

        #region "12.5 Storing Thread Specific Data Privately"
        public static void StoreThreadDataPrivately()
        {
            HandleClass.Run();
        }

        public class ApplicationData
        {
            // Application data is stored here
            public int Data { get; set; }
        }

        public class HandleClass
        {
            public static void Run()
            {
                // Create structure instance and store it in the named data slot
                ApplicationData appData = new ApplicationData();
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), appData);

                // Call another method that will use this structure
                HandleClass.MethodB();

                // When done, free this data slot
                Thread.FreeNamedDataSlot("appDataSlot");
            }

            public static void MethodB()
            {
                // Get the instance from the named data slot
                ApplicationData storedAppData = (ApplicationData)Thread.GetData(
                    Thread.GetNamedDataSlot("appDataSlot"));

                // Modify the ApplicationData 

                // When finished modifying this data, store the changes back into
                // into the named data slot
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), 
                    storedAppData);

                // Call another method that will use this structure
                HandleClass.MethodC();
            }

            public static void MethodC()
            {
                // Get the instance from the named data slot
                ApplicationData storedAppData = 
                    (ApplicationData)Thread.GetData(Thread.GetNamedDataSlot("appDataSlot"));

                // Modify the data

                // When finished modifying this data, store the changes back into
                // the named data slot
                Thread.SetData(Thread.GetNamedDataSlot("appDataSlot"), storedAppData);
            }
        }

        #endregion

		#region "12.6 Granting multiple access to resources with a Semaphore"
		// XboxOne with 8 ports, group of software developers want access. 8 players get access initially
		// Players die after random time, new players pop up in queue after waiting on semaphore

        public class XboxOnePlayer
        {
	        public class PlayerInfo
	        {
		        public ManualResetEvent Dead {get; set;}
		        public string Name {get; set;}
	        }

	        // Death Modes for Players
	        private static string[] _deaths = new string[7]{"bought the farm",
														        "choked on a rocket",
														        "shot their own foot",
														        "been captured",
														        "fallen to their death",
														        "died of lead poisoning",
														        "failed to dodge a grenade",
														        };

	        /// <summary>
	        /// Thread function
	        /// </summary>
	        /// <param name="info">XboxOnePlayer.Data item with Xbox reference and handle</param>
            public static void JoinIn(object info)
            {
	            // open up the semaphore by name so we can act on it
                using (Semaphore XboxOne = Semaphore.OpenExisting("XboxOne"))
                {

                    // get the data object
                    PlayerInfo player = (PlayerInfo)info;

                    // Each player notifies the XboxOne they want to play
                    Console.WriteLine($"{player.Name} is waiting to play!");

                    // they wait on the XboxOne (semaphore) until it lets them
                    // have a controller
                    XboxOne.WaitOne();

                    // The XboxOne has chosen the player! (or the semaphore has 
                    // allowed access to the resource...)
                    Console.WriteLine($"{player.Name} has been chosen to play. " +
                        $"Welcome to your doom {player.Name}. >:)");

                    // figure out a random value for how long the player lasts
                    System.Random rand = new Random(500);
                    int timeTillDeath = rand.Next(100, 1000);

                    // simulate the player is busy playing till they die
                    Thread.Sleep(timeTillDeath);

                    // figure out how they died
                    rand = new Random();
                    int deathIndex = rand.Next(6);

                    // notify of the player's passing
                    Console.WriteLine($"{player.Name} has {_deaths[deathIndex]} " +
                        "and gives way to another player");

                    // if all ports are open, everyone has played and the game is over
                    int semaphoreCount = XboxOne.Release();
                    if (semaphoreCount == 3)
                    {
                        Console.WriteLine("Thank you for playing, the game has ended.");
                        // set the Dead event for the player
                        player.Dead.Set();
                    }
                }
            }
        }		
 
        public class Halo5Session
        {
	        // A semaphore that simulates a limited resource pool.
	        private static Semaphore _XboxOne;

            public static void Play()
            {
	            // An XboxOne has 8 controller ports so 8 people can play at a time
	            // We use 8 as the max an zero to start with as we want Players
	            // to queue up at first until the XboxOne boots and loads the game
	            //
                using (_XboxOne = new Semaphore(0, 8, "XboxOne"))
                {
                    using (ManualResetEvent GameOver =
                        new ManualResetEvent(false))
                    {
                        //
                        // 13 Players log in to play
                        //
                        List<XboxOnePlayer.PlayerInfo> players =
                            new List<XboxOnePlayer.PlayerInfo>() {
                                new XboxOnePlayer.PlayerInfo { Name="Igor", Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="AxeMan", Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Dr. Death",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="HaPpyCaMpEr",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Executioner",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="FragMan",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Beatdown",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Stoney",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Pwned",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Big Dawg",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Playa",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="BOOM",Dead=GameOver},
                                new XboxOnePlayer.PlayerInfo { Name="Mr. Mxylplyx",Dead=GameOver}
                                };
                             
                        foreach (XboxOnePlayer.PlayerInfo player in players)
                        {
                            Thread t = new Thread(XboxOnePlayer.JoinIn);

                            // put a name on the thread
                            t.Name = player.Name;
                            // fire up the player
                            t.Start(player);
                        }

                        // Wait for the XboxOne to spin up and load Halo5 (3 seconds)
                        Console.WriteLine("XboxOne initializing...");
                        Thread.Sleep(3000);
                        Console.WriteLine(
                            "Halo 5 loaded & ready, allowing 8 players in now...");

                        // The XboxOne has the whole semaphore count.  We call 
                        // Release(8) to open up 8 slots and
                        // allows the waiting players to enter the XboxOne(semaphore)
                        // up to eight at a time.
                        //
                        _XboxOne.Release(8);

                        // wait for the game to end...
                        GameOver.WaitOne();
                    }
                }
            }
        }
        #endregion

        #region "12.7 Synchronizing multiple processes with the Mutex"
        // see the MutexFun project TODO: REOWRK USING System.IO.MemoryMappedFiles.MemoryMappedFile
        #endregion

        #region "12.8 Using events to make threads cooperate"
        public static void TestResetEvent()
        {
            // We have a diner with a cook who can only serve up one meal at a time 
            Cook Mel = new Cook() { Name = "Mel" };
            string[] waitressNames = { "Flo", "Alice", "Vera", "Jolene", "Belle" };

            // Have waitresses place orders
            foreach (var waitressName in waitressNames)
            {
                Task.Run(() =>
                    {
                        // The Waitress places the order and then waits for the order
                        Waitress.PlaceOrder(waitressName, Cook.OrderReady);
                    });
            }

            // Have the cook fill the orders
            for (int i = 0; i < waitressNames.Length; i++)
            {
                // make the waitresses wait...
                Thread.Sleep(2000);
                // ok, next waitress, pickup!
                Mel.CallWaitress();
            }
        }

        public class Cook
        {
            public string Name { get; set; }

	        public static AutoResetEvent OrderReady = 
                new AutoResetEvent(false);

	        public void CallWaitress()
	        {
                // we call Set on the AutoResetEvent and don't have to 
                // call Reset like we would with ManualResetEvent to fire it 
                // off again.  This sets the event that the waitress is waiting for
                // in GetInLine
                // order is ready....
                Console.WriteLine($"{Name} finished order!");
                OrderReady.Set();
	        }
        }

        public class Waitress
        {
	        public static void PlaceOrder(string waitressName, AutoResetEvent orderReady)
	        {
                // order is placed....
                Console.WriteLine($"Waitress {waitressName} placed order!");
                // wait for the order...
                orderReady.WaitOne();
		        // order is ready....
		        Console.WriteLine($"Waitress {waitressName} got order!");
	        }
        }

		#endregion

		#region "12.9 Performing atomic operations amongst threads"
		public static void TestInterlocked()
		{
            int i = 0;
            long l = 0;
            Interlocked.Increment(ref i); // i = 1
            Interlocked.Decrement(ref i); // i = 0
            Interlocked.Increment(ref l); // l = 1
            Interlocked.Decrement(ref i); // l = 0

            Interlocked.Add(ref i, 10); // i = 10;
            Interlocked.Add(ref l, 100); // l = 100;

            string name = "Mr. Ed";
            Interlocked.Exchange(ref name, "Barney");

            double runningTotal = 0.0;
            double startingTotal = 0.0;
            double calc = 0.0;
            for (i = 0; i < 10; i++)
            {
	            do
	            {
		            // store of the original total
		            startingTotal = runningTotal;
		            // do an intense calculation
		            calc = runningTotal + i * Math.PI * 2 / Math.PI;
	            }
	            // check to make sure runningTotal wasn't modified
	            // and replace it with calc if not.  If it was, 
	            // run through the loop until we get it current
	            while (startingTotal !=
		            Interlocked.CompareExchange(
			            ref runningTotal, calc, startingTotal));
            }
		}
        #endregion

        #region "12.10 Optimizing Read-Mostly Access"

        static Developer s_dev = null;
        static bool s_end = false;

        /// <summary>
        /// </summary>
        public static void TestReaderWriterLockSlim()
        {
            s_dev = new Developer(15);
            LaunchTeam(s_dev);
            Thread.Sleep(10000);
        }

        private static void LaunchTeam(Developer dev)
        {
            LaunchManager("CTO", dev);
            LaunchManager("Director", dev);
            LaunchManager("Project Manager", dev);
            LaunchDependent("Product Manager", dev);
            LaunchDependent("Test Engineer", dev);
            LaunchDependent("Technical Communications Professional", dev);
            LaunchDependent("Operations Staff", dev);
            LaunchDependent("Support Staff", dev);
        }

        public class DeveloperTaskInfo
        {
            public string Name { get; set; }
            public Developer Developer { get; set; }
        }

        private static void LaunchManager(string name, Developer dev)
        {
            var dti = new DeveloperTaskInfo() { Name = name, Developer = dev };
            Task manager = Task.Run(() => {
                Console.WriteLine($"Added {dti.Name} to the project...");
                DeveloperTaskManager mgr = new DeveloperTaskManager(dti.Name, dti.Developer);
            });
        }

        private static void LaunchDependent(string name, Developer dev)
        {
            var dti = new DeveloperTaskInfo() { Name = name, Developer = dev };
            Task manager = Task.Run(() => {
                Console.WriteLine($"Added {dti.Name} to the project...");
                DeveloperTaskDependent dep = new DeveloperTaskDependent(dti.Name, dti.Developer);
            });
        }

        public class DeveloperTask 
        {
            public DeveloperTask(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
            public int Priority { get; set; }
            public bool Status { get; set; }

            public override string ToString() => this.Name;

            public override bool Equals(object obj)
            {
                DeveloperTask task = obj as DeveloperTask;
                return this.Name == task?.Name;
            }

            public override int GetHashCode() => this.Name.GetHashCode();
        }

        public class Developer : IDisposable
        {
            /// <summary>
            /// Dictionary for the tasks
            /// </summary>
            private List<DeveloperTask> DeveloperTasks { get; } = new List<DeveloperTask>();
            private ReaderWriterLockSlim Lock { get; set; } = new ReaderWriterLockSlim();
            private System.Threading.Timer Timer { get; set; }
            private int MaxTasks { get; }

            public Developer(int maxTasks)
            {
                // the maximum number of tasks before the developer quits
                MaxTasks = maxTasks;
                // do some work every 1/4 second
                Timer = new Timer(new TimerCallback(DoWork), null, 1000, 250);            
            }

            ~Developer()
            {
                Dispose(true);
            }

            // Execute a task
            protected void DoWork(Object stateInfo)
            {
                ExecuteTask();
                try
                {
                    Lock.EnterWriteLock();
                    // if we finished all tasks, go on vacation!
                    if (DeveloperTasks.Count == 0)
                    {
                        s_end = true;
                        Console.WriteLine("Developer finished all tasks, go on vacation!");
                        return;
                    }

                    if (!s_end)
                    {
                        // if we have too many tasks quit
                        if (DeveloperTasks.Count > MaxTasks)
                        {
                            // get the number of unfinished tasks
                            var query = from t in DeveloperTasks
                                        where t.Status == false
                                        select t;
                            int unfinishedTaskCount = query.Count<DeveloperTask>();

                            s_end = true;
                            Console.WriteLine("Developer has too many tasks, quitting! " +
                                $"{unfinishedTaskCount} tasks left unfinished.");
                        }
                    }
                    else
                        Timer.Dispose();
                }
                finally
                {
                    Lock.ExitWriteLock();
                }
            }

            public void AddTask(DeveloperTask newTask)
            {
                try
                {
                    Lock.EnterWriteLock();
                    // if we already have this task (unique by name) 
                    // then just accept the add as sometimes people
                    // give you the same task more than once :)
                    var taskQuery = from t in DeveloperTasks
                                    where t == newTask
                                    select t;
                    if (taskQuery.Count<DeveloperTask>() == 0)
                    {
                        Console.WriteLine($"Task {newTask.Name} was added to developer");
                        DeveloperTasks.Add(newTask);
                    }
                }
                finally
                {
                    Lock.ExitWriteLock();
                }
            }

            /// <summary>
            /// Increase the priority of the task
            /// </summary>
            /// <param name="taskName">name of the task</param>
            public void IncreasePriority(string taskName)
            {
                try
                {
                    Lock.EnterUpgradeableReadLock();
                    var taskQuery = from t in DeveloperTasks
                                    where t.Name == taskName
                                    select t;
                    if(taskQuery.Count<DeveloperTask>()>0)
                    {
                        DeveloperTask task = taskQuery.First<DeveloperTask>();
                        Lock.EnterWriteLock();
                        task.Priority++;
                        Console.WriteLine($"Task {task.Name}" +
                            $" priority was increased to {task.Priority}" + 
                            " for developer");
                        Lock.ExitWriteLock();
                    }
                }
                finally
                {
                    Lock.ExitUpgradeableReadLock();
                }
            }

            /// <summary>
            /// Allows people to check if the task is done
            /// </summary>
            /// <param name="taskName">name of the task</param>
            /// <returns>False if the taks is undone or not in the list, true if done</returns>
            public bool IsTaskDone(string taskName)
            {
                try
                {
                    Lock.EnterReadLock();
                    var taskQuery = from t in DeveloperTasks
                                    where t.Name == taskName
                                    select t;
                    if (taskQuery.Count<DeveloperTask>() > 0)
                    {
                        DeveloperTask task = taskQuery.First<DeveloperTask>();
                        Console.WriteLine($"Task {task.Name} status was reported.");
                        return task.Status;
                    }
                }
                finally
                {
                    Lock.ExitReadLock();
                }
                return false;
            }

            private void ExecuteTask()
            {
                // look over the tasks and do the highest priority 
                var queryResult =   from t in DeveloperTasks
                                    where t.Status == false
                                    orderby t.Priority
                                    select t;
                if (queryResult.Count<DeveloperTask>() > 0)
                {
                    // do the task
                    DeveloperTask task = queryResult.First<DeveloperTask>();
                    task.Status = true;
                    task.Priority = -1;
                    Console.WriteLine($"Task {task.Name} executed by developer.");
                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<Timer>k__BackingField")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<Lock>k__BackingField")]
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        Lock?.Dispose();
                        Lock = null;
                        Timer?.Dispose();
                        Timer = null;
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }
            #endregion
        }

        public class DeveloperTaskManager : DeveloperTaskDependent, IDisposable
        {
            private System.Threading.Timer ManagerTimer { get; set; }

            public DeveloperTaskManager(string name, Developer taskExecutor) :
                base(name, taskExecutor)
            {
                // intervene every 2 seconds
                ManagerTimer = 
                    new Timer(new TimerCallback(Intervene), null, 0, 2000);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
            ~DeveloperTaskManager()
            {
                Dispose(true);
            }

            // Intervene in the plan
            protected void Intervene(Object stateInfo)
            {
                ChangePriority();
                // developer ended, kill timer
                if (s_end)
                {
                    ManagerTimer.Dispose();
                    TaskExecutor = null;
                }
            }

            public void ChangePriority()
            {
                if (DeveloperTasks.Count > 0)
                {
                    int taskIndex = _rnd.Next(0, DeveloperTasks.Count - 1);
                    DeveloperTask checkTask = DeveloperTasks[taskIndex];
                    // make those developers work faster on some random task!
                    if (TaskExecutor != null)
                    {
                        TaskExecutor.IncreasePriority(checkTask.Name);
                        Console.WriteLine($"{Name} intervened and changed priority for task {checkTask.Name}");
                    }
                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected override void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        ManagerTimer?.Dispose();
                        ManagerTimer = null;
                        base.Dispose(disposing);
                    }
                    disposedValue = true;
                }
            }

            public new void Dispose()
            {
                Dispose(true);
            }
            #endregion
        }

        public class DeveloperTaskDependent : IDisposable
        {
            protected List<DeveloperTask> DeveloperTasks { get; set; }
                = new List<DeveloperTask>();
            protected Developer TaskExecutor { get; set; }

            protected Random _rnd = new Random();
            private Timer TaskTimer { get; set; }
            private Timer StatusTimer { get; set; }

            public DeveloperTaskDependent(string name, Developer taskExecutor)
            {
                Name = name;
                TaskExecutor = taskExecutor;
                // add work every 1 second
                TaskTimer = new Timer(new TimerCallback(AddWork), null, 0, 1000);
                // check status every 3 seconds
                StatusTimer = new Timer(new TimerCallback(CheckStatus), null, 0, 3000);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
            ~DeveloperTaskDependent()
            {
                Dispose();
            }

            // Add more work to the developer
            protected void AddWork(Object stateInfo)
            {
                SubmitTask();
                // developer ended, kill timer
                if (s_end)
                {
                    TaskTimer.Dispose();
                    TaskExecutor = null;
                }
            }

            // Check Status of work with the developer
            protected void CheckStatus(Object stateInfo)
            {
                CheckTaskStatus();
                // developer ended, kill timer
                if (s_end)
                {
                    StatusTimer.Dispose();
                    TaskExecutor = null;
                }
            }

            public string Name { get; set; }

            public void SubmitTask()
            {
                int taskId = _rnd.Next(10000);
                string taskName = $"({taskId} for {Name})";
                DeveloperTask newTask = new DeveloperTask(taskName);
                if (TaskExecutor != null)
                {
                    TaskExecutor.AddTask(newTask);
                    DeveloperTasks.Add(newTask);
                }
            }

            public void CheckTaskStatus()
            {
                if (DeveloperTasks.Count > 0)
                {
                    int taskIndex = _rnd.Next(0, DeveloperTasks.Count - 1);
                    DeveloperTask checkTask = DeveloperTasks[taskIndex];
                    if (TaskExecutor != null &&
                        TaskExecutor.IsTaskDone(checkTask.Name))
                    {
                        Console.WriteLine($"Task {checkTask.Name} is done for {Name}");
                        // remove it from the todo list
                        DeveloperTasks.Remove(checkTask);
                    }
                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        TaskTimer?.Dispose();
                        TaskTimer = null;
                        StatusTimer?.Dispose();
                        StatusTimer = null;
                    }
                    disposedValue = true;
                }
            }
            
            public void Dispose()
            {
                Dispose(true);
            }
            #endregion
        }

        #endregion 

        #region "12.11 Making your database requests more scalable"
        public static async Task TestAsyncDatabase()
        {
            // DataReader
            using (SqlConnection conn = new SqlConnection(Settings.Default.NorthwindConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM CUSTOMERS", conn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Console.WriteLine($"Customer {reader["ContactName"].ToString()} " +
                        $"from {reader["CompanyName"].ToString()}");
                }
            }

            // EF6
            using (var efContext = new NorthwindEntities())
            {
                var list = await (from cust in efContext.Customers
                            select cust).ToListAsync();

                foreach(var cust in list)
                {
                    Console.WriteLine($"Customer {cust.ContactName} " +
                        $"from {cust.CompanyName}");
                }

                // Make a new customer and save them
                Customer c = new Customer();
                c.CustomerID = "JENNA";
                c.ContactName = "Jenna Roberts";
                c.CompanyName = "Flamingo Industries";
                efContext.Customers.Add(c);
                await efContext.SaveChangesAsync();

                var jenna = await efContext.Customers.Where(cu =>
                                    cu.ContactName == "Jenna Roberts").FirstOrDefaultAsync();
                Console.WriteLine($"New Customer {jenna.ContactName} " +
                        $"from {jenna.CompanyName}");
            }

            // LINQ 2 SQL
            using (var l2sContext = new NorthwindLinq2SqlDataContext())
            {
                //Additional information: The source IQueryable doesn't implement IDbAsyncEnumerable<NorthwindLinq2Sql.Customer>. Only sources that implement IDbAsyncEnumerable can be used for Entity Framework 
                //var list = await (from cust in l2sContext.Customers
                //                  select cust).ToListAsync();
                var list = (from cust in l2sContext.Customers
                            select cust);
                foreach (var cust in list)
                {
                    Console.WriteLine($"Customer {cust.ContactName} " +
                        $"from {cust.CompanyName}");
                }
            }
        }
        #endregion

        #region "12.12 Running tasks in order"
        public class RelayRunner
        {
            public string Country { get; set; }
            public int Leg { get; set; }
            public bool HasBaton { get; set; }
            public TimeSpan LegTime { get; set; }
            public int TotalLegs { get; set; }

            public RelayRunner Sprint() 
            {
                Console.WriteLine($"{Country} for Leg {Leg} has the baton and is running!");
                Random rnd = new Random();
                int ms = rnd.Next(100, 1000);
                Task.Delay(ms);
                // finished....
                LegTime = new TimeSpan(0,0,0,0,ms);
                if (Leg == TotalLegs)
                    Console.WriteLine($"{Country} has finished the race!");
                return this;
            }
        }

        public static void TestTaskContinuation()
        {
            // Relay race in the olympics
            string[] countries = { "Russia", "France", "England", "United States", "India", "Germany", "China" };
            Task<RelayRunner>[,] teams = new Task<RelayRunner>[countries.Length, 4];
            List<Task<RelayRunner>> runners = new List<Task<RelayRunner>>();
            List<Task<RelayRunner>> firstLegRunners = new List<Task<RelayRunner>>();

            for (int i = 0; i < countries.Length; i++)
            {
                for (int r = 0; r < 4; r++)
                {
                    var runner = new RelayRunner()
                    {
                        Country = countries[i],
                        Leg = r+1,
                        HasBaton = r == 0 ? true : false,
                        TotalLegs = 4
                    };

                    if (r == 0) // add starting leg for country
                    {
                        Func<RelayRunner> funcRunner = runner.Sprint;
                        teams[i, r] = new Task<RelayRunner>(funcRunner);
                        firstLegRunners.Add(teams[i, r]);
                    }
                    else // add other legs for country
                    {
                        teams[i, r] = teams[i, r - 1].ContinueWith((lastRunnerRunning) =>
                            {
                                var lastRunner = lastRunnerRunning.Result;
                                // Handoff the baton
                                Console.WriteLine($"{lastRunner.Country} hands off from " +
                                    $"{lastRunner.Leg} to {runner.Leg}!");
                                Random rnd = new Random();
                                int fumbleChance = rnd.Next(0, 10);
                                if (fumbleChance > 8)
                                {
                                    Console.WriteLine($"Oh no! {lastRunner.Country} for Leg {runner.Leg}" +
                                        $" fumbled the hand off from Leg {lastRunner.Leg}!");
                                    Thread.Sleep(1000);
                                    Console.WriteLine($"{lastRunner.Country} for Leg {runner.Leg}" + 
                                        " recovered the baton and is running again!");
                                }
                                lastRunner.HasBaton = false;
                                runner.HasBaton = true;
                                return runner.Sprint();
                            });
                    }
                    // add to our list of runners
                    runners.Add(teams[i, r]);
                }
            }

            //Fire the gun to start the race!
            Parallel.ForEach(firstLegRunners, r =>
            {
                r.Start();
            });
           
            // Wait for everyone to finish
            Task.WaitAll(runners.ToArray());

            Console.WriteLine("\r\nRace standings:");

            var standings = from r in runners
                            group r by r.Result.Country into countryTeams
                            select countryTeams;

            string winningCountry = string.Empty;
            int bestTime = int.MaxValue;

            HashSet<Tuple<int, string>> place = new HashSet<Tuple<int, string>>();
            foreach (var team in standings)
            {
                var time = team.Sum(r => r.Result.LegTime.Milliseconds);
                if (time < bestTime)
                {
                    bestTime = time;
                    winningCountry = team.Key;
                }
                place.Add(new Tuple<int, string>(time, $"{team.Key} with a time of {time}ms"));
            }
            int p = 1; 
            foreach(var item in place.OrderBy(t => t.Item1))
            {
                Console.WriteLine($"{p}: {item.Item2}");
                p++;
            }
            Console.WriteLine($"\n\nThe winning team is from {winningCountry}");
        }
        #endregion
    }
}
