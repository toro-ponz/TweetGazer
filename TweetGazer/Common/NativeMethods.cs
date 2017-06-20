using System;
using System.Runtime.InteropServices;

namespace TweetGazer.Common
{
    internal static class NativeMethods
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
    }
}