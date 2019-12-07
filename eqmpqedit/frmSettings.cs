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

            if (listFileModified)
                GlobalVariableContainer.listFiles.Clear();

            foreach (var item in listBox1.Items)
            {
                GlobalVariableContainer.listFiles.Add(item.ToString());
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
           if(listBox1.SelectedIndex != -1)
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
           listFileModified = true;
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            listFileModified = false;
        }
    }
}
