using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class StandardFileSystem : IFileSystem
	{
		private string id = "StandardFS";

		public string GetID()
		{
			return id;
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] array = null;
			return File.ReadAllBytes(filename);
		}

		public string ReadText(string filename)
		{
			byte[] bytes = ReadBytes(filename);
			return Encoding.UTF8.GetString(bytes);
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				string[] array = files;
				foreach (string filename in array)
				{
					string text = FSUtil.Normalize(filename);
					if (re.IsMatch(text))
					{
						result.Add(text);
					}
				}
			}
		}
	}
}
