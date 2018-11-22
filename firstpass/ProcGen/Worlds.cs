using Klei;
using System.Collections.Generic;
using System.IO;

namespace ProcGen
{
	public class Worlds
	{
		public struct Data
		{
			public World world;
		}

		public Dictionary<string, Data> worldCache = new Dictionary<string, Data>();

		public bool HasWorld(string name)
		{
			return worldCache.ContainsKey(name);
		}

		public Data GetWorldData(string name)
		{
			return worldCache[name];
		}

		public List<string> GetNames()
		{
			return new List<string>(worldCache.Keys);
		}

		public static string GetWorldName(string path)
		{
			return "worlds/" + System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public void LoadFiles(string path, IFileSystem filesystem)
		{
			worldCache.Clear();
			UpdateWorldCache(path, filesystem);
		}

		private void UpdateWorldCache(string path, IFileSystem filesystem)
		{
			List<string> list = new List<string>();
			FSUtil.GetFiles(filesystem, System.IO.Path.Combine(path, "worlds"), "*.yaml", list);
			foreach (string item in list)
			{
				World world = YamlIO<World>.LoadFile(item, null);
				string worldName = GetWorldName(item);
				worldCache[worldName] = new Data
				{
					world = world
				};
			}
		}
	}
}
