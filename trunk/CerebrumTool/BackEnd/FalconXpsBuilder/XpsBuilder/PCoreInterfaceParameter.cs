using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Represents a Parameter Definition within a PCore file
    /// </summary>
    public class PCoreInterfaceParameter
    {
        /// <summary>
        /// Default constructor.  Initializes and empty parameter specification.
        /// </summary>
        public PCoreInterfaceParameter()
        {
            this.ParameterName = string.Empty;
            this.IO_IF = string.Empty;
            this.IO_IS = string.Empty;
            this.ValidCond = string.Empty;
        }

        /// <summary>
        /// The Name of the parameter
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Indicates the IO interface of which the parameter is a part.
        /// </summary>
        public string IO_IF { get; set; }
        /// <summary>
        /// Indicates the interface port to which this parameter corresponds
        /// </summary>
        public string IO_IS { get; set; }
        /// <summary>
        /// Indicates conditions under which setting this parameter is valid
        /// </summary>
        public string ValidCond { get; set; }

        /// <summary>
        /// Parses a PARAMETER line from the MPD file
        /// </summary>
        /// <param name="ParameterLine">The line to parse</param>
        public void ParseMPDParameter(string ParameterLine)
        {
            string parameterPropertyValue = string.Empty;
            string parameterPropertyName = string.Empty;

            this.IO_IF = "NONE";
            while (ParameterLine.Length > 0)
            {
                int cIdx = ParameterLine.IndexOf(',');
                string partialLine = string.Empty;

                if (cIdx < 0)
                {
                    partialLine = ParameterLine.Trim();
                    ParameterLine = string.Empty;
                }
                else
                {
                    partialLine = ParameterLine.Substring(0, cIdx).Trim();
                    ParameterLine = ParameterLine.Substring(cIdx + 1).Trim();
                }
                int eqIdx = partialLine.IndexOf("=");
                if (eqIdx < 0)
                    return;
                parameterPropertyName = partialLine.Substring(0, eqIdx - 1).Trim();
                parameterPropertyValue = partialLine.Substring(eqIdx + 1).Trim();

                if (parameterPropertyName.StartsWith("PARAMETER"))
                {
                    int spIdx = parameterPropertyName.IndexOf(" ");
                    if (spIdx < 0)
                        return;
                    parameterPropertyValue = parameterPropertyName.Substring(spIdx + 1).Trim();
                    parameterPropertyName = parameterPropertyName.Substring(0, spIdx).Trim();

                    this.ParameterName = parameterPropertyValue;
                }
                else if (parameterPropertyName == "IO_IF")
                {
                    this.IO_IF = parameterPropertyValue;
                }
                else if (parameterPropertyName == "IO_IS")
                {
                    this.IO_IS = parameterPropertyValue;
                }
                else if (parameterPropertyName == "ISVALID")
                {
                    this.ValidCond = parameterPropertyValue;
                }
            }
        }
    }
}
