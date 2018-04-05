using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomNetworking
{
    /// <summary>
    /// Represents client connections that use StringSockets.
    /// </summary>
    public class StringSocketClient
    {
        /// <summary>
        /// Creates a client communicating with the remote host via the given port
        /// using the provided encoding over a StringSocket.
        /// </summary>
        public StringSocketClient(string host, int port, Encoding encoding)
        {
            Socket s = new TcpClient(host, port).Client;
            Client = new StringSocket(s, encoding);
        }

        /// <summary>
        /// The StringSocket over which the communication occurs.
        /// </summary>
        public StringSocket Client
        {
            get; private set;
        }
    }
}
