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
    along with this program.If not, see <https://www.gnu.org/licenses/>.
*/

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
            WAVE = 3,
            NO_COMPRESSION = 4,
            ENCRYPT = 5
        }

        public static List<string> listFiles = new List<string>
        {
            
        };

        public static bool ignoreEmbedListFile = false;
        public static uint MAX_MPQ_FILES = 4096;
        public static CompressionType compressionType = CompressionType.STANDARD;
        public static bool showMPQFileSize = true;
        public static bool dontGenerateListFile = false;
    }
}
