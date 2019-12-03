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

namespace EQUINEUpdater
{
    public partial class frmUpdateProgress : Form
    {
        public frmUpdateProgress()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                WebClient wc = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                wc.DownloadFile("https://sergi4ua.pp.ua/equine/EQUINEUpdate.zip", Application.StartupPath + "\\..\\equineupdate.zip");

                ZipStorer zip = ZipStorer.Open(Application.StartupPath + "\\..\\equineupdate.zip", FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Application.StartupPath + "\\..\\" + entry.FilenameInZip);
                }
                zip.Close();
                File.Delete(Application.StartupPath + "\\..\\equineupdate.zip");
                System.Threading.Thread.Sleep(2000);
                MessageBox.Show("EQUINE has been successfully updated!\nEQUINE will now restart.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var SelfProc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Application.StartupPath + "\\..\\",
                    FileName = Application.StartupPath + "\\..\\EQUINE.exe",
                };
                Process.Start(SelfProc);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed!\nWindows reported the error: " + ex.Message + "\nEQUINE will now quit. Please contact EQUINE developers to report this issue.", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
        }

        private void frmUpdateProgress_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
