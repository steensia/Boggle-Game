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
    class RequestHandler
    {
        static void Main(string[] args)
        {
            HttpStatusCode status;
            
            Username name = new Username { Nickname = "Joe" };
            BoggleService service = new BoggleService();
            User user = service.CreateUser(name, out status);
            Console.WriteLine(user.UserToken);
            Console.WriteLine(status.ToString());

            // This is our way of preventing the main thread from
            // exiting while the server is in use
            Console.ReadLine();
        }

        // Copy Pasted Code
        private StringSocket ss;

        private string firstLine;
        private int contentLength;
        private static readonly Regex makeUserPattern = new Regex(@"^POST /BoggleService.svc/users HTTP");
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
            if (makeUserPattern.IsMatch(firstLine))
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
                ss.BeginSend(result, (x, y) => { ss.Shutdown(SocketShutdown.Both); }, null);
            }
        }
    }
}
