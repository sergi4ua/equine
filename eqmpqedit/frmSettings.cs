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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eqmpqedit
{
    public partial class frmSettings : Form
    {
        public bool mpqOpen { get; set; }
        private bool listFileModified = false;

        public frmSettings()
        {
            InitializeComponent();
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach(var item in GlobalVariableContainer.listFiles)
            {
                if (File.Exists(item))
                    listBox1.Items.Add(item);
            }

            comboBox1.SelectedIndex = 0;
            numericUpDown1.Value = GlobalVariableContainer.MAX_MPQ_FILES;
            checkBox1.Checked = GlobalVariableContainer.ignoreEmbedListFile;
            checkBox2.Checked = GlobalVariableContainer.showMPQFileSize;
            checkBox3.Checked = GlobalVariableContainer.dontGenerateListFile;

            switch(GlobalVariableContainer.compressionType)
            {
                case GlobalVariableContainer.CompressionType.STANDARD:
                    comboBox1.SelectedIndex = 0;
                    break;
                case GlobalVariableContainer.CompressionType.BZIP2:
                    comboBox1.SelectedIndex = 1;
                    break;
                case GlobalVariableContainer.CompressionType.ZLIB:
                    comboBox1.SelectedIndex = 2;
                    break;
                case GlobalVariableContainer.CompressionType.WAVE:
                    comboBox1.SelectedIndex = 3;
                    break;
                case GlobalVariableContainer.CompressionType.NO_COMPRESSION:
                    comboBox1.SelectedIndex = 4;
                    break;

                case GlobalVariableContainer.CompressionType.ENCRYPT:
                    comboBox1.SelectedIndex = 5;
                    break;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!mpqOpen)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Open listfile...";
                ofd.Filter = "Listfile (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.InitialDirectory = Application.StartupPath;
                listFileModified = true;

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    listBox1.Items.Add(ofd.FileName);
                }
            }
            else
                MessageBox.Show("You can't change the listfiles when a MPQ is open.\nClose the file and try again.", "EQUINE MPQEdit", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GlobalVariableContainer.ignoreEmbedListFile = checkBox1.Checked;
            GlobalVariableContainer.listFiles.Clear();
            GlobalVariableContainer.showMPQFileSize = checkBox2.Checked;
            GlobalVariableContainer.dontGenerateListFile = checkBox3.Checked;

            foreach (var item in listBox1.Items)
            {
                GlobalVariableContainer.listFiles.Add(item.ToString());
            }

            switch(comboBox1.SelectedIndex)
            {
                case -1:
                case 0:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.STANDARD;
                    break;

                case 1:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.BZIP2;
                    break;

                case 2:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.ZLIB;
                    break;

                case 3:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.WAVE;
                    break;

                case 4:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.NO_COMPRESSION;
                    break;

                case 5:
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.ENCRYPT;
                    break;
            }

            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listFileModified = false;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!mpqOpen)
            {
                if (listBox1.SelectedIndex != -1)
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listFileModified = true;
            }
            else
                MessageBox.Show("You can't change the listfiles when a MPQ is open.\nClose the file and try again.", "EQUINE MPQEdit",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            listFileModified = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
