using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Klei
{
	public interface IFileSystem
	{
		byte[] ReadBytes(string filename);

		void GetFiles(Regex re, string path, ICollection<string> result);

		string GetID();

		bool FileExists(string path);
	}
}
