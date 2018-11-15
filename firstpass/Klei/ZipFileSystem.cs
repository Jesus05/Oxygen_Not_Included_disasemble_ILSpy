using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class ZipFileSystem : IFileSystem
	{
		private string id;

		private string mountPoint;

		private ZipFile zipfile;

		public string MountPoint => mountPoint;

		public ZipFileSystem(string id, Stream zip_data_stream, string mount_point = "")
		{
			this.id = id;
			mountPoint = FSUtil.Normalize(mount_point);
			zipfile = ZipFile.Read(zip_data_stream);
		}

		public string GetID()
		{
			return id;
		}

		public byte[] ReadBytes(string filename)
		{
			if (mountPoint.Length > 0)
			{
				filename = filename.Substring(mountPoint.Length);
			}
			ZipEntry zipEntry = zipfile[filename];
			if (zipEntry == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			zipEntry.Extract(memoryStream);
			return memoryStream.ToArray();
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (zipfile.Count > 0)
			{
				foreach (ZipEntry entry in zipfile.Entries)
				{
					if (!entry.IsDirectory)
					{
						string text = Path.Combine(mountPoint, entry.FileName);
						if (re.IsMatch(text))
						{
							result.Add(text);
						}
					}
				}
			}
		}
	}
}
