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

namespace SpreadsheetGUI
{
    public partial class Form_2 : Form
    {
        private SS.Spreadsheet sheet;
        private string selectedCell;
        public Form_2()
        {
            sheet = new SS.Spreadsheet(new Regex("^[A-Z][1-9][0-9]?$"));
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    spreadsheetPanel1.SetValue(c, r, sheet.GetCellValue(getCellName(r, c)).ToString());
                }
            }
            selectedCell = "A1";
            spreadsheetPanel1_SelectionChanged(spreadsheetPanel1);
        }

        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            sender.GetSelection(out int c, out int r);
            if (c >= 0 && r >= 0)
            {
                selectedCell = getCellName(r, c);
                Value.Text = sheet.GetCellValue(selectedCell).ToString();
                object o = sheet.GetCellContents(selectedCell);
                if (o is Formula)
                {
                    Contents.Text = "=" + o.ToString();
                }
                else
                {
                    Contents.Text = o.ToString();
                }
                CellName.Text = selectedCell;
            }
        }

        private void Contents_Changed(object sender, EventArgs e)
        {
            try
            {
                sheet.SetContentsOfCell(selectedCell, Contents.Text);
                Value.Text = sheet.GetCellValue(selectedCell).ToString();
                ErrorBox.Text = "";
            }
            catch (Exception ex)
            {
                ErrorBox.Text = ex.ToString();
            }

            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    spreadsheetPanel1.SetValue(c, r, sheet.GetCellValue(getCellName(r, c)).ToString());
                }
            }
        }

        private string getCellName(int r, int c)
        {
            return "" + (char)('A' + c) + (r + 1);
        }

        private void New_Click(object sender, EventArgs e)
        {
            Form_2  w = new Form_2();
            w.Show();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            
            //sheet.Save();
        }

        private void Load_Click(object sender, EventArgs e)
        {
            sheet = new SS.Spreadsheet();
            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    sheet.GetCellValue(getCellName(r, c)).ToString();
                }
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}
