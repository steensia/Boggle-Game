using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    /// <summary>
    /// Interface implemented by BoggleWindow
    /// </summary>
    public interface IBoggleView
    {
        /// <summary>
        /// Fires when a user decides to register and set up a Boggle game
        /// Parameters are the username and the domain.
        /// </summary>
        event Action<string,string> RegisterEvent;

        /// <summary>
        /// Fires when the user decides to cancel the register event
        /// </summary>
        event Action CancelRegisterEvent;

        /// <summary>
        /// Fires when the user wants to play with a user 
        /// </summary>
        event Action<int> RequestEvent;

        /// <summary>
        /// Fires when the user decides to cancel user request
        /// </summary>
        event Action CancelRequestEvent;

        /// <summary>
        /// Fires when the user has typed in and entered a word to be displayed onto the word list
        /// Parameter is the word entered by the user
        /// </summary>
        event Action<string> WordEnteredEvent;

        /// <summary>
        /// Contains the strings/words entered by both the users
        /// </summary>
        String Wordlist { set; }

        /// <summary>
        /// Textbox to enter a word for the user
        /// </summary>
        String EnterWordBox { set; }

        /// <summary>
        /// Identify when the Register button is active or not
        /// </summary>
        bool RegisterEnabled { set; }

        /// <summary>
        /// Identify when the Cancel button is active or not
        /// </summary>
        bool CancelEnabled { set; }

        /// <summary>
        /// Identify when the Request button is active or not
        /// </summary>
        bool RequestEnabled { set; }

        /// <summary>
        /// Identify when the CancelRequest button is active or not
        /// </summary>
        bool CancelRequestEnabled{ set; }

        /// <summary>
        /// Identify when the BoggleBoard is active or not
        /// </summary>
        bool BoardEnabled { set; }

        /// <summary>
        /// Identify when the timer is active or not
        /// </summary>
        bool TimeEnabled { set; }

        /// <summary>
        /// Display score of the first player
        /// </summary>
        string Score { set; }

        /// <summary>
        /// Display the time of the game
        /// </summary>
        string Time { set; }

        /// <summary>
        /// Display the second player
        /// </summary>
        string Player2 { set; }

        /// <summary>
        /// Display the second player's score
        /// </summary>
        string Player2Score { set; }

        /// <summary>
        /// Populates the boggle board with random letters
        /// </summary>
        /// <param name="board"></param>
        void LoadBoard(string board);

        
    }
}
