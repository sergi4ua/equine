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
    public partial class frmRename : Form
    {
        public string fileName { get; set; }
        private bool success = false;

        public frmRename()
        {
            InitializeComponent();
        }

        private void frmRename_Load(object sender, EventArgs e)
        {
            textBox1.Text = fileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Filename can't be blank.", "EQUINE MPQEdit");
                return;
            }

            fileName = textBox1.Text;
            DialogResult = DialogResult.OK;
            success = true;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                MessageBox.Show("No spaces allowed in filename.", "EQUINE MPQEdit");
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmRename_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!success)
                DialogResult = DialogResult.Cancel;
        }
    }
}
