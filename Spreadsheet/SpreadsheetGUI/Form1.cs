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

            dataGridView1.ColumnCount = 26;
            dataGridView1.RowCount = 99;

            sheet.SetContentsOfCell("B1", "2");
            sheet.SetContentsOfCell("A2", "=A1*2");

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].Name = "" + (char)('A' + i);
            }


            for (int r = 0; r < 99; r++)
            {
                string[] l = new string[26];

                for (int c = 0; c < 26; c++)
                {
                    l[c] = (sheet.GetCellValue(getCellName(r, c)).ToString());

                }
                dataGridView1.Rows[r].SetValues(l);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                selectedCell = getCellName(e.RowIndex, e.ColumnIndex);
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
            }
            catch (Exception ex)
            {
                Value.Text = ex.ToString();
            }

            for (int r = 0; r < 99; r++)
            {
                for (int c = 0; c < 26; c++)
                {
                    dataGridView1.Rows[r].Cells[c].Value = (sheet.GetCellValue(getCellName(r, c)).ToString());
                }
            }
        }

        private string getCellName(int r, int c)
        {
            return "" + (char)('A' + c) + (r + 1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
