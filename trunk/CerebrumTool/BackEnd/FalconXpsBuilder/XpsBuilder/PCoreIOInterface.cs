using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Represents a simple IO definition as defined in the PCore's MPD IO_INTERFACE line
    /// </summary>
    public class PCoreIOInterface
    {
        /// <summary>
        /// Default constructor.  Initializes an empty PCoreIOInterface
        /// </summary>
        public PCoreIOInterface()
        {
            this.IO_IF = string.Empty;
            this.IO_TYPE = string.Empty;
            this.Ports = new List<PCorePort>();
            this.Parameters = new List<PCoreInterfaceParameter>();
        }

        /// <summary>
        /// The name of the IO interface
        /// </summary>
        public string IO_IF { get; set; }
        /// <summary>
        /// The type of the IO interface
        /// </summary>
        public string IO_TYPE { get; set; }

        /// <summary>
        /// The list of PCore ports associated with the IO interface
        /// </summary>
        public List<PCorePort> Ports { get; set; }
        /// <summary>
        /// The list of PCore parameters associated with the IO interface
        /// </summary>
        public List<PCoreInterfaceParameter> Parameters { get; set; }
    }
}
