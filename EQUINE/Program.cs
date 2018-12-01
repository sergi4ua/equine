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
            bool noInit = false;

            if(!File.Exists("Diablo.exe"))
            {
                MessageBox.Show("Unable to locate Diablo.exe!\nYou must put EQUINE.exe into your Diablo installation directory.\nProgram will now exit.", "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }

            if(!Directory.Exists("EquineData"))
                noInit = true;

            if(!File.Exists("DIABDAT.MPQ"))
            {
                if(noInit == false)
                    MessageBox.Show("Unable to locate DIABDAT.MPQ. Game mods will not run without it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                GlobalVariableContainer.DIABDATPresent = true;

            try
            {
                if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                {
                    if (!Directory.Exists(Application.StartupPath + "\\EquineData\\GameBackup"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "\\EquineData\\GameBackup");
                    }
                    if (File.Exists(Application.StartupPath + "\\Storm.dll") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\Storm.dll"))
                    {
                        File.Copy(Application.StartupPath + "\\Storm.dll", Application.StartupPath + "\\EquineData\\GameBackup\\Storm.dll");
                    }
                    else
                        MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (File.Exists(Application.StartupPath + "\\SMACKW32.DLL") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\SMACKW32.DLL"))
                    {
                        File.Copy(Application.StartupPath + "\\SMACKW32.DLL", Application.StartupPath + "\\EquineData\\GameBackup\\SMACKW32.DLL");
                    }
                    else
                        MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (File.Exists(Application.StartupPath + "\\diabloui.dll") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\diabloui.dll"))
                    {
                        File.Copy(Application.StartupPath + "\\diabloui.dll", Application.StartupPath + "\\EquineData\\GameBackup\\diabloui.dll");
                    }
                    else
                        MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (File.Exists(Application.StartupPath + "\\Diablo.exe") && !File.Exists(Application.StartupPath + "\\EquineData\\GameBackup\\Diablo.exe"))
                    {
                        File.Copy(Application.StartupPath + "\\Diablo.exe", Application.StartupPath + "\\EquineData\\GameBackup\\Diablo.exe");
                    }
                    else
                        MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            { MessageBox.Show("Warning: can't backup Diablo.exe, Storm.dll, SMACKW32.dll, diabloui.dll", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (noInit == false)
                Application.Run(new Form1());
            else
                Application.Run(new frmSetupWizard());
        }
    }
}
