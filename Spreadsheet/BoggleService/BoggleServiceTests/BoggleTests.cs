using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.InteropServices.Expando;
using System.Dynamic;
using System.Threading.Tasks;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        [TestMethod]
        public void TestCreateUser()
        {
            dynamic temp = new ExpandoObject();
            temp.Nickname = "test";

            Response r = client.DoPostAsync("users", temp).Result;
            Assert.AreEqual(Created, r.Status);

            temp.Nickname = "";
   
            r = client.DoPostAsync("users", temp).Result;
            Assert.AreEqual(Forbidden, r.Status);

            r = client.DoGetAsync("games/G0").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void TestJoinGame()
        {
            dynamic user1 = new ExpandoObject();
            dynamic user2 = new ExpandoObject();
            dynamic temp = new ExpandoObject();


            temp.Nickname = "test1";

            Response r = client.DoPostAsync("users", temp).Result;
            user1.UserToken = r.Data.UserToken;

            Assert.AreEqual(Created, r.Status);


            temp.Nickname = "test2";

            r = client.DoPostAsync("users", temp).Result;
            user2.UserToken = r.Data.UserToken;

            Assert.AreEqual(Created,r.Status);


            temp = new ExpandoObject();
            temp.UserToken = user1.UserToken;
            temp.TimeLimit = 40;

            r = client.DoPostAsync("games", temp).Result;
            Assert.AreEqual(Accepted, r.Status);


            string GameID = (string)r.Data.GameID;

            r = client.DoGetAsync("games/" + GameID).Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("pending", (string)r.Data.GameState);


            temp = new ExpandoObject();
            temp.UserToken = user2.UserToken;
            temp.TimeLimit = 40;

            r = client.DoPostAsync("games", temp).Result;
            Assert.AreEqual(Created, r.Status);
            Assert.AreEqual(GameID , (string)r.Data.GameID);
        }

        [TestMethod]
        public void TestCancelJoin()
        {
            dynamic user1 = new ExpandoObject();
            dynamic user2 = new ExpandoObject();
            dynamic temp = new ExpandoObject();

            temp.Nickname = "test1";
            Response r = client.DoPostAsync("users", temp).Result;
            user1.UserToken = r.Data.UserToken;

            temp = new ExpandoObject();
            temp.UserToken = user1.UserToken;
            temp.TimeLimit = 40;

            r = client.DoPostAsync("games", temp).Result;

            temp = new ExpandoObject();
            temp.UserToken = user1.UserToken;
            r = client.DoPutAsync(temp, "games").Result;
            Assert.AreEqual(OK, r.Status);

            temp.Nickname = "test2";
            r = client.DoPostAsync("users", temp).Result;
            user2.UserToken = r.Data.UserToken;
        }
    }
}
