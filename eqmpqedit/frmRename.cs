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
