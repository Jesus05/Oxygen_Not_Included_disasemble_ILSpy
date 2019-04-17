using ClipperLib;
using Delaunay.Geo;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiTree
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		public enum NodeType
		{
			Unknown,
			Internal,
			Leaf
		}

		public enum VisitedType
		{
			MissingData = -2,
			Error,
			NotVisited,
			VisitedSuccess
		}

		public class SplitCommand
		{
			public enum SplitType
			{
				KeepParentAsCentroid = 1,
				ChildrenDuplicateParent = 2,
				ChildrenChosenFromLayer = 4
			}

			public delegate string NodeTypeOverride(Vector2 position);

			public SplitType splitType;

			public TagSet dontCopyTags;

			public TagSet moveTags;

			public int minChildCount = 2;

			public NodeTypeOverride typeOverride;

			public Action<Tree, SplitCommand> SplitFunction;
		}

		public static int maxDepth;

		public static uint maxIndex;

		[Serialize]
		public NodeType type;

		public VisitedType visited;

		public LoggerSSF log;

		[Serialize]
		public Diagram.Site site;

		[Serialize]
		public TagSet tags;

		public Dictionary<Tag, int> minDistaceToTag = new Dictionary<Tag, int>();

		public Tree parent
		{
			get;
			private set;
		}

		public Node()
		{
			type = NodeType.Unknown;
			log = new LoggerSSF("VoronoiNode", 35);
		}

		public Node(NodeType type)
		{
			this.type = type;
			tags = new TagSet();
			log = new LoggerSSF("VoronoiNode", 35);
		}

		protected Node(Diagram.Site site, NodeType type, Tree parent)
		{
			tags = new TagSet();
			this.site = site;
			this.type = type;
			this.parent = parent;
			log = new LoggerSSF("VoronoiNode", 35);
		}

		public void SetParent(Tree newParent)
		{
			parent = newParent;
		}

		public Node GetNeighbour(uint id)
		{
			HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key == id)
				{
					return GetSibling(id);
				}
			}
			return null;
		}

		public int DistanceToTag(int dist, int maxDist, TagSet targetTags, HashSet<Node> nvis)
		{
			if (nvis.Contains(this))
			{
				return -1;
			}
			nvis.Add(this);
			if (tags.ContainsOne(targetTags))
			{
				return dist;
			}
			if (maxDist == 0)
			{
				return -1;
			}
			maxDist--;
			dist++;
			List<Node> neighbors = GetNeighbors();
			int num = -1;
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (!nvis.Contains(neighbors[i]))
				{
					int num2 = neighbors[i].DistanceToTag(dist, maxDist, targetTags, nvis);
					if (num2 != -1 && (num == -1 || num2 < num))
					{
						num = num2;
					}
				}
			}
			return num;
		}

		public List<Node> GetNeighbors()
		{
			List<Node> list = new List<Node>();
			if (site.neighbours != null)
			{
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
				while (enumerator.MoveNext())
				{
					list.Add(GetSibling(enumerator.Current.Key));
				}
			}
			return list;
		}

		public List<KeyValuePair<Node, LineSegment>> GetNeighborsByEdge()
		{
			List<KeyValuePair<Node, LineSegment>> list = new List<KeyValuePair<Node, LineSegment>>();
			for (int i = 0; i < site.poly.Vertices.Count; i++)
			{
				if (site.neighbours != null)
				{
					LineSegment edge = site.poly.GetEdge(i);
					Node node = null;
					HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Value == i)
						{
							node = GetSibling(enumerator.Current.Key);
						}
					}
					if (node != null)
					{
						list.Add(new KeyValuePair<Node, LineSegment>(node, edge));
					}
				}
			}
			return list;
		}

		public Node GetSibling(uint siteId)
		{
			return parent.GetChildByID(siteId);
		}

		public List<Node> GetSiblings()
		{
			List<Node> list = new List<Node>();
			for (int i = 0; i < parent.ChildCount(); i++)
			{
				Node child = parent.GetChild(i);
				if (child != this)
				{
					list.Add(child);
				}
			}
			return list;
		}

		public void PlaceSites(List<Diagram.Site> sites, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			List<Vector2> list = null;
			List<Vector2> list2 = new List<Vector2>();
			for (int i = 0; i < sites.Count; i++)
			{
				list2.Add(sites[i].position);
			}
			int num = 0;
			for (int j = 0; j < sites.Count; j++)
			{
				if (!site.poly.Contains(sites[j].position))
				{
					if (list == null)
					{
						list = PointGenerator.GetRandomPoints(site.poly, 5f, 1f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, seededRandom, true, true);
					}
					if (num >= list.Count - 1)
					{
						list2.AddRange(list);
						list = PointGenerator.GetRandomPoints(site.poly, 0.5f, 0.5f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, seededRandom, true, true);
						num = 0;
					}
					if (list.Count == 0)
					{
						sites[j].position = sites[0].position + Vector2.one * seededRandom.RandomValue();
					}
					else
					{
						sites[j].position = list[num++];
					}
				}
			}
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			for (int k = 0; k < sites.Count; k++)
			{
				if (hashSet.Contains(sites[k].position))
				{
					visited = VisitedType.Error;
					sites[k].position += new Vector2((float)seededRandom.RandomRange(0, 1), (float)seededRandom.RandomRange(0, 1));
				}
				hashSet.Add(sites[k].position);
				sites[k].poly = null;
			}
		}

		public bool ComputeNode(List<Diagram.Site> sites)
		{
			if (site.poly == null || sites == null || sites.Count == 0)
			{
				visited = VisitedType.MissingData;
				return false;
			}
			visited = VisitedType.VisitedSuccess;
			if (sites.Count == 1)
			{
				sites[0].poly = site.poly;
				sites[0].position = sites[0].poly.Centroid();
				return true;
			}
			HashSet<Diagram.Site> hashSet = new HashSet<Diagram.Site>();
			for (int i = 0; i < sites.Count; i++)
			{
				hashSet.Add(new Diagram.Site(sites[i].id, sites[i].position, sites[i].weight));
			}
			hashSet.Add(new Diagram.Site(maxIndex + 1, new Vector2(site.poly.bounds.xMin - 500f, site.poly.bounds.yMin + site.poly.bounds.height / 2f), 1f));
			hashSet.Add(new Diagram.Site(maxIndex + 2, new Vector2(site.poly.bounds.xMax + 500f, site.poly.bounds.yMin + site.poly.bounds.height / 2f), 1f));
			hashSet.Add(new Diagram.Site(maxIndex + 3, new Vector2(site.poly.bounds.xMin + site.poly.bounds.width / 2f, site.poly.bounds.yMin - 500f), 1f));
			hashSet.Add(new Diagram.Site(maxIndex + 4, new Vector2(site.poly.bounds.xMin + site.poly.bounds.width / 2f, site.poly.bounds.yMax + 500f), 1f));
			Rect bounds = new Rect(site.poly.bounds.xMin - 500f, site.poly.bounds.yMin - 500f, site.poly.bounds.width + 500f, site.poly.bounds.height + 500f);
			Diagram diagram = new Diagram(bounds, hashSet);
			for (int j = 0; j < sites.Count; j++)
			{
				if (sites[j].id <= maxIndex)
				{
					List<Vector2> list = diagram.diagram.Region(sites[j].position);
					if (list == null)
					{
						if (type != NodeType.Leaf)
						{
							visited = VisitedType.Error;
							return false;
						}
					}
					else
					{
						Polygon polygon = new Polygon(list).Clip(site.poly, ClipType.ctIntersection);
						if (polygon == null || polygon.Vertices.Count < 3)
						{
							if (type != NodeType.Leaf)
							{
								visited = VisitedType.Error;
								return false;
							}
						}
						else
						{
							sites[j].poly = polygon;
						}
					}
				}
			}
			for (int k = 0; k < sites.Count; k++)
			{
				if (sites[k].id <= maxIndex)
				{
					HashSet<uint> neighbours = diagram.diagram.NeighborSitesIDsForSite(sites[k].position);
					FilterNeighbours(sites[k], neighbours, sites);
					sites[k].position = sites[k].poly.Centroid();
				}
			}
			return true;
		}

		public bool ComputeNodePD(List<Diagram.Site> sites, int maxIters = 500, float threashold = 0.2f)
		{
			if (site.poly == null || sites == null || sites.Count == 0)
			{
				visited = VisitedType.MissingData;
				return false;
			}
			visited = VisitedType.VisitedSuccess;
			List<Site> list = new List<Site>();
			for (int i = 0; i < sites.Count; i++)
			{
				Site item = new Site(sites[i].id, sites[i].position, sites[i].weight);
				list.Add(item);
			}
			PowerDiagram powerDiagram = new PowerDiagram(site.poly, list);
			powerDiagram.ComputeVD();
			powerDiagram.ComputePowerDiagram(maxIters, threashold);
			for (int j = 0; j < sites.Count; j++)
			{
				sites[j].poly = list[j].poly;
				if (sites[j].poly == null)
				{
					Debug.LogErrorFormat("Site [{0}] at index [{1}]: Poly shouldnt be null here ever", sites[j].id, j);
				}
				HashSet<uint> hashSet = new HashSet<uint>();
				for (int k = 0; k < list[j].neighbours.Count; k++)
				{
					if (!list[j].neighbours[k].dummy)
					{
						hashSet.Add((uint)list[j].neighbours[k].id);
					}
				}
				sites[j].position = sites[j].poly.Centroid();
			}
			return true;
		}

		private static void FilterNeighbours(Diagram.Site home, HashSet<uint> neighbours, List<Diagram.Site> sites)
		{
			if (home == null)
			{
				Debug.LogError("FilterNeighbours home == null");
			}
			HashSet<KeyValuePair<uint, int>> hashSet = new HashSet<KeyValuePair<uint, int>>();
			HashSet<uint>.Enumerator niter = neighbours.GetEnumerator();
			while (niter.MoveNext())
			{
				Diagram.Site site = sites.Find((Diagram.Site s) => s.id == niter.Current);
				if (site != null)
				{
					if (site.poly == null)
					{
						Debug.LogError("FilterNeighbours neighbour.poly == null");
					}
					int edgeIdx = -1;
					Polygon.Commonality commonality = home.poly.SharesEdge(site.poly, ref edgeIdx);
					if (commonality == Polygon.Commonality.Edge)
					{
						hashSet.Add(new KeyValuePair<uint, int>(niter.Current, edgeIdx));
					}
				}
			}
			home.neighbours = hashSet;
		}

		public void Reset(List<Diagram.Site> sites = null)
		{
			visited = VisitedType.NotVisited;
			if (sites != null)
			{
				HashSet<Vector2> hashSet = new HashSet<Vector2>();
				int num = 0;
				while (true)
				{
					if (num >= sites.Count)
					{
						return;
					}
					if (hashSet.Contains(sites[num].position))
					{
						break;
					}
					hashSet.Add(sites[num].position);
					num++;
				}
				visited = VisitedType.Error;
			}
		}

		public void SetTags(TagSet originalTags)
		{
			tags = new TagSet(originalTags);
		}

		public void AddTag(Tag tag)
		{
			if (tags == null)
			{
				tags = new TagSet();
			}
			tags.Add(tag);
		}

		public void AddTagToNeighbors(Tag tag)
		{
			HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GetNeighbour(enumerator.Current.Key).AddTag(tag);
			}
		}

		public virtual Tree Split(SplitCommand cmd = null)
		{
			return null;
		}
	}
}
