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
 * VortexRouter.cs
 * Name: Matthew Cotter
 * Date: 18 Oct 2010 
 * Description: Implementation of IVortex as a Vortex object
 * History: 
 * >> ( 9 May 2010) Matthew Cotter: Corrected bug in generation of router ID and TDAs from shifting Bus and Switch IDs.
 * >> (22 Mar 2011) Matthew Cotter: Added configuration attachment property for use in determining what attachment, if any, is to be used for configuration.
 * >> (25 Jan 2010) Matthew Cotter: Modified AttachedDevices property to return the list in order, sorted by port number
 * >> (18 Oct 2010) Matthew Cotter: Changed automatic generation of attached-device NIF instances
 * >> (18 Oct 2010) Matthew Cotter: Defined implementation of IVortex interface corresponding to the current version of Vortex Router
 * >> (18 Oct 2010) Matthew Cotter: Source file created -- Initial version.
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
    /// Implementation of IVortex interface to represent a Vortex Router object
    /// </summary>
    public class VortexRouter : IVortex
    {
        //private bool _IsConfigurable;
        private IVortexAttachment _ConfigDevice;
        private int _BusID;
        private int _SwitchID;
        private int _NumPorts;

        private Dictionary<int, IVortexAttachment> _AttachedDevices;

        /// <summary>
        /// Constructor used to initialize the vortex router with the specified bus ID and switch ID
        /// </summary>
        /// <param name="Bus">The ID of the bus identifying the vortex</param>
        /// <param name="Switch">The ID of the switch identifying the vortex</param>
        public VortexRouter(int Bus, int Switch)
        {
            this._BusID = Bus;
            this._SwitchID = Switch;
            this._NumPorts = 8;
            this._ConfigDevice = null;

            _AttachedDevices = new Dictionary<int, IVortexAttachment>();
        }

        #region Properties

        /// <summary>
        /// Core Type name of the router
        /// </summary>
        public string Type
        {
            get
            {
                return "vortex_router";
            }
        }
        /// <summary>
        /// Instance name of the router
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Version Info of the router
        /// </summary>
        public string Version
        {
            get
            {
                return "1.00.c";
            }
        }

        /// <summary>
        /// This value should indicate the Local Switch ID of the Vortex Router.
        /// </summary>
        public int LocalSwitchID 
        {
            get
            {
                return (BusID << 6) | (SwitchID << 3);
            }
            set
            {
            }
        }

        /// <summary>
        /// Boolean indicating whether the Vortex switch is configurable (enabling its configuration interface)
        /// </summary>
        public bool IsConfigurable 
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// Get or set the attachment to be used for configuring the Vortex.
        /// </summary>
        public IVortexAttachment ConfigurationAttachment
        {
            get
            {
                return _ConfigDevice;
            }
            set
            {
                _ConfigDevice = value;
            }
        }


        /// <summary>
        /// Indicates the Bus ID of the Vortex switch
        /// </summary>
        public int BusID 
        {
            get
            {
                return _BusID;
            }
        }
        /// <summary>
        /// Indicates the Switch ID of the Vortex switch
        /// </summary>
        public int SwitchID
        {
            get
            {
                return _SwitchID;
            }
        }
        /// <summary>
        /// Indicates the number of ports on the Vortex switch
        /// </summary>
        public int NumPorts 
        {
            get
            {
                return _NumPorts;
            }
        }
        /// <summary>
        /// Indicates the number of currently unattached ports on the Vortex switch
        /// </summary>
        public int AvailablePorts
        {
            get
            {
                return _NumPorts - this.AttachedDevices.Count;
            }
        }

        /// <summary>
        /// List of devices that have been attached to the Vortex switch
        /// </summary>
        public List<IVortexAttachment> AttachedDevices
        {
            get
            {
                List<IVortexAttachment> _Devices = new List<IVortexAttachment>();
                for (int p = 0; p < NumPorts; p++)
                {
                    if (_AttachedDevices.ContainsKey(p))
                        _Devices.Add(_AttachedDevices[p]);
                }
                return _Devices;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the set of required resources to synthesize the Vortex switch in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public ResourceInfo GetResources()
        {
            int UsedPorts = this.NumPorts - this.AvailablePorts;
            return VortexRouter.GetResources(UsedPorts);
        }
        /// <summary>
        /// Returns the set of required resources to synthesize the Vortex switch in hardware, given that its using the specified number of ports.
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public static ResourceInfo GetResources(int UsedPorts)
        {
            ResourceInfo RI = new ResourceInfo();
            switch (UsedPorts)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    RI.SetResource("Slice Registers", 10049);
                    RI.SetResource("Slice LUTs", 17037);
                    RI.SetResource("BRAMs", 65);
                    break;
                case 3:
                    RI.SetResource("Slice Registers", 11698);
                    RI.SetResource("Slice LUTs", 20513);
                    RI.SetResource("BRAMs", 81);
                    break;
                case 4:
                    RI.SetResource("Slice Registers", 13349);
                    RI.SetResource("Slice LUTs", 23937);
                    RI.SetResource("BRAMs", 97);
                    break;
                case 5:
                    RI.SetResource("Slice Registers", 14998);
                    RI.SetResource("Slice LUTs", 27471);
                    RI.SetResource("BRAMs", 113);
                    break;
                case 6:
                    RI.SetResource("Slice Registers", 16647);
                    RI.SetResource("Slice LUTs", 31005);
                    RI.SetResource("BRAMs", 129);
                    break;
                case 7:
                    RI.SetResource("Slice Registers", 18296);
                    RI.SetResource("Slice LUTs", 34593);
                    RI.SetResource("BRAMs", 145);
                    break;
                case 8:
                    RI.SetResource("Slice Registers", 19950);
                    RI.SetResource("Slice LUTs", 37998);
                    RI.SetResource("BRAMs", 160);
                    break;
                default:
                    break;
            }
            return RI;
        }
        
        /// <summary>
        /// Attaches the specified IVortexAttachment to the Vortex switch, if it is not already attached.
        /// </summary>
        /// <param name="Device">The IVortexAttachment to attach.</param>
        public void AttachDevice(IVortexAttachment Device)
        {
            if (Device == null)
            {
                // ERROR!
                return;
            }
            if ((this.AvailablePorts > 0) && (!_AttachedDevices.ContainsValue(Device)) && (!Device.Attached))
            {
                int p = -1;
                for (int i = 0; i < this.NumPorts; i++)
                {
                    if (!_AttachedDevices.ContainsKey(i))
                    {
                        p = i;
                        break;
                    }
                }
                if (p < 0)
                {
                    // ERROR!
                }
                Device.Port = p;
                Device.TDA = GenerateTDA(this.BusID, this.SwitchID, Device.Port);
                if (Device.NIF != null)
                {
                    Device.NIF.Instance = String.Format("{0}_{1}", Device.Instance, Device.NIF.Type);
                }
                _AttachedDevices.Add(p, Device);
                Device.AttachedTo = this;

                if ((Device.ConfiguresVortex) && (this.ConfigurationAttachment == null))
                {
                    this.ConfigurationAttachment = Device;
                }
            }
        }
        /// <summary>
        /// Detaches the specified IVortexAttachment from the Vortex switch, if it is attached.
        /// </summary>
        /// <param name="Device">The IVortexAttachment to detach.</param>
        public void DetachDevice(IVortexAttachment Device)
        {
            if (_AttachedDevices.ContainsValue(Device) && (Device.Attached))
            {
                _AttachedDevices.Remove(Device.Port);
                Device.AttachedTo = null;
                Device.Port = -1;
                Device.TDA = -1;
                if (Device.NIF != null)
                {
                    Device.NIF.Instance = string.Empty;
                }

                if ((Device.ConfiguresVortex) && (this.ConfigurationAttachment == Device))
                {
                    this.ConfigurationAttachment = null;
                    foreach (IVortexAttachment Attachment in this._AttachedDevices.Values)
                    {
                        if (Attachment.ConfiguresVortex)
                        {
                            this.ConfigurationAttachment = Attachment;
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        private int GenerateTDA(int Bus, int Switch, int Port)
        {
            int result = 0;
            result = (Bus << 8) | (Switch << 3) | Port;
            return result;
        }
    }
}
