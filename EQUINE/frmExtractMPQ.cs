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
using eqmpqedit;

namespace EQUINE
{
    public partial class frmExtractMPQ : Form
    {
        MpqExtract extractor = new MpqExtract();
        bool stopTimer = false;

        public frmExtractMPQ()
        {
            InitializeComponent();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            extractor.ExtractMPQ("DIABDAT.MPQ", "EquineData/eqmpqedit/Diablo.txt");
            BeginInvoke((MethodInvoker)delegate () { timer1.Enabled = true; timer1.Start(); });
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = extractor.GetProgressString + "\nFile : " + 
                extractor.Progress_File + " of " + extractor.Progress_MaxFiles;

            if (extractor.IsDone && !stopTimer)
            {
                stopTimer = true;
                if (extractor.Success)
                    MessageBox.Show("Operation completed successfully.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }

           // extractProgress = (extractor.Progress_File / extractor.Progress_MaxFiles) * 100;
            //progressBar1.Value = Convert.ToInt16(extractProgress);
            //progressBar1.PerformStep();
        }

        private void FrmExtractMPQ_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
            backgroundWorker1.RunWorkerAsync();
        }

        private void FrmExtractMPQ_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!extractor.IsDone)
            {
                if (MessageBox.Show("Cancel operation?", "EQUINE MPQEdit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // memory leak fix
                    extractor.cancel = true;
                    extractor.IsDone = true;
                    extractor.closeMPQ();
                    this.Hide();
                }
                else
                {
                    e.Cancel = true;
                }
            }

            GC.Collect();
        }
    }
}
