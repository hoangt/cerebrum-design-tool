//////////////////////////////////////////////////
//
//file name : JProgrammerMain.cs
//
//Created by: Abdul Abumurad
//
//Date : 25 April 2010
//
//Updated : 30 April 2010 used for testing.
//          
//
//////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Tamir.SharpSsh;

namespace FalconJTAG_Programmer
{
    public class JProgrammerMain
    {        
        static void Main(string[] args)
        {

            JProgrammer JP = new JProgrammer();

            // login test
            JP.Username = "aka133";
            JP.UserPass = "";
            JP.Servername = "archimedes.cse.psu.edu";
            JP.login();
            //JP.logout();
            Console.WriteLine("logged in");
          

            // set local directory test and remote directory
            JP.localOSType = JProgrammer.OSVersion.WINDOWS;
            JP.remoteOSType = JProgrammer.OSVersion.LINUX;
            JP.Set_Local_Dir("C:\\test");
            JP.Set_Remote_Dir("/home/grads/aka133/www/abed");

            //copy file and dir to test
            //JP.overWriteFiles = true;
            //JP.Copy_File_To("what_inside.txt");
            JP.createFolder_ifNotExist = true;
            JP.Copy_Dir_To();

            // copy file and Dir from test
            //JP.overWriteFiles = false;
            //JP.Copy_File_From("UnixProgrammingTools.pdf");
            JP.Set_Remote_Dir("/home/grads/aka133/www");
            //JP.createFolder_ifNotExist = true;
            //JP.Copy_Dir_From();

            // build up cmd and tcl files
            JP.ClearCMDfile(); // clean any previous commands
            JP.ClearTCLfile(); // clean any previous commands
            JP.Clean_iMPACT_Cable(); // generate script to clean impact cable.
            
            // necessary options to download bit file
            JP.impactModeOpt = "-bscan";
            JP.impactCableOpt = "-p auto";
            JP.JTAG = 1;
            JP.bitFileName = "rema.bit";
            JP.DownloadBitFile();   // will add commands in a internal string variable


            JP.cmdFileName = "rema_cmd.txt";
            JP.Generate_cmd_File(); // will save internal string into a .cmd file

            JP.PCType = JProgrammer.ProcessorType.PPC;
            JP.ProcessorOpt = "mdm";
            JP.elfFileName = "rema_elf.txt";
            JP.Clean_XMD_Cable();
            JP.DownloadElfFile();

            JP.tclFileName = "rema_tcl.txt";
            JP.Generate_tcl_file();

            JP.logout();
            Console.WriteLine("logged out");

            Console.ReadKey();
           
            return;


            #region Jprogrammer User Interface <Not Used>

            /////
            ///// Defining Global Variables may be used
            ///// through out the Application
            /////
            //#region Variables
            //String defProgServer = "archimedes.cse.psu.edu";
            //String progServer = "archimedes.cse.psu.edu"; // user to hold server default name
            //String userName = ""; // used to read user name
            //String userPass = ""; // used to read password
            //bool exitLoop = false; // exit condition
            //bool isRemoteProgrammer = false; // true if remote programming server is used.
            //bool threadStarted = false; // to indicate if thread already started
            //bool isLocalProgrammer = false; // to decide if local machine is used
            //bool localStarted = false; // idicate if local process is started or not.
            //String userCommand = ""; // command to be executed
            //String Prompt = "JProgrammer"; // console cursor
            //String defLocalDir = "";
            //String defRemoteDir = "";
            //JProgrammer ConThread = new JProgrammer(); // to handle remote prog.ing 
            //ProcessStartInfo startinfo = new ProcessStartInfo("xbash"); // for local prog.ing.
            //Process localProcess = new Process();
            //List<uint> JTAGs = new List<uint>();
            //#endregion


            /////////////////////////////////////////////
            /////   Main List with programer Commands ///
            /////////////////////////////////////////////
            //String Help = "\nJTAG Programmer Commands";
            //Help += "\n1- login (li)";
            //Help += "\n2- logout (lo)";
            //Help += "\n3- Clear Imapct Cable (cic)";
            //Help += "\n4- Clear Xmd Cable (cxc)";
            //Help += "\n5- set local Directory (sld)";
            //Help += "\n6- set remote directory (srd)";
            //Help += "\n7- copy file to (cft)";
            //Help += "\n8- copy file from (cff)";
            //Help += "\n8- set or read JTAG NRs (setjtags)";
            //Help += "\n9- download Bit Stream (dbs)";
            //Help += "\n10- download soft file (dsf)";
            //Help += "\n11- exit application (exit)\n\n";


            /////////////////////////////////////////////
            /////   Main Loop to Interface with user  ///
            /////////////////////////////////////////////

            //Console.Write(Help);
            //while (!exitLoop)
            //{
            //    ///
            //    /// Determine if local machine is used as a server
            //    /// or a remote machine is used as server.
            //    ///
            //    # region local or Remote server
            //    while (true && !localStarted && !threadStarted)
            //    {
            //        Console.Write("Local (l) or Remote (r) server?");
            //        string resp = Console.ReadLine();
            //        if (resp == "l")
            //        {
            //            isLocalProgrammer = true;
            //            break;
            //        }
            //        else
            //        {
            //            if (resp == "r")
            //            {
            //                isRemoteProgrammer = true;
            //                break;
            //            }
            //            else
            //                Console.WriteLine("unrecognized response! use l=local and r=remote?");
            //        }
            //    }

            //    ///
            //    /// start a process with "xbash" if local machine
            //    /// is used
            //    ///
            //    if (isLocalProgrammer && !localStarted)
            //    {
            //        startinfo.RedirectStandardOutput = true;
            //        startinfo.RedirectStandardInput = true;
            //        startinfo.UseShellExecute = false;
            //        startinfo.CreateNoWindow = true;
            //        localProcess.StartInfo = startinfo;
            //        localProcess.Start();
            //        while (!localProcess.Responding)
            //            Thread.Sleep(100);
            //        localStarted = true;
            //    }
            //    #endregion


            //    #region Printing a message from connection Thread
            //    // Printing Messages from connectionThread
            //    if (threadStarted && ConThread.isMessage)
            //    {
            //        Console.Write(ConThread.retMessage);
            //        Console.WriteLine();
            //        ConThread.isMessage = false;
            //        ConThread.retMessage = "";
            //    }
            //    #endregion


            //    Console.Write(Prompt + " >");
            //    userCommand = Console.ReadLine();

            //    switch (userCommand.ToLower())
            //    {
            //        #region Login Case
            //        case "li":
            //        case "login":
            //            if (!isLocalProgrammer)
            //            {
            //                if (threadStarted)
            //                {
            //                    Console.WriteLine("in order to login; please logout first from current server");
            //                }
            //                else
            //                {
            //                    Console.Write("\n" + Prompt + " >" + progServer + ">>");
            //                    string tempResp = Console.ReadLine();
            //                    if (tempResp.Replace("\n", "") != "")
            //                        Prompt = tempResp;
            //                    else
            //                        Prompt = defProgServer;

            //                    Console.Write("\nUser Name:");
            //                    userName = Console.ReadLine();
            //                    Console.Write("\nPass :");
            //                    ConsoleColor old_Color = Console.ForegroundColor;
            //                    Console.ForegroundColor = Console.BackgroundColor;
            //                    Console.CursorVisible = false;
            //                    userPass = Console.ReadLine();
            //                    Console.CursorVisible = true;
            //                    Console.ForegroundColor = old_Color;
            //                    // make sure all parametrs set before starting a thread
            //                    if (progServer != "" && userName != "" && userPass != "" && !threadStarted && isRemoteProgrammer)
            //                    {
            //                        ConThread.StartThread(progServer, userName, userPass);
            //                        threadStarted = true;
            //                    }
            //                }
            //            }
            //            else
            //                Console.WriteLine("login command cant be  used locally!");
            //            break;
            //        #endregion

            //        #region Logout Case
            //        case "lo":
            //        case "logout":
            //            Prompt = "JProgrammer";
            //            userPass = "";
            //            userName = "";
            //            if (threadStarted)
            //            {
            //                threadStarted = false;
            //                ConThread.StopFlag = true;
            //                isRemoteProgrammer = false;
            //            }
            //            if (isLocalProgrammer)
            //            {
            //                localProcess.Close();
            //                isLocalProgrammer = false;
            //                localStarted = false;
            //            }
            //            break;
            //        #endregion

            //        #region Clear iMPACT cable Case
            //        case "cic":
            //        case "clearimcable":
            //            if (isLocalProgrammer)
            //            {
            //                bool cleanProblem = false;
            //                localProcess.StandardInput.WriteLine("impact -batch");
            //                while (!localProcess.StandardOutput.EndOfStream)
            //                {
            //                    if (localProcess.StandardOutput.Peek() == '>')
            //                        break;
            //                    string tempResp = localProcess.StandardOutput.ReadLine();
            //                    Console.WriteLine(tempResp);
            //                }

            //                int exitcount = 0; // to exit if waited too long
            //                localProcess.StandardInput.WriteLine("cleancablelock");
            //                while (!localProcess.StandardOutput.ReadLine().Contains("Done"))
            //                {

            //                    Thread.Sleep(100);
            //                    exitcount++;
            //                    if (exitcount > 10) // all we need is two line to read "Done".
            //                    {
            //                        cleanProblem = true;
            //                        break;
            //                    }
            //                }
            //                if (!cleanProblem)
            //                    Console.WriteLine("cables Cleaned Successfully");
            //            }
            //            else
            //            {
            //                if (threadStarted)
            //                {
            //                    while (ConThread.isCommand)
            //                        Thread.Sleep(100);
            //                    ConThread.command = "cic";
            //                    ConThread.isCommand = true;
            //                }
            //                else
            //                {
            //                    Console.WriteLine("please login first using li command?");
            //                }
            //            }
            //            break;
            //        #endregion

            //        #region Clean xmd Cable
            //        case "cxc":
            //        case "clearxmdcable":
            //            if (threadStarted)
            //            {
            //                ConThread.command = "cxc";
            //                ConThread.isCommand = true;
            //            }
            //            else if (isLocalProgrammer)
            //            {
            //                localProcess.StandardInput.WriteLine("xmd");
            //                string tempResp = localProcess.StandardOutput.ReadToEnd();
            //                if (tempResp.Contains("XMD%"))
            //                {
            //                    Console.WriteLine("XMD started on local machine");
            //                    localProcess.StandardInput.WriteLine("xclean_cable");
            //                    tempResp = localProcess.StandardOutput.ReadToEnd();
            //                    while (!tempResp.Contains("X Cable Locks Removed"))
            //                    {
            //                        Thread.Sleep(100);
            //                        tempResp = localProcess.StandardOutput.ReadToEnd();
            //                    }
            //                }
            //                else
            //                    Console.WriteLine("Sorry, make sure that xmd is installed on local machine");
            //            }
            //            break;
            //        #endregion

            //        #region Set Local Directory Case
            //        case "sld":
            //        case "setlocdir":
            //            if (defLocalDir != "")
            //                Console.Write(Prompt + " >" + defLocalDir + ">>");
            //            else
            //                Console.Write("\nLocal Directory :");
            //            string tempResponse = Console.ReadLine();
            //            if (tempResponse.Replace("\n", "") != "")
            //                defLocalDir = tempResponse;
            //            break;
            //        #endregion

            //        #region Set Remote Directory Case
            //        case "srd":
            //        case "setremdir":
            //            if (defRemoteDir != "")
            //                Console.Write(Prompt + " >" + defRemoteDir + ">>");
            //            else
            //                Console.Write("\nRemote Directory :");
            //            tempResponse = Console.ReadLine();
            //            if (tempResponse.Replace("\n", "") != "")
            //                defRemoteDir = tempResponse;

            //            break;
            //        #endregion

            //        #region Copy local file to
            //        case "cft":
            //        case "cpfileto":
            //            //if (defLocalDir == "" || defRemoteDir == "")
            //            //{
            //            //    Console.WriteLine("Remote or/and Local Diretories is not Set");
            //            //    Console.WriteLine("Use srd or/and sld commands to set these directories");
            //            //}
            //            //else
            //            //{
            //            //    if(threadStarted)
            //            //    {
            //            //        while (ConThread.isCommand)
            //            //            Thread.Sleep(100);
            //            //        ConThread.localDir = defLocalDir;
            //            //        ConThread.remoteDir = defRemoteDir;
            //            //        ConThread.command = "cft";
            //            //        ConThread.isCommand = true;
            //            //    }
            //            //    else
            //            //        Console.WriteLine("user name or password is not set! use li command first");

            //            //}
            //            break;
            //        #endregion

            //        #region Copy file from Command
            //        case "cff":
            //        case "cpfilefrom":
            //            //if (defLocalDir == "" || defRemoteDir == "")
            //            //{
            //            //    Console.WriteLine("Remote or/and Local Diretories is not Set");
            //            //    Console.WriteLine("Use srd or/and sld commands to set these directories");
            //            //}
            //            //else
            //            //{
            //            //    if (threadStarted)
            //            //    {
            //            //        ConThread.localDir = defLocalDir;
            //            //        ConThread.remoteDir = defRemoteDir;
            //            //        ConThread.command = "cff";
            //            //        ConThread.isCommand = true;
            //            //    }
            //            //    else
            //            //        Console.WriteLine("user name or password is not set! use li command first");

            //            //}
            //            break;
            //        #endregion

            //        #region set/read JTAG NRs
            //        case "setjtag":
            //            //Console.Write("Set JTAGs Manually=(m) or Automatically=(a)? ");
            //            //string tempresp = Console.ReadLine();
            //            //if (tempresp == "m")
            //            //{
            //            //    bool EnterMore = true;
            //            //    while (EnterMore)
            //            //    {
            //            //        Console.Write("please Enter JTAG uint Number? ");
            //            //        tempresp = Console.ReadLine();
            //            //        JTAGs.Add(Convert.ToUInt32(tempresp));
            //            //        Console.Write("\nis there any more JTAG numbers to enter? (y/n) :");
            //            //        tempresp = Console.ReadLine();
            //            //        if (tempresp == "y")
            //            //            EnterMore = true;
            //            //        else
            //            //            if (tempresp == "n")
            //            //                EnterMore = false;
            //            //            else
            //            //                Console.WriteLine("Invalid Choice! use y=yes or n=nop");
            //            //    }
            //            //    ConThread.isJTAGsManual = true;
            //            //    ConThread.JTAGs = JTAGs.ToArray();
            //            //    //foreach (uint jtag in JTAGs)
            //            //    //    Console.Write("{0}, ",jtag);
            //            //    Console.WriteLine();
            //            //}
            //            //else
            //            //{
            //            //    if (tempresp == "a")
            //            //    {
            //            //        ConThread.isJTAGsManual = false;
            //            //    }
            //            //    else
            //            //        Console.WriteLine("Invalid choice! use a=automatic detection and m=manual assignment?");
            //            //}
            //            break;
            //        #endregion

            //        #region Download bit file
            //        case "dbs":
            //        case "dwnbitstream":

            //            //if (threadStarted)
            //            //{
            //            //    ConThread.command = "dbs";
            //            //    ConThread.isCommand = true;
            //            //}
            //            //else
            //            //{
            //            //    if (isLocalProgrammer)
            //            //    {
            //            //        if (defLocalDir == "")
            //            //        {
            //            //            Console.WriteLine("Please use set local Directory (sld) command first to enter bitfile path?");
            //            //            break;
            //            //        }
            //            //        else
            //            //        {
            //            //            bool filefound = false;
            //            //            Console.Write("\nbitstream file name (in local directory): ");
            //            //            string tempfile = Console.ReadLine();
            //            //            int fileIndex = 0;
            //            //            string []tempDirfiles = System.IO.Directory.GetFiles(defLocalDir);
            //            //            foreach (string file in tempDirfiles)
            //            //            {
            //            //                if (file.Replace(defLocalDir, "") == tempfile || file.Replace(defLocalDir, "") == "\\"+tempfile)
            //            //                {
            //            //                    filefound = true;
            //            //                    break;
            //            //                }
            //            //                fileIndex++;
            //            //            }
            //            //            if (filefound)
            //            //            {
            //            //                localProcess.StandardInput.WriteLine("setmode -bscan");
            //            //                while (!localProcess.StandardOutput.EndOfStream)
            //            //                {
            //            //                    if (localProcess.StandardOutput.Peek() == '>')
            //            //                        break;
            //            //                    string tempResp = localProcess.StandardOutput.ReadLine();
            //            //                    Console.WriteLine(tempResp);
            //            //                }
            //            //                localProcess.StandardInput.WriteLine("setcable -p auto");
            //            //                while (!localProcess.StandardOutput.EndOfStream)
            //            //                {
            //            //                    if (localProcess.StandardOutput.Peek() == '>')
            //            //                        break;
            //            //string tempResp = localProcess.StandardOutput.ReadLine();
            //            //                    Console.WriteLine(tempResp);
            //            //                }
            //            //                Console.Write("choose JTAG number to program ? ");
            //            //                tempresp =  Console.ReadLine();
            //            //                if (JTAGs.Contains(Convert.ToUInt32(tempresp)))
            //            //                {
            //            //                    localProcess.StandardInput.WriteLine("addDevice -p {0} -file {1}", Convert.ToUInt32(tempresp),tempDirfiles[fileIndex]);
            //            //                }
            //            //                else
            //            //                {
            //            //                    Console.WriteLine("Invalid JTAG number please try dbs command again");
            //            //                    break;
            //            //                }

            //            //                localProcess.StandardInput.WriteLine("program -p {0}", Convert.ToUInt32(tempresp));
            //            //                while (!localProcess.StandardOutput.EndOfStream)
            //            //                {
            //            //                    if (localProcess.StandardOutput.Peek() == '>')
            //            //                        break;
            //            //                    string tempResp = localProcess.StandardOutput.ReadLine();
            //            //                    Console.WriteLine(tempResp);
            //            //                }
            //            //            }
            //            //            else
            //            //            {
            //            //                Console.WriteLine("Bit file sepecified may not exist");
            //            //            }
            //            //        }
            //            //    }
            //            //    else
            //            //        Console.WriteLine("please login first using li command?");
            //            //}
            //            break;
            //        #endregion

            //        #region Download ELF file
            //        case "delf":
            //        case "dwnelf":
            //        //Console.Write("Enter Processor Type <ppc, mb> :");
            //        //string PCtype = Console.ReadLine();
            //        //if (threadStarted)
            //        //{
            //        //    ConThread.command = "delf"; // after installing xmd on programming server
            //        //    ConThread.isCommand = true;
            //        //}
            //        //else
            //        //{
            //        //    if (isLocalProgrammer)
            //        //    {
            //        //        if (defLocalDir == "")
            //        //        {
            //        //            Console.WriteLine("Please use set local Directory (sld) command first to enter .elf file path?");
            //        //            break;
            //        //        }
            //        //        else
            //        //        {
            //        //            bool filefound = false;
            //        //            Console.Write("\n ELF file name (in local directory): ");
            //        //            string tempfile = Console.ReadLine();
            //        //            int fileIndex = 0;
            //        //            try
            //        //            {
            //        //                string[] tempDirfiles = System.IO.Directory.GetFiles(defLocalDir);
            //        //                foreach (string file in tempDirfiles)
            //        //                {
            //        //                    if (file.Replace(defLocalDir, "") == tempfile || file.Replace(defLocalDir, "") == "\\" + tempfile)
            //        //                    {
            //        //                        filefound = true;
            //        //                        break;
            //        //                    }
            //        //                    fileIndex++;
            //        //                }

            //        //if (filefound)
            //        //{
            //        //    Console.Write("choose JTAG number to program ? ");
            //        //    tempresp = Console.ReadLine();
            //        //    if (JTAGs.Contains(Convert.ToUInt32(tempresp)))
            //        //    {
            //        //        if (PCtype.ToLower() == "ppc")
            //        //            localProcess.StandardInput.WriteLine("connect ppc hw -debugdevice devicenr" + Convert.ToUInt32(tempresp));
            //        //        else if (PCtype == "mb")
            //        //            localProcess.StandardInput.WriteLine("connect mb mbm -debugdevice devicenr" + Convert.ToUInt32(tempresp));
            //        //        else
            //        //            Console.WriteLine("Undefined Processor type, use delf command again");
            //        //    }
            //        //    else
            //        //    {
            //        //        Console.WriteLine("Invalid JTAG number please try delf command again");
            //        //        break;
            //        //                    }

            //        //                    while (!localProcess.StandardOutput.EndOfStream)
            //        //                    {
            //        //                        if (localProcess.StandardOutput.Peek() == '%')
            //        //                            break;
            //        //                        string tempResp = localProcess.StandardOutput.ReadLine();
            //        //                        Console.WriteLine(tempResp);
            //        //                    }




            //        //                    localProcess.StandardInput.WriteLine("dow {0}", tempDirfiles[fileIndex]);
            //        //                    while (!localProcess.StandardOutput.EndOfStream)
            //        //                    {
            //        //                        if (localProcess.StandardOutput.Peek() == '%')
            //        //                            break;
            //        //                        string tempResp = localProcess.StandardOutput.ReadLine();
            //        //                        Console.WriteLine(tempResp);
            //        //                    }
            //        //                }
            //        //                else
            //        //                {
            //        //                    Console.WriteLine("Bit file sepecified may not exist");
            //        //                }
            //        //            }
            //        //            catch (Exception e)
            //        //            {
            //        //                Console.WriteLine(e.Message);
            //        //            }
            //        //        }
            //        //    }
            //        //    else
            //        //        Console.WriteLine("please login first using li command?");
            //        //}
            //        //break; 
            //        #endregion

            //        #region Exit Case
            //        case "exit":
            //            if (threadStarted)
            //            {
            //                threadStarted = false;
            //                ConThread.StopFlag = true;
            //                isRemoteProgrammer = false;
            //            }
            //            if (isLocalProgrammer)
            //            {
            //                localProcess.Close();
            //                isLocalProgrammer = false;
            //                localStarted = false;
            //            }
            //            return;
            //        #endregion

            //        #region Default Case
            //        default:

            //            if ((userCommand.ToLower()).Contains("exe "))
            //            {
            //                ConThread.command = userCommand.ToLower();
            //                ConThread.isCommand = true;
            //            }
            //            else
            //            {
            //                Console.WriteLine("Undefined command");
            //            }
            //            break;

            //        #endregion


            //    }
            //}



            /////
            ///// Reading Server name to Server hstring
            ///// User name to user string
            ///// users' password to pass string
            ///////
            ////Console.Write("\nProgramming Server name:");
            ////progServer = Console.ReadLine();
            ////Console.Write("\nUser name:");
            ////userName = Console.ReadLine();


            /////
            ///// connecting to programming server
            ///// and test connection
            /////



            ////if (exeShell.Connected)
            ////{
            ////    string err = "";
            ////    string output = "";
            ////    Console.WriteLine("Server {0} is connected", progServer);
            ////    exeShell.RunCommand("impact -batch",ref output, ref err);
            ////    Console.Write("output = "+output);
            ////    Console.Write("Error = "+err);

            ////    Console.WriteLine("\nImpact is running ...");
            ////    exeShell.RunCommand("exit");
            ////    exeShell.Close();
            ////}
            ////else
            ////{
            ////    Console.WriteLine("Failed to connect");
            ////}
            
            #endregion               
        }
    }
}
