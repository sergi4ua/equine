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

namespace eqmpqedit
{
    public class MPQFileExtensions
    {
        public static string getFileExtensionType(string extension)
        {
            extension = extension.ToLower();

            if (extension == ".dun")
                return "Diablo 1 dungeon file (*.dun)";
            if (extension == ".cel")
                return "Diablo 1 graphics file (*.cel)";
            if (extension == ".cl2")
                return "Diablo 1 advanced graphics file (*.cl2)";
            if (extension == ".wav")
                return "Wave sound file (*.wav)";
            if (extension == ".d2s")
                return "Diablo 2 character file (*.d2s)";
            if (extension == ".sol")
                return "Diablo 1 automap file (*.sol)";
            if (extension == ".bin")
                return "Diablo 2 binary file (*.bin)";
            if (extension == ".txt")
                return "Text file (*.txt)";
            if (extension == ".dt1")
                return "Diablo 2 level file (*.dt1)";
            if (extension == ".dc6")
                return "Diablo 2 graphics file (*.dc6)";
            if (extension == ".pal")
                return "Palette file (*.pal)";
            if (extension == ".mpq")
                return "MPQ archive (*.mpq)";
            if (extension == ".smk")
                return "Smacker video file (*.smk)";
            if (extension == ".mp3")
                return "MPEG Audio File (*.mp3)";
            if (extension == ".j")
                return "JASS (*.j)";
            if (extension == ".dll")
                return "Dynamic link library (*.dll)";
            if (extension == ".exe")
                return "Win32 Executable (*.exe)";
            if (extension == ".amp")
                return "Automap file (*.amp)";

            return "Unknown file extension";
        }
    }
}
