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
using System.Runtime.InteropServices;

namespace EQUINE
{
    public partial class frmUninstall : Form
    {
        public string modExe { get; set; }

        public string modName { get; set; }

        private bool error = false;
        public frmUninstall()
        {
            InitializeComponent();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        private void frmUninstall_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
                List<string> fileNames = new List<string> { "Storm.dll", "DiabloUI.dll", "Diablo.exe", "DIABDAT.MPQ", "SMACKW32.DLL", "ddraw.dll", "STANDARD.SNP", "BATTLE.SNP", "hellfrui.dll", "hfmonk.mpq", "hfmusic.mpq", "hfvoice.mpq", "hellfire.mpq" };

                // delete symbolic links, because the Directory.Delete function will obliterate end-user's Diablo directory
                // and we don't want that.

                foreach (var item in fileNames)
                {
                    FileInfo fileAttr = new FileInfo(Application.StartupPath + "/" + modName + "/" + item);

                    if (File.Exists(Application.StartupPath + "/" + modName + "/" + item))
                    { 
                        if (!fileAttr.Attributes.HasFlag(FileAttributes.ReadOnly))
                           {
                              if (!DeleteFileW(Application.StartupPath + "/" + modName + "/" + item))
                                {
                                    throw new Win32Exception("code returned: " + Marshal.GetLastWin32Error());
                                }
                            }
                            else
                            {
                                MessageBox.Show("File " + item + " in modfolder " + modName + " is read only!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                    }
                }

                // delete the mod folder

                Directory.Delete(Application.StartupPath + "/" + modName, true);

           // } 
            //catch(Exception ex)
            //{
            //    MessageBox.Show("Unable to uninstall the following mod: " + modName + 
            //        "\nWindows reported the error: " + ex.Message, 
            //        "EQUINE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    error = true;
            //}
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!error)
            {
                MessageBox.Show("Uninstallation complete. EQUINE will now restart (if it didn't, please restart the application manually)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Directory.SetCurrentDirectory(Application.StartupPath);
                Application.Restart();
            }
            else
            {
                this.Close();
                this.Hide();
            }
        }
    }
}
