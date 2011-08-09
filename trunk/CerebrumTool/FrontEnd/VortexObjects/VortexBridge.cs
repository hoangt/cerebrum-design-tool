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
 * VortexBridge.cs
 * Name: Matthew Cotter
 * Date:  2 Feb 2010 
 * Description: Implementation of IVortexBridge as a Vortex-to-Vortex 'bridge'
 * History: 
 * >> ( 2 Feb 2011) Matthew Cotter: Defined implementation of IVortexBridge interface corresponding to the current version of Vortex Router
 * >> ( 2 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VortexInterfaces;
using FalconResources;

namespace VortexObjects
{
    /// <summary>
    /// Implementation of IVortexBridge interface to represent a Vortex-to-Vortex 'bridge'
    /// </summary>
    public class VortexBridge : IVortexBridge
    {
        private VortexBridgeAttachment Attachment1;
        private VortexBridgeAttachment Attachment2;

        /// <summary>
        /// Default constructor.  Creates an unattached Vortex-to-Vortex 'bridge'.
        /// </summary>
        public VortexBridge()
        {
            Attachment1 = new VortexBridgeAttachment();
            Attachment2 = new VortexBridgeAttachment();
        }

        #region Properties
        
        /// <summary>
        /// NOT USED.  Core Type name of the bridge
        /// </summary>
        public string Type
        {
            get
            {
                return "vortex_interfifo";
            }
            set
            {
            }
        }
        /// <summary>
        /// NOT USED.  Version Info of the bridge
        /// </summary>
        public string Version
        {
            get
            {
                return "1.00.b";
            }
            set
            {
            }
        }

        #region NOT USED IVortexAttachment Properties
        /// <summary>
        /// Flag indicating whether the first attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        public bool ConfiguresVortex
        {
            get
            {
                return Attachment1.ConfiguresVortex;
            }
            set
            {
                Attachment1.ConfiguresVortex = value;
            }
        }
        /// <summary>
        /// NOT USED.  This value should indicate the port the attachment is connected to on the first Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        public int Port
        {
            get
            {
                return Attachment1.Port;
            }
            set
            {
            }
        }
        /// <summary>
        /// NOT USED.  This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the first Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        public int TDA
        {
            get
            {
                return Attachment1.TDA;
            }
            set
            {
            }
        }
        /// <summary>
        /// NOT USED.  Boolean indicating whether this device is attached to the first Vortex switch.
        /// </summary>
        public bool Attached
        {
            get
            {
                return Attachment1.Attached;
            }
        }
        /// <summary>
        /// NOT USED.  Get or set the first IVortex object to which the bridge is attached, if any.
        /// </summary>
        public IVortex AttachedTo
        {
            get
            {
                return Attachment1.AttachedTo;
            }
            set
            {
            }
        }
        #endregion

        /// <summary>
        /// Currently unused. Native Core Instance name of the Bridge
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Identifier that is used to represent the bridge, or more accurately, the signals between the vortexes involved in the bridge.
        /// </summary>
        public string Instance
        {
            get
            {
                if (Bridged)
                {
                    return String.Format("{0}_{1}_bridge", Attachment1.AttachedTo.Instance, Attachment2.AttachedTo.Instance);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
            }
        }


        /// <summary>
        /// Flag indicating whether the first attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        public bool ConfiguresVortex_1
        {
            get
            {
                return Attachment1.ConfiguresVortex;
            }
            set
            {
                Attachment1.ConfiguresVortex = value;
            }
        }
        /// <summary>
        /// This value should indicate the port the attachment is connected to on the first Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        public int Port_1
        {
            get
            {
                return Attachment1.Port;
            }
        }
        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the first Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        public int TDA_1
        {
            get
            {
                return Attachment1.TDA;
            }
        }
        /// <summary>
        /// Flag indicating whether the second attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        public bool ConfiguresVortex_2
        {
            get
            {
                return Attachment2.ConfiguresVortex;
            }
            set
            {
                Attachment2.ConfiguresVortex = value;
            }
        }
        /// <summary>
        /// This value should indicate the port the attachment is connected to on the second Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        public int Port_2
        {
            get
            {
                return Attachment2.Port;
            }
        }
        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the second Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        public int TDA_2
        {
            get
            {
                return Attachment2.TDA;
            }
        }
        /// <summary>
        /// Boolean indicating whether this device is attached to two Vortex routers.
        /// </summary>
        public bool Bridged
        {
            get
            {
                return (Attached_1 && Attached_2);
            }
        }
        /// <summary>
        /// Boolean indicating whether this device is attached to the 'first' Vortex router.
        /// </summary>
        public bool Attached_1
        {
            get
            {
                return (Attachment1.AttachedTo != null);
            }

        }
        /// <summary>
        /// Boolean indicating whether this device is attached to the 'second' Vortex router.
        /// </summary>
        public bool Attached_2
        {
            get
            {
                return (Attachment2.AttachedTo != null);
            }

        }

        /// <summary>
        /// Returns 'null'.  The Vortex-to-Vortex 'bridge' is a direct connection with no NIF.
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
        /// Get the first IVortex object to which the device is attached, if any.
        /// </summary>
        public IVortex AttachedTo_1
        {
            get
            {
                return Attachment1.AttachedTo;
            }
        }
        /// <summary>
        /// Get the second IVortex object to which the device is attached, if any.
        /// </summary>
        public IVortex AttachedTo_2
        {
            get
            {
                return Attachment2.AttachedTo;
            }
        }

        /// <summary>
        /// Returns the set of required resources to synthesize the Bridge in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public ResourceInfo GetResources()
        {
            ResourceInfo RI = new ResourceInfo();

            return RI;
        }

        /// <summary>
        /// Creates a bridge between the two specified Vortex routers, but attaching the corresponding ends to the specified routers.
        /// </summary>
        /// <param name="Vortex1">The 'first' Vortex in the bridge.</param>
        /// <param name="Vortex2">The 'second' Vortex in the bridge.</param>
        public void Bridge(IVortex Vortex1, IVortex Vortex2)
        {
            Vortex1.AttachDevice(Attachment1);
            if (Attachment1.AttachedTo == Vortex1)
            {
                Attachment1.Instance = String.Format("{0}____{1}", Vortex1.Instance, Vortex2.Instance);
                Attachment1.Bridge = this;
            }

            Vortex2.AttachDevice(Attachment2);
            if (Attachment2.AttachedTo == Vortex2)
            {
                Attachment2.Instance = String.Format("{0}____{1}", Vortex2.Instance, Vortex1.Instance);
                Attachment2.Bridge = this;
            }
        }
        /// <summary>
        /// Disconnects both ends of the bridge from any attached vortex routers.
        /// </summary>
        public void Disconnect()
        {
            Attachment1.Bridge = null;
            if (Attached_1)
                Attachment1.AttachedTo.DetachDevice(Attachment1);
            Attachment1.Instance = String.Empty;

            Attachment2.Bridge = null;
            if (Attached_2)
                Attachment2.AttachedTo.DetachDevice(Attachment2);
            Attachment2.Instance = String.Empty;
        }
    }
}
