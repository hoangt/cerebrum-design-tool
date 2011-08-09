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
 * VortexCommon.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: General objects, constants and enumerations used as part of Vortex router integration.
 * History: 
 * >> (18 Oct 2010) Matthew Cotter: Added enumeration of Vortex attachment type.
 * >> ( 7 Oct 2010) Matthew Cotter: Created generic Flow class and associated constants.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VortexInterfaces
{
    namespace VortexCommon
    {
        /// <summary>
        /// Contains constants related to Flow management
        /// </summary>
        public static class FlowConstants
        {
            /// <summary>
            /// Flow ID to be used for setting a null Flow
            /// </summary>
            public const long FLOW_ID_NONE = 0;
            /// <summary>
            /// Flow ID to be used for setting a configuration Flow
            /// </summary>
            public const long FLOW_ID_CONFIG = 18;

            /// <summary>
            /// TDA used to indicate that the specified hop (Next or Previous) is null or non-existant.
            /// </summary>
            public const long HOP_TDA_NONE = -1;
        }


        /// <summary>
        /// Enumerates the types of vortex attachments
        /// </summary>
        public enum VortexAttachmentType
        {
            /// <summary>
            /// Default invalid attachment type.
            /// </summary>
            None = 0,
            /// <summary>
            /// The SAP attachment type
            /// </summary>
            SAP = 1,
            /// <summary>
            /// The SOP attachment type
            /// </summary>
            SOP = 2,
            /// <summary>
            /// The Vortex-to-Vortex Bridge attachment type
            /// </summary>
            VortexBridge = 3,
            /// <summary>
            /// The Vortex-to-Edge Bridge attachment type
            /// </summary>
            VortexEdge = 4
        }


        /// <summary>
        /// Represents a data "Flow" in the Vortex communication architecture.
        /// </summary>
        public class Flow
        {
            private long _FlowID;

            /// <summary>
            /// Creates a representation of a new Flow, with the specified Flow ID
            /// </summary>
            /// <param name="ID">The ID assigned to the new flow.</param>
            public Flow(long ID)
            {
                _FlowID = ID;
            }
            /// <summary>
            /// Creates a representation of a new Flow, with the specified Flow ID, as well as next/previous hop information
            /// </summary>
            /// <param name="ID">The ID assigned to the new flow.</param>
            /// <param name="NextHop">The TDA of the next device in the flow.   If there is no next device, this value should be 
            /// equal to FlowConstants.HOP_TDA_NONE.</param>
            /// <param name="PreviousHop">The TDA of the previous device in the flow.   If there is no previous device, this value should be 
            /// equal to FlowConstants.HOP_TDA_NONE.</param>
            public Flow(long ID, long NextHop, long PreviousHop)
            {
                _FlowID = ID;
                this.NextHopTDA = NextHop;
                this.PreviousHopTDA = PreviousHop;
            }

            /// <summary>
            /// The ID assigned to the flow.
            /// </summary>
            public long ID
            {
                get
                {
                    return _FlowID;
                }
            }
            /// <summary>
            /// The TDA of the device which is the next hop in the flow, from the device to which this flow is assigned.  If this device
            /// is the last in the flow, this value should be negative.
            /// </summary>
            public long NextHopTDA { get; set; }
            /// <summary>
            /// The TDA of the device from which this flow reaches the device to which this flow is assigned.   If the device is the first
            /// in the flow, this value should be negative.
            /// </summary>
            public long PreviousHopTDA { get; set; }
        }
    }
}
