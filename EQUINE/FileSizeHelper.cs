using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQUINE
{
    public class FileSizeHelper
    {
        public static string getFormattedFileSize(long bytes)
        {
            if(bytes > 1024)
            {
                long kb = bytes / 1024;

                if(kb > 1024)
                {
                    float mb = kb / 1024f;
                    return String.Format("{0:0.##} MiB", mb);
                }

                return String.Format("{0:0.##} KiB", kb);
            }
            else
            {
                return bytes + " bytes";
            }
        }
    }
}
