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

        public frmModDownloader()
        {
            InitializeComponent();
        }

        private void frmModDownloader_Load(object sender, EventArgs e)
        {
            label1.Text = "Please wait, while EQUINE installs " + modName + " to your Diablo installation. This may take a few moments.\nThe program may hang due to files being extracted from the ZIP archive.\nDo not kill EQUINE process when the program is not responding.";

            if (beforeDownloadMsg != "null")
                MessageBox.Show(beforeDownloadMsg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            List<string> dlLinks = new List<string>();
            dlLinks.Add(dlLink0);
            if(dlLink1 != "null")
                dlLinks.Add(dlLink1);
            dl = new Downloader(dlLinks, Application.StartupPath + "\\");
            timer1.Start();
            timer1.Enabled = true;
            dl.BeginDownload();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = (int)dl.downloadProgress;
            if(dl.IsDone == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;
                status.Text = "Extracting file...";
                ExtractFile();
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
                    extractedFileList.Add(Application.StartupPath + "\\" + entry.FilenameInZip);
                    zip.ExtractFile(entry, Application.StartupPath + "\\" + entry.FilenameInZip);
                }
                zip.Close();
                System.IO.File.Delete(Application.StartupPath + "\\" + fileName);
                if (afterDownloadMsg != "null")
                    MessageBox.Show(afterDownloadMsg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.MarqueeAnimationSpeed = 0;

                CreateUninstallFile();

                if(File.Exists(Application.StartupPath + startExe0))
                {
                    this.WindowState = FormWindowState.Minimized;
                    var exe = System.Diagnostics.Process.Start(Application.StartupPath + startExe0);
                    exe.WaitForExit();
                    this.WindowState = FormWindowState.Maximized;
                }

                MessageBox.Show("Mod " + modName + " installed!\nApplication will now restart.", "Installation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
            catch
            {
                MessageBox.Show("Unable to install modification.\nEither download is unavailable or ZIP extracting error has occured.\nPlease try again, if error persists contact EQUINE developers.", "Error", MessageBoxButtons.OK);
            }
        }

        private void CreateUninstallFile()
        {
            TextWriter textWriter = new StreamWriter(Application.StartupPath + "\\EquineData\\moduninstall\\" + modName + ".uninstall");
            textWriter.Flush();
            for (int i = 0; i < extractedFileList.Count; i++)
            {
                textWriter.WriteLine(extractedFileList[i]);
            }
            textWriter.Close();
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
    }
}
