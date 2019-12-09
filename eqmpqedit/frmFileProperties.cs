using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eqmpqedit
{
    public partial class frmFileProperties : Form
    {
        public frmFileProperties()
        {
            InitializeComponent();
        }

        public string fileName
            {
            get; set;
            }

        public int sizeUncompressed
        {
            get; set;
        }

        public int sizeCompressed2
        {
            get; set;
        }

        public int fileFlags
        {
            get; set;
        }

        public int hash
        {
            get; set;
        }
        public uint fileDecryptionKey { get; set; }

        private void frmFileProperties_Load(object sender, EventArgs e)
        {
            label2.Text = fileName;
            sizeUncompressedLabel.Text = Convert.ToString(sizeUncompressed);
            sizeCompressed.Text = Convert.ToString(sizeCompressed2);
            fileFlagsLabel.Text = Convert.ToString(fileFlags);
            hashIndexLabel.Text = Convert.ToString(hash);
            label8.Text = MPQFileExtensions.getFileExtensionType(System.IO.Path.GetExtension(fileName));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }
    }
}
