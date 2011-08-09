/*******************************************************
 * FalconGlobalInterfaces.cs
 * Author: Ahmed Al Maashri
 * Date 4 June 2010
 * Description: This file includes a number of classes and 
 *              interfaces that can be seen by all other
 *              Falcon components
 *              
 * History:
 * 
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
