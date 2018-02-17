// Jacob Haydel u1137077
// Skeleton written by Joe Zachary for CS 3500, January 2017

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        private List<string> formula;

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            if (formula == null) throw new ArgumentNullException();
            String lpPattern = @"^\($";
            String rpPattern = @"^\)$";
            String opPattern = @"^[\+\-*/]$";
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            String doublePattern = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:e[\+-]?\d+)?$";
            String spacePattern = @"^\s+$";

            IEnumerable<string> tokens = GetTokens(formula);
            this.formula = new List<string>();
            int lpCount = 0, rpCount = 0;
            Boolean shouldBeNumber = true;


            foreach (String s in tokens)
            {
                if (!Regex.IsMatch(s, spacePattern))
                {
                    if (shouldBeNumber)
                    {
                        if (Regex.IsMatch(s, varPattern + "|" + doublePattern))
                        {
                            shouldBeNumber = false;
                        }
                        else if (Regex.IsMatch(s, lpPattern))
                        {
                            lpCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException("Invalid token:" + s);
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(s, opPattern))
                        {
                            shouldBeNumber = true;
                        }
                        else if (Regex.IsMatch(s, rpPattern))
                        {
                            rpCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException("Invalid token:" + s);
                        }
                    }
                    if (lpCount < rpCount) throw new FormulaFormatException("Parethesis missmatch");
                    this.formula.Add(s);
                }
            }
            if (this.formula.Count <= 0) throw new FormulaFormatException("empty");
            if (Regex.IsMatch(this.formula[this.formula.Count - 1], opPattern)) throw new FormulaFormatException("ends in operator");
            if (lpCount != rpCount) throw new FormulaFormatException("Parethesis missmatch");
        }




        public Formula(String formula, Normalizer norm, Validator val)
        {
            if (formula == null || norm == null || val == null) throw new ArgumentNullException();
            String lpPattern = @"^\($";
            String rpPattern = @"^\)$";
            String opPattern = @"^[\+\-*/]$";
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            String doublePattern = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:e[\+-]?\d+)?$";
            String spacePattern = @"^\s+$";

            IEnumerable<string> tokens = GetTokens(formula);
            this.formula = new List<string>();
            int lpCount = 0, rpCount = 0;
            Boolean shouldBeNumber = true;
            String buff = "";


            foreach (String s in tokens)
            {
                buff = s;
                if (!Regex.IsMatch(s, spacePattern))
                {
                    if (shouldBeNumber)
                    {
                        if (Regex.IsMatch(s, doublePattern))
                        {
                            shouldBeNumber = false;
                        }
                        else if (Regex.IsMatch(s, varPattern))
                        {
                            shouldBeNumber = false;
                            buff = norm(s);
                            if (!Regex.IsMatch(buff, varPattern)) throw new FormulaFormatException("Invalide var");
                            if (!val(buff)) throw new FormulaFormatException("Invalide var");
                        }
                        else if (Regex.IsMatch(s, lpPattern))
                        {
                            lpCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException("Invalid token:" + s);
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(s, opPattern))
                        {
                            shouldBeNumber = true;
                        }
                        else if (Regex.IsMatch(s, rpPattern))
                        {
                            rpCount++;
                        }
                        else
                        {
                            throw new FormulaFormatException("Invalid token:" + s);
                        }
                    }
                    if (lpCount < rpCount) throw new FormulaFormatException("Parethesis missmatch");
                    this.formula.Add(buff);
                }
            }
            if (this.formula.Count <= 0) throw new FormulaFormatException("empty");
            if (Regex.IsMatch(this.formula[this.formula.Count - 1], opPattern)) throw new FormulaFormatException("ends in operator");
            if (lpCount != rpCount) throw new FormulaFormatException("Parethesis missmatch");
        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            if (lookup == null) throw new ArgumentNullException();
            Stack<string> tokens = NewMethod();
            Stack<double> numbers = new Stack<double>();
            tokens.Push(null);

            //Used to hold numbers while determining what to do with them for proccessing
            double dBuff;

            foreach (String s in formula)
            {
                if (double.TryParse(s, out dBuff))
                {
                    if ("*".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        numbers.Push(numbers.Pop() * dBuff);
                    }
                    else if ("/".Equals(tokens.Peek()))
                    {
                        if (dBuff == 0) throw new FormulaEvaluationException("/0");
                        tokens.Pop();
                        numbers.Push(numbers.Pop() / dBuff);
                    }
                    else
                    {
                        numbers.Push(dBuff);
                    }
                }
                else if (Regex.IsMatch(s, "^[a-zA-Z][0-9a-zA-Z]*$"))
                {

                    try
                    {
                        dBuff = lookup(s);
                    }
                    catch
                    {
                        throw new FormulaEvaluationException(s);
                    }

                    if ("*".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        numbers.Push(numbers.Pop() * dBuff);
                    }
                    else if ("/".Equals(tokens.Peek()))
                    {
                        if (dBuff == 0) throw new FormulaEvaluationException("/0");
                        tokens.Pop();
                        numbers.Push(numbers.Pop() / dBuff);
                    }
                    else
                    {
                        numbers.Push(dBuff);
                    }
                }
                else if (Regex.IsMatch(s, "[+-]"))
                {
                    if ("+".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        numbers.Push(numbers.Pop() + dBuff);
                    }
                    else if ("-".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        numbers.Push(numbers.Pop() - dBuff);
                    }
                    tokens.Push(s);
                }
                else if (s.Equals(")"))
                {
                    if ("+".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        numbers.Push(numbers.Pop() + dBuff);
                    }
                    else if ("-".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        numbers.Push(numbers.Pop() - dBuff);
                    }
                    tokens.Pop();
                    if ("*".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        numbers.Push(numbers.Pop() * dBuff);
                    }
                    else if ("/".Equals(tokens.Peek()))
                    {
                        tokens.Pop();
                        dBuff = numbers.Pop();
                        if (dBuff == 0) throw new FormulaEvaluationException("/0");
                        numbers.Push(numbers.Pop() / dBuff);
                    }
                }
                else
                {
                    tokens.Push(s);
                }
            }

            if (tokens.Peek() != null)
            {
                if ("+".Equals(tokens.Peek()))
                {
                    tokens.Pop();
                    dBuff = numbers.Pop();
                    numbers.Push(numbers.Pop() + dBuff);
                }
                else if ("-".Equals(tokens.Peek()))
                {
                    tokens.Pop();
                    dBuff = numbers.Pop();
                    numbers.Push(numbers.Pop() - dBuff);
                }
            }
            return numbers.Pop();
        }

        public ISet<string> GetVariables()
        {
            HashSet<string> s = new HashSet<string>();
            foreach (string t in formula)
            {
                if (Regex.IsMatch(t, @"^[a-zA-Z][0-9a-zA-Z]*$"))
                    s.Add(t);
            }
            return s;
        }

        override public string ToString()
        {
            string buff = "";
            foreach (string t in formula)
            {
                buff += t;
            }
            return buff;
        }

        private static Stack<string> NewMethod()
        {
            return new Stack<string>();
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            // NOTE:  These patterns are designed to be used to create a pattern to split a string into tokens.
            // For example, the opPattern will match any string that contains an operator symbol, such as
            // "abc+def".  If you want to use one of these patterns to match an entire string (e.g., make it so
            // the opPattern will match "+" but not "abc+def", you need to add ^ to the beginning of the pattern
            // and $ to the end (e.g., opPattern would need to be @"^[\+\-*/]$".)
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);
    public delegate string Normalizer(string s);
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
