/************************************************************************************************************
 * FalconCoerServerCompiler\Program.cs
 * Name: Matthew Cotter
 * Date: 05 Jul 2010 
 * Description: This library implements the required functionality to compile the Cerebrum Core Server Applications and Linux Drivers for testing and interaction.
 * Notes:
 * History: 
 * >> (26 Aug 2010) Matthew Cotter: Corrected a bug that was placing the "bootup.sh" script in the wrong location.
 * >> (24 Aug 2010) Matthew Cotter: Major overhaul to internal code to support compilation against multiple processors/architectures.
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
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
using Tamir;
using Tamir.SharpSsh;

namespace FalconCoreServerCompiler
{
    /// <summary>
    /// Class library to generate C source headers and compile core applications and drivers as required for all cores in the system.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/CoreServerCompiler Specification.pdf">
    /// Core Server Compiler Documentation</seealso>
    public class CoreServerCompiler : IFalconLibrary
    {
        private class CoreServerParameters
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string FPGA { get; set; }
            public string IP { get; set; }
            public string MAC { get; set; }
            public int Port { get; set; }
            public string BaseAddress { get; set; }
            public string HighAddress { get; set; }
            public string AddressSize { get; set; }
            public bool CompilePowerPC { get; set; }
            public bool CompileMicroblaze { get; set; }

            public CoreServerParameters()
            {
                this.ID = string.Empty;
                this.Name = string.Empty;
                this.FPGA = string.Empty;
                this.IP = string.Empty;
                this.MAC = string.Empty;
                this.Port = 0;
                this.BaseAddress = string.Empty;
                this.HighAddress = string.Empty;
                this.AddressSize = string.Empty;
                this.CompilePowerPC = true;
                this.CompileMicroblaze = false;                
            }
        }
        private class CoreProcessorInfo : IComparable
        {
            public string Instance { get; set; }
            public string FPGA { get; set; }
            public string Type { get; set; }
            public string LinuxSource { get; set; }
            public string CerebrumProcessorID { get; set; }
            public int CPUNumber { get; set; }
            public CoreProcessorInfo()
            {
                this.Instance = string.Empty;
                this.FPGA = string.Empty;
                this.Type = string.Empty;
                this.LinuxSource = string.Empty;
                this.CerebrumProcessorID = string.Empty;
            }

            public int CompareTo(object o)
            {
                const int THIS_BEFORE_OTHER = -1;
                const int THIS_SAME_ORDER = 0;
                //const int THIS_AFTER_OTHER = 1;

                if (o.GetType() != typeof(CoreProcessorInfo))
                    return THIS_BEFORE_OTHER;
                if (this == o)
                    return THIS_SAME_ORDER;

                CoreProcessorInfo ThisCPI = this;
                CoreProcessorInfo OtherCPI = (CoreProcessorInfo)o;

                if (ThisCPI.Type.ToLower() != OtherCPI.Type.ToLower())
                    return String.Compare(ThisCPI.Type, OtherCPI.Type, true);
                else if (ThisCPI.LinuxSource != OtherCPI.LinuxSource)
                {
                    return String.Compare(ThisCPI.LinuxSource, OtherCPI.LinuxSource, false);
                }
                else if (ThisCPI.FPGA != OtherCPI.FPGA)
                {
                    return String.Compare(ThisCPI.FPGA, OtherCPI.FPGA, false);
                }
                else
                {
                    return String.Compare(ThisCPI.Instance, OtherCPI.Instance, false);
                }
            }
        }
        /// <summary>
        /// Static function to convert a string formatted as a binary number (0b00000000) to a long int.
        /// </summary>
        /// <param name="BinaryNumber">The binary number in string format to be converted.</param>
        /// <returns>A long int value indicating the value of the binary string, if conversion was successful.</returns>
        public static long BinToLong(string BinaryNumber)
        {
            long val = 0;
            int power = 0;
            BinaryNumber = BinaryNumber.ToUpper();
            if (BinaryNumber.StartsWith("0B"))
                BinaryNumber = BinaryNumber.Substring(2);
            while (BinaryNumber.Length > 0)
            {
                int digitValue = 0;
                string digit = BinaryNumber.Substring(BinaryNumber.Length - 1);
                char cDigit = Char.Parse(digit.ToUpper());
                if (Char.IsDigit(cDigit))
                {
                    digitValue = (cDigit - '0');
                    if (digitValue > 1)
                        return 0;
                }
                else
                {
                    return 0;
                }
                val += (digitValue * (long)(Math.Pow(2, power)));
                BinaryNumber = BinaryNumber.Substring(0, BinaryNumber.Length - 1);
                power++;
            }
            return val;
        }
        /// <summary>
        /// Static function to convert a string formatted as a Hexadecimal number (0x0ff0Ac44) to a long int.
        /// </summary>
        /// <param name="HexNumber">The Hex number in string format to be converted.</param>
        /// <returns>A long int value indicating the value of the hexadecimal string, if conversion was successful.</returns>
        public static long HexToLong(string HexNumber)
        {
            HexNumber = HexNumber.ToUpper();
            if ((HexNumber.StartsWith("0X")) || (HexNumber.StartsWith("0H")))
                HexNumber = HexNumber.Substring(2);
            long val = 0;
            int power = 0;
            while (HexNumber.Length > 0)
            {
                int digitValue = 0;
                string digit = HexNumber.Substring(HexNumber.Length - 1);
                char cDigit = Char.Parse(digit.ToUpper());
                if (Char.IsDigit(cDigit))
                {
                    digitValue = (cDigit - '0');
                }
                else if (Char.IsLetter(cDigit))
                {
                    digitValue = (cDigit - 'A') + 1;
                    if (digitValue > 6)
                        return 0;
                    else
                        digitValue += 9;
                }
                val += (digitValue * (long)(Math.Pow(16, power)));
                HexNumber = HexNumber.Substring(0, HexNumber.Length - 1);
                power++;
            }
            return val;
        }
        /// <summary>
        /// Static function to convert a long int to a Hexadecimal string (0x000F000F).
        /// </summary>
        /// <param name="LongNumber">The long int to be converted.</param>
        /// <param name="MinDigits">The minimum number of Hex digits to be included in the result, accomplished with zero-padding.</param>
        /// <returns>A hexadecimal string representation of the long integer that was converted.</returns>
        public static string LongToHex(long LongNumber, int MinDigits)
        {
            string val = string.Empty;
            while (LongNumber > 0)
            {
                long remainder = LongNumber % 16;
                char digit;
                if (remainder >= 9)
                    digit = Convert.ToChar(Convert.ToInt64('A') + (remainder - 10));
                else
                    digit = Convert.ToChar(Convert.ToInt64('0') + remainder);
                val = digit + val;
                LongNumber = LongNumber / 16;
            }
            return "0x" + val.ToUpper().PadLeft(MinDigits, '0');
        }
        /// <summary>
        /// Static function to convert a long int to a binary string (0x00010001).
        /// </summary>
        /// <param name="LongNumber">The long int to be converted.</param>
        /// <param name="MinDigits">The minimum number of binary digits to be included in the result, accomplished with zero-padding.</param>
        /// <returns>A binary string representation of the long integer that was converted.</returns>
        public static string LongToBin(long LongNumber, int MinDigits)
        {
            string val = string.Empty;
            while (LongNumber > 0)
            {
                long remainder = LongNumber % 2;
                char digit;
                digit = Convert.ToChar(Convert.ToInt64('0') + remainder);
                val = digit + val;
                LongNumber = LongNumber / 2;
            }
            return "0b" + val.PadLeft(MinDigits, '0');
        }

        private FalconServer _Server;
        private LinkedList<FalconServer> _Servers;
        private PathManager _PathMan = null;
        private List<CoreServerParameters> _Cores = null;
        private List<CoreProcessorInfo> _Procs = null;
        private SshExec _SSHExec = null;
        private Scp _SSHXFer = null;

        private int MajorNum = 61;

        /// <summary>
        /// Load the project paths from the file specified.
        /// </summary>
        /// <param name="PathFile">The XML file containing all project paths.</param>
        /// <returns>True if loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Project Paths)</seealso>
        public bool LoadPaths(string PathFile)
        {
            try
            {
                _PathMan = new PathManager(PathFile);
                Console.WriteLine("Loaded Paths from: {0}", PathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Loads the component->FPGA address map containing information about each core that must be compiled into the source headers.
        /// </summary>
        /// <returns>True if loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Address Map)</seealso>
        public bool LoadAddressMap()
        {
            try
            {
                string AddressMapFile = _PathMan.GetPath("AddressMap");
                if (!File.Exists(AddressMapFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(AddressMapFile);
                Console.WriteLine("Loaded AddressMap from: {0}", AddressMapFile);
                _Cores = new List<CoreServerParameters>();
                _Cores.Clear();

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "addressmap")
                    {
                        foreach (XmlNode xCoreNode in xElem.ChildNodes)
                        {
                            if (xCoreNode.Name.ToLower() == "component")
                            {
                                CoreServerParameters csp = new CoreServerParameters();
                                foreach (XmlAttribute xAttr in xCoreNode.Attributes)
                                {
                                    if (xAttr.Name.ToLower() == "id")
                                    {
                                        csp.ID = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "name")
                                    {
                                        csp.Name = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "ip")
                                    {
                                        csp.IP = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "mac")
                                    {
                                        csp.MAC = xAttr.Value.ToLower().Trim().Replace("-", ":").Replace(" ", ":");
                                    }
                                    else if (xAttr.Name.ToLower() == "port")
                                    {
                                        int val = 0;
                                        if (int.TryParse(xAttr.Value, out val))
                                            csp.Port = val;
                                    }
                                    else if (xAttr.Name.ToLower() == "fpga")
                                    {
                                        csp.FPGA = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "baseaddr")
                                    {
                                        csp.BaseAddress = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "highaddr")
                                    {
                                        csp.HighAddress = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "addrsize")
                                    {
                                        csp.AddressSize = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "targets")
                                    {
                                        csp.CompilePowerPC = false;
                                        csp.CompileMicroblaze = false;
                                        string targets = xAttr.Value.ToLower();
                                        if ((targets.Contains("powerpc")) || (targets.Contains("ppc")))
                                        {
                                            csp.CompilePowerPC = true;
                                        }
                                        if ((targets.Contains("microblaze")) || (targets.Contains("mb")))
                                        {
                                            csp.CompileMicroblaze = true;
                                        }
                                    }
                                }
                                if ((csp.ID != string.Empty) && (csp.FPGA != string.Empty) && (csp.Name != string.Empty))
                                {
                                    _Cores.Add(csp);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Loads the remote servers on which the compilation will take place.  All work is done on a single server.   The first server to successfully connect is used.
        /// </summary>
        /// <param name="ServerFile">The XML file containing the servers available for compilation.</param>
        /// <returns>True if loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Servers)</seealso>
        public bool LoadServersFile(string ServerFile)
        {
            try
            {
                if (!File.Exists(ServerFile))
                    ServerFile = _PathMan.GetPath("LocalProjectRoot") + "\\" + ServerFile;
                if (!File.Exists(ServerFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(ServerFile);
                Console.WriteLine("Loaded Servers from: {0}", ServerFile);
                _Servers = new LinkedList<FalconServer>();
                _Servers.Clear();

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "servers")
                    {
                        foreach (XmlNode xEServer in xElem.ChildNodes)
                        {
                            if (xEServer.Name.ToLower() == "server")
                            {
                                FalconServer fs = new FalconServer();
                                fs.ParseServerNode(xEServer);
                                _Servers.AddLast(fs);
                                break;
                            }
                        }
                    }
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads an XML file specifying the design.
        /// </summary>
        /// <param name="DesignFile">The path to the XML file specifying the design.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Design)</seealso>
        public bool LoadDesignFile(string DesignFile)
        {
            try
            {
                if (!File.Exists(DesignFile))
                    DesignFile = _PathMan.GetPath("LocalProjectRoot") + "\\" + DesignFile;
                if (!File.Exists(DesignFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(DesignFile);
                _Procs = new List<CoreProcessorInfo>();

                Console.WriteLine("Loaded Design from: {0}", DesignFile);
                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "design")
                    {
                        foreach (XmlNode xDesNode in xElem.ChildNodes)
                        {
                            if (xDesNode.Name.ToLower() == "processors")
                            {
                                foreach (XmlNode xProcNode in xDesNode.ChildNodes)
                                {
                                    if (xProcNode.Name.ToLower() == "processor")
                                    {
                                        string deviceID = string.Empty;
                                        CoreProcessorInfo CPI = new CoreProcessorInfo();
                                        foreach (XmlAttribute xAttr in xProcNode.Attributes)
                                        {
                                            if (xAttr.Name.ToLower() == "ondevice")
                                            {
                                                CPI.FPGA = xAttr.Value;
                                            }
                                        }
                                        foreach (XmlNode xProcProp in xProcNode.ChildNodes)
                                        {
                                            if (xProcProp.Name.ToLower() == "instance")
                                            {
                                                CPI.Instance = xProcProp.InnerText;
                                            }
                                            else if (xProcProp.Name.ToLower() == "type")
                                            {
                                                CPI.Type = xProcProp.InnerText;
                                            }
                                            else if (xProcProp.Name.ToLower() == "linuxkernelsource")
                                            {
                                                CPI.LinuxSource = xProcProp.InnerText;
                                            }
                                            else if (xProcProp.Name.ToLower() == "cerebrumid")
                                            {
                                                CPI.CerebrumProcessorID = xProcProp.InnerText;
                                            }
                                            else if (xProcProp.Name.ToLower() == "cpunumber")
                                            {
                                                int val = 0;
                                                if (int.TryParse(xProcProp.InnerText, out val))
                                                {
                                                    CPI.CPUNumber = val;
                                                }
                                            }
                                        }
                                        if (CPI.LinuxSource == string.Empty)
                                        {
                                            Console.WriteLine("\t Processor {0}.{1} using default Linux Kernel Source.", CPI.FPGA, CPI.Instance);
                                            CPI.LinuxSource = _PathMan["LinuxKernelLocation"];
                                        }
                                        else
                                        {
                                            Console.WriteLine("\t Processor {0}.{1} overriding default Linux Kernel Source.", CPI.FPGA, CPI.Instance);
                                        }
                                        _Procs.Add(CPI);
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CoreServerCompiler.LoadDesignFile().  {0}", ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the password for the server from the Console.  The password characters are not echoed to the console, but replaced by asterisks.
        /// </summary>
        /// <param name="user">The user name to be included in the prompt.</param>
        /// <param name="server">The server address to included in the prompt.</param>
        /// <returns>The password as read in from the Console.</returns>        
        public string ReadPassword(string user, string server)
        {
            Console.Write("{0}@{1}'s password: ", user, server);
            string input = string.Empty;
            string rd;
            rd = Console.ReadKey(true).KeyChar.ToString();
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
                    Console.Write("*");
                }
                rd = Console.ReadKey(true).KeyChar.ToString();
            }
            input = input.Replace("\r", string.Empty);
            input = input.Replace("\n", string.Empty);
            Console.WriteLine();
            return input;
        }
        private bool Login()
        {
            bool bReadPassword = true;
            try
            {
                bReadPassword = (_Server.Password == string.Empty);
            }
            catch
            {
                // Clear password on entry error - required reentry next time.
                if (_Server != null) _Server.Password = string.Empty;
            }

            if (bReadPassword)
            {
                try
                {
                    _Server.Password = ReadPassword(_Server.UserName, _Server.Address);
                }
                catch
                {
                    // Clear password on connection error - required reentry next time.
                    _Server.Password = string.Empty;
                    return false;
                }
            }

            try
            {
                if ((_SSHExec == null) || (!_SSHExec.Connected))
                {
                    _SSHExec = new SshExec(_Server.Address, _Server.UserName, _Server.Password);
                    _SSHExec.Connect(_Server.Port);
                }
                if (!_SSHExec.Connected)
                    return false;
            }
            catch
            {
                // Clear password on connection error - required reentry next time.
                _Server.Password = string.Empty;
                return false;
            }

            try
            {
                if ((_SSHXFer == null) || (!_SSHXFer.Connected))
                {
                    _SSHXFer = new Scp(_Server.Address, _Server.UserName, _Server.Password);
                    _SSHXFer.Connect(_Server.Port);
                }
                if (!_SSHXFer.Connected)
                    return false;
            }
            catch
            {
                _Server.Password = string.Empty;
                return false;
            }
            return true;
        }
        private void Logout()
        {
            try
            {
                if (_SSHExec != null)
                {
                    if (_SSHExec.Connected)
                        _SSHExec.Close();
                    _SSHExec = null;
                }
                if (_SSHXFer != null)
                {
                    if (_SSHXFer.Connected)
                        _SSHXFer.Close();
                    _SSHXFer = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Iterates through all of the cores loaded from the AddressMap and generates their corresponding headers, compiles the application and driver, and inserts the 
        /// commands to load the drivers and applications into a shell script to be run on the FPGA when Linux boots.
        /// </summary>
        /// <returns>True if successful, false if an error occurs.</returns>
        public bool CompileCoreServers()
        {
            try
            {
                #region Connect to Compilation Server
                if (!ConnectToCompileServer())
                {
                    Console.WriteLine("ERROR: Unable to connect to compilation server... Terminating");
                    return false;
                }

                Console.WriteLine("Compiling Core Applications and Drivers...");
                #endregion

                #region Initialization/Constants/Fixed Paths
                string MountPoint = _PathMan.GetPath("LinuxBootMount"); // As logged into the machine exporting the filesystem
                string OnBoard_MountPoint = _PathMan.GetPath("OnBoardMount"); // As mounted on the FPGA

                // Append user name for now
                string MountPath = String.Format("{0}/{1}", MountPoint, _Server.UserName);
                string OnBoard_MountPath = String.Format("{0}/{1}", OnBoard_MountPoint, _Server.UserName);

                string CompileDir = _PathMan.GetPath("CoreServerSource");
                string OutputDir = _PathMan["LocalOutput"];
                if (!Directory.Exists(OutputDir))
                    Directory.CreateDirectory(OutputDir);

                string AppServerName = "AcceleroServer";
                string DrvModuleName = "bb_driver.ko";
                MajorNum = 61;

                Hashtable MACTable = new Hashtable();
                Hashtable ProcScripts = new Hashtable();
                Hashtable ProcTable = new Hashtable();
                #endregion
                
                #region --- Server App and Driver Compilation (Looped through Processors/Cores on FPGAs)
                _Procs.Sort();
                foreach (CoreProcessorInfo cpi in _Procs)
                {
                    foreach (CoreServerParameters csp in _Cores)
                    {
                        if (csp.FPGA == cpi.FPGA)
                        {
                            #region Verify Processor Architecture
                            string ProcArch = string.Empty;
                            if ((csp.CompilePowerPC) &&
                                ((cpi.Type.ToLower() == "powerpc") || (cpi.Type.ToLower() == "ppc")))
                            {
                                ProcArch = "ppc";
                            }
                            else if ((csp.CompileMicroblaze) &&
                                ((cpi.Type.ToLower() == "microblaze") || (cpi.Type.ToLower() == "mb")))
                            {
                                ProcArch = "mb";
                            }
                            else
                            {
                                Console.WriteLine("WARNING: Processor {0} on {1} has an recognized Type: {2}", cpi.Instance, cpi.FPGA, cpi.Type);
                                continue;
                            }
                            Console.Write("\t{0} on {1} {2}...", csp.ID, csp.FPGA, ProcArch);
                            #endregion

                            #region Generate Directories and Paths
                            string[] IDs = csp.FPGA.Split('.');
                            string Board_ID = IDs[0];
                            string FPGA_ID = IDs[1];

                            string BoardFPGAID = String.Format("{0}/{1}", Board_ID, FPGA_ID);
                            string BoardPath = String.Format("{0}/{1}", MountPath, Board_ID);
                            string FPGAPath = String.Format("{0}/{1}", BoardPath, FPGA_ID);
                            string ArchPath = String.Format("{0}/{1}", FPGAPath, ProcArch);
                            string CPUPath = String.Format("{0}/cpu{1}", ArchPath, cpi.CPUNumber);
                            string CorePath = String.Format("{0}/{1}", CPUPath, csp.ID);

                            string Script_BoardPath = String.Format("{0}/{1}", OnBoard_MountPath, Board_ID);
                            string Script_FPGAPath = String.Format("{0}/{1}", Script_BoardPath, FPGA_ID);
                            string Script_ArchPath = String.Format("{0}/{1}", Script_FPGAPath, ProcArch);
                            string Script_CPUPath = String.Format("{0}/cpu{1}", Script_ArchPath, cpi.CPUNumber);
                            string Script_CorePath = String.Format("{0}/{1}", Script_CPUPath, csp.ID);
                            #endregion

                            #region Create Core Directory Structure
                            CreateRemoteDirectory(MountPoint, "777", "falcon");
                            CreateRemoteDirectory(MountPath, "777", "falcon");
                            CreateRemoteDirectory(BoardPath, "777", "falcon");
                            CreateRemoteDirectory(FPGAPath, "777", "falcon");

                            CreateRemoteDirectory(ArchPath, "777", "falcon");
                            CreateRemoteDirectory(CPUPath, "777", "falcon");
                            CreateRemoteDirectory(CorePath, "777", "falcon");
                            #endregion

                            #region Update Scripts and Tables
                            if (!MACTable.ContainsKey(BoardFPGAID))
                            {
                                // Add this FPGA's MAC Address to the table
                                MACTable.Add(BoardFPGAID, csp.MAC.ToLower());
                            }
                            if (!ProcTable.Contains(cpi.CerebrumProcessorID))
                            {
                                // Add this processor's Cerebrum ID to the table
                                ProcTable.Add(cpi.CerebrumProcessorID, String.Format("{0} {1} {2}", cpi.CerebrumProcessorID, cpi.CPUNumber, cpi.Type));
                            }
                            if (!ProcScripts.Contains(cpi.CerebrumProcessorID))
                            {
                                // Create a new List of strings, to hold the required commands to boot the app and drivers
                                ProcScripts.Add(cpi.CerebrumProcessorID, new List<string>());
                            }

                            FileInfo AppHeaderFI;
                            FileInfo DriverHeaderFI;
                            FileInfo MakeFileFI;
                            FileInfo CompileScriptFI;
                            AppHeaderFI = WriteCoreAppHeaderFile(csp, MajorNum);
                            DriverHeaderFI = WriteCoreDriverHeaderFile(csp, MajorNum);
                            MakeFileFI = WriteCoreDriverMakeFile(csp);
                            CompileScriptFI = WriteCoreGlobalCompileScript(csp, ProcArch);

                            CopyFileToRemoteDirectory(AppHeaderFI, String.Format("{0}/apps/", CompileDir), "777", "falcon");
                            CopyFileToRemoteDirectory(DriverHeaderFI, String.Format("{0}/drivers/", CompileDir), "777", "falcon");
                            CopyFileToRemoteDirectory(MakeFileFI, String.Format("{0}/drivers/", CompileDir), "777", "falcon");
                            CopyFileToRemoteDirectory(CompileScriptFI, String.Format("{0}/compile_scripts/", CompileDir), "777", "falcon");
                            #endregion

                            #region Begin execution of scripts for compilation
                            string CompileLogFileName = String.Format("{0}_compile.log", csp.ID);
                            string GlobalCompileLog = String.Format("{0}/{1}", CompileDir, CompileLogFileName);
                            string LocalCompileLog = String.Format("{0}/{1}", OutputDir, CompileLogFileName);
                            string DriverPrepCommand = String.Format("cd {0}/drivers;cp bb_driver.c {1}.c;", CompileDir, csp.ID);
                            string Response = _SSHExec.RunCommand(DriverPrepCommand);
                            string CompileCommand = String.Format("cd {0}/compile_scripts;sh global_compile.sh {1} {2} {3} {4} {5} > {6};",
                                 CompileDir,
                                 ProcArch,                          // $1 = Processor type ("ppc" / "mb")
                                 CorePath,                          // $2 = Destination Path to copy app/driver to
                                 _PathMan["LinuxKernelLocation"],   // $3 = Path to Linux Source Tree
                                 _PathMan["ELDKLocation"],          // $4 = Path to ELDK Cross Compiler
                                 _PathMan["MicroblazeGNUTools"],    // $5 = Path to Microblaze GNU Tools (MB Only)
                                 GlobalCompileLog);
                            
                            DeleteRemoteFile(GlobalCompileLog);
                            Response = _SSHExec.RunCommand(CompileCommand);
                            _SSHXFer.From(GlobalCompileLog, LocalCompileLog);
                            DeleteRemoteFile(GlobalCompileLog);
                            if ((Response.Contains("ERROR")) || (Response.Contains("error")) || (Response.Contains("Error")))
                            {
                                Console.WriteLine("\n{0}\n\n", Response);
                                Console.WriteLine("Please review the log file at: {0}", LocalCompileLog);
                                throw new Exception("FAILED!");
                            }
                            #endregion

                            #region Write required commands to script files
                            string DrvDeviceName = String.Format("/dev/{0}", csp.ID);
                            DrvModuleName = String.Format("{0}.ko", csp.ID);

                            List<string> ProcScript = (List<string>)ProcScripts[cpi.CerebrumProcessorID];
                            ProcScript.Add(String.Format("cd {0}", Script_CorePath));
                            ProcScript.Add(String.Format("./init_board {0} {1} {2}", DrvDeviceName, MajorNum.ToString(), DrvModuleName));
                            ProcScript.Add(String.Format("{0}/{1} &", Script_CorePath, AppServerName));
                            #endregion

                            #region Prepare for compiling the next core
                            DeleteRemoteFile(String.Format("{0}/apps/{1}", CompileDir, AppHeaderFI.Name));
                            DeleteRemoteFile(String.Format("{0}/drivers/{1}", CompileDir, DriverHeaderFI.Name));
                            DeleteRemoteFile(String.Format("{0}/drivers/{1}", CompileDir, MakeFileFI.Name));
                            DeleteRemoteFile(String.Format("{0}/compile_scripts/{1}", CompileDir, CompileScriptFI.Name));
                            MajorNum++;
                            Console.WriteLine("Done");
                            #endregion
                        }
                    }
                }
                #endregion
                

                #region Create FPGA Boot-Time Self-Identification Table of MAC Addresses (<MountPoint>/mac_id_table)
                Console.Write("Creating FPGA-Identification Table...");
                {
                    FileInfo fiMACTable = new FileInfo(_PathMan.GetPath("ProjectTemp") + @"\mac_id_table");
                    if (fiMACTable.Exists)
                        fiMACTable.Delete();
                    StreamWriter MACWriter = new StreamWriter(fiMACTable.FullName);
                    foreach (string BoardFPGAID in MACTable.Keys)
                    {
                        MACWriter.Write(String.Format("{0} {1}\n", BoardFPGAID, (string)MACTable[BoardFPGAID]));
                    }
                    MACWriter.Close();
                    CopyFileToRemoteDirectory(fiMACTable, MountPoint, "777", "falcon");
                }
                Console.WriteLine("Done");
                #endregion

                #region Create Self-Identification Script (FPGA) (<MountPoint>/ip_id_script.sh)
                Console.Write("Creating Boot-Time FPGA Identification Script...");
                {
                    FileInfo fiMACIDScript = WriteMACIdentificationScript(OnBoard_MountPath);
                    CopyFileToRemoteDirectory(fiMACIDScript, MountPoint, "777", "falcon");
                }
                Console.WriteLine("Done");
                #endregion

                #region Create Self-Identification Script (Processor) (<MountPoint>/<UserName>/<Board>/<FPGA>/bootup.sh)
                Console.Write("Creating Boot-Time Processor Identification Scripts...");
                {
                    foreach (string BoardFPGAID in MACTable.Keys)
                    {
                        string RemoteScriptPath = String.Format("{0}/{1}", MountPath, BoardFPGAID);
                        FileInfo fiProcIDScript = WriteProcessorIdentificationScript();
                        CopyFileToRemoteDirectory(fiProcIDScript, RemoteScriptPath, "777", "falcon");
                    }
                }
                Console.WriteLine("Done");
                #endregion

                #region Create FPGA Boot-Time Processor-Identification Table of Cerebrum IDs (<MountPoint>/<UserName>/<Board>/<FPGA>/proc_id_table)
                Console.Write("Creating Processor-Identification Table...");
                {
                    FileInfo fiProcTable = new FileInfo(_PathMan.GetPath("ProjectTemp") + @"\proc_id_table");
                    if (fiProcTable.Exists)
                        fiProcTable.Delete();
                    StreamWriter ProcWriter = new StreamWriter(fiProcTable.FullName);
                    foreach (string CerebrumID in ProcTable.Keys)
                    {
                        ProcWriter.Write(String.Format("{0}\n", (string)ProcTable[CerebrumID]));
                    }
                    ProcWriter.Close();

                    foreach (string BoardFPGAID in MACTable.Keys)
                    {
                        string FPGAPath = String.Format("{0}/{1}", MountPath, BoardFPGAID);
                        CopyFileToRemoteDirectory(fiProcTable, FPGAPath, "777", "falcon");
                    }
                }
                Console.WriteLine("Done");
                #endregion

                #region Create Processor Startup Scripts (<MountPoint>/<UserName>/<Board>/<FPGA>/<Arch>/cpu<n>.sh)
                Console.Write("Creating Core Startup Scripts Table...");
                {
                    foreach (string CerebrumID in ProcScripts.Keys)
                    {
                        List<string> ScriptContents = (List<string>)ProcScripts[CerebrumID];
                        foreach (CoreProcessorInfo cpi in _Procs)
                        {
                            string ProcArch = string.Empty;
                            if ((cpi.Type.ToLower() == "powerpc") || (cpi.Type.ToLower() == "ppc"))
                            {
                                ProcArch = "ppc";
                            }
                            else if ((cpi.Type.ToLower() == "microblaze") || (cpi.Type.ToLower() == "mb"))
                            {
                                ProcArch = "mb";
                            }
                            else
                            {
                                continue;
                            }
                            string BoardFPGAID = cpi.FPGA.Replace('.', '/');
                            string ProcScriptPath = String.Format("{0}/{1}/{2}", MountPath, BoardFPGAID, ProcArch);
                            string cpunr = cpi.CPUNumber.ToString();

                            FileInfo fiProcScript = new FileInfo(_PathMan.GetPath("ProjectTemp") + @"\cpu" + cpunr + ".sh");
                            if (fiProcScript.Exists)
                                fiProcScript.Delete();
                            StreamWriter ProcScriptWriter = new StreamWriter(fiProcScript.FullName);
                            foreach (string ScriptLine in ScriptContents)
                            {
                                ProcScriptWriter.Write(String.Format("{0}\n", ScriptLine));
                            }
                            ProcScriptWriter.Close();
                            CopyFileToRemoteDirectory(fiProcScript, ProcScriptPath, "777", "falcon");
                        }
                    }
                }
                Console.WriteLine("Done");
                #endregion

                Console.WriteLine("Complete!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Exception caught in CoreServerCompiler.CompileCoreServers() -- {0}", ex.Message);
                return false;
            }
            finally
            {
                Logout();
            }
        }

        private bool ConnectToCompileServer()
        {
            LinkedListNode<FalconServer> fsNode = _Servers.First;
            while (fsNode != null)
            {
                _Server = fsNode.Value;
                try
                {
                    if ((_SSHExec == null) || (_SSHXFer == null))
                    {
                        Logout();
                        Login();
                        break;
                    }
                    if (((_SSHExec != null) && (!_SSHExec.Connected)) || ((_SSHXFer != null) && (!_SSHXFer.Connected)))
                    {
                        Logout();
                        Login();
                        break;
                    }
                    if ((!_SSHExec.Connected) || (!_SSHXFer.Connected))
                    {
                        _SSHExec.Close();
                        _SSHXFer.Close();
                        fsNode = fsNode.Next;
                    }
                }
                catch
                {
                    fsNode = fsNode.Next;
                }
            }
            if (fsNode == null)
            {
                Console.WriteLine("ERROR: Unable to connect to any servers specified in the servers file.");
                return false;
            }
            return true;
        }
        
        private FileInfo WriteCoreAppHeaderFile(CoreServerParameters csp, long MajorNum)
        {
            string HeadersPath = _PathMan.GetPath("LocalProjectRoot") + "\\headers";
            string CoreHeaderPath = HeadersPath + "\\" + csp.ID;
            string CoreAppHeaderPath = CoreHeaderPath + "\\apps";
            string CoreHeader = CoreAppHeaderPath + "\\custom_param.h";

            if (!Directory.Exists(HeadersPath))
                Directory.CreateDirectory(HeadersPath);
            if (!Directory.Exists(CoreHeaderPath))
                Directory.CreateDirectory(CoreHeaderPath);
            if (!Directory.Exists(CoreAppHeaderPath))
                Directory.CreateDirectory(CoreAppHeaderPath);
            if (File.Exists(CoreHeader))
                File.Delete(CoreHeader);

            FileInfo coreFI = new FileInfo(CoreHeader);
            StreamWriter writer = new StreamWriter(CoreHeader);


            writer.Write(String.Format("/**\n"));
            writer.Write(String.Format(" * Auto-generated Custom Core Parameters Header File for Core Server Application\n"));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" * {0}\n", this.GetFalconComponentVersion()));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" * Core: {0}\n", csp.ID));
            writer.Write(String.Format(" * FPGA: {0}\n", csp.FPGA));
            writer.Write(String.Format(" * Date: {0}\n", DateTime.Now.ToLongDateString()));
            writer.Write(String.Format(" * Time: {0}\n", DateTime.Now.ToLongTimeString()));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" */\n"));
            writer.Write(String.Format("/**\n"));
            writer.Write(String.Format(" * Preprocessors definitions: \n"));
            writer.Write(String.Format(" * @def BB_DEV_PATH\n"));
            writer.Write(String.Format(" * @brief name of the target device \n"));
            writer.Write(String.Format(" * @def SERVER_NAME\n"));
            writer.Write(String.Format(" * @brief IP address or name of the server\n"));
            writer.Write(String.Format(" * @def SERVER_PORT\n"));
            writer.Write(String.Format(" * @brief Port number that server is listening to\n"));
            writer.Write(String.Format(" * @def DM_NONE\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: No debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DM_BASIC\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: Essential debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DM_ALL\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: All debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DEBUG_MODE\n"));
            writer.Write(String.Format(" * @brief current level of debugging moe\n"));
            writer.Write(String.Format(" * @def TIMER_ON\n"));
            writer.Write(String.Format(" * @brief If this constant is set to 1, then execution time is computed.\n"));
            writer.Write(String.Format(" */\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("#define DM_NONE 			0\n"));
            writer.Write(String.Format("#define DM_BASIC			1\n"));
            writer.Write(String.Format("#define DM_ALL				2\n"));
            writer.Write(String.Format("#define DEBUG_MODE			DM_NONE\n"));
            writer.Write(String.Format("#define BB_DEV_PATH \"/dev/{0}\"\n", csp.ID));
            writer.Write(String.Format("#define SERVER_NAME {0}\n", csp.IP));
            writer.Write(String.Format("#define SERVER_PORT \"{0}\"\n", csp.Port.ToString()));
            writer.Write(String.Format("#define TIMER_ON			1\n"));
            writer.Write(String.Format("\n"));
            writer.Close();
            return coreFI;
        }
        private FileInfo WriteCoreDriverHeaderFile(CoreServerParameters csp, long MajorNum)
        {
            string HeadersPath = _PathMan.GetPath("LocalProjectRoot") + "\\headers";
            string CoreHeaderPath = HeadersPath + "\\" + csp.ID;
            string CoreDriverHeaderPath = CoreHeaderPath + "\\drivers";
            string CoreHeader = CoreDriverHeaderPath + "\\custom_param.h";

            if (!Directory.Exists(HeadersPath))
                Directory.CreateDirectory(HeadersPath);
            if (!Directory.Exists(CoreHeaderPath))
                Directory.CreateDirectory(CoreHeaderPath);
            if (!Directory.Exists(CoreDriverHeaderPath))
                Directory.CreateDirectory(CoreDriverHeaderPath);
            if (File.Exists(CoreHeader))
                File.Delete(CoreHeader);
            long aBase = HexToLong(csp.BaseAddress);
            long aHigh = HexToLong(csp.HighAddress);
            long aSize = aHigh - aBase + 1;
            csp.AddressSize = LongToHex(aSize, 0);

            FileInfo coreFI = new FileInfo(CoreHeader);
            StreamWriter writer = new StreamWriter(CoreHeader);

            writer.Write(String.Format("/**\n"));
            writer.Write(String.Format(" * Auto-generated Custom Core Parameters Header File for Core Driver Module\n"));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" * {0}\n", this.GetFalconComponentVersion()));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" * Core: {0}\n", csp.ID));
            writer.Write(String.Format(" * FPGA: {0}\n", csp.FPGA));
            writer.Write(String.Format(" * Date: {0}\n", DateTime.Now.ToLongDateString()));
            writer.Write(String.Format(" * Time: {0}\n", DateTime.Now.ToLongTimeString()));
            writer.Write(String.Format(" * \n"));
            writer.Write(String.Format(" */\n"));
            writer.Write(String.Format("/**\n"));
            writer.Write(String.Format(" * Preprocessors definitions: \n"));
            writer.Write(String.Format(" * @def debug_enabled\n"));
            writer.Write(String.Format(" * @brief if enabled, module print out additional debug messages\n"));
            writer.Write(String.Format(" * @def MODULE_NAME\n"));
            writer.Write(String.Format(" * @brief The name of module\n"));
            writer.Write(String.Format(" * @def DEVICE_NAME\n"));
            writer.Write(String.Format(" * @brief Name of the device added under /dev directory\n"));
            writer.Write(String.Format(" * @def MAJOR_NUM\n"));
            writer.Write(String.Format(" * @brief Major number of device (non-reserved major number)\n"));
            writer.Write(String.Format(" * @def BB_BASEADDRESS\n"));
            writer.Write(String.Format(" * @brief Base address of black box\n"));
            writer.Write(String.Format(" * @def BB_HIGHADDRESS\n"));
            writer.Write(String.Format(" * @brief High address of black box\n"));
            writer.Write(String.Format(" * @def BB_ADDRESS_SIZE\n"));
            writer.Write(String.Format(" * @brief Size of address space allocation to black box\n"));            
            writer.Write(String.Format(" * @def DM_NONE\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: No debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DM_BASIC\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: Essential debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DM_ALL\n"));
            writer.Write(String.Format(" * @brief DEBUG_MODE: All debugging messages are printed\n"));
            writer.Write(String.Format(" * @def DEBUG_MODE\n"));
            writer.Write(String.Format(" * @brief current level of debugging moe\n"));
            writer.Write(String.Format(" */\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("#define MODULE_NAME \"{0}\"\n", csp.ID.ToUpperInvariant()));
            writer.Write(String.Format("#define DEVICE_NAME \"{0}\"\n", csp.ID));
            writer.Write(String.Format("#define MAJOR_NUM {0}\n", MajorNum.ToString()));
            writer.Write(String.Format("#define BB_BASEADDRESS {0}\n", csp.BaseAddress));
            writer.Write(String.Format("#define BB_HIGHADDRESS {0}\n", csp.HighAddress));
            writer.Write(String.Format("#define BB_ADDRESS_SIZE {0}\n", csp.AddressSize));
            writer.Write(String.Format("#define DM_NONE 			0\n"));
            writer.Write(String.Format("#define DM_BASIC			1\n"));
            writer.Write(String.Format("#define DM_ALL				2\n"));
            writer.Write(String.Format("#define DEBUG_MODE			DM_NONE\n"));
            writer.Write(String.Format("\n"));
            writer.Close();
            return coreFI;
        }
        private FileInfo WriteCoreDriverMakeFile(CoreServerParameters csp)
        {
            string HeadersPath = _PathMan.GetPath("LocalProjectRoot") + "\\headers";
            string CoreHeaderPath = HeadersPath + "\\" + csp.ID;
            string CoreDriverHeaderPath = CoreHeaderPath + "\\drivers";
            string CoreMakefile = CoreDriverHeaderPath + "\\Makefile";

            if (!Directory.Exists(HeadersPath))
                Directory.CreateDirectory(HeadersPath);
            if (!Directory.Exists(CoreHeaderPath))
                Directory.CreateDirectory(CoreHeaderPath);
            if (!Directory.Exists(CoreDriverHeaderPath))
                Directory.CreateDirectory(CoreDriverHeaderPath);
            if (File.Exists(CoreMakefile))
                File.Delete(CoreMakefile);

            FileInfo makeFI = new FileInfo(CoreMakefile);
            StreamWriter writer = new StreamWriter(CoreMakefile);
            writer.Write("\n");
            writer.Write("TargetFile = {0}.o\n", csp.ID);
            writer.Write("obj-m := ${TargetFile}\n");
            writer.Write("\n");
            writer.Close();
            return makeFI;
        }
        private FileInfo WriteCoreGlobalCompileScript(CoreServerParameters csp, string ProcType)
        {
            string HeadersPath = _PathMan.GetPath("LocalProjectRoot") + "\\headers";
            string CoreHeaderPath = HeadersPath + "\\" + csp.ID;
            string CoreCompilePath = CoreHeaderPath + "\\compile_scripts";
            string CoreCompile = CoreCompilePath + "\\global_compile.sh";

            if (!Directory.Exists(HeadersPath))
                Directory.CreateDirectory(HeadersPath);
            if (!Directory.Exists(CoreHeaderPath))
                Directory.CreateDirectory(CoreHeaderPath);
            if (!Directory.Exists(CoreCompilePath))
                Directory.CreateDirectory(CoreCompilePath);
            if (File.Exists(CoreCompile))
                File.Delete(CoreCompile);

            FileInfo compileFI = new FileInfo(CoreCompile);
            StreamWriter writer = new StreamWriter(CoreCompile);
            writer.Write(String.Format("#!/bin/bash\n"));
            writer.Write(String.Format("###################################################\n"));
            FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated header file.     
            writer.Write(String.Format("# Arguments\n"));
            writer.Write(String.Format("# $1 = Processor type (\"ppc\" / \"mb\")\n"));
            writer.Write(String.Format("# $2 = Destination Path to copy app/driver to\n"));
            writer.Write(String.Format("# $3 = Path to Linux Source Tree\n"));
            writer.Write(String.Format("# $4 = Path to ELDK Cross Compiler\n"));
            writer.Write(String.Format("# $5 = Path to Microblaze GNU Tools (MB Only)\n"));
            writer.Write(String.Format("# e.g. ./global_compile.sh ppc /home/linux /home/output/board/fpga/ppc/cpu1/core /export/home/opt/eldk\n"));
            writer.Write(String.Format("# e.g. ./global_compile.sh mb /home/linux /home/output/board/fpga/mb/cpu1/core /export/home/opt/eldk /export/home/opt/microblaze-unknown-linux-gnu/bin\n"));
            writer.Write(String.Format("###################################################\n"));
            writer.Write(String.Format("echo Verifying arguments...\n"));
            writer.Write(String.Format("# Verify Architecture\n"));
            writer.Write(String.Format("if [ \"$1\" == \"\" ]; then\n"));
            writer.Write(String.Format("  echo ERROR! Target processor architecture must be specified!\n"));
            writer.Write(String.Format("  echo \"./global_compile.sh <Linux Source> <Output Destination> <ELDK Path> <Processor Arch.> <Microblaze GNU Path>\"\n"));
            writer.Write(String.Format("  exit 1\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("if [ \"$1\" == \"mb\" ]; then\n"));
            writer.Write(String.Format("  # Verify Microblaze GNU\n"));
            writer.Write(String.Format("  if [ \"$5\" == \"\" ]; then\n"));
            writer.Write(String.Format("    echo ERROR! Path to Microblaze GNU Tools must be specified when target architecture is 'mb'!\n"));
            writer.Write(String.Format("    echo \"./global_compile.sh <Linux Source> <Output Destination> <ELDK Path> <Processor Arch.> <Microblaze GNU Path>\"\n"));
            writer.Write(String.Format("    exit 1\n"));
            writer.Write(String.Format("  fi\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("# Verify Output\n"));
            writer.Write(String.Format("if [ \"$2\" == \"\" ]; then\n"));
            writer.Write(String.Format("  echo ERROR! Output Destination path must be specified!\n"));
            writer.Write(String.Format("  echo \"./global_compile.sh <Linux Source> <Output Destination> <ELDK Path> <Processor Arch.> <Microblaze GNU Path>\"\n"));
            writer.Write(String.Format("  exit 1\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("# Verify Source Tree\n"));
            writer.Write(String.Format("if [ \"$3\" == \"\" ]; then\n"));
            writer.Write(String.Format("  echo ERROR! Path to Linux Source Tree must be specified!\n"));
            writer.Write(String.Format("  echo \"./global_compile.sh <Processor Arch.> <Linux Source> <Output Destination> <ELDK Path> <Microblaze GNU Path>\"\n"));
            writer.Write(String.Format("  exit 1\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("# Verify ELDK\n"));
            writer.Write(String.Format("if [ \"$3\" == \"\" ]; then\n"));
            writer.Write(String.Format("  echo ERROR! Path to ELDK cross compiler must be specified!\n"));
            writer.Write(String.Format("  echo \"./global_compile.sh <Linux Source> <Output Destination> <ELDK Path> <Processor Arch.> <Microblaze GNU Path>\"\n"));
            writer.Write(String.Format("  exit 1\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("###################################################\n"));
            writer.Write(String.Format("# Begin compilation\n"));
            writer.Write(String.Format("###################################################\n"));
            writer.Write(String.Format("echo Processor Arch = $1\n"));
            writer.Write(String.Format("echo Output Path = $2\n"));
            writer.Write(String.Format("echo Linux Source = $3\n"));
            writer.Write(String.Format("echo ELDK Path = $4\n"));
            writer.Write(String.Format("echo MB GNU Path = $5\n"));

            writer.Write(String.Format("echo\n"));
            writer.Write(String.Format("echo Starting Core Server Application Compilation\n"));
            writer.Write(String.Format("cd ../apps/\n"));          
            writer.Write(String.Format("echo ./compile_app.sh $1 $4 $5\n"));
            writer.Write(String.Format("./compile_app.sh $1 $4 $5\n"));
            writer.Write(String.Format("echo cp AcceleroServer $2\n", ProcType));
            writer.Write(String.Format("cp AcceleroServer $2\n", ProcType));
            writer.Write(String.Format("chmod 777 $2/AcceleroServer\n", ProcType));

            writer.Write(String.Format("echo\n"));
            writer.Write(String.Format("echo Starting Core Driver Compilation\n"));
            writer.Write(String.Format("cd ../drivers/\n"));
            writer.Write(String.Format("./compile_driver.sh $1 $3 $4 $5\n"));
            writer.Write(String.Format("echo cp {0}.ko $2\n", csp.ID));
            writer.Write(String.Format("cp {0}.ko $2\n", csp.ID));
            writer.Write(String.Format("chmod 777 $2/{0}.ko\n", csp.ID));

            writer.Write(String.Format("echo\n"));
            writer.Write(String.Format("echo Copying init_board Core-Initialization Script\n"));
            writer.Write(String.Format("cd ../fpga_scripts\n"));
            writer.Write(String.Format("echo cp init_board $2\n"));
            writer.Write(String.Format("cp init_board $2\n"));
            writer.Write(String.Format("chmod 777 $2/init_board\n"));
            writer.Write(String.Format("\n"));
            writer.Close();
            return compileFI;

        }
        private FileInfo WriteMACIdentificationScript(string MountPath)
        {            
            string IPIDScript = _PathMan.GetPath("ProjectTemp") + "\\ip_id_script.sh";

            if (!Directory.Exists(_PathMan.GetPath("ProjectTemp")))
                Directory.CreateDirectory(_PathMan.GetPath("ProjectTemp"));
            if (File.Exists(IPIDScript))
                File.Delete(IPIDScript);

            FileInfo ipFI = new FileInfo(IPIDScript);
            StreamWriter writer = new StreamWriter(IPIDScript);
            writer.Write(String.Format("#!/bin/sh\n"));
            FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
            writer.Write(String.Format("MOUNTPOINT=\"{0}\"\n", MountPath));
            writer.Write(String.Format("\n"));
            writer.Write("MACLIST=`ip addr | grep \"link/\" | awk '{ print $2 }'`\n");   // | awk -F / '{ print $1 }'`\n");
            writer.Write(String.Format("for MAC in $MACLIST; do\n"));
            writer.Write(String.Format("    FPGAMATCH=`grep $MAC {0}/mac_id_table`\n", _PathMan.GetPath("OnBoardMount")));
	        writer.Write(String.Format("    if [ \"$FPGAMATCH\" != \"\" ]; then\n"));
		    writer.Write("        FPGAFOLDER=`echo $FPGAMATCH | awk '{ print $1 }'`\n");
	        writer.Write(String.Format("    fi\n"));
            writer.Write(String.Format("done\n"));
            writer.Write(String.Format("if [ \"$FPGAFOLDER\" != \"\" ]; then\n"));
            writer.Write(String.Format("    FPGASCRIPT=\"$MOUNTPOINT/$FPGAFOLDER/bootup.sh\"\n"));
	        writer.Write(String.Format("    echo Loading bootup script $FPGASCRIPT\n"));
	        writer.Write(String.Format("    # Uncomment the following line for use on the FPGA\n"));
            writer.Write(String.Format("    sh $FPGASCRIPT $MOUNTPOINT/$FPGAFOLDER\n"));
            writer.Write(String.Format("fi\n"));
            writer.Close();
            return ipFI;
        }
        private FileInfo WriteProcessorIdentificationScript()
        {
            string ProcIDScript = _PathMan.GetPath("ProjectTemp") + "\\bootup.sh";

            if (!Directory.Exists(_PathMan.GetPath("ProjectTemp")))
                Directory.CreateDirectory(_PathMan.GetPath("ProjectTemp"));
            if (File.Exists(ProcIDScript))
                File.Delete(ProcIDScript);

            FileInfo fInfo = new FileInfo(ProcIDScript);
            StreamWriter writer = new StreamWriter(fInfo.FullName);
            writer.Write(String.Format("#!/bin/sh\n"));
            FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
            writer.Write(String.Format("# $1 = Location of this file (bootup.sh)\n"));
            writer.Write(String.Format("\n"));
            writer.Write("UNAME_ANSWER=`uname -a`\n");
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("#determine processor architecture\n"));
            writer.Write("PROCTYPE=`echo $UNAME_ANSWER | grep ppc`\n");
            writer.Write(String.Format("if [ \"$PROCTYPE\" != \"\" ]; then\n"));
            writer.Write(String.Format("  PROCARCH=\"ppc\"\n"));
            writer.Write(String.Format("  else\n"));
            writer.Write("  PROCTYPE=`echo $UNAME_ANSWER | grep mb`\n");
            writer.Write(String.Format("  if [ \"$PROCTYPE\" != \"\" ]; then\n"));
            writer.Write(String.Format("    PROCARCH=\"mb\"\n"));
            writer.Write(String.Format("  fi\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("#determine processor id\n"));
            writer.Write(String.Format("SAVED_IFS=$IFS\n"));
            writer.Write(String.Format("IFS='\\n'\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("cd \"$1\"\n"));
            writer.Write("PROC_ID_TABLE=`cat proc_id_table | awk '{ print $0 }'`\n");
            writer.Write(String.Format("for PROC_ID_LINE in $PROC_ID_TABLE; do\n"));
            writer.Write("  PROC_ID=`echo $PROC_ID_LINE | awk '{ print $1 }'`\n");
            writer.Write("  MATCH=`echo $UNAME_ANSWER | grep $PROC_ID`\n");
            writer.Write(String.Format("  if [ \"$MATCH\" != \"\" ]; then\n"));
            writer.Write(String.Format("    #found the match\n"));
            writer.Write("    CPUNR=`echo $PROC_ID_LINE | awk '{ print $2 }'`\n");
            writer.Write(String.Format("  fi\n"));
            writer.Write(String.Format("done\n"));
            writer.Write(String.Format("IFS=$SAVED_IFS\n"));
            writer.Write(String.Format("\n"));
            writer.Write(String.Format("if [ \"$PROCARCH\" != \"\" ]; then\n"));
            writer.Write(String.Format("  if [ \"$CPUNR\" != \"\" ]; then\n"));
            writer.Write(String.Format("    echo \"Identified Processor Architecture $PROCARCH, CPU Number $CPUNR.\"\n"));
            writer.Write(String.Format("    echo \"Executing CPU Startup Script $1/$PROCARCH/cpu$CPUNR.sh\" \n"));
            writer.Write(String.Format("    sh $1/$PROCARCH/cpu$CPUNR.sh\n"));
            writer.Write(String.Format("  else \n"));
            writer.Write(String.Format("    echo \"ERROR: Identified Processor Architecture $PROCARCH, but unable to resolve CPU Number.\"\n"));
            writer.Write(String.Format("  fi\n"));
            writer.Write(String.Format("else\n"));
            writer.Write(String.Format("    echo \"ERROR: Unable to identify Processor Architecture.\"\n"));
            writer.Write(String.Format("fi\n"));
            writer.Write(String.Format("\n"));

            writer.Close();
            return fInfo;
        }

        private void CreateRemoteDirectory(string NewDirectory, string Permissions, string Group)
        {
            _SSHExec.RunCommand(String.Format("mkdir {0}", NewDirectory));
            _SSHExec.RunCommand(String.Format("chmod {1} {0}", NewDirectory, Permissions));
            _SSHExec.RunCommand(String.Format("chgrp {1} {0}", NewDirectory, Group));
        }
        private void CopyFileToRemoteDirectory(FileInfo LocalFile, string RemotePath, string Permissions, string Group)
        {
            string RemoteFile = String.Format("{0}{1}{2}", RemotePath, (RemotePath.EndsWith("/") ? "" : "/"), LocalFile.Name);
            _SSHXFer.To(LocalFile.FullName, RemoteFile);
            _SSHExec.RunCommand(String.Format("chmod {1} {0}", RemoteFile, Permissions));
            _SSHExec.RunCommand(String.Format("chgrp {1} {0}", RemoteFile, Group));
        }
        private void DeleteRemoteFile(string FilePath)
        {
            _SSHExec.RunCommand(String.Format("rm {0}", FilePath));
        }
        private void DeleteRemoteDirectory(string DirectoryPath)
        {
            _SSHExec.RunCommand(String.Format("rm -rf {0}", DirectoryPath));
        }

        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon Core Application/Driver Compiler";
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
