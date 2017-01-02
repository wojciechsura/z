using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShellFoldersModule.Infrastructure
{
    static class WinApiInterop
    {
        [DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        internal static extern int SHGetLocalizedName(string pszPath, StringBuilder pszResModule, ref int cch, out int pidsRes);

        [DllImport("user32.dll", EntryPoint = "LoadStringW", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        internal static extern int LoadString(IntPtr hModule, int resourceID, StringBuilder resourceValue, int len);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryExW")]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        internal const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern int FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStringsW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint ExpandEnvironmentStrings(string lpSrc, StringBuilder lpDst, int nSize);

        public static string GetFullPath(string path)
        {
            StringBuilder sb = new StringBuilder(1024);

            ExpandEnvironmentStrings(path, sb, sb.Capacity);

            return sb.ToString();
        }

        public static string GetLocalizedName(string path)
        {
            StringBuilder resourcePath = new StringBuilder(1024);
            StringBuilder localizedName = new StringBuilder(1024);
            int len, id;
            len = resourcePath.Capacity;

            if (SHGetLocalizedName(path, resourcePath, ref len, out id) == 0)
            {               
                ExpandEnvironmentStrings(resourcePath.ToString(), resourcePath, resourcePath.Capacity);
                IntPtr hMod = LoadLibraryEx(resourcePath.ToString(), IntPtr.Zero, DONT_RESOLVE_DLL_REFERENCES | LOAD_LIBRARY_AS_DATAFILE);
                if (hMod != IntPtr.Zero)
                {
                    if (LoadString(hMod, id, localizedName, localizedName.Capacity) != 0)
                    {
                        return localizedName.ToString();
                    }
                    FreeLibrary(hMod);
                }
            }

            return null;
        }
    }
}
