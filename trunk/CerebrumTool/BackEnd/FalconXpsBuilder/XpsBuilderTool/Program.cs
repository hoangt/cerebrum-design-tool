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
/************************************************************************************************************
 * FalconXpsBuilderTool\Program.cs
 * Name: Matthew Cotter
 * Date: 25 Jun 2010 
 * Description: This program implements the command-line bash-style interface between the Cerebrum command shell and that
 * of the FalconXpsBuilder library.
 * Notes:
 *      Commands can be disabled by removing/commenting their corresponding AddCommand() calls in PopulateCommandTable().
 *          Removing a command in this manner leaves the functionality in tact, but removes the capability for the command to be recognized
 *          as it will not be present in the command table when checked.  Translation from FalconCommand to XPSCommand is done inside
 *          the loop in RunSequence().
 *      New Commands are implemented by: 1) Adding a value to the XPSCommandEnum enumeration.
 *                                       2) Adding a call to AddCommand() with the appropriate information in PopulateCommandTable()
 *                                       3) Add the code to the 'switch()' block to execute the new command in:
 *                                                  a) class XPSCommand.Execute() for XPS Builder system commands
 *                                                  b) class XPSBuilderToolWrapperClass.RunSequence() for Built-in tool commands
 * History: 
 * >> (24 Aug 2010) Matthew Cotter: Added support to force a "clean", ensuring that ALL XPS Project files are removed--both locally and remotely.
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
 * >> (13 Aug 2010) Matthew Cotter: Removed platform file from cerebinput command due to new hierarchical location and format of platforms.
 * >> (29 Jul 2010) Matthew Cotter: Added '--force' command to allow for forcing "empty" platforms to be synthesized. 
 *                                      Empty platforms are those that have no cores other than the base system cores.
 * >>                               Corrected bug in which and unhandled exception was erroneously reported by RunSequence.
 * >> (21 Jul 2010) Matthew Cotter: Verified that XML-generated Documentation is current.
 * >> (25 Jun 2010) Matthew Cotter: Modelled/copied file from corresponding source for mapping tool.
 * >> (25 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconXpsBuilder;
using System.IO;
using System.Collections;
using FalconCommandLineParser;
using FalconGlobal;

namespace XpsBuilderTool
{
    class Program
    {
        private class XPSBuilderToolWrapperClass
        {

            public static void StandardWrite(string Text, params object[] list)
            {
                string outputText = Text;
                for (int i = 0; i < list.Length; i++)
                    if (list[i] != null)
                    {
                        outputText = outputText.Replace("{" + i.ToString() + "}", list[i].ToString());
                    }
                    else
                    {
                        outputText = outputText.Replace("{" + i.ToString() + "}", string.Empty);
                    }
                Console.Write(outputText);
            }
            public static void StandardWriteLine()
            {
                Console.WriteLine();
            }
            public static void StandardWriteLine(string Text, params object[] list)
            {
                StandardWrite(Text, list);
                StandardWriteLine();
            }
            
            public static string StandardReadLine()
            {
                return Console.ReadLine();
            }


            public enum XPSCommandEnum : int
            {
                InvalidCommand = 0,       // Default/dummy command.  NOP - halts execution when encountered in batch mode.
                // Essentially indicates an invalid command.

                #region Built-in Commands
                Help,
                Version,
                ScriptFile,
                InteractiveMode,
                PrintWorkingDirectory,
                ListDirectory,
                ChangeDirectory,
                #endregion

                Cerebrum_InputFiles,
                Property_AllowEmptySynth,
                Property_ForceClean
            }

            public static bool IsBuiltInCommand(XPSCommandEnum xce)
            {
                switch (xce)
                {
                    case XPSCommandEnum.Help:
                        return true;
                    case XPSCommandEnum.Version:
                        return true;
                    case XPSCommandEnum.PrintWorkingDirectory:
                        return true;
                    case XPSCommandEnum.ListDirectory:
                        return true;
                    case XPSCommandEnum.ChangeDirectory:
                        return true;

                    default:
                        return false;
                }
            }

            public bool bInteractive = false;

            public bool Interactive
            {
                get
                {
                    return bInteractive;
                }
                set
                {
                    bInteractive = value;
                }
            }

            public XPSBuilderToolWrapperClass()
            {
                xps = new FalconXPSBuilderControl();
                PopulateCommandTable();
            }

            private class XPSCommand : IComparable
            {
                private XPSCommandEnum _cmd;
                private ArrayList _args;
                private int _reqargs;
                private string _cmdstring;

                public XPSCommand(XPSCommandEnum Cmd, int ReqArgs, string CommandString)
                {
                    _cmd = Cmd;
                    _args = new ArrayList();
                    _reqargs = ReqArgs;
                    _cmdstring = CommandString;
                }

                public XPSCommandEnum Command
                {
                    get
                    {
                        return _cmd;
                    }
                }

                public int RequiredArgs
                {
                    get
                    {
                        return _reqargs;
                    }
                }

                public int ArgCount
                {
                    get
                    {
                        return _args.Count;
                    }
                }

                internal void SetArg(int i, string val)
                {
                    if ((i > _args.Count) || (i < 0))
                    {
                        StandardWriteLine("\tFATAL Error: Argument index out of bounds\n");
                    }
                    else
                    {
                        _args[i] = val;
                    }
                }

                public string CommandString
                {
                    get
                    {
                        return _cmdstring;
                    }
                }

                public string GetArg(int i)
                {
                    if ((i > _args.Count) || (i < 0))
                    {
                        StandardWriteLine("\tFATAL Error: Argument index out of bounds\n");
                        return string.Empty;
                    }
                    return (string)_args[i];
                }

                public void AddArg(string arg)
                {
                    _args.Add(arg);
                }

                public bool Execute(XpsBuilder xps)
                {
                    if (_args.Count < _reqargs)
                    {
                        StandardWriteLine("\tERROR: Insufficient Arguments\n");
                        return false;
                    }
                    try
                    {
                        switch (_cmd)
                        {


                            // Break out on built-in commands
                            // Break out on enumeration commands
                            case XPSCommandEnum.InvalidCommand:
                                break;
                            default:
                                StandardWriteLine("Unimplemented command: {0}.", _cmdstring);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        StandardWriteLine("ERROR: {0}", ex.Message);
                    }
                    return true;
                }

                public int CompareTo(object other)
                {
                    // Return < 0 -> This "is less than" other
                    // Return = 0 -> This "is equal to" other
                    // Return > 0 -> This "is greater than" other

                    if (other.GetType() != typeof(XPSCommand))
                        return 0;

                    XPSCommand This = this;
                    XPSCommand Other = (XPSCommand)other;

                    if (This.Command == Other.Command)
                        return 0;

                    //Sort invalids to the front
                    if (This.Command == XPSCommandEnum.InvalidCommand)
                        return -1;

                    //Sort invalids to the front
                    if (Other.Command == XPSCommandEnum.InvalidCommand)
                        return 1;

                    string ThisCommand = This.Command.ToString();
                    string OtherCommand = Other.Command.ToString();
                    bool ThisIsProperty = (ThisCommand.StartsWith("Property"));
                    bool OtherIsProperty = (OtherCommand.StartsWith("Property"));
                    bool ThisIsBuiltIn = XPSBuilderToolWrapperClass.IsBuiltInCommand(This.Command);
                    bool OtherIsBuiltIn = XPSBuilderToolWrapperClass.IsBuiltInCommand(Other.Command);

                    if ((ThisIsBuiltIn) && (OtherIsBuiltIn))
                        return 0;
                    if (ThisIsBuiltIn)
                        return -1;
                    if (OtherIsBuiltIn)
                        return 1;

                    if ((ThisIsProperty) && (OtherIsProperty))
                        return 0;
                    if ((!ThisIsProperty) && (!OtherIsProperty))
                        return 0;

                    if (ThisIsProperty)
                        return -1;
                    if (OtherIsProperty)
                        return 1;

                    return 0;
                }
            }
            private XPSCommand CreateFromString(string cmd)
            {
                XPSCommand xpc = null;
                cmd = cmd.ToLower().Trim('-');

                // The command table is first checked to ensure that the command string exists
                // If it does not exist, the command is interpreted as InvalidCommand, or a NOP.
                // When it's executed, an error will be displayed and if executing in batch-mode, execution will terminate.
                // If running in interactive mode, the state will be that of the system IMMEDIATELY before the invalid command.
                if (CommandTable.ContainsKey(cmd))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[cmd];
                    xpc = new XPSCommand(ci.Command, ci.MinArguments, cmd);
                }
                else
                {
                    xpc = new XPSCommand(XPSCommandEnum.InvalidCommand, 0, cmd);
                }
                return xpc;
            }

            private class CommandInfo
            {
                private int HELPLINE_LENGTH = 70;
                public XPSCommandEnum Command;
                public string Literal;
                public string Help;
                public int MinArguments;

                public CommandInfo(XPSCommandEnum ID, string CmdLiteral, int RequiredArgs, string HelpText)
                {
                    Command = ID;
                    Literal = CmdLiteral;
                    MinArguments = RequiredArgs;

                    StringBuilder helpBuilder = new StringBuilder();
                    Help = String.Format("{0} {1}", Literal, HelpText);
                    string[] help = Help.Split('\n');
                    for (int i = 0; i < help.Length; i++)
                    {
                        string hline = help[i].Trim();
                        if (i == 0)
                        {
                            helpBuilder.AppendLine(hline);
                        }
                        else if (hline.Length < HELPLINE_LENGTH)
                        {
                            helpBuilder.AppendLine("\t" + hline);
                        }
                        else
                        {
                            while (hline.Length > HELPLINE_LENGTH)
                            {
                                int lastsp = hline.IndexOf(" ", (int)(0.8 * HELPLINE_LENGTH));
                                string linestart;
                                if (lastsp >= 0)
                                {
                                    linestart = hline.Substring(0, lastsp).Trim();
                                    hline = hline.Substring(lastsp + 1).Trim();
                                }
                                else
                                {
                                    linestart = hline;
                                    hline = string.Empty;
                                }
                                helpBuilder.AppendLine("\t" + linestart);
                            }
                            if (hline.Length > 0)
                            {
                                helpBuilder.AppendLine("\t" + hline);
                            }
                        }
                    }
                    Help = helpBuilder.ToString();
                }
            }

            private Hashtable CommandTable;
            private void AddCommand(string Literal, XPSCommandEnum ID, int ReqArgs, string HelpInfo)
            {
                CommandTable.Add(Literal, new CommandInfo(ID, Literal, ReqArgs, HelpInfo));
            }
            private void PopulateCommandTable()
            {
                CommandTable = new Hashtable();

                #region Built-in Commands
                AddCommand("help", XPSCommandEnum.Help, 0,
                    "\n      Displays the command help screen.");
                AddCommand("version", XPSCommandEnum.Version, 0,
                    "\n      Displays the current version of the mapping tool.");
                AddCommand("batch", XPSCommandEnum.ScriptFile, 0,
                    "<file_path>\n      Executes a series of command as listed in the specified script file.  Must be the FIRST command in the sequence, and all other commands are ignored.  If it is not the first command, the script file is ignored.");

                AddCommand("console", XPSCommandEnum.InteractiveMode, 0,
                    "\n      Start the map tool in interactive mode.");

                AddCommand("pwd", XPSCommandEnum.PrintWorkingDirectory, 0,
                    "\n      Displays the current working directory.");
                AddCommand("ls", XPSCommandEnum.ListDirectory, 0,
                    "\n      Displays the contents of the current directory.");
                AddCommand("cd", XPSCommandEnum.ChangeDirectory, 1,
                    "\n      Changes the current working directory.");
                #endregion

                AddCommand("cerebinput", XPSCommandEnum.Cerebrum_InputFiles, 3,
                    "<paths_file> <server_file> <design_file>\n      Begins generation of the XPS projects for each of the FPGA platforms using the specified input files from cerebrum.");
                AddCommand("force", XPSCommandEnum.Property_AllowEmptySynth, 0,
                    "\n      Force synthesis of \"empty\" platforms. (Those with no cores other than the base system).");
                AddCommand("clean", XPSCommandEnum.Property_ForceClean, 0,
                    "\n      Forces an explicit removal of ALL associated XPS Project files both locally and on the synthesis server.");

            }

            private void DisplayHelpInfo()
            {
                ArrayList keys = new ArrayList();
                keys.AddRange(CommandTable.Keys);
                keys.Sort();
                for (int i = 0; i < keys.Count; i++)
                {
                    CommandInfo ci = (CommandInfo)CommandTable[keys[i]];
                    StandardWriteLine(ci.Help);
                }
            }
            private void DisplayHelpInfo(string command)
            {
                if (CommandTable.ContainsKey(command))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[command];
                    StandardWriteLine(ci.Help);
                }
                else
                {
                    StandardWriteLine("Unrecognized command: {0}\n", command);
                    DisplayHelpInfo();
                }
            }

            private static void ChangeDirectory(XPSCommand xpc)
            {
                if (xpc.ArgCount > 0)
                {
                    StringBuilder path = new StringBuilder();
                    for (int a = 0; a < xpc.ArgCount; a++)
                    {
                        path.Append(xpc.GetArg(a) + " ");
                    }
                    string newdir = path.ToString().Trim();
                    if (System.IO.Directory.Exists(newdir))
                    {
                        System.IO.Directory.SetCurrentDirectory(newdir);
                        StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                    }
                    else
                    {
                        StandardWriteLine("Unable to change directory: {0} was not found", newdir);
                    }
                }
                else
                {
                    System.IO.Directory.SetCurrentDirectory(
                        new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName);
                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                }
            }
            private static void ListDirectory()
            {
                string[] dirs = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory());
                string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory());
                ArrayList alDirs = new ArrayList();
                alDirs.AddRange(dirs);
                alDirs.Sort();
                ArrayList alFiles = new ArrayList();
                alFiles.AddRange(files);
                alFiles.Sort();

                StringBuilder listing = new StringBuilder();
                listing.AppendLine();
                listing.AppendLine("Directories:");
                int row = 0;
                for (int d = 0; d < alDirs.Count; d++)
                {
                    DirectoryInfo di = new DirectoryInfo((string)alDirs[d]);
                    string shortname = di.Name + "\\";
                    shortname = shortname.PadRight(30);
                    if (row == 0)
                        listing.Append("\t");
                    listing.Append(shortname);
                    row = (row + 1) % 2;
                    if (row == 0)
                        listing.AppendLine();
                }
                listing.AppendLine();
                listing.AppendLine();
                listing.AppendLine("Files:");
                row = 0;
                for (int f = 0; f < alFiles.Count; f++)
                {
                    FileInfo fi = new FileInfo((string)alFiles[f]);
                    string shortname = fi.Name;
                    shortname = shortname.PadRight(30);
                    if (row == 0)
                        listing.Append("\t");
                    listing.Append(shortname);
                    row = (row + 1) % 2;
                    if (row == 0)
                        listing.AppendLine();
                }
                listing.AppendLine();
                StandardWriteLine(listing.ToString());
            }
            private static FalconXPSBuilderControl xps;

            private bool SortSequence(LinkedList<FalconCommand> FalconCommands, out LinkedList<XPSCommand> XPSCommands, out string FailedCommand)
            {
                ArrayList cmdList = new ArrayList();
                XPSCommands = new LinkedList<XPSCommand>();
                FailedCommand = string.Empty;

                // Create set of JProgCommands and place them into an ArrayList
                for (LinkedListNode<FalconCommand> fcNode = FalconCommands.First; fcNode != null; fcNode = fcNode.Next)
                {
                    FalconCommand fc = fcNode.Value;
                    FailedCommand = fc.CommandSwitch;
                    XPSCommand jpc = CreateFromString(fc.CommandSwitch);
                    if (jpc.Command != XPSCommandEnum.InvalidCommand)
                    {
                        for (LinkedListNode<string> node = fc.Arguments.First; node != null; node = node.Next)
                        {
                            jpc.AddArg(node.Value);
                        }
                        cmdList.Add(jpc);
                        FailedCommand = jpc.CommandString;
                        if (jpc.RequiredArgs > jpc.ArgCount)
                        {
                            return false;
                        }
                        else
                        {
                            switch (jpc.Command)
                            {
                                

                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // Insertion Sort into LinkedList<XPSCommand>
                while (cmdList.Count > 0)
                {
                    XPSCommand thisXPC = (XPSCommand)cmdList[0];
                    if (XPSCommands.Count == 0)
                    {
                        XPSCommands.AddLast(thisXPC);
                    }
                    else
                    {
                        LinkedListNode<XPSCommand> xpsNode;
                        bool bAdded = false;
                        for (xpsNode = XPSCommands.First; xpsNode != null; xpsNode = xpsNode.Next)
                        {
                            int cmp = thisXPC.CompareTo(xpsNode.Value);
                            if (cmp < 0)
                            {
                                // thisJPC should come before jpNode
                                XPSCommands.AddBefore(xpsNode, thisXPC);
                                bAdded = true;
                                break;
                            }
                            else if (cmp > 0)
                            {
                                // thisJPC should come after jpNode
                            }
                            else
                            {
                                // These are the same
                            }
                        }
                        if (!bAdded)
                        {
                            if (xpsNode == null)
                            {
                                XPSCommands.AddLast(thisXPC);
                            }
                            else
                            {
                                XPSCommands.AddAfter(xpsNode, thisXPC);
                            }
                        }
                    }
                    cmdList.Remove(thisXPC);
                }
                FailedCommand = string.Empty;
                return true;
            }

            public bool RunSequence(LinkedList<FalconCommand> fCommands)
            {
                XPSCommand xpc = null;
                try
                {
                    if (fCommands.Count == 0)
                        return true;
                    LinkedList<XPSCommand> XPSCommands;

                    string failedCmd;
                    if (!SortSequence(fCommands, out XPSCommands, out failedCmd))
                    {
                        DisplayHelpInfo(failedCmd);
                        StandardWriteLine("Error parsing command ({0}) in sequence.", failedCmd);
                    }
                    for (LinkedListNode<XPSCommand> node = XPSCommands.First; node != null; node = node.Next)
                    {
                        xpc = node.Value;
                        if (xpc != null)
                        {
                            switch (xpc.Command)
                            {
                                case XPSCommandEnum.InvalidCommand:
                                    StandardWriteLine("Unrecognized command : {0}.\n", xpc.CommandString);
                                    DisplayHelpInfo();
                                    if (fCommands.Count > 1)
                                        StandardWriteLine("Batch execution halted.\n\n");
                                    return false;
                                case XPSCommandEnum.Help:
                                    if (xpc.ArgCount > 0)
                                        DisplayHelpInfo(xpc.GetArg(0));
                                    else
                                        DisplayHelpInfo();
                                    break;
                                case XPSCommandEnum.Version:
                                    DisplayVersionInfo();
                                    break;
                                case XPSCommandEnum.ScriptFile:
                                    break;
                                case XPSCommandEnum.InteractiveMode:
                                    break;
                                case XPSCommandEnum.PrintWorkingDirectory:
                                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                                    break;
                                case XPSCommandEnum.ListDirectory:
                                    ListDirectory();
                                    break;
                                case XPSCommandEnum.ChangeDirectory:
                                    ChangeDirectory(xpc);
                                    break;

                                case XPSCommandEnum.Cerebrum_InputFiles:
                                    if (xps.LoadPaths(xpc.GetArg(0)))
                                    {
                                        if (!xps.ConfigureAndBuild(xpc.GetArg(1), xpc.GetArg(2)))
                                            return false;
                                    }
                                    break;

                                case XPSCommandEnum.Property_AllowEmptySynth:
                                    xps.AllowEmptySynth = true;
                                    break;

                                case XPSCommandEnum.Property_ForceClean:
                                    xps.ForceClean = true;
                                    break;
                                    
                                default:
                                    DisplayHelpInfo(xpc.CommandString);
                                    return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (xpc != null)
                        StandardWriteLine("An error occurred in the XPS project builder:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An error occurred executing command '{0}'.", xpc.CommandString));
                    else
                        StandardWriteLine("An error occurred in the XPS project builder:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An unknown error occurred while processing commands."));
                    return false;
                }
                return true;
            }

            public void DisplayVersionInfo()
            {
                try
                {
                    //string DLLver = System.Reflection.Assembly.GetAssembly(typeof(FalconMapping_Objects)).GetName().Version.ToString(4);
                    //string EXEver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
                    //StandardWriteLine("Version Information:\n\tFalcon Component Mapping Tool Version: {0}\n\tFalcon Component Mapping Algorithm Version: {1}", EXEver, DLLver);
                    XpsBuilder XPS = new XpsBuilder();
                    if (XPS is IFalconLibrary)
                    {
                        IFalconLibrary IFalconDLL = (IFalconLibrary)XPS;
                        StandardWriteLine(IFalconDLL.GetFalconComponentVersion());
                    }
                    else
                    {
                        StandardWriteLine("{0} does not implement IFalconLibrary.", XPS.GetType().ToString());
                    }
                }
                catch (Exception ex)
                {
                    StandardWriteLine("An error occurred in the XPS project builder:\n\t{0}\n\t{1}", ex.Message,
                        String.Format("Unable to poll component version information."));
                }
            }

            /// <summary>
            /// Writes the JProgrammerTool command prompt to the StandardWriteLine
            /// </summary>
            public void Prompt()
            {
                StandardWrite("xpsbuild> ");
            }

            /// <summary>
            /// Writes the initial shell welcome to the System.Console.
            /// </summary>
            public void ShellWelcome()
            {
                Console.WriteLine(new XpsBuilder().GetFalconComponentVersion());
                Console.WriteLine();
            }

            /// <summary>
            /// Displays the interactive-mode welcome to the Console.
            /// </summary>
            public void InteractiveWelcome()
            {
                Console.WriteLine("Type 'batch <script_file>' to parse a script file.");
                Console.WriteLine("Type 'quit' or 'exit' to terminate.");
                Console.WriteLine();
            }


            /// <summary>
            /// Determines whether the specified string is a built-in Quit or Exit command.
            /// </summary>
            /// <param name="InputString">The raw string read in from input to be tested.</param>
            /// <returns>True if the string is a match for a quit command, False otherwise.</returns>
            public bool IsQuitCommand(string InputString)
            {
                return ((InputString == "quit") || (InputString == "exit"));
            }
        }

        private static XPSBuilderToolWrapperClass XPS;
        private static FalconCommandParser parser;
        private static bool bInteractiveMode = false;

        static int Main(string[] args)
        {
            XPS = new XPSBuilderToolWrapperClass();
            parser = new FalconCommandParser();
            XPS.ShellWelcome();

            if (args.Length > 0)
            {
                string argline = "\"" + string.Join("\" \"", args) + "\"";

                return ParseLine(argline);
            }
            // No arguments passed, just terminate
            return 0;
        }

        private static int ParseLine(string argline)
        {
            if (parser.ParseString(argline))
            {
                bool success = true;
                LinkedList<FalconCommand> llCommands;
                llCommands = parser.Commands;
                if (llCommands.Count > 0)
                {
                    // Hard-coded batch command
                    FalconCommand firstCommand = llCommands.First.Value;
                    if (firstCommand.CommandSwitch.ToLower() == "batch")
                    {
                        if (firstCommand.Arguments.Count > 0)
                        {
                            if (parser.ParseFile(firstCommand.Arguments.First.Value))
                            {
                                llCommands = parser.Commands;
                                success = success && XPS.RunSequence(llCommands);
                                if (success)
                                    return 0;
                                else
                                    return -1;
                            }
                            else
                            {
                                Console.WriteLine("Encountered an error in parsing the script file.");
                                return -1;
                            }
                        }
                        else
                        {
                            // No file specified to batch argument, remove it
                            llCommands.RemoveFirst();
                            return 0;
                        }
                    }
                    else if (firstCommand.CommandSwitch.ToLower() == "console")
                    {
                        // Start interactive mode
                        InteractiveMode();
                        return 0;
                    }
                    else
                    {
                        // First command is not a batch or console command
                        success = success && XPS.RunSequence(llCommands);
                        if (success)
                            return 0;
                        else
                            return -1;
                    }
                }
                // Parser succeeded, no commands found
            }
            else
            {
                Console.WriteLine("Encountered an error in parsing the command string.");
                return -1;
            }
            return 0;
        }

        public static void InteractiveMode()
        {
            if (bInteractiveMode)
                return;
            bInteractiveMode = true;
            XPS.Interactive = true;
            XPS.InteractiveWelcome();
            XPS.Prompt();
            string input = Console.ReadLine();
            while ((input != "quit") && (input != "exit"))
            {
                while (input.EndsWith(" \\"))
                    input = input + Environment.NewLine + Console.ReadLine();
                if (!input.StartsWith("--"))
                {
                    input = "--" + input;
                }
                ParseLine(input);
                XPS.Prompt();
                input = Console.ReadLine();
            }
        }
    }
}
