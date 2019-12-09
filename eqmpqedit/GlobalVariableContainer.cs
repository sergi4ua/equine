using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eqmpqedit
{
    public sealed class GlobalVariableContainer
    {
        public enum CompressionType
        {
            STANDARD = 0,
            BZIP2 = 1,
            ZLIB = 2,
            WAVE = 3
        }

        public static List<string> listFiles = new List<string>
        {
            
        };

        public static bool ignoreEmbedListFile = false;
        public static uint MAX_MPQ_FILES = 4096;
        public static CompressionType compressionType = CompressionType.STANDARD;
    }
}
