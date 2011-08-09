/************************************************************************************************************
 * JProgrammerTool\Program.cs
 * Name: Matthew Cotter
 * Date: 14 Jun 2010 
 * Description: This program implements the command-line bash-style interface between the Cerebrum command shell and that
 * of the JProgrammer library.
 * Notes:
 *      Commands can be disabled by removing/commenting their corresponding AddCommand() calls in PopulateCommandTable().
 *          Removing a command in this manner leaves the functionality in tact, but removes the capability for the command to be recognized
 *          as it will not be present in the command table when checked.  Translation from FalconCommand to JProgCommand is done inside
 *          the loop in RunSequence().
 *      New Commands are implemented by: 1) Adding a value to the JProgCommandEnum enumeration.
 *                                       2) Adding a call to AddCommand() with the appropriate information in PopulateCommandTable()
 *                                       3) Add the code to the 'switch()' block to execute the new command in:
 *                                                  a) class JProgCommand.Execute() for JProgrammer system commands
 *                                                  b) class JProgrammerToolWrapperClass.RunSequence() for Built-in tool commands
 * History: 
 * >> (21 Oct 2010) Matthew Cotter: Moved MultiJProgrammer class to FalconJTAGProgrammer library.
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
 * >> (13 Aug 2010) Matthew Cotter: Updated code that handles loading of Platform files to use new hierarchical location and format of platforms 
 *                                    (paths.ProjectPlatform -> paths.Platforms\<Platform>\<Platform.xml> -> 
 *                                      paths.Platforms\<Platform>\<Board>\<Board>.xml -> paths.Platforms\<Platform>\<Board>\<fpga>\<fpga>.xml.
 *                                  Removed platform file from cerebinput command due to new hierarchical location and format of platforms.
 * >> ( 2 Aug 2010) Matthew Cotter: Added some support for programming multiple processors.
 *                                  Added code to read processor options from processors section of design, as it may be 
 *                                      specific to the processor, rather than the board.   Cable information is still read from the Programming section.
 * >> (21 Jul 2010) Matthew Cotter: Verified that XML-generated Documentation is current.
 * >> ( 6 Jul 2010) Matthew Cotter: Added some cleanup to remove stale files from previous runs in programming folder.
 *                                   Updated LoadPlatformFile(), ProcessPlatforms(), and ProcessBoard() to handle hierarchical format of system platform.
 * >> ( 5 Jul 2010) Matthew Cotter: Significant internal changes made to JProgrammer class in order to support correct identification and programming of target board.
 * >> ( 1 Jul 2010) Matthew Cotter: Updated Multi-JProgrammer class to correctly reset and set options per platform being programmed.
 * >> (30 Jun 2010) Matthew Cotter: Started work on Creating Multi-JProgrammer wrapper class for programming multiple boards in a single invocation.
 * >> (10 Jun 2010) Matthew Cotter: Updated error messages to invoke the help command as well.
 * >> ( 1 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using FalconCommandLineParser;
using FalconJTAG_Programmer;
using FalconGlobal;
using System.Xml;
using System.Diagnostics;
using FalconPathManager;


namespace JProgrammerTool
{
    class Program
    {
        private class JProgrammerToolWrapperClass
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
            public static string StandardRead(bool echo)
            {
                char ch = Console.ReadKey(!echo).KeyChar;
                return ch.ToString();
            }
            public static string StandardReadLine()
            {
                return Console.ReadLine();
            }

            public static string ReadPassword(string user, string server)
            {
                StandardWrite("{0}@{1}'s password: ", user, server);
                string input = string.Empty;
                string rd = StandardRead(false);
                while ((rd != "\n") && (rd != "\r"))
                {
                    if (rd == "\b")
                    {
                        if (input.Length > 0)
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.CursorLeft = Console.CursorLeft - 1;
                            Console.Write(" ");
                            Console.CursorLeft = Console.CursorLeft - 1;
                        }
                    }
                    else
                    {
                        input += rd;
                        StandardWrite("*");
                    }
                    rd = StandardRead(false);
                }
                input = input.Replace("\r", string.Empty);
                input = input.Replace("\n", string.Empty);
                StandardWriteLine();
                return input;
            }

            public enum JProgCommandEnum : int
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

                //ClearOptions,

                ////RunImpact,
                ////RunXMD,

                //CleanImpactCable,
                //CleanXMDCable,
                //ClearCMDFile,
                //ClearTCLFile,

                //GenerateCMDFile,
                //GenerateTCLFile,
                //DownloadBITFile,
                //DownloadELFFile,

                //SetLocalDir,
                //SetRemoteDir,
                //CopyDirFrom,
                //CopyDirTo,
                //CopyFileFrom,
                //CopyFileTo,
                //Property_Overwrite,
                //Property_CreateFolder,

                //Property_BITFile,
                //Property_CMDFile,
                //Property_ELFFile,
                //Property_TCLFile,

                //Property_ImpactCableOpts,
                //Property_ImpactModeOpts,
                //Property_ProcessorType,
                //Property_ProcessorOpts,
                //Property_JTAGNumber,
                
                //Property_IsLocal,
                //Property_LocalOS,
                //Property_RemoteOS,
                
                //Property_ServerName,
                //Property_UserName,
                //Property_UserPass          
            }

            public static bool IsBuiltInCommand(JProgCommandEnum jpce)
            {
                switch (jpce)
                {
                    case JProgCommandEnum.Help:
                        return true;
                    case JProgCommandEnum.Version:
                        return true;
                    case JProgCommandEnum.PrintWorkingDirectory:
                        return true;
                    case JProgCommandEnum.ListDirectory:
                        return true;
                    case JProgCommandEnum.ChangeDirectory:
                        return true;

                    default:
                        return false;
                }
            }

            private static MultiJProgrammer MJP;

            internal delegate void ExProcessorDelegate(Exception ex, string Info);
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

            public JProgrammerToolWrapperClass()
            {
                MJP = new MultiJProgrammer();
                PopulateCommandTable();
            }

            private class JProgCommand : IComparable
            {
                private JProgCommandEnum _cmd;
                private ArrayList _args;
                private int _reqargs;
                private string _cmdstring;

                public JProgCommand(JProgCommandEnum Cmd, int ReqArgs, string CommandString)
                {
                    _cmd = Cmd;
                    _args = new ArrayList();
                    _reqargs = ReqArgs;
                    _cmdstring = CommandString;
                }

                public JProgCommandEnum Command
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

                public int CompareTo(object other)
                {
                    // Return < 0 -> This "is less than" other
                    // Return = 0 -> This "is equal to" other
                    // Return > 0 -> This "is greater than" other

                    if (other.GetType() != typeof(JProgCommand))
                        return 0;

                    JProgCommand This = this;
                    JProgCommand Other = (JProgCommand)other;

                    if (This.Command == Other.Command)
                        return 0;

                    //Sort invalids to the front
                    if (This.Command == JProgCommandEnum.InvalidCommand)
                        return -1;

                    //Sort invalids to the front
                    if (Other.Command == JProgCommandEnum.InvalidCommand)
                        return 1;

                    string ThisCommand = This.Command.ToString();
                    string OtherCommand = Other.Command.ToString();
                    bool ThisIsProperty = (ThisCommand.StartsWith("Property"));
                    bool OtherIsProperty = (OtherCommand.StartsWith("Property"));
                    bool ThisIsBuiltIn = JProgrammerToolWrapperClass.IsBuiltInCommand(This.Command);
                    bool OtherIsBuiltIn = JProgrammerToolWrapperClass.IsBuiltInCommand(Other.Command);

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
            private JProgCommand CreateFromString(string cmd)
            {
                JProgCommand jpc = null;
                cmd = cmd.ToLower().Trim('-');

                // The command table is first checked to ensure that the command string exists
                // If it does not exist, the command is interpreted as InvalidCommand, or a NOP.
                // When it's executed, an error will be displayed and if executing in batch-mode, execution will terminate.
                // If running in interactive mode, the state will be that of the system IMMEDIATELY before the invalid command.
                if (CommandTable.ContainsKey(cmd))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[cmd];
                    jpc = new JProgCommand(ci.Command, ci.MinArguments, cmd);
                }
                else
                {
                    jpc = new JProgCommand(JProgCommandEnum.InvalidCommand, 0, cmd);
                }
                return jpc;
            }

            private class CommandInfo
            {
                private int HELPLINE_LENGTH = 70;
                public JProgCommandEnum Command;
                public string Literal;
                public string Help;
                public int MinArguments;

                public CommandInfo(JProgCommandEnum ID, string CmdLiteral, int RequiredArgs, string HelpText)
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
            private void AddCommand(string Literal, JProgCommandEnum ID, int ReqArgs, string HelpInfo)
            {
                CommandTable.Add(Literal, new CommandInfo(ID, Literal, ReqArgs, HelpInfo));
            }
            private void PopulateCommandTable()
            {
                CommandTable = new Hashtable();

                #region Built-in Commands
                AddCommand("help", JProgCommandEnum.Help, 0,
                    "\n      Displays the command help screen.");
                AddCommand("version", JProgCommandEnum.Version, 0,
                    "\n      Displays the current version of the mapping tool.");
                AddCommand("batch", JProgCommandEnum.ScriptFile, 0,
                    "<file_path>\n      Executes a series of command as listed in the specified script file.  Must be the FIRST command in the sequence, and all other commands are ignored.  If it is not the first command, the script file is ignored.");

                AddCommand("console", JProgCommandEnum.InteractiveMode, 0,
                    "\n      Start the map tool in interactive mode.");

                AddCommand("pwd", JProgCommandEnum.PrintWorkingDirectory, 0,
                    "\n      Displays the current working directory.");
                AddCommand("ls", JProgCommandEnum.ListDirectory, 0,
                    "\n      Displays the contents of the current directory.");
                AddCommand("cd", JProgCommandEnum.ChangeDirectory, 1,
                    "\n      Changes the current working directory.");
                #endregion

                AddCommand("cerebinput", JProgCommandEnum.Cerebrum_InputFiles, 3,
                    "<paths_file> <server_file> <design_file>\n      Begins programming of the FPGA platforms using the specified input files from cerebrum.");

                //AddCommand("reset", JProgCommandEnum.ClearOptions, 0,
                //    "\n      Clears all JTAG Programmer Options/Settings.");

                ////AddCommand("impact", JProgCommandEnum.RunImpact, 0,
                ////    "\n      Executes the Xilinx iMPACT Tool.");
                ////AddCommand("xmd", JProgCommandEnum.RunXMD, 0,
                ////    "\n      Executes the Xilinx XMD Tool.");

                //AddCommand("clearimpact", JProgCommandEnum.CleanImpactCable, 0,
                //    "\n      Adds a command to clear any locks on the iMPACT programming cable to the CMD file.");
                //AddCommand("clearxmd", JProgCommandEnum.CleanXMDCable, 0,
                //    "\n      Adds a command to clear any locks on the XMD programming cable to the INIT file");
                //AddCommand("clearcmd", JProgCommandEnum.ClearCMDFile, 0,
                //    "\n      Resets the contents of the CMD file to be created.");
                //AddCommand("cleartcl", JProgCommandEnum.ClearTCLFile, 0,
                //    "\n      Resets the contents of the TCL file to be created.");

                //AddCommand("gencmd", JProgCommandEnum.GenerateCMDFile, 0,
                //    "\n      Generates the specified CMD file with the prepared sequence of commands.");
                //AddCommand("gentcl", JProgCommandEnum.GenerateTCLFile, 0,
                //    "\n      Generates the specified TCL file with the prepared sequence of commands.");
                //AddCommand("dlbit", JProgCommandEnum.DownloadBITFile, 0,
                //    "\n      Downloads the specified BIT file to the FPGA on the pre-set JTAG number.");
                //AddCommand("dlelf", JProgCommandEnum.DownloadELFFile, 0,
                //    "\n      Downloads the specified ELF file to the FPGA on the pre-set JTAG number.");

                //AddCommand("sld", JProgCommandEnum.SetLocalDir, 1,
                //    "<local_path>\n      Sets the current local directory.");
                //AddCommand("srd", JProgCommandEnum.SetRemoteDir, 1,
                //    "<remote_path>\n      Sets the current remote directory.");
                //AddCommand("cpdfrom", JProgCommandEnum.CopyDirFrom, 0,
                //    "\n      Copies the current remote directory to the current local directory.");
                //AddCommand("cpdto", JProgCommandEnum.CopyDirTo, 0,
                //    "\n      Copies the current local directory to the current remote directory.");
                //AddCommand("cpffrom", JProgCommandEnum.CopyFileFrom, 1,
                //    "<remote_file>\n      Copies the specified file in the current remote directory to the current local directory.");
                //AddCommand("cpfto", JProgCommandEnum.CopyFileTo, 1,
                //    "<local_file>\n      Copies the specified file in the current local directory to the current remote directory.");
                //AddCommand("overwrite", JProgCommandEnum.Property_Overwrite, 1,
                //    "<0 = No/1 = Yes>\n      Indicates whether the destination file should be overwritten if it already exists during a copy.");
                //AddCommand("createfolder", JProgCommandEnum.Property_CreateFolder, 1,
                //    "<0 = No/1 = Yes>\n      Indicates whether the destination folder should be created if it does not already exist during a copy.");

                //AddCommand("bitfile", JProgCommandEnum.Property_BITFile, 1,
                //    "<bitfile_name>\n      Sets the name of the BIT file to be downloaded to the FPGA.");
                //AddCommand("cmdfile", JProgCommandEnum.Property_CMDFile, 1,
                //    "<cmdfile_name>\n      Sets the name of the CMD file to be executed in iMPACT.");
                //AddCommand("elffile", JProgCommandEnum.Property_ELFFile, 1,
                //    "<elffile_name>\n      Sets the name of the ELF file to be downloaded to the FPGA.");
                //AddCommand("tclfile", JProgCommandEnum.Property_TCLFile, 1,
                //    "<tclfile_name>\n      Sets the name of the TCL file to be executed in XMD.");

                //AddCommand("impactopts", JProgCommandEnum.Property_ImpactCableOpts, 1,
                //    "<impact_opts ...>\n      Sets any iMPACT options that should be used for programming.");
                //AddCommand("impactmode", JProgCommandEnum.Property_ImpactModeOpts, 1,
                //    "<impact_mode ...>\n      Sets the iMPACT mode that should be used for programming.");
                //AddCommand("proc", JProgCommandEnum.Property_ProcessorType, 1,
                //    "<PPC = PowerPC/MB = MicroBlaze>\n      Sets the type of processor that will be programmed on the FPGA.");
                //AddCommand("procopts", JProgCommandEnum.Property_ProcessorOpts, 1,
                //    "<proc_options>\n      Sets any processor-type specifid options for programming the FPGA.");
                //AddCommand("jtag", JProgCommandEnum.Property_JTAGNumber, 1,
                //    "<#>\n      Sets the JTAG number to be used for programming the FPGA.");

                //// Set by servername?
                //AddCommand("local", JProgCommandEnum.Property_IsLocal, 1,
                //    "<0 = No/1 = Yes>\n      Indicates whether the local machine is the programming server.");
                //// Not available, OS type can be determined at runtime (i.e. It MUST be Windows)
                ////AddCommand("localos", JProgCommandEnum.Property_LocalOS, 1,
                ////    "<Windows / Linux>\n      Defines the operating system running on the local machine.");
                //// Override if IsLocalProgrammer? (Remote must be same as local)
                //AddCommand("remoteos", JProgCommandEnum.Property_RemoteOS, 1,
                //    "<Win / Linux>\n      Defines the operating system running on the (remote) programming server.");

                //AddCommand("server", JProgCommandEnum.Property_ServerName, 1,
                //    "<server_address>\n      Sets the host address for accessing a remote programming server.");
                //AddCommand("user", JProgCommandEnum.Property_UserName, 1,
                //    "<user_name>\n      Sets the user name to be used for logging into a remote programming server.");
                ////AddCommand("pass", JProgCommandEnum.Property_UserPass, 0,
                ////    "<password>\n      Sets the password to be used for logging into a remote programming server.");
                
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

            private static void ChangeDirectory(JProgCommand jpc)
            {
                if (jpc.ArgCount > 0)
                {
                    StringBuilder path = new StringBuilder();
                    for (int a = 0; a < jpc.ArgCount; a++)
                    {
                        path.Append(jpc.GetArg(a) + " ");
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
            
            private static void JProgrammerExceptionProcessor(Exception ex, string Info)
            {
                JProgrammerToolWrapperClass.StandardWriteLine("An error occurred in the JTAG Programmer:\n\t{0}\n\t{1}", ex.Message, Info);
            }
                       
            private bool SortSequence(LinkedList<FalconCommand> FalconCommands, out LinkedList<JProgCommand> JPCommands, out string FailedCommand)
            {
                ArrayList cmdList = new ArrayList();
                JPCommands = new LinkedList<JProgCommand>();
                FailedCommand = string.Empty;

                // Create set of JProgCommands and place them into an ArrayList
                for (LinkedListNode<FalconCommand> fcNode = FalconCommands.First; fcNode != null; fcNode = fcNode.Next)
                {
                    FalconCommand fc = fcNode.Value;
                    FailedCommand = fc.CommandSwitch;
                    JProgCommand jpc = CreateFromString(fc.CommandSwitch);
                    if (jpc.Command != JProgCommandEnum.InvalidCommand)
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
                                //case JProgCommandEnum.ClearOptions:
                                //    break;

                                ////case JProgCommandEnum.RunImpact:
                                ////    break;
                                ////case JProgCommandEnum.RunXMD:
                                ////    break;

                                //case JProgCommandEnum.CleanImpactCable:
                                //    break;
                                //case JProgCommandEnum.CleanXMDCable:
                                //    break;
                                //case JProgCommandEnum.ClearCMDFile:
                                //    break;
                                //case JProgCommandEnum.ClearTCLFile:
                                //    break;

                                //case JProgCommandEnum.GenerateCMDFile:
                                //    break;
                                //case JProgCommandEnum.GenerateTCLFile:
                                //    break;
                                //case JProgCommandEnum.DownloadBITFile:
                                //    break;
                                //case JProgCommandEnum.DownloadELFFile:
                                //    break;

                                //case JProgCommandEnum.SetLocalDir:
                                //    // Cannot check this (current directory may change at runtime)
                                //    break;
                                //case JProgCommandEnum.SetRemoteDir:
                                //    // Cannot check this (remote system)
                                //    break;
                                //case JProgCommandEnum.CopyDirFrom:
                                //    break;
                                //case JProgCommandEnum.CopyDirTo:
                                //    break;
                                //case JProgCommandEnum.CopyFileFrom:
                                //    // Cannot check this (remote system)
                                //    break;
                                //case JProgCommandEnum.CopyFileTo:
                                //    // Cannot check this (current directory may change at runtime)
                                //    break;
                                //case JProgCommandEnum.Property_Overwrite:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        bool arg;
                                //        if (bool.TryParse(jpc.GetArg(0), out arg))
                                //            jpc.SetArg(0, arg.ToString());
                                //        else
                                //            jpc.SetArg(0, "False");
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;
                                //case JProgCommandEnum.Property_CreateFolder:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        bool arg;
                                //        if (bool.TryParse(jpc.GetArg(0), out arg))
                                //            jpc.SetArg(0, arg.ToString());
                                //        else
                                //            jpc.SetArg(0, "False");
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;

                                //case JProgCommandEnum.Property_BITFile:
                                //    break;
                                //case JProgCommandEnum.Property_CMDFile:
                                //    break;
                                //case JProgCommandEnum.Property_ELFFile:
                                //    break;
                                //case JProgCommandEnum.Property_TCLFile:
                                //    break;

                                //case JProgCommandEnum.Property_ImpactCableOpts:
                                //    break;
                                //case JProgCommandEnum.Property_ImpactModeOpts:
                                //    break;
                                //case JProgCommandEnum.Property_ProcessorType:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        string sArg = jpc.GetArg(0);
                                //        if (sArg.ToLower().Contains("ppc"))
                                //            jpc.SetArg(0, JProgrammer.ProcessorType.PPC.ToString());
                                //        else if (sArg.ToLower().Contains("mb"))
                                //            jpc.SetArg(0, JProgrammer.ProcessorType.MB.ToString());
                                //        else
                                //            jpc.SetArg(0, JProgrammer.ProcessorType.PPC.ToString());
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;
                                //case JProgCommandEnum.Property_ProcessorOpts:
                                //    break;
                                //case JProgCommandEnum.Property_JTAGNumber:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        int arg;
                                //        if (int.TryParse(jpc.GetArg(0), out arg))
                                //            jpc.SetArg(0, arg.ToString());
                                //        else
                                //            return false;
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;

                                //case JProgCommandEnum.Property_IsLocal:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        bool arg;
                                //        if (bool.TryParse(jpc.GetArg(0), out arg))
                                //            jpc.SetArg(0, arg.ToString());
                                //        else
                                //            jpc.SetArg(0, "False");
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;
                                //case JProgCommandEnum.Property_LocalOS:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        string sArg = jpc.GetArg(0);
                                //        if (sArg.ToLower().Contains("win"))
                                //            jpc.SetArg(0, JProgrammer.OSVersion.WINDOWS.ToString());
                                //        else if (sArg.ToLower().Contains("nix"))
                                //            jpc.SetArg(0, JProgrammer.OSVersion.LINUX.ToString());
                                //        else
                                //            jpc.SetArg(0, JProgrammer.OSVersion.WINDOWS.ToString());
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;
                                //case JProgCommandEnum.Property_RemoteOS:
                                //    if (jpc.ArgCount == 1)
                                //    {
                                //        string sArg = jpc.GetArg(0);
                                //        if (sArg.ToLower().Contains("win"))
                                //            jpc.SetArg(0, JProgrammer.OSVersion.WINDOWS.ToString());
                                //        else if (sArg.ToLower().Contains("nix"))
                                //            jpc.SetArg(0, JProgrammer.OSVersion.LINUX.ToString());
                                //        else
                                //            jpc.SetArg(0, JProgrammer.OSVersion.LINUX.ToString());
                                //    }
                                //    else
                                //    {
                                //        return false;
                                //    }
                                //    break;

                                //case JProgCommandEnum.Property_ServerName:
                                //    break;
                                //case JProgCommandEnum.Property_UserName:
                                //    break;
                                //case JProgCommandEnum.Property_UserPass:
                                //    if (!Interactive)
                                //    {
                                //        if (jpc.ArgCount == 1)
                                //        {
                                //            break;
                                //        }
                                //        else
                                //        {
                                //            StandardWriteLine("Password is required as an argument when not running in interactive mode.");
                                //            return false;
                                //        }
                                //    }
                                //    break;

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

                // Insertion Sort into LinkedList<JProgCommand>
                while (cmdList.Count > 0)
                {
                    JProgCommand thisJPC = (JProgCommand)cmdList[0];
                    if (JPCommands.Count == 0)
                    {
                        JPCommands.AddLast(thisJPC);
                    }
                    else
                    {
                        LinkedListNode<JProgCommand> jpNode;
                        bool bAdded = false;
                        for (jpNode = JPCommands.First; jpNode != null; jpNode = jpNode.Next)
                        {
                            int cmp = thisJPC.CompareTo(jpNode.Value);
                            if (cmp < 0)
                            {
                                // thisJPC should come before jpNode
                                JPCommands.AddBefore(jpNode, thisJPC);
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
                            if (jpNode == null)
                            {
                                JPCommands.AddLast(thisJPC);
                            }
                            else
                            {
                                JPCommands.AddAfter(jpNode, thisJPC);
                            }
                        }
                    }
                    cmdList.Remove(thisJPC);
                }
                FailedCommand = string.Empty;
                return true;
            }

            public bool RunSequence(LinkedList<FalconCommand> fCommands)
            {
                JProgCommand jpc = null;
                try
                {
                    if (fCommands.Count == 0)
                        return true;
                    LinkedList<JProgCommand> JPCommands;;

                    string failedCmd;
                    if (!SortSequence(fCommands, out JPCommands, out failedCmd))
                    {
                        DisplayHelpInfo(failedCmd);
                        StandardWriteLine("Error parsing command ({0}) in sequence.", failedCmd);
                    }
                    for (LinkedListNode<JProgCommand> node = JPCommands.First; node != null; node = node.Next)
                    {
                        jpc = node.Value;
                        if (jpc != null)
                        {
                            if (jpc.ArgCount < jpc.RequiredArgs)
                            {
                                return false;
                            }
                            switch (jpc.Command)
                            {
                                case JProgCommandEnum.InvalidCommand:
                                    StandardWriteLine("Unrecognized command : {0}.\n", jpc.CommandString);
                                    DisplayHelpInfo();
                                    if (fCommands.Count > 1)
                                        StandardWriteLine("Batch execution halted.\n\n");
                                    return false;
                                case JProgCommandEnum.Help:
                                    if (jpc.ArgCount > 0)
                                        DisplayHelpInfo(jpc.GetArg(0));
                                    else
                                        DisplayHelpInfo();
                                    break;
                                case JProgCommandEnum.Version:
                                    DisplayVersionInfo();
                                    break;
                                case JProgCommandEnum.ScriptFile:
                                    break;
                                case JProgCommandEnum.InteractiveMode:
                                    break;
                                case JProgCommandEnum.PrintWorkingDirectory:
                                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                                    break;
                                case JProgCommandEnum.ListDirectory:
                                    ListDirectory();
                                    break;
                                case JProgCommandEnum.ChangeDirectory:
                                    ChangeDirectory(jpc);
                                    break;

                                case JProgCommandEnum.Cerebrum_InputFiles:
                                    MJP.LoadPaths(jpc.GetArg(0));
                                    MJP.LoadServersFile(jpc.GetArg(1));
                                    MJP.LoadPlatformFile();
                                    MJP.LoadDesignFile(jpc.GetArg(2));
                                    if (!MJP.ProgramSystem())
                                        return false;
                                    break;

                                default:
                                    DisplayHelpInfo();
                                    return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (jpc != null)
                        StandardWriteLine("An error occurred in the JTAG programmer:\n\t{0}\n\t{1}", ex.Message,
                            String.Format("An error occurred executing command '{0}'.", jpc.CommandString));
                    else
                        StandardWriteLine("An error occurred in the JTAG programmer:\n\t{0}\n\t{1}", ex.Message,
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
                    JProgrammer JProg = new JProgrammer();
                    if (JProg is IFalconLibrary)
                    {
                        IFalconLibrary IFalconDLL = (IFalconLibrary)JProg;
                        StandardWriteLine(IFalconDLL.GetFalconComponentVersion());
                    }
                    else
                    {
                        StandardWriteLine("{0} does not implement IFalconLibrary.", JProg.GetType().ToString());
                    }
                }
                catch (Exception ex)
                {
                    StandardWriteLine("An error occurred in the JTAG programmer:\n\t{0}\n\t{1}", ex.Message,
                        String.Format("Unable to poll component version information."));
                }
            }

            /// <summary>
            /// Writes the JProgrammerTool command prompt to the StandardWriteLine
            /// </summary>
            public void Prompt()
            {
                StandardWrite("jprog> ");
            }

            /// <summary>
            /// Writes the initial shell welcome to the System.Console.
            /// </summary>
            public void ShellWelcome()
            {
                Console.WriteLine(new JProgrammer().GetFalconComponentVersion());
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

        private static JProgrammerToolWrapperClass JP;
        private static FalconCommandParser parser;
        private static bool bInteractiveMode = false;

        static int Main(string[] args)
        {
            JP = new JProgrammerToolWrapperClass();
            parser = new FalconCommandParser();
            JP.ShellWelcome();

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
                                success = success && JP.RunSequence(llCommands);
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
                        success = success && JP.RunSequence(llCommands);
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
            JP.Interactive = true;
            JP.InteractiveWelcome();
            JP.Prompt();
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
                JP.Prompt();
                input = Console.ReadLine();
            }
        }
    }
}
