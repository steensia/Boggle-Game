using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        // This amounts to a "poor man's" database.  The state of the service is
        // maintained in users and items.  The sync object is used
        // for synchronization (because multiple threads can be running
        // simultaneously in the service).  The entire state is lost each time
        // the service shuts down, so eventually we'll need to port this to
        // a proper database.
        private readonly static Dictionary<String, UserInfo> users = new Dictionary<String, UserInfo>();
        private readonly static Dictionary<String, PendingGame> pendingUsers = new Dictionary<String, PendingGame>();
        private readonly static Dictionary<String, Player> players = new Dictionary<String, Player>();
        private readonly static Dictionary<String, TimeInfo> userTime = new Dictionary<String, TimeInfo>();
        private readonly static Dictionary<String, WordPlay> wordPlay = new Dictionary<String, WordPlay>();
        private readonly static Dictionary<String, GameStatus> game = new Dictionary<String, GameStatus>();
        private static readonly object sync = new object();

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        public string CreateUser(UserInfo user)
        {
            lock (sync)
            {
                if (user.Nickname == null || user.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    string userToken = Guid.NewGuid().ToString();
                    //UserInfo newUser = new UserInfo();
                    //{
                    //    newUser.Nickname = user.Nickname;
                    //    newUser.UserToken = userToken;
                    //};
                    user.UserToken = userToken;
                    if (!users.ContainsKey(user.Nickname))
                        users.Add(userToken, user);
                    SetStatus(Created);
                    return userToken;
                }
            }
        }

        public string JoinGame(TimeInfo user)
        {
            if (user.UserToken == null || user.TimeLimit < 5 || user.TimeLimit > 120)
            {
                SetStatus(Forbidden);
                return null;
            }
            else if (users.ContainsKey(user.UserToken))
            {
                SetStatus(Conflict);
                return null;
            }
            else if (user.Status.GameState == "pending")
            {
                user.Status.Player2.UserToken = user.UserToken;
                user.Status.GameState = "active";
                user.Status.TimeLimit = (user.Status.Player1.Time + user.Status.Player2.Time) / 2;
                GameStatus newGame = new GameStatus();
                {
                    newGame.GameState = "pending";
                    newGame.Board = null;
                    newGame.TimeLeft = 0;
                    newGame.TimeLimit = 0;
                    newGame.Player1 = null;
                    newGame.Player2 = null;
                };
                string gameID = Guid.NewGuid().ToString();
                SetStatus(Created);
                return gameID;
            }
            else
            {
                GameStatus newGame = new GameStatus();
                {
                    newGame.GameState = "pending";
                    newGame.Board = null;
                    newGame.TimeLeft = 0;
                    newGame.TimeLimit = user.Status.TimeLimit;
                    newGame.Player1 = user.Status.Player1;
                    newGame.Player2 = null;
                };
                string gameID = Guid.NewGuid().ToString();
                SetStatus(Accepted);
                return gameID;
            }
        }

        public void CancelJoin(string user)
        {
            if (user == null || !game[user].GameState.Equals("pending"))
            {
                SetStatus(Forbidden);
            }
            else
            {
                if(game != null)
                {
                    game.Remove(user);
                }
                SetStatus(OK);
            }
        }
    }
}
