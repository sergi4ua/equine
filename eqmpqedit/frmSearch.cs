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
    public partial class frmSearch : Form
    {
        public string fileName { get; set; }

        public frmSearch()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Empty filename not allowed.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            fileName = textBox1.Text;
            this.Close();
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {

        }
    }
}
