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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class BackupSave : Form
    {
        public List<string> installedMods { set; get; }
        bool a = false;

        public BackupSave()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("OK to backup saves?", "EQUINE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                backupVanillaSaves();

                // backup mod saves


                try
                {
                    foreach (var item in installedMods)
                    {
                        List<string> saveFileExts = new List<string> { "*.sv", "*.hsv", "*.ssv" };
                        for (int i = 0; i < saveFileExts.Count; i++)
                        {
                            string diabloDir = Application.StartupPath + "\\" + item + "\\";
                            string[] vanillaSaveFiles = Directory.GetFiles(diabloDir, saveFileExts[i]);
                            foreach (var file in vanillaSaveFiles)
                            {
                                if (!Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup\\" + item))
                                {
                                    Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\SaveBackup\\" + item);
                                }

                                if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                                {
                                    if (Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup\\" + item))
                                    {
                                        if (!Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup\\" + item + "\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")))
                                        {
                                            Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\SaveBackup\\" + item + "\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"));
                                            File.Copy(file, Application.StartupPath + "\\EquineData\\SaveBackup\\" + item + "\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + "\\" + Path.GetFileName(file));
                                        }
                                        else
                                        {
                                            File.Copy(file, Application.StartupPath + "\\EquineData\\SaveBackup\\" + item + "\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + "\\" + Path.GetFileName(file));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show("Backup successful.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Backup failed.\nWindows reported the error:\n" + ex.Message, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void backupVanillaSaves()
        {
            try
            {
                List<string> saveFileExts = new List<string> { "*.sv", "*.hsv", "*.ssv" };
                for (int i = 0; i < saveFileExts.Count; i++)
                {
                    string diabloDir = Application.StartupPath + "\\";
                    string[] vanillaSaveFiles = Directory.GetFiles(diabloDir, saveFileExts[i]);
                    foreach (var file in vanillaSaveFiles)
                    {
                        if (!Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup"))
                        {
                            Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\SaveBackup");
                        }

                        if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                        {
                            if (Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup"))
                            {
                                if (!Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")))
                                {
                                    Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\SaveBackup\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"));
                                    File.Copy(file, Application.StartupPath + "\\EquineData\\SaveBackup\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + "\\" + Path.GetFileName(file));
                                }
                                else
                                {
                                    File.Copy(file, Application.StartupPath + "\\EquineData\\SaveBackup\\" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + "\\" + Path.GetFileName(file));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup failed.\nWindows reported the error:\n" + ex.Message, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Hide();
            panel1.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Show();
            panel1.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                listBox1.SelectedIndex = 0;

            if (comboBox1.Text == "Vanilla Game")
            {
                if (MessageBox.Show("OK to restore saves?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string saveBackupFolder = listBox1.SelectedItem.ToString();
                    DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\EquineData\\SaveBackup\\" + saveBackupFolder);

                    foreach (var file in di.GetFiles())
                    {
                        File.Copy(file.FullName, Application.StartupPath + "\\" + file.Name, true);
                    }
                }
                else
                    return;
            }
            else
            {
                if (MessageBox.Show("OK to restore saves?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string saveBackupFolder = listBox1.SelectedItem.ToString();
                    DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\EquineData\\SaveBackup\\" + comboBox1.Text + "\\" + saveBackupFolder);

                    foreach (var file in di.GetFiles())
                    {
                        File.Copy(file.FullName, Application.StartupPath + "\\" + comboBox1.Text + "\\" + file.Name, true);
                    }
                }
                else
                    return;
            }

            MessageBox.Show("Restore successful.", "Backup");
        }

        private void BackupSave_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Vanilla Game");
            comboBox1.SelectedIndex = 0;
            a = true;
            foreach (var item in installedMods)
                comboBox1.Items.Add(item);

            if(Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup"))
            {
                try
                {
                    string[] backupSaveDirs = Directory.GetDirectories(Application.StartupPath + "\\EquineData\\SaveBackup\\");
                    foreach (var dir in backupSaveDirs)
                    {
                        listBox1.Items.Add(new DirectoryInfo(dir).Name);
                    }

                    // remove non date folders
                    int i = 0;
                    for(i = 0; i < listBox1.Items.Count; i++)
                    {
                        for(int b = 0; b < installedMods.Count; b++)
                        {
                            if(listBox1.Items[i].ToString() == installedMods[b])
                            {
                                listBox1.Items.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Can't fetch restore folders.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    this.Close();
                }
            }
            if(listBox1.Items.Count == 0)
            {
                groupBox2.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (a)
            {
                listBox1.Items.Clear();

                if (comboBox1.Text == "Vanilla Game")
                {
                    if (Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup"))
                    {
                        try
                        {
                            string[] backupSaveDirs = Directory.GetDirectories(Application.StartupPath + "\\EquineData\\SaveBackup\\");

                            foreach (var dir in backupSaveDirs)
                            {
                                listBox1.Items.Add(new DirectoryInfo(dir).Name);
                            }

                            // remove non date folders
                            int i = 0;
                            for (i = 0; i < listBox1.Items.Count; i++)
                            {
                                for (int b = 0; b < installedMods.Count; b++)
                                {
                                    if (listBox1.Items[i].ToString() == installedMods[b])
                                    {
                                        listBox1.Items.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Can't fetch restore folders.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            this.Close();
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup\\" + comboBox1.Text))
                    {
                        try
                        {
                            string[] backupSaveDirs = Directory.GetDirectories(Application.StartupPath + "\\EquineData\\SaveBackup\\" + comboBox1.Text);
                            foreach (var dir in backupSaveDirs)
                            {
                                listBox1.Items.Add(new DirectoryInfo(dir).Name);
                            }

                            // remove non date folders
                            int i = 0;
                            for (i = 0; i < listBox1.Items.Count; i++)
                            {
                                for (int b = 0; b < installedMods.Count; b++)
                                {
                                    if (listBox1.Items[i].ToString() == installedMods[b])
                                    {
                                        listBox1.Items.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Can't fetch restore folders.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            this.Close();
                        }
                    }
                }

                if (listBox1.Items.Count == 0)
                {
                    groupBox2.Enabled = false;
                    button2.Enabled = false;
                }
                else
                {
                    groupBox2.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }
    }
}
