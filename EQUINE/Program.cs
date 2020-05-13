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
using System.Security.Principal;
using System.Runtime.CompilerServices;

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
            //System.AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            // check if EQUINE is ran with -skipupdate argument

            Logger.initLogFile();
            Logger.log("EQUINE initalizing...");

            string[] args = Environment.GetCommandLineArgs();
            if(args.Any("-skipupdate".Contains))
            {
                GlobalVariableContainer.skipUpdates = true;
            }

            Logger.log("Checking if NewtonSoft.Json.dll exists...");

            /* Check for JSON dll */
            if (!File.Exists("NewtonSoft.Json.dll"))
            {
                Logger.log("File NewtonSoft.Json.dll does not exist, shutdown...", Logger.Level.ERROR, Logger.App.EQUINE);
                MessageBox.Show("Unable to find NewtonSoft.Json.dll.\nYou probably forgot to extract it from the archive.");
                Environment.Exit(1);
            }
            bool noInit = false;

            Logger.log("Checking if Diablo.exe exists...");

            if(!File.Exists("Diablo.exe"))
            {
                if (!File.Exists("Hellfire.exe"))
                {
                    Logger.log("File Diablo.exe does not exist, shutdown...", Logger.Level.ERROR, Logger.App.EQUINE);
                    MessageBox.Show("Unable to find Diablo.exe/Hellfire.exe!\nYou must put EQUINE.exe into your Diablo/Hellfire installation directory.\nProgram will now exit.", "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Environment.Exit(1);
                }
            }

            Logger.log("Files exist.");

            if (!Directory.Exists("EquineData"))
            {
                Logger.log("New install...");
                noInit = true;
            }

            Logger.log("Checking if DIABDAT.MPQ exist...");
            if (!File.Exists("DIABDAT.MPQ"))
            {
                Logger.log("DIABDAT.MPQ doesn't exist");
                if (noInit == false)
                    MessageBox.Show("Unable to locate DIABDAT.MPQ. Some game mods will not run without it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Logger.log("DIABDAT.MPQ is present");
                GlobalVariableContainer.DIABDATPresent = true;
            }

            Logger.log("Checking if IPX wrapper exist...");
            if(!Directory.Exists(Application.StartupPath + "\\EquineData\\ipx"))
            {
                if (noInit == false)
                {
                    Logger.log("IPX wrapepr doesn't exist...");
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmUpdateEquineData());
                    return;
                }
            }

            Logger.log("IPX wrapper install directory is present...");

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

            Logger.log("Checking for updates for the updater...");
            updateEquineUpdater();

            Logger.log("Enabling XP-styles...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.log("Setting custom exception handler...");
            // custom exception handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Program_UnhandledException);
            AppDomain.CurrentDomain.UnhandledException +=
        new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (noInit == false)
            {
                Logger.log("EQUINE is installed properly, launching...");
                Application.Run(new frmSplash());
            }
            else
            {
                Logger.log("Starting EQUINE Setup Wizard...");
                Application.Run(new frmSetupWizard());
            }
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
                        Logger.log("New update found for EQUINE Update Utility!");
                            File.Delete(Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash");
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmUpdateEquineData());
                        }

                    Logger.log("No updates found for EQUINE Update Utility");
                        File.Delete(Application.StartupPath + "\\EquineData\\EQUINEUpdater.hash");
                }
                    catch(Exception ex)
                    {
                        Logger.log("Couldn't update EQUINE Update Utility. The error was: " + ex.Message, Logger.Level.ERROR);
                        MessageBox.Show("Unable to update EQUINEUpdater.exe");
                    }
            }
            else
            {
                if (Directory.Exists(Application.StartupPath + "/EquineData"))
                {
                    Logger.log("Updating SHA-1 hash for EQUINE Update Utility...");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    WebClient wc = new WebClient();
                    wc.DownloadFile("https://www.sergi4ua.com/equine/EQUINEUpdater_hash.sha", Application.StartupPath + "/EquineData/EQUINEUpdater_hash.sha");
                    wc.Dispose();
                    updateEquineUpdater();
                }
            }
         }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.log("EQUINE CRASHED !!! in CurrentDomain", Logger.Level.ERROR);
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
            Logger.log("EQUINE CRASHED !!!", Logger.Level.ERROR);
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
