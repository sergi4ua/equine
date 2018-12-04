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
                    version = 1;
                    backgroundWorker1.RunWorkerAsync();
                break;

                case 2:
                    version = 2;
                    backgroundWorker1.RunWorkerAsync();
                break;

                default:

                break;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                download(version);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Downgrade failed!\nWindows reported the error:\n" + ex.Message + "\nUse 'Restore game data..' to return your game to it's original state.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void download(short dl)
        {
            if(dl == 0)
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("https://sergi4ua.pp.ua/equine/versions/Diablo-1.00.zip", Application.StartupPath + "\\dv100.zip");
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
                    wc.DownloadFile("https://sergi4ua.pp.ua/equine/versions/Diablo-1.07.zip", Application.StartupPath + "\\dv100.zip");
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
                    wc.DownloadFile("https://sergi4ua.pp.ua/equine/versions/Diablo-1.08.zip", Application.StartupPath + "\\dv100.zip");
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
