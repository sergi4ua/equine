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

namespace eqmpqedit
{
    public partial class frmMain : Form
    {

        uint mpqHandle = 0;
        bool listFileMPQ = false; // if list file inside MPQ is used

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
        }

        private void OpenMPQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog mpqOpenDialog = new OpenFileDialog();
            mpqOpenDialog.Title = "Open MPQ...";
            mpqOpenDialog.InitialDirectory = Application.StartupPath;
            mpqOpenDialog.Filter = "MPQ Files (*.mpq)|*.mpq|The Hell 1/2 Archive (*.mor)|*.mor|All files (*.*)|*.*";

            if(mpqOpenDialog.ShowDialog() == DialogResult.OK)
            {
                if (mpqHandle != 0)
                {
                    if (SFileCloseArchive(mpqHandle) == true)
                    {
                        listBox1.Items.Clear();
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

            uint hFile = 0;
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
                }
            }

            // get the files from each list file

            if (!listFileMPQ)
            {
                for (int i = 0; i < GlobalVariableContainer.listFiles.Count; i++)
                {
                    StreamReader listFile = new StreamReader(GlobalVariableContainer.listFiles[i]);

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

            // check if all files are valid

            foreach (var item in listFileContent)
            {
                if (Storm.SFileOpenFileEx(mpqHandle, item, 0x00, ref hFile))
                {
                    mpqFileNames.Add(item);
                }

                SFileCloseFile(hFile);
            }

            // display the files

            this.BeginInvoke((MethodInvoker)delegate () { listBox1.Items.AddRange(mpqFileNames.ToArray()); });
            this.BeginInvoke((MethodInvoker)delegate () { this.Enabled = true; });
            appStatus.Text = "Done.";

            // disable the wait cursor

            Application.UseWaitCursor = false;
        }

        private void MPQInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mpqHandle == 0)
            {
                MessageBox.Show("MPQ not opened or invalid.");
                return;
            }
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
                    listBox1.Items.Clear();
                    GC.Collect();
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

            appFiles.Text = "Files: " + listBox1.Items.Count;
        }
    }
}
