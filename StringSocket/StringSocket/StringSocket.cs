// Written by Joe Zachary for CS 3500, November 2012
// Revised by Joe Zachary April 2016
// Revised extensively by Joe Zachary April 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CustomNetworking
{
    /// <summary>
    /// The type of delegate that is called when a StringSocket send has completed.
    /// </summary>
    public delegate void SendCallback(bool wasSent, object payload);

    /// <summary>
    /// The type of delegate that is called when a receive has completed.
    /// </summary>
    public delegate void ReceiveCallback(String s, object payload);

    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchronization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// We can write a string to a StringSocket ss by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameter to the callback is the payload.  
    /// 
    /// We can read a string from a StringSocket ss by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a string and the payload.  The string is the requested string (with the newline removed).
    /// </summary>

    public class StringSocket : IDisposable
    {

        private struct Receive
        {
            public ReceiveCallback callback;
            public object payload;
            public int length;

            public Receive(ReceiveCallback callback, object payload, int length)
            {
                this.callback = callback;
                this.payload = payload;
                this.length = length;
            }
        }

        private struct Send
        {
            public string s;
            public SendCallback callback;
            public object payload;

            public Send(string s, SendCallback callback, object payload)
            {
                this.callback = callback;
                this.payload = payload;
                this.s = s;
            }
        }

        private Queue<Receive> ReceiveQueue;
        private Queue<Send> SendQueue;

        // Underlying socket
        private Socket socket;

        // Encoding used for sending and receiving
        private Encoding encoding;

        private byte[] incomingBytes;
        private byte[] pendingBytes;

        private StringBuilder incoming;

        private char[] incomingChars;

        private int pendingIndex = 0;

        private Decoder decoder;

        private object SendLock = new object();
        private object ReceiveLock = new object();
        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// StringSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        internal StringSocket(Socket s, Encoding e)
        {
            socket = s;
            encoding = e;

            SendQueue = new Queue<Send>();
            pendingBytes = new byte[0];

            ReceiveQueue = new Queue<Receive>();
            incomingBytes = new byte[10000];
            incomingChars = new char[10000];
            incoming = new StringBuilder();

            decoder = encoding.GetDecoder();
            // TODO: Complete implementation of StringSocket
        }

        /// <summary>
        /// Shuts down this StringSocket.
        /// </summary>
        public void Shutdown(SocketShutdown mode)
        {
            socket.Shutdown(mode);
        }

        /// <summary>
        /// Closes this StringSocket.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket it invokes the callback.  
        /// The parameters to the callback are true and the payload.
        /// 
        /// If it is impossible to send because the underlying Socket has closed, the callback 
        /// is invoked with false and the payload as parameters.
        ///
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            lock (SendLock)
                SendQueue.Enqueue(new Send(s, callback, payload));

            if (SendQueue.Count == 1)
            {
                lock (SendLock)
                    sendMessages();
            }
        }

        private void sendMessages()
        {
            if (pendingIndex < pendingBytes.Length)
            {
                socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex, SocketFlags.None, messageSent, null);
            }
            else if (SendQueue.Count > 0)
            {
                pendingIndex = 0;
                pendingBytes = encoding.GetBytes(SendQueue.First().s);

                socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex, SocketFlags.None, messageSent, null);
            }
        }

        private void messageSent(IAsyncResult result)
        {
            int bytesSent = socket.EndSend(result);

            lock (SendLock)
            {
                pendingIndex += bytesSent;
                Send send = SendQueue.Dequeue();

                Task.Run(() => send.callback(true, send.payload));
                sendMessages();
            }
        }


        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, it 
        /// invokes the callback.  The parameters to the callback are a string and the payload.  
        /// The string is the requested string (with the newline removed).
        /// 
        /// Alternatively, we can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload, length)
        ///     
        /// If length is negative or zero, this behaves identically to the first case.  If length
        /// is positive, then it reads and decodes length bytes from the underlying Socket, yielding
        /// a string s.  The parameters to the callback are s and the payload
        ///
        /// In either case, if there are insufficient bytes to service a request because the underlying
        /// Socket has closed, the callback is invoked with null and the payload.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload, int length = 0)
        {
            lock (ReceiveLock)
                ReceiveQueue.Enqueue(new Receive(callback, payload, length));

            if (ReceiveQueue.Count == 1)
            {
                lock (ReceiveLock)
                    ReceiveMessages();
            }
        }

        private void ReceiveMessages()
        {
            if (incoming.Length > 0)
            {
                int lastNewline = 0;
                int start = 0;
                int length = 0;
                int twoByteChars = 0;

                for (int i = 0; i < incoming.Length; i++)
                {
                    if (incoming[i] > 256) twoByteChars++;

                    if (ReceiveQueue.Count > 0)
                    {
                        length = ReceiveQueue.First().length;
                        if ((length > 0) ? i - start == length - twoByteChars -1 : incoming[i] == '\n')
                        {
                            String line;

                            if (length > 0)
                                 line = incoming.ToString(start, i - start + 1);
                            else
                                 line = incoming.ToString(start, i - start);

                            Receive receive = ReceiveQueue.Dequeue();
                            Task.Run(() => receive.callback(line, receive.payload));

                                start = i + 1;

                            lastNewline = start;
                            twoByteChars = 0;
                        }
                    }
                }

                incoming.Remove(0, lastNewline);
            }

            if (ReceiveQueue.Count > 0)
                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length, SocketFlags.None, MessageReceived, null);

        }

        private void MessageReceived(IAsyncResult result)
        {
            int bytesReceived = socket.EndReceive(result);

            lock (ReceiveLock)
            {
                int charsRead = decoder.GetChars(incomingBytes, 0, bytesReceived, incomingChars, 0, false);
                incoming.Append(incomingChars, 0, charsRead);

                int lastNewline = 0;
                int start = 0;
                int length = 0;
                int twoByteChars = 0;

                for (int i = 0; i < incoming.Length; i++)
                {
                    if (incoming[i] > 256) twoByteChars++;

                    if (ReceiveQueue.Count > 0)
                    {
                        length = ReceiveQueue.First().length;
                        if ((length > 0) ? i - start == length - twoByteChars - 1 : incoming[i] == '\n')
                        {
                            String line;

                            if (length > 0)
                                line = incoming.ToString(start, i - start + 1);
                            else
                                line = incoming.ToString(start, i - start);

                            Receive receive = ReceiveQueue.Dequeue();
                            Task.Run(() => receive.callback(line, receive.payload));

                            start = i + 1;

                            lastNewline = start;
                            twoByteChars = 0;
                        }
                    }
                }

                incoming.Remove(0, lastNewline);
                ReceiveMessages();
            }
        }

        /// <summary>
        /// Frees resources associated with this StringSocket.
        /// </summary>
        public void Dispose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }
    }
}
