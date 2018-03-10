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
        private ISpreadsheetView window;

        private Spreadsheet sheet;

        private string selectedCell;

        private string previousFile;

        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            this.sheet = new Spreadsheet(new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            this.previousFile = null;
            eventSetup();
        }

        public Controller(ISpreadsheetView window, TextReader file)
        {
            this.window = window;
            this.sheet = new Spreadsheet(file, new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            this.previousFile = file.ToString();
            eventSetup();
        }

        private void eventSetup()
        {
            window.LoadSpreadsheetEvent += HandleLoadSpreadsheet;
            window.SelectionChangedEvent += HandleSelectionChanged;
            window.ContentsChangedEvent += HandleContentsChanged;
            window.CloseFileEvent += HandleCloseFile;
            window.SaveFileEvent += HandleSaveFile;
            window.SaveEvent += HandleSave;
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
            window.NameBox = selectedCell;
            window.SetCellSelection(getColumn(selectedCell), getRow(selectedCell));
        }

        private void HandleSelectionChanged(int r, int c)
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

        private void HandleContentsChanged(String contents)
        {
            string temp = (sheet.GetCellContents(selectedCell) is Formula) ? "=" : "" + sheet.GetCellContents(selectedCell).ToString();
            if (contents.Equals(temp)) return;
            try
            {
                foreach (string s in sheet.SetContentsOfCell(selectedCell, contents))
                    window.SetCellValue(getRow(s), getColumn(s), sheet.GetCellValue(s).ToString());
                window.ValueBox = sheet.GetCellValue(selectedCell).ToString();
                window.ErrorBox = "";
            }
            catch (Exception ex)    
            {
                window.ErrorBox = ex.GetType().ToString();
            }
        }

        private void HandleCloseFile(FormClosingEventArgs e)
        {
            e.Cancel = true;
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
            previousFile = fileName;
            sheet.Save(r);
            r.Close();
        }

        private void HandleSave()
        {
            if (previousFile == null)
            {
                window.ShowSaveDialog();
            }
            else
            {
                TextWriter r = new StreamWriter(previousFile);
                sheet.Save(r);
                r.Close();
            }
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
            int.TryParse(name.Substring(1, name.Length - 1), out int row);
            return row -1;
        }
    }
}
