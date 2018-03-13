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
    public partial class BoggleWindow : Form, IBoggleView
    {
        public string Wordlist { set => WordList.Text = value; }
        public int Timer { set => TimerBox.Text = ""+value; }
        public int Score { set => TimerBox.Text = "" + value; }

        public BoggleWindow()
        {
            InitializeComponent();
        }

        private void BoggleWindow_Load(object sender, EventArgs e)
        {

        }

        public event Action<string, string> RegisterEvent;
        public event Action CancelRegisterEvent;
        public event Action RequestEvent;
        public event Action CancelRequestEvent;
        public event Action<string> WordEnteredEvent;
        public event Action<int> StartGameEvent;

        private void Help_Click(object sender, EventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            UsernameBox.Enabled = false;
            DomainBox.Enabled = false;
            RegisterButton.Enabled = false;
            RegisterEvent?.Invoke(DomainBox.Text, UsernameBox.Text);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            UsernameBox.Enabled = true;
            DomainBox.Enabled = true;
            RegisterButton.Enabled = true;
            CancelRegisterEvent?.Invoke();
        }

        private void RequestButton_Click(object sender, EventArgs e)
        {
            RequestButton.Enabled = false;
            RequestEvent?.Invoke();
        }

        private void CancelRequestButton_Click(object sender, EventArgs e)
        {
            RequestButton.Enabled = true;
            CancelRequestEvent?.Invoke();
        }

        private void WordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                WordEnteredEvent?.Invoke(WordBox.Text);
                WordBox.Text = "";
            }
        }

        private void StartGame(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                WordEnteredEvent?.Invoke(WordBox.Text);
                WordBox.Text = "";

                if (int.TryParse(TimerBox.Text, out int time))
                {
                    StartGameEvent?.Invoke(time);
                    TimerBox.Enabled = false;
                }
                else
                {
                    TimerBox.Text = "";
                }
            }
        }

        public void GameOver()
        {
            TimerBox.Enabled = true;
            TimerBox.Text = "";
        }

        public void LoadBoard(char[][] board)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    BoggleBoard.SetValue(i,j,""+board[i][j]);
                }
            }         
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void WordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TimerBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
