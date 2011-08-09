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
 * ClockSignal.cs
 * Name: Matthew Cotter
 * Date: 11 Oct 2010 
 * Description: Representation of a Clock signal used by the clock generator for use in allocating mapping resources and creating XPS projects.
 * History: 
 * >> (26 May 2010) Matthew Cotter: Corrected project parameter related bug that was causing clock generators to have difficulty in identifying a 100MHz input clock.
 * >> (17 May 2010) Matthew Cotter: Implemented support for capping the number of signals driven by any one clock signal.  While supported, this feature may not be used.
 * >> (16 Feb 2010) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Added properties to indicate Source Component/Core/Port/Lock.
 *                                      Added methods to auto-generate signal names, auto-populate source fields from another ClockSignal and clear source fields
 * >> (15 Oct 2010) Matthew Cotter: Corrected type of Phase to be signed integer.
 *                                  Corrected automatic generation of clock signal names.
 * >> (11 Oct 2010) Matthew Cotter: Created basic clock signal interface for defining clock signals.
 * >> (11 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;

namespace FalconClockManager
{
    /// <summary>
    /// Enumeration of clock group types
    /// </summary>
    public enum ClockGroup
    {
        /// <summary>
        /// Standard clock group type
        /// </summary>
        NONE,
        /// <summary>
        /// Clock Phase-Locked Loop 0 group
        /// </summary>
        PLL0,
        /// <summary>
        /// Adjusted Clock Phase-Locked Loop 0 group
        /// </summary>
        PLL0_ADJUST,
    }

    /// <summary>
    /// Enumerates clock signal directions: INPUT and OUTPUT
    /// </summary>
    public enum ClockDirection
    {
        /// <summary>
        /// Describes a clock signal required as INPUT
        /// </summary>
        INPUT,
        /// <summary>
        /// Describes a clock signal produced as OUTPUT
        /// </summary>
        OUTPUT
    }
    /// <summary>
    /// Class that defines a simple clock signal and its properties
    /// </summary>
    public class ClockSignal
    {
        private int _Phase;

        /// <summary>
        /// Default constructor.  Creates a 100MHz, Phase 0, Buffered, Group-NONE clock signal
        /// </summary>
        public ClockSignal()
        {
            this.Name = string.Empty;
            this.Frequency = 100000000;
            this.Phase = 0;
            this.Group = ClockGroup.NONE;
            this.Buffered = true;
            this.SignalDirection = ClockDirection.OUTPUT;

            this.ComponentInstance = string.Empty;
            this.CoreInstance = string.Empty;
            this.Port = string.Empty;
            this.LockedPort = string.Empty;

            this.DriveCount = 0;
            this.FrequencyRatio = 1.0;
            DisconnectFromSource();
        }
        /// <summary>
        /// Copy constructor.  Creates a new clock signal that is a copy of an existing clock signal.
        /// </summary>
        /// <param name="Copy"></param>
        public ClockSignal(ClockSignal Copy)
        {
            this.Name = Copy.Name;
            this.Phase = Copy.Phase;
            this.Group = Copy.Group;
            this.Buffered = Copy.Buffered;
            this.SignalDirection = Copy.SignalDirection;
            this.Frequency = Copy.FrequencyValue;

            this.ComponentInstance = string.Empty;
            this.CoreInstance = Copy.CoreInstance;
            this.Port = Copy.Port;
            this.LockedPort = Copy.LockedPort;
            this.FrequencyRatio = 1.0;

            DisconnectFromSource();
        }

        #region Clock Signal Core / Port Properties
        /// <summary>
        /// Get or set a decscription of the clock signal, if available.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the instance name of the component that this clock corresponds to.
        /// </summary>
        public string ComponentInstance { get; set; }
        /// <summary>
        /// Get or set the instance name of the core that this clock corresponds to.
        /// </summary>
        public string CoreInstance { get; set; }
        /// <summary>
        /// Get or set the name of the port this clock signal is attached to
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Get or set the name of the port that corresponds to the clock stable signal for this clock.
        /// </summary>
        public string LockedPort { get; set; }
        /// <summary>
        /// Defines the direction this ClockSignal flows from the Component/Core specified.
        /// </summary>
        public ClockDirection SignalDirection { get; set; }
        /// <summary>
        /// Get or set the instance name of the source component that the source clock corresponds to.
        /// </summary>
        public string SourceComponentInstance { get; set; }
        /// <summary>
        /// Get or set the instance name of the source core that the source signal corresponds to.
        /// </summary>
        public string SourceCoreInstance { get; set; }
        /// <summary>
        /// Get or set the name of the source component/core port the source signal is attached to.
        /// </summary>
        public string SourcePort { get; set; }
        /// <summary>
        /// Get or set the name of the source component/core port that corresponds to the clock stable signal for the source signal.
        /// </summary>
        public string SourceLockedPort { get; set; }
        #endregion

        #region Clock Signal Parameters
        /// <summary>
        /// Get the frequency (in Hz) of the clock signal
        /// </summary>
        public long FrequencyValue
        {
            get
            {
                try
                {
                    if (Frequency is string)
                    {
                        string FreqString = (string)Frequency;
                        if ((FreqString.StartsWith("{$}")) && (GetFrequencyFromParameter == null))
                        {
                            System.Diagnostics.Debug.WriteLine("Unable to parse parameter string, GetParameterDelegate not set.");
                            System.Diagnostics.Debug.WriteLine("\tin ClockSignal.cs");
                            System.Diagnostics.Debug.WriteLine(String.Format("Clock Info:\n\tComponent {0}\n\tCore {1}\n\tFrequency {2}", this.ComponentInstance, this.CoreInstance, (string)this.Frequency));
                        }
                        if (GetFrequencyFromParameter != null)
                        {
                            object r = UtilMethods.GetParameter(FreqString, this.ComponentInstance, this.CoreInstance, GetFrequencyFromParameter);
                            string result = (string)r;
                            result = result.ToUpper();
                            long F = 0;
                            result = result.Replace("GHZ", "000MHZ");
                            result = result.Replace("MHZ", "000KHZ");
                            result = result.Replace("KHZ", "000HZ");
                            result = result.Replace("HZ", "");
                            long.TryParse(result, out F);
                            return (long)(F * FrequencyRatio);
                        }
                        else
                        {
                            string result = (string)Frequency;
                            result = result.ToUpper();
                            long F = 0;
                            result = result.Replace("GHZ", "000MHZ");
                            result = result.Replace("MHZ", "000KHZ");
                            result = result.Replace("KHZ", "000HZ");
                            result = result.Replace("HZ", "");
                            long.TryParse(result, out F);
                            return (long)(F * FrequencyRatio);
                        }
                    }
                    else if ((Frequency is long) || (Frequency is int))
                    {
                        long F = 0;
                        long.TryParse(Frequency.ToString(), out F);
                        double FR = F * FrequencyRatio;
                        return (long)(FR);
                    }
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                }
                return 0;
            }
        }
        /// <summary>
        /// Get or set the object used to define the frequency, whether is is a string, number or parameterized source.
        /// </summary>
        public object Frequency
        {
            get { return _Frequency; }
            set { _Frequency = value; }
        }
        private object _Frequency;

        /// <summary>
        /// Delegate used to invoke a method to return the parameter indicating the frequency.
        /// </summary>
        public GetParameterDelegate GetFrequencyFromParameter;

        /// <summary>
        /// Get or set the phase of the clock signal
        /// </summary>
        public int Phase
        {
            get
            {
                return _Phase;
            }
            set
            {
                _Phase = value % 360;
            }
        }
        /// <summary>
        /// Get or set the group of the clock signal
        /// </summary>
        public ClockGroup Group { get; set; }
        /// <summary>
        /// Get or set whether the clock signal is buffered.
        /// </summary>
        public bool Buffered { get; set; }

        /// <summary>
        /// Get or set the ratio by which the frequency property is to be multiplied
        /// </summary>
        public double FrequencyRatio { get; set; }
        #endregion

        /// <summary>
        /// Get or set the number of input clock signals that are driven by this output clock signal.
        /// </summary>
        public int DriveCount { get; set; }

        /// <summary>
        /// Determines whether the specified clock signal matches this signal
        /// </summary>
        /// <param name="CS">The clock signal to test for compatibility</param>
        /// <returns>True if the test clock is compatible with this clock, False otherwise.</returns>
        public bool IsCompatibleWith(ClockSignal CS)
        {
            // Frequency, Phase, Group, Buffer all match
            // Directions differ (Input vs Output)
            return ((this.FrequencyValue == CS.FrequencyValue) && (this.Phase == CS.Phase) && (this.Group == CS.Group) && (this.Buffered == CS.Buffered) && (this.SignalDirection != CS.SignalDirection));
        }

        /// <summary>
        /// Generates a default signal name for the clock, indicating its frequency, phase and group.
        /// </summary>
        /// <returns>A string representing the default clock signal name generated by this clock signal.</returns>
        public string GenerateClockGenSignal()
        {
            return String.Format("clk_{0}_{1}MHz{2}_{3}",
                ((long)FrequencyValue / 1000000).ToString(),
                (((long)FrequencyValue % 1000000) / 1000).ToString("##0"),
                Phase.ToString(),
                Group.ToString());
        }
        /// <summary>
        /// Generates the default signal name, based on the component, core, and port attached to this clock.
        /// </summary>
        /// <returns>A string representing the default signal name, based on the component, core, and port attached to this clock/</returns>
        public string GenerateCoreSignal()
        {
            return String.Format("{0}_{1}_{2}",
                this.ComponentInstance,
                this.CoreInstance,
                this.Port);
        }
        /// <summary>
        /// Generates the default lock signal name, based on the component, core, and lock port specified.
        /// </summary>
        /// <returns>A string representing the default lock signal name, based on the component, core, and lock port specified./</returns>
        public string GenerateLockSignal()
        {
            return String.Format("{0}_{1}_{2}",
                this.ComponentInstance,
                this.CoreInstance,
                this.LockedPort);
        }

        /// <summary>
        /// Applies only when (SignalDirection == ClockDirection.INPUT).  
        /// Clears the Source* property fields for this clock signal, in effect indicating that it is not connected to a source clock.
        /// </summary>
        public void DisconnectFromSource()
        {
            this.SourceComponentInstance = string.Empty;
            this.SourceCoreInstance = string.Empty;
            this.SourcePort = string.Empty;
            this.SourceLockedPort = string.Empty;
        }
        /// <summary>
        /// Applies only when (SignalDirection == ClockDirection.INPUT).  
        /// Populates the approprate source properties from the specified source clock to indicate this clock is connected.
        /// </summary>
        public void ConnectToSource(ClockSignal SourceClock)
        {
            this.SourceComponentInstance = SourceClock.ComponentInstance;
            this.SourceCoreInstance = SourceClock.CoreInstance;
            this.SourcePort = SourceClock.Port;
            this.SourceLockedPort = SourceClock.LockedPort;
        }
        /// <summary>
        /// Applies only when (SignalDirection == ClockDirection.INPUT).  
        /// Returns a boolean value indicating whether the approprate source properties have been set to indicate this clock is connected.
        /// </summary>
        public bool Connected
        {
            get
            {
                return ((this.SourceComponentInstance != string.Empty) && (this.SourceCoreInstance != string.Empty) && (this.SourcePort != string.Empty));
            }
        }
        /// <summary>
        /// Applies only when (SignalDirection == ClockDirection.INPUT).  
        /// Generates a signal name, based on the set source component/core/port parameters.
        /// </summary>
        /// <returns>A string representing the source signal name, based on thesource component, core, and port specified</returns>
        public string GenerateSourceSignal()
        {
            return String.Format("{0}_{1}_{2}",
                this.SourceComponentInstance,
                this.SourceCoreInstance,
                this.SourcePort);
        }
    }
}
