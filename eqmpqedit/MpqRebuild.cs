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
using System.IO;

namespace eqmpqedit
{
    public class MpqRebuild
    {
        int hMPQ;
        List<string> listFile;

        private string currentFile = "...";

        public string GetProgressString
        {
            get
            {
                return currentFile;
            }
        }

        /// <summary>
        /// Rebuild DIABDAT.MPQ from DIABDAT folder.
        /// </summary>
        public void RebuildDiabdat()
        {
            try
            {
                // create new mpq
                hMPQ = Storm.MpqOpenArchiveForUpdate(Environment.CurrentDirectory + "/diabdat.mpq", Storm.MOAU_CREATE_NEW, ushort.MaxValue);

                // read list file
                listFile = File.ReadAllLines(Environment.CurrentDirectory + "/EquineData/DIABDAT/DIABDAT.listfile.txt").ToList();
            
                // add files to mpq
                foreach(var file in listFile)
                {
                    // update the string
                    currentFile = file;

                    // check if file exists
                    if(File.Exists(Environment.CurrentDirectory + "\\EquineData\\DIABDAT\\" + file))
                    {
                        switch(Path.GetExtension(file))
                        {
                            // if the file is wav

                            case ".wav":
                            case ".WAV":
                                currentFile = currentFile + " (WAVE)";
                                Storm.MpqAddWaveToArchive(hMPQ, Environment.CurrentDirectory + "\\EquineData\\DIABDAT\\" + file, file, 0x00000001, 1);
                                break;

                            case ".smk":
                            case ".SMK":
                            case ".DUN":
                            case ".dun":
                            case ".MIN":
                            case ".min":
                            case ".SOL":
                            case ".sol":
                            case ".TIL":
                            case ".til":
                            case ".PAL":
                            case ".pal":
                            case ".AMP":
                            case ".amp":
                            case ".MPQ":
                            case ".mpq":
                                Storm.MpqAddFileToArchiveEx(hMPQ, Environment.CurrentDirectory + "\\EquineData\\DIABDAT\\" + file, file, 0x80000000, 0,
                    0);
                                break;

                            // if the file is not wav
                            default:
                                Storm.MpqAddFileToArchiveEx(hMPQ, Environment.CurrentDirectory + "\\EquineData\\DIABDAT\\" + file, file, 0x00000100|0x00010000, Storm.MAFA_COMPRESS_STANDARD,
                    0);
                                break;
                        }
                    }
                }

                Storm.MpqCloseUpdatedArchive(hMPQ, 0);
            }
            catch(Exception ex)
            {
                throw new System.IO.IOException("MPQ/Listfile I/O error.\n" + ex.ToString());
            }
        }
    }
}
