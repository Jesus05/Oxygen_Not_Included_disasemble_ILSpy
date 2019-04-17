using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KMod
{
	internal struct Directory : IFileSource
	{
		private PrefixFileSystem file_system;

		private string root;

		public Directory(string root)
		{
			this.root = root;
			file_system = new PrefixFileSystem(root, root, Application.streamingAssetsPath);
		}

		public string GetRoot()
		{
			return root;
		}

		public bool Exists()
		{
			return System.IO.Directory.Exists(GetRoot());
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(root);
			FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
			foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
			{
				file_system_items.Add(new FileSystemItem
				{
					name = fileSystemInfo.Name,
					type = ((!(fileSystemInfo is DirectoryInfo)) ? FileSystemItem.ItemType.File : FileSystemItem.ItemType.Directory)
				});
			}
		}

		public IFileSystem GetFileSystem()
		{
			return file_system;
		}

		public void CopyTo(string path, List<string> extensions = null)
		{
			try
			{
				CopyDirectory(root, path, extensions);
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path));
			}
			catch (IOException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, path));
			}
			catch (Exception ex3)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, ex3.Message));
			}
		}

		public string Read(string relative_path)
		{
			try
			{
				using (FileStream fileStream = File.OpenRead(Path.Combine(root, relative_path)))
				{
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					return Encoding.UTF8.GetString(array);
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		private static int CopyDirectory(string sourceDirName, string destDirName, List<string> extensions)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				return 0;
			}
			if (!FileUtil.CreateDirectory(destDirName))
			{
				return 0;
			}
			FileInfo[] files = directoryInfo.GetFiles();
			int num = 0;
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					foreach (string extension in extensions)
					{
						if (extension == Path.GetExtension(fileInfo.Name).ToLower())
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					string destFileName = Path.Combine(destDirName, fileInfo.Name);
					fileInfo.CopyTo(destFileName, false);
					num++;
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			DirectoryInfo[] array2 = directories;
			foreach (DirectoryInfo directoryInfo2 in array2)
			{
				string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
				num += CopyDirectory(directoryInfo2.FullName, destDirName2, extensions);
			}
			if (num == 0)
			{
				FileUtil.DeleteDirectory(destDirName);
			}
			return num;
		}
	}
}
