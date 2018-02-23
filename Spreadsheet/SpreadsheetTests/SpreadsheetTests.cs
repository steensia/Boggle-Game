using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;
using System.Text.RegularExpressions;

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
            s.SetContentsOfCell("A1", "1.0");
            s.SetContentsOfCell("A2", "=A4 + 1.0");
            s.SetContentsOfCell("A3", "=A2 + 1.0");
            s.SetContentsOfCell("A4", "=A3 + 1.0");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularTest1000()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A999");
            for (int i = 2; i < 1000; i++)
                s.SetContentsOfCell("A" + i, ("=A" + i) + "+" + ("A" + (i - 1)));
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
        public void InvalidNameGetContentsa1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("a*1");
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
            s.SetContentsOfCell("A0", "=1+1");
        }

        [TestMethod]
        public void SetCellContentsEmptyString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsa1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a*1", "test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsZ02()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z02", "3.14159");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetContentsNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "0.0");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void SetContentsNullValue()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("JACOB13", null);
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
                s.SetContentsOfCell(cell, cell);
                refrence.Add(cell);
            }

            HashSet<string> sheetNames = new HashSet<string>(s.GetNamesOfAllNonemptyCells());


            Assert.IsTrue(refrence.Equals(refrence));
        }

        [TestMethod]
        public void GetValidCellContentsDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1.0");
            Assert.AreEqual(1.0, s.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetValidCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "test");
            Assert.AreEqual("test", s.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetCellValueEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("",s.GetCellValue("BlAzE420"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "test");
            s.GetCellContents(null);
            Assert.AreEqual("test", s.GetCellContents(null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "test");
            s.GetCellValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsInvalidName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "test");
            s.GetCellContents("420_BLAZE_");
            Assert.AreEqual("test", s.GetCellContents("420_BLAZE_"));
        }

        [TestMethod]
        public void GetEmptyCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "test");
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        [TestMethod]
        public void AddingTextCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1.0");
            for (int i = 1; i <= 10; i++)
            {
                s.SetContentsOfCell("A" + i, "A" + i);
            }
        }

        [TestMethod]
        public void FormulaReferencingText()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "text");
            s.SetContentsOfCell("B1", "=A1 * 2");
        }

        [TestMethod]
        public void UpdateCells()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1.0");
            for (int i = 2; i <= 10; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i - 1) + " + 1.0");
                Assert.AreEqual((double)i,s.GetCellValue("A"+i));
            }

            s.SetContentsOfCell("A1", "2.0");

            for (int i = 1; i <= 10; i++)
            {
                Assert.AreEqual(i+1.0, s.GetCellValue("A" + i));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCellsNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1.0");
            for (int i = 2; i <= 10; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i - 1) + " + 1.0");
                Assert.AreEqual((double)i, s.GetCellValue("A" + i));
            }

            s.SetContentsOfCell("A1", null);

            for (int i = 1; i <= 10; i++)
            {
                Assert.AreEqual(i + 1.0, s.GetCellValue("A" + i));
            }
        }

        [TestMethod]
        public void EvaluationError()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "blaze");
            s.SetContentsOfCell("A2", "=X5+1");
            s.SetContentsOfCell("A3", "=A2+1");
            Exception e = new FormulaEvaluationException("");
            Assert.IsTrue(typeof(FormulaEvaluationException) == ((Exception)s.GetCellValue("A3")).GetType());
        }

        [TestMethod]
        public void Fibinochi()
        {
            double i0, i1, i2;
            i0 = 1.0;
            i1 = 1.0;
            Spreadsheet s = new Spreadsheet(new Regex("^.*$"));
            s.SetContentsOfCell("A1", "1.0");
            s.SetContentsOfCell("A2", "1.0");

            for (int i = 3; i <= 20; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                s.SetContentsOfCell("A" + i, "=A" + (i - 1) + "+A" + (i - 2));
                Assert.AreEqual(s.GetCellValue("A" + i), i2);
            }

            i0 = 42;
            i1 = 42;
            s.SetContentsOfCell("A1", ""+i0);
            s.SetContentsOfCell("A2", ""+i1);
            for (int i = 3; i <= 20; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                Assert.AreEqual(i2, s.GetCellValue("A" + i));
            }      
        }

        [TestMethod]
        public void FibonacciSave()
        {
            double i0, i1, i2;
            i0 = 1.0;
            i1 = 1.0;
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1.0");
            s.SetContentsOfCell("A2", "1.0");
            s.SetContentsOfCell("B1", "Fibonacci");

            for (int i = 3; i <= 10; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                s.SetContentsOfCell("A" + i, "=A" + (i - 1) + "+A" + (i - 2));
                Assert.AreEqual(s.GetCellValue("A" + i), i2);
            }
            Assert.IsTrue(s.Changed);
            TextWriter t = new StreamWriter("Fibonacci.xml");
            s.Save(t);
            t.Close();
            TextReader r = new StreamReader("Fibonacci.xml");
            s = new Spreadsheet(r, new Regex("^[A-Z]+[1-9][0-9]*$"));

            i0 = 1.0;
            i1 = 1.0;

            for (int i = 3; i <= 10; i++)
            {
                i2 = i0 + i1;
                i0 = i1;
                i1 = i2;
                Assert.AreEqual(s.GetCellValue("A" + i), i2);
            }
        }

        [TestMethod]
        public void LoadValid()
        {
            TextReader r = new StreamReader("./SampleSavedSpreadsheet.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^[A-Z]+[1-9][0-9]*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void LoadInvalidSchema()
        {
            TextReader r = new StreamReader("./invalidSchema.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void LoadInvalidRegex()
        {
            TextReader r = new StreamReader("./invalidRegex.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void LoadDuplicate()
        {
            TextReader r = new StreamReader("./repeatedElement.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void LoadInvalidWithOldIsValid()
        {
            TextReader r = new StreamReader("./invalidCellNameA08.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetVersionException))]
        public void LoadInvalidWithNewIsValid()
        {
            TextReader r = new StreamReader("./valid.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^[B-Z]+[1-9][0-9]*$"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void LoadCircularException()
        {
            TextReader r = new StreamReader("./circularDependency.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
        }

        [TestMethod]
        public void LoadValid2()
        {
            TextReader r = new StreamReader("./valid.xml");
            Spreadsheet s = new Spreadsheet(r, new Regex("^.*$"));
            Assert.IsFalse(s.Changed);
        }
    }
}
