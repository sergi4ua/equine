using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace EQUINE
{
    public class Logger
    {
        public static string logFilePath = "";

        public enum Level
        { 
            INFO,
            WARNING,
            ERROR,
        }

        public enum App
        {
            EQUINE,
            MOD_UPDATER,
            MOD_DOWNLOADER,
            MPQ_EXTRACT,
            MPQ_REBUILD,
            DIABDAT_COPIER
        }


        public static void initLogFile()
        {
            // if the log directory doesn't exist, create it
            if (!Directory.Exists(Application.StartupPath + "/EquineLogs"))
                Directory.CreateDirectory(Application.StartupPath + "/EquineLogs");

            // Create the log file
            logFilePath = Application.StartupPath + "/EquineLogs/" + "EquineLog_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".log";
            FileStream fs = File.Create(logFilePath);
            fs.Close();
        }

        /// <summary>
        ///     Write to log file
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="level">Severity level</param>
        /// <param name="app">App</param>
        public static void log(string text, Level level = Level.INFO, App app = App.EQUINE)
        {
            string severityLevel = "", appLog = "";

            switch(level)
            {
                case Level.INFO:
                    severityLevel = "[INFO]";
                    break;

                case Level.WARNING:
                    severityLevel = "[WARNING]";
                    break;

                case Level.ERROR:
                    severityLevel = "[ERROR]";
                    break;
            }

            switch (app)
            {
                case App.EQUINE:
                    appLog = "[EQUINE]";
                    break;

                case App.DIABDAT_COPIER:
                    appLog = "[DIABDAT_COPIER]";
                    break;

                case App.MOD_DOWNLOADER:
                    appLog = "[MOD_DOWNLOADER]";
                    break;

                case App.MOD_UPDATER:
                    appLog = "[MOD_UPDATER]";
                    break;

                case App.MPQ_EXTRACT:
                    appLog = "[MPQ_EXTRACT]";
                    break;

                case App.MPQ_REBUILD:
                    appLog = "[MPQ_REBUILD]";
                    break;
            }

            TextWriter tw = new StreamWriter(logFilePath, true);
            tw.WriteLine("[" + DateTime.UtcNow.ToString() + "]" +  appLog  +  severityLevel + " " + text);
            tw.Close();
        }
    }
}
