using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eqmpqedit
{
    public sealed class GlobalVariableContainer
    {
        public static List<string> listFiles = new List<string>
        {
            Environment.CurrentDirectory + "\\EquineData\\eqmpqedit\\Diablo.txt",
            Environment.CurrentDirectory + "\\EquineData\\eqmpqedit\\Hellfire.txt"
        };
    }
}
