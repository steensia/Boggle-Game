using Formulas;
using SS;
using SSGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public class Controller
    {
        private IView window;

        private Spreadsheet sheet;

        private string selectedCell;

        public Controller(IView window)
        {
            this.window = window;
            this.sheet = new Spreadsheet(new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            eventSetup();
            window.NameBox = selectedCell;
            window.SetCellSelection(getColumn(selectedCell), getRow(selectedCell));
        }

        public Controller(IView window, TextReader file)
        {
            this.window = window;
            this.sheet = new Spreadsheet(file, new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            eventSetup();
            window.NameBox = selectedCell;
            window.SetCellSelection(getColumn(selectedCell), getRow(selectedCell));
        }

        private void eventSetup()
        {
            window.LoadSpreadsheetEvent += HandleLoadSpreadsheet;
            window.SelectionChangedEvent += HandleSelectionChanged;
            window.ContentsChangedEvent += HandleContentsChanged;
            window.NewClickEvent += HandleNewClick;
            window.SaveClickEvent += HandleSaveClick;
            window.OpenClickEvent += HandleOpenClick;
            window.CloseClickEvent += HandleCloseClick;
            window.SaveFileEvent += HandleSaveFile;
            window.OpenFileEvent += HandleOpenFile;
        }


        private void HandleLoadSpreadsheet()
        {
            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    window.SetCellValue(r, c, sheet.GetCellValue(getCellName(r, c)).ToString());
                }
            }
        }

        private void HandleSelectionChanged(int r, int c)
        {
            if (c >= 0 && r >= 0)
            {
                selectedCell = getCellName(r, c);
                window.ValueBox = sheet.GetCellValue(selectedCell).ToString();
                object o = sheet.GetCellContents(selectedCell);
                if (o is Formula)
                {
                    window.ContentBox = "=" + o.ToString();
                }
                else
                {
                    window.ContentBox = o.ToString();
                }
                window.NameBox = selectedCell;
            }
        }

        private void HandleContentsChanged(String contents)
        {
            try
            {
                sheet.SetContentsOfCell(selectedCell, contents);
                window.ValueBox = sheet.GetCellValue(selectedCell).ToString();
                window.ErrorBox = "";
            }
            catch (Exception ex)
            {
                window.ErrorBox = ex.GetType().ToString();
            }

            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    window.SetCellValue(r, c, sheet.GetCellValue(getCellName(r, c)).ToString());
                }
            }
        }

        private void HandleNewClick()
        {
            window.OpenNew();
        }

        private void HandleSaveClick()
        {
            window.ShowSaveDialog();
        }

        private void HandleOpenClick()
        {
            window.ShowOpenDialog();
        }

        private void HandleCloseClick(FormClosingEventArgs e)
        {
            if (sheet.Changed)
            {
                window.ShowFileNotSavedDialog(e);
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void HandleSaveFile(String fileName)
        {
            TextWriter r = new StreamWriter(fileName);
            sheet.Save(r);
            r.Close();
        }

        private void HandleOpenFile(String fileName)
        {
            TextReader r = new StreamReader(fileName);
            window.OpenNew(r);
            r.Close();
        }

        private string getCellName(int r, int c)
        {
            return "" + (char)('A' + c) + (r + 1);
        }

        private int getColumn(string name)
        {
            return (name.ToCharArray()[0] - 'A');
        }

        private int getRow(string name)
        {
            int.TryParse(name.Substring(1, name.Length - 2), out int row);
            return row - 1;
        }
    }
}
