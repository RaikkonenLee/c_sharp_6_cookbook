using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace NamedPipes
{
    /// <summary>
    /// NamedPipeClient - An implementation of a synchronous, message-based, 
    /// named pipe client
    ///
    /// </summary>
    public class NamedPipeClient : IDisposable
    {
        #region Private Members
        /// <summary>
        ///    the full name of the pipe being connected to  
        /// </summary>
        private string _pipeName = "";

        /// <summary>
        /// the pipe handle once connected
        /// </summary>
        private SafeFileHandle _handle = new SafeFileHandle(NamedPipeInterop.INVALID_HANDLE_VALUE,true);

        /// <summary>
        /// default response buffer size (1K) 
        /// </summary>
        private int _responseBufferSize = 1024;

        /// <summary>
        /// track if dispose has been called
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// timeout for the retry after first failed connect
        /// </summary>
        private int _retryTimeout = 20000;

        /// <summary>
        /// number of times to retry connecting
        /// </summary>
        private int _retryConnect = 5;
        #endregion

        #region Construction / Cleanup
        /// <summary>
        /// CTOR 
        /// </summary>
        /// <param name="pipeName">name of the pipe</param>
        public NamedPipeClient(string pipeName)
        {
            _pipeName = pipeName;
            Trace.WriteLine("NamedPipeClient using pipe name of " + _pipeName);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NamedPipeClient()
        {
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
                ClosePipe();
            }
            disposed = true;
        }

        private void ClosePipe()
        {
            if (!_handle.IsInvalid)
            {
                _handle.Close();
            }
        }

        /// <summary>
        /// Close - because it is more intuitive than Dispose... :)
        /// </summary>
        public void Close()
        {
            ClosePipe();
        }
        #endregion

        #region Properties
        /// <summary>
        /// ResponseBufferSize Property - the size used to create response buffers 
        /// for messages written using WriteMessage
        /// </summary>
        public int ResponseBufferSize
        {
            get
            {
                return _responseBufferSize;
            }
            set
            {
                _responseBufferSize = value;
            }
        }

        /// <summary>
        /// the number of milliseconds to wait when attempting to retry a connection
        /// </summary>
        public int RetryConnectCount
        {
            get
            {
                return _retryConnect;
            }
            set
            {
                _retryConnect = value;
            }
        }

        /// <summary>
        /// the number of milliseconds to wait when attempting to retry a connection
        /// </summary>
        public int RetryConnectTimeout
        {
            get
            {
                return _retryTimeout;
            }
            set
            {
                _retryTimeout = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Connect - connect to an existing pipe 
        /// </summary>
        /// <returns>true if connected</returns>
        public void Connect()
        {
            if (!_handle.IsInvalid)
                throw new InvalidOperationException("Pipe is already connected!");

            string errMsg = "";
            int errCode = 0;
            int retryAttempts = _retryConnect;
            // keep trying to connect 
            while (retryAttempts > 0)
            {
                // mark off one attempt
                retryAttempts--;

                // connect to existing pipe
                _handle = NamedPipeInterop.CreateFile(_pipeName,
                    NamedPipeInterop.GENERIC_READ |
                    NamedPipeInterop.GENERIC_WRITE,
                    0,
                    IntPtr.Zero,
                    NamedPipeInterop.OPEN_EXISTING,
                    0,
                    0);

                // check to see if we connected
                if (!_handle.IsInvalid)
                    break;

                // the pipe could not be opened as all instances are busy
                // any other error we bail for
                errCode = Marshal.GetLastWin32Error();
                if (errCode !=
                    NamedPipeInterop.ERROR_PIPE_BUSY)
                {
                    errMsg = string.Format("Could not open pipe {0} with error {1}",_pipeName,errCode);
                    Trace.WriteLine(errMsg);
                    throw new Win32Exception(errCode, errMsg);
                }
                // if it was busy, see if we can wait it out
                else if (!NamedPipeInterop.WaitNamedPipe(_pipeName, (uint)_retryTimeout))
                {
                    errCode = Marshal.GetLastWin32Error();
                    errMsg = 
                        string.Format("Wait for pipe {0} timed out after {1} milliseconds with error code {2}.",
                                   _pipeName, _retryTimeout,errCode);
                    Trace.WriteLine(errMsg);
                    throw new Win32Exception(errCode, errMsg);
                }
            }
            // indicate connection in debug
            Trace.WriteLine("Connected to pipe: " + _pipeName);

            // The pipe connected; change to message-read mode. 
            bool success = false;
            int mode = (int)NamedPipeInterop.PIPE_READMODE_MESSAGE;

            // set to message mode
            success = NamedPipeInterop.SetNamedPipeHandleState(
                _handle,    // pipe handle 
                ref mode,  // new pipe mode 
                IntPtr.Zero,     // don't set maximum bytes 
                IntPtr.Zero);    // don't set maximum time 

            // currently implemented for just synchronous, message based pipes 
            // so bail if we couldn't set the client up properly
            if (false == success)
            {
                errCode = Marshal.GetLastWin32Error();
                errMsg =
                    string.Format("Could not change pipe mode to message with error code {0}",
                        errCode);
                Trace.WriteLine(errMsg);
                Dispose();
                throw new Win32Exception(errCode, errMsg);
            }
        }

        /// <summary>
        /// WriteMessage - write an array of bytes and return the response from the 
        /// server 
        /// </summary>
        /// <param name="buffer">bytes to write</param>
        /// <param name="bytesToWrite">number of bytes to write</param>
        /// <returns>true if written successfully</returns>
        public MemoryStream WriteMessage(byte[] buffer,  // the write buffer
            uint bytesToWrite)  // number of bytes in the write buffer
        // message responses
        {
            // buffer to get the number of bytes read/written back
            byte[] _numReadWritten = new byte[4];
            MemoryStream responseStream = null;

            bool success = false;
            // Write the byte buffer to the pipe
            success = NamedPipeInterop.WriteFile(_handle,
                buffer,
                bytesToWrite,
                _numReadWritten,
                0);

            if (success)
            {
                byte[] responseBuffer = new byte[_responseBufferSize];
                responseStream = new MemoryStream(_responseBufferSize);
                {
                    do
                    {
                        // Read the response from the pipe. 
                        success = NamedPipeInterop.ReadFile(
                            _handle,    // pipe handle 
                            responseBuffer,    // buffer to receive reply 
                            (uint)_responseBufferSize,      // size of buffer 
                            _numReadWritten,  // number of bytes read 
                            0);    // not overlapped 

                        // failed, not just more data to come
                        if (!success && Marshal.GetLastWin32Error() != NamedPipeInterop.ERROR_MORE_DATA)
                            break;

                        // concat response to stream
                        responseStream.Write(responseBuffer,
                            0,
                            responseBuffer.Length);

                    } while (!success);

                }
            }
            return responseStream;
        }
        #endregion
    }
}
