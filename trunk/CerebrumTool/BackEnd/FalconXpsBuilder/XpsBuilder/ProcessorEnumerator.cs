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
/************************************************************************************************************
 * ProcessorEnumerator.cs
 * Name: Matthew Cotter
 * Date: 11 Aug 2010 
 * Description: Read an XPS Project's MHS File and determine the CPU Numbers for each processor instance.
 * History: 
 * >> (11 Aug 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;


namespace FalconXpsBuilder
{
    /// <summary>
    /// Class to parse the an XPS Project MHS File and determine the CPU Numbering scheme used for all processors in the project.
    /// </summary>
    public class ProcessorEnumerator
    {
        /// <summary>
        /// Defines an system processor--Instance, PowerPC or not, it's attached Debug Bus Interface, and its identified CPU Number.
        /// </summary>
        public class ProcessorEntry
        {
            /// <summary>
            /// The instance name of the processor.
            /// </summary>
            public string Instance { get; set; }
            /// <summary>
            /// Indicates whether the processor instance is a PowerPC (true) or Microblaze (false).
            /// </summary>
            public bool PowerPC { get; set; }
            /// <summary>
            /// The name of the bus interface signal to which this processor instance is attached.
            /// </summary>
            public string DebugBus { get; set; }
            /// <summary>
            /// The corresponding CPU number assigned to the bus interface.
            /// </summary>
            public int CPUNumber { get; set; }
        }

        private class DebugBus
        {
            public string BusPort { get; set; }
            public string BusSignal { get; set; }
        }

        private class ControllerEntry
        {
            public string Instance { get; set; }
            public bool PPCController { get; set; }
            public List<DebugBus> DebugBusList { get; set; }
        }

        private List<ProcessorEntry> _Procs;
        private List<ControllerEntry> _Controllers;

        /// <summary>
        /// Default constructor -- Performs required initialization of internal data structures.
        /// </summary>
        public ProcessorEnumerator()
        {
        }

        /// <summary>
        /// Reads the specified MHS file and stores the collection of Processor Instances and Debug/Programming Controllers for later review.
        /// After the file has been processed, each processor is tagged with it's corresponding 'cpunr' for debug/programming.
        /// </summary>
        /// <param name="MHSFile">The path to the MHS file to be read.</param>
        public void ParseMHS(string MHSFile)
        {
            try
            {
                _Procs = new List<ProcessorEntry>();
                _Controllers = new List<ControllerEntry>();

                StreamReader reader = new StreamReader(MHSFile);
                int i = 0;
                string line = string.Empty;
                ProcessorEntry PEntry = null;
                ControllerEntry CEntry = null;

                #region Parse Processors and Debug Controllers from MHS File
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Trim();
                    i++;

                    if (line.EndsWith("BEGIN mdm"))
                    {
                        // Microblaze Debug Manager block
                        PEntry = null;
                        CEntry = new ControllerEntry();
                        CEntry.Instance = string.Empty;
                        CEntry.PPCController = false;
                        CEntry.DebugBusList = new List<DebugBus>();
                    }
                    else if (line.EndsWith("BEGIN jtagppc_cntlr"))
                    {
                        // JTAG PowerPC Controller
                        PEntry = null;
                        CEntry = new ControllerEntry();
                        CEntry.Instance = string.Empty;
                        CEntry.PPCController = true;
                        CEntry.DebugBusList = new List<DebugBus>();
                    }
                    else if (line.EndsWith("BEGIN microblaze"))
                    {
                        // Microblaze Instance
                        CEntry = null;
                        PEntry = new ProcessorEntry();
                        PEntry.Instance = string.Empty;
                        PEntry.PowerPC = false;
                        PEntry.CPUNumber = -1;
                        PEntry.DebugBus = string.Empty;
                    }
                    else if ((line.EndsWith("BEGIN ppc440_virtex5")) || (line.EndsWith("BEGIN ppc405_virtex4")))
                    {
                        // PowerPC 440/405 Instance
                        CEntry = null;
                        PEntry = new ProcessorEntry();
                        PEntry.Instance = string.Empty;
                        PEntry.PowerPC = true;
                        PEntry.CPUNumber = -1;
                        PEntry.DebugBus = string.Empty;
                    }
                    else if (line.EndsWith("END"))
                    {
                        // End of a block
                        if (CEntry != null)
                            _Controllers.Add(CEntry);
                        if (PEntry != null)
                            _Procs.Add(PEntry);
                        CEntry = null;
                        PEntry = null;
                    }
                    else
                    {
                        // Something else 
                        // Middle of a block
                        if (CEntry != null)
                        {
                            int eqIdx = line.IndexOf("=");
                            int spIdx = line.IndexOf(" ");
                            if ((CEntry.PPCController && line.Contains("BUS_INTERFACE JTAGPPC")) || (!CEntry.PPCController && line.Contains("BUS_INTERFACE MBDEBUG")))
                            {
                                DebugBus bus = new DebugBus();
                                bus.BusPort = line.Substring(spIdx + 1, eqIdx - spIdx - 1).Trim();
                                bus.BusSignal = line.Substring(eqIdx + 1).Trim();
                                CEntry.DebugBusList.Add(bus);
                            }
                            else if (line.Contains("PARAMETER INSTANCE"))
                            {
                                CEntry.Instance = line.Substring(eqIdx + 1).Trim();
                            }
                        }
                        else if (PEntry != null)
                        {
                            int eqIdx = line.IndexOf("=");
                            int spIdx = line.IndexOf(" ");
                            if ((PEntry.PowerPC && line.Contains("BUS_INTERFACE JTAGPPC")) || (!PEntry.PowerPC && line.Contains("BUS_INTERFACE DEBUG")))
                            {
                                PEntry.DebugBus = line.Substring(eqIdx + 1).Trim();
                            }
                            else if (line.Contains("PARAMETER INSTANCE"))
                            {
                                PEntry.Instance = line.Substring(eqIdx + 1).Trim();
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                reader.Close();
                #endregion

                #region Traverse Processor and Controller Lists, matching bus ports to identify CPU Numbers
                foreach (ProcessorEntry Proc in _Procs)
                {
                    bool bDone = false;
                    foreach (ControllerEntry Cntrlr in _Controllers)
                    {
                        if (Proc.PowerPC == Cntrlr.PPCController)
                        {
                            foreach (DebugBus bus in Cntrlr.DebugBusList)
                            {
                                if (bus.BusSignal == Proc.DebugBus)
                                {
                                    string PortID = bus.BusPort;
                                    PortID = PortID.Replace("JTAGPPC", string.Empty);
                                    PortID = PortID.Replace("MBDEBUG_", string.Empty);
                                    int cpunr = 0;
                                    if (int.TryParse(PortID, out cpunr))
                                    {
                                        Proc.CPUNumber = cpunr + 1;
                                        bDone = true;
                                        break;
                                    }
                                    else
                                    {
                                        _Builder.RaiseMessageEvent("Unable to parse port number from string: {0} - {1} - Failed", bus.BusPort, PortID);
                                    }
                                }
                            }
                        }
                        if (bDone)
                            break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _Builder.RaiseMessageEvent("Caught in ProcessorEnumerator.ParseMHS()\n\t{0}", ex.Message);
            }
        }

        /// <summary>
        /// Prints out the list of CPUs, as read, to the console.
        /// </summary>
        public void PrintCPUs()
        {
            try 
            {
                foreach (ProcessorEntry Proc in _Procs)
                {
                    _Builder.RaiseMessageEvent(String.Format("{0} #{1}: {2}",
                        (Proc.PowerPC ? "PowerPC" : "Microblaze"),
                        Proc.CPUNumber,
                        Proc.Instance));
                }
            }
            catch (Exception ex)
            {
                _Builder.RaiseMessageEvent("Caught in ProcessorEnumerator.PrintCPUs()\n\t{0}", ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the 'cpunr' of a specific processor instance read from the MHS file, if it exists.  If it does not or an error occurs, a -1 is returned.
        /// </summary>
        /// <param name="ProcessorInstance">The case-sensitive instance name of the processor to be located.</param>
        /// <returns>An positive, non-zero integer indicating the corresponding 'cpunr' of the specified processor instance, or a -1 indicating that the instance was not found or an error occured.</returns>
        public int GetCPUNumber(string ProcessorInstance)
        {
            try 
            {
                foreach (ProcessorEntry Proc in _Procs)
                {
                    if (Proc.Instance == ProcessorInstance)
                        return Proc.CPUNumber;
                }
                return -1;
            }
            catch (Exception ex)
            {
                _Builder.RaiseMessageEvent("Caught in ProcessorEnumerator.PrintCPUs()\n\t{0}", ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Returns the list of processors that were parsed out of the MHS file.
        /// </summary>
        /// <returns></returns>
        public List<ProcessorEntry> GetProcessors()
        {
            return _Procs;
        }


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
    }
}
