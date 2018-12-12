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
    public partial class frmProperties : Form
    {
        public string ModName
        {
            get { return label3.Text; }
            set { label3.Text = value;}
        }

        public string ModAuthor
        {
            get { return label5.Text; }
            set { label5.Text = value; }
        }

        public string ModWebsite
        {
            get { return linkLabel1.Text; }
            set { linkLabel1.Text = value; }
        }

        public string ModVersion { get { return label8.Text;} set { label8.Text = value; } }
        public string Description { get { return textBox1.Text; } set { textBox1.Text = value; } }

        public frmProperties()
        {
            InitializeComponent();
        }

        private void frmProperties_Load(object sender, EventArgs e)
        {
            label1.Text = label3.Text;
            Text = "Properties: '" + label3.Text + "'";
            if(label3.Text == "Vanilla Game")
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(linkLabel1.Text != "N/A" && linkLabel1.Text != "null")
                System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}
