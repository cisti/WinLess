using System;
using System.Threading;

namespace WinLessCore.Helpers
{
    public static class SingleInstance
    {
        private const string MutexGuid = "6319DF72-507B-4131-ADAE-EB2EA65D7FC0";

        public static readonly int WM_SHOWFIRSTINSTANCE = WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", MutexGuid);

        private static Mutex mutex;

        public static bool Start()
        {
            mutex = new Mutex(true, MutexGuid, out bool onlyInstance);

            return onlyInstance;
        }

        public static void ShowFirstInstance()
        {
            WinApi.PostMessage(
                (IntPtr)WinApi.HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static void Stop()
        {
            mutex.ReleaseMutex();
        }
    }
}
