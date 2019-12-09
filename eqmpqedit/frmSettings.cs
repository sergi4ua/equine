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
        bool listFileModified = false;

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
                case GlobalVariableContainer.CompressionType.DEFLATE:
                    comboBox1.SelectedIndex = 3;
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
                    GlobalVariableContainer.compressionType = GlobalVariableContainer.CompressionType.DEFLATE;
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
    }
}
