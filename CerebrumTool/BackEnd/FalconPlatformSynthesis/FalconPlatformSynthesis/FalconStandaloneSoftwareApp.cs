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
 * FalconPlatformSynthesis\FalconStandaloneSoftwareApp.cs
 * Name: Matthew Cotter
 * Date: 22 Jun 2010 
 * Description: Small class to represent a standalone software application to run on a processor instance, 
 *      and contains all information required to compile the applications.
 * Notes:
 *     
 * History: 
 * >> (24 Jun 2010): Added information for standalone applications.
 * >> (22 Jun 2010): Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconPlatformSynthesis
{
    /// <summary>
    /// Simple class representing a directory filter, the path and the file filter.
    /// </summary>
    public class DirectoryFilter
    {
        /// <summary>
        /// Get or set the path of the directory represented by this filter.
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// Get or set the file filter represented by this filter.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Creates a directory filter using the given path and filter.
        /// </summary>
        /// <param name="Directory">The path of the directory.</param>
        /// <param name="Filter">The file filter to be used.</param>
        public DirectoryFilter(string Directory, string Filter)
        {
            this.Directory = Directory;
            this.Filter = Filter;
        }
    }

    /// <summary>
    /// Represents a Standalone software application to be compiled and run on a processor in the design system.
    /// </summary>
    public class FalconStandaloneSoftwareApp
    {
        private int _CompilerOptLevel = 0;

        /// <summary>
        /// Constructor to initialize all the required fields of the software application for compilation.
        /// </summary>
        public FalconStandaloneSoftwareApp()
        {
            this.Name = "";
            this.CompilerOptLevel = 0;
            this.HeapSize = string.Empty;
            this.InitBRAM = false;
            this.LinkerScript = string.Empty;
            this.GlobPtrOpt = false;
            this.StackSize = string.Empty;
            this.ProgCCFlags = string.Empty;
            this.ProgStartAddr = string.Empty;

            this.Libraries = new List<string>();
            this.Headers = new List<string>();
            this.Sources = new List<string>();
            this.LibSearchPath = new List<string>();
            this.IncludeSearchPath = new List<string>();

            this.RuntimeSources = new List<DirectoryFilter>();
            this.RuntimeHeaders = new List<DirectoryFilter>();
        }

        /// <summary>
        /// Get or set the name of the software application.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the compiler optimization effort (0 to 3).
        /// </summary>
        public int CompilerOptLevel 
        {
            get
            {
                return _CompilerOptLevel;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 3)
                    value = 3;
                _CompilerOptLevel = value;
            }
        }
        /// <summary>
        /// Get or set the size of the heap to be allocated for the software application.
        /// </summary>
        public string HeapSize { get; set; }
        /// <summary>
        /// Get or set whether this ELF file should be initialized in BRAM.
        /// </summary>
        public bool InitBRAM { get; set; }
        /// <summary>
        /// Get or set the path to the linker script to be used for compilation of this application.
        /// </summary>
        public string LinkerScript { get; set; }
        /// <summary>
        /// Get or set whether global pointer optimizations should be used during compilation.
        /// </summary>
        public bool GlobPtrOpt { get; set; }
        /// <summary>
        /// Get or set the size of the stack to be allocated for the software application.
        /// </summary>
        public string StackSize { get; set; }
        /// <summary>
        /// Get or set additional compiler flags to be used for compilation.
        /// </summary>
        public string ProgCCFlags { get; set; }
        /// <summary>
        /// Get or set the starting address of the program.
        /// </summary>
        public string ProgStartAddr { get; set; }

        /// <summary>
        /// Get or set the list of libraries to be included during compilation.
        /// </summary>
        public List<string> Libraries { get; set; }
        /// <summary>
        /// Adds a library to the list of libraries to be included during compilation.
        /// </summary>
        public void AddLibrary(string Library)
        {
            if (!this.Libraries.Contains(Library))
                this.Libraries.Add(Library);
        }
        /// <summary>
        /// Removes a library from the list of libraries to be included during compilation.
        /// </summary>
        public void RemoveLibrary(string Library)
        {
            if (this.Libraries.Contains(Library))
                this.Libraries.Add(Library);
        }

        /// <summary>
        /// Get or set the list of library search paths to be included during compilation.
        /// </summary>
        public List<string> LibSearchPath { get; set; }
        /// <summary>
        /// Adds a location to the list of library search paths to be included during compilation.
        /// </summary>
        public void AddLibSearchPath(string LibPath)
        {
            if (!this.LibSearchPath.Contains(LibPath))
                this.LibSearchPath.Add(LibPath);
        }
        /// <summary>
        /// Removes a location from the list of library search paths to be included during compilation.
        /// </summary>
        public void RemoveLibSearchPath(string LibPath)
        {
            if (this.LibSearchPath.Contains(LibPath))
                this.LibSearchPath.Add(LibPath);
        }


        /// <summary>
        /// Get or set the list of include search paths to be included during compilation.
        /// </summary>
        public List<string> IncludeSearchPath { get; set; }
        /// <summary>
        /// Adds a location to the list of include search paths to be included during compilation.
        /// </summary>
        public void AddIncludeSearchPath(string IncludePath)
        {
            if (!this.IncludeSearchPath.Contains(IncludePath))
                this.IncludeSearchPath.Add(IncludePath);
        }
        /// <summary>
        /// Removes a location from the list of include search paths to be included during compilation.
        /// </summary>
        public void RemoveIncludeSearchPath(string IncludePath)
        {
            if (this.IncludeSearchPath.Contains(IncludePath))
                this.IncludeSearchPath.Remove(IncludePath);
        }

        /// <summary>
        /// Get or set the list of locations to be searched at runtime for source files (*.c) at compile time.
        /// </summary>
        public List<DirectoryFilter> RuntimeSources { get; set; }
        /// <summary>
        /// Get or set the list of source files (*.c) to be compiled.
        /// </summary>
        public List<string> Sources { get; set; }
        /// <summary>
        /// Adds a file to the list of source files (*.c) to be compiled.
        /// </summary>
        public void AddSource(string Source)
        {
            if (!this.Sources.Contains(Source))
                this.Sources.Add(Source);
        }
        /// <summary>
        /// Removes a file from the list of source files (*.c) to be compiled.
        /// </summary>
        public void RemoveSource(string Source)
        {
            if (this.Sources.Contains(Source))
                this.Sources.Remove(Source);
        }
        /// <summary>
        /// Adds a location to be searched at runtime for source files (*.c) at compile time.
        /// </summary>
        public void AddRuntimeSources(string Path, string Filter)
        {
            this.RuntimeSources.Add(new DirectoryFilter(Path, Filter));
        }

        /// <summary>
        /// Get or set the list of locations to be searched at runtime for header files (*.h) at compile time.
        /// </summary>
        public List<DirectoryFilter> RuntimeHeaders { get; set; }
        /// <summary>
        /// Get or set the list of header files (*.h) to be compiled.
        /// </summary>
        public List<string> Headers { get; set; }
        /// <summary>
        /// Adds a file to the list of header files (*.h) to be compiled.
        /// </summary>
        public void AddHeader(string Header)
        {
            if (!this.Headers.Contains(Header))
                this.Headers.Add(Header);
        }
        /// <summary>
        /// Removes a file from the list of header files (*.h) to be compiled.
        /// </summary>
        public void RemoveHeader(string Header)
        {
            if (this.Headers.Contains(Header))
                this.Headers.Remove(Header);
        }
        /// <summary>
        /// Adds a location to be searched at runtime for header files (*.h) at compile time.
        /// </summary>
        public void AddRuntimeHeaders(string Path, string Filter)
        {
            this.RuntimeHeaders.Add(new DirectoryFilter(Path, Filter));
        }

        /// <summary>
        /// Determines if the standalone application has enough information to be compiled.
        /// </summary>
        /// <returns>True if the application is ready, False otherwise.</returns>
        public bool Ready()
        {
            bool bReady = true;
            bReady = (bReady) &&
                     (this.Name != string.Empty) &&
                     ((this.Sources.Count > 0) || (this.RuntimeSources.Count > 0));
            return bReady;
        }
    }
}
