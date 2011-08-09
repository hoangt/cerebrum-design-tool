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
/******************************************************************** 
 * ComponentCore.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: This is a Netron-based object used to view library tabs of core libraries.
 * History: 
 * >> ( 7 Apr 2010) Matthew Cotter: Converted Resources property to a Dictionary rather than a generic List.
 * >> (15 Feb 2010) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Created/exposed several methods and properties to be utilized by the mapping algorithm and XPS builder libraries.
 *                                      Implemented functions to locate, parse and retrieve corresponding MPD files, given an available and connected FileServices object.
 *                                      Added method to create MHS configuration block for the core, based on the values located within the saved property collection.
 * >> (14 Feb 2010) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Integrated improved property management system into component/core structure.
 * >> (24 Oct 2010) Matthew Cotter: Added support for property access and valid conditions.
 * >> (22 Oct 2010) Matthew Cotter: Basic implementation of a pcore object contained as part of a Cerebrum core package.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using FalconPathManager;
using CerebrumSharedClasses;
using CerebrumSharedClasses.MPD_Interfaces;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Defines a subcore object
    /// </summary>
    public class ComponentCore
    {
        /// <summary>
        /// Constructor.  Creates a new Component Core object of with the specified instance and type
        /// </summary>
        /// <param name="NativeInstance">The native instance type of the core</param>
        /// <param name="CoreType">The type name of the core</param>
        public ComponentCore(string NativeInstance, string CoreType)
        {
            this.NativeInstance = NativeInstance;
            this.CoreInstance = string.Empty;
            this.CoreType = CoreType;
            this.ResetPorts = new List<string>();
            this.OwnerComponent = null;
            this.MappedFPGA = string.Empty;
            this.CoreUCF = string.Empty;
            this.TargetFamily = string.Empty;
            this.InterfaceInstances = string.Empty;
            this.IsPE = false;
            this.InterfaceType = VortexInterfaces.VortexCommon.VortexAttachmentType.None;
            _Properties = new CerebrumPropertyCollection(this.CoreInstance, this.NativeInstance, this.CoreType);
        }

        /// <summary>
        /// Copy constructor.  Creates a new Component Core object as a copy of the specified subcore
        /// </summary>
        /// <param name="OwnerInstance">The instance of the new component core's parent CerebrumCore</param>
        /// <param name="CloneSource">The ComponentCore from which to copy properties and values</param>
        public ComponentCore(string OwnerInstance, ComponentCore CloneSource)
        {
            this.NativeInstance = CloneSource.NativeInstance;
            this.CoreType = CloneSource.CoreType;
            this.CoreInstance = String.Format("{0}_{1}", OwnerInstance, this.NativeInstance);
            this.ResetPorts = new List<string>();
            this.OwnerComponent = null;
            this.MappedFPGA = CloneSource.MappedFPGA;
            this.CoreUCF = CloneSource.CoreUCF;
            this.TargetFamily = CloneSource.TargetFamily;
            this.InterfaceInstances = CloneSource.InterfaceInstances;
            this.IsPE = CloneSource.IsPE;
            this.InterfaceType = CloneSource.InterfaceType;

            _Properties = new CerebrumPropertyCollection(this.CoreInstance, this.NativeInstance, this.CoreType);
            _Properties.CloneFrom(CloneSource.Properties);
        }

        /// <summary>
        /// Get or set the CerebrumCore of which this Component Core is a part.
        /// </summary>
        public CerebrumCore OwnerComponent { get; set; }

        /// <summary>
        /// Get or set the baseline instance name of the component core
        /// </summary>
        public string NativeInstance { get; set; }
        /// <summary>
        /// Get or set the actual instance name of the component core
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Get or set the type name of the component core
        /// </summary>
        public string CoreType { get; set; }
        /// <summary>
        /// Get or set the condition indicating whether this core is valid
        /// </summary>
        public string ValidCondition { get; set; }
        /// <summary>
        /// Get or set the version number of the component core
        /// </summary>
        public string CoreVersion { get; set; }
        /// <summary>
        /// Get or set the relative source location of the component core
        /// </summary>
        public string CoreSource { get; set; }
        /// <summary>
        /// Get or set the relative UCF location of the component core
        /// </summary>
        public string CoreUCF { get; set; }

        private CerebrumPropertyCollection _Properties;
        /// <summary>
        /// Get or set the list of core properties associated with this subcore
        /// </summary>
        public CerebrumPropertyCollection Properties
        {
            get
            {
                return _Properties;
            }
        }
        ///// <summary>
        ///// Set the specified propetty to the specified value
        ///// </summary>
        ///// <param name="PropertyName">The name of the property to set</param>
        ///// <param name="PropertyValue">The new value of the property</param>
        //public void SetPropertyValue(string PropertyName, object PropertyValue)
        //{
        //    this._Properties.SetValue(CerebrumPropertyTypes.PARAMETER, PropertyName, PropertyValue, true);
        //}
        ///// <summary>
        ///// Get the value of the specified property
        ///// </summary>
        ///// <param name="PropertyName">The name of the property to get</param>
        ///// <returns>The value of the property, if it exists</returns>
        //public object GetPropertyValue(string PropertyName)
        //{
        //    return (string)this._Properties.GetValue(CerebrumPropertyTypes.PARAMETER, PropertyName);
        //}

        /// <summary>
        /// Save core configuration parameters and properties associated with this core.
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void SaveCoreConfig(string ProjectPath)
        {
            _Properties.PropertyCollectionInstance = this.CoreInstance;
            _Properties.PropertyCollectionType = this.CoreType;
            _Properties.SavePropertyCollection(ProjectPath);
        }
        /// <summary>
        /// Load core configuration parameters and properties associated with this core.
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void LoadCoreConfig(string ProjectPath)
        {
            _Properties.PropertyCollectionInstance = this.CoreInstance;
            _Properties.PropertyCollectionType = this.CoreType;
            _Properties.LoadPropertyCollection(ProjectPath);
        }

        /// <summary>
        /// Translates parameters within the specified condition string, in the context of the core's parameters and its defaults
        /// </summary>
        /// <param name="Input">Input condition string</param>
        /// <returns>Translated string to be evaluated.</returns>
        public string TranslateString(string Input)
        {
            string Output = Input;
            if (Output == null)
                return String.Empty;
            foreach (CerebrumPropertyEntry CPE in this.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER))
            {
                if (Output.Contains(CPE.PropertyName))
                    Output = Output.Replace(CPE.PropertyName, CPE.PropertyValue);
            }
            return Output;
        }

        #region Properties & Methods Used by Mapping Algorithm

        ///// <summary>
        ///// Get a list of connections (Port and Bus) to be connected for this ComponentCore
        ///// </summary>
        //public List<CoreConnectionEntry> Connections { get; set; }

        /// <summary>
        /// Get a list of ports on the core that must be tied to the component reset signal
        /// </summary>
        public List<string> ResetPorts { get; set; }

        /// <summary>
        /// Get or set the Vortex interface exposed by this ComponentCore
        /// </summary>
        public VortexInterfaces.VortexCommon.VortexAttachmentType InterfaceType { get; set; }
        #endregion

        #region Properties & Methods Used by XPS Builder

        /// <summary>
        /// Get or set the FPGA to which this ComponentCore is mapped.
        /// </summary>
        public string MappedFPGA { get; set; }
        /// <summary>
        /// Get or set the architecture family of the FPGA to which this ComponentCore is mapped.
        /// </summary>
        public string TargetFamily { get; set; }
        /// <summary>
        /// Get or set the semicolon-delimited list of Hardware Interface Instances to be used by this ComponentCore
        /// </summary>
        public string InterfaceInstances { get; set; }
        
        private Dictionary<string, PCoreIOInterface> _IO_Interfaces;
        /// <summary>
        /// Get the set of MPD interfaces exposed by this core.
        /// </summary>
        public Dictionary<string, PCoreIOInterface> Interfaces
        {
            get
            {
                if (_IO_Interfaces == null)
                    return new Dictionary<string, PCoreIOInterface>();
                else
                    return _IO_Interfaces;
            }
        }

        /// <summary>
        /// Get or set the list of Default Properties and values for this core
        /// </summary>
        public CerebrumPropertyCollection DefaultProperties
        {
            get
            {
                if (_DefaultProperties == null)
                    _DefaultProperties = new CerebrumPropertyCollection(this.CoreInstance, this.CoreInstance, this.CoreType);
                return _DefaultProperties;
            }
            set
            {
                _DefaultProperties = value;
            }
        }
        private CerebrumPropertyCollection _DefaultProperties;

        /// <summary>
        /// List of "chipscope_signal*" ports exposed by the MPD interface.
        /// </summary>
        public List<string> ChipscopePorts 
        { 
            get 
            {
                if (_ChipscopePorts == null)
                    _ChipscopePorts = new List<string>();
                return _ChipscopePorts; 
            } 
        }
        private List<string> _ChipscopePorts;

        /// <summary>
        /// Parse the specified MPD file for information about the PCores IO Interfaces
        /// </summary>
        /// <param name="MPDFilePath">The path to the MPD file to be parsed.</param>
        public void ParseMPD(string MPDFilePath)
        {
            _ChipscopePorts = new List<string>();
            _ChipscopePorts.Clear();

            _IO_Interfaces = new Dictionary<string, PCoreIOInterface>();
            PCoreIOInterface ioif = new PCoreIOInterface();
            ioif.IO_TYPE = "NONE";
            ioif.IO_IF = "NONE";
            _IO_Interfaces.Add("NONE", ioif);

            StreamReader mpdReader = new StreamReader(MPDFilePath);
            try
            {
                while (!mpdReader.EndOfStream)
                {
                    string line = mpdReader.ReadLine().Trim();
                    if (line.ToUpper().StartsWith("IO_INTERFACE "))
                    {
                        string ifName = string.Empty;
                        string ifType = string.Empty;

                        int ifStart = line.IndexOf("IO_IF");
                        int itStart = line.IndexOf("IO_TYPE");
                        if ((ifStart < 0) || (itStart < 0))
                            continue;

                        int ifEq = line.IndexOf('=', ifStart);
                        int itEq = line.IndexOf('=', itStart);
                        if ((ifStart < 0) || (itStart < 0))
                            continue;

                        int ifEnd = line.IndexOf(',', ifEq);
                        int itEnd = line.IndexOf(',', itEq);

                        if (ifEnd < 0)
                        {
                            ifName = line.Substring(ifEq + 1).Trim();
                        }
                        else
                        {
                            ifName = line.Substring(ifEq + 1, (itStart - ifEq - 1)).Trim().Trim(',').Trim();
                        }
                        if (itEnd < 0)
                        {
                            ifType = line.Substring(itEq + 1).Trim();
                        }
                        else
                        {
                            ifType = line.Substring(itEq + 1, (ifStart - itEq - 1)).Trim().Trim(',').Trim();
                        }
                        PCoreIOInterface newIF = new PCoreIOInterface();
                        newIF.IO_IF = ifName;
                        newIF.IO_TYPE = ifType;
                        _IO_Interfaces.Add(ifName, newIF);
                    }
                    else if (line.ToUpper().StartsWith("PORT "))
                    {
                        PCorePort port = new PCorePort();
                        port.ParseMPDPort(line);
                        foreach (string IO_IF in port.IO_IF)
                        {
                            if (_IO_Interfaces.ContainsKey(IO_IF))
                            {
                                _IO_Interfaces[IO_IF].Ports.Add(port);
                            }
                        }
                        if (port.PortName.StartsWith("chipscope_signal"))
                        {
                            _ChipscopePorts.Add(port.PortName);
                        }
                    }
                    else if (line.ToUpper().StartsWith("PARAMETER "))
                    {
                        PCoreInterfaceParameter param = new PCoreInterfaceParameter();
                        param.ParseMPDParameter(line);
                        foreach (string IO_IF in param.IO_IF)
                        {
                            if (_IO_Interfaces.ContainsKey(IO_IF))
                            {
                                _IO_Interfaces[IO_IF].Parameters.Add(param);
                            }
                        }
                    }
                }
            }
            finally
            {
                mpdReader.Close();
            }
        }

        /// <summary>
        /// Locates and retrieves the MPD file associated with the ComponentCore.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <param name="FS">A FileServices object already connected and authenticated to the remote server.</param>
        /// <returns>A FileInfo object point to the local copy of the MPD file, if it was able to be located.  Null otherwise.</returns>
        public FileInfo RetrieveMPD(PathManager PathMan, FileServices FS)
        {
            string LocalDir = FS.localDir;
            string tempPath = PathMan.GetPath("ProjectTemp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            string CoreFolder = this.CoreType + "_v" + this.CoreVersion.Replace(".", "_");
            FileInfo coreFile = null;

            string localCoreMPD = string.Empty;
            localCoreMPD = LocateCoreMPD(PathMan);
            if (File.Exists(localCoreMPD))
            {
                // Use local file
                coreFile = new FileInfo(localCoreMPD);
            }
            else
            {
                // Look on remote synthesis server
                string remoteFile = LocateRemoteCoreMPD(PathMan, FS);
                if ((remoteFile == null) || (remoteFile == string.Empty))
                    return null;
                coreFile = GetRemoteFile(FS, remoteFile, tempPath, String.Format("{0}.mpd", this.CoreInstance));
            }
            FS.Set_Local_Dir(LocalDir);
            return coreFile;
        }

        /// <summary>
        /// Locate the core's directory using the project's path manager.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <returns>The string path to the core directory, if it was found.  Returns an empty string otherwise.</returns>
        public string LocateCoreDirectory(PathManager PathMan)
        {
            // Add the local Cerebrum Core Repository to the end of the search path
            string SearchPathParameter = PathMan["CoreSearchPaths"];
            string[] CoreSearchPaths = SearchPathParameter.Split(';');
            foreach (string SearchPath in CoreSearchPaths)
            {
                // Generate the folder path
                string CorePath = String.Format("{0}\\Global_IP_Lib\\pcores\\{1}_v{2}", SearchPath.Trim(), this.CoreType, this.CoreVersion.Replace('.', '_'));

                // For now, just check if the folder exists
                if (Directory.Exists(CorePath))
                    return CorePath;
            }
            string XCorePath = String.Format("{0}/{3}/{1}_v{2}", PathMan["LocalXilinxEDKDirectory"], this.CoreType, this.CoreVersion.Replace('.', '_'), @"\hw\XilinxProcessorIPLib\pcores\");
            if (Directory.Exists(XCorePath))
                return XCorePath;
            return string.Empty;
        }
        /// <summary>
        /// Locate a core's directory on the remote synthesis server using the project's path manager.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <param name="FS">A FileServices object already connected and authenticated to the remote server.</param>
        /// <returns>The string path to the core directory, if it was found.  Returns an empty string otherwise.</returns>
        public string LocateRemoteCoreDirectory(PathManager PathMan, FileServices FS)
        {
            // Add the remote synthesis search cores path to the end of the search path
            string SearchPathParameter = PathMan["GlobalSynthPCores"] + ";" + PathMan["XilinxEDKDirectory"] + @"\hw\XilinxProcessorIPLib\pcores\";
            string[] CoreSearchPaths = SearchPathParameter.Split(';');
            string Response = string.Empty;

            foreach (string SearchPath in CoreSearchPaths)
            {
                // Generate the folder path
                string CorePath = String.Format("{0}/{1}_v{2}", SearchPath.Trim(), this.CoreType, this.CoreVersion.Replace('.', '_'));
                // For now, check if the folder exists
                Response = FS.RunCommand(String.Format("ls {0}", CorePath));
                if (Response != string.Empty)
                {
                    if (Response.Contains("data"))
                    {
                        Response = FS.RunCommand(String.Format("ls {0}/data/*.mpd", CorePath));
                        if (Response != string.Empty)
                        {
                            if (Response.Contains(".mpd"))
                            {
                                return CorePath;
                            }
                        }
                    }
                }
            }
            string XCorePath = String.Format("{0}/{3}/{1}_v{2}", PathMan["XilinxEDKDirectory"], this.CoreType, this.CoreVersion.Replace('.', '_'), @"\hw\XilinxProcessorIPLib\pcores\");
            Response = FS.RunCommand(String.Format("ls {0}", XCorePath));
            if (Response != string.Empty)
            {
                if (Response.Contains("data"))
                {
                    Response = FS.RunCommand(String.Format("ls {0}/data/*.mpd", XCorePath));
                    if (Response != string.Empty)
                    {
                        if (Response.Contains(".mpd"))
                        {
                            return XCorePath;
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Locates the full path to an MPD file, given the core's "root" pcore directory.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <returns>The full path to the MPD file, if it exists within the available core search paths.</returns>
        public string LocateCoreMPD(PathManager PathMan)
        {
            string CoreDir = LocateCoreDirectory(PathMan);
            if (!Directory.Exists(CoreDir + "\\data"))
                return string.Empty;
            string[] Files = System.IO.Directory.GetFiles(CoreDir + "\\data", "*.MPD");

            // remove old MPD files from the list
            for (int index = 0; index < Files.Length; index++)
            {
                if (Files[index].Contains("_old"))
                    Files[index] = "";
            }

            string MPDFileName = "";
            foreach (string file in Files)
                if (file != "")
                    MPDFileName = file;
            return MPDFileName;
        }
        /// <summary>
        /// Locates the full path to an MPD file, given the core's "root" pcore directory, on the remote synthesis server.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <param name="FS">A FileServices object already connected and authenticated to the remote server.</param>
        /// <returns>The full path to the MPD file, if it exists within the available core search paths.</returns>
        public string LocateRemoteCoreMPD(PathManager PathMan, FileServices FS)
        {
            // Add the remote synthesis search cores path to the end of the search path
            string SearchPathParameter = PathMan["GlobalSynthPCores"] + ";" + PathMan["XilinxEDKDirectory"];
            string[] CoreSearchPaths = SearchPathParameter.Split(';');
            string Response = string.Empty;

            foreach (string SearchPath in CoreSearchPaths)
            {
                // Generate the folder path
                string CorePath = String.Format("{0}/Global_IP_Lib/pcores/{1}_v{2}", SearchPath.Trim(), this.CoreType, this.CoreVersion.Replace('.', '_'));

                // For now, check if the folder exists
                Response = FS.RunCommand(String.Format("ls {0}", CorePath));
                if (Response != string.Empty)
                {
                    if (Response.Contains("data"))
                    {
                        Response = FS.RunCommand(String.Format("ls {0}/data/*.mpd", CorePath));
                        if (Response != string.Empty)
                        {
                            if (Response.Contains(".mpd"))
                            {
                                string MPDFile = Response.Trim();
                                return MPDFile;
                            }
                        }
                    }
                }
            }
            string XCorePath = String.Format("{0}/{3}/{1}_v{2}", PathMan["XilinxEDKDirectory"], this.CoreType, this.CoreVersion.Replace('.', '_'), @"\hw\XilinxProcessorIPLib\pcores\");
            Response = FS.RunCommand(String.Format("ls {0}", XCorePath));
            if (Response != string.Empty)
            {
                if (Response.Contains("data"))
                {
                    Response = FS.RunCommand(String.Format("ls {0}/data/*.mpd", XCorePath));
                    if (Response != string.Empty)
                    {
                        if (Response.Contains(".mpd"))
                        {
                            string MPDFile = Response.Trim();
                            return MPDFile;
                        }
                    }
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Locates the full path to an MDD file, given the core's "root" driver directory.
        /// </summary>
        /// <param name="PathMan">The project Path Manager loaded with project paths.</param>
        /// <returns>The full path to the MDD file, if it exists within the available core search paths.</returns>
        public string LocateCoreMDD(PathManager PathMan)
        {
            string CoreDir = LocateCoreDirectory(PathMan);
            string[] Files = System.IO.Directory.GetFiles(CoreDir + "\\data", "*.MDD");

            // remove old MPD files from the list
            for (int index = 0; index < Files.Length; index++)
            {
                if (Files[index].Contains("_old"))
                    Files[index] = "";
            }

            string MDDFileName = "";
            foreach (string file in Files)
                if (file != "")
                    MDDFileName = file;
            return MDDFileName;
        }
        /// <summary>
        /// Retrieves a file from the remote server and stores it in the local directory specified.
        /// </summary>
        /// <param name="FS">A FileServices object already connected and authenticated to the remote server.</param>
        /// <param name="remoteFilePath">The path to the remote file.</param>
        /// <param name="LocalCopyDirectory">The local directory in which the file should be stored.</param>
        /// <param name="localFileName">The filename to be given to the local copy.</param>
        /// <returns>A FileInfo object point to the local copy of the file, if it was able to be located.  Null otherwise.</returns>
        public FileInfo GetRemoteFile(FileServices FS, string remoteFilePath, string LocalCopyDirectory, string localFileName)
        {
            FileInfo newFI = null;

            if (FS.isLocalSynthServer)
                return null;
            else
            {
                if ((remoteFilePath == null) || (remoteFilePath == string.Empty))
                    return null;

                string Command = "ls " + remoteFilePath;
                string Response = FS.RunCommand(Command);
                if (Response == string.Empty)
                    return null;
                string[] files = Response.Split('\n');
                string remoteFile = string.Empty;
                if (files.Length >= 1)
                    remoteFile = files[0].Replace("\r", string.Empty);
                if (remoteFile != string.Empty)
                {
                    string remoteDir = string.Empty;
                    string fileName = string.Empty;
                    if (FS.remoteOSType == FileServices.OSVersion.LINUX)
                    {
                        remoteDir = remoteFile.Substring(0, remoteFilePath.LastIndexOf("/") + 1);
                        fileName = remoteFile.Substring(remoteFilePath.LastIndexOf("/") + 1);
                    }
                    else
                    {
                        remoteDir = remoteFile.Substring(remoteFilePath.LastIndexOf("\\"));
                        fileName = remoteFile.Substring(remoteFilePath.LastIndexOf("\\") + 1);
                    }
                    newFI = new FileInfo(LocalCopyDirectory + "\\" + fileName);
                    FS.Set_Local_Dir(LocalCopyDirectory);
                    FS.Set_Remote_Dir(remoteDir);
                    FS.Copy_File_From(fileName);
                    if (newFI.Exists)
                    {
                        FileInfo localFI = new FileInfo(LocalCopyDirectory + "\\" + localFileName);
                        if (localFI.Exists)
                            localFI.Delete();
                        newFI.MoveTo(localFI.FullName);
                        return localFI;
                    }
                    else
                        return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Writes the MHS format block defining this core and its parameters to the specified StreamWriter object.
        /// </summary>
        /// <param name="writer">The open StreamWriter object to which the MHS block is to be written.</param>
        public void WriteMHSBlock(StreamWriter writer)
        {
            writer.Write(String.Format("BEGIN {0}\n", this.CoreType));
            writer.Write(String.Format("  PARAMETER INSTANCE = {0}\n", this.CoreInstance));
            writer.Write(String.Format("  PARAMETER HW_VER = {0}\n", this.CoreVersion));
            foreach (CerebrumPropertyEntry CPEntry in this.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER))
            {
                writer.Write(String.Format("  {0} {1} = {2}\n", CPEntry.PropertyType.ToString(), CPEntry.PropertyName, CPEntry.PropertyValue));
            }
            foreach (CerebrumPropertyEntry CPEntry in this.Properties.GetEntries(CerebrumPropertyTypes.BUS_INTERFACE))
            {
                writer.Write(String.Format("  {0} {1} = {2}\n", CPEntry.PropertyType.ToString(), CPEntry.PropertyName, CPEntry.PropertyValue));
            }
            foreach (CerebrumPropertyEntry CPEntry in this.Properties.GetEntries(CerebrumPropertyTypes.PORT))
            {
                writer.Write(String.Format("  {0} {1} = {2}\n", CPEntry.PropertyType.ToString(), CPEntry.PropertyName, CPEntry.PropertyValue));
            }
            writer.Write(String.Format("END\n\n"));
        }
        #endregion

        #region Properties & Methods Used by Platform Synthesizer
        /// <summary>
        /// Get or set a flag indicating whether pre-synthesized information about core should be purged prior to starting synthesis.
        /// </summary>
        public bool PurgeBeforeSynthesis { get; set; }

        /// <summary>
        /// Get or set a flag indicating whether this component core is a Processing Element, requiring codelet compilation.
        /// </summary>
        public bool IsPE { get; set; }

        /// <summary>
        /// Get a flag to determine whether this component core has been assigned a codelet source module.
        /// </summary>
        public bool IsAssignedCode
        {
            get
            {
                return ((this.LocalCodeSource != null) && (this.LocalCodeSource != string.Empty));
            }
        }
        /// <summary>
        /// Local file path to code source file for the core.
        /// </summary>
        public string LocalCodeSource 
        {
            get
            {
                return (string)this.Properties.GetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, "CerebrumCodeSource");
            }
            set
            {
                this.Properties.SetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, "CerebrumCodeSource", value, true);
            }
        }
        /// <summary>
        /// Get the auto-generated ELF file name to be used for this component core.
        /// </summary>
        public string ELFName
        {
            get
            {
                return String.Format("{0}.elf", this.CoreInstance);
            }
        }
        #endregion
    }
}
