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
 * FalconPlatformSynthesis\FalconProcessorOS.cs
 * Name: Matthew Cotter
 * Date: 22 Jun 2010 
 * Description: Small class to represent a processor instance, and contains all information required to compile
 *      software for that instance, including any applications set to run on it.
 * Notes:
 *     
 * History: 
 * >> (24 Aug 2010) Matthew Cotter: Added CerebrumProcessorID in support of self-identification of processors running in a multi-processor environment on an FPGA.
 * >> (18 Aug 2010) Matthew Cotter: Reinstated changes to platform/processor property shuffling.  Details on new locations can be found in the Project XML Documentation.
 * >> (11 Aug 2010) Matthew Cotter: Properties that have been proposed to be moved from Platform/Board to Design.Processors have been reverted pending approval of changes.
 * >> ( 2 Aug 2010) Matthew Cotter: Moved properties and paths specific to the cross-compilation of Linux for a processor from the FalconPlatform class.
 *                                    Renamed ELDKInitArg to CompilerArgs.
 *                                    Moved ELDKLocation from Path Manager to FalconProcessorOS property.   Renamed ELDKLocation to CompilerLocation.
 * >> ( 6 Jul 2010) Matthew Cotter: Moved DTSFile, ELDKInitArg and MakeConfig properties from FalconProcessorOS to FalconPlatform due to location in the board specification.
 * >> (24 Jun 2010) Matthew Cotter: Added information for standalone applications.
 * >> (23 Jun 2010) Matthew Cotter: Implemented properties to represent processor and OS
 * >> (22 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconPlatformSynthesis
{
    /// <summary>
    /// Enumeration of the processor types (PowerPC or Microblaze) available.
    /// </summary>
    public enum SystemProcessorType
    {
        /// <summary>
        /// PowerPC Processor type.
        /// </summary>
        PowerPC,
        /// <summary>
        /// Microblaze Processor type.
        /// </summary>
        MicroBlaze
    }

    /// <summary>
    /// Enumeration of the OS types (Linux or Standalone) available.
    /// </summary>
    public enum SystemProcessorOS
    {
        /// <summary>
        /// Standalone Software application.
        /// </summary>
        Standalone,
        /// <summary>
        /// Custom-compiled Linux kernel.
        /// </summary>
        Linux
    }

    /// <summary>
    /// Represents a processor instance in the design, including all required information to compile the software
    /// that will run on it.
    /// </summary>
    public class FalconProcessorOS
    {
        private List<FalconStandaloneSoftwareApp> _Applications;
        private string _OutputELF;

        /// <summary>
        /// Constructor to initialize all of the fields required for compilation of software.
        /// </summary>
        public FalconProcessorOS()
        {
            this.Instance = string.Empty;
            this.Type = "PowerPC";
            this.OS = SystemProcessorOS.Linux;
            this.OutputELF = string.Empty;
            this.ConsoleDevice = string.Empty;

            this.LinuxKernelSource = string.Empty;
            this.CompilerArgs = string.Empty;
            this.MakeConfig = string.Empty;
            this.DTSFile = string.Empty;
            this.CerebrumProcessorID = string.Empty;

            _Applications = new List<FalconStandaloneSoftwareApp>();
        }

        /// <summary>
        /// Get or set the Instance name of this processor.
        /// </summary>
        public string Instance { get; set; }
        
        /// <summary>
        /// Get or set the type of this processor.
        /// </summary>
        public string Type { get; set; }
        //public SystemProcessorType Type { get; set; }

        /// <summary>
        /// Get or set the operating system (software) to be run on this processor.
        /// </summary>
        public SystemProcessorOS OS { get; set; }
        /// <summary>
        /// Get or set the list of Applications that are configured to run on this processor.
        /// </summary>
        public List<FalconStandaloneSoftwareApp> StandaloneApps
        {
            get
            {
                return _Applications;
            }
            set
            {
                if (value == null)
                    value = new List<FalconStandaloneSoftwareApp>();
                _Applications = value;
            }
        }

        /// <summary>
        /// Get or set the core instance name to be used as the console device (Linux) or STDIN/STDOUT (Standalone) for the software on this processor.
        /// </summary>
        public string ConsoleDevice { get; set; }
        /// <summary>
        /// Get or set the name of the output ELF file generated for this processor.
        /// </summary>
        public string OutputELF 
        {
            get
            {
                if ((_OutputELF == string.Empty) || (_OutputELF == null))
                    return this.Instance + ".elf";
                else
                    return _OutputELF;
            }
            set
            {
                _OutputELF = value;
            }
        }

        /// <summary>
        /// Unique identifier assigned to the processor that is compiled into the Linux Kernel.
        /// </summary>
        public string CerebrumProcessorID { get; set; }

        /// <summary>
        /// Get or set the full path to the location of the linux kernel source.
        /// </summary>
        public string LinuxKernelSource { get; set; }
        /// <summary>
        /// OS = Linux.  Get or set the arguments to be passed to the compiler for Linux compilation.
        /// </summary>
        public string CompilerArgs { get; set; }
        /// <summary>
        /// OS = Linux.  Get or set the default make config to be used for setting the linux kernel compilation.
        /// </summary>
        public string MakeConfig { get; set; }
        /// <summary>
        /// OS = Linux.  Get or set the intermediate filename to be set for the DTS during Linux compilation.  
        /// Linux compile for the FPGA appears to be name-senstive.
        /// </summary>
        public string DTSFile { get; set; }

        /// <summary>
        /// The instance name of the top-level component owning this processor.
        /// </summary>
        public string OwnerComponent { get; set; }
    }
}
