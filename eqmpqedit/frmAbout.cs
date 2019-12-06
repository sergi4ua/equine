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
