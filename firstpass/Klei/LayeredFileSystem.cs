using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class LayeredFileSystem : IFileSystem
	{
		private string id = "LayeredFS";

		private List<IFileSystem> filesystems = new List<IFileSystem>();

		public static LayeredFileSystem instance;

		private LayeredFileSystem()
		{
		}

		public string GetID()
		{
			return id;
		}

		public IList<IFileSystem> GetFileSystems()
		{
			return filesystems;
		}

		public static void CreateInstance()
		{
			instance = new LayeredFileSystem();
		}

		public static void DestroyInstance()
		{
			instance = null;
		}

		public void AddFileSystem(IFileSystem fs)
		{
			filesystems.Add(fs);
		}

		public void RemoveFileSystem(IFileSystem fs)
		{
			filesystems.Remove(fs);
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] array = null;
			for (int num = filesystems.Count - 1; num >= 0; num--)
			{
				array = filesystems[num].ReadBytes(filename);
				if (array != null)
				{
					break;
				}
			}
			return array;
		}

		public string ReadText(string filename)
		{
			byte[] bytes = ReadBytes(filename);
			return Encoding.UTF8.GetString(bytes);
		}

		public void GetFiles(string path, string filename_glob_pattern, ICollection<string> result)
		{
			FSUtil.GetFiles(this, path, filename_glob_pattern, result);
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			foreach (IFileSystem filesystem in filesystems)
			{
				filesystem.GetFiles(re, path, result);
			}
		}

		public bool Exists(string fullpath)
		{
			string fileName = Path.GetFileName(fullpath);
			string directoryName = Path.GetDirectoryName(fullpath);
			FSUtil.GetFilesSearchParams(directoryName, fileName, out string normalized_path, out Regex filename_regex);
			List<string> list = new List<string>();
			foreach (IFileSystem filesystem in filesystems)
			{
				filesystem.GetFiles(filename_regex, normalized_path, list);
				if (list.Count > 0)
				{
					break;
				}
			}
			GetFiles(directoryName, fileName, list);
			return list.Count > 0;
		}
	}
}
