using Boggle;
using CustomNetworking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyBoggleService
{
    public class MyServer
    {
        public static void Main ()
        {
            StringSocketListener server = new StringSocketListener(60000, Encoding.UTF8);
            server.Start();
            server.BeginAcceptStringSocket(ConnectionMade, server);
            Console.ReadLine();            
        }

        private static void ConnectionMade(StringSocket ss, object payload)
        {
            StringSocketListener server = (StringSocketListener)payload;
            server.BeginAcceptStringSocket(ConnectionMade, server);
            new RequestHandler(ss);
        }

        private class RequestHandler
        {


            // Copy Pasted Code
            private StringSocket ss;

            private string firstLine;
            private int contentLength;
            private static readonly Regex CreateUserPattern = new Regex(@"^POST /BoggleService.svc/users HTTP");
            private static readonly Regex JoinGamePattern = new Regex(@"^POST /BoggleService.svc/games HTTP");
            private static readonly Regex CancelJoinPattern = new Regex(@"^PUT /BoggleService.svc/games HTTP");
            private static readonly Regex PlayeWordPattern = new Regex(@"^PUT /BoggleService.svc/games/[0-9]* HTTP");
            private static readonly Regex Pattern = new Regex(@"^GET /BoggleService.svc/games/[0-9]*(/brief)? HTTP");
            private static readonly Regex contentLengthPattern = new Regex(@"^content-length: (\d+)", RegexOptions.IgnoreCase);

            public RequestHandler(StringSocket ss)
            {
                this.ss = ss;
                contentLength = 0;
                ss.BeginReceive(ReadLines, null);
            }


            private void ReadLines(string line, object p)
            {
                if (line.Trim().Length == 0 && contentLength > 0)
                {
                    ss.BeginReceive(ProcessRequest, null, contentLength);
                }
                else if (line.Trim().Length == 0)
                {
                    ProcessRequest(null);
                }
                else if (firstLine != null)
                {
                    Match m = contentLengthPattern.Match(line);
                    if (m.Success)
                    {
                        contentLength = int.Parse(m.Groups[1].ToString());
                    }
                    ss.BeginReceive(ReadLines, null);
                }
                else
                {
                    firstLine = line;
                    ss.BeginReceive(ReadLines, null);
                }
            }
            private void ProcessRequest(string line, object p = null)
            {
                if (CreateUserPattern.IsMatch(firstLine))
                {
                    Username n = JsonConvert.DeserializeObject<Username>(line);
                    User user = new BoggleService().CreateUser(n, out HttpStatusCode status);
                    String result = "HTTP/1.1 " + (int)status + " " + status + "\r\n";
                    if ((int)status / 100 == 2)
                    {
                        string res = JsonConvert.SerializeObject(user);
                        result += "Content-Length: " + Encoding.UTF8.GetByteCount(res) + "\r\n";
                        result += res;
                    }
                    ss.BeginSend(result, (x, y) => { Console.WriteLine("Reached Callback"); } ,null);
                }
            }
        }
    }
}
