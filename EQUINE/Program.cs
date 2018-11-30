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
using System.Windows.Forms;
using System.IO;

namespace EQUINE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if(!File.Exists("Diablo.exe"))
            {
                MessageBox.Show("Unable to locate Diablo.exe!\nProgram will now exit.", "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }

            if(!File.Exists("DIABDAT.MPQ"))
            {
                MessageBox.Show("Unable to locate DIABDAT.MPQ. Game mods will not run without it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                GlobalVariableContainer.DIABDATPresent = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
