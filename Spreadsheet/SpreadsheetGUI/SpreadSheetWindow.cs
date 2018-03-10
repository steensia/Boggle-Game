using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SS;
using Formulas;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SSGui;
using System.IO;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Top-level window of Spreadsheet
    /// </summary>
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {
        public string NameBox { set => CellName.Text = value; }
        public string ContentBox { set => Content.Text = value; }
        public string ValueBox { set => Value.Text = value; }
        string ISpreadsheetView.ErrorBox { set => Error.Text = value; }

        /// <summary>
        /// Initializes a new instance of the Spreadsheet Window class.
        /// </summary>
        public SpreadsheetWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the spreadsheet GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSpreadsheetEvent?.Invoke();
        }
        /// <summary>
        /// Fired when a load action is requested.
        /// </summary>
        public event Action LoadSpreadsheetEvent;

        /// <summary>
        /// Fired when a selection of cell action is requested.
        /// </summary>
        public event Action<int, int> SelectionChangedEvent;

        /// <summary>
        /// Fired when a editing of cell action is requested.
        /// </summary>
        public event Action<string> ContentsChangedEvent;

        /// <summary>
        /// Fired when a save as file action is requested.
        /// </summary>
        public event Action<string> SaveFileEvent;

        /// <summary>
        /// Fired when a save file action is requested.
        /// </summary>
        public event Action SaveEvent;

        /// <summary>
        /// Fired when a open file action is requested.
        /// </summary>
        public event Action<string> OpenFileEvent;

        /// <summary>
        /// Fired when a close action is requested.
        /// </summary>
        public event Action<FormClosingEventArgs> CloseFileEvent;


        /// <summary>
        /// Handles the selection changed event when selecting cells
        /// </summary>
        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            sender.GetSelection(out int c, out int r);
            ContentsChangedEvent?.Invoke(Content.Text);
            SelectionChangedEvent?.Invoke(r, c);
            spreadsheetPanel1.Focus();
        }

        /// <summary>
        /// Handles the KeyPress(arrow keys) event when selecting cells
        /// </summary>
        private void spreadsheetPanel1_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Content.Focus();
            }
            else
            {
                spreadsheetPanel1.GetSelection(out int c, out int r);
                switch (e.KeyData)
                {
                    case Keys.Up:
                        r--;
                        break;
                    case Keys.Down:
                        r++;
                        break;
                    case Keys.Left:
                        c--;
                        break;
                    case Keys.Right:
                        c++;
                        break;
                }
                if (c < 0) c = 0;
                if (r < 0) r = 0;
                if (c > 25) c = 25;
                if (r > 99) r = 99;
                SetCellSelection(r, c);
            }      
        }

        /// <summary>
        /// Handles the KeyDown event when entering into a cell
        /// </summary>
        private void Content_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ContentsChangedEvent?.Invoke(Content.Text);
                spreadsheetPanel1.Focus();
            }
        }

        /// <summary>
        /// Handles the Click event of the openItem control.
        /// </summary>
        private void New_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Handles the Save Click event of the File menu control.
        /// </summary>
        private void Save_Click(object sender, EventArgs e)
        {
           SaveEvent?.Invoke();
        }

        /// <summary>
        /// Handles the Open Click event of the File menu control.
        /// </summary>
        private void Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Handles the Close Click event of the File menu control.
        /// </summary>
        private void Close_Click(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            CloseFileEvent?.Invoke(e);
        }

        /// <summary>
        /// Handles the Save File event of the File menu control.
        /// </summary>
        private void Save_File(object sender, CancelEventArgs e)
        {
            SaveFileEvent?.Invoke(saveFileDialog1.FileName);
        }

        /// <summary>
        /// Handles the Open File event of the File menu control.
        /// </summary>
        private void Open_File(object sender, CancelEventArgs e)
        {
            OpenFileEvent?.Invoke(openFileDialog1.FileName);
        }

        /// <summary>
        /// Handle the ShowSaveDialog event when saving
        /// </summary>
        public void ShowSaveDialog()
        {
            saveFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Handle the ShowFileNotSaveDialog event when saving
        /// </summary>
        public void ShowFileNotSavedDialog(FormClosingEventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show("This file has not been save, would you still like to continue?", "File not saved", buttons);
            if (result == DialogResult.Yes)
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// Sets the cells value using spreadsheetpanel
        /// </summary>
        public void SetCellValue(int r, int c, string value)
        {
            spreadsheetPanel1.SetValue(c, r, value);
        }

        /// <summary>
        /// Sets the cell selection using spreadsheetpanel
        /// </summary>
        public void SetCellSelection(int r, int c)
        {
            if (spreadsheetPanel1.SetSelection(c, r))
                SelectionChangedEvent?.Invoke(r, c);
        }

        /// <summary>
        /// Opens a file from folder
        /// </summary>
        public void OpenNew(TextReader file)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(file);
        }

        /// <summary>
        /// Handles the Help_Click event of the menu control, showing features to help TA
        /// </summary>
        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to navigate: \n" +
                                "\t Use arrow keys or mouse to navigate \n" +
                            "How to add a cell: \n" +
                                "\tPress enter or click on a cell to register contents\n" +
                            "How to use File Menu: \n" +
                                "\t Click File > New or Ctrl + N to open a new window \n" +
                                "\t Click File > Save or Ctrl + S to save a file \n" +
                                "\t Click File > Save as or Ctrl + Alt + S to save or overwrite a file\n" +
                                "\t Click File > Exit or Ctrl + W or X to exit the window ", "Help Box");
        }

        /// <summary>
        /// Handles the SaveAs Click event of the File menu control.
        /// </summary>
        private void SaveAs_Click(object sender, EventArgs e)
        {
                saveFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Handles the MenuClick event of the File menu control.
        /// </summary>
        private void MenuClose_Click(object sender, EventArgs e)
        {
                Close();
        }
    }
}
