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
// File Name : XpsProjectOptions.cs
// Created By: Abdulrahman Abumurad
// Date : 5/25/2010
// 
// Description : This class of parsing the Mapping Algorithm output
//                 and link each  FPGA information with a set of Cores
//                 that should be mapped into it.
// >> (20 Apr 2011) Matthew Cotter: Corrected bug in LoadXPSMap() that caused all components to ALWAYS be imported into all projects in a multi FPGA platform.
// >> (18 Feb 2011) Matthew Cotter: Moved core implementation of LoadXPSMap to static method in CerebrumNetronObjects.CoreLibrary to allow all tools to read the file through a standard method.
// >> (15 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
//                                      Modified loading of XPSMap to load component definitions (and component cores) where possible, using their methods.
// Updated : 6/10/2010
//                  
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using FalconPathManager;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Class that defines several key properties of creating the XPS project.
    /// </summary>
    public class XpsProjectOptions
    {
        /////////////////////////
        //  Class Data Members //
        ///////////////////////// 

        /// <summary>
        /// Enumeration of available HDL design languages that may be used for building the XPS project.
        /// </summary>
        public enum DesignLanguages 
        {
            /// <summary>
            /// Verilog Hardware Definition Language.
            /// </summary>
            Verilog, 
            /// <summary>
            /// VHDL Hardware Definition Language.
            /// </summary>
            VHDL
        };
        private string _PlatFormArch;
        /// <summary>
        /// Platform Architecture represented as numbers and letters.
        /// </summary>
        public string PlatFormArch
        {
            get { return _PlatFormArch; }
            set { _PlatFormArch = value; }
        }
        private string _TargetDevice;
        /// <summary>
        /// Target FPGA number or ID. one project per FPGA.
        /// </summary>
        public string TargetDevice
        {
            get { return _TargetDevice; }
            set { _TargetDevice = value; }
        }
        private string _Package;
        /// <summary>
        /// paltform package number used to setup the working environment
        /// </summary>
        public string Package
        {
            get { return _Package; }
            set { _Package = value; }
        }
        private string _SpeedGrade;
        /// <summary>
        /// the speed grade associated with a certain FPGA
        /// </summary>
        public string SpeedGrade
        {
            get { return _SpeedGrade; }
            set { _SpeedGrade = value; }
        }
        private string _Hierarchy;
        /// <summary>
        /// Defines the top level module of the core.
        /// </summary>
        public string Hierarchy
        {
            get { return _Hierarchy; }
            set { _Hierarchy = value; }
        }
        private string _SearchPath;
        /// <summary>
        /// a path used to search for EDK local Cores.
        /// </summary>
        public string SearchPath
        {
            get { return _SearchPath; }
            set { _SearchPath = value; }
        }
        private string _GlobalSearchPath;
        /// <summary>
        /// Search path for the project itself.
        /// </summary>
        public string GlobalSearchPath
        {
            get { return _GlobalSearchPath; }
            set { _GlobalSearchPath = value; }
        }
        private string _UCFlocation;
        /// <summary>
        /// holds the ath and the name for User Constrains File UCF.
        /// </summary>
        public string UCFlocation
        {
            get { return _UCFlocation; }
            set { _UCFlocation = value; }
        }
        private string _MappingAlgOutputFile;
        /// <summary>
        /// Holds Mapping Algorithm output file location, the file should be .xml format
        /// </summary>
        public string XPSMapFile
        {
            get { return _MappingAlgOutputFile; }
            set { _MappingAlgOutputFile = value; }
        }
        private string _CoreRepositoryFile;
        /// <summary>
        /// Holds Cores Repository file location, the file should be .xml format
        /// </summary>
        public string CoreRepositoryFile
        {
            get { return _CoreRepositoryFile; }
            set { _CoreRepositoryFile = value;}
        }
        private string _XPSBaseSystemCores;
        /// <summary>
        /// Holds the path to xml file that contains all default
        /// cores used to build the XPS project without users custom
        /// cores
        /// </summary>
        public string XPSBaseSystemCores
        {
            get { return _XPSBaseSystemCores; }
            set { _XPSBaseSystemCores = value; }
        }
        private string _XpsProjectDirectory = "";
        /// <summary>
        /// XPS project Directory contains the FPGA name
        /// </summary>
        public string XpsProjectDirectory
        {
            get { return _XpsProjectDirectory; }
            set { _XpsProjectDirectory = value; }
        }
        private string _XpsProjectName = "system";
        /// <summary>
        /// project name that will be used in MHS, MSS names
        /// </summary>
        public string XpsProjectName
        {
            get { return _XpsProjectName; }
            set { _XpsProjectName = value; }
        }
        private string _CoreVersion = "";
        /// <summary>
        /// holds the current core version. used in core integration
        /// </summary>
        public string CoreVersion
        {
            get { return _CoreVersion; }
            set { _CoreVersion = value; }
        }
        private string _CoreInstance = "";
        /// <summary>
        /// Core instance name provided to the update MHS file and MSS file
        /// </summary>
        public string CoreInstance
        {
            get { return _CoreInstance; }
            set { _CoreInstance = value; }
        }
        /// <summary>
        /// list of FPGAs exist in the current PlatForm.
        /// FPGA names as: fpga0, fpga1, fpga2, ...etc
        /// </summary>
        public List<string> PlatFormFPGAs = new List<string>();
        private bool _XpsProjectCreated = false;
        /// <summary>
        /// used as a flag, true if project created correctly
        /// </summary>
        public bool XpsProjectCreated
        {
            get { return _XpsProjectCreated; }
            set { _XpsProjectCreated = value; }
        }
        private DesignLanguages _hdlLang;
        /// <summary>
        /// the language used in the IP Core.
        /// </summary>
        public DesignLanguages hdlLang
        {
            get { return _hdlLang; }
            set { _hdlLang = value; }
        }
        /// <summary>
        /// A list that holds Component information, see ComponentDescription class Attributes 
        /// </summary>
        public List<CerebrumCore> ComponentList = new List<CerebrumCore>();
        /// <summary>
        /// A list that holds PCores information, see PCoreDescription class Attributes 
        /// </summary>
        public List<ComponentCore> PCoresList = new List<ComponentCore>();

        /// <summary>
        /// Searches the PCoresList for a pcore matching the specified instance
        /// </summary>
        /// <param name="Instance">The instance name to search for</param>
        /// <returns>The PCoreDescription of the pcore, if it was found.  null otherwise.</returns>
        public ComponentCore GetPCore(string Instance)
        {
            foreach (ComponentCore pcore in PCoresList)
            {
                if (pcore.CoreInstance == Instance)
                {
                    return pcore;
                }
            }
            return null;
        }

        //////////////////////
        //   Class Methods  //
        ////////////////////// 

        private XpsBuilder _Builder;

        /// <summary>
        /// The XpsBuilder object associated with this project options object
        /// </summary>
        public XpsBuilder Builder
        {
            get
            {
                return _Builder;
            }
            set
            {
                _Builder = value;
            }
        }

        /// <summary>
        /// Parses the output of the Component Mapping Algorithm and assigns each core in the design to its target FPGA for integration into the corresponding XPS project.
        /// </summary>
        /// <param name="PathMan">The project path manager used to locate the Platform and CerebrumCore repositories.</param>
        /// <param name="FPGAID">The ID of the FPGA whose XPS map is to be loaded.</param>
        /// <returns>Returns true if the parsing was successful, False otherwise.</returns>
        public bool LoadXPSMap(PathManager PathMan, string FPGAID)
        {
            return CoreLibrary.LoadXPSMap(_MappingAlgOutputFile, ref ComponentList, ref PCoresList, PathMan, FPGAID);
        }
    }
}
