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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmUpdateEquineData : Form
    {
        public frmUpdateEquineData()
        {
            InitializeComponent();
        }

        private void frmUpdateEquineData_Load(object sender, EventArgs e)
        {
            label1.Parent = pictureBox1;
            this.Cursor = Cursors.WaitCursor;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
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
                System.Threading.Thread.Sleep(2000);
                MessageBox.Show("EquineData updated successfully!\nEQUINE should now restart.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Initalization failed!\n" + ex.Message + "\nEQUINE will now quit.", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath,
            };
            Process.Start(SelfProc);
            Application.Exit();
        }
    }
}
