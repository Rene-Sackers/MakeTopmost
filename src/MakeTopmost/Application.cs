using System;
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

		private static void QueryForTargetWindow()
		{
			Console.WriteLine("Choose a window:");

			var windows = Native.GetOpenedWindows()
				.Where(w => !string.IsNullOrWhiteSpace(w.Title))
				.OrderBy(w => w.Title)
				.ToArray();

			for (var i = 0; i < windows.Length; i++)
			{
				var windowInfo = windows[i];
				var processName = (windowInfo.Process.ProcessName + ".exe").TakeMaxLength(13);
				var windowTitle = windowInfo.Title.TakeMaxLength(50);

				Console.WriteLine($"[{i}]\t- {processName}\t\t-\t{windowTitle}");
			}

			Console.Write("ID: ");

			int chosenHandleId;
			while (!int.TryParse(Console.ReadLine(), out chosenHandleId) || chosenHandleId < 0 || chosenHandleId >= windows.Length)
				continue;

			var chosenWindowHandle = windows[chosenHandleId].Handle;

			ToggleWindowTopmost(chosenWindowHandle);
		}

		private static void ToggleWindowTopmost(IntPtr chosenWindowHandle)
		{
			Native.ToggleWindowTopmost(chosenWindowHandle, !Native.IsWindowTopmost(chosenWindowHandle));
		}
	}
}
