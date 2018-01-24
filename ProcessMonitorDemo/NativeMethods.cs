using System;
using System.Runtime.InteropServices;

namespace ProcessMonitorDemo
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi,
            EntryPoint = "CloseHandle", ExactSpelling = true, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode, EntryPoint = "CreateFileW",
            ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr CreateFile(
            string fileName,
            uint desiredAccess,
            uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi,
            EntryPoint = "DeviceIoControl", ExactSpelling = true, SetLastError = true)]
        internal static extern bool DeviceIoControl(
            IntPtr handle,
            uint ioControlCode,
            IntPtr inBuffer,
            uint inBufferSize,
            IntPtr outBuffer,
            uint outBufferSize,
            out uint bytesReturned,
            IntPtr overlapped);
    }
}