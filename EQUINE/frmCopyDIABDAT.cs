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
    along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace EQUINE
{
    public partial class frmCopyDIABDAT : Form
    {
        private DriveInfo[] drv = DriveInfo.GetDrives();
        private List<string> rawDriveLetter = new List<string>();

        public frmCopyDIABDAT()
        {
            InitializeComponent();
        }

        CopyFileCallbackAction myCallback(FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred)
        {
            double dProgress = (totalBytesTransferred / (double)totalFileSize) * 100.0;
            progressBar1.Value = (int)dProgress;
            if (progressBar1.Value >= 100)
            {
                MessageBox.Show("Operation completed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.SetAttributes(Application.StartupPath + "\\DIABDAT.MPQ", FileAttributes.Normal);
                Hide();
            }
            return CopyFileCallbackAction.Continue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            comboBox1.Enabled = false;
            Text = "Progress...";
            try
            {
                FileRoutines.CopyFile(new FileInfo(rawDriveLetter[comboBox1.SelectedIndex] + "\\DIABDAT.MPQ"), new FileInfo(Application.StartupPath + "\\DIABDAT.MPQ"), CopyFileOptions.None, myCallback);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to copy DIABDAT.MPQ. Windows reported the error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Hide();
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void frmCopyDIABDAT_Load_1(object sender, EventArgs e)
        {
            if (File.Exists("DIABDAT.MPQ"))
            {
                if (MessageBox.Show("DIABDAT.MPQ already exists. Do you still want to copy it?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        
                        File.Delete(Application.StartupPath + "\\DIABDAT.MPQ");
                    }
                    catch
                    {
                        MessageBox.Show("Unable to delete DIABDAT.MPQ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Hide();
                        Close();
                    }
                }
                else
                {
                    Hide();
                    Close();
                }
            }

            foreach (DriveInfo d in drv)
            {
                if (d.IsReady == true)
                {
                    if (d.DriveType == DriveType.CDRom)
                    {
                        comboBox1.Items.Add(d.Name + " " + d.VolumeLabel);
                        rawDriveLetter.Add(d.Name);
                    }
                }
            }

            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("No CD inserted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Hide();
                Close();
            }
            else
                comboBox1.SelectedIndex = 0;
        }
    }
}
