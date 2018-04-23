using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
        private static HashSet<String> validWords;
        private static string BoggleDB;

        static BoggleService()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }
        public BoggleService()
        {
            if (validWords == null)
            {
                validWords = new HashSet<string>();
                using (StreamReader words = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
                {
                    string temp;
                    while ((temp = words.ReadLine()) != null)
                    {
                        validWords.Add(temp);
                    }
                }
                addNewGame();
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
            if (u.Nickname == null || u.Nickname.Trim().Length == 0 || u.Nickname.Trim().Length > 50)
            {
                SetStatus(Forbidden);
                return null;
            }

            User user = new User();
            user.UserToken = Guid.NewGuid().ToString();

            addUser(user.UserToken, u.Nickname.Trim());

            SetStatus(Created);
            return user;
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
            if (g.UserToken == null || g.TimeLimit < 5 || g.TimeLimit > 120 || !tryGetPlayer(g.UserToken, out Player player) || !tryGetGame(getPendingGame(), out GameStatus game))
            {
                SetStatus(Forbidden);
                return null;
            }

            // Conflict if same user joins game
            if (game.Player1.UserToken != null && g.UserToken.Equals(game.Player1.UserToken))
            {
                SetStatus(Conflict);
                return null;
            }
            else if (game.Player1.UserToken == null)
            {
                int pendingGame = getPendingGame();
                // Create new game 
                GameRoom room = new GameRoom();
                room.GameID = "" + pendingGame;

                // Add information to first player in pending game  
                game.Player1.UserToken = player.UserToken;
                game.TimeLimit = g.TimeLimit;
                //  game.StartTime = System.DateTime.UtcNow;
                game.GameState = "pending";

                updateGame(pendingGame, game);

                SetStatus(Accepted);
                return room;
            }
            else
            {
                int pendingGame = getPendingGame();

                GameRoom room = new GameRoom();
                room.GameID = "" + pendingGame;


                // Add first and second player, and time information to GameStatus

                game.TimeLimit = (game.TimeLimit + g.TimeLimit) / 2;
                game.TimeLeft = game.TimeLimit;
                game.StartTime = System.DateTime.UtcNow;
                game.GameState = "active";
                game.Player2.UserToken = player.UserToken;
                game.Board = new BoggleBoard().ToString();

                updateGame(pendingGame, game);

                // Create new pending game.
                addNewGame();

                SetStatus(Created);
                return room;
            }
        }

        /// Cancel a pending request to join a game.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        public void CancelJoin(User u)
        {
            int pendingGame = getPendingGame();
            if (u.UserToken == null || !tryGetPlayer(u.UserToken, out Player p) || pendingGame == 0 || !tryGetGame(pendingGame, out GameStatus g) || g.Player1.UserToken == null || !u.UserToken.Equals(g.Player1.UserToken))
            {
                SetStatus(Forbidden);
            }
            else
            {
                // Remove the player's contact to the server        
                g.Player1.UserToken = null;
                updateGame(pendingGame, g);

                SetStatus(OK);
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
            int timeLeft;
            if (w.Word == null || w.Word.Equals("") || w.Word.Trim().Length > 30 || w.UserToken == null || !tryGetPlayer(w.UserToken, out Player p) || gameID == null || !int.TryParse(gameID, out int GameID) || !((timeLeft = getTimeLeft(GameID)) is int) || !tryGetGame(GameID, out GameStatus g) || (!w.UserToken.Equals(g.Player1.UserToken) && !w.UserToken.Equals(g.Player2.UserToken)))
            {
                SetStatus(Forbidden);
                return null;
            }
            else
            {

                if (!g.GameState.Equals("active"))
                {
                    SetStatus(Conflict);
                    return null;
                }
                else
                {

                    Words wordPlay = new Words();
                    wordPlay.Word = w.Word;

                    // Generate boggle board
                    BoggleBoard board = new BoggleBoard(g.Board);

                    // Score 0 if the word is less than 3 characters or -1 if it doesn't exist in dic.
                    wordPlay.Score = board.CanBeFormed(wordPlay.Word) ? GetScore(wordPlay.Word) : -1;

                    if (wordPlay.Word.Length < 3)
                        wordPlay.Score = 0;
                    else
                        foreach (Words word in getWordsPlayed(w.UserToken, GameID))
                            if (word.Word.ToUpper().Equals(wordPlay.Word.ToUpper())) wordPlay.Score = 0;

                    // Set the appropriate score for each word
                    WordScore scoreWord = new WordScore();
                    scoreWord.Score = wordPlay.Score;

                    // Update the players' played words
                    addWordToList(w, GameID, scoreWord.Score);

                    SetStatus(OK);
                    return scoreWord;
                }
            }
        }

        /// Get game status information.
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below. Note that the information returned depends on whether "Brief=yes" 
        /// was included as a parameter as well as on the state of the game. Responds with status code 200 (OK). Note: The Board and Words are not case sensitive.
        public GameStatus GameStatus(string gameID, string brief)
        {
            int timeLeft;
            if (gameID == null || !int.TryParse(gameID, out int GameID) || !((timeLeft = getTimeLeft(GameID)) is int) || !tryGetGame(GameID, out GameStatus temp))
            {
                SetStatus(Forbidden);
                return null;
            }
            else
            {
                GameStatus game = new GameStatus();

                // Display pending status
                if (temp.GameState.Equals("pending"))
                {
                    GameStatus pendStatus = new GameStatus();
                    pendStatus.GameState = "pending";

                    SetStatus(OK);
                    return pendStatus;
                }
                // Conceal board, time limit, player nicknames and word lists
                else if ((temp.GameState.Equals("active") || temp.GameState.Equals("completed")) && "yes".Equals(brief))
                {
                    game.GameState = temp.GameState;
                    game.TimeLeft = timeLeft;

                    game.Player1 = new Player();
                    game.Player1.Score = getScore(temp.Player1.UserToken, GameID);

                    game.Player2 = new Player();
                    game.Player2.Score = getScore(temp.Player2.UserToken, GameID);

                    return game;
                }
                // Conceal Players' word lists
                else if (temp.GameState.Equals("active"))
                {
                    game.GameState = temp.GameState;
                    game.Board = temp.Board;
                    game.TimeLimit = temp.TimeLimit;
                    game.TimeLeft = timeLeft;

                    game.Player1 = new Player();
                    game.Player1.Nickname = getPlayer(temp.Player1.UserToken);
                    game.Player1.Score = getScore(temp.Player1.UserToken, GameID);

                    game.Player2 = new Player();
                    game.Player2.Nickname = getPlayer(temp.Player2.UserToken);
                    game.Player2.Score = getScore(temp.Player2.UserToken, GameID);

                    SetStatus(OK);
                    return game;

                }
                // Display all information
                else if (temp.GameState.Equals("completed"))
                {
                    game.GameState = temp.GameState;
                    game.Board = temp.Board;
                    game.TimeLimit = temp.TimeLimit;
                    game.TimeLeft = 0;

                    game.Player1 = new Player();
                    game.Player1.Nickname = getPlayer(temp.Player1.UserToken);
                    game.Player1.Score = getScore(temp.Player1.UserToken, GameID);

                    game.Player2 = new Player();
                    game.Player2.Nickname = getPlayer(temp.Player2.UserToken);
                    game.Player2.Score = getScore(temp.Player2.UserToken, GameID);

                    game.Player1.WordsPlayed = getWordsPlayed(temp.Player1.UserToken, GameID);
                    game.Player2.WordsPlayed = getWordsPlayed(temp.Player2.UserToken, GameID);

                    SetStatus(OK);
                    return game;
                }
                return temp;
            }
        }

        /// <summary>
        /// Private helper method to update the current time
        /// </summary>
        /// <param name="gameID"></param>
        private int getTimeLeft(int gameID)
        {

            tryGetGame(gameID, out GameStatus temp);

            int timeLeft = temp.TimeLimit + (int)(temp.StartTime.Ticks - System.DateTime.UtcNow.Ticks) / 10000000;

            if (timeLeft <= 0)
            {

                temp.GameState = "completed";
                updateGame(gameID, temp);
            }
            return timeLeft;
        }

        /// <summary>
        /// Private helper method to receive the score for each word
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Private helper method to add user to the users table
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="nickname"></param>
        private void addUser(string userToken, string nickname)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("insert into Users (UserToken, Nickname) values(@UserToken, @Nickname)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserToken", userToken);
                        command.Parameters.AddWithValue("@Nickname", nickname);

                        command.ExecuteNonQuery();


                        trans.Commit();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to create new game
        /// </summary>
        private void addNewGame()
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("insert into Games (Board) output inserted.GameID values(@Board)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Board", DBNull.Value);

                        command.ExecuteNonQuery();

                        trans.Commit();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to insert words to word table
        /// </summary>
        /// <param name="Word"></param>
        /// <param name="GameID"></param>
        /// <param name="Score"></param>
        private void addWordToList(WordToPlay Word, int GameID, int Score)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("insert into Words (Word, GameID, Player, Score) output inserted.Id values(@Word, @GameID, @Player, @Score)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Word", Word.Word);
                        command.Parameters.AddWithValue("@Player", Word.UserToken);
                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@Score", Score);

                        command.ExecuteScalar().ToString();

                        trans.Commit();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Obtain the pending game ID
        /// </summary>
        /// <returns></returns>
        private int getPendingGame()
        {
            int temp = 0;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM Games ORDER BY GameID DESC", conn, trans))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                temp = 0;
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    temp = (int)reader["GameID"];
                                }
                            }
                        }

                    }
                    trans.Commit();
                    return temp;
                }
            }
        }

        /// <summary>
        /// Private helper method to register the words played to the list
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private List<Words> getWordsPlayed(string UserToken, int GameID)
        {
            List<Words> temp = new List<Words>();
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where GameID = @GameID AND Player = @Player", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@Player", UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Words w = new Words();
                                    w.Word = (string)reader["Word"];
                                    w.Score = (int)reader["Score"];
                                    temp.Add(w);
                                }
                            }
                        }

                    }
                    trans.Commit();
                    return temp;
                }
            }
        }

        /// <summary>
        /// Private helper method to update the score
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private int getScore(string UserToken, int GameID)
        {
            List<Words> temp = getWordsPlayed(UserToken, GameID);
            int i = 0;
            foreach (Words w in temp)
                i += w.Score;
            return i;
        }

        /// <summary>
        /// Private helper method to get player's usertoken
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        private string getPlayer(string userToken)
        {
            tryGetPlayer(userToken, out Player p);
            return p.Nickname;
        }

        /// <summary>
        /// Private helper method to get player
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        private bool tryGetPlayer(string userToken, out Player u)
        {
            bool temp = false;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                u = null;
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("select UserToken, Nickname from Users where UserToken = @UserToken", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserToken", userToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                temp = false;
                            }
                            else
                            {
                                u = new Player();
                                u.UserToken = userToken;
                                while (reader.Read())
                                {
                                    u.Nickname = (string)reader["Nickname"];
                                }
                                temp = true;
                            }
                        }
                        trans.Commit();
                        return temp;
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to obtain the game
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private bool tryGetGame(int gameID, out GameStatus g)
        {
            bool temp = false;
            g = null;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("select GameID, Board, TimeLimit, StartTime, GameState, Player1, Player2 from Games where GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                temp = false;
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    object o;
                                    g = new GameStatus();
                                    g.Board = !((o = reader["Board"]) is DBNull) ? (string)o : null;
                                    g.TimeLimit = !((o = reader["TimeLimit"]) is DBNull) ? (int)o : 0;
                                    if (!((o = reader["StartTime"]) is DBNull)) g.StartTime = (DateTime)o;
                                    g.GameState = !((o = reader["GameState"]) is DBNull) ? (string)o : null;
                                    g.Player1 = new Player();
                                    g.Player2 = new Player();
                                    g.Player1.UserToken = !((o = reader["Player1"]) is DBNull) ? (string)o : null; ;
                                    g.Player2.UserToken = !((o = reader["Player2"]) is DBNull) ? (string)o : null; ;
                                }
                                temp = true;
                            }
                        }
                        trans.Commit();
                        return temp;
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to update the game status
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="g"></param>
        private void updateGame(int GameID, GameStatus g)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    string vars = "";

                    //   if (g.Board != null)
                    vars += "Board = @Board,";

                    if (!g.StartTime.Equals(new DateTime()))
                        vars += "StartTime = @StartTime,";

                    // if (g.Player1.UserToken != null)
                    vars += "Player1 = @Player1,";

                    // if (g.Player2.UserToken != null)
                    vars += "Player2 = @Player2,";

                    using (SqlCommand command = new SqlCommand("update Games set " + vars + " TimeLimit = @TimeLimit, GameState = @GameState where GameID = @GameID", conn, trans))
                    {

                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@TimeLimit", g.TimeLimit);
                        command.Parameters.AddWithValue("@GameState", g.GameState);

                        if (!g.StartTime.Equals(new DateTime()))
                            command.Parameters.AddWithValue("@StartTime", g.StartTime);


                        if (g.Board != null)
                            command.Parameters.AddWithValue("@Board", g.Board);
                        else
                            command.Parameters.AddWithValue("@Board", DBNull.Value);

                        if (g.Player1.UserToken != null)
                            command.Parameters.AddWithValue("@Player1", g.Player1.UserToken);
                        else
                            command.Parameters.AddWithValue("@Player1", DBNull.Value);

                        if (g.Player2.UserToken != null)
                            command.Parameters.AddWithValue("@Player2", g.Player2.UserToken);
                        else
                            command.Parameters.AddWithValue("@Player2", DBNull.Value);


                        command.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
        }
    }
}
