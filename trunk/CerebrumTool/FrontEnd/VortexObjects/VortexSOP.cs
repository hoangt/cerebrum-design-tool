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
 * VortexSOP.cs
 * Name: Matthew Cotter
 * Date: 18 Oct 2010 
 * Description: Implementation of IVortexSOP as a Vortex SOP object
 * History: 
 * >> (18 Oct 2010) Matthew Cotter: Defined implementation of IVortexSOP interface corresponding to the current version of Vortex Router SOP
 * >> (18 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VortexInterfaces;
using FalconResources;
using VortexInterfaces.VortexCommon;

namespace VortexObjects
{
    /// <summary>
    /// Implementation of IVortexSOP interface to represent a Vortex SOP object
    /// </summary>
    public class VortexSOP : IVortexSOP
    {
        private string _CommandSource;
        private Vortex_SOP_NIF _NIF;
        private int _Port;
        private int _TDA;
        private List<Flow> _Flows;
        private IVortex _AttachedTo;

        /// <summary>
        /// Default constructor.  Creates an unattached SOP with an NIF
        /// </summary>
        public VortexSOP()
        {
            _NIF = new Vortex_SOP_NIF();
            this._CommandSource = string.Empty;
            this._Port = -1;
            this._TDA = -1;
            this._AttachedTo = null;
            _Flows = new List<Flow>();
        }

        #region Properties
        /// <summary>
        /// Core Type name of the SOP
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Native Core Instance name of the SOP
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Instance name of the SOP
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Version Info of the SOP
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Flag indicating whether the attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        public bool ConfiguresVortex
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        /// <summary>
        /// This value should indicate the port the attachment is connected to on a Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        public int Port 
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        public int TDA
        {
            get
            {
                return _TDA;
            }
            set
            {
                _TDA = value;
            }
        }

        /// <summary>
        /// Boolean indicating whether this device is attached to a Vortex switch.
        /// </summary>
        public bool Attached
        {
            get
            {
                return (_AttachedTo != null);
            }
        }

        /// <summary>
        /// Boolean indicating whether the SOP is configurable (accepting configuration information)
        /// </summary>
        public bool IsConfigurable { get; set; }

        /// <summary>
        /// Returns an object representing the NIF this SOP uses to communicate with a Vortex switch.
        /// </summary>
        public IVortexNIF NIF 
        {
            get
            {
                return _NIF;
            }
        }
        #endregion

        /// <summary>
        /// Sets the path to a file specifying a Falcon Language command sequence to be utilized by the SOP.
        /// </summary>
        /// <param name="FilePath">The local file path containing the command sequence.</param>
        public void SetCommandSource(string FilePath)
        {
            _CommandSource = FilePath;
        }

        /// <summary>
        /// Returns the path to a file specifying a Falcon Language command sequence to be utilized by the SOP.
        /// </summary>
        public string GetCommandSource()
        {
            return _CommandSource;
        }

        /// <summary>
        /// Returns the path to a file containing configuration information for the SOP.
        /// </summary>
        public string GetSOPConfigs()
        {
            return string.Empty;
        }



        /// <summary>
        /// Assigns the specified flow to this SOP.
        /// </summary>
        /// <param name="f">The Flow object representing the flow assigned to the SOP.</param>
        public void AddFlow(Flow f)
        {
            if (!_Flows.Contains(f))
            {
                _Flows.Add(f);
            }
        }
        /// <summary>
        /// Removes the specified flow from this SOP.
        /// </summary>
        /// <param name="f">The Flow object representing the flow removed from the SOP.</param>
        public void RemoveFlow(Flow f)
        {
            if (_Flows.Contains(f))
            {
                _Flows.Remove(f);
            }
        }
        /// <summary>
        /// Removes the specified flow from this SOP.
        /// </summary>
        /// <param name="FlowID">The ID of the flow to be removed from the SOP.</param>
        public void RemoveFlow(int FlowID)
        {
            foreach (Flow f in _Flows)
            {
                if (f.ID == FlowID)
                {
                    _Flows.Remove(f);
                }
            }
        }

        /// <summary>
        /// Gets a list of all flows currently assigned to the SOP.
        /// </summary>
        /// <returns>A List of all Flow objects representing flows assigned to the SOP</returns>
        public List<Flow> GetFlows()
        {
            return _Flows;
        }

        /// <summary>
        /// Get or set the IVortex object to which the SOP is attached, if any.
        /// </summary>
        public IVortex AttachedTo 
        {
            get
            {
                return _AttachedTo;
            }
            set
            {
                _AttachedTo = value;
            }
        }
    }
}
