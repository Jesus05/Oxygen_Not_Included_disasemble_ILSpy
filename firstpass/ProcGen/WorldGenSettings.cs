using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ProcGen
{
	public class WorldGenSettings
	{
		private delegate bool ParserFn<T>(string input, out T res);

		private MutatedWorldData mutatedWorldData;

		public const string defaultWorldName = "worlds/SandstoneDefault";

		[CompilerGenerated]
		private static ParserFn<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ParserFn<float> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ParserFn<int> _003C_003Ef__mg_0024cache2;

		public World world => mutatedWorldData.world;

		public WorldGenSettings(string worldName = "worlds/SandstoneDefault", List<string> traits = null)
		{
			if (!SettingsCache.worlds.HasWorld(worldName))
			{
				DebugUtil.LogWarningArgs(string.Format("Failed to get worldGen data for {0}. Using {1} instead", worldName, "worlds/SandstoneDefault"));
				DebugUtil.Assert(SettingsCache.worlds.HasWorld("worlds/SandstoneDefault"));
				worldName = "worlds/SandstoneDefault";
			}
			World worldData = SettingsCache.worlds.GetWorldData(worldName);
			List<WorldTrait> list = new List<WorldTrait>();
			if (!worldData.disableWorldTraits && traits != null)
			{
				DebugUtil.LogArgs("Generating a world with the traits:", string.Join(", ", traits.ToArray()));
				foreach (string trait in traits)
				{
					list.Add(SettingsCache.GetCachedTrait(trait));
				}
			}
			else
			{
				Debug.Log("Generating a world without traits. Either this world has traits disabled or none were specified.");
			}
			mutatedWorldData = new MutatedWorldData(worldData, list);
			Debug.Log("Set world to [" + worldName + "] " + SettingsCache.GetPath());
		}

		public BaseLocation GetBaseLocation()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.baseData != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding baseData");
				return world.defaultsOverrides.baseData;
			}
			return SettingsCache.defaults.baseData;
		}

		public List<string> GetOverworldAddTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.overworldAddTags != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding overworldAddTags");
				return world.defaultsOverrides.overworldAddTags;
			}
			return SettingsCache.defaults.overworldAddTags;
		}

		public List<string> GetDefaultMoveTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.defaultMoveTags != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding defaultMoveTags");
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
					DebugUtil.LogArgs($"World '{world.name}' is overriding setting '{target}'");
				}
			}
			else if (!GetSetting(SettingsCache.defaults, target, parser, out res))
			{
				DebugUtil.LogWarningArgs($"Couldn't find setting '{target}' in default settings!");
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

		public bool HasFeature(string name)
		{
			return mutatedWorldData.features.ContainsKey(name);
		}

		public FeatureSettings GetFeature(string name)
		{
			if (!mutatedWorldData.features.ContainsKey(name))
			{
				throw new Exception("Couldnt get feature from active world data [" + name + "]");
			}
			return mutatedWorldData.features[name];
		}

		public FeatureSettings TryGetFeature(string name)
		{
			mutatedWorldData.features.TryGetValue(name, out FeatureSettings value);
			return value;
		}

		public bool HasSubworld(string name)
		{
			return mutatedWorldData.subworlds.ContainsKey(name);
		}

		public SubWorld GetSubWorld(string name)
		{
			if (!mutatedWorldData.subworlds.ContainsKey(name))
			{
				throw new Exception("Couldnt get subworld from active world data [" + name + "]");
			}
			return mutatedWorldData.subworlds[name];
		}

		public SubWorld TryGetSubWorld(string name)
		{
			mutatedWorldData.subworlds.TryGetValue(name, out SubWorld value);
			return value;
		}

		public List<WeightedSubWorld> GetSubworldsForWorld(List<WeightedName> subworldList)
		{
			List<WeightedSubWorld> list = new List<WeightedSubWorld>();
			foreach (KeyValuePair<string, SubWorld> subworld in mutatedWorldData.subworlds)
			{
				foreach (WeightedName subworld2 in subworldList)
				{
					if (subworld.Key == subworld2.name)
					{
						list.Add(new WeightedSubWorld(subworld2.weight, subworld.Value));
					}
				}
			}
			return list;
		}

		public bool HasMob(string id)
		{
			return mutatedWorldData.mobs.HasMob(id);
		}

		public Mob GetMob(string id)
		{
			return mutatedWorldData.mobs.GetMob(id);
		}

		public ElementBandConfiguration GetElementBandForBiome(string name)
		{
			if (!mutatedWorldData.biomes.BiomeBackgroundElementBandConfigurations.TryGetValue(name, out ElementBandConfiguration value))
			{
				return null;
			}
			return value;
		}
	}
}
