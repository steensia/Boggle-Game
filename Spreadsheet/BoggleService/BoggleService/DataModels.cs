using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    public class UserInfo
    {
        public string UserToken { get; set; }
        public string Nickname { get; set; }
    }

    public class Player
    {
        public string UserToken { get; set; }
        public string Nickname { get; set; }
        //public UserInfo User { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public IList WordsPlayed { get; set; }
    }

    public class PlayedWords
    {
        public string Word { get; set; }
        public int Score { get; set; }
    }



    public class PendingGame
    {
        public bool IsPendingUser { get; set; }
        public bool HasPlayer { get; set; }
    }

    public class TimeInfo
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
        public GameStatus Status { get; set; }
    }

    public class WordPlay
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    public class GameStatus
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }    
    }
}