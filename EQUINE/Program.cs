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
