using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using eqmpqedit;

namespace EQUINE
{
    public partial class frmRebuildMpq : Form
    {
        MpqRebuild rebuilder;
        public bool CopyOriginal { private get; set; }
        bool copying = false;

        public frmRebuildMpq()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if(CopyOriginal)
            {
                if (File.Exists(Application.StartupPath + "/EquineData/DIABDAT.bak"))
                    File.Delete(Application.StartupPath + "/EquineData/DIABDAT.bak");

                label2.BeginInvoke((MethodInvoker)delegate () {
                    label2.Text = "DIABDAT.MPQ (backup)";
                });
                File.Copy(Application.StartupPath + "/DIABDAT.MPQ",
                    Application.StartupPath + "/EquineData/DIABDAT.bak");
            }

            label2.BeginInvoke((MethodInvoker)delegate () {
                label2.Text = "DIABDAT.MPQ (delete original)";
            });

            File.Delete(Application.StartupPath + "/DIABDAT.MPQ");

            copying = true;

            rebuilder = new MpqRebuild();
            rebuilder.RebuildDiabdat();
        }

        private void frmRebuildMpq_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(copying)
                label2.Text = rebuilder.GetProgressString;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!CopyOriginal)
                MessageBox.Show("Operation completed successfully!", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Operation completed successfully!\n\nOriginal file is located in: " + Application.StartupPath + "\\EquineData\\DIABDAT.bak" + "\nRename .bak to .mpq and copy it to the root folder if you need to restore the original file.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
            this.Hide();
        }
    }
}
