using Klei;
using System.Collections.Generic;
using System.IO;

namespace ProcGen
{
	public class Worlds
	{
		public Dictionary<string, World> worldCache = new Dictionary<string, World>();

		public bool HasWorld(string name)
		{
			return worldCache.ContainsKey(name);
		}

		public World GetWorldData(string name)
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

		public void LoadFiles(string path, List<YamlIO.Error> errors)
		{
			worldCache.Clear();
			UpdateWorldCache(path, errors);
		}

		private void UpdateWorldCache(string path, List<YamlIO.Error> errors)
		{
			ListPool<FileHandle, Worlds>.PooledList pooledList = ListPool<FileHandle, Worlds>.Allocate();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "worlds")), "*.yaml", pooledList);
			foreach (FileHandle item in pooledList)
			{
				FileHandle world_file = item;
				World world = YamlIO.LoadFile<World>(world_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					error.file = world_file;
					errors.Add(error);
				}, null);
				if (world == null)
				{
					DebugUtil.LogWarningArgs("Failed to load world: ", world_file.full_path);
				}
				else if (!world.skip)
				{
					world.filePath = GetWorldName(world_file.full_path);
					worldCache[world.filePath] = world;
				}
			}
			pooledList.Recycle();
		}
	}
}
