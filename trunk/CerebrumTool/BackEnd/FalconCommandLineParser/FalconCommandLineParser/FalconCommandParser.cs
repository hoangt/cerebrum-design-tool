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
/*********************************************************************************************************** 
 * FalconCommandParser.cs
 * Name: Matthew Cotter
 * Date: 4 Jun 2010 
 * Description: Class library to be used for parsing an input string from the Cerebrum bash shell command line into FalconCommand objects
 * that expose the switch/command and all arguments to that command. Based on the following format:
 *      -[-] <command_switch> [arg0 arg1 ...]
 * Notes:
 *     
 * History: 
 * >> (10 Jun 2010): Corrected parsing of multiple hyphens and whitespace ignoring outside of quoted strings only.
 * >> ( 7 Jun 2010): Added support for dynamic command delimiter. (Default is "--", settable by constructor or property)
 *                   Removed collapsing of double-delimiters into a single delimiter.
 * >> ( 4 Jun 2010): Added Implementation of FalconGlobal.IFalconLibrary to FalconCommandParser class
 * >> ( 4 Jun 2010): Implemented FalconCommandParser class and FalconCommand class for Parsing the command line specification.
 * >> ( 4 Jun 2010): Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace FalconCommandLineParser
{
    /// <summary>
    /// Defines a single parsed command or switch, exposing the command string as well as arguments passed to it.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/FalconCommandLineParser Specification.pdf">
    /// Falcon Command Line Parser Documentation</seealso>
    public class FalconCommand
    {
        private string cmd;
        private LinkedList<string> args;

        /// <summary>
        /// Public constructor for creating a new FalconCommand object, using the specified command string.
        /// </summary>
        /// <param name="command">The command string that was parsed as a new command or switch.</param>
        public FalconCommand(string command)
        {
            //cmd = command.ToLower();    // Case-insensitive switches
            cmd = command;              // Case-sensitive switches
            args = new LinkedList<string>();
        }

        /// <summary>
        /// Adds a new argument to the collection of arguments for this command while parsing the command.
        /// </summary>
        /// <param name="newArg"></param>
        public void AddArgument(string newArg)
        {
            args.AddLast(newArg);
        }

        /// <summary>
        /// Public accessor to get the command string parsed to create this FalconCommand.
        /// </summary>
        public string CommandSwitch
        {
            get
            {
                return cmd;
            }
        }

        /// <summary>
        /// Public accessor to get the list of arguments parsed to create this FalconCommand.
        /// </summary>
        public LinkedList<string> Arguments
        {
            get
            {
                if (args == null)
                    return new LinkedList<string>();
                else
                    return new LinkedList<string>(args);
            }
        }
    }

    /// <summary>
    /// Class that is used to parse a string to extract a set of FalconCommands based on the Cerebrum command-line specification.
    /// </summary>
    public class FalconCommandParser : FalconGlobal.IFalconLibrary 
    {
        private LinkedList<FalconCommand> commandList;
        private bool bParseError;
        private string sDelimiter;

        /// <summary>
        /// Public constructor to initialize the FalconCommandParser object using the default command delimiter "--".
        /// </summary>
        public FalconCommandParser()
        {
            sDelimiter = "--";
        }

        /// <summary>
        /// Public constructor to initialize the FalconCommandParser object using the specified string as the command delimiter.
        /// </summary>
        public FalconCommandParser(string CommandDelimiter)
        {
            sDelimiter = CommandDelimiter;
        }
        /// <summary>
        /// Read-Only. Indicates whether the most recent call to either ParseString(string) or ParseFile(string). resulted in an error during parsing.   
        /// A value of true here indicates that theparsing may be incomplete and use of the Commands property is not advised until a 
        /// successful parsing has been completed.
        /// </summary>
        public bool ParseError
        {
            get
            {
                return bParseError;
            }
        }

        /// <summary>
        /// Read-Only. Public accessor to get the list of FalconCommand objects extracted during the most recent call to either ParseString(string) or 
        /// ParseFile(string). If ParseError is true, this list may be incomplete and should not be used until a succesful parsing has been completed.
        /// </summary>
        public LinkedList<FalconCommand> Commands
        {
            get
            {
                if (commandList == null)
                    return new LinkedList<FalconCommand>();
                else
                    return new LinkedList<FalconCommand>(commandList);
            }
        }

        /// <summary>
        /// Get or set the string used to delimit commands within a string. Default value is "--".
        /// </summary>
        public string CommandDelimiter
        {
            get
            {
                return sDelimiter;
            }
            set
            {
                sDelimiter = value;
            }
        }

        /// <summary>
        /// Processes the input string and attempts to extract a series of FalconCommand objects from it.   This function sets the value of the 
        /// ParseError property as well as populates the list returned by the Commands property.
        /// </summary>
        /// <param name="inputString">The string from which to parse FalconCommands.</param>
        /// <returns>True if the parsing was successful, False otherwise.   Upon return, the ParseError will has the inverse of this function's return value.</returns>
        public bool ParseString(string inputString)
        {
            try
            {
                // Prepare the string

                inputString = inputString.Trim();
                inputString = inputString.Replace(" \\\r\n", " ");          // Convert Properly-Escaped Windows NewLine to space
                inputString = inputString.Replace(" \\\n", " ");            // Convert Properly-Escaped Unix/Linux NewLine to space

                // Simple sanity check: Check for un-balanced double-quotes
                int quoteCount = 0;
                int qIndex = 0;
                qIndex = inputString.IndexOf("\"", qIndex);
                while (qIndex != -1)
                {
                    qIndex += 1;
                    quoteCount++; 
                    qIndex = inputString.IndexOf("\"", qIndex);
                }
                if ((quoteCount % 2) != 0)
                {
                    bParseError = true;
                    return !bParseError;
                }


                commandList = new LinkedList<FalconCommand>();
                FalconCommand lastParsedCommand = null;

                int sIndex = 0;
                bParseError = false;
                while (sIndex < inputString.Length)
                {
                    int spaceIndex = inputString.IndexOf(" ", sIndex);
                    int tabIndex = inputString.IndexOf("\t", sIndex);
                    if ((spaceIndex != sIndex) && (tabIndex != sIndex))
                    {
                        int spIndex;
                        if (spaceIndex != -1)
                        {
                            spIndex = spaceIndex;
                            if (spIndex < tabIndex)
                                spIndex = tabIndex;
                        }
                        else if (tabIndex != -1)
                        {
                            spIndex = tabIndex;
                        }
                        else
                        {
                            spIndex = -1;
                        }
                        
                        int quoteIndex = inputString.IndexOf("\"", sIndex);
                        int tokenEnd = -1;
                        if (quoteIndex == sIndex)
                            tokenEnd = inputString.IndexOf("\"", quoteIndex + 1);
                        else if (spIndex != -1)
                            tokenEnd = spIndex;
                        else
                            tokenEnd = inputString.Length;

                        int startIndex = sIndex;
                        int tokenLen = tokenEnd - sIndex + (tokenEnd == inputString.Length ? 0 : 1);

                        string token = inputString.Substring(startIndex, tokenLen);
                        token = token.Replace("\"", string.Empty).Trim();
                        if (token.StartsWith(sDelimiter))
                        {
                            // Check that the command doesn't contain another delimiter
                            token = token.Substring(1).Trim();
                            if ((token.StartsWith(sDelimiter)) || (token == string.Empty))
                            {
                                bParseError = true;
                                break;
                            }
                            token = token.Substring(sDelimiter.Length - 1).Trim();
                            if (token == string.Empty)
                            {
                                bParseError = true;
                                break;
                            }

                            // New Command
                            lastParsedCommand = new FalconCommand(token);
                            commandList.AddLast(lastParsedCommand);
                        }
                        else if (lastParsedCommand == null)
                        {
                            // Non switch token parsed without a valid command parsed beforehand
                            bParseError = true;
                            break;
                        }
                        else
                        {
                            token = token.Replace("\"", string.Empty).Trim();
                            lastParsedCommand.AddArgument(token);
                        }

                        sIndex += tokenLen;
                    }
                    else
                    {
                        sIndex++;
                    }
                }
                return !bParseError;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An unexpected exception was thrown. \n{0}", ex.Message);
                bParseError = true;
                return !bParseError;
            }
        }

        /// <summary>
        /// Processes the file text and attempts to extract a series of FalconCommand objects from it.   This function sets the value of the 
        /// ParseError property as well as populates the list returned by the Commands property.
        /// </summary>
        /// <param name="FilePath">The path to the file from which to parse FalconCommands.</param>
        /// <returns>True if the parsing was successful, False otherwise.   Upon return, the ParseError will has the inverse of this function's return value.</returns>
        public bool ParseFile(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    StreamReader reader = new StreamReader(FilePath);

                    //StringBuilder fileContents = new StringBuilder();
                    //while (!reader.EndOfStream)
                    //    fileContents.AppendLine(reader.ReadLine());
                    //return ParseString(fileContents.ToString());
                    string fileContents = reader.ReadToEnd();
                    reader.Close();

                    // Pre-process the file to eliminate new-lines
                    // Process escaped new-lines
                    fileContents = fileContents.Replace(" \\\r\n", " ");          // Convert Properly-Escaped Windows NewLine to space
                    fileContents = fileContents.Replace(" \\\n", " ");            // Convert Properly-Escaped Unix/Linux NewLine to space

                    // Process non-escaped new-lines
                    fileContents = fileContents.Replace("\r\n", " ");          // Convert Windows NewLine to space
                    fileContents = fileContents.Replace("\n", " ");            // Convert Unix/Linux NewLine to space
                    return ParseString(fileContents);
                }
                else
                {
                    Console.WriteLine("{0} could not be opened.", FilePath);
                    bParseError = true;
                    return !bParseError;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected exception was thrown. \n{0}", ex.Message);
                bParseError = true;
                return !bParseError;
            }
        }

        #region Implementation of FalconGlobal.IFalconLibrary

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon Command-Line Parser";
            }
        }

        /// <summary>
        /// Returns the version of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentVersion.
        /// </summary>
        public string FalconComponentVersion
        {
            get
            {
                return "1.0.0";
            }
        }

        /// <summary>
        /// Returns the name/version/copyright information of this library component.  Implementation of 
        /// FalconGlobal.IFalconLibrary.GetFalconComponentVersion().
        /// </summary>
        public string GetFalconComponentVersion()
        {
            return String.Format("{0} {1} Copyright (c) 2010 PennState", FalconComponentName, FalconComponentVersion);
        }

        #endregion
    }
}
