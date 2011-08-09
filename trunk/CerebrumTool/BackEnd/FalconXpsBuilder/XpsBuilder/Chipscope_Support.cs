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
 * Chipscope_Support.cs
 * Name: Matthew Cotter
 * Date: 26 May 2011 
 * Description: This class implements the functionality necessary to add a Chipscope controller and supporting ILAs to the MHS
 *      file of an XPS Project.
 * History: 
 * >> (26 May 2011): Created basic initial implementation of Chipscope Controller and ILA
 * >> (26 May 2011): Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Defines an object interface representing a ChipScope ILA Controller
    /// </summary>
    public class ChipscopeController
    {
        private class ChipscopeILA
        {
            public ChipscopeILA(string ILAClockSignal, string CoreInstance, string CorePort)
            {
                this.DataSamples = 1024;
                this.Version = "1.03.a";
                this.TriggerUnits = 1;
                this.TriggerWidth = 256;
                this.TriggerMatchType = "basic with edges";
                this.ClockSignalName = ILAClockSignal;

                this.TriggerSignalName = String.Format("{0}_{1}_to_chipscope_ila", CoreInstance, CorePort);
            }

            public string Instance { get; set; }
            public string Version { get; set; }
            public uint DataSamples { get; set; }
            public string ControlSignalName { get; set; }

            public uint TriggerUnits { get; set; }
            public uint TriggerWidth { get; set; }
            public string TriggerMatchType { get; set; }
            public string ClockSignalName { get; set; }
            public string TriggerSignalName { get; set; }


            public StringBuilder GetILABlock()
            {
                StringBuilder ILABlock = new StringBuilder();
                ILABlock.AppendFormat("BEGIN chipscope_ila\n");
                ILABlock.AppendFormat("  PARAMETER INSTANCE = {0}\n", this.Instance);
                ILABlock.AppendFormat("  PARAMETER HW_VER = {0}\n", this.Version);
                ILABlock.AppendFormat("  PORT chipscope_ila_control = {0}\n", this.ControlSignalName);
                ILABlock.AppendFormat("  PORT CLK = {0}\n", this.ClockSignalName);
                ILABlock.AppendFormat("  PARAMETER C_TRIG0_TRIGGER_IN_WIDTH = {0}\n", this.TriggerWidth);
                ILABlock.AppendFormat("  PARAMETER C_TRIG0_UNITS = {0}\n", this.TriggerUnits);
                ILABlock.AppendFormat("  PARAMETER C_TRIG0_UNIT_MATCH_TYPE = {0}\n", this.TriggerMatchType);
                ILABlock.AppendFormat("  PORT TRIG0 = {0}\n", this.TriggerSignalName);
                ILABlock.AppendFormat("END\n");
                return ILABlock;
            }
        }

        private Dictionary<string, ChipscopeILA> _ControlledILAs;
        
        /// <summary>
        /// Constructor to create and initialize the Chipscope Controller object
        /// </summary>
        /// <param name="Instance">The instance name assigned to the controller</param>
        public ChipscopeController(string Instance)
        {
            this.Instance = Instance;
            this.Version = "1.04.a";

            this._ControlledILAs = new Dictionary<string,ChipscopeILA>();
        }

        /// <summary>
        /// Instance name to be assigned to the controller
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Hardware version of the controller block
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Number of control ports in-use on the controller
        /// </summary>
        public int NumControlPorts 
        { 
            get
            {
                return _ControlledILAs.Count;
            }
        }
        /// <summary>
        /// Adds a new ILA to monitor the specified port of the specified core, using the given clock signal name.
        /// </summary>
        /// <param name="CoreInstance">The instance name of the core to be scoped</param>
        /// <param name="CorePort">The port name on the core to be scoped</param>
        /// <param name="ILAClockSignal">The clock signal to use for sampling the scoped port</param>
        /// <returns>The name of the signal to be tied to the Chipscope port on the core.</returns>
        public string AddILA(string CoreInstance, string CorePort, string ILAClockSignal)
        {
            ChipscopeILA NewILA = new ChipscopeILA(ILAClockSignal, CoreInstance, CorePort);
            RemoveILA(CoreInstance, CorePort);
            _ControlledILAs.Add(NewILA.TriggerSignalName, NewILA);
            return NewILA.TriggerSignalName;
        }
        /// <summary>
        /// Removes an ILA from monitoring the specified port of the specified core, using the given clock signal name.
        /// </summary>
        /// <param name="CoreInstance">The instance name of the core to no longer be scoped</param>
        /// <param name="CorePort">The port name on the core to no longer be scoped</param>
        public void RemoveILA(string CoreInstance, string CorePort)
        {
            string TriggerSignalName = String.Format("{0}_{1}_to_chipscope_ila", CoreInstance, CorePort);
            if (_ControlledILAs.ContainsKey(TriggerSignalName))
            {
                _ControlledILAs.Remove(TriggerSignalName);
            }
        }

        /// <summary>
        /// Gets the MHS Project blocks representing the configurations of the Chipscope controller and any associated ILAs.
        /// </summary>
        /// <returns>A StringBuilder object containing the required configuration.</returns>
        public StringBuilder GetChipscopeBlocks()
        {
            StringBuilder CSBlock = new StringBuilder();
            CSBlock.AppendFormat("BEGIN chipscope_icon\n");
            CSBlock.AppendFormat("  PARAMETER INSTANCE = {0}\n", this.Instance);
            CSBlock.AppendFormat("  PARAMETER HW_VER = {0}\n", this.Version);
            CSBlock.AppendFormat("  PARAMETER C_NUM_CONTROL_PORTS = {0}\n", this.NumControlPorts);
            
            List<ChipscopeILA> Scopes = new List<ChipscopeILA>();
            Scopes.AddRange(this._ControlledILAs.Values);
            for(int scope = 0; scope < Scopes.Count; scope++)
            {
                Scopes[scope].Instance = String.Format("{0}_ila_{1}", this.Instance, scope);
                Scopes[scope].ControlSignalName = String.Format("{1}_control{0}", scope, this.Instance);

                CSBlock.AppendFormat("  PORT control{0} = {1}\n", scope, Scopes[scope].ControlSignalName);
            }
            CSBlock.AppendFormat("END\n");

            for (int scope = 0; scope < Scopes.Count; scope++)
            {
                CSBlock.Append(String.Format("{0}\n", Scopes[scope].GetILABlock().ToString()));
            }
            return CSBlock;
        }
    }

}
