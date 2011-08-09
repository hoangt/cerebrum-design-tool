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
 * ErrorReporting.cs
 * Name: Matthew Cotter
 * Date: 17 Sep 2010 
 * Description: Static class defining methods to trace and debug detailed exception information.
 * History: 
 * >> (19 Oct 2010) Matthew Cotter: Corrected bug in full stack trace generation.
 * >> (15 Oct 2010) Matthew Cotter: Added overloads to output full stack-traces of exception details.
 * >> (10 Oct 2010) Matthew Cotter: Added Trace output to MessageBox output.
 * >> (17 Sep 2010) Matthew Cotter: Defined methods to Trace and Debug exceptions with details.
 * >> (17 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Static class to facilitate error reporting, tracing and debugging of exceptions.
    /// </summary>
    public static class ErrorReporting
    {
        /// <summary>
        /// Outputs a simple report of detail and stack trace of an exception to Trace output
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        public static void TraceException(Exception ex)
        {
            Trace.WriteLine(ExceptionDetails(ex));
        }
        /// <summary>
        /// Outputs a simple report of detail and stack trace of an exception to Debug output
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        public static void DebugException(Exception ex)
        {
            Debug.WriteLine(ExceptionDetails(ex));
        }
        /// <summary>
        /// Outputs a simple report of detail and stack trace of an exception to a MessageBox
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        public static void MessageBoxException(Exception ex)
        {
            TraceException(ex);
            MessageBox.Show(ExceptionDetails(ex), "Exception Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Outputs a report of detail and stack trace of an exception to Trace output
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        /// <param name="FullTrace">If True, a full stack trace is reported.  If False, only a single stack frame is output</param>
        public static void TraceException(Exception ex, bool FullTrace)
        {
            Trace.WriteLine(ExceptionDetails(ex, FullTrace));
        }
        /// <summary>
        /// Outputs a report of detail and stack trace of an exception to Debug output
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        /// <param name="FullTrace">If True, a full stack trace is reported.  If False, only a single stack frame is reported.</param>
        public static void DebugException(Exception ex, bool FullTrace)
        {
            Debug.WriteLine(ExceptionDetails(ex, FullTrace));
        }
        /// <summary>
        /// Outputs a report of detail and stack trace of an exception to a MessageBox
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        /// <param name="FullTrace">If True, a full stack trace is reported.  If False, only a single stack frame is reported.</param>
        public static void MessageBoxException(Exception ex, bool FullTrace)
        {
            TraceException(ex, FullTrace);
            MessageBox.Show(ExceptionDetails(ex, FullTrace), "Exception Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Creates a simple report of detail and stack trace of an exception
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        /// <returns>The exception details report</returns>
        public static string ExceptionDetails(Exception ex)
        {
            return ExceptionDetails(ex, false);
        }
        /// <summary>
        /// Creates a report of detail and stack trace of an exception
        /// </summary>
        /// <param name="ex">The exception to report on</param>
        /// <param name="FullTrace">If True, a full stack trace is created.  If False, only a single stack frame is created</param>
        /// <returns>The exception details report</returns>
        public static string ExceptionDetails(Exception ex, bool FullTrace)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            StringBuilder ErrorMessage = new StringBuilder();
            ErrorMessage.AppendLine(String.Format("Exception thrown: {0}", ex.GetType().ToString()));
            ErrorMessage.AppendLine(String.Format("Message: {0}", ex.Message));
            ErrorMessage.AppendLine(String.Format("Module: {0}", ex.TargetSite.Module.Name));
            ErrorMessage.AppendLine(String.Format("Method: {0}", ex.TargetSite.ToString()));
            ErrorMessage.AppendLine(String.Format("File: {0}", trace.GetFrame(0).GetFileName()));
            ErrorMessage.AppendLine(String.Format("Line: {0}", trace.GetFrame(0).GetFileLineNumber()));
            ErrorMessage.AppendLine(String.Format("Column: {0}", trace.GetFrame(0).GetFileColumnNumber()));

            if (FullTrace)
            {
                for (int i = 1; i < trace.FrameCount; i++)
                {
                    if (trace.GetFrame(i).GetFileName() == null)
                        break;
                    if (trace.GetFrame(i).GetFileName().Trim() == string.Empty)
                        break;
                    ErrorMessage.AppendLine(String.Format("File: {0}", trace.GetFrame(i).GetFileName()));
                    ErrorMessage.AppendLine(String.Format("Line: {0}", trace.GetFrame(i).GetFileLineNumber()));
                    ErrorMessage.AppendLine(String.Format("Column: {0}", trace.GetFrame(i).GetFileColumnNumber()));
                }
            }
            return ErrorMessage.ToString();
        }
    }
}
