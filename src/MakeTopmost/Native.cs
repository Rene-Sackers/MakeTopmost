using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MakeTopmost
{
	public class WindowInfo
	{
		public IntPtr Handle { get; set; }

		public string Title { get; set; }
	}
	
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class Native
	{
		public static List<WindowInfo> GetOpenedWindows()
		{
			var shellWindow = GetShellWindow();
			var windows = new List<WindowInfo>();

			EnumWindows((hWnd, param) =>
			{
				if (hWnd == shellWindow)
					return true;

				if (!IsWindowVisible(hWnd))
					return true;


				var info = new WindowInfo
				{
					Handle = hWnd,
					Title = GetWindowTitle(hWnd)
				};

				windows.Add(info);

				return true;

			}, 0);

			return windows;
		}

		public static string GetWindowTitle(IntPtr hWnd)
		{
			var length = GetWindowTextLength(hWnd);

			if (length == 0)
				return null;

			var builder = new StringBuilder(length);
			GetWindowText(hWnd, builder, length + 1);
			
			return builder.ToString();
		}

		public static void ToggleWindowTopmost(IntPtr hWnd, bool topmost)
		{
			SetWindowPos(hWnd, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
		}

		public static bool IsWindowTopmost(IntPtr hWnd)
		{
			var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
			return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
		}

		private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

		[DllImport("USER32.DLL")]
		private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

		[DllImport("USER32.DLL")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("USER32.DLL")]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("USER32.DLL")]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("USER32.DLL")]
		private static extern IntPtr GetShellWindow();

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		private const uint SWP_NOSIZE = 0x0001;
		private const uint SWP_NOMOVE = 0x0002;
		private const uint SWP_SHOWWINDOW = 0x0040;

		private const int GWL_EXSTYLE = -20;
		private const int WS_EX_TOPMOST = 0x0008;
	}
}
