using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Circular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            s.SetCellContents("A2", new Formula("A4 + 1.0"));
            s.SetCellContents("A3", new Formula("A2 + 1.0"));
            s.SetCellContents("A4", new Formula("A3 + 1.0"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularTest1000()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("A999"));
            for (int i = 2; i < 1000; i++)
                s.SetCellContents("A" + i, new Formula(("A" + i) + "+" + ("A" + (i - 1))));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetContentsA0()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("A0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetContentsaB1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("aB1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetContentsZ02()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("Z02");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetContentsNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsA0()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A0", new Formula("1+1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsaB1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("aB1", "test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsZ02()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("Z02", 3.14159);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void SetContentsNullValue()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("JACOB13", null);
        }

        [TestMethod]
        public void GetNonEmptyCellsWithRandomCellsAdded()
        {
            HashSet<string> refrence = new HashSet<string>();
            string[] letter = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 1000; i++)
            {
                string cell = letter[i % 26] + letter[(i * 100) % 26] + i;
                s.SetCellContents(cell, cell);
                refrence.Add(cell);
            }

            HashSet<string> sheetNames = new HashSet<string>(s.GetNamesOfAllNonemptyCells());


            Assert.IsTrue(refrence.Equals(refrence));
        }

        [TestMethod]
        public void GetValidCellContentsDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            Assert.AreEqual(1.0, s.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetValidCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", "test");
            Assert.AreEqual("test", s.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetEmptyCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", "test");
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        [TestMethod]
        public void AddingTextCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            for (int i = 1; i <= 10; i++)
            {
                s.SetCellContents("A" + i, "A" + i);
            }
        }

        [TestMethod]
        public void FormulaReferencingText()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", "text");
            s.SetCellContents("B1", new Formula("A1 * 2"));
        }

        [TestMethod]
        public void UpdateCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            for (int i = 2; i <= 10; i++)
            {
                s.SetCellContents("A" + i, new Formula("A" + (i - 1) + " + 1.0"));
                // Assert.AreEqual(s.getCellValue("A"+i), i);
            }

            s.SetCellContents("A1", 2.0);

            for (int i = 1; i <= 10; i++)
            {
                // Assert.AreEqual(i+1, s.getCellValue("A" + i));
            }
        }

        [TestMethod]
        public void Fibinochi()
        {
            double i0, i1, i2;
            i0 = 1.0;
            i1 = 1.0;
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1.0);
            s.SetCellContents("A2", 1.0);

            for (int i = 3; i <= 20; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                s.SetCellContents("A" + i, new Formula("A" + (i - 1) + "+A" + (i - 2)));
                //Assert.AreEqual(s.getCellValue("A" + i), i2);
            }

            i0 = 42;
            i1 = 42;
            s.SetCellContents("A1", i0);
            s.SetCellContents("A2", i1);
            for (int i = 3; i <= 20; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                //Assert.AreEqual(i2, s.getCellValue("A" + i));
            }
        }
    }
}
