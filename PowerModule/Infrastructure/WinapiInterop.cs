using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerModule.Infrastructure
{
    public static class WinapiInterop
    {
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, 
            int DesiredAccess, 
            ref IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string lpSystemName, 
            string lpName,
            ref long lpLuid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, 
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState, 
            int BufferLength, 
            IntPtr PreviousState, 
            IntPtr ReturnLength);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx(int uFlags, int dwReson);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

        private static bool DoExitWin(int flg)
        {
            bool result;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;

            result = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Luid = 0,
                Attributes = SE_PRIVILEGE_ENABLED
            };

            if (result)
                result = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            if (result)
                result = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            if (result)
                result = ExitWindowsEx(flg, 0);

            return result;
        }

        public static void Shutdown()
        {
            DoExitWin(EWX_SHUTDOWN);
        }

        public static void Reboot()
        {
            DoExitWin(EWX_REBOOT);
        }

        public static void Logoff()
        {
            DoExitWin(EWX_LOGOFF);
        }

        public static void Sleep()
        {
            Application.SetSuspendState(PowerState.Suspend, true, false);
        }

        public static void Hibernate()
        {
            Application.SetSuspendState(PowerState.Hibernate, true, false);
        }

        public static void Lock()
        {
            LockWorkStation();
        }
    }
}