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
 * VortexEdgeAttachment.cs
 * Name: Matthew Cotter
 * Date:  2 Feb 2010 
 * Description: Implementation of IVortexEdgeAttachment as a Vortex-to-Vortex bridge 'end-cap'
 * History: 
 * >> ( 2 Feb 2011) Matthew Cotter: Defined implementation of IVortexEdgeAttachment interface corresponding to the current version of Vortex Router
 * >> ( 2 Feb 2011) Matthew Cotter: Source file created -- Initial version.
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
    /// Implementation of IVortexEdgeAttachment interface to represent a Vortex Edge Attachment object, by emulating an SOP.
    /// </summary>
    public class VortexEdgeAttachment : IVortexEdgeAttachment
    {
        private bool _ConfiguresVortex;
        private int _Port;
        private int _TDA;
        private IVortex _AttachedTo;
        private string _Instance;

        /// <summary>
        /// Default constructor.  Creates an unattached Edge Attachment with an NIF
        /// </summary>
        public VortexEdgeAttachment()
        {
            this._Port = -1;
            this._TDA = -1;
            this._AttachedTo = null;
            this._ConfiguresVortex = true;
        }

        #region Properties
        /// <summary>
        /// Core Type name of the Edge Attachment
        /// </summary>
        public string Type
        {
            get
            {
                return "vortex_interlink";
            }
            set
            {
            }
        }
        /// <summary>
        /// Native Core Instance name of the Edge Attachment
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Instance name of the Edge Attachment
        /// </summary>
        public string Instance
        {
            get
            {
                if (this.EdgeComponent != null)
                    return _Instance;
                else
                    return string.Empty;
            }
            set
            {
                _Instance = value;
            }
        }
        /// <summary>
        /// Version Info of the Edge Attachment
        /// </summary>
        public string Version
        {
            get
            {
                return "1.00.c";
            }
            set
            {
            }
        }

        /// <summary>
        /// Flag indicating whether the attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        public bool ConfiguresVortex
        {
            get
            {
                return _ConfiguresVortex;
            }
            set
            {
                _ConfiguresVortex = value;
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
        /// Get or set the Edge Component object to which this attachment is connected.
        /// </summary>
        public object EdgeComponent { get; set; }

        /// <summary>
        /// Returns an object representing the NIF this SOP uses to communicate with a Vortex switch.
        /// </summary>
        public IVortexNIF NIF
        {
            get
            {
                return null;
            }
        }
        #endregion

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
        
        /// <summary>
        /// Returns the set of required resources to synthesize the Edge Attachment in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public ResourceInfo GetResources()
        {
            ResourceInfo RI = new ResourceInfo();

            RI.SetResource("Slice Registers", 201);
            RI.SetResource("Slice LUTs", 225);

            return RI;
        }
    }
}

