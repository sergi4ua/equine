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
using System.Threading;
using System.Windows.Forms;

namespace EQUINE
{
    public partial class frmFatalError : Form
    {
        public UnhandledExceptionEventArgs OException { set; private get; }
        public ThreadExceptionEventArgs OException2 { set; private get; }

        public frmFatalError()
        {
            InitializeComponent();
        }

        private void frmFatalError_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(1);
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sergi4ua/equine/issues");
        }

        private void frmFatalError_Load(object sender, EventArgs e)
        {
            Exception ex;

            if (OException != null)
                ex = (Exception)OException.ExceptionObject;
            else
                ex = OException2.Exception;

            richTextBox1.Text =
                ex.Message + "\n" + ex.StackTrace + "\n" + ex.Data
                ;
        }
    }
}
