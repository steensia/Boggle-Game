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
        event Action RequestEvent;

        /// <summary>
        /// Fires when the user decides to cancel requesting for a user to play with
        /// </summary>
        event Action CancelRequestEvent;

        /// <summary>
        /// Fires when the user has typed in and entered a word to be displayed onto the word list
        /// Parameter is the word entered by the user
        /// </summary>
        event Action<string> WordEnteredEvent;

        /// <summary>
        /// Fires when the user decides how long he wants to play the game for
        /// Parameter is the time 
        /// </summary>
        event Action<int> StartGameEvent;

        /// <summary>
        /// Contains the strings/words entered by both the users
        /// </summary>
        String Wordlist { set; }

        /// <summary>
        /// The time limit of the game
        /// </summary>
        int Timer { set; }

        /// <summary>
        /// The score of the game
        /// </summary>
        int Score { set; }

        /// <summary>
        /// Notifies the user that the game is over
        /// </summary>
        void GameOver();

        /// <summary>
        /// Loads the board with random letters
        /// </summary>
        /// <param name="board"></param>
        void LoadBoard(char[][] board);
    }
}
