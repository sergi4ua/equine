using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace eqmpqedit
{
    public class MpqExtract
    {
        private uint hMpq = 0;
        private uint hFile = 0;
        private bool done = false;
        private string progressString = "Please wait...";

        public string GetProgressString
        {
            get
            {
                return progressString;
            }
        }

        public bool IsDone
        {
            get
            {
                return done;
            }
            set
            {
                done = value;
            }
        }

        public bool Success
        {
            get;
            set;
        }

        public int Progress_File
        {
            get;
            set;
        }

        public int Progress_MaxFiles
        {
            get;
            set;
        }

        public void ExtractMPQ(string fileName, string listFile)
        {
            Progress_MaxFiles = 1;

            if(File.Exists(fileName))
            {
                if(Storm.SFileOpenArchive(fileName, 2, 0x8000, ref hMpq))
                {
                    // List file contents
                    List<string> listFileContent = new List<string>();
                    // Valid files in MPQ
                    List<string> validMPQFiles = new List<string>();

                    // Verify if list file exists
                    if(!File.Exists(listFile))
                    {
                        MessageBox.Show("Unable to extract DIABDAT.MPQ without a list file!", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        done = true;
                        return;
                    }

                    // Set the progress string
                    progressString = "Reading listfile...";

                    // Read contents of listfile into List

                    listFileContent = File.ReadAllLines(listFile).ToList();

                    // loop through the listfile and probe the MPQ for valid files inside the MPQ

                    for(int i = 0; i < listFileContent.Count(); i++)
                    {
                        progressString = "Probing file:\n" + listFileContent[i];
                        hFile = 0;

                        if (Storm.SFileOpenFileEx(hMpq, listFileContent[i], 0x00, ref hFile))
                        {
                            validMPQFiles.Add(listFileContent[i]);
                        }

                        if (hFile != 0)
                            Storm.SFileCloseFile(hFile);
                    }

                    // create the folder for DIABDAT.MPQ inside EquineData

                    progressString = "Preparing folder...";

                    try
                    {
                        Directory.CreateDirectory("EquineData/DIABDAT");
                    }
                    catch
                    {
                        MessageBox.Show("Can't create folder for extracted files!", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        done = true;
                        return;
                    }


                    // create the valid list file for DIABDAT.MPQ

                    File.WriteAllLines("EquineData/DIABDAT/DIABDAT.listfile.txt", validMPQFiles);

                    // report to Progress_MaxFiles
                    Progress_MaxFiles = validMPQFiles.Count();

                    // and finally extract the files

                    for(int i = 0; i < validMPQFiles.Count(); i++)
                    {
                        int _hFile = -1;
                        progressString = "Extracting file:\n" + validMPQFiles[i];

                        if (Storm.SFileOpenFile(validMPQFiles[i], ref _hFile))
                        {
                            uint fileSizeHigh = 0;
                            uint fileSize = Storm.SFileGetFileSize(_hFile, ref fileSizeHigh);
                            if ((fileSizeHigh == 0) && (fileSize > 0))
                            {
                                byte[] bs = new byte[fileSize];
                                uint countRead = 0;
                                Storm.SFileReadFile(_hFile, bs, fileSize, ref countRead, 0);

                                if (!Directory.Exists(Path.GetDirectoryName("EquineData/DIABDAT/" + validMPQFiles[i])))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName("EquineData/DIABDAT/" + validMPQFiles[i]));
                                }

                                FileStream F = new FileStream("EquineData/DIABDAT/" + validMPQFiles[i], FileMode.Create, FileAccess.ReadWrite);
                                F.Write(bs, 0, bs.Length);
                                F.Close();
                                Storm.SFileCloseFile(hFile);
                            }
                        }
                        Progress_File = i;
                    }

                    // we done :)
                    done = true;
                    Success = true;
                }
                else
                {
                    MessageBox.Show("Couldn't open DIABDAT.MPQ for reading.", "EQUINE MPQEdit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    done = true;
                    return;
                }
            }

            // close the file
            if (hFile != 0)
                Storm.SFileCloseFile(hFile);

            // close the archive
            if (hMpq != 0)
                Storm.SFileCloseArchive(hMpq);
        }
    }
}
