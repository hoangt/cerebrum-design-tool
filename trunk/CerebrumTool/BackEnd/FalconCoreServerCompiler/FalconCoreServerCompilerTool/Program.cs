/************************************************************************************************************
 * FalconCoreServerCompilerTool\Program.cs
 * Name: Matthew Cotter
 * Date: 05 Jul 2010 
 * Description: This program implements the command-line bash-style interface between the Cerebrum command shell and that
 * of the FalconCoreServerCompiler library.
 * Notes:
 *      Commands can be disabled by removing/commenting their corresponding AddCommand() calls in PopulateCommandTable().
 *          Removing a command in this manner leaves the functionality in tact, but removes the capability for the command to be recognized
 *          as it will not be present in the command table when checked.  Translation from FalconCommand to ToolCommand is done inside
 *          the loop in RunSequence().
 *      New Commands are implemented by: 1) Adding a value to the ToolCommandEnum enumeration.
 *                                       2) Adding a call to AddCommand() with the appropriate information in PopulateCommandTable()
 *                                       3) Add the code to the 'switch()' block to execute the new command in class ToolWrapperClass.RunSequence()
 * History: 
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
 * >> (29 Jul 2010) Matthew Cotter: Corrected bug in which and unhandled exception was erroneously reported by RunSequence.
 * >> ( 5 Jul 2010) Matthew Cotter: Modelled/copied file from corresponding source for mapping tool.
 *                                    Changed mapping commands to synthesis commands.
 * >> ( 5 Jul 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using FalconGlobal;
using FalconPathManager;
using FalconCoreServerCompiler;
using FalconCommandLineParser;

namespace FalconCoreServerCompilerTool
{    
    class Program
    {
        private class ToolWrapperClass
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

            public enum ToolCommandEnum : int
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

                Cerebrum_InputFiles
            }

            private static FalconCommandParser parser;
            private object _WrappedObject;

            public ToolWrapperClass(object LibWrapObj)
            {
                _WrappedObject = LibWrapObj;
                parser = new FalconCommandParser();
                PopulateCommandTable();
            }
            private class ToolCommand
            {
                private ToolCommandEnum _cmd;
                private ArrayList _args;
                private int _reqargs;
                private string _cmdstring;

                public ToolCommand(ToolCommandEnum Cmd, int ReqArgs, string CommandString)
                {
                    _cmd = Cmd;
                    _args = new ArrayList();
                    _reqargs = ReqArgs;
                    _cmdstring = CommandString;
                }

                public ToolCommandEnum Command
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
            }
            private ToolCommand CreateFromString(string cmd)
            {
                ToolCommand tc = null;
                cmd = cmd.ToLower().Trim('-');

                // The command table is first checked to ensure that the command string exists
                // If it does not exist, the command is interpreted as InvalidCommand, or a NOP.
                // When it's executed, an error will be displayed and if executing in batch-mode, execution will terminate.
                // If running in interactive mode, the state will be that of the system IMMEDIATELY before the invalid command.
                if (CommandTable.ContainsKey(cmd))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[cmd];
                    tc = new ToolCommand(ci.Command, ci.MinArguments, cmd);
                }
                else
                {
                    tc = new ToolCommand(ToolCommandEnum.InvalidCommand, 0, cmd);
                }
                return tc;
            }

            private class CommandInfo
            {
                private int HELPLINE_LENGTH = 70;
                public ToolCommandEnum Command;
                public string Literal;
                public string Help;
                public int MinArguments;

                public CommandInfo(ToolCommandEnum ID, string CmdLiteral, int RequiredArgs, string HelpText)
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
            private void AddCommand(string Literal, ToolCommandEnum ID, int ReqArgs, string HelpInfo)
            {
                CommandTable.Add(Literal, new CommandInfo(ID, Literal, ReqArgs, HelpInfo));
            }
            private void PopulateCommandTable()
            {
                CommandTable = new Hashtable();

                #region Built-in Commands
                AddCommand("help", ToolCommandEnum.Help, 0,
                    "\n      Displays the command help screen.");
                AddCommand("version", ToolCommandEnum.Version, 0,
                    "\n      Displays the current version of the synthesis tool.");
                AddCommand("batch", ToolCommandEnum.ScriptFile, 0,
                    "<file_path>\n      Executes a series of command as listed in the specified script file.  Must be the FIRST command in the sequence, and all other commands are ignored.  If it is not the first command, the script file is ignored.");

                AddCommand("console", ToolCommandEnum.InteractiveMode, 0,
                    "\n      Start the synthesis tool in interactive mode.");

                AddCommand("pwd", ToolCommandEnum.PrintWorkingDirectory, 0,
                    "\n      Displays the current working directory.");
                AddCommand("ls", ToolCommandEnum.ListDirectory, 0,
                    "\n      Displays the contents of the current directory.");
                AddCommand("cd", ToolCommandEnum.ChangeDirectory, 1,
                    "\n      Changes the current working directory.");
                #endregion

                AddCommand("cerebinput", ToolCommandEnum.Cerebrum_InputFiles, 3,
                    "<path_file> <server_file> <design_file>\n      Generates headers and compilers core applications and drivers using the address map generated by the Cerebrum tool flow.");
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

            private static void ChangeDirectory(ToolCommand sc)
            {
                if (sc.ArgCount > 0)
                {
                    StringBuilder path = new StringBuilder();
                    for (int a = 0; a < sc.ArgCount; a++)
                    {
                        path.Append(sc.GetArg(a) + " ");
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
            public bool RunSequence(LinkedList<FalconCommand> fCommands)
            {
                ToolCommand tc = null;
                try
                {
                    if (fCommands.Count == 0)
                        return true;
                    for (LinkedListNode<FalconCommand> node = fCommands.First; node != null; node = node.Next)
                    {
                        FalconCommand fc = node.Value;
                        tc = CreateFromString(fc.CommandSwitch);
                        if (tc != null)
                        {
                            for (LinkedListNode<string> argNode = fc.Arguments.First; argNode != null; argNode = argNode.Next)
                            {
                                tc.AddArg(argNode.Value);
                            }
                            if (tc.ArgCount < tc.RequiredArgs)
                            {
                                Console.WriteLine("Insufficient arguments to command: {0}.", tc.CommandString);
                                DisplayHelpInfo(tc.CommandString);
                                return false;
                            }
                            switch (tc.Command)
                            {
                                #region Built-in Commands
                                case ToolCommandEnum.InvalidCommand:
                                    DisplayHelpInfo();
                                    StandardWriteLine("Unrecognized command : {0}.\n", tc.CommandString);
                                    if (fCommands.Count > 1)
                                        StandardWriteLine("Batch execution halted.\n\n");
                                    return false;
                                case ToolCommandEnum.Help:
                                    if (tc.ArgCount > 0)
                                        DisplayHelpInfo(tc.GetArg(0));
                                    else
                                        DisplayHelpInfo();
                                    break;
                                case ToolCommandEnum.Version:
                                    DisplayVersionInfo();
                                    break;
                                case ToolCommandEnum.ScriptFile:
                                    break;
                                case ToolCommandEnum.InteractiveMode:
                                    break;
                                case ToolCommandEnum.PrintWorkingDirectory:
                                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                                    break;
                                case ToolCommandEnum.ListDirectory:
                                    ListDirectory();
                                    break;
                                case ToolCommandEnum.ChangeDirectory:
                                    ChangeDirectory(tc);
                                    break;
                                #endregion

                                #region Tool-Specific Commands
                                case ToolCommandEnum.Cerebrum_InputFiles:
                                    CoreServerCompiler CSC = (CoreServerCompiler)_WrappedObject;
                                    if (CSC.LoadPaths(tc.GetArg(0)))
                                    {
                                        CSC.LoadServersFile(tc.GetArg(1));
                                        CSC.LoadDesignFile(tc.GetArg(2));
                                        CSC.LoadAddressMap();
                                        if (!CSC.CompileCoreServers())
                                            return false;
                                    }
                                    break;

                                #endregion
                                default:
                                    DisplayHelpInfo(tc.CommandString);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (tc != null)
                        StandardWriteLine("An error occurred in the core server compiler:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An error occurred executing command '{0}'.", tc.CommandString));
                    else
                        StandardWriteLine("An error occurred in the core server compiler:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An unknown error occurred while processing commands."));
                    return false;
                }
                return true;
            }
            public void DisplayVersionInfo()
            {
                try
                {
                    if (_WrappedObject is IFalconLibrary)
                    {
                        IFalconLibrary IFalconDLL = (IFalconLibrary)_WrappedObject;
                        StandardWriteLine(IFalconDLL.GetFalconComponentVersion());
                    }
                    else
                    {
                        StandardWriteLine("{0} does not implement IFalconLibrary.", _WrappedObject.GetType().ToString());
                    }
                }
                catch (Exception ex)
                {
                    StandardWriteLine("An error occurred in the core server compiler:\n\t{0}\n\t{1}", ex.Message,
                        String.Format("Unable to poll component version information."));
                }
            }

            /// <summary>
            /// Writes the SynthesisTool command prompt to the StandardWriteLine
            /// </summary>
            public void Prompt()
            {
                StandardWrite("coreappmake> ");
            } 
            /// <summary>
            /// Writes the initial shell welcome to the System.Console.
            /// </summary>
            public void ShellWelcome()
            {
                Console.WriteLine(((CoreServerCompiler)(_WrappedObject)).GetFalconComponentVersion());
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

        private static ToolWrapperClass Tool;
        private static FalconCommandParser parser;
        private static bool bInteractiveMode = false;

        static int Main(string[] args)
        {
            Tool = new ToolWrapperClass(new CoreServerCompiler());
            parser = new FalconCommandParser();
            Tool.ShellWelcome();

            if (args.Length > 0)
            {
                string argline = "\"" + string.Join("\" \"", args) +  "\"";

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
                                success = success && Tool.RunSequence(llCommands);
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
                        success = success && Tool.RunSequence(llCommands);
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
            Tool.InteractiveWelcome();
            Tool.Prompt();
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
                Tool.Prompt();
                input = Console.ReadLine();
            }
        }
    }
}