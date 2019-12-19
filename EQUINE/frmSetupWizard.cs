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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmSetupWizard : Form
    {
        public string btnText = "Downloading...";

        public frmSetupWizard()
        {
            InitializeComponent();
        }

        private void frmSetupWizard_Load(object sender, EventArgs e)
        {
            diabloDir.Text = Application.StartupPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = btnText;
            button1.Enabled = false;
           try
            {
                // add support for TLS 1.2 (screaming)
                WebClient wc = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                wc.DownloadFile("https://sergi4ua.pp.ua/equine/EquineData.zip", Application.StartupPath + "\\equinedata.zip");

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\equinedata.zip", FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\EquineData\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete("equinedata.zip");
                MessageBox.Show("Initalization completed successfully!\nEQUINE will now restart (if the program didn't restart, please start it manually).", "EQUINE initalized", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();

                try
                {
                    if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                    {
                        if (!Directory.Exists(Application.StartupPath + "\\EquineData\\GameBackup"))
                        {
                            Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\GameBackup");
                        }
                        if (File.Exists(Application.StartupPath + "\\Storm.dll") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\Storm.dll"))
                        {
                            File.Copy(Application.StartupPath + "\\Storm.dll", Application.StartupPath + "\\EquineData\\GameBackup\\Storm.dll");
                        }

                        if (File.Exists(Application.StartupPath + "\\SMACKW32.DLL") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\SMACKW32.DLL"))
                        {
                            File.Copy(Application.StartupPath + "\\SMACKW32.DLL", Application.StartupPath + "\\EquineData\\GameBackup\\SMACKW32.DLL");
                        }

                        if (File.Exists(Application.StartupPath + "\\diabloui.dll") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\diabloui.dll"))
                        {
                            File.Copy(Application.StartupPath + "\\diabloui.dll", Application.StartupPath + "\\EquineData\\GameBackup\\diabloui.dll");
                        }

                        if (File.Exists(Application.StartupPath + "\\Diablo.exe") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\Diablo.exe"))
                        {
                            File.Copy(Application.StartupPath + "\\Diablo.exe", Application.StartupPath + "\\EquineData\\GameBackup\\Diablo.exe");
                        }
                        if (File.Exists(Application.StartupPath + "\\standard.snp") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\standard.snp"))
                        {
                            File.Copy(Application.StartupPath + "\\standard.snp", Application.StartupPath + "\\EquineData\\GameBackup\\standard.snp");
                        }
                        if (File.Exists(Application.StartupPath + "\\battle.snp") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\battle.snp"))
                        {
                            File.Copy(Application.StartupPath + "\\battle.snp", Application.StartupPath + "\\EquineData\\GameBackup\\battle.snp");
                        }
                    }
                }
                catch
                { MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            }
            catch (Exception ex)
           {
                MessageBox.Show("Failed to initalize EQUINE.\n" + ex.Message, "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(2);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(new Uri("http://sergi4ua.pp.ua/equine/EquineData.zip"), Application.StartupPath + "\\equinedata.zip");

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\equinedata.zip", FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\EquineData\\" + entry.FilenameInZip);
                }
                zip.Close();
                MessageBox.Show("Initalization completed successfully!", "EQUINE initalized", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initalize EQUINE.\n" + ex.Message, "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(2);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            button1.PerformClick();
        }

        private void frmSetupWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
