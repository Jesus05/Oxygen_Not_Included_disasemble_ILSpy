using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class PrefixFileSystem : IFileSystem
	{
		private string id;

		private string root;

		private string prefix;

		public PrefixFileSystem(string id, string actual_location, string path_prefix)
		{
			this.id = id;
			actual_location = FSUtil.Normalize(actual_location);
			path_prefix = FSUtil.Normalize(path_prefix);
			root = actual_location;
			prefix = path_prefix;
		}

		public string GetID()
		{
			return id;
		}

		private string GetActualPath(string filename)
		{
			if (!filename.StartsWith(prefix))
			{
				return filename;
			}
			string str = filename.Substring(prefix.Length);
			return FSUtil.Normalize(root + str);
		}

		private string GetVirtualPath(string filename)
		{
			if (!filename.StartsWith(root))
			{
				return filename;
			}
			string str = filename.Substring(root.Length);
			return FSUtil.Normalize(prefix + str);
		}

		public byte[] ReadBytes(string src_filename)
		{
			string actualPath = GetActualPath(src_filename);
			if (File.Exists(actualPath))
			{
				byte[] array = null;
				return File.ReadAllBytes(actualPath);
			}
			return null;
		}

		public void GetFiles(Regex re, string src_path, ICollection<string> result)
		{
			string actualPath = GetActualPath(src_path);
			if (Directory.Exists(actualPath))
			{
				string[] files = Directory.GetFiles(actualPath);
				string[] array = files;
				foreach (string filename in array)
				{
					string filename2 = FSUtil.Normalize(filename);
					string virtualPath = GetVirtualPath(filename2);
					if (re.IsMatch(virtualPath))
					{
						result.Add(virtualPath);
					}
				}
			}
		}
	}
}
