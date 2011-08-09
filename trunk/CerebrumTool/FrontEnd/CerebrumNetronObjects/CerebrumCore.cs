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
 * CerebrumCore.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object representing a Cerebrum Core (package).
 * History: 
 * >> (20 Jun 2011) Matthew Cotter: Added CoreLibraryCategory property to the CerebrumCore object.
 * >> ( 9 May 2011) Matthew Cotter: Added support for Vortex Types Edge and Bridge to the Vortex "interfaces" supported by the Cerebrum Core.
 * >> ( 7 Apr 2011) Matthew Cotter: Converted Resources property to a Dictionary rather than a generic List.
 * >> (18 Feb 2011) Matthew Cotter: Added properties and methods that will be used by the Platform Synthesis tool.
 * >> (16 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Corrected issues in saving and loading connected clock signals from property system.
 * >> (15 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Created/exposed several methods and properties to be utilized by the mapping algorithm and XPS builder libraries.
 * >> (14 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Integrated improved property management system into component/core structure.
 * >> (28 Jan 2011) Matthew Cotter: Added capability for some platform components to be restricted from visibility within the design GUI.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components to be visible within the design GUI.
 * >> (22 Dec 2010) Matthew Cotter: Added additional support for customizable clock management.
 * >> (21 Dec 2010) Matthew Cotter: Added Input/Output Clock list properties to component design object.
 * >> (16 Dec 2010) Matthew Cotter: Modified GetPortByName and AddPort to reference port names attached to internal cores.
 *                                  Added support for GUI rotation of core, and corresponding visual relocation of attached ports.
 * >> (24 Oct 2010) Matthew Cotter: Added support for translating and evaluating conditions and properties within the context of previously set properties.
 * >> (23 Oct 2010) Matthew Cotter: Added support for differentiated port location (Input and Output) located differently based on corresponding type.
 *                                  Corrected minor bug in constructor that improperly initialized Owner property.
 * >> (22 Oct 2010) Matthew Cotter: Corrected crash bug that results from incorrect location of core port connectors
 *                                  Added support for sub-component property load/save within core packages.
 * >> (11 Oct 2010) Matthew Cotter: Moved core package integration into CoreLibrary where package definition is loaded.
 * >> (10 Oct 2010) Matthew Cotter: Initial support for core packages (multiple pcores per CCore) and integration with ClockGenerator library.
 *                                  Corrected core port location and association in concert with Vortex integration for GUI support
 * >> (24 Sep 2010) Matthew Cotter: Continued work on right-click support for properties.
 * >> (23 Sep 2010) Matthew Cotter: Added support for loading, saving, and accessing properties associated with a CCore.
 *                                  Started event support for click events (left and right)
 * >> (21 Sep 2010) Matthew Cotter: Added support for accessing custom properties associated with a CCore.
 * >> (17 Sep 2010) Matthew Cotter: Added ImagePath property and initialization in constructor(s).
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inspired SquareShape class, along with Cerebrum/Core specific properties.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using CerebrumSharedClasses;
using System.Xml;
using FalconClockManager;
using VortexInterfaces;
using VortexInterfaces.VortexCommon;
using VortexObjects;

namespace CerebrumNetronObjects
{

    /// <summary>
    /// Delegate method used to pass an error message to an higher level object
    /// </summary>
    /// <param name="Core">The CerebrumCore object that generated the error message</param>
    /// <param name="Message">The message generated</param>
    public delegate void CoreErrorMessage(CerebrumCore Core, string Message);
   
    /// <summary>
    /// Netron-inspired object defining properties of a Cerebrum core package and inherits from a displayable design object in the netron library.
    /// </summary>
    public class CerebrumCore : SquareShape
    {
        #region Private Members
        private Dictionary<string, long> _Resources;
        private List<string> _Architectures;
        private List<string> _Keywords;

        #endregion

        #region Initialization and Constructors
        /// <summary>
        /// Default constructor.  Creates a blank/empty core object.
        /// </summary>
        public CerebrumCore() : base()
        {
            this.Initialize();
        }
        /// <summary>
        /// Copy constructor.  Creates a new CerebrumCore instance whose properties and values are direct copies of existing CerebrumCore object.
        /// </summary>
        /// <param name="NewInstance">The instance value of the new CerebrumCore</param>
        /// <param name="CloneSource">The CerebrumCore whose values and properties are to be copied.</param>
        public CerebrumCore(string NewInstance, CerebrumCore CloneSource) : base()
        {
            this.Initialize();

            #region Copy Object properties
            this.CoreInstance = NewInstance;
            this.CoreName = CloneSource.CoreName;
            this.CoreType = CloneSource.CoreType;
            this.CoreVersion = CloneSource.CoreVersion;
            this.CoreOwner = CloneSource.CoreOwner;
            this.CoreServer = CloneSource.CoreServer;
            this.CoreBitmap = CloneSource.CoreBitmap;
            this.CoreDescription = CloneSource.CoreDescription;
            this.CoreInstancePrefix = CloneSource.CoreInstancePrefix;
            this.CoreLocation = CloneSource.CoreLocation;
            this.CoreImagePath = CloneSource.CoreImagePath;
            this.RotationAngle = CloneSource.RotationAngle;
            this.VisibleInLibrary = CloneSource.VisibleInLibrary;
            this.VisibleInDesign = CloneSource.VisibleInDesign;
            #endregion

            #region Copy subcomponent cores & hardware properties/parameters

            this.MaxSize = CloneSource.MaxSize;
            this.MinSize = CloneSource.MinSize;
            this.Width = CloneSource.Width;
            this.Height = CloneSource.Height;

            this.SupportedArchitectures.Clear();
            foreach (string arch in CloneSource.SupportedArchitectures)
                this.SupportedArchitectures.Add(arch);

            this.CoreKeywords.Clear();
            foreach (string keyword in CloneSource.CoreKeywords)
                this.CoreKeywords.Add(keyword);

            this.Resources.Clear();
            foreach (KeyValuePair<string, long> Pair in CloneSource.Resources)
                this.Resources.Add(Pair.Key.ToLower(), Pair.Value);

            this.MHSImports.Clear();
            foreach (string MHSImport in CloneSource.MHSImports)
                this.MHSImports.Add(MHSImport);

            _Properties = new CerebrumPropertyCollection(NewInstance, NewInstance, CloneSource.CoreType);
            _Properties.CloneFrom(CloneSource.Properties);

            foreach (ComponentCore CompCore in CloneSource.ComponentCores.Values)
            {
                this.ComponentCores.Add(CompCore.NativeInstance, new ComponentCore(this.CoreInstance, CompCore));
            }
            #endregion

            #region Copy Ports
            this.Connectors.Clear();
            foreach (CoreConnector cConn in CloneSource.Connectors)
            {
                this.AddPort(cConn.PortName, cConn.PortType, cConn.PortInterface, cConn.CoreInstance, cConn.XOffset, cConn.YOffset, cConn.ScaleFactor);
            }
            #endregion

            #region Copy Vortex Devices
            foreach (IVortexAttachment IVA in CloneSource.VortexDevices)
            {
                VortexAttachmentType vType = VortexAttachmentType.SAP;
                string InterfaceInstance = IVA.Instance;
                if (IVA is IVortexSAP)
                {
                    vType = VortexAttachmentType.SAP;
                }
                else if (IVA is IVortexSOP)
                {
                    vType = VortexAttachmentType.SOP;
                }
                AddVortexAttachmentInstance(IVA.CoreInstance, vType);
            }
            #endregion

            #region Copy clock signals
            this._InputClocks.Clear();
            foreach (ClockSignal CS in CloneSource.InputClocks)
            {
                ClockSignal newCS = new ClockSignal(CS);
                newCS.ComponentInstance = this.CoreInstance;
                newCS.CoreInstance = CS.CoreInstance;
                newCS.SignalDirection = CS.SignalDirection;
                newCS.Name = CS.Name;
                newCS.Port = CS.Port;
                this.InputClocks.Add(newCS);
            }
            this._OutputClocks.Clear();
            foreach (ClockSignal CS in CloneSource.OutputClocks)
            {
                ClockSignal newCS = new ClockSignal(CS);
                newCS.ComponentInstance = this.CoreInstance;
                newCS.CoreInstance = CS.CoreInstance;
                newCS.SignalDirection = CS.SignalDirection;
                newCS.Name = CS.Name;
                newCS.CoreInstance = CS.CoreInstance;
                newCS.Port = CS.Port;
                this.OutputClocks.Add(newCS);
            }
            #endregion

            #region Copy reset signals
            //this._InputResets = new List<ResetSignal>();
            //foreach (ResetSignal CS in this.InputResets)
            //{
            //    ResetSignal newCS = new ResetSignal(CS);
            //    newCS.IsOutput = false;
            //    newCS.Description = string.Empty;
            //    newCS.SourceCore = string.Empty;
            //    this.InputResets.Add(newCS);
            //}
            //this._OutputResets = new List<ResetSignal>();
            //foreach (ResetSignal CS in this.OutputResets)
            //{
            //    ResetSignal newCS = new ResetSignal(CS);
            //    newCS.IsOutput = true;
            //    newCS.Description = CS.Description;
            //    newCS.SourceCore = CS.SourceCore;
            //    this.OutputResets.Add(newCS);
            //}
            #endregion


            Debug.WriteLine(String.Format("Cloned {0}/{1} to {2}/{3}", CloneSource.CoreInstance, CloneSource.CoreName, this.CoreInstance, this.CoreName));
        }

        /// <summary>
        /// Method used to initialize border, connectors, sizing, internal lists, and properties.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            mPenStyle = new LinePenStyle();
            mPenStyle.Color = Color.Black;
            mPenStyle.DashStyle = DashStyle.Solid;
            mPenStyle.Width = 5;

            mPaintStyle = new SolidPaintStyle(Color.White);

            Transform(this.Width, this.Height, 150, 150);
            mConnectors.Clear();

            this.CoreInstance = string.Empty;
            this.CoreName = string.Empty;
            this.CoreType = string.Empty;
            this.CoreVersion = string.Empty;
            this.CoreOwner = string.Empty;
            this.CoreServer = false;
            this.CoreBitmap = null;
            this.CoreDescription = string.Empty;
            this.CoreInstancePrefix = string.Empty;
            this.CoreLocation = string.Empty;
            this.CoreImagePath = string.Empty;
            this.RotationAngle = 0;
            this.VisibleInLibrary = true;
            this.VisibleInDesign = true;

            this._IsVirtual = false;
            this._VirtualParent = null;

            _Architectures = new List<string>();
            _Keywords = new List<string>();
            _Resources = new Dictionary<string, long>();
            _ComponentCores = new Dictionary<string, ComponentCore>();
            _Properties = new CerebrumPropertyCollection(this.CoreInstance, this.CoreInstance, this.CoreType);
            
            this._VirtualChildren = new List<CerebrumCore>();

            this.VortexDevices = new List<IVortexAttachment>();

            this.MappedFPGA = string.Empty;
            this._MHSImports = new List<string>();

            this._InputClocks = new List<ClockSignal>();
            this._OutputClocks = new List<ClockSignal>();
            //this._InputResets = new List<ResetSignal>();
            //this._OutputResets = new List<ResetSignal>();

            this.MinSize = new Size(100, 100);
            this.MaxSize = new Size(100, 100);
        }
        #endregion

        #region GUI Interface / Netron.ShapeBase Methods
        /// <summary>
        /// Scales bitmap image to the specified width and height scale
        /// </summary>
        /// <param name="b">The input bitmap image to scale</param>
        /// <param name="nWidth">The desired width</param>
        /// <param name="nHeight">The desired height</param>
        /// <returns>A Bitmap object as a copy of b, scaled to nWidth-by-nHeight dimensions</returns>
        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result)) g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }
        /// <summary>
        /// Get or set the maximum size of the display object
        /// </summary>
        public new Size MaxSize { get; set; }
        /// <summary>
        /// Get or set the minimum size of the display object
        /// </summary>
        public new Size MinSize { get; set; }
        /// <summary>
        /// Get the Bitmap object displayed on this object in the Netron design UI
        /// </summary>
        public virtual Bitmap DisplayBitmap
        {
            get { return this.CoreBitmap; }
        }

        private void PaintShadowBorder(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            Pen pen = mPenStyle.DrawingPen();
            
            // Draw the shadow first so it's in the background.
            //DrawShadow(g);
            GraphicsPath border = BorderPath;
            int alpha = (int)(255 * this.DisplayOpacity);
            if (alpha > 255)
                alpha = 255;
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(alpha, pen.Color));
            g.FillPath(backBrush, border);
        }
        private void PaintDefaultBackground(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            GraphicsPath path = Path;
            int alpha = (int)(255 * this.DisplayOpacity);
            if (alpha > 255)
                alpha = 255;
            Color gradStart = Color.FromArgb(alpha, Color.Blue);
            Color gradEnd = Color.FromArgb(alpha, Color.Black);

            Point TopLeft = new Point();
            Point TopRight = new Point();
            Point BottomLeft = new Point();
            Point BottomRight = new Point();
            float Width = this.Width;
            float Height = this.Height;
            if (this.RotationAngle == 0)
            {
                TopLeft = (TopLeftCorner);
                TopRight = (TopRightCorner);
                BottomLeft = (BottomLeftCorner);
                BottomRight = (BottomRightCorner);
            }
            else if (this.RotationAngle == 90)
            {
                TopLeft = (TopRightCorner);
                TopRight = (BottomRightCorner);
                BottomLeft = (TopLeftCorner);
                BottomRight = (BottomLeftCorner);
            }
            else if (this.RotationAngle == 180)
            {
                TopLeft = (BottomRightCorner);
                TopRight = (BottomLeftCorner);
                BottomLeft = (TopRightCorner);
                BottomRight = (TopLeftCorner);
            }
            else if (this.RotationAngle == 270)
            {
                TopLeft = (BottomLeftCorner);
                TopRight = (TopLeftCorner);
                BottomLeft = (BottomRightCorner);
                BottomRight = (TopRightCorner);
            }
            LinearGradientBrush bgBrush = new LinearGradientBrush(TopLeft, BottomRight, gradStart, gradEnd);
            g.FillPath(bgBrush, path);
        }
        private void PaintImageBackground(Graphics g)
        {
            Bitmap bmp = ResizeBitmap(this.DisplayBitmap, this.Width, this.Height);
            Point TopLeft = new Point();
            Point TopRight = new Point();
            Point BottomLeft = new Point();
            Point BottomRight = new Point();
            float Width = this.Width;
            float Height = this.Height;
            if (this.RotationAngle == 0)
            {
                TopLeft = (TopLeftCorner);
                TopRight = (TopRightCorner);
                BottomLeft = (BottomLeftCorner);
                BottomRight = (BottomRightCorner);
            }
            else if (this.RotationAngle == 90)
            {
                Width = this.Height;
                Height = this.Width;
                TopLeft = (TopRightCorner);
                TopRight = (BottomRightCorner);
                BottomLeft = (TopLeftCorner);
                BottomRight = (BottomLeftCorner);
            }
            else if (this.RotationAngle == 180)
            {
                TopLeft = (BottomRightCorner);
                TopRight = (BottomLeftCorner);
                BottomLeft = (TopRightCorner);
                BottomRight = (TopLeftCorner);
            }
            else if (this.RotationAngle == 270)
            {
                Width = this.Height;
                Height = this.Width;
                TopLeft = (BottomLeftCorner);
                TopRight = (TopLeftCorner);
                BottomLeft = (BottomRightCorner);
                BottomRight = (TopRightCorner);
            }
            GraphicsUnit units = GraphicsUnit.Pixel;
            ColorMatrix CM = new ColorMatrix();
            ImageAttributes IA = new ImageAttributes();
            Rectangle imRect = new Rectangle(0, 0, this.Width, this.Height);
            CM.Matrix33 = this.DisplayOpacity;
            IA.SetColorMatrix(CM, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //g.DrawImage(bmp, new Point[] { TopLeft, TopRight, BottomLeft } );
            g.DrawImage(bmp, new Point[] { TopLeft, TopRight, BottomLeft }, imRect, units, IA);
        }
        private void PaintConnectors(Graphics g)
        {
            foreach (IConnector con in mConnectors)
            {
                con.Paint(g);
            }
        }
        /// <summary>
        /// Method called when the object is display, used to draw the core's Bitmap and connectors using the supplied Graphics object.
        /// </summary>
        /// <param name="g">The graphics object to be used to render the core graphics</param>
        public override void Paint(Graphics g)
        {
            if (mVisible == false)
            {
                return;
            }
            PaintShadowBorder(g);
            if (this.DisplayBitmap == null)
            {
                PaintDefaultBackground(g);
            }
            else
            {
                PaintImageBackground(g);
            }
            PaintConnectors(g);
        }
        /// <summary>
        /// Get the Image object used as graphical representation of the core in the design UI
        /// </summary>
        public override Image LibraryImage
        {
            get
            {
                if (this.DisplayBitmap != null)
                {
                    Stream myStream = new MemoryStream();
                    Bitmap img = ResizeBitmap(new Bitmap(DisplayBitmap), 16, 16);
                    img.Save(myStream, ImageFormat.Bmp);
                    return Image.FromStream(myStream);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Method used to determine whether the specified point is a 'hit' on the object.
        /// </summary>
        /// <param name="p">The point to perform hit-testing on</param>
        /// <returns>True if the specified point falls on or in this object or any of its connectors; False otherwise.</returns>
        public override bool Hit(Point p)
        {
            bool bHitsCore = base.Hit(p);
            if (!bHitsCore)
            {
                foreach (CoreConnector cc in mConnectors)
                {
                    if (cc.Hit(p))
                        return true;
                }
            }
            return bHitsCore;
        }
        /// <summary>
        /// Override of AllowTextEditing to prevent users from accidentally opening a text-edit dialog on a design core.
        /// </summary>
        public override bool AllowTextEditing
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region CerebrumCore Properties
        /// <summary>
        /// Indicates whether the loaded Cerebrum Core is visible in the toolbox library at design time
        /// </summary>
        public bool VisibleInLibrary { get; set; }
        /// <summary>
        /// Indicates whether the loaded Cerebrum Core is visible in the design layout at design time
        /// </summary>
        public bool VisibleInDesign { get; set; }
        private string _CoreInstance;
        /// <summary>
        /// Get or set the instance name of the CerebrumCore
        /// </summary>
        public string CoreInstance
        {
            get
            {
                return _CoreInstance;
            }
            set
            {
                _CoreInstance = value;
                if (_ComponentCores != null)
                {
                    foreach (ComponentCore CompCore in _ComponentCores.Values)
                    {
                        CompCore.CoreInstance = String.Format("{0}_{1}", _CoreInstance, CompCore.NativeInstance);
                    }
                }
            }
        }
        /// <summary>
        /// Get or set the entity name of the CerebrumCore
        /// </summary>
        public string CoreName
        {
            get
            {
                return base.mEntityName;
            }
            set
            {
                base.mEntityName = value;
            }
        }
        /// <summary>
        /// Get or set the True Core Name of the CerebrumCore
        /// </summary>
        public string CoreTrueName { get; set; }
        /// <summary>
        /// Get or set the type name of the CerebrumCore
        /// </summary>
        public string CoreType { get; set; }
        /// <summary>
        /// Get or set the version of the CerebrumCore
        /// </summary>
        public string CoreVersion { get; set; }
        /// <summary>
        /// Get or set the owner of the CerebrumCore
        /// </summary>
        public string CoreOwner { get; set; }
        /// <summary>
        /// Get or set the flag indicating whether the CerebrumCore requires a compiled server application
        /// </summary>
        public bool CoreServer { get; set; }
        /// <summary>
        /// Get or set the description of the CerebrumCore
        /// </summary>
        public string CoreDescription { get; set; }
        /// <summary>
        /// Get or set the Location of the CerebrumCore
        /// </summary>
        public string CoreLocation { get; set; }
        /// <summary>
        /// Get or set the list of Keywords associated with the CerebrumCore
        /// </summary>
        public List<string> CoreKeywords
        {
            get
            {
                return _Keywords;
            }
            set
            {
                _Keywords = value;
            }
        }
        /// <summary>
        /// Get or set the instance prefix associated with instances of the CerebrumCore
        /// </summary>
        public string CoreInstancePrefix { get; set; }
        /// <summary>
        /// Get or set the Image Path, relative to the core definition file.
        /// </summary>
        public string RelativeImagePath { get; set; }
        /// <summary>
        /// Get or set the path to the image file displayed for the CerebrumCore
        /// </summary>
        public string CoreImagePath { get; set; }
        /// <summary>
        /// Get or set the bitmap used for display of the CerebrumCore
        /// </summary>
        public Bitmap CoreBitmap { get; set; }
        /// <summary>
        /// The Design UI Library Toolbox category under which this component can be found.
        /// </summary>
        public string CoreLibraryCategory { get; set; }
        /// <summary>
        /// The Angle of rotation to be used for displaying the core in the design environment.  Legal values of RotationAngle are: [0, 90, 180, 270] % 360.
        /// If the specified value is not one of these values, the nearest corresponding angle will be selected.  
        /// </summary>
        public int RotationAngle
        {
            get
            {
                return _rotationAngle;
            }
            set
            {
                value = value % 360;
                double nearest = (double)value / 360.0;
                nearest = nearest / 0.25F;
                nearest = Math.Round(nearest, 0);
                _rotationAngle = Convert.ToInt32(nearest * 90);
                if (_rotationAngle < 0)
                    _rotationAngle += 360;

                foreach (IConnector con in mConnectors)
                {
                    RotatePort((CoreConnector)con);
                }
            }
        }
        private int _rotationAngle;


        /// <summary>
        /// Gets a list of properties associated with the core, and all component subcores
        /// </summary>
        public List<CerebrumPropertyEntry> CoreProperties
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.CoreProperties;
                }
                List<CerebrumPropertyEntry> AllProperties = new List<CerebrumPropertyEntry>();
                foreach (ComponentCore CompCore in this.ComponentCores.Values)
                {
                    AllProperties.AddRange(CompCore.Properties.GetEntries(CerebrumPropertyTypes.CEREBRUMPROPERTY));
                    AllProperties.AddRange(CompCore.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER));
                }
                AllProperties.AddRange(this.Properties.GetEntries(CerebrumPropertyTypes.CEREBRUMPROPERTY));
                AllProperties.AddRange(this.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER));
                return AllProperties;
            }
        }
        /// <summary>
        /// Get a list of all FPGA architectures supported by this core
        /// </summary>
        public List<string> SupportedArchitectures
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.SupportedArchitectures;
                }
                return _Architectures;
            }
            set
            {
                if (IsVirtual)
                {
                    return;
                }
                _Architectures = value;
            }
        }
        /// <summary>
        /// Get a list of all resources required by this core
        /// </summary>
        public Dictionary<string, long> Resources 
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.Resources;
                }
                return _Resources;
            }
            set
            {
                if (IsVirtual)
                {
                    return;
                }
                _Resources = value;
            }
        }

        #endregion

        #region Component/SubComponent Configuration & Access to Properties Collection
        /// <summary>
        /// Translates parameters within the specified condition string, in the context of the core's parameters and its defaults
        /// </summary>
        /// <param name="Input">Input condition string</param>
        /// <returns>Translated string to be evaluated.</returns>
        public string TranslateString(string Input)
        {
            if (IsVirtual)
            {
                return this.VirtualParent.TranslateString(Input);
            }
            string Output = Input;
            if (Output == null)
                return String.Empty;
            foreach (CerebrumPropertyEntry CPE in this.Properties.GetEntries(CerebrumPropertyTypes.PARAMETER))
            {
                if (Output.Contains(CPE.PropertyName))
                    Output = Output.Replace(CPE.PropertyName, CPE.PropertyValue);
            }
            return Output;
        }

        /// <summary>
        /// Get a list of properties specific to this core, excluding properties of component subcores
        /// </summary>
        public CerebrumPropertyCollection Properties
        {
            get
            {
                if (IsVirtual)
                    return this.VirtualParent.Properties;
                return _Properties;
            }
        }
        private CerebrumPropertyCollection _Properties;

        /// <summary>
        /// Set a property value specific to this core.
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <param name="PropertyValue">The value to set</param>
        public void SetPropertyValue(string PropertyName, string PropertyValue)
        {
            if (IsVirtual)
            {
                this.VirtualParent.SetPropertyValue(PropertyName, PropertyValue);
                return;
            }
            _Properties.SetValue(CerebrumPropertyTypes.PARAMETER, PropertyName, PropertyValue, true);
        }
        /// <summary>
        /// Get a property value specific to this core.
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <returns>Returns the value of the property, if it exists; null otherwise.</returns>
        public string GetPropertyValue(string PropertyName)
        {
            if (IsVirtual)
                return this.VirtualParent.GetPropertyValue(PropertyName);
            return (string)_Properties.GetValue(CerebrumPropertyTypes.PARAMETER, PropertyName);
        }

        /// <summary>
        /// Load core configuration parameters and properties associated with this core.
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void LoadComponentConfig(string ProjectPath)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to LoadCoreConfig() on virtualized component\n");
                return;
            }
            _Properties.PropertyCollectionInstance = this.CoreInstance;
            _Properties.PropertyCollectionType = this.CoreType;
            _Properties.LoadPropertyCollection(ProjectPath);
        }
        /// <summary>
        /// Load configuration parameters and properties for all component subcores
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void LoadCoreConfigs(string ProjectPath)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to LoadCoreConfigs() on virtualized component\n");
                return;
            }

            #region Load Configurations
            this.LoadComponentConfig(ProjectPath);
            foreach (ComponentCore CompCore in _ComponentCores.Values)
            {
                CompCore.LoadCoreConfig(ProjectPath);
                if (CompCore.InterfaceType != VortexAttachmentType.SAP)
                {
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_CMD_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_DATA_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "SLAVE_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_clk");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_rst");
                }
                if (CompCore.InterfaceType != VortexAttachmentType.SOP)
                {
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "EGRESS");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "INGRESS");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIG");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_clk");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_rst");
                }
            }
            #endregion

            #region Update ClockSignals
            foreach (ComponentCore CompCore in this.ComponentCores.Values)
            {
                List<CerebrumPropertyEntry> InClocks = CompCore.Properties.GetEntries(CerebrumPropertyTypes.INPUT_CLOCK);
                foreach (CerebrumPropertyEntry CPE in InClocks)
                {
                    foreach (ClockSignal InCS in InputClocks)
                    {
                        if ((String.Compare(InCS.CoreInstance, CPE.AssociatedCore, true) == 0) &&
                            (String.Compare(InCS.Port, CPE.ClockPort, true) == 0))
                        {
                            if ((CPE.ClockInputComponent == string.Empty) || (CPE.ClockInputComponent == null))
                            {
                                InCS.DisconnectFromSource();
                                CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, InCS.Port);
                            }
                            else
                            {
                                if (String.Compare(CPE.ClockInputComponent, "clock_generator") != 0)
                                {
                                    InCS.SourceComponentInstance = CPE.ClockInputComponent;
                                    InCS.SourceCoreInstance = CPE.ClockInputCore;
                                    InCS.SourcePort = CPE.ClockInputCorePort;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Save core configuration parameters and properties associated with this core.
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void SaveComponentConfig(string ProjectPath)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to SaveCoreConfig() on virtualized component\n");
                return;
            }
            _Properties.PropertyCollectionInstance = this.CoreInstance;
            _Properties.PropertyCollectionType = this.CoreType;
            _Properties.SavePropertyCollection(ProjectPath);
        }        
        /// <summary>
        /// Save configuration parameters and properties for all component subcores
        /// </summary>
        /// <param name="ProjectPath">The path to the project, where the core_config folder is located.</param>
        public void SaveCoreConfigs(string ProjectPath)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to SaveCoreConfigs() on virtualized component\n");
                return;
            }

            //#region Update Clock Signal Properties
            //foreach (ClockSignal InCS in InputClocks)
            //{
            //    ComponentCore CompCore = _ComponentCores[InCS.CoreInstance];
            //    ClockInfoStruct CIS = null;
            //    if ((InCS.SourceComponentInstance == string.Empty) ||
            //        (InCS.SourceCoreInstance == string.Empty) ||
            //        (InCS.SourcePort == string.Empty))
            //    {
            //        InCS.DisconnectFromSource();
            //        CompCore.Properties.DeleteValue(CerebrumPropertyTypes.INPUT_CLOCK, InCS.Port);
            //    }
            //    else
            //    {
            //        CIS = new ClockInfoStruct(InCS.Port, InCS.SourceComponentInstance, InCS.SourceCoreInstance, InCS.SourcePort);
            //        CompCore.Properties.SetValue(CerebrumPropertyTypes.INPUT_CLOCK, InCS.Port, CIS, true);
            //    }
            //}
            //#endregion

            #region Save Configurations
            this.SaveComponentConfig(ProjectPath);
            foreach (ComponentCore CompCore in _ComponentCores.Values)
            {
                CompCore.SaveCoreConfig(ProjectPath);
                if (CompCore.InterfaceType != VortexAttachmentType.SAP)
                {
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_CMD_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_DATA_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "SLAVE_IF");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_clk");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_rst");
                }
                if (CompCore.InterfaceType != VortexAttachmentType.SOP)
                {
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "EGRESS");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "INGRESS");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIG");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_clk");
                    CompCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_rst");
                }
            }
            #endregion
        }
        
        /// <summary>
        /// Gets a ClockSignal with the specified name, that corresponds to the direction specified
        /// </summary>
        /// <param name="Name">The name of the clock signal to locate</param>
        /// <param name="OutputClock">Indicates whether output (true) or input clocks (false) should be searched</param>
        /// <returns>A ClockSignal object representing the desired clock, if found.  Null otherwise.</returns>
        public ClockSignal GetClockByName(string Name, bool OutputClock)
        {
            if (IsVirtual)
            {
                return this.VirtualParent.GetClockByName(Name, OutputClock);
            }
            List<ClockSignal> L = null;

            if (OutputClock)
            {
                L = this.OutputClocks;
            }
            else
            {
                L = this.InputClocks;
            }
            foreach (ClockSignal CS in L)
            {
                if (String.Compare(Name, CS.Name, true) == 0)
                {
                    return CS;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the list of defined ComponentCores within this component that expose a Vortex Interface
        /// </summary>
        public List<ComponentCore> InterfaceCores
        {
            get
            {
                List<ComponentCore> IFCores = new List<ComponentCore>();
                foreach (IVortexAttachment IVA in this.VortexDevices)
                {
                    if (this.ComponentCores.ContainsKey(IVA.CoreInstance))
                        IFCores.Add(this.ComponentCores[IVA.CoreInstance]);
                }
                return IFCores;
            }
        }
        /// <summary>
        /// Gets the list of defined ComponentCores within this component that are flagged as Processing Elements
        /// </summary>
        public List<ComponentCore> ProcessingElementCores
        {
            get
            {
                List<ComponentCore> PEs = new List<ComponentCore>();
                foreach (ComponentCore CompCore in this.ComponentCores.Values)
                {
                    if (CompCore.IsPE)
                        PEs.Add(CompCore);
                }
                return PEs;
            }
        }

        /// <summary>
        /// Loads required component properties and parameters from child nodes beneath the required component node.
        /// </summary>
        /// <param name="FilePath">The path to the file containing the XML to be parsed.</param>
        /// <param name="RequiredComponentNode">The XML node under which the property and parameters nodes can be found.</param>
        public void LoadComponentPropertiesFromPlatformNode(string FilePath, XmlNode RequiredComponentNode)
        {
            foreach (XmlNode xNode in RequiredComponentNode.ChildNodes)
            {
                if (xNode.NodeType != XmlNodeType.Element)
                    continue;
                if (String.Compare(xNode.Name, "Parameter", true) == 0)
                {
                    if (((XmlElement)xNode).HasAttribute("Instance"))
                    {
                        continue;
                    }
                }
                CerebrumPropertyEntry CPE = new CerebrumPropertyEntry();
                CPE.ParseRequiredComponentProperty(FilePath, xNode);
                if (ComponentCores.ContainsKey(CPE.AssociatedCore))
                {
                    ComponentCores[CPE.AssociatedCore].Properties.SetValue(CPE, true);
                }
            }
        }

        /// <summary>
        /// Delegate used to pass a core object whose properties have been requested.
        /// </summary>
        /// <param name="core">The core whose properties have been requested</param>
        public delegate void CorePropertiesRequestedHandler(CerebrumCore core);
        /// <summary>
        /// Event fired when this core's properties have been requested.
        /// </summary>
        public event CorePropertiesRequestedHandler CorePropertiesRequested;
        /// <summary>
        /// Method used to invoke and fire the CorePropertiesRequested event, if its defined
        /// </summary>
        public void RaisePropertiesRequested()
        {
            if (IsVirtual)
            {
                this.VirtualParent.RaisePropertiesRequested();
            }
            else
            {
                if (CorePropertiesRequested != null)
                    CorePropertiesRequested(this);
            }
        }
        /// <summary>
        /// Override of menu property to prevent Netron UI from automatically displaying an irrelevant popup menu on a right click
        /// </summary>
        /// <returns></returns>
        public override System.Windows.Forms.ToolStripItem[] Menu()
        {
            return null;

            //ToolStripMenuItem item = new ToolStripMenuItem(
            //    "Properties...");
            //item.Click += new EventHandler(Properties_Click);
            //ToolStripItem[] items = new ToolStripItem[] { item };
            //return items;
        }

        Dictionary<string, ComponentCore> _ComponentCores;

        /// <summary>
        /// Gets the specified ComponentCore object if the specified instance exists.  Returns null otherwise.
        /// </summary>
        /// <param name="Instance">The instance name of the ComponentCore object to retrieve.</param>
        /// <returns>Returns the specified ComponentCore object if the specified instance exists.  Returns null otherwise.</returns>
        public ComponentCore GetComponentCore(string Instance)
        {
            if (_ComponentCores.ContainsKey(Instance))
                return _ComponentCores[Instance];
            else
                return null;
        }
        /// <summary>
        /// Gets the specified ComponentCore object if the specified instance exists.  Returns null otherwise.
        /// </summary>
        /// <param name="NativeInstance">The Native instance name of the ComponentCore object to retrieve.</param>
        /// <returns>Returns the specified ComponentCore object if the specified instance exists.  Returns null otherwise.</returns>
        public ComponentCore GetNativeComponentCore(string NativeInstance)
        {
            foreach (ComponentCore CompCore in this.ComponentCores.Values)
            {
                if (String.Compare(CompCore.NativeInstance, NativeInstance) == 0)
                    return CompCore;
            }
            return null;
        }

        /// <summary>
        /// Get a named list of component subcores associated with this CerebrumCore
        /// </summary>
        public Dictionary<string, ComponentCore> ComponentCores
        {
            get
            {
                if (IsVirtual)
                    return this.VirtualParent.ComponentCores;
                return _ComponentCores;
            }
        }
        /// <summary>
        /// Adds a property entry to the specified component subcore
        /// </summary>
        /// <param name="CoreInst">The instance name of the component subcore with which to associate the new property entry.</param>
        /// <param name="CPE">The new CorePropertyEntry</param>
        public void AddProperty(string CoreInst, CerebrumPropertyEntry CPE)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to AddProperty() on virtualized component\n");
                return;
            }
            if ((CoreInst == this.CoreInstance) || (CoreInst == string.Empty))
            {
                this.Properties.SetValue(CPE, true);
            }
            else
            {
                _ComponentCores[CoreInst].Properties.SetValue(CPE, true);
            }
        }
        /// <summary>
        /// Adds a new component subcore to this CerebrumCore
        /// </summary>
        /// <param name="SubInstance">The instance name of the subcore</param>
        /// <param name="SubType">The type name of the subcore</param>
        /// <param name="ValidCond">A property condition indicating whether this subcore is valid and should be included.</param>
        public void AddSubComponent(string SubInstance, string SubType, string ValidCond)
        {
            if (IsVirtual)
            {
                Debug.WriteLine("Attempt to AddSubComponent() on virtualized component\n");
                return;
            }
            ComponentCore CompCore = new ComponentCore(SubInstance, SubType);
            CompCore.CoreInstance = String.Format("{0}_{1}", this.CoreInstance, CompCore.NativeInstance);
            CompCore.CoreType = SubType;
            CompCore.ValidCondition = ValidCond;
            _ComponentCores.Add(CompCore.NativeInstance, CompCore);
        }

        /// <summary>
        /// Gets a list of subcores that match a specific type
        /// </summary>
        /// <param name="CoreType">The type of subcores to return</param>
        /// <returns>A List of subcores that match the specified type, if any exist</returns>
        public List<string> GetComponentCoresOfType(string CoreType)
        {
            if (IsVirtual)
            {
                return this.VirtualParent.GetComponentCoresOfType(CoreType);
            }
            List<string> CompCores = new List<string>();
            foreach (ComponentCore CompCore in _ComponentCores.Values)
            {
                if (String.Compare(CompCore.CoreType, CoreType, true) == 0)
                {
                    CompCores.Add(CompCore.CoreInstance);
                }
            }
            return CompCores;
        }

        private List<ClockSignal> _InputClocks;
        /// <summary>
        /// Get a list of clocks signals that feed into this component
        /// </summary>
        public List<ClockSignal> InputClocks
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.InputClocks;
                }
                if (_InputClocks == null)
                    _InputClocks = new List<ClockSignal>();
                return _InputClocks;
            }
            set
            {
                if (IsVirtual)
                {
                    Debug.WriteLine("Attempt to set InputClocks on virtualized component\n");
                    return;
                }
                _InputClocks = value;
            }
        }
        private List<ClockSignal> _OutputClocks;
        /// <summary>
        /// Get a list of clocks signals that are generated (output) by this component
        /// </summary>
        public List<ClockSignal> OutputClocks
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.OutputClocks;
                }
                if (_OutputClocks == null)
                    _OutputClocks = new List<ClockSignal>();
                return _OutputClocks;
            }
            set
            {
                if (IsVirtual)
                {
                    Debug.WriteLine("Attempt to set OutputClocks on virtualized component\n");
                    return;
                }
                _OutputClocks = value;
            }
        }
        #endregion

        #region Core Virtualization
        private const float VIRTUAL_OPACITY = 0.5F;
        private const float REAL_OPACITY = 1.0F;

        /// <summary>
        /// Get the opacity with which the component is drawn/painted on the design UI
        /// </summary>
        public float DisplayOpacity
        {
            get
            {
                if (IsVirtual)
                    return VIRTUAL_OPACITY;
                else
                    return REAL_OPACITY;
            }
        }

        private bool _IsVirtual;
        /// <summary>
        /// Get (Internal-only: or Set) flag indicating whether this core is virtual representation of another core.
        /// </summary>
        public bool IsVirtual
        {
            get
            {
                return _IsVirtual;
            }
            internal set
            {
                _IsVirtual = value;
            }
        }
        private CerebrumCore _VirtualParent;
        /// <summary>
        /// If IsVirtual is True, Gets (Internal-only: or Sets) the reference to the core of which this core is a virtualization;  otherwise returns null.
        /// </summary>
        public CerebrumCore VirtualParent
        {
            get
            {
                if (IsVirtual)
                    return _VirtualParent;
                else
                    return null;
            }
            internal set
            {
                if (IsVirtual)
                {
                    _VirtualParent = value;
                }
            }
        }
        /// <summary>
        /// Creates a copy of this core as a virtualized core.  A Virtualized core acts and is configured EXACTLY as its Virtual parent.
        /// </summary>
        /// <returns></returns>
        public CerebrumCore Virtualize()
        {
            CerebrumCore VChild = new CerebrumCore(this.CoreInstance, this);
            VChild.IsVirtual = true;
            VChild.VirtualParent = this;
            this.AddVirtualizedChild(VChild);
            return VChild;
        }
        private List<CerebrumCore> _VirtualChildren;
        /// <summary>
        /// Gets a list of CerebrumCores that are virtualized children of this core.
        /// </summary>
        public List<CerebrumCore> VirtualChildren
        {
            get
            {
                if (!this.IsVirtual)
                    return new List<CerebrumCore>();
                if (_VirtualChildren == null)
                    _VirtualChildren = new List<CerebrumCore>();                
                return _VirtualChildren;
            }
        }
        /// <summary>
        /// Adds the specified core as a virtual child to this core.  If this core is virtual, the child is added to the parent core.  If the specified core is not virtual, 
        /// and is not already a copy of parent core, it is not added.
        /// </summary>
        /// <param name="VChild">The Virtual core to be added.</param>
        internal void AddVirtualizedChild(CerebrumCore VChild)
        {
            if (!VChild.IsVirtual)
                return;
            if (this.IsVirtual)
            {
                this.VirtualParent.AddVirtualizedChild(VChild);
            }
            else
            {
                if (String.Compare(this.CoreInstance, VChild.CoreInstance) != 0)
                    return;
                if (String.Compare(this.CoreVersion, VChild.CoreVersion) != 0)
                    return;
                if (String.Compare(this.CoreType, VChild.CoreType) != 0)
                    return;
                if (_VirtualChildren == null)
                    _VirtualChildren = new List<CerebrumCore>();
                if (!_VirtualChildren.Contains(VChild))
                    _VirtualChildren.Add(VChild);
            }
        }
        /// <summary>
        /// Removes the specified core as a virtual child from this core.  If this core is virtual, the child is removed instead from to the parent core.  If the specified core is not virtual, 
        /// and is not already a copy of the parent core, it is not removed.
        /// </summary>
        /// <param name="VChild">The Virtual core to be added.</param>
        internal void RemoveVirtualizedChild(CerebrumCore VChild)
        {
            if (!VChild.IsVirtual)
                return;
            if (this.IsVirtual)
            {
                this.VirtualParent.RemoveVirtualizedChild(VChild);
            }
            else
            {
                if (String.Compare(this.CoreInstance, VChild.CoreInstance) != 0)
                    return;
                if (String.Compare(this.CoreVersion, VChild.CoreVersion) != 0)
                    return;
                if (String.Compare(this.CoreType, VChild.CoreType) != 0)
                    return;
                if (_VirtualChildren == null)
                    _VirtualChildren = new List<CerebrumCore>();
                if (_VirtualChildren.Contains(VChild))
                    _VirtualChildren.Remove(VChild);
            }
        }
        /// <summary>
        /// Determines whether or not this core has any virtual children.   A virtual core itself, cannot have any children.
        /// </summary>
        /// <returns>True if this core is not virtual, and has virtual children.  False otherwise.</returns>
        public bool HasVirtualChildren()
        {
            if (this.IsVirtual)
                return false;
            if (_VirtualChildren == null)
                _VirtualChildren = new List<CerebrumCore>();
            return _VirtualChildren.Count > 0;
        }

        /// <summary>
        /// If this core is virtual, removes it from the parent's list of children.  If this core is the parent, this removes all virtual children.
        /// </summary>
        public void DeVirtualize()
        {
            if (this.IsVirtual)
            {
                this.VirtualParent.RemoveVirtualizedChild(this);
            }
            else
            {
                while (this.VirtualChildren.Count > 0)
                {
                    CerebrumCore VChild = this.VirtualChildren[0];
                    VChild.DeVirtualize();
                }
            }
        }
        #endregion

        #region I/O Ports
        /// <summary>
        /// Gets the list of IO Ports exposed by the core.
        /// </summary>
        public List<CoreConnector> CorePorts 
        { 
            get 
            {
                List<CoreConnector> _Ports = new List<CoreConnector>();
                foreach (Connector c in mConnectors)
                {
                    if (c is CoreConnector)
                    {
                        _Ports.Add((CoreConnector)c);
                    }
                }
                return _Ports;
            } 
        }

        private void RotatePort(CoreConnector port)
        {
            float szX = (this.TopRightCorner.X - this.TopLeftCorner.X);
            float szY = (this.BottomRightCorner.Y - this.TopLeftCorner.Y);

            int portCenterX = 0;
            int portCenterY = 0;
            if (this.RotationAngle == 0)
            {
                portCenterX = this.TopLeftCorner.X + Convert.ToInt32(port.XOffset * szX);
                portCenterY = this.TopLeftCorner.Y + Convert.ToInt32(port.YOffset * szY);
            }
            else if (this.RotationAngle == 90)
            {
                portCenterX = this.TopLeftCorner.X + Convert.ToInt32(port.YOffset * szX);
                portCenterY = this.TopLeftCorner.Y + Convert.ToInt32(port.XOffset * szY);
            }
            else if (this.RotationAngle == 180)
            {
                portCenterX = this.BottomRightCorner.X - Convert.ToInt32(port.XOffset * szX);
                portCenterY = this.BottomRightCorner.Y - Convert.ToInt32(port.YOffset * szY);
            }
            else if (this.RotationAngle == 270)
            {
                portCenterX = this.BottomRightCorner.X - Convert.ToInt32(port.YOffset * szX);
                portCenterY = this.BottomRightCorner.Y - Convert.ToInt32(port.XOffset * szY);
            }
            Point previousPt = new Point(port.Point.X, port.Point.Y);
            Point delta = new Point((portCenterX - previousPt.X), (portCenterY - previousPt.Y));
            port.MoveBy(delta);
        }
        /// <summary>
        /// Adds a new port to this core with the specified parameters at the specified location and size scale
        /// </summary>
        /// <param name="PortName">The name of the port</param>
        /// <param name="Type">The type of the port</param>
        /// <param name="Interface">The interface associated with the port</param>
        /// <param name="Instance">The internal core instance associated with the port</param>
        /// <param name="X">The x-location (0 (leftmost) to 100 (rightmost) ) of the port</param>
        /// <param name="Y">The x-location (0 (topmost) to 100 (bottommost) ) of the port</param>
        /// <param name="Scale">The relative sizing scale of the port connector</param>
        public void AddPort(string PortName, string Type, string Interface, string Instance, float X, float Y, float Scale)
        {
            try
            {
                PortType newType = PortType.Invalid;
                if ((String.Compare(Type, "IOINTERFACE", true) == 0) || (String.Compare(Type, "IOIF", true) == 0))
                {
                    newType = PortType.IOInterface;
                }
                else if ((String.Compare(Type, "INPUT", true) == 0) || (String.Compare(Type, "IN", true) == 0))
                {
                    newType = PortType.SOPInput;
                }
                else if ((String.Compare(Type, "OUTPUT", true) == 0) || (String.Compare(Type, "OUT", true) == 0))
                {
                    newType = PortType.SOPOutput;
                }
                else if ((String.Compare(Type, "INITIATOR", true) == 0) || (String.Compare(Type, "INIT", true) == 0))
                {
                    newType = PortType.SAPInitiator;
                }
                else if ((String.Compare(Type, "TARGET", true) == 0) || (String.Compare(Type, "TGT", true) == 0))
                {
                    newType = PortType.SAPTarget;
                }
                if (newType != PortType.Invalid)
                {
                    AddPort(PortName, newType, Interface, Instance, X, Y, Scale);
                }
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
        }
        
        /// <summary>
        /// Adds a new port to this core with the specified parameters at the specified location and size scale
        /// </summary>
        /// <param name="PortName">The name of the port</param>
        /// <param name="Type">The type of the port</param>
        /// <param name="Interface">The interface associated with the port</param>
        /// <param name="Instance">The internal core instance associated with the port</param>
        /// <param name="X">The x-location (0 (leftmost) to 100 (rightmost) ) of the port</param>
        /// <param name="Y">The x-location (0 (topmost) to 100 (bottommost) ) of the port</param>
        /// <param name="Scale">The relative sizing scale of the port connector</param>
        public void AddPort(string PortName, PortType Type, string Interface, string Instance, float X, float Y, float Scale)
        {
            try
            {
                CoreConnector port = new CoreConnector(Model);
                port.Parent = this;
                port.Core = this;
                port.ConnectorSize = Convert.ToInt32(20 * Scale);
                port.XOffset = X;
                port.YOffset = Y;
                float szX = (this.TopRightCorner.X - this.TopLeftCorner.X);
                float szY = (this.BottomLeftCorner.Y - this.TopLeftCorner.Y);

                int portCenterX = this.TopLeftCorner.X + Convert.ToInt32(port.XOffset * szX);
                int portCenterY = this.TopLeftCorner.Y + Convert.ToInt32(port.YOffset * szY);

                port.Point = new Point(portCenterX, portCenterY);

                port.PortName = PortName;
                port.PortType = Type;
                port.PortInterface = Interface;
                port.CoreInstance = Instance;
                port.ScaleFactor = Scale;
                switch (port.PortType)
                {
                    case PortType.IOInterface:
                        port.ConnectorStyle = Ports.Styles.IOInterface;
                        port.ConnectorColor = Ports.Colors.IOInterface;
                        break;

                    case PortType.SAPInitiator:
                        port.ConnectorStyle = Ports.Styles.SAPInitiator;
                        port.ConnectorColor = Ports.Colors.SAPInitiator;
                        break;
                    case PortType.SAPTarget:
                        port.ConnectorStyle = Ports.Styles.SAPTarget;
                        port.ConnectorColor = Ports.Colors.SAPTarget;
                        break;

                    case PortType.SOPInput:
                        port.ConnectorStyle = Ports.Styles.SOPInput;
                        port.ConnectorColor = Ports.Colors.SOPInput;
                        break;
                    case PortType.SOPOutput:
                        port.ConnectorStyle = Ports.Styles.SOPOutput;
                        port.ConnectorColor = Ports.Colors.SOPOutput;
                        break;
                }
                mConnectors.Add(port);
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Locates and returns the output connector attached to this core that is both closest to the specified (X,Y) coordinates and compatibile with the specified port
        /// </summary>
        /// <param name="X">The x-coordinate</param>
        /// <param name="Y">The y-coordinate</param>
        /// <param name="SourcePort">The source port with which the located port must be compatible</param>
        /// <returns>A CoreConnector object if a suitable connector is found.</returns>
        public CoreConnector GetCompatiblePortClosestToXY(float X, float Y, CoreConnector SourcePort)
        {
            try
            {
                if ((X < -30) || (Y < -30)) // Some flexibility in edging
                    return null;
                if ((X > (this.X + this.Width + 30)) || (Y > (this.Y + this.Height + 30))) // Some flexibility in edging
                    return null;
                float minDist = float.MaxValue;
                CoreConnector nearestConn = null;
                List<PortType> CompatibleTargets = Ports.GetCompatibleTargetTypes(SourcePort.PortType);
                foreach (CoreConnector c in mConnectors)
                {
                    if (!CompatibleTargets.Contains(c.PortType))
                    {
                        continue;
                    }
                    if (String.Compare(c.PortInterface, SourcePort.PortInterface, true) != 0)
                    {
                        continue;
                    }
                    float deltaX = (X - c.Center.X);
                    float deltaY = (Y - c.Center.Y);
                    float dist = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestConn = c;
                    }
                }
                return nearestConn;
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
            return null;
        }

        /// <summary>
        /// Locates and returns the input connector attached to this core that is closest to the specified (X,Y) coordinates
        /// </summary>
        /// <param name="X">The x-coordinate</param>
        /// <param name="Y">The y-coordinate</param>
        /// <returns>A CoreConnector object if a suitable connector is found.</returns>
        public CoreConnector GetInPortClosestToXY(float X, float Y)
        {
            try
            {
                if ((X < this.X - 30) || (Y < this.Y - 30)) // Some flexibility in edging
                    return null;
                if ((X > (this.X + this.Width + 30)) || (Y > (this.Y + this.Height + 30))) // Some flexibility in edging
                    return null;
                float minDist = float.MaxValue;
                CoreConnector nearestConn = null;
                foreach (CoreConnector c in mConnectors)
                {
                    if ((c.PortType == PortType.SOPInput) || (c.PortType == PortType.SAPTarget))
                    {
                        float deltaX = (X - c.Center.X);
                        float deltaY = (Y - c.Center.Y);
                        float dist = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestConn = c;
                        }
                    }
                }
                return nearestConn;
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
            return null;
        }
        
        /// <summary>
        /// Locates and returns the output connector attached to this core that is closest to the specified (X,Y) coordinates
        /// </summary>
        /// <param name="X">The x-coordinate</param>
        /// <param name="Y">The y-coordinate</param>
        /// <returns>A CoreConnector object if a suitable connector is found.</returns>
        public CoreConnector GetOutPortClosestToXY(float X, float Y)
        {
            try
            {
                if ((X < this.X - 30) || (Y < this.Y - 30)) // Some flexibility in edging
                    return null;
                if ((X > (this.X + this.Width + 30)) || (Y > (this.Y + this.Height + 30))) // Some flexibility in edging
                    return null;
                float minDist = float.MaxValue;
                CoreConnector nearestConn = null;
                foreach (CoreConnector c in mConnectors)
                {
                    if ((c.PortType == PortType.SOPOutput) || (c.PortType == PortType.SAPInitiator))
                    {
                        float deltaX = (X - c.Center.X);
                        float deltaY = (Y - c.Center.Y);
                        float dist = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestConn = c;
                        }
                    }
                }
                return nearestConn;
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
            return null;
        }
                
        /// <summary>
        /// Locates and returns the connector attached to this core that is closest to the specified (X,Y) coordinates
        /// </summary>
        /// <param name="X">The x-coordinate</param>
        /// <param name="Y">The y-coordinate</param>
        /// <returns>A CoreConnector object if a suitable connector is found.</returns>
        public CoreConnector GetPortClosestToXY(float X, float Y)
        {
            try
            {
                if ((X < this.X - 30) || (Y < this.Y - 30)) // Some flexibility in edging
                    return null;
                if ((X > (this.X + this.Width + 30)) || (Y > (this.Y + this.Height + 30))) // Some flexibility in edging
                    return null;
                float minDist = float.MaxValue;
                CoreConnector nearestConn = null;
                foreach (CoreConnector c in mConnectors)
                {
                    float deltaX = (X - c.Center.X);
                    float deltaY = (Y - c.Center.Y);
                    float dist = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestConn = c;
                    }
                }
                return nearestConn;
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
            return null;
        }

        /// <summary>
        /// Returns the connector attached to this core with the specified name, if it exists.
        /// </summary>
        /// <param name="PortName">The name of the port to locate</param>
        /// <param name="CoreInstance">The name of the instance core instance associated with the desired port name.  
        /// If this parameter is empty or null, the first port matching PortName is returned.</param>
        /// <returns>A CoreConnector object if a connector with the specified name and associated core is found.</returns>
        public CoreConnector GetPortByName(string PortName, string CoreInstance)
        {
            try
            {
                foreach (CoreConnector cc in mConnectors)
                {
                    if ((String.Compare(cc.PortName, PortName, true) == 0) && 
                        ((String.Compare(cc.CoreInstance, CoreInstance, true) == 0) || 
                         (CoreInstance == string.Empty) || 
                         (CoreInstance == null)
                        )
                       )
                        return cc;
                }
            }
            catch (Exception ex)
            {
                RaiseCoreError(ErrorReporting.ExceptionDetails(ex));
                ErrorReporting.DebugException(ex);
            }
            return null;
        }
        #endregion

        #region Vortex Attachments

        /// <summary>
        /// Get the list of Vortex-attached devices contained within this component.
        /// </summary>
        public List<IVortexAttachment> VortexDevices { get; set; }

        internal void AddVortexAttachmentInstance(string InstanceName, VortexInterfaces.VortexCommon.VortexAttachmentType VortexType)
        {
            IVortexAttachment newAttachment = null;
            if (VortexType == VortexInterfaces.VortexCommon.VortexAttachmentType.SAP)
                newAttachment = new VortexSAP();
            else if (VortexType == VortexInterfaces.VortexCommon.VortexAttachmentType.SOP)
                newAttachment = new VortexSOP();
            else if (VortexType == VortexInterfaces.VortexCommon.VortexAttachmentType.VortexBridge)
                newAttachment = new VortexBridgeAttachment();
            else if (VortexType == VortexInterfaces.VortexCommon.VortexAttachmentType.VortexEdge)
            {
                newAttachment = new VortexEdgeAttachment();
                VortexEdgeAttachment VEA = newAttachment as VortexEdgeAttachment;
                VEA.EdgeComponent = this.ComponentCores[InstanceName];
            }
            newAttachment.CoreInstance = InstanceName;
            newAttachment.Instance = String.Format("{0}_{1}", this.CoreInstance, newAttachment.CoreInstance);
            VortexDevices.Add(newAttachment);
        }
        internal IVortexAttachment GetAttachmentInstance(string InstanceName)
        {
            for (int i = 0; i < VortexDevices.Count; i++)
            {
                if (string.Compare(VortexDevices[i].CoreInstance, InstanceName, true) == 0)
                {
                    return VortexDevices[i];
                }
            }
            return null;
        }
        #endregion

        #region Events

        /// <summary>
        /// Event fired when this core generates an error message
        /// </summary>
        public event CoreErrorMessage CoreError;
        /// <summary>
        /// Method used to invoke and fire the CoreError event, if its defined.
        /// </summary>
        /// <param name="Message">The message of the error to be raised</param>
        private void RaiseCoreError(string Message)
        {
            if (CoreError != null)
                CoreError(this, Message);
        }
        /// <summary>
        /// Method used to invoke and fire the MouseUp method (and eventually MouseUp event)
        /// </summary>
        /// <param name="e">Mouse event arguments generated by the Netron design UI.</param>
        public override void RaiseOnMouseUp(EntityMouseEventArgs e)
        {
            base.RaiseOnMouseUp(e);
        }
        /// <summary>
        /// Override and implementation of MouseUp method used to (eventually) fire the MouseUp event
        /// </summary>
        /// <param name="e">Mouse event arguments generated by the Netron design UI.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            base.MouseUp(e);
        }

        #endregion

        #region Methods and Properties Used by XPS Builder

        /// <summary>
        /// Get a list of MHS Imports required by this core
        /// </summary>
        public List<string> MHSImports
        {
            get
            {
                if (IsVirtual)
                {
                    return this.VirtualParent.MHSImports;
                }
                return _MHSImports;
            }
            set
            {
                if (IsVirtual)
                {
                    return;
                }
                _MHSImports = value;
            }
        }
        private List<string> _MHSImports;

        /// <summary>
        /// Get or set the FPGA to which this CerebrumCore is mapped.
        /// </summary>
        public string MappedFPGA
        {
            get
            {
                return _MappedFPGA;
            }
            set
            {
                _MappedFPGA = value;
                foreach (ComponentCore CompCore in ComponentCores.Values)
                {
                    CompCore.MappedFPGA = _MappedFPGA;
                }
            }
        }
        private string _MappedFPGA;
        /// <summary>
        /// Get or set the architecture family of the FPGA to which this CerebrumCore is mapped.
        /// </summary>
        public string TargetFamily { get; set; }

        /// <summary>
        /// Reads all MHS Imports files, importing the parameters, bus interfaces and ports specified therein to the corresponding component core property collections.
        /// </summary>
        /// <param name="ExternalPorts">(ref) A list of external ports defined by the MHS files.  This list is not cleared prior to parsing.</param>
        /// <returns>True if the import was successful; False otherwise.</returns>
        public bool ImportXMLAsMHS(ref List<string> ExternalPorts)
        {
            try
            {
                string ComponentID = this.CoreInstance;

                foreach (string XMLPath in this.MHSImports)
                {
                    #region Read Import File
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(XMLPath);

                    XmlNode xPorts = CerebrumSharedClasses.CerebrumXmlInterface.GetXmlNode(xDoc, "MHSImport.Ports");
                    XmlNode xCores = CerebrumSharedClasses.CerebrumXmlInterface.GetXmlNode(xDoc, "MHSImport.Cores");

                    if (xPorts != null)
                    {
                        foreach (XmlNode xPort in xPorts.ChildNodes)
                        {
                            if (xPort.NodeType == XmlNodeType.Comment)
                                continue;
                            if (String.Compare(xPort.Name, "PORT_PIN", true) == 0)
                            {
                                bool bValid = true;
                                string sValue = string.Empty;
                                foreach (XmlAttribute xPortAttr in xPort.Attributes)
                                {
                                    if (String.Compare(xPortAttr.Name, "Value", true) == 0)
                                    {
                                        sValue = String.Format(" PORT {0}\r\n", xPortAttr.Value).Trim();
                                    }
                                    else if (String.Compare(xPortAttr.Name, "Valid", true) == 0)
                                    {
                                        bValid = Conditions.EvaluateAsBoolean(this.TranslateString(xPortAttr.Value));
                                        if (!bValid)
                                            break;
                                    }
                                }
                                if (bValid)
                                {
                                    sValue = ReplaceNativeInstances(sValue);
                                    ExternalPorts.Add(sValue);
                                }
                            }
                        }
                    }

                    if (xCores != null)
                    {
                        foreach (XmlNode xCore in xCores.ChildNodes)
                        {
                            if (xCore.NodeType == XmlNodeType.Comment)
                                continue;
                            if (String.Compare(xCore.Name, "CORE", true) == 0)
                            {
                                string type = string.Empty;
                                string nativeInst = string.Empty;
                                string inst = string.Empty;
                                string ver = string.Empty;
                                bool bValid = true;
                                foreach (XmlAttribute xCoreAttr in xCore.Attributes)
                                {
                                    if (String.Compare(xCoreAttr.Name, "Name", true) == 0)
                                    {
                                        type = xCoreAttr.Value;
                                    }
                                    else if (String.Compare(xCoreAttr.Name, "Instance", true) == 0)
                                    {
                                        nativeInst = xCoreAttr.Value;
                                        inst = String.Format("{0}_{1}", ComponentID, xCoreAttr.Value);
                                    }
                                    else if (String.Compare(xCoreAttr.Name, "HW_VER", true) == 0)
                                    {
                                        ver = xCoreAttr.Value;
                                    }
                                    else if (String.Compare(xCoreAttr.Name, "Valid", true) == 0)
                                    {
                                        bValid = Conditions.EvaluateAsBoolean(this.TranslateString(xCoreAttr.Value));
                                        if (!bValid)
                                            break;
                                    }
                                }
                                if (bValid)
                                {
                                    foreach (XmlNode xCoreProp in xCore.ChildNodes)
                                    {
                                        ComponentCore CompCore = this.ComponentCores[nativeInst];
                                        string propType = string.Empty;
                                        string propName = string.Empty;
                                        string propValue = string.Empty;

                                        propType = xCoreProp.Name.ToUpper();
                                        bool bPropValid = true;
                                        foreach (XmlAttribute xCorePropAttr in xCoreProp.Attributes)
                                        {
                                            if (String.Compare(xCorePropAttr.Name, "Valid", true) == 0)
                                            {
                                                bPropValid = Conditions.EvaluateAsBoolean(CompCore.TranslateString(xCorePropAttr.Value));
                                                if (!bPropValid)
                                                    break;
                                            }
                                            else
                                            {
                                                propName = xCorePropAttr.Name;
                                                propValue = xCorePropAttr.Value;
                                                propValue = ReplaceNativeInstances(propValue);
                                            }
                                        }
                                        if (bPropValid)
                                        {
                                            try
                                            {
                                                CerebrumPropertyTypes PropType = (CerebrumPropertyTypes)Enum.Parse(typeof(CerebrumPropertyTypes), propType);
                                                CompCore.Properties.SetValue(PropType, propName, propValue, true);
                                            }
                                            finally
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                return false;
            }
            return true;
        }
                
        /// <summary>
        /// Translates a native instance into a hardware instance as part of the given component definition.
        /// </summary>
        /// <param name="CC">The CerebrumCore object on which to translate.</param>
        /// <param name="Value">The value string to translate.</param>
        /// <returns>The translated instance ID, if the Value is a native instance of the component; the original value, if not.</returns>
        public string TranslateAsNativeInstance(CerebrumCore CC, string Value)
        {
            if (IsNativeInstance(this, Value))
                return String.Format("{0}_{1}", this.CoreInstance, Value);
            else
                return Value;
        }

        private string ReplaceNativeInstances(string Value)
        {
            foreach (ComponentCore CompCore in this.ComponentCores.Values)
            {
                if (Value.Contains(CompCore.NativeInstance))
                {
                    Value = Value.Replace(CompCore.NativeInstance, String.Format("{0}_{1}", this.CoreInstance, CompCore.NativeInstance));
                }
            }
            return Value;
        }
        /// <summary>
        /// Determines whether a string value matches a native instance within a given component description.
        /// </summary>
        /// <param name="CC">The CerebrumCore object to test.</param>
        /// <param name="Value">The instance name to test for existence as a native component within the CerebrumCore.</param>
        /// <returns>True if the Value matches a native component instance; False if not.</returns>
        public bool IsNativeInstance(CerebrumCore CC, string Value)
        {
            foreach (ComponentCore CompCore in this.ComponentCores.Values)
            {
                if (String.Compare(CompCore.NativeInstance, Value) == 0)
                    return true;
            }
            return false;
        }
        
        #endregion

        #region Properties & Methods Used by Platform Synthesizer

        /// <summary>
        /// Get or set a flag indicating whether pre-synthesized information about cores within this component should be purged prior to starting synthesis.
        /// </summary>
        /// <remarks> When this value is set to CheckState.Checked, ALL cores within this component will be purged prior to synthesis.  When this is set to CheckState.Unchecked, 
        /// NO cores within this component will be purged prior to synthesis.   When this value is set to CheckState.Indeterminate, each core within this component will be purged
        /// on a case-by-case basis, based on the value of the core's PurgeBeforeSynthesis property.</remarks>
        public CheckState PurgeBeforeSynthesis
        { 
            get
            {
                return _PurgeBeforeSynthesis;
            }
            set
            {
                _PurgeBeforeSynthesis = value;
                if (_PurgeBeforeSynthesis != CheckState.Indeterminate)
                {
                    foreach (ComponentCore CompCore in this.ComponentCores.Values)
                    {
                        CompCore.PurgeBeforeSynthesis = (_PurgeBeforeSynthesis == CheckState.Checked);
                    }
                }
            }
        }
        private CheckState _PurgeBeforeSynthesis;

        #endregion
    }
}
