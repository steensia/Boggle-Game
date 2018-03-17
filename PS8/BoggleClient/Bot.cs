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
    public partial class Bot : Form,IBoggleView
    {
        private string board;
        private const string url = "http://ice.eng.utah.edu/BoggleService.svc/";
        private const string name = "GodBot";
        private const int time = 5;
        private Button button1;
        private bool playing;
        private Timer t;
        private Timer join;

        public string Wordlist { set => nothing(true); }
        public string EnterWordBox { set => nothing(true); }
        public bool RegisterEnabled { set => nothing(value); }

        public bool CancelEnabled { set => nothing(value); }
        public bool RequestEnabled { set => joinGame(value); }

        public bool CancelRequestEnabled { set => nothing(value); }
        public bool BoardEnabled { set => joinGame(value); }
        public bool TimeEnabled { set => nothing(value); }
        public string Score { set => Console.WriteLine("Score:" + value); }
        public string Time { set => Console.WriteLine("Time:" + value); }
        public string Player2 { set => Console.WriteLine("Opponent:" + value); }
        public string Player2Score { set => Console.WriteLine("Opponent Score:" + value); }

        public event Action<string, string> RegisterEvent;
        public event Action CancelRegisterEvent;
        public event Action<int> RequestEvent;
        public event Action CancelRequestEvent;
        public event Action<string> WordEnteredEvent;

        public Bot()
        {
            InitializeComponent();
            t = new Timer();
            t.Interval = 100;
            t.Enabled = false;
            t.Tick += delegate
            {
                playWord();
            };

            join = new Timer();
            join.Interval = 1000;
            join.Enabled = false;
            join.Tick += delegate
            {
                MessageBox.Show("JoiningGame");
                RequestEvent?.Invoke(time);
            };

        }

        private void enable(bool value)
        {         
            if (value)
            {
                join.Enabled = true;
            }
        }
        private void playWord()
        {
            WordEnteredEvent?.Invoke("GodBot");
        }

        private void joinGame(bool b)
        {
            if (b)
            {
                MessageBox.Show("JoiningGame");
                RequestEvent?.Invoke(time);
              //  join.Enabled = false;
              //  t.Enabled = true;
            }
        }
        private void nothing(bool value){ }

        public void LoadBoard(string board)
        {
            this.board = board;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Registering");
            RegisterEvent?.Invoke(url, name);
        }
    }
}
