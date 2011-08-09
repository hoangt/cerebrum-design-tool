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
 * PECodeEditorDialog.cs
 * Name: Matthew Cotter
 * Date: 24 Feb 2011 
 * Description: Dialog to display, manage and edit PE code sources.
 * History:
 * >> (26 Feb 2011) Matthew Cotter: Corrected bugs in loading, saving, and assigning code files.
 * >> (25 Feb 2011) Matthew Cotter: Additional work on interface and toolbars.
 * >> (24 Feb 2011) Matthew Cotter: Basic implementation of dialog to manage and edit PE code sources.
 * >> (24 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FalconPathManager;
using CerebrumNetronObjects;
using CerebrumSharedClasses;
using System.Diagnostics;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of PE code sources.
    /// </summary>
    public partial class PECodeEditorDialog : Form
    {
        private ProjectManager _ProjMan;
        private ComponentCore _CompCore;
        private FileInfo _SourceFile;
        private bool _SavedHere;
        private DateTime _LastSavedHere;
        private string _LocalCode;
        private bool _Checking = false;
        private bool _Assigning = false;

        /// <summary>
        /// Constructor.  Creates an empty code editor dialog form.
        /// </summary>
        /// <param name="ProjMan">The project manager used to locate project directories.</param>
        /// <param name="CompCore">The component core whose source is to be edited.</param>
        public PECodeEditorDialog(ProjectManager ProjMan, ComponentCore CompCore)
        {
            InitializeComponent();
            _ProjMan = ProjMan;
            _CompCore = CompCore;
            _SourceFile = null;
            _LastSavedHere = DateTime.MinValue;
            _SavedHere = true;
            this.Text = String.Format("PE Source Editor ({0})", _CompCore.CoreInstance);


            _LocalCode = String.Format("{0}\\{1}.c", _ProjMan.ProjectPESourceDirectory.FullName, _CompCore.CoreInstance);
            this.Activated += new EventHandler(PECodeEditorDialog_Activated);
            this.Closing += new CancelEventHandler(PECodeEditorDialog_Closing);
            txtSource.TextChanged += new EventHandler(txtSource_TextChanged);
        }

        void PECodeEditorDialog_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (!_SavedHere)
                {
                    string Message = String.Format("The loaded source file, {0}, has unsaved changes.  Save now?", _SourceFile.Name);
                    string Title = "Source Modified";
                    DialogResult res;
                    res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    this.Focus();
                    if (res == DialogResult.Yes)
                    {
                        SaveSourceFile();
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                e.Cancel = false;
            }
        }

        void PECodeEditorDialog_Activated(object sender, EventArgs e)
        {
            CheckForOutsideUpdate();
        }

        void txtSource_TextChanged(object sender, EventArgs e)
        {
            _SavedHere = false;
        }
        
        /// <summary>
        /// Checks to determine whether the loaded file was modified outside the Cerebrum environment, and requests that the user reload it if necessary.
        /// </summary>
        public void CheckForOutsideUpdate()
        {
            if (!_Checking)
            {
                _Checking = true;
                if (_SourceFile != null)
                {
                    if (!_SourceFile.Exists)
                    {
                        LoadSourceFile();
                    }
                    else
                    {
                        _SourceFile = new FileInfo(_SourceFile.FullName);
                        if (_SourceFile.LastWriteTime > _LastSavedHere)
                        {
                            string Message = String.Format("The loaded source file, {0}, was modified outside the editor.  Reload?", _SourceFile.Name);
                            string Title = "Source Modified";
                            DialogResult res;
                            res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            this.Focus();
                            if (res == DialogResult.Yes)
                            {
                                LoadSourceFile();
                            }
                            else
                            {
                                status.Text = "File modified outside editor! Currently displayed contents is not the most recent version!";
                            }
                        }
                    }
                }
                else
                {
                    if (_CompCore.IsAssignedCode)
                    {
                        if (File.Exists(_ProjMan.PathManager.DecodePath(_CompCore.LocalCodeSource)))
                        {
                            _SourceFile = new FileInfo(_ProjMan.PathManager.DecodePath(_CompCore.LocalCodeSource));
                        }
                    }
                    LoadSourceFile();
                }
                _Checking = false;
            }
        }

        /// <summary>
        /// Launches the default Windows editor associated with the source file type.
        /// </summary>
        /// <param name="SourcePath">The path to the source file.</param>
        public void LaunchExternalEditor(string SourcePath)
        {
            try
            {
                if (File.Exists(SourcePath))
                {
                    Process FileProcess = new Process();
                    FileProcess.StartInfo.FileName = SourcePath;
                    FileProcess.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("There was an error opening the file in the default Windows editor.\nYou may open the file yourself at {0}",
                    SourcePath), "Error Opening Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        /// <summary>
        /// Assigns a source file to the PE core.
        /// </summary>
        /// <param name="SourcePath">The path to the source file to be assigned.</param>
        public void AssignSourceFile(string SourcePath)
        {
            if (SourcePath.Contains(_ProjMan.PathManager["LocalProjectRoot"]))
            {
                string RelativeSource = SourcePath.Replace(_ProjMan.PathManager["LocalProjectRoot"], "${LocalProjectRoot}");
                _CompCore.LocalCodeSource = RelativeSource;
            }
            else
            {
                _CompCore.LocalCodeSource = SourcePath;
            }
            _SourceFile = new FileInfo(SourcePath);
        }
        /// <summary>
        /// Prompts the user to assign an existing code file to the PE Core.
        /// </summary>
        /// <param name="LocalCode">The path to where the tool would automatically create the source for this PE core.</param>
        /// <returns>True if the assignment was successful, false if not or an error occurs.</returns>
        private bool AssignExistingFile(string LocalCode)
        {
            try
            {
                _Assigning = true;
                DialogResult res;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.RestoreDirectory = true;
                ofd.InitialDirectory = _ProjMan.ProjectPESourceDirectory.FullName;
                ofd.Filter = "C Source Code Files (*.c)|*.c";
                ofd.FileName = String.Format("{0}.c", _CompCore.NativeInstance);
                ofd.Title = "Locate Existing Code for PE";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                res = ofd.ShowDialog();
                this.Focus();
                if (res == DialogResult.OK)
                {
                    if (ofd.FileName != LocalCode)
                    {
                        string Message = String.Format("{0}\n{1}\n{2}\n{3}",
                                                        "Do you want to leave this file in it's current location?",
                                                        "Yes - Leave the file in its current location",
                                                        "No - Copy it to the local Project",
                                                        "Cancel - Abort");
                        string Title = "Copy assigned source code?";
                        res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        this.Focus();
                        if (res == DialogResult.No)
                        {
                            File.Copy(ofd.FileName, _LocalCode);
                            AssignSourceFile(_LocalCode);
                            LoadSourceFile();
                        }
                        else if (res == DialogResult.Yes)
                        {
                            AssignSourceFile(ofd.FileName);
                            LoadSourceFile();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        AssignSourceFile(_LocalCode);
                        if (_SourceFile.Exists)
                        {
                            LoadSourceFile();
                        }
                        else
                        {
                            AssignEmptyFile(_LocalCode);
                        }
                    }
                }
                _Assigning = false;
                return true;
            }
            catch
            {
                _Assigning = false;
                return false;
            }
        }
        /// <summary>
        /// Creates and Assigns an empty source file to the PE core, at the provided path location.
        /// </summary>
        /// <param name="SourcePath">The path to the source file to be created.</param>
        /// <returns>True if the assignment was successful, false if not or an error occurred.</returns>
        public bool AssignEmptyFile(string SourcePath)
        {
            try
            {
                if (File.Exists(SourcePath))
                {
                    // File already exists, confirm overwrite
                    string Message = String.Format("The specified file, {0}, already exists.  Overwrite it?", SourcePath);
                    string Title = "File Already Exists";
                    DialogResult res;
                    res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    this.Focus();
                    if (res == DialogResult.No)
                    {
                        return false;
                    }
                }
                AssignSourceFile(SourcePath);
                txtSource.Text = string.Empty;
                SaveSourceFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Clears any reference to code source assigned to the core.
        /// </summary>
        public void ClearAssignedFile()
        {
            _SourceFile = null;
            _CompCore.LocalCodeSource = string.Empty;
            txtSource.Clear();
            txtSource.Enabled = false;
            _SavedHere = true;
            _LastSavedHere = DateTime.MinValue;
            status.Text = "PE Core is no longer assigned any source code file.";
        }

        /// <summary>
        /// Saves the contents of the textbox to the associated source file and updates the last-saved time.
        /// </summary>
        public void SaveSourceFile()
        {
            if (_SourceFile != null)
            {
                StreamWriter writer = new StreamWriter(_SourceFile.FullName);
                string Contents = txtSource.Text;
                Contents = Contents.Replace("\r\n", "\n");
                writer.Write(Contents);
                writer.Close();
                _LastSavedHere = _SourceFile.LastWriteTime;
                _SavedHere = true;
            }
        }
        /// <summary>
        /// Loads the source file associated with the PE core into the text box, if possible.  Also prompts for assignment, creation, copy, and overwrite of code files as they are loaded.
        /// </summary>
        public void LoadSourceFile()
        {
            if (_SourceFile != null)
            {
                if (_SourceFile.Exists)
                {
                    StreamReader reader = new StreamReader(_SourceFile.FullName);
                    string Contents = reader.ReadToEnd();
                    Contents = Contents.Replace("\n", "\r\n");
                    while (Contents.Contains("\r\r\n"))
                        Contents = Contents.Replace("\r\r\n", "\r\n");
                    txtSource.Text = Contents;
                    reader.Close();
                    _LastSavedHere = _SourceFile.LastWriteTime;
                    status.Text = String.Format("Loaded file: {0}", _SourceFile.FullName);
                    status.Enabled = true;
                    _SavedHere = true;
                    this.Focus();
                }
                else
                {
                    string Message = String.Format("The specified file, {0}, does not exist.  Create it?", _SourceFile.FullName);
                    string Title = "File Doesn't Exist";
                    DialogResult res;
                    res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    this.Focus();
                    if (res == DialogResult.Yes)
                    {
                        if (AssignEmptyFile(_SourceFile.FullName))
                        {
                            LoadSourceFile();
                        }
                        else
                        {
                            status.Text = "No source file assigned.";
                            txtSource.Enabled = false;
                        }
                    }
                    else
                    {
                        status.Text = "File modified outside editor! Currently displayed contents is not the most recent version!";
                    }
                }
            }
            else
            {
                if (_Assigning)
                    return;
                string Message = String.Format("There is no code assigned to this PE core.  Assign existing code?");
                string Title = "No Code Assigned";
                DialogResult res;
                res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                this.Focus();
                if (res == DialogResult.Yes)
                {
                    // Yes - Assign existing
                    AssignExistingFile(_LocalCode);
                }
                else if (res == DialogResult.No)
                {
                    // No - Create new code file
                    AssignEmptyFile(_LocalCode);
                    LoadSourceFile();
                }
                else
                {
                    // Cancel - Abort assignment
                    this.Close();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (_SourceFile != null)
            {
                string Message = String.Format("Are you sure you want to erase the contents of this source file? {0}", _SourceFile.Name);
                string Title = "Create new source file?";
                DialogResult res;
                res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                this.Focus();
                if (res == DialogResult.No)
                {
                    return;
                }
            }
            AssignEmptyFile(_LocalCode);
            LoadSourceFile();
        }
        private void btnOpenExisting_Click(object sender, EventArgs e)
        {
            if (AssignExistingFile(_LocalCode))
            {
                LoadSourceFile();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_SourceFile != null)
            {
                SaveSourceFile();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_SourceFile != null)
            {
                string Message = "Are you sure you want to remove the code assigned to this core?";
                string Title = "Remove Code Assignment?";
                DialogResult res;
                res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                this.Focus();
                if (res == DialogResult.Yes)
                {
                    ClearAssignedFile();
                    return;
                }
            }
        }
        private void btnReload_Click(object sender, EventArgs e)
        {
            if (_SourceFile != null)
            {
                LoadSourceFile();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnExternalEditor_Click(object sender, EventArgs e)
        {
            if (_SourceFile != null)
            {
                if (_SourceFile.Exists)
                {
                    LaunchExternalEditor(_SourceFile.FullName);
                }
            }
        }
    }
}
