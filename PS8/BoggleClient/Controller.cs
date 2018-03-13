﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    class Controller
    {
        char[][] board;
        private IBoggleView window;

        public Controller(IBoggleView window)
        {
            this.window = window;
            EventSetup();
        }

        void EventSetup()
        {
            window.RegisterEvent += HandleRegister();
            window.CancelRegisterEvent += HandleCancelRegister;
            window.RequestEvent += HandleRequest;
            window.CancelRequestEvent += HandleCancelRequest;
            window.WordEnteredEvent += HandleWordEntered;
            window.StartGameEvent += HandelStartGame;
        }

        private Action<string, string> HandleRegister()
        {
            throw new NotImplementedException();
        }

        private void HandleCancelRegister()
        {
            throw new NotImplementedException();
        }

        private void HandleRequest()
        {
            throw new NotImplementedException();
        }

        private void HandleCancelRequest()
        {
            throw new NotImplementedException();
        }

        private void HandleWordEntered(string obj)
        {
            throw new NotImplementedException();
        }

        private void HandelStartGame(int obj)
        {
            throw new NotImplementedException();
        }
    }
}

