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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using eqmpqedit;

namespace MPQEditLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                eqmpqedit.frmMain mpqEditForm = new eqmpqedit.frmMain();
                Application.Run(mpqEditForm);
            }
            catch(Exception ex)
            {
                MessageBox.Show("An internal error has occured in EQUINE MPQEdit.\nPlease file a issue at https://github.com/sergi4ua/equine \nMessage:\t" + ex.Message + "\nStack trace:\n" + ex.StackTrace + "\n" + ex.Source + "\n" + ex.InnerException + "\n\nThe error was copied to your clipboard.",
                    "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Clipboard.SetText("An internal error has occured in EQUINE MPQEdit.\nPlease file a issue at https://github.com/sergi4ua/equine \nMessage:\t" + ex.Message + "\nStack trace:\n" + ex.StackTrace + "\n" + ex.Source + "\n" + ex.InnerException);
                Environment.Exit(1);
            }
            Application.Exit();
        }
    }
}
