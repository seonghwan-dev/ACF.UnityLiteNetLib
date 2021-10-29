using System;
using System.Runtime.InteropServices;

namespace Calci.Windows
{
	public static class NativeImport
	{
		internal static class User32
		{
			private const string DLL = "user32.dll";
			
			[DllImport(DLL)]
			internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

			[DllImport(DLL)]
			internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

			[DllImport(DLL)]
			internal static extern IntPtr GetActiveWindow();

			[DllImport(DLL)]
			internal static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

			[DllImport(DLL)]
			internal static extern IntPtr GetForegroundWindow();

			[DllImport(DLL)]
			internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);	
			
			public struct FLASHWINFO
			{
				public uint cbSize;
				public IntPtr hwnd;
				public uint dwFlags;
				public uint uCount;
				public uint dwTimeout;
			}
			
			internal const int SW_SHOWNORMAL = 1;
			internal const int SW_SHOWMINIMIZED = 2;
			internal const int SW_SHOWMAXIMIZED = 3;
			
			internal const int HWND_TOPMOST = -1;
			internal const int HWND_NOTOPMOST = -2;
			
			internal const int SWP_NOSIZE = 1;
			internal const int SWP_NOMOVE = 2;
			internal const int SWP_SHOWWINDOW = 64;

			internal const uint FLASHW_STOP = 0U;
			internal const uint FLASHW_ALL = 3U;
			internal const uint FLASHW_TIMER = 4U;
			internal const uint FLASHW_TIMERNOFG = 12U;
		}
	}
}