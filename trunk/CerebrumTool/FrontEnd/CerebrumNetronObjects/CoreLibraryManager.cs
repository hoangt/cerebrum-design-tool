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
 * CoreLibraryManager.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object used to manage Cerebrum Core Libraries.
 * History: 
 * >> (16 Dec 2010) Matthew Cotter: Corrected instantiation of cores by type/version rather than Netron GUID.
 * >> (22 Oct 2010) Matthew Cotter: Created functions to create core instances with pre-defined instance names rather than defaults.
 * >> (10 Oct 2010) Matthew Cotter: Added support for clearing libraries and propagating Core Error messages to top-level panel.
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inspired LibraryManager class, along with Cerebrum/Core specific methods to load Core package/library definitions.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;


namespace CerebrumNetronObjects
{
    /// <summary>
    /// Manages libraries containing CerebrumCore definition object to be displayed in the Netron library toolbox.
    /// </summary>
    public class CoreLibraryManager
    {
        private CollectionBase<CoreLibrary> myLibraries;
        /// <summary>
        /// Get a list of CoreLibrary objects defined in the CoreLibraryManager
        /// </summary>
        public CollectionBase<CoreLibrary> Libraries
        {
            get
            {
                return myLibraries;
            }
        }
        /// <summary>
        /// Default constructor.  Initializes an empty list of Core Libraries
        /// </summary>
        public CoreLibraryManager()
        {
            myLibraries = new CollectionBase<CoreLibrary>();            
        }
        /// <summary>
        /// Determines whether the specified library is contained within the manager
        /// </summary>
        /// <param name="LibName">The name of the library to search for</param>
        /// <returns>True if the library with the specified name exists, false otherwise</returns>
        public bool ContainsLibrary(string LibName)
        {
            return (this.GetLibrary(LibName) != null);
        }
        /// <summary>
        /// Gets the Core Library with the specified name, if it exists
        /// </summary>
        /// <param name="LibName">The name of the library to search for</param>
        /// <returns>The CoreLibrary with the specified name if it exists, null otherwise</returns>
        public CoreLibrary GetLibrary(string LibName)
        {
            foreach (CoreLibrary coreLib in this.Libraries)
            {
                if (coreLib.Name == LibName)
                    return coreLib;
            }
            return null;
        }
        /// <summary>
        /// Removes all library information from the manager
        /// </summary>
        public void Clear()
        {
            myLibraries.Clear();
        }
        /// <summary>
        /// Creates a new library with the specified name
        /// </summary>
        /// <param name="LibName">The name of the new library</param>
        /// <returns>The new CoreLibrary created</returns>
        public CoreLibrary CreateLibrary(string LibName)
        {
            CoreLibrary coreLib = new CoreLibrary(LibName);
            this.Libraries.Add(coreLib);
            coreLib.CoreError += new CoreErrorMessage(OnCoreError);
            return coreLib;
        }
        /// <summary>
        /// Creates a new CerebrumCore object of the specified type and version, with the specified instance name
        /// </summary>
        /// <param name="Instance">The instance name of the new core object</param>
        /// <param name="Type">The type name of the core object to be created</param>
        /// <param name="Version">The version number of the core object to be created</param>
        /// <returns></returns>
        public CerebrumCore CreateCoreInstance(string Instance, string Type, string Version)
        {
            foreach (CoreLibrary lib in myLibraries)
            {
                if (lib.ContainsCore(Type, Version))
                    return lib.CreateCoreInstance(Instance, Type, Version);
            }
            return null;
        }
        /// <summary>
        /// Netron method.  Creates a new object with the specified GUID from a library if it exists
        /// </summary>
        /// <param name="guid">the GUID of the object to create</param>
        /// <returns>An IShape object if the GUID was found, null otherwise</returns>
        public IShape CreateNewInstance(string guid)
        {
            foreach (CoreLibrary lib in myLibraries)
            {
                if (lib.ContainsShape(guid))
                {
                    return lib.CreateNewInstance(guid);
                }
            }
            return null;
        }
        /// <summary>
        /// Event fired when this core generates an error message
        /// </summary>
        public event CoreErrorMessage CoreError;
        void OnCoreError(CerebrumCore Core, string Message)
        {
            if (CoreError != null)
                CoreError(Core, Message);
        }
    }
}
