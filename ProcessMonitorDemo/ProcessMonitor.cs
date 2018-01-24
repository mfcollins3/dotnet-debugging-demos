using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ProcessMonitorDemo
{
    public class ProcessMonitor : IDisposable
    {
        private const uint IoControlCode = (0x9535U << 16) | (0x2U << 14) | (0x81U << 2);

        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        private readonly IntPtr handle;

        private bool disposed;

        public ProcessMonitor()
        {
            this.handle = NativeMethods.CreateFile(
                "\\\\.\\Global\\ProcmonDebugLogger",
                0xC0000000U,
                7U,
                IntPtr.Zero,
                3U,
                0x80U,
                IntPtr.Zero);
            if (InvalidHandleValue != this.handle)
            {
                return;
            }

            var errorMessage = string.Format(
                CultureInfo.CurrentCulture,
                "CreateFile returned {0}",
                Marshal.GetLastWin32Error());
            throw new Exception(errorMessage);
        }

        ~ProcessMonitor()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void WriteMessage(string message)
        {
            var buffer = IntPtr.Zero;
            try
            {
                buffer = Marshal.StringToHGlobalUni(message);
                uint bytesWritten;
                var inBufferSize = Convert.ToUInt32(Math.Min(4096, message.Length * 2));
                var succeeded = NativeMethods.DeviceIoControl(
                    this.handle,
                    IoControlCode,
                    buffer,
                    inBufferSize,
                    IntPtr.Zero,
                    0,
                    out bytesWritten,
                    IntPtr.Zero);
                if (succeeded)
                {
                    return;
                }

                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "DeviceIoControl returned {0}",
                    Marshal.GetLastWin32Error());
                throw new Exception(errorMessage);
            }
            finally
            {
                if (IntPtr.Zero != buffer)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        public void WriteMessage(
            IFormatProvider formatProvider,
            string format,
            params object[] args)
        {
            var message = string.Format(formatProvider, format, args);
            this.WriteMessage(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (!NativeMethods.CloseHandle(this.handle))
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "CloseHandle returned {0}",
                    Marshal.GetLastWin32Error());
                throw new Exception(message);
            }

            if (!disposing)
            {
                return;
            }

            this.disposed = true;
        }
    }
}