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
 * CerebrumPlatformFPGA.cs
 * Name: Matthew Cotter
 * Date: 15 Sep 2010 
 * Description: Representation of Cerebrum Board including constituent FPGAs.
 * History: 
 * >> (22 Dec 2010) Matthew Cotter: Added enumeration of required cores across the platform.
 * >> (22 Oct 2010) Matthew Cotter: Removed Processors from FPGA Specification, and corrected null-reference exception in accessing FPGAs.
 *                                  Added support for accessing programming cable configuration.
 * >> (07 Oct 2010) Matthew Cotter: Added support for access to list of Processors defined in the FPGA.
 * >> (15 Sep 2010) Matthew Cotter: Basic implementation and properties of the FPGA specification.
 * >> (15 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using FalconPathManager;
using System.Diagnostics;
using CerebrumNetronObjects;
using CerebrumSharedClasses;

namespace CerebrumProjectManager
{
    /// <summary>
    /// Object representation of an FPGA defined within the Cerebrum Framework
    /// </summary>
    public class CerebrumPlatformFPGA
    {
        private string _ID;
        private ProgrammerConfiguration _ProgramConfig;

        /// <summary>
        /// Default constructor.  Initializes an empty FPGA.
        /// </summary>
        public CerebrumPlatformFPGA()
        {
            this.Family = string.Empty;
            this.Device = string.Empty;
            this.Package = string.Empty;
            this.SpeedGrade = string.Empty;
            this.JTAGPosition = string.Empty;

            this.ID = string.Empty;
            this.SourceFile = null;
            this.Platform = null;
            this.Board = null;
            _ProgramConfig = new ProgrammerConfiguration();
        }

        /// <summary>
        /// Get the Mapping ID of the FPGA
        /// </summary>
        public string MappingID
        {
            get
            {
                if (this.Board == null)
                    return this.ID;
                return String.Format("{0}.{1}",
                    this.Board.ID,
                    this.ID);
            }
        }
        /// <summary>
        /// Get or set the ID of the FPGA
        /// </summary>
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        /// <summary>
        /// Get the name of the FPGA
        /// </summary>
        public string Name
        {
            get
            {
                return this.ID;
            }
        }
        /// <summary>
        /// Get or set the FileInfo of the file defining this FPGA
        /// </summary>
        public FileInfo SourceFile { get; set; }
        /// <summary>
        /// Get or set the CerebrumPlatform of which this FPGA is a part
        /// </summary>
        public CerebrumPlatform Platform { get; set; }
        /// <summary>
        /// Get or set the CerebrumPlatformBoard of which this FPGA is a part
        /// </summary>
        public CerebrumPlatformBoard Board { get; set; }

        /// <summary>
        /// Get or set the Architecture Family of this FPGA
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// Get or set the Device/Part of this FPGA
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// Get or set the Device/Part of this FPGA
        /// </summary>
        public string Part 
        {
            get
            {
                return this.Device;
            }
            set
            {
                this.Device = value;
            }
        }
        /// <summary>
        /// Get or set the Package ID of this FPGA
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// Get or set the speed grade of this FPGA
        /// </summary>
        public string SpeedGrade { get; set; }
        /// <summary>
        /// Get or set the JTAG Position of this FPGA
        /// </summary>
        public string JTAGPosition { get; set; }

        /// <summary>
        /// Loads the FPGA specification from the specified file
        /// </summary>
        /// <param name="FPGAFilePath">The path to the FPGA definition</param>
        /// <returns>True if the file was loaded successfully</returns>
        public bool LoadFPGAFromFile(string FPGAFilePath)
        {
            try
            {
                SourceFile = new FileInfo(FPGAFilePath);
                if (!SourceFile.Exists)
                    return false;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(SourceFile.FullName);

                foreach (XmlNode xNode in xDoc.ChildNodes)
                {
                    if (String.Compare(xNode.Name, "xml", true) != 0)
                    {
                        foreach (XmlNode xBoardItem in xNode.ChildNodes)
                        {
                            if (String.Compare(xBoardItem.Name, "fpga", true) == 0)
                            {
                                string fpgaArch = string.Empty;
                                // Get Key information
                                foreach (XmlAttribute xAttr in xBoardItem.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "family", true) == 0)
                                    {
                                        this.Family = xAttr.Value;
                                    }
                                    else if (String.Compare(xAttr.Name, "device", true) == 0)
                                    {
                                        this.Device = xAttr.Value;
                                    }
                                    else if (String.Compare(xAttr.Name, "package", true) == 0)
                                    {
                                        this.Package = xAttr.Value;
                                    }
                                    else if (String.Compare(xAttr.Name, "speed_grade", true) == 0)
                                    {
                                        this.SpeedGrade = xAttr.Value;
                                    }
                                    else if (String.Compare(xAttr.Name, "jtag_position", true) == 0)
                                    {
                                        this.JTAGPosition = xAttr.Value;
                                    }
                                }
                            }
                        }
                    }
                }
                return (
                         (this.ID != string.Empty) &&
                         (this.Family != string.Empty) &&
                         (this.Device != string.Empty) &&
                         (this.Package != string.Empty) &&
                         (this.SpeedGrade != string.Empty) &&
                         (this.JTAGPosition != string.Empty) &&
                         (this.SourceFile != null) &&
                         (this.Board != null) &&
                         (this.Platform != null)
                       );
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Defines the programming configuration (cable/type) associated with this FPGA
        /// </summary>
        public ProgrammerConfiguration ProgramConfig
        {
            get
            {
                if (_ProgramConfig == null)
                    _ProgramConfig = new ProgrammerConfiguration();
                return _ProgramConfig;
            }
        }

        /// <summary>
        /// Disposes any resources allocated by the object
        /// </summary>
        public void Dispose()
        {
        }

        private List<CerebrumCore> _RequiredCores;
        /// <summary>
        /// Defines a list of components that will automatically be instantiated on the FPGA
        /// </summary>
        public List<CerebrumCore> RequiredCores
        {
            get
            {
                if (_RequiredCores == null)
                    _RequiredCores = new List<CerebrumCore>();
                return _RequiredCores;
            }
            set
            {
                _RequiredCores = value;
            }
        }

    }
}
