using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly static Dictionary<String, TimeInfo> time = new Dictionary<String, TimeInfo>();
        private readonly static Dictionary<String, Player> player = new Dictionary<String, Player>();
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

        /// <summary>
        /// Create a new user.
        /// If Nickname is null, or is empty when trimmed, or contains more than 50 characters when trimmed, responds with status 403 (Forbidden).
        /// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname. The returned UserToken should be used to identify the 
        /// user in subsequent requests. Responds with status 201 (Created).
        /// </summary>
        public string CreateUser(UserInfo u)
        {
            lock (sync)
            {
                if (u.Nickname == null || u.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    string userToken = Guid.NewGuid().ToString();
                    users.Add(userToken, u);
                    TimeInfo temp = new TimeInfo();
                    {
                        temp.UserToken = userToken;
                    }
                    time.Add(userToken, temp);
                    SetStatus(Created);
                    return userToken;
                }
            }
        }

        /// <summary>
        /// Join a game.
        /// If UserToken is invalid, TimeLimit < 5, or TimeLimit > 120, responds with status 403 (Forbidden).
        /// Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict). 
        /// Otherwise, if there is already one player in the pending game, adds UserToken as the second player. The pending game becomes active and 
        /// a new pending game with no players is created. The active game's time limit is the integer average of the time limits requested by the 
        /// two players. Returns the new active game's GameID (which should be the same as the old pending game's GameID). Responds with status 201 (Created).
        /// Otherwise, adds UserToken as the first player of the pending game, and the TimeLimit as the pending game's requested time limit. Returns the 
        /// pending game's GameID. Responds with status 202 (Accepted).
        /// </summary>
        public string JoinGame(TimeInfo t)
        {
            if (t.UserToken == null || !users.ContainsKey(t.UserToken) || t.TimeLimit < 5 || t.TimeLimit > 120)
            {
                SetStatus(Forbidden);
                return null;
            }
            else if (game.Count > 0)
            {
                SetStatus(Conflict);
                return null;
            }
            else if (game.Count < 0)
            {
                string gameID = Guid.NewGuid().ToString();
                GameStatus currentGame = new GameStatus();
                {
                    currentGame.GameState = "active";
                    var first = users.First();
                    time.TryGetValue(first.Key, out TimeInfo temp);
                    currentGame.TimeLimit = (t.TimeLimit + temp.TimeLimit) / 2;
                };

                currentGame.Player2.UserToken = t.UserToken;

                GameStatus newGame = new GameStatus();
                {
                    newGame.GameState = "pending";
                };
                game.Add(gameID, currentGame);
                time.Add(t.TimeLimit.ToString(), t);
                SetStatus(Created);
                return gameID;
            }
            else
            {
                string gameID = Guid.NewGuid().ToString();
                GameStatus currentGame = new GameStatus();
                {
                    currentGame.GameState = "pending";
                    currentGame.Player1.UserToken = t.UserToken;
                    currentGame.TimeLimit = t.TimeLimit;
                };
                game.Add(gameID, currentGame);
                time.Add(t.TimeLimit.ToString(), t);
                SetStatus(Accepted);
                return gameID;
            }
        }

        /// Cancel a pending request to join a game.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        public void CancelJoin(TimeInfo t)
        {
            if (t.UserToken == null || !users.ContainsKey(t.UserToken))
            {
                SetStatus(Forbidden);
            }
            else
            {
                time.Remove(t.UserToken);
                SetStatus(OK);
            }
        }

        /// Play a word in a game.
        /// If Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid, or if UserToken is not a player in the 
        /// game identified by GameID, responds with response code 403 (Forbidden).
        /// Otherwise, if the game state is anything other than "active", responds with response code 409 (Conflict).
        /// Otherwise, records the trimmed Word as being played by UserToken in the game identified by GameID. Returns the score for Word in the context of the 
        /// game (e.g. if Word has been played before the score is zero). Responds with status 200 (OK). Note: The word is not case sensitive.
        public int PlayWord(Player p, string gameID)
        {
            if (p.PlayWord == null || p.PlayWord.Trim().Length > 30 || p.GameID == null 
                || p.UserToken == null || !users.ContainsKey(p.UserToken) || !game.ContainsKey(p.UserToken))
            {
                SetStatus(Forbidden);
                return 0;
            }
            else if (game[p.UserToken].GameState.Equals("active"))
            {
                SetStatus(Conflict);
                return 0;
            }
            else
            {
                Int32.TryParse(Guid.NewGuid().ToString(), out int score);
                SetStatus(OK);
                return score;
            }
        }

        /// Get game status information.
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below. Note that the information returned depends on whether "Brief=yes" 
        /// was included as a parameter as well as on the state of the game. Responds with status code 200 (OK). Note: The Board and Words are not case sensitive.
        public IList<GameStatus> GameStatus(string gameID, string brief)
        {
            if (gameID == null || !game.ContainsKey(gameID))
            {
                SetStatus(Forbidden);
                return null;
            }
            else
            {
                SetStatus(OK);
                IList<GameStatus> stats = new List<GameStatus>();
                foreach (var status in game.Values)
                {
                    // Display the following: GameState, TimeLeft, Player1/Player2 Scores
                    if ((status.GameState.Equals("active") || status.GameState.Equals("completed")) && brief.Equals("yes"))
                    {

                    }
                    // Display the following: GameState, Board, TimeLimit, TimeLeft, Player1/Player2 Nicknames/Scores
                    else if (status.GameState.Equals("active") && !brief.Equals("yes"))
                    {

                    }
                    // Display the following: GameState, Board, TimeLimit, TimeLeft, Player1/Player2 Nicknames/Scores/WordsPlayed
                    else if (status.GameState.Equals("completed") && !brief.Equals("yes"))
                    {

                    }
                }
                return stats;
            }
        }
    }
}
