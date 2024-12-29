using UnityEngine;

namespace MH3
{
    public static class NativeCaller
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _SyncFile();
#endif

        public static void SyncFile()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            _SyncFile();
#endif
        }
    }
}
