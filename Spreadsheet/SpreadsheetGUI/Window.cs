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
        public event Action NewClickEvent;
        public event Action SaveClickEvent;
        public event Action OpenClickEvent;
        public event Action<FormClosingEventArgs> CloseClickEvent;
        public event Action KeyEvent;

        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            sender.GetSelection(out int c, out int r);
            SelectionChangedEvent?.Invoke(r, c);
        }
        /*
                private void spreadsheetPanel1_KeyPress(object sender, KeyPressEventArgs e)
                {

                    spreadsheetPanel1.GetSelection(out int c, out int r);

                    switch (e.KeyCode)
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
                    if(r < 0)
                    {
                        r = 0;
                    }
                    if(c < 0)
                    {
                        c = 0;
                    }
                    if( r > 99)
                    {
                        r = 99;
                    }
                    if( c > 25)
                    {
                        c = 25;
                    }
                    spreadsheetPanel1.SetSelection(c,r);
                    SelectionChangedEvent?.Invoke(r, c);
                }
                */

        private void Contents_Changed(object sender, EventArgs e)
        {
            ContentsChangedEvent?.Invoke(Content.Text);
        }

        private void New_Click(object sender, EventArgs e)
        {
            NewClickEvent?.Invoke();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveClickEvent?.Invoke();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenClickEvent?.Invoke();
        }

        private void Close_Click(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            CloseClickEvent?.Invoke(e);
        }

        private void Save_File(object sender, CancelEventArgs e)
        {

            SaveFileEvent?.Invoke(saveFileDialog1.FileName);
        }

        private void Open_File(object sender, CancelEventArgs e)
        {
            OpenFileEvent?.Invoke(openFileDialog1.FileName);
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

        public void ShowOpenDialog()
        {
            openFileDialog1.ShowDialog();
        }

        public void ShowSaveDialog()
        {
            saveFileDialog1.ShowDialog();
        }

        public void SetCellValue(int r, int c, string value)
        {
            spreadsheetPanel1.SetValue(c, r, value);
        }

        public void SetCellSelection(int r, int c)
        {
            spreadsheetPanel1.SetSelection(c, r);
        }

        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        public void OpenNew(TextReader file)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(file);
        }
    }
}
