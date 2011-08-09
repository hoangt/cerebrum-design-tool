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
 * ISOPNIF.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: Interface to implement a Vortex SOP NIF
 * History: 
 * >> (19 Oct 2010) Matthew Cotter: Moved Generic NIF properties to IVortexNIF interface.
 * >> ( 7 Oct 2010) Matthew Cotter: Defined basic interface of SOP NIF.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconResources;

namespace VortexInterfaces
{
    /// <summary>
    /// Interface representing the SOP-Vortex Network Interface
    /// </summary>
    public interface ISOP_NIF : IVortexNIF
    {
    }
}
