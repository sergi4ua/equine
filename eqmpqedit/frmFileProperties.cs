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
