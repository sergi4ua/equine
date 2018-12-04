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
                    wc.DownloadFile("http://sergi4ua.pp.ua/equine/versions/d1forceupdate.zip", Application.StartupPath + "\\d1forceupdate.zip");
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
    }
}
