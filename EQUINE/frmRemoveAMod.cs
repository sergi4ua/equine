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
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EQUINE
{
    public partial class frmRemoveAMod : Form
    {
        List<CustomModInfo> modInfos;

        public frmRemoveAMod()
        {
            InitializeComponent();
        }

        private void frmRemoveAMod_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "/EquineData/customModList.json"))
            {
                MessageBox.Show("Nothing to delete.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Hide();
                this.Close();
                return;
            }
            else
            {
                modInfos = JsonConvert.DeserializeObject<List<CustomModInfo>>(File.ReadAllText(Application.StartupPath + "/EquineData/customModList.json"));

                if (modInfos.Count == 0)
                {
                    MessageBox.Show("Nothing to delete.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Hide();
                    this.Close();
                }

                foreach (var i in modInfos)
                {
                    listBox1.Items.Add(i.Name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("WARNING: All files in the following mod folder will be removed: \n\n" + listBox1.GetItemText(listBox1.SelectedItem) + "\nContinue?", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (listBox1.SelectedItems.Count > 0)
                    {
                        modInfos.RemoveAt(listBox1.SelectedIndex);
                        listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                        
                        // DO NOT UNCOMMENT !!
                       //Directory.Delete(Application.StartupPath + "\\" + listBox1.GetItemText(listBox1.SelectedItem) + "\\", true);
                    }

                    File.WriteAllText(Application.StartupPath + "/EquineData/customModList.json", JsonConvert.SerializeObject(modInfos, Formatting.Indented));
                    MessageBox.Show("Mod removed. Restart EQUINE for changes to take affect.", "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
           {
                MessageBox.Show("Unknown error has occured.");
           }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
    }
}
