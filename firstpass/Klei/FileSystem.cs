using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public static class FileSystem
	{
		public static List<IFileDirectory> file_sources = new List<IFileDirectory>();

		public static void Initialize()
		{
			if (file_sources.Count == 0)
			{
				file_sources.Add(new RootDirectory());
			}
		}

		public static byte[] ReadBytes(string filename)
		{
			Initialize();
			foreach (IFileDirectory file_source in file_sources)
			{
				byte[] array = file_source.ReadBytes(filename);
				if (array != null)
				{
					return array;
				}
			}
			return null;
		}

		public static void GetFiles(Regex re, string path, ICollection<FileHandle> result)
		{
			Initialize();
			ListPool<string, IFileDirectory>.PooledList pooledList = ListPool<string, IFileDirectory>.Allocate();
			foreach (IFileDirectory file_source in file_sources)
			{
				pooledList.Clear();
				file_source.GetFiles(re, path, pooledList);
				foreach (string item in pooledList)
				{
					result.Add(new FileHandle
					{
						full_path = item,
						source = file_source
					});
				}
			}
			pooledList.Recycle();
		}

		public static void GetFiles(Regex re, string path, ICollection<string> result)
		{
			Initialize();
			foreach (IFileDirectory file_source in file_sources)
			{
				file_source.GetFiles(re, path, result);
			}
		}

		public static void GetFiles(string path, string filename_glob_pattern, ICollection<string> result)
		{
			GetFilesSearchParams(path, filename_glob_pattern, out string normalized_path, out Regex filename_regex);
			GetFiles(filename_regex, normalized_path, result);
		}

		public static void GetFiles(string path, string filename_glob_pattern, ICollection<FileHandle> result)
		{
			GetFilesSearchParams(path, filename_glob_pattern, out string normalized_path, out Regex filename_regex);
			GetFiles(filename_regex, normalized_path, result);
		}

		public static void GetFiles(string filename, ICollection<FileHandle> result)
		{
			GetFilesSearchParams(Path.GetDirectoryName(filename), Path.GetFileName(filename), out string normalized_path, out Regex filename_regex);
			GetFiles(filename_regex, normalized_path, result);
		}

		public static bool FileExists(string path)
		{
			Initialize();
			foreach (IFileDirectory file_source in file_sources)
			{
				if (file_source.FileExists(path))
				{
					return true;
				}
			}
			return false;
		}

		public static void ReadFiles(string filename, ICollection<byte[]> result)
		{
			Initialize();
			foreach (IFileDirectory file_source in file_sources)
			{
				byte[] array = file_source.ReadBytes(filename);
				if (array != null)
				{
					result.Add(array);
				}
			}
		}

		public static string ConvertToText(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		public static string Normalize(string filename)
		{
			return filename.Replace("\\", "/");
		}

		private static void GetFilesSearchParams(string path, string filename_glob_pattern, out string normalized_path, out Regex filename_regex)
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
					path = path.Substring(0, num + 1);
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
	}
}
