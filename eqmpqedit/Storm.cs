/*

Copyright (c) 2019, Sergi4UA <sergiy4ua@gmail.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace eqmpqedit
{
    public class Storm
    {
        private const string STORMDLL = "eqsfmpq.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct LCID
        {
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
            public char[] lcLocale;
        }

        public const uint MAFA_COMPRESS_STANDARD = 0x08; //Standard PKWare DCL compression
        public const uint MAFA_COMPRESS_DEFLATE = 0x02; //ZLib's deflate compression
        public const uint MAFA_COMPRESS_WAVE = 0x81; //Standard wave compression
        public const uint MAFA_COMPRESS_WAVE2 = 0x41; //Unused wave compression

        const uint SFILE_INFO_BLOCK_SIZE = 0x01; //Block size in MPQ
        const uint SFILE_INFO_HASH_TABLE_SIZE = 0x02; //Hash table size in MPQ
        const uint SFILE_INFO_NUM_FILES = 0x03; //Number of files in MPQ
        const uint SFILE_INFO_TYPE = 0x04; //Is int a file or an MPQ?
        const uint SFILE_INFO_SIZE = 0x05; //Size of MPQ or uncompressed file
        const uint SFILE_INFO_COMPRESSED_SIZE = 0x06; //Size of compressed file
        const uint SFILE_INFO_FLAGS = 0x07; //File flags (compressed, etc.), file attributes if a file not in an archive
        const uint SFILE_INFO_PARENT = 0x08; //int of MPQ that file is in
        const uint SFILE_INFO_POSITION = 0x09; //Position of file pointer in files
        const uint SFILE_INFO_LOCALEID = 0x0A; //Locale ID of file in MPQ
        const uint SFILE_INFO_PRIORITY = 0x0B; //Priority of open MPQ
        const uint SFILE_INFO_HASH_INDEX = 0x0C; //Hash index of file in MPQ

        public const uint MOAU_CREATE_NEW = 0x00;
        public const uint MOAU_CREATE_ALWAYS = 0x08; //Was wrongly named MOAU_CREATE_NEW
        public const uint MOAU_OPEN_EXISTING = 0x04;
        public const uint MOAU_OPEN_ALWAYS = 0x20;
        public const uint MOAU_READ_ONLY = 0x10; //Must be used with MOAU_OPEN_EXISTING
        public const uint MOAU_MAINTAIN_LISTFILE = 0x01;

        [DllImport(STORMDLL, CallingConvention = CallingConvention.Winapi, EntryPoint = "SFileOpenArchive")]
        public static extern bool SFileOpenArchive(
            string lpFileName, 
            uint dwPriority, uint dwFlags, ref uint hMpq);

        //BOOL      SFMPQAPI WINAPI SFileOpenFileEx(MPQHANDLE hMPQ, LPCSTR lpFileName, DWORD dwSearchScope, MPQHANDLE *hFile);
        [DllImport(STORMDLL, CallingConvention = CallingConvention.Winapi, EntryPoint = "SFileOpenFileEx")]
        [HandleProcessCorruptedStateExceptions]
        public static extern bool SFileOpenFileEx(uint hMpq, string lpFileName, uint dwSearchScope, ref uint hFile);

        [DllImport(STORMDLL, EntryPoint = "SFileSetLocale")]
        public static extern uint SFileSetLocale(uint nNewLocale);

        [DllImport(STORMDLL, EntryPoint = "SFileCloseFile")]
        public static extern bool SFileCloseFile(uint hFile);

        [DllImport(STORMDLL, EntryPoint = "SFileCloseArchive")]
        public static extern bool SFileCloseArchive(uint hMPQ);

        [DllImport(STORMDLL, EntryPoint = "SFileGetFileSize")]
        public static extern uint SFileGetFileSize(int hFile, ref uint highPartOfFileSize);
        [DllImport(STORMDLL, EntryPoint = "SFileGetFileSize")]
        public static extern uint SFileGetFileSize(uint hFile, ref uint highPartOfFileSize);

        [DllImport(STORMDLL, EntryPoint = "SFileReadFile")]
        public static extern bool SFileReadFile(int hFile, byte[] buffer, uint numberOfBytesToRead, ref uint numberOfBytesRead, int overlapped);

        [DllImport(STORMDLL, EntryPoint = "SFileOpenFile")]
        public static extern bool SFileOpenFile(string fileName, ref int hFile);

        [DllImport(STORMDLL, EntryPoint = "SFileCloseFile")]
        public static extern int SFileCloseFile(int hFile);

        [DllImport(STORMDLL)]
        public static extern int MpqOpenArchiveForUpdate(String lpFileName, uint dwFlags, uint dwMaximumFilesInArchive);
        [DllImport(STORMDLL)]
        public static extern uint MpqCloseUpdatedArchive(int hMPQ, uint dwUnknown2);
        [DllImport(STORMDLL)]
        public static extern int MpqAddFileToArchive(int hMPQ, String lpSourceFileName, String lpDestFileName, uint dwFlags);
        [DllImport(STORMDLL)]
        public static extern int MpqAddWaveToArchive(int hMPQ, String lpSourceFileName, String lpDestFileName, uint dwFlags, uint dwQuality);
        [DllImport(STORMDLL)]
        public static extern int MpqRenameFile(int hMPQ, String lpcOldFileName, String lpcNewFileName);
        [DllImport(STORMDLL)]
        public static extern int MpqDeleteFile(int hMPQ, String lpFileName);
        [DllImport(STORMDLL)]
        public static extern int MpqCompactArchive(int hMPQ);

        // Extra archive editing functions
        [DllImport(STORMDLL)]
        public static extern int MpqAddFileToArchiveEx(int hMPQ, String lpSourceFileName, String lpDestFileName, uint dwFlags, uint dwCompressionType, uint dwCompressLevel);
        [DllImport(STORMDLL)]
        public static extern int MpqAddFileFromBufferEx(int hMPQ, byte[] lpBuffer, uint dwLength, String lpFileName, uint dwFlags, uint dwCompressionType, uint dwCompressLevel);
        [DllImport(STORMDLL)]
        public static extern int MpqAddFileFromBuffer(int hMPQ, byte[] lpBuffer, uint dwLength, String lpFileName, uint dwFlags);
        [DllImport(STORMDLL)]
        public static extern int MpqAddWaveFromBuffer(int hMPQ, byte[] lpBuffer, uint dwLength, String lpFileName, uint dwFlags, uint dwQuality);
        [DllImport(STORMDLL)]
        public static extern int MpqSetFileLocale(int hMPQ, String lpFileName, LCID nOldLocale, LCID nNewLocale);
        [DllImport(STORMDLL)]
        public static extern int SFileGetFileInfo(int hFile, uint dwInfoType);
    }
}
