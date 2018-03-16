using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    /// <summary>
    /// Window for BoggleClient
    /// </summary>  
    public partial class BoggleWindow : Form, IBoggleView
    {
        /// <summary>
        /// Creates the window
        /// </summary>
        public BoggleWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Help Click event of the menu control, showing features to help TA
        /// </summary>a
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to use Client: \n" +
                                "\t 1) Provide a server domain and name, then hit register \n" +
                                "\t 2) Provide the desired time(seconds) to play the game \n" +
                                "\t 3) Proceed to request a user to play with\n" +
                                "\t 4) The game starts when a user is found\n", "Help Box");
        }

        /// <summary>
        /// Handles Register button event in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterEvent?.Invoke(DomainBox.Text, UsernameBox.Text);
        }

        /// <summary>
        /// Handles Cancel button event in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelRegisterEvent?.Invoke();
        }

        /// <summary>
        /// Handles Request button event in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(TimerBox.Text, out int time))
            {
                RequestEvent?.Invoke(time);
            }
            else
            {
                MessageBox.Show("The time needs to be an integer");
            }
        }

        /// <summary>
        /// Handles Cancel Request button event in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelRequestButton_Click(object sender, EventArgs e)
        {
            CancelRequestEvent?.Invoke();
        }

        /// <summary>
        /// Handles words being entered in text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                WordEnteredEvent?.Invoke(WordBox.Text);
                WordBox.Text = "";
            }
        }

        private void RegistrationEnabled(bool b)
        {
            RegisterButton.Enabled = b;
            DomainBox.Enabled = b;
            UsernameBox.Enabled = b;
        }

        /// <summary>
        /// Populates the boggle board with random letters
        /// </summary>
        public void LoadBoard(string board)
        {
            for (int i = 0; i < 16; i++)
            {
                int r = i / 4;
                int c = i % 4;
                if (i >= board.Length)
                    BoggleBoard.SetValue(c, r, "");
                else
                    BoggleBoard.SetValue(c, r, "  " + board.Substring(i, 1));
            }         
        }

        /// <summary>
        /// Fires when a user decides to register and set up a Boggle game
        /// Parameters are the username and the domain.
        /// </summary>
        public event Action<string, string> RegisterEvent;

        /// <summary>
        /// Fires when the user decides to cancel the register event
        /// </summary>
        public event Action CancelRegisterEvent;

        /// <summary>
        /// Fires when the user wants to play with a user 
        /// </summary>
        public event Action<int> RequestEvent;

        /// <summary>
        /// Fires when the user decides to cancel user request
        /// </summary>
        public event Action CancelRequestEvent;

        /// <summary>
        /// Fires when the user has typed in and entered a word to be displayed onto the word list
        /// Parameter is the word entered by the user
        /// </summary>
        public event Action<string> WordEnteredEvent;

        /// <summary>
        /// Contains the strings/words entered by both the users
        /// </summary>
        public string Wordlist { set => WordList.Text = value; }

        /// <summary>
        /// Identify when the Register button is active or not
        /// </summary>
        public bool RegisterEnabled { set => RegistrationEnabled(value); }

        /// <summary>
        /// Identify when the Cancel button is active or not
        /// </summary>
        public bool CancelEnabled { set => CancelButton.Enabled = value; }

        /// <summary>
        /// Identify when the Request button is active or not
        /// </summary>
        public bool RequestEnabled { set => RequestButton.Enabled = value; }

        /// <summary>
        /// Identify when the CancelRequest button is active or not
        /// </summary>
        public bool CancelRequestEnabled { set => Cancel.Enabled = value; }

        /// <summary>
        /// Identify when the BoggleBoard is active or not
        /// </summary>
        public bool BoardEnabled { set => WordBox.Enabled = value; }

        /// <summary>
        /// Identify when the timer is active or not
        /// </summary>
        public bool TimeEnabled { set => TimerBox.Enabled = value; }

        /// <summary>
        /// Display the time of the game
        /// </summary>
        public string Time { set => TimerBox.Text = value; }

        /// <summary>
        /// Display score of the first player
        /// </summary>
        public string Score { set => ScoreBox.Text = value; }

        public string Player2 { set => Player2UsernameBox.Text = value; }
        
        /// <summary>
        /// Display the second player's score
        /// </summary>
        public string Player2Score { set => Player2ScoreBox.Text = value; }

    }
}
