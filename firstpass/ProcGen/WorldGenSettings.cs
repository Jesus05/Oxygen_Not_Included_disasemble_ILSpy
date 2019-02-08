using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ProcGen
{
	public class WorldGenSettings
	{
		private delegate bool ParserFn<T>(string input, out T res);

		public const string defaultWorldName = "worlds/Default";

		[CompilerGenerated]
		private static ParserFn<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ParserFn<float> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ParserFn<int> _003C_003Ef__mg_0024cache2;

		public World world
		{
			get;
			private set;
		}

		public WorldGenSettings(string worldName = "worlds/Default")
		{
			if (!SettingsCache.worlds.HasWorld(worldName))
			{
				Output.LogWarning(string.Format("Failed to get worldGen data for {0}. Using {1} instead", worldName, "worlds/Default"));
				DebugUtil.Assert(SettingsCache.worlds.HasWorld("worlds/Default"));
				worldName = "worlds/Default";
			}
			Worlds.Data worldData = SettingsCache.worlds.GetWorldData(worldName);
			world = worldData.world;
			Debug.Log("Set world to [" + worldName + "] " + SettingsCache.GetPath(), null);
		}

		public BaseLocation GetBaseLocation()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.baseData != null)
			{
				Output.Log($"World '{world.name}' is overriding baseData");
				return world.defaultsOverrides.baseData;
			}
			return SettingsCache.defaults.baseData;
		}

		public List<string> GetOverworldAddTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.overworldAddTags != null)
			{
				Output.Log($"World '{world.name}' is overriding overworldAddTags");
				return world.defaultsOverrides.overworldAddTags;
			}
			return SettingsCache.defaults.overworldAddTags;
		}

		public List<string> GetDefaultMoveTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.defaultMoveTags != null)
			{
				Output.Log($"World '{world.name}' is overriding defaultMoveTags");
				return world.defaultsOverrides.defaultMoveTags;
			}
			return SettingsCache.defaults.defaultMoveTags;
		}

		private bool GetSetting<T>(DefaultSettings set, string target, ParserFn<T> parser, out T res)
		{
			if (set != null && set.data != null && set.data.ContainsKey(target))
			{
				object obj = set.data[target];
				if (obj.GetType() != typeof(T))
				{
					bool flag = parser(obj as string, out res);
					if (flag)
					{
						set.data[target] = res;
					}
					return flag;
				}
				res = (T)obj;
				return true;
			}
			res = default(T);
			return false;
		}

		private T GetSetting<T>(string target, ParserFn<T> parser)
		{
			T res;
			if (world != null)
			{
				if (!GetSetting(world.defaultsOverrides, target, parser, out res))
				{
					GetSetting(SettingsCache.defaults, target, parser, out res);
				}
				else
				{
					Output.Log($"World '{world.name}' is overriding setting '{target}'");
				}
			}
			else if (!GetSetting(SettingsCache.defaults, target, parser, out res))
			{
				Output.LogWarning($"Couldn't find setting '{target}' in default settings!");
			}
			return res;
		}

		public bool GetBoolSetting(string target)
		{
			return GetSetting<bool>(target, bool.TryParse);
		}

		private bool TryParseString(string input, out string res)
		{
			res = input;
			return true;
		}

		public string GetStringSetting(string target)
		{
			return GetSetting<string>(target, this.TryParseString);
		}

		public float GetFloatSetting(string target)
		{
			return GetSetting<float>(target, float.TryParse);
		}

		public int GetIntSetting(string target)
		{
			return GetSetting<int>(target, int.TryParse);
		}

		public E GetEnumSetting<E>(string target) where E : struct
		{
			return GetSetting<E>(target, WorldGenSettings.TryParseEnum<E>);
		}

		public List<SubWorld> GetSubWorldList()
		{
			return new List<SubWorld>(world.Zones.Values);
		}

		public Dictionary<string, SubWorld> GetSubWorlds()
		{
			return world.Zones;
		}

		public SubWorld GetSubWorld(string name)
		{
			return world.GetSubWorld(name);
		}

		private bool GetPathAndName(string srcPath, string srcName, out string name)
		{
			if (!File.Exists(srcPath + srcName + ".yaml"))
			{
				string[] array = srcName.Split('/');
				name = array[0];
				for (int i = 1; i < array.Length - 1; i++)
				{
					name = name + "/" + array[i];
				}
				if (!File.Exists(srcPath + name + ".yaml"))
				{
					name = srcName;
					return false;
				}
				return true;
			}
			name = srcName;
			return true;
		}

		private static bool TryParseEnum<E>(string value, out E result) where E : struct
		{
			try
			{
				result = (E)Enum.Parse(typeof(E), value);
				return true;
			}
			catch (Exception)
			{
				result = new E();
			}
			return false;
		}

		public static string GetSimpleName(string longName)
		{
			string[] array = longName.Split('/');
			return array[array.Length - 1];
		}
	}
}
