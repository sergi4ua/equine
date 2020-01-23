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
using System.IO;
using System.IO.Compression;

namespace EQUINE
{
    public partial class frmGOGBNETFixer : Form
    {
        bool error = false;

        public frmGOGBNETFixer()
        {
            InitializeComponent();
        }

        private void frmGOGBNETFixer_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    wc.DownloadFile("http://sergi4ua.com/equine/versions/Diablo-1.09b.zip", Application.StartupPath + "\\d1forceupdate.zip");
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
            }
            catch
            {
                MessageBox.Show("Update failed!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                error = true;
            }

            try
            {
                string[] battleNetGateway = new string[29];
                battleNetGateway[0] = "2000";
                battleNetGateway[1] = "08";
                battleNetGateway[2] = "connect-forever.classic.blizzard.com";
                battleNetGateway[3] = "-1";
                battleNetGateway[4] = "Global";
                battleNetGateway[5] = "94.76.252.154";
                battleNetGateway[6] = "-1";
                battleNetGateway[7] = "NetCraft";
                battleNetGateway[8] = "37.187.100.90";
                battleNetGateway[9] = "-1";
                battleNetGateway[10] = "EuroBattle";
                battleNetGateway[11] = "uswest.battle.net";
                battleNetGateway[12] = "8";
                battleNetGateway[13] = "US West";
                battleNetGateway[14] = "useast.battle.net";
                battleNetGateway[15] = "6";
                battleNetGateway[16] = "U.S. East";
                battleNetGateway[17] = "asia.battle.net";
                battleNetGateway[18] = "-9";
                battleNetGateway[19] = "Asia";
                battleNetGateway[20] = "europe.battle.net";
                battleNetGateway[21] = "-1";
                battleNetGateway[22] = "Europe";
                battleNetGateway[23] = "play.slashdiablo.net";
                battleNetGateway[24] = "0";
                battleNetGateway[25] = "Slash Diablo";
                battleNetGateway[26] = "rubattle.net";
                battleNetGateway[27] = "-1";
                battleNetGateway[28] = "RuBattle.net";

                Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\\Software\\Battle.net\\Configuration", "Battle.net Gateways",
                     battleNetGateway, Microsoft.Win32.RegistryValueKind.MultiString);
            }
            catch
            {
                MessageBox.Show("Failed to set value in the Windows Registry", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                error = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!error)
                MessageBox.Show("Operation completed successfully!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}
