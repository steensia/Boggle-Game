using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomNetworking
{
    /// <summary>
    /// The type of delegate that is called when a StringSocketListener has
    /// opened a StringSocket.
    /// </summary>
    public delegate void StringSocketCallback(StringSocket ss, object payload);

    /// <summary>
    /// A server capable of communicating with multiple remote clients over 
    /// a StringSocket.
    /// </summary>
    public class StringSocketListener
    {
        /// <summary>
        /// The TcpListener that underlies this server
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// The encoding being used
        /// </summary>
        private Encoding encoding;

        /// <summary>
        /// Creates a StringSocketListener that listens for incoming connections on the
        /// specified port.  Uses the provided encoding for a StringSockets that are
        /// connected.
        /// </summary>
        public StringSocketListener(int port, Encoding encoding)
        {
            this.encoding = encoding;
            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Begins listening asynchonously for an incoming socket reqest.  When a
        /// StringSocket is established, invokes the callback wih the StringSocket
        /// and the payload as parameters.
        /// </summary>
        public void BeginAcceptStringSocket(StringSocketCallback callback, object payload)
        {
            tcpListener.BeginAcceptSocket(SocketAccepted, new Tuple<StringSocketCallback, object>(callback, payload));
        }

        /// <summary>
        /// Called when a Socket has been established as a result of a
        /// BeginAcceptSocket call.
        /// </summary>
        private void SocketAccepted(IAsyncResult ar)
        {
            Socket s = tcpListener.EndAcceptSocket(ar);
            var internalPayload = (Tuple<StringSocketCallback, object>)ar.AsyncState;
            StringSocketCallback callback = internalPayload.Item1;
            StringSocket ss = new StringSocket(s, encoding);
            object externalPayload = internalPayload.Item2;
            callback(ss, externalPayload);
        }

        /// <summary>
        /// Blocking call that obtains a StringSocket connection.
        /// </summary>
        public StringSocket AcceptStringSocket()
        {
            return new StringSocket(tcpListener.AcceptSocket(), encoding);
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            tcpListener.Start();
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            tcpListener.Stop();
        }
    }
}
