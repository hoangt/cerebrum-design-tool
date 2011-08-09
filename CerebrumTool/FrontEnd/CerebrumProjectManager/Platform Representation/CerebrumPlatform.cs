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
 * CerebrumPlatform.cs
 * Name: Matthew Cotter
 * Date: 15 Sep 2010 
 * Description: Representation of entire Cerebrum platform, including access to constituent board and FPGA objects.
 * History: 
 * >> (22 Dec 2010) Matthew Cotter: Added enumeration of required cores across the platform.
 * >> (22 Oct 2010) Matthew Cotter: Removed Processors from Platform Specification, and corrected null-reference exception in accessing Boards.
 * >> (07 Oct 2010) Matthew Cotter: Added support for access to list of boards, FPGAs, and Processors defined in the platform.
 * >> (15 Sep 2010) Matthew Cotter: Basic implementation and properties of the platform specification.
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

namespace CerebrumProjectManager
{
    /// <summary>
    /// Object representing an entire Cerebrum Hardware Platform
    /// </summary>
    public class CerebrumPlatform
    {
        private List<CerebrumPlatformBoard> _Boards;

        /// <summary>
        /// Default constructor.  Creates an empty list of boards
        /// </summary>
        public CerebrumPlatform()
        {
            _Boards = new List<CerebrumPlatformBoard>();
        }

        /// <summary>
        /// Get or set the FileInfo of the file defining this Platform
        /// </summary>
        public FileInfo SourceFile { get; set; }

        /// <summary>
        /// Get the name of the Platform
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates whether the platform has been successfully loaded.
        /// </summary>
        public bool Loaded
        {
            get
            {
                return (
                        (this.BoardCount > 0) &&
                        (this.FPGACount > 0) &&
                        (this.Name != string.Empty) &&
                        (this.SourceFile != null)
                       );
            }
        }

        /// <summary>
        /// Get a List of CerebrumPlatformBoard objects defined as members of this platform
        /// </summary>
        public List<CerebrumPlatformBoard> Boards
        {
            get
            {
                if (this._Boards != null)
                    return _Boards;
                else
                    return new List<CerebrumPlatformBoard>();
            }
        }
        /// <summary>
        /// Get the number of boards defined as members of this platform
        /// </summary>
        public int BoardCount
        {
            get
            {
                if (_Boards != null)
                    return _Boards.Count;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Get a List of CerebrumPlatformFPGA objects defined as members of this platform
        /// </summary>
        public List<CerebrumPlatformFPGA> FPGAs
        {
            get
            {
                List<CerebrumPlatformFPGA> _FPGAs = new List<CerebrumPlatformFPGA>();
                foreach (CerebrumPlatformBoard Board in this.Boards)
                    _FPGAs.AddRange(Board.FPGAs);
                return _FPGAs;
            }
        }
        /// <summary>
        /// Get the number of FPGAs defined as members of this platform
        /// </summary>
        public int FPGACount
        {
            get
            {
                return this.FPGAs.Count;
            }
        }

        /// <summary>
        /// Get a List of CerebrumCore objects that will be instantiated across Boards/FPGA in this platform
        /// </summary>
        public List<CerebrumCore> RequiredCores
        {
            get
            {
                List<CerebrumCore> _Cores = new List<CerebrumCore>();
                foreach (CerebrumPlatformBoard Board in this.Boards)
                    _Cores.AddRange(Board.RequiredCores);
                return _Cores;
            }
        }
        /// <summary>
        /// Loads the platform specified in the Project PathManager
        /// </summary>
        /// <param name="PathMan">The Project Path manager associated with the object</param>
        /// <returns>True if the platform was loaded successfully, false otherwise.</returns>
        public bool LoadProjectPlatform(PathManager PathMan)
        {
            return LoadPlatformFromFile(PathMan, PathMan["ProjectPlatform"]);
        }
        /// <summary>
        /// Loads the specified platform as located via the path manager
        /// </summary>
        /// <param name="PathMan">A project path manager indicating the location of Cerebrum Platforms</param>
        /// <param name="PlatformName">The name of the platform to be loaded</param>
        /// <returns>True if the platform was loaded successfully, false otherwise.</returns>
        public bool LoadPlatformFromFile(PathManager PathMan, string PlatformName)
        {
            try
            {
                SourceFile = new FileInfo(String.Format(@"{0}\{1}\{1}.xml", PathMan["Platforms"], PlatformName));
                if (!SourceFile.Exists)
                    return false;
                this.Name = PlatformName;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(SourceFile.FullName);
                _Boards = new List<CerebrumPlatformBoard>();
                _Boards.Clear();

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (String.Compare(xElem.Name, "platform", true) == 0)
                    {
                        foreach (XmlNode xBoardNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xBoardNode.Name, "board", true) == 0)
                            {
                                string boardFile = string.Empty;
                                string boardID = string.Empty;
                                foreach (XmlAttribute xAttr in xBoardNode.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "file", true) == 0)
                                    {
                                        boardFile = xAttr.Value.Replace(".xml", string.Empty); ;
                                    }
                                    else if (String.Compare(xAttr.Name, "id", true) == 0)
                                    {
                                        boardID = xAttr.Value;
                                    }
                                }
                                if ((boardID != string.Empty) && (boardFile != string.Empty))
                                {
                                    CerebrumPlatformBoard newBoard = new CerebrumPlatformBoard();
                                    newBoard.ID = boardID;
                                    newBoard.Platform = this;
                                    if (newBoard.LoadBoardFromFile(String.Format("{0}\\{1}\\{1}.xml",
                                        SourceFile.Directory.FullName,
                                        boardFile)))
                                    {
                                        _Boards.Add(newBoard);
                                    }
                                }
                            }
                        }
                        foreach (XmlNode xLinksNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xLinksNode.Name, "Links", true) == 0)
                            {
                                foreach (XmlNode xLinkNode in xLinksNode.ChildNodes)
                                {
                                    if (String.Compare(xLinkNode.Name, "Link", true) == 0)
                                    {
                                        foreach (XmlNode xCoreNode in xLinkNode.ChildNodes)
                                        {
                                            if (String.Compare(xCoreNode.Name, "RequiredCore", true) == 0)
                                            {
                                                string OnFPGA = string.Empty;
                                                foreach (XmlAttribute xAttr in xCoreNode.Attributes)
                                                {
                                                    if (String.Compare(xAttr.Name, "OnFPGA", true) == 0)
                                                    {
                                                        OnFPGA = xAttr.Value;
                                                        break;
                                                    }
                                                }
                                                if (OnFPGA != string.Empty)
                                                {
                                                    CerebrumCore LinkCore = CoreLibrary.ReadRequiredComponentFromXML(PathMan, OnFPGA, xCoreNode);
                                                    if (LinkCore != null)
                                                    {
                                                        foreach (CerebrumPlatformFPGA FPGA in this.FPGAs)
                                                        {
                                                            if (String.Compare(FPGA.MappingID, OnFPGA, true) == 0)
                                                            {
                                                                FPGA.RequiredCores.Add(LinkCore);
                                                                break;
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
                    }
                }
                return (this.Loaded);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Get the current project manager which has loaded this platform
        /// </summary>
        public ProjectManager ProjectManager { get; set; }

        /// <summary>
        /// Disposes any resources allocated by the object
        /// </summary>
        public void Dispose()
        {
            foreach (CerebrumPlatformBoard Board in Boards)
                Board.Dispose();
            Boards.Clear();
        }

    }
}
