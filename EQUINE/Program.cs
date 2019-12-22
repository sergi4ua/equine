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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

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
            // check if EQUINE is ran with -skipupdate argument

            string[] args = Environment.GetCommandLineArgs();
            if(args.Any("-skipupdate".Contains))
            {
                GlobalVariableContainer.skipUpdates = true;
            }

            /* Check for JSON dll */
            if (!File.Exists("NewtonSoft.Json.dll"))
            {
                MessageBox.Show("Unable to find NewtonSoft.Json.dll.\nYou probably forgot to extract it from the archive.");
                Environment.Exit(1);
            }
            bool noInit = false;

            if(!File.Exists("Diablo.exe"))
            {
                if (!File.Exists("Hellfire.exe"))
                {
                    MessageBox.Show("Unable to find Diablo.exe/Hellfire.exe!\nYou must put EQUINE.exe into your Diablo/Hellfire installation directory.\nProgram will now exit.", "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Environment.Exit(1);
                }
            }

            if(!Directory.Exists("EquineData"))
                noInit = true;

            if(!File.Exists("DIABDAT.MPQ"))
            {
                if(noInit == false)
                    MessageBox.Show("Unable to locate DIABDAT.MPQ. Some game mods will not run without it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                GlobalVariableContainer.DIABDATPresent = true;
            
            if(!Directory.Exists(Application.StartupPath + "\\EquineData\\ipx"))
            {
                if (noInit == false)
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmUpdateEquineData());
                    return;
                }
            }

            if(!File.Exists(Application.StartupPath + "\\EquineData\\modlist.json"))
            {
                if (noInit == false)
                {
                    if (File.Exists(Application.StartupPath + "\\EquineData\\modlist.xml"))
                        File.Delete(Application.StartupPath + "\\EquineData\\modlist.xml");
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmUpdateEquineData());
                    return;
                }
            }

            updateEquineUpdater();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // custom exception handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Program_UnhandledException);
            AppDomain.CurrentDomain.UnhandledException +=
        new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (noInit == false)
                Application.Run(new frmSplash());
            else
                Application.Run(new frmSetupWizard());
        }

        private static void updateEquineUpdater()
        {
            if (File.Exists(Application.StartupPath + "\\EquineData\\EQUINEUpdater_hash.sha"))
            {
                try
                    {
                        File.Copy(Application.StartupPath + "\\EquineData\\EQUINEUpdater.exe", Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash", true);

                        sha1 hash = new sha1();
                        string fromfilehash = "";
                        string apphash = "";

                        fromfilehash = File.ReadAllText(Application.StartupPath + "\\EquineData\\EQUINEUpdater_hash.sha");
                        apphash = hash.CheckFileHash(Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash");

                        if (fromfilehash != apphash)
                        {
                            File.Delete(Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash");
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmUpdateEquineData());
                        }

                        File.Delete(Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash");
                }
                    catch
                    {
                        MessageBox.Show("Unable to update EQUINEUpdater.exe");
                    }
                }
            }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            foreach(Form form in Application.OpenForms)
            {
                form.Hide();
            }

            frmFatalError iamerror = new frmFatalError();
            iamerror.OException = e;
            iamerror.ShowDialog();
        }

        private static void Program_UnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                form.Hide();
            }

            frmFatalError iamerror = new frmFatalError();
            iamerror.OException2 = e;
            iamerror.ShowDialog();
        }
    }
}
