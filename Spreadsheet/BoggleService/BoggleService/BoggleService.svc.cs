using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly static Dictionary<String, Player> players = new Dictionary<String, Player>();
        private readonly static Dictionary<String, GameStatus> games = new Dictionary<String, GameStatus>();
        private readonly static Dictionary<String, long> times = new Dictionary<String, long>();
        private readonly static HashSet<String> validWords = new HashSet<String>();
        private readonly static object sync = new object();

        private static BoggleBoard board;
        private static string pendingGame;

        public BoggleService()
        {
            lock (sync)
            {
                if (pendingGame == null)
                {
                    using (StreamReader words = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
                    {
                        string temp;
                        while ((temp = words.ReadLine()) != null)
                        {
                            validWords.Add(temp);
                        }
                    }
                    games.Add(pendingGame = uniqueGameID(), new GameStatus());
                }
            }
        }

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
        public User CreateUser(Username u)
        {
            lock (sync)
            {
                if (u.Nickname == null || u.Nickname.Trim().Length == 0 || u.Nickname.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    // Generate usertoken
                    User user = new User();
                    user.UserToken = Guid.NewGuid().ToString();

                    // Create new player and add usertoken and nickname
                    Player player = new Player();
                    player.Nickname = u.Nickname;
                    player.UserToken = user.UserToken;

                    players.Add(user.UserToken, player);

                    SetStatus(Created);
                    return user;
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
        public GameRoom JoinGame(GameInfo g)
        {
            lock (sync)
            {
                // Create pending game
                if (g.UserToken == null || !players.ContainsKey(g.UserToken) || g.TimeLimit < 5 || g.TimeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else if (games.TryGetValue(pendingGame, out GameStatus game) && players.TryGetValue(g.UserToken, out Player player))
                {
                    // Conflict if same user joins game
                    if (game.Player1 != null && g.UserToken.Equals(game.Player1.UserToken))
                    {
                        SetStatus(Conflict);
                        return null;
                    }
                    else if (game.Player1 == null)
                    {
                        // Create new game 
                        GameRoom room = new GameRoom();
                        room.GameID = pendingGame;

                        games.TryGetValue(pendingGame, out GameStatus newGame);
                        players.TryGetValue(g.UserToken, out Player player1);

                        games.Remove(pendingGame);

                        // Add usertoken as first player in pending game  
                        newGame.Player1 = player1;
                        newGame.TimeLimit = g.TimeLimit;
                        newGame.GameState = "pending";

                        games.Add(pendingGame, newGame);

                        SetStatus(Accepted);
                        return room;
                    }
                    else
                    {
                        GameRoom room = new GameRoom();
                        room.GameID = pendingGame;

                        games.TryGetValue(pendingGame, out GameStatus currentGame);

                        games.Remove(pendingGame);

                        // Adds usertoken as second player in active game
                        players.TryGetValue(g.UserToken, out Player player2);
                        currentGame.Player2 = player2;
                        currentGame.Player1.WordsPlayed = new List<Words>();
                        currentGame.Player2.WordsPlayed = new List<Words>();
                        currentGame.TimeLimit = (game.TimeLimit + g.TimeLimit) / 2;
                        times.Add(pendingGame, System.DateTime.UtcNow.Ticks);
                        currentGame.TimeLeft = currentGame.TimeLimit;
                        currentGame.GameState = "active";

                        board = new BoggleBoard();
                        currentGame.Board = board.ToString();

                        games.Add(pendingGame, currentGame);

                        // Create new pending game.
                        games.Add(pendingGame = uniqueGameID(), new GameStatus());

                        SetStatus(Created);
                        return room;
                    }
                }
                return null;
            }
        }

        /// Cancel a pending request to join a game.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        public void CancelJoin(User u)
        {
            lock (sync)
            {
                if (u.UserToken == null || !players.ContainsKey(u.UserToken) || pendingGame == null || !games.TryGetValue(pendingGame, out GameStatus g) || g.Player1 == null || !u.UserToken.Equals(g.Player1.UserToken))
                {
                    SetStatus(Forbidden);
                }
                else
                {
                    games.Remove(pendingGame);
                    g.Player1 = null;
                    games.Add(pendingGame, g);

                    SetStatus(OK);
                }
            }
        }

        /// Play a word in a game.
        /// If Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid, or if UserToken is not a player in the 
        /// game identified by GameID, responds with response code 403 (Forbidden).
        /// Otherwise, if the game state is anything other than "active", responds with response code 409 (Conflict).
        /// Otherwise, records the trimmed Word as being played by UserToken in the game identified by GameID. Returns the score for Word in the context of the 
        /// game (e.g. if Word has been played before the score is zero). Responds with status 200 (OK). Note: The word is not case sensitive.
        public WordScore PlayWord(WordToPlay w, string gameID)
        {
            lock (sync)
            {

                if (w.Word == null || w.Word.Trim().Length > 30 || w.UserToken == null || !players.ContainsKey(w.UserToken) || gameID == null || !(games.TryGetValue(gameID, out GameStatus temp) || (temp.Player1.UserToken != w.UserToken || temp.Player2.UserToken != w.UserToken)))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    games.TryGetValue(gameID, out GameStatus g);
                    if (!g.GameState.Equals("active"))
                    {
                        SetStatus(Conflict);
                        return null;
                    }
                    else
                    {
                        updateTime(gameID);

                        Words wordPlay = new Words();
                        wordPlay.Word = w.Word;

                        // Need to work on this
                        board = new BoggleBoard(g.Board);
                        players.TryGetValue(w.UserToken, out Player p);

                        wordPlay.Score = board.CanBeFormed(wordPlay.Word) ? GetScore(wordPlay.Word) : -1;

                        if (wordPlay.Word.Length < 3)
                            wordPlay.Score = 0;
                        else
                            foreach (Words word in p.WordsPlayed)
                                if (word.Word.ToUpper().Equals(wordPlay.Word.ToUpper())) wordPlay.Score = 0;

                        WordScore scoreWord = new WordScore();
                        scoreWord.Score = wordPlay.Score;

                        p.Score += wordPlay.Score;
                        p.WordsPlayed.Add(wordPlay);

                        SetStatus(OK);
                        return scoreWord;
                    }
                }
            }
        }

        /// Get game status information.
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below. Note that the information returned depends on whether "Brief=yes" 
        /// was included as a parameter as well as on the state of the game. Responds with status code 200 (OK). Note: The Board and Words are not case sensitive.
        public GameStatus GameStatus(string gameID, string brief)
        {
            lock (sync)
            {
                if (gameID == null || !games.ContainsKey(gameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    updateTime(gameID);
                    games.TryGetValue(gameID, out GameStatus temp);
                    GameStatus game = new GameStatus();
 

                    // Display pending status
                    if (temp.GameState.Equals("pending"))
                    {
                        game.GameState = temp.GameState;

                        SetStatus(OK);
                        return game;
                    }
                    else if ((temp.GameState.Equals("active")|| temp.GameState.Equals("completed")) && "yes".Equals(brief))
                    {
                        game.GameState = temp.GameState;
                        game.TimeLeft = temp.TimeLeft;

                        game.Player1 = new Player();
                        game.Player1.Score = temp.Player1.Score;

                        game.Player2 = new Player();
                        game.Player2.Score = temp.Player2.Score;

                        return game;
                    }
                    else if (temp.GameState.Equals("active"))
                    {
                        game.GameState = temp.GameState;
                        game.Board = temp.Board;
                        game.TimeLimit = temp.TimeLimit;
                        game.TimeLeft = temp.TimeLeft;

                        game.Player1 = new Player();
                        game.Player1.Nickname = temp.Player1.Nickname;
                        game.Player1.Score = temp.Player1.Score;

                        game.Player2 = new Player();
                        game.Player2.Nickname = temp.Player2.Nickname;
                        game.Player2.Score = temp.Player2.Score;

                        SetStatus(OK);
                        return game;

                    }
                    else if (temp.GameState.Equals("completed"))
                    {
                        game.GameState = temp.GameState;
                        game.Board = temp.Board;
                        game.TimeLimit = temp.TimeLimit;
                        game.TimeLeft = 0;

                        game.Player1 = new Player();
                        game.Player1.Nickname = temp.Player1.Nickname;
                        game.Player1.Score = temp.Player1.Score;

                        game.Player2 = new Player();
                        game.Player2.Nickname = temp.Player2.Nickname;
                        game.Player2.Score = temp.Player2.Score;

                        game.Player1.WordsPlayed = new List<Words>(temp.Player1.WordsPlayed);
                        game.Player2.WordsPlayed = new List<Words>(temp.Player2.WordsPlayed); ;

                        SetStatus(OK);
                        return game;
                    }

                    return temp;
                }
            }
        }

        /// <summary>
        /// Private helper method to generate game ID
        /// </summary>
        private string uniqueGameID()
        {
            string i = null;
            while (games.Keys.Contains(i = "G" + (new object().GetHashCode() % 512 + 1))) ;
            return i;
        }

        private void updateTime(string gameID)
        {
            games.TryGetValue(gameID, out GameStatus temp);
            if (!times.TryGetValue(gameID, out long startTime)) return;

            temp.TimeLeft = temp.TimeLimit + (int)(startTime - System.DateTime.UtcNow.Ticks) / 10000000;

            if (temp.TimeLeft <= 0)
            {
                temp.TimeLeft = 0;
                temp.GameState = "completed";
            }

            games.Remove(gameID);
            games.Add(gameID, temp);
        }
        private int GetScore(string s)
        {
            switch (s.Length)
            {
                case 0:
                case 1:
                case 2:
                    return 0;
                case 3:
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 3;
                case 7:
                    return 5;
                default:
                    return 11;
            }
        }
    }
}
