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
    public partial class Window : Form, IView
    {
        public string NameBox { set => CellName.Text = value; }
        public string ContentBox { set => Content.Text = value; }
        public string ValueBox { set => Value.Text = value; }
        string IView.ErrorBox { set => Error.Text = value; }

        public Window()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSpreadsheetEvent?.Invoke();
        }

        public event Action LoadSpreadsheetEvent;
        public event Action<int, int> SelectionChangedEvent;
        public event Action<string> ContentsChangedEvent;
        public event Action<string> SaveFileEvent;
        public event Action<string> OpenFileEvent;
        public event Action<FormClosingEventArgs> CloseFileEvent;

        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            sender.GetSelection(out int c, out int r);
            ContentsChangedEvent?.Invoke(Content.Text);
            SelectionChangedEvent?.Invoke(r, c);
            spreadsheetPanel1.Focus();
        }

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

        private void Content_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ContentsChangedEvent?.Invoke(Content.Text);
                spreadsheetPanel1.Focus();
            }
        }

        private void New_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void Close_Click(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            CloseFileEvent?.Invoke(e);
        }

        private void Save_File(object sender, CancelEventArgs e)
        {
            SaveFileEvent?.Invoke(saveFileDialog1.FileName);
        }

        private void Open_File(object sender, CancelEventArgs e)
        {
            OpenFileEvent?.Invoke(openFileDialog1.FileName);
        }

        private void Help_Click(object sender, EventArgs e)
        {
           
        }

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

        public void SetCellValue(int r, int c, string value)
        {
            spreadsheetPanel1.SetValue(c, r, value);
        }

        public void SetCellSelection(int r, int c)
        {
            if (spreadsheetPanel1.SetSelection(c, r))
                SelectionChangedEvent?.Invoke(r, c);
        }

        public void OpenNew(TextReader file)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(file);
        }
    }
}
