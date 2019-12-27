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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmDowngrader : Form
    {
        private short version = 0;
        private bool noErr;

        public frmDowngrader()
        {
            InitializeComponent();
        }

        private void frmDowngrader_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "Working...";
            groupBox1.Enabled = false;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    version = 0;
                    backgroundWorker1.RunWorkerAsync();
                break;

                case 1:
                    version = 3;
                    backgroundWorker1.RunWorkerAsync();
                break;

                case 2:
                    version = 4;
                    backgroundWorker1.RunWorkerAsync();
                break;

                case 3:
                    version = 5;
                    backgroundWorker1.RunWorkerAsync();
                    break;

                case 4:
                    version = 1;
                    backgroundWorker1.RunWorkerAsync();
                    break;

                case 5:
                    version = 2;
                    backgroundWorker1.RunWorkerAsync();
                    break;

                default:

                break;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           // try
           // {
                download(version);
           // }
          //  catch(Exception ex)
           // {
           //     MessageBox.Show("Downgrade failed!\nWindows reported the error:\n" + ex.Message + "\nUse 'Restore game data..' to return your game to it's original state.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
           // }
        }

        private void download(short dl)
        {
            if(dl == 0)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://sergi4ua.com/equine/versions/Diablo-1.00.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            else if (dl == 1)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://sergi4ua.com/equine/versions/Diablo-1.07.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            if (dl == 2)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://sergi4ua.com/equine/versions/Diablo-1.08.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            if (dl == 3)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://www.sergi4ua.com/equine/versions/Diablo-1.02.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            if (dl == 4)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://www.sergi4ua.com/equine/versions/Diablo-1.03.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            if (dl == 5)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://www.sergi4ua.com/equine/versions/Diablo-1.04.zip", Application.StartupPath + "\\dv100.zip");
                }

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\dv100.zip", System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\dv100.zip");
            }
            noErr = true;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(noErr)
                MessageBox.Show("Operation completed successfully!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
            this.Close();
        }
    }
}
