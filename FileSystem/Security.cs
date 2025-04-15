public static class Security
{
	public static void ValidateIsAllowedDirectory(string path)
	{
		var args = Environment.GetCommandLineArgs();
		var normalizedTargetPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

		bool isAllowed = false;

		foreach (var dir in args)
		{
			var normalizedDir = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

			if (normalizedTargetPath.StartsWith(normalizedDir, StringComparison.OrdinalIgnoreCase))
			{
				isAllowed = true;
				break;
			}
		}

		if (!isAllowed)
		{
			throw new UnauthorizedAccessException($"Path '{path}' is not within allowed directories.");
		}
	}
}
