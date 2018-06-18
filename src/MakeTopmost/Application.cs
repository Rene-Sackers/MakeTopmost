using System;
using System.Collections.Generic;
using System.Linq;

namespace MakeTopmost
{
	public class Application
	{
		public void Process(Options options)
		{
			if (options.WindowHandle.HasValue)
			{
				ToggleWindowTopmost(options.WindowHandle.Value);
				return;
			}

			QueryForTargetWindow();
		}

		private void QueryForTargetWindow()
		{
			Console.WriteLine("Choose a process:");

			var windows = Native.GetOpenedWindows();

			var windowHandles = new Dictionary<int, IntPtr>();
			var handleId = 0;

			foreach (var window in windows.OrderBy(w => w.Title))
			{
				windowHandles.Add(++handleId, window.Handle);
				Console.WriteLine($"{handleId}\t-\t{window.Title}");
			}

			Console.Write("ID: ");

			int chosenHandleId;
			while (!int.TryParse(Console.ReadLine(), out chosenHandleId) || !windowHandles.ContainsKey(chosenHandleId))
				continue;

			var chosenWindowHandle = windowHandles[chosenHandleId];

			var targetWindowTitle = Native.GetWindowTitle(chosenWindowHandle);

			ToggleWindowTopmost(chosenWindowHandle);
		}

		private static void ToggleWindowTopmost(IntPtr chosenWindowHandle)
		{
			Native.ToggleWindowTopmost(chosenWindowHandle, !Native.IsWindowTopmost(chosenWindowHandle));
		}
	}
}
