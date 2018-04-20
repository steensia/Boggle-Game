using Boggle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyBoggleService
{
    class Program
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
    }
}
