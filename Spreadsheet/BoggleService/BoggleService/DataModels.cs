using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    //Create users
    public class Username
    {
        public string Nickname { get; set; }
    }
  
    public class User
    {
        public string UserToken { get; set; }
    }

    //Join game
    public class GameInfo
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }

    public class GameRoom
    {
        public string GameID { get; set; }
    }

    //cancel join


    //Play word
    public class WordToPlay
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    public class WordScore
    {
        public int Score { get; set; }
    } 

    //game status

    public class GameStatus
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }


    public class Player
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public List<Words> WordsPlayed { get; set; }
        public string UserToken { get; set; }
    }

    public class Words
    {
    public string Word { get; set; }
    public int Score { get; set; }
    }
}