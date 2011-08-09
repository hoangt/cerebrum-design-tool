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
 * BackEndToolDelegates.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Defines delegates that are used for GUI interfacing to Back End Tools.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Defined basic delegates for message passing and Password requests.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Delegate for library tool to request a password from the GUI
    /// </summary>
    /// <param name="User">The user name whose password is requested</param>
    /// <param name="Server">The server on which the login attempt is being made</param>
    public delegate string PasswordRequestDelegate(string User, string Server);

    /// <summary>
    /// Delegate for MessageEvent event
    /// </summary>
    /// <param name="Message">The message to be transmitted</param>
    public delegate void MessageEventDelegate(string Message);

    /// <summary>
    /// Delegate used to define methods for acquiring parameters from an object in the system.
    /// </summary>
    /// <param name="Source">The source of the parameter to be acquired.</param>
    /// <param name="ComponentInstance">The instance name of the Component from which the parameter is needed, if applicable.</param>
    /// <param name="CoreInstance">>The instance name of the Core within the Component from which the parameter is needed, if applicable.</param>
    /// <param name="ParameterName">The name of the parameter to be acquired.</param>
    /// <returns>If found, an object representing the value of the specified parameter in the project.</returns>
    public delegate object GetParameterDelegate(ParameterSourceTypes Source, string ComponentInstance, string CoreInstance, string ParameterName);

    /// <summary>
    /// Enumeration of valid sources of parameter values to be retrieved
    /// </summary>
    public enum ParameterSourceTypes
    {
        /// <summary>
        /// Indicates an invalid parameter source
        /// </summary>
        PARAMETER_INVALID = 0,
        /// <summary>
        /// Indicates that the parameter is to be retrieved from the project
        /// </summary>
        PARAMETER_PROJECT = 1,
        /// <summary>
        /// Indicates that the parameter is to be retrieved from the specified component
        /// </summary>
        PARAMETER_COMPONENT = 2,
        /// <summary>
        /// Indicates that the parameter is to be retrieved from the specified core within the specified component
        /// </summary>
        PARAMETER_CORE = 3
    }

    /// <summary>
    /// Defines utility methods that may be shared by classes and libraries throughout the Cerebrum system.
    /// </summary>
    public static class UtilMethods
    {
        /// <summary>
        /// Provides a wrapper method for parsing a Parameterized string and invoking a GetParameterDelegate to retrieve the appropriate value.
        /// </summary>
        /// <param name="ParameterString">The Parameterized string from which indicates the required target parameter data is to be retrieved.</param>
        /// <param name="ComponentInstance">The instance name of the Component from which the parameter is needed, if applicable.</param>
        /// <param name="CoreInstance">>The instance name of the Core within the Component from which the parameter is needed, if applicable.</param>
        /// <param name="GetMethod">The GetParameterDelegate to be used in retrieving the target value.</param>
        /// <returns>If found, an object representing the value of the parameter specified ParameterString from an object in the project.</returns>
        public static object GetParameter(string ParameterString, string ComponentInstance, string CoreInstance, GetParameterDelegate GetMethod)
        {
            if ((ParameterString.StartsWith("${") && (ParameterString.EndsWith("}"))))
            {
                string ParamString = ParameterString.Substring(2, ParameterString.Length - 3);
                string[] SplitString = ParamString.Split(new char[] { ':' });

                string SrcType = SplitString[0].Trim();
                if (String.Compare(SrcType, "Project", true) == 0)
                {
                    return GetMethod(ParameterSourceTypes.PARAMETER_PROJECT, null, null, SplitString[SplitString.Length - 1]);
                }
                else if (String.Compare(SrcType, "Component", true) == 0)
                {
                    return GetMethod(ParameterSourceTypes.PARAMETER_COMPONENT, ComponentInstance, null, SplitString[SplitString.Length - 1]);
                }
                else if (String.Compare(SrcType, "Core", true) == 0)
                {
                    return GetMethod(ParameterSourceTypes.PARAMETER_CORE, ComponentInstance, CoreInstance, SplitString[SplitString.Length - 1]);
                }
            }
            return ParameterString;
        }
    }
}
