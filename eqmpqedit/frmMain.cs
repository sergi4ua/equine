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
            
        }

        private void OpenMPQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog mpqOpenDialog = new OpenFileDialog();
            mpqOpenDialog.Title = "Open MPQ...";
            mpqOpenDialog.InitialDirectory = Application.StartupPath;
            mpqOpenDialog.Filter = "MPQ Files (*.mpq)|*.mpq|The Hell 1/2 Archive (*.mor)|*.mor|All files (*.*)|*.*";

            if(mpqOpenDialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();

                if (System.IO.File.Exists(mpqOpenDialog.FileName))
                {
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
            Application.UseWaitCursor = true;
            List<string> mpqFileNames = new List<string>();
            mpqFileNames.Clear();
            uint hFile = 0;
            uint files = 0;

            foreach (var item in GlobalVariableContainer.listFiles)
            {
                if (File.Exists(item))
                {
                    List<string> temp = File.ReadAllLines(item).ToList();
                    mpqFileNames.AddRange(temp);
                }
            }

            foreach (var file in mpqFileNames)
            {
                appStatus.Text = "Probing file: " + file;

                if (SFileOpenFileEx(mpqHandle, file, 0x00, ref hFile))
                {
                    listBox1.BeginInvoke((MethodInvoker)delegate () { listBox1.Items.Add(file); });
                    files++;
                    appFiles.Text = "Files: " + Convert.ToString(files);
                }
            }

            if(hFile != 0)
                SFileCloseFile(hFile);

            Application.UseWaitCursor = false;
            this.BeginInvoke((MethodInvoker)delegate () { this.Enabled = true; });
            appStatus.Text = "Done.";
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
    }
}
