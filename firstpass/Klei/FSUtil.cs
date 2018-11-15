using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Klei
{
	public static class FSUtil
	{
		public static string Normalize(string filename)
		{
			return filename.Replace("\\", "/");
		}

		public static void GetFilesSearchParams(string path, string filename_glob_pattern, out string normalized_path, out Regex filename_regex)
		{
			normalized_path = null;
			filename_regex = null;
			int num = path.Length - 1;
			while ((num >= 0 && path[num] == '\\') || path[num] == '/')
			{
				num--;
			}
			if (num >= 0)
			{
				if (num < path.Length - 1)
				{
					path = path.Substring(0, num);
				}
				normalized_path = (path = Normalize(path));
				string str = filename_glob_pattern.Replace(".", "\\.").Replace("*", ".*");
				string str2 = path.Replace("\\", "\\\\").Replace("/", "\\/").Replace("(", "\\(")
					.Replace(")", "\\)")
					.Replace("[", "\\[")
					.Replace("]", "\\]")
					.Replace(".", "\\.")
					.Replace("+", "\\+");
				str2 = str2 + "/" + str + "$";
				filename_regex = new Regex(str2);
			}
		}

		public static void GetFiles(IFileSystem fs, string path, string filename_glob_pattern, ICollection<string> result)
		{
			GetFilesSearchParams(path, filename_glob_pattern, out string normalized_path, out Regex filename_regex);
			fs.GetFiles(filename_regex, normalized_path, result);
		}
	}
}
