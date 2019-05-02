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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EQUINE
{
    public partial class AddAMod : Form
    {
        List<CustomModInfo> modInfos = new List<CustomModInfo>();
        CustomModInfo modInfo;

        public AddAMod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Mod name cannot be blank!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                modInfo = new CustomModInfo();
                modInfo.Name = textBox1.Text;
                if (textBox2.Text == "")
                    modInfo.Website = "N/A";
                else
                    modInfo.Website = textBox2.Text;

                modInfo.Description = textBox3.Text;
                modInfo.Version = textBox4.Text;
                if (textBox5.Text == "")
                {
                    MessageBox.Show("Mod Executable cannot be blank!", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                modInfo.Executable = textBox5.Text;
                modInfo.Author = textBox6.Text;
                modInfos.Add(modInfo);

                Directory.CreateDirectory(Application.StartupPath + "/" + textBox1.Text);

                List<string> fileNames = new List<string> { "Storm.dll", "DiabloUI.dll", "Diablo.exe", "DIABDAT.MPQ", "SMACKW32.DLL", "ddraw.dll", "STANDARD.SNP", "BATTLE.SNP", "hellfrui.dll", "hfmonk.mpq", "hfmusic.mpq", "hfvoice.mpq" };

                for (short i = 0; i < fileNames.Count; i++)
                {
                    if (File.Exists(Application.StartupPath + "\\" + fileNames[i]))
                    {
                        CreateSymbolicLink(Application.StartupPath + "\\" + textBox1.Text + "\\" + fileNames[i],
                        Application.StartupPath + "\\" + fileNames[i], SymbolicLink.File);
                    }
                }

                System.Diagnostics.Process.Start(Application.StartupPath + "/" + textBox1.Text);
                MessageBox.Show("Copy the contents of your mod to the created directory. Click OK to continue");

                // serialize to JSON
                File.WriteAllText(Application.StartupPath + "/EquineData/customModList.json", JsonConvert.SerializeObject(modInfos, Formatting.Indented));
                MessageBox.Show("Mod successfully added. EQUINE will now restart.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EQUINE Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Mod Executables (*.exe)|*.exe";
                dlg.Title = "Search Mod Executable...";
                dlg.CheckPathExists = true;
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox5.Text = Path.GetFileName(dlg.FileName);
                }


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EQUINE Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        private void AddAMod_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "/EquineData/customModList.json"))
                {
                    modInfos = JsonConvert.DeserializeObject<List<CustomModInfo>>(File.ReadAllText(Application.StartupPath + "/EquineData/customModList.json"));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EQUINE Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
