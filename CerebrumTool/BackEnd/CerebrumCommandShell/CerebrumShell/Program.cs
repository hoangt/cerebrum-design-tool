/*********************************************************************************************************** 
 * CerebrumShell\Program.cs
 * Name: Matthew Cotter
 * Date: 4 Jun 2010 
 * Description: Program to wrap functionality of the CerebrumCommandShell library.
 * Notes:
 *     
 * History: 
 * >> (11 Jun 2010): Replaced complicated code with modularized code from CerebrumCommandShell library.
 * >> (10 Jun 2010): Corrected batch file processing and execution.
 * >> ( 4 Jun 2010): Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumCommandShell;
using System.Reflection;
using System.IO;

namespace CerebrumShell
{
    class Program
    {
        private static CerebrumCommandShell.CerebrumCommandShell ccs;

        static void Main(string[] args)
        {
            ccs = new CerebrumCommandShell.CerebrumCommandShell();
            ccs.BinaryDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            if (!ccs.BinaryDirectory.EndsWith("\\"))
                ccs.BinaryDirectory += "\\";
            ccs.WorkingDirectory = Directory.GetCurrentDirectory();
            ccs.ShellWelcome();

            if (args.Length == 0)
            {
                // Start in interactive-mode
                ccs.InteractiveMode = true;
                ccs.InteractiveWelcome();
                ccs.Prompt();
                string input = Console.ReadLine();
                while (!ccs.IsQuitCommand(input))
                {
                    if (input == null)
                        return;
                    if (input.Length > 0)
                    {
                        while (input.EndsWith(" \\"))
                            input = input + Environment.NewLine + Console.ReadLine();
                        ccs.ExecuteString(input);
                        Console.WriteLine();
                    }
                    ccs.TerminateBatch = false;
                    ccs.Prompt();
                    input = Console.ReadLine();
                }
            }
            else
            {
                // Start in single-shot / batch-mode
                string argline = string.Join(" ", args);
                ccs.ExecuteString(argline);
            }
        }
    }
}
