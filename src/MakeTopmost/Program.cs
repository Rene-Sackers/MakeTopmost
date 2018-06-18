using CommandLine;

namespace MakeTopmost
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var application = new Application();

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(application.Process);
		}
	}
}
