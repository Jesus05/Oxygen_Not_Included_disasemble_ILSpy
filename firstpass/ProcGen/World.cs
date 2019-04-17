using Klei;
using ProcGen.Noise;
using System.Collections.Generic;

namespace ProcGen
{
	public class World : YamlIO<World>
	{
		public enum LayoutMethod
		{
			Default = 0,
			VoronoiTree = 0,
			PowerTree = 1
		}

		public class AllowedCellsFilter
		{
			public enum TagCommand
			{
				Default,
				ContainsOne,
				ContainsAll,
				ContainsNone,
				DistanceFrom
			}

			public enum Command
			{
				Clear,
				Replace,
				UnionWith,
				IntersectWith,
				ExceptWith,
				SymmetricExceptWith
			}

			public TagCommand tagcommand
			{
				get;
				private set;
			}

			public string tagset
			{
				get;
				private set;
			}

			public int distance
			{
				get;
				private set;
			}

			public int maxDistance
			{
				get;
				private set;
			}

			public int distCmp
			{
				get;
				private set;
			}

			public Command command
			{
				get;
				private set;
			}

			public List<Temperature.Range> temperatureRanges
			{
				get;
				private set;
			}

			public List<SubWorld.ZoneType> zoneTypes
			{
				get;
				private set;
			}

			public List<string> subworldNames
			{
				get;
				private set;
			}

			public AllowedCellsFilter()
			{
				temperatureRanges = new List<Temperature.Range>();
				zoneTypes = new List<SubWorld.ZoneType>();
				subworldNames = new List<string>();
			}
		}

		public Dictionary<string, SubWorld> Zones;

		public string name
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public bool show
		{
			get;
			private set;
		}

		public Vector2I worldsize
		{
			get;
			private set;
		}

		public DefaultSettings defaultsOverrides
		{
			get;
			private set;
		}

		public LayoutMethod layoutMethod
		{
			get;
			private set;
		}

		public List<WeightedName> ZoneFiles
		{
			get;
			private set;
		}

		public Dictionary<string, List<string>> DefineTagSet
		{
			get;
			private set;
		}

		public List<AllowedCellsFilter> UnknownCellsAllowedSubworlds
		{
			get;
			private set;
		}

		public World()
		{
			Zones = new Dictionary<string, SubWorld>();
			ZoneFiles = new List<WeightedName>();
			DefineTagSet = new Dictionary<string, List<string>>();
			UnknownCellsAllowedSubworlds = new List<AllowedCellsFilter>();
		}

		public SubWorld GetSubWorld(string name)
		{
			if (Zones.ContainsKey(name))
			{
				return Zones[name];
			}
			return null;
		}

		public void LoadZones(NoiseTreeFiles noise, string path)
		{
			foreach (WeightedName zoneFile in ZoneFiles)
			{
				SubWorld subWorld = null;
				string text = WorldGenSettings.GetSimpleName(zoneFile.name);
				if (zoneFile.overrideName != null && zoneFile.overrideName.Length > 0)
				{
					text = zoneFile.overrideName;
				}
				if (!Zones.ContainsKey(text))
				{
					SubWorldFile subWorldFile = YamlIO<SubWorldFile>.LoadFile(path + zoneFile.name + ".yaml", null);
					if (subWorldFile != null)
					{
						subWorld = subWorldFile.zone;
						subWorld.name = text;
						subWorld.pdWeight = zoneFile.weight;
						Zones[text] = subWorld;
						noise.LoadTree(subWorld.biomeNoise, path);
						noise.LoadTree(subWorld.densityNoise, path);
						noise.LoadTree(subWorld.overrideNoise, path);
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load zone: " + zoneFile.name + " failed");
					}
				}
			}
		}
	}
}
