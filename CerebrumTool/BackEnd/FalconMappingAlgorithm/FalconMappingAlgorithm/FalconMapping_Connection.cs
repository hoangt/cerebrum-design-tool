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
 * FalconMapping_Connection.cs
 * Name: Matthew Cotter
 * Date: 2 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a logical inter-Component connection for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> (16 Dec 2010) Matthew Cotter: Added connection properties to reflect not only the source/sink component, but source/sink subcores as well in support of cores
 *                                      with multiple interfaces.
 * >> ( 1 Dec 2010) Matthew Cotter: Integration of Multiple-SAP Components into mapping.
 *                                  Added support to distinguish the instance name of the component core sourcing or sinking the connection.
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> ( 7 Jun 2010) Matthew Cotter: Corrected calculation of normalization factor in DataDensity property
 * >> (15 Apr 2010) Matthew Cotter: Updated/Corrected Functions to normalize data density
 * >> ( 2 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconMappingAlgorithm
{

    /// <summary>
    /// Represents a logical inter-component connection between two components in a design.
    /// </summary>
    public class FalconMapping_Connection
    {
        private string strID;
        private string strName;
        private double fDataDensity;
        private double fNormalFactor;

        private string strSourceComponent;
        private string strSinkComponent;

        /// <summary>
        /// Creates a new FalconMapping_Connection object representing a connection between two 
        /// components within a design.
        /// </summary>
        /// <param name="sID">The ID of the new connection</param>
        /// <param name="sSourceComponent">The source component ID for the new connection</param>
        /// <param name="sSinkComponent">The sink component ID for the new connection</param>
        public FalconMapping_Connection(string sID, string sSourceComponent, string sSinkComponent)
        {
            strID = sID;
            strName = (sSourceComponent + "->" + sSinkComponent);
            fDataDensity = 1.0F;
            fNormalFactor = 1.0F;

            strSourceComponent = sSourceComponent;
            strSinkComponent = sSinkComponent;
        }

        /// <summary>
        /// Get or set the ID of this connection.
        /// Note: This write mode of this property is provided only for ID re-assignment.  Changing this value without removing/re-adding it to
        /// any collections based on this ID can have unexpected or unknown results.
        /// </summary>
        public string ID
        {
            get
            {
                return strID;
            }
            set
            {
                strID = value;
            }
        }

        /// <summary>
        /// Get or set the name of this connection.
        /// </summary>
        public string Name
        {
            get
            {
                return strName;
            }
            set
            {
                strName = value;
            }
        }

        /// <summary>
        /// Get or set the density of the data sent across the connection.  NormalizedDataDensity is based 
        /// on this value.  If changed, a new normalization factor is computed based on the old value and
        /// old normalization factor. Regardless, after any write to this value, NormalizeDataDensityTo() 
        /// should be called.
        /// </summary>
        public double DataDensity
        {
            get
            {
                return fDataDensity;
            }
            set
            {
                fNormalFactor = (fNormalFactor * (fDataDensity / value));
                fDataDensity = value;
            }
        }

        /// <summary>
        /// Get or set the ID of the source component for this connection.
        /// </summary>
        public string SourceComponent
        {
            get
            {
                return strSourceComponent;
            }
            set
            {
                strSourceComponent = value;
            }
        }
        /// <summary>
        /// Get or set the Instance of the source (sub)component for this connection.
        /// </summary>
        public string SourceComponentInstance { get; set; }

        /// <summary>
        /// Get or set the ID of the sink component for this connection.
        /// </summary>
        public string SinkComponent
        {
            get
            {
                return strSinkComponent;
            }
            set
            {
                strSinkComponent = value;
            }
        }
        /// <summary>
        /// Get or set the Instance of the sink (sub)component for this connection.
        /// </summary>
        public string SinkComponentInstance { get; set; }

        /// <summary>
        /// Read-Only.  Returns the current normalization factor used in calculating NormalizedDataDensity.
        /// This value is set directly via call to NormalizeDataDensityTo() or indirectly 
        /// through a write to the DataDensity.
        /// </summary>
        public double NormalizationFactor
        {
            get
            {
                return fNormalFactor;
            }
        }

        /// <summary>
        /// Read-Only.  Returns the normalized data density, relative to the least dense connection
        /// in the design, which is set by NormalizeDataDensityTo.
        /// </summary>
        public double NormalizedDataDensity
        {
            get
            {
                return (fNormalFactor);
            }
        }

        /// <summary>
        /// Sets the NormalizationFactor of this link, based on supplied data density, which should be the 
        /// lowest in the system.
        /// </summary>
        /// <param name="MinDataDensity">The density of the least dense data connection.</param>
        public void NormalizeDataDensityTo(double MinDataDensity)
        {
            fNormalFactor = (fDataDensity / MinDataDensity);
        }
    }
}
