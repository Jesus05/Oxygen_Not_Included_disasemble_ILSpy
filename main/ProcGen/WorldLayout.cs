using Delaunay.Geo;
using KSerialization;
using ProcGen.Map;
using ProcGenGame;
using Satsuma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VoronoiTree;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class WorldLayout
	{
		[Flags]
		public enum DebugFlags
		{
			LocalGraph = 0x1,
			OverworldGraph = 0x2,
			VoronoiTree = 0x4
		}

		[SerializationConfig(MemberSerialization.OptOut)]
		private class ExtraIO
		{
			public List<Leaf> leafs = new List<Leaf>();

			public List<Tree> internals = new List<Tree>();

			public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();

			public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();

			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				leafs = new List<Leaf>();
				internals = new List<Tree>();
				leafInternalParent = new List<KeyValuePair<int, int>>();
				internalInternalParent = new List<KeyValuePair<int, int>>();
			}
		}

		private Tree voronoiTree;

		[Serialize]
		public MapGraph localGraph;

		[Serialize]
		public MapGraph overworldGraph;

		[EnumFlags]
		public static DebugFlags drawOptions;

		private LineSegment topEdge;

		private LineSegment bottomEdge;

		private LineSegment leftEdge;

		private LineSegment rightEdge;

		private SeededRandom myRandom;

		private WorldGen worldGen;

		[Serialize]
		private ExtraIO extra;

		[Serialize]
		public int mapWidth
		{
			get;
			private set;
		}

		[Serialize]
		public int mapHeight
		{
			get;
			private set;
		}

		public bool layoutOK
		{
			get;
			private set;
		}

		public static LevelLayer levelLayerGradient
		{
			get;
			private set;
		}

		public WorldLayout(WorldGen worldGen, int seed)
		{
			this.worldGen = worldGen;
			localGraph = new MapGraph(seed);
			overworldGraph = new MapGraph(seed);
			SetSeed(seed);
		}

		public WorldLayout(WorldGen worldGen, int width, int height, int seed)
			: this(worldGen, seed)
		{
			mapWidth = width;
			mapHeight = height;
		}

		public void SetSeed(int seed)
		{
			myRandom = new SeededRandom(seed);
			localGraph.SetSeed(seed);
			overworldGraph.SetSeed(seed);
		}

		public Tree GetVoronoiTree()
		{
			return voronoiTree;
		}

		public static void SetLayerGradient(LevelLayer newGradient)
		{
			levelLayerGradient = newGradient;
		}

		public static string GetNodeTypeFromLayers(Vector2 point, float mapHeight, SeededRandom rnd)
		{
			string name = WorldGenTags.TheVoid.Name;
			int index = rnd.RandomRange(0, levelLayerGradient[levelLayerGradient.Count - 1].content.Count);
			name = levelLayerGradient[levelLayerGradient.Count - 1].content[index];
			for (int i = 0; i < levelLayerGradient.Count; i++)
			{
				if (point.y < levelLayerGradient[i].maxValue * mapHeight)
				{
					int index2 = rnd.RandomRange(0, levelLayerGradient[i].content.Count);
					name = levelLayerGradient[i].content[index2];
					break;
				}
			}
			return name;
		}

		public Tree GenerateOverworld(bool usePD)
		{
			Diagram.Site site = new Diagram.Site(0u, new Vector2((float)(mapWidth / 2), (float)(mapHeight / 2)), 1f);
			topEdge = new LineSegment(new Vector2(0f, (float)(mapHeight - 5)), new Vector2((float)mapWidth, (float)(mapHeight - 5)));
			bottomEdge = new LineSegment(new Vector2(0f, 5f), new Vector2((float)mapWidth, 5f));
			leftEdge = new LineSegment(new Vector2(5f, 0f), new Vector2(5f, (float)mapHeight));
			rightEdge = new LineSegment(new Vector2((float)(mapWidth - 5), 0f), new Vector2((float)(mapWidth - 5), (float)mapHeight));
			site.poly = new Polygon(new Rect(0f, 0f, (float)mapWidth, (float)mapHeight));
			voronoiTree = new Tree(site, null, myRandom.seed);
			VoronoiTree.Node.maxIndex = 0u;
			float floatSetting = worldGen.Settings.GetFloatSetting("OverworldDensityMin");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("OverworldDensityMax");
			float density = myRandom.RandomRange(floatSetting, floatSetting2);
			float floatSetting3 = worldGen.Settings.GetFloatSetting("OverworldAvoidRadius");
			PointGenerator.SampleBehaviour enumSetting = worldGen.Settings.GetEnumSetting<PointGenerator.SampleBehaviour>("OverworldSampleBehaviour");
			Node node = overworldGraph.AddNode(WorldGenTags.StartWorld.Name);
			node.SetPosition(new Vector2((float)(mapWidth / 2), (float)(mapHeight / 2)));
			List<Vector2> list = new List<Vector2>();
			list.Add(node.position);
			VoronoiTree.Node node2 = voronoiTree.AddSite(new Diagram.Site((uint)node.node.Id, node.position, 1f), VoronoiTree.Node.NodeType.Internal);
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, density, floatSetting3, list, enumSetting, false, myRandom, false, true);
			int intSetting = worldGen.Settings.GetIntSetting("OverworldMaxNodes");
			if (randomPoints.Count > intSetting)
			{
				randomPoints.ShuffleSeeded(myRandom.RandomSource());
				randomPoints.RemoveRange(intSetting, randomPoints.Count - intSetting);
			}
			for (int i = 0; i < randomPoints.Count; i++)
			{
				Node node3 = overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name);
				node3.SetPosition(randomPoints[i]);
				VoronoiTree.Node node4 = voronoiTree.AddSite(new Diagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiTree.Node.NodeType.Internal);
				node4.tags.Add(WorldGenTags.UnassignedNode);
				node3.tags.Add(WorldGenTags.UnassignedNode);
			}
			if (usePD)
			{
				List<Diagram.Site> list2 = new List<Diagram.Site>();
				for (int j = 0; j < voronoiTree.ChildCount(); j++)
				{
					list2.Add(voronoiTree.GetChild(j).site);
				}
				voronoiTree.ComputeNode(list2);
				voronoiTree.ComputeNodePD(list2, 500, 0.2f);
			}
			else
			{
				voronoiTree.ComputeChildren(myRandom.seed + 1, false, false);
			}
			voronoiTree.AddTagToChildren(WorldGenTags.Overworld);
			node2.AddTag(WorldGenTags.StartWorld);
			node2.AddTagToNeighbors(WorldGenTags.StartNear);
			List<VoronoiTree.Node> siblings = node2.GetSiblings();
			List<VoronoiTree.Node> neighbors = node2.GetNeighbors();
			for (int k = 0; k < neighbors.Count; k++)
			{
				VoronoiTree.Node node5 = neighbors[k];
				if (siblings.Contains(node5))
				{
					siblings.Remove(node5);
				}
				List<VoronoiTree.Node> neighbors2 = node5.GetNeighbors();
				for (int l = 0; l < neighbors2.Count; l++)
				{
					VoronoiTree.Node node6 = neighbors2[l];
					if (siblings.Contains(node6))
					{
						siblings.Remove(node6);
					}
					if (!node6.tags.Contains(WorldGenTags.StartNear) && !node6.tags.Contains(WorldGenTags.StartWorld))
					{
						node6.AddTag(WorldGenTags.StartMedium);
					}
				}
			}
			for (int m = 0; m < siblings.Count; m++)
			{
				VoronoiTree.Node node7 = siblings[m];
				if (!node7.tags.Contains(WorldGenTags.StartNear) && !node7.tags.Contains(WorldGenTags.StartWorld) && !node7.tags.Contains(WorldGenTags.StartMedium))
				{
					node7.AddTag(WorldGenTags.StartFar);
				}
			}
			int intSetting2 = worldGen.Settings.GetIntSetting("OverworldRelaxIterations");
			float floatSetting4 = worldGen.Settings.GetFloatSetting("OverworldRelaxEnergyMin");
			voronoiTree.RelaxRecursive(0, intSetting2, floatSetting4, usePD);
			TagTopAndBottomSites(WorldGenTags.NearSurface, WorldGenTags.NearDepths);
			TagEdgeSites(WorldGenTags.NearEdge, WorldGenTags.NearEdge);
			for (int n = 0; n < voronoiTree.ChildCount(); n++)
			{
				VoronoiTree.Node child = voronoiTree.GetChild(n);
				Node node8 = overworldGraph.FindNodeByID(child.site.id);
				node8.tags.Union(child.tags);
				node8.SetPosition(child.site.position);
				List<VoronoiTree.Node> neighbors3 = child.GetNeighbors();
				for (int num = 0; num < neighbors3.Count; num++)
				{
					Node nodeB = overworldGraph.FindNodeByID(neighbors3[num].site.id);
					overworldGraph.AddArc(node8, nodeB, "Neighbor");
				}
			}
			PropegateOverworldTags(voronoiTree, WorldGenTags.DistanceTags);
			ConvertUnknownCells();
			if (worldGen.Settings.GetOverworldAddTags() != null)
			{
				foreach (string overworldAddTag in worldGen.Settings.GetOverworldAddTags())
				{
					int childIndex = myRandom.RandomSource().Next(voronoiTree.ChildCount());
					VoronoiTree.Node child2 = voronoiTree.GetChild(childIndex);
					child2.AddTag(new Tag(overworldAddTag));
				}
			}
			FlatternOverworld();
			return voronoiTree;
		}

		public void PopulateSubworlds()
		{
			AddSubworldChildren();
			GetStartLocation();
		}

		private void PropegateOverworldTags(Tree tree, TagSet tags)
		{
			foreach (Tag tag in tags)
			{
				Dictionary<uint, int> distanceToTag = overworldGraph.GetDistanceToTag(tag);
				if (distanceToTag != null)
				{
					for (int i = 0; i < tree.ChildCount(); i++)
					{
						VoronoiTree.Node child = tree.GetChild(i);
						uint id = child.site.id;
						if (distanceToTag.ContainsKey(id) && distanceToTag[id] > 0)
						{
							child.minDistaceToTag.Add(tag, distanceToTag[id]);
						}
					}
				}
			}
		}

		private char ConvertSignToCmp(int val)
		{
			if (val > 0)
			{
				return '>';
			}
			if (val < 0)
			{
				return '<';
			}
			return '=';
		}

		private HashSet<SubWorld> GetDistanceFilterSet(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, List<SubWorld> subworlds)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			Node node = overworldGraph.FindNodeByID(vn.site.id);
			int distanceToTagSetFromNode = overworldGraph.GetDistanceToTagSetFromNode(node, tagsets[filter.tagset]);
			if (distanceToTagSetFromNode >= 0)
			{
				int num = distanceToTagSetFromNode.CompareTo(filter.distance);
				if (num == filter.distCmp && distanceToTagSetFromNode < filter.maxDistance)
				{
					int i;
					for (i = 0; i < filter.subworldNames.Count; i++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[i]));
					}
				}
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetNameFilterSet(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, List<SubWorld> subworlds)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				int i;
				for (i = 0; i < filter.subworldNames.Count; i++)
				{
					hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[i]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					int j;
					for (j = 0; j < filter.subworldNames.Count; j++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[j]));
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					int k;
					for (k = 0; k < filter.subworldNames.Count; k++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[k]));
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					int l;
					for (l = 0; l < filter.subworldNames.Count; l++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[l]));
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetZoneTypeFilterSet(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, Dictionary<string, List<SubWorld>> subworldsByZoneType)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				for (int k = 0; k < filter.zoneTypes.Count; k++)
				{
					hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[k].ToString()]);
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int j = 0; j < filter.zoneTypes.Count; j++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					for (int l = 0; l < filter.zoneTypes.Count; l++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[l].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int i = 0; i < filter.zoneTypes.Count; i++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[i].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetTemperatureFilterSet(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, Dictionary<string, List<SubWorld>> subworldsByTemperature)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				for (int k = 0; k < filter.temperatureRanges.Count; k++)
				{
					hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[k].ToString()]);
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int j = 0; j < filter.temperatureRanges.Count; j++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					for (int l = 0; l < filter.temperatureRanges.Count; l++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[l].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int i = 0; i < filter.temperatureRanges.Count; i++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[i].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private void RunFilterClearCommand(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, HashSet<SubWorld> allowedSubworldsSet)
		{
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				allowedSubworldsSet.Clear();
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			}
		}

		private HashSet<SubWorld> Filter(VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, List<SubWorld> allSubWorlds, Dictionary<string, List<SubWorld>> subworldsByTemperature, Dictionary<string, List<SubWorld>> subworldsByZoneType)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			World world = worldGen.Settings.world;
			foreach (World.AllowedCellsFilter unknownCellsAllowedSubworld in world.UnknownCellsAllowedSubworlds)
			{
				HashSet<SubWorld> hashSet2 = new HashSet<SubWorld>();
				if (unknownCellsAllowedSubworld.subworldNames != null && unknownCellsAllowedSubworld.subworldNames.Count > 0)
				{
					hashSet2.UnionWith(GetNameFilterSet(vn, tagsets, unknownCellsAllowedSubworld, allSubWorlds));
				}
				if (unknownCellsAllowedSubworld.temperatureRanges != null && unknownCellsAllowedSubworld.temperatureRanges.Count > 0)
				{
					hashSet2.UnionWith(GetTemperatureFilterSet(vn, tagsets, unknownCellsAllowedSubworld, subworldsByTemperature));
				}
				if (unknownCellsAllowedSubworld.zoneTypes != null && unknownCellsAllowedSubworld.zoneTypes.Count > 0)
				{
					hashSet2.UnionWith(GetZoneTypeFilterSet(vn, tagsets, unknownCellsAllowedSubworld, subworldsByZoneType));
				}
				if (unknownCellsAllowedSubworld.tagcommand == World.AllowedCellsFilter.TagCommand.DistanceFrom)
				{
					hashSet2.UnionWith(GetDistanceFilterSet(vn, tagsets, unknownCellsAllowedSubworld, allSubWorlds));
				}
				switch (unknownCellsAllowedSubworld.command)
				{
				case World.AllowedCellsFilter.Command.Clear:
					RunFilterClearCommand(vn, tagsets, unknownCellsAllowedSubworld, hashSet);
					break;
				case World.AllowedCellsFilter.Command.Replace:
					if (hashSet2.Count > 0)
					{
						hashSet.Clear();
						hashSet.UnionWith(hashSet2);
					}
					break;
				case World.AllowedCellsFilter.Command.UnionWith:
					hashSet.UnionWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.ExceptWith:
					hashSet.ExceptWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.IntersectWith:
					hashSet.IntersectWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.SymmetricExceptWith:
					hashSet.SymmetricExceptWith(hashSet2);
					break;
				}
			}
			return hashSet;
		}

		private void ConvertUnknownCells()
		{
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithTag(WorldGenTags.UnassignedNode, list);
			List<SubWorld> subWorldList = worldGen.Settings.GetSubWorldList();
			subWorldList.Remove(worldGen.Settings.GetSubWorld(WorldGenTags.StartWorld.Name));
			Dictionary<string, List<SubWorld>> dictionary = new Dictionary<string, List<SubWorld>>();
			IEnumerator enumerator = Enum.GetValues(typeof(Temperature.Range)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Temperature.Range range = (Temperature.Range)enumerator.Current;
					dictionary.Add(range.ToString(), subWorldList.FindAll((SubWorld sw) => sw.temperatureRange == range));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Dictionary<string, List<SubWorld>> dictionary2 = new Dictionary<string, List<SubWorld>>();
			IEnumerator enumerator2 = Enum.GetValues(typeof(SubWorld.ZoneType)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					SubWorld.ZoneType zt = (SubWorld.ZoneType)enumerator2.Current;
					dictionary2.Add(zt.ToString(), subWorldList.FindAll((SubWorld sw) => sw.zoneType == zt));
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			Dictionary<string, TagSet> dictionary3 = new Dictionary<string, TagSet>();
			World world = worldGen.Settings.world;
			foreach (KeyValuePair<string, List<string>> item in world.DefineTagSet)
			{
				dictionary3.Add(item.Key, new TagSet(item.Value));
			}
			foreach (World.AllowedCellsFilter unknownCellsAllowedSubworld in world.UnknownCellsAllowedSubworlds)
			{
				if (unknownCellsAllowedSubworld.tagset != null && !dictionary3.ContainsKey(unknownCellsAllowedSubworld.tagset))
				{
					dictionary3.Add(unknownCellsAllowedSubworld.tagset, new TagSet(unknownCellsAllowedSubworld.tagset));
				}
			}
			foreach (VoronoiTree.Node item2 in list)
			{
				Node node = overworldGraph.FindNodeByID(item2.site.id);
				item2.tags.Remove(WorldGenTags.UnassignedNode);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				HashSet<SubWorld> collection = Filter(item2, dictionary3, subWorldList, dictionary, dictionary2);
				List<SubWorld> list2 = new List<SubWorld>(collection);
				list2.ShuffleSeeded(myRandom.RandomSource());
				string type = "NONE";
				foreach (KeyValuePair<string, SubWorld> subWorld in worldGen.Settings.GetSubWorlds())
				{
					if (list2.Count > 0)
					{
						if (subWorld.Value == list2[0])
						{
							type = subWorld.Key;
							break;
						}
					}
					else
					{
						Debug.LogWarning("No allowed Subworld types. Using default.", null);
						type = "Default";
					}
				}
				node.SetType(type);
				if (list2.Count > 0)
				{
					foreach (string tag in list2[0].tags)
					{
						item2.AddTag(new Tag(tag));
					}
				}
			}
		}

		public void ComputeSubWorlds(List<TemplateContainer> poi)
		{
			try
			{
				SplitTopAndBottomSites();
				SplitLargeStartingSites();
				PropagateStartTag();
				SetTemperatureTags();
				TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
				TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Output.LogError("ex: " + message + " " + stackTrace);
			}
		}

		private void FlatternOverworld()
		{
			try
			{
				for (int i = 0; i < voronoiTree.ChildCount(); i++)
				{
					VoronoiTree.Node child = voronoiTree.GetChild(i);
					if (child.type == VoronoiTree.Node.NodeType.Internal)
					{
						Tree tree = child as Tree;
						Node node = overworldGraph.FindNodeByID(tree.site.id);
						node.tags.Union(tree.tags);
						Cell cell = overworldGraph.GetCell(node.position, node.node, true);
						cell.tags.Union(tree.tags);
					}
				}
				for (int j = 0; j < voronoiTree.ChildCount(); j++)
				{
					VoronoiTree.Node child2 = voronoiTree.GetChild(j);
					if (child2.type == VoronoiTree.Node.NodeType.Internal)
					{
						Tree tree2 = child2 as Tree;
						List<KeyValuePair<VoronoiTree.Node, LineSegment>> neighborsByEdge = tree2.GetNeighborsByEdge();
						for (int k = 0; k < neighborsByEdge.Count; k++)
						{
							KeyValuePair<VoronoiTree.Node, LineSegment> keyValuePair = neighborsByEdge[k];
							MapGraph mapGraph = overworldGraph;
							Vector2? p = keyValuePair.Value.p0;
							mapGraph.GetCorner(p.Value, true);
							MapGraph mapGraph2 = overworldGraph;
							Vector2? p2 = keyValuePair.Value.p1;
							mapGraph2.GetCorner(p2.Value, true);
						}
					}
				}
				TagSet tagSet = new TagSet();
				tagSet.Add(WorldGenTags.NearDepths);
				for (int l = 0; l < voronoiTree.ChildCount(); l++)
				{
					VoronoiTree.Node child3 = voronoiTree.GetChild(l);
					if (child3.type == VoronoiTree.Node.NodeType.Internal)
					{
						Tree tree3 = child3 as Tree;
						Node node2 = overworldGraph.FindNodeByID(tree3.site.id);
						Cell cell2 = overworldGraph.GetCell(node2.node);
						List<KeyValuePair<VoronoiTree.Node, LineSegment>> neighborsByEdge2 = tree3.GetNeighborsByEdge();
						for (int m = 0; m < neighborsByEdge2.Count; m++)
						{
							KeyValuePair<VoronoiTree.Node, LineSegment> keyValuePair2 = neighborsByEdge2[m];
							MapGraph mapGraph3 = overworldGraph;
							Vector2? p3 = keyValuePair2.Value.p0;
							Corner corner = mapGraph3.GetCorner(p3.Value, false);
							MapGraph mapGraph4 = overworldGraph;
							Vector2? p4 = keyValuePair2.Value.p1;
							Corner corner2 = mapGraph4.GetCorner(p4.Value, false);
							Edge edge = null;
							VoronoiTree.Node key = keyValuePair2.Key;
							if (key != null)
							{
								Node node3 = overworldGraph.FindNodeByID(key.site.id);
								Cell cell3 = overworldGraph.GetCell(node3.node);
								edge = overworldGraph.GetEdge(corner, corner2, cell2, cell3, true);
								SubWorld subWorld = worldGen.Settings.GetSubWorld(node2.type);
								SubWorld subWorld2 = worldGen.Settings.GetSubWorld(node3.type);
								if (node2.type == node3.type || subWorld.zoneType == subWorld2.zoneType || (subWorld.zoneType == SubWorld.ZoneType.Space && subWorld2.zoneType == SubWorld.ZoneType.Space) || (cell2.tags.ContainsOne(tagSet) && cell3.tags.ContainsOne(tagSet)))
								{
									edge.tags.Add(WorldGenTags.EdgeOpen);
								}
								else
								{
									edge.tags.Add(WorldGenTags.EdgeClosed);
								}
								cell3.Add(edge);
							}
							else
							{
								edge = overworldGraph.GetEdge(corner, corner2, cell2, cell2, true);
								edge.tags.Add(WorldGenTags.EdgeUnpassable);
							}
							cell2.Add(edge);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Output.LogError("ex: " + message + " " + stackTrace);
			}
			UpdateEdgesAroundStart();
		}

		private void UpdateEdgesAroundStart()
		{
			Node node = overworldGraph.FindNode((Node n) => n.type == WorldGenTags.StartWorld.Name);
			Cell cell = overworldGraph.GetCell(node.node);
			foreach (Edge edge in cell.edges)
			{
				edge.tags.Add(WorldGenTags.RoomBorderMixed);
			}
		}

		private void AddSubworldChildren()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet moveTags = new TagSet(worldGen.Settings.GetDefaultMoveTags());
			VoronoiTree.Node.SplitCommand splitCommand = new VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = moveTags;
			splitCommand.SplitFunction = SplitFunction;
			for (int i = 0; i < voronoiTree.ChildCount(); i++)
			{
				VoronoiTree.Node child = voronoiTree.GetChild(i);
				if (child.type == VoronoiTree.Node.NodeType.Internal)
				{
					Tree tree = child as Tree;
					Node node = overworldGraph.FindNodeByID(tree.site.id);
					SubWorld subWorld = worldGen.Settings.GetSubWorld(node.type);
					tree.AddTag(new Tag(node.type));
					tree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					GenerateChildren(subWorld, tree, localGraph, (float)mapHeight, i + myRandom.seed);
					int num = tree.ChildCount();
					if (num < subWorld.minChildCount)
					{
						tree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
						splitCommand.dontCopyTags = tagSet;
						splitCommand.minChildCount = subWorld.minChildCount;
						tree.Split(splitCommand);
						if (subWorld.biomes != null && subWorld.biomes.Count > 0)
						{
							for (int j = num; j < tree.ChildCount(); j++)
							{
								WeightedBiome weightedBiome = WeightedRandom.Choose(subWorld.biomes, myRandom);
								Node node2 = localGraph.FindNodeByID(tree.GetChild(j).site.id);
								node2.SetType(weightedBiome.name);
								tree.GetChild(j).AddTag(new Tag(node2.type));
							}
						}
						else
						{
							for (int k = num; k < tree.ChildCount(); k++)
							{
								Node node3 = localGraph.FindNodeByID(tree.GetChild(k).site.id);
								node3.SetType(GetNodeTypeFromLayers(tree.site.position, (float)mapHeight, myRandom));
								tree.GetChild(k).AddTag(new Tag(node3.type));
							}
						}
					}
					tree.RelaxRecursive(0, 10, 1f, worldGen.Settings.world.layoutMethod == World.LayoutMethod.PowerTree);
					List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
					tree.GetNodesWithTag(WorldGenTags.Feature, list);
					TagSet tagSet2 = new TagSet();
					tagSet2.Add(WorldGenTags.Feature);
					tagSet2.Add(WorldGenTags.SplitOnParentDensity);
					splitCommand.dontCopyTags = tagSet2;
					for (int l = 0; l < list.Count; l++)
					{
						if (!list[l].tags.Contains(WorldGenTags.CenteralFeature))
						{
							if (list[l].tags.Contains(WorldGenTags.SplitOnParentDensity))
							{
								list[l].Split(splitCommand);
							}
							if (list[l].tags.Contains(WorldGenTags.SplitTwice))
							{
								Tree tree2 = list[l].Split(splitCommand);
								if (tree2.ChildCount() <= 1)
								{
									Debug.LogError("split did not work.", null);
								}
								for (int m = 0; m < tree2.ChildCount(); m++)
								{
									VoronoiTree.Node child2 = tree2.GetChild(m);
									child2.Split(splitCommand);
								}
							}
						}
					}
				}
			}
			VoronoiTree.Node.maxDepth = voronoiTree.MaxDepth(0);
		}

		private List<Vector2> GetPoints(string name, LoggerSSF log, int minPointCount, Polygon boundingArea, float density, float avoidRadius, List<Vector2> avoidPoints, PointGenerator.SampleBehaviour sampleBehaviour, bool testInsideBounds, SeededRandom rnd, bool doShuffle = true, bool testAvoidPoints = true)
		{
			List<Vector2> list = null;
			int num = 0;
			do
			{
				list = PointGenerator.GetRandomPoints(boundingArea, density, avoidRadius, avoidPoints, sampleBehaviour, testInsideBounds, rnd, doShuffle, testAvoidPoints);
				if (list.Count < minPointCount)
				{
					density *= 0.8f;
					if (worldGen.isRunningDebugGen)
					{
						Debug.LogWarning("Recaluclating points for " + name + " iter: " + num + " density:" + density, null);
					}
				}
				num++;
			}
			while (list.Count < minPointCount && num < 10);
			return list;
		}

		public void GenerateChildren(SubWorld sw, Tree node, Graph graph, float worldHeight, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			TagSet tagSet = new TagSet(worldGen.Settings.GetDefaultMoveTags());
			TagSet tagSet2 = new TagSet();
			if (tagSet != null)
			{
				for (int i = 0; i < tagSet.Count; i++)
				{
					Tag item = tagSet[i];
					if (node.tags.Contains(item))
					{
						node.tags.Remove(item);
						tagSet2.Add(item);
					}
				}
			}
			TagSet tagSet3 = new TagSet(node.tags);
			tagSet3.Remove(WorldGenTags.Overworld);
			for (int j = 0; j < sw.tags.Count; j++)
			{
				tagSet3.Add(new Tag(sw.tags[j]));
			}
			float randomValueWithinRange = sw.density.GetRandomValueWithinRange(seededRandom);
			Node node2 = sw.AddCenteralFeature(node, graph, tagSet3);
			List<Vector2> list = new List<Vector2>();
			if (node2 != null)
			{
				list.Add(node2.position);
				foreach (WeightedBiome biome in sw.biomes)
				{
					if (biome.name == node2.type)
					{
						TagSet others = new TagSet(biome.tags);
						node2.tags.Union(others);
						break;
					}
				}
			}
			node.dontRelaxChildren = sw.dontRelaxChildren;
			int num = (sw.features.Count <= 0) ? 2 : sw.features.Count;
			List<Vector2> points = GetPoints(sw.name, node.log, num, node.site.poly, randomValueWithinRange, sw.avoidRadius, list, sw.sampleBehaviour, true, seededRandom, true, sw.doAvoidPoints);
			for (int k = 0; k < sw.samplers.Count; k++)
			{
				list.AddRange(points);
				float randomValueWithinRange2 = sw.samplers[k].density.GetRandomValueWithinRange(seededRandom);
				List<Vector2> randomPoints = PointGenerator.GetRandomPoints(node.site.poly, randomValueWithinRange2, sw.samplers[k].avoidRadius, list, sw.samplers[k].sampleBehaviour, true, seededRandom, true, sw.samplers[k].doAvoidPoints);
				points.AddRange(randomPoints);
			}
			if (points.Count > 200)
			{
				points.RemoveRange(200, points.Count - 200);
			}
			if (points.Count < num)
			{
				string arg = string.Empty;
				for (int l = 0; l < node.site.poly.Vertices.Count; l++)
				{
					arg = arg + node.site.poly.Vertices[l] + ", ";
				}
				if (!worldGen.isRunningDebugGen)
				{
					return;
				}
				return;
			}
			int m = 0;
			for (int n = 0; n < sw.features.Count; n++)
			{
				Feature feature = sw.features[n];
				Node node3 = null;
				TerrainFeature terrainFeature = null;
				TagSet tagSet4 = new TagSet(feature.tags.ToArray());
				if (SettingsCache.features.TerrainFeatures.ContainsKey(feature.type) && SettingsCache.features.TerrainFeatures[feature.type] != null)
				{
					terrainFeature = SettingsCache.features.TerrainFeatures[feature.type];
					if (terrainFeature.tags != null)
					{
						tagSet4.Union(new TagSet(terrainFeature.tags.ToArray()));
					}
				}
				if (feature.excludesTags != null && feature.excludesTags.Count > 0)
				{
					tagSet4.Remove(new TagSet(feature.excludesTags.ToArray()));
				}
				tagSet4.Add(new Tag(feature.type));
				tagSet4.Add(WorldGenTags.Feature);
				TagSet tagSet5 = new TagSet();
				if (terrainFeature != null)
				{
					Feature defaultBiome = terrainFeature.defaultBiome;
					if (defaultBiome.tags != null)
					{
						TagSet others2 = new TagSet(defaultBiome.tags);
						tagSet5.Union(others2);
						tagSet4.Union(others2);
					}
					foreach (WeightedBiome biome2 in sw.biomes)
					{
						if (biome2.name == defaultBiome.type)
						{
							tagSet4.Add(new Tag(defaultBiome.type));
							if (biome2.tags != null)
							{
								TagSet others3 = new TagSet(biome2.tags);
								tagSet5.Union(others3);
								tagSet4.Union(others3);
							}
							break;
						}
					}
				}
				if (feature.type.Contains(WorldGenTags.River.Name) && m + 2 < points.Count)
				{
					Node node4 = graph.AddNode(feature.type);
					node4.biomeSpecificTags = new TagSet(tagSet5);
					VoronoiTree.Node node5 = node.AddSite(new Diagram.Site((uint)node4.node.Id, node4.position, 1f), VoronoiTree.Node.NodeType.Internal);
					node5.tags = new TagSet(tagSet4);
					node4.SetPosition(points[m++]);
					Node node6 = graph.AddNode(feature.type);
					node6.biomeSpecificTags = new TagSet(tagSet5);
					VoronoiTree.Node node7 = node.AddSite(new Diagram.Site((uint)node6.node.Id, node6.position, 1f), VoronoiTree.Node.NodeType.Internal);
					node7.tags = new TagSet(tagSet4);
					node6.SetPosition(points[m++]);
					node3 = node4;
					graph.AddArc(node4, node6, feature.type);
				}
				else if (m < points.Count)
				{
					node3 = graph.AddNode(feature.type);
					node3.biomeSpecificTags = new TagSet(tagSet5);
					node3.SetPosition((!(feature.type == WorldGenTags.StartLocation.Name)) ? points[m++] : node.site.poly.Centroid());
					VoronoiTree.Node node8 = node.AddSite(new Diagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiTree.Node.NodeType.Internal);
					node8.tags = new TagSet(tagSet4);
				}
			}
			if (sw.features.Count <= points.Count)
			{
				goto IL_07ea;
			}
			goto IL_07ea;
			IL_07ea:
			for (; m < points.Count; m++)
			{
				string text = null;
				TagSet tagSet6 = null;
				if (sw.biomes.Count > 0)
				{
					WeightedBiome weightedBiome = WeightedRandom.Choose(sw.biomes, seededRandom);
					text = weightedBiome.name;
					if (weightedBiome.tags != null && weightedBiome.tags.Count > 0)
					{
						tagSet6 = new TagSet(weightedBiome.tags);
					}
				}
				else
				{
					text = GetNodeTypeFromLayers(points[m], worldHeight, seededRandom);
				}
				Node node9 = graph.AddNode(text);
				node9.biomeSpecificTags = tagSet6;
				node9.SetPosition(points[m]);
				VoronoiTree.Node node10 = node.AddSite(new Diagram.Site((uint)node9.node.Id, node9.position, 1f), VoronoiTree.Node.NodeType.Internal);
				node10.tags = new TagSet(tagSet3);
				if (tagSet6 != null)
				{
					node10.tags.Union(tagSet6);
				}
				node10.AddTag(new Tag(text));
			}
			node.ComputeChildren(seededRandom.seed + 1, false, false);
			if (node.ChildCount() > 0)
			{
				for (int num5 = 0; num5 < tagSet2.Count; num5++)
				{
					Debug.Log($"Applying Moved Tag {tagSet2[num5].Name} to {node.site.id}", null);
					VoronoiTree.Node child = node.GetChild(seededRandom.RandomSource().Next(node.ChildCount()));
					child.AddTag(tagSet2[num5]);
				}
			}
		}

		private void SplitTopAndBottomSites()
		{
			float floatSetting = worldGen.Settings.GetFloatSetting("SplitTopAndBottomSitesMaxArea");
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet moveTags = new TagSet(worldGen.Settings.GetDefaultMoveTags());
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithTag(WorldGenTags.NearSurface, list);
			VoronoiTree.Node.SplitCommand splitCommand = new VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = moveTags;
			splitCommand.SplitFunction = SplitFunction;
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiTree.Node node = list[i];
				if (node.site.poly.Area() > floatSetting)
				{
					node.Split(splitCommand);
				}
			}
			List<VoronoiTree.Node> list2 = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithTag(WorldGenTags.NearDepths, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiTree.Node node2 = list2[j];
				if (node2.site.poly.Area() > floatSetting)
				{
					node2.Split(splitCommand);
				}
			}
			VoronoiTree.Node.maxDepth = voronoiTree.MaxDepth(0);
			voronoiTree.ForceLowestToLeaf();
			list = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithTag(WorldGenTags.AtSurface, list);
			for (int k = 0; k < list.Count; k++)
			{
				VoronoiTree.Node node3 = list[k];
				node3.tags.Remove(WorldGenTags.Geode);
				node3.tags.Remove(WorldGenTags.Feature);
			}
		}

		private void SplitFunction(Tree tree, VoronoiTree.Node.SplitCommand cmd)
		{
			Node node = null;
			node = ((!tree.tags.Contains(WorldGenTags.Overworld)) ? worldGen.WorldLayout.localGraph.FindNodeByID(tree.site.id) : worldGen.WorldLayout.overworldGraph.FindNodeByID(tree.site.id));
			TagSet tagSet = new TagSet(tree.tags);
			if (cmd.dontCopyTags != null)
			{
				tagSet.Remove(cmd.dontCopyTags);
				if (cmd.moveTags != null)
				{
					tagSet.Remove(cmd.moveTags);
				}
			}
			TagSet tagSet2 = new TagSet();
			if (cmd.moveTags != null)
			{
				for (int i = 0; i < cmd.moveTags.Count; i++)
				{
					Tag item = cmd.moveTags[i];
					if (tree.tags.Contains(item))
					{
						tree.tags.Remove(item);
						tagSet2.Add(item);
					}
				}
			}
			List<Vector2> list = new List<Vector2>();
			if (tagSet.Contains(WorldGenTags.Feature))
			{
				Node node2 = worldGen.WorldLayout.localGraph.AddNode(node.type);
				node2.SetPosition((!tagSet.Contains(WorldGenTags.StartLocation)) ? tree.site.position : tree.site.poly.Centroid());
				VoronoiTree.Node node3 = tree.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiTree.Node.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					node3.SetTags(tagSet);
				}
				tagSet.Remove(WorldGenTags.Feature);
				tagSet.Remove(new Tag(node.type));
				list.Add(node2.position);
			}
			float floatSetting = worldGen.Settings.GetFloatSetting("SplitDensityMin");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("SplitDensityMax");
			if (tree.tags.Contains(WorldGenTags.UltraHighDensitySplit))
			{
				floatSetting = worldGen.Settings.GetFloatSetting("UltraHighSplitDensityMin");
				floatSetting2 = worldGen.Settings.GetFloatSetting("UltraHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.VeryHighDensitySplit))
			{
				floatSetting = worldGen.Settings.GetFloatSetting("VeryHighSplitDensityMin");
				floatSetting2 = worldGen.Settings.GetFloatSetting("VeryHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.HighDensitySplit))
			{
				floatSetting = worldGen.Settings.GetFloatSetting("HighSplitDensityMin");
				floatSetting2 = worldGen.Settings.GetFloatSetting("HighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.MediumDensitySplit))
			{
				floatSetting = worldGen.Settings.GetFloatSetting("MediumSplitDensityMin");
				floatSetting2 = worldGen.Settings.GetFloatSetting("MediumSplitDensityMax");
			}
			float density = tree.myRandom.RandomRange(floatSetting, floatSetting2);
			List<Vector2> points = GetPoints(tree.site.id.ToString(), tree.log, cmd.minChildCount, tree.site.poly, density, 1f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, tree.myRandom, true, true);
			if (points.Count < cmd.minChildCount)
			{
				if (!worldGen.isRunningDebugGen)
				{
					goto IL_039f;
				}
				goto IL_039f;
			}
			goto IL_03ac;
			IL_039f:
			if (points.Count == 0)
			{
				return;
			}
			goto IL_03ac;
			IL_03ac:
			for (int j = 0; j < points.Count; j++)
			{
				Node node4 = worldGen.WorldLayout.localGraph.AddNode((cmd.typeOverride != null) ? cmd.typeOverride(points[j]) : node.type);
				node4.SetPosition(points[j]);
				VoronoiTree.Node node5 = tree.AddSite(new Diagram.Site((uint)node4.node.Id, node4.position, 1f), VoronoiTree.Node.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					node5.SetTags(tagSet);
				}
			}
			for (int k = 0; k < tagSet2.Count; k++)
			{
				Tag tag = tagSet2[k];
				tree.GetChild(tree.myRandom.RandomRange(0, tree.ChildCount())).AddTag(tag);
			}
		}

		private void SprinklePOI(List<TemplateContainer> poi)
		{
			List<VoronoiTree.Node> leafNodesWithTag = GetLeafNodesWithTag(WorldGenTags.StartFar);
			leafNodesWithTag.RemoveAll((VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.AtDepths) || vn.tags.Contains(WorldGenTags.AtSurface));
			leafNodesWithTag.RemoveAll((VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.AtEdge));
			leafNodesWithTag.RemoveAll((VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.EdgeOfVoid));
			for (int i = 0; i < poi.Count; i++)
			{
				VoronoiTree.Node random = leafNodesWithTag.GetRandom(myRandom);
				random.AddTag(new Tag(poi[i].name));
				random.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(random);
				random = leafNodesWithTag.GetRandom(myRandom);
				random.AddTag(new Tag(poi[i].name));
				random.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(random);
				random = leafNodesWithTag.GetRandom(myRandom);
				random.AddTag(new Tag(poi[i].name));
				random.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(random);
			}
		}

		private void TagTopAndBottomSites(Tag topTag, Tag bottomTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			voronoiTree.GetIntersectingLeafSites(topEdge, list);
			voronoiTree.GetIntersectingLeafSites(bottomEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiTree.Node nodeForSite = voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(topTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiTree.Node nodeForSite2 = voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(bottomTag);
			}
		}

		private void TagEdgeSites(Tag leftTag, Tag rightTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			voronoiTree.GetIntersectingLeafSites(leftEdge, list);
			voronoiTree.GetIntersectingLeafSites(rightEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiTree.Node nodeForSite = voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(leftTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiTree.Node nodeForSite2 = voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(rightTag);
			}
		}

		private void SetTemperatureTags()
		{
			List<Leaf> list = new List<Leaf>();
			List<Leaf> list2 = new List<Leaf>();
			voronoiTree.GetIntersectingLeafNodes(topEdge, list);
			voronoiTree.GetIntersectingLeafNodes(bottomEdge, list2);
			TagSet tagSet = new TagSet();
			IEnumerator enumerator = Enum.GetValues(typeof(Temperature.Range)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					tagSet.Add(new Tag(((Temperature.Range)enumerator.Current).ToString()));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Tag item = new Tag(1.ToString());
			for (int i = 0; i < list.Count; i++)
			{
				TagSet tagSet2 = new TagSet(list[i].tags);
				tagSet2.Remove(tagSet);
				tagSet2.Add(item);
				list[i].SetTags(tagSet2);
			}
			Tag item2 = new Tag(9.ToString());
			for (int j = 0; j < list2.Count; j++)
			{
				TagSet tagSet3 = new TagSet(list2[j].tags);
				tagSet3.Remove(tagSet);
				tagSet3.Add(item2);
				list2[j].SetTags(tagSet3);
			}
		}

		private bool StartAreaTooLarge(VoronoiTree.Node node)
		{
			if (node.tags.Contains(WorldGenTags.StartWorld))
			{
				float num = node.site.poly.Area();
				return num > 2000f;
			}
			return false;
		}

		private void SplitLargeStartingSites()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet moveTags = new TagSet(worldGen.Settings.GetDefaultMoveTags());
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetLeafNodes(list, StartAreaTooLarge);
			VoronoiTree.Node.SplitCommand splitCommand = new VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = moveTags;
			splitCommand.SplitFunction = SplitFunction;
			while (list.Count > 0)
			{
				foreach (VoronoiTree.Node item in list)
				{
					item.AddTag(WorldGenTags.DEBUG_SplitLargeStartingSites);
					item.Split(splitCommand);
				}
				list.Clear();
				voronoiTree.GetLeafNodes(list, StartAreaTooLarge);
			}
		}

		private void PropagateStartTag()
		{
			List<VoronoiTree.Node> startNodes = GetStartNodes();
			foreach (VoronoiTree.Node item in startNodes)
			{
				item.AddTagToNeighbors(WorldGenTags.NearStartLocation);
				item.AddTag(WorldGenTags.IgnoreCaveOverride);
			}
		}

		public List<VoronoiTree.Node> GetStartNodes()
		{
			return GetLeafNodesWithTag(WorldGenTags.StartLocation);
		}

		public List<VoronoiTree.Node> GetLeafNodesWithTag(Tag tag)
		{
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetLeafNodes(list, (VoronoiTree.Node node) => node.tags != null && node.tags.Contains(tag));
			return list;
		}

		public List<Node> GetTerrainNodesForTag(Tag tag)
		{
			List<Node> list = new List<Node>();
			List<VoronoiTree.Node> leafNodesWithTag = GetLeafNodesWithTag(tag);
			foreach (VoronoiTree.Node item in leafNodesWithTag)
			{
				Node node = localGraph.FindNodeByID(item.site.id);
				if (node != null)
				{
					list.Add(node);
				}
			}
			return list;
		}

		private Node FindFirstNode(string nodeType)
		{
			return localGraph.FindNode((Node node) => node.type == nodeType);
		}

		private Node FindFirstNodeWithTag(Tag tag)
		{
			return localGraph.FindNode((Node node) => node.tags != null && node.tags.Contains(tag));
		}

		public Vector2I GetStartLocation()
		{
			Node node2 = FindFirstNodeWithTag(WorldGenTags.StartLocation);
			if (node2 == null)
			{
				List<VoronoiTree.Node> nodes = GetStartNodes();
				if (nodes == null || nodes.Count == 0)
				{
					Debug.LogWarning("Couldnt find start node", null);
					return new Vector2I(mapWidth / 2, mapHeight / 2);
				}
				node2 = localGraph.FindNode((Node node) => (uint)node.node.Id == nodes[0].site.id);
				node2.tags.Add(WorldGenTags.StartLocation);
			}
			if (node2 == null)
			{
				Debug.LogWarning("Couldnt find start node", null);
				return new Vector2I(mapWidth / 2, mapHeight / 2);
			}
			Vector2 position = node2.position;
			int a = (int)position.x;
			Vector2 position2 = node2.position;
			return new Vector2I(a, (int)position2.y);
		}

		public List<River> GetRivers()
		{
			List<River> list = new List<River>();
			foreach (Satsuma.Arc item in localGraph.baseGraph.Arcs(ArcFilter.All))
			{
				Satsuma.Node n3 = localGraph.baseGraph.U(item);
				Satsuma.Node n2 = localGraph.baseGraph.V(item);
				Node tn2 = localGraph.FindNode((Node n) => n.node == n3);
				Node tn = localGraph.FindNode((Node n) => n.node == n2);
				if (tn2 != null && tn != null && !(tn2.type != tn.type) && tn2.type.Contains(WorldGenTags.River.Name) && list.Find((River r) => r.SinkPosition() == tn2.position && r.SourcePosition() == tn.position) == null)
				{
					River river = null;
					if (SettingsCache.rivers.rivers.ContainsKey(tn2.type))
					{
						river = new River(SettingsCache.rivers.rivers[tn2.type], false);
						river.AddSection(tn2, tn);
					}
					else
					{
						river = new River(tn2, tn, 1836671383.ToString(), "Granite", 373f, 2000f, 1000f, 100f, 1.5f, 1.5f);
					}
					river.widthCenter = myRandom.RandomRange(1f, river.widthCenter + 0.5f);
					river.widthBorder = myRandom.RandomRange(1f, river.widthBorder + 0.5f);
					river.Stagger(myRandom, (float)myRandom.RandomRange(8, 20), (float)myRandom.RandomRange(1, 3));
					list.Add(river);
				}
			}
			return list;
		}

		private List<Diagram.Site> GetIntersectingSites(VoronoiTree.Node intersectingSiteSource, Tree sitesSource)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			list = new List<Diagram.Site>();
			LineSegment edge;
			for (int i = 1; i < intersectingSiteSource.site.poly.Vertices.Count - 1; i++)
			{
				edge = new LineSegment(intersectingSiteSource.site.poly.Vertices[i - 1], intersectingSiteSource.site.poly.Vertices[i]);
				sitesSource.GetIntersectingLeafSites(edge, list);
			}
			edge = new LineSegment(intersectingSiteSource.site.poly.Vertices[intersectingSiteSource.site.poly.Vertices.Count - 1], intersectingSiteSource.site.poly.Vertices[0]);
			sitesSource.GetIntersectingLeafSites(edge, list);
			return list;
		}

		public void GetEdgeOfMapSites(Tree vt, List<Diagram.Site> topSites, List<Diagram.Site> bottomSites, List<Diagram.Site> leftSites, List<Diagram.Site> rightSites)
		{
			vt.GetIntersectingLeafSites(topEdge, topSites);
			vt.GetIntersectingLeafSites(bottomEdge, bottomSites);
			vt.GetIntersectingLeafSites(leftEdge, leftSites);
			vt.GetIntersectingLeafSites(rightEdge, rightSites);
		}

		public void ConvertEdgeCells(Tree vt)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			List<Diagram.Site> leftSites = new List<Diagram.Site>();
			List<Diagram.Site> rightSites = new List<Diagram.Site>();
			GetEdgeOfMapSites(vt, list, list2, leftSites, rightSites);
			for (int i = 0; i < localGraph.nodes.Count; i++)
			{
				int num = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if ((uint)localGraph.nodes[i].node.Id == list[j].id)
					{
						num = j;
						break;
					}
				}
				if (num != -1)
				{
					localGraph.nodes[i].SetType(WorldGenTags.TheVoid.Name);
					list.RemoveAt(num);
				}
				else
				{
					int num2 = 0;
					num2++;
				}
				num = -1;
				for (int k = 0; k < list2.Count; k++)
				{
					if ((uint)localGraph.nodes[i].node.Id == list2[k].id)
					{
						num = k;
						break;
					}
				}
				if (num != -1)
				{
					float num3 = myRandom.RandomValue();
					localGraph.nodes[i].SetType((!(num3 > 0.33f)) ? "MagmaLake" : ((!(num3 > 0.66f)) ? "MagmaBed" : "MagmaPool"));
					list2.RemoveAt(num);
				}
			}
		}

		[OnSerializing]
		internal void OnSerializingMethod()
		{
			try
			{
				extra = new ExtraIO();
				if (voronoiTree != null)
				{
					extra.internals.Add(voronoiTree);
					voronoiTree.GetInternalNodes(extra.internals);
					List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
					voronoiTree.GetLeafNodes(list, null);
					foreach (Leaf item in list)
					{
						if (item != null)
						{
							extra.leafInternalParent.Add(new KeyValuePair<int, int>(extra.leafs.Count, extra.internals.FindIndex(0, (Tree n) => n == item.parent)));
							extra.leafs.Add(item);
						}
					}
					for (int i = 0; i < extra.internals.Count; i++)
					{
						Tree vt = extra.internals[i];
						if (vt.parent != null)
						{
							int num = extra.internals.FindIndex(0, (Tree n) => n == vt.parent);
							if (num >= 0)
							{
								extra.internalInternalParent.Add(new KeyValuePair<int, int>(i, num));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				Debug.Log("Error deserialising " + ex.Message, null);
			}
		}

		[OnSerialized]
		internal void OnSerializedMethod()
		{
			extra = null;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			extra = new ExtraIO();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				voronoiTree = extra.internals[0];
				for (int i = 0; i < extra.internalInternalParent.Count; i++)
				{
					KeyValuePair<int, int> keyValuePair = extra.internalInternalParent[i];
					Tree child = extra.internals[keyValuePair.Key];
					Tree tree = extra.internals[keyValuePair.Value];
					tree.AddChild(child);
				}
				for (int j = 0; j < extra.leafInternalParent.Count; j++)
				{
					KeyValuePair<int, int> keyValuePair2 = extra.leafInternalParent[j];
					VoronoiTree.Node child2 = extra.leafs[keyValuePair2.Key];
					Tree tree2 = extra.internals[keyValuePair2.Value];
					tree2.AddChild(child2);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				Debug.Log("Error deserialising " + ex.Message, null);
			}
			extra = null;
		}
	}
}
