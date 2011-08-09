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
 * FalconMapping_Exceptions.cs
 * Name: Matthew Cotter
 * Date:  6 May 2010 
 * Description: These classes represent the exceptional situations and conditions that arise throughout the loading, manipulation, and 
 * storing of the mapping system objects.
 * History: 
 * >> (21 Sep 2010) Matthew Cotter: Created MappingException as over-arching Exception class.
 * >> (12 May 2010) Matthew Cotter: Added XML Commenting and documentation to Exceptions and Classes.
 *                                  Updated Exception Constructors and Constructor calls throughout project to accept a string message.
 *                                  Moved Exceptions to FalconMappingAlgorithm.Exceptions Namespace for ease of access
 * >> ( 6 May 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconMappingAlgorithm.Exceptions
{
    /// <summary>
    /// Generic class of Mapping Exceptions, all Algorithm-Specific exceptions will inherit from this.
    /// </summary>
    public abstract class MappingException : System.Exception
    {
        /// <summary>
        /// Public constructor for MappingException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public MappingException(string message) : base(message) { }
    }

    #region Resource-Related Exceptions
    /// <summary>
    /// Exception thrown when attemping to allocate a group to an FPGA when there are 
    /// insufficient resources available.
    /// </summary>
    public class InsufficientResourcesException : MappingException
    {
        /// <summary>
        /// Public constructor for InsufficientResourcesException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public InsufficientResourcesException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when allocation or deallocation of resources would result in an impossible state.
    /// </summary>
    public class ResourceAllocationException : MappingException
    {
        /// <summary>
        /// Public constructor for ResourceAllocationException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ResourceAllocationException(string message) : base(message) { }
    }
    #endregion

    #region Component-Related Exceptions
    /// <summary>
    /// Exception thrown when attempting to add a component to a group when it is already
    /// a member of a group.
    /// </summary>
    public class ComponentAlreadyGroupedException : MappingException
    {
        /// <summary>
        /// Public constructor for ComponentAlreadyGroupedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ComponentAlreadyGroupedException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to un-group a component from a group when it is not
    /// already a member of a group.
    /// </summary>
    public class ComponentNotGroupedException : MappingException
    {
        /// <summary>
        /// Public constructor for ComponentNotGroupedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ComponentNotGroupedException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when no components were able to be read from the input file
    /// </summary>
    public class NoComponentsReadException : MappingException
    {
        /// <summary>
        /// Public constructor for NoComponentsReadException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public NoComponentsReadException(string message) : base(message) { }
    }
    #endregion

    #region FPGA-Related Exceptions
    /// <summary>
    /// Exception thrown when attempting to add an FPGA to a cluster when it is already
    /// a member of a cluster.
    /// </summary>
    public class FPGAAlreadyClusteredException : MappingException
    {
        /// <summary>
        /// Public constructor for FPGAAlreadyClusteredException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public FPGAAlreadyClusteredException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to un-cluster an FPGA from a cluster when it is not
    /// already a member of a cluster.
    /// </summary>
    public class FPGANotClusteredException : MappingException
    {
        /// <summary>
        /// Public constructor for FPGANotClusteredException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public FPGANotClusteredException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when no FPGAs were able to be read from the input file
    /// </summary>
    public class NoFPGAsReadException : MappingException
    {
        /// <summary>
        /// Public constructor for NoFPGAsReadException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public NoFPGAsReadException(string message) : base(message) { }
    }
    #endregion

    #region Group-Related Exceptions
    /// <summary>
    /// Exception thrown when attempting to map a Group that is already mapped.
    /// </summary>
    public class GroupAlreadyMappedException : MappingException
    {
        /// <summary>
        /// Public constructor for GroupAlreadyMappedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public GroupAlreadyMappedException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to un-map a Group that is not already mapped.
    /// </summary>
    public class GroupNotMappedException : MappingException
    {
        /// <summary>
        /// Public constructor for GroupNotMappedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public GroupNotMappedException(string message) : base(message) { }
    }
    #endregion

    #region Cluster-Related Exceptions
    #endregion

    #region Connection-Related Exceptions
    /// <summary>
    /// Exception thrown when no connections were able to be read from the input file
    /// </summary>
    public class NoConnectionsReadException : MappingException
    {
        /// <summary>
        /// Public constructor for NoConnectionsReadException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public NoConnectionsReadException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to create a new connection with an invalid (non-postive) value for Data Density
    /// </summary>
    public class InvalidConnectionDensityException : MappingException
    {
        /// <summary>
        /// Public constructor for InvalidConnectionDensityException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public InvalidConnectionDensityException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when attempting to create a new connection with an invalid Source Component ID
    /// </summary>
    public class ConnectionSourceDoesNotExistException : MappingException
    {
        /// <summary>
        /// Public constructor for ConnectionSourceDoesNotExistException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ConnectionSourceDoesNotExistException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when attempting to create a new connection with an invalid Sink Component ID
    /// </summary>
    public class ConnectionSinkDoesNotExistException : MappingException
    {
        /// <summary>
        /// Public constructor for ConnectionSinkDoesNotExistException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ConnectionSinkDoesNotExistException(string message) : base(message) { }
    }
    #endregion

    #region Link-Related Exceptions
    /// <summary>
    /// Exception thrown when no links were able to be read from the input file
    /// </summary>
    public class NoLinksReadException : MappingException
    {
        /// <summary>
        /// Public constructor for NoLinksReadException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public NoLinksReadException(string message) : base(message) { }
    }
     /// <summary>
    /// Exception thrown when attempting to allocate a Connection to the same link multiple times, 
    /// or when removing a connection from a link that results in an impossible state.
    /// </summary>
    public class ConnectionLinkAllocationException : MappingException
    {
        /// <summary>
        /// Public constructor for ConnectionLinkAllocationException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public ConnectionLinkAllocationException(string message) : base(message) { }
    }
    
    /// <summary>
    /// Exception thrown when attempting to create a new link with an invalid (non-postive) value for Link Speed
    /// </summary>
    public class InvalidLinkSpeedException : MappingException
    {
        /// <summary>
        /// Public constructor for InvalidLinkSpeedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public InvalidLinkSpeedException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to create a new link with an invalid Source FPGA ID
    /// </summary>
    public class LinkSourceDoesNotExistException : MappingException
    {
        /// <summary>
        /// Public constructor for LinkSourceDoesNotExistException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public LinkSourceDoesNotExistException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when attempting to create a new link with an invalid Sink FPGA ID
    /// </summary>
    public class LinkSinkDoesNotExistException : MappingException
    {
        /// <summary>
        /// Public constructor for LinkSinkDoesNotExistException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public LinkSinkDoesNotExistException(string message) : base(message) { }
    }
    #endregion

    #region General Mapping Exceptions
    /// <summary>
    /// Exception thrown when the mapping process is unable to be completed.
    /// </summary>
    public class MappingFailedException : MappingException
    {
        /// <summary>
        /// Public constructor for MappingFailedException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public MappingFailedException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to create pre-synthesis XML file with an incomplete mapping state.
    /// </summary>
    public class MappingIncompleteException : MappingException
    {
        /// <summary>
        /// Public constructor for MappingIncompleteException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public MappingIncompleteException(string message) : base(message) { }
    }    
    /// <summary>
    /// Exception thrown when attempting to add a duplicate ID within a collection of mapping objects
    /// </summary>
    public class IDAlreadyExistsException : MappingException
    {
        /// <summary>
        /// Public constructor for IDAlreadyExistsException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public IDAlreadyExistsException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when attempting to remove or access a non-existant ID within a collection of mapping objects
    /// </summary>
    public class IDDoesNotExistException : MappingException
    {
        /// <summary>
        /// Public constructor for IDDoesNotExistException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public IDDoesNotExistException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception thrown when calculate link usage and an unbalanced connection to path ratio occurs.
    /// </summary>
    public class LinkUsageCalculationException : MappingException
    {
        /// <summary>
        /// Public constructor for LinkUsageCalculationException Class.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception</param>
        public LinkUsageCalculationException(string message) : base(message) { }
    }
    #endregion
    
}
