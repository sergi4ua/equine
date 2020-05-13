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

// TODO: implement cancel button someday

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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
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

        public bool Polska { get; set; }

        public bool toolDLMode { get; set; }

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        Int64 fileRozmir = 0;

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        bool cancelled = false;

        public frmModDownloader()
        {
            InitializeComponent();
        }

        private void frmModDownloader_Load(object sender, EventArgs e)
        {
            Logger.log("Init download for: " + modName, Logger.Level.INFO, Logger.App.MOD_DOWNLOADER);
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
                Logger.log("EQUINE is not running in admin mode", Logger.Level.WARNING, Logger.App.MOD_DOWNLOADER);
                MessageBox.Show("You must have Administrator rights to install or update mods. Relaunching application...", "This action requires elevation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                string fileName = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = fileName;

                Process.Start(processInfo);

                Environment.Exit(0);
             }

            // file rozmir = file size in ukrainian (translit)
            if (!Polska)
                label1.Text = "Please wait while EQUINE installs " + modName + " to your Diablo installation.";
            else
                label1.Text = "EQUINE zmienia język twojej instalacji Diablo na Polski...";
            getFileSize.RunWorkerAsync();

            if (Polska)
            {
                groupBox1.Text = "Postęp";
                label3.Text = "Rozmiar pliku:";
                Text = "Instalowanie...";
            }

            if (toolDLMode)
                status.Text = "Downloading tool data...";

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
                Logger.log("Communication error.", Logger.Level.WARNING, Logger.App.MOD_DOWNLOADER);
                MessageBox.Show("Unable to communicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Hide();
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dl != null)
            {
                progressBar1.Value = (int)dl.downloadProgress;
                if (!toolDLMode)
                {
                    if (!Polska)
                        status.Text = "Downloading mod data (" + progressBar1.Value + "%)";
                    else
                        status.Text = "Pobieranie modyfikacji (" + progressBar1.Value + "%)";
                }
                else
                    status.Text = "Downloading mod tool (" + progressBar1.Value + "%)";

            }
            if (dl != null && dl.IsDone == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;
                if (!Polska)
                    status.Text = "Extracting ZIP-archive...";
                else
                    status.Text = "Wypakowywanie modyfikacji...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void ExtractFile()
        {
            Logger.log("Extracting ZIP file: " + fileName, Logger.Level.INFO, Logger.App.MOD_DOWNLOADER);
            try
            {
                if (!cancelled)
                {
                    //button1.BeginInvoke((MethodInvoker)delegate () { button1.Enabled = false; });
                    Uri dlUri = new Uri(dlLink0);
                    fileName = System.IO.Path.GetFileName(dlUri.LocalPath);

                    ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\" + fileName, System.IO.FileAccess.Read);
                    List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                    foreach (ZipStorer.ZipFileEntry entry in dir)
                    {
                        //extractedFileList.Add(Application.StartupPath + "\\" + modName + "\\" + entry.FilenameInZip);
                        Logger.log("Extracting: " + modName + "\\" + entry.FilenameInZip, Logger.Level.INFO, Logger.App.MOD_DOWNLOADER);
                        if (!toolDLMode)
                            zip.ExtractFile(entry, Application.StartupPath + "\\" + modName + "\\" + entry.FilenameInZip);
                        else
                            zip.ExtractFile(entry, Application.StartupPath + "\\EquineData\\ModdingTools\\" + modName + "\\" + entry.FilenameInZip);
                    }
                    Logger.log("ZIP file extracted", Logger.Level.INFO, Logger.App.MOD_DOWNLOADER);
                    zip.Close();
                    System.IO.File.Delete(Application.StartupPath + "\\" + fileName);
                    if (afterDownloadMsg != "null")
                        MessageBox.Show(afterDownloadMsg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    progressBar1.BeginInvoke((MethodInvoker)delegate () { progressBar1.Style = ProgressBarStyle.Continuous; });
                    progressBar1.BeginInvoke((MethodInvoker)delegate () { progressBar1.MarqueeAnimationSpeed = 0; });

                    CreateUninstallFile();

                    if (File.Exists(Application.StartupPath + "\\" + modName + "\\" + startExe0))
                    {
                        this.BeginInvoke((MethodInvoker)delegate () { this.WindowState = FormWindowState.Minimized; });
                        var exe = System.Diagnostics.Process.Start(Application.StartupPath + "\\" + modName + "\\" + startExe0);
                        exe.WaitForExit();
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            this.WindowState = FormWindowState.Normal;
                        });
                    }

                    List<string> fileNames = new List<string> { "Storm.dll", "DiabloUI.dll", "Diablo.exe", "DIABDAT.MPQ", "SMACKW32.DLL", "ddraw.dll", "STANDARD.SNP", "BATTLE.SNP", "hellfrui.dll", "hfmonk.mpq", "hfmusic.mpq", "hfvoice.mpq", "hellfire.mpq" };


                    try
                    {
                        // don't create symlinks for the tools
                        if (!toolDLMode)
                        {
                            
                            for (short i = 0; i < fileNames.Count; i++)
                            {
                                if (File.Exists(Application.StartupPath + "\\" + fileNames[i]))
                                {
                                    Logger.log("Creating symlink: " + fileNames[i] + " for mod " + modName, Logger.Level.INFO, Logger.App.MOD_DOWNLOADER);
                                    CreateSymbolicLink(Application.StartupPath + "\\" + modName + "\\" + fileNames[i],
                                    Application.StartupPath + "\\" + fileNames[i], SymbolicLink.File);

                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Unknown error:\n" + ex.Message, "Critical Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                if (!toolDLMode)
                {
                    if (!Polska)
                        MessageBox.Show("Mod " + modName + " installed successfully!\nApplication will now restart (if it didn't, please restart the application manually)", "Installation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Sukces. EQUINE uruchomi się ponownie.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Tool " + modName + " installed successfully!\nApplication will now restart (if it didn't, please restart the application manually)", "Installation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                }

                Polska = false;
            }
            catch(Exception ex)
            {
                Logger.log("Download failed!", Logger.Level.ERROR, Logger.App.MOD_DOWNLOADER);
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
                Logger.log("Can't get file size", Logger.Level.WARNING, Logger.App.MOD_DOWNLOADER);
                fileSize.Text = "Failed to get file size";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //cancelled = true;
            //dl.Destroy();

            //this.Close();
            //this.Hide();
        }

        private void frmModDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancelled = false;
        }
    }
}
