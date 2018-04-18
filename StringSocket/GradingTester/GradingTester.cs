using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using CustomNetworking;
using System.Threading.Tasks;

namespace GradingTester
{
    [TestClass]
    public class StringSocketTest
    {
        /// <summary>
        /// Opens and returns (with out parameters) a pair of communicating sockets.
        /// </summary>
        private static void OpenSockets(int port, out StringSocketListener server, out StringSocket s1, out SS s2)
        {
            server = new StringSocketListener(port, new UTF8Encoding());
            server.Start();
            SSClient client = new SSClient("localhost", port, new UTF8Encoding());
            s1 = server.AcceptStringSocket();
            s2 = client.Client;
        }

        /// <summary>
        /// Closes stuff down
        /// </summary>
        private static void CloseSockets(StringSocketListener server, StringSocket s1, SS s2)
        {
            try
            {
                s1.Shutdown(SocketShutdown.Both);
            }
            finally
            {
            }
            try
            {
                s2.Shutdown(SocketShutdown.Both);
            }
            finally
            {
            }
            try
            {
                s1.Close();
            }
            finally
            {
            }
            try
            {
                s2.Close();
            }
            finally
            {
            }
            try
            {
                server.Stop();
            }
            finally
            {
            }
        }

        /// <summary>
        /// Tests whether StringSocket can receive a line of text
        /// </summary>
        [TestMethod()] 
        public void Test1()
        {
            new Test1ClassReceiver().run(4001);
        }

        public class Test1ClassReceiver
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    sender.BeginSend("Hello\n", (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual("Hello", line);
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Tests whether StringSocket can send a line of text
        /// </summary>
        [TestMethod()]
        public void Test2()
        {
            new Test2ClassSender().run(4002);
        }

        public class Test2ClassSender
        {
            public void run(int port)
            {
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    sender.BeginSend("Hello\n", (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual("Hello", line);
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Like Test1, make sure payload comes through.
        /// </summary>
        [TestMethod()]
        public void Test3()
        {
            new Test3ClassReceiver().run(4003);
        }

        public class Test3ClassReceiver
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                object payload = null;
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    sender.BeginSend("Hello\n", (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { payload = p; mre.Set(); }, "Payload");
                    mre.WaitOne();
                    Assert.AreEqual("Payload", payload);
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test2, make sure the payload comes through
        /// </summary>
        [TestMethod()]
        public void Test4()
        {
            new Test4ClassSender().run(4004);
        }

        public class Test4ClassSender
        {
            public void run(int port)
            {
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                object payload = null;
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    sender.BeginSend("Hello\n", (s, p) => { payload = p; mre.Set(); }, "Payload");
                    mre.WaitOne();
                    Assert.AreEqual("Payload", payload);
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Like Test1, but send one character at a time.
        /// </summary>
        [TestMethod()]
        public void Test5()
        {
            new Test5ClassReceiver().run(4005);
        }

        public class Test5ClassReceiver
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    foreach (char c in "Hello\n")
                    {
                        sender.BeginSend(c.ToString(), (s, p) => { }, null);
                    }
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual("Hello", line);
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test2, but send one character at a time.
        /// </summary>
        [TestMethod()]
        public void Test6()
        {
            new Test6ClassSender().run(4006);
        }

        public class Test6ClassSender
        {
            public void run(int port)
            {
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    foreach (char c in "Hello\n")
                    {
                        sender.BeginSend(c.ToString(), (s, p) => { }, null);
                    }
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual("Hello", line);
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Like Test1, but send a very long string.
        /// </summary>
        [TestMethod()]
        public void Test7()
        {
            new Test7ClassReceiver().run(4007);
        }

        public class Test7ClassReceiver
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    StringBuilder text = new StringBuilder();
                    for (int i = 0; i < 100000; i++)
                    {
                        text.Append(i);
                    }
                    String str = text.ToString();
                    text.Append('\n');
                    sender.BeginSend(text.ToString(), (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual(str, line);
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test2, but send a very long string.
        /// </summary>
        [TestMethod()]
        public void Test8()
        {
            new Test8ClassSender().run(4008);
        }

        public class Test8ClassSender
        {
            public void run(int port)
            {
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    StringBuilder text = new StringBuilder();
                    for (int i = 0; i < 100000; i++)
                    {
                        text.Append(i);
                    }
                    String str = text.ToString();
                    text.Append('\n');
                    sender.BeginSend(text.ToString(), (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual(str, line);
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Send multiple lines, make sure they're received in order.
        /// </summary>
        [TestMethod()]
        public void Test9()
        {
            new Test9ClassReceiver().run(4009);
        }

        public class Test9ClassReceiver
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String[] lines = new String[LIMIT];
                ManualResetEvent mre = new ManualResetEvent(false);
                int count = 0;

                try
                {
                    for (int i = 0; i < LIMIT; i++)
                    {
                        receiver.BeginReceive((s, p) => { lines[(int)p] = s; Interlocked.Increment(ref count); }, i);
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    if (!SpinWait.SpinUntil(() => count == LIMIT, 5000))
                    {
                        Assert.Fail();
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i.ToString(), lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test9, but the class being tested does the sending.
        /// </summary>
        [TestMethod()]
        public void Test10()
        {
            new Test10ClassSender().run(4010);
        }

        public class Test10ClassSender
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                String[] lines = new String[LIMIT];
                ManualResetEvent mre = new ManualResetEvent(false);
                int count = 0;

                try
                {
                    for (int i = 0; i < LIMIT; i++)
                    {
                        receiver.BeginReceive((s, p) => { lines[(int)p] = s; Interlocked.Increment(ref count); }, i);
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }

                    if (!SpinWait.SpinUntil(() => count == LIMIT, 5000))
                    {
                        Assert.Fail();
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i.ToString(), lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Like Test9, except the receive calls are made on separate threads.
        /// </summary>
        [TestMethod()]
        public void Test11()
        {
            new Test11ClassReceiverr().run(4011);
        }

        public class Test11ClassReceiverr
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                List<int> lines = new List<int>();
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    for (int i = 0; i < LIMIT; i++)
                    {
                        ThreadPool.QueueUserWorkItem(x =>
                            receiver.BeginReceive((s, p) => { lock (lines) { lines.Add(Int32.Parse(s)); } }, null));
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    if (!SpinWait.SpinUntil(() => { lock (lines) { return lines.Count == LIMIT; } }, 5000))
                    {
                        Assert.Fail();
                    }

                    lines.Sort();
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i, lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test10, except the send calls are made on separate threads.
        /// </summary>
        [TestMethod()]
        public void Test12()
        {
            new Test12ClassSender().run(4012);
        }

        public class Test12ClassSender
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                List<int> lines = new List<int>();

                try
                {
                    for (int i = 0; i < LIMIT; i++)
                    {
                        receiver.BeginReceive((s, p) => { lock (lines) { lines.Add(Int32.Parse(s)); } }, null);
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        String str = i.ToString();
                        ThreadPool.QueueUserWorkItem(x =>
                            sender.BeginSend(str + "\n", (s, p) => { }, null));
                    }
                    if (!SpinWait.SpinUntil(() => { lock (lines) { return lines.Count == LIMIT; } }, 5000))
                    {
                        Assert.Fail();
                    }

                    lines.Sort();
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i, lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Like Test9, except the sends and receives are interleaved.
        /// </summary>
        [TestMethod()]
        public void Test13()
        {
            new Test13ClassReceiverr().run(4013);
        }

        public class Test13ClassReceiverr
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                int count = LIMIT;
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                List<int> lines = new List<int>();
                for (int i = 0; i < LIMIT; i++)
                {
                    lines.Add(-1);
                }

                try
                {
                    for (int i = 0; i < LIMIT / 4; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); } Interlocked.Decrement(ref count); }, null);
                    }
                    for (int i = 0; i < LIMIT / 2; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    for (int i = LIMIT / 4; i < 3 * LIMIT / 4; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); } Interlocked.Decrement(ref count); }, null);
                    }
                    for (int i = LIMIT / 2; i < LIMIT; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    for (int i = 3 * LIMIT / 4; i < LIMIT; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); } Interlocked.Decrement(ref count); }, null);
                    }

                    if (!SpinWait.SpinUntil(() => count == 0, 5000))
                    {
                        Assert.Fail();
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i, lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test10, except the sends and receives are interleaved.
        /// </summary>
        [TestMethod()]
        public void Test14()
        {
            new Test14ClassSender().run(4014);
        }

        public class Test14ClassSender
        {
            public void run(int port)
            {
                int LIMIT = 1000;
                int count = LIMIT;
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                List<int> lines = new List<int>();
                for (int i = 0; i < LIMIT; i++)
                {
                    lines.Add(-1);
                }

                try
                {
                    for (int i = 0; i < LIMIT / 4; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); } Interlocked.Decrement(ref count); }, null);
                    }
                    for (int i = 0; i < LIMIT / 2; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    for (int i = LIMIT / 4; i < 3 * LIMIT / 4; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); } Interlocked.Decrement(ref count); }, null);
                    }
                    for (int i = LIMIT / 2; i < LIMIT; i++)
                    {
                        sender.BeginSend(i.ToString() + "\n", (s, p) => { }, null);
                    }
                    for (int i = 3 * LIMIT / 4; i < LIMIT; i++)
                    {
                        int j = i;
                        receiver.BeginReceive((s, p) => { lock (lines) { lines[j] = Int32.Parse(s); Interlocked.Decrement(ref count); } }, null);
                    }

                    if (!SpinWait.SpinUntil(() => count == 0, 5000))
                    {
                        Assert.Fail();
                    }
                    for (int i = 0; i < LIMIT; i++)
                    {
                        Assert.AreEqual(i, lines[i]);
                    }
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Blocks the receive callbacks, makes sure receiving keeps working.
        /// </summary>
        [TestMethod()]
        public void Test15()
        {
            new Test15ClassReceiverr().run(4015);
        }

        public class Test15ClassReceiverr
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                int LIMIT = 2;

                try
                {
                    int count = LIMIT;

                    for (int i = 0; i < LIMIT; i++)
                    {
                        sender.BeginSend("Hello\n", (s, p) => { }, null);
                    }

                    for (int i = 0; i < LIMIT; i++)
                    {
                        Task.Run(
                            () => receiver.BeginReceive((s, p) => { Interlocked.Decrement(ref count); while (true) ; }, null));
                    }
                   if (!SpinWait.SpinUntil(() => count == 0, 5000))
                   {
                       Assert.Fail();
                   }
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Blocks the send callbacks, makes sure receiving keeps working.
        /// </summary>
        [TestMethod()]
        public void Test16()
        {
            new Test16ClassSender().run(4016);
        }

        public class Test16ClassSender
        {
            public void run(int port)
            {
                StringSocket sender;
                SS receiver;
                StringSocketListener server;
                OpenSockets(port, out server, out sender, out receiver);
                int LIMIT = 2;

                try
                {
                    int count1 = LIMIT;
                    int count2 = LIMIT;

                    for (int i = 0; i < LIMIT; i++)
                    {
                        sender.BeginSend("Hello\n", (s, p) => { Interlocked.Decrement(ref count1); while (true) ; }, null);
                    }

                    for (int i = 0; i < LIMIT; i++)
                    {
                        receiver.BeginReceive((s, p) => { Interlocked.Decrement(ref count2); }, null);
                    }

                    if (!SpinWait.SpinUntil(() => count1 == 0 && count2 == 0, 5000))
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    CloseSockets(server, sender, receiver);
                }
            }
        }

        /// <summary>
        /// Tests whether StringSocket deals properly with requests for strings composed
        /// from a specific number of bytes.
        /// </summary>
        [TestMethod()]
        public void Test17()
        {
            new Test17ClassReceiverr().run(4017);
        }

        public class Test17ClassReceiverr
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    sender.BeginSend("This is a test\n", (s, p) => { }, null);
                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 11);
                    mre.WaitOne();
                    Assert.AreEqual("This is a t", line);
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }

        /// <summary>
        /// Like Test17 but more comprehensive.
        /// </summary>
        [TestMethod()]
        public void Test18()
        {
            new Test18ClassReceiverr().run(4018);
        }

        public class Test18ClassReceiverr
        {
            public void run(int port)
            {
                StringSocket receiver;
                SS sender;
                StringSocketListener server;
                OpenSockets(port, out server, out receiver, out sender);
                String line = "";
                ManualResetEvent mre = new ManualResetEvent(false);

                try
                {
                    // Note that there's an alpha in the first line of this string.
                    // It has a two byte encoding in UTF-8
                    sender.BeginSend("This is α test\nanother test\none more test\n", (s, p) => { }, null);

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 4);
                    mre.WaitOne();
                    Assert.AreEqual("This", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 13);
                    mre.WaitOne();
                    Assert.AreEqual(" is α test\na", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 3);
                    mre.WaitOne();
                    Assert.AreEqual("not", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 6);
                    mre.WaitOne();
                    Assert.AreEqual("her te", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null);
                    mre.WaitOne();
                    Assert.AreEqual("st", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 10);
                    mre.WaitOne();
                    Assert.AreEqual("one more t", line);
                    mre.Reset();

                    receiver.BeginReceive((s, p) => { line = s; mre.Set(); }, null, 4);
                    mre.WaitOne();
                    Assert.AreEqual("est\n", line);
                    mre.Reset();
                }
                finally
                {
                    CloseSockets(server, receiver, sender);
                }
            }
        }
    }
}
