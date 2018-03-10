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
    /// <summary>
    /// Controls the operation of an ISpreadsheetView.
    /// </summary>
    public class Controller
    {
        // Fields
        private ISpreadsheetView window;
        private Spreadsheet sheet;
        private string selectedCell;
        private string previousFile;

        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="window"></param>
        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            this.sheet = new Spreadsheet(new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            this.previousFile = null;
            eventSetup();
        }

        /// <summary>
        /// Constructor that reads in file
        /// </summary>
        public Controller(ISpreadsheetView window, TextReader file)
        {
            this.window = window;
            this.sheet = new Spreadsheet(file, new Regex("^[A-Z][1-9][0-9]?$"));
            this.selectedCell = "A1";
            this.previousFile = file.ToString();
            eventSetup();
        }

        /// <summary>
        /// Begin controlling the events
        /// </summary>
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

        /// <summary>
        /// Sets up/sends information to load the Spreadsheet GUI
        /// </summary>
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

        /// <summary>
        /// Event that allows user to move cell
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
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

        /// <summary>
        /// Event that allows user to edit cell
        /// </summary>
        /// <param name="contents"></param>
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

        /// <summary>
        /// Event to close file, prompts dialog if not saved
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event to save as file, prompts dialog if not saved
        /// </summary>
        /// <param name="fileName"></param>
        private void HandleSaveFile(String fileName)
        {
            TextWriter r = new StreamWriter(fileName);
            previousFile = fileName;
            sheet.Save(r);
            r.Close();
        }

        /// <summary>
        /// Event to save file, prompts dialog if not saved initially
        /// </summary>
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

        /// <summary>
        /// Event to open file, prompts dialog if not saved
        /// </summary>
        /// <param name="fileName"></param>
        private void HandleOpenFile(String fileName)
        {
            TextReader r = new StreamReader(fileName);
            window.OpenNew(r);
            r.Close();
        }

        /// <summary>
        /// Private helper method to retrieve cell name 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private string getCellName(int r, int c)
        {
            return "" + (char)('A' + c) + (r + 1);
        }

        /// <summary>
        /// Private helper method to retrieve column 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int getColumn(string name)
        {
            return (name.ToCharArray()[0] - 'A');
        }

        /// <summary>
        /// Private helper method to retrieve row 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int getRow(string name)
        {
            int.TryParse(name.Substring(1, name.Length - 1), out int row);
            return row -1;
        }
    }
}
