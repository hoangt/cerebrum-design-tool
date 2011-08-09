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
 * ServerManagerDialog.cs
 * Name: Matthew Cotter
 * Date: 10 Oct 2010 
 * Description: Dialog to display, manage and edit servers for synthesis, programming and compilation.
 * History: 
 * >> (23 Oct 2010) Matthew Cotter: Added support for adding new servers via this dialog.
 * >> (10 Oct 2010) Matthew Cotter: Drag-and-drop implementation of server management functions.
 * >> (10 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CerebrumSharedClasses;
using FalconGlobal;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of server lists
    /// </summary>
    public partial class ServerManagerDialog : Form
    {
        private class ServerDragDrop
        {
            public ServerDragDrop(FalconServer FS, ListViewItem LVI, ListView LV)
            {
                this.Server = FS;
                this.Item = LVI;
                this.List = LV;
            }
            public FalconServer Server { get; set; }
            public ListViewItem Item { get; set; }
            public ListView List { get; set; }
        }

        private ProjectManager ProjMan;

        Dictionary<string, FalconServer> GlobalServers;
        Dictionary<string, FalconServer> SynthesisServers;
        Dictionary<string, FalconServer> ProgrammingServers;
        Dictionary<string, FalconServer> CompilationServers;

        /// <summary>
        /// Default constructor.  Initializes listviews and events for server list management.
        /// </summary>
        /// <param name="ProjMan">The Project Manager associated with the loaded project</param>
        public ServerManagerDialog(ProjectManager ProjMan)
        {
            this.ProjMan = ProjMan;
            InitializeComponent();
            ConfigureListViews();

            txtAddress.GotFocus += new EventHandler(textBox_GotFocus);
            txtUserName.GotFocus += new EventHandler(textBox_GotFocus);
            txtPort.GotFocus += new EventHandler(textBox_GotFocus);
            txtPort.KeyPress += new KeyPressEventHandler(txtPort_KeyPress);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveServersFiles();
            this.Close();
        }

        #region ListView Management
        private const int COLUMN_ADDRESS = 0;
        private const int COLUMN_USER = 1;
        private const int COLUMN_PORT = 2;
        private const int COLUMN_ID = 3;

        private void ConfigureListViews()
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType() == typeof(ListView))
                {
                    ListView LV = (ListView)c;
                    LV.View = View.SmallIcon;
                    LV.LargeImageList = imgListServers;
                    LV.SmallImageList = imgListServers;
                    LV.Columns.Add("Address");
                    LV.Columns.Add("User");
                    LV.Columns.Add("Port");
                    LV.Columns.Add("ID");
                    
                    LV.AllowDrop = true;
                    LV.MultiSelect = false;
                    LV.ItemDrag += new ItemDragEventHandler(listView_ItemDrag);
                    LV.DragEnter += new DragEventHandler(listView_DragEnter);
                    LV.DragOver += new DragEventHandler(listView_DragOver);
                    LV.DragLeave += new EventHandler(listView_DragLeave);
                    LV.DragDrop += new DragEventHandler(listView_DragDrop);
                    LV.SelectedIndexChanged += new EventHandler(listView_SelectedIndexChanged);
                }
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            ListView LV = (ListView)sender;
            if (LV.SelectedItems.Count > 0)
            {
                ListViewItem LVI = (ListViewItem)LV.SelectedItems[0];
                PopulateInfoBox(LVI);
            }
            if ((lvAllServers.SelectedItems.Count == 0) &&
                (lvProgramServers.SelectedItems.Count == 0) &&
                (lvSynthServers.SelectedItems.Count == 0) &&
                (lvCompileServers.SelectedItems.Count == 0))
            {
                ClearInfoBox();
            }            
        }
        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem lvi = ((ListView)sender).FocusedItem;
            string SID = lvi.SubItems[COLUMN_ID].Text;
            FalconServer dragServer = null;
            if ((sender == lvAllServers) && (GlobalServers.ContainsKey(SID)))
            {
                dragServer = GlobalServers[SID];
            }
            else if ((sender == lvSynthServers) && (SynthesisServers.ContainsKey(SID)))
            {
                dragServer = SynthesisServers[SID];
            }
            else if ((sender == lvProgramServers) && (ProgrammingServers.ContainsKey(SID)))
            {
                dragServer = ProgrammingServers[SID];
            }
            else if ((sender == lvCompileServers) && (CompilationServers.ContainsKey(SID)))
            {
                dragServer = CompilationServers[SID];
            }
            if (dragServer != null)
            {
                ClearInfoBox();
                DoDragDrop(new ServerDragDrop(dragServer, lvi, (ListView)sender), DragDropEffects.Copy);
            }
        }
        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }
        private void listView_DragOver(object sender, DragEventArgs e)
        {
        }
        private void listView_DragLeave(object sender, EventArgs e)
        {
        }
        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            ClearInfoBox();
            if (e.Effect == DragDropEffects.Copy)
            {
                ListView targetLV = (ListView)sender;
                if (e.Data.GetDataPresent(typeof(ServerDragDrop)))
                {
                    ServerDragDrop serverDD = (ServerDragDrop)e.Data.GetData(typeof(ServerDragDrop));
                    if (serverDD.List == targetLV)
                        return;
                    string SID = serverDD.Server.ID;
                    if ((targetLV == lvAllServers))
                    {
                        if ((serverDD.List == lvSynthServers) && (SynthesisServers.ContainsKey(SID)))
                        {
                            SynthesisServers.Remove(SID);
                        }
                        else if ((serverDD.List == lvProgramServers) && (ProgrammingServers.ContainsKey(SID)))
                        {
                            ProgrammingServers.Remove(SID);
                        }
                        else if ((serverDD.List == lvCompileServers) && (CompilationServers.ContainsKey(SID)))
                        {
                            CompilationServers.Remove(SID);
                        }
                    }
                    else if ((targetLV == lvSynthServers) && (!SynthesisServers.ContainsKey(SID)))
                    {
                        SynthesisServers.Add(SID, serverDD.Server);
                    }
                    else if ((targetLV == lvProgramServers) && (!ProgrammingServers.ContainsKey(SID)))
                    {
                        ProgrammingServers.Add(SID, serverDD.Server);
                    }
                    else if ((targetLV == lvCompileServers) && (!CompilationServers.ContainsKey(SID)))
                    {
                        CompilationServers.Add(SID, serverDD.Server);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
            PopulateListViewsFromHashtables();
        }
        
        private void PopulateListViewsFromHashtables()
        {
            ClearInfoBox();
            lvAllServers.Items.Clear();
            lvSynthServers.Items.Clear();
            lvProgramServers.Items.Clear();
            lvCompileServers.Items.Clear();

            foreach (KeyValuePair<string, FalconServer> pair in GlobalServers)
            {
                string[] items = 
                    new string[] { 
                        pair.Value.Address, 
                        pair.Value.UserName, 
                        pair.Value.Port.ToString(), 
                        pair.Value.ID 
                    };

                lvAllServers.Items.Add(new ListViewItem(items, "server"));
                if (SynthesisServers.ContainsKey(pair.Key))
                {
                    lvSynthServers.Items.Add(new ListViewItem(items, "server"));
                }
                if (ProgrammingServers.ContainsKey(pair.Key))
                {
                    lvProgramServers.Items.Add(new ListViewItem(items, "server"));
                }
                if (CompilationServers.ContainsKey(pair.Key))
                {
                    lvCompileServers.Items.Add(new ListViewItem(items, "server"));
                }
            }
        }
        #endregion
                
        #region Manage Hashtable structures
        /// <summary>
        /// Loads servers from the available server list files, based on the Project Manager
        /// </summary>
        public void LoadServers()
        {
            string SynthServersFile = ProjMan.SynthesisServersFile.FullName;
            string ProgServersFile = ProjMan.ProgrammingServersFile.FullName;
            string CompServersFile = ProjMan.CompilationServersFile.FullName;
            
            ClearAll();
            LoadServersFile(SynthServersFile, ref SynthesisServers);
            LoadServersFile(ProgServersFile, ref ProgrammingServers);
            LoadServersFile(CompServersFile, ref CompilationServers);

            foreach (KeyValuePair<string, FalconServer> pair in SynthesisServers)
            {
                if (!GlobalServers.ContainsKey(pair.Key))
                    GlobalServers.Add(pair.Key, pair.Value);
            }
            foreach (KeyValuePair<string, FalconServer> pair in ProgrammingServers)
            {
                if (!GlobalServers.ContainsKey(pair.Key))
                    GlobalServers.Add(pair.Key, pair.Value);
            }
            foreach (KeyValuePair<string, FalconServer> pair in CompilationServers)
            {
                if (!GlobalServers.ContainsKey(pair.Key))
                    GlobalServers.Add(pair.Key, pair.Value);
            }

            PopulateListViewsFromHashtables();
        }
        private void ClearAll()
        {
            GlobalServers = new Dictionary<string, FalconServer>();
            SynthesisServers = new Dictionary<string, FalconServer>();
            ProgrammingServers = new Dictionary<string, FalconServer>();
            CompilationServers = new Dictionary<string, FalconServer>();
        }
        private bool LoadServersFile(string ServerFile, ref Dictionary<string, FalconServer> ServersList)
        {
            try
            {
                ServersList = new Dictionary<string, FalconServer>();
                if (!File.Exists(ServerFile))
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(ServerFile);

                foreach (XmlNode xServersNode in xDoc.ChildNodes)
                {
                    if (xServersNode.Name.ToLower() == "servers")
                    {
                        foreach (XmlNode xEServer in xServersNode.ChildNodes)
                        {
                            if (xEServer.Name.ToLower() == "server")
                            {
                                FalconServer fs = new FalconServer();
                                fs.ParseServerNode(xEServer);
                                ServersList.Add(fs.ID, fs);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        private void SaveServersFiles()
        {
            string SynthServersFile = ProjMan.SynthesisServersFile.FullName;
            string ProgServersFile = ProjMan.ProgrammingServersFile.FullName;
            string CompServersFile = ProjMan.CompilationServersFile.FullName;

            XmlDocument xSynthServerDoc = new XmlDocument();
            xSynthServerDoc.AppendChild(xSynthServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xSynthServerRoot = xSynthServerDoc.CreateElement("Servers");
            xSynthServerDoc.AppendChild(xSynthServerRoot);

            XmlDocument xProgServerDoc = new XmlDocument();
            xProgServerDoc.AppendChild(xProgServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xProgServerRoot = xProgServerDoc.CreateElement("Servers");
            xProgServerDoc.AppendChild(xProgServerRoot);

            XmlDocument xCompileServerDoc = new XmlDocument();
            xCompileServerDoc.AppendChild(xCompileServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xCompileServerRoot = xCompileServerDoc.CreateElement("Servers");
            xCompileServerDoc.AppendChild(xCompileServerRoot);

            foreach (KeyValuePair<string, FalconServer> pair in GlobalServers)
            {
                if (SynthesisServers.ContainsKey(pair.Key))
                {
                    pair.Value.WriteServerNode(xSynthServerRoot);
                }
                if (ProgrammingServers.ContainsKey(pair.Key))
                {
                    pair.Value.WriteServerNode(xProgServerRoot);
                }
                if (CompilationServers.ContainsKey(pair.Key))
                {
                    pair.Value.WriteServerNode(xCompileServerRoot);
                }
            }
            xSynthServerDoc.Save(SynthServersFile);
            xProgServerDoc.Save(ProgServersFile);
            xCompileServerDoc.Save(CompServersFile);
        }
        #endregion

        #region InfoBox Management
        private void textBox_GotFocus(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
            {
                e.Handled = true;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (grpSelected.Tag != null)
            {
                if (grpSelected.Tag.GetType() == typeof(FalconServer))
                {
                    SaveInfoBox((FalconServer)grpSelected.Tag);
                }
                PopulateListViewsFromHashtables();
            }
        }
        private void ClearInfoBox()
        {
            txtAddress.Clear();
            txtUserName.Clear();
            txtPort.Clear();
            grpSelected.Tag = null;
        }
        private void PopulateInfoBox(ListViewItem LVI)
        {
            foreach(FalconServer fServer in GlobalServers.Values)
            {
                if (LVI.SubItems[COLUMN_ID].Text == fServer.ID)
                {
                    txtAddress.Text = fServer.Address;
                    txtUserName.Text = fServer.UserName;
                    txtPort.Text = fServer.Port.ToString();
                    grpSelected.Tag = fServer;
                    break;
                }
            }
        }
        private void SaveInfoBox(FalconServer fs)
        {
            fs.Address = txtAddress.Text;
            fs.UserName = txtUserName.Text;
            fs.Port = int.Parse(txtPort.Text);
        }
        #endregion

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            int newID = 0;
            foreach (FalconServer FS in GlobalServers.Values)
            {
                int val = 0;
                if (int.TryParse(FS.ID, out val))
                {
                    if (val >= newID)
                        newID = val + 1;
                }
            }
            FalconServer FServer = new FalconServer();
            FServer.ID = newID.ToString();
            FServer.UserName = "new_user";
            GlobalServers.Add(FServer.ID, FServer);
            PopulateListViewsFromHashtables();
            ClearInfoBox();
            if (GlobalServers.ContainsKey(FServer.ID))
            {
                FalconServer fServer = (FalconServer)GlobalServers[FServer.ID];
                txtAddress.Text = fServer.Address;
                txtUserName.Text = fServer.UserName;
                txtPort.Text = fServer.Port.ToString();
                grpSelected.Tag = fServer;
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (grpSelected.Tag != null)
            {
                FalconServer FS = (FalconServer)grpSelected.Tag;
                if (GlobalServers.ContainsKey(FS.ID))
                {
                    GlobalServers.Remove(FS.ID);
                }
                PopulateListViewsFromHashtables();
                ClearInfoBox();
            }
        }
    }
}
