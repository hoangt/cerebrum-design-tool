using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using Tamir.SharpSsh;

namespace FalconJTAG_Programmer
{
    ///// <summary>
    ///// Type enumerating the legal possible values for processor types that may be programmed with JProgrammer.
    ///// </summary>
    //public enum PROC_TYPE 
    //{ 
    //    /// <summary>
    //    /// PowerPC processor.
    //    /// </summary>
    //    PPC = 0, 
    //    /// <summary>
    //    /// MicroBlaze processor.
    //    /// </summary>
    //    MB = 1 
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public class Utilities
    //{

    //    ///// <summary>
    //    ///// Executes shell command
    //    ///// </summary>
    //    ///// <param name="command">Command to execute</param>
    //    //public static void ExecuteShellCommand_old(string command)
    //    //{
    //    //    string stdout_results;
    //    //    System.Diagnostics.ProcessStartInfo sinf = new System.Diagnostics.ProcessStartInfo("xbash", command);
            
    //    //    sinf.RedirectStandardOutput = true;
            
    //    //    sinf.UseShellExecute = false;
    //    //    sinf.CreateNoWindow = true;
            
    //    //    System.Diagnostics.Process p = new System.Diagnostics.Process();
    //    //    p.StartInfo = sinf;
    //    //    p.Start();           

    //    //    while (!p.HasExited)
    //    //    {
    //    //        stdout_results = p.StandardOutput.ReadLine();
    //    //        Console.WriteLine(stdout_results);                
    //    //    }
    //    //}


    //    /// <summary>
    //    /// Executes an xbash shell command locally, by starting the xbash shell with the command as an argument.
    //    /// </summary>
    //    /// <param name="command">Command to execute</param>
    //    public static void ExecuteShellCommand(string command)
    //    {
    //        string stdout_results;
    //        System.Diagnostics.ProcessStartInfo sinf = new System.Diagnostics.ProcessStartInfo("xbash", command);

    //        sinf.RedirectStandardOutput = true;

    //        sinf.UseShellExecute = false;
    //        sinf.CreateNoWindow = true;

    //        System.Diagnostics.Process p = new System.Diagnostics.Process();
    //        p.StartInfo = sinf;
    //        p.Start();            
            
    //        while (!p.HasExited)
    //        {
    //            stdout_results = p.StandardOutput.ReadLine();
    //            Console.WriteLine(stdout_results);
    //        }
    //    }

    //    /// <summary>
    //    /// Executes the xbash shell locally, and writes the command to its input after it has started.
    //    /// </summary>
    //    /// <param name="command">Command to execute</param>
    //    public static void XbashCommand(string command)
    //    {
    //        string stdout_results;
    //        System.Diagnostics.ProcessStartInfo sinf = new System.Diagnostics.ProcessStartInfo("xbash");

    //        sinf.RedirectStandardOutput = true;
    //        sinf.RedirectStandardInput = true;

    //        sinf.UseShellExecute = false;
    //        sinf.CreateNoWindow = true;

    //        System.Diagnostics.Process p = new System.Diagnostics.Process();
    //        p.StartInfo = sinf;
    //        p.Start();

    //        p.StandardInput.WriteLine(command);

    //        p.WaitForExit();
    //        //while (!p.HasExited)
    //        //{
    //        //    stdout_results = p.StandardOutput.ReadLine();
    //            //MsgDisplayClient.DisplayMessage(stdout_results);
    //        //}
    //    }
        
    //    /// <summary>
    //    /// Executes a command remotely
    //    /// </summary>
    //    /// <param name="ServerName">The name of the server to connect to</param>
    //    /// <param name="UserName"></param>
    //    /// <param name="Password"></param>
    //    /// <param name="Command"></param>
    //    public static string RemoteSSHCommand(string server, string user, string pass, string cmd)
    //    {
    //        try
    //        {
    //            // populate crednetials                
    //            SshExec exec = new SshExec(server, user, pass);
    //            // establish connection
    //            exec.Connect();
    //            // execute command                
    //            string output = exec.RunCommand(cmd);
    //            // Print out results
    //            Console.WriteLine(output);
    //            // Disconnect from server
    //            exec.Close();
    //            return output;
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //            return null;
    //        }
    //    }
        
    //    /// <summary>
    //    /// Uses SCP to send a file from host to remote server
    //    /// </summary>
    //    /// <param name="server"></param>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="LocalFilePath"></param>
    //    /// <param name="RemoteFilePath"></param>
    //    public static void RemoteSendFile(string server, string user, string pass, string LocalFilePath, string RemoteFilePath)
    //    {
    //        try
    //        {
    //            Scp sshCp = new Scp(server, user, pass);

    //            sshCp.OnTransferStart += new FileTransferEvent(sshCp_OnTransferStart);
    //            sshCp.OnTransferProgress += new FileTransferEvent(sshCp_OnTransferProgress);
    //            sshCp.OnTransferEnd += new FileTransferEvent(sshCp_OnTransferEnd);

    //            sshCp.Connect();
                
    //            sshCp.To(LocalFilePath, RemoteFilePath);
                
    //            sshCp.Close();                
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Uses SCP to send a file from host to remote server
    //    /// </summary>
    //    /// <param name="server"></param>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="LocalFilePath"></param>
    //    /// <param name="RemoteFilePath"></param>
    //    /// <param name="permissions"></param>
    //    public static void RemoteSendFile(string server, string user, string pass, string LocalFilePath, 
    //        string RemoteFilePath, string permissions)
    //    {
    //        Scp sshCp = new Scp(server, user, pass); 
    //        try
    //        {
    //            sshCp.OnTransferStart += new FileTransferEvent(sshCp_OnTransferStart);
    //            sshCp.OnTransferProgress += new FileTransferEvent(sshCp_OnTransferProgress);
    //            sshCp.OnTransferEnd += new FileTransferEvent(sshCp_OnTransferEnd);

    //            sshCp.Connect();

    //            sshCp.To(LocalFilePath, RemoteFilePath);

    //            sshCp.Close();
    //            // change permission of file
    //            string command = "chmod " + permissions + " " + RemoteFilePath + " ";
    //            RemoteSSHCommand(server, user, pass, command);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //            sshCp.Close();
    //        }
    //    }

    //    /// <summary>
    //    /// uses SCP to transfer a file from remote server to host
    //    /// </summary>
    //    /// <param name="server"></param>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="RemoteFilePath"></param>
    //    /// <param name="LocalFilePath"></param>
    //    public static void RemoteReceiveFile(string server, string user, string pass, string RemoteFilePath, string LocalFilePath)
    //    {
    //        Scp sshCp = new Scp(server, user, pass);
            
    //        try
    //        {
    //            sshCp.OnTransferStart += new FileTransferEvent(sshCp_OnTransferStart);
    //            sshCp.OnTransferProgress += new FileTransferEvent(sshCp_OnTransferProgress);
    //            sshCp.OnTransferEnd += new FileTransferEvent(sshCp_OnTransferEnd);

    //            sshCp.Connect();
                
    //            sshCp.From(RemoteFilePath, LocalFilePath);

    //            sshCp.Close();                
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //            sshCp.Close();
    //        }
    //    }

    //    /// <summary>
    //    /// Transfer directory and its content recursively to remote server
    //    /// </summary>
    //    /// <param name="server"></param>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="LocalFilePath"></param>
    //    /// <param name="RemoteFilePath"></param>
    //    public static void RemoteSendDir(string server, string user, string pass, string LocalFilePath, string RemoteFilePath)
    //    {
    //        Scp sshCp = new Scp(server, user, pass); 
    //        try
    //        {
    //            sshCp.Recursive = true;
    //            sshCp.Verbos = false;

    //            sshCp.Connect();

    //            RemoteFilePath = RemoteFilePath + "/" + LocalFilePath.Substring(LocalFilePath.LastIndexOf("\\") + 1);

    //            sshCp.To(LocalFilePath, RemoteFilePath);

    //            sshCp.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //            sshCp.Close();
    //        }
    //    }        

    //    /// <summary>
    //    /// Transfer directory and its content recursively from remote server
    //    /// </summary>
    //    /// <param name="server"></param>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="RemoteFilePath"></param>
    //    /// <param name="LocalFilePath"></param>
    //    public static void RemoteReceiveDir(string server, string user, string pass, string RemoteFilePath, string LocalFilePath)
    //    {
    //        Scp sshCp = new Scp(server, user, pass); 
    //        try
    //        {
    //            sshCp.Recursive = true;
    //            sshCp.Verbos = false;

    //            sshCp.Connect();

    //            sshCp.From(RemoteFilePath, LocalFilePath);

    //            sshCp.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //            sshCp.Close();
    //        }
    //    }

    //    /// <summary>
    //    /// Called upon start of transfer
    //    /// </summary>
    //    /// <param name="src">Source</param>
    //    /// <param name="dst">Destination</param>
    //    /// <param name="transferredBytes">Data Transfered in bytes</param>
    //    /// <param name="totalBytes">Total size of transfer</param>
    //    /// <param name="message">Info message</param>
    //    private static void sshCp_OnTransferStart(string src, string dst, int transferredBytes, int totalBytes, string message)
    //    {
    //        Console.WriteLine();
    //        //progressBar = new ConsoleProgressBar();
    //        //progressBar.Update(transferredBytes, totalBytes, message);
    //        Console.WriteLine("File Transfer Started!");
    //    }

    //    /// <summary>
    //    /// invoked throughout transfer duration
    //    /// </summary>
    //    /// <param name="src">Source</param>
    //    /// <param name="dst">Destination</param>
    //    /// <param name="transferredBytes">Data Transfered in bytes</param>
    //    /// <param name="totalBytes">Total size of transfer</param>
    //    /// <param name="message">Info message</param>
    //    private static void sshCp_OnTransferProgress(string src, string dst, int transferredBytes, int totalBytes, string message)
    //    {
    //        //if(progressBar!=null)
    //        //{
    //        //	progressBar.Update(transferredBytes, totalBytes, message);
    //        //}
    //    }

    //    /// <summary>
    //    /// Invoked when transfer ends
    //    /// </summary>
    //    /// <param name="src">Source</param>
    //    /// <param name="dst">Destination</param>
    //    /// <param name="transferredBytes">Data Transfered in bytes</param>
    //    /// <param name="totalBytes">Total size of transfer</param>
    //    /// <param name="message">Info message</param>
    //    private static void sshCp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
    //    {
    //        //if(progressBar!=null)
    //        //{
    //        //    progressBar.Update(transferredBytes, totalBytes, message);
    //        //    progressBar=null;
    //        //}
    //        Console.WriteLine("Transfer Done!");
    //    }

    //    /// <summary>
    //    /// Modifies a text file by replacing occurances of a pattern with a specified string
    //    /// </summary>
    //    /// <param name="FilePath">Location of text file</param>
    //    /// <param name="pattern">Search pattern</param>
    //    /// <param name="ToBeReplacedWith">Text to replace pattern</param>
    //    public static void EditTextFile(string FilePath, string pattern, string ToBeReplacedWith)
    //    {
    //        // check if file exists
    //        if (!File.Exists(FilePath))
    //        {
    //            Console.WriteLine("Specified File or Path doesn't exist: " + FilePath);
    //            return;
    //        }

    //        Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    //        Match m;
    //        // Read file
    //        string[] Lines = File.ReadAllLines(FilePath);
    //        StringBuilder ModifiedLines = new StringBuilder();

    //        // Search for pattern and replace
    //        foreach (string line in Lines)
    //        {
    //            m = r.Match(line, 0);
    //            if (m.Success)
    //                ModifiedLines.AppendLine(line.Replace(m.Value, ToBeReplacedWith));
    //            else
    //                ModifiedLines.AppendLine(line);
    //        }
            
    //        // Write file back
    //        File.WriteAllText(FilePath, ModifiedLines.ToString());
    //    }

    //    /// <summary>
    //    /// Tars and Zipps a folder 
    //    /// </summary>
    //    /// <param name="TarFile">Name of generated file</param>
    //    /// <param name="TargetFolder">Folder to be tarred and zipped</param>
    //    public static void TarZipFolder(string TarFile, string TargetFolder)
    //    {
    //        const string TarBaseCommand = "/usr/bin/tar -czf ";
    //        const string TarExtention = ".tgz";

    //        System.Diagnostics.ProcessStartInfo sinf = new System.Diagnostics.ProcessStartInfo("xbash");
    //        sinf.RedirectStandardOutput = true;
    //        sinf.RedirectStandardInput = true;
    //        sinf.UseShellExecute = false;
    //        sinf.CreateNoWindow = true;

    //        System.Diagnostics.Process p = new System.Diagnostics.Process();
    //        p.StartInfo = sinf;
    //        p.Start();

    //        // Change directory to parent of target folder
    //        string ParentFolder = Directory.GetParent(TargetFolder).Parent.FullName;             
    //        // Replace '\' with '/'
    //        ParentFolder = ParentFolder.Replace("\\", "/");
    //        p.StandardInput.WriteLine("cd " + ParentFolder);

    //        // Execute tar+zip command
    //        string TarCommand = TarBaseCommand + TarFile + TarExtention + " " +
    //            Directory.GetParent(TargetFolder).Name;
    //        p.StandardInput.WriteLine(TarCommand);
    //        p.StandardInput.WriteLine("echo DoneOperation");
    //        // wait until folder is zipped 
    //        string Result;
    //        while (!(Result = p.StandardOutput.ReadLine()).Contains("DoneOperation")) 
    //            Thread.Sleep(1000);

    //        p.Close();
    //        return;
    //    }
    //}
}
