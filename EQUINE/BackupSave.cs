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
        public BackupSave()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("OK to backup saves?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //try
                //{
                List<string> saveFileExts = new List<string> { "*.sv", "*.hsv", "*.ssv"};
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
                    MessageBox.Show("Backup successful.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
               // }
                /*catch (Exception ex)
                {
                    MessageBox.Show("Backup failed.\nWindows reported the error:\n" + ex.Message, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }*/
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
            if (MessageBox.Show("OK to restore saves?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string saveBackupFolder = listBox1.SelectedItem.ToString();
                DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\EquineData\\SaveBackup\\" + saveBackupFolder);

                foreach(var file in di.GetFiles())
                {
                    File.Copy(file.FullName, Application.StartupPath + "\\" + file.Name, true);
                }
                MessageBox.Show("Restore successful.", "Backup");
            }
        }

        private void BackupSave_Load(object sender, EventArgs e)
        {
            if(Directory.Exists(Application.StartupPath + "\\EquineData\\SaveBackup"))
            {
                try
                {
                    string[] backupSaveDirs = Directory.GetDirectories(Application.StartupPath + "\\EquineData\\SaveBackup\\");
                    foreach (var dir in backupSaveDirs)
                    {
                        listBox1.Items.Add(new DirectoryInfo(dir).Name);
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
    }
}
