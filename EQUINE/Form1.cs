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
    along with this program.If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;
using Newtonsoft.Json;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace EQUINE
{
    public partial class Form1 : Form
    {
        RootObject ModInfos;
        const bool _DEBUG = false;
        List<string> installedMods = new List<string>();
        public static Config config { get; set; }
        List<CustomModInfo> customModInfos;
        // toolinfo
        List<ToolInfo> toolInfo;

        bool failOnNextToolInfoDL = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Preloader();
            Text = GlobalVariableContainer.AppName;
            if (!GlobalVariableContainer.DIABDATPresent)
                menuItem40.Enabled = false;

            initModList();
            initCustomModList();
            initToolList();
            checkGameBackup();
            Random r = new Random();
            label1.Text = GlobalVariableContainer.Messages[r.Next(GlobalVariableContainer.Messages.Length)];
            readConfig();

            if (config.autoUpdate)
                updateModsRoutine.RunWorkerAsync();
        }

        private void initToolList()
        {
            if (File.Exists(Application.StartupPath + "/EquineData/toolList.json"))
            {
                toolInfo = JsonConvert.DeserializeObject<List<ToolInfo>>(File.ReadAllText(Application.StartupPath + "/EquineData/toolList.json"));

                for (int i = 0; i < toolInfo.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(toolInfo[i].Name);
                    lvi.SubItems.Add(toolInfo[i].Description);
                    lvi.SubItems.Add(toolInfo[i].Website);
                    lvi.SubItems.Add(toolInfo[i].Version);
                    lvi.SubItems.Add(toolInfo[i].Author);
                    listView2.Items.Add(lvi);
                }
            }
            else
            {
                // tool list doesn't exist download it

                if (CheckForInternetConnection())
                {
                    if (!failOnNextToolInfoDL)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                        using (WebClient toolListDownloader = new WebClient())
                        {
                            toolListDownloader.DownloadFile("https://raw.githubusercontent.com/sergi4ua/equine/master/EquineData/toolList.json",
                                Application.StartupPath + "/EquineData/toolList.json");
                        }

                        initToolList();
                        failOnNextToolInfoDL = true;
                    }
                }
            }
        }

        private void initCustomModList()
        {
            if (File.Exists(Application.StartupPath + "/EquineData/customModList.json"))
            {
                customModInfos = JsonConvert.DeserializeObject<List<CustomModInfo>>(File.ReadAllText(Application.StartupPath + "/EquineData/customModList.json"));

                for (int i = 0; i < customModInfos.Count; i++)
                {
                    ListViewItem mi = new ListViewItem(customModInfos[i].Name);
                    mi.SubItems.Add("Diablo");
                    mi.SubItems.Add(customModInfos[i].Description);
                    mi.SubItems.Add(customModInfos[i].Author);
                    mi.SubItems.Add(customModInfos[i].Version);
                    mi.SubItems.Add(customModInfos[i].Website);
                    mi.SubItems.Add("Custom");
                    listView1.Items.Add(mi);
                }
            }
        }

        private void readConfig()
        {
            checkBox1.Checked = config.autoUpdate;
            checkBox2.Checked = config.checkForUpdates;
        }

        private void checkModUpdates()
        {
            string currentMod = "";

            // check for internet connection

            if (CheckForInternetConnection())
            {
                string modName = "", DL = "", EXE = "", DL2 = "";

                // if network connected check if mods needs updating
                for (int i = 0; i < ModInfos.ModInfo.Count; i++)
                {
                    // check if mod installed
                    if (installedMods.Contains(ModInfos.ModInfo[i].ModName))
                    {

                        if (i > ModInfos.ModInfo.Count)
                        {
                            MessageBox.Show("checkModUpdates() :: overflow\nplease report to Sergi4UA", "EQUINE");
                            break;
                        }

                        if(ModInfos.ModInfo[i] == null)
                        {
                            MessageBox.Show("checkModUpdates() :: null\nplease report to Sergi4UA", "EQUINE");
                        }

                        currentMod = ModInfos.ModInfo[i].ModName;

                        BeginInvoke((MethodInvoker)delegate () {
                            status.Text = "Checking for updates: " + currentMod;
                        });

                        // check for sha1 match
                        string modHash = "";
                        string jsonModHash = "";
                        sha1 hash = new sha1();

                        jsonModHash = ModInfos.ModInfo[i].md5;
                        if (ModInfos.ModInfo[i].ModName == "Tchernobog")
                        {
                            if (Directory.Exists(Application.StartupPath + "/Tchernobog"))
                            {
                                if (!File.Exists(Application.StartupPath + "/Tchernobog/Tchernobog" + ModInfos.ModInfo[i].ModVersion + ".exe"))
                                {
                                    BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        modName = ModInfos.ModInfo[i].ModName;
                                        DL = ModInfos.ModInfo[i].DL;
                                        EXE = ModInfos.ModInfo[i].Executable;
                                        DL2 = ModInfos.ModInfo[i].DL2;
                                    });
                                    break;
                                }
                                else
                                    if (File.Exists(Application.StartupPath + "\\" + ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable))
                                    modHash = hash.CheckFileHash(Application.StartupPath + "\\" + ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable);
                            }
                        }
                        else
                        {
                            if (File.Exists(Application.StartupPath + "\\" + ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable))
                                modHash = hash.CheckFileHash(Application.StartupPath + "\\" + ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable);
                        }

                        if (jsonModHash != "null")
                        {
                            if (modHash == jsonModHash)
                                continue;
                            else
                            {
                                // mod needs updating
                                modName = ModInfos.ModInfo[i].ModName;
                                DL = ModInfos.ModInfo[i].DL;
                                EXE = ModInfos.ModInfo[i].Executable;
                                DL2 = ModInfos.ModInfo[i].DL2;
                                break;
                            }
                        }
                    }
                }

                if (modName != "" && DL != "")
                {
                    BeginInvoke((MethodInvoker)delegate () { status.Text = "Fetching update information for: " + modName; });
                    //BeginInvoke((MethodInvoker)delegate ()
                    //{
                    frmNewUpdate u = new frmNewUpdate();
                    u.modName = modName;
                    u.DL = DL;
                    u.DL2 = DL2;
                    u.TopMost = true;
                    u.ShowDialog();
                    //});
                }
            }
        }
            

        private void checkGameBackup()
        {
            List<string> files = new List<string> { "Diablo.exe", "storm.dll", "battle.snp", "standard.snp", "diabloui.dll", "SMACKW32.DLL" };
            short filesInBackup = 0;

            for (int i = 0; i < files.Count; i++)
            {
                if (File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\" + files[i]))
                {
                    filesInBackup++;
                }
            }

            if (filesInBackup == files.Count)
                menuItem13.Enabled = true;
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

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool Elevate()
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath,
                Verb = "runas"
            };
            try
            {
                Process.Start(SelfProc);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void Preloader()
        {
            //MessageBox.Show("NOTE! This version uses mod folders instead of the previous version where all mod files are downloaded and extracted to Diablo root directory.\nIf you have used EQUINE before you will notice that all your mods are no longer available. Don't worry all your saves are not deleted.\nYou will need to redownload your mod and copy your save games to the mod folder.\nFor instance, for The Hell you will copy your save files to the The Hell folder (for TH the save filenames are hellsp_x.hsv where x is a number.)\n\nBackup and Restore are currently Vanilla Only but will be fixed soon :)\nPlease report any bugs you found to the GitHub issue tracker (available from EQUINE > Report Bug).", "Sergi4UA", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (CheckForInternetConnection() == true && _DEBUG == false)
            {
                try
                {
                    WebClient modInfo = new WebClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    modInfo.DownloadFile("https://raw.githubusercontent.com/sergi4ua/equine/master/EquineData/modlist.json", Application.StartupPath + "\\EquineData\\modlist.json");
                    modInfo.Dispose();
                }
                catch
                {
                    MessageBox.Show("Error updating ModInfo :(");
                }

                try
                {
                    WebClient toolInfo = new WebClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    toolInfo.DownloadFile("https://raw.githubusercontent.com/sergi4ua/equine/master/EquineData/toolList.json", Application.StartupPath + "\\EquineData\\toolList.json");
                    toolInfo.Dispose();
                }
                catch
                {
                    MessageBox.Show("Error updating ToolInfo :(");
                }
            }

            if (!Directory.Exists(Application.StartupPath + "\\EquineData\\ipx"))
                button6.Enabled = false;

            List<string> ipxWrapperFiles = new List<string> { "directplay-win32.reg", "directplay-win64.reg", "dpwsockx.dll", "ipxconfig.exe", "ipxwrapper.dll", "license.txt", "mswsock.dll", "readme.txt", "wsock32.dll" };
            int checkedfiles = 0;

            for (int i = 0; i < ipxWrapperFiles.Count; i++)
            {
                if (File.Exists(Application.StartupPath + "\\" + ipxWrapperFiles[i]))
                {
                    checkedfiles++;
                }
            }

            if (checkedfiles == ipxWrapperFiles.Count)
            {
                panel1.Hide();
                panel2.Show();
            }

            if (File.Exists(Application.StartupPath + "\\ipxwrapper.log"))
                textBox1.Text = File.ReadAllText(Application.StartupPath + "\\ipxwrapper.log");

            if (File.Exists(Application.StartupPath + "\\hellfire.exe"))
            {

            }
        }

        private void initModList()
        {
            // add to list
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            ModInfos = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(Application.StartupPath + "\\EquineData\\modlist.json"), jsonSerializerSettings);


            for (int i = 0; i < ModInfos.ModInfo.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ModInfos.ModInfo[i].ModName;
                lvi.SubItems.Add(ModInfos.ModInfo[i].Game);
                lvi.SubItems.Add(ModInfos.ModInfo[i].Description);
                lvi.SubItems.Add(ModInfos.ModInfo[i].Author);
                lvi.SubItems.Add(ModInfos.ModInfo[i].ModVersion);
                lvi.SubItems.Add(ModInfos.ModInfo[i].WebSite);
                if (!System.IO.File.Exists(ModInfos.ModInfo[i].ModName + "/" + ModInfos.ModInfo[i].Executable))
                {
                    if (Directory.Exists(Application.StartupPath + "/Tchernobog") && lvi.Text == "Tchernobog")
                    {
                        lvi.SubItems.Add("Yes");
                        installedMods.Add(ModInfos.ModInfo[i].ModName);
                    }
                    else
                        lvi.SubItems.Add("No");
                }
                else
                {
                    lvi.SubItems.Add("Yes");
                    installedMods.Add(ModInfos.ModInfo[i].ModName);
                }
                listView1.Items.Add(lvi);
            }
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {
            frmPing ping = new frmPing();
            ping.ShowDialog();
        }

        private void menuItem16_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            label1.Text = GlobalVariableContainer.Messages[r.Next(GlobalVariableContainer.Messages.Length)];
            r = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                installPlayMod();
            else
                installPlayCustomMod();
        }

        private void installPlayCustomMod()
        {
            int index = listView1.SelectedIndices[0] + 1;
            int it = (index - listView1.Items.Count) + customModInfos.Count - 1;

            if (File.Exists(Application.StartupPath + "/" + customModInfos[it].Name + "/" + customModInfos[it].Executable))
            {
                try
                {
                    Process.Start(Application.StartupPath + "/" + customModInfos[it].Name + "/" + customModInfos[it].Executable);
                }
                catch
                {
                    MessageBox.Show("Unable to launch custom mod.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Mod exectuable file doesn't exist.", "EQUINE Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void installPlayMod()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    try
                    {
                        Directory.SetCurrentDirectory(Application.StartupPath);
                        System.Diagnostics.Process.Start("Diablo.exe");
                    }
                    catch
                    {
                        if (listView1.SelectedItems.Count == 0)
                        {
                            button1.Text = "Install";
                            button1.Enabled = false;
                        }
                    }
                    return;
                }
                try
                {

                    if (System.IO.File.Exists(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable))
                    {
                        Directory.SetCurrentDirectory(Application.StartupPath + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName);
                        System.Diagnostics.Process.Start(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable);
                        Directory.SetCurrentDirectory(Application.StartupPath);
                    }
                    else
                    {
                        if (CheckForInternetConnection() == true)
                        {
                            if (File.Exists(Application.StartupPath + "\\DIABDAT.MPQ"))
                            {
                                Directory.SetCurrentDirectory(Application.StartupPath);
                                frmModDownloader modDL = new frmModDownloader();
                                modDL.beforeDownloadMsg = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].BeforeInstallMessage;
                                modDL.afterDownloadMsg = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].AfterInstallMessage;
                                modDL.dlLink0 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DL;
                                modDL.dlLink1 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DL2;
                                modDL.modName = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName;
                                modDL.startExe0 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].RunExeAfterInstall;
                                modDL.startExe1 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].RunExeAfterInstall2;
                                if (modDL.modName == "Polish Localization")
                                    modDL.Polska = true;
                                modDL.ShowDialog();

                                Uri modDLuri = new Uri(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DL);
                                string mod_fileName = Application.StartupPath + "/" + Path.GetFileName(modDLuri.LocalPath);

                                if (File.Exists(mod_fileName))
                                {
                                    File.Delete(mod_fileName);
                                }

                                modDL.Dispose();
                                listView1.SelectedItems.Clear();
                            }
                            else
                            {
                                if (ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DiabdatRequired == "true")
                                {
                                    MessageBox.Show("DIABDAT.MPQ is required for this mod.\nYou can use 'Copy DIABDAT.MPQ from Diablo CD' to copy the requested file from your Diablo CD.\nIf you have the file somewhere on your HDD, copy it to the root of your Diablo installation directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unable to communicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to launch mod. " + ex.Message + "\nCurWorkDir: " + Directory.GetCurrentDirectory(), "EQUINE", MessageBoxButtons.OK);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    button1.Text = "Play";
                    button1.Enabled = true;
                }
                else
                {
                    if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                    {
                        forceUpdateToolStripMenuItem.Enabled = true;

                        if (!System.IO.File.Exists(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable))
                        {
                            button1.Text = "Install";
                            button1.Enabled = true;
                            button2.Enabled = false;
                            launchToolStripMenuItem.Text = "Install";
                            uninstallToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            button1.Text = "Play";
                            button1.Enabled = true;
                            button2.Enabled = true;
                            launchToolStripMenuItem.Text = "Launch";
                            uninstallToolStripMenuItem.Enabled = true;
                        }
                    }
                    else
                    {
                        // custom mod list
                        button1.Text = "Play";
                        button1.Enabled = true;
                        button2.Enabled = false;
                        uninstallToolStripMenuItem.Enabled = false;
                        forceUpdateToolStripMenuItem.Enabled = false;
                    }
                }
            }
            else
            {
                button1.Text = "Install";
                button1.Enabled = false;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void menuItem18_Click(object sender, EventArgs e)
        {

        }

        private void menuItem19_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
            BackupSave saves = new BackupSave();
            saves.installedMods = this.installedMods;
            saves.ShowDialog();
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            createShortcutToolStripMenuItem.Enabled = false;

            if (listView1.SelectedItems.Count == 0)
            {
                e.Cancel = true;
            }
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    uninstallToolStripMenuItem.Enabled = false;
                }

                if (listView1.SelectedItems[0].SubItems[6].Text == "Yes" || listView1.SelectedItems[0].SubItems[6].Text == "Custom")
                {
                    createShortcutToolStripMenuItem.Enabled = true;
                }
            }


        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                frmProperties props = new frmProperties();
                props.ModName = listView1.SelectedItems[0].Text;
                props.ModAuthor = listView1.SelectedItems[0].SubItems[3].Text;
                props.ModWebsite = listView1.SelectedItems[0].SubItems[5].Text;
                props.ModVersion = listView1.SelectedItems[0].SubItems[4].Text;
                props.Description = listView1.SelectedItems[0].SubItems[2].Text;
                props.ShowDialog();
            }
        }

        private void menuItem20_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Please insert your Diablo CD in your disc drive and press OK to continue.", "Copy DIABDAT.MPQ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                frmCopyDIABDAT copyDIABDAT = new frmCopyDIABDAT();
                copyDIABDAT.ShowDialog();
            }
        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            frmDDrawWrapper dwrap = new frmDDrawWrapper();
            dwrap.installedMods = this.installedMods;
            dwrap.ShowDialog();
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {
            frmDowngrader downgrader = new frmDowngrader();
            downgrader.ShowDialog();
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    return;
                }
                else
                {
                    try
                    {
                        if (MessageBox.Show("You about to uninstall " + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "!\nAfter Uninstallation is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nContinue?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            frmUninstall frmUninstall = new frmUninstall();
                            frmUninstall.modExe = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable;
                            frmUninstall.modName = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName;
                            frmUninstall.ShowDialog();
                        }
                    }
                    catch { }
                }
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    return;
                }
                else
                {
                    if (MessageBox.Show("You are about to uninstall " + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "!\n\nAfter Uninstallation is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nContinue?", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        frmUninstall frmUninstall = new frmUninstall();
                        frmUninstall.modExe = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable;
                        frmUninstall.modName = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName;
                        frmUninstall.ShowDialog();
                    }
                }
            }
            else
            {
                uninstallToolStripMenuItem.Enabled = false;
            }
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text == "Vanilla Game")
                {
                    try
                    {
                        System.Diagnostics.Process.Start("Diablo.exe");
                    }
                    catch
                    {
                        MessageBox.Show("Unable to start game.");
                    }
                }
                else
                {
                    if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                        installPlayMod();
                    else
                        installPlayCustomMod();
                }
            }
        }

        private void menuItem13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: restoring game data may break compatibilty with some mods. Continue?", "Backup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                List<string> files = new List<string> { "Diablo.exe", "storm.dll", "battle.snp", "standard.snp", "diabloui.dll", "SMACKW32.DLL" };
                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        File.Copy(Application.StartupPath + "\\EquineData\\GameBackup\\" + files[i], Application.StartupPath + "\\" + Path.GetFileName(files[i]), true);
                    }
                    MessageBox.Show("Game data restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Operation failed.\nWindows reported the error:\n" + ex.Message + "\nPlease use 'Force Update' to try to bring your game to it's original state.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://goo.gl/forms/wYbW4DUqoB7IHCsF2");
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            frmForceUpdate fupd = new frmForceUpdate();
            fupd._109b = false;
            fupd.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator())
            {
                Elevate();
                Application.Exit();
            }
            else
            {
                button6.Text = "Working...";
                button6.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void menuItem14_Click(object sender, EventArgs e)
        {
            frmForceUpdate fupd = new frmForceUpdate();
            fupd._109b = true;
            fupd.ShowDialog();
        }

        bool ipxError = false;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<string> ipxWrapperFiles = new List<string> { "directplay-win32.reg", "directplay-win64.reg", "dpwsockx.dll", "ipxconfig.exe", "ipxwrapper.dll", "license.txt", "mswsock.dll", "readme.txt", "wsock32.dll" };

                for (int i = 0; i < ipxWrapperFiles.Count; i++)
                {
                    File.Copy(Application.StartupPath + "\\EquineData\\ipx\\" + ipxWrapperFiles[i], Application.StartupPath + "\\" + ipxWrapperFiles[i], true);
                }

                if (Environment.Is64BitOperatingSystem == true)
                {
                    Process regeditProcess = Process.Start("regedit.exe", "/s " + Application.StartupPath + "\\directplay-win64.reg");
                    regeditProcess.WaitForExit();
                }
                else
                {
                    Process regeditProcess = Process.Start("regedit.exe", "/s " + Application.StartupPath + "\\directplay-win32.reg");
                    regeditProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Operation failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ipxError = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ipxError == false)
            {
                button6.Text = "Done!";
                MessageBox.Show("IPX Wrapper has been successfully installed!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panel1.Hide();
                panel2.Show();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "\\ipxconfig.exe"))
                Process.Start(Application.StartupPath + "\\ipxconfig.exe");
        }

        private void menuItem15_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/sergi4ua/equine");
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/yyFc3Db");
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/ZdbRTb9");
        }

        private void menuItem16_Click_1(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/UxKrvQu");
        }

        private void menuItem17_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/UeMGY4D");
        }

        private void menuItem21_Click(object sender, EventArgs e)
        {
            Process.Start("https://d1legit.com/");
        }

        private void menuItem22_Click(object sender, EventArgs e)
        {
            Process.Start("https://freshmeat-blog.de.tl/");
        }

        private void menuItem23_Click(object sender, EventArgs e)
        {
            Process.Start("http://diablo1.96.lt/");
        }

        private void menuItem25_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/sergi4ua/equine/issues");
        }

        // create shortcut

        private void createShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text != "Vanilla Game")
                {
                    if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                    {
                        // Process.Start(Application.StartupPath + "\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName);
                        object shDesktop = (object)"Desktop";
                        WshShell shell = new WshShell();
                        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + ".lnk";
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                        shortcut.TargetPath = Application.StartupPath + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable;
                        shortcut.WorkingDirectory = Application.StartupPath + "\\" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "\\";
                        shortcut.Save();
                    }
                    else
                    {
                        // Process.Start(Application.StartupPath + "/" + customModInfos[it].Name);
                        int index = listView1.SelectedIndices[0] + 1;
                        int it = (index - listView1.Items.Count) + customModInfos.Count - 1;

                        object shDesktop = (object)"Desktop";
                        WshShell shell = new WshShell();
                        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + "\\" + customModInfos[it].Name + ".lnk";
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                        shortcut.TargetPath = Application.StartupPath + "\\" + customModInfos[it].Name + "\\" + customModInfos[it].Executable;
                        shortcut.WorkingDirectory = shortcut.WorkingDirectory = Application.StartupPath + "\\";
                        shortcut.Save();
                    }

                    MessageBox.Show("Shortcut created.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(Application.StartupPath + "\\EquineData\\config.json");
                File.WriteAllText(Application.StartupPath + "\\EquineData\\config.json", JsonConvert.SerializeObject(config));
                MessageBox.Show("Settings saved!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save options... for some reason\nReport this to Sergi:\n" + ex.ToString());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            config.autoUpdate = checkBox1.Checked;
        }

        private void openModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int it = 0;

                if (customModInfos != null)
                {
                    int index = listView1.SelectedIndices[0] + 1;
                    it = (index - listView1.Items.Count) + customModInfos.Count - 1;
                }

                if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                {
                    if (listView1.SelectedItems[0].Text == "Vanilla Game")
                    {
                        Process.Start(Application.StartupPath);
                        return;
                    }
                    Process.Start(Application.StartupPath + "/" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName);
                }
                else
                    Process.Start(Application.StartupPath + "/" + customModInfos[it].Name);
            }
            else
                openModFolderToolStripMenuItem.Enabled = false;
        }

        private void forceUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                try
                {
                    if (listView1.SelectedItems[0].Text == "Vanilla Game")
                    {
                        forceUpdateToolStripMenuItem.Enabled = false;
                        return;
                    }

                    if (MessageBox.Show("OK to force update?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        if (File.Exists(Application.StartupPath + "\\DIABDAT.MPQ"))
                        {
                            Directory.SetCurrentDirectory(Application.StartupPath);
                            frmModDownloader modDL = new frmModDownloader();
                            modDL.beforeDownloadMsg = "null";
                            modDL.afterDownloadMsg = "null";
                            modDL.dlLink0 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DL;
                            modDL.dlLink1 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DL2;
                            modDL.modName = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName;
                            modDL.startExe0 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].RunExeAfterInstall;
                            modDL.startExe1 = ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].RunExeAfterInstall2;
                            modDL.ShowDialog();
                        }
                        else
                        {
                            if (ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].DiabdatRequired == "true")
                            {
                                MessageBox.Show("DIABDAT.MPQ is required for this mod.\nYou can use 'Copy DIABDAT.MPQ from Diablo CD' to copy the requested file from your Diablo CD.\nIf you have the file somewhere on your HDD, copy it to the root of your Diablo installation directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }
                    }
                }
                catch { }
            }
            else
                openModFolderToolStripMenuItem.Enabled = false;
        }

        private void menuItem26_Click(object sender, EventArgs e)
        {
            AddAMod addAMod = new AddAMod();
            addAMod.ShowDialog();
        }

        private void menuItem27_Click(object sender, EventArgs e)
        {
            frmRemoveAMod gdbye = new frmRemoveAMod();
            gdbye.ShowDialog();
        }

        private void menuItem28_Click(object sender, EventArgs e) => Process.Start("https://discord.gg/ymqHuWE");

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                installPlayMod();
            else
                installPlayCustomMod();
        }

        private void MenuItem32_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ModsCounter = ModInfos.ModInfo.Count;
            about.ToolsCounter = listView2.Items.Count;
            about.ShowDialog();
        }

        private void MenuItem30_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Click 'Yes' to open it online using your default web browser. Click 'No' to display the PDF using your default PDF viewer (PDF viewer must be installed beforehand)", "EQUINE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

            switch (result)
            {
                case DialogResult.Yes:
                    Process.Start("http://www.lurkerlounge.com/diablo/jarulf/jarulf162.pdf");
                    break;

                case DialogResult.No:
                    try
                    {
                        Process.Start(Application.StartupPath + "/EquineData/JarulfGuide.pdf");
                    }
                    catch (Exception) { MessageBox.Show("Unable to launch your default PDF viewer. Make sure that PDF viewer is installed and try again.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    break;
                default:

                    break;
            }


        }

        private void MenuItem18_Click_1(object sender, EventArgs e)
        {
            Process.Start("http://www.lurkerlounge.com/forums/portal.php");
        }

        private void MenuItem33_Click(object sender, EventArgs e)
        {
            Process.Start("https://d2mods.info/home.php");
        }

        private void MenuItem34_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.reddit.com/r/diablo");
        }

        private void MenuItem35_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.reddit.com/r/diablo1");
        }

        private void MenuItem36_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.github.com/sergi4ua/equine");
        }

        private void MenuItem37_Click(object sender, EventArgs e)
        {
            Process.Start("https://sergi4ua.com/equine");
        }

        private void MenuItem38_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/gMJN8Dm");
        }

        private void MenuItem39_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Width = 816;
            this.Height = 528;
        }

        private void MenuItem41_Click(object sender, EventArgs e)
        {
#if (!DEBUG)
            try
            {
                eqmpqedit.frmMain mpqEditForm = new eqmpqedit.frmMain();
                mpqEditForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured in eqmpqedit.dll\n" + ex.Message, "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
#else
            eqmpqedit.frmMain mpqEditForm = new eqmpqedit.frmMain();
            mpqEditForm.ShowDialog();
#endif
        }

        private void MenuItem44_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://goo.gl/forms/wYbW4DUqoB7IHCsF2");
        }

        private void MenuItem42_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Extract DIABDAT.MPQ?", "EQUINE MPQEdit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    frmExtractMPQ extractor = new frmExtractMPQ();
                    extractor.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occured in eqmpqedit.dll\n" + ex.Message,
                        "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            config.checkForUpdates = checkBox2.Checked;
        }

        private void menuItem45_Click(object sender, EventArgs e)
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
                    if (MessageBox.Show("A new update is out. Run EQUINE Update Utility?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
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
                else
                {
                    MessageBox.Show("You have the latest version.",
                        "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Failed to check for updates.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 0)
            {
                installLaunchToolBtn.Enabled = false;
                uninstallToolBtn.Enabled = false;
                visitWebsiteBtn.Enabled = false;
            }
            else
            {
                installLaunchToolBtn.Enabled = true;
                uninstallToolBtn.Enabled = true;
                visitWebsiteBtn.Enabled = true;
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // f my mozg

            if (listView2.SelectedItems.Count == 0) // don't crash ok thx
            {
                installLaunchToolBtn.Enabled = false;
                visitWebsiteBtn.Enabled = false;
                uninstallToolBtn.Enabled = false;
                return;
            }

            if (File.Exists(Application.StartupPath + "/EquineData/ModdingTools/" + listView2.SelectedItems[0].Text + "/" +
                toolInfo[listView2.SelectedIndices[0]].Executable))
            {
                installLaunchToolBtn.Text = "Launch";
                installLaunchToolBtn.Enabled = true;
                uninstallToolBtn.Enabled = true;
                visitWebsiteBtn.Enabled = true;
            }
            else
            {
                installLaunchToolBtn.Text = "Install";
                installLaunchToolBtn.Enabled = true;
                uninstallToolBtn.Enabled = false;
                visitWebsiteBtn.Enabled = false;
            }
        }

        private void installLaunchToolBtn_Click(object sender, EventArgs e)
        {
            ToolInfo localToolInfo = toolInfo[listView2.SelectedIndices[0]];

            if (installLaunchToolBtn.Text == "Install")
            {
                frmModDownloader dl = new frmModDownloader();
                dl.toolDLMode = true;
                dl.modName = localToolInfo.Name;
                dl.dlLink0 = localToolInfo.DL;
                dl.startExe0 = localToolInfo.Executable;
                dl.beforeDownloadMsg = "null";
                dl.afterDownloadMsg = "null";
                dl.ShowDialog();
            }
            else
            {
                try
                {
                    Process.Start(Application.StartupPath + "/EquineData/ModdingTools/" +
                    localToolInfo.Name + "/" + localToolInfo.Executable);
                }
                catch
                {
                    MessageBox.Show("Unable to launch.\nFile " + Application.StartupPath + "/EquineData/ModdingTools/" +
                    localToolInfo.Name + "/" + localToolInfo.Executable + " is missing or corrupt.\nForce updating the tool may fix the problem", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void uninstallToolBtn_Click(object sender, EventArgs e)
        {
            ToolInfo localToolInfo = toolInfo[listView2.SelectedIndices[0]];
            string toolPath = Application.StartupPath + "/EquineData/ModdingTools/" + localToolInfo.Name;

            try
            {
                if (MessageBox.Show("Uninstall: " + localToolInfo.Name + "?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Directory.Delete(toolPath, true);
                    MessageBox.Show("Tool " + localToolInfo.Name + " has been successfully uninstalled from your computer.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    uninstallToolBtn.Enabled = false;
                    visitWebsiteBtn.Enabled = false;
                    installLaunchToolBtn.Enabled = false;
                    installLaunchToolBtn.Text = "Install";
                    listView1.SelectedItems.Clear();
                }
            }
            catch
            {
                MessageBox.Show("Failed to uninstall tool.", "EQUINE Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                
        }

        private void menuItem47_Click(object sender, EventArgs e)
        {
            Process.Start("https://reddit.com/r/devilution");
        }

        private void menuItem43_Click(object sender, EventArgs e)
        {
            DialogResult mpq = MessageBox.Show("Make backup of your current DIABDAT.MPQ? (recommended)\n\nThe original file will be overwritten.\n\nWARNING: The rebuild process is not perfect, any added file to the DIABDAT.listfile.txt will be included in the MPQ file. If the list doesn't include the file, it will not be included in the MPQ, this will also apply if the file doesn't exist. Improper editing of this file may corrupt your game. Freeablo will not recognize the MPQ created with EQUINE.\nUse EQUINE MPQEdit to edit Patch_rt.mpq if you just need to use custom graphics/music.", "EQUINE MPQEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (mpq == DialogResult.Yes)
            {
                frmRebuildMpq rebuild = new frmRebuildMpq();
                rebuild.CopyOriginal = true;
                rebuild.ShowDialog();
            }
            else if(mpq == DialogResult.No)
            {
                frmRebuildMpq rebuild = new frmRebuildMpq();
                rebuild.ShowDialog();
            }
        }

        private void menuItem40_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Application.StartupPath + "/EquineData/DIABLODAT"))
                menuItem43.Enabled = false;
            else
                menuItem43.Enabled = true;
        }

        private void menuItem48_Click(object sender, EventArgs e)
        {
            Process.Start("https://patreon.com/sergi4ua");
        }

        private void visitWebsiteBtn_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ToolInfo localToolInfo = toolInfo[listView2.SelectedIndices[0]];

                if (localToolInfo.Website != "N/A")
                    Process.Start(localToolInfo.Website);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (Directory.Exists(Application.StartupPath + "/EquineData/ModdingTools"))
                Directory.CreateDirectory(Application.StartupPath + "/EquineData/ModdingTools");

            Process.Start(Application.StartupPath + "/EquineData/ModdingTools");
        }

        private void menuItem49_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("EQUINE will now attempt to update your game to 1.09b. Battle.net gateways will be set to default.\n\n" +
                "Because Diablo 1 is an old game, you will need to port forward 6112-6119. Please refer to your router instruction manual or contact your ISP technical support for assistance.\nYou can also attempt using UPnP to automatically port forward the required ports (you can use open-source UPnP Port Mapper app to do this: https://github.com/kaklakariada/portmapper)\n" +
                "WARNING: PORT FORWARD WILL NOT WORK IF YOUR ISP DOES NOT SUPPORT IPV4. MAKE SURE YOUR ISP SUPPORT IPV4 BEFORE ATTEMPTING TO PORT-FORWARD ! (Diablo 1 does not support IPv6)\n\n\nContinue?", "Note", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                frmGOGBNETFixer gog = new frmGOGBNETFixer();
                gog.ShowDialog();
            }
        }

        private void menuItem50_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("OK to fix Battle.net Gateways?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
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
                }

                MessageBox.Show("Operation completed successfully!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void updateModsRoutine_DoWork(object sender, DoWorkEventArgs e)
        {
            checkModUpdates();
            BeginInvoke((MethodInvoker)delegate () { status.Text = "Done."; });
        }
    }
}