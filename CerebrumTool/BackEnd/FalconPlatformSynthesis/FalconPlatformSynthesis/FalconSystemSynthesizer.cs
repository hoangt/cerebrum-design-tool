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
 * FalconPlatformSynthesis\FalconSystemSynthesizer.cs
 * Name: Matthew Cotter
 * Date: 24 Jun 2010 
 * Description: Library to automate hardware synthesis and software compilation of an FPGA platform using Xilinx tools.
 * Notes:
 *     
 * History: 
 * >> (18 May 2010) Matthew Cotter: Implemented support for parsing and reporting post-synthesis resource utilization, for possible use in future resource estimations.
 * >> (13 May 2010) Matthew Cotter: Implemented support for synthesizing a specific subset of FPGAs from the platform.
 * >> (10 May 2010) Matthew Cotter: Implemented multithreaded synthesis, with each FPGA process being synthesized on its own thread, using its own SSH connections.
 * >> (27 Feb 2011) Matthew Cotter: Completed basic functionality of BRAM reprogramming methods
 * >> (25 Feb 2011) Matthew Cotter: Initial work on BlockRAM reprogrammability.
 * >> (18 Feb 2011) Matthew Cotter: Added methods to load mapped components/cores, and assign them to loaded FPGAs.
 * >> (10 Feb 2011) Matthew Cotter: Initial work on verifying parallelization of synthesis and implementation of BRAM programming.
 * >> (26 Aug 2010) Matthew Cotter: Added auto-generation of Cerebrum Processor ID if it is not specified in the file.
 * >> (13 Aug 2010) Matthew Cotter: Updated code that handles loading of Platform files to use new hierarchical location and format of platforms 
 *                                    (paths.ProjectPlatform -> paths.Platforms\<Platform>\<Platform.xml> -> 
 *                                      paths.Platforms\<Platform>\<Board>\<Board>.xml -> paths.Platforms\<Platform>\<Board>\<fpga>\<fpga>.xml.
 * >> (12 Aug 2010) Matthew Cotter: Early support for compiling Linux for Microblaze.
 * >> (11 Aug 2010) Matthew Cotter: Properties that have been proposed to be moved from Platform/Board to Design.Processors have been reverted pending approval of changes.
 * >> ( 2 Aug 2010) Matthew Cotter: Added ForceClean property to enforce that previous synthesis results must be cleaned prior to starting the current synthesis.
 *                                    Added initial functionality for Synthesizing Hardware only, or Software only, for testing purposes.
 * >> ( 2 Aug 2010) Matthew Cotter: Moved properties and paths specific to the cross-compilation of Linux for a processor to the FalconProcessorOS class.
 *                                    Moved parsing of relevant data to the processor parsing function, and therefore input data was relocated to design.xml.
 * >> (21 Jul 2010) Matthew Cotter: Verified that XML-generated Documentation is current.
 * >> ( 6 Jul 2010) Matthew Cotter: Updated LoadPlatformFile(), ProcessPlatforms(), and ProcessBoard() to handle hierarchical format of system platform.
 *                                   Moved DTSFile, ELDKInitArg and MakeConfig properties from FalconProcessorOS to FalconPlatform due to location in the board specification.
 * >> (24 Jun 2010) Matthew Cotter: Created functions to Compile and Synthesize multiple platforms (Information read in from external files).
 * >> (24 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Tamir;
using Tamir.SharpSsh;

using FalconGlobal;
using FalconPathManager;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconPlatformSynthesis
{
    /// <summary>
    /// Delegate method used to provide the list of loaded cores to an outside method, that will populate the appropriate PurgeBeforeSynthesis properties of the 
    /// CerebrumCore and/or ComponentCore objects.
    /// </summary>
    /// <param name="FPGAs">A List of the FPGA IDs that were loaded for this project.</param>
    /// <param name="LoadedComponents">(ref) A List of the CerebrumCore that were loaded for this project.</param>
    /// <param name="LoadedCores">(ref) A List of the ComponentCore that were loaded for this project.</param>
    public delegate bool SelectivePurgeEnumerationDelegate(List<string> FPGAs, ref List<CerebrumCore> LoadedComponents, ref List<ComponentCore> LoadedCores);
    /// <summary>
    /// Delegate method used to provide the list of loaded FPGAs to an outside method, that will determine which should be synthesized.
    /// </summary>
    /// <param name="FPGAs">A List of the FPGA IDs that were loaded for this project.</param>
    public delegate bool SelectiveSynthesisDelegate(ref Dictionary<string, bool> FPGAs);

    /// <summary>
    /// Class to handle multi-platform synthesis and compilation by reading files detailing the design, the platform(s), and servers.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/System Synthesis Specification.pdf">
    /// System Synthesis Documentation</seealso>
    public class FalconSystemSynthesizer : IFalconLibrary
    {
        private class SynthesisThreadInfo
        {
            public FalconPlatform Platform { get; set; }
            public string PurgeFilter { get; set; }
            public SynthesisThreadInfo(FalconPlatform fp, string purge)
            {
                this.Platform = fp;
                this.PurgeFilter = purge;
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
        public void RaiseMessageEvent(string Message)
        {
            if (MessageEvent != null)
            {
                MessageEvent(Message);
            }
            //else
            //{
            //    RaiseMessageEvent(Message);
            //}
        }
        /// <summary>
        /// Raises an event indicating that a message has been generated.
        /// </summary>
        /// <param name="Message">The message to be transmitted</param>
        /// <param name="Args">List of replacements for token placeholders in the message.</param>
        public void RaiseMessage(string Message, params object[] Args)
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
            RaiseMessageEvent(outputText);
        }

        /// <summary>
        /// Delegate that is invoked to request a password from an external source.  If this delegate is null,
        /// the password is requested from the console.
        /// </summary>
        public PasswordRequestDelegate OnRequirePassword;

        /// <summary>
        /// Delegate that is invoked to request selective core purge parameters.  If this delegate is null,
        /// then Selective purge is disabled.
        /// </summary>
        public SelectivePurgeEnumerationDelegate OnRequestSelectivePurge;

        /// <summary>
        /// Delegate that is invoked to request selective FPGA synthesis permission.  If this delegate is null,
        /// then Selective Synthesis is disabled and all FPGAs are synthesized.
        /// </summary>
        public SelectiveSynthesisDelegate OnRequestSelectiveSynthesis;

        private string RequestPassword(string User, string Server)
        {
            if (OnRequirePassword != null)
            {
                return OnRequirePassword(User, Server);
            }
            else
            {
                return string.Empty;
            }
        }

        private List<CerebrumCore> _Components;
        private List<ComponentCore> _CompCores;
        private List<FalconPlatform> _Platforms;
        private LinkedList<FalconServer> _Servers;
        private List<string> _LocalIPs;
        private string _LocalHost = string.Empty;

        private LinkedListNode<FalconServer> _nextServer;
        private FalconServer GetNextServer()
        {
            if (_nextServer == null)
            {
                _nextServer = _Servers.First;
            }
            else
            {
                _nextServer = _nextServer.Next;
                if (_nextServer == null)
                    _nextServer = _Servers.First;
            }

            if (_nextServer != null)
                return _nextServer.Value;
            else
                return null;
        }
        private PathManager PathMan;

        /// <summary>
        /// Get or set whether previous synthesis results should be cleaned prior to beginning synthesis.
        /// </summary>
        public bool ForceClean { get; set; }
        /// <summary>
        /// Get or set whether selective cleaning of specific PCores should be performed.
        /// </summary>
        public bool SelectiveClean { get; set; }
        /// <summary>
        /// Get or set whether selective synthesis of a subset of the FPGAs available should be performed.
        /// </summary>
        public bool SelectiveSynthesis { get; set; }
        /// <summary>
        /// Get or set whether only Software should be compiled.  If SoftwareOnly == HardwareOnly, this value is ignored.
        /// </summary>
        public bool SoftwareOnly { get; set; }
        /// <summary>
        /// Get or set whether only Hardware should be synthesized.  If SoftwareOnly == HardwareOnly, this value is ignored.
        /// </summary>
        public bool HardwareOnly { get; set; }
        /// <summary>
        /// Get or set a flag indicating whether hardware should be synthesized in addition to compiling and merging PE Code sources.
        /// </summary>
        public bool PerformFullSynthesis { get; set; }


        /// <summary>
        /// Constructor to initialize the fields and collections to default values.
        /// </summary>
        public FalconSystemSynthesizer()
        {
            Reset();
        }

        /// <summary>
        /// Resets all collections and fields to their default values, wiping out any information loaded into the system.
        /// </summary>
        public void Reset()
        {
            this.ForceClean = false;
            this.SelectiveClean = false;
            this.SelectiveSynthesis = false;
            _Platforms = new List<FalconPlatform>();
            _Servers = new LinkedList<FalconServer>();
            _LocalIPs = new List<string>();

            _LocalHost = System.Net.Dns.GetHostName();
            foreach (IPAddress ipa in System.Net.Dns.GetHostEntry(_LocalHost).AddressList)
            {
                _LocalIPs.Add(ipa.ToString());
            }
        }

        /// <summary>
        /// Determines if the loaded platforms are ready for synthesis.
        /// </summary>
        /// <returns>True if all platforms are ready, False if any are not or no platforms have been loaded.</returns>
        public bool Ready()
        {
            if (_Platforms.Count == 0)
                return false;

            bool bReady = true;
            foreach (FalconPlatform fp in _Platforms)
                bReady = bReady && fp.Ready();
            return bReady;
        }

        /// <summary>
        /// Loads an XML file specifying the design to be synthesized.
        /// </summary>
        /// <param name="PathFile">The path to the XML file specifying the project paths.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Project Paths)</seealso>
        public bool LoadPathsFile(string PathFile)
        {
            try
            {
                PathMan = new PathManager(PathFile);
                RaiseMessage("Loaded Paths from: {0}", PathFile);
            }
            catch (Exception ex)
            {
                RaiseMessageEvent(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Loads an XML file specifying the design to be synthesized.
        /// </summary>
        /// <param name="DesignFile">The path to the XML file specifying the design.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Design)</seealso>
        public bool LoadDesignFile(string DesignFile)
        {
            try
            {
                bool bSaveRequired = false;

                if (!File.Exists(DesignFile))
                    DesignFile = PathMan.GetPath("LocalProjectRoot") + "\\" + DesignFile;
                if (!File.Exists(DesignFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(DesignFile);
                RaiseMessage("Loaded Design from: {0}", DesignFile);
                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "design")
                    {
                        #region Load the processors
                        foreach (XmlNode xDesNode in xElem.ChildNodes)
                        {
                            if (xDesNode.Name.ToLower() == "processors")
                            {
                                foreach (XmlNode xProcNode in xDesNode.ChildNodes)
                                {
                                    if (String.Compare(xProcNode.Name, "processor", true) == 0)
                                    {
                                        string deviceID = string.Empty;
                                        bool bInvalidOS = false;
                                        //bool bInvalidType = false;
                                        FalconProcessorOS fpOS = new FalconProcessorOS();
                                        fpOS.Instance = string.Empty;
                                        foreach (XmlAttribute xAttr in xProcNode.Attributes)
                                        {
                                            if (String.Compare(xAttr.Name, "Instance", true) == 0)
                                            {
                                                fpOS.Instance = xAttr.Value;
                                            }
                                        }
                                        if (fpOS.Instance != string.Empty)
                                        {
                                            foreach (XmlNode xProcProp in xProcNode.ChildNodes)
                                            {
                                                if (String.Compare(xProcProp.Name, "FPGA", true) == 0)
                                                {
                                                    deviceID = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "os", true) == 0)
                                                {
                                                    string ostype = xProcProp.InnerText;
                                                    if (String.Compare(ostype, "linux", true) == 0)
                                                    {
                                                        fpOS.OS = SystemProcessorOS.Linux;
                                                    }
                                                    else if (String.Compare(ostype, "standalone", true) == 0)
                                                    {
                                                        fpOS.OS = SystemProcessorOS.Standalone;
                                                    }
                                                    else
                                                    {
                                                        bInvalidOS = true;
                                                        fpOS.OS = SystemProcessorOS.Linux;
                                                    }
                                                }
                                                else if (String.Compare(xProcProp.Name, "type", true) == 0)
                                                {
                                                    fpOS.Type = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "consoledevice", true) == 0)
                                                {
                                                    fpOS.ConsoleDevice = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "dts", true) == 0)
                                                {
                                                    fpOS.DTSFile = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "makeconfig", true) == 0)
                                                {
                                                    fpOS.MakeConfig = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "compilerargs", true) == 0)
                                                {
                                                    fpOS.CompilerArgs = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "linuxkernelsource", true) == 0)
                                                {
                                                    fpOS.LinuxKernelSource = xProcProp.InnerText;
                                                }
                                                else if (String.Compare(xProcProp.Name, "cerebrumid", true) == 0)
                                                {
                                                    fpOS.CerebrumProcessorID = xProcProp.InnerText;
                                                }
                                            }
                                            if (fpOS.CerebrumProcessorID == string.Empty)
                                            {
                                                bSaveRequired = true;
                                                fpOS.CerebrumProcessorID = Guid.NewGuid().ToString();
                                                fpOS.CerebrumProcessorID = fpOS.CerebrumProcessorID.Replace("{", string.Empty);
                                                fpOS.CerebrumProcessorID = fpOS.CerebrumProcessorID.Replace("}", string.Empty);
                                            }
                                            if (bInvalidOS)
                                                RaiseMessage("Warning: Invalid OS specified for Processor {0}, defaulting to Linux", fpOS.Instance);
                                            //if (bInvalidType)
                                            //    RaiseMessageEvent("Warning: Invalid Type specified for Processor {0}, defaulting to PowerPC", fpOS.Instance);
                                            
                                            // Find the associated platform
                                            FalconPlatform pf = null;
                                            foreach (FalconPlatform plat in _Platforms)
                                            {
                                                if (plat.PlatformID == deviceID)
                                                {
                                                    pf = plat;
                                                    break;
                                                }
                                            }
                                            if (pf != null)
                                            {
                                                fpOS.OutputELF = pf.PlatformID + "_" + fpOS.Instance + ".elf";
                                                if (fpOS.LinuxKernelSource == string.Empty)
                                                {
                                                    RaiseMessage("\t Processor {0}.{1} using default Linux Kernel Source.", pf.PlatformID, fpOS.Instance);
                                                    fpOS.LinuxKernelSource = pf.LinuxKernelLocation;
                                                }
                                                else
                                                {
                                                    RaiseMessage("\t Processor {0}.{1} overriding default Linux Kernel Source.", pf.PlatformID, fpOS.Instance);
                                                }
                                                pf.Processors.Add(fpOS);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Link Processors to owner-components for DTS Purge

                        foreach (XmlNode xDesNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xDesNode.Name, "Logic", true) == 0)
                            {
                                foreach (XmlNode xCoreNode in xDesNode.ChildNodes)
                                {
                                    string ComponentName = string.Empty;
                                    foreach (XmlAttribute xCoreAttr in xCoreNode.Attributes)
                                    {
                                        if (string.Compare(xCoreAttr.Name, "ID", true) == 0)
                                        {
                                            ComponentName = xCoreAttr.Value;
                                            break;
                                        }
                                    }
                                    if (ComponentName != string.Empty)
                                    {
                                        bool bFoundProc = false;
                                        foreach (FalconPlatform fp in _Platforms)
                                        {
                                            foreach (FalconProcessorOS fpOS in fp.Processors)
                                            {
                                                if (fpOS.Instance.StartsWith(ComponentName))
                                                {
                                                    bFoundProc = true;
                                                    fpOS.OwnerComponent = ComponentName;
                                                    break;
                                                }
                                            }
                                            if (bFoundProc)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        return true;
                    }
                }
                if (bSaveRequired)
                    xDoc.Save(DesignFile);
            }
            catch (Exception ex)
            {
                RaiseMessageEvent(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads the individual FPGA platforms from the master platform file.
        /// </summary>
        /// <returns>True if loading the platforms was successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Platform)</seealso>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/Platform XML Specifications.pdf">
        /// Platform XML File Documentation</seealso>
        public bool LoadPlatformFile()
        {
            try
            {
                FileInfo PlatformFile = new FileInfo(String.Format(@"{0}\{1}\{1}.xml", PathMan["Platforms"], PathMan["ProjectPlatform"]));
                if (!PlatformFile.Exists)
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(PlatformFile.FullName);
                _Platforms = new List<FalconPlatform>();
                _Platforms.Clear();

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "platform")
                    {
                        foreach (XmlNode xBoardNode in xElem.ChildNodes)
                        {
                            if (xBoardNode.Name.ToLower() == "board")
                            {
                                string boardFile = string.Empty;
                                string boardID = string.Empty;
                                foreach (XmlAttribute xAttr in xBoardNode.Attributes)
                                {
                                    if (xAttr.Name.ToLower() == "file")
                                    {
                                        boardFile = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "id")
                                    {
                                        boardID = xAttr.Value;
                                    }
                                }
                                if ((boardID != string.Empty) && (boardFile != string.Empty))
                                {
                                    ProcessBoard(boardID, boardFile);
                                }
                            }
                        }
                    }
                }
                RaiseMessage("Loaded Platform from: {0}", PlatformFile);
                LoadXPSMapFile();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        private void ProcessBoard(string boardID, string BoardFileName)
        {
            string BoardShortName = BoardFileName.Substring(0, BoardFileName.IndexOf("."));
            FileInfo BoardFile = new FileInfo(String.Format(@"{0}\{1}\{2}\{2}.xml", PathMan["Platforms"], PathMan["ProjectPlatform"], BoardShortName));
            if (!BoardFile.Exists)
                return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(BoardFile.FullName);

            foreach (XmlNode xElem in xDoc.ChildNodes)
            {
                if (xElem.Name.ToLower() == "board")
                {
                    foreach (XmlNode xBoardNode in xElem.ChildNodes)
                    {
                        if (xBoardNode.Name.ToLower() == "fpga")
                        {
                            FalconPlatform currentFPGA = null;

                            string fpgaFile = string.Empty;
                            string fpgaID = string.Empty;
                            foreach (XmlAttribute xAttr in xBoardNode.Attributes)
                            {
                                if (xAttr.Name.ToLower() == "file")
                                {
                                    fpgaFile = xAttr.Value;
                                }
                                else if (xAttr.Name.ToLower() == "id")
                                {
                                    fpgaID = xAttr.Value;
                                }
                            }
                            if ((fpgaID != string.Empty) && (fpgaFile != string.Empty))
                            {
                                currentFPGA = ProcessFPGA(boardID, BoardShortName, fpgaID, fpgaFile);
                            }
                            if (currentFPGA != null)
                            {
                                string bgScriptPath = string.Empty;

                                foreach (XmlNode xBitGenScriptNode in xBoardNode.ChildNodes)
                                {
                                    if (String.Compare(xBitGenScriptNode.Name, "BitGenScript", true) == 0)
                                    {
                                        foreach(XmlAttribute xAttr in xBitGenScriptNode.Attributes)
                                        {
                                            if (String.Compare(xAttr.Name, "File", true) == 0)
                                            {
                                                bgScriptPath = xAttr.Value;
                                                break;
                                            }
                                        }
                                    }
                                }
                                currentFPGA.BitGenScriptPath = String.Format("{0}\\{1}", BoardFile.Directory.FullName, bgScriptPath);
                            }
                        }
                    }
                }
            }
        }
        private FalconPlatform ProcessFPGA(string boardID, string BoardShortName, string FPGAID, string FPGAFileName)
        {
            string FPGAShortName = FPGAFileName.Substring(0, FPGAFileName.IndexOf("."));
            FileInfo FPGAFile = new FileInfo(String.Format(@"{0}\{1}\{2}\{3}\{3}.xml", PathMan["Platforms"], PathMan["ProjectPlatform"], BoardShortName, FPGAShortName));
            if (!FPGAFile.Exists)
                return null;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(FPGAFile.FullName);
            foreach (XmlNode xNode in xDoc.ChildNodes)
            {
                if (xNode.Name.ToLower() != "xml")
                {
                    foreach (XmlNode xBoardItem in xNode.ChildNodes)
                    {
                        if (xBoardItem.Name.ToLower() == "fpga")
                        {
                            FalconPlatform fp = new FalconPlatform();
                            fp.ProjectPathManager = PathMan;
                            // Get next synthesis server in circular list
                            fp.SynthesisServer = GetNextServer();
                            fp.LocalSynthesis = (_LocalIPs.Contains(fp.SynthesisServer.Address)) || (_LocalHost == fp.SynthesisServer.Address);
                            fp.LinuxHost = fp.SynthesisServer.LinuxHost;
                            fp.PlatGenHDLLang = HDLLanguage.VHDL;
                            fp.SystemBMM = string.Empty;    // Check whether this is generated by Synthesis??

                            // Get Key information
                            foreach (XmlAttribute xAttr in xBoardItem.Attributes)
                            {
                                if (xAttr.Name.ToLower() == "device")
                                {
                                    fp.PlatGenXPartSize = xAttr.Value;
                                }
                                else if (xAttr.Name.ToLower() == "package")
                                {
                                    fp.PlatGenXPartPackage = xAttr.Value;
                                }
                                else if (xAttr.Name.ToLower() == "speed_grade")
                                {
                                    fp.PlatGenXPartSpeedGrade = xAttr.Value;
                                }
                                else if (xAttr.Name.ToLower() == "jtag_position")
                                {
                                }
                            }

                            fp.PlatformID = boardID + "." + FPGAID;
                            fp.SystemPrefix = new DirectoryInfo(PathMan.GetPath("LocalProjectRoot")).Name;
                            fp.PlatformDirectory = PathMan.GetPath("RemoteProject") + "/" + fp.PlatformID;
                            fp.XilinxEDKDirectory = PathMan.GetPath("XilinxEDKDirectory");
                            fp.LinuxKernelLocation = PathMan.GetPath("LinuxKernelLocation");
                            fp.ELDKLocation = PathMan.GetPath("ELDKLocation");
                            fp.MBGNULocation = PathMan.GetPath("MicroblazeGNUTools");
                            fp.DeviceTree = PathMan.GetPath("DeviceTreeLocation");
                            fp.OutputBIT = String.Format("{0}_{1}.bit", fp.SystemPrefix, fp.PlatformID);

                            fp.MessageEvent += new MessageEventDelegate(RaiseMessageEvent);
                            fp.OnRequirePassword = new PasswordRequestDelegate(RequestPassword);
                            _Platforms.Add(fp);
                            return fp;
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Parses the output of the Component Mapping Algorithm to determine how components/cores were mapped to FPGAs for various component/core-specific purposes.
        /// </summary>
        /// <remarks>The project platform must be loaded prior to calling this method.</remarks>
        /// <returns>Returns true if the parsing was successful, False otherwise.</returns>
        public bool LoadXPSMapFile()
        {
            string XPSMap = String.Format("{0}\\xpsmap.xml", PathMan["LocalProjectRoot"]);
            bool rVal = CoreLibrary.LoadXPSMap(XPSMap, ref _Components, ref _CompCores, PathMan, null);
            if (rVal)
            {
                foreach (FalconPlatform FP in _Platforms)
                {
                    foreach (CerebrumCore CC in _Components)
                    {
                        if (String.Compare(FP.PlatformID, CC.MappedFPGA) == 0)
                        {
                            FP.AssignedCCores.Add(CC);
                        }
                    }
                    foreach (ComponentCore CompCore in _CompCores)
                    {
                        if (String.Compare(FP.PlatformID, CompCore.MappedFPGA) == 0)
                        {
                            FP.AssignedPCores.Add(CompCore);
                        }
                    }
                }
            }
            return rVal;
        }

        /// <summary>
        /// This method causes the synthesis tool to request the PurgeBeforeSynthesis fields of the components be populated by an outside source by providing the loaded list
        /// to that source via the provided delegate method.
        /// </summary>
        /// <returns>True if synthesis should continue, False if it is to be aborted.</returns>
        public bool RequestSelectivePurge()
        {
            bool Continue = true;
            if (this.OnRequestSelectivePurge != null)
            {
                List<string> FPGAs = new List<string>();
                foreach (FalconPlatform FP in _Platforms)
                {
                    if (!FP.SkipSynthesis)
                    {
                        FPGAs.Add(FP.PlatformID);
                    }
                }
                Continue = this.OnRequestSelectivePurge(FPGAs, ref _Components, ref _CompCores);

                //Debug.WriteLine("Selective Clean Results");
                //foreach (CerebrumCore CC in _Components)
                //{
                //    string PurgeType = string.Empty;
                //    if (CC.PurgeBeforeSynthesis == CheckState.Checked)
                //        PurgeType = "ALL";
                //    else if (CC.PurgeBeforeSynthesis == CheckState.Indeterminate)
                //        PurgeType = "SOME";
                //    else
                //        PurgeType = "NONE";
                //    Debug.WriteLine(String.Format("\tCerebrumCore: {0} ---> {1}", CC.CoreInstance, PurgeType));
                //    foreach (ComponentCore CompCore in CC.ComponentCores.Values)
                //    {
                //        Debug.WriteLine(String.Format("\t\tCore: {0} ---> {1}", CompCore.NativeInstance, (CompCore.PurgeBeforeSynthesis ? "YES" : "NO")));
                //    }
                //}
                //Debug.WriteLine("\tOther Cores");
                //foreach (ComponentCore CompCore in _CompCores)
                //{
                //    if (CompCore.OwnerComponent == null)
                //    {
                //        Debug.WriteLine(String.Format("\t\tCore: {0} ---> {1}", CompCore.NativeInstance, (CompCore.PurgeBeforeSynthesis ? "YES" : "NO")));
                //    }
                //}
            }
            return Continue;
        }

        /// <summary>
        /// This method causes the synthesis tool to request whether each FPGA in the platform should be synthesized.
        /// </summary>
        /// <returns>True if synthesis should continue, False if it is to be aborted.</returns>
        public bool RequestSelectiveSynthesis()
        {
            bool Continue = true;
            if (this.OnRequestSelectiveSynthesis != null)
            {
                Dictionary<string, bool> FPGAs = new Dictionary<string, bool>();
                foreach (FalconPlatform FP in _Platforms)
                    FPGAs.Add(FP.PlatformID, true);

                Continue = this.OnRequestSelectiveSynthesis(ref FPGAs);
                if (Continue)
                {
                    foreach (FalconPlatform FP in _Platforms)
                    {
                        FP.SkipSynthesis = !FPGAs[FP.PlatformID];
                    }
                }
            }
            return Continue;
        }

        /// <summary>
        /// Loads an XML file specifying the synthesis servers to be used for synthesis.
        /// </summary>
        /// <param name="ServerFile">The path to the XML file specifying the server list.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Servers)</seealso>
        public bool LoadServersFile(string ServerFile)
        {
            try
            {
                _Servers = new LinkedList<FalconServer>();
                if (!File.Exists(ServerFile))
                    ServerFile = PathMan.GetPath("LocalProjectRoot") + "\\" + ServerFile;
                if (!File.Exists(ServerFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(ServerFile);
                RaiseMessage("Loaded Servers from: {0}", ServerFile);

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "servers")
                    {
                        ReadServers(xElem);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                RaiseMessageEvent(ex.Message);
                return false;
            }
            return true;
        }
        private void ReadServers(XmlNode ServersNode)
        {
            foreach (XmlNode xEServer in ServersNode.ChildNodes)
            {
                if (xEServer.Name.ToLower() == "server")
                {
                    FalconServer fs = new FalconServer();
                    fs.ParseServerNode(xEServer);
                    _Servers.AddLast(fs);
                }
            }
        }

        /// <summary>
        /// Loads an XML file specifying the communications interfaces for the platform.  The communications file must be loaded after the Platforms.
        /// </summary>
        /// <param name="CommsFile">The path to the XML file specifying the communications interfaces.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files, Communications)</seealso>
        public bool LoadCommsFile(string CommsFile)
        {
            try
            {
                if (!File.Exists(CommsFile))
                    CommsFile = PathMan.GetPath("LocalProjectRoot") + "\\" + CommsFile;
                if (!File.Exists(CommsFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(CommsFile);

                RaiseMessage("Loaded Communications from: {0}", CommsFile);
                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (String.Compare(xElem.Name, "interfaces", true) == 0)
                    {
                        foreach (XmlNode xCommNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xCommNode.Name, "interface", true) == 0)
                            {
                                string HardwareInstance = string.Empty;
                                string PlatformID = string.Empty;
                                string EthernetMAC = string.Empty;
                                string IPAddress = string.Empty;
                                bool UseDHCP = true;
                                foreach (XmlAttribute xAttr in xCommNode.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "Instance", true) == 0)
                                    {
                                        HardwareInstance = xAttr.Value;
                                    }
                                }
                                foreach (XmlNode xPlatIntProp in xCommNode.ChildNodes)
                                {
                                    string xPropValue = xPlatIntProp.InnerText;
                                    if (String.Compare(xPlatIntProp.Name, "FPGA", true) == 0)
                                    {
                                        PlatformID = xPlatIntProp.InnerText;
                                    }
                                    else if (String.Compare(xPlatIntProp.Name, "ethernetmac", true) == 0)
                                    {
                                        EthernetMAC = xPlatIntProp.InnerText;
                                    }
                                    else if (String.Compare(xPlatIntProp.Name, "ipaddress", true) == 0)
                                    {
                                        IPAddress = xPlatIntProp.InnerText;
                                    }
                                    else if (String.Compare(xPlatIntProp.Name, "dhcp", true) == 0)
                                    {
                                        bool val;
                                        if (bool.TryParse(xPlatIntProp.InnerText, out val))
                                            UseDHCP = val;
                                        else
                                            UseDHCP = false;
                                    }
                                }
                                foreach(FalconPlatform fp in _Platforms)
                                {
                                    if (String.Compare(fp.PlatformID, PlatformID) == 0)
                                    {
                                        fp.EthernetDevice = HardwareInstance;
                                        fp.EthernetMAC = EthernetMAC;
                                        fp.IPAddress = IPAddress;
                                        fp.IPFromDHCP = UseDHCP;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseMessageEvent(ex.Message);
                return false;
            }
            return true;
        }
       

        //private FalconProcessorOS ParseProcessor(XmlNode ProcNode)
        //{
        //    FalconProcessorOS fpOS = new FalconProcessorOS();
        //    foreach (XmlNode xEProcProperty in ProcNode.ChildNodes)
        //    {
        //        string xEProcPropName = xEProcProperty.Name.ToLower();
        //        if (xEProcPropName == "instance")
        //        {
        //            fpOS.Instance = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "os")
        //        {
        //            string val = xEProcProperty.InnerText;
        //            try
        //            {
        //                SystemProcessorOS os = (SystemProcessorOS)Enum.Parse(typeof(SystemProcessorOS), val);
        //                fpOS.OS = os;
        //            }
        //            catch (Exception ex)
        //            {
        //                RaiseMessageEvent(ex.Message);
        //                return null;
        //                throw ex;
        //            }
        //        }
        //        else if (xEProcPropName == "type")
        //        {
        //            string val = xEProcProperty.InnerText;
        //            try
        //            {
        //                SystemProcessorType typ = (SystemProcessorType)Enum.Parse(typeof(SystemProcessorType), val);
        //                fpOS.Type = typ;
        //            }
        //            catch (Exception ex)
        //            {
        //                RaiseMessageEvent(ex.Message);
        //                return null;
        //                throw ex;
        //            }
        //        }
        //        else if (xEProcPropName == "consoledevice")
        //        {
        //            fpOS.ConsoleDevice = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "elffile")
        //        {
        //            fpOS.OutputELF = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "applications")
        //        {
        //            foreach (XmlNode xEApp in xEProcProperty.ChildNodes)
        //            {
        //                if (xEApp.Name.ToLower() == "application")
        //                {
        //                    FalconStandaloneSoftwareApp fsApp = ParseApplication(xEApp);
        //                    fpOS.StandaloneApps.Add(fsApp);
        //                }
        //            }
        //        }
        //        else if (xEProcPropName == "dts")
        //        {
        //            fpOS.DTSFile = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "makeconfig")
        //        {
        //            fpOS.MakeConfig = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "compilerlocation")
        //        {
        //            fpOS.CompilerLocation = xEProcProperty.InnerText;
        //        }
        //        else if (xEProcPropName == "compilerargs")
        //        {
        //            fpOS.CompilerArgs = xEProcProperty.InnerText;
        //        }
        //    }
        //    return fpOS;
        //}
        //private FalconStandaloneSoftwareApp ParseApplication(XmlNode AppNode)
        //{
        //    FalconStandaloneSoftwareApp fsApp = new FalconStandaloneSoftwareApp();
        //    foreach (XmlNode xEAppProperty in AppNode.ChildNodes)
        //    {
        //        string xEAppPropertyName = xEAppProperty.Name.ToLower();
        //        if (xEAppPropertyName == "name")
        //        {
        //            fsApp.Name = xEAppProperty.InnerText;
        //        }
        //        else if (xEAppPropertyName == "compileroptlevel")
        //        {
        //            int val;
        //            if (int.TryParse(xEAppProperty.InnerText, out val))
        //                fsApp.CompilerOptLevel = val;
        //            else
        //                return null;
        //        }
        //        else if (xEAppPropertyName == "globptropt")
        //        {
        //            bool val;
        //            if (bool.TryParse(xEAppProperty.InnerText, out val))
        //                fsApp.GlobPtrOpt = val;
        //            else
        //                return null;
        //        }
        //        else if (xEAppPropertyName == "initbram")
        //        {
        //            bool val;
        //            if (bool.TryParse(xEAppProperty.InnerText, out val))
        //                fsApp.InitBRAM = val;
        //            else
        //                return null;
        //        }
        //        else if (xEAppPropertyName == "linkerscript")
        //        {
        //            fsApp.LinkerScript = xEAppProperty.InnerText;
        //        }
        //        else if (xEAppPropertyName == "libsearchpaths")
        //        {
        //            fsApp.LibSearchPath = ParseStringListNode("LibSearchPath", xEAppProperty);
        //        }
        //        else if (xEAppPropertyName == "includesearchpaths")
        //        {
        //            fsApp.IncludeSearchPath = ParseStringListNode("IncludeSearchPath", xEAppProperty);
        //        }
        //        else if (xEAppPropertyName == "sourcetrees")
        //        {
        //            List<string> list = ParseStringListNode("SourceTree", xEAppProperty);
        //            foreach (string listItem in list)
        //            {
        //                fsApp.AddRuntimeSources(listItem, "*.c");
        //            }
        //        }
        //        else if (xEAppPropertyName == "headertrees")
        //        {
        //            List<string> list = ParseStringListNode("HeaderTree", xEAppProperty);
        //            foreach (string listItem in list)
        //            {
        //                fsApp.AddRuntimeHeaders(listItem, "*.h");
        //            }
        //        }
        //    }
        //    return fsApp;
        //}
        private List<string> ParseStringListNode(string NodeName, XmlNode StringListNode)
        {
            List<string> list = new List<string>();
            foreach (XmlNode xENode in StringListNode.ChildNodes)
            {
                if (xENode.Name.ToLower() == NodeName.ToLower())
                {
                    list.Add(xENode.InnerText);
                }
            }
            return list;
        }

        /// <summary>
        /// Peforms synthesis and compilation of all platforms loaded across all synthesis servers.
        /// </summary>
        /// <returns>A SynthesisErrorCodes enum indicating the progress of synthesis, resetting on each project.</returns>
        public SynthesisErrorCodes SynthesizeDesign()
        {
            bool Continue = true;
            if (Continue && (this.SelectiveSynthesis))
            {
                Continue = this.RequestSelectiveSynthesis();
            }
            if (Continue && (this.SelectiveClean))
            { 
                Continue = this.RequestSelectivePurge();
            }
            if (!Continue)
            {
                RaiseMessageEvent("Synthesis aborted");
                return SynthesisErrorCodes.SYTHNESIS_USER_ABORT;
            }
            SynthesisErrorCodes code = SynthesisErrorCodes.SYNTHESIS_OK;
            List<Thread> SynthesisThreadList = new List<Thread>();

            try
            {
                foreach (FalconPlatform fp in _Platforms)
                {
                    if (!fp.SkipSynthesis)
                    {
                        if (fp.Ready())
                        {
                            continue;
                        }
                        else
                        {
                            return SynthesisErrorCodes.SYTHNESIS_NOT_READY;
                        }
                    }
                }
                string purgeFilter = string.Empty;
                RequestSynthesisServerPasswords();
                foreach (FalconPlatform fp in _Platforms)
                {
                    if (!fp.SkipSynthesis)
                    {
                        fp.PerformFullSynthesis = this.PerformFullSynthesis;
                        SynthesisThreadInfo StartArgs = new SynthesisThreadInfo(fp, purgeFilter);
                        ParameterizedThreadStart pts = new ParameterizedThreadStart(SynthesisThreadMain);
                        Thread SynthesisThread = new Thread(pts);
                        SynthesisThreadList.Add(SynthesisThread);
                        SynthesisThread.Start(StartArgs);
                    }
                    else
                    {
                        fp.ReturnCode = SynthesisErrorCodes.SYNTHESIS_OK;
                    }
                }
                while (SynthesisThreadList.Count > 0)
                {
                    for(int tidx = 0; tidx < SynthesisThreadList.Count; tidx++)
                    {
                        Thread SynthesisThread = SynthesisThreadList[tidx];
                        bool ThreadEnded = false;
                        switch (SynthesisThread.ThreadState)
                        {
                            case System.Threading.ThreadState.Aborted:
                                ThreadEnded = true;
                                break;
                            case System.Threading.ThreadState.AbortRequested:
                                ThreadEnded = true;
                                break;
                            case System.Threading.ThreadState.Stopped:
                                ThreadEnded = true;
                                break;
                            case System.Threading.ThreadState.StopRequested:
                                ThreadEnded = true;
                                break;
                            default:
                                break;
                        }
                        if (ThreadEnded)
                        {
                            SynthesisThreadList.Remove(SynthesisThread);
                            break;
                        }
                        Thread.Sleep(500);
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException TAEx)
            {
                ErrorReporting.ExceptionDetails(TAEx);
            }
            finally
            {
                foreach (FalconPlatform fp in _Platforms)
                {
                    try
                    {
                        if (!fp.SkipSynthesis)
                        {
                            fp.Logout();
                            RaiseMessageEvent(String.Format("[{0}] Synthesis terminated with code: {1}", fp.PlatformID, fp.ReturnCode));
                            if (fp.ReturnCode != SynthesisErrorCodes.SYNTHESIS_OK)
                            {
                                code = fp.ReturnCode;
                            }
                            else
                            {
                                fp.CompileResourceReport();
                            }
                        }
                    }
                    catch { }
                }
            }
            if (code != SynthesisErrorCodes.SYNTHESIS_OK)
            {
                RaiseMessageEvent(String.Format("Design synthesis failed.  Synthesis of one or more FPGAs returned an error."));
            }
            return code;
        }

        private void SynthesisThreadMain(object ThreadArgs)
        {
            SynthesisThreadInfo SynthInfo = (SynthesisThreadInfo)ThreadArgs;
            FalconPlatform currentPlatform = SynthInfo.Platform;
            try
            {
                currentPlatform.ReturnCode = currentPlatform.Login();
                if (currentPlatform.ReturnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                {
                    RaiseMessageEvent("-----------------------------------------------");
                    if (this.ForceClean)
                    {
                        currentPlatform.PerformClean(this.SelectiveClean);
                    }
                    if (currentPlatform.ReturnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                    {
                        if (this.HardwareOnly || !this.SoftwareOnly)        // Either HardwareOnly, both, or neither
                        {
                            currentPlatform.ReturnCode = currentPlatform.BuildHardware(ForceClean);
                        }
                        if (currentPlatform.ReturnCode == SynthesisErrorCodes.SYNTHESIS_OK)
                        {
                            if (this.SoftwareOnly || !this.HardwareOnly)        // Either SoftwareOnly, both, or neither
                            {
                                currentPlatform.ReturnCode = currentPlatform.BuildSoftware(ForceClean);
                            }
                        }
                    }

                    if (currentPlatform.ReturnCode != SynthesisErrorCodes.SYNTHESIS_SKIPPED)
                    {
                        currentPlatform.DownloadOutputs();
                    }
                    currentPlatform.Logout();
                    ProjectEventRecorder Events = new ProjectEventRecorder();
                    Events.Open(PathMan);
                    Events.LogFPGAEvent("LastSynthesisCompleted", currentPlatform.PlatformID);
                    Events.Close();
                }
                else
                {
                    currentPlatform.ReturnCode = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
                }
            }
            catch (ThreadAbortException TAEx)
            {
                ErrorReporting.ExceptionDetails(TAEx);
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                currentPlatform.ReturnCode = SynthesisErrorCodes.SYTHNESIS_UNSPECIFIED_ERROR;
            }
        }

        /// <summary>
        /// Iterates over all platforms to be synthesized and requests passwords be entered for the synthesis server assigned to each, if necessary.
        /// If multiple platforms use the same server, this should prevent requesting the password multiple times for that server.
        /// </summary>
        private void RequestSynthesisServerPasswords()
        {
            foreach (FalconPlatform fp in _Platforms)
            {
                if (fp.SynthesisServer.Password == string.Empty)
                {
                    fp.SynthesisServer.Password = RequestPassword(fp.SynthesisServer.UserName, fp.SynthesisServer.Address);
                }
            }
        }

        /// <summary>
        /// Compiles Resource Reports for each FPGA in the Platform
        /// </summary>
        public void CompileResourceReports()
        {
            foreach (FalconPlatform FP in this._Platforms)
            {
                FP.CompileResourceReport();
            }
        }

        ///// <summary>
        ///// Peforms synthesis of all platforms loaded across synthesis servers.
        ///// </summary>
        ///// <returns>A SynthesisErrorCodes enum indicating the progress of synthesis, resetting on each project.</returns>
        //private SynthesisErrorCodes SynthesizeHardwareOnly(string purgeFilter)
        //{
        //    SynthesisErrorCodes code = SynthesisErrorCodes.SYTHNESIS_NO_STATUS;
        //    FalconPlatform currentPlatform = null;
        //    try
        //    {
        //        foreach (FalconPlatform fp in _Platforms)
        //        {
        //            if (fp.ReadyForSynthesis())
        //            {
        //                continue;
        //            }
        //            else 
        //            {
        //                return SynthesisErrorCodes.SYTHNESIS_NOT_READY;
        //            }
        //        }
        //        foreach (FalconPlatform fp in _Platforms)
        //        {
        //            currentPlatform = fp;
        //            if (fp.Login())
        //            {
        //                RaiseMessageEvent("-----------------------------------------------");
        //                code = fp.PurgeOutputDirectory(purgeFilter);
        //                if (code == SynthesisErrorCodes.SYNTHESIS_OK)
        //                {
        //                    code = fp.BuildHardware(ForceClean);
        //                    if (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED)
        //                    {
        //                        fp.DownloadOutputs();
        //                    }
        //                }

        //                fp.Logout();
        //            }
        //            else
        //            {
        //                code = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
        //            }
        //            if ((code != SynthesisErrorCodes.SYNTHESIS_OK) && (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED))
        //            {
        //                if (currentPlatform != null) currentPlatform.Logout();
        //                return code;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (currentPlatform != null) currentPlatform.Logout();
        //    }
        //    return code;
        //}

        ///// <summary>
        ///// Peforms compilation of all platforms loaded across synthesis servers.
        ///// </summary>
        ///// <returns>A SynthesisErrorCodes enum indicating the progress of synthesis, resetting on each project.</returns>
        //private SynthesisErrorCodes SynthesizeSoftwareOnly(string purgeFilter)
        //{
        //    SynthesisErrorCodes code = SynthesisErrorCodes.SYTHNESIS_NO_STATUS;
        //    FalconPlatform currentPlatform = null;
        //    try
        //    {
        //        foreach (FalconPlatform fp in _Platforms)
        //        {
        //            if (fp.ReadyForCompilation())
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                return SynthesisErrorCodes.SYTHNESIS_NOT_READY;
        //            }
        //        }
        //        foreach (FalconPlatform fp in _Platforms)
        //        {
        //            currentPlatform = fp;
        //            if (fp.Login())
        //            {
        //                RaiseMessageEvent("-----------------------------------------------");
        //                code = fp.PurgeOutputDirectory(purgeFilter);
        //                if (code == SynthesisErrorCodes.SYNTHESIS_OK)
        //                {
        //                    code = fp.BuildSoftware(ForceClean);
        //                    if (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED)
        //                    {
        //                        fp.DownloadOutputs();
        //                    }
        //                }

        //                fp.Logout();
        //            }
        //            else
        //            {
        //                code = SynthesisErrorCodes.SYNTHESIS_REMOTE_AUTH_FAIL;
        //            }
        //            if ((code != SynthesisErrorCodes.SYNTHESIS_OK) && (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED))
        //            {
        //                if (currentPlatform != null) currentPlatform.Logout();
        //            }
        //            return code;
        //        }
        //    }
        //    finally
        //    {
        //        if (currentPlatform != null) currentPlatform.Logout();
        //    }
        //    return code;
        //}

        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon System Synthesizer";
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
