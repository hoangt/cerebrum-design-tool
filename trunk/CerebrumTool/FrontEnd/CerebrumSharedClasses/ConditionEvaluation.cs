/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/******************************************************************** 
 * ConditionEvaluation.cs
 * Name: Matthew Cotter
 * Date: 24 Oct 2010 
 * Description: Defines static methods to evaluate string expressions as integers or booleans.
 * History: 
 * >> (24 Oct 2010) Matthew Cotter: Defined methods to reduce and evaluate a condition string as either a boolean or integer using XML XPath evaluation engine.
 * >> (24 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Static class used to provide methods for reducing and evaluation integer and boolean expression strings.
    /// </summary>
    public static class Conditions
    {
        /// <summary>
        /// Evaluates the specified string as a boolean.
        /// </summary>
        /// <param name="Condition">Condition string to evaluate.</param>
        /// <returns>True if the condition evaluates to true.  False if it does not, or an error occurs.</returns>
        public static bool EvaluateAsBoolean(string Condition)
        {
            string Expression = Condition;
            if ((Expression == null) || (Expression == string.Empty))
                return true;
            try
            {
                System.Xml.XPath.XPathDocument xpd = new System.Xml.XPath.XPathDocument(new System.IO.StringReader("<r/>"));
                System.Xml.XPath.XPathNavigator xpn = xpd.CreateNavigator();
                Expression = ReduceToXPathString(Expression);
                object result = xpn.Evaluate(Expression);
                return (bool)result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("XPATH EXPRESSION FAILED to evaluate: '{0}'", Expression));
                ErrorReporting.DebugException(ex, true);
            }
            return false;
        }
        /// <summary>
        /// Evaluates the specified string as ag integer.
        /// </summary>
        /// <param name="Expression">Expression string to evaluate.</param>
        /// <returns>The value the expression evaluates to, if it is an integer; 0 if the expression is empty; int.MinValue otherwise</returns>
        public static int EvaluateAsInteger(string Expression)
        {
            if ((Expression == null) || (Expression == string.Empty))
                return 0;
            try
            {
                System.Xml.XPath.XPathDocument xpd = new System.Xml.XPath.XPathDocument(new System.IO.StringReader("<r/>"));
                System.Xml.XPath.XPathNavigator xpn = xpd.CreateNavigator();
                Expression = ReduceToXPathString(Expression);
                object result = xpn.Evaluate(Expression);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("XPATH EXPRESSION FAILED to evaluate: '{0}'", Expression));
                ErrorReporting.DebugException(ex, true);
            }
            return int.MinValue;
        }
        private static string ReduceToXPathString(string Expression)
        {
            //Trace.WriteLine(String.Format("Reducing Expression: {0}", Expression));
            while (Expression.Contains("! "))
                Expression = Expression.Replace("! ", "!");

            while (Expression.Contains("[xstrncmp "))
            {
                string equalityOp = "=";

                int negatebrack = Expression.IndexOf("![");
                int leftbrack = Expression.IndexOf("[");
                int rightbrack = Expression.IndexOf("]");

                if ((negatebrack > 0) && (negatebrack == (leftbrack - 1)))
                {
                    leftbrack = negatebrack;
                    equalityOp = "!=";
                }

                string substring = Expression.Substring(leftbrack, rightbrack - leftbrack + 1);
                string first = string.Empty;
                string second = string.Empty;
                string[] tokens = substring.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 3)
                {
                    Expression = Expression.Replace(substring, "1");
                }
                first = tokens[1].Trim();
                second = tokens[2].Trim();
                if ((first == string.Empty) || (second == string.Empty))
                {
                    Expression = Expression.Replace(substring, "1");
                }

                Expression = Expression.Replace(substring, String.Format("(\"{0}\" {1} \"{2}\")", first, equalityOp, second));
            }
            while (Expression.Contains("&&"))
                Expression = Expression.Replace("&&", " and ");
            while (Expression.Contains("=="))
                Expression = Expression.Replace("==", "=");
            while (Expression.Contains("||"))
                Expression = Expression.Replace("||", " or ");
            while (Expression.Contains("!("))
                Expression = Expression.Replace("!(", " not(");
            while (Expression.Contains("  "))
                Expression = Expression.Replace("  ", " ");

            //Trace.WriteLine(String.Format("Reduced To: {0}", Expression));
            return Expression;
        }
    }
}
