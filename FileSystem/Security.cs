public static class Security
{
	public static void ValidateIsAllowedDirectory(string path)
	{
		var args = Environment.GetCommandLineArgs();
		// Normalize the target path
		string normalizedTargetPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

		// デフォルトでは未許可とする
		bool isAllowed = false;

		foreach (var dir in args)
		{
			string normalizedDir = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

			// Check if the target path is within the allowed directory path
			// 対象パスが許可されたディレクトリで始まるかをチェック（逆になっていたロジックを修正）
			if (normalizedTargetPath.StartsWith(normalizedDir, StringComparison.OrdinalIgnoreCase))
			{
				isAllowed = true;
				break;
			}
		}

		if (!isAllowed)
		{
			Console.Error.WriteLine("Unauthorized access path");
			throw new UnauthorizedAccessException($"Path '{path}' is not within allowed directories.");
		}
	}
}
