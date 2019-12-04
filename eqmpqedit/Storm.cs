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
    }
}
