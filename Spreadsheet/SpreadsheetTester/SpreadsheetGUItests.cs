using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.IO;
using System.Windows.Forms;

namespace SpreadsheetTester
{
    /// <summary>
    /// Summary description for SpreadsheetGUItests
    /// </summary>
    [TestClass]
    public class SpreadsheetGUItests
    {
        /// <summary>
        /// Fire CloseFileEvent and see if it executes code in control, dialog doesn't show
        /// </summary>
        [TestMethod]
        public void CloseFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireCloseFileEvent(new FormClosingEventArgs(new CloseReason(), false));
            Assert.IsFalse(stub.CalledShowFileNotSavedDialog);
        }
        [TestMethod]
        /// <summary>
        /// Fire CloseFileEvent and see if it executes code in control, dialog shows
        /// </summary>
        public void CloseFileTest2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireContentsChangedEvent("k");
            stub.FireCloseFileEvent(new FormClosingEventArgs(new CloseReason(), false));
            Assert.IsTrue(stub.CalledShowFileNotSavedDialog);
        }
        [TestMethod]
        /// <summary>
        /// Fire OpenFileEvent and see if it executes code in control, open new called
        /// </summary>
        public void OpenFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireOpenFileEvent("../../test.xml");
            Assert.IsTrue(stub.CalledOpenNew);
        }
        [TestMethod]
        /// <summary>
        /// Fire LoadSpreadsheetEvent and see if it executes code in control, loads the GUI
        /// </summary>
        public void LoadSpreadsheetTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireLoadSpreadsheetEvent();
            Assert.IsTrue(stub.CalledSetCellSelection);
            Assert.IsTrue(stub.CalledSetCellValue);
        }
        [TestMethod]
        /// <summary>
        /// Fire SelectionChangeEvent and see if it executes code in control, enter contents
        /// </summary>
        public void SelectionChangedTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireSelectionChangedEvent(2, 2);
            stub.FireContentsChangedEvent("=1");
            stub.FireSelectionChangedEvent(1, 1);
            stub.FireContentsChangedEvent("=B2");
            stub.FireSelectionChangedEvent(2, 2);
            Assert.IsTrue(stub.NameBox.Equals("C3"));
        }

        [TestMethod]
        /// <summary>
        /// Fire SaveFileEvent and see if it executes code in control, prompt save as 
        /// </summary>
        public void SaveFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            StreamReader file = new StreamReader("../../SampleSavedSpreadsheet.xml");
            Controller controller = new Controller(stub, file);
            stub.FireSelectionChangedEvent(1, 1);
            stub.FireSaveFileEvent("../../test2.xml");
            Assert.IsTrue(stub.NameBox.Equals("B2"));
        }

        [TestMethod]
        /// <summary>
        /// Fire SaveFileEvent and see if it executes code in control, prompt save
        /// </summary>
        public void SaveTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            StreamReader file = new StreamReader("../../SampleSavedSpreadsheet.xml");
            Controller controller = new Controller(stub, file);
            stub.FireContentsChangedEvent("");
            stub.FireSaveEvent();

            SpreadsheetViewStub stub2 = new SpreadsheetViewStub();
            Controller controller2 = new Controller(stub2);
            stub2.FireSaveEvent();
            Assert.IsTrue(stub2.CalledShowSaveDialog);
        }
    }
}
