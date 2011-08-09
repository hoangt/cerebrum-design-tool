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
 * ProjectEventRecorder.cs
 * Name: Matthew Cotter
 * Date:  9 Feb 2011 
 * Description: Defines a class that is used to provide a simple interface to accessing and saving events to and from an XML file.
 * History: 
 * >> ( 9 Feb 2011) Matthew Cotter: Created basic interface to Log and Get project events.
 * >> ( 9 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using FalconPathManager;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Class used for logging and loading project events by name and related component/core.
    /// </summary>
    public class ProjectEventRecorder : IDisposable
    {
        private const string EVENT_LOG_FILENAME = "project_events.xml";
        private PathManager PathMan;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProjectEventRecorder()
        {
            this.PathMan = null;
        }

        #region Public Methods

        /// <summary>
        /// Associates this event recorder with the project loaded in the project manager.
        /// </summary>
        /// <param name="ProjectPathManager">The Path manager pre-loaded from the project's file(s).</param>
        public void Open(PathManager ProjectPathManager)
        {
            this.PathMan = ProjectPathManager;
        }
        /// <summary>
        /// Disassociates this event recorder with any previously held project.
        /// </summary>
        public void Close()
        {
            this.PathMan = null;
        }
        /// <summary>
        /// Logs an event with the given name.
        /// </summary>
        /// <param name="EventName">The name of the event to log.</param>
        public void LogProjectEvent(string EventName)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "LogProjectEvent"));
            LogEvent(EventName, null, null, null);
        }
        /// <summary>
        /// Logs an event with the given name, associated with the specified FPGA.
        /// </summary>
        /// <param name="EventName">The name of the event to log.</param>
        /// <param name="FPGA_ID">The FPGA associated with the event.</param>
        public void LogFPGAEvent(string EventName, string FPGA_ID)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "LogFPGAEvent"));
            LogEvent(EventName, null, null, FPGA_ID);
        } 
        /// <summary>
        /// Logs an event with the given name, associated with the specified component and core.
        /// </summary>
        /// <param name="EventName">The name of the event to log.</param>
        /// <param name="Component">The component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="Core">The core within the component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        public void LogComponentEvent(string EventName, string Component, string Core)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "LogComponentEvent"));
            LogEvent(EventName, Component, Core, null);
        } 
        /// <summary>
        /// Loads a previously logged event with the given name.  If no such event is found, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="EventName">The name of the event to load.</param>
        public DateTime GetProjectEvent(string EventName)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "GetProjectEvent"));
            return GetEvent(EventName, null, null, null);
        }
        /// <summary>
        /// Loads a previously logged event with the given name and associated FPGA.  If no such event is found, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="EventName">The name of the event to load.</param>
        /// <param name="FPGA_ID">The FPGA associated with the event.</param>
        public DateTime GetFPGAEvent(string EventName, string FPGA_ID)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "GetFPGAEvent"));
            return GetEvent(EventName, null, null, FPGA_ID);
        }
        /// <summary>
        /// Loads a previously logged event with the given name and associated Component and Core.  If no such event is found, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="EventName">The name of the event to load.</param>
        /// <param name="Component">The component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="Core">The core within the component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        public DateTime GetComponentEvent(string EventName, string Component, string Core)
        {
            if (PathMan == null)
                throw new Exception(String.Format("Path Manager not loaded into Project Event Recorder! Cannot perform operation: {0}", "GetComponentEvent"));
            return GetEvent(EventName, Component, Core, null);
        }
        /// <summary>
        /// Implementation of IDisposable.Dispose.  Releases internal references and resources.
        /// </summary>
        public void Dispose()
        {
            PathMan = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the current date and time as a string to be logged for an event.
        /// </summary>
        /// <returns>Returns the current date and time as a string to be logged for an event.</returns>
        private string GetTimestamp()
        {
            DateTime stamp = DateTime.Now;
            return String.Format("{0} {1}", stamp.ToShortDateString(), stamp.ToShortTimeString());
        }
        /// <summary>
        /// Logs an event with the given name, associated with the specified component and core, if any.
        /// </summary>
        /// <param name="EventName">The name of the event to log.</param>
        /// <param name="Component">The component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="Core">The core within the component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="FPGA_ID">The FPGA ID associated with the event.  The parameter is ignored unless, both Component and Core are null.</param>
        private void LogEvent(string EventName, string Component, string Core, string FPGA_ID)
        {
            string TS = GetTimestamp();
            XmlDocument EventDoc = LoadProjectEventLog();
            XmlNode EventNode = GetEventNode(EventDoc, EventName, Component, Core, FPGA_ID);
            if (EventNode == null)
            {
                EventNode = EventDoc.CreateElement(EventName);
            }
            if ((Component != null) && (Core != null))
            {
                ((XmlElement)EventNode).SetAttribute("Component", Component);
                ((XmlElement)EventNode).SetAttribute("Core", Core);
            }
            else if (FPGA_ID != null)
            {
                ((XmlElement)EventNode).SetAttribute("FPGA", FPGA_ID);
            }
            ((XmlElement)EventNode).SetAttribute("Timestamp", TS);
            AddUpdateNode(EventDoc, EventNode);
            SaveProjectEventLog(EventDoc);
        }
        /// <summary>
        /// Loads a previously logged event with the given name and associated Component and Core.  If no such event is found, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="EventName">The name of the event to load.</param>
        /// <param name="Component">The component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="Core">The core within the component associated with the event. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="FPGA_ID">The FPGA ID associated with the event.  The parameter is ignored unless, both Component and Core are null.</param>
        private DateTime GetEvent(string EventName, string Component, string Core, string FPGA_ID)
        {
            XmlDocument EventDoc = LoadProjectEventLog();
            XmlNode EventNode = GetEventNode(EventDoc, EventName, Component, Core, FPGA_ID);
            DateTime stamp = DateTime.MinValue;
            if (EventNode != null)
            {
                DateTime.TryParse(((XmlElement)EventNode).GetAttribute("Timestamp"), out stamp);
            }
            return stamp;
        }       
        /// <summary>
        /// Opens the project events log for the project loaded in the path manager.
        /// </summary>
        /// <returns>An XmlDocument loaded with the project events, if the file exists.  A document with only an XML declaration and root element, if it does not.</returns>
        private XmlDocument LoadProjectEventLog()
        {
            XmlDocument xEventDoc = new XmlDocument();
            FileInfo LogFile = new FileInfo(String.Format("{0}\\{1}", PathMan["LocalProjectRoot"], EVENT_LOG_FILENAME));
            if (LogFile.Exists)
            {
                xEventDoc.Load(LogFile.FullName);
            }
            else
            {
                XmlNode xRoot = xEventDoc.CreateElement("ProjectEvents");
                xEventDoc.AppendChild(xEventDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                xEventDoc.AppendChild(xRoot);
            }
            return xEventDoc;
        }
        /// <summary>
        /// Saves the project events log for the project loaded in the path manager.
        /// </summary>
        /// <param name="EventLogXML">The XmlDocument populated with the project events to be saved.</param>
        private void SaveProjectEventLog(XmlDocument EventLogXML)
        {
            FileInfo LogFile = new FileInfo(String.Format("{0}\\{1}", PathMan["LocalProjectRoot"], EVENT_LOG_FILENAME));
            EventLogXML.Save(LogFile.FullName);
        }
        /// <summary>
        /// Updates the node in the specified XML document which corresponds to the UpdatedNode.  If no such node exists, the Updated node is appended to the document.
        /// </summary>
        /// <param name="EventLogXML">The XML document to update.</param>
        /// <param name="UpdatedNode">The node containing the information to be added to the document.</param>
        private void AddUpdateNode(XmlDocument EventLogXML, XmlNode UpdatedNode)
        {
            foreach (XmlNode xRoot in EventLogXML.ChildNodes)
            {
                if (String.Compare(xRoot.Name, "xml", true) != 0)
                {
                    foreach (XmlNode xEventNode in xRoot.ChildNodes)
                    {
                        if (String.Compare(xEventNode.Name, UpdatedNode.Name, true) == 0)
                        {
                            if (xEventNode == UpdatedNode)
                                return;
                            else 
                            {
                                XmlElement EventElement = (XmlElement)xEventNode;
                                XmlElement UpdatedElement = (XmlElement)UpdatedNode;
                                if ((EventElement.HasAttribute("Component") && EventElement.HasAttribute("Core")) &&
                                    (UpdatedElement.HasAttribute("Component") && UpdatedElement.HasAttribute("Core")))
                                {
                                    // Make sure this corresponds to the right event
                                    if ((String.Compare(EventElement.GetAttribute("Component"), EventElement.GetAttribute("Component")) == 0) &&
                                        (String.Compare(EventElement.GetAttribute("Core"), EventElement.GetAttribute("Core")) == 0))
                                    {
                                        xRoot.ReplaceChild(UpdatedNode, xEventNode);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    xRoot.AppendChild(UpdatedNode);
                }
            }
        }
        /// <summary>
        /// Searches the XML hierarchy to locate a node corresponding to the specified event name, component, and core, as applicable.
        /// </summary>
        /// <param name="EventLogXML">The XMLDocument object to search for the event node.</param>
        /// <param name="EventNodeName">The name of the event to locate.</param>
        /// <param name="Component">The component associated with the event to locate. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="Core">The core within the component associated with the event to locate. If null, both the Component and Core parameters are ignored.</param>
        /// <param name="FPGA_ID">The FPGA ID associated with the event.  The parameter is ignored unless, both Component and Core are null.</param>
        /// <returns>An XML Node object from the document, if found.  Null otherwise.</returns>
        private XmlNode GetEventNode(XmlDocument EventLogXML, string EventNodeName, string Component, string Core, string FPGA_ID)
        {
            foreach (XmlNode xRoot in EventLogXML.ChildNodes)
            {
                if (String.Compare(xRoot.Name, "xml", true) != 0)
                {
                    foreach (XmlNode xEventNode in xRoot.ChildNodes)
                    {
                        if (String.Compare(xEventNode.Name, EventNodeName, true) == 0)
                        {
                            bool bComponentMatch = false;
                            bool bCoreMatch = false;
                            if ((Component != null) && (Core != null))
                            {
                                // Look for Component/Core Event
                                foreach (XmlAttribute xAttr in xEventNode.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "Component", true) == 0)
                                    {
                                        bComponentMatch = true;
                                    }
                                    else if (String.Compare(xAttr.Name, "Component", true) == 0)
                                    {
                                        bCoreMatch = true;
                                    }
                                }
                                if (bComponentMatch && bCoreMatch)
                                {
                                    return xEventNode;
                                }
                            }
                            else if (FPGA_ID != null)
                            {
                                // Look for FPGA Event
                                foreach (XmlAttribute xAttr in xEventNode.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "FPGA", true) == 0)
                                    {
                                        return xEventNode;
                                    }
                                }
                            }
                            else
                            {
                                // Look for a Project Event
                                return xEventNode;
                            }
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
