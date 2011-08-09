/*********************************************************************************************************** 
 * CerebrumCommandShell.cs
 * Name: Matthew Cotter
 * Date: 4 Jun 2010 
 * Description: Class library to implement core functionality of the Cerebrum bash command shell.
 * Notes:
 *     Cerebrum shell implements the following built-in commands:
 *     help                     - Displays the help screen; Lists built-in commands as well as programs available in the "bin" directory.
 *     help <program>           - If <program> is specified, the shell attempts to invoke the help for that program in the "bin" directory.
 *     help <program> <command> - If <program> and <command> are specified, the shell attempts to invoke the help for that command in the specified program.
 * **  batch <script_file>      - Attempts to execute a series of commands listed in <script_file>
 *     version                  - Displays the version information of the Cerebrum shell library
 *     version <program>        - If <program> is specified, the shell attempts to invoke the version for that program in the "bin" directory.
 *     pwd                      - Displays the current directory
 *     ls <filter>              - Lists the contents of the current directory.
 *     cd <dir>                 - Changes the current directory
 *     <program> [args...]      - Attempts to execute <program> in the "bin" directory with the specified arguments.
 * History: 
 * >> (22 Aug 2010) Matthew Cotter: Corrected a bug that prevented subsequent commands from executing after pressing Ctrl+C to terminate a batch script.
 * >> (20 Aug 2010) Matthew Cotter: Added code to correctly display 'batch' command in main Cerebrum Help Screen.
 *                                  Added code to interpret strings starting with # (COMMENT_STRING) as comments, and ignores them.
 *                                  Corrected bug that prevented sub-tool help screens from being displayed in the Cerebrum shell.
 * >> (29 Jun 2010) Matthew Cotter: Added built-in commands for 'ls', 'pwd', and 'cd'.  
 *                                  Added support for a 'shell-to-DOS' prefix, "!", that specifies that the commands arguments following it be routed to the 
 *                                    DOS command line rather than the Cerebrum Shell.   
 * >> (11 Jun 2010) Matthew Cotter: Moved code from executable into library in order to simplify executable implementation.
 * >> (10 Jun 2010) Matthew Cotter: Corrected batch file processing and execution.
 * >> ( 8 Jun 2010) Matthew Cotter: Corrected bug in parsing escaped-multiline commands sequences.
 * >> ( 7 Jun 2010) Matthew Cotter: Modified sub-tool invocations to use double-hyphens for help argument.
 * >> ( 4 Jun 2010) Matthew Cotter: Implemented basic commands help, version, and batch.   As well as bin-dir search for unrecognized executables.
 *                                    Arguments to executables are packaged and passed as command-line parameters to the process that starts the tool.
 *                                    Redirect console output from spawned tools to local Console by reading the console stdout while its executing.
 *                                    Added support for using Ctrl+C to early-terminate a long-running or otherwise unwieldly command/sequence.
 * >> ( 4 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace CerebrumCommandShell
{
    /// <summary>
    /// Library class to parse and interpret Cerebrum Command Shell commands.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/Guideline_command_line_environment.pdf">
    /// Cerebrum Shell Documentation</seealso>
    public class CerebrumCommandShell : FalconGlobal.IFalconLibrary
    {
        private const string COMMENT_STRING = "#";
        private string tool_command_delimiter = "--";
        private string bin_path;
        private string cereb_exec_bin;
        private string work_path;
        private bool bTerminateBatch = false;
        private bool bExecuting = false;

        /// <summary>
        ///  Get or set whether the command shell is running in interactive mode.
        /// </summary>
        public bool InteractiveMode { get; set; }

        /// <summary>
        /// Public constructor to create a new CerebrumCommandShell.   This constructor initializes both the "bin" path and 
        /// the tool working directory to the directory that Cerebrum Command shell is executing in.
        /// </summary>
        public CerebrumCommandShell()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            string exec_dir = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            cereb_exec_bin = fi.Name.Replace(fi.Extension, string.Empty);
            if (!exec_dir.EndsWith("\\"))
                exec_dir = exec_dir + "\\";
            bin_path = exec_dir;
            work_path = exec_dir;
            //Directory.SetCurrentDirectory(bin_path);

        }

        /// <summary>
        /// Public constructor to create a new CerebrumCommandShell.   This constructor initializes the "bin" path  
        /// and the tool working directory to the directories specified.  Passing either argument as string.Empty or null
        /// uses the default of the directory that Cerebrum Command shell is executing in for that parameter.
        /// </summary>
        public CerebrumCommandShell(string BinDirectory, string WorkingDirectory)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            string exec_dir = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            cereb_exec_bin = fi.Name.Replace(fi.Extension, string.Empty);
            if (!exec_dir.EndsWith("\\"))
                exec_dir = exec_dir + "\\";
            bin_path = exec_dir;
            work_path = exec_dir;

            if (BinDirectory != null)
                if (BinDirectory != string.Empty)
                    bin_path = BinDirectory;

            if (WorkingDirectory != null)
                if (WorkingDirectory != string.Empty)
                    work_path = WorkingDirectory;
            Directory.SetCurrentDirectory(bin_path);
        }


        /// <summary>
        /// Intercepts the Ctrl+C key combination at the console to terminate a batch executing without terminating the command shell.
        /// </summary>
        /// <param name="sender">The console object that has intercepted the Ctrl+C.</param>
        /// <param name="e">ConsoleCancelEventArgs object used to prevent the Ctrl+C from terminating the application.</param>
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            this.TerminateBatch = true;
        }


        /// <summary>
        /// Gets or sets the "bin" directory.  A trailing backslash is appended if not specified for set, and always returned on get.
        /// </summary>
        public string BinaryDirectory
        {
            get
            {
                return bin_path;
            }
            set
            {
                bin_path = value;
                if (!bin_path.EndsWith("\\"))
                    bin_path = bin_path + "\\";
            }
        }

        /// <summary>
        /// Gets or sets the working directory.  A trailing backslash is appended if not specified for set, and always returned on get.
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                return work_path;
            }
            set
            {
                work_path = value;
                if (!work_path.EndsWith("\\"))
                    work_path = work_path + "\\";
            }
        }

        /// <summary>
        /// Get (public) or set (private) the flag to terminate a batch executing of program commands.
        /// </summary>
        public bool TerminateBatch
        {
            get
            {
                return bTerminateBatch;
            }
            set
            {
                bTerminateBatch = value;
            }
        }

        /// <summary>
        /// Get (public) or set (private) the flag indicating whether a batch of program commands is executing.
        /// </summary>
        public bool Executing
        {
            get
            {
                return bExecuting;
            }
            private set
            {
                bExecuting = value;
            }
        }

        /// <summary>
        /// Writes the Cerebrum Shell command prompt to the System.Console.
        /// </summary>
        public void Prompt()
        {
            Console.Write("cereb> ");
        }

        /// <summary>
        /// Writes the initial shell welcome to the System.Console.
        /// </summary>
        public void ShellWelcome()
        {
            Console.WriteLine(GetFalconComponentVersion());
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

        /// <summary>
        /// Determines whether the specified string is a comment
        /// </summary>
        /// <param name="InputString">The raw string to be tested.</param>
        /// <returns>True if the string is a Comment, False otherwise.</returns>
        public bool IsCommentCommand(string InputString)
        {
            return (InputString.StartsWith(COMMENT_STRING));
        }

        /// <summary>
        /// Attempts to execute a string as a Cerebrum Shell command, or series of commands, or batch file processing command.
        /// </summary>
        /// <param name="InputString">The raw string read in from input to be processed.</param>
        /// <returns>True if all commands were valid and executed, False if an error occurred or any command was terminated.</returns>
        public bool ExecuteString(string InputString)
        {
            if (IsCommentCommand(InputString))
                return true;

            InputString = InputString.Trim();
            InputString = InputString.Replace(" \\\r\n", " ");          // Convert Properly-Escaped Windows NewLine to space
            InputString = InputString.Replace(" \\\n", " ");            // Convert Properly-Escaped Unix/Linux NewLine to space
            if (InputString.StartsWith("--batch"))
            {
                string scriptfile = InputString.Substring("--batch".Length).Trim();
                InputString = scriptfile;
                return ExecuteFile(InputString);
            }
            else if (InputString.StartsWith("batch"))
            {
                string scriptfile = InputString.Substring("batch".Length).Trim();
                InputString = scriptfile;
                return ExecuteFile(InputString);
            }
            else
            {
                if (ValidateCommand(InputString))
                {
                    return ExecuteCommand(InputString);
                }
            }
            return false;
        }

        private void ChangeDirectory(string NewDirectory)
        {
            if (System.IO.Directory.Exists(NewDirectory))
            {
                System.IO.Directory.SetCurrentDirectory(NewDirectory);
                Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            }
            else
            {
                Console.WriteLine("Unable to change directory: {0} was not found", NewDirectory);
            }
        }
        private void ListDirectory(string filter)
        {
            if (filter == string.Empty)
                filter = "*.*";

            string[] dirs = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory());
            string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), filter);
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
            Console.WriteLine(listing.ToString());
        }

        /// <summary>
        /// Attempts to execute a single Cerebrum Shell command.
        /// </summary>
        /// <param name="Command">The Cerebrum Shell command to execute.</param>
        /// <returns>True if the command was executed, False if an error occured or the command was terminated.</returns>
        public bool ExecuteCommand(string Command)
        {
            if (Command.Trim() == string.Empty)
                return true;

            if (IsCommentCommand(Command))
                return true;
            TextReader ConsoleIn = Console.In;
            TextWriter ConsoleOut = Console.Out;

            bool bSuccess = true;
            try
            {
                if (!ValidateCommand(Command))
                    return false;
                this.TerminateBatch = false;
                this.Executing = true;
                string cmd = string.Empty;
                string args = string.Empty;

                // Get the command from the line
                int spaceIndex = Command.IndexOf(" ");
                if (spaceIndex == -1)
                {
                    // Single-word line
                    cmd = Command;
                    args = string.Empty;
                }
                else
                {
                    // Multi-word line
                    cmd = Command.Substring(0, spaceIndex);
                    args = Command.Substring(spaceIndex + 1);
                }

                string program = string.Empty;
                string subcmd = string.Empty;

                // Assuming the command is help or version, get the program name
                if (args != string.Empty)
                {
                    spaceIndex = args.IndexOf(" ");
                    if (spaceIndex == -1)
                    {
                        // Single-word arg
                        program = args;
                    }
                    else
                    {
                        // Multi-word arg, get the first
                        program = args.Substring(0, spaceIndex);
                        subcmd = args.Substring(spaceIndex + 1);
                    }
                }
                System.Diagnostics.Process p = null;

                if ((cmd == "help") ||          // Built-in Help
                    (cmd == "version") ||       // Built-in Version
                    (cmd == cereb_exec_bin) ||  // Self-reference -> Invokes help
                    (cmd.StartsWith("!")) ||    // Shell-to-DOS Prefix
                    (cmd == "ls") ||            // Built-in Directory List
                    (cmd == "pwd") ||           // Built-in Print Working Directory
                    (cmd == "cd")               // Built-in Change Directory
                   )
                {

                    if (((cmd == "help") && (program == string.Empty)) || (cmd == cereb_exec_bin))
                    {
                        DisplayHelpScreen();
                    }
                    else if (cmd == "version")
                    {
                        Console.WriteLine(GetFalconComponentVersion());
                    }
                    else if (cmd == "cd")
                    {
                        ChangeDirectory(args);
                    }
                    else if (cmd == "pwd")
                    {
                        Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
                    }
                    else if (cmd == "ls")
                    {
                        ListDirectory(args);
                    }
                    else if (cmd.StartsWith("!"))
                    {
                        program = cmd.Substring(1);
                        p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "cmd";
                        p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                        p.StartInfo.Arguments = "/c " + program + " " + args;
                    }
                    else if (program.Length > 0)
                    {
                        if (CheckBinExists(program))
                        {
                            // Start program with help args
                            p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = program + ".exe"; // bin_path + program + ".exe";
                            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                            p.StartInfo.Arguments = tool_command_delimiter + "help " + subcmd;
                        }
                        else
                        {
                            bSuccess = false;
                            Console.WriteLine("Unable to get help on specified command: {0}.", program);
                            DisplayHelpScreen();
                        }
                    }
                }
                else
                {
                    if (CheckBinExists(cmd))
                    {
                        // Start program with specified args
                        p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = cmd + ".exe";  //bin_path + cmd + ".exe";
                        p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                        args = args.Trim();
                        if (args.Length == 0)
                            args = tool_command_delimiter + "help";

                        p.StartInfo.Arguments = args;
                    }
                    else
                    {
                        bSuccess = false;
                        Console.WriteLine("Unable to execute specified command: {0}.", cmd);
                        DisplayHelpScreen();
                    }
                }
                if (p != null)
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = false;
                    p.StartInfo.RedirectStandardInput = false;
                    p.StartInfo.CreateNoWindow = false;
                    Console.WriteLine("\n\nRunning {0}...", cmd);
                    p.Start();

                    p.WaitForExit(10);
                    while (!p.HasExited)
                    {
                        if (this.TerminateBatch)
                        {
                            bSuccess = false;
                            p.Kill();
                        }
                        try
                        {
                            while ((!p.HasExited) && (!p.StandardOutput.EndOfStream))
                            {
                                int iChar = p.StandardOutput.Read();
                                char cChar = Convert.ToChar(iChar);
                                Console.Write(cChar);
                            }
                        }
                        catch
                        { }

                        try
                        {
                            while ((!p.HasExited) && (Console.KeyAvailable))
                            {
                                int iChar = Console.Read();
                                char cChar = Convert.ToChar(iChar);
                                p.StandardInput.Write(cChar);
                            }
                        }
                        catch
                        { }
                        if ((!p.HasExited)) p.WaitForExit(10);
                    }
                    if (p.ExitCode != 0)
                    {
                        Console.WriteLine("Program returned error code.");
                        this.TerminateBatch = true;
                    }
                    Console.SetIn(ConsoleIn);
                    Console.SetOut(ConsoleOut);
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                this.Executing = false;
                bSuccess = false;
                Console.WriteLine("An unexpected exception was thrown. \n{0}", ex.ToString());
                DisplayHelpScreen();
            }
            finally
            {
                Console.SetIn(ConsoleIn);
                Console.SetOut(ConsoleOut);
            }

            this.Executing = false;
            return bSuccess;
        }

        private void DisplayHelpScreen()
        {
            Console.WriteLine("Cerebrum Command-Line Help");
            Console.WriteLine("-----------------------");
            Console.WriteLine("{0}{1}", "help".PadRight(20, ' '), "Displays this help screen");
            Console.WriteLine("{0}{1}", "help <prog>".PadRight(20, ' '), "Calls the specified program's help.");
            Console.WriteLine("{0}{1}", "help <prog> <cmd>".PadRight(20, ' '), "Calls the specified program's help for the given command.");
            Console.WriteLine("{0}{1}", "".PadRight(20, ' '), "If supported by the tool.");
            Console.WriteLine("{0}{1}", "batch <script_file>".PadRight(20, ' '), "Executes the specified batch file containing a list");
            Console.WriteLine("{0}{1}", "".PadRight(20, ' '), "of Cerebrum Commands.");
            Console.WriteLine("{0}{1}", "version".PadRight(20, ' '), "Displays version information for this shell.");
            Console.WriteLine("{0}{1}", "pwd".PadRight(20, ' '), "Displays the current directory");
            Console.WriteLine("{0}{1}", "ls <filter>".PadRight(20, ' '), "Lists the contents of the current directory.");
            Console.WriteLine("{0}{1}", "cd <dir>".PadRight(20, ' '), "Changes the current directory.");
            Console.WriteLine("{0}{1}", "!<cmd> <args>".PadRight(20, ' '), "Passes the specified command and arguments to the");
            Console.WriteLine("{0}{1}", "".PadRight(20, ' '), "Windows Command Interpreter.");
            Console.WriteLine();
            Console.WriteLine("Tools in 'bin' directory:");
            Console.WriteLine("-----------------------");
            string[] Binaries = Directory.GetFiles(bin_path, "*.exe", SearchOption.TopDirectoryOnly);
            ArrayList bins = new ArrayList();
            bins.AddRange(Binaries);
            bins.Sort();
            for (int f = 0; f < bins.Count; f++)
            {
                FileInfo fi = new FileInfo(bins[f].ToString());
                string pcmd = fi.Name.Replace(fi.Extension, string.Empty);
                if (pcmd != cereb_exec_bin)
                    Console.WriteLine("{0}", pcmd.PadRight(20, ' '));
            }
        }

        /// <summary>
        /// Attempts to validate a single Cerebrum Shell command.  
        /// </summary>
        /// <param name="Command">The Cerebrum Shell command to validate.</param>
        /// <returns>True if the command is valid, False otherwise.  A return value of true here does not guarantee that the command will success
        /// but rather only that it will execute.</returns>
        public bool ValidateCommand(string Command)
        {
            bool valid = true;
            if (Command.Trim() == string.Empty)
                return true;
            if (Command.StartsWith("!"))
                return true;
            if (IsCommentCommand(Command))
                return true;
            try
            {
                string cmd = string.Empty;
                string args = string.Empty;

                // Get the command from the line
                int spaceIndex = Command.IndexOf(" ");
                if (spaceIndex == -1)
                {
                    // Single-word line
                    cmd = Command;
                    args = string.Empty;
                }
                else 
                {
                    // Multi-word line
                    cmd = Command.Substring(0, spaceIndex);
                    args = Command.Substring(spaceIndex + 1);
                }
                    
                string program = string.Empty;
                // Assuming the command is help or version, get the program name
                if (args != string.Empty)
                {
                    spaceIndex = args.IndexOf(" ");
                    if (spaceIndex == -1)
                    {
                        // Single-word arg
                        program = args;
                    }
                    else
                    {
                        // Multi-word arg, get the first
                        program = args.Substring(0, spaceIndex);
                    }
                }

                if ((cmd == "ls") || (cmd == "pwd") || (cmd == "cd"))
                    return true;

                if ((cmd == "help") || (cmd == "version"))
                {
                    if (program.Length > 0)
                    {
                        valid = CheckBinExists(program);
                        if (!valid)
                        {
                            Console.WriteLine("Unrecognized command: {0}", cmd);
                            DisplayHelpScreen();
                        }
                    }
                }
                else
                {
                    valid = CheckBinExists(cmd);
                    if (!valid)
                    {
                        Console.WriteLine("Unrecognized command: {0}", cmd);
                        DisplayHelpScreen();
                    }
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected exception was thrown. \n{0}", ex.ToString());
                DisplayHelpScreen();
                return false;
            }
            return valid;
        }

        /// <summary>
        /// Attempts to execute a series of Cerebrum Shell commands.
        /// </summary>
        /// <param name="Commands">An array of Cerebrum Shell commands to execute.</param>
        /// <returns>True if all commands were valid and executed, False if an error occurred or any command was terminated.</returns>
        public bool ExecuteCommands(string[] Commands)
        {
            bool bAllValid = true;
            try
            {
                this.Executing = true;
                int i = 0;
                while ((i < Commands.Length) && (bAllValid) && (!TerminateBatch))
                {
                    bAllValid = ValidateCommand(Commands[i]);
                    i++;
                }
                if (bAllValid)
                {
                    i = 0;
                    while ((i < Commands.Length) && (bAllValid) && (!TerminateBatch))
                    {
                        bAllValid = ExecuteCommand(Commands[i]);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                DisplayHelpScreen();
                bAllValid = false;
            }
            this.Executing = false;
            return bAllValid;
        }

        /// <summary>
        /// Attempts to execute a series of Cerebrum Shell commands that are read in from a file.
        /// </summary>
        /// <param name="FilePath">The path to the file where the commands are located.  If a full path is not specified, the bin and work folders
        /// searched for matching file names, in that order.</param>
        /// <returns>True if the file exists, and all commands within were valid and executed, False if an error occurred or any command was terminated.</returns>
        public bool ExecuteFile(string FilePath)
        {
            try
            {
                string FileName = FilePath;
                if (!File.Exists(FileName))
                {
                    FileName = bin_path + FilePath;
                }
                if (!File.Exists(FileName))
                {
                    FileName = work_path + FilePath;
                }
                if (File.Exists(FileName))
                {
                    StreamReader reader = new StreamReader(FileName);
                    ArrayList lines = new ArrayList();
                    while (!reader.EndOfStream)
                        lines.Add(reader.ReadLine());
                    reader.Close();
                    string[] commands = new string[lines.Count];
                    lines.CopyTo(commands);
                    return ExecuteCommands(commands);
                }
                else
                {
                    throw new FileNotFoundException(String.Format("File not found: {0}", FilePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                DisplayHelpScreen();
                this.Executing = false;
                return false;
            }
        }

        /// <summary>
        /// Simple boolean function to verify that the specified program binary exists in the bin folder.
        /// </summary>
        /// <param name="ProgramBin">The short/simple name of the program to search for.</param>
        /// <returns>True if the program exists in the bin folder, False otherwise.</returns>
        private bool CheckBinExists(string ProgramBin)
        {
            return File.Exists(bin_path + ProgramBin + ".exe");
        }


        #region Implementation of FalconGlobal.IFalconLibrary

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Cerebrum Command-Line Shell";
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
