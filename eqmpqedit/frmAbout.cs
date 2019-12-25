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

using System.Media;
using System.IO;

namespace eqmpqedit
{
    public partial class frmAbout : Form
    {
        Stream wavFile;
        SoundPlayer snd;

        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            wavFile = Properties.Resources.stupid_bird;
            snd = new SoundPlayer(wavFile);
            snd.PlayLooping();
        }

        private void frmAbout_FormClosing(object sender, FormClosingEventArgs e)
        {
            snd.Stop();
            snd.Dispose();

            wavFile.Close();
            wavFile.Dispose();

            GC.Collect();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://patreon.com/sergi4ua");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.sergi4ua.com/equine");
        }
    }
}
