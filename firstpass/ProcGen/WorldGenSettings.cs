using Klei;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ProcGen
{
	public class WorldGenSettings
	{
		private delegate bool ParserFn<T>(string input, out T res);

		private World world;

		private Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private string base_path;

		private static string LAYERS_FILE = "layers";

		private static string FEATURES_FILE = "features";

		private static string RIVERS_FILE = "rivers";

		private static string ROOMS_FILE = "rooms";

		private static string TEMPERATURES_FILE = "temperatures";

		private static string DEFAULTS_FILE = "defaults";

		private static string MOBS_FILE = "mobs";

		[CompilerGenerated]
		private static ParserFn<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ParserFn<float> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ParserFn<int> _003C_003Ef__mg_0024cache2;

		public TerrainElementBandSettings biomes
		{
			get;
			private set;
		}

		public LevelLayerSettings layers
		{
			get;
			private set;
		}

		public TerrainFeatureSettings features
		{
			get;
			private set;
		}

		public Worlds worlds
		{
			get;
			private set;
		}

		public Rivers rivers
		{
			get;
			private set;
		}

		public RoomDescriptions rooms
		{
			get;
			private set;
		}

		public Temperatures temperatures
		{
			get;
			private set;
		}

		public NoiseTreeFiles noise
		{
			get;
			private set;
		}

		private DefaultSettings defaults
		{
			get;
			set;
		}

		public MobSettings mobs
		{
			get;
			private set;
		}

		public WorldGenSettings()
		{
			noise = new NoiseTreeFiles();
			worlds = new Worlds();
			biomes = new TerrainElementBandSettings();
		}

		public string GetDefaultBiome(string name)
		{
			if (features.TerrainFeatures.ContainsKey(name))
			{
				return features.TerrainFeatures[name].defaultBiome.type;
			}
			Debug.LogError("Couldn't get default biome [" + name + "]", null);
			return null;
		}

		public FeatureSettings GetFeature(string name)
		{
			if (name == "features/Sedimentary/StartLocation")
			{
				int num = 0;
				num++;
			}
			if (!name.StartsWith("features/"))
			{
				return null;
			}
			if (featuresettings.ContainsKey(name))
			{
				return featuresettings[name];
			}
			throw new Exception("Couldnt get feature [" + name + "]");
		}

		public string[] GetFeatureSettingsNames()
		{
			string[] array = new string[featuresettings.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, FeatureSettings> featuresetting in featuresettings)
			{
				array[num++] = featuresetting.Key;
			}
			return array;
		}

		public BaseLocation GetBaseLocation()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.baseData != null)
			{
				Output.Log($"World '{world.name}' is overriding baseData");
				return world.defaultsOverrides.baseData;
			}
			return defaults.baseData;
		}

		public List<string> GetOverworldAddTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.overworldAddTags != null)
			{
				Output.Log($"World '{world.name}' is overriding overworldAddTags");
				return world.defaultsOverrides.overworldAddTags;
			}
			return defaults.overworldAddTags;
		}

		public List<string> GetDefaultMoveTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.defaultMoveTags != null)
			{
				Output.Log($"World '{world.name}' is overriding defaultMoveTags");
				return world.defaultsOverrides.defaultMoveTags;
			}
			return defaults.defaultMoveTags;
		}

		private bool GetSetting<T>(DefaultSettings set, string target, ParserFn<T> parser, out T res)
		{
			if (set == null || set.data == null || !set.data.ContainsKey(target))
			{
				res = default(T);
				return false;
			}
			object obj = set.data[target];
			if (obj.GetType() == typeof(T))
			{
				res = (T)obj;
				return true;
			}
			bool flag = parser(obj as string, out res);
			if (flag)
			{
				set.data[target] = res;
			}
			return flag;
		}

		private T GetSetting<T>(string target, ParserFn<T> parser)
		{
			T res;
			if (world != null)
			{
				if (!GetSetting(world.defaultsOverrides, target, parser, out res))
				{
					GetSetting(defaults, target, parser, out res);
				}
				else
				{
					Output.Log($"World '{world.name}' is overriding setting '{target}'");
				}
			}
			else if (!GetSetting(defaults, target, parser, out res))
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
			if (File.Exists(srcPath + srcName + ".yaml"))
			{
				name = srcName;
				return true;
			}
			string[] array = srcName.Split('/');
			name = array[0];
			for (int i = 1; i < array.Length - 1; i++)
			{
				name = name + "/" + array[i];
			}
			if (File.Exists(srcPath + name + ".yaml"))
			{
				return true;
			}
			name = srcName;
			return false;
		}

		private void LoadBiome(string longName)
		{
			string name = string.Empty;
			if (GetPathAndName(base_path, longName, out name) && !biomeSettingsCache.ContainsKey(name))
			{
				BiomeSettings biomeSettings = YamlIO<BiomeSettings>.LoadFile(base_path + name + ".yaml", null);
				if (biomeSettings != null)
				{
					biomeSettingsCache.Add(name, biomeSettings);
					foreach (KeyValuePair<string, ElementBandConfiguration> item in biomeSettings.TerrainBiomeLookupTable)
					{
						string key = name + "/" + item.Key;
						if (!biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(key))
						{
							biomes.BiomeBackgroundElementBandConfigurations.Add(key, item.Value);
						}
					}
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load biome: " + name + " failed", null);
				}
			}
		}

		public static string GetSimpleName(string longName)
		{
			string[] array = longName.Split('/');
			return array[array.Length - 1];
		}

		private string LoadFeature(string longName)
		{
			string name = string.Empty;
			if (!GetPathAndName(base_path, longName, out name))
			{
				Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + name + " failed", null);
				return longName;
			}
			if (!featuresettings.ContainsKey(name))
			{
				FeatureSettings featureSettings = YamlIO<FeatureSettings>.LoadFile(base_path + name + ".yaml", null);
				if (featureSettings != null)
				{
					featuresettings.Add(name, featureSettings);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load feature: " + name + " failed", null);
				}
			}
			return name;
		}

		public void SetDefaultWorld(string path)
		{
			SetWorld("worlds/Default", path);
		}

		public bool SetWorld(string name, string path)
		{
			bool result = false;
			if (worlds.HasWorld(name))
			{
				Worlds.Data worldData = worlds.GetWorldData(name);
				world = worldData.world;
				Debug.Log("Set world to [" + name + "] " + path, null);
				biomeSettingsCache.Clear();
				base_path = path;
				world.LoadZones(noise, path);
				foreach (KeyValuePair<string, SubWorld> zone in world.Zones)
				{
					if (zone.Value.centralFeature != null)
					{
						zone.Value.centralFeature.type = LoadFeature(zone.Value.centralFeature.type);
					}
					foreach (WeightedBiome biome in zone.Value.biomes)
					{
						LoadBiome(biome.name);
					}
					foreach (Feature feature in zone.Value.features)
					{
						feature.type = LoadFeature(feature.type);
					}
				}
				foreach (KeyValuePair<string, TerrainFeature> terrainFeature in features.TerrainFeatures)
				{
					if (terrainFeature.Value.defaultBiome != null && terrainFeature.Value.defaultBiome.type != null)
					{
						LoadBiome(terrainFeature.Value.defaultBiome.type);
					}
				}
				foreach (KeyValuePair<string, ElementBandConfiguration> biomeBackgroundElementBandConfiguration in biomes.BiomeBackgroundElementBandConfigurations)
				{
					biomeBackgroundElementBandConfiguration.Value.ConvertBandSizeToMaxSize();
				}
				result = true;
			}
			return result;
		}

		public List<string> GetWorldNames()
		{
			return worlds.GetNames();
		}

		public Dictionary<string, Worlds.Data> GetAllWorldData()
		{
			return worlds.worldCache;
		}

		public World GetWorld()
		{
			return world;
		}

		public void Save(string path)
		{
			layers.Save(path + LAYERS_FILE + ".yaml", null);
			features.Save(path + FEATURES_FILE + ".yaml", null);
			rivers.Save(path + RIVERS_FILE + ".yaml", null);
			rooms.Save(path + ROOMS_FILE + ".yaml", null);
			temperatures.Save(path + TEMPERATURES_FILE + ".yaml", null);
			defaults.Save(path + DEFAULTS_FILE + ".yaml", null);
			mobs.Save(path + MOBS_FILE + ".yaml", null);
		}

		public static WorldGenSettings LoadFile(string path, IFileSystem filesystem)
		{
			WorldGenSettings worldGenSettings = new WorldGenSettings();
			worldGenSettings.worlds.LoadFiles(path, filesystem);
			worldGenSettings.layers = YamlIO<LevelLayerSettings>.LoadFile(path + LAYERS_FILE + ".yaml", null);
			worldGenSettings.layers.LevelLayers.ConvertBandSizeToMaxSize();
			worldGenSettings.features = YamlIO<TerrainFeatureSettings>.LoadFile(path + FEATURES_FILE + ".yaml", null);
			foreach (KeyValuePair<string, TerrainFeature> terrainFeature in worldGenSettings.features.TerrainFeatures)
			{
				terrainFeature.Value.name = terrainFeature.Key;
			}
			worldGenSettings.rivers = YamlIO<Rivers>.LoadFile(path + RIVERS_FILE + ".yaml", null);
			worldGenSettings.rooms = YamlIO<RoomDescriptions>.LoadFile(path + ROOMS_FILE + ".yaml", null);
			foreach (KeyValuePair<string, Room> room in worldGenSettings.rooms.rooms)
			{
				room.Value.name = room.Key;
			}
			worldGenSettings.temperatures = YamlIO<Temperatures>.LoadFile(path + TEMPERATURES_FILE + ".yaml", null);
			worldGenSettings.defaults = YamlIO<DefaultSettings>.LoadFile(path + DEFAULTS_FILE + ".yaml", null);
			worldGenSettings.mobs = YamlIO<MobSettings>.LoadFile(path + MOBS_FILE + ".yaml", null);
			foreach (KeyValuePair<string, Mob> item in worldGenSettings.mobs.MobLookupTable)
			{
				item.Value.name = item.Key;
			}
			return worldGenSettings;
		}
	}
}
