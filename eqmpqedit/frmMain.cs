/*Copyright(C) 2018 Sergi4UA

This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using static eqmpqedit.Storm;
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace eqmpqedit
{
    public partial class frmMain : Form
    {

        uint mpqHandle = 0;
        bool listFileMPQ = false; // if list file inside MPQ is used
        string openMPQPath = "";
        public frmMain()
        {
            InitializeComponent();
        }

        unsafe void openMPQ(string fileName)
        {
            if (SFileOpenArchive(fileName, 2, 0x8000, ref mpqHandle) == true)
            {
                Text = "EQUINE MPQEdit - " + Path.GetFileName(fileName);
            }
            else
            {
                MessageBox.Show("Couldn't open: " + fileName + "\nThe file is either corrupted or not a compatible MPQ archive at all.",
                    "Broken Pipe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Text = "EQUINE MPQEdit - " + Path.GetFileName(fileName) + " [ERROR]";
            }

            Enabled = false;
            Application.UseWaitCursor = true;

            appStatus.Text = "Opening: " + fileName;
            openMPQPath = fileName;
            buildList.RunWorkerAsync();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mpqHandle != 0)
                SFileCloseArchive(mpqHandle);

            this.Hide();
            this.Dispose();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if(File.Exists("EquineData/eqmpqedit/listfile.tmp"))
            {
                File.Delete("EquineData/eqmpqedit/listfile.tmp");
            }

            GlobalVariableContainer.listFiles.Clear();
            GlobalVariableContainer.listFiles.Add(Application.StartupPath + "\\EquineData\\eqmpqedit\\Diablo.txt");
            GlobalVariableContainer.listFiles.Add(Application.StartupPath + "\\EquineData\\eqmpqedit\\Hellfire.txt");

            listView1.AllowDrop = true;
        }

        private void OpenMPQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog mpqOpenDialog = new OpenFileDialog();
            mpqOpenDialog.Title = "Open MPQ...";
            mpqOpenDialog.Filter = "MPQ Files (*.mpq)|*.mpq|The Hell 1/2 Archive (*.mor)|*.mor|Diablo 1 Save Files (*.sv)|*.sv|Hellfire Save Files (*.hsv)|*.hsv|All files (*.*)|*.*";

            if(mpqOpenDialog.ShowDialog() == DialogResult.OK)
            {
                if (mpqHandle != 0)
                {
                    if (SFileCloseArchive(mpqHandle) == true)
                    {
                        listView1.Items.Clear();
                        mpqHandle = 0;
                        GC.Collect();
                    }
                }

                if (System.IO.File.Exists(mpqOpenDialog.FileName))
                {
                    listFileMPQ = false;
                    openMPQ(mpqOpenDialog.FileName);
                }
                else
                    MessageBox.Show("Internal error.");
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            openMPQToolStripMenuItem.PerformClick();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            if (mpqHandle != 0)
                settings.mpqOpen = true;
            else
                settings.mpqOpen = false;
            settings.ShowDialog();
        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private unsafe void BuildList_DoWork(object sender, DoWorkEventArgs e)
        {
            // enable the wait cursor

            Application.UseWaitCursor = true;
            List<string> mpqFileNames = new List<string>(); // contains valid MPQ filenames
            List<string> listFileContent = new List<string>(); // contains files from listfiles
            uint[] mpqFileSize;

            int listFileMPQ_handle = -1;

            // check if (listfile) exists

            if (!GlobalVariableContainer.ignoreEmbedListFile)
            {
                if (Storm.SFileOpenFile("(listfile)", ref listFileMPQ_handle))
                {
                    listFileMPQ = true;
                    uint fileSizeHigh = 0;
                    uint fileSize = Storm.SFileGetFileSize(listFileMPQ_handle, ref fileSizeHigh);

                    if ((fileSizeHigh == 0) && (fileSize > 0))
                    {
                        byte[] bs = new byte[fileSize];
                        uint countRead = 0;

                        Storm.SFileReadFile(listFileMPQ_handle, bs, fileSize, ref countRead, 0);

                        FileStream F = new FileStream("EquineData/eqmpqedit/listfile.tmp",
                            FileMode.Create, FileAccess.ReadWrite);
                        F.Write(bs, 0, bs.Length);
                        F.Close();
                        Storm.SFileCloseFile(listFileMPQ_handle);
                    }
                    else
                        listFileMPQ = false;
                }
                else
                {
                    if(GlobalVariableContainer.listFiles.Count == 0)
                    {
                        MessageBox.Show("No internal list file found. Please provide a list file in the settings.", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
            } else {
                if (GlobalVariableContainer.listFiles.Count == 0)
                {
                    MessageBox.Show("Using internal list files disabled in settings. Please provide a list file in the settings.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    MessageBox.Show("Using internal list files disabled.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // get the files from each list file

            if (!listFileMPQ)
            {
                for (int i3 = 0; i3 < GlobalVariableContainer.listFiles.Count; i3++)
                {
                    StreamReader listFile = new StreamReader(GlobalVariableContainer.listFiles[i3]);

                    while (!listFile.EndOfStream)
                    {
                        string temp = listFile.ReadLine();
                        listFileContent.Add(temp);
                    }

                    listFile.Close();
                    listFile.Dispose();
                }
            } 
            else
            {
                StreamReader listFile = new StreamReader("EquineData/eqmpqedit/listfile.tmp");
                while (!listFile.EndOfStream)
                {
                    string temp = listFile.ReadLine();
                    listFileContent.Add(temp);
                }

                listFile.Close();
                listFile.Dispose();
                File.Delete("EquineData/eqmpqedit/listfile.tmp");
            }

            // resize the mpqfilesize array
            mpqFileSize = new uint[listFileContent.Count];
            int i = 0;

            // check if all files are valid

            foreach (var item in listFileContent.Distinct())
            {
                uint nothing = 0;
                int tempHFile = 0;

                if (Storm.SFileOpenFile(item, ref tempHFile))
                {
                    if(GlobalVariableContainer.showMPQFileSize)
                        mpqFileSize[i] = SFileGetFileSize(tempHFile, ref nothing);
                    mpqFileNames.Add(item);
                    i++;
                }

                SFileCloseFile(tempHFile);
            }

            // display the files

            for(int i2 = 0; i2 < mpqFileNames.Count; i2++)
            {
                ListViewItem tempItem = new ListViewItem();
                tempItem.Text = mpqFileNames[i2];
                if (GlobalVariableContainer.showMPQFileSize)
                    tempItem.SubItems.Add(Convert.ToString(mpqFileSize[i2]));
                else
                    tempItem.SubItems.Add("N/A");

                this.BeginInvoke((MethodInvoker)delegate () {
                    listView1.Items.Add(tempItem);
                });
            }

            this.BeginInvoke((MethodInvoker)delegate () { this.Enabled = true; });
            appStatus.Text = "Done.";

            // disable the wait cursor

            Application.UseWaitCursor = false;
        }

        private void MPQInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(mpqHandle != 0)
                SFileCloseArchive(mpqHandle);

            
        }        

        private void CloseMPQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mpqHandle != 0)
            {
                if (SFileCloseArchive(mpqHandle) == true)
                {
                    listView1.Items.Clear();
                    mpqHandle = 0;
                    GC.Collect();
                    this.Text = "EQUINE MPQEdit";
                    toolStripButton4.Enabled = false;
                    toolStripButton5.Enabled = false;
                    toolStripButton2.Enabled = false;
                    addToolStripMenuItem.Enabled = false;
                    listFileMPQ = false;
                }
                else
                    MessageBox.Show("Internal error.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mpqHandle == 0)
                closeMPQToolStripMenuItem.Enabled = false;
            else
                closeMPQToolStripMenuItem.Enabled = true;
        }

        private void buildList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.UseWaitCursor = false;
            this.Cursor = Cursors.Default;

            if(listFileMPQ)
                appStatus.Text = "Done (internal listfile used)";

            appFiles.Text = "Files: " + listView1.Items.Count;

            this.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton4.Enabled = true;
            toolStripButton5.Enabled = true;
            addToolStripMenuItem.Enabled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Create new MPQ...";
            sfd.Filter = "MPQ files (*.mpq;*.mor)|*.mpq;*.mor|All files (*.*)|*.*";
            sfd.DefaultExt = ".mpq";
            
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(sfd.FileName))
                {
                    File.Delete(sfd.FileName);
                }

                int hMPQ = 0;
                hMPQ = MpqOpenArchiveForUpdate(sfd.FileName, MOAU_CREATE_NEW | MOAU_MAINTAIN_LISTFILE,
                    GlobalVariableContainer.MAX_MPQ_FILES);

                MpqCloseUpdatedArchive(hMPQ, 0);
                hMPQ = 0;
                listFileMPQ = false;
                closeMpq();
                System.Threading.Thread.Sleep(1000);
                openMPQ(sfd.FileName);
                addToolStripMenuItem.Enabled = true;
                toolStripButton4.Enabled = false;
                toolStripButton5.Enabled = false;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            addToolStripMenuItem.PerformClick();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Add file...";
            ofd.Filter = "All files (*.*)|*.*";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.DefaultExt = "*.*";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                addFileToMpq(ofd.FileName);
            }
        }

        private void addFileToMpq(string fileName)
        {
            if (File.Exists(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp"))
                File.Delete(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp");

            try
            {
                string shortFileName = Path.GetFileName(fileName);
                int listFileMPQ_handle = -1;
                int _hMPQ = -1;
                List<string> listFile = new List<string>();
                listFile.Add("(listfile)");
                bool fileExists = false;

                if(mpqHandle == 0)
                {
                    MessageBox.Show("MPQ is null", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Storm.SFileOpenFile("(listfile)", ref listFileMPQ_handle))
                {
                    uint fileSizeHigh = 0;
                    uint fileSize = Storm.SFileGetFileSize(listFileMPQ_handle, ref fileSizeHigh);

                    if ((fileSizeHigh == 0) && (fileSize > 0))
                    {
                        byte[] bs = new byte[fileSize];
                        uint countRead = 0;

                        Storm.SFileReadFile(listFileMPQ_handle, bs, fileSize, ref countRead, 0);

                        FileStream F = new FileStream(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp",
                            FileMode.Create, FileAccess.ReadWrite);
                        F.Write(bs, 0, bs.Length);
                        F.Close();
                        Storm.SFileCloseFile(listFileMPQ_handle);

                        // read the internal list file
                        listFile = File.ReadAllLines(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp").ToList();
                    }
                }
                else
                {
                    // create new internal list file
                    using(StreamWriter sw = new StreamWriter(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp"))
                    {
                        sw.WriteLine("(listfile)");
                        sw.WriteLine("(signature)");

                        listFile.Add("(listfile)");
                        listFile.Add("(signature)");

                        sw.Close();
                    }
                }

                string folderName = "";
                string originalSFD = shortFileName;
                var dResult = ShowInputDialog(ref folderName);
                if (dResult == DialogResult.Cancel)
                {
                    if(_hMPQ != -1)
                        MpqCloseUpdatedArchive(_hMPQ, 0);
                    MessageBox.Show("Operation cancelled.", "EQUINE MPQEdit");
                    return;
                }

                if (folderName != "")
                    shortFileName = folderName + "\\" + shortFileName;

                if (Storm.SFileOpenFile(shortFileName, ref listFileMPQ_handle))
                {
                    fileExists = true;
                }

                SFileCloseArchive(mpqHandle);
                mpqHandle = 0;
                System.Threading.Thread.Sleep(1000);

                bool replaceFile = false;

                if (fileExists)
                {
                    if (MessageBox.Show("File " + shortFileName + " already exists. The file will be replaced. Continue?", "EQUINE MPQEdit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        replaceFile = true;
                    }
                    else
                    {
                        replaceFile = false;
                        return;
                    }
                }

                _hMPQ = MpqOpenArchiveForUpdate(openMPQPath, Storm.MOAU_OPEN_EXISTING, GlobalVariableContainer.MAX_MPQ_FILES);

                if(_hMPQ == 0)
                {
                    throw new Exception("ErrCode: " + Convert.ToString(Marshal.GetLastWin32Error()));
                }

                if(replaceFile)
                {
                    MpqDeleteFile(_hMPQ, shortFileName);
                }

                // MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, Storm.MAFA_COMPRESS_STANDARD, 0x00000001,
                //     Storm.MAFA_COMPRESS_STANDARD);

                switch (GlobalVariableContainer.compressionType)
                {
                    case GlobalVariableContainer.CompressionType.STANDARD:
                        MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, 0x00000100, Storm.MAFA_COMPRESS_STANDARD,
                    Storm.MAFA_COMPRESS_STANDARD);
                        break;

                    case GlobalVariableContainer.CompressionType.BZIP2:
                        MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, 0x00000200, 0x10,
                    Storm.MAFA_COMPRESS_STANDARD);
                        break;

                    case GlobalVariableContainer.CompressionType.ZLIB:
                        MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, 0x00000200, MAFA_COMPRESS_DEFLATE,
                    1);
                        break;

                    case GlobalVariableContainer.CompressionType.WAVE:
                        MpqAddWaveToArchive(_hMPQ, fileName, shortFileName, 0x00000001, 1);
                        MessageBox.Show("WAVE compression enabled.", "Note");
                        break;

                    case GlobalVariableContainer.CompressionType.NO_COMPRESSION:
                        MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, 0x80000000, 0, 0);
                        break;

                    default:
                        MpqAddFileToArchiveEx(_hMPQ, fileName, shortFileName, 0x00000100, Storm.MAFA_COMPRESS_STANDARD,
                    Storm.MAFA_COMPRESS_STANDARD);
                        break;
                }

                if(!GlobalVariableContainer.dontGenerateListFile)
                    MpqDeleteFile(_hMPQ, "(listfile)");

                listFile.Add(shortFileName);

                if (!GlobalVariableContainer.dontGenerateListFile)
                {
                    using (StreamWriter sw = new StreamWriter(Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp"))
                    {
                        foreach (var item in listFile.Distinct())
                        {
                            sw.WriteLine(item);
                        }
                        sw.Close();
                    }
                }

                if (!GlobalVariableContainer.dontGenerateListFile)
                {
                    MpqAddFileToArchiveEx(_hMPQ, Application.StartupPath + "/EquineData/eqmpqedit/listfile.tmp", "listfile.tmp", 0x00000100, Storm.MAFA_COMPRESS_STANDARD,
                       Storm.MAFA_COMPRESS_STANDARD);

                    MpqDeleteFile(_hMPQ, "(listfile)");

                    MpqRenameFile(_hMPQ, "listfile.tmp", "(listfile)");

                }
                MpqCloseUpdatedArchive(_hMPQ, 0);

                listView1.Items.Clear();
                openMPQ(openMPQPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding file:\t" + fileName + "\nMessage:\t" + ex.Message,
                    "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            
        }

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(250, 180);
            Form inputBox = new Form();
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Folder name";

            Label label = new Label();
            label.Text = "Please type the path, where the file can\nbe accessed inside the MPQ (without \\ and filename).\nIf the file already exists, it will be overwritten.";
            label.Size = new System.Drawing.Size(size.Width - 10, 70);
            label.Location = new System.Drawing.Point(5, 63);
            inputBox.Controls.Add(label);

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem.PerformClick();
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if(files.Length > 1)
                {
                    if(MessageBox.Show("You have selected multiple files. Currently EQUINE MPQEdit does not support multifile drag 'n' drop.\nIf you click 'Yes' random file from your selection will be chosen.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                }

                try
                {
                    addFileToMpq(files[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to add file via drag'n'drop.\nMessage:\t " + ex.Message);
                    return;
                }
            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void closeMpq()
        {
            if (mpqHandle != 0)
            {
                if (SFileCloseArchive(mpqHandle) == true)
                {
                    listView1.Items.Clear();
                    GC.Collect();
                    this.Text = "EQUINE MPQEdit";
                    toolStripButton4.Enabled = false;
                    toolStripButton5.Enabled = false;
                    toolStripButton2.Enabled = false;
                }
                else
                    MessageBox.Show("Internal error.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                if(MessageBox.Show("File " + listView1.SelectedItems[0].Text + " will be deleted.\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    MpqFuncs.deleteFile(mpqHandle, listView1.SelectedItems[0].Text, openMPQPath, closeMpq);
                    openMPQ(openMPQPath);
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem.PerformClick();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem.PerformClick();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFileProperties fp = new frmFileProperties();
            fp.fileName = listView1.SelectedItems[0].Text;
            int hFile = -1;
            uint fileSizeHigh = 0;

            try
            {
                if(SFileOpenFile(listView1.SelectedItems[0].Text, ref hFile))
                {
                    fp.sizeUncompressed = SFileGetFileInfo(hFile, Storm.SFILE_INFO_SIZE);
                    fp.fileFlags = SFileGetFileInfo(hFile, Storm.SFILE_INFO_FLAGS);
                    fp.sizeCompressed2 = (int)SFileGetFileSize(hFile, ref fileSizeHigh);
                    fp.hash = SFileGetFileInfo(hFile, Storm.SFILE_INFO_HASH_INDEX);
                    SFileCloseFile(hFile);
                    hFile = 0;
                }
                else
                {
                    throw new System.IO.IOException("");
                }
            }
            catch
            {
                MessageBox.Show("I/O error", "EQUINE MPQEdit",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            fp.ShowDialog();
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string mpqFileName = listView1.SelectedItems[0].Text;
                if (!MpqFuncs.fileExtact(mpqFileName))
                {
                    if (Marshal.GetLastWin32Error() != 0)
                        MessageBox.Show("Unable to extract file. ErrCode: " + Convert.ToString(Marshal.GetLastWin32Error()));
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                extractToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
                toolStripButton4.Enabled = true;
                toolStripButton5.Enabled = true;
            }
            else
            {
                extractToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.Enabled = false;
                toolStripButton4.Enabled = false;
                toolStripButton5.Enabled = false;
            }

            if (mpqHandle == 0)
                addToolStripMenuItem.Enabled = false;
            else
                addToolStripMenuItem.Enabled = true;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            extractToolStripMenuItem.PerformClick();
        }

        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            extractToolStripMenuItem.PerformClick();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string oldFileName = listView1.SelectedItems[0].Text;

                frmRename rename = new frmRename();
                rename.fileName = oldFileName;
                rename.ShowDialog();
                var dResult = rename.DialogResult;

                if (dResult == DialogResult.OK)
                {
                    string currentMpqPath = openMPQPath;
                    closeMpq();
                    System.Threading.Thread.Sleep(1000);
                    int __hMpq = MpqOpenArchiveForUpdate(currentMpqPath, MOAU_OPEN_EXISTING | MOAU_MAINTAIN_LISTFILE, GlobalVariableContainer.MAX_MPQ_FILES);
                    if(__hMpq == -1)
                    {
                        MessageBox.Show("Failed to rename file.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        openMPQ(currentMpqPath);
                        return;
                    }

                    MpqRenameFile(__hMpq, oldFileName, rename.fileName);
                    MpqCloseUpdatedArchive(__hMpq, 0);
                    openMPQ(currentMpqPath);
                }
            }
        }

        private void moveToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void supportOnPatreonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://patreon.com/sergi4ua");
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                extractToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
                toolStripButton4.Enabled = true;
                toolStripButton5.Enabled = true;
            }
            else
            {
                extractToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.Enabled = false;
                toolStripButton4.Enabled = false;
                toolStripButton5.Enabled = false;
            }

            if (mpqHandle == 0)
                addToolStripMenuItem.Enabled = false;
            else
                addToolStripMenuItem.Enabled = true;
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
                renameToolStripMenuItem.PerformClick();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool no_results = true;

            frmSearch searcher = new frmSearch();
            searcher.ShowDialog();

            if(searcher.DialogResult == DialogResult.OK)
            {
                listView1.SelectedIndices.Clear();
                foreach(ListViewItem lvi in listView1.Items)
                {
                    if(lvi.Text == searcher.fileName)
                    {
                        listView1.SelectedIndices.Add(lvi.Index);
                        listView1.EnsureVisible(lvi.Index);
                        no_results = false;
                        break;
                    }
                }

                if (no_results)
                    MessageBox.Show("No file found.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
