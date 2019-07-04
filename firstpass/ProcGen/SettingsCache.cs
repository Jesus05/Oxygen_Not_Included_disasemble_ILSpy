using Klei;
using ObjectCloner;
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

		private static Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>();

		public static Dictionary<string, SubWorld> subworlds = new Dictionary<string, SubWorld>();

		private static string path = null;

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private const string LAYERS_FILE = "layers";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string BORDERS_FILE = "borders";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		private const string TRAITS_PATH = "traits";

		public static LevelLayerSettings layers
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, River> rivers
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, Room> rooms
		{
			get;
			private set;
		}

		public static ComposableDictionary<Temperature.Range, Temperature> temperatures
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, List<WeightedSimHash>> borders
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
				path = FileSystem.Normalize(System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/"));
			}
			return path;
		}

		public static void CloneInToNewWorld(MutatedWorldData worldData)
		{
			worldData.subworlds = SerializingCloner.Copy(subworlds);
			worldData.features = SerializingCloner.Copy(featuresettings);
			worldData.biomes = SerializingCloner.Copy(biomes);
			worldData.mobs = SerializingCloner.Copy(mobs);
		}

		public static List<string> GetCachedFeatureNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureSettings> featuresetting in featuresettings)
			{
				list.Add(featuresetting.Key);
			}
			return list;
		}

		public static FeatureSettings GetCachedFeature(string name)
		{
			if (!featuresettings.ContainsKey(name))
			{
				throw new Exception("Couldnt get feature from cache [" + name + "]");
			}
			return featuresettings[name];
		}

		public static List<string> GetCachedTraitNames()
		{
			return new List<string>(traits.Keys);
		}

		public static WorldTrait GetCachedTrait(string name)
		{
			if (!traits.ContainsKey(name))
			{
				throw new Exception("Couldnt get trait [" + name + "]");
			}
			return traits[name];
		}

		public static SubWorld GetCachedSubWorld(string name)
		{
			if (!subworlds.ContainsKey(name))
			{
				throw new Exception("Couldnt get subworld [" + name + "]");
			}
			return subworlds[name];
		}

		private static bool GetPathAndName(string srcPath, string srcName, out string name)
		{
			if (!FileSystem.FileExists(srcPath + srcName + ".yaml"))
			{
				string[] array = srcName.Split('/');
				name = array[0];
				for (int i = 1; i < array.Length - 1; i++)
				{
					name = name + "/" + array[i];
				}
				if (!FileSystem.FileExists(srcPath + name + ".yaml"))
				{
					name = srcName;
					return false;
				}
				return true;
			}
			name = srcName;
			return true;
		}

		private static void LoadBiome(string longName, List<YamlIO.Error> errors)
		{
			string name = "";
			if (GetPathAndName(GetPath(), longName, out name) && !biomeSettingsCache.ContainsKey(name))
			{
				BiomeSettings biomeSettings = MergeLoad<BiomeSettings>(GetPath() + name + ".yaml", errors);
				if (biomeSettings == null)
				{
					Debug.LogWarning("WorldGen: Attempting to load biome: " + name + " failed");
				}
				else
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
			}
		}

		private static string LoadFeature(string longName, List<YamlIO.Error> errors)
		{
			string name = "";
			if (GetPathAndName(GetPath(), longName, out name))
			{
				if (!featuresettings.ContainsKey(name))
				{
					FeatureSettings featureSettings = YamlIO.LoadFile<FeatureSettings>(GetPath() + name + ".yaml", null, null);
					if (featureSettings != null)
					{
						featuresettings.Add(name, featureSettings);
						if (featureSettings.forceBiome != null)
						{
							LoadBiome(featureSettings.forceBiome, errors);
							DebugUtil.Assert(biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(featureSettings.forceBiome), longName, "(feature) referenced a missing biome named", featureSettings.forceBiome);
						}
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load feature: " + name + " failed");
					}
				}
				return name;
			}
			Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + name + " failed");
			return longName;
		}

		public static void LoadFeatures(Dictionary<string, int> features, List<YamlIO.Error> errors)
		{
			foreach (KeyValuePair<string, int> feature in features)
			{
				LoadFeature(feature.Key, errors);
			}
		}

		public static void LoadSubworlds(List<WeightedName> subworlds, List<YamlIO.Error> errors)
		{
			foreach (WeightedName subworld in subworlds)
			{
				SubWorld subWorld = null;
				string text = subworld.name;
				if (subworld.overrideName != null && subworld.overrideName.Length > 0)
				{
					text = subworld.overrideName;
				}
				if (!SettingsCache.subworlds.ContainsKey(text))
				{
					SubWorld subWorld2 = YamlIO.LoadFile<SubWorld>(path + subworld.name + ".yaml", null, null);
					if (subWorld2 != null)
					{
						subWorld = subWorld2;
						subWorld.name = text;
						SettingsCache.subworlds[text] = subWorld;
						noise.LoadTree(subWorld.biomeNoise, path);
						noise.LoadTree(subWorld.densityNoise, path);
						noise.LoadTree(subWorld.overrideNoise, path);
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load subworld: " + subworld.name + " failed");
					}
					if (subWorld.centralFeature != null)
					{
						subWorld.centralFeature.type = LoadFeature(subWorld.centralFeature.type, errors);
					}
					foreach (WeightedBiome biome in subWorld.biomes)
					{
						LoadBiome(biome.name, errors);
						DebugUtil.Assert(biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(biome.name), subWorld.name, "(subworld) referenced a missing biome named", biome.name);
					}
					DebugUtil.Assert(subWorld.features != null, "Features list for subworld", subWorld.name, "was null! Either remove it from the .yaml or set it to the empty list []");
					foreach (Feature feature in subWorld.features)
					{
						feature.type = LoadFeature(feature.type, errors);
					}
				}
			}
		}

		public static List<string> GetWorldNames()
		{
			return worlds.GetNames();
		}

		public static void Save(string path)
		{
			YamlIO.Save(layers, path + "layers.yaml", null);
			YamlIO.Save(rivers, path + "rivers.yaml", null);
			YamlIO.Save(rooms, path + "rooms.yaml", null);
			YamlIO.Save(temperatures, path + "temperatures.yaml", null);
			YamlIO.Save(borders, path + "borders.yaml", null);
			YamlIO.Save(defaults, path + "defaults.yaml", null);
			YamlIO.Save(mobs, path + "mobs.yaml", null);
		}

		public static void Clear()
		{
			worlds.worldCache.Clear();
			layers = null;
			biomes.BiomeBackgroundElementBandConfigurations.Clear();
			biomeSettingsCache.Clear();
			rivers = null;
			rooms = null;
			temperatures = null;
			borders = null;
			noise.Clear();
			defaults = null;
			mobs = null;
			featuresettings.Clear();
			traits.Clear();
			subworlds.Clear();
			DebugUtil.LogArgs("World Settings cleared!");
		}

		private static T MergeLoad<T>(string filename, List<YamlIO.Error> errors) where T : class, IMerge<T>, new()
		{
			ListPool<FileHandle, WorldGenSettings>.PooledList pooledList = ListPool<FileHandle, WorldGenSettings>.Allocate();
			FileSystem.GetFiles(filename, pooledList);
			if (((List<FileHandle>)pooledList).Count == 0)
			{
				pooledList.Recycle();
				throw new Exception($"File not found in any file system: {filename}");
			}
			((List<FileHandle>)pooledList).Reverse();
			ListPool<T, WorldGenSettings>.PooledList pooledList2 = ListPool<T, WorldGenSettings>.Allocate();
			((List<T>)pooledList2).Add(new T());
			foreach (FileHandle item in (List<FileHandle>)pooledList)
			{
				FileHandle file = item;
				T val = YamlIO.Parse<T>(FileSystem.ConvertToText(file.source.ReadBytes(file.full_path)), file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					error.file = file;
					errors.Add(error);
				}, null);
				if (val != null)
				{
					((List<T>)pooledList2).Add(val);
				}
			}
			pooledList.Recycle();
			T result = ((List<T>)pooledList2)[0];
			for (int i = 1; i != ((List<T>)pooledList2).Count; i++)
			{
				((IMerge<T>)result).Merge(((List<T>)pooledList2)[i]);
			}
			pooledList2.Recycle();
			return result;
		}

		private static int FirstUncommonCharacter(string a, string b)
		{
			int num = Mathf.Min(a.Length, b.Length);
			int num2 = -1;
			while (++num2 < num)
			{
				if (a[num2] != b[num2])
				{
					return num2;
				}
			}
			return num2;
		}

		public static bool LoadFiles(List<YamlIO.Error> errors)
		{
			if (worlds.worldCache.Count <= 0)
			{
				worlds.LoadFiles(GetPath(), errors);
				List<FileHandle> list = new List<FileHandle>();
				FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "traits")), "*.yaml", list);
				foreach (FileHandle item in list)
				{
					FileHandle trait_file = item;
					WorldTrait worldTrait = YamlIO.LoadFile<WorldTrait>(trait_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						error.file = trait_file;
						errors.Add(error);
					}, null);
					int num = FirstUncommonCharacter(path, trait_file.full_path);
					string text = (num <= -1) ? trait_file.full_path : trait_file.full_path.Substring(num);
					text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(text), System.IO.Path.GetFileNameWithoutExtension(text));
					if (worldTrait == null)
					{
						DebugUtil.LogWarningArgs("Failed to load trait: ", text);
					}
					else
					{
						DebugUtil.LogArgs("Adding a world trait:", text);
						traits[text] = worldTrait;
					}
				}
				foreach (KeyValuePair<string, World> item2 in worlds.worldCache)
				{
					LoadFeatures(item2.Value.globalFeatures, errors);
					LoadSubworlds(item2.Value.subworldFiles, errors);
				}
				foreach (KeyValuePair<string, WorldTrait> trait in traits)
				{
					LoadFeatures(trait.Value.globalFeatureMods, errors);
					LoadSubworlds(trait.Value.additionalSubworldFiles, errors);
				}
				layers = MergeLoad<LevelLayerSettings>(GetPath() + "layers.yaml", errors);
				layers.LevelLayers.ConvertBandSizeToMaxSize();
				rivers = MergeLoad<ComposableDictionary<string, River>>(GetPath() + "rivers.yaml", errors);
				rooms = MergeLoad<ComposableDictionary<string, Room>>(path + "rooms.yaml", errors);
				foreach (KeyValuePair<string, Room> room in rooms)
				{
					room.Value.name = room.Key;
				}
				temperatures = MergeLoad<ComposableDictionary<Temperature.Range, Temperature>>(GetPath() + "temperatures.yaml", errors);
				borders = MergeLoad<ComposableDictionary<string, List<WeightedSimHash>>>(GetPath() + "borders.yaml", errors);
				defaults = YamlIO.LoadFile<DefaultSettings>(GetPath() + "defaults.yaml", null, null);
				mobs = MergeLoad<MobSettings>(GetPath() + "mobs.yaml", errors);
				foreach (KeyValuePair<string, Mob> item3 in mobs.MobLookupTable)
				{
					item3.Value.name = item3.Key;
				}
				DebugUtil.LogArgs("World settings reload complete!");
				return true;
			}
			return false;
		}

		public static List<string> GetRandomTraits(int seed)
		{
			System.Random random = new System.Random(seed);
			int num = random.Next(2, 5);
			List<string> list = new List<string>(traits.Keys);
			List<string> list2 = new List<string>();
			while (list2.Count < num && list.Count > 0)
			{
				int index = random.Next(list.Count);
				string item = list[index];
				list2.Add(item);
				list.RemoveAt(index);
			}
			DebugUtil.LogArgs("Getting traits for seed", seed, string.Join(", ", list2.ToArray()));
			return list2;
		}
	}
}
