using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface IBoggleView
    {
        event Action<string,string> RegisterEvent;
        event Action CancelRegisterEvent;
        event Action RequestEvent;
        event Action CancelRequestEvent;
        event Action<string> WordEnteredEvent;
        event Action<int> StartGameEvent;

        String Wordlist { set; }
        int Timer { set; }
        int Score { set; }

        void GameOver();
        void LoadBoard(char[][] board);
    }
}
