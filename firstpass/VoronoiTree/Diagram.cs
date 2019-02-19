using Delaunay;
using Delaunay.Geo;
using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace VoronoiTree
{
	public class Diagram
	{
		[SerializationConfig(MemberSerialization.OptIn)]
		public class Site
		{
			[Serialize]
			public uint id;

			[Serialize]
			public float weight;

			public float currentWeight;

			public float previousWeightAdaption;

			[Serialize]
			public Vector2 position;

			[Serialize]
			public Polygon poly;

			[Serialize]
			public HashSet<KeyValuePair<uint, int>> neighbours;

			public Site()
			{
				neighbours = new HashSet<KeyValuePair<uint, int>>();
			}

			public Site(uint id, Vector2 pos, float weight = 1f)
			{
				this.id = id;
				position = pos;
				this.weight = weight;
				currentWeight = weight;
				neighbours = new HashSet<KeyValuePair<uint, int>>();
			}

			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				neighbours = new HashSet<KeyValuePair<uint, int>>();
			}
		}

		public static int maxPowerIterations;

		public static float maxPowerError;

		private List<Vector2> points;

		private List<float> weights;

		private Rect bounds;

		private float weightSum;

		private List<uint> ids = new List<uint>();

		public int siteIndex;

		public Voronoi diagram
		{
			get;
			private set;
		}

		public int completeIterations
		{
			get;
			set;
		}

		public Diagram()
		{
			diagram = null;
		}

		public Diagram(Rect bounds, IEnumerable<Site> sites)
		{
			this.bounds = bounds;
			ids = new List<uint>();
			points = new List<Vector2>();
			weights = new List<float>();
			weightSum = 0f;
			IEnumerator<Site> enumerator = sites.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				AddSite(enumerator.Current);
				num++;
			}
			MakeVD();
		}

		private void AddSite(Site site)
		{
			ids.Add(site.id);
			points.Add(site.position);
			weights.Add(site.weight);
			weightSum += site.weight;
			site.currentWeight = site.weight;
		}

		private void MakeVD()
		{
			diagram = new Voronoi(points, ids, weights, bounds);
		}

		public void UpdateWeights(List<Site> sites)
		{
			for (int i = 0; i < sites.Count; i++)
			{
				Site site = sites[i];
				site.position = site.poly.Centroid();
				site.currentWeight = Mathf.Max(site.currentWeight, 1f);
			}
			float num = 0f;
			for (int j = 0; j < sites.Count; j++)
			{
				Site site2 = sites[j];
				float num2 = site2.poly.Area();
				float num3 = site2.weight / weightSum * Area();
				float num4 = Mathf.Sqrt(num2 / 3.14159274f);
				float num5 = Mathf.Sqrt(num3 / 3.14159274f);
				float num6 = num4 - num5;
				float num7 = num3 / num2;
				if (((double)num7 > 1.1 && (double)site2.previousWeightAdaption < 0.9) || ((double)num7 < 0.9 && (double)site2.previousWeightAdaption > 1.1))
				{
					num7 = Mathf.Sqrt(num7);
				}
				if ((double)num7 < 1.1 && (double)num7 > 0.9 && site2.currentWeight != 1f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				if (site2.currentWeight < 10f)
				{
					num7 *= num7;
				}
				if (site2.currentWeight > 10f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				site2.previousWeightAdaption = num7;
				site2.currentWeight *= num7;
				if (site2.currentWeight < 1f)
				{
					float num8 = Mathf.Sqrt(site2.currentWeight) - num6;
					if (num8 < 0f)
					{
						site2.currentWeight = 0f - num8 * num8;
						if (site2.currentWeight < num)
						{
							num = site2.currentWeight;
						}
					}
				}
			}
			if (num < 0f)
			{
				num = 0f - num;
				for (int k = 0; k < sites.Count; k++)
				{
					Site site3 = sites[k];
					site3.currentWeight += num + 1f;
				}
			}
			float num9 = 1f;
			for (int l = 0; l < sites.Count; l++)
			{
				Site site4 = sites[l];
				List<uint> neighbours = diagram.ListNeighborSitesIDsForSite(points[l]);
				int nIndex;
				for (nIndex = 0; nIndex < neighbours.Count; nIndex++)
				{
					Site site5 = sites.Find((Site s) => s.id == neighbours[nIndex]);
					float num10 = (site4.position - site5.position).sqrMagnitude / (Mathf.Abs(site4.currentWeight - site5.currentWeight) + 1f);
					if (num10 < num9)
					{
						num9 = num10;
					}
				}
			}
			for (int m = 0; m < sites.Count; m++)
			{
				sites[m].currentWeight *= num9;
				weights[m] = sites[m].currentWeight;
				points[m] = sites[m].position;
			}
		}

		private float Area()
		{
			return bounds.width * bounds.height;
		}

		public List<Site> ComputePowerDiagram(List<Site> sites, int maxIterations)
		{
			completeIterations = 0;
			for (int i = 1; i <= maxIterations; i++)
			{
				UpdateWeights(sites);
				MakeVD();
				float num = 0f;
				foreach (Site site in sites)
				{
					float num2 = site.poly.Area();
					float num3 = site.weight / weightSum * Area();
					num = Mathf.Max(Mathf.Abs(num2 - num3) / num3, num);
				}
				if (num < 0.001f)
				{
					completeIterations = i;
					break;
				}
			}
			return sites;
		}

		public int GetIdxForNode(uint nodeID)
		{
			for (int i = 0; i < points.Count; i++)
			{
				if (ids[i] == nodeID)
				{
					return i;
				}
			}
			return -1;
		}

		public List<uint> GetNodeIdsForTopEdgeCells()
		{
			List<uint> list = new List<uint>();
			for (int i = 0; i < points.Count; i++)
			{
				if (IsTopEdgeCell(i))
				{
					list.Add(ids[i]);
				}
			}
			return list;
		}

		public bool IsTopEdgeCell(int cell)
		{
			if (cell < 0 || cell >= points.Count)
			{
				return false;
			}
			List<Vector2> list = diagram.Region(points[cell]);
			if (list.Count == 0)
			{
				return false;
			}
			Vector2 vector = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				Vector2 vector2 = list[i];
				if (vector.y == vector2.y && vector2.y == bounds.height)
				{
					return true;
				}
				vector = vector2;
			}
			float y = vector.y;
			Vector2 vector3 = list[0];
			if (y == vector3.y)
			{
				Vector2 vector4 = list[0];
				if (vector4.y == bounds.height)
				{
					return true;
				}
			}
			return false;
		}
	}
}
