using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    [DataContract]
    public class GameStatusState
    {
        [DataMember]
        public string GameState { get; set; }
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

    [DataContract]
    public class GameStatus:GameStatusState
    {
        [DataMember]
        public string GameState { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int TimeLimit { get; set; }
        [DataMember]
        public int TimeLeft { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Player Player1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Player Player2 { get; set; }
    }

    [DataContract]
    public class Player
    {
        [DataMember]
        public int Score { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<Words> WordsPlayed { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string UserToken { get; set; }
    }

    [DataContract]
    public class Words
    {
        [DataMember]
        public string Word { get; set; }
        [DataMember]
        public int Score { get; set; }
    }
}