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
 * PathManager.cs
 * Name: Matthew Cotter
 * Date:  1 Jul 2010 
 * Description: This class allows a simple way of loading a project's relevant paths and accessing them in a consistent manner from within the tool libraries.
 * History: 
 * >> (20 Aug 2010) Matthew Cotter: Added DecodeString function that is capable of decoding a string using stored references.
 * >> (19 Aug 2010) Matthew Cotter: Corrected a bug that produced errors when parsing a path entry containing multiple path-references.
 * >> (12 Aug 2010) Matthew Cotter: Added IFalconLibrary interface implementation.
 * >> ( 8 Jul 2010) Matthew Cotter: Added check to remove trailing slashes (both forward and backward on paths as they are set and/or loaded.   This
 *                                    ensures a consistent mode of using the paths to append slashes.
 * >> ( 7 Jul 2010) Matthew Cotter: Added ability to set, change, and save paths loaded in the table at runtime.
 *                                    Added [] index operator to complement GetPath() and SetPath() methods.
 * >> ( 1 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using Microsoft.Win32;
using FalconGlobal;

namespace FalconPathManager
{
    /// <summary>
    /// Collector class to define a common interface for accessing and managing all paths relevant to a design project. 
    /// Path support inludes a level of indirection, allowing one path to reference another.  This functionality allows support 
    /// for top-level- and relative- paths to be defined.
    /// </summary>
    public class PathManager : IFalconLibrary
    {
        private Hashtable _Paths;

        /// <summary>
        /// Default constructor.   Initializes the path Hash table to an empty table.
        /// </summary>
        public PathManager()
        {
            Clear();
            GetInstallTimePaths();
        }

        private const string PRIMARY_REG_KEY = "SOFTWARE\\PennState\\Cerebrum";
        private void GetInstallTimePaths()
        {
            this["CerebrumRoot"] = GetCerebrumInstallPath();
            this["BinDirectory"] = GetCerebrumBinPath();
            this["CerebrumCores"] = GetCerebrumCoresPath();
            this["Platforms"] = GetCerebrumPlatformsPath();
        }

        /// <summary>
        /// Get the path to the location of the Cerebrum Install.  If it is not set in the registry, it is assumed to be the parent directory of the
        /// directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for CerebrumRoot for each project loaded.</returns>
        private string GetCerebrumInstallPath()
        {
            string _CerebrumRoot = string.Empty;
            if (_CerebrumRoot == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _CerebrumRoot = (string)InstallKey.GetValue("CerebrumRoot", string.Empty);
                    bRegSuccess = (_CerebrumRoot != string.Empty);
                    if (!bRegSuccess)
                    {
                        _CerebrumRoot = (string)InstallKey.GetValue("InstallPath", string.Empty);
                        bRegSuccess = (_CerebrumRoot != string.Empty);
                    }
                }
                catch { }
                if (!bRegSuccess)
                {
                    // Path to Cerebrum Global (Install-Time) Paths
                    Assembly Entry = System.Reflection.Assembly.GetEntryAssembly();
                    _CerebrumRoot = new FileInfo(Entry.FullName).Directory.Parent.FullName;
                }
            }
            return _CerebrumRoot;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum binary executables.  If it is not set in the registry, it is assumed to be the 
        /// directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for BinDirectory for each project loaded.</returns>
        private string GetCerebrumBinPath()
        {
            string _BinDirectory = string.Empty;
            if (_BinDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _BinDirectory = (string)InstallKey.GetValue("BinDirectory", string.Empty);
                    bRegSuccess = (_BinDirectory != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _BinDirectory = String.Format("{0}\\bin", GetCerebrumInstallPath());
                }
            }
            return _BinDirectory;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum Core packages.  If it is not set in the registry, it is assumed to be the 'Cores' subdirectory of the
        /// parent directory of the directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for CerebrumCores for each project loaded.</returns>
        private string GetCerebrumCoresPath()
        {
            string _CerebrumCores = string.Empty;
            if (_CerebrumCores == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _CerebrumCores = (string)InstallKey.GetValue("CerebrumCores", string.Empty);
                    bRegSuccess = (_CerebrumCores != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _CerebrumCores = String.Format("{0}\\Cores", GetCerebrumInstallPath());
                }
            }
            return _CerebrumCores;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum Platforms.  If it is not set in the registry, it is assumed to be the 'Platforms' subdirectory of the
        /// parent directory of the directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for Platforms for each project loaded.</returns>
        private string GetCerebrumPlatformsPath()
        {
            string _Platforms = string.Empty;
            if (_Platforms == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _Platforms = (string)InstallKey.GetValue("Platforms", string.Empty);
                    bRegSuccess = (_Platforms != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _Platforms = String.Format("{0}\\Platforms", GetCerebrumInstallPath());
                }
            }
            return _Platforms;
        }



        /// <summary>
        /// Loads the specified XML file, loading all named paths contained within into the path table.
        /// </summary>
        /// <param name="PathXMLFile">The path to the XML file from which paths are to be loaded.</param>
        public PathManager(string PathXMLFile)
        {
            Clear();
            LoadPaths(PathXMLFile);
        }
        
        /// <summary>
        /// Erases all stored path data.
        /// </summary>
        public void Clear()
        {
            _Paths = new Hashtable();
            GetInstallTimePaths();
        }

        /// <summary>
        /// Loads the specified XML file, loading all named paths contained within into the path table.  Any existing paths that are 
        /// defined in the new file will be overwritten with the new data.  If preservation of existing data is desired, use the 
        /// LoadPaths(string, bool) overload with the boolean parameter set to false.
        /// </summary>
        /// <param name="PathXMLFile">The path to the XML file from which paths are to be loaded.</param>
        public void LoadPaths(string PathXMLFile)
        {
            LoadPaths(PathXMLFile, true);
            GetInstallTimePaths();
            this["LocalProjectRoot"] = new FileInfo(PathXMLFile).Directory.FullName;
        }

        /// <summary>
        /// Loads the specified XML file, loading all named paths contained within into the path table.
        /// </summary>
        /// <param name="PathXMLFile">The path to the XML file from which paths are to be loaded.</param>
        /// <param name="OverwriteExisting">If true, entries that exist are already loaded will be ovewritten by a matching entry
        /// in the new file.  If false, any existing values will be preserved.</param>
        public void LoadPaths(string PathXMLFile, bool OverwriteExisting)
        {
            if (!File.Exists(PathXMLFile))
                throw new FileNotFoundException(PathXMLFile + " not found.");

            XmlDocument PathDoc;
            PathDoc = new XmlDocument();
            PathDoc.Load(PathXMLFile);
            foreach (XmlNode xNode in PathDoc.ChildNodes)
            {
                if (xNode.Name.ToLower() == "paths")
                {
                    foreach (XmlNode xPathNode in xNode.ChildNodes)
                    {
                        if (xPathNode.Name.ToLower() == "path")
                        {
                            string key = string.Empty;
                            string value = string.Empty;

                            foreach (XmlAttribute xAttr in xPathNode.Attributes)
                            {
                                if (xAttr.Name.ToLower() == "name")
                                    key = xAttr.Value.ToString().ToLower();
                                else if (xAttr.Name.ToLower() == "value")
                                    value = xAttr.Value.ToString();
                            }
                            if ((OverwriteExisting) || (!this.HasPath(key)))
                                SetPath(key, value);
                        }
                    }
                }
            }

            GetInstallTimePaths();
            this["LocalProjectRoot"] = new FileInfo(PathXMLFile).Directory.FullName;
        }

        /// <summary>
        /// Decodes a string, replacing any encoded references with those extracted from the currently loaded file.   Any strings enclosed in
        /// ${...} are replaced--any strings outside of these enclosures are left as-is.
        /// </summary>
        /// <param name="EncodedString">The encoded path string to be decoded.</param>
        /// <returns>The decoded string, with any valid references replaced.</returns>
        public string DecodePath(string EncodedString)
        {
            if (EncodedString != string.Empty)
            {
                while (EncodedString.Contains("${"))
                {
                    int start = EncodedString.IndexOf("${");
                    int end = EncodedString.IndexOf("}");
                    if ((start >= 0) && (end >= 0))
                    {
                        string subPath = EncodedString.Substring(start + 2, end - start - 2);
                        string replaceString = "${" + subPath + "}";
                        EncodedString = EncodedString.Replace(replaceString, this.GetPath(subPath));
                    }
                }
            }
            return EncodedString;
        }

        /// <summary>
        /// Returns a value indicating whether a particular named path has been defined.
        /// </summary>
        /// <param name="PathName">The name of the path to find.</param>
        /// <returns>True if the path has been defined, false if it has not.</returns>
        public bool HasPath(string PathName)
        {
            PathName = PathName.ToLower();
            return _Paths.ContainsKey(PathName);
        }
        /// <summary>
        /// Index operator that allows for array-style indexing of named paths by passing the path name as a 
        /// string for the index.  Path names are treated as case-insensitive.
        /// </summary>
        /// <param name="PathName">The name of the path to access.</param>
        /// <returns>The value of the path that was to be retrieved, if it was found;  an empty string otherwise.</returns>
        public string this[string PathName]
        {
            get
            {
                return GetPath(PathName);
            }
            set
            {
                SetPath(PathName, value);
            }
        }
        /// <summary>
        /// Retrieves the value of the specified named path, if it has been defined.
        /// </summary>
        /// <param name="PathName">The name of the path to retrieve.</param>
        /// <returns>The value of the path that was to be retrieved, if it was found; an empty string otherwise</returns>
        public string GetPath(string PathName)
        {
            string returnPath = string.Empty;

            PathName = PathName.ToLower();
            if (_Paths.ContainsKey(PathName))
                returnPath = (string)_Paths[PathName];
            else
            {
                Console.WriteLine("WARNING: Requested token that does not exist: ({0})", PathName);
            }
            returnPath = DecodePath(returnPath);
            return returnPath;
        }

        /// <summary>
        /// Sets the named path to the specified value.   If the path name already exists, it will be replaced.
        /// </summary>
        /// <param name="PathName">The name of the path to set.</param>
        /// <param name="PathValue">The value to set for the path.</param>
        public void SetPath(string PathName, string PathValue)
        {
            if ((PathValue.EndsWith("\\")) || (PathValue.EndsWith("/")))
                PathValue = PathValue.Substring(0, PathValue.Length - 1);

            PathName = PathName.ToLower();
            if (_Paths.ContainsKey(PathName))
            {
                _Paths.Remove(PathName);
            }
            _Paths.Add(PathName, PathValue);
        }
        /// <summary>
        /// Save all stored paths to an XML document so that they may be loaded later.
        /// </summary>
        /// <param name="PathXMLFile">The path to the XML file where the paths should be saved.</param>
        public void SavePaths(string PathXMLFile)
        {
            XmlDocument PathDoc;
            PathDoc = new XmlDocument();
            XmlNode xRoot = PathDoc.CreateElement("Paths");
            FalconFileRoutines.WriteCerebrumDisclaimerXml(xRoot);    // Added by Matthew Cotter 8/18/2010

            this["LocalProjectRoot"] = new FileInfo(PathXMLFile).Directory.FullName;

            foreach (string path in _Paths.Keys)
            {
                XmlNode xPath = PathDoc.CreateElement("Path");
                XmlAttribute xPathName = PathDoc.CreateAttribute("Name");
                xPathName.Value = path;
                XmlAttribute xPathValue = PathDoc.CreateAttribute("Value");
                xPathValue.Value = (string)_Paths[path];
                xPath.Attributes.Append(xPathName);
                xPath.Attributes.Append(xPathValue);
                xRoot.AppendChild(xPath);
            }

            PathDoc.AppendChild(xRoot);
            PathDoc.Save(PathXMLFile);
        }

        /// <summary>
        /// Returns a clone/copy of the Hashtable structure used to store the paths.
        /// </summary>
        /// <returns>A Hashtable object that is a copy of that used to store the path variables.</returns>
        public Hashtable GetAllPaths()
        {
            return (Hashtable)_Paths.Clone();
        }

        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon Project Path/Environment Manager";
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
