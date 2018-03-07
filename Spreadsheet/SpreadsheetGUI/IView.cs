﻿using SSGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public interface IView
    {
        event Action LoadSpreadsheetEvent;

        event Action<int, int> SelectionChangedEvent;

        event Action<string> ContentsChangedEvent;

        event Action<string> SaveFileEvent;
        event Action<string> OpenFileEvent;

        event Action NewClickEvent;
        event Action SaveClickEvent;
        event Action OpenClickEvent;
        event Action<FormClosingEventArgs> CloseClickEvent;

        event Action KeyEvent;

        string NameBox { set; }
        string ContentBox { set; }
        string ValueBox { set; }
        string ErrorBox { set; }

        void ShowFileNotSavedDialog(FormClosingEventArgs e);
        void ShowOpenDialog();
        void ShowSaveDialog();
        void SetCellSelection(int r, int c);
        void SetCellValue(int r, int c, string value);
        void OpenNew();
        void OpenNew(TextReader file);
    }
}
