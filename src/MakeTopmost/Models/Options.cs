using System;
using CommandLine;

namespace MakeTopmost.Models
{
	public class Options
	{
		[Option('w', "windowhandle", Default = null, HelpText = "The window handle of the window to set topmost.")]
		public IntPtr? WindowHandle { get; set; }
	}
}
