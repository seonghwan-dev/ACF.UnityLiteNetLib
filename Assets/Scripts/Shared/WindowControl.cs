#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
#define UNITY_STANDALONE_NOT_EDITOR
#endif

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Calci.Windows
{
	public static class WindowControl
	{
		private static IntPtr hWnd;
		private static bool isInitialized;
		private static bool isFlashing;
		
		public static void Init()
		{
#if UNITY_STANDALONE_NOT_EDITOR
			Init_Internal();
#endif
		}

		[Conditional("UNITY_STANDALONE_NOT_EDITOR")]
		public static void Find()
		{
#if UNITY_STANDALONE_NOT_EDITOR
			Find_Internal();
#endif
		}
		
		public static bool FlashWindowExUntilFront()
		{
#if UNITY_STANDALONE_NOT_EDITOR
			return FlashWindowExUntilFront_Internal();
#endif
			return false;
		}
		
		public static bool FlashWindowExUntilStop()
		{
#if UNITY_STANDALONE_NOT_EDITOR
			return FlashWindowExUntilStop_Internal();
#endif
			return false;
		}

		public static bool FlashStopWindowEx()
		{
#if UNITY_STANDALONE_NOT_EDITOR
			return FlashStopWindowEx_Internal();
#endif
			return false;
		}

		public static IEnumerator SetWindowPosition(int x, int y)
		{
#if UNITY_STANDALONE_NOT_EDITOR
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			NativeImport.User32.SetWindowPos(hWnd, 0, x, y, 0, 0, 5);
#else
			yield break;
#endif
		}

		[Conditional("UNITY_STANDALONE_NOT_EDITOR")]
		private static void Init_Internal()
		{
			if (!isInitialized)
			{
				isInitialized = true;
				hWnd = NativeImport.User32.GetActiveWindow();
			}
		}

		[Conditional("UNITY_STANDALONE_NOT_EDITOR")]
		private static void Find_Internal()
		{
			if (!hWnd.Equals(IntPtr.Zero))
			{
				NativeImport.User32.ShowWindowAsync(hWnd, 1);
			}

			SetForegroundWindowForce_Internal();
		}

		private static void SetForegroundWindowForce_Internal()
		{
			if (NativeImport.User32.GetForegroundWindow() != hWnd)
			{
				NativeImport.User32.SetWindowPos(hWnd, -1, 0, 0, 0, 0, 67);
				NativeImport.User32.SetWindowPos(hWnd, -2, 0, 0, 0, 0, 67);
			}
		}

		private static bool FlashWindowExUntilFront_Internal()
		{
			NativeImport.User32.FLASHWINFO structure = default(NativeImport.User32.FLASHWINFO);
			structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<NativeImport.User32.FLASHWINFO>(structure));
			structure.hwnd = hWnd;
			structure.dwFlags = 7U;
			structure.uCount = uint.MaxValue;
			structure.dwTimeout = 0U;
			return NativeImport.User32.FlashWindowEx(ref structure);
		}

		private static bool FlashWindowExUntilStop_Internal()
		{
			NativeImport.User32.FLASHWINFO structure = default(NativeImport.User32.FLASHWINFO);
			structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<NativeImport.User32.FLASHWINFO>(structure));
			structure.hwnd = hWnd;
			structure.dwFlags = 7U;
			structure.uCount = uint.MaxValue;
			structure.dwTimeout = 0U;
			isFlashing = true;
			return NativeImport.User32.FlashWindowEx(ref structure);
		}

		private static bool FlashStopWindowEx_Internal()
		{
			if (!isFlashing)
			{
				return false;
			}

			NativeImport.User32.FLASHWINFO structure = default(NativeImport.User32.FLASHWINFO);
			structure.cbSize = Convert.ToUInt32(Marshal.SizeOf<NativeImport.User32.FLASHWINFO>(structure));
			structure.hwnd = hWnd;
			structure.dwFlags = 0U;
			structure.uCount = uint.MaxValue;
			structure.dwTimeout = 0U;
			isFlashing = false;
			return NativeImport.User32.FlashWindowEx(ref structure);
		}
	}
}