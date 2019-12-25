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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmNewUpdate : Form
    {
        public string modName { get; set; }
        public string DL { get; set; }
        public string DL2 { get; set; }

        public frmNewUpdate()
        {
            InitializeComponent();
        }

        private void frmNewUpdate_Load(object sender, EventArgs e)
        {
            label1.Text = modName + ": new update available!";

            try
            {
                var webRequest = WebRequest.Create(this.DL2);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    var strContent = reader.ReadToEnd();
                    textBox1.Text = strContent;
                    textBox1.Text = strContent.Replace("\n", Environment.NewLine);
                }
            }
            catch
            {
                textBox1.Text = "Failed to retrieve ChangeLog";
            }
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
