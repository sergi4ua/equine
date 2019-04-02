using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmNewUpdate : Form
    {
        public string modName { get; set; }
        public string DL { get; set; }

        public frmNewUpdate()
        {
            InitializeComponent();
        }

        private void frmNewUpdate_Load(object sender, EventArgs e)
        {
            label1.Text = modName + " new update available!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(Application.StartupPath);
            frmModDownloader modDL = new frmModDownloader();
            modDL.beforeDownloadMsg = "null";
            modDL.afterDownloadMsg = "null";
            modDL.dlLink0 = this.DL;
            modDL.dlLink1 = "null";
            modDL.modName = this.modName;
            modDL.startExe0 = "null";
            modDL.startExe1 = "null";
            modDL.ShowDialog();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
