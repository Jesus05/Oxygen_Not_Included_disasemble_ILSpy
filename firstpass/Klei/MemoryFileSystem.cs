using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class MemoryFileSystem : IFileSystem
	{
		private string id;

		private string mountPoint;

		private Dictionary<string, byte[]> dataMap = new Dictionary<string, byte[]>();

		public MemoryFileSystem(string id, string mount_point = "")
		{
			this.id = id;
			mountPoint = FSUtil.Normalize(mount_point);
		}

		public string GetID()
		{
			return id;
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] value = null;
			dataMap.TryGetValue(filename, out value);
			return value;
		}

		private string GetFullFilename(string filename)
		{
			string path = FSUtil.Normalize(filename);
			return Path.Combine(mountPoint, path);
		}

		public void Map(string filename, byte[] data)
		{
			string fullFilename = GetFullFilename(filename);
			if (dataMap.ContainsKey(fullFilename))
			{
				throw new ArgumentException(string.Format("MemoryFileSystem: '{0}' is already mapped."));
			}
			dataMap[fullFilename] = data;
		}

		public void Unmap(string filename)
		{
			string fullFilename = GetFullFilename(filename);
			dataMap.Remove(fullFilename);
		}

		public void Clear()
		{
			dataMap.Clear();
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			string[] files = Directory.GetFiles(path);
			foreach (string key in dataMap.Keys)
			{
				if (re.IsMatch(key))
				{
					result.Add(key);
				}
			}
		}
	}
}
