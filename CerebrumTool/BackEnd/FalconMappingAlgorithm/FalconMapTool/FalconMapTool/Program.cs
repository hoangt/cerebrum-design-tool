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
/************************************************************************************************************
 * MapTool\Program.cs
 * Name: Matthew Cotter
 * Date: 1 Jun 2010 
 * Description: This program implements the command-line bash-style interface between the Cerebrum command shell and that
 * of the FalconMappingAlgorithm library.
 * Notes:
 *      Commands can be disabled by removing/commenting their corresponding AddCommand() calls in PopulateCommandTable().
 *          Removing a command in this manner leaves the functionality in tact, but removes the capability for the command to be recognized
 *          as it will not be present in the command table when checked.  Translation from FalconCommand to MappingCommand is done inside
 *          the loop in RunSequence().
 *      New Commands are implemented by: 1) Adding a value to the MapCommandEnum enumeration.
 *                                       2) Adding a call to AddCommand() with the appropriate information in PopulateCommandTable()
 *                                       3) Add the code to the 'switch()' block to execute the new command in:
 *                                                  a) class MappingCommand.Execute() for Mapping system commands
 *                                                  b) class MappingParser.RunSequence() for Built-in tool commands
 * History: 
 * >> (14 Oct 2010) Matthew Cotter: Updated tool for compatibility with changes required to Mapping Algorithm Library.
 * >> (23 Aug 2010) Matthew Cotter: Updated tool to return an exit code (0 or -1) indicating whether it was successful.
 * >> (13 Aug 2010) Matthew Cotter: Removed platform file from cerebinput command due to new hierarchical location and format of platforms.
 * >> (29 Jul 2010) Matthew Cotter: Corrected bug in which and unhandled exception was erroneously reported by RunSequence.
 * >> (22 Jul 2010) Matthew Cotter: Added addrmap command to generate address map from standalone execution.
 * >> (30 Jun 2010) Matthew Cotter: Implemented Sort and Added Command to Sort 'Cerebrum_InputFiles' to the front of the list and override input system and mapping files.
 * >> (28 Jun 2010) Matthew Cotter: Removed access to commands to modify core object IDs.
 * >> (15 Jun 2010) Matthew Cotter: Added implementation of command to generate FPGA routing table information.
 *                                    Updated argument checking to be more flexible, rather than having values hard-coded in multiple places throughout the code.
 *                                    Corrected several bugs in command registration, handling, and processing.
 *                                    Implemented FalconCommandLineParser class and removed old references to previous custom parser implementation.
 *                                    Removed commands to modify Link and Connection names as those properties are not exposed anywhere outside of the mapping system.
 * >> (14 Jun 2010) Matthew Cotter: Removed functions deprecated by use of FalconCommandParser class.
 * >> (10 Jun 2010) Matthew Cotter: Updated error messages to invoke the help command as well.
 * >> ( 7 Jun 2010) Matthew Cotter: Implemented use of the FalconCommandParser class to parse the command line and generate the commands.
 * >> ( 4 Jun 2010) Matthew Cotter: Added support for commands 'reset', 'ungroupall', 'unmapall', and 'unclusterall'.
 *                                    Updated script command to be 'batch' rather than 'script'.
 * >> ( 2 Jun 2010) Matthew Cotter: All output is done via calls to static void MappingParser.StandardWriteLine().   As of now, this just writes the string 
 *                                    to the console.  Future versions may be able to duplicate and/or redirect this output by modifying this function.
 *                                    Errors/Exceptions are reported to the console rather than have exceptions thrown.
 *                                    Implemented console-style feel for implementation testing.   Updated parsing to recognize 'quit' as
 *                                    command to terminate the shell, as well as pwd, cd, and ls commands for interactive convenience 
 *                                    and implementation testing.
 * >> ( 1 Jun 2010) Matthew Cotter: Implemented vast majority of command-line commands.   The trigger switches/options are not 
 *                                    yet finalized, so the current strings are only for implementation verification.
 * >> ( 1 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconMappingAlgorithm;
using System.Reflection;
using System.IO;
using FalconCommandLineParser;
using FalconGlobal;
using FalconGraph;
using FalconPathManager;
using CerebrumSharedClasses;

namespace FalconMapTool
{
    class Program 
    {
        private class MapToolWrapperClass
        {
            private const int OUTPUT_ID_PADDING = 3;
            private const int OUTPUT_NAME_PADDING = 15;

            public static void StandardWrite(string Text, params object[] list)
            {
                string outputText = Text;
                for (int i = 0; i < list.Length; i++)
                    if (list[i] != null)
                    {
                        outputText = outputText.Replace("{" + i.ToString() + "}", list[i].ToString());
                    }
                    else
                    {
                        outputText = outputText.Replace("{" + i.ToString() + "}", string.Empty);
                    }
                Console.Write(outputText);
            }
            public static void StandardWriteLine()
            {
                Console.WriteLine();
            }
            public static void StandardWriteLine(string Text, params object[] list)
            {
                StandardWrite(Text, list);
                StandardWriteLine();
            }

            public static bool IsBuiltInCommand(MapCommandEnum mce)
            {
                switch (mce)
                {
                    case MapCommandEnum.Help:
                        return true;
                    case MapCommandEnum.Version:
                        return true;
                    case MapCommandEnum.PrintWorkingDirectory:
                        return true;
                    case MapCommandEnum.ListDirectory:
                        return true;
                    case MapCommandEnum.ChangeDirectory:
                        return true;

                    default:
                        return false;
                }
            }

            public enum MapCommandEnum : int
            {
                InvalidCommand = 0,       // Default/dummy command.  NOP - halts execution when encountered in batch mode.
                                // Essentially indicates an invalid command.

                #region Built-in Commands
                Help,
                Version,
                ScriptFile,
                InteractiveMode,
                PrintWorkingDirectory,
                ListDirectory,
                ChangeDirectory,
                #endregion

                #region Cerebrum Input Files
                Cerebrum_InputFiles,
                //Cerebrum_PlatformInputFile,
                //Cerebrum_DesignInputFile,
                //Cerebrum_CommunicationInputFile,
                #endregion


                #region Enumeration/List Commands
                ListComponents,
                ListFPGAs,
                ListGroups,
                ListClusters,
                ListConnections,
                ListLinks,
                #endregion

                #region Input/Output/Execution Commands
                InputSystemFile,                        //    <Path to XML SystemFile>
                InputMappingFile,                       //    <Path to XML MappingFile>, <PreMapping/PostMapping>
                OutputSystemFile,                       //    <Path to XML SystemFile>
                OutputMappingFile,                      //    <Path to XML MappingFile>
                //OutputRoutingInformation,               //    <Path to XML RoutingFile>
                OutputXPSProjectInformation,            //    <Path to XML XPSProjectFile>
                GenerateRoutingXML,                     //    <Path to XML RoutingFile>
                DoMapping,                              //    No parameters
                GenerateAddressMap,
                #endregion

                Reset,                                  //    No parameters

                #region Component Commands
                AddComponent,                           //  * NewCID, NewName
                RemoveComponent,                        //  * CID
                ModifyComponentID,                      //  * CID, NewCID
                ModifyComponentName,                    //    CID, NewName
                ModifyComponentResource,                //    CID, Resource, Amount
                UnGroupAll,                             //    No parameters
                //GetComponentResource,                 //    CID, Resource
                //GetComponentName,                     //    CID
                //IsComponentGrouped,                   //    CID
                //GetComponentGroupID,                  //    CID
                //GetComponentFPGAID,                   //    CID
                //IsComponentMapped,                    //    CID
                #endregion

                #region FPGA Commands
                AddFPGA,                                //  * NewFID, NewName
                RemoveFPGA,                             //  * FID
                ModifyFPGAID,                           //  * FID, NewFID
                ModifyFPGAName,                         //    FID, NewName
                ModifyFPGAResource,                     //    FID, Resource, Amount
                UnClusterAll,                           //    No parameters
                //GetFPGAResource,                      //    FID, Resource
                //GetFPGAName,                          //    FID
                //IsFPGAClustered,                      //    FID
                //GetFPGAClusterID,                     //    FID
                //GetFPGAResourceUtilization,           //    FID, Resource
                //GetAverageFPGAResourceUtilization,    //    FID
                //GetAverageFPGAResourceUtilizationAll, //
                #endregion

                #region Group Commands
                AddGroup,                               //    NewGID, NewName
                RemoveGroup,                            //    GID
                ModifyGroupID,                          //  * GID, NewGID
                ModifyGroupName,                        //    GID, NewName
                AddComponentToGroup,                    //    CID, GID
                RemoveComponentFromGroup,               //    CID
                MapGroupToFPGA,                         //    GID, FID
                UnMapGroup,                             //    GID
                UnMapAll,                               //    No parameters
                //GetGroupName,                         //    GID
                //IsGroupMapped,                        //    GID
                //GetGroupTargetFPGAID,                 //    GID
                #endregion

                #region Cluster Commands
                AddFPGAToCluster,                     //    FID, CID
                RemoveFPGAFromCluster,                //    FID
                AddCluster,                           //    NewCID, NewName
                RemoveCluster,                        //    CID
                ModifyClusterID,                      //  * CID, NewCID
                ModifyClusterName,                    //    CID, NewName
                //GetClusterName,                       //    CID
                #endregion

                #region Connection Commands
                AddConnection,                          //    NewCID, NewName, Density, SourceCID, SinkCID
                RemoveConnection,                       //    CID
                ModifyConnectionID,                     //  * CID, NewCID
                ModifyConnectionName,                   //    CID, NewName
                ModifyConnectionDensity,                //    CID, NewDensity
                ModifyConnectionSource,                 //  * CID, NewSourceCID
                ModifyConnectionSink,                   //  * CID, NewSinkCID
                #endregion

                #region Link Commands
                AddLink,                                //    NewLID, NewName, Speed, SourceFID, SinkFID, Direction
                RemoveLink,                             //    LID
                ModifyLinkID,                           //  * LID, NewLID
                ModifyLinkName,                         //    LID, NewName
                ModifyLinkSpeed,                        //    LID, NewSpeed
                ModifyLinkSource,                       //  * LID, NewSourceFID
                ModifyLinkSink,                         //  * LID, NewSinkFID
                ModifyLinkBidirectionality,             //  * LID, Bidirectional
                ModifyLinkReverse                       //  * LID
                #endregion
            }

            private static FalconMapping_Objects MapObjects;
            private static FalconCommandParser parser;

            public MapToolWrapperClass()
            {
                MapObjects = new FalconMapping_Objects();
                parser = new FalconCommandParser();

                PopulateCommandTable();
            }

            private class MappingCommand
            {
                private MapCommandEnum _cmd;
                private ArrayList _args;
                private int _reqargs;
                private string _cmdstring;

                public MappingCommand(MapCommandEnum Cmd, int ReqArgs, string CommandString)
                {
                    _cmd = Cmd;
                    _args = new ArrayList();
                    _reqargs = ReqArgs;
                    _cmdstring = CommandString;
                }

                public MapCommandEnum Command
                {
                    get
                    {
                        return _cmd;
                    }
                }

                public int RequiredArgs
                {
                    get
                    {
                        return _reqargs;
                    }
                }

                public int ArgCount
                {
                    get
                    {
                        return _args.Count;
                    }
                }

                public string CommandString
                {
                    get
                    {
                        return _cmdstring;
                    }
                }

                public string GetArg(int i)
                {
                    if ((i > _args.Count) || (i < 0))
                    {
                        StandardWriteLine("\tFATAL Error: Argument index out of bounds\n");
                        return string.Empty;
                    }
                    return (string)_args[i];
                }

                public void AddArg(string arg)
                {
                    _args.Add(arg);
                }

                public bool Execute(FalconMapping_Objects MapObj)
                {
                    if (_args.Count < _reqargs)
                    {
                        StandardWriteLine("\tERROR: Insufficient Arguments\n");
                        return false;
                    }
                    switch (_cmd)
                    {
                        #region Input/Output/Execution Commands
                        case MapCommandEnum.InputSystemFile:                        //    <Path to XML SystemFile>
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ReadSystemFile(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.InputMappingFile:                       //    <Path to XML MappingFile>, <PreMapping/PostMapping>
                            if (ArgCount >= RequiredArgs + 1)
                            {
                                MapObj.ReadMappingFile(GetArg(0), (GetArg(1) == "0" ? true : false));
                            }
                            else if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ReadMappingFile(GetArg(0), true);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.OutputSystemFile:                       //    <Path to XML SystemFile>
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.WriteSystemFile(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.OutputMappingFile:                       //    <Path to XML MappingFile>, <PreMapping/PostMapping>
                            if (ArgCount >= RequiredArgs + 1)
                            {
                                MapObj.WriteMappingFile(GetArg(0), (GetArg(1) == "0" ? true : false), true);
                            }
                            else if (ArgCount >= RequiredArgs)
                            {
                                MapObj.WriteMappingFile(GetArg(0), false, true);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.OutputXPSProjectInformation:            //    <Path to XML XPSProjectFile>
                            if (ArgCount >= RequiredArgs)
                            {
                                if (MapObj.DoMapping())
                                {
                                    MapObj.WriteFinalOutputs();
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.GenerateRoutingXML:                     //    <Path to XML RoutingFile>
                            if (ArgCount >= RequiredArgs)
                            {
                                if (MapObj.DoMapping())
                                {
                                    MapObj.GenerateRouting(GetArg(0));
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                            
                        case MapCommandEnum.GenerateAddressMap:                     //    <Path to XML AddressMap>
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.WriteComponentAddressMap(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.DoMapping:                              //    No parameters
                            if (!MapObj.DoMapping())
                            {
                                StandardWriteLine("Mapping FAILED!");
                                return false;
                            }
                            break;
                        #endregion

                        case MapCommandEnum.Reset:                                  //    No parameters
                            MapObj.Reset();
                            break;
                            
                        #region Cerebrum Input Files
                        case MapCommandEnum.Cerebrum_InputFiles:
                            PathManager PMan = new PathManager(GetArg(0));
                            MapObj.LoadPathsFile(GetArg(0));
                            if (!MapObj.ReadProjectFiles(GetArg(1), GetArg(2)))  // Platform, Design, Communications
                            {
                                StandardWriteLine("Loading of Platform/Design/Communication from Cerebrum failed!");
                                return false;
                            }
                            else
                            {
                                if (MapObj.DoMapping())
                                {
                                    MapObj.WriteFinalOutputs();
                                    MapObj.WriteComponentAddressMap(PMan["LocalProjectRoot"] + "\\addressmap.xml");
                                    MapObj.WriteSystemFile(PMan["LocalProjectRoot"] + "\\system_spec.xml");
                                    MapObj.WriteMappingFile(PMan["LocalProjectRoot"] + "\\system_map.xml", false, false);
                                    MapObj.GenerateRouting(PMan["LocalProjectRoot"] + "\\routing.xml");
                                    MapObj.UpdateInstanceMappings(PMan["LocalProjectRoot"] + "\\design.xml", PMan["LocalProjectRoot"] + "\\comms.xml");
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            break;
                        //case MapCommandEnum.Cerebrum_PlatformInputFile:
                            //break;
                        //case MapCommandEnum.Cerebrum_DesignInputFile:
                            //break;
                        //case MapCommandEnum.Cerebrum_CommunicationInputFile:
                            //break;
                        #endregion

                        #region Component Commands
                        case MapCommandEnum.AddComponent:                           //  * NewCID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddComponent(GetArg(0), GetArg(1), GetArg(2), null);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveComponent:                        //  * CID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveComponent(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyComponentID:                      //  * CID, NewCID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyComponentID(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyComponentName:                    //    CID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyComponentName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyComponentResource:                //    CID, Resource, Amount
                            if (ArgCount >= RequiredArgs)
                            {
                                int resAmt;
                                if (int.TryParse(GetArg(2), out resAmt))
                                {
                                    MapObj.ModifyComponentResource(GetArg(0), GetArg(1), resAmt);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.UnGroupAll:                             //    No parameters
                                MapObj.UnGroupAll();
                            break;
                        //case MapCommandEnum.GetComponentResource:                 //    CID, Resource
                        //case MapCommandEnum.GetComponentName:                     //    CID
                        //case MapCommandEnum.IsComponentGrouped:                   //    CID
                        //case MapCommandEnum.GetComponentGroupID:                  //    CID
                        //case MapCommandEnum.GetComponentFPGAID:                   //    CID
                        //case MapCommandEnum.IsComponentMapped:                    //    CID
                        #endregion

                        #region FPGA Commands
                        case MapCommandEnum.AddFPGA:                                //  * NewFID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddFPGA(GetArg(0), GetArg(1), GetArg(2), null);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveFPGA:                             //  * FID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveFPGA(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyFPGAID:                           //  * FID, NewFID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyFPGAID(GetArg(0), GetArg(1));
                            }
                            break;
                        case MapCommandEnum.ModifyFPGAName:                         //    FID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyFPGAName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyFPGAResource:                     //    FID, Resource, Amount
                            if (ArgCount >= RequiredArgs)
                            {
                                int resAmt;
                                if (int.TryParse(GetArg(2), out resAmt))
                                {
                                    MapObj.ModifyFPGAResource(GetArg(0), GetArg(1), resAmt);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.UnClusterAll:                             //    No parameters
                            MapObj.UnClusterAll();
                            break;
                        //case MapCommandEnum.GetFPGAResource:                      //    FID, Resource
                        //case MapCommandEnum.GetFPGAName:                          //    FID
                        //case MapCommandEnum.IsFPGAClustered:                      //    FID
                        //case MapCommandEnum.GetFPGAClusterID:                     //    FID
                        //case MapCommandEnum.GetFPGAResourceUtilization:           //    FID, Resource
                        //case MapCommandEnum.GetAverageFPGAResourceUtilization:    //    FID
                        //case MapCommandEnum.GetAverageFPGAResourceUtilizationAll: //
                        #endregion
                            
                        #region Group Commands
                        case MapCommandEnum.AddGroup:                               //    NewGID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddGroup(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveGroup:                            //    GID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveGroup(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyGroupID:                          //  * GID, NewGID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyGroupID(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyGroupName:                        //    GID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyGroupName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.AddComponentToGroup:                    //    CID, GID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddComponentToGroup(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveComponentFromGroup:               //    CID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveComponentFromGroup(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.MapGroupToFPGA:                         //    GID, FID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.MapGroupToFPGA(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.UnMapGroup:                             //    GID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.UnMapGroup(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.UnMapAll:                               //    No parameters
                            MapObj.UnMapAll();
                            break;
                        //case MapCommandEnum.GetGroupName:                         //    GID
                        //case MapCommandEnum.IsGroupMapped:                        //    GID
                        //case MapCommandEnum.GetGroupTargetFPGAID:                 //    GID
                        #endregion

                        #region Cluster Commands
                        case MapCommandEnum.AddFPGAToCluster:                     //    FID, CID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddFPGAToCluster(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveFPGAFromCluster:                //    FID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveFPGAFromCluster(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.AddCluster:                           //    NewCID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.AddCluster(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveCluster:                        //    CID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveCluster(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyClusterID:                      //  * CID, NewCID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyClusterID(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyClusterName:                    //    CID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyClusterName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        //case MapCommandEnum.GetClusterName:                       //    CID
                        #endregion
                            
                        #region Connection Commands
                        case MapCommandEnum.AddConnection:                          //    NewCID, NewName, Density, SourceCID, SinkCID
                            if (ArgCount >= RequiredArgs)
                            {
                                double wt;
                                if (double.TryParse(GetArg(2), out wt))
                                {
                                    MapObj.AddConnection(GetArg(0), GetArg(1), wt, GetArg(3), GetArg(4));
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveConnection:                       //    CID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveConnection(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyConnectionID:                     //  * CID, NewCID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyConnectionID(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyConnectionName:                   //    CID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyConnectionName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyConnectionDensity:                //    CID, NewDensity
                            if (ArgCount >= RequiredArgs)
                            {
                                double wt;
                                if (double.TryParse(GetArg(1), out wt))
                                {
                                    MapObj.ModifyConnectionDensity(GetArg(0), wt);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyConnectionSource:                 //  * CID, NewSourceCID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyConnectionSource(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyConnectionSink:                   //  * CID, NewSinkCID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyConnectionSink(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        #endregion
                                                        
                        #region Link Commands
                        case MapCommandEnum.AddLink:                                //    NewLID, NewName, Speed, SourceFID, SinkFID, Direction
                            if (ArgCount >= RequiredArgs)
                            {
                                double wt;
                                if (double.TryParse(GetArg(2), out wt))
                                {
                                    MapObj.AddLink(GetArg(0), GetArg(1), wt, GetArg(3), GetArg(4), (GetArg(5) == "1" ? false : true));
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.RemoveLink:                             //    LID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.RemoveLink(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkID:                           //  * LID, NewLID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkID(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkName:                         //    LID, NewName
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkName(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkSpeed:                        //    LID, NewSpeed
                            if (ArgCount >= RequiredArgs)
                            {
                                double wt;
                                if (double.TryParse(GetArg(1), out wt))
                                {
                                    MapObj.ModifyLinkSpeed(GetArg(0), wt);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkSource:                       //  * LID, NewSourceFID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkSource(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkSink:                         //  * LID, NewSinkFID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkSink(GetArg(0), GetArg(1));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkBidirectionality:             //  * LID, Bidirectional
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkBidirectionality(GetArg(0), (GetArg(1) == "1" ? false : true));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case MapCommandEnum.ModifyLinkReverse:                      //  * LID
                            if (ArgCount >= RequiredArgs)
                            {
                                MapObj.ModifyLinkReverse(GetArg(0));
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        #endregion

                        // Break out on built-in commands
                        // Break out on enumeration commands
                        case MapCommandEnum.InvalidCommand:
                            StandardWriteLine("Invalid command: {0}.", _cmdstring);
                            return false;
                        default:
                            StandardWriteLine("Unimplemented command: {0}.", _cmdstring);
                            return false;
                    }
                    return true;
                }

                public int CompareTo(object other)
                {
                    // Return < 0 -> This "is less than" other
                    // Return = 0 -> This "is equal to" other
                    // Return > 0 -> This "is greater than" other

                    if (other.GetType() != typeof(MappingCommand))
                        return 0;

                    MappingCommand This = this;
                    MappingCommand Other = (MappingCommand)other;

                    if (This.Command == Other.Command)
                        return 0;

                    //Sort invalids to the front
                    if (This.Command == MapCommandEnum.InvalidCommand)
                        return -1;

                    //Sort invalids to the front
                    if (Other.Command == MapCommandEnum.InvalidCommand)
                        return 1;

                    string ThisCommand = This.Command.ToString();
                    string OtherCommand = Other.Command.ToString();
                    bool ThisIsCereb = ThisCommand.StartsWith("Cerebrum");
                    bool ThisIsBuiltIn = MapToolWrapperClass.IsBuiltInCommand(This.Command);
                    bool OtherIsBuiltIn = MapToolWrapperClass.IsBuiltInCommand(Other.Command);

                    if (ThisIsCereb)
                        return -1;

                    // Sort built-in commands
                    if ((ThisIsBuiltIn) && (OtherIsBuiltIn))
                        return 0;
                    if (ThisIsBuiltIn)
                        return -1;
                    if (OtherIsBuiltIn)
                        return 1;

                    return 0;
                }
            }
            private MappingCommand CreateFromString(string cmd)
            {
                MappingCommand mc = null;
                cmd = cmd.ToLower().Trim('-');

                // The command table is first checked to ensure that the command string exists
                // If it does not exist, the command is interpreted as InvalidCommand, or a NOP.
                // When it's executed, an error will be displayed and if executing in batch-mode, execution will terminate.
                // If running in interactive mode, the state will be that of the system IMMEDIATELY before the invalid command.
                if (CommandTable.ContainsKey(cmd))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[cmd];
                    mc = new MappingCommand(ci.Command, ci.MinArguments, cmd);
                }
                else
                {
                    mc = new MappingCommand(MapCommandEnum.InvalidCommand, 0, cmd);
                }
                return mc;
            }
            
            private class CommandInfo
            {
                private int HELPLINE_LENGTH = 70;
                public MapCommandEnum Command;
                public string Literal;
                public string Help;
                public int MinArguments;
                
                public CommandInfo(MapCommandEnum ID, string CmdLiteral, int RequiredArgs, string HelpText)
                {
                    Command = ID;
                    Literal = CmdLiteral;
                    MinArguments = RequiredArgs;

                    StringBuilder helpBuilder = new StringBuilder();
                    Help = String.Format("{0} {1}", Literal, HelpText);
                    string[] help = Help.Split('\n');
                    for (int i = 0; i < help.Length; i++)
                    {
                        string hline = help[i].Trim();
                        if (i == 0)
                        {
                            helpBuilder.AppendLine(hline);
                        }
                        else if (hline.Length < HELPLINE_LENGTH)
                        {
                            helpBuilder.AppendLine("\t" + hline);
                        }
                        else
                        {
                            while (hline.Length > HELPLINE_LENGTH)
                            {
                                int lastsp = hline.IndexOf(" ", (int)(0.8 * HELPLINE_LENGTH));
                                string linestart;
                                if (lastsp >= 0)
                                {
                                    linestart = hline.Substring(0, lastsp).Trim();
                                    hline = hline.Substring(lastsp + 1).Trim();
                                }
                                else
                                {
                                    linestart = hline;
                                    hline = string.Empty;
                                }
                                helpBuilder.AppendLine("\t" + linestart);
                            }
                            if (hline.Length > 0)
                            {
                                helpBuilder.AppendLine("\t" + hline);
                            }
                        }
                    }
                    Help = helpBuilder.ToString();
                }
            }
            
            private Hashtable CommandTable;
            private void AddCommand(string Literal, MapCommandEnum ID, int ReqArgs, string HelpInfo)
            {
                CommandTable.Add(Literal, new CommandInfo(ID, Literal, ReqArgs, HelpInfo));
            }
            private void PopulateCommandTable()
            {
                CommandTable = new Hashtable();

                #region Built-in Commands
                AddCommand("help", MapCommandEnum.Help, 0,
                    "\n      Displays the command help screen.");
                AddCommand("version", MapCommandEnum.Version, 0,
                    "\n      Displays the current version of the mapping tool.");
                AddCommand("batch", MapCommandEnum.ScriptFile, 0,
                    "<file_path>\n      Executes a series of command as listed in the specified script file.  Must be the FIRST command in the sequence, and all other commands are ignored.  If it is not the first command, the script file is ignored.");

                AddCommand("console", MapCommandEnum.InteractiveMode, 0,
                    "\n      Start the map tool in interactive mode.");
               
                AddCommand("pwd", MapCommandEnum.PrintWorkingDirectory, 0,
                    "\n      Displays the current working directory.");
                AddCommand("ls", MapCommandEnum.ListDirectory, 0,
                    "\n      Displays the contents of the current directory.");
                AddCommand("cd", MapCommandEnum.ChangeDirectory, 1,
                    "\n      Changes the current working directory.");
                #endregion
                
                #region Enumeration/List/Reset Commands
                AddCommand("lscomp", MapCommandEnum.ListComponents, 0, 
                    "\n      Enumerates all Components as currently loaded in the system.");
                AddCommand("lsfpga", MapCommandEnum.ListFPGAs, 0, 
                    "\n      Enumerates all FPGAs as currently loaded in the system.");
                AddCommand("lsgroup", MapCommandEnum.ListGroups, 0, 
                    "\n      Enumerates all Groups as currently loaded in the system.");
                AddCommand("lscluster", MapCommandEnum.ListClusters, 0, 
                    "\n      Enumerates all Clusters as currently loaded in the system.");
                AddCommand("lsconn", MapCommandEnum.ListConnections, 0, 
                    "\n      Enumerates all Connections as currently loaded in the system.");
                AddCommand("lslink", MapCommandEnum.ListLinks, 0,
                    "\n      Enumerates all Links as currently loaded in the system.");
                AddCommand("reset", MapCommandEnum.Reset, 0,
                    "\n      Resets ALL information in the mapping system, clearing all loaded objects and their state.");
                #endregion

                #region "Cerebrum Input Files"
                AddCommand("cerebinput", MapCommandEnum.Cerebrum_InputFiles, 3,
                    "<paths_file> <design_file> <comms_file>\n      Specifies the project paths, platform, design, and communication files upon which the system is to be built.  NOTE: This command overrides 'insfile' and 'inmfile' commands.");
                //AddCommand("platform", MapCommandEnum.Required_PlatformInputFile, 1,
                //    "<platform_file>\n      Specifies the platform file upon which the system is to be built.");
                //AddCommand("comms", MapCommandEnum.Required_CommunicationInputFile, 1,
                //    "<design_file>\n      Specifies the file from which inter-FPGA communication links in the system are to be drawn..");
                //AddCommand("design", MapCommandEnum.Required_DesignInputFile, 1,
                //    "<comms_file>\n      Specifies the design file from which the system design is to be extracted.");
                #endregion

                #region Input/Output/Execution Commands
                AddCommand("insfile", MapCommandEnum.InputSystemFile, 1, 
                    "<file_path>\n      Specifies the input file describing the system to be processsed.");
                AddCommand("inmfile", MapCommandEnum.InputMappingFile, 1,
                    "<file_path> <Default=0/1>\n      Specifies the input file describing the pre-determined mappings to be processsed and whether those mappings should be read from the <PreMapping> (0) or <PostMapping> (1) section of the file.");
                AddCommand("outsfile", MapCommandEnum.OutputSystemFile, 1,
                    "<file_path>\n      Specifies the output file describing the system after commands have been processed.");
                AddCommand("outmfile", MapCommandEnum.OutputMappingFile, 1,
                    "<file_path> <Default=1/0>\n      Specifies the output file describing any pre-determined mappings after commands have been processed and whether those mappings should be written to the <PreMapping> (0) or <PostMapping> (1) section of the file.");
                //AddCommand("routefile", MapCommandEnum.OutputRoutingInformation, 1,
                //    "<file_path>\n      Specifies the output file to be used for generation of FPGA routing tables after mapping is completed.");
                AddCommand("xpsfile", MapCommandEnum.OutputXPSProjectInformation , 1,
                    "\n      Creates the output file to be used for generation of XPS Project and Synthesis after mapping is completed.");
                AddCommand("routegen", MapCommandEnum.GenerateRoutingXML, 1,
                    "<file_path>\n      Specifies the output file to be generated for FPGA routing tables.");
                AddCommand("addrmap", MapCommandEnum.GenerateAddressMap, 1,
                    "<file_path>\n      Specifies the output file to be generated for Component Address Map.");
                AddCommand("map", MapCommandEnum.DoMapping, 0,
                    "\n      Executes the component mapping algorithm based on the current system state at the time of execution.");
                
                
                #endregion

                #region Component Commands
                //AddCommand("newcomp", MapCommandEnum.AddComponent, 3,
                //    "<new_id> <new_name> \"<supported_arches>\"\n      Adds a new component to the mapping system with the specified name and ID, if doing so would not create an ID-conflict.");
                //AddCommand("rmcomp", MapCommandEnum.RemoveComponent, 1,
                //    "<id>\n      Removes the component with the specified ID from the mapping system, if it exists.");
                //AddCommand("modcompid", MapCommandEnum.ModifyComponentID, 2,
                //    "<id> <new_id>\n      Changes the ID of the component within the mapping system, if doing so would not create an ID-conflict.");
                //AddCommand("modcompname", MapCommandEnum.ModifyComponentName, 2,
                //    "<id> <new_name>\n      Changes the name of the component within the mapping system.");
                //AddCommand("modcompres", MapCommandEnum.ModifyComponentResource, 3,
                //    "<id> <resource> <amount>\n      Sets the amount of a particular resource required by a component within the mapping system.");
                AddCommand("ungroupall", MapCommandEnum.UnGroupAll, 0,
                    "\n      Removes all groupings from the mapping system, resulting in all components being ungrouped.");
                #endregion

                #region FPGA Commands
                //AddCommand("newfpga", MapCommandEnum.AddFPGA, 3,
                //    "<new_id> <new_name> <new_family>\n      Adds a new FPGA to the mapping system with the specified ID, name, and architecture family, if doing so would not create an ID-conflict.");
                //AddCommand("rmfpga", MapCommandEnum.RemoveFPGA, 1,
                //    "<id>\n      Removes the FPGA with the specified ID from the mapping system, if it exists.");
                //AddCommand("modfpgaid", MapCommandEnum.ModifyFPGAID, 2,
                //    "<id> <new_id>\n      Changes the ID of the FPGA within the mapping system, if doing so would not create an ID-conflict.");
                //AddCommand("modfpganame", MapCommandEnum.ModifyFPGAName, 2,
                //    "<id> <new_name>\n      Changes the name of the FPGA within the mapping system.");
                //AddCommand("modfpgares", MapCommandEnum.ModifyFPGAResource, 3,
                //    "<id> <resource> <amount>\n      Sets the total amount of a particular resource contained on an FPGA within the mapping system.");
                AddCommand("unclusterall", MapCommandEnum.UnClusterAll, 0,
                    "\n      Removes all clusterings from the mapping system, resulting in all FPGAs being unclustered.");
                #endregion 

                #region Group Commands
                AddCommand("newgroup", MapCommandEnum.AddGroup, 2,
                    "<new_id> <new_name>\n      Adds a new Group to the mapping system with the specified name and ID, if doing so would not create an ID-conflict.");
                AddCommand("rmgroup", MapCommandEnum.RemoveGroup, 1,
                    "<id>\n      Removes the Group with the specified ID from the mapping system, if it exists.");
                //AddCommand("modgroupid", MapCommandEnum.ModifyGroupID, 2,
                //    "<id> <new_id>\n      Changes the ID of the Group within the mapping system, if doing so would not create an ID-conflict.");
                //AddCommand("modgroupname", MapCommandEnum.ModifyGroupName, 2,
                //    "<id> <new_name>\n      Changes the name of the Group within the mapping system.");

                AddCommand("groupcomp", MapCommandEnum.AddComponentToGroup, 2,
                    "<comp_id> <group_id>\n      Adds the the component <comp_id> to the group <group_id> if both exist.");
                AddCommand("ungroupcomp", MapCommandEnum.RemoveComponentFromGroup, 1,
                    "<comp_id>\n      Removes <comp_id> from the group it's in, if its grouped.");
                AddCommand("mapgroup", MapCommandEnum.MapGroupToFPGA, 2,
                    "<group_id> <fpga_id>\n      Maps group <group_id> to FPGA <fpga_id>, if there is sufficient free resources.");
                AddCommand("unmapgroup", MapCommandEnum.UnMapGroup, 1,
                    "<group_id>\n      Unmaps group <group_id> from the FPGA it's mapped to, if its mapped.");
                AddCommand("unmapall", MapCommandEnum.UnMapAll, 0,
                    "\n      Removes all mappings from the mapping system, resulting in all groups being unmapped.");

                #endregion

                #region Cluster Commands
                AddCommand("clusterfpga", MapCommandEnum.AddFPGAToCluster, 2,
                    "<fpga_id> <cluster_id>\n      Adds the the FPGA <fpga_id> to the cluster <cluster_id> if both exist.");
                AddCommand("unclusterfpga", MapCommandEnum.RemoveFPGAFromCluster, 1,
                    "<fpga_id>\n      Removes FPGA <fpga_id> from the cluster it's in, if its grouped.");
                AddCommand("newcluster", MapCommandEnum.AddCluster, 2,
                    "<new_id> <new_name>\n      Adds a new Cluster to the mapping system with the specified name and ID, if doing so would not create an ID-conflict.");
                AddCommand("rmcluster", MapCommandEnum.RemoveCluster, 1,
                    "<id>\n      Removes the Cluster with the specified ID from the mapping system, if it exists.");
                //AddCommand("modclusterid", MapCommandEnum.ModifyClusterID, 2,
                //    "<id> <new_id>\n      Changes the ID of the Cluster within the mapping system, if doing so would not create an ID-conflict.");
                AddCommand("modclustername", MapCommandEnum.ModifyClusterName, 2,
                    "<id> <new_name>\n      Changes the name of the Cluster within the mapping system.");
                #endregion

                #region Connection Commands
                AddCommand("newconn", MapCommandEnum.AddConnection, 5,
                    "<new_id> <new_name> <density> <source_id> <sink_id>\n      Adds a new Connection to the mapping system with the specified name and ID, if doing so would not create an ID-conflict, connecting components <source_id> and <sink_id>.");
                AddCommand("rmconn", MapCommandEnum.RemoveConnection, 1,
                    "<id>\n      Removes the Connection with the specified ID from the mapping system, if it exists.");
                //AddCommand("modconnid", MapCommandEnum.ModifyConnectionID, 2,
                //    "<id> <new_id>\n      Changes the ID of the Connection within the mapping system, if doing so would not create an ID-conflict.");
                //AddCommand("modconnname", MapCommandEnum.ModifyConnectionName, 2,
                //    "<id> <new_name>\n      Changes the name of the Connection within the mapping system.");
                AddCommand("modconndensity", MapCommandEnum.ModifyConnectionDensity, 2,
                    "<id> <new_density>\n      Changes the data density of the Connection within the mapping system.");
                AddCommand("modconnsource", MapCommandEnum.ModifyConnectionSource, 2,
                    "<id> <source_id>\n      Changes the source component of the Connection within the mapping system to <source_id>.");
                AddCommand("modconnsink", MapCommandEnum.ModifyConnectionSink, 2,
                    "<id> <sink_id>\n      Changes the sink component of the Connection within the mapping system to <sink_id>.");
                #endregion

                #region Link Commands
                AddCommand("newlink", MapCommandEnum.AddLink, 6,
                    "<new_id> <new_name> <speed> <source_id> <sink_id> <bi/uni>\n      Adds a new Link to the mapping system with the specified name and ID, if doing so would not create an ID-conflict, connecting FPGAs <source_id> and <sink_id> with either a Bi- or Uni-directional link.");
                AddCommand("rmlink", MapCommandEnum.RemoveLink, 1,
                    "<id>\n      Removes the Link with the specified ID from the mapping system, if it exists.");
                //AddCommand("modlinkid", MapCommandEnum.ModifyLinkID, 2,
                //    "<id> <new_id>\n      Changes the ID of the Link within the mapping system, if doing so would not create an ID-conflict.");
                //AddCommand("modlinkname", MapCommandEnum.ModifyLinkName, 2,
                //    "<id> <new_name>\n      Changes the name of the Link within the mapping system.");
                AddCommand("modlinkspeed", MapCommandEnum.ModifyLinkSpeed, 2,
                    "<id> <new_speed>\n      Changes the transfer speed of the Link within the mapping system.");
                AddCommand("modlinksource", MapCommandEnum.ModifyLinkSource, 2,
                    "<id> <source_id>\n      Changes the source FPGA of the Link within the mapping system to <source_id>.");
                AddCommand("modlinksink", MapCommandEnum.ModifyLinkSink, 2,
                    "<id> <sink_id>\n      Changes the sink FPGA of the Link within the mapping system to <sink_id>.");
                AddCommand("modlinkbidir", MapCommandEnum.ModifyLinkBidirectionality, 2,
                    "<id> <2/1>\n      Changes the directionality of the Link within the mapping system to Bi-directional (2) or Uni-directional (1).");
                AddCommand("modlinkrev", MapCommandEnum.ModifyLinkReverse, 1,
                    "<id>\n      Changes the directionality of the Link within the mapping system by swapping the source and sink FPGAs.  Only useful if the link is uni-directional.");
                #endregion
            }

            private void DisplayHelpInfo()
            {
                ArrayList keys = new ArrayList();
                keys.AddRange(CommandTable.Keys);
                keys.Sort();
                for (int i = 0; i < keys.Count; i++)
                {
                    CommandInfo ci = (CommandInfo)CommandTable[keys[i]];
                    StandardWriteLine(ci.Help);
                }
            }
            private void DisplayHelpInfo(string command)
            {
                if (CommandTable.ContainsKey(command))
                {
                    CommandInfo ci = (CommandInfo)CommandTable[command];
                    StandardWriteLine(ci.Help);
                }
                else
                {
                    StandardWriteLine("Unrecognized command: {0}\n", command);
                    DisplayHelpInfo();
                }
            }

            private static void ChangeDirectory(MappingCommand mc)
            {
                if (mc.ArgCount > 0)
                {
                    StringBuilder path = new StringBuilder();
                    for (int a = 0; a < mc.ArgCount; a++)
                    {
                        path.Append(mc.GetArg(a) + " ");
                    }
                    string newdir = path.ToString().Trim();
                    if (System.IO.Directory.Exists(newdir))
                    {
                        System.IO.Directory.SetCurrentDirectory(newdir);
                        StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                    }
                    else
                    {
                        StandardWriteLine("Unable to change directory: {0} was not found", newdir);
                    }
                }
                else
                {
                    System.IO.Directory.SetCurrentDirectory(
                        new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName);
                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                }
            }
            private static void ListDirectory()
            {
                string[] dirs = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory());
                string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory());
                ArrayList alDirs = new ArrayList();
                alDirs.AddRange(dirs);
                alDirs.Sort();
                ArrayList alFiles = new ArrayList();
                alFiles.AddRange(files);
                alFiles.Sort();

                StringBuilder listing = new StringBuilder();
                listing.AppendLine();
                listing.AppendLine("Directories:");
                int row = 0;
                for (int d = 0; d < alDirs.Count; d++)
                {
                    DirectoryInfo di = new DirectoryInfo((string)alDirs[d]);
                    string shortname = di.Name + "\\";
                    shortname = shortname.PadRight(30);
                    if (row == 0)
                        listing.Append("\t");
                    listing.Append(shortname);
                    row = (row + 1) % 2;
                    if (row == 0)
                        listing.AppendLine();
                }
                listing.AppendLine();
                listing.AppendLine();
                listing.AppendLine("Files:");
                row = 0;
                for (int f = 0; f < alFiles.Count; f++)
                {
                    FileInfo fi = new FileInfo((string)alFiles[f]);
                    string shortname = fi.Name;
                    shortname = shortname.PadRight(30);
                    if (row == 0)
                        listing.Append("\t");
                    listing.Append(shortname);
                    row = (row + 1) % 2;
                    if (row == 0)
                        listing.AppendLine();
                }
                listing.AppendLine();
                StandardWriteLine(listing.ToString());
            }

            private void ListComponents(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_Component> objectTable = MapObj.GetComponents();
                StandardWriteLine("Component List");
                foreach (FalconMapping_Component obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Name: {1} \t Grouped?: {2}{3}", obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), obj.Name.PadLeft(OUTPUT_NAME_PADDING, ' '), obj.IsGrouped, (obj.IsGrouped ? "  in " + obj.GroupID.PadLeft(OUTPUT_ID_PADDING, ' ') : string.Empty));
                }
                StandardWriteLine("{0} components listed", objectTable.Keys.Count);
            }
            private void ListFPGAs(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_FPGA> objectTable = MapObj.GetFPGAs();
                StandardWriteLine("FPGA List");
                foreach (FalconMapping_FPGA obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Name: {1} \t Arch: {3} \tUtilization: {2}", 
                        obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), 
                        obj.Name.PadLeft(OUTPUT_NAME_PADDING, ' '), 
                        MapObj.GetAverageFPGAResourceUtilization(obj.ID).ToString("##0.00"), 
                        obj.Architecture);
                }
                StandardWriteLine("{0} FPGAs listed", objectTable.Keys.Count);
            }
            private void ListGroups(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_Group> objectTable = MapObj.GetGroups();
                StandardWriteLine("Group List");
                foreach(FalconMapping_Group obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Name: {1} \t Mapped?: {2}{3}", obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), obj.Name.PadLeft(OUTPUT_NAME_PADDING, ' '), obj.IsMapped, (obj.IsMapped ? "  on " + obj.TargetFPGA.PadLeft(OUTPUT_ID_PADDING, ' ') : string.Empty));
                }
                StandardWriteLine("{0} groups listed", objectTable.Keys.Count);
            }
            private void ListClusters(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_Cluster> objectTable = MapObj.GetClusters();
                StandardWriteLine("Cluster List");
                foreach (FalconMapping_Cluster obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Name: {1}", obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), obj.Name.PadLeft(OUTPUT_NAME_PADDING, ' '));
                }
                StandardWriteLine("{0} clusters listed", objectTable.Keys.Count);
            }
            private void ListConnections(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_Connection> objectTable = MapObj.GetConnections();
                StandardWriteLine("Connection List");
                foreach (FalconMapping_Connection obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Density: {1} \t Connecting {2} to {3}", obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), obj.DataDensity.ToString("##0.000"), obj.SourceComponent.PadLeft(OUTPUT_ID_PADDING, ' '), obj.SinkComponent.PadLeft(OUTPUT_ID_PADDING, ' '));
                }
                StandardWriteLine("{0} connections listed", objectTable.Keys.Count);
            }
            private void ListLinks(FalconMapping_Objects MapObj)
            {
                Dictionary<string, FalconMapping_Link> objectTable = MapObj.GetLinks();
                StandardWriteLine("Link List");
                foreach (FalconMapping_Link obj in objectTable.Values)
                {
                    StandardWriteLine("\tID {0} - Speed: {1} \t Connecting {2} to {3}", obj.ID.PadLeft(OUTPUT_ID_PADDING, ' '), obj.LinkSpeed.ToString("##0.000"), obj.SourceFPGA.PadLeft(OUTPUT_ID_PADDING, ' '), obj.SinkFPGA.PadLeft(OUTPUT_ID_PADDING, ' '));
                }
                StandardWriteLine("{0} links listed", objectTable.Keys.Count);
            }

            private bool SortSequence(LinkedList<FalconCommand> FalconCommands, out LinkedList<MappingCommand> MapCommands, out string FailedCommand)
            {
                ArrayList cmdList = new ArrayList();
                MapCommands = new LinkedList<MappingCommand>();
                FailedCommand = string.Empty;
                bool bContainsCerebrumCommand = false;
                bool bIssueWarning = false;

                // Create set of JProgCommands and place them into an ArrayList
                for (LinkedListNode<FalconCommand> fcNode = FalconCommands.First; fcNode != null; fcNode = fcNode.Next)
                {
                    FalconCommand fc = fcNode.Value;
                    FailedCommand = fc.CommandSwitch;
                    MappingCommand mc = CreateFromString(fc.CommandSwitch);
                    if (mc.Command != MapCommandEnum.InvalidCommand)
                    {
                        for (LinkedListNode<string> node = fc.Arguments.First; node != null; node = node.Next)
                        {
                            mc.AddArg(node.Value);
                        }
                        cmdList.Add(mc);
                        FailedCommand = mc.CommandString;
                        if (mc.RequiredArgs > mc.ArgCount)
                        {
                            return false;
                        }
                        if (mc.Command == MapCommandEnum.Cerebrum_InputFiles)
                            bContainsCerebrumCommand = true;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Insertion Sort into LinkedList<JProgCommand>
                while (cmdList.Count > 0)
                {
                    MappingCommand thisMC = (MappingCommand)cmdList[0];

                    if (!((bContainsCerebrumCommand) && ((thisMC.Command == MapCommandEnum.InputMappingFile) || (thisMC.Command == MapCommandEnum.InputSystemFile))))
                    {
                        if (MapCommands.Count == 0)
                        {
                            MapCommands.AddLast(thisMC);
                        }
                        else
                        {
                            LinkedListNode<MappingCommand> mcNode;
                            bool bAdded = false;
                            for (mcNode = MapCommands.First; mcNode != null; mcNode = mcNode.Next)
                            {
                                int cmp = thisMC.CompareTo(mcNode.Value);
                                if (cmp < 0)
                                {
                                    // this should come before Node
                                    MapCommands.AddBefore(mcNode, thisMC);
                                    bAdded = true;
                                    break;
                                }
                                else if (cmp > 0)
                                {
                                    // this should come after Node
                                }
                                else
                                {
                                    // These are the same, add this one before to maintain order as default
                                    MapCommands.AddBefore(mcNode, thisMC);
                                    bAdded = true;
                                    break;
                                }
                            }
                            if (!bAdded)
                            {
                                if (mcNode == null)
                                {
                                    MapCommands.AddLast(thisMC);
                                }
                                else
                                {
                                    MapCommands.AddAfter(mcNode, thisMC);
                                }
                            }
                        }
                    }
                    else 
                    {
                        bIssueWarning = true;
                    }
                    cmdList.Remove(thisMC);
                }
                FailedCommand = string.Empty;
                if ((bIssueWarning) && (bContainsCerebrumCommand))
                {
                    StandardWriteLine("   WARNING: --cerebinput command overrides 'insfile' and 'inmfile' commands.\n");
                }
                return true;
            }

            public bool RunSequence(LinkedList<FalconCommand> fCommands)
            {
                MappingCommand mc = null;
                try
                {
                    if (fCommands.Count == 0)
                        return true;
                    LinkedList<MappingCommand> MapCommands; ;

                    string failedCmd;
                    if (!SortSequence(fCommands, out MapCommands, out failedCmd))
                    {
                        DisplayHelpInfo(failedCmd);
                        StandardWriteLine("Error parsing command ({0}) in sequence.", failedCmd);
                        return false;
                    }

                    for (LinkedListNode<FalconCommand> node = fCommands.First; node != null; node = node.Next)
                    {
                        FalconCommand fc = node.Value;
                        mc = CreateFromString(fc.CommandSwitch);
                        if (mc != null)
                        {
                            for (LinkedListNode<string> argNode = fc.Arguments.First; argNode != null; argNode = argNode.Next)
                                mc.AddArg(argNode.Value);

                            if (mc.ArgCount < mc.RequiredArgs)
                            {
                                return false;
                            }
                            switch (mc.Command)
                            {
                                case MapCommandEnum.InvalidCommand:
                                    DisplayHelpInfo();
                                    StandardWriteLine("Unrecognized command : {0}.\n", mc.CommandString);
                                    if (fCommands.Count > 1)
                                        StandardWriteLine("Batch execution halted.\n\n");
                                    return false;
                                case MapCommandEnum.Help:
                                    if (mc.ArgCount > 0)
                                        DisplayHelpInfo(mc.GetArg(0));
                                    else
                                        DisplayHelpInfo();
                                    break;
                                case MapCommandEnum.Version:
                                    DisplayVersionInfo();
                                    break;
                                case MapCommandEnum.ScriptFile:
                                    break;
                                case MapCommandEnum.InteractiveMode:
                                    break;
                                case MapCommandEnum.PrintWorkingDirectory:
                                    StandardWriteLine(System.IO.Directory.GetCurrentDirectory());
                                    break;
                                case MapCommandEnum.ListDirectory:
                                    ListDirectory();
                                    break;
                                case MapCommandEnum.ChangeDirectory:
                                    ChangeDirectory(mc);
                                    break;


                                case MapCommandEnum.ListComponents:
                                    ListComponents(MapObjects);
                                    break;
                                case MapCommandEnum.ListFPGAs:
                                    ListFPGAs(MapObjects);
                                    break;
                                case MapCommandEnum.ListGroups:
                                    ListGroups(MapObjects);
                                    break;
                                case MapCommandEnum.ListClusters:
                                    ListClusters(MapObjects);
                                    break;
                                case MapCommandEnum.ListConnections:
                                    ListConnections(MapObjects);
                                    break;
                                case MapCommandEnum.ListLinks:
                                    ListLinks(MapObjects);
                                    break;
                                case MapCommandEnum.Reset:
                                    MapObjects.Reset();
                                    StandardWriteLine("ALL mapping information has been cleared.");
                                    break;


                                default:
                                    if (!mc.Execute(MapObjects))
                                    {
                                        DisplayHelpInfo(mc.CommandString);
                                        return false;
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (MapObjects.ExProcessor != null)
                        MapObjects.ExProcessor(ex, String.Format("An error occurred executing command '{0}'.", mc.CommandString));
                    else
                        MapObjects.ExProcessor(ex, "An unknown error occurred while processing commands");
                    return false;
                }
                return true;
            }
            public void DisplayVersionInfo()
            {
                try
                {
                    //string DLLver = System.Reflection.Assembly.GetAssembly(typeof(FalconMapping_Objects)).GetName().Version.ToString(4);
                    //string EXEver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
                    //StandardWriteLine("Version Information:\n\tFalcon Component Mapping Tool Version: {0}\n\tFalcon Component Mapping Algorithm Version: {1}", EXEver, DLLver);
                    if (MapObjects is IFalconLibrary)
                    {
                        IFalconLibrary IFalconDLL = (IFalconLibrary)MapObjects;
                        StandardWriteLine(IFalconDLL.GetFalconComponentVersion());
                    }
                    else
                    {
                        StandardWriteLine("{0} does not implement IFalconLibrary.", MapObjects.GetType().ToString());
                    }
                }
                catch (Exception ex)
                {
                    if (MapObjects.ExProcessor != null)
                        MapObjects.ExProcessor(ex, "Unable to poll component version information.");
                }
            }


            /// <summary>
            /// Writes the MapTool command prompt to the StandardWriteLine
            /// </summary>
            public void Prompt()
            {
                StandardWrite("map> ");
            }

            /// <summary>
            /// Writes the initial shell welcome to the System.Console.
            /// </summary>
            public void ShellWelcome()
            {
                Console.WriteLine(((FalconMapping_Objects)(MapObjects)).GetFalconComponentVersion());
                Console.WriteLine();
            }

            /// <summary>
            /// Displays the interactive-mode welcome to the Console.
            /// </summary>
            public void InteractiveWelcome()
            {
                Console.WriteLine("Type 'batch <script_file>' to parse a script file.");
                Console.WriteLine("Type 'quit' or 'exit' to terminate.");
                Console.WriteLine();
            }

            /// <summary>
            /// Determines whether the specified string is a built-in Quit or Exit command.
            /// </summary>
            /// <param name="InputString">The raw string read in from input to be tested.</param>
            /// <returns>True if the string is a match for a quit command, False otherwise.</returns>
            public bool IsQuitCommand(string InputString)
            {
                return ((InputString == "quit") || (InputString == "exit"));
            }
        }

        private static MapToolWrapperClass MP;
        private static FalconCommandParser parser;
        private static bool bInteractiveMode = false;

        static int Main(string[] args)
        {
            MP = new MapToolWrapperClass();
            parser = new FalconCommandParser();
            MP.ShellWelcome();

            if (args.Length > 0)
            {
                string argline = "\"" + string.Join("\" \"", args) + "\"";

                return ParseLine(argline);
            }
            // No arguments passed, just terminate
            return 0;
        }

        private static int ParseLine(string argline)
        {
            if (parser.ParseString(argline))
            {
                bool success = true;
                LinkedList<FalconCommand> llCommands;
                llCommands = parser.Commands;
                if (llCommands.Count > 0)
                {
                    // Hard-coded batch command
                    FalconCommand firstCommand = llCommands.First.Value;
                    if (firstCommand.CommandSwitch.ToLower() == "batch")
                    {
                        if (firstCommand.Arguments.Count > 0)
                        {
                            if (parser.ParseFile(firstCommand.Arguments.First.Value))
                            {
                                llCommands = parser.Commands;
                                MP.RunSequence(llCommands);
                                success = success && MP.RunSequence(llCommands);
                                if (success)
                                    return 0;
                                else
                                    return -1;
                            }
                            else
                            {
                                Console.WriteLine("Encountered an error in parsing the script file.");
                                return -1;
                            }
                        }
                        else
                        {
                            // No file specified to batch argument, remove it
                            llCommands.RemoveFirst();
                            return 0;
                        }
                    }
                    else if (firstCommand.CommandSwitch.ToLower() == "console")
                    {
                        // Start interactive mode
                        InteractiveMode();
                        return 0;
                    }
                    else
                    {
                        // First command is not a batch or console command
                        success = success && MP.RunSequence(llCommands);
                        if (success)
                            return 0;
                        else
                            return -1;
                    }
                }
                // Parser succeeded, no commands found
            }
            else
            {
                Console.WriteLine("Encountered an error in parsing the command string.");
                return -1;
            }
            return 0;
        }

        public static void InteractiveMode()
        {
            if (bInteractiveMode)
                return;
            bInteractiveMode = true;
            MP.InteractiveWelcome();
            MP.Prompt();
            string input = Console.ReadLine();
            while ((input != "quit") && (input != "exit"))
            {
                while (input.EndsWith(" \\"))
                    input = input + Environment.NewLine + Console.ReadLine();
                if (!input.StartsWith("--"))
                {
                    input = "--" + input;
                }
                ParseLine(input);
                MP.Prompt();
                input = Console.ReadLine();
            }
        }
    }
}
