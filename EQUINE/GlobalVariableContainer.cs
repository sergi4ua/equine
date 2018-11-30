using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EQUINE
{
    public class GlobalVariableContainer
    {
        public static string Version = Application.ProductVersion.ToString();
        public static string AppName = "EQUINE for Diablo 1";
        public static bool DIABDATPresent = false;
    }
}
