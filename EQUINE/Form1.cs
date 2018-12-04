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

namespace EQUINE
{
    public partial class Form1 : Form
    {
        private List<ModInfo> ModInfos = new List<ModInfo>();
        private const bool _DEBUG = false;

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
            checkGameBackup();
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

        void Preloader()
        {
            if (CheckForInternetConnection() == true && _DEBUG == false)
            {
                try
                {
                    WebClient modInfo = new WebClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    modInfo.DownloadFile("https://raw.githubusercontent.com/sergi4ua/equine/master/EquineData/modlist.xml", Application.StartupPath + "\\EquineData\\modlist.xml");
                }
                catch
                {
                    label1.Text = "Error updating ModInfo :(";
                }
            }
        }

        private void initModList()
        {
            // read modlist.xml and add modinfos to list

            XmlDocument xmlModList = new XmlDocument();
            xmlModList.Load("EquineData\\modlist.xml");
            XmlElement xmlRoot = xmlModList.DocumentElement;

            // read modlist.xml data

            foreach (XmlNode xmlNode in xmlRoot)
            {
                ModInfo mi = new ModInfo();
                foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
                {
                    if (xmlChildNode.Name == "ModId")
                        mi._modID = Convert.ToInt32(xmlChildNode.InnerText);
                    if (xmlChildNode.Name == "ModName")
                        mi._modName = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "Game")
                        mi._game = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "Description")
                        mi._description = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "Author")
                        mi._author = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "ModVersion")
                        mi._modVersion = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "WebSite")
                        mi._website = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "DL")
                        mi._DL1 = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "DL2")
                        mi._DL2 = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "md5")
                        mi._md5 = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "RunExeAfterInstall")
                        mi._startExe0 = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "RunExeAfterInstall2")
                        mi._startExe1 = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "BeforeInstallMessage")
                        mi._beforeInstallMessage = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "AfterInstallMessage")
                        mi._afterInstallMessage = xmlChildNode.InnerText;
                    if (xmlChildNode.Name == "DiabdatRequired")
                        mi._diabdatRequired = Convert.ToBoolean(xmlChildNode.InnerText);
                    if (xmlChildNode.Name == "Executable")
                        mi._modExecutable = xmlChildNode.InnerText;
                }
                ModInfos.Add(mi);
            }

            // add to list
            for(int i = 0; i < ModInfos.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ModInfos[i]._modName;
                lvi.SubItems.Add(ModInfos[i]._game);
                lvi.SubItems.Add(ModInfos[i]._description);
                lvi.SubItems.Add(ModInfos[i]._author);
                lvi.SubItems.Add(ModInfos[i]._modVersion);
                lvi.SubItems.Add(ModInfos[i]._website);
                if (!System.IO.File.Exists(ModInfos[i]._modExecutable))
                    lvi.SubItems.Add("No");
                else
                    lvi.SubItems.Add("Yes");
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
                            button1.Text = "Install";
                            button1.Enabled = false;
                        }
                    }
                    return;
                }

                if (System.IO.File.Exists(ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable))
                {
                    System.Diagnostics.Process.Start(ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable);
                }
                else
                {
                    if (CheckForInternetConnection() == true)
                    {
                        if (File.Exists(Application.StartupPath + "\\DIABDAT.MPQ"))
                        {

                            frmModDownloader modDL = new frmModDownloader();
                            modDL.beforeDownloadMsg = ModInfos[listView1.SelectedIndices[0] - 1]._beforeInstallMessage;
                            modDL.afterDownloadMsg = ModInfos[listView1.SelectedIndices[0] - 1]._afterInstallMessage;
                            modDL.dlLink0 = ModInfos[listView1.SelectedIndices[0] - 1]._DL1;
                            modDL.dlLink1 = ModInfos[listView1.SelectedIndices[0] - 1]._DL2;
                            modDL.modName = ModInfos[listView1.SelectedIndices[0] - 1]._modName;
                            modDL.startExe0 = ModInfos[listView1.SelectedIndices[0] - 1]._startExe0;
                            modDL.startExe1 = ModInfos[listView1.SelectedIndices[0] - 1]._startExe1;
                            modDL.ShowDialog();
                        }
                        else
                        {
                            if (ModInfos[listView1.SelectedIndices[0] - 1]._diabdatRequired == true)
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
                        if (!System.IO.File.Exists(ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable))
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
            MessageBox.Show("EQUINE © 2018 Sergi4UA.\nThis software is in no way associated with or endorsed by Blizzard Entertainment®.\n\nVersion 0.6\nhttps://sergi4ua.pp.ua/equine\nFor any questions please contact me at: https://sergi4ua.pp.ua/contact.html or visit the GitHub: http://github.com/sergi4ua/equine \n\nBeta-testers:\nOgodei\nRadTang\nfearedbliss\nDavias \n\nHave an awesome day! :)", "About EQUINE...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItem19_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
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
                    if (MessageBox.Show("WARNING: you about to remove " + ModInfos[listView1.SelectedIndices[0] - 1]._modName + "!\nDoing this can lead to unpredictable results!\n\nAfter Uninstall is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nDO YOU WANT TO CONTINUE?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        frmUninstall frmUninstall = new frmUninstall();
                        frmUninstall.modExe = ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable;
                        frmUninstall.modName = ModInfos[listView1.SelectedIndices[0] - 1]._modName;
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
                    if (MessageBox.Show("WARNING: you about to remove " + ModInfos[listView1.SelectedIndices[0] - 1]._modName + "!\nDoing this can lead to unpredictable results!\n\nAfter Uninstall is complete:\nDo not load any saves created in this mod.\nDelete the mod saves\n\nDO YOU WANT TO CONTINUE?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        frmUninstall frmUninstall = new frmUninstall();
                        frmUninstall.modExe = ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable;
                        frmUninstall.modName = ModInfos[listView1.SelectedIndices[0] - 1]._modName;
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

                if (System.IO.File.Exists(ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable))
                {
                    System.Diagnostics.Process.Start(ModInfos[listView1.SelectedIndices[0] - 1]._modExecutable);
                }
                else
                {
                    if (CheckForInternetConnection() == true)
                    {
                        if (File.Exists(Application.StartupPath + "\\DIABDAT.MPQ"))
                        {
                            frmModDownloader modDL = new frmModDownloader();
                            modDL.beforeDownloadMsg = ModInfos[listView1.SelectedIndices[0] - 1]._beforeInstallMessage;
                            modDL.afterDownloadMsg = ModInfos[listView1.SelectedIndices[0] - 1]._afterInstallMessage;
                            modDL.dlLink0 = ModInfos[listView1.SelectedIndices[0] - 1]._DL1;
                            modDL.dlLink1 = ModInfos[listView1.SelectedIndices[0] - 1]._DL2;
                            modDL.modName = ModInfos[listView1.SelectedIndices[0] - 1]._modName;
                            modDL.startExe0 = ModInfos[listView1.SelectedIndices[0] - 1]._startExe0;
                            modDL.startExe1 = ModInfos[listView1.SelectedIndices[0] - 1]._startExe1;
                            modDL.ShowDialog();
                        }
                        else
                        {
                            if (ModInfos[listView1.SelectedIndices[0] - 1]._diabdatRequired == true)
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
            fupd.ShowDialog();
        }
    }
}
