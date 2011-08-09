/************************************************************************************************************
 * FalconPlatformSynthesisTool\Program.cs
 * Name: Matthew Cotter
 * Date: 25 Jun 2010 
 * Description: This program implements the command-line bash-style interface between the Cerebrum command shell and that
 * of the FalconPlatformSynthesis library.
 * Notes:
 *      Commands can be disabled by removing/commenting their corresponding AddCommand() calls in PopulateCommandTable().
 *          Removing a command in this manner leaves the functionality in tact, but removes the capability for the command to be recognized
 *          as it will not be present in the command table when checked.  Translation from FalconCommand to SynthesisCommand is done inside
 *          the loop in RunSequence().
 *      New Commands are implemented by: 1) Adding a value to the SynthCommandEnum enumeration.
 *                                       2) Adding a call to AddCommand() with the appropriate information in PopulateCommandTable()
 *                                       3) Add the code to the 'switch()' block to execute the new command in:
 *                                                  a) class SynthesisCommand.Execute() for Synthesis system commands
 *                                                  b) class SynthesisToolWrapperClass.RunSequence() for Built-in tool commands
 * History: 
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
 * >> (13 Aug 2010) Matthew Cotter: Removed platform file from cerebinput command due to new hierarchical location and format of platforms.
 * >> ( 2 Aug 2010) Matthew Cotter: Added support for 'clean', 'hwonly' and 'swonly' command flags to support forced-clean,
 *                                    hardware-only, and software-only modes for Synthesis.
 * >> (29 Jul 2010) Matthew Cotter: Corrected bug in which and unhandled exception was erroneously reported by RunSequence.
 * >> (21 Jul 2010) Matthew Cotter: Verified that XML-generated Documentation is current.
 * >> (25 Jun 2010) Matthew Cotter: Modelled/copied file from corresponding source for mapping tool.
 * >> (25 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconPlatformSynthesis;
using System.Reflection;
using System.IO;
using FalconCommandLineParser;
using FalconGlobal;

namespace FalconPlatformSynthesisTool
{
    class Program
    {
        private class ToolWrapperClass
        {
            private const int OUTPUT_ID_PADDING = 3;
            private const int OUTPUT_NAME_PADDING = 15;

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

                Reset,                                  //    No parameters

                Cerebrum_InputFiles,
                Property_ForceClean,
                Property_HW_ONLY,
                Property_SW_ONLY,
                //LoadPaths,
                //LoadServers,
                //LoadDesign,
                //LoadPlatform,
                //Synthesize
            }

            private static FalconSystemSynthesizer Synth;
            private static FalconCommandParser parser;

            public ToolWrapperClass()
            {
                Synth = new FalconSystemSynthesizer();
                parser = new FalconCommandParser();

                PopulateCommandTable();
            }

            private class ToolCommand : IComparable
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

                public bool Execute(FalconSystemSynthesizer SynthObj)
                {
                    if (_args.Count < _reqargs)
                    {
                        StandardWriteLine("\tERROR: Insufficient Arguments\n");
                        return false;
                    }
                    switch (_cmd)
                    {
                        //case SynthesisCommandEnum.LoadServers:
                        //    if (ArgCount > 0)
                        //        Synth.LoadSynthesisServers(GetArg(0));
                        //    break;
                        //case SynthesisCommandEnum.LoadDesign:
                        //    if (ArgCount > 0)
                        //        Synth.LoadDesignFile(GetArg(0));
                        //    break;
                        //case SynthesisCommandEnum.LoadPlatform:
                        //    if (ArgCount > 0)
                        //        Synth.LoadPlatformFile(GetArg(0));
                        //    break;
                        //case SynthesisCommandEnum.Synthesize:
                        //    if (Synth.Ready())
                        //    {
                        //        SynthesisErrorCodes code = Synth.SynthesizeDesign();
                        //        if (code != SynthesisErrorCodes.SYNTHESIS_OK)
                        //        {
                        //            StandardWriteLine("Synthesis failed with error code {0}", code.ToString());
                        //        }
                        //    }
                        //    else
                        //    {
                        //        StandardWriteLine("System/Design not ready for synthesis.  Verify all information has been loaded and is correct.");
                        //    }
                        //    break;

                        case ToolCommandEnum.Property_HW_ONLY:
                            Synth.HardwareOnly = true;
                            break;

                        case ToolCommandEnum.Property_SW_ONLY:
                            Synth.SoftwareOnly = true;
                            break;

                        case ToolCommandEnum.Property_ForceClean:
                            Synth.ForceClean = true;
                            break;

                        case ToolCommandEnum.Cerebrum_InputFiles:
                            Synth.LoadPathsFile(GetArg(0));
                            Synth.LoadServersFile(GetArg(1));
                            Synth.LoadPlatformFile();
                            Synth.LoadDesignFile(GetArg(2));
                            Synth.LoadCommsFile(GetArg(3));
                            if (Synth.Ready())
                            {
                                SynthesisErrorCodes code = Synth.SynthesizeDesign();
                                if ((code != SynthesisErrorCodes.SYNTHESIS_OK) && (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED))
                                {
                                    StandardWriteLine("Synthesis failed with error code {0}", code.ToString());
                                    return false;
                                }
                                else
                                {
                                    StandardWriteLine("Synthesis Complete!");
                                }
                            }
                            else
                            {
                                StandardWriteLine("System/Design not ready for synthesis.  Verify all information has been loaded and is correct.");
                                return false;
                            }
                            break;
                        // Break out on built-in commands
                        // Break out on enumeration commands
                        case ToolCommandEnum.InvalidCommand:
                            return false;
                        default:
                            StandardWriteLine("Unimplemented command: {0}.", _cmdstring);
                            return false;
                    }
                    return true;
                }

                public int CompareTo(object other)
                {
                    // Return < 0 -> This "is less than" other
                    // Return = 0 -> This "is equal to" other
                    // Return > 0 -> This "is greater than" other

                    if (other.GetType() != typeof(ToolCommand))
                        return 0;

                    ToolCommand This = this;
                    ToolCommand Other = (ToolCommand)other;

                    if (This.Command == Other.Command)
                        return 0;

                    //Sort invalids to the front
                    if (This.Command == ToolCommandEnum.InvalidCommand)
                        return -1;

                    //Sort invalids to the front
                    if (Other.Command == ToolCommandEnum.InvalidCommand)
                        return 1;

                    string ThisCommand = This.Command.ToString();
                    string OtherCommand = Other.Command.ToString();
                    bool ThisIsProperty = (ThisCommand.StartsWith("Property"));
                    bool OtherIsProperty = (OtherCommand.StartsWith("Property"));
                    bool ThisIsBuiltIn = ToolWrapperClass.IsBuiltInCommand(This.Command);
                    bool OtherIsBuiltIn = ToolWrapperClass.IsBuiltInCommand(Other.Command);

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
            private ToolCommand CreateFromString(string cmd)
            {
                ToolCommand sc = null;
                cmd = cmd.ToLower().Trim('-');

                // The command table is first checked to ensure that the command string exists
                // If it does not exist, the command is interpreted as InvalidCommand, or a NOP.
                // When it's executed, an error will be displayed and if executing in batch-mode, execution will terminate.
                // If running in interactive mode, the state will be that of the system IMMEDIATELY before the invalid command.
                if (CommandTable.ContainsKey(cmd))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[cmd];
                    sc = new ToolCommand(ci.Command, ci.MinArguments, cmd);
                }
                else
                {
                    sc = new ToolCommand(ToolCommandEnum.InvalidCommand, 0, cmd);
                }
                return sc;
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

                AddCommand("reset", ToolCommandEnum.Reset, 0,
                    "\n      Resets ALL information in the synthesis system, clearing all loaded objects and their state.");
                
                AddCommand("clean", ToolCommandEnum.Property_ForceClean, 0,
                    "\n      Forces the synthesis tool to clean any previous work before beginning synthesis for each project.");

                AddCommand("hwonly", ToolCommandEnum.Property_HW_ONLY, 0,
                    "\n      Causes the synthesis tool to synthesize hardware only.   If specified with the --swonly flag, both hardware and software are processed as normal.");

                AddCommand("swonly", ToolCommandEnum.Property_SW_ONLY, 0,
                    "\n      Causes the synthesis tool to compile software only.   If specified with the --hwonly flag, both hardware and software are processed as normal.");

                AddCommand("cerebinput", ToolCommandEnum.Cerebrum_InputFiles, 4,
                    "<path_file> <server_file> <design_file> <comms_file>\n      Begins hardware/software synthesis and compilation using specified input files from cerebrum.");

                //AddCommand("paths", SynthesisCommandEnum.LoadPaths, 1,
                //    "<path_file>\n      Loads the list of servers and their corresponding connection information.");
                //AddCommand("servers", SynthesisCommandEnum.LoadServers, 1,
                //    "<server_file>\n      Loads the list of servers and their corresponding connection information.");
                //AddCommand("design", SynthesisCommandEnum.LoadDesign, 1,
                //    "<design_file>\n      Loads the design specification for software applications to be run on the system.");
                //AddCommand("platform", SynthesisCommandEnum.LoadPlatform, 1,
                //    "<platform_file>\n      Loads the platform specification for hardware to be synthesized on the system.");
                //AddCommand("synth", SynthesisCommandEnum.Synthesize, 0,
                //    "\n      Performs synthesis and compilation on all platforms and designs loaded into the system.");
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

            private static void SynthesisExceptionProcessor(Exception ex, string Info)
            {
                ToolWrapperClass.StandardWriteLine("An error occurred in the synthesis system:\n\t{0}\n\t{1}", ex.Message, Info);
            }

            public static bool IsBuiltInCommand(ToolCommandEnum sce)
            {
                switch (sce)
                {
                    case ToolCommandEnum.Help:
                        return true;
                    case ToolCommandEnum.Version:
                        return true;
                    case ToolCommandEnum.PrintWorkingDirectory:
                        return true;
                    case ToolCommandEnum.ListDirectory:
                        return true;
                    case ToolCommandEnum.ChangeDirectory:
                        return true;

                    default:
                        return false;
                }
            }

            private bool SortSequence(LinkedList<FalconCommand> FalconCommands, out LinkedList<ToolCommand> SynthCommands, out string FailedCommand)
            {
                ArrayList cmdList = new ArrayList();
                SynthCommands = new LinkedList<ToolCommand>();
                FailedCommand = string.Empty;

                // Create set of JProgCommands and place them into an ArrayList
                for (LinkedListNode<FalconCommand> fcNode = FalconCommands.First; fcNode != null; fcNode = fcNode.Next)
                {
                    FalconCommand fc = fcNode.Value;
                    FailedCommand = fc.CommandSwitch;
                    ToolCommand sc = CreateFromString(fc.CommandSwitch);
                    if (sc.Command != ToolCommandEnum.InvalidCommand)
                    {
                        for (LinkedListNode<string> node = fc.Arguments.First; node != null; node = node.Next)
                        {
                            sc.AddArg(node.Value);
                        }
                        cmdList.Add(sc);
                        FailedCommand = sc.CommandString;
                        if (sc.RequiredArgs > sc.ArgCount)
                        {
                            return false;
                        }
                        else
                        {
                            switch (sc.Command)
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
                    ToolCommand thisSC = (ToolCommand)cmdList[0];
                    if (SynthCommands.Count == 0)
                    {
                        SynthCommands.AddLast(thisSC);
                    }
                    else
                    {
                        LinkedListNode<ToolCommand> scNode;
                        bool bAdded = false;
                        for (scNode = SynthCommands.First; scNode != null; scNode = scNode.Next)
                        {
                            int cmp = thisSC.CompareTo(scNode.Value);
                            if (cmp < 0)
                            {
                                // thisJPC should come before jpNode
                                SynthCommands.AddBefore(scNode, thisSC);
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
                            if (scNode == null)
                            {
                                SynthCommands.AddLast(thisSC);
                            }
                            else
                            {
                                SynthCommands.AddAfter(scNode, thisSC);
                            }
                        }
                    }
                    cmdList.Remove(thisSC);
                }
                FailedCommand = string.Empty;
                return true;
            }

            public bool RunSequence(LinkedList<FalconCommand> fCommands)
            {
                ToolCommand sc = null;
                try
                {
                    if (fCommands.Count == 0)
                        return true;
                    LinkedList<ToolCommand> SynthCommands;

                    string failedCmd;
                    if (!SortSequence(fCommands, out SynthCommands, out failedCmd))
                    {
                        DisplayHelpInfo(failedCmd);
                        StandardWriteLine("Error parsing command ({0}) in sequence.", failedCmd);
                    }

                    for (LinkedListNode<ToolCommand> node = SynthCommands.First; node != null; node = node.Next)
                    {
                        sc = node.Value;
                        switch (sc.Command)
                        {
                            case ToolCommandEnum.InvalidCommand:
                                DisplayHelpInfo();
                                StandardWriteLine("Unrecognized command : {0}.\n", sc.CommandString);
                                if (fCommands.Count > 1)
                                    StandardWriteLine("Batch execution halted.\n\n");
                                return false;
                            case ToolCommandEnum.Help:
                                if (sc.ArgCount > 0)
                                    DisplayHelpInfo(sc.GetArg(0));
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
                                ChangeDirectory(sc);
                                break;
                            case ToolCommandEnum.Reset:
                                Synth.Reset();
                                StandardWriteLine("ALL loaded synthesis information has been cleared.");
                                break;


                            default:
                                if (!sc.Execute(Synth))
                                {
                                    DisplayHelpInfo(sc.CommandString);
                                    return false;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (sc != null)
                        StandardWriteLine("An error occurred in the system synthesizer:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An error occurred executing command '{0}'.", sc.CommandString));
                    else
                        StandardWriteLine("An error occurred in the system synthesizer:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An unknown error occurred while processing commands."));
                    return false;
                }
                return true;
            }
            public void DisplayVersionInfo()
            {
                try
                {
                    if (Synth is IFalconLibrary)
                    {
                        IFalconLibrary IFalconDLL = (IFalconLibrary)Synth;
                        StandardWriteLine(IFalconDLL.GetFalconComponentVersion());
                    }
                    else
                    {
                        StandardWriteLine("{0} does not implement IFalconLibrary.", Synth.GetType().ToString());
                    }
                }
                catch (Exception ex)
                {
                    StandardWriteLine("An error occurred in the system synthesizer:\n\t{0}\n\t{1}", ex.Message,
                        String.Format("Unable to poll component version information."));
                }
            }


            /// <summary>
            /// Writes the SynthesisTool command prompt to the StandardWriteLine
            /// </summary>
            public void Prompt()
            {
                StandardWrite("synth> ");
            }

            /// <summary>
            /// Writes the initial shell welcome to the System.Console.
            /// </summary>
            public void ShellWelcome()
            {
                Console.WriteLine(((FalconSystemSynthesizer)(Synth)).GetFalconComponentVersion());
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
            Tool = new ToolWrapperClass();
            parser = new FalconCommandParser();
            Tool.ShellWelcome();

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
