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
                MessageBox.Show("Initalization completed successfully!", "EQUINE initalized", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
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
    }
}
