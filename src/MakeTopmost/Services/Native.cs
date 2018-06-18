using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using MakeTopmost.Models;

namespace MakeTopmost.Services
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class Native
	{
		private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

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

		public static List<OpenWindowInfo> GetOpenedWindows()
		{
			var shellWindow = GetShellWindow();
			var windows = new List<OpenWindowInfo>();

			EnumWindows((windowHandle, param) =>
			{
				if (windowHandle == IntPtr.Zero)
					return true;

				if (windowHandle == shellWindow)
					return true;

				if (!IsWindowVisible(windowHandle))
					return true;

				var title = GetWindowTitle(windowHandle);
				var process = GetProcessFromWindow(windowHandle);

				var info = new OpenWindowInfo(title, windowHandle, process);

				windows.Add(info);

				return true;

			}, 0);

			return windows;
		}

		public static string GetWindowTitle(IntPtr windowHandle)
		{
			var length = GetWindowTextLength(windowHandle);

			if (length == 0)
				return null;

			var builder = new StringBuilder(length);
			GetWindowText(windowHandle, builder, length + 1);
			
			return builder.ToString();
		}

		public static void ToggleWindowTopmost(IntPtr windowHandle, bool topmost)
		{
			SetWindowPos(windowHandle, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
		}

		public static bool IsWindowTopmost(IntPtr windowHandle)
		{
			var exStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
			return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
		}

		public static Process GetProcessFromWindow(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero) 
				throw new InvalidOperationException("Window handle is zero.");

			GetWindowThreadProcessId(windowHandle, out var processId);

			return processId == 0 ?
				null :
				Process.GetProcessById((int)processId);
		}
	}
}
