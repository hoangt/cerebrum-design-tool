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
///////////////////////////////////////////////
// File Name : MHSConfig.cs
// Created By: Abdulrahman Abumurad
// Date : 5/25/2010
// 
// Updated : 6/30/2010 : Corrected code to extract cores from MHS by instance AND type rather than only type.
// Updated : 6/10/2010
//                  
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Class that integrates custom core configuration parameters for a particular core instance into an existing MHS file.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/XpsBuilder Specification.pdf">
    /// XPS Builder Documentation</seealso>
    public class MHSConfig
    {
        /////////////////////////
        //  Class Data Members //
        ///////////////////////// 
        /// <summary>
        /// string Data member Holds XML file Path including file name
        /// </summary>
        private string _xmlFilePath = "";
        /// <summary>
        /// set get methods to update _xmlFilePath value
        /// </summary>
        public string xmlFilePath
        {
            set
            {
                _xmlFilePath = value;
            }
            get
            {
                return _xmlFilePath;
            }
        }
        /// <summary>
        /// string data member holds MHS file Path including file name.
        /// </summary>
        private string _MHSFilePath = "";
        /// <summary>
        /// set get methods to update _MHSFilePath value
        /// </summary>
        public string MHSFilePath
        {
            get
            {
                return _MHSFilePath;
            }
            set
            {
                _MHSFilePath = value;
            }
        }
        
        private XpsBuilder _Builder;

        /// <summary>
        /// The XpsBuilder object associated with this MHS Configurator
        /// </summary>
        public XpsBuilder Builder
        {
            get
            {
                return _Builder;
            }
            set
            {
                _Builder = value;
            }
        }

        //////////////////////
        //   Class Methods  //
        ////////////////////// 

        /// <summary>
        /// UpdateMHSFile method reads a cutom_mpd file and append its configurations to 
        /// the system MHS (Microprocessor Hardware Specifications) file. any configurations found in the Custom_MPD xml file
        /// wll overwrite MHS file configurations.
        /// </summary>
        /// <returns>returns true if the update process ended successfully,else returns false</returns>
        public bool UpdateMHSFile()
        {
            // reads xml file
            //XmlTextReader xmlReader = new XmlTextReader(_xmlFilePath);
            if (_MHSFilePath == "")
            {
                _Builder.RaiseMessageEvent("MHS File Name and Path is not set to a value");
                return false;
            }
            if (!System.IO.File.Exists(_MHSFilePath))
            {
                _Builder.RaiseMessageEvent("MHS file does not exist in the specified Directory");
                return false;
            }
            if (_xmlFilePath == "")
            {
                _Builder.RaiseMessageEvent("XML file Name and Path are not set to a value");
                return false;
            }

            XmlDocument xConfig = new XmlDocument();
            xConfig.Load(_xmlFilePath);

            XmlNode xMPD = null;

            foreach(XmlNode xRoot in xConfig.ChildNodes)
            {
                if (xRoot.Name.ToLower() == "coreconfig")
                {
                    foreach (XmlNode xNode in xRoot.ChildNodes)
                    {
                        if (xNode.Name.ToLower() == "mpd")
                        {
                            xMPD = xNode;
                            break;
                        }
                    }
                }
            }
            if (xMPD == null)
            {
                _Builder.RaiseMessageEvent("WARNING: Custom config file {0} contained no parameters", _xmlFilePath);
                return true;
            }

            string CoreName = xMPD.Attributes.GetNamedItem("Core").Value;
            string CoreInst = xMPD.Attributes.GetNamedItem("Instance").Value;

            // read Ipcore name and search for it in MHS file
            StreamReader sr = new StreamReader(_MHSFilePath);
            string TempMHSFile = "";
            int indexer = 0;
            while(!sr.EndOfStream)
            {
                TempMHSFile += sr.ReadLine().Replace("\n","").Replace("\r","") + "\n";
                indexer++;
            }
            string[] MHS_file_lines = TempMHSFile.Split('\n');
            sr.Close();
            //MHS_file.Replace("\r","");
           
            string strCoreLConfigLines = "";

            int index = 0;
            bool bDone = false;
            for ( ; index < MHS_file_lines.Length; index++ )
            {
                if (bDone)
                    break;
                if (MHS_file_lines[index].Contains(" " + CoreName) && MHS_file_lines[index].Contains("BEGIN"))
                {
                    int block_start_index = index;
                    string block_instance = string.Empty;
                    // extract config block from MHS file
                    while (!MHS_file_lines[index].Contains("END"))
                    {
                        if (MHS_file_lines[index].Contains("PARAMETER INSTANCE"))
                        {
                            block_instance = MHS_file_lines[index].Substring(("PARAMETER INSTANCE = ").Length + 1).Trim();
                            if (block_instance == CoreInst)
                            {
                                // This is the core instance we're looking for.  Purge it from the MHS
                                index = block_start_index;
                                while (!MHS_file_lines[index].Contains("END"))
                                {
                                    if (!MHS_file_lines[index].Contains("BUS_INTERFACE BUS = "))
                                    {
                                        strCoreLConfigLines += MHS_file_lines[index] + "\n";
                                    }
                                    MHS_file_lines[index] = "";
                                    index++;
                                }
                                strCoreLConfigLines += MHS_file_lines[index] + "\n";
                                MHS_file_lines[index] = "";
                                bDone = true;
                                break;
                            }
                            else
                            {
                                // This is the right type, but the wrong instance.  Leave it be.
                                break;
                            }
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
            }


            

            //_Builder.RaiseMessageEvent(strCoreLConfigLines);

            // convert Config Block to array of string
            string[] arrCoreConfigLines = strCoreLConfigLines.Split('\n');
            string strAddUpConfigs = "";
            foreach(XmlNode xParam in xMPD.ChildNodes)
            {
                if (xParam.Name.ToUpper() == "PARAMETER" || xParam.Name.ToUpper() == "PORT" || xParam.Name.ToUpper() == "BUS_INTERFACE")
                {
                    string XParamType = xParam.Name.ToUpper();
                    string XParamName = xParam.Attributes.GetNamedItem("Name").Value;
                    string XParamValue = xParam.Attributes.GetNamedItem("Value").Value;
                    bool boolFoundFlag = false;
                    for (int i = 0; i < arrCoreConfigLines.Length; i++ )
                    {
                        if (arrCoreConfigLines[i].Contains(String.Format("{0} {1}", XParamType, XParamName)))
                        {
                            arrCoreConfigLines[i] = String.Format("  {0} {1} = {2}\n", XParamType, XParamName, XParamValue);
                            boolFoundFlag = true;
                            break;
                        }
                    }
                    if (!boolFoundFlag)
                    {
                        strAddUpConfigs += String.Format("  {0} {1} = {2}\n", XParamType, XParamName, XParamValue);
                    }
                }
            }

            string FinalConfigs = ""; // to be passed as a parameter
            for (int y = 0; y < arrCoreConfigLines.Length; y++)
            {
                arrCoreConfigLines[y] = arrCoreConfigLines[y].Trim() + "\n";
                if (!((arrCoreConfigLines[y].StartsWith("BEGIN")) || (arrCoreConfigLines[y].StartsWith("END"))))
                {
                    if (arrCoreConfigLines[y].Contains("BUS_INTERFACE BUS = "))
                        arrCoreConfigLines[y] = string.Empty;   // Remove the invalid BUS type
                    else 
                        arrCoreConfigLines[y] = "  " + arrCoreConfigLines[y];
                }
                else
                {
                    arrCoreConfigLines[y] = string.Empty;   // Remove the BEGIN/END Pair
                }
                FinalConfigs += arrCoreConfigLines[y];
            }
            FinalConfigs = String.Format("{0}\n{1}", FinalConfigs, strAddUpConfigs);
            FinalConfigs = String.Format("BEGIN {0}\n{1}\nEND\n\n", CoreName, FinalConfigs);
            // write the configuration back to MHS file
            StreamWriter sw = new StreamWriter(_MHSFilePath,false);
            for (int i = 0; i < MHS_file_lines.Length; i++)
            {
                if (MHS_file_lines[i].Trim().Length > 0)
                    sw.Write(MHS_file_lines[i] + "\n");
            }

            while (FinalConfigs.Contains(" \n"))
                FinalConfigs = FinalConfigs.Replace(" \n", "\n");           
            while (FinalConfigs.Contains("\n\n"))
                FinalConfigs = FinalConfigs.Replace("\n\n", "\n");
            sw.Write(FinalConfigs);
            sw.Close();

            return true;
        }
    }
}
