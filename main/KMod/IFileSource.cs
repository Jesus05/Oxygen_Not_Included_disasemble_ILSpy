using Klei;
using System.Collections.Generic;

namespace KMod
{
	public interface IFileSource
	{
		string GetRoot();

		bool Exists();

		void GetTopLevelItems(List<FileSystemItem> file_system_items);

		IFileSystem GetFileSystem();

		void CopyTo(string path, List<string> extensions = null);

		string Read(string relative_path);
	}
}
