using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EQUINE
{
    public partial class frmSplash : Form
    {
        static Config config;
        bool noInit = false;

        public frmSplash()
        {
            InitializeComponent();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void FrmSplash_Load(object sender, EventArgs e)
        {
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            label4.Parent = pictureBox1;

            this.Cursor = Cursors.WaitCursor;
            backgroundWorker1.RunWorkerAsync();
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void readJsonConfig()
        {

            try
            {
                if (File.Exists(Application.StartupPath + "/EquineData/config.json"))
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Application.StartupPath + "/EquineData/config.json"));
                    Form1.config = config;
                }
            }
            catch
            {
                MessageBox.Show("Unable to read config file.");
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate () { label3.Text = "Checking for active internet connection"; });
            if (CheckForInternetConnection() == true)
            {
                if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile("https://sergi4ua.pp.ua/equine/EQUINE_hash.sha", Application.StartupPath + "\\EquineData\\EQUINE_hash.sha");
                }
            }

            BeginInvoke((MethodInvoker)delegate () { label3.Text = "Reading config file"; });
            if (!File.Exists(Application.StartupPath + "\\EquineData\\config.json"))
            {
                if (noInit == false)
                {
                    Config defaultConfigFile = new Config();
                    defaultConfigFile.autoUpdate = true;
                    defaultConfigFile.checkForUpdates = true;
                    File.WriteAllText(Application.StartupPath + "\\EquineData\\config.json", JsonConvert.SerializeObject(defaultConfigFile));
                }
            }

            readJsonConfig();

            //#if RELEASE

            if (config.checkForUpdates && !GlobalVariableContainer.skipUpdates)
            {
                BeginInvoke((MethodInvoker)delegate () { label3.Text = "Checking for updates"; });
                if (File.Exists(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha"))
                {
                    if (noInit == false)
                    {
                        try
                        {
                            File.Copy(Application.ExecutablePath, Application.StartupPath + "\\EQUINE.hash", true);

                            sha1 hash = new sha1();
                            string fromfilehash = "";
                            string apphash = "";

                            fromfilehash = File.ReadAllText(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha");
                            apphash = hash.CheckFileHash(Application.StartupPath + "\\EQUINE.hash");

                            if (fromfilehash != apphash)
                            {
                                BeginInvoke((MethodInvoker)delegate () { label3.Text = "New update is out. Launching EQUINE Update Utility"; });

                                var SelfProc = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    WorkingDirectory = Environment.CurrentDirectory,
                                    FileName = Application.StartupPath + "\\EquineData\\EQUINEUpdater.exe",
                                    Arguments = "-update",
                                };
                                File.Delete(Application.StartupPath + "\\EQUINE.hash");
                                Process.Start(SelfProc);
                                Environment.Exit(0);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to check for updates!\n" + ex.Message);
                        }
                    }
                }
            }
//#endif

            if (!File.Exists(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha"))
            {
                if (noInit == false)
                {
                    Form1 mainForm = new Form1();
                    BeginInvoke((MethodInvoker)delegate () { mainForm.Show(); });
                    BeginInvoke((MethodInvoker)delegate () { this.Hide(); });
                    return;
                }
            }

            if (Directory.Exists(Application.StartupPath + "/Tchernobog (64 BIT ONLY)"))
            {
                Directory.Move(Application.StartupPath + "/Tchernobog (64 BIT ONLY)", Application.StartupPath + "/Tchernobog");
            }

            BeginInvoke((MethodInvoker)delegate () { label3.Text = "Launching"; });
            Form1 mainForm2 = new Form1();
            BeginInvoke((MethodInvoker)delegate () { mainForm2.Show(); }); 
            BeginInvoke((MethodInvoker)delegate () { this.Hide(); });
        }

        private void Label3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Label3_TextAlignChanged(object sender, EventArgs e)
        {
            
        }
    }
}
