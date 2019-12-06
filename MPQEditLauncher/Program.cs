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
