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

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32")]
        public static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        static internal void AddShieldToButton(Button b)
        {
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Preloader();
            Text = GlobalVariableContainer.AppName;
            if (!GlobalVariableContainer.DIABDATPresent)
                menuItem2.Enabled = false;

            initModList();
            checkGameBackup();
            getModNamesforDiabloClicker();
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
                    label1.Text = "Error updating ModInfo :(";
                }
            }

            if (!Directory.Exists(Application.StartupPath + "\\EquineData\\ipx"))
                button6.Enabled = false;

            if(!IsAdministrator())
                AddShieldToButton(button6);

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
                if (!System.IO.File.Exists(ModInfos.ModInfo[i].ModName + "\\" + ModInfos.ModInfo[i].Executable))
                    lvi.SubItems.Add("No");
                else
                    lvi.SubItems.Add("Yes");
                listView1.Items.Add(lvi);
            }
        }

        private void getModNamesforDiabloClicker()
        {/*
            for(int i = 0; i < listView1.Items.Count; i++)
            {
                comboBox1.Items.Add(listView1.Items[i].Text);
            }*/
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
            installPlayMod();
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
            if(listView1.SelectedItems.Count > 0)
            {
                if(listView1.SelectedItems[0].Text == "Vanilla Game")
                    {
                        button1.Text = "Play";
                        button1.Enabled = true;
                    }
                    else
                    {
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
            MessageBox.Show("EQUINE © 2018 Sergi4UA.\nThis software is in no way associated with or endorsed by Blizzard Entertainment®.\n\nVersion 0.7\nhttps://sergi4ua.pp.ua/equine\nFor any questions please contact me at: https://sergi4ua.pp.ua/contact.html or visit the GitHub: http://github.com/sergi4ua/equine \n\nBeta-testers:\nOgodei\nRadTang\nfearedbliss\nDavias\nQndel \n\nHave an awesome day! :)", "About EQUINE...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItem19_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("backup/restore feature is broken for mods", "Note");
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
                    if (MessageBox.Show("WARNING: you about to remove " + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "!\nDoing this can lead to unpredictable results!\n\nAfter Uninstall is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nDO YOU WANT TO CONTINUE?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                    if (MessageBox.Show("WARNING: you about to remove " + ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].ModName + "!\nDoing this can lead to unpredictable results!\n\nAfter Uninstall is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nDO YOU WANT TO CONTINUE?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                        if (listView1.SelectedItems.Count == 0)
                        {
                            launchToolStripMenuItem.Text = "Install";
                            launchToolStripMenuItem.Enabled = false;
                        }
                    }
                    return;
                }

                if (System.IO.File.Exists(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable))
                {
                    System.Diagnostics.Process.Start(ModInfos.ModInfo[listView1.SelectedIndices[0] - 1].Executable);
                }
                else
                {
                    if (CheckForInternetConnection() == true)
                    {
                        if (File.Exists(Application.StartupPath + "\\DIABDAT.MPQ"))
                        {
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
    }
}
