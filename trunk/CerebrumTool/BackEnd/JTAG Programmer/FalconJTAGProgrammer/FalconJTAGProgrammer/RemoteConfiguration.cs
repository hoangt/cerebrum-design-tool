using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;



namespace FalconJTAG_Programmer
{
    //public class RemoteConfiguration
    //{        

    //    /// <summary>
    //    /// Launches CableServer on remote server
    //    /// </summary>
    //    /// <param name="ServerName">IP Address of remote server</param>
    //    /// <param name="UserName">User name</param>
    //    /// <param name="Password">password</param>
    //    public static void RemoteLaunchCableServer(string ServerName, string UserName, 
    //                                                string Password)
    //    {
    //        try
    //        {
    //            // Populate Credentials
    //            SshShell ssh = new SshShell(ServerName, UserName, Password); // Used to launch CableServer
    //            SshExec exec = new SshExec(ServerName, UserName, Password); // used to check if CableServer Running 				
    //            // establish connection
    //            exec.Connect();
    //            // checking if CableServer already Running
    //            if ((exec.RunCommand("ps axu | grep -v grep | grep -o CableServer")).Contains("CableServer"))
    //            {
    //                Console.WriteLine("CableServer is already Running!");
    //            }
    //            else
    //            {
    //                ssh.Connect();
    //                ssh.ExpectPattern = "Main -> Waiting for a connection...";
    //                ssh.RemoveTerminalEmulationCharacters = true;
    //                Console.WriteLine("");
    //                Console.WriteLine("Launching...\n");
    //                ssh.WriteLine("CableServer &");

    //                if ((exec.RunCommand("ps axu | grep -v grep | grep -o CableServer")).Contains("CableServer"))
    //                {
    //                    Console.WriteLine("CableServer is Launched Successful!");
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Launching CableServe has Failed! Please Try to launch it Manually!");
    //                }
    //            }
    //            // Disconnecting 
    //            ssh.Close();
    //            exec.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //    }

        
    //    /// <summary>
    //    /// Launches xmd server and return port
    //    /// </summary>
    //    /// <param name="server">IP address of xmd server</param>
    //    /// <param name="user">User Name</param>
    //    /// <param name="pass">Password</param>
    //    /// <param name="ProcessorType">Type of processor</param>
    //    /// <param name="JTAG_NR">FPA Number to debug</param>
    //    /// <returns>Port Number xmd server is listening to</returns>
    //    public static int RemoteLaunchXMDServer(ref SshShell xmd_shell, string server, 
    //                                            string user, string pass, PROC_TYPE ProcessorType, 
    //                                            uint JTAG_NR)
    //    {
    //        string ConnectCommand;
    //        int XMDPort = -1;
    //        string tmp;
    //        try
    //        {
    //            StringBuilder output = new StringBuilder();
    //            SshShell ssh = new SshShell(server, user, pass);
    //            // Connect to server
    //            ssh.Connect();

    //            if (ProcessorType == PROC_TYPE.PPC)
    //                ConnectCommand = "connect ppc hw -debugdevice devicenr " + JTAG_NR;
    //            else if (ProcessorType == PROC_TYPE.MB)
    //                ConnectCommand = "connect mb mdm -debugdevice devicenr " + JTAG_NR;
    //            else
    //            {
    //                Console.WriteLine("Unknown Processor Type!");
    //                return -1;
    //            }
    //            // Launching XMD                
    //            ssh.RemoveTerminalEmulationCharacters = true;
    //            ssh.WriteLine("xmd");
    //            tmp = ssh.Expect("XMD%");
    //            if (!tmp.Contains("XMD% \r\r\nXMD% "))
    //                tmp = ssh.Expect("XMD%");
    //            // Connect to processor
    //            ssh.WriteLine(ConnectCommand);

    //            string result = ssh.Expect("XMD%");
    //            if (result.Contains(" at TCP port no "))
    //            {
    //                XMDPort = Convert.ToInt32((result.Substring(result.LastIndexOf("at TCP port no ") + 15, 5)));
    //                Console.WriteLine("XMD Successfully connected to FPGA at port: " + XMDPort);

    //                ssh.SuspendCommand();
    //                tmp = ssh.Expect("Suspended");
    //                ssh.WriteLine("bg");
    //                tmp = ssh.Expect("xmd");
    //                //ssh.WriteLine("exit");
    //                //ssh.Close();
    //                xmd_shell = ssh;
    //            }
    //            else if (result.Contains("Cable is LOCKED") && result.Contains("xclean_cablelock"))
    //            {
    //                Console.WriteLine("JTAG Cable is LOCKED!\nTry using one of the following:");
    //                Console.WriteLine("1. Execute XMD Clean Cable\n2. Execute iMPACT Clean Cable");
    //                XMDPort = -1;
    //                ssh.Close();
    //            }
    //            else
    //            {
    //                Console.WriteLine("XMD Operation Failed!");
    //                XMDPort = -1;
    //                ssh.Close();
    //            }

    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //        return XMDPort;
    //    }

    //    /// <summary>
    //    /// Clean XMD Cable Locks
    //    /// </summary>
    //    /// <param name="server">Server Address</param>
    //    /// <param name="user">User Name</param>
    //    /// <param name="pass">Password</param>
    //    public static void RemoteXMDCleanCable(string server, string user, 
    //                                        string pass)
    //    {
    //        try
    //        {
    //            string tmp;
    //            SshShell ssh = new SshShell(server, user, pass);
    //            // Connect to server
    //            ssh.Connect();

    //            // Launching XMD                
    //            ssh.RemoveTerminalEmulationCharacters = true;
    //            ssh.WriteLine("xmd");
    //            tmp = ssh.Expect("XMD%");
    //            if (!tmp.Contains("XMD% \r\r\nXMD% "))
    //                tmp = ssh.Expect("XMD%");

    //            ssh.WriteLine("xclean_cable");

    //            tmp = ssh.Expect("Cable Locks Removed");

    //            Console.WriteLine("XMD Cable Locks Removed!");
    //            ssh.WriteLine("exit");
    //            ssh.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Clean iMPACT Cable Locks
    //    /// </summary>
    //    /// <param name="server">Server Address</param>
    //    /// <param name="user">User Name</param>
    //    /// <param name="pass">Password</param>
    //    public static void RemoteiMPACTCleanCable(string server, string user, 
    //                                        string pass)
    //    {
    //        try
    //        {
    //            string tmp;
    //            SshShell ssh = new SshShell(server, user, pass);
    //            // Connect to server
    //            ssh.Connect();

    //            // Launching XMD                
    //            ssh.RemoveTerminalEmulationCharacters = true;
    //            ssh.WriteLine("impact -batch");
    //            tmp = ssh.Expect(">");

    //            ssh.WriteLine("cleancablelock");

    //            tmp = ssh.Expect(">");

    //            Console.WriteLine("iMPACT Cable Locks Removed!");
    //            ssh.WriteLine("exit");
    //            ssh.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //    }
    //}
}
