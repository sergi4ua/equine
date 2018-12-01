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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EQUINE
{
    public partial class frmUninstall : Form
    {
        private List<string> files;

        public string modName { get; set; }

        public frmUninstall()
        {
            InitializeComponent();
        }

        private void frmUninstall_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                files = new List<string>(File.ReadAllLines(Application.StartupPath + "\\EquineData\\moduninstall\\"+modName+".uninstall"));

                for (int i = 0; i < files.Count; i++)
                {
                    FileAttributes attr = File.GetAttributes(files[i]);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                       
                            System.IO.DirectoryInfo di = new DirectoryInfo(files[i]);
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                       
                    }
                    else
                    {
                        if (File.Exists(files[i]))
                            File.Delete(files[i]);
                    }
                }
            }
            catch(Exception ex)
            {
               /* if (!ignoreerr) { }
                MessageBox.Show("Uninstallation failed.\nWindows reported the error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Invoke(new Action(() => Hide()));
                this.Invoke(new Action(() => Close()));*/
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Uninstallation complete. EQUINE will now restart.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
        }
    }
}
