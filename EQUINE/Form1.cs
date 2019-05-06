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
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;
using Newtonsoft.Json;

namespace EQUINE
{
    public partial class Form1 : Form
    {
        RootObject ModInfos;
        private const bool _DEBUG = false;
        private List<string> installedMods = new List<string>();
        public static Config config { get;  set; }
        List<CustomModInfo> customModInfos;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Preloader();
            Text = GlobalVariableContainer.AppName;
            if (!GlobalVariableContainer.DIABDATPresent)
                menuItem2.Enabled = false;

            initModList();
            initCustomModList();
            checkGameBackup();
            Random r = new Random();
            label1.Text = GlobalVariableContainer.Messages[r.Next(GlobalVariableContainer.Messages.Length)];
            readConfig();
            if(config.autoUpdate)
                checkModUpdates();
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
        }

        private void checkModUpdates()
        {
            // check for internet connection

            if(CheckForInternetConnection())
            {
                string modName = "", DL = "", EXE = "", DL2 = "";

                // if network connected check if mods needs updating
                for (int i = 0; i < ModInfos.ModInfo.Count; i++)
                {  
                    // check if mod installed
                    if(installedMods.Contains(ModInfos.ModInfo[i].ModName))
                    {
                        // check for sha1 match
                        string modHash = "";
                        string jsonModHash = "";
                        sha1 hash = new sha1();

                        jsonModHash = ModInfos.ModInfo[i].md5;
                        if (ModInfos.ModInfo[i].ModName == "Tchernobog")
                        {
                            if(Directory.Exists(Application.StartupPath + "/Tchernobog"))
                            {
                                if(!File.Exists(Application.StartupPath + "/Tchernobog/Tchernobog"+ModInfos.ModInfo[i].ModVersion+".exe"))
                                {
                                    modName = ModInfos.ModInfo[i].ModName;
                                    DL = ModInfos.ModInfo[i].DL;
                                    EXE = ModInfos.ModInfo[i].Executable;
                                    DL2 = ModInfos.ModInfo[i].DL2;
                                    break;
                                }
                                else
                                    if(File.Exists(Application.StartupPath + "\\" + ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable))
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
                    frmNewUpdate u = new frmNewUpdate();
                    u.modName = modName;
                    u.DL = DL;
                    u.DL2 = DL2;
                    u.ShowDialog();
                }
            }
        }

        private void checkGameBackup()
        {
            List<string> files = new List<string> { "Diablo.exe", "storm.dll", "battle.snp", "standard.snp", "diabloui.dll", "SMACKW32.DLL" };
            short filesInBackup = 0;

            for (int i = 0; i < files.Count; i++)
            {
                if(File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\" + files[i]))
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
                }
                catch
                {
                    MessageBox.Show("Error updating ModInfo :(");
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

            if(checkedfiles == ipxWrapperFiles.Count)
            {
                panel1.Hide();
                panel2.Show();
            }

            if (File.Exists(Application.StartupPath + "\\ipxwrapper.log"))
                textBox1.Text = File.ReadAllText(Application.StartupPath + "\\ipxwrapper.log");

            if(File.Exists(Application.StartupPath + "\\hellfire.exe"))
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
            MessageBox.Show("No clicky!");
            label1.Hide();
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
            int it = (index - listView1.Items.Count  ) + customModInfos.Count - 1;

            if (File.Exists(Application.StartupPath + "/" + customModInfos[it].Name + "/" + customModInfos[it].Executable)) {
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
                        else
                        {
                            MessageBox.Show("Unable to communicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                }
                catch(Exception ex)
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
            MessageBox.Show("EQUINE © 2019 Sergi4UA.\nThis software is in no way associated with or endorsed by Blizzard Entertainment®.\n\nVersion 0.8\nhttps://sergi4ua.pp.ua/equine\nFor any questions please contact me at: https://sergi4ua.pp.ua/contact.html or visit the GitHub: http://github.com/sergi4ua/equine \n\nBeta-testers:\nOgodei\nRadTang\nfearedbliss\nDavias\nQndel \n\nHave an awesome day! :)", "About EQUINE...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItem19_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("backup/restore feature is broken for mod folders", "Note");
            BackupSave saves = new BackupSave();
            saves.ShowDialog();
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
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
            if(MessageBox.Show("Please insert your Diablo CD in your disc drive and press OK to continue.", "Copy DIABDAT.MPQ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
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
            if(!IsAdministrator())
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
            catch(Exception ex)
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
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(Application.StartupPath + "\\EquineData\\config.json");
                File.WriteAllText(Application.StartupPath + "\\EquineData\\config.json", JsonConvert.SerializeObject(config));
                MessageBox.Show("Settings saved!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
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
                try
                {
                    int index = listView1.SelectedIndices[0] + 1;
                    int it = (index - listView1.Items.Count) + customModInfos.Count - 1;

                    if (listView1.SelectedItems[0].SubItems[6].Text != "Custom")
                        Process.Start(Application.StartupPath + "/" + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName);
                    else
                        Process.Start(Application.StartupPath + "/" + customModInfos[it].Name);
                }
                catch { }
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

                    if(MessageBox.Show("OK to force update?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
    }
}
