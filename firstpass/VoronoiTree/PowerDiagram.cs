using ClipperLib;
using Delaunay.Geo;
using MIConvexHull;
using ProcGen.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoronoiTree
{
	public class PowerDiagram
	{
		private class Edge : MathUtil.Pair<DualSite3d, DualSite3d>
		{
			public Edge(DualSite3d first, DualSite3d second)
			{
				base.First = first;
				base.Second = second;
			}
		}

		public class ConvexFaceExt<TVertex> : ConvexFace<TVertex, ConvexFaceExt<TVertex>> where TVertex : IVertex
		{
			private Site site;

			private Vector2 dualPoint;

			private Vector2? circumCenter;

			public TVertex vertex0 => base.Vertices[0];

			public TVertex vertex1 => base.Vertices[1];

			public TVertex vertex2 => base.Vertices[2];

			public ConvexFaceExt<TVertex> edge0 => base.Adjacency[0];

			public ConvexFaceExt<TVertex> edge1 => base.Adjacency[1];

			public ConvexFaceExt<TVertex> edge2 => base.Adjacency[2];

			public Vector2 Circumcenter
			{
				get
				{
					Vector2? nullable = circumCenter;
					circumCenter = ((!nullable.HasValue) ? GetCircumcenter() : nullable.Value);
					return circumCenter.Value;
				}
			}

			public Vector2 GetDualPoint()
			{
				if (dualPoint.x == 0f && dualPoint.y == 0f)
				{
					Vector3 vector = new Vector3((float)vertex0.Position[0], (float)vertex0.Position[1], (float)vertex0.Position[2]);
					Vector3 vector2 = new Vector3((float)vertex1.Position[0], (float)vertex1.Position[1], (float)vertex1.Position[2]);
					Vector3 vector3 = new Vector3((float)vertex2.Position[0], (float)vertex2.Position[1], (float)vertex2.Position[2]);
					double num = (double)(vector.y * (vector2.z - vector3.z) + vector2.y * (vector3.z - vector.z) + vector3.y * (vector.z - vector2.z));
					double num2 = (double)(vector.z * (vector2.x - vector3.x) + vector2.z * (vector3.x - vector.x) + vector3.z * (vector.x - vector2.x));
					double num3 = -0.5 / (double)(vector.x * (vector2.y - vector3.y) + vector2.x * (vector3.y - vector.y) + vector3.x * (vector.y - vector2.y));
					dualPoint = new Vector2((float)(num * num3), (float)(num2 * num3));
				}
				return dualPoint;
			}

			private double Det(double[,] m)
			{
				return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
			}

			private double LengthSquared(double[] v)
			{
				double num = 0.0;
				foreach (double num2 in v)
				{
					num += num2 * num2;
				}
				return num;
			}

			private Vector2 GetCircumcenter()
			{
				TVertex[] vertices = base.Vertices;
				double[,] array = new double[3, 3];
				for (int i = 0; i < 3; i++)
				{
					double[,] array2 = array;
					int num = i;
					double num2 = vertices[i].Position[0];
					array2[num, 0] = num2;
					double[,] array3 = array;
					int num3 = i;
					double num4 = vertices[i].Position[1];
					array3[num3, 1] = num4;
					array[i, 2] = 1.0;
				}
				double num5 = Det(array);
				double num6 = -1.0 / (2.0 * num5);
				for (int j = 0; j < 3; j++)
				{
					double[,] array4 = array;
					int num7 = j;
					double num8 = LengthSquared(vertices[j].Position);
					array4[num7, 0] = num8;
				}
				double num9 = 0.0 - Det(array);
				for (int k = 0; k < 3; k++)
				{
					double[,] array5 = array;
					int num10 = k;
					double num11 = vertices[k].Position[0];
					array5[num10, 1] = num11;
				}
				double num12 = Det(array);
				return new Vector2((float)(num6 * num9), (float)(num6 * num12));
			}
		}

		public class TriangulationCellExt<TVertex> : TriangulationCell<TVertex, TriangulationCellExt<TVertex>> where TVertex : IVertex
		{
			public TVertex Vertex0 => base.Vertices[0];

			public TVertex Vertex1 => base.Vertices[1];

			public TVertex Vertex2 => base.Vertices[2];

			public TriangulationCellExt<TVertex> Edge0 => base.Adjacency[0];

			public TriangulationCellExt<TVertex> Edge1 => base.Adjacency[1];

			public TriangulationCellExt<TVertex> Edge2 => base.Adjacency[2];
		}

		public class DualSite2d : IVertex
		{
			public double[] Position => new double[2]
			{
				(double)site.position[0],
				(double)site.position[1]
			};

			public Site site
			{
				get;
				set;
			}

			public bool visited
			{
				get;
				set;
			}

			public DualSite2d(Site site)
			{
				this.site = site;
				visited = false;
			}
		}

		public class DualSite3d : IVertex
		{
			public double[] Position => new double[3]
			{
				(double)coord[0],
				(double)coord[1],
				(double)coord[2]
			};

			public Vector3 coord
			{
				get;
				set;
			}

			public Site site
			{
				get;
				set;
			}

			public bool visited
			{
				get;
				set;
			}

			public DualSite3d()
				: this(0.0, 0.0, 0.0)
			{
			}

			public DualSite3d(double _x, double _y, double _z)
			{
				coord = new Vector3((float)_x, (float)_y, (float)_z);
				visited = false;
			}

			public DualSite3d(Vector3 pos)
			{
				coord = pos;
				visited = false;
			}

			public DualSite3d(double _x, double _y, double _z, Site _originalSite)
				: this(_x, _y, _z)
			{
				site = _originalSite;
				visited = false;
			}
		}

		public const Winding ForcedWinding = Winding.COUNTERCLOCKWISE;

		public MapGraph mg = new MapGraph(0);

		private Polygon bounds;

		private List<Site> externalEdgePoints = new List<Site>();

		private float weightSum;

		private List<Site> sites = new List<Site>();

		private List<DualSite2d> dualSites = new List<DualSite2d>();

		public VoronoiMesh<DualSite2d, Site, VoronoiEdge<DualSite2d, Site>> voronoiMesh
		{
			get;
			private set;
		}

		public int completedIterations
		{
			get;
			set;
		}

		public PowerDiagram(Polygon polyBounds, IEnumerable<Site> newSites)
		{
			bounds = polyBounds;
			bounds.ForceWinding(Winding.COUNTERCLOCKWISE);
			weightSum = 0f;
			sites.Clear();
			IEnumerator<Site> enumerator = newSites.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				if (!bounds.Contains(enumerator.Current.position))
				{
					Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", enumerator.Current.id, enumerator.Current.position);
				}
				if (bounds.Contains(enumerator.Current.position))
				{
					AddSite(enumerator.Current);
				}
				num++;
			}
			Vector2 b = bounds.Centroid();
			for (int i = 0; i < bounds.Vertices.Count; i++)
			{
				Vector2 vector = bounds.Vertices[i];
				Vector2 vector2 = bounds.Vertices[(i < bounds.Vertices.Count - 1) ? (i + 1) : 0];
				Vector2 b2 = (vector - b).normalized * 1000f;
				Vector2 pos = vector + b2;
				Site site = new Site(pos)
				{
					dummy = true
				};
				externalEdgePoints.Add(site);
				site.weight = Mathf.Epsilon;
				site.currentWeight = Mathf.Epsilon;
				dualSites.Add(new DualSite2d(site));
				Vector2 a = (vector2 - vector) * 0.5f + vector2;
				Vector2 b3 = (a - b).normalized * 1000f;
				Vector2 pos2 = vector2 + b3;
				Site site2 = new Site(pos2)
				{
					dummy = true,
					weight = Mathf.Epsilon,
					currentWeight = Mathf.Epsilon
				};
				externalEdgePoints.Add(site2);
				dualSites.Add(new DualSite2d(site2));
			}
		}

		public List<Site> GetSites()
		{
			return sites;
		}

		public void ComputePowerDiagram(int maxIterations, float threashold = 1f)
		{
			completedIterations = 0;
			float num = 0f;
			foreach (Site site in sites)
			{
				if (site.poly == null)
				{
					throw new Exception("site poly is null for [" + site.id + "]" + site.position);
				}
				site.position = site.poly.Centroid();
			}
			for (int i = 0; i <= maxIterations; i++)
			{
				try
				{
					UpdateWeights(sites);
					ComputePD();
				}
				catch (Exception ex)
				{
					Debug.LogError("Error [" + num + "] iters " + completedIterations + "/" + maxIterations + " Exception:" + ex.Message + "\n" + ex.StackTrace, null);
					return;
				}
				num = 0f;
				foreach (Site site2 in sites)
				{
					float num2 = (site2.poly != null) ? site2.poly.Area() : 0.1f;
					float num3 = site2.weight / weightSum * bounds.Area();
					num = Mathf.Max(Mathf.Abs(num2 - num3) / num3, num);
				}
				if (num < threashold)
				{
					completedIterations = i;
					break;
				}
				completedIterations++;
			}
			Debug.Log("error [" + num + "] iters " + completedIterations + "/" + maxIterations, null);
		}

		public void ComputeVD()
		{
			voronoiMesh = VoronoiMesh.Create<DualSite2d, Site>(dualSites);
			foreach (Site vertex in voronoiMesh.Vertices)
			{
				Vector2 circumcenter = vertex.Circumcenter;
				Cell cell = mg.GetCell(circumcenter);
				DualSite2d[] vertices = vertex.Vertices;
				foreach (DualSite2d dualSite2d in vertices)
				{
					if (!dualSite2d.visited)
					{
						dualSite2d.visited = true;
						if (!dualSite2d.site.dummy)
						{
							List<Vector2> list = new List<Vector2>();
							dualSite2d.site.neighbours = TouchingFaces(dualSite2d, vertex);
							foreach (Site neighbour in dualSite2d.site.neighbours)
							{
								Vector2 circumcenter2 = neighbour.Circumcenter;
								Color red = Color.red;
								red.a = 0.3f;
								list.Add(circumcenter2);
								Vector2 position = circumcenter2;
								Cell cell2 = mg.GetCell(position);
								if (cell != null && cell2 != null)
								{
									cell.Add(cell2);
									cell2.Add(cell);
									Corner corner = mg.GetCorner(circumcenter, true);
									Corner corner2 = mg.GetCorner(position, true);
									cell.Add(corner);
									cell.Add(corner2);
									cell2.Add(corner);
									cell2.Add(corner2);
								}
							}
							if (list.Count > 0)
							{
								Polygon polygon = PolyForRandomPoints(list);
								dualSite2d.site.poly = polygon.Clip(bounds, ClipType.ctIntersection);
							}
						}
					}
				}
			}
			ClipNeighbors();
		}

		public void ComputeVD3d()
		{
			List<DualSite3d> list = new List<DualSite3d>();
			foreach (Site site in sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < externalEdgePoints.Count; i++)
			{
				list.Add(externalEdgePoints[i].ToDualSite());
			}
			VoronoiMesh<DualSite3d, TriangulationCellExt<DualSite3d>, VoronoiEdge<DualSite3d, TriangulationCellExt<DualSite3d>>> voronoiMesh = VoronoiMesh.Create<DualSite3d, TriangulationCellExt<DualSite3d>>(list);
			foreach (TriangulationCellExt<DualSite3d> vertex in voronoiMesh.Vertices)
			{
				Vector3 a = Vector3.zero;
				DualSite3d[] vertices = vertex.Vertices;
				foreach (DualSite3d dualSite3d in vertices)
				{
					a += dualSite3d.coord;
				}
				a *= 0.333333343f;
				DebugExtension.DebugPoint(a, Color.red, 1f, 0f, true);
			}
		}

		private bool ContainsVert(Site face, DualSite2d target)
		{
			if (face != null && face.Vertices != null)
			{
				for (int i = 0; i < face.Vertices.Length; i++)
				{
					if (face.Vertices[i] == target)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void AddSite(Site site)
		{
			weightSum += site.weight;
			site.currentWeight = site.weight;
			sites.Add(site);
			dualSites.Add(new DualSite2d(site));
		}

		private List<Site> TouchingFaces(DualSite2d site, Site startingFace)
		{
			List<Site> list = new List<Site>();
			Stack<Site> stack = new Stack<Site>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				Site site2 = stack.Pop();
				if (ContainsVert(site2, site) && !list.Contains(site2))
				{
					list.Add(site2);
					for (int i = 0; i < site2.Adjacency.Length; i++)
					{
						if (ContainsVert(site2.Adjacency[i], site))
						{
							stack.Push(site2.Adjacency[i]);
						}
					}
				}
			}
			return list;
		}

		private void ClipNeighbors()
		{
			foreach (Site site in sites)
			{
				Cell cell = mg.GetCell(site.position);
				if (cell != null && cell.corners != null && cell.corners.Count > 2)
				{
					Cell cell2 = mg.GetCell(site.position);
					if (site.poly != null)
					{
						foreach (ProcGen.Map.Edge edge in cell2.edges)
						{
							LineSegment segment = new LineSegment(edge.corner0.position, edge.corner1.position);
							LineSegment intersectingSegment = new LineSegment(null, null);
							Vector2 normNear = Vector2.zero;
							Vector2 normFar = Vector2.zero;
							bool flag = site.poly.ClipSegment(segment, ref intersectingSegment, ref normNear, ref normFar);
							Vector2? p = intersectingSegment.p0;
							if (p.HasValue)
							{
								Vector2? p2 = intersectingSegment.p1;
								if (p2.HasValue && flag)
								{
									edge.corner0.SetPosition(intersectingSegment.p0.Value);
									edge.corner1.SetPosition(intersectingSegment.p1.Value);
								}
							}
						}
					}
				}
			}
		}

		private ConvexFaceExt<DualSite3d> GetNeigborFaceForEdge(ConvexFaceExt<DualSite3d> currentFace, DualSite3d sharedVert0, DualSite3d sharedVert1)
		{
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt != null)
				{
					int num = 0;
					for (int j = 0; j < convexFaceExt.Vertices.Length; j++)
					{
						if (sharedVert0 == convexFaceExt.Vertices[j])
						{
							num++;
						}
						else if (sharedVert1 == convexFaceExt.Vertices[j])
						{
							num++;
						}
						if (num == 2)
						{
							return convexFaceExt;
						}
					}
				}
			}
			return null;
		}

		private Edge GetEdge(ConvexFaceExt<DualSite3d> face0, ConvexFaceExt<DualSite3d> face1)
		{
			Edge edge = null;
			for (int i = 0; i < face0.Vertices.Length; i++)
			{
				for (int j = 0; j < face1.Vertices.Length; j++)
				{
					if (face0.Vertices[i] == face1.Vertices[j])
					{
						if (edge == null)
						{
							edge = new Edge(face0.Vertices[i], null);
						}
						else
						{
							edge.Second = face0.Vertices[i];
						}
					}
				}
			}
			return edge;
		}

		private bool ContainsVert(ConvexFaceExt<DualSite3d> face, DualSite3d target)
		{
			for (int i = 0; i < face.Vertices.Length; i++)
			{
				if (face.Vertices[i] == target)
				{
					return true;
				}
			}
			return false;
		}

		private List<ConvexFaceExt<DualSite3d>> TouchingFaces(DualSite3d site, ConvexFaceExt<DualSite3d> startingFace)
		{
			List<ConvexFaceExt<DualSite3d>> list = new List<ConvexFaceExt<DualSite3d>>();
			Stack<ConvexFaceExt<DualSite3d>> stack = new Stack<ConvexFaceExt<DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = stack.Pop();
				if (ContainsVert(convexFaceExt, site) && !list.Contains(convexFaceExt))
				{
					list.Add(convexFaceExt);
					for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
					{
						if (ContainsVert(convexFaceExt.Adjacency[i], site) && !list.Contains(convexFaceExt.Adjacency[i]))
						{
							stack.Push(convexFaceExt.Adjacency[i]);
						}
					}
				}
			}
			return list;
		}

		private List<Site> GenerateNeighbors(DualSite3d dualSite, ConvexFaceExt<DualSite3d> startingFace)
		{
			List<Site> list = new List<Site>();
			List<ConvexFaceExt<DualSite3d>> list2 = new List<ConvexFaceExt<DualSite3d>>();
			Stack<ConvexFaceExt<DualSite3d>> stack = new Stack<ConvexFaceExt<DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = stack.Pop();
				for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
				{
					if (ContainsVert(convexFaceExt.Adjacency[i], dualSite) && !list2.Contains(convexFaceExt.Adjacency[i]))
					{
						Edge edge = GetEdge(convexFaceExt, convexFaceExt.Adjacency[i]);
						DualSite3d dualSite3d = (edge.First != dualSite) ? edge.First : edge.Second;
						list.Add(dualSite3d.site);
						list2.Add(convexFaceExt.Adjacency[i]);
						stack.Push(convexFaceExt.Adjacency[i]);
					}
				}
			}
			return list;
		}

		private void ComputePD()
		{
			List<DualSite3d> list = new List<DualSite3d>();
			foreach (Site site in sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < externalEdgePoints.Count; i++)
			{
				list.Add(externalEdgePoints[i].ToDualSite());
			}
			CheckPositions(list);
			ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>> convexHull = CreateHull(list, 1E-10);
			foreach (ConvexFaceExt<DualSite3d> face in convexHull.Faces)
			{
				if (!(face.Normal[2] >= (double)(0f - Mathf.Epsilon)))
				{
					DualSite3d[] vertices = face.Vertices;
					foreach (DualSite3d dualSite3d in vertices)
					{
						if (!dualSite3d.site.dummy && !dualSite3d.visited)
						{
							dualSite3d.visited = true;
							List<Vector2> list2 = new List<Vector2>();
							List<ConvexFaceExt<DualSite3d>> list3 = TouchingFaces(dualSite3d, face);
							dualSite3d.site.neighbours = GenerateNeighbors(dualSite3d, face);
							foreach (ConvexFaceExt<DualSite3d> item in list3)
							{
								Vector2 dualPoint = item.GetDualPoint();
								list2.Add(dualPoint);
							}
							Polygon polygon = PolyForRandomPoints(list2);
							Polygon polygon2 = polygon.Clip(bounds, ClipType.ctIntersection);
							if (polygon2 == null)
							{
								polygon.DebugDraw(Color.yellow, false, 1f, 0f);
								dualSite3d.site.poly.DebugDraw(Color.black, false, 1f, 2f);
								DebugExtension.DebugCircle2d(dualSite3d.site.position, Color.magenta, 5f, 0f, true, 20f);
							}
							else
							{
								dualSite3d.site.poly = polygon2;
							}
						}
					}
				}
			}
		}

		private void UpdateWeights(List<Site> sites)
		{
			foreach (Site site in sites)
			{
				if (site.poly == null)
				{
					throw new Exception("site poly is null for [" + site.id + "]" + site.position);
				}
				site.position = site.poly.Centroid();
				site.currentWeight = Mathf.Max(site.currentWeight, 1f);
			}
			float num = 0f;
			foreach (Site site2 in sites)
			{
				float num2 = (site2.poly != null) ? site2.poly.Area() : 0.1f;
				float num3 = site2.weight / weightSum * bounds.Area();
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
				foreach (Site site3 in sites)
				{
					site3.currentWeight += num + 1f;
				}
			}
			float num9 = 1f;
			foreach (Site site4 in sites)
			{
				foreach (Site neighbour in site4.neighbours)
				{
					float num10 = (site4.position - neighbour.position).sqrMagnitude / (Mathf.Abs(site4.currentWeight - neighbour.currentWeight) + 1f);
					if (num10 < num9)
					{
						num9 = num10;
					}
				}
			}
			foreach (Site site5 in sites)
			{
				site5.currentWeight *= num9;
			}
		}

		private List<ConvexFaceExt<DualSite3d>> GetNeigborFaces(ConvexFaceExt<DualSite3d> currentFace)
		{
			List<ConvexFaceExt<DualSite3d>> list = new List<ConvexFaceExt<DualSite3d>>();
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt != null)
				{
					list.Add(convexFaceExt);
				}
			}
			return list;
		}

		private void CheckPositions(List<DualSite3d> dual3dSites)
		{
			for (int i = 0; i < dual3dSites.Count; i++)
			{
				if (!dual3dSites[i].site.dummy)
				{
					for (int j = i + 1; j < dual3dSites.Count; j++)
					{
						if (!dual3dSites[j].site.dummy && dual3dSites[i].coord == dual3dSites[j].coord)
						{
							dual3dSites[j].coord += new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, 0f);
						}
					}
				}
			}
		}

		public static Polygon PolyForRandomPoints(List<Vector2> verts)
		{
			double[][] array = new double[verts.Count][];
			for (int i = 0; i < verts.Count; i++)
			{
				double[][] array2 = array;
				int num = i;
				double[] obj = new double[2];
				Vector2 vector = verts[i];
				obj[0] = (double)vector.x;
				Vector2 vector2 = verts[i];
				obj[1] = (double)vector2.y;
				array2[num] = obj;
			}
			ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull = ConvexHull.Create(array, 1E-10);
			double[][] array3 = (from p in convexHull.Points
			select p.Position).ToArray();
			Polygon polygon = new Polygon();
			for (int j = 0; j < array3.Length; j++)
			{
				polygon.Add(new Vector2((float)array3[j][0], (float)array3[j][1]));
			}
			polygon.Initialize();
			polygon.ForceWinding(Winding.COUNTERCLOCKWISE);
			return polygon;
		}

		public static ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>> CreateHull(IList<DualSite3d> data, double PlaneDistanceTolerance = 1E-10)
		{
			return ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>>.Create(data, PlaneDistanceTolerance);
		}
	}
}
