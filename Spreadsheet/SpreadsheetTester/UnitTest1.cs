using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;

namespace SpreadsheetTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CloseFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireCloseFileEvent(new FormClosingEventArgs(new CloseReason(), false));
            Assert.IsFalse(stub.CalledShowFileNotSavedDialog);
        }
        [TestMethod]
        public void CloseFileTest2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireContentsChangedEvent("k");
            stub.FireCloseFileEvent(new FormClosingEventArgs(new CloseReason(), false));
            Assert.IsTrue(stub.CalledShowFileNotSavedDialog);
        }
        [TestMethod]
        public void OpenFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireOpenFileEvent("../../test.xml");
            Assert.IsTrue(stub.CalledOpenNew);
        }
        [TestMethod]
        public void LoadSpreadsheetTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireLoadSpreadsheetEvent();
            Assert.IsTrue(stub.CalledSetCellSelection);
            Assert.IsTrue(stub.CalledSetCellValue);
        }
        [TestMethod]
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
        public void SaveFileTest()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            StreamReader file = new StreamReader("../../SampleSavedSpreadsheet.xml");
            Controller controller = new Controller(stub, file);
            stub.FireSelectionChangedEvent(1, 1);
            stub.FireSaveFileEvent("../../test2.xml");
            Assert.IsTrue(stub.NameBox.Equals("B2"));
        }

    }
}
