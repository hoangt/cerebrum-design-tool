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
/*********************************************************************************************************** 
 * FalconXpsBuilder\FalconXPSAddressSpace.cs
 * Name: Matthew Cotter
 * Date: 18 Jun 2010 
 * Description: Classes to handle verification of overlapping and sufficient address spaces in an XPS Project.
 * Notes:
 *     
 * History: 
 * >> (15 Feb 2010) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Address ranges employ new CoreAddressRangeInfo object, and utilize improved property system.
 * >> (27 Jan 2011) Matthew Cotter: Removed address validation from tool flow.  Retrieval of remote MPD files is still required by UCF generation, however.
 * >> (11 Aug 2010) Matthew Cotter: Began work on implementing multi-tiered search for design cores.
 * >> ( 2 Jul 2010) Matthew Cotter: Completed address verification and generation on a per-bus connection basis.
 * >> ( 1 Jul 2010) Matthew Cotter: Completed implementation of verification of overlapping address spaces.   Initial work
 *                 					  on support of generating and correcting insufficient address spaces.
 *                 					  Work on generating and correcting address space awaiting test.
 * >> (18 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using FalconPathManager;
using System.Xml;
using System.Diagnostics;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Defines an address pair (base and high addresses) that define an address range for address verification.
    /// </summary>
    public class AddressPair
    {
        /// <summary>
        /// Default constructor.   Initializes the Base and High addresses to empty strings.
        /// </summary>
        public AddressPair()
        {
            this.Base = string.Empty;
            this.High = string.Empty;
        }
        /// <summary>
        /// Name of the base address parameter of the pair.
        /// </summary>
        public string BaseName { get; set; }
        /// <summary>
        /// String representation of the base address.
        /// </summary>
        public string Base { get; set; }
        /// <summary>
        /// String representation of the high address.
        /// </summary>
        public string High { get; set; }
    }

    /// <summary>
    /// A range of addresses, within which multiple free spaces may be defined.   The range abstracts the range of available addresses on a bus, with
    /// each contained space representing unallocated or free ranges on the bus.
    /// </summary>
    public class AddressRange
    {
        /// <summary>
        /// An address space representing a subset of an address range.
        /// </summary>
        private class AddressSpace
        {
            long _start;
            long _end;

            /// <summary>
            /// Custom constructor.  Initializes the boundaries of the address range using starting and ending addresses.
            /// </summary>
            /// <param name="StartAddr">The starting or base address of the space.</param>
            /// <param name="EndAddr">The ending or high address of the space.</param>
            public AddressSpace(long StartAddr, long EndAddr)
            {
                if (StartAddr <= EndAddr)
                {
                    _start = StartAddr;
                    _end = EndAddr;
                }
                else
                {
                    _start = -1;
                    _end = -10000;
                }
                ReservedFor = string.Empty;
            }

            /// <summary>
            /// The core that has reserved the address space.
            /// </summary>
            public string ReservedFor { get; set; }
            /// <summary>
            /// The value of the base address of the address space.
            /// </summary>
            public long Start
            {
                get
                {
                    return _start;
                }
            }
            /// <summary>
            /// The value of the high address of the address space.
            /// </summary>
            public long End
            {
                get
                {
                    return _end;
                }
            }

            /// <summary>
            /// The size of the address space as calculated by subtracting the Start from the End, and adding one to include the Start.
            /// </summary>
            public long Size
            {
                get
                {
                    return (_end - _start) + 1;
                }
            }
            /// <summary>
            /// Determines whether this address space overlaps with another.
            /// </summary>
            /// <param name="other">Another address space object with which to determine any overlap.</param>
            /// <returns>True if the other address space overlaps with this one, by even one byte.</returns>
            public bool Overlaps(AddressSpace other)
            {
                if (other.Start > this.Start)
                {
                    if (other.Start <= this.End)
                        return true;
                    else
                        return false;
                }
                else if (other.Start < this.Start)
                {
                    if (other.End >= this.Start)
                        return true;
                    else
                        return false;
                }
                else //if (other.Start == this.Start)
                {
                    return true;
                }
            }
            /// <summary>
            /// Determines whether this address space completely envelops another.
            /// </summary>
            /// <param name="other">Another address space object to determine containment.</param>
            /// <returns>True if the other address space is contained entirely within this address space.</returns>
            public bool Contains(AddressSpace other)
            {
                return ((other.Start >= this.Start) && (other.End <= this.End));
            }
            /// <summary>
            /// Allocates a chunk of this address space, and if necessary splitting itself into upto two new address spaces, removing 
            /// the allocated chunk from itself.
            /// </summary>
            /// <param name="StartAddr">The base address of the space to allocate.</param>
            /// <param name="EndAddr">The high address of the space to allocate.</param>
            /// <param name="Before">A newly created address space before the allocation, if any space remains.</param>
            /// <param name="After">A newly created address space after the allocation, if any space remains.</param>
            /// <returns>True if the space was successfully allocated, False otherwise.</returns>
            public bool Allocate(long StartAddr, long EndAddr, out AddressSpace Before, out AddressSpace After)
            {
                AddressSpace newSpace = new AddressSpace(StartAddr, EndAddr);
                if (!this.Contains(newSpace))
                {
                    Before = null;
                    After = null;
                    return false;
                }
                else
                {
                    Before = new AddressSpace(this.Start, StartAddr - 1);
                    After = new AddressSpace(EndAddr + 1, this.End);
                    if (Before.Size <= 0)
                        Before = null;
                    if (After.Size <= 0)
                        After = null;
                    return true;
                }
            }

        }

        /// <summary>
        /// A name given to the address range.
        /// </summary>
        public string Name { get; set; }

        private List<AddressSpace> _UnAllocatedSpaces;

        private List<AddressSpace> _AllocatedSpaces;

        /// <summary>
        /// Custom constructor.  Initializes the address range to the specified Base and High addresss, and creates a single free
        /// address space within representing the entire range as unallocated.
        /// </summary>
        /// <param name="BaseAddress"></param>
        /// <param name="HighAddress"></param>
        public AddressRange(long BaseAddress, long HighAddress)
        {
            _UnAllocatedSpaces = new List<AddressSpace>();
            _UnAllocatedSpaces.Add(new AddressSpace(BaseAddress, HighAddress));
            _AllocatedSpaces = new List<AddressSpace>();
        }

        /// <summary>
        /// Determines whether the address space specified by the Base and High address pair is unallocated within any free spaces contained in the range.
        /// Also, determines whether a core already has the space reserved (and is already available to it)
        /// </summary>
        /// <param name="BaseAddress">The base address of the space to query.</param>
        /// <param name="HighAddress">The high address of the space to query.</param>
        /// <param name="ReservedFor">The instance of the core for which the range is to be checked for availability.</param>
        /// <returns>True if the specified space is unallocated within the address range.</returns>
        public bool IsAvailable(long BaseAddress, long HighAddress, string ReservedFor)
        {
            bool bFree = false;
            AddressSpace DesiredSpace = new AddressSpace(BaseAddress, HighAddress);
            DesiredSpace.ReservedFor = ReservedFor;
            foreach (AddressSpace space in _UnAllocatedSpaces)
            {
                bFree = space.Contains(DesiredSpace);
                if (bFree)
                    break;
            }
            if (!bFree)
            {
                foreach (AddressSpace space in _AllocatedSpaces)
                {
                    if (String.Compare(space.ReservedFor, ReservedFor) == 0)
                    {
                        if (space.Contains(DesiredSpace))
                        {
                            bFree = true;
                            break;
                        }
                    }
                }
            }
            return bFree;
        }

        /// <summary>
        /// Allocates the address space specified by the Base and High address pair if it is unallocated within any free spaces contained in the range.
        /// </summary>
        /// <param name="BaseAddress">The base address of the space to allocate.</param>
        /// <param name="HighAddress">The high address of the space to allocate.</param>
        /// <param name="ReservedFor">The instance of the core for which the range is to be reserved.</param>
        /// <returns>True if the specified space was successfully allocated within the address range.</returns>
        public bool AllocateRange(long BaseAddress, long HighAddress, string ReservedFor)
        {
            bool bFree = false;
            AddressSpace DesiredSpace = new AddressSpace(BaseAddress, HighAddress);
            DesiredSpace.ReservedFor = ReservedFor;

            foreach (AddressSpace space in _UnAllocatedSpaces)
            {
                bFree = space.Contains(DesiredSpace);
                space.ReservedFor = ReservedFor;
                if (bFree)
                {
                    AddressSpace Before = null;
                    AddressSpace After = null;
                    Trace.WriteLine(String.Format("Reserving ({0} to {1}) on Bus {2} for {3}",
                        FalconXPSAddressVerifier.LongToHex(BaseAddress, 8),
                        FalconXPSAddressVerifier.LongToHex(HighAddress, 8),
                        this.Name,
                        ReservedFor));
                    space.Allocate(BaseAddress, HighAddress, out Before, out After);
                    _UnAllocatedSpaces.Remove(space);
                    _AllocatedSpaces.Add(DesiredSpace);
                    if (Before != null)
                    {
                        _UnAllocatedSpaces.Add(Before);
                    }
                    if (After != null)
                    {
                        _UnAllocatedSpaces.Add(After);
                    }
                    break;
                }
            }
            if (!bFree)
            {
                foreach (AddressSpace space in _AllocatedSpaces)
                {
                    if (String.Compare(space.ReservedFor, ReservedFor) == 0)
                    {
                        if (space.Contains(DesiredSpace))
                        {
                            bFree = true;
                            break;
                        }
                    }
                }
            }
            return bFree;
        }

        /// <summary>
        /// Locates any available free space within the address range, matching the specified address criteria, and returning the newly found
        /// base and high addresses through the corresponding arguments.  The value of MinimumBaseAddress limits the range of available addresses
        /// to be generated by placing a lower bound on the value of the base address.
        /// </summary>
        /// <param name="AddressSize">The size of the address space to be located.</param>
        /// <param name="BaseAddress">(out) The base address generated, if successful.</param>
        /// <param name="HighAddress">(out) The high address generated, if successful.</param>
        /// <param name="MinimumBaseAddress">The lower bound on which a base address can be generated.</param>
        /// <param name="ReservedFor">Instance for which the address space is to be reserved for.</param>
        /// <returns>True if a free space meeting the size and base address criteria was found, False otherwise.</returns>
        public bool LocateFreeSpace(long AddressSize, out long BaseAddress, out long HighAddress, long MinimumBaseAddress, string ReservedFor)
        {
            HighAddress = (long)(Math.Pow(2, 32)) - 1;
            BaseAddress = 0;
            bool bFoundSpace = false;

            foreach (AddressSpace space in _UnAllocatedSpaces)
            {
                if ((space.Size >= AddressSize) && (space.Start >= MinimumBaseAddress))
                {
                    BaseAddress = space.Start;
                    HighAddress = BaseAddress + AddressSize;
                    bFoundSpace = true;
                    this.AllocateRange(BaseAddress, HighAddress, ReservedFor);
                }
                if (bFoundSpace)
                    break;
            }
            return bFoundSpace;
        }
    }

    /// <summary>
    /// Class to read an XPS project MHS file and validate all address spaces on cores connected to each bus, ensuring that no address ranges overlap.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/XpsBuilder Specification.pdf">
    /// XPS Builder Documentation</seealso>
    public class FalconXPSAddressVerifier
    {
        PathManager _PathMan;
        FileServices _FileServ;
        XpsBuilder _Builder;
        XpsProjectOptions _ProjectOpts;
        private string _PlatformID = string.Empty;

        /// <summary>
        /// Custom contructor.  Initializes the verifier with the Platform ID, and using the pre-defined and prepared FileServices, PathManager, 
        /// and XpsProjectOptions classes.
        /// </summary>
        /// <param name="PlatformID">The ID of the platform being verified</param>
        /// <param name="fs">The file servers object ot be used for retrieving remote MPD files</param>
        /// <param name="pm">The project path manager associated with the Cerebrum project</param>
        /// <param name="builder">The XpsBuilder object used to generate and build the XPS project</param>
        public FalconXPSAddressVerifier(string PlatformID, FileServices fs, PathManager pm, XpsBuilder builder)
        {
            _PathMan = pm;
            _FileServ = fs;
            _PlatformID = PlatformID;
            _Builder = builder;
            _ProjectOpts = builder.ProjectOpts;
        }
        
        /// <summary>
        /// Static function to convert a string formatted as a binary number (0b00000000) to a long int.
        /// </summary>
        /// <param name="BinaryNumber">The binary number in string format to be converted.</param>
        /// <returns>A long int value indicating the value of the binary string, if conversion was successful.</returns>
        public static long BinToLong(string BinaryNumber)
        {
            long val = 0;
            int power = 0;
            BinaryNumber = BinaryNumber.ToUpper();
            if (BinaryNumber.StartsWith("0B"))
                BinaryNumber = BinaryNumber.Substring(2);
            while (BinaryNumber.Length > 0)
            {
                int digitValue = 0;
                string digit = BinaryNumber.Substring(BinaryNumber.Length - 1);
                char cDigit = Char.Parse(digit.ToUpper());
                if (Char.IsDigit(cDigit))
                {
                    digitValue = (cDigit - '0');
                    if (digitValue > 1)
                        return 0;
                }
                else
                {
                    return 0;
                }
                val += (digitValue * (long)(Math.Pow(2, power)));
                BinaryNumber = BinaryNumber.Substring(0, BinaryNumber.Length - 1);
                power++;
            }
            return val;
        }
        /// <summary>
        /// Static function to convert a string formatted as a Hexadecimal number (0x0ff0Ac44) to a long int.
        /// </summary>
        /// <param name="HexNumber">The Hex number in string format to be converted.</param>
        /// <returns>A long int value indicating the value of the hexadecimal string, if conversion was successful.</returns>
        public static long HexToLong(string HexNumber)
        {
            HexNumber = HexNumber.ToUpper();
            if ((HexNumber.StartsWith("0X")) || (HexNumber.StartsWith("0H")))
                HexNumber = HexNumber.Substring(2);
            long val = 0;
            int power = 0;
            while (HexNumber.Length > 0)
            {
                int digitValue = 0;
                string digit = HexNumber.Substring(HexNumber.Length - 1);
                char cDigit = Char.Parse(digit.ToUpper());
                if (Char.IsDigit(cDigit))
                {
                    digitValue = (cDigit - '0');
                }
                else if (Char.IsLetter(cDigit))
                {
                    digitValue = (cDigit - 'A') + 1;
                    if (digitValue > 6)
                        return 0;
                    else
                        digitValue += 9;
                }
                val += (digitValue * (long)(Math.Pow(16, power)));
                HexNumber = HexNumber.Substring(0, HexNumber.Length - 1);
                power++;
            }
            return val;
        }
        /// <summary>
        /// Static function to convert a long int to a Hexadecimal string (0x000F000F).
        /// </summary>
        /// <param name="LongNumber">The long int to be converted.</param>
        /// <param name="MinDigits">The minimum number of Hex digits to be included in the result, accomplished with zero-padding.</param>
        /// <returns>A hexadecimal string representation of the long integer that was converted.</returns>
        public static string LongToHex(long LongNumber, int MinDigits)
        {
            string val = string.Empty;
            while (LongNumber > 0)
            {
                long remainder = LongNumber % 16;
                char digit;
                if (remainder >= 9)
                    digit = Convert.ToChar(Convert.ToInt64('A') + (remainder - 10));
                else
                    digit = Convert.ToChar(Convert.ToInt64('0') + remainder);
                val = digit + val;
                LongNumber = LongNumber / 16;
            }
            return "0x" + val.ToUpper().PadLeft(MinDigits, '0');
        }
        /// <summary>
        /// Static function to convert a long int to a binary string (0x00010001).
        /// </summary>
        /// <param name="LongNumber">The long int to be converted.</param>
        /// <param name="MinDigits">The minimum number of binary digits to be included in the result, accomplished with zero-padding.</param>
        /// <returns>A binary string representation of the long integer that was converted.</returns>
        public static string LongToBin(long LongNumber, int MinDigits)
        {
            string val = string.Empty;
            while (LongNumber > 0)
            {
                long remainder = LongNumber % 2;
                char digit;
                digit = Convert.ToChar(Convert.ToInt64('0') + remainder);
                val = digit + val;
                LongNumber = LongNumber / 2;
            }
            return "0b" + val.PadLeft(MinDigits, '0');
        }

        /// <summary>
        /// Parses the specfied MHS file using the supplied XpsProjectOptions for project parameters and validates the 
        /// address spaces defined.   If any overlaps are found, new spaces are generated for the offending cores, if possible.
        /// </summary>
        /// <param name="MHSFile">The path to the MHS file to be parsed.</param>
        /// <param name="SystemBusList">List of system buses available/used/required in the MHS file.</param>
        /// <param name="CompCores">List of ComponentCore to be written to the MHS file.</param>
        /// <param name="Ports">List of the external ports added to the MHS file.</param>
        /// <param name="ProcTargets">List of the types of Processors instantiated on the FPGA platform.</param>
        /// <returns>True if address validation was successful, False otherwise.  Success indicates that no address spaces overlap
        /// AFTER execution, but provides no information about overlap prior to the call.</returns>
        public bool VerifyMHSAddresses(string MHSFile, List<string> SystemBusList, List<ComponentCore> CompCores, List<string> Ports, string ProcTargets)
        {
            try
            {
                ValidateAddresses(CompCores, SystemBusList, ProcTargets);
                _Builder.RewriteMHS(MHSFile, CompCores, Ports);
                return true;
            }
            catch (Exception ex)
            {
                _Builder.RaiseMessageEvent(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
                return false;
            }
        }

        private void GetCoreAddressInfo(ComponentCore CompCore, out List<CoreAddressRangeInfo> CoreInfos)
        {
            CoreInfos = new List<CoreAddressRangeInfo>();
            string tempPath = _Builder.PathMan.GetPath("ProjectTemp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            FileInfo coreFile = CompCore.RetrieveMPD(_PathMan, _FileServ);
            if ((coreFile != null) && (coreFile.Exists))
            {
                // Parse MPD
                _Builder.ReadBusAddressParameters(coreFile.FullName, out CoreInfos, CompCore);
                coreFile.Attributes = FileAttributes.Normal;
                if (coreFile.FullName.Contains(tempPath))
                {
                    try
                    {
                        coreFile.Delete();
                    }
                    catch { }
                }
            }
            coreFile = null;
        }


        private bool ValidateAddresses(List<ComponentCore> CompCores, List<string> SystemBusList, string ProcTargets)
        {
            Dictionary<string, AddressRange> BusRanges = new Dictionary<string, AddressRange>();
            // Create address maps for all busses
            foreach (string BusName in SystemBusList)
            {
                if (!BusRanges.ContainsKey(BusName))
                {
                    AddressRange newRange = new AddressRange(0, (long)(Math.Pow(2, 32)) - 1);
                    newRange.Name = BusName;
                    BusRanges.Add(BusName, newRange);
                }
            }
            // POWERPC ONLY: First find the PPC Memory controller and ensure that it is located at base address 0.
            foreach (ComponentCore CompCore in CompCores)
            {
                if (CompCore.CoreType == "ppc440mc_ddr2")
                {
                    if (!ValidateCoreSpace(BusRanges, CompCore, true, ProcTargets))
                        return false;
                }
            }

            // Go through core list ...
            foreach (ComponentCore CompCore in CompCores)
            {
                if (CompCore.CoreType == "ppc440mc_ddr2")
                    continue;

                if (!ValidateCoreSpace(BusRanges, CompCore, false, ProcTargets))
                    return false;
            }
            return true;
        }

        private bool ValidateCoreSpace(Dictionary<string, AddressRange> BusRanges, ComponentCore CompCore, bool IsPPCMemCtrlr, string ProcTargets)
        {
            // Get the core information
            List<CoreAddressRangeInfo> CoreInfos;
            GetCoreAddressInfo(CompCore, out CoreInfos);

            // Create the set of Core Address Pairs
            if ((CoreInfos != null) && (CoreInfos.Count > 0))
            {
                CerebrumPropertyEntry baseAddr = null;
                CerebrumPropertyEntry highAddr = null;
                foreach (CoreAddressRangeInfo CI in CoreInfos)
                {
                    // For each address in the MPD
                    string baseName = CI.BaseParameter.PropertyName;
                    string highName = CI.HighParameter.PropertyName;
                    AddressRange Bus = null;
                    // Determine if this address parameter is valid
                    if (!(CI.IsValidCond == string.Empty))
                    {
                        try
                        {
                            if (!Conditions.EvaluateAsBoolean(CompCore.TranslateString(CI.IsValidCond)))
                                continue;
                        }
                        catch
                        {
                            throw new Exception("ERROR: Evaluation parameter conditions for address validation failed");
                        }
                    }
                    string OnBusName = string.Empty;

                    foreach (CerebrumPropertyEntry BusEntry in CompCore.Properties.GetEntries(CerebrumPropertyTypes.BUS_INTERFACE))
                    {
                        OnBusName = BusEntry.PropertyName;
                        if (CI.LegalBusList.Contains(OnBusName))
                        {
                            if (BusRanges.ContainsKey(BusEntry.PropertyName))
                            {
                                Bus = (AddressRange)BusRanges[BusEntry.PropertyName];
                                break;
                            }
                        }
                    }
                    if (Bus == null)
                    {
                        // No matching bus found in system connections
                        continue;
                    }

                    // Find the corresponding parameters in the MHS, if they exist
                    foreach (CerebrumPropertyEntry CPEntry in CompCore.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER))
                    {
                        if (String.Compare(CPEntry.PropertyName, baseName) == 0)
                            baseAddr = CPEntry;
                        else if (String.Compare(CPEntry.PropertyName, highName) == 0)
                            highAddr = CPEntry;
                        if ((baseAddr != null) && (highAddr != null))
                        {
                            // Found both, break the search
                            break;
                        }
                    }
                    long Base = 32768;
                    long High = 0;
                    // Verify we found both
                    if ((baseAddr == null) || (highAddr == null))
                    {
                        // At least one address is not specified
                        if (!GenerateOrPurgeAddress(CompCore, baseAddr, highAddr, CI, Bus, ref Base, ref High, false))
                        {
                            return false;
                        }
                        UpdateAddressMap(_PathMan.GetPath("AddressMap"), CompCore.CoreInstance, CompCore.CoreType, _PlatformID, LongToHex(Base, 8), LongToHex(High, 8), ProcTargets);
                    }
                    else
                    {
                        // We found both addresses
                        // Verify that the space is reservable

                        if (baseAddr.PropertyValue.StartsWith("0b"))
                            Base = FalconXPSAddressVerifier.BinToLong(baseAddr.PropertyValue.Substring(2));
                        else if ((baseAddr.PropertyValue.StartsWith("0x")) || (baseAddr.PropertyValue.StartsWith("0h")))
                            Base = FalconXPSAddressVerifier.HexToLong(baseAddr.PropertyValue.Substring(2));

                        if (highAddr.PropertyValue.StartsWith("0b"))
                            High = FalconXPSAddressVerifier.BinToLong(highAddr.PropertyValue.Substring(2));
                        else if ((baseAddr.PropertyValue.StartsWith("0x")) || (baseAddr.PropertyValue.StartsWith("0h")))
                            High = FalconXPSAddressVerifier.HexToLong(highAddr.PropertyValue.Substring(2));

                        if ((High - Base) <= 0)
                        {
                            // Current assignment is invalid
                            if (!GenerateOrPurgeAddress(CompCore, baseAddr, highAddr, CI, Bus, ref Base, ref High, false))
                            {
                                return false;
                            }
                            UpdateAddressMap(_PathMan.GetPath("AddressMap"), CompCore.CoreInstance, CompCore.CoreType, _PlatformID, LongToHex(Base, 8), LongToHex(High, 8), ProcTargets);
                        }
                        else if (Bus.IsAvailable(Base, High, CompCore.CoreInstance))
                        {
                            if (IsPPCMemCtrlr)  // Only one PPC MC and only one address range for it, and no minumum size
                            {
                                // Shift the base address to 0
                                if (Base > 0)
                                {
                                    High -= Base;
                                    Base -= Base;
                                    baseAddr.PropertyValue = FalconXPSAddressVerifier.LongToHex(Base, 8);
                                    highAddr.PropertyValue = FalconXPSAddressVerifier.LongToHex(High, 8);
                                    CompCore.Properties.SetValue(baseAddr, true);
                                    CompCore.Properties.SetValue(highAddr, true);
                                }
                            }
                            // Reserve this space on the bus
                            UpdateAddressMap(_PathMan.GetPath("AddressMap"), CompCore.CoreInstance, CompCore.CoreType, _PlatformID, LongToHex(Base, 8), LongToHex(High, 8), ProcTargets);
                            Bus.AllocateRange(Base, High, CompCore.CoreInstance);
                        }
                        else
                        {
                            // Current assignment overlaps an existing assignment
                            if (!GenerateOrPurgeAddress(CompCore, baseAddr, highAddr, CI, Bus, ref Base, ref High, false))
                            {
                                return false;
                            }
                            UpdateAddressMap(_PathMan.GetPath("AddressMap"), CompCore.CoreInstance, CompCore.CoreType, _PlatformID, LongToHex(Base, 8), LongToHex(High, 8), ProcTargets);
                        }
                    }
                }
            }
            return true;
        }

        private bool GenerateOrPurgeAddress(ComponentCore Core, CerebrumPropertyEntry baseAddr, CerebrumPropertyEntry highAddr,
                                            CoreAddressRangeInfo CI, AddressRange Bus, ref long Base, ref long High, bool ObserveProtectedMemory)
        {
            long CurrentSize = High - Base;
            // Try to make the assignment
            if (GenerateAddressRange(CI.MinSize, CurrentSize, Bus, out Base, out High, ObserveProtectedMemory, Core.CoreInstance))
            {
                baseAddr.PropertyValue = LongToHex(Base, 8);
                highAddr.PropertyValue = LongToHex(High, 8);
                return true;
            }

            // The assignment failed -- check if it MUST be assigned (Required)
            if (CI.Required)
            {
                _Builder.RaiseMessageEvent("ERROR: Core {0} could not be assigned a required address range on the bus.", Core.CoreInstance);
                return false;
            }
            else
            {
                // Remove these addresses from MHS ? -- VERIFY
                _Builder.RaiseMessageEvent("WARNING: Core {0} could not be assigned an optional address range on the bus.", Core.CoreInstance);
                Core.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, baseAddr.PropertyName);
                Core.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, highAddr.PropertyName);
                return true;
            }
        }
        private bool GenerateAddressRange(string MinimumSize, long CurrentSize, AddressRange OnBus, out long BaseAddr, out long HighAddr, bool ObserveProtectedMemory, string ReservedFor)
        {
            BaseAddr = (long)(Math.Pow(2, 32)) - 1;
            HighAddr = 0;
            long minSz = 0;
            if (MinimumSize.Length > 0)
            {
                if (MinimumSize.StartsWith("0b"))
                    minSz = FalconXPSAddressVerifier.BinToLong(MinimumSize.Substring(2));
                else if ((MinimumSize.StartsWith("0x")) || (MinimumSize.StartsWith("0h")))
                    minSz = FalconXPSAddressVerifier.HexToLong(MinimumSize.Substring(2));
            }
            // Use 0xA000000 as base of "free" memory space
            string MinBaseAddress = "0x00000000";
            if (ObserveProtectedMemory)
                MinBaseAddress = "0xA0000000";

            bool bSuccess = false;
            if (CurrentSize > minSz)
                bSuccess = OnBus.LocateFreeSpace(CurrentSize, out BaseAddr, out HighAddr, HexToLong(MinBaseAddress), ReservedFor);
            if (!bSuccess)
                bSuccess = OnBus.LocateFreeSpace(minSz, out BaseAddr, out HighAddr, HexToLong(MinBaseAddress), ReservedFor);
            if ((HighAddr < BaseAddr) || (!bSuccess))
                return false;
            return bSuccess;
        }

        private bool UpdateAddressMap(string AddressMapFile, string CoreInst, string CoreType, string PlatformID, string BaseAddress, string HighAddress, string ProcTargets)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(AddressMapFile);
            bool bUpdated = false;
            foreach (XmlNode xNode in xDoc.ChildNodes)
            {
                if (xNode.Name.ToLower() == "addressmap")
                {
                    foreach (XmlNode xCoreNode in xNode.ChildNodes)
                    {
                        if (xCoreNode.Name.ToLower() == "component")
                        {
                            string CoreID = xCoreNode.Attributes.GetNamedItem("ID").Value;
                            string CoreName = xCoreNode.Attributes.GetNamedItem("Name").Value;
                            string fpgaID = xCoreNode.Attributes.GetNamedItem("FPGA").Value;
                            if ((CoreID == CoreInst) && (CoreName == CoreType) && (fpgaID == PlatformID))
                            {
                                // This is the one we need to update
                                XmlAttribute xBaseAddr = xDoc.CreateAttribute("BASEADDR");
                                XmlAttribute xHighAddr = xDoc.CreateAttribute("HIGHADDR");
                                XmlAttribute xAddrSize = xDoc.CreateAttribute("ADDRSIZE");

                                string AddrSize = LongToHex(HexToLong(HighAddress) - HexToLong(BaseAddress), 8);
                                xBaseAddr.Value = BaseAddress.ToUpper().Replace("0X", "0x");
                                xHighAddr.Value = HighAddress.ToUpper().Replace("0X", "0x");
                                xAddrSize.Value = AddrSize.ToUpper().Replace("0X", "0x");
                                xCoreNode.Attributes.SetNamedItem(xBaseAddr);
                                xCoreNode.Attributes.SetNamedItem(xHighAddr);
                                xCoreNode.Attributes.SetNamedItem(xAddrSize);

                                XmlAttribute xProcTargets = xDoc.CreateAttribute("TARGETS");
                                xProcTargets.Value = ProcTargets;
                                xCoreNode.Attributes.SetNamedItem(xProcTargets);
                                bUpdated = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (bUpdated) xDoc.Save(AddressMapFile);
            return true;
        }
    }
}
