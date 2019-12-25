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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eqmpqedit
{
    public class MpqFuncs
    {
        /// <summary>
        /// Deletes a file from the MPQ. CALL openMPQ(); after this !!
        /// </summary>
        /// <param name="mpqHandle">Handle of the MPQ file.</param>
        /// <param name="fileName">Filename to delete</param>
        /// <param name="mpqFileName">MPQ file location (open for modification)</param>
        /// <param name="mpqCloseFunc">Function to close the MPQ file</param>
        /// <returns></returns>
        public static bool deleteFile(uint mpqHandle, string fileName, string mpqFileName, Action mpqCloseFunc)
        {
            uint hFile = 0;

            if (!Storm.SFileOpenFileEx(mpqHandle, fileName, 0, ref hFile))
            {
                throw new System.IO.FileNotFoundException("File not found in the MPQ: " + fileName);
            }

            if (hFile != 0)
                Storm.SFileCloseFile(hFile);

            System.Threading.Thread.Sleep(1000);
            mpqCloseFunc();

            int modMPQ = Storm.MpqOpenArchiveForUpdate(mpqFileName, Storm.MOAU_OPEN_EXISTING, uint.MaxValue);

            if (modMPQ != 0)
                Storm.MpqDeleteFile(modMPQ, fileName);
            else
                throw new System.IO.IOException("ErrCode: " + Convert.ToString(Marshal.GetLastWin32Error()));

            Storm.MpqCloseUpdatedArchive(modMPQ, 0);

            return true;
        }

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="mpqHandle">The handle of the MPQ file.</param>
        /// <param name="fileName">The file to check</param>
        /// <returns></returns>
        public static bool fileExists(uint mpqHandle, string fileName)
        {
            uint hFile = 0; // unused

            if (!Storm.SFileOpenFileEx(mpqHandle, fileName, 0, ref hFile))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Extracts the file from the MPQ using SaveFileDialog
        /// </summary>
        /// <param name="fileName">File name to extract</param>
        /// <returns></returns>
        public static bool fileExtact(string fileName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(fileName);
            sfd.Filter = "All files (*.*)|*.*";
            sfd.Title = "Extract file...";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                int _hFile = -1;

                if (Storm.SFileOpenFile(fileName, ref _hFile))
                {
                    uint fileSizeHigh = 0;
                    uint fileSize = Storm.SFileGetFileSize(_hFile, ref fileSizeHigh);
                    if ((fileSizeHigh == 0) && (fileSize > 0))
                    {
                        byte[] bs = new byte[fileSize];
                        uint countRead = 0;

                        Storm.SFileReadFile(_hFile, bs, fileSize, ref countRead, 0);

                        FileStream F = new FileStream(sfd.FileName, FileMode.Create, FileAccess.ReadWrite);
                        F.Write(bs, 0, bs.Length);
                        F.Close();
                        Storm.SFileCloseFile(_hFile);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
