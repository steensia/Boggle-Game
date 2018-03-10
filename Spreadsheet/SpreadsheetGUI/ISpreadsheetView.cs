using SSGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Controllable interface of SpreadsheetWindow
    /// </summary>
    public interface ISpreadsheetView
    {
        // High levels events to be fired
        event Action LoadSpreadsheetEvent;
        event Action<int, int> SelectionChangedEvent;
        event Action<string> ContentsChangedEvent;
        event Action<string> SaveFileEvent;
        event Action<string> OpenFileEvent;
        event Action SaveEvent;
        event Action<FormClosingEventArgs> CloseFileEvent;

        // GUI Properties
        string NameBox { set; }
        string ContentBox { set; }
        string ValueBox { set; }
        string ErrorBox { set; }

        // Private methods to invoke some events
        void ShowSaveDialog();
        void ShowFileNotSavedDialog(FormClosingEventArgs e);
        void SetCellSelection(int r, int c);
        void SetCellValue(int r, int c, string value);
        void OpenNew(TextReader file);
    }
}
