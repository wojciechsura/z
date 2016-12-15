using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ControlPanelModule.Infrastructure
{
    static class WinapiInterop
    {
        private static uint LOAD_LIBRARY_AS_DATAFILE = 0x2;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        public static string GetStringResource(string encodedName)
        {
            Regex regex = new Regex(@"@?(.+),([\-0-9]+)[^\\]*");
            Match match = regex.Match(encodedName);
            if (match.Success)
            {
                try
                {
                    string filename = match.Groups[1].Value;
                    int index = int.Parse(match.Groups[2].Value);

                    IntPtr lib = LoadLibraryEx(filename, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                    var error = Marshal.GetLastWin32Error();
                    if (error != 0)
                        System.Diagnostics.Debug.WriteLine($"Win32 error: {error}");

                    if (lib != IntPtr.Zero)
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Capacity = 1024;
                            LoadString(lib, -index, sb, 1024);

                            // FreeLibrary will be called anyway
                            return sb.ToString();
                        }
                        finally
                        {
                            FreeLibrary(lib);
                        }
                    }
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }
    }
}
