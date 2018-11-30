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
