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
 * ClockGenerator.cs
 * Name: Matthew Cotter
 * Date: 11 Oct 2010 
 * Description: Representation of a Clock generator object for use in allocating mapping resources and creating XPS projects.
 * History: 
 * >> (18 Feb 2011) Matthew Cotter: Corrected bug in naming and assignment of reset signal
 * >> (16 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Added methods to auto-generate signal names
 * >> (19 Oct 2010) Matthew Cotter: Corrected implementation of acceses to generated clock signals and search for existing signals.
 * >> (15 Oct 2010) Matthew Cotter: Added support for specific instances of clock generators.
 *                                  Corrected type of Phase to be signed integer.
 *                                  Corrected saving of clock generator properties to XML file.
 * >> (11 Oct 2010) Matthew Cotter: Created basic clock generator interface for managing and accessing clocks.
 * >> (11 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CerebrumSharedClasses;

namespace FalconClockManager
{
    /// <summary>
    /// Object representing a clock generator core used to create, manage, and assign clock signals to cores that use them.
    /// </summary>
    public class ClockGenerator
    {
        private int _MaxClocks;

        private List<ClockSignal> _Clocks;

        /// <summary>
        /// Constructor used to create a new clock generator with the specified instance name, type, version, and maximum number of supported clocks.
        /// </summary>
        /// <param name="Type">The type name of the clock generator core.</param>
        /// <param name="Component">The (component) instance name of the clock generator core. Typically this will be "clock_generator".</param>
        /// <param name="Core">The instance name of the clock generator core.  Typically this will be the ID of the FPGA for the clock generator.</param>
        /// <param name="Version">The version number of the clock generator core.</param>
        /// <param name="MaxClocks">The maximum number of clocks supported by the clock generator.</param>
        public ClockGenerator(string Type, string Component, string Core, string Version, int MaxClocks)
        {
            this.Type = Type;
            this.ComponentInstance = Component;
            this.CoreInstance = Core;
            this.Version = Version;
            this.MaxDrivePerClock = Int32.MaxValue;

            _MaxClocks = MaxClocks;
            _Clocks = new List<ClockSignal>();
        }
        /// <summary>
        /// Get or set the type name of the clock generator core
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Get or set the instance name of the component that this clock corresponds to.  The clock generator component is typically virtual, but this property is specified to conform to naming conventions.
        /// </summary>
        public string ComponentInstance { get; set; }
        /// <summary>
        /// Get or set the instance name of the core that this clock corresponds to.  The clock generator component is typically virtual, but this property is specified to conform to naming conventions.
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Get or set the instance name of the clock generator core
        /// </summary>
        public string Instance 
        {
            get
            {
                return String.Format("{0}_{1}", this.ComponentInstance, this.CoreInstance);
            }
        }
        /// <summary>
        /// Get or set the version number of the clock generator core
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Get or set the input frequency to the clock generator
        /// </summary>
        public long InputFrequency { get; set; }
        /// <summary>
        /// Get or set the name of the input signal to the clock generator
        /// </summary>
        public string ClockInSignal { get; set; }
        /// <summary>
        /// Get or set the name of the reset signal to the clock generator
        /// </summary>
        public string ResetSignal { get; set; }
        /// <summary>
        /// Get or set the name of the clocks-locked signal to the clock generator
        /// </summary>
        public string LockedSignal { get; set; }

        /// <summary>
        /// Get or set the maximum number of output signals that one clock generator signal may drive.
        /// </summary>
        public int MaxDrivePerClock { get; set; }

        /// <summary>
        /// Gets the maximum number of clocks supported
        /// </summary>
        public int MaxClocks
        {
            get
            {
                return _MaxClocks;
            }
        }
        /// <summary>
        /// Gets the current number of clocks in-use
        /// </summary>
        public int NumClocks
        {
            get
            {
                return _Clocks.Count;
            }
        }
        /// <summary>
        /// Gets the number of unused clocks
        /// </summary>
        public int UnusedClocks
        {
            get
            {
                return this.MaxClocks - this.NumClocks;
            }
        }

        /// <summary>
        /// Creates a new clock with the specified parameters
        /// </summary>
        /// <param name="Frequency">The frequency of the new clock</param>
        /// <param name="Phase">The frequency of the new clock</param>
        /// <param name="Group">The frequency of the new clock</param>
        /// <param name="Buffered">The frequency of the new clock</param>
        /// <returns>Returns true if the clock parameters are supported by a clock generated by the clock generator after the call, false if not.</returns>
        public bool AddClock(long Frequency, int Phase, ClockGroup Group, bool Buffered)
        {
            if (NumClocks >= MaxClocks)
            {
                return false;
            }
            else
            {
                ClockSignal NewClock = new ClockSignal();
                NewClock.Frequency = Frequency;
                NewClock.Phase = Phase;
                NewClock.Group = Group;
                NewClock.Buffered = Buffered;
                if (!HasAvailableClock(NewClock))
                    _Clocks.Add(NewClock);
                return true;
            }
        }
        /// <summary>
        /// Creates a new clock that with parameters that support the specified clock
        /// </summary>
        /// <param name="RequiredClock">The clock that must be supported by the clock generator</param>
        /// <returns>Returns true if the clock parameters are supported by a clock generated by the clock generator after the call, false if not.</returns>
        public bool AddClock(ClockSignal RequiredClock)
        {
            if (NumClocks >= MaxClocks)
            {
                return false;
            }
            else
            {
                if (!HasAvailableClock(RequiredClock))
                {
                    ClockSignal NewClock = new ClockSignal(RequiredClock);
                    NewClock.ComponentInstance = this.ComponentInstance;
                    NewClock.CoreInstance = this.CoreInstance; 
                    _Clocks.Add(NewClock);
                    UpdateClockGenPort(NewClock);
                }
                return true;
            }
        }

        /// <summary>
        /// Determines whether a clock with the specified parameters is generated by the clock generator
        /// </summary>
        /// <param name="Frequency">The frequency of the new clock</param>
        /// <param name="Phase">The frequency of the new clock</param>
        /// <param name="Group">The frequency of the new clock</param>
        /// <param name="Buffered">The frequency of the new clock</param>
        /// <returns>Returns true if the clock parameters are supported by a clock generated by the clock generator, false if not.</returns>
        private bool HasAvailableClock(long Frequency, int Phase, ClockGroup Group, bool Buffered)
        {
            foreach (ClockSignal clock in _Clocks)
            {
                if (((long)clock.FrequencyValue == (long)Frequency) && 
                    (clock.Phase == Phase) && 
                    (clock.Group == Group) &&
                    (clock.Buffered == Buffered) &&
                    (clock.DriveCount < MaxDrivePerClock))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether a clock with the specified parameters is generated by the clock generator
        /// </summary>
        /// <param name="RequiredClock">The clock that must be supported by the clock generator</param>
        /// <returns>Returns true if the clock parameters are supported by a clock generated by the clock generator, false if not.</returns>
        public bool HasAvailableClock(ClockSignal RequiredClock)
        {
            foreach (ClockSignal clock in _Clocks)
            {
                if (((long)clock.FrequencyValue == (long)RequiredClock.FrequencyValue) && 
                    (clock.Phase == RequiredClock.Phase) && 
                    (clock.Group == RequiredClock.Group) &&
                    (clock.Buffered == RequiredClock.Buffered) &&
                    (clock.DriveCount < MaxDrivePerClock))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Locates the clock signal name of a clock in the generator with the specified parameters
        /// </summary>
        /// <param name="Frequency">The frequency of the clock</param>
        /// <param name="Phase">The frequency of the clock</param>
        /// <param name="Group">The frequency of the clock</param>
        /// <param name="Buffered">The frequency of the clock</param>
        /// <returns>Returns name of the clock signal associated with the clock generator output for the specified clock if found, an empty string otherwise.</returns>
        public string GetMatchingClockSignal(long Frequency, int Phase, ClockGroup Group, bool Buffered)
        {
            foreach (ClockSignal clock in _Clocks)
            {
                if (((long)clock.FrequencyValue == (long)Frequency) &&
                    (clock.Phase == Phase) &&
                    (clock.Group == Group) &&
                    (clock.Buffered == Buffered))
                {
                    UpdateAllClockGenPorts();
                    return clock.GenerateCoreSignal();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Locates the clock signal name of a clock in the generator supporting the specified clock
        /// </summary>
        /// <param name="RequiredClock">The clock that must be supported by the clock generator</param>
        /// <returns>Returns name of the clock signal associated with the clock generator output for the specified clock if found, an empty string otherwise.</returns>
        public string GetMatchingClockSignal(ClockSignal RequiredClock)
        {
            return GetMatchingClockSignal((long)RequiredClock.FrequencyValue, RequiredClock.Phase, RequiredClock.Group, RequiredClock.Buffered);
        }
        /// <summary>
        /// Locates the clock signal of a clock in the generator supporting the specified clock
        /// </summary>
        /// <param name="RequiredClock">The clock that must be supported by the clock generator</param>
        /// <returns>Returns the clock signal associated with the clock generator output for the specified clock if found, null otherwise.</returns>
        public ClockSignal GetAvailableClock(ClockSignal RequiredClock)
        {
            return GetMatchingClock((long)RequiredClock.FrequencyValue, RequiredClock.Phase, RequiredClock.Group, RequiredClock.Buffered);
        }
        /// <summary>
        /// Locates the clock signal of a clock in the generator supporting the specified clock
        /// </summary>
        /// <param name="Frequency">The frequency of the clock</param>
        /// <param name="Phase">The frequency of the clock</param>
        /// <param name="Group">The frequency of the clock</param>
        /// <param name="Buffered">The frequency of the clock</param>
        /// <returns>Returns the clock signal associated with the clock generator output for the specified clock if found, null otherwise.</returns>
        public ClockSignal GetMatchingClock(long Frequency, int Phase, ClockGroup Group, bool Buffered)
        {
            foreach (ClockSignal clock in _Clocks)
            {
                if (((long)clock.FrequencyValue == (long)Frequency) &&
                    (clock.Phase == Phase) &&
                    (clock.Group == Group) &&
                    (clock.Buffered == Buffered))
                {
                    UpdateAllClockGenPorts();
                    return clock;
                }
            }
            return null;
        }

        /// <summary>
        /// Increments the drive counter on the specified clock signal, if it is part of this clock generator.
        /// </summary>
        /// <param name="RequiredClock">The clock signal to be acquired.</param>
        public void AcquireClockSignal(ClockSignal RequiredClock)
        {
            if (_Clocks.Contains(RequiredClock))
            {
                if (RequiredClock.DriveCount < MaxDrivePerClock)
                    RequiredClock.DriveCount += 1;
            }
        }
        /// <summary>
        /// Decrements the drive counter on the specified clock signal, if it is part of this clock generator.  If the signal no longer
        /// has any drivers, it is removed.
        /// </summary>
        /// <param name="ReleasedClock">The clock signal to be released.</param>
        public void ReleaseClockSignal(ClockSignal ReleasedClock)
        {
            if (_Clocks.Contains(ReleasedClock))
            {
                if (ReleasedClock.DriveCount > 0)
                    ReleasedClock.DriveCount -= 1;
                if (ReleasedClock.DriveCount == 0)
                    _Clocks.Remove(ReleasedClock);
            }
        }

        /// <summary>
        /// Updates a ClockSignal object that was created by this clock generator, to reflect its correct port location.  If the clock signal was not generated by 
        /// this clock generator, it is left unmodified.
        /// </summary>
        /// <param name="GeneratedClock">The clock signal to modify, if it was generated by this clock generator.</param>
        public void UpdateClockGenPort(ClockSignal GeneratedClock)
        {
            if (_Clocks.Contains(GeneratedClock))
            {
                GeneratedClock.Port = String.Format("CLKOUT{0}", _Clocks.IndexOf(GeneratedClock).ToString());
            }
        }
        /// <summary>
        /// Updates all ClockSignal objects created by this clock generator, to reflect their correct port location.
        /// </summary>
        public void UpdateAllClockGenPorts()
        {
            foreach (ClockSignal clock in _Clocks)
            {
                UpdateClockGenPort(clock);
            }
        }
        /// <summary>
        /// Saves core-config definition file for the clock generator
        /// </summary>
        /// <param name="ProjectPath">The path to the project for which the configuration is to be saved.</param>
        public void SaveClockConfig(string ProjectPath)
        {
            CerebrumPropertyCollection CPC = new CerebrumPropertyCollection(this.Instance, this.Instance, this.Type);

            CPC.SetValue(CerebrumPropertyTypes.PORT, "CLKIN", this.ClockInSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, this.LockedSignal, this.GenerateLockSignal(), true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, this.ResetSignal, this.GenerateResetSignal(), true);
            CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "C_CLKIN_FREQ", this.InputFrequency, true);
            
            UpdateAllClockGenPorts();
            for(int i = 0; i < _Clocks.Count; i++)
            {
                ClockSignal CS = _Clocks[i];

                CPC.SetValue(CerebrumPropertyTypes.PORT, String.Format("CLKOUT{0}", i), CS.GenerateCoreSignal(), true);
                CPC.SetValue(CerebrumPropertyTypes.PARAMETER, String.Format("C_CLKOUT{0}_FREQ", i), CS.FrequencyValue, true);
                CPC.SetValue(CerebrumPropertyTypes.PARAMETER, String.Format("C_CLKOUT{0}_PHASE", i), CS.Phase, true);
                CPC.SetValue(CerebrumPropertyTypes.PARAMETER, String.Format("C_CLKOUT{0}_GROUP", i), CS.Group.ToString().ToUpper(), true);
                CPC.SetValue(CerebrumPropertyTypes.PARAMETER, String.Format("C_CLKOUT{0}_BUF", i), CS.Buffered.ToString().ToUpper(), true);
            }
            CPC.SavePropertyCollection(ProjectPath);
        }

        /// <summary>
        /// Generates the clocks stable/locked output signal name for this clock generator.
        /// </summary>
        /// <returns></returns>
        public string GenerateLockSignal()
        {
            return String.Format("{0}_{1}_{2}",
                this.ComponentInstance,
                this.CoreInstance,
                this.LockedSignal);
        }
        /// <summary>
        /// Generates the clocks reset signal name for this clock generator.
        /// </summary>
        /// <returns></returns>
        public string GenerateResetSignal()
        {
            return String.Format("net_gnd");
        }
    }
}
