using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetTester
{
    class SpreadsheetViewStub : ISpreadsheetView
    {
        // These properties indicate whether an event was called
        public bool CalledOpenNew { get; private set; }
        public bool CalledSetCellSelection { get; private set; }
        public bool CalledSetCellValue { get; private set; }
        public bool CalledShowFileNotSavedDialog { get; private set; }
        public bool CalledShowSaveDialog { get; private set; }

        // These methods cause events to be fired
        public void FireLoadSpreadsheetEvent() // do
        {
            if (LoadSpreadsheetEvent != null) LoadSpreadsheetEvent();
        }
        public void FireSelectionChangedEvent(int c, int r) // do
        {
            if (SelectionChangedEvent != null) SelectionChangedEvent(c, r);
        }
        public void FireContentsChangedEvent(string s) // do
        {
            if (ContentsChangedEvent != null) ContentsChangedEvent(s);
        }
        public void FireSaveEvent()
        {
            if (SaveEvent != null) SaveEvent();
        }
        public void FireSaveFileEvent(string s) // do
        {
            if (SaveFileEvent != null) SaveFileEvent(s);
        }
        public void FireOpenFileEvent(string s) // done
        {
            if (OpenFileEvent != null) OpenFileEvent(s);
        }
        public void FireCloseFileEvent(FormClosingEventArgs f) // done
        {
            if (CloseFileEvent != null) CloseFileEvent(f);
        }

        // These properties implement the interface
        public string NameBox { get; set; }
        public string ContentBox { get; set; }
        public string ValueBox { get; set; }
        public string ErrorBox { get; set; }

        // These events implement the interface
        public event Action LoadSpreadsheetEvent;
        public event Action<int, int> SelectionChangedEvent;
        public event Action<string> ContentsChangedEvent;
        public event Action<string> SaveFileEvent;
        public event Action<string> OpenFileEvent;
        public event Action<FormClosingEventArgs> CloseFileEvent;
        public event Action SaveEvent;

        // These methods implement the interface and change respective properties
        public void ShowFileNotSavedDialog(FormClosingEventArgs e)
        {
            CalledShowFileNotSavedDialog = true;
        }

        public void SetCellSelection(int r, int c)
        {
            CalledSetCellSelection = true;
        }

        public void SetCellValue(int r, int c, string value)
        {
            CalledSetCellValue = true;
        }

        public void OpenNew(TextReader file)
        {
            CalledOpenNew = true;
        }

        public void ShowSaveDialog()
        {
            CalledShowSaveDialog = true;
        }
    }
}
