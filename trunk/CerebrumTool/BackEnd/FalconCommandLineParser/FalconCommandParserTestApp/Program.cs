/*********************************************************************************************************** 
 * FalconCommandParseTestApp\Program.cs
 * Name: Matthew Cotter
 * Date: 4 Jun 2010 
 * Description: Console Application used to perform initial testing of the command line parser library.
 * Notes:
 *     
 * History: 
 * >> ( 4 Jun 2010): Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconCommandLineParser;
using System.IO;


namespace FalconCommandParserTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FalconCommandParser fcp = new FalconCommandParser("::");
            fcp.CommandDelimiter = "--";

            Console.WriteLine(fcp.GetFalconComponentVersion());
            Console.WriteLine();
            Console.WriteLine("Type 'batch <script_file>' to parse a script file.");
            Console.WriteLine("Type 'quit' or 'exit' to terminate.");
            Console.WriteLine();
            Console.Write("> ");
            string input = Console.ReadLine();
            while ((input != "quit") && (input != "exit"))
            {
                while (input.EndsWith(" \\"))
                    input = input + Environment.NewLine + Console.ReadLine();
                if (input.StartsWith("batch"))
                {
                    string scriptfile = input.Substring(5).Trim();
                    if (fcp.ParseFile(scriptfile.Replace("\"", string.Empty)))
                    {
                        LinkedList<FalconCommand> clist = fcp.Commands;

                        for (LinkedListNode<FalconCommand> node = clist.First; node != null; node = node.Next)
                        {
                            Console.WriteLine("\t{0}", node.Value.CommandSwitch);
                            int i = 0;
                            for (LinkedListNode<string> arg = node.Value.Arguments.First; arg != null; arg = arg.Next)
                            {
                                Console.WriteLine("\t\tArg {0}: {1}", i, arg.Value);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Encountered an error in parsing the command string.");
                    }
                }
                else
                {
                    if (fcp.ParseString(input))
                    {
                        LinkedList<FalconCommand> clist = fcp.Commands;

                        for (LinkedListNode<FalconCommand> node = clist.First; node != null; node = node.Next)
                        {
                            Console.WriteLine("\t{0}", node.Value.CommandSwitch);
                            int i = 0;
                            for (LinkedListNode<string> arg = node.Value.Arguments.First; arg != null; arg = arg.Next)
                            {
                                Console.WriteLine("\t\tArg {0}: {1}", i, arg.Value);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Encountered an error in parsing the command string.");
                    }
                }
                Console.WriteLine();
                Console.Write("> ");
                input = Console.ReadLine();
            }
        }
    }
}
