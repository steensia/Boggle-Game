using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            s.SetCellContents("A2", new Formula("A1 + 1.0"));
            s.SetCellContents("A3", new Formula("A2 + 1.0"));
            Assert.AreEqual(s.GetCellContents("A1"),1.0);
        }
    }
}
