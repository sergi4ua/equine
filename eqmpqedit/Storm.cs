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

        [DllImport(STORMDLL, EntryPoint = "SFileReadFile")]
        public static extern bool SFileReadFile(int hFile, byte[] buffer, uint numberOfBytesToRead, ref uint numberOfBytesRead, int overlapped);

        [DllImport(STORMDLL, EntryPoint = "SFileOpenFile")]
        public static extern bool SFileOpenFile(string fileName, ref int hFile);
    }
}
