/******************************************************************** 
 * MHStoXML.cs
 * Name: Matthew Cotter
 * Date:  1 Jul 2010 
 * Description: This class allows a simple way of converting an MHS file into a "base_system" XML format..
 * History: 
 * >> (18 Jul 2010) Matthew Cotter: Comments added, IFalconLibrary interface implemented.
 * >> ( 1 Jul 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using FalconGlobal;
namespace FalconMHSToXML
{
    /// <summary>
    /// Simple class to quickly convert an MHS file into a comparable XML format.
    /// </summary>
    public class MHSToXML
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MHSToXML()
        {}

        /// <summary>
        /// Translates the specified MHS file into an XML file saved at the specified location.
        /// </summary>
        /// <param name="InputMHSFile">The MHS file to be converted.</param>
        /// <param name="OutputXMLFile">The corresponding XML file to be created.</param>
        /// <returns>True if conversion was successful, False otherwise.</returns>
        public bool Translate(string InputMHSFile, string OutputXMLFile)
        {
            bool bSuccess = true;
            StreamReader reader = null;
            XmlDocument xDoc = null;
            try
            {
                if (!File.Exists(InputMHSFile))
                    return false;
                reader = new StreamReader(InputMHSFile);
                xDoc = new XmlDocument();
                XmlNode xRoot = xDoc.CreateElement("BasePlatform");
                FalconFileRoutines.WriteCerebrumDisclaimerXml(xRoot);    // Added by Matthew Cotter 8/18/2010
                XmlNode xPorts = xDoc.CreateElement("Ports");
                XmlNode xCores = xDoc.CreateElement("Cores");
                while (!reader.EndOfStream)
                {
                    string inLine = reader.ReadLine();
                    inLine = inLine.Trim();
                    if (inLine.Length >= 0)
                    {
                        if (inLine.StartsWith("BEGIN"))
                        {
                            string CoreType = inLine.Substring(6);
                            XmlNode xCore = TranslateCore(CoreType, reader, xDoc);
                            if (xCore != null)
                            {
                                xCores.AppendChild(xCore);
                            }
                            else
                            {
                                bSuccess = false;
                                break;
                            }
                        }
                        else if (inLine.StartsWith("PORT"))
                        {
                            XmlNode xPort = xDoc.CreateElement("PORT_PIN");
                            XmlAttribute xPortText = xDoc.CreateAttribute("Value");
                            xPortText.Value = inLine.Substring(5).Trim();
                            xPort.Attributes.Append(xPortText);
                            xPorts.AppendChild(xPort);
                        }
                    }
                }
                xRoot.AppendChild(xPorts);
                xRoot.AppendChild(xCores);
                xDoc.AppendChild(xRoot);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (xDoc != null) xDoc.Save(OutputXMLFile);
            }
            return bSuccess;
        }

        private XmlNode TranslateCore(string CoreType, StreamReader reader, XmlDocument xDoc)
        {
            try
            {
                XmlNode xCoreNode = xDoc.CreateElement("CORE");
                XmlAttribute xACoreType = xDoc.CreateAttribute("Name");
                XmlAttribute xACoreInst = xDoc.CreateAttribute("Instance");
                XmlAttribute xACoreVer = xDoc.CreateAttribute("HW_VER");
                while (!reader.EndOfStream)
                {
                    string inLine = reader.ReadLine();
                    string NodeName = string.Empty;
                    string NodeAttrName = string.Empty;
                    string NodeAttrValue = string.Empty;

                    inLine = inLine.Trim();
                    if (inLine.StartsWith("END"))
                    {
                        break;
                    }
                    else
                    {
                        int spaceIdx = inLine.IndexOf(" ");
                        int eqIdx = inLine.IndexOf("=");
                        if ((eqIdx > spaceIdx) && (spaceIdx > 0))
                        {
                            NodeName = inLine.Substring(0, spaceIdx).Trim();
                            NodeAttrName = inLine.Substring(spaceIdx, eqIdx - spaceIdx).Trim();
                            NodeAttrValue = inLine.Substring(eqIdx + 1).Trim();
                            if (NodeAttrName == "INSTANCE")
                            {
                                xACoreInst.Value = NodeAttrValue;
                            }
                            else if (NodeAttrName == "HW_VER")
                            {
                                xACoreVer.Value = NodeAttrValue;
                            }
                            else
                            {
                                XmlNode xNode = xDoc.CreateElement(NodeName);
                                XmlAttribute xNodeAttr = xDoc.CreateAttribute(NodeAttrName);
                                xNodeAttr.Value = NodeAttrValue;
                                xNode.Attributes.Append(xNodeAttr);
                                xCoreNode.AppendChild(xNode);
                            }
                        }
                    }
                    xACoreType.Value = CoreType;
                    xCoreNode.Attributes.Append(xACoreType);
                    xCoreNode.Attributes.Append(xACoreInst);
                    xCoreNode.Attributes.Append(xACoreVer);
                }
                return xCoreNode;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon MHS to XML Conversion Tool";
            }
        }

        /// <summary>
        /// Returns the version of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentVersion.
        /// </summary>
        public string FalconComponentVersion
        {
            get
            {
                return "1.0.0";
            }
        }

        /// <summary>
        /// Returns the name/version/copyright information of this library component.  Implementation of 
        /// FalconGlobal.IFalconLibrary.GetFalconComponentVersion().
        /// </summary>
        public string GetFalconComponentVersion()
        {
            return String.Format("{0} {1} Copyright (c) 2010 PennState", FalconComponentName, FalconComponentVersion);
        }

        #endregion
    }
}
