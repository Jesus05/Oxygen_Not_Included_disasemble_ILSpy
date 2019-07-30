using Delaunay.Geo;
using KSerialization;
using ObjectCloner;
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
			VoronoiTree = 0x4,
			PowerDiagram = 0x8
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
			Debug.Assert(mapWidth != 0 && mapHeight != 0, "Map size has not been set");
			Debug.Assert(worldGen.Settings.world != null, "You need to set a world");
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
			float num = myRandom.RandomRange(floatSetting, floatSetting2);
			float floatSetting3 = worldGen.Settings.GetFloatSetting("OverworldAvoidRadius");
			PointGenerator.SampleBehaviour enumSetting = worldGen.Settings.GetEnumSetting<PointGenerator.SampleBehaviour>("OverworldSampleBehaviour");
			Debug.Log($"Generating overworld points using {enumSetting.ToString()}, density {num}");
			string startSubworldName = worldGen.Settings.world.startSubworldName;
			SubWorld subWorld = worldGen.Settings.GetSubWorld(startSubworldName);
			Vector2 vector = new Vector2((float)mapWidth * worldGen.Settings.world.startingBasePositionHorizontal.GetRandomValueWithinRange(myRandom), (float)mapHeight * worldGen.Settings.world.startingBasePositionVertical.GetRandomValueWithinRange(myRandom));
			Debug.Log("Start node position is " + vector);
			Node node = overworldGraph.AddNode(startSubworldName);
			node.SetPosition(vector);
			VoronoiTree.Node node2 = voronoiTree.AddSite(new Diagram.Site((uint)node.node.Id, node.position, subWorld.pdWeight), VoronoiTree.Node.NodeType.Internal);
			ApplySubworldToNode(node2, subWorld);
			List<Vector2> list = new List<Vector2>();
			list.Add(node.position);
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num, floatSetting3, list, enumSetting, false, myRandom, false, true);
			Debug.Log($" -> Generated {randomPoints.Count} points");
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
			node2.AddTag(WorldGenTags.AtStart);
			TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
			TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
			for (int k = 0; k < voronoiTree.ChildCount(); k++)
			{
				VoronoiTree.Node child = voronoiTree.GetChild(k);
				Node node5 = overworldGraph.FindNodeByID(child.site.id);
				node5.tags.Union(child.tags);
				node5.SetPosition(child.site.position);
				List<VoronoiTree.Node> neighbors = child.GetNeighbors();
				for (int l = 0; l < neighbors.Count; l++)
				{
					Node nodeB = overworldGraph.FindNodeByID(neighbors[l].site.id);
					overworldGraph.AddArc(node5, nodeB, "Neighbor");
				}
			}
			PropagateDistanceTags(voronoiTree, WorldGenTags.DistanceTags);
			ConvertUnknownCells();
			int intSetting2 = worldGen.Settings.GetIntSetting("OverworldRelaxIterations");
			float floatSetting4 = worldGen.Settings.GetFloatSetting("OverworldRelaxEnergyMin");
			voronoiTree.RelaxRecursive(0, intSetting2, floatSetting4, usePD);
			if (worldGen.Settings.GetOverworldAddTags() != null)
			{
				foreach (string overworldAddTag in worldGen.Settings.GetOverworldAddTags())
				{
					int childIndex = myRandom.RandomSource().Next(voronoiTree.ChildCount());
					VoronoiTree.Node child2 = voronoiTree.GetChild(childIndex);
					child2.AddTag(new Tag(overworldAddTag));
				}
			}
			FlattenOverworld();
			return voronoiTree;
		}

		public void PopulateSubworlds()
		{
			AddSubworldChildren();
			GetStartLocation();
			PropagateStartTag();
		}

		private void PropagateDistanceTags(Tree tree, TagSet tags)
		{
			foreach (Tag tag in tags)
			{
				Dictionary<uint, int> distanceToTag = overworldGraph.GetDistanceToTag(tag);
				if (distanceToTag != null)
				{
					int num = 0;
					for (int i = 0; i < tree.ChildCount(); i++)
					{
						VoronoiTree.Node child = tree.GetChild(i);
						uint id = child.site.id;
						if (distanceToTag.ContainsKey(id))
						{
							child.minDistanceToTag.Add(tag, distanceToTag[id]);
							num++;
							if (distanceToTag[id] > 0)
							{
								child.AddTag(new Tag(tag.Name + "_Distance" + distanceToTag[id]));
							}
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

		private HashSet<WeightedSubWorld> GetNameFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, List<WeightedSubWorld> subworlds)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				int i;
				for (i = 0; i < filter.subworldNames.Count; i++)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[i]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					int j;
					for (j = 0; j < filter.subworldNames.Count; j++)
					{
						hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[j]));
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					int k;
					for (k = 0; k < filter.subworldNames.Count; k++)
					{
						hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[k]));
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<WeightedSubWorld> GetZoneTypeFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				for (int j = 0; j < filter.zoneTypes.Count; j++)
				{
					hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[j].ToString()]);
				}
				break;
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int k = 0; k < filter.zoneTypes.Count; k++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
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

		private HashSet<WeightedSubWorld> GetTemperatureFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				for (int j = 0; j < filter.temperatureRanges.Count; j++)
				{
					hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[j].ToString()]);
				}
				break;
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int k = 0; k < filter.temperatureRanges.Count; k++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
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

		private void RunFilterClearCommand(VoronoiTree.Node vn, World.AllowedCellsFilter filter, HashSet<WeightedSubWorld> allowedSubworldsSet)
		{
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				allowedSubworldsSet.Clear();
				break;
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					allowedSubworldsSet.Clear();
				}
				break;
			}
		}

		private HashSet<WeightedSubWorld> Filter(VoronoiTree.Node vn, List<WeightedSubWorld> allSubWorlds, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			World world = worldGen.Settings.world;
			string text = string.Empty;
			foreach (KeyValuePair<Tag, int> item in vn.minDistanceToTag)
			{
				string text2 = text;
				text = text2 + item.Key.Name + ":" + item.Value.ToString() + ", ";
			}
			foreach (World.AllowedCellsFilter unknownCellsAllowedSubworld in world.unknownCellsAllowedSubworlds)
			{
				HashSet<WeightedSubWorld> hashSet2 = new HashSet<WeightedSubWorld>();
				if (unknownCellsAllowedSubworld.subworldNames != null && unknownCellsAllowedSubworld.subworldNames.Count > 0)
				{
					hashSet2.UnionWith(GetNameFilterSet(vn, unknownCellsAllowedSubworld, allSubWorlds));
				}
				if (unknownCellsAllowedSubworld.temperatureRanges != null && unknownCellsAllowedSubworld.temperatureRanges.Count > 0)
				{
					hashSet2.UnionWith(GetTemperatureFilterSet(vn, unknownCellsAllowedSubworld, subworldsByTemperature));
				}
				if (unknownCellsAllowedSubworld.zoneTypes != null && unknownCellsAllowedSubworld.zoneTypes.Count > 0)
				{
					hashSet2.UnionWith(GetZoneTypeFilterSet(vn, unknownCellsAllowedSubworld, subworldsByZoneType));
				}
				switch (unknownCellsAllowedSubworld.command)
				{
				case World.AllowedCellsFilter.Command.Clear:
					RunFilterClearCommand(vn, unknownCellsAllowedSubworld, hashSet);
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
			List<WeightedName> list2 = new List<WeightedName>(worldGen.Settings.world.subworldFiles);
			list2.RemoveAll((WeightedName s) => s.name == worldGen.Settings.world.startSubworldName);
			List<WeightedSubWorld> subworldsForWorld = worldGen.Settings.GetSubworldsForWorld(list2);
			Dictionary<string, List<WeightedSubWorld>> dictionary = new Dictionary<string, List<WeightedSubWorld>>();
			IEnumerator enumerator = Enum.GetValues(typeof(Temperature.Range)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Temperature.Range range = (Temperature.Range)enumerator.Current;
					dictionary.Add(range.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.temperatureRange == range));
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
			Dictionary<string, List<WeightedSubWorld>> dictionary2 = new Dictionary<string, List<WeightedSubWorld>>();
			IEnumerator enumerator2 = Enum.GetValues(typeof(SubWorld.ZoneType)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					SubWorld.ZoneType zt = (SubWorld.ZoneType)enumerator2.Current;
					dictionary2.Add(zt.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.zoneType == zt));
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
			foreach (VoronoiTree.Node item in list)
			{
				Node node = overworldGraph.FindNodeByID(item.site.id);
				item.tags.Remove(WorldGenTags.UnassignedNode);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				HashSet<WeightedSubWorld> collection = Filter(item, subworldsForWorld, dictionary, dictionary2);
				List<WeightedSubWorld> list3 = new List<WeightedSubWorld>(collection);
				WeightedSubWorld weightedSubWorld = WeightedRandom.Choose(list3, myRandom);
				if (weightedSubWorld != null)
				{
					SubWorld subWorld = weightedSubWorld.subWorld;
					ApplySubworldToNode(item, subWorld);
				}
				else
				{
					string text = string.Empty;
					foreach (KeyValuePair<Tag, int> item2 in item.minDistanceToTag)
					{
						string text2 = text;
						text = text2 + item2.Key.Name + ":" + item2.Value.ToString() + ", ";
					}
					DebugUtil.LogWarningArgs("No allowed Subworld types. Using default. ", node.tags.ToString(), "Distances:", text);
					node.SetType("Default");
				}
			}
		}

		private Node ApplySubworldToNode(VoronoiTree.Node vn, SubWorld subWorld)
		{
			Node node = overworldGraph.FindNodeByID(vn.site.id);
			node.SetType(subWorld.name);
			vn.site.weight = subWorld.pdWeight;
			foreach (string tag in subWorld.tags)
			{
				vn.AddTag(new Tag(tag));
			}
			return node;
		}

		private void FlattenOverworld()
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
						bool didCreate;
						Cell cell = overworldGraph.GetCell(node.position, node.node, true, out didCreate);
						Debug.Assert(didCreate, "Tried creating a new cell but one already exists. Huh? " + child.site.id);
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
						Debug.Assert(cell2 != null, "cell is null: " + node2.node);
						List<KeyValuePair<VoronoiTree.Node, LineSegment>> neighborsByEdge2 = tree3.GetNeighborsByEdge();
						for (int m = 0; m < neighborsByEdge2.Count; m++)
						{
							KeyValuePair<VoronoiTree.Node, LineSegment> keyValuePair2 = neighborsByEdge2[m];
							MapGraph mapGraph3 = overworldGraph;
							Vector2? p3 = keyValuePair2.Value.p0;
							Corner corner = mapGraph3.GetCorner(p3.Value, false);
							Debug.Assert(corner != null, "corner0 is null: " + keyValuePair2.Value.p0);
							MapGraph mapGraph4 = overworldGraph;
							Vector2? p4 = keyValuePair2.Value.p1;
							Corner corner2 = mapGraph4.GetCorner(p4.Value, false);
							Debug.Assert(corner2 != null, "corner1 is null: " + keyValuePair2.Value.p1);
							Edge edge = null;
							VoronoiTree.Node key = keyValuePair2.Key;
							if (key != null)
							{
								Node node3 = overworldGraph.FindNodeByID(key.site.id);
								Cell cell3 = overworldGraph.GetCell(node3.node);
								Debug.Assert(cell3 != null, "otherCell is null: " + node3.node);
								edge = overworldGraph.GetEdge(corner, corner2, cell2, cell3, true, out bool _);
								SubWorld subWorld = worldGen.Settings.GetSubWorld(node2.type);
								Debug.Assert(subWorld != null, "SubWorld is null: " + node2.type);
								SubWorld subWorld2 = worldGen.Settings.GetSubWorld(node3.type);
								Debug.Assert(subWorld2 != null, "other SubWorld is null: " + node3.type);
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
								edge = overworldGraph.GetEdge(corner, corner2, cell2, cell2, true, out bool _);
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
				Debug.LogError("ex: " + message + " " + stackTrace);
			}
		}

		public static bool TestEdgeConsistency(Cell cell, out Edge problemEdge)
		{
			for (int i = 0; i < cell.edges.Count; i++)
			{
				Edge edge = cell.edges[i];
				if (!IsEdgeConsistent(cell, edge))
				{
					problemEdge = edge;
					return false;
				}
			}
			problemEdge = null;
			return true;
		}

		public static bool IsEdgeConsistent(Cell cell, Edge edge1)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < cell.edges.Count; i++)
			{
				Edge edge2 = cell.edges[i];
				if (edge1 != edge2)
				{
					if (edge1.corner0 == edge2.corner0 || edge1.corner0 == edge2.corner1)
					{
						flag = true;
					}
					if (edge1.corner1 == edge2.corner0 || edge1.corner1 == edge2.corner1)
					{
						flag2 = true;
					}
				}
			}
			if (!flag || !flag2)
			{
				return false;
			}
			return true;
		}

		public bool IsNodeBorderOpen(VoronoiTree.Node n1, VoronoiTree.Node n2, TagSet edgeOpenTags)
		{
			Debug.Assert(n1 != null, "Border test: n1 was null");
			Debug.Assert(n2 != null, "Border test: n2 was null");
			Node node = overworldGraph.FindNodeByID(n1.site.id);
			Node node2 = overworldGraph.FindNodeByID(n2.site.id);
			Debug.Assert(node != null, "Border test: tn1 was null");
			Debug.Assert(node2 != null, "Border test: tn2 was null");
			Cell cell = overworldGraph.GetCell(node.node);
			Cell cell2 = overworldGraph.GetCell(node2.node);
			Debug.Assert(cell != null, "Border test: cell1 was null");
			Debug.Assert(cell2 != null, "Border test: cell2 was null");
			SubWorld subWorld = worldGen.Settings.GetSubWorld(node.type);
			SubWorld subWorld2 = worldGen.Settings.GetSubWorld(node2.type);
			Debug.Assert(subWorld != null, "Border test: sw1 was null");
			Debug.Assert(subWorld2 != null, "Border test: sw2 was null");
			return node.type == node2.type || subWorld.zoneType == subWorld2.zoneType || (subWorld.zoneType == SubWorld.ZoneType.Space && subWorld2.zoneType == SubWorld.ZoneType.Space) || (cell.tags.ContainsOne(edgeOpenTags) && cell2.tags.ContainsOne(edgeOpenTags));
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
			List<Feature> list = new List<Feature>();
			foreach (KeyValuePair<string, int> globalFeature in worldGen.Settings.world.globalFeatures)
			{
				for (int i = 0; i < globalFeature.Value; i++)
				{
					Feature feature = new Feature();
					feature.type = globalFeature.Key;
					list.Add(feature);
				}
			}
			Dictionary<uint, List<Feature>> dictionary = new Dictionary<uint, List<Feature>>();
			List<VoronoiTree.Node> list2 = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithoutTag(WorldGenTags.NoGlobalFeatureSpawning, list2);
			list2.ShuffleSeeded(myRandom.RandomSource());
			foreach (Feature item in list)
			{
				VoronoiTree.Node node = list2[0];
				list2.RemoveAt(0);
				if (!dictionary.ContainsKey(node.site.id))
				{
					dictionary[node.site.id] = new List<Feature>();
				}
				dictionary[node.site.id].Add(item);
			}
			for (int j = 0; j < voronoiTree.ChildCount(); j++)
			{
				VoronoiTree.Node child = voronoiTree.GetChild(j);
				if (child.type == VoronoiTree.Node.NodeType.Internal)
				{
					Tree tree = child as Tree;
					Node node2 = overworldGraph.FindNodeByID(tree.site.id);
					SubWorld subWorld = SerializingCloner.Copy(worldGen.Settings.GetSubWorld(node2.type));
					tree.AddTag(new Tag(node2.type));
					tree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					if (dictionary.ContainsKey(child.site.id))
					{
						subWorld.features.AddRange(dictionary[child.site.id]);
					}
					GenerateChildren(subWorld, tree, localGraph, (float)mapHeight, j + myRandom.seed);
					int num = tree.ChildCount();
					if (num < subWorld.minChildCount)
					{
						tree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
						splitCommand.dontCopyTags = tagSet;
						splitCommand.minChildCount = subWorld.minChildCount - num;
						tree.Split(splitCommand);
						if (subWorld.biomes != null && subWorld.biomes.Count > 0)
						{
							for (int k = num; k < tree.ChildCount(); k++)
							{
								WeightedBiome weightedBiome = WeightedRandom.Choose(subWorld.biomes, myRandom);
								Node node3 = localGraph.FindNodeByID(tree.GetChild(k).site.id);
								node3.SetType(weightedBiome.name);
								tree.GetChild(k).AddTag(new Tag(node3.type));
							}
						}
						else
						{
							for (int l = num; l < tree.ChildCount(); l++)
							{
								Node node4 = localGraph.FindNodeByID(tree.GetChild(l).site.id);
								node4.SetType(GetNodeTypeFromLayers(tree.site.position, (float)mapHeight, myRandom));
								tree.GetChild(l).AddTag(new Tag(node4.type));
							}
						}
					}
					tree.RelaxRecursive(0, 10, 1f, worldGen.Settings.world.layoutMethod == World.LayoutMethod.PowerTree);
					List<VoronoiTree.Node> list3 = new List<VoronoiTree.Node>();
					tree.GetNodesWithTag(WorldGenTags.Feature, list3);
					TagSet tagSet2 = new TagSet();
					tagSet2.Add(WorldGenTags.Feature);
					tagSet2.Add(WorldGenTags.SplitOnParentDensity);
					splitCommand.dontCopyTags = tagSet2;
					for (int m = 0; m < list3.Count; m++)
					{
						if (!list3[m].tags.Contains(WorldGenTags.CenteralFeature))
						{
							if (list3[m].tags.Contains(WorldGenTags.SplitOnParentDensity))
							{
								list3[m].Split(splitCommand);
							}
							if (list3[m].tags.Contains(WorldGenTags.SplitTwice))
							{
								Tree tree2 = list3[m].Split(splitCommand);
								if (tree2.ChildCount() <= 1)
								{
									Debug.LogError("split did not work.");
								}
								for (int n = 0; n < tree2.ChildCount(); n++)
								{
									VoronoiTree.Node child2 = tree2.GetChild(n);
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
					if (!worldGen.isRunningDebugGen)
					{
						goto IL_0042;
					}
				}
				goto IL_0042;
				IL_0042:
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
			List<Vector2> list = new List<Vector2>();
			if (sw.centralFeature != null)
			{
				list.Add(node.site.poly.Centroid());
				VoronoiTree.Node node2 = CreateTreeNodeWithFeatureAndBiome(worldGen.Settings, sw, node, graph, sw.centralFeature, node.site.poly.Centroid(), tagSet3, -1);
				node2.AddTag(WorldGenTags.CenteralFeature);
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
				if (worldGen.isRunningDebugGen)
				{
					Debug.Assert(points.Count >= num, "Error not enough points " + sw.name + " in node " + node.site.id);
				}
				return;
			}
			if (sw.features.Count <= points.Count)
			{
				goto IL_0349;
			}
			goto IL_0349;
			IL_0349:
			for (int m = 0; m < points.Count; m++)
			{
				Feature feature = null;
				if (m < sw.features.Count)
				{
					feature = sw.features[m];
				}
				CreateTreeNodeWithFeatureAndBiome(worldGen.Settings, sw, node, graph, feature, points[m], tagSet3, m);
			}
			node.ComputeChildren(seededRandom.seed + 1, false, false);
			if (node.ChildCount() > 0)
			{
				for (int n = 0; n < tagSet2.Count; n++)
				{
					Debug.Log($"Applying Moved Tag {tagSet2[n].Name} to {node.site.id}");
					VoronoiTree.Node child = node.GetChild(seededRandom.RandomSource().Next(node.ChildCount()));
					child.AddTag(tagSet2[n]);
				}
			}
		}

		private VoronoiTree.Node CreateTreeNodeWithFeatureAndBiome(WorldGenSettings settings, SubWorld sw, Tree node, Graph graph, Feature feature, Vector2 pos, TagSet newTags, int i)
		{
			bool flag = false;
			TagSet tagSet = new TagSet();
			TagSet tagSet2 = new TagSet();
			string type;
			if (feature != null)
			{
				FeatureSettings feature2 = settings.GetFeature(feature.type);
				type = feature.type;
				tagSet2.Union(new TagSet(feature2.tags));
				if (feature.tags != null && feature.tags.Count > 0)
				{
					tagSet2.Union(new TagSet(feature.tags));
				}
				if (feature.excludesTags != null && feature.excludesTags.Count > 0)
				{
					tagSet2.Remove(new TagSet(feature.excludesTags));
				}
				tagSet2.Add(new Tag(feature.type));
				tagSet2.Add(WorldGenTags.Feature);
				if (feature2.forceBiome != null)
				{
					tagSet.Add(feature2.forceBiome);
					flag = true;
				}
				if (feature2.biomeTags != null)
				{
					tagSet.Union(new TagSet(feature2.biomeTags));
				}
			}
			if (!flag && sw.biomes.Count > 0)
			{
				WeightedBiome weightedBiome = WeightedRandom.Choose(sw.biomes, myRandom);
				type = weightedBiome.name;
				tagSet.Add(weightedBiome.name);
				if (weightedBiome.tags != null && weightedBiome.tags.Count > 0)
				{
					tagSet.Union(new TagSet(weightedBiome.tags));
				}
			}
			else
			{
				type = "UNKNOWN";
			}
			Node node2 = graph.AddNode(type);
			node2.biomeSpecificTags = new TagSet(tagSet);
			node2.featureSpecificTags = new TagSet(tagSet2);
			node2.SetPosition(pos);
			VoronoiTree.Node node3 = node.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiTree.Node.NodeType.Internal);
			node3.tags = new TagSet(newTags);
			node3.tags.Add(type);
			node3.tags.Union(tagSet);
			node3.tags.Union(tagSet2);
			return node3;
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
			Debug.Assert(node != null, "Null terrain node WTF");
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
				node2.SetPosition((!tagSet.Contains(WorldGenTags.CenteralFeature)) ? tree.site.position : tree.site.poly.Centroid());
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
				if (worldGen.isRunningDebugGen)
				{
					Debug.Assert(points.Count >= cmd.minChildCount, "Error not enough points [" + cmd.minChildCount + "] for tree split " + tree.site.id.ToString());
				}
				if (points.Count == 0)
				{
					return;
				}
			}
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

		private bool StartAreaTooLarge(VoronoiTree.Node node)
		{
			if (node.tags.Contains(WorldGenTags.AtStart))
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
					Debug.LogWarning("Couldnt find start node");
					return new Vector2I(mapWidth / 2, mapHeight / 2);
				}
				node2 = localGraph.FindNode((Node node) => (uint)node.node.Id == nodes[0].site.id);
				node2.tags.Add(WorldGenTags.StartLocation);
			}
			if (node2 == null)
			{
				Debug.LogWarning("Couldnt find start node");
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
					if (SettingsCache.rivers.ContainsKey(tn2.type))
					{
						river = new River(SettingsCache.rivers[tn2.type], false);
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
				Debug.Log("Error deserialising " + ex.Message);
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
				Debug.Log("Error deserialising " + ex.Message);
			}
			extra = null;
		}
	}
}
