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

        public frmSettings()
        {
            InitializeComponent();
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            foreach(var item in GlobalVariableContainer.listFiles)
            {
                if (File.Exists(item))
                    listBox1.Items.Add(item);
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
            }
            else
                MessageBox.Show("You can't change the listfiles when a MPQ is open.\nClose the file and try again.", "EQUINE MPQEdit", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVariableContainer.ignoreEmbedListFile = checkBox1.Checked;
        }
    }
}
