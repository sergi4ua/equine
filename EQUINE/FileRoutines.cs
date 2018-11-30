// C# wrapper for win32 CopyFileEx

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
    along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace EQUINE
{
    public sealed class FileRoutines
    {
        public static void CopyFile(FileInfo source, FileInfo destination)
        {
            CopyFile(source, destination, CopyFileOptions.None);
        }

        public static void CopyFile(FileInfo source, FileInfo destination,
            CopyFileOptions options)
        {
            CopyFile(source, destination, options, null);
        }

        public static void CopyFile(FileInfo source, FileInfo destination,
            CopyFileOptions options, CopyFileCallback callback)
        {
            CopyFile(source, destination, options, callback, null);
        }

        public static void CopyFile(FileInfo source, FileInfo destination,
            CopyFileOptions options, CopyFileCallback callback, object state)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if ((options & ~CopyFileOptions.All) != 0)
                throw new ArgumentOutOfRangeException("options");

            new FileIOPermission(
                FileIOPermissionAccess.Read, source.FullName).Demand();
            new FileIOPermission(
                FileIOPermissionAccess.Write, destination.FullName).Demand();

            CopyProgressRoutine cpr = callback == null ?
                null : new CopyProgressRoutine(new CopyProgressData(
                    source, destination, callback, state).CallbackHandler);

            bool cancel = false;
            if (!CopyFileEx(source.FullName, destination.FullName, cpr,
                IntPtr.Zero, ref cancel, (int)options))
            {
                throw new IOException(new Win32Exception().Message);
            }
        }

        private class CopyProgressData
        {
            private FileInfo _source = null;
            private FileInfo _destination = null;
            private CopyFileCallback _callback = null;
            private object _state = null;

            public CopyProgressData(FileInfo source, FileInfo destination,
                CopyFileCallback callback, object state)
            {
                _source = source;
                _destination = destination;
                _callback = callback;
                _state = state;
            }

            public int CallbackHandler(
                long totalFileSize, long totalBytesTransferred,
                long streamSize, long streamBytesTransferred,
                int streamNumber, int callbackReason,
                IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
            {
                return (int)_callback(_source, _destination, _state,
                    totalFileSize, totalBytesTransferred);
            }
        }

        private delegate int CopyProgressRoutine(
            long totalFileSize, long TotalBytesTransferred, long streamSize,
            long streamBytesTransferred, int streamNumber, int callbackReason,
            IntPtr sourceFile, IntPtr destinationFile, IntPtr data);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CopyFileEx(
            string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData, ref bool pbCancel, int dwCopyFlags);
    }

    public delegate CopyFileCallbackAction CopyFileCallback(
        FileInfo source, FileInfo destination, object state,
        long totalFileSize, long totalBytesTransferred);

    public enum CopyFileCallbackAction
    {
        Continue = 0,
        Cancel = 1,
        Stop = 2,
        Quiet = 3
    }

    [Flags]
    public enum CopyFileOptions
    {
        None = 0x0,
        FailIfDestinationExists = 0x1,
        Restartable = 0x2,
        AllowDecryptedDestination = 0x8,
        All = FailIfDestinationExists | Restartable | AllowDecryptedDestination
    }
}
