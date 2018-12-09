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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO.Compression;
using System.IO;

namespace EQUINE
{
    public partial class frmForceUpdate : Form
    {
        public bool _109b { get; internal set; }

        public frmForceUpdate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("OK to force update?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                forceUpdate();
                button1.Enabled = false;
            }
        }

        private void forceUpdate()
        {
            button1.Text = "Working...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    if(_109b == false)
                        wc.DownloadFile("http://sergi4ua.pp.ua/equine/versions/d1forceupdate.zip", Application.StartupPath + "\\d1forceupdate.zip");
                    else
                        wc.DownloadFile("http://sergi4ua.pp.ua/equine/versions/Diablo-1.09b.zip", Application.StartupPath + "\\d1forceupdate.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\d1forceupdate.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\d1forceupdate.zip");

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

                MessageBox.Show("Operation completed successfully!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Force update failed.\nWindows reported the error:\n" + ex.Message, "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            this.Close();
        }

        private void frmForceUpdate_Load(object sender, EventArgs e)
        {
            if (_109b == true)
                Text = "Force update (1.09b)";
            else
                Text = "Force update (1.09)";
        }
    }
}
