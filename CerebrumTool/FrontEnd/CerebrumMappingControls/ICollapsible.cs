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
 * ICollapsible.cs
 * Name: Matthew Cotter
 * Date: 17 Sep 2010 
 * Description: Defines the interface exposed by a mapping control that is collapsible.
 * History: 
 * >> (17 Sep 2010) Matthew Cotter: Defined basic 'collapsible' interface.
 * >> (17 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumMappingControls
{
    /// <summary>
    /// Interface defining a collapsible mapping control
    /// </summary>
    public interface ICollapsible
    {
        /// <summary>
        /// Initialzes the collapsible interface
        /// </summary>
        /// <param name="xScale">The initial x-dimension collapsing scale</param>
        /// <param name="yScale">The initial y-dimension collapsing scale</param>
        void InitCollapsible(double xScale, double yScale);
        /// <summary>
        /// Get or set the x-dimension collapsing scale
        /// </summary>
        double CollapsedXScale { get; set; }
        /// <summary>
        /// Get or set the y-dimension collapsing scale
        /// </summary>
        double CollapsedYScale { get; set; }
        /// <summary>
        /// Get the current x-dimension scale
        /// </summary>
        double CurrentXScale { get; }
        /// <summary>
        /// Get the current y-dimension scale
        /// </summary>
        double CurrentYScale { get; }

        /// <summary>
        /// Indicates whether the control is collapsed
        /// </summary>
        bool Collapsed { get; }

        /// <summary>
        /// Collapses the control if it is not collapsed
        /// </summary>
        void Collapse();
        /// <summary>
        /// Expands the control if it is collapsed
        /// </summary>
        void Expand();
        /// <summary>
        /// Toggles the current state from Collapsed to Expanded, or vice versa.
        /// </summary>
        void Toggle();
    }
}
