using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmDDrawWrapper : Form
    {
        public frmDDrawWrapper()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
            radioButton3.Checked = !radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
            radioButton3.Checked = !radioButton2.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton3.Checked;
            radioButton2.Checked = !radioButton3.Checked;
        }

        private void frmDDrawWrapper_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(radioButton4.Checked)
            {
                if (System.IO.File.Exists("ddraw.dll"))
                {
                    System.IO.File.Delete("ddraw.dll");
                    MessageBox.Show("Game will use original ddraw.dll provided by your OS.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("ddraw.dll not found. Game is already using original ddraw.dll", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Hide();
        }
    }
}
