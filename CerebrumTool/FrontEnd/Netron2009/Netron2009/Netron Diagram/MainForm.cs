using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetronProject;
using Netron.Diagramming.Win;
using Netron.DataVisualization;
using System.Drawing.Imaging;

namespace NetronDiagram
{
    public partial class MainForm : Form
    {
        ProjectPanel myProjectPanel;

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public MainForm()
        {
            InitializeComponent();

            myProjectPanel = new ProjectPanel();
            Controls.Add(myProjectPanel);
            myProjectPanel.Dock = DockStyle.Fill;
            myProjectPanel.ShowLibrary();
            //myProjectPanel.ShowExplorer();
            myProjectPanel.AddDiagram();

            // Load all libraries in the "Libraries" folder located in the
            // run directory.
            string librariesPath =
                Application.StartupPath + @"\..\..\..\..\library\";
            myProjectPanel.LoadAllLibraries(librariesPath);

            WindowState = FormWindowState.Maximized;

            myProjectPanel.ActiveDiagramTab.Diagram.Invalidate();
            //ChartSpaceShape excelChart = new ChartSpaceShape(
            //    myProjectPanel.ActiveDiagramTab.Diagram.Controller.Model);
            //myProjectPanel.ActiveDiagramTab.Diagram.AddShape(excelChart);
        }
    }
}