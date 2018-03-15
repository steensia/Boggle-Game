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
        public bool RegisterEnabled { set => RegisterButton.Enabled = value; }
        public bool CancelEnabled { set => CancelButton.Enabled = value; }
        public bool RequestEnabled { set => RequestButton.Enabled = value; }
        public bool CancleRequestEnabled { set => CancelRequestButton.Enabled = value; }
        public bool BoardEnabled { set => WordBox.Enabled = value; }
        public bool TimeEnabled { set => TimerBox.Enabled = value; }
        public string Time { set => TimerBox.Text = value; }
        public string Player2 { set => Player2UsernameBox.Text = value; }
        public string Player2Score { set => Player2ScoreBox.Text = value; }

        public BoggleWindow()
        {
            InitializeComponent();
        }

        private void BoggleWindow_Load(object sender, EventArgs e)
        {

        }

        public event Action<string, string> RegisterEvent;
        public event Action CancelRegisterEvent;
        public event Action<int> RequestEvent;
        public event Action CancelRequestEvent;
        public event Action<string> WordEnteredEvent;

        private void Help_Click(object sender, EventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterEvent?.Invoke(DomainBox.Text, UsernameBox.Text);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelRegisterEvent?.Invoke();
        }

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

        private void CancelRequestButton_Click(object sender, EventArgs e)
        {
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

        private void WordBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
