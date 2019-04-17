using Ionic.Zip;
using Klei;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KMod
{
	internal struct ZipFile : IFileSource
	{
		private string filename;

		private Ionic.Zip.ZipFile zipfile;

		private ZipFileSystem file_system;

		public ZipFile(string filename)
		{
			this.filename = filename;
			zipfile = Ionic.Zip.ZipFile.Read(filename);
			file_system = new ZipFileSystem(zipfile.Name, zipfile, Application.streamingAssetsPath);
		}

		public string GetRoot()
		{
			return filename;
		}

		public bool Exists()
		{
			return File.Exists(GetRoot());
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items)
		{
			HashSetPool<string, ZipFile>.PooledHashSet pooledHashSet = HashSetPool<string, ZipFile>.Allocate();
			foreach (ZipEntry item in zipfile)
			{
				string[] array = FSUtil.Normalize(item.FileName).Split('/');
				string text = array[0];
				if (pooledHashSet.Add(text))
				{
					file_system_items.Add(new FileSystemItem
					{
						name = text,
						type = ((1 >= array.Length) ? FileSystemItem.ItemType.File : FileSystemItem.ItemType.Directory)
					});
				}
			}
			pooledHashSet.Recycle();
		}

		public IFileSystem GetFileSystem()
		{
			return file_system;
		}

		public void CopyTo(string path, List<string> extensions = null)
		{
			foreach (ZipEntry entry in zipfile.Entries)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					foreach (string extension in extensions)
					{
						if (entry.FileName.ToLower().EndsWith(extension))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					string path2 = FSUtil.Normalize(Path.Combine(path, entry.FileName));
					string directoryName = Path.GetDirectoryName(path2);
					if (string.IsNullOrEmpty(directoryName) || FileUtil.CreateDirectory(directoryName))
					{
						using (MemoryStream memoryStream = new MemoryStream((int)entry.UncompressedSize))
						{
							entry.Extract(memoryStream);
							using (FileStream fileStream = FileUtil.Create(path2))
							{
								fileStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
							}
						}
					}
				}
			}
		}

		public string Read(string relative_path)
		{
			ICollection<ZipEntry> collection = zipfile.SelectEntries(relative_path);
			if (collection.Count == 0)
			{
				return string.Empty;
			}
			using (IEnumerator<ZipEntry> enumerator = collection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ZipEntry current = enumerator.Current;
					using (MemoryStream memoryStream = new MemoryStream((int)current.UncompressedSize))
					{
						current.Extract(memoryStream);
						return Encoding.UTF8.GetString(memoryStream.GetBuffer());
					}
				}
			}
			return string.Empty;
		}
	}
}
