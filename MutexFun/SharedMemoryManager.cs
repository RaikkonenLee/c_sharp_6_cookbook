using System;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.MemoryMappedFiles;

namespace MutexFun
{
    /// <summary>
    /// Class for sending objects through shared memory using a mutex
    /// to synchronize access to the shared memory
    /// </summary>
    public class SharedMemoryManager<TransferItemType> : IDisposable 
    {
        #region Private members
        private bool disposed = false;
        #endregion
        
        #region Construction / Cleanup
        public SharedMemoryManager(string name,int sharedMemoryBaseSize)
        {
            // can only be built for serializable objects 
            if (!typeof(TransferItemType).IsSerializable)
                throw new ArgumentException($"Object {typeof(TransferItemType)} is not serializeable.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (sharedMemoryBaseSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(sharedMemoryBaseSize),
                    "Shared Memory Base Size must be a value greater than zero");

            // set name of the region
            Name = name;
            
            // save base size
            SharedMemoryBaseSize = sharedMemoryBaseSize;

            // set up the shared memory region
            MemMappedFile = MemoryMappedFile.CreateOrOpen(Name, MemoryRegionSize);

            // set up the mutex
            MutexForSharedMem = new Mutex(true, MutexName);
        }

        ~SharedMemoryManager()
	    {
		    // make sure we close
		    Dispose(false);
	    }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                CloseSharedMemory();
            }
            disposed = true;
        }

        private void CloseSharedMemory()
        {
            if(MemMappedFile != null)
                MemMappedFile.Dispose();
        }

        public void Close()
        {
            CloseSharedMemory();
        }
        #endregion

        #region Properties
        /// <summary>
        /// How big of a memory mapped file to have
        /// </summary>
        public int SharedMemoryBaseSize { get; protected set; }

        /// <summary>
        /// The actual size of the memory region to include size of the
        /// object being transferred
        /// </summary>
        private long MemoryRegionSize => (long)(SharedMemoryBaseSize + sizeof(Int32));

        /// <summary>
        /// Name of the shared memory region
        /// </summary>
        private string Name { get; }

        /// <summary>
        /// The name of the mutex protecting the shared region
        /// </summary>
        private string MutexName => $"{typeof(TransferItemType)}mtx{Name}";

        /// <summary>
        /// The mutex protecting the shared region
        /// </summary>
        private Mutex MutexForSharedMem { get; } = null;

        /// <summary>
        /// The MemoryMappedFile used to transfer objects
        /// </summary>
        private MemoryMappedFile MemMappedFile { get; } = null;

        #endregion

        #region Public Methods
        /// <summary>
	    /// Send a serializeable object through the shared memory
	    /// and wait for it to be picked up
	    /// </summary>
	    /// <param name="transferObject"> the object to send</param>
        public void SendObject(TransferItemType transferObject)
        {
	        // create a memory stream, initialize size
            using (MemoryStream ms = new MemoryStream())
            {
                // get a formatter to serialize with
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    // serialize the object to the stream
                    formatter.Serialize(ms, transferObject);

                    // get the bytes for the serialized object 
                    byte[] bytes = ms.ToArray();

                    // check that this object will fit
                    if(bytes.Length + sizeof(Int32)  > MemoryRegionSize)
                    {
                        string msg = 
                            $"{typeof(TransferItemType)} object instance serialized" +
                            $"to {bytes.Length} bytes which is too large for the shared memory region";

                        throw new ArgumentException(msg, nameof(transferObject));
                    }

                    // write to the shared memory region
                    using (MemoryMappedViewStream stream = 
                        MemMappedFile.CreateViewStream())
                    {
                        BinaryWriter writer = new BinaryWriter(stream);
                        writer.Write(bytes.Length); // write the size
                        writer.Write(bytes); // write the object
                    }
                }
                finally
                {
                    // signal the other process using the mutex to tell it
                    // to do receive processing
                    MutexForSharedMem.ReleaseMutex();

                    // wait for the other process to signal it has received
                    // and we can move on
                    MutexForSharedMem.WaitOne();
                }
            }
        }

	    /// <summary>
	    /// Wait for an object to hit the shared memory and then deserialize it
	    /// </summary>
	    /// <returns>object passed</returns>
        public TransferItemType ReceiveObject()
        {
            // wait on the mutex for an object to be queued by the sender
            MutexForSharedMem.WaitOne();

            // get the object from the shared memory
            byte[] serializedObj = null;
            using (MemoryMappedViewStream stream = 
                MemMappedFile.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                int objectLength = reader.ReadInt32();
                serializedObj = reader.ReadBytes(objectLength);
            }

	        // set up the memory stream with the object bytes
            using (MemoryStream ms = new MemoryStream(serializedObj))
            {
                // set up a binary formatter
                BinaryFormatter formatter = new BinaryFormatter();

                // get the object to return
                TransferItemType item;
                try
                {
                    item = (TransferItemType)formatter.Deserialize(ms);
                }
                finally
                {
                    // signal that we received the object using the mutex
                    MutexForSharedMem.ReleaseMutex();
                }
                // give them the object
                return item;
            }
        }
	    #endregion
    }
}
