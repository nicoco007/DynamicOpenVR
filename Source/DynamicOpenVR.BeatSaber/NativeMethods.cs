using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DynamicOpenVR.BeatSaber
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError=true)]
        internal static extern int QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
    }
}
