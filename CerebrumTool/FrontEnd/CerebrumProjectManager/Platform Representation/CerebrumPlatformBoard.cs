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
 * CerebrumPlatformBoard.cs
 * Name: Matthew Cotter
 * Date: 15 Sep 2010 
 * Description: Representation of Cerebrum Board including constituent FPGAs.
 * History: 
 * >> (22 Dec 2010) Matthew Cotter: Added enumeration of required cores across the platform.
 * >> (22 Oct 2010) Matthew Cotter: Removed Processors from Board Specification, and corrected null-reference exception in accessing FPGAs.
 *                                  Added support for saving programming cable configuration.
 * >> (07 Oct 2010) Matthew Cotter: Added support for access to list of FPGAs and Processors defined in the board.
 * >> (15 Sep 2010) Matthew Cotter: Basic implementation and properties of the board specification.
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
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace CerebrumProjectManager
{
    /// <summary>
    /// Object representation of an Board defined within the Cerebrum Framework
    /// </summary>
    public class CerebrumPlatformBoard
    {
        private List<CerebrumPlatformFPGA> _FPGAs;

        /// <summary>
        /// Default constructor.  Initializes an empty list of FPGAs
        /// </summary>
        public CerebrumPlatformBoard()
        {
            _FPGAs = new List<CerebrumPlatformFPGA>();
        }
        /// <summary>
        /// Get or set the ID of the Board
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Get the name of the Board
        /// </summary>
        public string Name
        {
            get
            {
                return this.ID;
            }
        }
        /// <summary>
        /// Get or set the FileInfo of the file defining this Board
        /// </summary>
        public FileInfo SourceFile { get; set; }
        /// <summary>
        /// Get or set the CerebrumPlatform of which this Board is a part
        /// </summary>
        public CerebrumPlatform Platform { get; set; }

        /// <summary>
        /// Get a List of CerebrumPlatformFPGA objects defined as members of this Board
        /// </summary>
        public List<CerebrumPlatformFPGA> FPGAs
        {
            get
            {
                if (this._FPGAs != null)
                    return _FPGAs;
                else
                    return new List<CerebrumPlatformFPGA>();
            }
        }
        /// <summary>
        /// Get the number of FPGAs defined as members of this Board
        /// </summary>
        public int FPGACount
        {
            get
            {
                return this.FPGAs.Count;
            }
        }

        /// <summary>
        /// Get a List of CerebrumCore objects that will be instantiated across FPGAs on this board
        /// </summary>
        public List<CerebrumCore> RequiredCores
        {
            get
            {
                List<CerebrumCore> _Cores = new List<CerebrumCore>();
                foreach (CerebrumPlatformFPGA FPGA in this.FPGAs)
                    _Cores.AddRange(FPGA.RequiredCores);
                return _Cores;
            }
        }

        /// <summary>
        /// Loads the specified Board definition from the file specified
        /// </summary>
        /// <param name="BoardFilePath">The path to the board file to be loaded</param>
        /// <returns>True if the Board was loaded successfully, false otherwise.</returns>
        public bool LoadBoardFromFile(string BoardFilePath)
        {
            try
            {
                SourceFile = new FileInfo(BoardFilePath);
                if (!SourceFile.Exists)
                    return false;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(SourceFile.FullName);
                _FPGAs = new List<CerebrumPlatformFPGA>();
                _FPGAs.Clear();

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (String.Compare(xElem.Name, "board", true) == 0)
                    {
                        foreach (XmlNode xBoardNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xBoardNode.Name, "fpga", true) == 0)
                            {
                                string fpgaFile = string.Empty;
                                string fpgaID = string.Empty;
                                foreach (XmlAttribute xAttr in xBoardNode.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "file", true) == 0)
                                    {
                                        fpgaFile = xAttr.Value.Replace(".xml", string.Empty);
                                    }
                                    else if (String.Compare(xAttr.Name, "id", true) == 0)
                                    {
                                        fpgaID = xAttr.Value;
                                    }
                                }
                                if ((fpgaID != string.Empty) && (fpgaFile != string.Empty))
                                {
                                    CerebrumPlatformFPGA newFPGA = new CerebrumPlatformFPGA();
                                    newFPGA.ID = fpgaID;
                                    newFPGA.Platform = this.Platform;
                                    newFPGA.Board = this;

                                    if (newFPGA.LoadFPGAFromFile(String.Format("{0}\\{1}\\{2}.xml",
                                        SourceFile.Directory.FullName,
                                        fpgaFile,
                                        fpgaFile)))
                                    {
                                        newFPGA.RequiredCores = CoreLibrary.ReadRequiredComponentsFromXML(this.Platform.ProjectManager.PathManager, xBoardNode, newFPGA.MappingID);
                                        _FPGAs.Add(newFPGA);
                                    }
                                }
                            }
                        }
                    }
                }
                return (
                        (_FPGAs.Count > 0) &&
                        (this.ID != string.Empty) &&
                        (this.Platform != null) &&
                        (this.SourceFile != null)
                       );
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                return false;
            }
        }

        /// <summary>
        /// Disposes any resources allocated by the object
        /// </summary>
        public void Dispose()
        {
            foreach (CerebrumPlatformFPGA FPGA in FPGAs)
                FPGA.Dispose();
            FPGAs.Clear();
        }

        /// <summary>
        /// Saves programming information for all FPGAs attached to this Board
        /// </summary>
        /// <param name="ProgrammingNode">The programming node under which the program nodes should be attached.</param>
        public void SaveProgrammingConfig(XmlNode ProgrammingNode)
        {
            foreach (CerebrumPlatformFPGA fpga in this.FPGAs)
            {
                ProgrammingNode.AppendChild(fpga.ProgramConfig.SaveToXml(ProgrammingNode.OwnerDocument, fpga.MappingID));
            }
        }
    }
}
