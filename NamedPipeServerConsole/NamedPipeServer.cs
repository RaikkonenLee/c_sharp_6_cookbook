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
    /// NamedPipeServer - An implementation of a synchronous, message-based, 
    /// named pipe server
    ///
    /// </summary>
    public class NamedPipeServer : IDisposable
    {
        #region Private Members
        /// <summary>
        /// the pipe handle 
        /// </summary>
        private SafeFileHandle _handle = new SafeFileHandle(NamedPipeInterop.INVALID_HANDLE_VALUE, true);

        /// <summary>
        /// the name of the pipe 
        /// </summary>
        private string _pipeName = "";

        /// <summary>
        /// default size of message buffer to read 
        /// </summary>
        private int _receiveBufferSize = 1024;

        /// <summary>
        /// track if dispose has been called
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// PIPE_SERVER_BUFFER_SIZE set to 8192 by default
        /// </summary>
        private const int PIPE_SERVER_BUFFER_SIZE = 8192;
        #endregion

        #region Construction / Cleanup
        /// <summary>
        /// CTOR 
        /// </summary>
        /// <param name="pipeBaseName">the base name of the pipe</param>
        /// <param name="msgReceivedDelegate">delegate to be notified when 
        /// a message is received</param>
        public NamedPipeServer(string pipeBaseName)
        {
            // assemble the pipe name
            _pipeName = "\\\\.\\PIPE\\" + pipeBaseName;
            Trace.WriteLine("NamedPipeServer using pipe name " + _pipeName);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NamedPipeServer()
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
            Trace.WriteLine("NamedPipeServer closing pipe");

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
        /// PipeName 
        /// </summary>
        /// <returns>the composed pipe name</returns>
        public string PipeName
        {
            get
            {
                return _pipeName;
            }
        }

        /// <summary>
        /// ReceiveBufferSize Property - the size used to create receive buffers 
        /// for messages received using WaitForMessage
        /// </summary>
        public int ReceiveBufferSize
        {
            get
            {
                return _receiveBufferSize;
            }
            set
            {
                _receiveBufferSize = value;
            }
        }
        #endregion 

        #region Public Methods
        /// <summary>
        /// CreatePipe - create the named pipe 
        /// </summary>
        /// <returns>true is pipe created</returns>
        public bool CreatePipe()
        {
            // make a named pipe in message mode
            _handle = NamedPipeInterop.CreateNamedPipe(_pipeName,
                NamedPipeInterop.PIPE_ACCESS_DUPLEX,
                NamedPipeInterop.PIPE_TYPE_MESSAGE | NamedPipeInterop.PIPE_READMODE_MESSAGE |
                NamedPipeInterop.PIPE_WAIT,
                NamedPipeInterop.PIPE_UNLIMITED_INSTANCES,
                PIPE_SERVER_BUFFER_SIZE,
                PIPE_SERVER_BUFFER_SIZE,
                NamedPipeInterop.NMPWAIT_WAIT_FOREVER,
                IntPtr.Zero);

            // make sure we got a good one
            if (_handle.IsInvalid)
            {
                Debug.WriteLine("Could not create the pipe (" +
                    _pipeName + ") - os returned " +
                    Marshal.GetLastWin32Error());

                return false;
            }
            return true;
        }

        /// <summary>
        /// WaitForClientConnect - wait for a client to connect to this pipe 
        /// </summary>
        /// <returns>true if connected, false if timed out</returns>
        public bool WaitForClientConnect()
        {
            // wait for someone to talk to us
            return NamedPipeInterop.ConnectNamedPipe(_handle, IntPtr.Zero);
        }

        /// <summary>
        /// WaitForMessage - have the server wait for a message 
        /// </summary>
        /// <returns>a non-null MessageStream if it got a message, null if timed out or error</returns>
        public MemoryStream WaitForMessage()
        {
            bool fullyRead = false;
            string errMsg = "";
            int errCode = 0;
            // they want to talk to us, read their messages and write 
            // replies
            MemoryStream receiveStream = new MemoryStream();
            byte[] buffer = new byte[_receiveBufferSize];
            byte[] _numReadWritten = new byte[4];

            // need to read the whole message and put it in one message 
            // byte buffer
            do
            {
                // Read the response from the pipe. 
                if (!NamedPipeInterop.ReadFile(
                    _handle,    // pipe handle 
                    buffer,    // buffer to receive reply 
                    (uint)_receiveBufferSize,      // size of buffer 
                    _numReadWritten,  // number of bytes read 
                    0))    // not overlapped 
                {
                    // failed, not just more data to come
                    errCode = Marshal.GetLastWin32Error();
                    if (errCode != NamedPipeInterop.ERROR_MORE_DATA)
                        break;
                    else
                    {
                        errMsg = string.Format("Could not read from pipe with error {0}", errCode);
                        Trace.WriteLine(errMsg);
                        throw new Win32Exception(errCode, errMsg);
                    }
                }
                else
                {
                    // we succeeded and no more data is coming
                    fullyRead = true;
                }
                // concat the message bytes to the stream
                receiveStream.Write(buffer, 0, buffer.Length);

            } while (!fullyRead);  

            if (receiveStream.Length > 0)
            {
                // now set up response with a polite response using the same 
                // Unicode string protocol
                string reply = "Thanks for the message!";
                byte[] msgBytes = Encoding.Unicode.GetBytes(reply);

                uint len = (uint)msgBytes.Length;
                // write the response message provided 
                // by the delegate
                if (!NamedPipeInterop.WriteFile(_handle,
                    msgBytes,
                    len,
                    _numReadWritten,
                    0))
                {
                    errCode = Marshal.GetLastWin32Error();
                    errMsg = string.Format("Could not write response with error {0}", errCode);
                    Trace.WriteLine(errMsg);
                    throw new Win32Exception(errCode, errMsg);
                }

                // return the message we received
                return receiveStream;
            }
            else // didn't receive anything
                return null;
        }
        #endregion
    }
}
