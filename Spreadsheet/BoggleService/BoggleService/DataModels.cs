using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    public class UserInfo
    {
        public string Nickname { get; set; }
    }

    public class TimeInfo
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }

    public class GameStatus
    {
        public string GameState { get; set; }
        public string GameID { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }

    public class Player
    {
        public string Nickname { get; set; }
        public string Score { get; set; }
        public string GameID { get; set; }
        public string UserToken { get; set; }
        public string PlayWord { get; set; }
    }

    public class PlayerStats
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public string WordsPlayed { get; set; }
    }
}