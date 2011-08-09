﻿/******************************************************************** 
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
 * ICerebrumWizard.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Interface representing a Cerebrum Step-by-Step wizard process.
 * History: 
 * >> (13 Sep 2010) Matthew Cotter: Defined basic functions required to navigate through a wizard process.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconPathManager;


namespace CerebrumProjectManager.Wizards
{
    /// <summary>
    /// Delegate indicating the end of a Cerebrum Wizard
    /// </summary>
    /// <param name="wiz">The Wizard which completed</param>
    public delegate void WizardCompleteHandlerDelegate(ICerebrumWizard wiz);

    /// <summary>
    /// Interface defining a CerebrumWizard
    /// </summary>
    public interface ICerebrumWizard
    {
        /// <summary>
        /// Event fired when the wizard has completed
        /// </summary>
        event WizardCompleteHandlerDelegate WizardCompleteEvent;

        /// <summary>
        /// Property indicating whether the wizard was cancelled
        /// </summary>
        bool Cancelled { get; set; }
        /// <summary>
        /// Property indicating whether the wizard resulted in an error
        /// </summary>
        bool Error { get; set; }
        /// <summary>
        /// String indicating the error message, if any, generated by the wizard
        /// </summary>
        string ErrorMessage { get; set; }
        
        /// <summary>
        /// Method to start the wizard
        /// </summary>
        void Start();
        /// <summary>
        /// Method to end the wizard
        /// </summary>
        void End();

        /// <summary>
        /// Reverts the wizard to the previous step
        /// </summary>
        void GoToPreviousStep();
        /// <summary>
        /// Advances the wizard to the next step
        /// </summary>
        void GoToNextStep();
        /// <summary>
        /// Forces the wizard to display the current step
        /// </summary>
        void DisplayWizardStep();

        /// <summary>
        /// The PathManager object defining the project within(or for) which the wizard is running.
        /// </summary>
        PathManager Paths { get; set;  }        
    }
}
