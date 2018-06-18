namespace MakeTopmost
{
	public static class StringExtensions
	{
		public static string TakeMaxLength(this string @string, int maxLength, string ellipsis = "...")
		{
			if (@string.Length <= maxLength)
				return @string;

			var ellipsisLength = ellipsis.Length;

			if (@string.Length <= ellipsisLength)
				return @string.Substring(0, maxLength);

			return @string.Substring(0, maxLength - ellipsisLength) + ellipsis;
		}
	}
}
