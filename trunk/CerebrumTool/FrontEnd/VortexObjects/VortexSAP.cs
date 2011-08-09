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
 * VortexSAP.cs
 * Name: Matthew Cotter
 * Date: 18 Oct 2010 
 * Description: Implementation of IVortexSAP as a Vortex SAP object
 * History: 
 * >> (18 Oct 2010) Matthew Cotter: Defined implementation of IVortexSAP interface corresponding to the current version of Vortex Router SAP
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
    /// Implementation of IVortexSAP interface to represent a Vortex SAP object
    /// </summary>
    public class VortexSAP : IVortexSAP
    {
        private bool _ConfiguresVortex;
        private string _CommandSource;
        private Vortex_SAP_NIF _NIF;
        private int _Port;
        private int _TDA;
        private IVortex _AttachedTo;
        /// <summary>
        /// Default constructor.  Creates an unattached SAP with an NIF
        /// </summary>
        public VortexSAP()
        {
            _NIF = new Vortex_SAP_NIF();
            this._CommandSource = string.Empty;
            this._Port = -1;
            this._TDA = -1;
            this._AttachedTo = null;
            this._ConfiguresVortex = false;
        }

        #region Properties

        /// <summary>
        /// Core Type name of the SAP
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Native Core Instance name of the SAP
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Instance name of the SAP
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Version Info of the SAP
        /// </summary>
        public string Version { get; set; }

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
        /// Boolean indicating whether the SAP is configurable (accepting configuration information)
        /// </summary>
        public bool IsConfigurable { get; set; }

        /// <summary>
        /// Returns an object representing the NIF this SAP uses to communicate with a Vortex switch.
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
        /// Sets the path to a file specifying a Falcon Language command sequence to be utilized by the SAP.
        /// </summary>
        /// <param name="FilePath">The local file path containing the command sequence.</param>
        public void SetCommandSource(string FilePath)
        {
            _CommandSource = FilePath;
        }

        /// <summary>
        /// Returns the path to a file specifying a Falcon Language command sequence to be utilized by the SAP.
        /// </summary>
        public string GetCommandSource()
        {
            return _CommandSource;
        }

        /// <summary>
        /// Returns the path to a file containing configuration information for the SAP.
        /// </summary>
        public string GetSAPConfigs()
        {
            return string.Empty;
        }

        /// <summary>
        /// Get or set the IVortex object to which the SAP is attached, if any.
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
