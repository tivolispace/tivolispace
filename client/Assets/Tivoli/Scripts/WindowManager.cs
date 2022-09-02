using System;
using System.Runtime.InteropServices;

namespace Tivoli.Scripts
{
    public class WindowManager
    {
        #if UNITY_STANDALONE_WIN
        [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowText(IntPtr hwnd, String lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(String className, String windowName);
        // [DllImport("user32.dll")]
        // private static extern System.IntPtr GetActiveWindow();

        private IntPtr windowPtr;
        #endif
        
        public void UpdateWindowTitle()
        {
            var windowTitle = "Maki @ Squirrel Nut Cafe (Not Connected) v0.1 - Tivoli Cloud VR";
            
            #if UNITY_STANDALONE_WIN
            SetWindowText(windowPtr, windowTitle);
            #endif
        }

        public WindowManager()
        {
            #if UNITY_STANDALONE_WIN
            windowPtr = FindWindow(null, "Tivoli Cloud VR");
            #endif
            
            // TODO: use xdotool for linux support
            // TODO: mac os needs support
            
            UpdateWindowTitle();
        }
    }
}