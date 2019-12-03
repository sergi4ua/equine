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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmModDownloader : Form
    {
        public string modName { set; get; }
        public string afterDownloadMsg { set; get; }
        public string beforeDownloadMsg { set; get; }
        public string startExe0 { set; get; }
        public string startExe1 { set; get; }
        public string dlLink0 { set; get; }
        public string dlLink1 { set; get; }
        private Downloader dl;
        private List<string> extractedFileList = new List<string>();
        private string fileName;

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        Int64 fileRozmir = 0;

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public frmModDownloader()
        {
            InitializeComponent();
        }

        private void frmModDownloader_Load(object sender, EventArgs e)
        {
            // file rozmir = file size in ukrainian (translit)
            label1.Text = "Please wait while EQUINE installs " + modName + " to your Diablo installation.";
            getFileSize.RunWorkerAsync();

            if (beforeDownloadMsg != "null")
                MessageBox.Show(beforeDownloadMsg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            List<string> dlLinks = new List<string>();
            dlLinks.Add(dlLink0);
            if(dlLink1 != "null")
                dlLinks.Add(dlLink1);
            dl = new Downloader(dlLinks, Application.StartupPath + "\\");
            timer1.Start();
            timer1.Enabled = true;
            try
            {
                dl.BeginDownload();
            }
            catch
            {
                MessageBox.Show("Unable to communicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Hide();
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = (int)dl.downloadProgress;
            status.Text = "Downloading mod data (" + progressBar1.Value + "%)";

            if (dl.IsDone == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;
                status.Text = "Extracting mod ZIP-archive...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void ExtractFile()
        {
            try
            {
                Uri dlUri = new Uri(dlLink0);
                fileName = System.IO.Path.GetFileName(dlUri.LocalPath);

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\" + fileName, System.IO.FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    extractedFileList.Add(Application.StartupPath + "\\" + modName + "\\" + entry.FilenameInZip);
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + modName + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                System.IO.File.Delete(Application.StartupPath + "\\" + fileName);
                if (afterDownloadMsg != "null")
                    MessageBox.Show(afterDownloadMsg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar1.BeginInvoke((MethodInvoker)delegate () { progressBar1.Style = ProgressBarStyle.Continuous; });
                progressBar1.BeginInvoke((MethodInvoker)delegate () { progressBar1.MarqueeAnimationSpeed = 0; });

                CreateUninstallFile();

                if(File.Exists(Application.StartupPath + "\\" + startExe0))
                {
                    this.WindowState = FormWindowState.Minimized;
                    var exe = System.Diagnostics.Process.Start(Application.StartupPath + "\\"+startExe0);
                    exe.WaitForExit();
                    this.WindowState = FormWindowState.Normal;
                }

                List<string> fileNames = new List<string>{ "Storm.dll", "DiabloUI.dll", "Diablo.exe", "DIABDAT.MPQ", "SMACKW32.DLL", "ddraw.dll", "STANDARD.SNP", "BATTLE.SNP", "hellfrui.dll", "hfmonk.mpq", "hfmusic.mpq", "hfvoice.mpq", "hellfire.mpq" };

                try
                {

                    for (short i = 0; i < fileNames.Count; i++)
                    {
                        if (File.Exists(Application.StartupPath + "\\" + fileNames[i]))
                        {
                            CreateSymbolicLink(Application.StartupPath + "\\" + modName + "\\" + fileNames[i],
                            Application.StartupPath + "\\" + fileNames[i], SymbolicLink.File);
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Unknown error:\n" + ex.Message, "Critical Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                MessageBox.Show("Mod " + modName + " installed successfully!\nApplication will now restart (if it didn't, please restart the application manually)", "Installation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to install modification.\nEither download is unavailable or ZIP extracting error has occured.\nPlease try again, if the error persists please contact EQUINE developers.\n\n" + ex.Message, "Error", MessageBoxButtons.OK);
                this.BeginInvoke((MethodInvoker)delegate () { this.Hide(); });
                this.BeginInvoke((MethodInvoker)delegate () { this.Close(); });
            }
        }

        private void CreateUninstallFile()
        {
            // do nothing
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.MarqueeAnimationSpeed = 0;
            status.Text = "Canceling and cleaning up...";
            try
            {
                dl.Abort();
                System.IO.File.Delete(Application.StartupPath + "\\" + fileName);

                if(extractedFileList.Count > 0)
                {
                    for (int i = 0; i < extractedFileList.Count; i++)
                    {
                        FileAttributes attr = File.GetAttributes(extractedFileList[i]);

                        if (attr.HasFlag(FileAttributes.Directory))
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(extractedFileList[i]);
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                        else
                        {
                            File.Delete(extractedFileList[i]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Hide();
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ExtractFile();
        }

        private void getFileSize_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient client = new WebClient();
            client.OpenRead(dlLink0);
            fileRozmir = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
        }

        private void getFileSize_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(fileRozmir > 0)
            {
                fileSize.Text = FileSizeHelper.getFormattedFileSize(fileRozmir);
            }
            else
            {
                fileSize.Text = "Failed to get file size";
            }
        }
    }
}
