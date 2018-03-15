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
        event Action<int> RequestEvent;
        event Action CancelRequestEvent;
        event Action<string> WordEnteredEvent;

        String Wordlist { set; }
        int Timer { set; }
        int Score { set; }
        bool RegisterEnabled { set; }
        bool CancelEnabled { set; }
        bool RequestEnabled { set; }
        bool CancleRequestEnabled{ set; }
        bool BoardEnabled { set; }
        bool TimeEnabled { set; }
        string Time { set; }
        string Player2 { set; }
        string Player2Score { set; }

        void GameOver();
        void LoadBoard(char[][] board);

        
    }
}
