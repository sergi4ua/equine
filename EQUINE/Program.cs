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
using Newtonsoft.Json;

namespace EQUINE
{
    static class Program
    {
        static Config config;

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool noInit = false;

            if(!File.Exists("Diablo.exe"))
            {
                if (!File.Exists("Hellfire.exe"))
                {
                    MessageBox.Show("Unable to locate Diablo.exe/Hellfire.exe!\nYou must put EQUINE.exe into your Diablo/Hellfire installation directory.\nProgram will now exit.", "Critical error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Environment.Exit(1);
                }
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

            if(CheckForInternetConnection() == true)
            {
                if (Directory.Exists(Application.StartupPath + "\\EquineData"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile("https://sergi4ua.pp.ua/equine/EQUINE_hash.sha", Application.StartupPath + "\\EquineData\\EQUINE_hash.sha");
                }
            }

#if RELEASE
            if(File.Exists(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha"))
            {
                if (noInit == false)
                {
                    try
                    {
                        File.Copy(Application.ExecutablePath, Application.StartupPath + "\\EQUINE.hash", true);

                        sha1 hash = new sha1();
                        string fromfilehash = "";
                        string apphash = "";

                        fromfilehash = File.ReadAllText(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha");
                        apphash = hash.CheckFileHash(Application.StartupPath + "\\EQUINE.hash");

                        if (fromfilehash != apphash)
                        {
                            var SelfProc = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Environment.CurrentDirectory,
                                FileName = Application.StartupPath + "\\EquineData\\EQUINEUpdater.exe",
                                Arguments = "-update",
                            };
                            File.Delete(Application.StartupPath + "\\EQUINE.hash");
                            Process.Start(SelfProc);
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to check for updates!\n" + ex.Message);
                    }
                }
            }
#endif
            
            if(!File.Exists(Application.StartupPath + "\\EquineData\\EQUINE_hash.sha"))
            {
                if (noInit == false)
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmUpdateEquineData());
                    return;
                }
            }

            if(!File.Exists(Application.StartupPath + "\\EquineData\\config.json"))
            {
                Config defaultConfigFile = new Config();
                defaultConfigFile.autoUpdate = true;
                File.WriteAllText(Application.StartupPath + "\\EquineData\\config.json", JsonConvert.SerializeObject(defaultConfigFile));
            }

            readJsonConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (noInit == false)
                Application.Run(new Form1());
            else
                Application.Run(new frmSetupWizard());
        }

        private static void readJsonConfig()
        {
            try
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Application.StartupPath + "/EquineData/config.json"));
                Form1.config = config;
            }
            catch
           {
                MessageBox.Show("Unable to read config file.");
            }
        }
    }
}
