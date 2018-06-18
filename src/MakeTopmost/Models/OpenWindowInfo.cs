using System;
using System.Diagnostics;

namespace MakeTopmost.Models
{
	public class OpenWindowInfo
	{
		public string Title { get; set; }

		public IntPtr Handle { get; set; }

		public Process Process { get; set; }

		public OpenWindowInfo(string title, IntPtr handle, Process process)
		{
			Title = title;
			Handle = handle;
			Process = process;
		}
	}
}