using Klei;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProcGen
{
	public static class SettingsCache
	{
		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static Worlds worlds = new Worlds();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static string path = null;

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private const string LAYERS_FILE = "layers";

		private const string FEATURES_FILE = "features";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		public static LevelLayerSettings layers
		{
			get;
			private set;
		}

		public static TerrainFeatureSettings features
		{
			get;
			private set;
		}

		public static Rivers rivers
		{
			get;
			private set;
		}

		public static RoomDescriptions rooms
		{
			get;
			private set;
		}

		public static Temperatures temperatures
		{
			get;
			private set;
		}

		public static DefaultSettings defaults
		{
			get;
			set;
		}

		public static MobSettings mobs
		{
			get;
			private set;
		}

		public static string GetPath()
		{
			if (path == null)
			{
				path = FSUtil.Normalize(System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/"));
			}
			return path;
		}

		public static string GetDefaultBiome(string name)
		{
			if (features.TerrainFeatures.ContainsKey(name))
			{
				return features.TerrainFeatures[name].defaultBiome.type;
			}
			Debug.LogError("Couldn't get default biome [" + name + "]");
			return null;
		}

		public static FeatureSettings GetFeature(string name)
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

		public static string[] GetFeatureSettingsNames()
		{
			string[] array = new string[featuresettings.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, FeatureSettings> featuresetting in featuresettings)
			{
				array[num++] = featuresetting.Key;
			}
			return array;
		}

		private static bool GetPathAndName(IFileSystem file_system, string srcPath, string srcName, out string name)
		{
			if (file_system.FileExists(srcPath + srcName + ".yaml"))
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
			if (file_system.FileExists(srcPath + name + ".yaml"))
			{
				return true;
			}
			name = srcName;
			return false;
		}

		private static void LoadBiome(IFileSystem file_system, string longName)
		{
			string name = string.Empty;
			if (GetPathAndName(file_system, GetPath(), longName, out name) && !biomeSettingsCache.ContainsKey(name))
			{
				BiomeSettings biomeSettings = YamlIO<BiomeSettings>.LoadFile(GetPath() + name + ".yaml", null);
				if (biomeSettings != null)
				{
					Debug.Assert(biomeSettings.TerrainBiomeLookupTable.Count > 0, longName);
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
					Debug.LogWarning("WorldGen: Attempting to load biome: " + name + " failed");
				}
			}
		}

		private static string LoadFeature(IFileSystem file_system, string longName)
		{
			string name = string.Empty;
			if (!GetPathAndName(file_system, GetPath(), longName, out name))
			{
				Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + name + " failed");
				return longName;
			}
			if (!featuresettings.ContainsKey(name))
			{
				FeatureSettings featureSettings = YamlIO<FeatureSettings>.LoadFile(GetPath() + name + ".yaml", null);
				if (featureSettings != null)
				{
					featuresettings.Add(name, featureSettings);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load feature: " + name + " failed");
				}
			}
			return name;
		}

		public static void LoadZoneContents(IFileSystem file_system, IEnumerable<SubWorld> zones)
		{
			foreach (SubWorld zone in zones)
			{
				if (zone.centralFeature != null)
				{
					zone.centralFeature.type = LoadFeature(file_system, zone.centralFeature.type);
				}
				foreach (WeightedBiome biome in zone.biomes)
				{
					LoadBiome(file_system, biome.name);
				}
				foreach (Feature feature in zone.features)
				{
					feature.type = LoadFeature(file_system, feature.type);
				}
			}
		}

		public static List<string> GetWorldNames()
		{
			return worlds.GetNames();
		}

		public static Dictionary<string, Worlds.Data> GetAllWorldData()
		{
			return worlds.worldCache;
		}

		public static void Save(string path)
		{
			layers.Save(path + "layers.yaml", null);
			features.Save(path + "features.yaml", null);
			rivers.Save(path + "rivers.yaml", null);
			rooms.Save(path + "rooms.yaml", null);
			temperatures.Save(path + "temperatures.yaml", null);
			defaults.Save(path + "defaults.yaml", null);
			mobs.Save(path + "mobs.yaml", null);
		}

		public static void Clear()
		{
			worlds.worldCache.Clear();
			layers = null;
			features = null;
			biomes.BiomeBackgroundElementBandConfigurations.Clear();
			biomeSettingsCache.Clear();
			rivers = null;
			rooms = null;
			temperatures = null;
			noise.tree_files.Clear();
			defaults = null;
			mobs = null;
			featuresettings.Clear();
		}

		public static bool LoadFiles(IFileSystem file_system)
		{
			if (worlds.worldCache.Count > 0)
			{
				return false;
			}
			worlds.LoadFiles(GetPath(), file_system);
			foreach (KeyValuePair<string, Worlds.Data> item in worlds.worldCache)
			{
				Worlds.Data value = item.Value;
				value.world.LoadZones(noise, GetPath());
				Worlds.Data value2 = item.Value;
				LoadZoneContents(file_system, value2.world.Zones.Values);
			}
			layers = YamlIO<LevelLayerSettings>.LoadFile(GetPath() + "layers.yaml", null);
			layers.LevelLayers.ConvertBandSizeToMaxSize();
			features = YamlIO<TerrainFeatureSettings>.LoadFile(GetPath() + "features.yaml", null);
			foreach (KeyValuePair<string, TerrainFeature> terrainFeature in features.TerrainFeatures)
			{
				terrainFeature.Value.name = terrainFeature.Key;
				if (terrainFeature.Value.defaultBiome != null && terrainFeature.Value.defaultBiome.type != null)
				{
					LoadBiome(file_system, terrainFeature.Value.defaultBiome.type);
				}
			}
			rivers = YamlIO<Rivers>.LoadFile(GetPath() + "rivers.yaml", null);
			rooms = YamlIO<RoomDescriptions>.LoadFile(path + "rooms.yaml", null);
			foreach (KeyValuePair<string, Room> room in rooms.rooms)
			{
				room.Value.name = room.Key;
			}
			temperatures = YamlIO<Temperatures>.LoadFile(GetPath() + "temperatures.yaml", null);
			defaults = YamlIO<DefaultSettings>.LoadFile(GetPath() + "defaults.yaml", null);
			mobs = YamlIO<MobSettings>.LoadFile(GetPath() + "mobs.yaml", null);
			foreach (KeyValuePair<string, Mob> item2 in mobs.MobLookupTable)
			{
				item2.Value.name = item2.Key;
			}
			foreach (KeyValuePair<string, ElementBandConfiguration> biomeBackgroundElementBandConfiguration in biomes.BiomeBackgroundElementBandConfigurations)
			{
				biomeBackgroundElementBandConfiguration.Value.ConvertBandSizeToMaxSize();
			}
			return true;
		}
	}
}
