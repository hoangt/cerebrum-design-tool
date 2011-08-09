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

///////////////////////////////////////////////
// File Name : FileServices.cs
// Created By: Abdulrahman Abumurad
// Date : 5/25/2010
// Description : this file contains All methods neccessary to copy files from and to remote server
//                  zip and unzip also tar files. login users and log them out. connect to remote server
//                  
// >> (22 Mar 2011) Matthew Cotter: Added static method to recursively delete a directory and its children, aware of issues that may arise from read-only or otherwise inaccessible files 
//                                      such as those caused by SVN management.
// >> (15 Feb 2011) Matthew Cotter: Migrated FileServices class from XPSBuilder to CerebrumSharedClasses where it may be used by all libraries rather than each having its own copy.
///////////////////////////////////////////////



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;
using System.IO;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Defines a set of methods and properties to be used for transferring files and directories from one location to another, both locally and remotely.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/XpsBuilder Specification.pdf">
    /// XPS Builder Documentation</seealso>
    public class FileServices
    {
        /////////////////////////////////////////
        // Class Options and Private variables //
        /////////////////////////////////////////

        /// <summary>
        /// Enum used to define OS version or type,
        /// either for local or remote OS
        /// </summary>
        public enum OSVersion 
        { 
            /// <summary>
            /// Microsoft Windows-based Operating System
            /// </summary>
            WINDOWS, 
            /// <summary>
            /// Linux-based Operating System
            /// </summary>
            LINUX
        };
        /// <summary>
        /// processor type used in either power PC or MicroBlaze
        /// </summary>
        public enum ProcessorType 
        { 
            /// <summary>
            /// PowerPC processor
            /// </summary>
            PPC = 1, 
            /// <summary>
            /// MicroBlaze processor
            /// </summary>
            MB = 2 
        };
        /// <summary>
        /// read and write from ssh connection
        /// </summary>
        public SshStream sshStream;
        /// <summary>
        /// used to execute linux commands remotely
        /// using ssh connection
        /// </summary>
        public SshExec execShell;
        /// <summary>
        /// used to start launch xps with a script file provided
        /// to form a complete project.
        /// </summary>
        public SshShell xpsShell;
        /// <summary>
        /// used to copy files from and to remote server
        /// by copying folders and files.
        /// </summary>
        private Scp scpShell;
        private bool ServerConnected = false;
        /// <summary>
        /// used as a flag to determine if the 
        /// remote Programming server is connected.
        /// </summary>
        public bool isServerConnected
        {
            get { return ServerConnected; }
        }
        private bool _isLocalSynthServer = true;
        /// <summary>
        /// to determine if the server
        /// on the local machine on a remote machine
        /// </summary>
        public bool isLocalSynthServer
        {
            set
            {
                _isLocalSynthServer = value;
            }
            get
            {
                return _isLocalSynthServer;
            }
        }
        private bool _createFolder_ifNotExist = false;
        /// <summary>
        /// takes user permission to create a Directory
        /// if not exist locally or Remotly.
        /// used with Set_local_Dir() and Set_remote_Dir()
        /// methods
        /// </summary>
        public bool createFolder_ifNotExist
        {
            get
            {
                return _createFolder_ifNotExist;
            }
            set
            {
                _createFolder_ifNotExist = value;
            }
        }
        private bool _overWriteFiles = false;
        /// <summary>
        /// take permission to over write files
        /// when copy_File_TO() amd Copy_File_From()
        /// </summary>
        public bool overWriteFiles
        {
            get
            {
                return _overWriteFiles;
            }
            set
            {
                _overWriteFiles = value;
            }
        }
        private string _Username = "";
        /// <summary>
        /// Class Data memeber to hold user's name
        /// used when attempt to connect to remote server.
        /// </summary>
        public string Username
        {
            set
            {
                _Username = value;
            }
            get
            {
                return _Username;
            }
        }
        private string _UserPass;
        /// <summary>
        /// read-only.  set user password
        /// </summary>
        public string UserPass
        {
            set
            {
                _UserPass = value;
            }
        }
        private string _Servername;
        /// <summary>
        /// holds Servers name to connect to
        /// </summary>
        public string Servername
        {
            get
            {
                return _Servername;
            }
            set
            {
                _Servername = value;
            }
        }
        /// <summary>
        /// holds a path to the specified local directory
        /// </summary>
        public string localDir = "";
        /// <summary>
        /// holds a path to the specified remote Directory.
        /// </summary>
        public string remoteDir = "";
        private OSVersion _localOSType;
        /// <summary>
        /// if local OS type is
        /// windows or linux
        /// </summary>
        public OSVersion LocalOSType
        {
            get
            {
                return _localOSType;
            }
            set
            {
                _localOSType = value;
            }
        }
        private OSVersion _remoteOSType;
        /// <summary>
        /// if remote OS type is
        /// windows or linux
        /// </summary>
        public OSVersion remoteOSType
        {
            get
            {
                return _remoteOSType;
            }
            set
            {
                _remoteOSType = value;
            }

        }

        /// <summary>
        /// Event fired when a message has been generated by the library
        /// </summary>
        public event MessageEventDelegate MessageEvent;
        /// <summary>
        /// Raises an event indicating that a message has been generated.
        /// </summary>
        /// <param name="Message">The message to be transmitted</param>
        /// <param name="Args">List of replacements for token placeholders in the message.</param>
        public void RaiseMessageEvent(string Message, params object[] Args)
        {
            string outputText = Message;
            for (int i = 0; i < Args.Length; i++)
            {
                if (Args[i] != null)
                {
                    outputText = outputText.Replace("{" + i.ToString() + "}", Args[i].ToString());
                }
                else
                {
                    outputText = outputText.Replace("{" + i.ToString() + "}", string.Empty);
                }
            }
            if (MessageEvent != null)
            {
                MessageEvent(outputText);
            }
            else
            {
                Console.WriteLine(outputText);
            }
        }
        
        ///////////////////
        // Class Methods //
        ///////////////////

        /// <summary>
        /// log in to remote server
        /// establish SSH connections
        /// </summary>
        public void login()
        {
            if (_Username == "" || _UserPass == "" || _Servername == "")
            {
                RaiseMessageEvent("either one of the following attributes is not set, UserName, Userpassword or servername");
                return;
            }

            sshStream = new SshStream(_Servername, _Username, _UserPass);
            execShell = new SshExec(_Servername, _Username, _UserPass);
            execShell.Connect();
            xpsShell = new SshShell(_Servername, _Username, _UserPass);
            xpsShell.Connect();
            scpShell = new Scp(_Servername, _Username, _UserPass);
            scpShell.Connect();
            ServerConnected = true;

            if (!execShell.Connected)
            {
                RaiseMessageEvent("Failed to login! check username, password and Server name");
            }


        }

        /// <summary>
        /// release all connections and logout remote server
        /// </summary>
        public void logout()
        {
            try 
            {
                execShell.Close();
            }
            catch {}
            try 
            {
                sshStream.Close();
            }
            catch {}
            try 
            {
                xpsShell.Close();
            }
            catch {}
            try
            {
                scpShell.Close();
            }
            catch { }
            ServerConnected = false;
        }

        /// <summary>
        /// Executes the specified command on the remote system and returns the string result.
        /// </summary>
        /// <param name="Command">The command to be executed.</param>
        /// <returns>The output of running the specified command on the remote system.</returns>
        public string RunCommand(string Command)
        {
            if (!ServerConnected)
            {
                RaiseMessageEvent("remote Server is not connected, Server should beconnected to run commands.");
                return null;
            }
            return execShell.RunCommand(Command);
        }
        /// <summary>
        /// Sets local directory that will be used to copy from or
        /// to it. project dorectory
        /// </summary>
        /// <param name="localPath"> local Directory Path, passed by User</param>
        /// <returns>returns true if it is successful</returns>
        public bool Set_Local_Dir(string localPath)
        {
            if (!System.IO.Directory.Exists(localPath))
            {
                RaiseMessageEvent("Selected Directory is not exist: {0}", localPath);
                return false;
            }
            if (_localOSType == OSVersion.WINDOWS)
            {
                if (localPath.Contains("/"))
                {
                    RaiseMessageEvent("illegal Windows directory Path provided: {0}", localPath);
                    return false;
                }

                else
                {
                    localDir = localPath;
                    return true;
                }
            }
            else if (_localOSType == OSVersion.LINUX)
            {
                if (localPath.Contains("\\"))
                {
                    RaiseMessageEvent("illegal linux or Unix directory Path provided: {0}", localPath);
                    return false;
                }
                else
                {
                    localDir = localPath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets Remote directory that will be used to copy from or
        /// to it. Destination project directory
        /// </summary>
        /// <param name="remotePath"> local Directory Path, passed by User</param>
        /// <returns>returns true if it is successful</returns>
        public bool Set_Remote_Dir(string remotePath)
        {
            if (_isLocalSynthServer)
            {
                RaiseMessageEvent("Local Synthesis Server does not require remote Directory: {0}", remotePath);
                return false;
            }
            if (!ServerConnected)
            {
                RaiseMessageEvent("remote Server is not connected, Server should beconnected to verify remote Directory: {0}", remotePath);
                return false;
            }
            if (remotePath != "")
            {
                string resp = execShell.RunCommand("ls " + remotePath);
                if (resp.Contains("No such file or directory"))
                {
                    RaiseMessageEvent("Selected Remote Directory is not exist: {0}", remotePath);
                    return false;
                }
            }
            if (_remoteOSType == OSVersion.WINDOWS)
            {
                if (remotePath.Contains("/"))
                {
                    RaiseMessageEvent("illegal Windows directory Path provided: {0}", remotePath);
                    return false;
                }
                else
                {
                    remoteDir = remotePath;
                    return true;
                }
            }
            else if (_remoteOSType == OSVersion.LINUX)
            {
                if (remotePath.Contains("\\"))
                {
                    RaiseMessageEvent("illegal linux or Unix directory Path provided: {0}", remotePath);
                    return false;
                }
                else
                {
                    remoteDir = remotePath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// copy a file from local Directory set in LocalDir variable
        /// to Remote Directory found in RemoteDir variable
        /// if localDir and/or remoteDir not set throws exception
        /// if local Directory is not exist throws exception
        /// if file is not present in local Directory throes exception
        /// </summary>
        /// <param name="fileName"> file name to be copied to remote directory</param>
        /// <returns>returns true if copy operation is done successfuly else returns false</returns>
        public bool Copy_File_To(string fileName)
        {
            if (_isLocalSynthServer)
            {
                RaiseMessageEvent("Copy_File_to() command connect be used locally: {0}", fileName);
                return false;
            }
            if (!ServerConnected)
            {
                RaiseMessageEvent("Remote server is not connected, Please login() command: {0}", fileName);
                return false;
            }
            if (localDir == "" || remoteDir == "")
            {
                RaiseMessageEvent("either Local Directory or Remote Directory is not set. use sld or srd commands: {0}", fileName);
                return false;
            }
            if (!System.IO.Directory.Exists(localDir))
            {
                RaiseMessageEvent("Given Path is not exist, please check local Directory before usage: {0}", fileName);
                return false;
            }
            if (!_overWriteFiles)
            {
                string resp = execShell.RunCommand("ls " + remoteDir + "/" + fileName);
                if (resp.Replace("\n", "") == remoteDir + "/" + fileName)
                {
                    RaiseMessageEvent(fileName + " Already exist, OverWrite OPtion is set to False. to overWirte desitnation file set _overWriteFiles = true");
                    return false;
                }
            }
            
            
            bool found = false;
            string[] dirFiles = System.IO.Directory.GetFiles(localDir);
            foreach (string file in dirFiles)
            {
                string tempFile = file.Replace(localDir + "\\", "");
                if (tempFile == fileName)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                if (_remoteOSType == OSVersion.WINDOWS)
                    scpShell.To(localDir + "\\" + fileName, remoteDir + "\\" + fileName);
                else if (_remoteOSType == OSVersion.LINUX)
                    scpShell.To(localDir + "\\" + fileName, remoteDir + "/" + fileName);

                return true;
            }
            else
            {
                RaiseMessageEvent("File name is not found in local Directory provided");
                return false;
            }
        }

        /// <summary>
        /// move the whole directory from local machine to remote machine
        /// check if both localDir and remoteDir varable are set.
        /// check is local directory provided is exit. 
        /// </summary>
        /// <returns>true if copy is done successfully else returns false</returns>
        public bool Copy_Dir_To()
        {
            if (_isLocalSynthServer)
            {
                RaiseMessageEvent("Copy_Dir_to() command connect be used locally");
                return false;
            }
            if (!ServerConnected)
            {
                RaiseMessageEvent("Remote server is not connected, Please login() command");
                return false;
            }

            if (localDir == "" || remoteDir == "")
            {
                RaiseMessageEvent("either Local Directory or Remote Directory is not set. use set_local_dir() or set_remote_dir() commands");
                return false;
            }
            else if (!_createFolder_ifNotExist)
            {
                // check if local Directory exist or not
                if (!System.IO.Directory.Exists(localDir))
                {
                    RaiseMessageEvent("Local Directory is not exist, please use set_local_dir() command to provide a valid directory path,\n or use createFolder_ifNotExist option to create it if not exist?");
                    return false;
                }
                // check the remote directory existance
                if (!_isLocalSynthServer)
                {
                    string resp = execShell.RunCommand("ls " + remoteDir);
                    if (resp == "")
                    {
                        RaiseMessageEvent("Remote Directory is not exist, please use set_remote_dir() command to provide a valid directory path,\n or use createFolder_ifNotExist option to create it if not exist?");
                        return false;
                    }
                }
            }

            string lastDir;
            if (remoteOSType == OSVersion.LINUX)
                lastDir = remoteDir.Substring(remoteDir.LastIndexOf("/") + 1);
            else
                lastDir = remoteDir.Substring(remoteDir.LastIndexOf("\\") + 1);
            scpShell.To(localDir, remoteDir, true);

            if (remoteOSType == OSVersion.LINUX)
            {
                execShell.RunCommand(String.Format("cd {0}; mv -f ./{1}/* .; rm -rf {1}", remoteDir, lastDir));
            }
            else
            {
                execShell.RunCommand(String.Format("cd {0}", remoteDir, lastDir));
                execShell.RunCommand(String.Format("move /Y {1} ..", remoteDir, lastDir));
            }
            return true;
        }

        /// <summary>
        /// copy a file from remote Directory set in remoteDir variable
        /// to local Directory found in localDir variable
        /// if localDir and/or remoteDir not set throws exception
        /// if local Directory is not exist throws exception
        /// if file is not present in remote Directory throws exception
        /// </summary>
        /// <param name="fileName"> file name to be copied to remote directory</param>
        /// <returns>returns true if copy operation is done successfuly else returns false</returns>
        public bool Copy_File_From(string fileName)
        {
            if (_isLocalSynthServer)
            {
                RaiseMessageEvent("Copy_File_to() command connect be used locally: {0}", fileName);
                return false;
            }
            if (!ServerConnected)
            {
                RaiseMessageEvent("Remote server is not connected, Please login() command: {0}", fileName);
                return false;
            }
            if (localDir == "" || remoteDir == "")
            {
                RaiseMessageEvent("either Local Directory or Remote Directory is not set. use sld or srd commands: {0}", fileName);
                return false;
            }
            if (!System.IO.Directory.Exists(localDir))
            {
                RaiseMessageEvent("Given local Path is not exist, please check local Directory before usage: {0}", fileName);
                return false;
            }
            if (!_overWriteFiles)
            {
                if (System.IO.File.Exists(localDir + "\\" + fileName))
                {
                    RaiseMessageEvent(fileName + " Already exist, OverWrite OPtion is set to False. to overWirte desitnation file set _overWriteFiles = true");
                    return false;
                }
            }
            if (_remoteOSType == OSVersion.LINUX)
            {
                string FullFilePath = remoteDir + "/" + fileName;
                string resp = execShell.RunCommand("ls " + FullFilePath);
                if (!(resp.Replace("\n", "") == FullFilePath))
                {
                    RaiseMessageEvent("Given remote Path is not exist, please check local Directory before usage");
                    return false;
                }
            }
           if (_remoteOSType == OSVersion.WINDOWS)
            {
                // for future inplementation not for testing
            }
            
            bool found = false;
            string Response = execShell.RunCommand("cd " + remoteDir);
            Response = execShell.RunCommand("ls " + remoteDir + "/" + fileName);
            if (Response.Contains(fileName))
            {
                found = true;
            }
            if (found)
            {
                if (_remoteOSType == OSVersion.WINDOWS)
                    scpShell.From(remoteDir + "\\" + fileName, localDir + "\\" + fileName);
                else if (_remoteOSType == OSVersion.LINUX)
                    scpShell.From(remoteDir + "/" + fileName, localDir + "\\" + fileName);

                return true;
            }
            else
            {
                RaiseMessageEvent("File name is not found in local Directory provided");
                return false;
            }
        }

        /// <summary>
        /// move the whole directory from remote machine to local machine
        /// check if both localDir and remoteDir varable are set.
        /// check is local directory provided is exit. 
        /// </summary>
        /// <returns>true if copy is done successfully else returns false</returns>
        public bool Copy_Dir_From()
        {
            if (_isLocalSynthServer)
            {
                RaiseMessageEvent("Copy_File_to() command connect be used locally");
                return false;
            }
            if (!ServerConnected)
            {
                RaiseMessageEvent("Remote server is not connected, Please login() command");
                return false;
            }
            if (localDir == "" || remoteDir == "")
            {
                RaiseMessageEvent("either Local Directory or Remote Directory is not set. use sld or srd commands");
                return false;
            }
            if (!_createFolder_ifNotExist)
            {
                // check if local Directory exit or not
                if (System.IO.Directory.Exists(localDir))
                {
                    RaiseMessageEvent("Local Directory is not exist, please use set_local_dir() command to provide a valid directory path,\n or use createFolder_ifNotExist option to create it if not exist?");
                    return false;
                }
                // check the remote directory existance
                if (!_isLocalSynthServer)
                {
                    string resp = execShell.RunCommand("ls " + remoteDir);
                    if (resp.Contains("ls:") || resp.Contains(" No such file or directory"))
                    {
                        RaiseMessageEvent("Remote Directory is not exist, please use set_remote_dir() command to provide a valid directory path,\n or use createFolder_ifNotExist option to create it if not exist?");
                        return false;
                    }
                }
            }
          

            if (_remoteOSType == OSVersion.WINDOWS && remoteDir.Contains("\\"))
                scpShell.From(remoteDir, localDir, true);
            else if (_remoteOSType == OSVersion.LINUX && remoteDir.Contains("/"))
                scpShell.From(remoteDir, localDir, true);

            return true;

           
        }

        /// <summary>
        /// method is used locally to compress folders before transfer them
        /// to Remote Synthesis Server. to save transfer time.
        /// </summary>
        /// <param name="FolderName">Folder Name that desired to be compressed</param>
        /// <param name="TaredFileName">Compressed file name to be generated and saved into local directory</param>
        /// <returns>true if compression is successful otherwise returns false</returns>
        public bool CompressFolder(string FolderName, string TaredFileName)
        {
            // check that local Directory is set to a value
            if(localDir == "")
            {
                RaiseMessageEvent("Local Directory is not set to a value, use Set_local_Directory() mothod.");
                return false;
            }
            // make  sure the folder desired to be compressed already exist in the local directory.
            if (!System.IO.Directory.Exists(localDir+"\\"+FolderName))
            {
                RaiseMessageEvent("Folder name provided is not exit in the local directory, please check folder name then call this method again");
                return false;
            }
            if(TaredFileName == "")
            {
                RaiseMessageEvent("TaredFileName is not set a value");
                return false;
            }
            //create process info to start xbash in a local process to perform the task
            System.Diagnostics.ProcessStartInfo p_info = new System.Diagnostics.ProcessStartInfo("xbash");
            p_info.RedirectStandardInput = true;
            p_info.RedirectStandardOutput = true;
            p_info.RedirectStandardError = true;
            p_info.CreateNoWindow = true;
            p_info.UseShellExecute = false;

            //create local process object
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = p_info; 
            p.Start();

            // change the directory to local one.
            p.StandardInput.WriteLine("cd " + localDir);
            p.StandardInput.WriteLine("\n");

            // compress a file and save as TaredFileName.tgz
            p.StandardInput.WriteLine("tar -czf " + TaredFileName + ".tgz " + FolderName);

            return true; 
        }

        /// <summary>
        /// decompress given Compressed file name FileName parameter, check if FileName in a local synthesis server or on remote Server
        /// then Decompress the file.FileName should be in remote Directory if remote server is used or it should be in local Directory si local
        /// synthesis Server is used.
        /// </summary>
        /// <param name="FileName">Compressed file to br decompressed</param>
        /// <returns>true if decompressions is successful</returns>
        public bool DecompressFile(string FileName)
        {
            // decompress a file either locally or remotly
            if (isLocalSynthServer)
            {
                // check that local Directory is set to a value
                if (localDir == "")
                {
                    RaiseMessageEvent("Local Directory is not set to a value, use Set_local_Directory() mothod.");
                    return false;
                }
                // make sure the given file name is exist.
                if (!System.IO.File.Exists(localDir + "\\" + FileName))
                {
                    RaiseMessageEvent("file name provided is not exit in the local directory, please check folder name then call this method again");
                    return false;
                }

                System.Diagnostics.ProcessStartInfo p_info = new System.Diagnostics.ProcessStartInfo("xbash");
                p_info.RedirectStandardError = true;
                p_info.RedirectStandardInput = true;
                p_info.RedirectStandardOutput = true;
                p_info.CreateNoWindow = true;
                p_info.UseShellExecute = false;

                // create a local process to handle decompression

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = p_info;
                p.Start();

                // go to local directory
                p.StandardInput.WriteLine("cd " + localDir);
                // decompress file
                p.StandardInput.WriteLine("tar -xzf " + FileName);
            }
            else
            {
                if (ServerConnected)
                {
                    if (remoteDir == "")
                    {
                        RaiseMessageEvent("Remote Directory is not set to value, Use Set_Remote_Dir() method");
                        return false;
                    }
                    // make sure the remote directory contains the specified file
                    string resp = execShell.RunCommand("cd "+remoteDir);
                    resp = execShell.RunCommand("ls " + FileName);
                    if (resp == FileName)
                    {
                        resp = execShell.RunCommand("tar -xzf " + FileName);
                        if (!(resp == ""))
                        {
                            RaiseMessageEvent("Decompression Operation Failed");
                            return false;
                        }
                    }
                    else
                    {
                        RaiseMessageEvent("The specified file name is not exist on the remote directory folder, use copy method to move desired files");
                        return false;
                    }
                }
                else
                {
                    RaiseMessageEvent("Synthesis Server should be Specified to a Local Machine or to a Remote Machine");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// copy a file from source localtion to destination location
        /// </summary>
        /// <param name="srcFile">Source file name and path</param>
        /// <param name="DestFile">Destination file name and path</param>
        /// <returns>true if copying is successful</returns>
        public bool Local_File_Copy(string srcFile, string DestFile)
        {
            try
            {
                if (!System.IO.File.Exists(srcFile))
                {
                    RaiseMessageEvent("srcFile is not exist in the specified Location: {0}", srcFile);
                    return false;
                }
                string tempDest = DestFile.Substring(0, DestFile.LastIndexOf('\\') + 1);
                if (!System.IO.Directory.Exists(tempDest))
                {
                    RaiseMessageEvent("DestFile is not exist in the specified Location: {0}", DestFile);
                    return false;
                }
                System.IO.File.Copy(srcFile, DestFile, _overWriteFiles);
            }
            catch
            {
                RaiseMessageEvent("Unable to copy specified file to new location");
                return false;
            }
            return true;
        }

        /// <summary>
        /// copy a file from source localtion to destination location
        /// </summary>
        /// <param name="srcDir">Source Directory name and path to be moved to Dest Directory</param>
        /// <param name="DestDir">Destination Directory name and path</param>
        /// <returns>true if copying is successful</returns>
        public bool Local_Directory_Copy(string srcDir, string DestDir)
        {
            try
            {
                if (!System.IO.Directory.Exists(srcDir))
                {
                    RaiseMessageEvent("srcDir is not exist in the specified Path: {0}", srcDir);
                    return false;
                }

                if (!System.IO.Directory.Exists(DestDir))
                {
                    if (createFolder_ifNotExist)
                    {
                        Directory.CreateDirectory(DestDir);
                    }
                    else
                    {
                        RaiseMessageEvent("DestDir is not exist in the specified Path: {0}", DestDir);
                        return false;
                    }
                }
                DirectoryInfo src = new DirectoryInfo(srcDir);
                DirectoryInfo dest = new DirectoryInfo(DestDir);
                CopyAll(src, dest);
            }
            catch(Exception exp)
            {
                RaiseMessageEvent("Unable to copy specified Directory to new the location");
                RaiseMessageEvent(exp.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This pribate method is used by Local_Dorectory_copy() method to copy from src to
        /// dest. if the destination directory existed then it will overwrite it else it will create the destination
        /// folder.
        /// </summary>
        /// <param name="source">Source Folder to be copied DorectoryInfo type</param>
        /// <param name="target">Destination folder to copy to. DirectoryInfo type</param>
        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Skip SVN directories
            if (String.Compare(source.Name, ".svn", true) == 0)
                return;
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        /// <summary>
        /// Performs a recursive purge of the directory, attempting to delete ALL files and subdirectories that can be deleted.
        /// </summary>
        /// <param name="Directory">The path to the directory to be deleted.</param>
        public static void CleanDirectory(string Directory)
        {
            CleanDirectory(new DirectoryInfo(Directory));
        }

        /// <summary>
        /// Performs a recursive purge of the directory, attempting to delete ALL files and subdirectories that can be deleted.
        /// </summary>
        /// <param name="Directory">The DirectoryInfo of the directory to be deleted.</param>
        public static void CleanDirectory(DirectoryInfo Directory)
        {
            if (Directory.Exists)
            {
                foreach (DirectoryInfo DI in Directory.GetDirectories())
                {
                    try
                    {
                        CleanDirectory(DI);
                    }
                    catch { }
                }
                foreach (FileInfo FI in Directory.GetFiles())
                {

                    try
                    {
                        FI.Delete();
                    }
                    catch { }
                }
                try
                {
                    Directory.Delete();
                }
                catch { }
            }
        }
    }
}
