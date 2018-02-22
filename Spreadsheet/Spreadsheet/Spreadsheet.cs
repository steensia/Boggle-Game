// Written by Jacob Haydel for CS 3500, February 2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SS
{
    // MODIFIED PARAGRAPHS 1-3 AND ADDED PARAGRAPH 4 FOR PS6
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of a regular expression (called IsValid below) and an infinite 
    /// number of named cells.
    /// 
    /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
    /// expression IsValid.IsMatch(s.ToUpper()) is true.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
    /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
    /// names, regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized by converting all letters to upper case before it is used by this 
    /// this spreadsheet.  For example, the Formula "x3+a5" should be normalize to "X3+A5" before 
    /// use.  Similarly, all cell names and Formulas that are returned or written to a file must also
    /// be normalized.
    /// 
    /// A spreadsheet contains a unique cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //stores the dependecy relations between cells in the spread sheet
        private DependencyGraph dependancyGraph;
        //stores a list of non-epty cells
        private Dictionary<string, Cell> cells;
        //stores the regular expression to validate the cell names in the current instance of spreadsheet
        private Regex isValid;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        //a simple struct to store value and content of unkown type
        private struct Cell
        {
            public object content, value;

            public Cell(object content, object value)
            {
                this.content = content;
                this.value = value;
            }
        }

        /// <summary>
        /// zero argument constructor that makes an empty spread sheet
        /// </summary>
        public Spreadsheet()
        {
            dependancyGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
            isValid = new Regex("^[A-Z]+[1-9][0-9]*$");
            Changed = false;
        }

        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        public Spreadsheet(Regex isValid)
        {
            dependancyGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
            this.isValid = isValid;
            Changed = false;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newIsValid)
        {
            dependancyGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
            isValid = newIsValid;


            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(null, "./Spreadsheet.xsd");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = schema;

            using (XmlReader t = XmlReader.Create(source, settings))
            {
                t.MoveToContent();
                string temp = t.GetAttribute("IsValid");
                Regex oldIsValid;

                try
                {
                    oldIsValid = new Regex(temp);
                }
                catch (ArgumentException e)
                {
                    throw new SpreadsheetReadException("invalid regex read from file");
                }

                try
                {
                    while (t.Read())
                    {
                        if (t.IsStartElement())
                        {
                            string name = t.GetAttribute("name");
                            if (!IsValidName(name, oldIsValid)) throw new SpreadsheetReadException(name);
                            if (cells.ContainsKey(name)) throw new SpreadsheetReadException(name + " was repeated");
                            if (!IsValidName(name)) throw new SpreadsheetVersionException(name);
                            string contents = t.GetAttribute("contents");
                            SetContentsOfCell(name, contents);
                        }
                    }
                }
                catch (CircularException e)
                {
                    throw new SpreadsheetReadException(e.ToString());
                }
            }
            Changed = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (!IsValidName(name)) throw new InvalidNameException();

            if (cells.TryGetValue(name, out Cell c))
                return c.content;
            else return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter t = XmlWriter.Create(dest, settings))
            {
                t.WriteStartElement("spreadsheet");
                t.WriteAttributeString("IsValid", isValid.ToString());

                foreach (String s in cells.Keys)
                {
                    if (cells.TryGetValue(s, out Cell c))
                    {
                        t.WriteStartElement("cell");
                        t.WriteAttributeString("name", s);
                        if (c.content.GetType() == typeof(Double))
                        {
                            t.WriteAttributeString("contents", ((double)c.content).ToString());
                        }
                        else if (c.content.GetType() == typeof(Formula))
                        {
                            t.WriteAttributeString("contents", "=" + ((Formula)c.content).ToString());
                        }
                        else
                        {
                            t.WriteAttributeString("contents", (string)c.content);
                        }
                        t.WriteWhitespace("");
                        t.WriteEndElement();
                    }
                }
                t.WriteEndElement();
            }
            Changed = false;
        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (!IsValidName(name)) throw new InvalidNameException();

            if (cells.TryGetValue(name, out Cell c))
            {
                return c.value;
            }
            return "";
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) throw new ArgumentNullException();
            if (!IsValidName(name)) throw new InvalidNameException();
            name = name.ToUpper();

            if (Double.TryParse(content, out double d))
            {
                return SetCellContents(name, d);
            }
            else if (content.Length > 1 && content.Substring(0, 1).Equals("="))
            {
                Formula f = new Formula(content.Substring(1, content.Length - 1), s => s.ToUpper(), s => IsValidName(s));
                return SetCellContents(name, f);
            }
            else
            {
                return SetCellContents(name, content);
            }

        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (!IsValidName(name)) throw new InvalidNameException();

            cells.Remove(name);
            cells.Add(name, new Cell(number, number));

            dependancyGraph.ReplaceDependees(name, new Stack<string>());

            IEnumerable<string> rec = GetCellsToRecalculate(name);

            foreach (string s in rec)
            {
                recalulate(s);
            }

            Changed = true;
            return getAllDependents(name);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException();
            if (!IsValidName(name)) throw new InvalidNameException();

            dependancyGraph.ReplaceDependees(name, new Stack<string>());

            cells.Remove(name);

            if (text.Equals("")) return getAllDependents(name);

            cells.Add(name, new Cell(text, text));

            Changed = true;
            return getAllDependents(name);
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (!IsValidName(name)) throw new InvalidNameException();

            dependancyGraph.ReplaceDependees(name, formula.GetVariables().Distinct());

            ISet<string> dependents = getAllDependents(name);

            cells.Remove(name);
            cells.Add(name, new Cell(formula, null));

            IEnumerable<string> rec = GetCellsToRecalculate(name);

            foreach (string s in rec)
            {
                recalulate(s);
            }

            Changed = true;
            return dependents;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null) throw new ArgumentNullException();
            if (!IsValidName(name)) throw new InvalidNameException();

            return dependancyGraph.GetDependents(name);
        }

        /// <summary>
        /// gets all direct and indirect dependees of a given cell
        /// </summary>
        private HashSet<string> getAllDependents(string name)
        {
            HashSet<string> h0 = new HashSet<string>(new string[] { name });
            HashSet<string> h1 = new HashSet<string>(dependancyGraph.GetDependents(name));
            HashSet<string> h2 = new HashSet<string>();

            while (h1.Count > 0)
            {
                foreach (string s in h1)
                {
                    if (s.Equals(name)) throw new CircularException();
                    h0.Add(s);
                }

                h2 = new HashSet<string>();

                foreach (string s in h1)
                {
                    foreach (string t in dependancyGraph.GetDependents(s))
                    {
                        h2.Add(t);
                    }
                }
                h1 = h2;
            }
            return h0;
        }

        /// <summary>
        /// recalulates the value of a given cell if it is a formula cell
        /// </summary>
        private void recalulate(string name)
        {
            if (cells.TryGetValue(name, out Cell c))
            {
                if (c.content.GetType() == typeof(Formula))
                {
                    try
                    {
                        c.value = ((Formula)c.content).Evaluate(s => formulaCellValue(s));
                    }
                    catch (Exception e)
                    {
                        c.value = e;
                    }
                    cells.Remove(name);
                    cells.Add(name, c);
                }
            }
        }

        /// <summary>
        /// get the value of a given cell
        /// </summary>
        private double formulaCellValue(string name)
        {
            if (cells.TryGetValue(name, out Cell c))
            {
                if (c.value.GetType() == typeof(Double))
                {
                    return (Double)c.value;
                }
                else if (c.value.GetType() == typeof(Exception))
                {
                    throw (Exception)(c.value);
                }
            }
            throw new UndefinedVariableException(name);
        }

        /// <summary>
        /// returns true if name is a valid cell name and false otherwise
        /// </summary>
        private Boolean IsValidName(string name)
        {
            if (name == null || !Regex.IsMatch(name.ToUpper(), "^[A-Z]+[1-9][0-9]*$") || !isValid.IsMatch(name.ToUpper()))
                return false;
            return true;
        }

        /// <summary>
        /// returns true if name is a valid cell name and false otherwise
        /// </summary>
        private Boolean IsValidName(string name, Regex valid)
        {
            if (name == null || !Regex.IsMatch(name.ToUpper(), "^[A-Z]+[1-9][0-9]*$") || !valid.IsMatch(name.ToUpper()))
                return false;
            return true;
        }

    }
}
