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
/*********************************************************************************************************** 
 * FalconPlatformSynthesis\FalconPlatform.cs
 * Name: Matthew Cotter
 * Date: 18 Jun 2010 
 * Description: Library to automate hardware synthesis and software compilation of an FPGA platform using Xilinx tools.
 * Notes:
 *     
 * History: 
 * >> (20 Jun 2010) Matthew Cotter: Added STDERR to logfile output of scripts and tools generated and used by Cerebrum
 * >> (29 May 2010) Matthew Cotter: Added parsing of Place and Route log file to detect missing timing constraint and unroutable designs.
 * >>                               Added *.par and *.unroutes to the list of files copied from the synthesis server post-synthesis.
 * >> (18 May 2010) Matthew Cotter: Implemented support for parsing and reporting post-synthesis resource utilization, for possible use in future resource estimations.
 * >> (13 May 2010) Matthew Cotter: Implemented support for synthesizing a specific subset of FPGAs from the platform.
 * >> ( 7 Mar 2011) Matthew Cotter: Restructured file manipulation during codelet compilation.
 * >> ( 3 Mar 2011) Matthew Cotter: Added project path 'PECompileDir' to CompileCodelet().
 * >> (27 Feb 2011) Matthew Cotter: Completed basic functionality of BRAM reprogramming methods
 * >> (25 Feb 2011) Matthew Cotter: Initial work on BlockRAM reprogrammability.
 * >> (18 Feb 2011) Matthew Cotter: Added properties to allow assignment of pcores that were mapped to this FPGA.
 * >> (26 Jan 2011) Matthew Cotter: Added ./synthesis/*.srp log report files to output retrieved post-synthesis.
 * >> ( 7 Jan 2011) Matthew Cotter: Began work on support for custom bitfile generation scripts rather than requiring use of the default script.
 * >> (24 Aug 2010) Matthew Cotter: Added explicit redirection of synthesis flow output to logfiles to catch messages only written to STDOUT.
 * >> (13 Aug 2010) Matthew Cotter: Updated Library Generation to include ModuleSearchPaths in libgen.
 * >> (12 Aug 2010) Matthew Cotter: Early support for compiling Linux for Microblaze.
 *                                  Added synthesis support for additional Module Search Paths for global pcores.
 * >> (11 Aug 2010) Matthew Cotter: Properties that have been proposed to be moved from Platform/Board to Design.Processors have been reverted pending approval of changes.
 *                                  Removed MoveLogs() function to avoid confusion, as it was no longer used.
 * >> (10 Aug 2010) Matthew Cotter: Added code to PrepareSoftware() to delete and create blank MSS file prior to adding Processor OS blocks to it.
 *                                  This change was made to ensure that no stale processor OS blocks are left in the file. This type of situation
 *                                  would have occured if a processor was modified between synthesis tool executions without a re-run of the XPS Project Builder.
 *                                  Added <project>\implementation\*.mrp to the list of files that are copied to output prior to packaging for download.
 * >> ( 2 Aug 2010) Matthew Cotter: Added ForcedClean flag to both BuildHardware and BuildSoftware to enforce that previous synthesis work should be cleaned 
 *                                  before continuing with the current synthesis.
 * >> ( 2 Aug 2010) Matthew Cotter: Moved properties and paths specific to the cross-compilation of Linux for a processor to the FalconProcessorOS class.
 *                                  Moved ELDKLocation from Path Manager to FalconProcessorOS property.   Renamed ELDKLocation to CompilerLocation.
 * >> (26 Jul 2010) Matthew Cotter: Removed calls that clean results of previous synthesis, if any, prior to starting current synthesis.
 * >> (22 Jul 2010) Matthew Cotter: Corrected issue in recognizing that a remote task was completed that arose with modification made to SharpSSH library.
 * >> ( 2 Jul 2010) Matthew Cotter: Modified Linux compilation script to avoid slow-boot issue on ML510.
 * >> (25 Jun 2010) Matthew Cotter: Added code to create batch files along the same lines as bash scripts to execute batch commands on a remote windows server.
 * >> (23 Jun 2010) Matthew Cotter: Completed work on compiling standalone software.
 * >> (23 Jun 2010) Matthew Cotter: Added functions to automate manual editing of MSS and DTS files for Linux Kernel Cross-compilation.
 *                                  Continued work on compilation of software libraries - Completed for linux kernel
 * >> (22 Jun 2010) Matthew Cotter: Completed automation of hardware tool flow on remote (Linux) machines.
 *                                  Started work on implementation of software compilation on remote (Linux) machines.
 * >> (21 Jun 2010) Matthew Cotter: Added commands to process entire Xilinx hardware synthesis tool flow on a local (Windows) Machine.
 *                                      (platgen -> xst -> ngcbuild -> ngdbuild -> map -> par -> bitgen)
 *                                  Began work on implementation of automating tool flow on remote (Linux) machine.
 * >> (18 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using FalconGlobal;
using Tamir;
using Tamir.SharpSsh;
using System.Collections;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using FalconPathManager;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconPlatformSynthesis
{
    /// <summary>
    /// Error/response codes generated by SynthesizeDesign() (via calls to PurgeOutputDirectory(), BuildHardware(), and BuildSoftware())
    /// </summary>
    public enum SynthesisErrorCodes : int
    {
        /// <summary>
        /// All.  A general reset condition indicating that no synthesis has been done yet.
        /// </summary>
        SYTHNESIS_NO_STATUS,
        /// <summary>
        /// All. Generated because performing synthesis/compilaton on a remote windows server is not yet supported.
        /// </summary>
        SYNTHESIS_ON_REMOTE_WINDOWS_FAIL,
        /// <summary>
        /// All.  Generated when an unidentified or unhandled exception occurs.
        /// </summary>
        SYTHNESIS_UNSPECIFIED_ERROR,
        /// <summary>
        /// All.  Generated when the platform is not ready for compilation.
        /// </summary>
        SYTHNESIS_NOT_READY,
        /// <summary>
        /// PurgeOutputDirectory.  Generated when the output directory was not purged successfully.
        /// </summary>
        SYNTHESIS_PURGE_FAIL,
        /// <summary>
        /// All.  Generated when the synthesis has completed successfully.
        /// </summary>
        SYNTHESIS_OK,
        /// <summary>
        /// All.  Generated when the directory structure is unable to be created.
        /// </summary>
        SYNTHESIS_CREATE_DIR_FAIL,
        /// <summary>
        /// BuildHardware.  Generated when platform generation is unsuccessful.
        /// </summary>
        SYNTHESIS_PLATGEN_FAIL,
        /// <summary>
        /// BuildHardware.  Generated when hardware synthesis (xst and ngcbuild) are unsuccessful.
        /// </summary>
        SYNTHESIS_HW_SYNTH_FAIL,
        /// <summary>
        /// BuildHardware.  Generated when xflow (ngdbuild, map, and par) are unsuccessful.
        /// </summary>
        SYNTHESIS_XFLOW_FAIL,
        /// <summary>
        /// BuildHardware.  Generated when bitfile generation is unsuccessful.
        /// </summary>
        SYNTHESIS_BITGEN_FAIL,
        /// <summary>
        /// BuildSoftware.  Generated when software preparation (MSS modification) is unsuccessful.
        /// </summary>
        SYNTHESIS_SOFTWARE_PREPARE_FAIL,
        /// <summary>
        /// BuildSoftware.  Generated when library generation is unsuccessful.
        /// </summary>
        SYNTHESIS_LIBGEN_FAIL,
        /// <summary>
        /// BuildSoftware.  Generated when DTS modification is unsuccessful.
        /// </summary>
        SYNTHESIS_DTS_UPDATE_FAIL,
        /// <summary>
        /// BuildSoftware.  Generated when ELF file generation/compilation is unsuccessful.
        /// </summary>
        SYNTHESIS_COMPILE_FAIL,
        /// <summary>
        /// All.  Generated when remote server authentication is unsuccessful.
        /// </summary>
        SYNTHESIS_REMOTE_AUTH_FAIL,
        /// <summary>
        /// All.  Generated when retrieval of output files from remote server is unsuccessful.
        /// </summary>
        SYNTHESIS_FILE_DOWNLOAD_FAIL,
        /// <summary>
        /// Indicates that synthesis for this platform was skipped due to pre-emptyed MHS file.
        /// </summary>
        SYNTHESIS_SKIPPED,
        /// <summary>
        /// Indicates that the thread that was executing synthesis was aborted.
        /// </summary>
        SYTHNESIS_THREAD_ABORT,
        /// <summary>
        /// Indicates that the user aborted synthesis before it could begin.
        /// </summary>
        SYTHNESIS_USER_ABORT,
        
        /// <summary>
        /// Indicates that codelet compilation failed for one or more components.
        /// </summary>
        SYTHNESIS_CODELET_COMPILE_FAIL,
        /// <summary>
        /// Indicates that codelet merge failed for one or more components.
        /// </summary>
        SYTHNESIS_CODELET_MERGE_FAIL
    }

    /// <summary>
    /// Enumeration of the HDL languages (VHDL or Verilog) available.
    /// </summary>
    public enum HDLLanguage : int
    {
        /// <summary>
        /// Verilog HDL Language
        /// </summary>
        Verilog = 0,
        /// <summary>
        /// VHDL HDL Language
        /// </summary>
        VHDL = 1
    }
    
    /// <summary>
    /// Represents a specific platform to be synthesized and compiled for programming on an FPGA.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/System Synthesis Specification.pdf">
    /// System Synthesis Documentation</seealso>
    public class FalconPlatform : IFalconLibrary
    {
        #region Private Members, Events and Methods
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
            outputText = String.Format("[{0}]: {1}", this.PlatformID, outputText);
            if (MessageEvent != null)
            {
                MessageEvent(outputText);
            }
            else
            {
                Console.WriteLine(outputText);
            }
        }

        /// <summary>
        /// Event that fires to request a password from an external source.  If this event is not attached,
        /// the password is requested from the console.
        /// </summary>
        public PasswordRequestDelegate OnRequirePassword;

        private ProjectEventRecorder EventRecord;

        #region Synthesis Tool Declarations and "Constants"
        private string Separator { get { return (this.LinuxHost ? "/" : "\\"); } }

        private string _PlatformDirectory = string.Empty;
        private string _XilinxEDKDirectory = string.Empty;
        private string _LinuxKernelLocation = string.Empty;

        private string _OutputBIT = string.Empty;
        //private string _OutputELF = string.Empty;
        
        private bool _LinuxHost = false;
        private FalconServer _SynthesisServer;
        private List<FalconProcessorOS> _Processors;

        private SshExec _SSHExec;
        private Scp _SSHXFer;
        private SshShell _SSHShell;

        private StreamWriter _logWriter = null;

        // Xilinx tool commands
        private readonly string XTOOL_PLATGEN = "platgen";
        private readonly string XTOOL_NGCBUILD = "ngcbuild";
        private readonly string XTOOL_XST = "xst";
        private readonly string XTOOL_NGDBUILD = "ngdbuild";
        private readonly string XTOOL_MAP = "map";
        private readonly string XTOOL_PAR = "par";
        private readonly string XTOOL_BITGEN = "bitgen";
        private readonly string XTOOL_XPS = "xps";
        private readonly string XTOOL_LIBGEN = "libgen";
        private readonly string XTOOL_DATA2MEM = "data2mem";


        // Interpreter Strings
        private readonly string FINISH_EXPECT = "FalconSynthesisStage Finished!";
        private readonly string FINISH_SUCCESS = "Complete: FalconSynthesisStage Finished!";
        private readonly string FINISH_ERROR = "Error: FalconSynthesisStage Finished!";

        // Remote Listing Strings
        private readonly string DIR_LIST_START = "DIRECTORY_LISTING_STARTED";
        private readonly string DIR_LIST_FINISH = "DIRECTORY_LISTING_FINISHED";

        #endregion

        private PathManager _PathMan;
        /// <summary>
        /// Get or set the Project Path Manager to be used for managing relevant project paths.
        /// </summary>
        public PathManager ProjectPathManager
        {
            get
            {
                return _PathMan;
            }
            set
            {
                _PathMan = value;
            }
        }

        private static readonly string IO_REDIRECT_STDOUT_STDERR = "2>&1";
        private static readonly string LINUX_COMMAND_SHELL = "sh";


        #endregion

        #region Properties
        /// <summary>
        /// Get or set the Error Code that this platform synthesis has produced
        /// </summary>
        public SynthesisErrorCodes ReturnCode { get; set; }
        /// <summary>
        /// Get or set the ID of this platform.
        /// </summary>
        public string PlatformID { get; set; }
        /// <summary>
        /// Get or set the full path to the platform root directory {platform_root} - the location of system.mhs and system.mss.
        /// </summary>
        public string PlatformDirectory 
        {
            get 
            {
                if (!_PlatformDirectory.EndsWith(this.Separator))
                    return _PlatformDirectory + this.Separator; 
                else 
                    return _PlatformDirectory;
            }
            set
            {
                _PlatformDirectory = value;
            }
        }
        /// <summary>
        /// Get the short name of the system UCF file (system.ucf)
        /// </summary>
        public string SystemUCF { get { return "system.ucf"; } }
        /// <summary>
        /// Get the short name of the system MHS file (system.mhs)
        /// </summary>
        public string SystemMHS { get { return "system.mhs"; } }
        /// <summary>
        /// Get the short name of the system MSS file (system.mss)
        /// </summary>
        public string SystemMSS { get { return "system.mss"; } }
        /// <summary>
        /// Get or set the short name of the system BMM file
        /// </summary>
        public string SystemBMM { get; set; }

        /// <summary>
        /// Get or set the bit file to be generated for the hardware platform.
        /// </summary>
        public string OutputBIT 
        { 
            get
            {
                return _OutputBIT;
            }
            set
            {
                if (!value.EndsWith(".bit"))
                    value += ".bit";
                _OutputBIT = value;
            }
        }

        /// <summary>
        /// Get or set the suffix appended to generated output files.  The default is "system".
        /// </summary>
        public string SystemPrefix { get; set; }

        /// <summary>
        /// Read-Only.  Gets the list of Processor instances that are part of this platform.
        /// </summary>
        public List<FalconProcessorOS> Processors
        {
            get
            {
                return _Processors;
            }
        }
        /// <summary>
        /// Get or set the full path to the default location of the linux kernel to be used for compiling the Embeded Linux for on-board processors.
        /// If the processor specifies the LinuxKernelSource parameter, that value will override this one.   This property may be phased out over time.
        /// </summary>
        public string LinuxKernelLocation
        {
            get
            {
                if (!_LinuxKernelLocation.EndsWith(this.Separator))
                    return _LinuxKernelLocation + this.Separator;
                else
                    return _LinuxKernelLocation;
            }
            set
            {
                _LinuxKernelLocation = value;
            }
        }
        /// <summary>
        /// Get or set the IP address to be set in the bootargs in this platform.  If IPFromDHCP is set to true, this value is ignored
        /// and bootargs will be set to auto-obtain an IP address.   If IPFromDHCP is false, bootargs will be set to this IP address, if it is set.  If it
        /// not set, DHCP will be enabled anyway.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Overrides the value of IPAddress if set to true.   If set to false, the value of IP address is used in bootargs, if it is set.   If not,
        /// DHCP is enabled anyway.
        /// </summary>
        public bool IPFromDHCP { get; set; }
        /// <summary>
        /// Get or set the core instance name of the ethernet device to be used in this platform.
        /// </summary>
        public string EthernetDevice { get; set; }
        /// <summary>
        /// Get or set the MAC address to be assigned to the ethernet device in this platform.
        /// </summary>
        public string EthernetMAC { get; set; }

        /// <summary>
        /// Get or set the full path to the location of the Xilinx EDK directory.
        /// </summary>
        public string XilinxEDKDirectory
        {
            get
            {
                if (!_XilinxEDKDirectory.EndsWith(this.Separator))
                    return _XilinxEDKDirectory + this.Separator;
                else
                    return _XilinxEDKDirectory;
            }
            set
            {
                _XilinxEDKDirectory = value;
            }
        }
        /// <summary>
        /// Get the full path to the location of the Xilinx IPLib PCores directory.
        /// </summary>
        public string XilinxProcessorIPLibPCoresDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this._XilinxEDKDirectory + "hw" + this.Separator + "XilinxProcessorIPLib" + this.Separator + "pcores" + this.Separator;
            }
        }
        /// <summary>
        /// Get or set the full path to the location of the Linux kernel device-tree files.
        /// </summary>
        public string DeviceTree { get; set; }

        /// <summary>
        /// Get or set whether this platform is to be synthesized and compiled locally.
        /// </summary>
        public bool LocalSynthesis { get; set; }
        /// <summary>
        /// Get or set whether this platform is to be synthesized and compiled on a linux host.
        /// </summary>
        public bool LinuxHost 
        {
            get
            {
                return _LinuxHost;
            }
            set
            {
                string oldSeparator = this.Separator;
                _LinuxHost = value;
                this.PlatformDirectory = this.PlatformDirectory.Replace(oldSeparator, this.Separator);
            }
        }
        /// <summary>
        /// Get or set the synthesis server object to be used for synthesis and compilation.
        /// </summary>
        public FalconServer SynthesisServer 
        {
            get
            {
                return _SynthesisServer;
            }
            set
            {
                _SynthesisServer = value;
                if (_SynthesisServer != null)
                    this.LinuxHost = _SynthesisServer.LinuxHost;
            }
        }

        /// <summary>
        /// Get or set whether previous synthesis results should be cleaned prior to beginning synthesis.
        /// </summary>
        public bool ForceClean { get; set; }

        /// <summary>
        /// Get or set a flag indicating whether hardware should be synthesized in addition to compiling and merging PE Code sources.
        /// </summary>
        public bool PerformFullSynthesis { get; set; }

        /// <summary>
        /// Get or set a flag indicating whether this Platform should be skipped during all synthesis processing.
        /// </summary>
        public bool SkipSynthesis { get; set; }


        /// <summary>
        /// Get the full path to the location of the {project_root}/bsp directory.
        /// </summary>
        public string BSPDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "bsp" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/data directory.
        /// </summary>
        public string DataDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "data" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/etc directory.
        /// </summary>
        public string ETCDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "etc" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/hdl directory.
        /// </summary>
        public string HDLDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "hdl" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/implementation directory.
        /// </summary>
        public string ImplementationDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "implementation" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/pcores directory.
        /// </summary>
        public string PCoresDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "pcores" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/synthesis directory.
        /// </summary>
        public string SynthesisDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "synthesis" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/lib directory.
        /// </summary>
        public string LIBDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "lib" + this.Separator;
            }
        }
        /// <summary>
        /// Get the full path to the location of the {platform_root}/output directory.
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                if (this.PlatformDirectory == string.Empty)
                    return string.Empty;
                else
                    return this.PlatformDirectory + "output" + this.Separator;
            }
        }

        /// <summary>
        /// OS = Linux.  Get or set the full path to the location of the ELDK Cross Compiler for Linux compilation.
        /// </summary>
        public string ELDKLocation { get; set; }
        /// <summary>
        /// OS = Linux.  Get or set the full path to the location of the Microblaze GNU Tools for Linux compilation.
        /// </summary>
        public string MBGNULocation { get; set; }

        #region PlatGen Properties
        /// <summary>
        /// Get or set the part size of the fpga achitecture to be used.
        /// </summary>
        public string PlatGenXPartSize { get; set; }
        /// <summary>
        /// Get or set the package of the fpga achitecture to be used.
        /// </summary>
        public string PlatGenXPartPackage { get; set; }
        /// <summary>
        /// Get or set the speedgrade of the fpga achitecture to be used.
        /// </summary>
        public string PlatGenXPartSpeedGrade { get; set; }
        /// <summary>
        /// Get or set the part name of the fpga achitecture to be used (Concatenation of PartSize, PartPackage, and PartSpeedGrade).
        /// </summary>
        public string PlatGenXPartName
        {
            get { return String.Format("{0}{1}{2}", this.PlatGenXPartSize, this.PlatGenXPartPackage, this.PlatGenXPartSpeedGrade); }
        }
        /// <summary>
        /// Get or set the HDL language to be used for this hardware.
        /// </summary>
        public HDLLanguage PlatGenHDLLang { get; set; }
        #endregion

        #region BitGen Properties
        /// <summary>
        /// Get or set the path to the bitfile generation script to be used.  If the file does not exist when synthesis starts, an error will be thrown and synthesis aborted.
        /// If the value is set to NULL or an empty string, the default script will be generated and used.
        /// </summary>
        public string BitGenScriptPath { get; set; }
        #endregion

        #endregion

        #region Constructor/Initialization
        /// <summary>
        /// Constructor to initialize all fields of the platform in preparation for configuration.
        /// </summary>
        public FalconPlatform()
        {
            this.PlatformID = string.Empty;
            this.PlatformDirectory = string.Empty;
            this.SystemBMM = string.Empty;
            this.OutputBIT = "system.bit";
            this.LinuxKernelLocation = string.Empty;
            this.XilinxEDKDirectory = string.Empty;
            this.SystemPrefix = "system";

            this.LinuxHost = false;
            this.LocalSynthesis = true;
            this.SynthesisServer = null;
            this.EthernetDevice = string.Empty;
            this.EthernetMAC = string.Empty;
            this.DeviceTree = string.Empty;
            this.IPAddress = string.Empty;


            //_SSHExec = null;
            _SSHShell = null;
            _SSHXFer = null;
            _Processors = new List<FalconProcessorOS>();
            EventRecord = new ProjectEventRecorder();
        }
        #endregion

        #region Preparation/Cleanup
        /// <summary>
        /// Attempts to purge all previous outputs by removing the ./output directory entirely.
        /// </summary>
        /// <returns>SynthesisErrorCodes enum indicating whether the purge was successful (SYNTHESIS_OK), it failed 
        /// (SYNTHESIS_PURGE_FAIL), or it failed due to invalid remote authorization (SYNTHESIS_REMOTE_AUTH_FAIL). </returns>
        public SynthesisErrorCodes PurgeOutputDirectory(string filter)
        {
            try
            {
                PurgeLocalOutputDirectory();

                if (this.LocalSynthesis)
                {
                    if (Directory.Exists(this.OutputDirectory))
                        Directory.Delete(this.OutputDirectory, true);                    
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
                }
                else
                {
                    // Not local synthesis, create and connect to remote host
                    this.LinuxHost = this.SynthesisServer.LinuxHost;
                    SynthesisErrorCodes code = Login();
                    if (code != SynthesisErrorCodes.SYNTHESIS_OK)
                        return LogoutAndReturn(code);
                    
                    string response;
                    if (this.LinuxHost)
                    {
                        if (filter != string.Empty)
                        {
                            response = _SSHExec.RunCommand(String.Format("rm -rf {0}/{1}", this.OutputDirectory, filter));
                        }
                        else
                        {
                            response = _SSHExec.RunCommand(String.Format("rm -rf {0}", this.OutputDirectory));
                        }
                    }
                    else
                    {
                        if (filter != string.Empty)
                        {
                            response = _SSHExec.RunCommand(String.Format("rmdir /S /Q {0}/{1}", this.OutputDirectory, filter));
                        }
                        else
                        {
                            response = _SSHExec.RunCommand(String.Format("rmdir /S /Q {0}", this.OutputDirectory));
                        }
                    }
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return LogoutAndReturn(SynthesisErrorCodes.SYNTHESIS_PURGE_FAIL);
            }
        }

        private void PurgeLocalOutputDirectory()
        {
            FileInfo fiAssembly = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string targetDir = fiAssembly.Directory.FullName + "\\" + this.PlatformID + "_output";
            DirectoryInfo diTarget = new DirectoryInfo(targetDir);
            if (diTarget.Exists)
            {
                foreach (FileInfo fi in diTarget.GetFiles())
                {
                    try
                    {
                        if (fi.Name != this.PlatformID + ".log")
                            fi.Delete();
                    }
                    catch { }
                }
                foreach (DirectoryInfo di in diTarget.GetDirectories())
                {
                    try
                    {
                        di.Delete(true);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Builds and synthesizes the hardware platform.
        /// </summary>
        /// <param name="ForcedClean">Indicates whether previous work should be cleaned before beginning synthesis.</param>
        /// <returns>Returns a SynthesisErrorCodes enumerated value indicating the progress made during synthesis.</returns>
        public SynthesisErrorCodes BuildHardware(bool ForcedClean)
        {
            SynthesisErrorCodes returnCode = SynthesisErrorCodes.SYNTHESIS_OK;
            if (this.SkipSynthesis)
            {
                RaiseMessageEvent("Synthesis Skipped for this platform.");
                return returnCode;
            }

            if (!this.ReadyForSynthesis())
            {
                RaiseMessageEvent("Platform not ready for synthesis.");
                returnCode = (SynthesisErrorCodes.SYTHNESIS_NOT_READY);
            }

            try
            {
                OpenLog();
                if (!this.LocalSynthesis)
                {
                    // Not local synthesis, create and connect to remote host
                    this.LinuxHost = this.SynthesisServer.LinuxHost;
                    SynthesisErrorCodes code = Login();
                    if (code != SynthesisErrorCodes.SYNTHESIS_OK)
                    {
                        returnCode = code;
                    }
                }
                if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                {
                    if (VerifyMHSExists())
                    {
                        if (ForcedClean)
                            CleanSynthesisDirectory();

                        if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                            if (!CreateDirectories())
                                returnCode = (SynthesisErrorCodes.SYNTHESIS_CREATE_DIR_FAIL);

                        if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                        {
                            if (this.PerformFullSynthesis)
                            {
                                if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                    if (!GeneratePlatform())
                                        returnCode = (SynthesisErrorCodes.SYNTHESIS_PLATGEN_FAIL);
                                if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                    if (!SynthesizeHardware())
                                        returnCode = (SynthesisErrorCodes.SYNTHESIS_HW_SYNTH_FAIL);
                                if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                    if (!XFlow())
                                        return (SynthesisErrorCodes.SYNTHESIS_XFLOW_FAIL);
                                if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                    if (!GenerateBITFile())
                                        returnCode = (SynthesisErrorCodes.SYNTHESIS_BITGEN_FAIL);
                            }
                            if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                if (!CompileCodeletELFs())
                                    returnCode = (SynthesisErrorCodes.SYTHNESIS_CODELET_COMPILE_FAIL);
                            if (returnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                                if (!MergeCodeletELFs())
                                    returnCode = (SynthesisErrorCodes.SYTHNESIS_CODELET_MERGE_FAIL);
                        }
                    }
                    else
                    {
                        RaiseMessageEvent("Skipping Synthesis for {0} -- No MHS file found.", this.PlatformID);
                        returnCode = SynthesisErrorCodes.SYNTHESIS_SKIPPED;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                returnCode = (SynthesisErrorCodes.SYTHNESIS_UNSPECIFIED_ERROR);
            }
            finally
            {
                CloseLog();
            }
            return returnCode;
        }
        
        /// <summary>
        /// Copies the contents of the output directory to the local system.
        /// </summary>
        /// <returns>Returns a SynthesisErrorCodes enumerated value indicating the progress made during synthesis.</returns>
        public SynthesisErrorCodes DownloadOutputs()
        {
            //VerifyMSSExists();
            if (!CopyOutputToLocal())
                return LogoutAndReturn(SynthesisErrorCodes.SYNTHESIS_FILE_DOWNLOAD_FAIL);
            return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
        }
        
        /// <summary>
        /// Builds and compiles the software platform(s).
        /// </summary>
        /// <param name="ForcedClean">Indicates whether previous work should be cleaned before beginning compilation.</param>
        /// <returns>Returns a SynthesisErrorCodes enumerated value indicating the progress made during compilation.</returns>
        public SynthesisErrorCodes BuildSoftware(bool ForcedClean)
        {
            if (this.SkipSynthesis)
            {
                RaiseMessageEvent("Synthesis Skipped for this platform.");
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
            }
            if (!this.ReadyForCompilation())
            {
                return (SynthesisErrorCodes.SYTHNESIS_NOT_READY);
            }
            if (this.Processors.Count == 0)
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;

            try
            {
                OpenLog();
                if (!this.LocalSynthesis)
                {
                    // Not local synthesis, create and connect to remote host
                    this.LinuxHost = this.SynthesisServer.LinuxHost;
                    SynthesisErrorCodes code = Login();
                    if (code != SynthesisErrorCodes.SYNTHESIS_OK)
                    {
                        return code;
                    }
                }

                if (VerifyMHSExists())
                {
                    //VerifyMSSExists();
                    if (!CreateDirectories())
                        return (SynthesisErrorCodes.SYNTHESIS_CREATE_DIR_FAIL);
                    //VerifyMSSExists();
                    if (!PrepareSoftware())
                        return (SynthesisErrorCodes.SYNTHESIS_SOFTWARE_PREPARE_FAIL);
                    //VerifyMSSExists();
                    if (!GenerateLibraries())
                        return (SynthesisErrorCodes.SYNTHESIS_LIBGEN_FAIL);
                    //VerifyMSSExists();
                    if (!UpdateDTS())
                        return (SynthesisErrorCodes.SYNTHESIS_DTS_UPDATE_FAIL);
                    //VerifyMSSExists();
                    if (!CompileSoftware())
                        return (SynthesisErrorCodes.SYNTHESIS_COMPILE_FAIL);
                    //VerifyMSSExists();
                }
                else
                {
                    RaiseMessageEvent("Skipping Compilation for {0} -- No MHS file found.", this.PlatformID);
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_SKIPPED;
                }
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return LogoutAndReturn(SynthesisErrorCodes.SYTHNESIS_UNSPECIFIED_ERROR);
            }

        }
        
        private bool VerifyMHSExists()
        {
            try
            {
                if (this.LocalSynthesis)
                {
                    return (File.Exists(this.PlatformDirectory + this.SystemMHS));
                }
                else
                {
                    string listCommand;
                    string expectedResponse;
                    if (this.LinuxHost)
                    {
                        listCommand = String.Format("ls {0}{1}", this.PlatformDirectory, this.SystemMHS);
                        expectedResponse = String.Format("{0}{1}", this.PlatformDirectory, this.SystemMHS);
                    }
                    else
                    {
                        listCommand = String.Format("dir {0}{1}", this.PlatformDirectory, this.SystemMHS);
                        expectedResponse = String.Format(" {1}", this.PlatformDirectory, this.SystemMHS);
                    }
                    string Response = _SSHExec.RunCommand(listCommand);
                    return (Response.Contains(expectedResponse));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return false;
        }

        #region Cross-stage Utilities

        /// <summary>
        /// Reads the password for the locally specified synthesis server from the Console.  The password characters are not echoed to the console, but replaced by asterisks.
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

        private void OpenLog()
        {
            try 
            {
                if (_logWriter != null)
                    return;
                EventRecord.Open(_PathMan);
                string targetDir = _PathMan.GetPath("LocalProject") + "\\" + this.PlatformID + "\\output";
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                _logWriter = new StreamWriter(targetDir + "\\" + this.PlatformID + ".log");
                LogMessage(String.Format("Hardware Synthesis/Software Compilation for Platform {0} began at {1} on {2}", this.PlatformID, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()));
            }
            catch
            {
                _logWriter = null;
            }

        }
        private void CloseLog()
        {
            try
            {
                EventRecord.Close();
                if (_logWriter != null)
                {
                    LogMessage(String.Format("Hardware Synthesis/Software Compilation for Platform {0} terminated at {1} on {2}", this.PlatformID, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString()));
                    _logWriter.Close();
                }
            }
            catch
            {
            }
            finally
            {
                _logWriter = null;
            }
        }
        private void LogException(Exception ex)
        {
            try
            {
                if (_logWriter != null)
                {
                    _logWriter.WriteLine(String.Format("[{0} {1}]: EXCEPTION -- {2}", DateTime.Now.ToShortDateString().PadLeft(10), DateTime.Now.ToLongTimeString().PadLeft(8), ex.Message));
                    _logWriter.Flush();
                }
            }
            catch
            {
                _logWriter = null;
            }
        }
        private void LogMessage(string Message)
        {
            try
            {
                if (_logWriter != null)
                {
                    _logWriter.WriteLine(String.Format("[{0} {1}]: Message -- {2}", DateTime.Now.ToShortDateString().PadLeft(10), DateTime.Now.ToLongTimeString().PadLeft(8), Message));
                    _logWriter.Flush();
                }
            }
            catch
            {
                _logWriter = null;
            }
        }

        /// <summary>
        /// Attempts to establish a set of SSH connections to the remote synthesis server.  The user is prompted for a password at the console.
        /// </summary>
        /// <returns>A SynthesisErrorCodes value indicating whether the login was successful, and if not, why not.</returns>
        public SynthesisErrorCodes Login()
        {
            bool bReadPassword = true;
            try
            {
                bReadPassword = (this.SynthesisServer.Password == string.Empty);
            }
            catch
            {
                // Clear password on entry error - required reentry next time.
                if (this.SynthesisServer != null) this.SynthesisServer.Password = string.Empty;
            }
            if (bReadPassword)
            {
                try
                {
                    if (OnRequirePassword != null)
                    {
                        this.SynthesisServer.Password = OnRequirePassword(this.SynthesisServer.UserName, this.SynthesisServer.Address);
                    }
                    else
                    {
                        this.SynthesisServer.Password = ReadPassword(this.SynthesisServer.UserName, this.SynthesisServer.Address);
                    }
                }
                catch (Exception ex)
                {
                    // Clear password on connection error - required reentry next time.
                    this.SynthesisServer.Password = string.Empty;
                    LogException(ex);
                    return this.ReturnCode = SynthesisErrorCodes.SYTHNESIS_USER_ABORT;
                }
            }
            if (this.SynthesisServer != null) 
                if (this.SynthesisServer.Password == string.Empty)
                    return this.ReturnCode = SynthesisErrorCodes.SYTHNESIS_USER_ABORT;

            try
            {
                if ((_SSHExec == null) || (!_SSHExec.Connected))
                {
                    _SSHExec = new SshExec(this.SynthesisServer.Address, this.SynthesisServer.UserName, this.SynthesisServer.Password);
                    _SSHExec.Connect(this.SynthesisServer.Port);
                }
                if (!_SSHExec.Connected)
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            catch (Exception ex)
            {
                // Clear password on connection error - required reentry next time.
                this.SynthesisServer.Password = string.Empty;
                LogException(ex);
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            try
            {
                if ((_SSHXFer == null) || (!_SSHXFer.Connected))
                {
                    _SSHXFer = new Scp(this.SynthesisServer.Address, this.SynthesisServer.UserName, this.SynthesisServer.Password);
                    _SSHXFer.Connect(this.SynthesisServer.Port);
                }
                if (!_SSHXFer.Connected)
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            catch (Exception ex)
            {
                // Clear password on connection error - required reentry next time.
                this.SynthesisServer.Password = string.Empty;
                LogException(ex);
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            try
            {
                if ((_SSHShell == null) || (!_SSHShell.Connected))
                {
                    _SSHShell = new SshShell(this.SynthesisServer.Address, this.SynthesisServer.UserName, this.SynthesisServer.Password);
                    _SSHShell.Connect(this.SynthesisServer.Port);
                    _SSHShell.ExpectPattern = "\n";
                    //_SSHShell.GetStream();
                }
                if (!_SSHShell.Connected)
                    return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            catch (Exception ex)
            {
                // Clear password on connection error - required reentry next time.
                this.SynthesisServer.Password = string.Empty;
                LogException(ex);
                return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
            }
            return this.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
        }
        /// <summary>
        /// Attempts to forcefully disconnect the remote SSH sessions if they have been connected.   Upon completion, it must be assumed that all connections have 
        /// been severed and Login() must be called again to establish connectivity with the remote synthesis server.
        /// </summary>
        public void Logout()
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
                if (_SSHShell != null)
                {
                    if (_SSHShell.Connected)
                        _SSHShell.Close();
                    _SSHShell = null;
                }
            }
            finally
            {
                CloseLog();
            }
        }

        private static string GetPathShortName(string Path)
        {
            string name;
            if (Path.EndsWith("/"))
                Path = Path.Substring(0, Path.Length - 1);
            if (Path.EndsWith("\\"))
                Path = Path.Substring(0, Path.Length - 1);

            name = Path.Replace("\\", " ");
            name = name.Replace("/", " ");
            string[] hierarchy = name.Split(' ');
            if (hierarchy.Length > 1)
                name = hierarchy[hierarchy.Length - 1].Trim();
            return name;
        }
        private string GetComponentInstance(string MHSFile, string ComponentType)
        {
            StreamReader reader = new StreamReader(MHSFile);
            string inLine = string.Empty;
            bool bFoundBlock = false;
            while (!reader.EndOfStream)
            {
                inLine = reader.ReadLine();
                if (inLine.StartsWith("BEGIN " + ComponentType))
                    bFoundBlock = true;
                else
                {
                    if (bFoundBlock && inLine.Contains("PARAMETER INSTANCE"))
                    {
                        string inst = inLine.Replace("PARAMETER INSTANCE = ", string.Empty).Trim();
                        reader.Close();
                        return inst;
                    }
                }
            }
            reader.Close();
            return string.Empty;
        }
        private SynthesisErrorCodes LogoutAndReturn(SynthesisErrorCodes code)
        {
            Logout();
            return code;
        }
        
        private static string ConvertToCygDriveNotation(string path)
        {
            if (!path.Contains(":"))
                return path;
            string cygPath = path.Replace("\\", "/");
            cygPath = (cygPath.Substring(0, 1).ToLower()) + cygPath.Substring(1);
            cygPath = "/cygdrive/" + cygPath.Replace(":", string.Empty);
            return cygPath;
        }

        private void MoveFileToOutput(string FilePath, string prefix)
        {
            try
            {
                string shortName = prefix + "_" + GetPathShortName(FilePath);

                if (this.LocalSynthesis)
                {
                    if (File.Exists(this.OutputDirectory + shortName))
                        File.Delete(this.OutputDirectory + shortName);

                    File.Move(FilePath, this.OutputDirectory + shortName);
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        _SSHShell.WriteLine(String.Format("mv {0} {1}{2}", FilePath, this.OutputDirectory, shortName));
                        _SSHShell.Expect();
                    }
                    else
                    {
                        _SSHShell.WriteLine(String.Format("move /Y {0} {1}{2}", FilePath, this.OutputDirectory, shortName));
                        _SSHShell.Expect();
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        private void MoveFilesToOutput(string FilePath)
        {
            try
            {
                if (this.LocalSynthesis)
                {
                    string filesDir = FilePath.Substring(0, FilePath.LastIndexOf("\\") - 1);
					foreach (FileInfo fi in new DirectoryInfo(filesDir).GetFiles())
					{
                        fi.CopyTo(this.OutputDirectory + fi.Name);
					}					
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        _SSHExec.RunCommand(String.Format("cp {0} {1}", FilePath, this.OutputDirectory));
                    }
                    else
                    {
                        _SSHExec.RunCommand(String.Format("xcopy /E /Y /H /R {0} {1}", FilePath, this.OutputDirectory));
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        private void DirectoryCopy(string SourceDirectory, string DestinationDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(SourceDirectory);
            DirectoryInfo diDest = new DirectoryInfo(DestinationDirectory);
            if (!diDest.Exists)
                diDest.Create();

            foreach(FileInfo fi in diSource.GetFiles())
            {
                fi.CopyTo(diDest.FullName + "\\" + fi.Name);
            }
            foreach(DirectoryInfo di in diSource.GetDirectories())
            {
                DirectoryCopy(di.FullName, diDest.FullName);
            }
        }

        private bool CopyOutputToLocal()
        {
            RaiseMessageEvent("Copying output files for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Downloading output files starting.", this.PlatformID));
            try
            {
                MoveFilesToOutput(this.PlatformDirectory + "*.xrpt");
                MoveFilesToOutput(this.PlatformDirectory + "*.log");

                MoveFilesToOutput(this.SynthesisDirectory + "*.xrpt");
                MoveFilesToOutput(this.SynthesisDirectory + "*.srp");
                MoveFilesToOutput(this.SynthesisDirectory + "*.log");

                MoveFilesToOutput(this.ImplementationDirectory + "*.xrpt");
                MoveFilesToOutput(this.ImplementationDirectory + "*.mrp");
                MoveFilesToOutput(this.ImplementationDirectory + "*.par");
                MoveFilesToOutput(this.ImplementationDirectory + "*.blc");
                MoveFilesToOutput(this.ImplementationDirectory + "*.bld");
                MoveFilesToOutput(this.ImplementationDirectory + "*.log");
                MoveFilesToOutput(this.ImplementationDirectory + "*.bmm");
                MoveFilesToOutput(this.ImplementationDirectory + "*.unroutes");

                string targetDir = _PathMan.GetPath("LocalProject") + "\\" + this.PlatformID + "\\output";
                if (this.LocalSynthesis)
                {
                    if (this.OutputDirectory != targetDir)
                        DirectoryCopy(this.OutputDirectory, targetDir);
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        string remoteSystem = String.Format("{0}system", this.PlatformDirectory);
                        string localSystem = _PathMan.GetPath("LocalProject") + "\\" + this.PlatformID + "\\system";
                        string sourceFile = String.Format("{0}{1}.tar", this.OutputDirectory, this.PlatformID);
                        string targetFile = String.Format("{0}\\{1}.tar", targetDir, this.PlatformID);
                        string sourceBSPFile = String.Format("{0}bsp.tar", this.PlatformDirectory);
                        string targetBSPFile = String.Format("{0}\\bsp.tar", _PathMan.GetPath("LocalProject") + "\\" + this.PlatformID);

                        _SSHExec.RunCommand(String.Format("rm {0}{1}.tar", this.OutputDirectory, this.PlatformID));
                        _SSHExec.RunCommand(String.Format("cd {0}; tar cvf {1}.tar *", this.OutputDirectory, this.PlatformID));

                        _SSHExec.RunCommand(String.Format("cd {0}; rm bsp.tar; tar cvf bsp.tar bsp;", this.PlatformDirectory));

                        _SSHXFer.From(sourceFile, targetFile);
                        _SSHXFer.From(sourceBSPFile, targetBSPFile);
                        try
                        {
                            _SSHXFer.From(remoteSystem + ".mhs", localSystem + ".mhs");
                        }
                        catch { }
                        try
                        {
                            _SSHXFer.From(remoteSystem + ".mss", localSystem + ".mss");
                        }
                        catch { }
                        if (File.Exists(targetFile))
                        {
                            FileStream fs = new FileStream(targetFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            //Stream gzipStream = new GZipInputStream(fs);
                            TarArchive tar = TarArchive.CreateInputTarArchive(fs);
                            tar.ProgressMessageEvent += new ProgressMessageHandler(tar_ProgressMessageEvent);
                            RaiseMessageEvent("\n\tStarting Extraction...");
                            LogMessage(String.Format("Platform {0} Starting Extraction...", this.PlatformID));
                            try
                            {
                                tar.ExtractContents(targetDir);
                                RaiseMessageEvent("\tComplete!");
                                LogMessage(String.Format("Platform {0} Extraction Complete!", this.PlatformID));
                            }
                            catch (Exception ex)
                            {
                                RaiseMessageEvent("\tERROR!");
                                LogMessage(String.Format("Platform {0} Extraction ERROR!", this.PlatformID));
                                RaiseMessageEvent(ex.Message);
                            }
                            finally
                            {
                                tar.Close();
                                //gzipStream.Close();
                                fs.Close();
                            }
                            try
                            {
                                File.Delete(targetFile);
                            }
                            catch { }
                        }
                        else
                        {
                            throw new Exception("Error downloading output archive.");
                        }

                        if (File.Exists(targetBSPFile))
                        {
                            FileStream fs = new FileStream(targetBSPFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            //Stream gzipStream = new GZipInputStream(fs);
                            TarArchive tar = TarArchive.CreateInputTarArchive(fs);
                            tar.ExtractContents(_PathMan.GetPath("LocalProject") + "\\" + this.PlatformID);
                            tar.Close();
                            //gzipStream.Close();
                            fs.Close();
                            try
                            {
                                File.Delete(targetBSPFile);
                            }
                            catch { }
                        }
                        else
                        {
                            throw new Exception("Error downloading output archive.");
                        }

                        if (!Directory.Exists(_PathMan["LocalOutput"]))
                            Directory.CreateDirectory(_PathMan["LocalOutput"]);
                        // Copy bit/elf files to local output
                        foreach (string file in Directory.GetFiles(targetDir, "*.bit"))
                        {
                            FileInfo fi = new FileInfo(file);
                            fi.CopyTo(_PathMan["LocalOutput"] + "\\" + fi.Name, true);
                        }
                        foreach (string file in Directory.GetFiles(targetDir, "*.elf"))
                        {
                            FileInfo fi = new FileInfo(file);
                            fi.CopyTo(_PathMan["LocalOutput"] + "\\" + fi.Name, true);
                        }
                    }
                    else
                    {
                        throw new Exception("Remote compression not implemented on Windows host.");
                    }
                }
                RaiseMessageEvent("Complete!");
                LogMessage(String.Format("Platform {0} Downloading output files complete.", this.PlatformID));
                return true;
            }
            catch (Exception ex)
            {
                RaiseMessageEvent("ERROR!");
                LogMessage(String.Format("Platform {0} Downloading output files failed.", this.PlatformID));
                LogException(ex);
                return false;
            }
        }

        void tar_ProgressMessageEvent(TarArchive archive, TarEntry entry, string message)
        {
            string OutputMessage = String.Format("{0}", entry.Name);
            //RaiseMessageEvent("\t{0}", OutputMessage);
            LogMessage(String.Format("\t{0}", OutputMessage));
        }
        #endregion

        #region Synthesis/Compilation Preparation

        /// <summary>
        /// Iterates over all of the PCores assigned to this FPGA and cleans the project of any files related
        /// to those cores that have been tagged for Selective Purge
        /// </summary>
        /// <param name="DoSelectiveClean">Indicates whether the clean should be selective, or full.</param>
        public void PerformClean(bool DoSelectiveClean)
        {
            if (!DoSelectiveClean)
            {
                PerformCoreClean(null);
            }
            else
            {
                foreach (ComponentCore CompCore in this.AssignedPCores)
                {
                    if (CompCore.PurgeBeforeSynthesis)
                    {
                        PerformCoreClean(CompCore);
                    }
                }
            }
        }
        private void PerformCoreClean(ComponentCore CompCore)
        {
            string CleanProjectCommand = string.Empty;
            string CleanHDLCommand = string.Empty;
            string CleanImplementationCommand = string.Empty;
            string CleanImplementationCacheCommand = string.Empty;
            string CleanSynthesisCommand = string.Empty;
            string CleanOutputCommand = string.Empty;

            if (CompCore == null)
            {
                RaiseMessageEvent("Cleaning ALL old synthesis files for FPGA: {0}", this.PlatformID);
                // Clean HDL
                CleanHDLCommand = String.Format("rm -rf {0}", this.HDLDirectory);
                // Clean Implementation
                CleanImplementationCommand = String.Format("rm -rf {0}", this.ImplementationDirectory);
                // Clean Synthesis
                CleanSynthesisCommand = String.Format("rm -rf {0}", this.SynthesisDirectory);
                // Clean Output
                CleanOutputCommand = String.Format("rm -rf {0}", this.OutputDirectory);
            }
            else
            {
                RaiseMessageEvent("Cleaning old synthesis files for: {0}", CompCore.CoreInstance);

                // Clean Project Root
                CleanProjectCommand = String.Format("rm {0}{1}*", this.PlatformDirectory, CompCore.CoreInstance);
                // Clean HDL
                CleanHDLCommand = String.Format("rm {0}{1}*", this.HDLDirectory, CompCore.CoreInstance);
                // Clean Implementation
                CleanImplementationCommand = String.Format("rm -rf {0}{1}*", this.ImplementationDirectory, CompCore.CoreInstance);
                // Clean Implementation Cache
                CleanImplementationCacheCommand = String.Format("rm -rf {0}cache/{1}*", this.ImplementationDirectory, CompCore.CoreInstance);
                // Clean Synthesis
                CleanSynthesisCommand = String.Format("rm -rf {0}{1}*", this.SynthesisDirectory, CompCore.CoreInstance);
                // Clean Output
                CleanOutputCommand = String.Format("rm -rf {0}{1}*", this.OutputDirectory, CompCore.CoreInstance);
            }

            if (CompCore != null) _SSHExec.RunCommand(CleanProjectCommand);
            _SSHExec.RunCommand(CleanHDLCommand);
            _SSHExec.RunCommand(CleanImplementationCommand);
            if (CompCore != null) _SSHExec.RunCommand(CleanImplementationCacheCommand);
            _SSHExec.RunCommand(CleanSynthesisCommand);
            _SSHExec.RunCommand(CleanOutputCommand);

            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanProjectCommand));
            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanHDLCommand));
            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanImplementationCommand));
            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanImplementationCacheCommand));
            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanSynthesisCommand));
            //RaiseMessageEvent(String.Format("DEBUG: {0}", CleanOutputCommand));
        }

        /// <summary>
        /// Determines if the loaded platform is ready for synthesis and compilation.
        /// </summary>
        /// <returns>True if the platform is ready, False otherwise.</returns>
        public bool Ready()
        {
            bool bReady = true;
            bReady = bReady && (this.PlatformDirectory != string.Empty) && (this.PlatformID != string.Empty);
            bReady = ReadyForSynthesis() && ReadyForCompilation();
            return bReady;
        }

        /// <summary>
        /// Determines if the loaded platform is ready for hardware synthesis ONLY.
        /// </summary>
        /// <returns>True if the platform is ready, False otherwise.</returns>        
        public bool ReadyForSynthesis()
        {
            bool bReady = true;

            bReady = (bReady) &&
                                 (this.PlatformID != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {0}", "Platform ID"));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatformDirectory != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Directory", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemUCF != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System UCF", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemMHS != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System MHS", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemMSS != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System MSS", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.OutputBIT != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Output BIT File", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartPackage != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Package", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartSize != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Size", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartSpeedGrade != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Speed Grade", this.PlatformID));
                return false;
            }
            if (!((this.BitGenScriptPath == null) || (this.BitGenScriptPath == string.Empty)))
            {
                if (File.Exists(this.BitGenScriptPath))
                {
                    //RaiseMessageEvent(String.Format("Using platform-specified bitfile generation script: {0}.", this.BitGenScriptPath));
                }
                else
                {
                    RaiseMessageEvent(String.Format("Missing required information: Specified bitfile generation script was not found locally: {0}.  Synthesis aborted.", this.BitGenScriptPath));
                    return false;
                }
            }
            else
            {
                //RaiseMessageEvent(String.Format("Using default bitfile generation script."));
            }
            if (!this.LocalSynthesis)
            {
                bReady = (bReady) && (this.SynthesisServer != null);
                if (this.SynthesisServer != null)
                {
                    bReady = (bReady) &&
                                         (this.SynthesisServer.Address != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Synthesis Server Address", this.PlatformID));
                        return false;
                    }
                    bReady = (bReady) &&
                                         (this.SynthesisServer.UserName != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Synthesis Server UserName", this.PlatformID));
                        return false;
                    }
                }
            }
            return bReady;
        }
        /// <summary>
        /// Determines if the loaded platform is ready for software compilation ONLY.
        /// </summary>
        /// <returns>True if the platform is ready, False otherwise.</returns>        
        public bool ReadyForCompilation()
        {
            bool bReady = true;

            bReady = (bReady) &&
                                 (this.PlatformID != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {0}", "Platform ID"));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatformDirectory != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Directory", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemUCF != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System UCF", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemMHS != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System MHS", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.SystemMSS != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "System MSS", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.OutputBIT != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Output BIT File", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartPackage != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Package", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartSize != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Size", this.PlatformID));
                return false;
            }
            bReady = (bReady) &&
                                 (this.PlatGenXPartSpeedGrade != string.Empty);
            if (!bReady)
            {
                RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Platform Part Speed Grade", this.PlatformID));
                return false;
            }
            if (_Processors.Count > 0)
            {
                foreach (FalconProcessorOS fpOS in _Processors)
                {
                    bReady = (bReady) &&
                                      (fpOS.Instance != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required processor information information: {1}.{0}", "Processor Instance", this.PlatformID));
                        return false;
                    }
                    bReady = (bReady) &&
                                      (fpOS.ConsoleDevice != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{2}.{0}", "Processor Console Device", this.PlatformID, fpOS.Instance));
                        return false;
                    }
                    bReady = (bReady) &&
                                      (fpOS.OutputELF != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{2}.{0}", "Processor Output ELF File", this.PlatformID, fpOS.Instance));
                        return false;
                    }
                    if (fpOS.OS == SystemProcessorOS.Linux)
                    {
                        if (fpOS.MakeConfig == string.Empty)
                        {
                            RaiseMessageEvent(String.Format("Missing required information: {1}.{2}.{0}", "Processor 'make config'", this.PlatformID, fpOS.Instance));
                            return false;
                        }
                        if (fpOS.DTSFile == string.Empty)
                        {
                            RaiseMessageEvent(String.Format("Missing required information: {1}.{2}.{0}", "Processor DTS File", this.PlatformID, fpOS.Instance));
                            return false;
                        }
                        if (this.DeviceTree == string.Empty)
                        {
                            RaiseMessageEvent(String.Format("Missing required information: Path '{0}'", "DeviceTreeLocation"));
                            return false;
                        }
                        if (this.ELDKLocation == string.Empty)
                        {
                            RaiseMessageEvent(String.Format("Missing required information: Path '{0}'", "ELDKLocation"));
                            return false;
                        }
                        if (fpOS.LinuxKernelSource == string.Empty)
                        {
                            RaiseMessageEvent(String.Format("Missing required information: Path '{0}'", "LinuxKernelLocation"));
                            return false;
                        }
                        if ((fpOS.Type.ToLower() == "microblaze") && (this.MBGNULocation == string.Empty))
                        {
                            RaiseMessageEvent(String.Format("Missing required information: Path '{0}'", "MicroblazeGNUTools"));
                            return false;
                        }
                        // NOT IMPLEMENTED: Cannot compile linux kernel on Windows Machine
                        if ((this.LocalSynthesis) || (!this.LinuxHost))
                            bReady = false;
                    }
                }
            }

            if (!this.LocalSynthesis)
            {
                bReady = (bReady) && (this.SynthesisServer != null);
                if (this.SynthesisServer != null)
                {
                    bReady = (bReady) &&
                                         (this.SynthesisServer.Address != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Synthesis Server Address", this.PlatformID));
                        return false;
                    }
                    bReady = (bReady) &&
                                         (this.SynthesisServer.UserName != string.Empty);
                    if (!bReady)
                    {
                        RaiseMessageEvent(String.Format("Missing required information: {1}.{0}", "Synthesis Server UserName", this.PlatformID));
                        return false;
                    }
                }
            }
            return bReady;
        }
        private bool CreateDirectories()
        {
            RaiseMessageEvent("Verifying directory structure for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Verifying directory structure starting.", this.PlatformID));
            try
            {
                if (this.LocalSynthesis)
                {
                    if (Directory.Exists(this.PlatformDirectory))
                    {
                        if (!Directory.Exists(this.BSPDirectory))
                            Directory.CreateDirectory(this.BSPDirectory);
                        if (!Directory.Exists(this.DataDirectory))
                            Directory.CreateDirectory(this.DataDirectory);
                        if (!Directory.Exists(this.ETCDirectory))
                            Directory.CreateDirectory(this.ETCDirectory);
                        if (!Directory.Exists(this.HDLDirectory))
                            Directory.CreateDirectory(this.HDLDirectory);
                        if (!Directory.Exists(this.ImplementationDirectory))
                            Directory.CreateDirectory(this.ImplementationDirectory);
                        if (!Directory.Exists(this.PCoresDirectory))
                            Directory.CreateDirectory(this.PCoresDirectory);
                        if (!Directory.Exists(this.SynthesisDirectory))
                            Directory.CreateDirectory(this.SynthesisDirectory);
                        if (!Directory.Exists(this.LIBDirectory))
                            Directory.CreateDirectory(this.LIBDirectory);
                        if (!Directory.Exists(this.OutputDirectory))
                            Directory.CreateDirectory(this.OutputDirectory);
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Verifying directory structure complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Verifying directory structure Failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        // SSH commands to verify/create directory structure
                        // Create script, send it, and execute it
                        string ScriptFile = String.Format("createdirs{0}.sh", this.PlatformID);
                        FileInfo FIScript = new FileInfo(ScriptFile);
                        StreamWriter writer = new StreamWriter(ScriptFile, false);
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n echo \"{1}\"\n exit 1\nfi\n", this.PlatformDirectory, FINISH_ERROR));
                        writer.Write(String.Format("cd {0}\n", this.DataDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.DataDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.ETCDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.HDLDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.ImplementationDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.PCoresDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.SynthesisDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.LIBDirectory));
                        writer.Write(String.Format("if [ ! -d {0} ]; then\n mkdir {0}\nfi\n", this.OutputDirectory));
                        writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                        writer.Close();

                        _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                        FIScript.Delete();

                        string CommandResponse;
                        _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, ScriptFile));
                        _SSHShell.Expect();
                        _SSHShell.WriteLine(String.Format("{2} {0}{1}", this.PlatformDirectory, ScriptFile, LINUX_COMMAND_SHELL));
                        CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                        if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                        {
                            RaiseMessageEvent("Complete!");
                            LogMessage(String.Format("Platform {0} Verifying directory structure complete.", this.PlatformID));
                            return true;
                        }
                        else
                        {
                            RaiseMessageEvent("ERROR!");
                            LogMessage(String.Format("Platform {0} Verifying directory structure Failed.", this.PlatformID));
                            return false;
                        }
                    }
                    else
                    {
                        // SSH commands to verify/create directory structure
                        // Create script, send it, and execute it
                        string ScriptFile = String.Format("createdirs{0}.bat", this.PlatformID);
                        FileInfo FIScript = new FileInfo(ScriptFile);
                        StreamWriter writer = new StreamWriter(ScriptFile, false);
                        writer.WriteLine(String.Format("@ECHO OFF"));
                        writer.WriteLine(String.Format("IF NOT EXIST {0} GOTO :exit_fail", this.PlatformDirectory));
                        writer.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                        writer.WriteLine(String.Format(""));
                        writer.WriteLine(String.Format(":data"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :etc", this.DataDirectory));
                        writer.WriteLine(String.Format("md {0}", this.DataDirectory));
                        writer.WriteLine(String.Format(":etc"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :hdl", this.ETCDirectory));
                        writer.WriteLine(String.Format("md {0}", this.ETCDirectory));
                        writer.WriteLine(String.Format(":hdl"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :implementation", this.HDLDirectory));
                        writer.WriteLine(String.Format("md {0}", this.HDLDirectory));
                        writer.WriteLine(String.Format(":implementation"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :pcores", this.ImplementationDirectory));
                        writer.WriteLine(String.Format("md {0}", this.ImplementationDirectory));
                        writer.WriteLine(String.Format(":pcores"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :synthesis", this.PCoresDirectory));
                        writer.WriteLine(String.Format("md {0}", this.PCoresDirectory));
                        writer.WriteLine(String.Format(":synthesis"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :lib", this.SynthesisDirectory));
                        writer.WriteLine(String.Format("md {0}", this.SynthesisDirectory));
                        writer.WriteLine(String.Format(":lib"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :output", this.LIBDirectory));
                        writer.WriteLine(String.Format("md {0}", this.LIBDirectory));
                        writer.WriteLine(String.Format(":output"));
                        writer.WriteLine(String.Format("IF EXIST {0} GOTO :exit_success", this.OutputDirectory));
                        writer.WriteLine(String.Format("md {0}", this.OutputDirectory));
                        writer.WriteLine(String.Format(""));
                        writer.WriteLine(String.Format(":exit_success"));
                        writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                        writer.WriteLine(String.Format("exit /b 0"));
                        writer.WriteLine(String.Format(":exit_fail"));
                        writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                        writer.WriteLine(String.Format("exit /b 1"));
                        writer.Close();

                        _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                        FIScript.Delete();

                        string CommandResponse;
                        _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                        _SSHShell.Expect();
                        _SSHShell.WriteLine(String.Format("{0}{1}", this.PlatformDirectory, ScriptFile));
                        CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                        if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                        {
                            RaiseMessageEvent("Complete!");
                            LogMessage(String.Format("Platform {0} Verifying directory structure complete.", this.PlatformID));
                            return true;
                        }
                        else
                        {
                            RaiseMessageEvent("ERROR!");
                            LogMessage(String.Format("Platform {0} Verifying directory structure Failed.", this.PlatformID));
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                RaiseMessageEvent("ERROR!");
                return false;
            }
        }
        #endregion

        #endregion

        #region /********************* Hardware Synthesis Start ********************************/

        #region Platform Generation (platgen)
        private bool GeneratePlatform()
        {
            string SearchPathIncludes = string.Empty;
            string[] SynthPCoreRepos = _PathMan["GlobalSynthPCores"].Split(';');
            int includeCount = 0;
            foreach (string Repo in SynthPCoreRepos)
            {
                string SynthFolder = Repo.Trim();
                if (SynthFolder != string.Empty)
                {
                    SearchPathIncludes = String.Format("{0} -lp {1}", SearchPathIncludes, SynthFolder).Trim();
                    includeCount++;
                }
            }
            if (includeCount == 0)
                SearchPathIncludes = string.Empty;

            string PGArgs = string.Format("-p {0} -lang {1} {2} {3} -intstyle xflow", 
                this.PlatGenXPartName, 
                this.PlatGenHDLLang.ToString().ToLower(), 
                this.SystemMHS, 
                SearchPathIncludes);
            RaiseMessageEvent("Starting Platform generation for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Platform generation starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the platform root (./) directory
                Directory.SetCurrentDirectory(this.PlatformDirectory);
                Process platgen = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_PLATGEN;
                psi.Arguments = PGArgs;
                psi.WorkingDirectory = this.PlatformDirectory;
                psi.UseShellExecute = false;
                platgen.StartInfo = psi;
                platgen.Start();

                // Wait for platgen to terminate
                platgen.WaitForExit();
                MoveFileToOutput(this.PlatformDirectory + "platgen.log", this.PlatformID);
                if (platgen.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Platform generation complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} Platform generation failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("platgen{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.PlatformDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_PLATGEN, PGArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_PLATGEN, PGArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                    FIScript.Delete();
                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}_output.log", this.OutputDirectory, this.PlatformID, "PLATGEN");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.PlatformDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    MoveFileToOutput(this.PlatformDirectory + "platgen.log", this.PlatformID);
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Platform generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Platform generation failed.", this.PlatformID));
                        return false;
                    }                    
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("platgen{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_PLATGEN, PGArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_PLATGEN, PGArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                    FIScript.Delete();
                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    _SSHShell.Expect();
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    MoveFileToOutput(this.PlatformDirectory + "platgen.log", this.PlatformID);
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Platform generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Platform generation failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        #endregion

        #region Hardware Core / System Synthesis (xst, ngcbuild)
        private bool SynthesizeHardware()
        {
            string XSTScript = CreateXSTScript();
            FileInfo FIXSTScript = new FileInfo(XSTScript);
            string XSTArgs = "-ifn " + XSTScript + " -intstyle xflow";
            RaiseMessageEvent("Starting XST Synthesis for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} XST Synthesis starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./synthesis directory
                PurgeXSTDirectory();
                Directory.SetCurrentDirectory(this.SynthesisDirectory);
                Process xst = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_XST;
                psi.Arguments = XSTArgs;
                psi.WorkingDirectory = this.SynthesisDirectory;
                psi.UseShellExecute = false;
                xst.StartInfo = psi;
                xst.Start();

                // Wait for xst to terminate
                xst.WaitForExit();
                if (xst.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} XST Synthesis complete.", this.PlatformID));
                    return SynthesizeSystem();
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} XST Synthesis failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("xst{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.SynthesisDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_XST, XSTArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_XST, XSTArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIXSTScript.FullName, this.SynthesisDirectory + XSTScript);
                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIXSTScript.Delete();
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.SynthesisDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "XST");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.SynthesisDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} XST Synthesis complete.", this.PlatformID));
                        return SynthesizeSystem();
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} XST Synthesis failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("xst{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_XST, XSTArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_XST, XSTArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIXSTScript.FullName, this.SynthesisDirectory + XSTScript);
                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIXSTScript.Delete();
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.SynthesisDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} XST Synthesis complete.", this.PlatformID));
                        return SynthesizeSystem();
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} XST Synthesis failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private void PurgeXSTDirectory()
        {
            try
            {
                if (this.LocalSynthesis)
                {
                    if (Directory.Exists(this.SynthesisDirectory + "xst"))
                        Directory.Delete(this.SynthesisDirectory + "xst");
                }
                else
                {
                    string response;
                    if (this.LinuxHost)
                    {
                        response = _SSHExec.RunCommand(String.Format("rm -rf {0}", this.SynthesisDirectory + "xst"));
                    }
                    else
                    {
                        response = _SSHExec.RunCommand(String.Format("rmdir /S /Q {0}", this.SynthesisDirectory + "xst"));
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            finally { }
        }
        private void CleanSynthesisDirectory()
        {
            if (!this.ForceClean)
                return;
            RaiseMessageEvent("Cleaning old synthesis files for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Cleaning old synthesis files started.", this.PlatformID));
            try
            {
                if (this.LocalSynthesis)
                {
                    if (Directory.Exists(this.ImplementationDirectory))
                        Directory.Delete(this.ImplementationDirectory);
                    if (Directory.Exists(this.SynthesisDirectory))
                        Directory.Delete(this.SynthesisDirectory);
                }
                else
                {
                    string response;
                    if (this.LinuxHost)
                    {
                        response = _SSHExec.RunCommand(String.Format("rm -rf {0}", this.ImplementationDirectory));
                        response = _SSHExec.RunCommand(String.Format("rm -rf {0}", this.SynthesisDirectory));
                    }
                    else
                    {
                        response = _SSHExec.RunCommand(String.Format("rmdir /S /Q {0}", this.ImplementationDirectory));
                        response = _SSHExec.RunCommand(String.Format("rmdir /S /Q {0}", this.SynthesisDirectory));
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            finally 
            {
                RaiseMessageEvent("Complete!");
                LogMessage(String.Format("Platform {0} Cleaning old synthesis files complete.", this.PlatformID));
            }
        }

        private string CreateXSTScript()
        {
            string SearchPathIncludes = string.Empty;
            string[] SynthPCoreRepos = _PathMan["GlobalSynthPCores"].Split(';');
            foreach (string Repo in SynthPCoreRepos)
            {
                string SynthFolder = Repo.Trim();
                SearchPathIncludes = String.Format("{0} \"{1}\"", SearchPathIncludes, SynthFolder).Trim();
            }

            string XSTScriptFile = "system_xst.scr";
            string XSTScriptPath = XSTScriptFile;
            if (File.Exists(XSTScriptPath))
                File.Delete(XSTScriptPath);
            StreamWriter writer = new StreamWriter(XSTScriptPath, false);

            writer.WriteLine("run");
            writer.WriteLine("-opt_mode speed");
            writer.WriteLine("-netlist_hierarchy as_optimized");
            writer.WriteLine("-opt_level 1");
            writer.WriteLine("-p " + this.PlatGenXPartName);
            writer.WriteLine("-top system");
            writer.WriteLine("-ifmt MIXED");
            //writer.WriteLine("-fsm_extract NO");
            writer.WriteLine("-ifn system_xst.prj");
            writer.WriteLine("-equivalent_register_removal no");
            writer.WriteLine("-ofn ../implementation/system.ngc");
            writer.WriteLine("-hierarchy_separator /");
            writer.WriteLine("-iobuf YES");
            writer.WriteLine("-max_fanout 10000");
            writer.WriteLine("-sd {../implementation}");
            writer.WriteLine("-vlgincdir {" + SearchPathIncludes + " \"" + this.PCoresDirectory + "\" \"" + this.XilinxProcessorIPLibPCoresDirectory + "\" }");

            writer.Close();
            return XSTScriptFile;
        }

        private bool SynthesizeSystem()
        {
            string NGCArgs = "./system.ngc ../implementation/system.ngc -sd ../implementation -i";
            RaiseMessageEvent("Starting NGCBuild-System for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} NGCBuild-System starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./synthesis directory
                MoveSystemNGC();
                PurgeXSTDirectory();
                Directory.SetCurrentDirectory(this.SynthesisDirectory);
                Process ngc = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_NGCBUILD;
                psi.Arguments = NGCArgs;
                psi.WorkingDirectory = this.SynthesisDirectory;
                psi.UseShellExecute = false;
                ngc.StartInfo = psi;
                ngc.Start();

                // Wait for ngcbuild to terminate
                ngc.WaitForExit();
                if (ngc.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} NGCBuild-System complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} NGCBuild-System failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("ngcbuild{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.SynthesisDirectory));
                    writer.Write(String.Format("echo mv {0}{1} {2}{3}\n", this.ImplementationDirectory, "system.ngc", this.SynthesisDirectory, "system.ngc"));
                    writer.Write(String.Format("mv {0}{1} {2}{3}\n", this.ImplementationDirectory, "system.ngc", this.SynthesisDirectory, "system.ngc"));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_NGCBUILD, NGCArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_NGCBUILD, NGCArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.SynthesisDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "NGCBUILD");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.SynthesisDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} NGCBuild-System complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} NGCBuild-System failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("ngcbuild{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_NGCBUILD, NGCArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_NGCBUILD, NGCArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.SynthesisDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} NGCBuild-System complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} NGCBuild-System failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private bool MoveSystemNGC()
        {
            try
            {
                if (File.Exists(this.ImplementationDirectory + "system.ngc"))
                    File.Move(this.ImplementationDirectory + "system.ngc", this.SynthesisDirectory + "system.ngc");
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return false;
        }
        #endregion

        #region XFlow (NGDBuild -> Map -> PAR)
        private bool XFlow()
        {
            //VerifyMSSExists();
            if (!XFlow_NGDBuild())
                return false;
            //VerifyMSSExists();
            if (!XFlow_Map())
                return false;
            //VerifyMSSExists();
            if (!XFlow_PAR())
                return false;
            //VerifyMSSExists();
            return true;
        }
        private bool XFlow_NGDBuild()
        {
            int PE_Count = 0;
            FileInfo BMM = AssembleGlobalBMM(ref PE_Count);
            if (PE_Count == 0)
            {
                this.SystemBMM = string.Empty;
            }
            string NGDArgs = String.Format("-p {0} -nt timestamp \"{1}system.ngc\" {2} -uc \"{3}\" system.ngd -intstyle xflow",
                                               this.PlatGenXPartName,
                                               this.ImplementationDirectory,
                                               (this.SystemBMM != string.Empty ? "-bm " + this.ImplementationDirectory + this.SystemBMM : string.Empty),
                                               this.DataDirectory + this.SystemUCF);
            RaiseMessageEvent("Starting NGDBuild for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} NGDBuild starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./implementation directory
                Directory.SetCurrentDirectory(this.ImplementationDirectory);
                if (BMM.Exists)
                {
                    BMM.CopyTo(String.Format("{0}{1}", this.ImplementationDirectory, BMM.Name));
                }
                Process ngd = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_NGDBUILD;
                psi.Arguments = NGDArgs;
                psi.WorkingDirectory = this.ImplementationDirectory;
                psi.UseShellExecute = false;
                ngd.StartInfo = psi;
                ngd.Start();

                // Wait for ngdbuild to terminate
                ngd.WaitForExit();
                if (ngd.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} NGDBuild complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} NGDBuild failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("ngdbuild{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.ImplementationDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_NGDBUILD, NGDArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_NGDBUILD, NGDArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    if ((BMM != null) && (PE_Count > 0))
                    {
                        BMM = new FileInfo(BMM.FullName);
                        if (BMM.Exists)
                        {
                            _SSHXFer.To(BMM.FullName, this.ImplementationDirectory + BMM.Name);
                        }
                    }
                    else
                    {
                        _SSHShell.WriteLine(String.Format("rm {0}/*.bmm", this.ImplementationDirectory));
                    }
                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.ImplementationDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "NGDBUILD");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.ImplementationDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} NGDBuild complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} NGDBuild failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("ngdbuild{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_NGDBUILD, NGDArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_NGDBUILD, NGDArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.ImplementationDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} NGDBuild complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} NGDBuild failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private bool XFlow_Map()
        {
            string MAPArgs = String.Format("-o system_map.ncd -w -pr b -ol high -timing -detail -xe n -mt 4 system.ngd system.pcf -intstyle xflow");
            RaiseMessageEvent("Starting MAP for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} MAP starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./implementation directory
                Directory.SetCurrentDirectory(this.ImplementationDirectory);
                Process map = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_MAP;
                psi.Arguments = MAPArgs;
                psi.WorkingDirectory = this.ImplementationDirectory;
                psi.UseShellExecute = false;
                map.StartInfo = psi;
                map.Start();

                // Wait for map to terminate
                map.WaitForExit();
                if (map.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} MAP complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} MAP failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("map{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.ImplementationDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_MAP, MAPArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_MAP, MAPArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.ImplementationDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "MAP");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.ImplementationDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} MAP complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} MAP failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("map{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_MAP, MAPArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_MAP, MAPArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.ImplementationDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} MAP complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} MAP failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private bool XFlow_PAR()
        {
            string PARArgs = String.Format("-w -ol high -xe n -mt 4 system_map.ncd system.ncd system.pcf -intstyle xflow");
            RaiseMessageEvent("Starting Place-And-Route for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Place-And-Route starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./implementation directory
                Directory.SetCurrentDirectory(this.ImplementationDirectory);
                Process par = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_PAR;
                psi.Arguments = PARArgs;
                psi.WorkingDirectory = this.ImplementationDirectory;
                psi.UseShellExecute = false;
                par.StartInfo = psi;
                par.Start();

                // Wait for map to terminate
                par.WaitForExit();
                if (par.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Place-And-Route complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} Place-And-Route failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("par{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.ImplementationDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_PAR, PARArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_PAR, PARArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.ImplementationDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "PAR");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.ImplementationDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Place-And-Route complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Place-And-Route failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("par{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_PAR, PARArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_PAR, PARArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.ImplementationDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));

                    XFlow_ValidatePAR();
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Place-And-Route complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Place-And-Route failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Performs simple parsing of PAR log file to determine whether timing and routing constraints were successfully met.
        /// </summary>
        private void XFlow_ValidatePAR()
        {
            string CONSTRAINTS_OK = "All constraints were met";
            string ROUTING_OK = "All signals are completely routed";
            string ROUTING_BAD = "signals are not completely routed";
            string RemotePARLog = String.Format("{0}/system.par", this.ImplementationDirectory);
            string LocalPARLog = String.Format("{0}\\{1}_{2}.par", _PathMan["ProjectTemp"], this.SystemPrefix, this.PlatformID);

            StreamReader reader = null;
            try
            {
                _SSHXFer.From(RemotePARLog, LocalPARLog);
                if (File.Exists(LocalPARLog))
                {
                    reader = new StreamReader(LocalPARLog);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.StartsWith(CONSTRAINTS_OK))
                        {
                            string Message = String.Format("\tINFO: Timing constraints passed OK.");
                            RaiseMessageEvent(Message);
                            LogMessage(String.Format("Platform {0} {1}", this.PlatformID, Message));
                        }
                        else if (line.EndsWith("not met."))
                        {
                            if ((line.Contains("constraint not met")) || (line.Contains("constraints not met")))
                            {
                                string Message = String.Format("\\tWARNING: Verify timing constraints -- {0}.", line);
                                RaiseMessageEvent(Message);
                                LogMessage(String.Format("Platform {0} {1}", this.PlatformID, Message));
                            }
                        }
                        else if (line.StartsWith(ROUTING_OK))
                        {
                            string Message = String.Format("\tINFO: Routing passed OK.");
                            RaiseMessageEvent(Message);
                            LogMessage(String.Format("Platform {0} {1}", this.PlatformID, Message));
                        }
                        else if (line.Contains(ROUTING_BAD))
                        {
                            string Message = String.Format("\tERROR: Routing FAILED -- Check the system.unroutes file for more information.");
                            RaiseMessageEvent(Message);
                            LogMessage(String.Format("Platform {0} {1}", this.PlatformID, Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }

            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (File.Exists(LocalPARLog))
                {
                    try 
                    {
                        File.Delete(LocalPARLog);
                    }
                    finally { }
                }
            }
        }
        #endregion

        #region Bitfile Generation (bitgen)
        private bool GenerateBITFile()
        {
            bool bGeneratedScript = false;
            string BitGenCmdFile = this.BitGenScriptPath;
            if ((BitGenCmdFile == null) || (BitGenCmdFile == string.Empty))
            {
                BitGenCmdFile = CreateBitGenCmdFile();
                bGeneratedScript = true;
            }
            else if (!File.Exists(BitGenCmdFile))
            {
                RaiseMessageEvent(String.Format("Specified bitfile generation script was not found locally: {0}.  Synthesis aborted.", this.BitGenScriptPath));
                return false;
            }
            FileInfo FIBGScript = new FileInfo(BitGenCmdFile);
            string BITArgs = String.Format("-w -f {0} system.ncd {1} -intstyle xflow", FIBGScript.Name, this.OutputBIT);

            
            RaiseMessageEvent("Starting Bitfile-Generation for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Bitfile-Generation starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./implementation directory
                Directory.SetCurrentDirectory(this.ImplementationDirectory);
                Process bit = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_BITGEN;
                psi.Arguments = BITArgs;
                psi.WorkingDirectory = this.ImplementationDirectory;
                psi.UseShellExecute = false;
                bit.StartInfo = psi;
                bit.Start();

                // Wait for bitgen to terminate
                bit.WaitForExit();
                if (bit.ExitCode == 0)
                {
                    MoveBitFileLocal();
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Bitfile-Generation complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} Bitfile-Generation failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("bitgen{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.ImplementationDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_BITGEN, BITArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_BITGEN, BITArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo cp {0}{1} {2}{3}\n", this.ImplementationDirectory, this.OutputBIT, this.OutputDirectory, this.OutputBIT));
                    writer.Write(String.Format("cp {0}{1} {2}{3}\n", this.ImplementationDirectory, this.OutputBIT, this.OutputDirectory, this.OutputBIT));
                    writer.Write(String.Format("echo cp {0}{1} {2}\n", this.ImplementationDirectory, "*.bmm", this.OutputDirectory));
                    writer.Write(String.Format("cp {0}{1} {2}\n", this.ImplementationDirectory, "*.bmm", this.OutputDirectory));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIBGScript.FullName, this.ImplementationDirectory + FIBGScript.Name);
                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    if (bGeneratedScript)
                    {
                        FIBGScript.Delete();
                    }
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.ImplementationDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "BITGEN");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.ImplementationDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Bitfile-Generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Bitfile-Generation failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("bitgen{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_BITGEN, BITArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_BITGEN, BITArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIBGScript.FullName, this.ImplementationDirectory + BitGenCmdFile);
                    _SSHXFer.To(FIScript.FullName, this.ImplementationDirectory + ScriptFile);
                    FIBGScript.Delete();
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.ImplementationDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.ImplementationDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Bitfile-Generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Bitfile-Generation failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private string CreateBitGenCmdFile()
        {
            string BitGenCmdFile = "bitgen.ut";
            string BitGenCmdPath = BitGenCmdFile;
            if (File.Exists(BitGenCmdPath))
                File.Delete(BitGenCmdPath);
            StreamWriter writer = new StreamWriter(BitGenCmdPath, false);
            writer.WriteLine("-g TdoPin:PULLNONE");
            writer.WriteLine("-g DriveDone:No");
            writer.WriteLine("-g StartUpClk:JTAGCLK");
            writer.WriteLine("-g DONE_cycle:4");
            writer.WriteLine("-g GTS_cycle:5");
            writer.WriteLine("-g TckPin:PULLUP");
            writer.WriteLine("-g TdiPin:PULLUP");
            writer.WriteLine("-g TmsPin:PULLUP");
            writer.WriteLine("-g DonePipe:No");
            writer.WriteLine("-g GWE_cycle:6");
            writer.WriteLine("-g LCK_cycle:NoWait");
            writer.WriteLine("-g Security:NONE");
            writer.WriteLine("-g Persist:No");

            writer.Close();
            return BitGenCmdFile;
        }

        private bool MoveBitFileLocal()
        {
            try
            {
                if (File.Exists(this.ImplementationDirectory + this.PlatformID + "_" + this.OutputBIT))
                {
                    File.Move(this.ImplementationDirectory + this.PlatformID + "_" + this.OutputBIT, this.OutputDirectory + this.PlatformID + "_" + this.OutputBIT);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return false;
        }
        #endregion

        #endregion /********************* Hardware Synthesis End **********************************/

        #region /********************* SAP PE Codelet Start ************************************/
        /// <summary>
        /// Get or set the list of CerebrumCores assigned to be synthesized on this FPGA/Platform.
        /// </summary>
        public List<CerebrumCore> AssignedCCores
        {
            get
            {
                if (_AssignedCCores == null)
                    _AssignedCCores = new List<CerebrumCore>();
                return _AssignedCCores;
            }
            set
            {
                _AssignedCCores = value;
            }
        }
        private List<CerebrumCore> _AssignedCCores;

        /// <summary>
        /// Get or set the list of ComponentCores assigned to be synthesized on this FPGA/Platform.
        /// </summary>
        public List<ComponentCore> AssignedPCores
        {
            get
            {
                if (_AssignedPCores == null)
                    _AssignedPCores = new List<ComponentCore>();
                return _AssignedPCores;
            }
            set
            {
                _AssignedPCores = value;
            }
        }
        private List<ComponentCore> _AssignedPCores;

        /// <summary>
        /// Creates a global Block-Memory-Map file to be used as input to NGDBuild during full synthesis.
        /// </summary>
        /// <param name="PE_Count">(ref) Returns the number of PE Components assigned in this Platform.</param>
        /// <returns>A FileInfo object pointing to the BMM file created locally.</returns>
        private FileInfo AssembleGlobalBMM(ref int PE_Count)
        {
            FileInfo BMMInfo = new FileInfo(String.Format("{0}\\{1}.bmm", ProjectPathManager["ProjectTemp"], this.PlatformID));
            if (BMMInfo.Exists)
                BMMInfo.Delete();
            StreamWriter writer = new StreamWriter(BMMInfo.FullName);
            PE_Count = 0;

            foreach (ComponentCore CompCore in this.AssignedPCores)
            {
                if (CompCore.IsPE)
                {
                    writer.Write(String.Format("ADDRESS_MAP {0} MICROBLAZE {1}\n", CompCore.CoreInstance, (PE_Count + 100).ToString()));
                    writer.Write(String.Format("\tADDRESS_SPACE {0} COMBINED [{1}:{2}]\n", "ram_combined", "0x00000000", "0x00000fff"));
                    writer.Write(String.Format("\t\tADDRESS_RANGE RAMB32\n"));
                    writer.Write(String.Format("\t\t\tBUS_BLOCK\n"));
                    writer.Write(String.Format("\t\t\t\t{0}/{0}/control_path_i/fdb_i/cntrl_proc/ram/ram/ramb36_0 [{1}:{2}];\n", CompCore.CoreInstance, 31, 0));
                    writer.Write(String.Format("\t\t\tEND_BUS_BLOCK;\n"));
                    writer.Write(String.Format("\t\tEND_ADDRESS_RANGE;\n"));
                    writer.Write(String.Format("\tEND_ADDRESS_SPACE;\n"));
                    writer.Write(String.Format("END_ADDRESS_MAP;\n"));
                    writer.Write(String.Format("\t\n"));
                    
                    PE_Count++;
                }
            }
            writer.Close();
            if (PE_Count > 0)
            {
                // Send the BMM to the appropriate place
                this.SystemBMM = BMMInfo.Name;
            }
            else
            {
                BMMInfo.Delete();
                this.SystemBMM = string.Empty;
                BMMInfo = null;
            }
            return BMMInfo;
        }

        /// <summary>
        /// Compiles the codelet sources for each processing element core in the platform.
        /// </summary>
        /// <returns>True if ALL compilations were successful.  False if any fail or an error occurs.</returns>
        private bool CompileCodeletELFs()
        {
            bool bSuccess = true;
            try
            {
                foreach (ComponentCore CompCore in this.AssignedPCores)
                {
                    if (CompCore.IsPE)
                    {
                        if (CompCore.IsAssignedCode)
                        {
                            if (File.Exists(_PathMan.DecodePath(CompCore.LocalCodeSource)))
                            {
                                if (!CompileCodelet(CompCore))
                                {
                                    RaiseMessageEvent("ERROR: Code compilation failed for {0}.  Local code file: {1}\n", CompCore.CoreInstance);
                                    RaiseMessageEvent("INFO: Local source code file: {0}\n", _PathMan.DecodePath(CompCore.LocalCodeSource));
                                    RaiseMessageEvent("INFO: See {0} for more details\n",
                                        String.Format("{0}_{1}.log", "codelet", CompCore.CoreInstance));
                                    bSuccess = false;
                                }
                            }
                            else
                            {
                                RaiseMessageEvent("WARNING: Code source for {0} not found locally.  Local code file: {1}\n", CompCore.CoreInstance, _PathMan.DecodePath(CompCore.LocalCodeSource));
                                bSuccess = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                bSuccess = false;
            }
            return (bSuccess);
        }
        /// <summary>
        /// Compiles a SAP PE Codelet into an ELF using scripts provide as part of the SAP PE SDK.
        /// </summary>
        /// <param name="ProcElement">The ComponentCore object representing a PE Core to be compiled.</param>
        /// <returns>True if compilation was successful and updates the ProcElement object to reflect the location of the remote compiled ELF file.  False otherwise.</returns>
        private bool CompileCodelet(ComponentCore ProcElement)
        {
            try
            {
                RaiseMessageEvent("Starting Codelet compilation for core {0}...", ProcElement.CoreInstance);
                LogMessage(String.Format("Platform {0} Codelet compilation {1} starting.", this.PlatformID, ProcElement.CoreInstance));

                string RemoteSDKDir = _PathMan["PECompileDir"];
                string RemoteCompileDir = String.Format("{0}/sdk_ws/pe_instructions", RemoteSDKDir);
                string CompileArgs = string.Empty;

                FileInfo LocalSource = new FileInfo(_PathMan.DecodePath(ProcElement.LocalCodeSource));
                string RemoteSource = String.Format("{0}/main.c", RemoteCompileDir);
                string CompiledELF = String.Format("{0}/Release/pe_instructions.elf", RemoteCompileDir);
                string TargetELF = String.Format("{0}{1}", this.OutputDirectory, ProcElement.ELFName);
                string RemoteCompileLog = String.Format("{0}{1}", this.OutputDirectory, "compile_script.log");


                string ScriptFile = String.Format("compile_{1}.sh", this.PlatformID, ProcElement.CoreInstance);
                FileInfo FIScript = new FileInfo(ScriptFile);
                StreamWriter writer = new StreamWriter(ScriptFile, false);
                FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                writer.Write(String.Format("cd {0}\n", RemoteCompileDir));
                writer.Write(String.Format("rm {0}\n", CompiledELF));
                writer.Write(String.Format("rm {0}\n", TargetELF));
                writer.Write(String.Format("echo {0} {1} > {2}\n", "compile.sh", CompileArgs, RemoteCompileLog));
                writer.Write(String.Format("{0} {1} > {2}\n", "compile.sh", CompileArgs, RemoteCompileLog));
                writer.Write(String.Format("mv {0} {1}\n", CompiledELF, TargetELF));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                writer.Close();

                _SSHExec.RunCommand(String.Format("rm {0}", TargetELF));
                _SSHExec.RunCommand(String.Format("rm {0}", RemoteSource));
                _SSHXFer.To(LocalSource.FullName, RemoteSource);
                _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                FIScript.Delete();

                string CommandResponse;
                _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, ScriptFile));
                _SSHShell.Expect();
                string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, "codelet", ProcElement.CoreInstance);
                _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                    this.PlatformDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Codelet compilation {1} complete.", this.PlatformID, ProcElement.CoreInstance));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");

                    //FileInfo FICompileLog = new FileInfo("local_compile_log.log");
                    //try { if (FICompileLog.Exists) FICompileLog.Delete(); }
                    //catch (Exception ex) { ErrorReporting.DebugException(ex); }
                    //_SSHXFer.From(RemoteCompileLog, FICompileLog.FullName);
                    //{
                    //    StreamReader reader = new StreamReader(FICompileLog.FullName);
                    //    while (!reader.EndOfStream)
                    //    {
                    //        string line = reader.ReadLine();
                    //        RaiseMessageEvent(line);
                    //    }
                    //    reader.Close();
                    //}
                    //try { if (FICompileLog.Exists) FICompileLog.Delete(); }
                    //catch (Exception ex) { ErrorReporting.DebugException(ex); }
                    LogMessage(String.Format("Platform {0} Codelet compilation {1} failed.", this.PlatformID, ProcElement.CoreInstance));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                RaiseMessageEvent(String.Format("ERROR: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Merges the compiled ELF files into the bitstream.
        /// </summary>
        /// <returns>True if ALL merges were successful.  False if any fail or an error occurs.</returns>
        private bool MergeCodeletELFs()
        {
            bool bSuccess = true;
            try
            {
                string OriginalBIT = String.Format("{0}{1}_{2}.bit", this.OutputDirectory, this.SystemPrefix, this.PlatformID);
                string RemoteBIT = OriginalBIT;
                string RemoteBMM = String.Format("{0}{1}_bd.bmm", this.OutputDirectory, this.PlatformID);

                foreach (ComponentCore CompCore in this.AssignedPCores)
                {
                    if (CompCore.IsPE)
                    {
                        if (CompCore.IsAssignedCode)
                        {
                            string RemoteELF = String.Format("{0}{1}.elf", this.OutputDirectory, CompCore.CoreInstance);
                            string TargetBIT = String.Format("{0}{1}_{2}.bit", this.OutputDirectory, this.PlatformID, CompCore.CoreInstance);
                            if (MergeELFIntoBitstream(CompCore.CoreInstance, RemoteBMM, RemoteELF, RemoteBIT, TargetBIT))
                            {
                                RaiseMessageEvent(" Merged {0} ELF into bitstream.\n", CompCore.CoreInstance);
                                _SSHExec.RunCommand(String.Format("mv {0} {1}", TargetBIT, RemoteBIT));
                            }
                            else
                            {
                                RaiseMessageEvent("ERROR: Merge failed for {0} ELF.\n", CompCore.CoreInstance);
                                bSuccess = false;
                            }
                        }
                    }
                }
                if (bSuccess)
                {
                    if (RemoteBIT != OriginalBIT)
                    {
                        _SSHExec.RunCommand(String.Format("mv {0} {1}", RemoteBIT, OriginalBIT));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                bSuccess = false;
            }
            return (bSuccess);
        }
        /// <summary>
        /// Merges the specified RemoteELF file, using the RemoteBMM file as a map, and PEInstance as a tag/guide, into SourceBIT, producing TargetBIT.  
        /// All files are assumed to exist on the remote system.
        /// </summary>
        /// <param name="PEInstance">The PE Core instance used as a tag to locate the BRAM in the bitstream.</param>
        /// <param name="RemoteBMM">The Block Memory Map generated from the previous synthesis.</param>
        /// <param name="RemoteELF">The ELF file associated with the PE Core to be integrated into the bitstream.</param>
        /// <param name="SourceBIT">The original bitstream into which the ELF is to be integrated.</param>
        /// <param name="TargetBIT">The output bitstream, containing the integrated ELF file.</param>
        /// <returns></returns>
        private bool MergeELFIntoBitstream(string PEInstance, string RemoteBMM, string RemoteELF, string SourceBIT, string TargetBIT)
        {
            string D2MArgs = String.Format("-bm {0} -bd {1} tag {2} -bt {3} -o b {4}",
                        RemoteBMM, RemoteELF, PEInstance, SourceBIT, TargetBIT);

            // Create script, send it, and execute it
            string ScriptFile = String.Format("merge_{0}_{1}.sh", this.PlatformID, PEInstance);
            FileInfo FIScript = new FileInfo(ScriptFile);
            StreamWriter writer = new StreamWriter(ScriptFile, false);
            FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
            writer.Write(String.Format("cd {0}\n", this.OutputDirectory));
            writer.Write(String.Format("echo {0} {1}\n", XTOOL_DATA2MEM, D2MArgs));
            writer.Write(String.Format("{0} {1}\n", XTOOL_DATA2MEM, D2MArgs));
            writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
            writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
            writer.Close();

            _SSHXFer.To(FIScript.FullName, this.OutputDirectory + ScriptFile);
            FIScript.Delete();

            string CommandResponse;
            _SSHShell.WriteLine(String.Format("cd {0}", this.OutputDirectory));
            _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.OutputDirectory, ScriptFile));
            _SSHShell.Expect();
            string LogFile = String.Format("{0}{1}_{2}_{3}.log", this.OutputDirectory, this.PlatformID, "DATA2MEM", PEInstance);
            _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                this.OutputDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
            CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
            if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion /********************* SAP PE Codelet End **************************************/

        #region /********************* Software Compilation Start ******************************/

        #region Software Processor Operating Systems
        /// <summary>
        /// Assigns a processor instance to this platform.
        /// </summary>
        /// <param name="ProcOS">The processor object to be compiled for this platform.</param>
        public void AddProcessorOS(FalconProcessorOS ProcOS)
        {
            _Processors.Add(ProcOS);
        }
        /// <summary>
        /// Removes a processor instance from this platform.
        /// </summary>
        /// <param name="Instance">The instance name of the processor to be removed.</param>
        public void RemoveProcessorOS(string Instance)
        {
            foreach (FalconProcessorOS fpOS in _Processors)
            {
                if (fpOS.Instance == Instance)
                {
                    _Processors.Remove(fpOS);
                    return;
                }
            }
        }
        /// <summary>
        /// Removes a processor instance from this platform.
        /// </summary>
        /// <param name="ProcOS">The processor object to be removed.</param>
        public void RemoveProcessorOS(FalconProcessorOS ProcOS)
        {
            this.RemoveProcessorOS(ProcOS.Instance);
        }
        /// <summary>
        /// Gets the type of the processor of the specified instance.
        /// </summary>
        /// <param name="Instance">The instance name of the processor to be located.</param>
        public FalconProcessorOS GetProcessorOS(string Instance)
        {
            foreach (FalconProcessorOS fpOS in _Processors)
            {
                if (fpOS.Instance == Instance)
                {
                    return fpOS;
                }
            }
            return null;
        }
        #endregion

        #region Software Preparation (MSS, Bootargs, Console Device, OS)
        private bool PrepareSoftware()
        {
            bool bSuccess = true;
            RaiseMessageEvent("Preparing software-specific files for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Software-specific preparation starting.", this.PlatformID));
            string MHSPath = string.Empty;
            string MSSPath = string.Empty;
            if (FetchMHS_MSS(out MHSPath, out MSSPath))
            {
                // Wipe out the contents of the MSS file, to ensure that no previously defined processors are copied
                // ALL OS Blocks are re-created from the design files and (re-)written by the synthesis tool.
                try { if (File.Exists(MSSPath)) File.Delete(MSSPath); }
                finally { }
                StreamWriter eraser = new StreamWriter(MSSPath);
                eraser.Close();
                foreach (FalconProcessorOS fpOS in _Processors)
                {
                    bSuccess = (bSuccess) && (UpdateProcessor(fpOS, MHSPath, MSSPath));
                    if (!bSuccess)
                    {
                        break;
                    }
                }
                if (bSuccess)
                {
                    bSuccess = ReplaceMHS_MSS(MHSPath, MSSPath);
                }
            }
            else
            {
                bSuccess = false;
            }
            if (bSuccess)
            {
                RaiseMessageEvent("Complete!");
                LogMessage(String.Format("Platform {0} Software-specific File preparation complete.", this.PlatformID));
            }
            else
            {
                RaiseMessageEvent("ERROR!");
                LogMessage(String.Format("Platform {0} Software-specific File preparation failed.", this.PlatformID));
            }
            return bSuccess;
        }

        private bool FetchMHS_MSS(out string MHSFile, out string MSSFile)
        {
            MHSFile = string.Empty;
            MSSFile = string.Empty;
            LogMessage(String.Format("Platform {0} Retrieving MHS and MSS Files.", this.PlatformID));
            if (this.LocalSynthesis)
            {
                MHSFile = this.PlatformDirectory + this.SystemMHS;
                MSSFile = this.PlatformDirectory + this.SystemMSS;
                return true;
            }
            else 
            {
                // Same for windows and linux remote machines
                try
                {
                    MHSFile = Directory.GetCurrentDirectory() + "\\" + this.SystemMHS;
                    MSSFile = Directory.GetCurrentDirectory() + "\\" + this.SystemMSS;
                    if (File.Exists(MHSFile))
                        File.Delete(MHSFile);
                    if (File.Exists(MSSFile))
                        File.Delete(MSSFile);
                    try
                    {
                        _SSHXFer.From(this.PlatformDirectory + this.SystemMHS, MHSFile);
                    }
                    catch { }
                    try
                    {
                        _SSHXFer.From(this.PlatformDirectory + this.SystemMSS, MSSFile);
                    }
                    catch 
                    {
                        StreamWriter writer = new StreamWriter(MSSFile);
                        writer.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    return false;
                }
            }
        }
        private bool ReplaceMHS_MSS(string MHSFile, string MSSFile)
        {
            LogMessage(String.Format("Platform {0} Replacing MHS and MSS Files.", this.PlatformID));
            string currentMHSFile = this.PlatformDirectory + this.SystemMHS;
            string currentMSSFile = this.PlatformDirectory + this.SystemMSS;
            if (this.LocalSynthesis)
            {
                try
                {
                    if (currentMHSFile != MHSFile)
                    {
                        File.Copy(MHSFile, currentMHSFile);
                        File.Delete(MHSFile);
                    }
                    if (currentMSSFile != MSSFile)
                    {
                        File.Copy(MSSFile, currentMSSFile);
                        File.Delete(MSSFile);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    try
                    {
                        LogMessage(String.Format("Platform {0} Renaming MHS and MSS Files on Remote System as *.old.", this.PlatformID));
                        _SSHShell.WriteLine(String.Format("cp {0} {0}.old", currentMHSFile));
                        _SSHShell.WriteLine(String.Format("cp {0} {0}.old", currentMSSFile));
                        _SSHShell.Expect();
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        LogMessage(String.Format("Platform {0} Renaming MHS and MSS Files on Remote System as *.old.", this.PlatformID));
                        _SSHShell.WriteLine(String.Format("copy /Y {0} {0}.old", currentMHSFile));
                        _SSHShell.WriteLine(String.Format("copy /Y {0} {0}.old", currentMSSFile));
                        _SSHShell.Expect();
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        return false;
                    }
                }

                try
                {
                    LogMessage(String.Format("Platform {0} Copying MHS File to Remote System as {1}.", this.PlatformID, currentMHSFile));
                    _SSHXFer.To(MHSFile, currentMHSFile);
                    System.Threading.Thread.Sleep(1000);
                    LogMessage(String.Format("Platform {0} Copying MSS File to Remote System as {1}.", this.PlatformID, currentMSSFile));
                    _SSHXFer.To(MSSFile, currentMSSFile);
                    System.Threading.Thread.Sleep(1000);
                    File.Delete(MHSFile);
                    File.Delete(MSSFile);
                    return true;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    return false;
                }
            }
        }
        private bool UpdateProcessor(FalconProcessorOS fpOS, string MHSFile, string InMSSFile)
        {
            string OutMSSFile = InMSSFile + ".new";
            StreamReader reader = new StreamReader(InMSSFile);
            StreamWriter writer = new StreamWriter(OutMSSFile);

            string inLine = string.Empty;
            string outLine = string.Empty;
            bool bInOSBlock = false;
            string OSBlockProcInstance = string.Empty;
            StringBuilder LinuxOSBlock = new StringBuilder();
            StringBuilder StandAloneOSBlock = new StringBuilder();
            StringBuilder BufferedOSBlock = new StringBuilder();
            bool bSuccess = true;
            bool bDone = false;

            LogMessage(String.Format("Platform {0}, Processor '{1}' Updating STDIO/Console", this.PlatformID, fpOS.Instance));
            if (fpOS.OS == SystemProcessorOS.Linux)
            {
                // Build Linux OS Block

                string ipArg = string.Empty;
                if ((this.IPAddress == string.Empty) || (this.IPAddress == null) || (this.IPFromDHCP))
                    ipArg = "on";
                else
                    ipArg = this.IPAddress;
                string bootargs = String.Format("console=ttyS0 root=/dev/ram rw ip={0}", ipArg);
                string os_ver;
                string os_name;
                string osPath;

                CopyDeviceTree(this.DeviceTree);
                osPath = GetPathShortName(this.DeviceTree);
                if (osPath.Contains("_v"))
                {
                    os_name = osPath.Substring(0, osPath.IndexOf("_v"));
                    os_ver = osPath.Substring(osPath.IndexOf("_v") + 2).Replace("_", ".");
                    LinuxOSBlock.AppendLine(String.Format("BEGIN OS"));
                    LinuxOSBlock.AppendLine(String.Format(" PARAMETER OS_NAME = {0}", os_name));
                    LinuxOSBlock.AppendLine(String.Format(" PARAMETER OS_VER = {0}", os_ver));
                    LinuxOSBlock.AppendLine(String.Format(" PARAMETER PROC_INSTANCE = {0}", fpOS.Instance));
                    LinuxOSBlock.AppendLine(String.Format(" PARAMETER bootargs = {0}", bootargs));
                    if (fpOS.ConsoleDevice.Length > 0) LinuxOSBlock.AppendLine(String.Format(" PARAMETER console device = {0}", fpOS.ConsoleDevice));
                    LinuxOSBlock.Append("END");
                }
            }
            else
            {
                if (fpOS.OS == SystemProcessorOS.Standalone)
                {
                    StandAloneOSBlock.AppendLine(String.Format("BEGIN OS"));
                    StandAloneOSBlock.AppendLine(String.Format(" PARAMETER OS_NAME = {0}", "Standalone"));
                    StandAloneOSBlock.AppendLine(String.Format(" PARAMETER OS_VER = {0}", "2.00.a"));
                    StandAloneOSBlock.AppendLine(String.Format(" PARAMETER PROC_INSTANCE = {0}", fpOS.Instance));
                    if (fpOS.ConsoleDevice.Length > 0) StandAloneOSBlock.AppendLine(String.Format(" PARAMETER STDIN = {0}", fpOS.ConsoleDevice));
                    if (fpOS.ConsoleDevice.Length > 0) StandAloneOSBlock.AppendLine(String.Format(" PARAMETER STDOUT = {0}", fpOS.ConsoleDevice));
                    StandAloneOSBlock.Append("END");
                }
            }

            int i = 0;

            try
            {
                while (!reader.EndOfStream)
                {
                    i++;
                    inLine = reader.ReadLine();
                    if (!bInOSBlock)
                    {
                        if (inLine.StartsWith("BEGIN OS"))
                        {
                            bInOSBlock = true;
                            OSBlockProcInstance = string.Empty;
                            BufferedOSBlock = new StringBuilder();
                            BufferedOSBlock.AppendLine(inLine);
                        }
                        else 
                        {
                            writer.WriteLine(inLine);
                        }
                    }
                    else
                    {
                        BufferedOSBlock.AppendLine(inLine);
                        if (inLine.Contains(" PARAMETER PROC_INSTANCE ="))
                        {
                            int idx = inLine.IndexOf(" PROC_INSTANCE = ");
                            OSBlockProcInstance = inLine.Substring(idx + (" PROC_INSTANCE = ").Length).Trim();
                        }
                        else if (inLine.StartsWith("END"))
                        {
                            if (OSBlockProcInstance == fpOS.Instance)
                            {
                                if (fpOS.OS == SystemProcessorOS.Linux)
                                {
                                    writer.WriteLine(LinuxOSBlock.ToString());
                                    bDone = true;
                                }
                                else
                                {
                                    writer.WriteLine(StandAloneOSBlock.ToString());
                                    bDone = true;
                                }
                            }
                            else
                            {
                                // Do not copy any buffered OS Blocks from a previous MSS File.
                                // ALL OS Blocks are re-created from the design files and (re-)written by the synthesis tool.
                                writer.WriteLine(BufferedOSBlock.ToString());
                            }
                            BufferedOSBlock = new StringBuilder();
                            OSBlockProcInstance = string.Empty;
                            bInOSBlock = false;
                        }
                    }
                }
                if (!bDone)
                {
                    if (fpOS.OS == SystemProcessorOS.Linux)
                    {
                        writer.WriteLine(LinuxOSBlock.ToString());
                        bDone = true;
                    }
                    else
                    {
                        writer.WriteLine(StandAloneOSBlock.ToString());
                        bDone = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                bSuccess = false;
            }
            reader.Close();
            writer.Close();
            reader.Dispose();
            writer.Dispose();
            reader = null;
            writer = null;
            try
            {
                if (File.Exists(InMSSFile))
                    File.Delete(InMSSFile);
                File.Move(OutMSSFile, InMSSFile);
            }
            catch (Exception ex)
            {
                LogException(ex);
                bSuccess = false;
            }
            return bSuccess;
        }
        #endregion

        #region Library Generation (Libgen)
        private bool GenerateLibraries()
        {
            string SearchPathIncludes = string.Empty;
            string[] SynthPCoreRepos = _PathMan["GlobalSynthPCores"].Split(';');
            int includeCount = 0;
            foreach (string Repo in SynthPCoreRepos)
            {
                string SynthFolder = Repo.Trim();
                if (SynthFolder != string.Empty)
                {
                    SearchPathIncludes = String.Format("{0} -lp {1}", SearchPathIncludes, SynthFolder).Trim();
                    includeCount++;
                }
            }
            if (includeCount == 0)
                SearchPathIncludes = string.Empty;

            string LIBArgs = String.Format("-mhs {0} -p {1} {2} {3}", 
                this.SystemMHS, 
                this.PlatGenXPartName,
                this.SystemMSS,
                SearchPathIncludes);
            RaiseMessageEvent("Starting Library Generation for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Library generation starting.", this.PlatformID));

            if (this.LocalSynthesis)
            {
                // This part is run from the ./ directory
                Directory.SetCurrentDirectory(this.PlatformDirectory);
                Process libgen = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = XTOOL_LIBGEN;
                psi.Arguments = LIBArgs;
                psi.WorkingDirectory = this.PlatformDirectory;
                psi.UseShellExecute = false;
                libgen.StartInfo = psi;
                libgen.Start();

                // Wait for libgen to terminate
                libgen.WaitForExit();
                MoveFileToOutput(this.PlatformDirectory + "libgen.log", this.PlatformID);
                if (libgen.ExitCode == 0)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Library generation complete.", this.PlatformID));
                    return true;
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} Library generation failed.", this.PlatformID));
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("libgen{0}.sh", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                    writer.Write(String.Format("cd {0}\n", this.PlatformDirectory));
                    writer.Write(String.Format("echo {0} {1}\n", XTOOL_LIBGEN, LIBArgs));
                    writer.Write(String.Format("{0} {1}\n", XTOOL_LIBGEN, LIBArgs));
                    writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                    writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, ScriptFile));
                    _SSHShell.Expect();
                    string LogFile = String.Format("{0}{1}_{2}.log", this.OutputDirectory, this.PlatformID, "LIBGEN");
                    _SSHShell.WriteLine(String.Format("{3} {0}{1} >& {2}; tail {2}",
                        this.PlatformDirectory, ScriptFile, LogFile, LINUX_COMMAND_SHELL));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    MoveFileToOutput(this.PlatformDirectory + "libgen.log", this.PlatformID);
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Library generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Library generation failed.", this.PlatformID));
                        return false;
                    }
                }
                else
                {
                    // Create script, send it, and execute it
                    string ScriptFile = String.Format("libgen{0}.bat", this.PlatformID);
                    FileInfo FIScript = new FileInfo(ScriptFile);
                    StreamWriter writer = new StreamWriter(ScriptFile, false);
                    writer.WriteLine(String.Format("@ECHO OFF"));
                    writer.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    writer.WriteLine(String.Format("echo {0} {1}", XTOOL_LIBGEN, LIBArgs));
                    writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_LIBGEN, LIBArgs));
                    writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                    writer.WriteLine(String.Format(""));
                    writer.WriteLine(String.Format(":exit_success"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                    writer.WriteLine(String.Format("exit /b 0"));
                    writer.WriteLine(String.Format(":exit_fail"));
                    writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                    writer.WriteLine(String.Format("exit /b 1"));
                    writer.Close();

                    _SSHXFer.To(FIScript.FullName, this.PlatformDirectory + ScriptFile);
                    FIScript.Delete();

                    string CommandResponse;
                    _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                    _SSHShell.Expect();
                    _SSHShell.WriteLine(String.Format("{0}{1}", this.PlatformDirectory, ScriptFile));
                    CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                    MoveFileToOutput(this.PlatformDirectory + "libgen.log", this.PlatformID);
                    if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                    {
                        RaiseMessageEvent("Complete!");
                        LogMessage(String.Format("Platform {0} Library generation complete.", this.PlatformID));
                        return true;
                    }
                    else
                    {
                        RaiseMessageEvent("ERROR!");
                        LogMessage(String.Format("Platform {0} Library generation failed.", this.PlatformID));
                        return false;
                    }
                }
            }
        }
        private bool CopyDeviceTree(string DevTreePath)
        {
            LogMessage(String.Format("Copying device tree \"{0}\" for Platform '{1}' to \"{2}\"", DeviceTree, this.PlatformID, this.BSPDirectory));
            if (this.LocalSynthesis)
            {
                DirectoryInfo devDI = new DirectoryInfo(DevTreePath);
                Directory.CreateDirectory(this.BSPDirectory + devDI.Name);
                foreach (FileInfo fi in devDI.GetFiles())
                {
                    fi.CopyTo(this.BSPDirectory + devDI.Name + this.Separator + fi.Name);
                }
                return true;
            }
            else
            {
                if (this.LinuxHost)
                {
                    try
                    {
                        string CmdVerify = String.Format("ls {0}", DevTreePath);
                        string Resp = _SSHExec.RunCommand(CmdVerify);
                        if (Resp != string.Empty)
                        {
                            string Command = string.Format("cp -r {0} {1};echo {2}", DevTreePath, this.BSPDirectory, FINISH_SUCCESS);
                            _SSHExec.RunCommand(Command);
                            string sVerify = string.Empty;
                            string dtFolder = GetPathShortName(DevTreePath);
                            CmdVerify = String.Format("ls {0}", this.BSPDirectory);
                            sVerify = _SSHExec.RunCommand(CmdVerify);
                            while (!sVerify.Contains(dtFolder))
                            {
                                _SSHExec.RunCommand(Command);
                                sVerify = _SSHExec.RunCommand(CmdVerify);
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch(Exception ex)
                    {
                        LogException(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        string Command = string.Format("xcopy /E /Y /H /R {0} {1};echo {2}", DevTreePath, this.BSPDirectory, FINISH_SUCCESS);
                        _SSHExec.RunCommand(Command); 
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region DTS Maintenance (Ethernet MAC, IP, DHCP, Invalid Object Purge)
        private bool UpdateDTS()
        {
            bool bSuccess = false;
            RaiseMessageEvent("Updating DTS File for Platform '{0}'...", this.PlatformID);
            LogMessage(String.Format("Platform {0} Updating DTS File starting.", this.PlatformID));
            
            foreach (FalconProcessorOS fpOS in _Processors)
            {
                if (fpOS.OS != SystemProcessorOS.Linux)
                    continue;

                string DTSPath = string.Empty;

                // TODO: Double check this for Standalone compilation (Source Path may not be needed - double-up on (rename)DeviceTree?)
                string devTree = GetPathShortName(this.DeviceTree);
                if (FetchDTS(out DTSPath, fpOS.Instance, devTree))
                {
                    bSuccess = UpdateEthernetMACs(DTSPath, fpOS);
                    if (bSuccess)
                    {
                        bSuccess = PurgeDTS(DTSPath, fpOS, DTSPath);
                        if (bSuccess)
                        {
                            ReplaceDTS(DTSPath, fpOS.Instance, devTree);
                        }
                    }
                }
            }
            if (bSuccess)
            {
                RaiseMessageEvent("Complete!");
                LogMessage(String.Format("Platform {0} Updating DTS complete.", this.PlatformID));
            }
            else
            {
                RaiseMessageEvent("ERROR!");
                LogMessage(String.Format("Platform {0} Updating DTS failed.", this.PlatformID));
            }
            return bSuccess;
        }
        private bool UpdateEthernetMACs(string DTSPath, FalconProcessorOS fpOS)
        {
            RaiseMessageEvent("\tUpdating Ethernet MAC Addresses");
            LogMessage(String.Format("\tUpdating Ethernet MAC Addresses."));
            if ((this.EthernetDevice == string.Empty) || (this.EthernetMAC == string.Empty))
            {
                RaiseMessageEvent("\t\tUpdate Ethernet MAC failed: Please verify that Ethernet Device and MAC address are set for the platform.");
                return false;
            }
            bool bSuccess = true;
            bSuccess = UpdateEthernetMAC(DTSPath, this.EthernetDevice, this.EthernetMAC);
            if (bSuccess)
            {
                RaiseMessageEvent("\tComplete!");
                LogMessage(String.Format("Platform {0}, Processor {1} Updating Ethernet MAC Addresses complete.", this.PlatformID, fpOS.Instance));
            }
            else
            {
                RaiseMessageEvent("\tERROR!");
                LogMessage(String.Format("Platform {0}, Processor {1} Updating Ethernet MAC Addresses failed.", this.PlatformID, fpOS.Instance));
            }
            return bSuccess;
        }
        private bool UpdateEthernetMAC(string InDTSFile, string EthernetCoreInstance, string EthernetMACAddress)
        {
            LogMessage(String.Format("Platform {0} Updating Ethernet Core '{1}' MAC Address [{2}]", this.PlatformID, EthernetCoreInstance, EthernetMACAddress));
            bool bSuccess = true;
            string OutDTSFile = InDTSFile + ".new";
            if ((EthernetCoreInstance == string.Empty) || (EthernetMACAddress == string.Empty))
            {
                RaiseMessageEvent("\tUpdate Ethernet MAC failed: Please verify that Ethernet Device and MAC address are set for the platform.");
                return false;
            }
            EthernetMACAddress = EthernetMACAddress.Replace(":", " ");
            EthernetMACAddress = EthernetMACAddress.Replace("-", " ");
            EthernetMACAddress = EthernetMACAddress.ToLower();

            //if (File.Exists(InDTSFile))
            //    File.Delete(InDTSFile);
            if (File.Exists(OutDTSFile))
                File.Delete(OutDTSFile);

            StreamReader reader = new StreamReader(InDTSFile);
            StreamWriter writer = new StreamWriter(OutDTSFile);

            string inLine = string.Empty;
            string outLine = string.Empty;
            bool bInBlock = false;
            bool bDone = false;
            int i = 0;

            string blockStart = String.Format("{0}: ", EthernetCoreInstance);
            string macLine = String.Format("{0} = ", "local-mac-address");
            try
            {
                while (!reader.EndOfStream)
                {
                    i++;
                    inLine = reader.ReadLine();
                    string workLine = inLine.Trim();
                    int tabCount = inLine.Length - workLine.Length;
                    if (!bInBlock)
                    {
                        if (workLine.StartsWith(blockStart))
                        {
                            bInBlock = true;
                        }
                    }
                    else
                    {
                        if (workLine.StartsWith(macLine) && (!bDone))
                        {
                            string newLine;
                            newLine = new String('\t', tabCount);
                            newLine += macLine;
                            newLine += String.Format("[ {0} ];", EthernetMACAddress);
                            inLine = newLine;
                            bDone = true;
                        }
                    }
                    writer.WriteLine(inLine);
                }
            }
            catch (Exception ex)
            {
                LogMessage(String.Format("Platform {0} Libgen-DTS File download failed.", this.PlatformID));
                LogException(ex);
                bSuccess = false;
            }
            reader.Close();
            writer.Close();
            reader.Dispose();
            writer.Dispose();
            reader = null;
            writer = null;
            try
            {
                if (File.Exists(InDTSFile))
                    File.Delete(InDTSFile);
                File.Move(OutDTSFile, InDTSFile);
            }
            catch (Exception ex)
            {
                LogMessage(String.Format("Platform {0} Libgen-DTS File download failed.", this.PlatformID));
                LogException(ex);
                bSuccess = false;
            }
            return bSuccess;
        }
        private bool FetchDTS(out string DTSFile, string ProcInst, string DevTree)
        {
            DTSFile = string.Empty;
            LogMessage(String.Format("Platform {0} Retrieving Libgen-DTS File.", this.PlatformID));
            if (this.LocalSynthesis)
            {
                DTSFile = this.PlatformDirectory + ProcInst + this.Separator + "libsrc" + this.Separator + DevTree + this.Separator + "xilinx.dts";
                return true;
            }
            else
            {
                try 
                {       
                    string remoteDTSFile = this.PlatformDirectory + ProcInst + this.Separator + "libsrc" + this.Separator + DevTree + this.Separator + "xilinx.dts";
                    DTSFile = String.Format("{0}\\xilinx.dts", _PathMan["ProjectTemp"]);

                    LogMessage(String.Format("Platform {0} Downloading Libgen-DTS File from '{1}' to '{2}'.", this.PlatformID, remoteDTSFile, DTSFile));
                    if (File.Exists(DTSFile))
                        File.Delete(DTSFile);
                    _SSHXFer.From(remoteDTSFile, DTSFile);
                    LogMessage(String.Format("Platform {0} Libgen-DTS File download successful.", this.PlatformID));
                    return true;
                }
                catch (Exception ex)
                {
                    LogMessage(String.Format("Platform {0} Libgen-DTS File download failed.", this.PlatformID));
                    LogException(ex);
                    return false;
                }
            }
        }
        private bool ReplaceDTS(string DTSFile, string ProcInst, string DevTree)
        {
            LogMessage(String.Format("Platform {0} Replacing Libgen-DTS File.", this.PlatformID));
            string currentDTSFile = this.PlatformDirectory + ProcInst + this.Separator + "libsrc" + this.Separator + DevTree + this.Separator + "xilinx.dts";
            if (this.LocalSynthesis)
            {
                try
                {
                    if (currentDTSFile != DTSFile)
                    {
                        File.Copy(DTSFile, currentDTSFile);
                        File.Delete(DTSFile);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogMessage(String.Format("Platform {0} Libgen-DTS File upload failed.", this.PlatformID));
                    LogException(ex);
                    return false;
                }
            }
            else
            {
                if (this.LinuxHost)
                {
                    try
                    {
                        _SSHShell.WriteLine(String.Format("mv {0} {0}.old", currentDTSFile));
                        _SSHShell.Expect();
                    }
                    catch (Exception ex)
                    {
                        LogMessage(String.Format("Platform {0} Libgen-DTS File upload failed.", this.PlatformID));
                        LogException(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        _SSHShell.WriteLine(String.Format("move /Y {0} {0}.old", currentDTSFile));
                        _SSHShell.Expect();
                    }
                    catch (Exception ex)
                    {
                        LogMessage(String.Format("Platform {0} Libgen-DTS File upload failed.", this.PlatformID));
                        LogException(ex);
                        return false;
                    }
                }

                try 
                {

                    LogMessage(String.Format("Platform {0} Uploading Libgen-DTS File from '{1}' to '{2}'.", this.PlatformID, DTSFile, currentDTSFile));
                    _SSHXFer.To(DTSFile, currentDTSFile);
                    File.Delete(DTSFile);
                    LogMessage(String.Format("Platform {0} Libgen-DTS File upload successful.", this.PlatformID));
                    return true;
                }
                catch (Exception ex)
                {
                    LogMessage(String.Format("Platform {0} Libgen-DTS File upload failed.", this.PlatformID));
                    LogException(ex);
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Purges top-level instances that do not match the specified Component Instance
        /// </summary>
        /// <param name="DTSPath">The path to the input DTS file.</param>
        /// <param name="fpOS">The FalconProcessorOS object that is being isolated in the DTS.</param>
        /// <param name="OutputPath">The path to the output DTS file. (May be the same as input)</param>
        private bool PurgeDTS(string DTSPath, FalconProcessorOS fpOS, string OutputPath)
        {
            try
            {
                FileInfo fi = new FileInfo(DTSPath);
                if (!fi.Exists)
                    return false;

                LogMessage(String.Format("Platform {0}, Processor {1} Purging DTS starting.", this.PlatformID, fpOS.Instance));
                string KeepInstance = fpOS.OwnerComponent;
                StreamReader reader = new StreamReader(fi.FullName);
                StringBuilder contents = new StringBuilder();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    int colonIdx = line.IndexOf(":");
                    int atIdx = line.IndexOf("@");
                    int lBraceIdx = line.IndexOf("{");
                    int rBraceIdx = line.IndexOf("}");

                    if (lBraceIdx > 0)
                    {
                        if ((colonIdx > 0) && (atIdx > 0) && (lBraceIdx > 0) &&
                            (colonIdx < atIdx) && (atIdx < lBraceIdx))
                        {
                            string instance = line.Substring(0, colonIdx).Trim();
                            if (instance.StartsWith(KeepInstance))
                            {
                                contents.AppendLine(line);
                                GoDeeper(reader, contents, false);
                            }
                            else
                            {
                                GoDeeper(reader, contents, true);
                            }
                        }
                        else
                        {
                            contents.AppendLine(line);
                        }
                    }
                    else
                    {
                        contents.AppendLine(line);
                    }
                }
                reader.Close();

                StreamWriter writer = new StreamWriter(OutputPath);
                writer.Write(contents.ToString());
                writer.Close();

                LogMessage(String.Format("Platform {0}, Processor {1} Purging DTS complete.", this.PlatformID, fpOS.Instance));
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                LogMessage(String.Format("Platform {0}, Processor {1} Purging DTS failed.", this.PlatformID, fpOS.Instance));
            }
            return false;
        }    
        private void GoDeeper(StreamReader reader, StringBuilder contents, bool Skip)
        {
            int nBraces = 1;
            while (nBraces > 0)
            {
                string line = reader.ReadLine();
                if (!Skip) contents.AppendLine(line);
                int lBraceIdx = line.IndexOf("{");
                int rBraceIdx = line.IndexOf("}");
                if (lBraceIdx > 0) nBraces++;
                if (rBraceIdx > 0) nBraces--;
            }
        }     
        #endregion

        #region Software Compilation
        private bool CompileSoftware()
        {
            bool bSuccess = true;
            try
            {
                RaiseMessageEvent("Starting Software Compilation for Platform '{0}'...", this.PlatformID);
                LogMessage(String.Format("Platform {0} Software Compilation starting.", this.PlatformID));

                if (this.LocalSynthesis)
                {
                    foreach (FalconProcessorOS fpOS in _Processors)
                    {
                        if (fpOS.OS == SystemProcessorOS.Linux)
                        {
                            throw new Exception("Local Linux compilation on a Windows machine is not yet implemented.");
                        }
                        else
                        {
                            bSuccess = CompileStandalone(fpOS);
                        }
                    }
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        foreach (FalconProcessorOS fpOS in _Processors)
                        {
                            if (fpOS.OS == SystemProcessorOS.Linux)
                            {
                                bSuccess = CompileLinuxKernel(fpOS);
                            }
                            else
                            {
                                bSuccess = CompileStandalone(fpOS);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Remote Linux compilation on a Windows server is not yet implemented.");
                    }
                }
                if (bSuccess)
                {
                    RaiseMessageEvent("Complete!");
                    LogMessage(String.Format("Platform {0} Software compilation complete.", this.PlatformID));
                }
                else
                {
                    RaiseMessageEvent("ERROR!");
                    LogMessage(String.Format("Platform {0} Software compilation complete.", this.PlatformID));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                bSuccess = false;
            }
            return bSuccess;
        }
        #endregion

        #region Standalone Software Compilation
        private bool CompileStandalone(FalconProcessorOS fpOS)
        {
            try
            {
                LogMessage(String.Format("Platform {0}, Procesor {1} Performing Standalone App Compilation.", this.PlatformID, fpOS.Instance));
                string XPSScriptXMP = CreateXPSScript(true);
                string XPSScriptMHS = CreateXPSScript(false);
                FileInfo FIXPSScriptXMP = new FileInfo(XPSScriptXMP);
                FileInfo FIXPSScriptMHS = new FileInfo(XPSScriptMHS);
                string XPSArgs = string.Empty;
                string XPSArgsXMP = String.Format("-nw -scr {0}", FIXPSScriptXMP.Name);
                string XPSArgsMHS = String.Format("-nw -scr {0}", FIXPSScriptMHS.Name);

                if (this.LocalSynthesis)
                {
                    // This part is run from the ./ directory

                    File.Copy(FIXPSScriptXMP.FullName, this.PlatformDirectory + FIXPSScriptXMP.Name, true);
                    File.Copy(FIXPSScriptMHS.FullName, this.PlatformDirectory + FIXPSScriptMHS.Name, true);
                    Directory.SetCurrentDirectory(this.PlatformDirectory);
                    Process xps = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo();
                    if (File.Exists(this.PlatformDirectory + "\\system.xmp"))
                        XPSArgs = XPSArgsXMP;
                    else
                        XPSArgs = XPSArgsMHS;

                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.CreateNoWindow = true;
                    psi.FileName = XTOOL_XPS;
                    psi.Arguments = XPSArgs;
                    psi.WorkingDirectory = this.PlatformDirectory;
                    psi.UseShellExecute = false;
                    xps.StartInfo = psi;
                    xps.Start();

                    // Wait for map to terminate
                    xps.WaitForExit();
                    
                    if (xps.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        // Create script, send it, and execute it
                        string ScriptFile = String.Format("xpsmake{0}.sh", this.PlatformID);
                        FileInfo FIScript = new FileInfo(ScriptFile);
                        StreamWriter writer = new StreamWriter(ScriptFile, false);
                        writer.Write(String.Format("echo cd {0}\n", this.PlatformDirectory));
                        writer.Write(String.Format("cd {0}\n", this.PlatformDirectory));
                        writer.Write(String.Format("if [ -f {0}{1} ]; then\n", this.PlatformDirectory, "system.xmp"));
                        writer.Write(String.Format("echo {0} {1}\n", XTOOL_XPS, XPSArgsXMP));
                        writer.Write(String.Format("{0} {1}\n", XTOOL_XPS, XPSArgsXMP));
                        writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                        writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                        writer.Write(String.Format("else"));
                        writer.Write(String.Format("echo {0} {1}\n", XTOOL_XPS, XPSArgsMHS));
                        writer.Write(String.Format("{0} {1}\n", XTOOL_XPS, XPSArgsMHS));
                        writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                        writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                        writer.Write(String.Format("fi", FINISH_SUCCESS));
                        writer.Close();

                        _SSHXFer.To(FIXPSScriptXMP.FullName, this.SynthesisDirectory + FIXPSScriptXMP.Name);
                        _SSHXFer.To(FIXPSScriptMHS.FullName, this.SynthesisDirectory + FIXPSScriptMHS.Name);
                        _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + FIScript.Name);
                        FIXPSScriptXMP.Delete();
                        FIXPSScriptMHS.Delete();
                        FIScript.Delete();

                        string CommandResponse;
                        _SSHShell.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, ScriptFile));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, FIXPSScriptXMP));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", this.PlatformDirectory, FIXPSScriptMHS));
                        _SSHShell.Expect();
                        _SSHShell.WriteLine(String.Format("{2} {0}{1}", this.ImplementationDirectory, ScriptFile, LINUX_COMMAND_SHELL));
                        CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                        if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {

                        // Create script, send it, and execute it
                        string ScriptFile = String.Format("xpsmake{0}.bat", this.PlatformID);
                        FileInfo FIScript = new FileInfo(ScriptFile);
                        StreamWriter writer = new StreamWriter(ScriptFile, false);
                        writer.WriteLine(String.Format("@ECHO OFF"));
                        writer.WriteLine(String.Format("cd {0}", this.PlatformDirectory));
                        writer.WriteLine(String.Format("IF EXIST {0}{1} GOTO :use_xmp", this.PlatformDirectory));

                        writer.WriteLine(String.Format(":use_mhs"));
                        writer.WriteLine(String.Format("echo {0} {1}", XTOOL_XPS, XPSArgsMHS));
                        writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_XPS, XPSArgsMHS));
                        writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                        writer.WriteLine(String.Format("GOTO :exit_success"));

                        writer.WriteLine(String.Format(":use_xmp"));
                        writer.WriteLine(String.Format("echo {0} {1}", XTOOL_XPS, XPSArgsXMP));
                        writer.WriteLine(String.Format("start /wait {0} {1}", XTOOL_XPS, XPSArgsXMP));
                        writer.WriteLine(String.Format("IF \"%errorlevel%\" == \"1\" GOTO :exit_fail"));
                        writer.WriteLine(String.Format("GOTO :exit_success"));
                        writer.WriteLine(String.Format(""));
                        writer.WriteLine(String.Format(":exit_success"));
                        writer.WriteLine(String.Format("echo {0}", FINISH_SUCCESS));
                        writer.WriteLine(String.Format("exit /b 0"));
                        writer.WriteLine(String.Format(":exit_fail"));
                        writer.WriteLine(String.Format("echo {0}", FINISH_ERROR));
                        writer.WriteLine(String.Format("exit /b 1"));
                        writer.Close();

                        _SSHXFer.To(FIXPSScriptXMP.FullName, this.SynthesisDirectory + FIXPSScriptXMP.Name);
                        _SSHXFer.To(FIXPSScriptMHS.FullName, this.SynthesisDirectory + FIXPSScriptMHS.Name);
                        _SSHXFer.To(FIScript.FullName, this.SynthesisDirectory + FIScript.Name);
                        FIXPSScriptXMP.Delete();
                        FIXPSScriptMHS.Delete();
                        FIScript.Delete();

                        string CommandResponse;
                        _SSHShell.WriteLine(String.Format("cd {0}", this.SynthesisDirectory));
                        _SSHShell.Expect();
                        _SSHShell.WriteLine(String.Format("{0}{1}", this.SynthesisDirectory, ScriptFile));
                        CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                        if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private string CreateXPSScript(bool UseXMP)
        {
            string XPSScriptFile = String.Format("compile{0}.tcl", this.PlatformID);
            string XPSScriptPath = XPSScriptFile;
            if (File.Exists(XPSScriptPath))
                File.Delete(XPSScriptPath);
            string XPSPlatDir = this.PlatformDirectory;
            XPSPlatDir = XPSPlatDir.Replace(@"\", @"\\");

            StreamWriter writer = new StreamWriter(XPSScriptPath, false);
            writer.Write(String.Format("cd {0}\n", XPSPlatDir));
            if (UseXMP)
            {
                writer.Write(String.Format("xload xmp {0}\n", "system.xmp"));
            }
            else 
            {
                writer.Write(String.Format("xload mhs {0}\n", "system.mhs"));
                writer.Write(String.Format("xload mss {0}\n", "system.mss"));
            }
            string PLAT_CYG = ConvertToCygDriveNotation(this.PlatformDirectory);
            foreach (FalconProcessorOS fpOS in _Processors)
            {
                foreach (FalconStandaloneSoftwareApp app in fpOS.StandaloneApps)
                {
                    writer.Write(String.Format("xadd_swapp {0} {1}\n", app.Name, fpOS.Instance));
                    writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "compileroptlevel", app.CompilerOptLevel));
                    writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "executable", @"output\\" + this.PlatformID + "_" + fpOS.Instance + "_" + app.Name));

                    writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "globptropt", (app.GlobPtrOpt ? "1" : "0")));
                    writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "init_bram", (app.InitBRAM ? "1" : "0")));
                    if (app.HeapSize.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "heapsize", app.HeapSize));
                    if (app.LinkerScript.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "linkerscript", app.LinkerScript));
                    if (app.ProgCCFlags.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "progccflags", app.ProgCCFlags));
                    if (app.ProgStartAddr.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "progstart", app.ProgStartAddr));
                    if (app.StackSize.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} {2}\n", app.Name, "stacksize", app.StackSize));

                    string libs = string.Empty;
                    foreach (string Library in app.Libraries)
                    {
                        libs += ConvertToCygDriveNotation(Library) + " ";
                    }
                    libs = libs.Trim();
                    if (libs.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} \"{2}\"\n", app.Name, "lflags", libs));

                    string libdirs = string.Empty;
                    app.LibSearchPath.Add(String.Format("./{0}/lib/", fpOS.Instance));
                    foreach (string LibDir in app.LibSearchPath)
                    {
                        libdirs += ConvertToCygDriveNotation(LibDir) + " ";
                    }
                    libdirs = libdirs.Trim();
                    if (libdirs.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} \"{2}\"\n", app.Name, "searchlibs", libdirs));

                    string includes = string.Empty;
                    app.IncludeSearchPath.Add(String.Format("./{0}/include/", fpOS.Instance));
                    foreach (string Include in app.IncludeSearchPath)
                    {
                        includes += ConvertToCygDriveNotation(Include) + " ";
                    }
                    includes = includes.Trim();
                    if (includes.Length > 0)
                        writer.Write(String.Format("xset_swapp_prop_value {0} {1} \"{2}\"\n", app.Name, "searchincl", includes));
                    foreach (DirectoryFilter df in app.RuntimeSources)
                    {
                        ArrayList list = new ArrayList();
                        if (this.LocalSynthesis)
                        {
                            list = EnumLocalFiles(df.Directory, true, df.Filter, true);
                        }
                        else
                        {
                            list = EnumRemoteFiles(df.Directory, true, df.Filter);
                        }
                        foreach (object o in list)
                        {
                            string relPath = (string)o;
                            relPath = relPath.Replace(PLAT_CYG, string.Empty);
                            app.AddSource(relPath);
                        }
                    }
                    foreach (string Source in app.Sources)
                    {
                        if (Source.Length > 0)
                            writer.Write(String.Format("xadd_swapp_progfile {0} {1}\n", app.Name, Source));
                    }


                    foreach (DirectoryFilter df in app.RuntimeHeaders)
                    {
                        ArrayList list = new ArrayList();
                        if (this.LocalSynthesis)
                        {
                            list = EnumLocalFiles(df.Directory, true, df.Filter, true);
                        }
                        else
                        {
                            list = EnumRemoteFiles(df.Directory, true, df.Filter);
                        }
                        foreach (object o in list)
                        {
                            string relPath = (string)o;
                            relPath = relPath.Replace(PLAT_CYG, string.Empty);
                            app.AddHeader(relPath);
                        }
                    }
                    foreach (string Header in app.Headers)
                    {
                        if (Header.Length > 0)
                            writer.Write(String.Format("xadd_swapp_progfile {0} {1}\n", app.Name, Header));
                    }
                }
            }

            writer.Write(String.Format("save mhs\n"));
            writer.Write(String.Format("save mss\n"));
            writer.Write(String.Format("save make\n"));
            writer.Write(String.Format("save proj\n"));
            writer.Write(String.Format("run program\n"));
            writer.Write(String.Format("exit\n"));

            writer.Close();
            return XPSScriptPath;
        }

        private ArrayList EnumLocalFiles(string directory, bool IncludeSubdirectories, string Filter, bool ConvertToCygDriveFormat)
        {
            ArrayList list = new ArrayList();
            string[] dirs = Directory.GetDirectories(directory);
            DirectoryInfo di = new DirectoryInfo(directory);

            if (IncludeSubdirectories)
            {
                foreach (string dir in dirs)
                {
                        ArrayList subList = new ArrayList();
                        subList = EnumLocalFiles(dir, IncludeSubdirectories, Filter, ConvertToCygDriveFormat);
                        list.AddRange(subList);
                }
            }

            string[] files = Directory.GetFiles(di.FullName, Filter);
            foreach (string file in files)
            {
                list.Add(file);
            }
            ArrayList outList;
            if (ConvertToCygDriveFormat)
            {
                outList = new ArrayList();
                foreach (object o in list)
                {
                    outList.Add(ConvertToCygDriveNotation((string)o));
                }
            }
            else
            {
                outList = list;
            }
            return outList;
        }
        private ArrayList EnumRemoteFiles(string directory, bool IncludeSubdirectories, string Filter)
        {
            ArrayList list = new ArrayList();
            string dirName = GetPathShortName(directory);

            if (this.LinuxHost)
            {
                string listing;
                _SSHShell.WriteLine(String.Format("echo {0};ls -l {1}/{2}; echo {3}", DIR_LIST_START, directory, Filter, DIR_LIST_FINISH));
                listing = _SSHShell.Expect(DIR_LIST_FINISH);
                int startIdx = listing.LastIndexOf(DIR_LIST_START) + DIR_LIST_START.Length;
                int endIdx = listing.LastIndexOf(DIR_LIST_FINISH);

                if ((startIdx >= 0) && (endIdx >= 0) && (endIdx > startIdx))
                {
                    listing = listing.Substring(startIdx, endIdx - startIdx);
                    listing = listing.Replace("\r", string.Empty);
                    string[] listings = listing.Split('\n');
                    foreach(string item in listings)
                    {
                        string thisItem = item;
                        if (thisItem.Length == 0)
                            continue;
                        // -rw------- 1 user group  size Jun 23 15:36 name.extension
                        // drwx------ 2 user group  size Jun 23 15:53 dirname
                        //0          1 2    3     4     5   6  7     8
                        string[] columns = item.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        thisItem = string.Empty;
                        for(int i = 8; i < columns.Length; i++)
                            thisItem += columns[i] + " ";
                        thisItem = thisItem.Trim();
                        if (columns[0].StartsWith("d"))
                        {
                            // item is a directory
                            if (IncludeSubdirectories)
                            {
                                ArrayList subList = new ArrayList();
                                subList = EnumRemoteFiles(directory + "/" + thisItem, IncludeSubdirectories, Filter);
                                list.AddRange(subList);
                            }
                        }
                        else if (columns[0].StartsWith("-"))
                        {
                            // item is a file
                            if (item.EndsWith(Filter))
                            {
                                string path = directory + "/" + item;
                                path = path.Replace("//", "/");
                                list.Add(path);
                            }
                        }
                    }
                }
            }
            else 
            {
            }
            return list;
        }
        
        #endregion

        #region Linux Kernel Compilation
        private bool CompileLinuxKernel(FalconProcessorOS fpOS)
        {
            LogMessage(String.Format("Platform {0}, Procesor {1} Performing Linux Kernel Compilation.", this.PlatformID, fpOS.Instance));
            try
            {
                if (this.LocalSynthesis)
                {
                    throw new Exception("Local Linux compilation on a Windows machine is not yet implemented.");
                }
                else
                {
                    if (this.LinuxHost)
                    {
                        string devTree = GetPathShortName(this.DeviceTree);
                        string CompileScript = CreateLinuxCompileScript(fpOS.LinuxKernelSource, fpOS);
                        FileInfo FICompileScript = new FileInfo(CompileScript);

                        string SourceDTS = this.PlatformDirectory + fpOS.Instance + this.Separator + "libsrc" + this.Separator + devTree + this.Separator + "xilinx.dts";
                        string TargetDTS = String.Format("{0}arch/{1}/boot/dts/{2}.dts", fpOS.LinuxKernelSource, fpOS.Type.ToLower(), fpOS.DTSFile);
                        string OutputELF = String.Format("{0}arch/{1}/boot/simpleImage.initrd.{2}.elf", fpOS.LinuxKernelSource, fpOS.Type.ToLower(), fpOS.DTSFile);
                        // Create script, send it, and execute it
                        string ScriptFile = String.Format("compile{0}.sh", this.PlatformID);
                        FileInfo FIScript = new FileInfo(ScriptFile);
                        StreamWriter writer = new StreamWriter(ScriptFile, false);
                        writer.Write(String.Format("#!/bin/bash\n"));
                        FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
                        writer.Write(String.Format("echo rm {0}\n", TargetDTS));
                        writer.Write(String.Format("rm {0}\n", TargetDTS));
                        writer.Write(String.Format("echo cp {0} {1}\n", SourceDTS, TargetDTS));
                        writer.Write(String.Format("cp {0} {1}\n", SourceDTS, TargetDTS));
                        writer.Write(String.Format("echo cd {0}\n", fpOS.LinuxKernelSource));
                        writer.Write(String.Format("cd {0}\n", fpOS.LinuxKernelSource));
                        writer.Write(String.Format("echo rm {0}\n", OutputELF));
                        writer.Write(String.Format("rm {0}\n", OutputELF));
                        writer.Write(String.Format("echo bash {0}\n", fpOS.LinuxKernelSource + FICompileScript.Name));
                        writer.Write(String.Format("bash {0}\n", fpOS.LinuxKernelSource + FICompileScript.Name));
                        writer.Write(String.Format("if [ $? -ne 0 ]; then\n echo \"{0}\"\n exit 1\nfi\n", FINISH_ERROR));
                        writer.Write(String.Format("echo mv {0} {1}{2}\n", OutputELF, this.OutputDirectory, fpOS.OutputELF));
                        writer.Write(String.Format("mv {0} {1}{2}\n", OutputELF, this.OutputDirectory, fpOS.OutputELF));
                        writer.Write(String.Format("echo \"{0}\"", FINISH_SUCCESS));
                        writer.Close();

                        _SSHXFer.To(FICompileScript.FullName, fpOS.LinuxKernelSource + FICompileScript.Name);
                        _SSHXFer.To(FIScript.FullName, fpOS.LinuxKernelSource + FIScript.Name);
                        FICompileScript.Delete();
                        FIScript.Delete();

                        string CommandResponse;
                        _SSHShell.WriteLine(String.Format("cd {0}", fpOS.LinuxKernelSource));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", fpOS.LinuxKernelSource, CompileScript));
                        _SSHShell.WriteLine(String.Format("chmod 700 {0}{1}", fpOS.LinuxKernelSource, ScriptFile));
                        _SSHShell.Expect();
                        string CompileLog = String.Format("{0}{1}_LINUX", this.OutputDirectory, fpOS.OutputELF.Replace(".elf", ".log"));
                        _SSHShell.WriteLine(String.Format("bash {0}{1} > {2};cat {2};", fpOS.LinuxKernelSource, ScriptFile, CompileLog));
                        CommandResponse = _SSHShell.Expect(new System.Text.RegularExpressions.Regex(FINISH_EXPECT));
                        if (CommandResponse.Trim().Contains(FINISH_SUCCESS))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new Exception("Remote Linux compilation on a Windows server is not yet implemented.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }
        private string CreateLinuxCompileScript(string LinuxLocation, FalconProcessorOS fpOS)
        {
            string CompileFile = String.Format("compile{0}_{1}.sh", this.PlatformID, fpOS.Instance);
            string CompilePath = CompileFile;
            if (File.Exists(CompilePath))
                File.Delete(CompilePath);

            StreamWriter writer = new StreamWriter(CompilePath, false);
            writer.Write(String.Format("#!/bin/bash\n"));
            FalconFileRoutines.WriteCerebrumDisclaimer(writer, "# ");   // Added for auto-generated shell script file.
            writer.Write(String.Format("echo \"====================================================\"\n"));
            writer.Write(String.Format("export CEREBRUM_PROC_ID={0}\n", fpOS.CerebrumProcessorID));
            writer.Write(String.Format("rm {0}include/linux/compile.h\n", fpOS.LinuxKernelSource));

            string CompiledELF = string.Empty;
            string OutputELF = String.Format("{0}arch/{1}/boot/simpleImage.initrd.{2}.elf", 
                fpOS.LinuxKernelSource, 
                fpOS.Type.ToLower(), 
                fpOS.DTSFile);

            if (fpOS.Type.ToLower() == "powerpc")
            {
                // POWERPC LINUX COMPILATION SCRIPT
                CompiledELF = String.Format("{0}arch/{1}/boot/simpleImage.initrd.{2}.elf",
                    fpOS.LinuxKernelSource,
                    fpOS.Type.ToLower(),
                    fpOS.DTSFile);
                writer.Write(String.Format("echo \"Configuring Chain Compiler and Environment...\"\n"));
                writer.Write(String.Format("source {0}/eldk_init {1}\n", this.ELDKLocation, fpOS.CompilerArgs));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write(String.Format("export ARCH={0}\n", fpOS.Type.ToLower()));
                writer.Write(String.Format("echo $ARCH\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");

                writer.Write(String.Format("echo \"Cleaning up...\"\n"));
                writer.Write(String.Format("make mrproper\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write(String.Format("make clean\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");

                writer.Write(String.Format("echo  \"Setting configuration file...\"\n"));
                writer.Write(String.Format("make {0}\n", fpOS.MakeConfig));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");

                writer.Write(String.Format("echo \"Generating kernel image for {0}\"\n", fpOS.DTSFile));
                writer.Write(String.Format("make simpleImage.initrd.{0}\n", fpOS.DTSFile));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");
            }
            else if (fpOS.Type.ToLower() == "microblaze")
            {
                // MICROBLAZE LINUX COMPILATION SCRIPT
                CompiledELF = String.Format("{0}arch/{1}/boot/simpleImage.{2}",
                    fpOS.LinuxKernelSource,
                    fpOS.Type.ToLower(),
                    fpOS.DTSFile);
                writer.Write(String.Format("echo \"Configuring Chain Compiler and Environment...\"\n"));
                writer.Write(String.Format("EXISTINPATH=`echo $PATH|grep {0}:`\n", this.MBGNULocation));
                writer.Write(String.Format("if [ \"$EXISTINPATH\" == \"\" ]; then\n"));
                writer.Write(String.Format("  export PATH={0}:$PATH\n", this.MBGNULocation));
                writer.Write(String.Format("  echo $PATH\n"));
                writer.Write(String.Format("fi\n"));
                writer.Write(String.Format("export CROSS_COMPILE=mb-linux-\n"));
                writer.Write(String.Format("echo $CROSS_COMPILE\n"));
                writer.Write(String.Format("export ARCH={0}\n", fpOS.Type.ToLower()));
                writer.Write(String.Format("echo $ARCH\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");

                writer.Write(String.Format("echo \"Cleaning up...\"\n"));
                writer.Write(String.Format("make mrproper\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write(String.Format("make clean\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");
                                
                writer.Write(String.Format("echo  \"Setting configuration file...\"\n"));
                writer.Write(String.Format("make xilinx_mmu_defconfig\n"));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write(String.Format("cp arch/microblaze/configs/{0} .config\n", fpOS.MakeConfig));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));
                writer.Write("\n");
                
                writer.Write(String.Format("echo \"Generating kernel image for {0}\"\n", fpOS.DTSFile));
                writer.Write(String.Format("make simpleImage.{0}\n", fpOS.DTSFile));
                writer.Write(String.Format("if [ $? -ne 0 ]; then\n exit 1\nfi\n", FINISH_ERROR));

                // Rename the output file so that it matches the expected file name after completion
                writer.Write(String.Format("echo \"Renaming Microblaze ELF File\"\n"));
                writer.Write(String.Format("echo mv {0} {1}\n",
                        CompiledELF,
                        OutputELF)); 
                writer.Write(String.Format("mv {0} {1}\n",
                         CompiledELF,
                         OutputELF));
            }
            writer.Write(String.Format("echo \"====================================================\"\n"));
            writer.Write(String.Format("exit 0\n"));

            writer.Close();
            return CompilePath;
        }        
        #endregion
        
        #endregion /********************* Software Compilation End ********************************/

        #region Post-Synthesis Resource Estimate Extraction

        /// <summary>
        /// Parse Post-Synthesis Report Files and Generate a Unified Resource Utilization Report
        /// </summary>
        public void CompileResourceReport()
        {
            char DivChar = '-';
            int DivCount = 80;
            string EstimateOutputDir = _PathMan.GetPath("LocalProjectRoot") + "\\output_files";
            string ProjectOutputDir = _PathMan.GetPath("LocalProject") + "\\" + this.PlatformID + "\\output";

            #region Initialize Dictionaries for Storing Resource Estimates
            Dictionary<string, Dictionary<string, Dictionary<string, long>>> ComponentsUsed = new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();
            Dictionary<string, long> TotalUsed = new Dictionary<string, long>();
            Dictionary<string, long> TotalAvailable = new Dictionary<string, long>();
            #endregion

            try
            {
                #region Iterate over PCores on FPGA, and Parse their synthesis reports
                foreach (ComponentCore CompCore in this.AssignedPCores)
                {
                    string DEVICE_UTIL_MARKER = "Device utilization summary";
                    string RESOURCE_MARKER = " Number of ";
                    string END_RESOURCE_MARKER = "Partition Resource Summary";

                    string Instance = CompCore.CoreInstance;
                    string ReportFile = String.Format("{0}\\{1}_wrapper_xst.srp", ProjectOutputDir, Instance);
                    if (File.Exists(ReportFile))
                    {
                        StreamReader reader = new StreamReader(ReportFile);
                        string ReportLine = string.Empty;

                        bool bFoundDeviceUtil = false;

                        #region Parse Synthesis Report
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.StartsWith(END_RESOURCE_MARKER))
                            {
                                break;
                            }
                            if (line.StartsWith(DEVICE_UTIL_MARKER))
                            {
                                bFoundDeviceUtil = true;
                                continue;
                            }
                            if (bFoundDeviceUtil)
                            {
                                if (line.StartsWith(RESOURCE_MARKER))
                                {
                                    #region Extract Resource Estimate
                                    string ResLine = line.Replace(RESOURCE_MARKER, string.Empty);
                                    while (ResLine.Contains("  "))
                                        ResLine = ResLine.Replace("  ", " ");
                                    ResLine = ResLine.Trim();
                                    // Line should now look like the following
                                    // "[resource]: [number] out of [total] [percent]%"

                                    int ColonIndex = ResLine.IndexOf(":");
                                    int OutOfIndex = ResLine.IndexOf(" out of ");
                                    int LastSpaceIndex = ResLine.LastIndexOf(" ");

                                    string Resource = string.Empty;
                                    string Number = string.Empty;
                                    long iNumber = 0;
                                    string Avail = string.Empty;
                                    long iAvail = 0;
                                    //string Percent = string.Empty;

                                    Resource = ResLine.Substring(0, ColonIndex).Trim();
                                    if (String.Compare(Resource, "Slice Registers") == 0)
                                    {
                                    }
                                    else if (String.Compare(Resource, "Slice LUTs") == 0)
                                    {
                                    }
                                    else if (String.Compare(Resource, "Block RAM/FIFO") == 0)
                                    {
                                        Resource = "BRAMs";
                                    }
                                    else if (String.Compare(Resource, "DSP48Es") == 0)
                                    {
                                        Resource = "DSPs";
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    if (OutOfIndex > 0)
                                    {
                                        Number = ResLine.Substring(ColonIndex + 1, OutOfIndex - ColonIndex).Trim();
                                        Avail = ResLine.Substring(OutOfIndex + 8, LastSpaceIndex - (OutOfIndex + 8)).Trim(new char[] { '%', ' ' });
                                    }
                                    else
                                    {
                                        Number = ResLine.Substring(ColonIndex + 1).Trim();
                                    }
                                    if (!long.TryParse(Number, out iNumber)) continue;
                                    if (!long.TryParse(Avail, out iAvail)) continue;
                                    #endregion

                                    #region Store Resource Estimate for Later Reporting
                                    string ComponentInstance = string.Empty;
                                    if (CompCore.OwnerComponent != null)
                                    {
                                        ComponentInstance = CompCore.OwnerComponent.CoreInstance;
                                    }
                                    else
                                    {
                                        ComponentInstance = "Required Components/Infrastructure";
                                    }

                                    string CoreInstance = CompCore.CoreInstance;

                                    // Add to Component
                                    if (!ComponentsUsed.ContainsKey(ComponentInstance))
                                        ComponentsUsed.Add(ComponentInstance, new Dictionary<string, Dictionary<string, long>>());

                                    if (!ComponentsUsed[ComponentInstance].ContainsKey(CoreInstance))
                                        ComponentsUsed[ComponentInstance].Add(CoreInstance, new Dictionary<string, long>());

                                    if (!ComponentsUsed[ComponentInstance][CoreInstance].ContainsKey(Resource))
                                        ComponentsUsed[ComponentInstance][CoreInstance].Add(Resource, 0);

                                    ComponentsUsed[ComponentInstance][CoreInstance][Resource] = ComponentsUsed[ComponentInstance][CoreInstance][Resource] + iNumber;



                                    // Add to Total
                                    if (!TotalUsed.ContainsKey(Resource))
                                        TotalUsed.Add(Resource, 0);
                                    TotalUsed[Resource] = TotalUsed[Resource] + iNumber;

                                    // Set Available
                                    if (!TotalAvailable.ContainsKey(Resource))
                                        TotalAvailable.Add(Resource, 0);
                                    TotalAvailable[Resource] = iAvail;
                                    #endregion
                                }
                            }
                        }
                        reader.Close();
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }

            StreamWriter ReportOutput = null;
            try
            {
                // Write the results to a report
                #region Create File, Deleting it if it previously existed
                FileInfo Report = new FileInfo(String.Format("{0}\\{1}_resource_report.txt", EstimateOutputDir, this.PlatformID));
                if (Report.Exists)
                {
                    try
                    {
                        Report.Delete();
                    }
                    finally
                    { }
                }
                ReportOutput = new StreamWriter(Report.FullName);
                #endregion

                #region Write Report Header
                ReportOutput.WriteLine(String.Format("Post-Synthesis Resource Report"));
                ReportOutput.WriteLine(String.Format("Generated for FPGA {0}, as part of Platform {1}", this.PlatformID, _PathMan["ProjectPlatform"]));
                ReportOutput.WriteLine(String.Format("Generated on {0} at {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()));
                ReportOutput.WriteLine(String.Format("{0}", new String(DivChar, DivCount)));
                ReportOutput.WriteLine(String.Format("{0}", new String(DivChar, DivCount)));
                ReportOutput.WriteLine(String.Format("{0}", new String(DivChar, DivCount)));
                ReportOutput.WriteLine();
                #endregion

                #region Write Total Utilization first
                ReportOutput.WriteLine(String.Format("Overall Primary Resource Utilization"));
                foreach (KeyValuePair<string, long> KV in TotalUsed)
                {
                    string Resource = KV.Key;
                    string Used = KV.Value.ToString();
                    string Available = (TotalAvailable.ContainsKey(Resource) ? TotalAvailable[Resource].ToString() : "UNKNOWN");
                    string Percent = (TotalAvailable.ContainsKey(Resource) ? ((double)(KV.Value * 100) / (double)TotalAvailable[Resource]).ToString("#0.00") : "--");

                    ReportOutput.WriteLine(String.Format("\t{0,-20} {1,9} out of {2,10} used({3,6}%)", Resource, Used, Available, Percent));
                }
                ReportOutput.WriteLine();
                ReportOutput.WriteLine(String.Format("{0}", new String(DivChar, DivCount)));
                #endregion

                #region Write Component Utilization
                foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, long>>> CCKV in ComponentsUsed)
                {
                    string ComponentInstance = CCKV.Key;
                    Dictionary<string, long> ComponentTotals = new Dictionary<string, long>();

                    #region First Calculate Component Totals
                    foreach (KeyValuePair<string, Dictionary<string, long>> CKV in CCKV.Value)
                    {
                        string CoreInstance = CKV.Key;

                        foreach (KeyValuePair<string, long> KV in CKV.Value)
                        {
                            string Resource = KV.Key;
                            long Used = KV.Value;

                            if (!ComponentTotals.ContainsKey(Resource))
                                ComponentTotals.Add(Resource, 0);

                            ComponentTotals[Resource] = ComponentTotals[Resource] + Used;
                        }
                    }
                    #endregion


                    #region Write Calculate Component Totals
                    ReportOutput.WriteLine(String.Format("\t + Component: {0}", ComponentInstance));
                    foreach (KeyValuePair<string, long> KV in ComponentTotals)
                    {
                        string Resource = KV.Key;
                        string Used = KV.Value.ToString();
                        string Available = (TotalAvailable.ContainsKey(Resource) ? TotalAvailable[Resource].ToString() : "UNKNOWN");
                        string Percent = (TotalAvailable.ContainsKey(Resource) ? ((double)(KV.Value * 100) / (double)TotalAvailable[Resource]).ToString("#0.00") : "--");

                        ReportOutput.WriteLine(String.Format("\t\t{0,-20} {1,9} out of {2,10} used ({3,6}%)", Resource, Used, Available, Percent));
                    }
                    #endregion


                    #region Write Per-PCore Breakdown Totals
                    foreach (KeyValuePair<string, Dictionary<string, long>> CKV in CCKV.Value)
                    {
                        string CoreInstance = CKV.Key;
                        ReportOutput.WriteLine();
                        ReportOutput.WriteLine(String.Format("\t\t + Core: {1} [of Component {0}]", ComponentInstance, CoreInstance));

                        foreach (KeyValuePair<string, long> KV in CKV.Value)
                        {
                            string Resource = KV.Key;
                            string Used = KV.Value.ToString();
                            string Available = (TotalAvailable.ContainsKey(Resource) ? TotalAvailable[Resource].ToString() : "UNKNOWN");
                            string Percent = (TotalAvailable.ContainsKey(Resource) ? ((double)(KV.Value * 100) / (double)TotalAvailable[Resource]).ToString("#0.00") : "--");
                            string ComponentTotal = (ComponentTotals.ContainsKey(Resource) ? ComponentTotals[Resource].ToString() : "UNKNOWN");
                            string ComponentPercent = (ComponentTotals.ContainsKey(Resource) ? ((double)(KV.Value * 100) / (double)ComponentTotals[Resource]).ToString("#0.00") : "--");

                            ReportOutput.WriteLine(String.Format("\t\t\t{0,-20} {1,9} out of {2,10} [{4,10}] used ({3,6}% [{5,6}%])",
                                Resource, Used, Available, Percent, ComponentTotal, ComponentPercent));
                        }
                    }
                    ReportOutput.WriteLine(String.Format("{0}", new String(DivChar, DivCount)));
                    #endregion
                }
                #endregion

                ReportOutput.WriteLine(String.Format("End of Resource Utilization Report"));
                ReportOutput.Close();
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            finally
            {
                if (ReportOutput != null)
                {
                    try
                    {
                        ReportOutput.Close();
                    }
                    finally { }
                }
            }
        }
        #endregion


        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon Platform Synthesis Object";
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

