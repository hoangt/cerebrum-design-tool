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
/*******************************************************
 * FalconGlobalInterfaces.cs
 * Author: Ahmed Al Maashri
 * Date 4 June 2010
 * Description: This file includes a number of classes and 
 *              interfaces that can be seen by all other
 *              Falcon components
 *              
 * History:
 * >> Ahmed Al Maashri (21 July 2010): Added a reference to 
 *    Guideline to Command Line Environment document
 * *****************************************************/

#region Using block
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion


namespace FalconGlobal
{
    #region IFalconLibrary
    /// <summary>
    /// This interface needs to be implemented by all cerebrum library
    /// component. This interface allows us to have control over what is
    /// essential in implemented components in terms of properties and 
    /// methods.
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/Guideline_command_line_environment.pdf">
    /// Guideline to Command Line Environment</seealso>
    /// </summary>
    public interface IFalconLibrary
    {
        #region Properties
        /// <summary>
        /// Name of the component
        /// </summary>
        string FalconComponentName
        {
            get;
        }
        /// <summary>
        /// Version of the component expressed in X.Y.Z format
        /// </summary>
        string FalconComponentVersion
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method is used to retrieve the version of the component/tool.
        /// The format of the version should be as follow:
        /// <code>ComponentName X.Y.Z Copyright (c) Year PennState</code>
        /// <example>FalconTool 1.0.0 Copyright (c) 2010 PennState</example>
        /// </summary>
        /// <returns>Version of the tool in the format specified above.</returns>
        string GetFalconComponentVersion();
        #endregion
    }
    #endregion
}
