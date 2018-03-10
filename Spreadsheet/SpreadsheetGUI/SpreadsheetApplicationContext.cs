using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    class SpreadsheetApplicationContext : ApplicationContext
    {
        private int windows = 0;

        private static SpreadsheetApplicationContext context;

        public static SpreadsheetApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadsheetApplicationContext();
            }
            return context;
        }

        public void RunNew()
        {
            SpreadsheetWindow window = new SpreadsheetWindow();
            new Controller(window);

            windows++;

            window.FormClosed += (a, b) => { if (--windows <= 0) ExitThread(); };

            window.Show();
        }

        public void RunNew(TextReader file)
        {
            SpreadsheetWindow window = new SpreadsheetWindow();
            new Controller(window, file);

            windows++;

            window.FormClosed += (a, b) => { if (--windows <= 0) ExitThread(); };

            window.Show();
        }
    }
}
