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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;
using System.Xml;
using System.IO;

namespace CerebrumSharedClasses.Platform_Details
{
    /// <summary>
    /// Represents an FPGA and its corresponding available/used IO Interfaces and adapters.
    /// </summary>
    public class FPGA
    {
        private Dictionary<string, long> _Resources;
        private Dictionary<string, string> _GlobalAttributes;
        private List<FPGA_External_Port> _ExternalPorts;
        private List<FPGA_IO_Adapter> _Adapters;
        private List<FPGA_IO_Interface> _Interfaces;
        private List<FPGA_IO_Port> _Ports;

        /// <summary>
        /// Default constructor.  Initializes an empty FPGA object.
        /// </summary>
        public FPGA()
        {
            this.Instance = string.Empty;
            this.Family = string.Empty;
            this.Device = string.Empty;
            this.Package = string.Empty;
            this.SpeedGrade = string.Empty;
            this.JTAGPos = string.Empty;
            _ExternalPorts = new List<FPGA_External_Port>();
            _Adapters = new List<FPGA_IO_Adapter>();
            _Interfaces = new List<FPGA_IO_Interface>();
            _Ports = new List<FPGA_IO_Port>();
            _GlobalAttributes = new Dictionary<string, string>();
            _Resources = new Dictionary<string, long>();
        }
        /// <summary>
        /// Constructor populating the FPGA with information about its available I/O Adapters, Interfaces and Ports
        /// </summary>
        /// <param name="FPGA_Spec_Doc">The Cerebrum Platform FPGA specification XML document from which to populate the FPGA.</param>
        public FPGA(string FPGA_Spec_Doc)
        {
            ParseFPGADoc(FPGA_Spec_Doc);
        }
        /// <summary>
        /// The root "node" of the FPGA
        /// </summary>
        public string Root { get; set; }
        /// <summary>
        /// The internal instance name of the FPGA
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// The architecture family of the FPGA
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// The architecture family of the FPGA
        /// </summary>
        public string Architecture 
        {
            get
            {
                return this.Family;
            }
            set
            {
                this.Family = value;
            }
        }
        /// <summary>
        /// The device model of the FPGA
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// The device package of the FPGA
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// The device speed grade of the FPGA
        /// </summary>
        public string SpeedGrade { get; set; }
        /// <summary>
        /// The JTAG cable position/number of the FPGA
        /// </summary>
        public string JTAGPos { get; set; }
        /// <summary>
        /// Path to the file used to populate the FPGA
        /// </summary>
        public string SourceFile { get; set; }
        /// <summary>
        /// Get or set the Dictionary containing the names and amounts of resources available on the FPGA.
        /// </summary>
        public Dictionary<string, long> Resources
        {
            get { return _Resources; }
            set { _Resources = value; }
        }
        /// <summary>
        /// Parses the specified FPGA definition document to extract information required to implement necessary hardware connectivity and constraints.
        /// </summary>
        /// <param name="FPGA_Spec_Doc">The path to the XML document defining the FPGA hardware.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseFPGADoc(string FPGA_Spec_Doc)
        {
            this.Instance = string.Empty;
            this.Family = string.Empty;
            this.Device = string.Empty;
            this.Package = string.Empty;
            this.SpeedGrade = string.Empty;
            this.JTAGPos = string.Empty;
            this._GlobalAttributes = new Dictionary<string, string>();
            _Adapters = new List<FPGA_IO_Adapter>();
            _Interfaces = new List<FPGA_IO_Interface>();
            _Ports = new List<FPGA_IO_Port>();

            if (File.Exists(FPGA_Spec_Doc))
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(FPGA_Spec_Doc);

                    foreach (XmlNode xRoot in xDoc.ChildNodes)
                    {
                        if (String.Compare(xRoot.Name, "xml", true) != 0)
                        {
                            this.Root = xRoot.Name;
                            foreach (XmlNode xChild in xRoot.ChildNodes)
                            {
                                if (String.Compare(xChild.Name, "Global_Attributes", true) == 0)
                                {
                                    foreach (XmlNode xGlobalAttr in xChild.ChildNodes)
                                    {
                                        if (String.Compare(xGlobalAttr.Name, "ATTRIBUTE", true) == 0)
                                        {
                                            if (xGlobalAttr.Attributes.Count > 0)
                                            {
                                                XmlAttribute xAttr = xGlobalAttr.Attributes[0];
                                                this.GlobalAttributes.Add(xAttr.Name, xAttr.Value);
                                            }
                                        }
                                    }
                                }
                                if (String.Compare(xChild.Name, "IO_INTERFACE", true) == 0)
                                {
                                    FPGA_IO_Interface newIF = new FPGA_IO_Interface();
                                    if (newIF.ParseInterfaceNode(xChild))
                                    {
                                        _Interfaces.Add(newIF);
                                    }
                                }
                                else if (String.Compare(xChild.Name, "IO_ADAPTER", true) == 0)
                                {
                                    FPGA_IO_Adapter newAd = new FPGA_IO_Adapter();
                                    if (newAd.ParseAdapterNode(xChild))
                                    {
                                        _Adapters.Add(newAd);
                                    }
                                }
                                else if (String.Compare(xChild.Name, "FPGA") == 0)
                                {
                                    foreach (XmlAttribute xAttr in xChild.Attributes)
                                    {
                                        if (String.Compare(xAttr.Name, "INSTANCE", true) == 0)
                                        {
                                            this.Instance = xAttr.Value.Trim();
                                        }
                                        else if (String.Compare(xAttr.Name, "FAMILY", true) == 0)
                                        {
                                            this.Family = xAttr.Value.Trim();
                                        }
                                        else if (String.Compare(xAttr.Name, "DEVICE", true) == 0)
                                        {
                                            this.Device = xAttr.Value.Trim();
                                        }
                                        else if (String.Compare(xAttr.Name, "PACKAGE", true) == 0)
                                        {
                                            this.Package = xAttr.Value.Trim();
                                        }
                                        else if (String.Compare(xAttr.Name, "SPEED_GRADE", true) == 0)
                                        {
                                            this.SpeedGrade = xAttr.Value.Trim();
                                        }
                                        else if (String.Compare(xAttr.Name, "JTAG_POSITION", true) == 0)
                                        {
                                            this.JTAGPos = xAttr.Value.Trim();
                                        }
                                    }
                                    foreach (XmlNode xFPGAChild in xChild.ChildNodes)
                                    {
                                        if (String.Compare(xFPGAChild.Name, "PORT", true) == 0)
                                        {
                                            FPGA_IO_Port port = new FPGA_IO_Port();
                                            if (port.ParsePortNode(xFPGAChild))
                                            {
                                                _Ports.Add(port);
                                            }
                                        }
                                        else if (String.Compare(xFPGAChild.Name, "RESOURCE", true) == 0)
                                        {
                                            string RName = string.Empty;
                                            string RAmount = string.Empty;
                                            long ResAmt = 0;
                                            foreach (XmlAttribute xAttr in xFPGAChild.Attributes)
                                            {
                                                if (String.Compare(xAttr.Name, "Name", true) == 0)
                                                {
                                                    RName = xAttr.Value.ToLower();
                                                }
                                                else if (String.Compare(xAttr.Name, "Amount", true) == 0)
                                                {
                                                    RAmount = xAttr.Value.ToLower();
                                                }
                                            }
                                            if ((RName != string.Empty) && (RAmount != string.Empty))
                                            {
                                                if (long.TryParse(RAmount, out ResAmt))
                                                {
                                                    if (ResAmt > 0)
                                                    {
                                                        if (this._Resources.ContainsKey(RName))
                                                        {
                                                            _Resources[RName] = ResAmt;
                                                        }
                                                        else
                                                        {
                                                            _Resources.Add(RName, ResAmt);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    ErrorReporting.TraceException(ex);
                    this.Instance = string.Empty;
                    this.Family = string.Empty;
                    this.Device = string.Empty;
                    this.Package = string.Empty;
                    this.SpeedGrade = string.Empty;
                    this.JTAGPos = string.Empty;
                    _Adapters = new List<FPGA_IO_Adapter>();
                    _Interfaces = new List<FPGA_IO_Interface>();
                    _Ports = new List<FPGA_IO_Port>();
                }
            }
            return false;
        }

        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XBD Document "Node".
        /// </summary>
        /// <param name="reader">The StreamReader from which the FPGA block is to be read.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        private bool ParseXBDFPGA(StreamReader reader)
        {
            try
            {
                bool AtEndofBlock = false;
                while (!AtEndofBlock)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    if (line.Contains("#"))
                    {
                        line = line.Substring(0, line.IndexOf("#") - 1).Trim();
                    }
                    if (String.Compare(line, "END", true) == 0)
                    {
                        AtEndofBlock = true;
                        continue;
                    }
                    if (line.ToUpper().StartsWith("ATTRIBUTE"))
                    {
                        line = line.Substring(9).Trim();
                        string attrName = string.Empty;
                        string attrValue = string.Empty;
                        string[] pair = line.Split('=');
                        attrName = pair[0].Trim().ToUpper();
                        attrValue = pair[1].Trim();
                        if (String.Compare(attrName, "INSTANCE", true) == 0)
                        {
                            this.Instance = attrValue;
                        }
                        else if (String.Compare(attrName, "FAMILY", true) == 0)
                        {
                            this.Family = attrValue;
                        }
                        else if (String.Compare(attrName, "DEVICE", true) == 0)
                        {
                            this.Device = attrValue;
                        }
                        else if (String.Compare(attrName, "PACKAGE", true) == 0)
                        {
                            this.Package = attrValue;
                        }
                        else if (String.Compare(attrName, "SPEED_GRADE", true) == 0)
                        {
                            this.SpeedGrade = attrValue;
                        }
                        else if (String.Compare(attrName, "JTAG_POSITION", true) == 0)
                        {
                            this.JTAGPos = attrValue;
                        }
                    }
                    else if (line.ToUpper().StartsWith("PORT"))
                    {
                        FPGA_IO_Port port = new FPGA_IO_Port();
                        if (port.ParseXBDPort(line))
                        {
                            _Ports.Add(port);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        /// <summary>
        /// Parses the specified Xilinx-format XBD file for information required for Cerebrum to mimic the platform.
        /// </summary>
        /// <param name="XBDFile">The path to the XBD document defining the FPGA hardware.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBD(string XBDFile)
        {
            try
            {
                FileInfo InFile = new FileInfo(XBDFile);
                if (InFile.Exists)
                {
                    this.Root = InFile.Name;
                    StreamReader reader = new StreamReader(InFile.FullName);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine().Trim();
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        if (line.Contains("#"))
                        {
                            line = line.Substring(0, line.IndexOf("#") - 1).Trim();
                        }
                        if (line.StartsWith("BEGIN"))
                        {
                            line = line.Substring(5).Trim();
                            if (String.Compare(line, "IO_INTERFACE", true) == 0)
                            {
                                FPGA_IO_Interface newIF = new FPGA_IO_Interface();
                                if (newIF.ParseXBDInterface(reader))
                                {
                                    _Interfaces.Add(newIF);
                                }
                            }
                            else if (String.Compare(line, "IO_ADAPTER", true) == 0)
                            {
                                FPGA_IO_Adapter newAd = new FPGA_IO_Adapter();
                                if (newAd.ParseXBDAdapter(reader))
                                {
                                    _Adapters.Add(newAd);
                                }
                            }
                            else if (String.Compare(line, "FPGA", true) == 0)
                            {
                                this.ParseXBDFPGA(reader);
                            }
                        }
                        else if (line.StartsWith("ATTRIBUTE"))
                        {
                            line = line.Substring(9).Trim();
                            string attrName = string.Empty;
                            string attrValue = string.Empty;
                            string[] pair = line.Split('=');
                            attrName = pair[0].Trim().ToUpper();
                            attrValue = pair[1].Trim();
                            if (this.GlobalAttributes.ContainsKey(attrName))
                                this.GlobalAttributes[attrName] = attrValue;
                            else
                                this.GlobalAttributes.Add(attrName, attrValue);
                        }
                        else
                        {
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        /// <summary>
        /// Writes the current Platform FPGA details to the specified output file.
        /// </summary>
        /// <param name="OutputPath">The full path to the target output file.</param>
        public void WritePlatformFPGA(string OutputPath)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));

                if ((this.Root == null) || (this.Root == string.Empty))
                    this.Root = "FPGARootNode";

                XmlElement xRoot = xDoc.CreateElement(this.Root);

                #region Global Attributes
                XmlElement xGlobal = xDoc.CreateElement("GLOBAL_ATTRIBUTES");
                foreach(KeyValuePair<string, string> GAttr in this.GlobalAttributes)
                {
                    XmlElement xAttribute = xDoc.CreateElement("ATTRIBUTE");
                    xAttribute.SetAttribute(GAttr.Key, GAttr.Value);
                    xGlobal.AppendChild(xAttribute);
                }
                xRoot.AppendChild(xGlobal);
                #endregion

                #region IO Interfaces
                for (int i = 0; i < this.Interfaces.Count; i++)
                {
                    FPGA_IO_Interface ioif = this.Interfaces[i];
                    ioif.WritePlatformInterface(xRoot);
                }
                #endregion

                #region IO Adapters
                for (int i = 0; i < this.Adapters.Count; i++)
                {
                    FPGA_IO_Adapter ioad = this.Adapters[i];
                    ioad.WritePlatformAdapter(xRoot);
                }
                #endregion

                #region FPGA
                this.WriteFPGA(xRoot);
                #endregion

                xDoc.AppendChild(xRoot);
                xDoc.Save(OutputPath);
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Writes this FPGA Block as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this FPGA's block should be attached.</param>
        public void WriteFPGA(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("FPGA");
            xItemRoot.SetAttribute("Instance", this.Instance);
            xItemRoot.SetAttribute("Family", this.Family);
            xItemRoot.SetAttribute("Device", this.Device);
            xItemRoot.SetAttribute("Package", this.Package);
            xItemRoot.SetAttribute("Speed_Grade", this.SpeedGrade);
            xItemRoot.SetAttribute("JTAG_Position", this.JTAGPos);

            #region Resources
            List<string> Keys = _Resources.Keys.ToList<string>();
            for (int i = 0; i < Keys.Count; i++)
            {
                XmlElement xResItem = xDoc.CreateElement("Resource");
                xResItem.SetAttribute("Name", Keys[i]);
                xResItem.SetAttribute("Amount", _Resources[Keys[i]].ToString());
                xItemRoot.AppendChild(xResItem);
            }
            #endregion

            #region Ports
            for (int i = 0; i < Ports.Count; i++)
            {
                FPGA_IO_Port fiop = Ports[i];
                fiop.WritePlatformFPGAIOPort(xItemRoot);
            }
            #endregion

            xRoot.AppendChild(xItemRoot);
        }

        /// <summary>
        /// List of Global attributes attached to the FPGA, indexed by name
        /// </summary>
        public Dictionary<string, string> GlobalAttributes
        {
            get
            {
                return _GlobalAttributes;
            }
            set
            {
                _GlobalAttributes = value;
            }
        }
        /// <summary>
        /// Returns a list of IO Adapters defined in the FPGA
        /// </summary>
        public List<FPGA_IO_Adapter> Adapters
        {
            get
            {
                return _Adapters;
            }
        }
        /// <summary>
        /// Returns a list of IO Interfaces defined in the FPGA
        /// </summary>
        public List<FPGA_IO_Interface> Interfaces
        {
            get
            {
                return _Interfaces;
            }
        }
        /// <summary>
        /// Returns a list of IO Ports defined on the FPGA
        /// </summary>
        public List<FPGA_IO_Port> Ports
        {
            get
            {
                return _Ports;
            }
        }

        /// <summary>
        /// Returns a list of Top-level External to be included on the FPGA
        /// </summary>
        public List<FPGA_External_Port> ExternalPorts
        {
            get
            {
                return _ExternalPorts;
            }
        }
    }
}
