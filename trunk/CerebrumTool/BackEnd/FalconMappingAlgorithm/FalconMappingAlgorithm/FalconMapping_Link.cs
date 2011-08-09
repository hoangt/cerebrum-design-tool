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
 * FalconMapping_Link.cs
 * Name: Matthew Cotter
 * Date: 2 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a physical inter-FPGA link for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> (28 Jan 2011) Matthew Cotter: Began work on supporting/defining Links that are backed by Vortex-based hardware cores in the platform.
 *                                  Added Link properties to specify the component and ingress/egress cores that provide hardware backing for a link, on both sides.
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> ( 7 Jun 2010) Matthew Cotter: Corrected calculation of normalization factor in LinkSpeed property
 * >> (15 Apr 2010) Matthew Cotter: Updated/Corrected Functions to Normalize Link Speed
 * >> ( 2 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Represents a physical Link between two FPGAs within a platform.  
    /// </summary>
    public class FalconMapping_Link
    {
        private string strID;
        private string strName;

        private double fLinkUsage;
        private double fLinkSpeed;
        private double fNormalFactor;

        private string strSourceFPGA;
        private string strSourceComponent;
        private string strSourceIngressCore;
        private string strSourceEgressCore;
        private string strSinkFPGA;
        private string strSinkComponent;
        private string strSinkIngressCore;
        private string strSinkEgressCore;
        private bool bBidir;


        private ArrayList alConnections;

        /// <summary>
        /// Creates a new FalconMapping_Link object, representing a physical link between two FPGAs.
        /// </summary>
        /// <param name="sID">The ID of the new Link</param>
        /// <param name="sSourceFPGA">The source FPGA ID for the new Link</param>
        /// <param name="sSinkFPGA">The sink FPGA ID for the new Link.</param>
        /// <param name="bBidirectional">Boolean indicating whether this Link is Bi-directional.</param>
        public FalconMapping_Link(string sID, string sSourceFPGA, string sSinkFPGA, bool bBidirectional)
        {
            this.Link_Constructor(sID, sSourceFPGA, sSinkFPGA, bBidirectional);
        }

        /// <summary>
        /// Creates a new FalconMapping_Link object, representing a physical link between two FPGAs.  
        /// This constructor assumes a bidirectional link.  If a unidirectional link is desired,
        /// use the FalconMapping_Link(string, string, string, bool) constructor, with the bBidirectional 
        /// argument as false.
        /// </summary>
        /// <param name="sID">The ID of the new Link</param>
        /// <param name="sSourceFPGA">The source FPGA ID for the new Link</param>
        /// <param name="sSinkFPGA">The sink FPGA ID for the new Link.</param>
        public FalconMapping_Link(string sID, string sSourceFPGA, string sSinkFPGA)
        {
            this.Link_Constructor(sID, sSourceFPGA, sSinkFPGA, true);
        }

        /// <summary>
        /// Initializes this FalconMapping_Link object; called from the public constructor
        /// </summary>
        /// <param name="sID">The ID of the new Link</param>
        /// <param name="sSourceFPGA">The source FPGA ID for the new Link</param>
        /// <param name="sSinkFPGA">The sink FPGA ID for the new Link.</param>
        /// <param name="bBidirectional">Boolean indicating whether this Link is Bi-directional.</param>
        private void Link_Constructor(string sID, string sSourceFPGA, string sSinkFPGA, bool bBidirectional)
        {
            strID = sID;
            strName = (sSourceFPGA + (bBidirectional ? "<->" : "->") + sSinkFPGA);
            fLinkUsage = 0.0F;
            fLinkSpeed = 1.0F;
            fNormalFactor = 1.0F;

            strSourceFPGA = sSourceFPGA;
            strSinkFPGA = sSinkFPGA;

            bBidir = bBidirectional;
            alConnections = new ArrayList();
        }


        /// <summary>
        /// Get or set the ID of this link.
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
        /// Get or set the name of this link.
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
        /// Get or set whether this link is bidirectional.
        /// </summary>
        public bool Bidirectional
        {
            get
            {
                return bBidir;
            }
            set
            {
                bBidir = value;
            }
        }

        /// <summary>
        /// Get or set the speed of the data sent across the link.  NormalizedLinkSpeed is based 
        /// on this value.  If changed, a new normalization factor is computed based on the old value and
        /// old normalization factor. Regardless, after any write to this value, NormalizeLinkSpeedTo() 
        /// should be called.
        /// </summary>
        public double LinkSpeed
        {
            get
            {
                return fLinkSpeed;
            }
            set
            {
                fNormalFactor = (fNormalFactor * (fLinkSpeed / value));
                fLinkSpeed = value;
            }
        }

        /// <summary>
        /// Read-Only.  Provides an indication of the estimated load on a Link.  The calculation is based on the NormalizedLinkSpeed, 
        /// and associated connections that use this link.
        /// </summary>
        public double LinkUsage
        {
            get
            {
                return (fLinkUsage / fNormalFactor);
            }
        }
        
        /// <summary>
        /// Get or set the ID of the source FPGA for this link.
        /// </summary>
        public string SourceFPGA
        {
            get
            {
                return strSourceFPGA;
            }
            set
            {
                strSourceFPGA = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the component acting as the external I/O on the source FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the "underscored" FPGA ID.
        /// </summary>
        public string SourceComponent
        {
            get
            {
                if ((strSourceComponent != null) && (strSourceComponent != string.Empty) && (strSourceFPGA != null) && (strSourceFPGA != string.Empty))
                    return String.Format("{0}_{1}", strSourceFPGA.Replace(".", "_"), strSourceComponent);
                else
                    return string.Empty;
            }
            set
            {
                strSourceComponent = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the core acting as the "receiver" core on the source FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the 
        /// "underscored" FPGA ID and the native instance of the core's parent component.
        /// </summary>
        public string SourceIngressCore
        {
            get
            {
                if ((this.SourceComponent != string.Empty) && (strSourceIngressCore != null) && (strSourceIngressCore != string.Empty))
                    return String.Format("{0}_{1}", this.SourceComponent, strSourceIngressCore);
                else
                    return string.Empty;
            }
            set
            {
                strSourceIngressCore = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the core acting as the "transmitter" core on the source FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the 
        /// "underscored" FPGA ID and the native instance of the core's parent component.
        /// </summary>
        public string SourceEgressCore
        {
            get
            {
                if ((this.SourceComponent != string.Empty) && (strSourceEgressCore != null) && (strSourceEgressCore != string.Empty))
                    return String.Format("{0}_{1}", this.SourceComponent, strSourceEgressCore);
                else
                    return string.Empty;
            }
            set
            {
                strSourceEgressCore = value;
            }
        }
        
        /// <summary>
        /// Get or set the ID of the sink FPGA for this link.
        /// </summary>
        public string SinkFPGA
        {
            get
            {
                return strSinkFPGA;
            }
            set
            {
                strSinkFPGA = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the component acting as the external I/O on the sink FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the "underscored" FPGA ID.
        /// </summary>
        public string SinkComponent
        {
            get
            {
                if ((strSinkComponent != null) && (strSinkComponent != string.Empty) && (strSinkFPGA != null) && (strSinkFPGA != string.Empty))
                    return String.Format("{0}_{1}", strSinkFPGA.Replace(".", "_"), strSinkComponent);
                else
                    return string.Empty;
            }
            set
            {
                strSinkComponent = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the core acting as the "receiver" core on the sink FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the 
        /// "underscored" FPGA ID and the native instance of the core's parent component.
        /// </summary>
        public string SinkIngressCore
        {
            get
            {
                if ((this.SinkComponent != string.Empty) && (strSinkIngressCore != null) && (strSinkIngressCore != string.Empty))
                    return String.Format("{0}_{1}", this.SinkComponent, strSinkIngressCore);
                else
                    return string.Empty;
            }
            set
            {
                strSinkIngressCore = value;
            }
        }
        /// <summary>
        /// When setting this value, the native instance name of the core acting as the "transmitter" core on the sink FPGA should be specified.
        /// When getting this value, this property will return the full (true) instance name, with the native instance pre-pended with the 
        /// "underscored" FPGA ID and the native instance of the core's parent component.
        /// </summary>
        public string SinkEgressCore
        {
            get
            {
                if ((this.SinkComponent != string.Empty) && (strSinkEgressCore != null) && (strSinkEgressCore != string.Empty))
                    return String.Format("{0}_{1}", this.SinkComponent, strSinkEgressCore);
                else
                    return string.Empty;
            }
            set
            {
                strSinkEgressCore = value;
            }
        }
       
        /// <summary>
        /// Read-Only.  Returns the current normalization factor used in calculating LinkUsage.
        /// This value is set directly via call to NormalizeLinkSpeedTo() or indirectly 
        /// through a write to the LinkSpeed.
        /// </summary>
        public double NormalizationFactor
        {
            get
            {
                return fNormalFactor;
            }
        }

        /// <summary>
        /// Read-Only.  Returns the normalized link speed, relative to the highest speed link
        /// in the system, which is set by NormalizeLinkSpeedTo.
        /// </summary>
        public double NormalizedLinkSpeed
        {
            get
            {
                return (double)(fNormalFactor);
            }
        }
        
        /// <summary>
        /// Sets the NormalizationFactor of this link, based on supplied link speed, which should be the 
        /// fastest in the system.
        /// </summary>
        /// <param name="MaxLinkSpeed">The speed of the fastest link.</param>
        public void NormalizeLinkSpeedTo(double MaxLinkSpeed)
        {
            fNormalFactor = (fLinkSpeed / MaxLinkSpeed);
        }

        /// <summary>
        /// Clear all associated connection densities and reset LinkUsage.
        /// </summary>
        public void ClearAllConnections()
        {
            fLinkUsage = 0.0F;
            alConnections.Clear();
        }

        /// <summary>
        /// Applies the "weight" of the passed FalconMapping_Connection to this link, indicating that the 
        /// connection utilizes this link.  If the specified connection is already using this link, a
        /// ConnectionLinkAllocationException is thrown.
        /// </summary>
        /// <param name="Connection">The FalconMapping_Connection that will be utilizing this link.</param>
        public void AddConnection(FalconMapping_Connection Connection)
        {
            if (!alConnections.Contains(Connection))
            {
                fLinkUsage = fLinkUsage + Connection.DataDensity;
                alConnections.Add(Connection);
            }
            else
            {
                throw new Exceptions.ConnectionLinkAllocationException(
                            String.Format("Unable to apply connection {0} to link {1}.  It has already been applied.",
                            Connection.ID, this.ID));
            }
        }

        /// <summary>
        /// Removes the "weight" of the passed FalconMapping_Connection to this link, indicating that the 
        /// connection no longer utilizes this link. NOTE: Currently assumes that the connection's weight 
        /// has been applied via a corresponding call to AddConnection, but no check to verify this is
        /// currently made.  If removing this connection results in a negative link usage, a ConnectionLinkAllocationException 
        /// will be thrown.
        /// </summary>
        /// <param name="Connection">The FalconMapping_Connection that will no longer be utilizing this link.</param>
        public void RemoveConnection(FalconMapping_Connection Connection)
        {
            if (alConnections.Contains(Connection))
            {
                double newUsage = fLinkUsage - Connection.DataDensity;
                if (newUsage < 0.0)
                {
                    throw new Exceptions.ConnectionLinkAllocationException(
                            String.Format("Unable to remove connection {0} from link {1}.  This would result would be a negative link usage.",
                            Connection.ID, this.ID));
                }
                else
                {
                    fLinkUsage = newUsage;
                    alConnections.Remove(Connection);
                }
            }
        }
    }
}
