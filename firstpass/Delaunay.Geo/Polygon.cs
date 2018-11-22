using ClipperLib;
using KSerialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace Delaunay.Geo
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class Polygon
	{
		public enum Commonality
		{
			None,
			Point,
			Edge
		}

		[Serialize]
		private List<Vector2> vertices;

		private Vector2? centroid = null;

		private const int CLIPPER_INTEGER_SCALE = 10000;

		private const float CLIPPER_INVERSE_SCALE = 0.0001f;

		public Rect bounds
		{
			get;
			private set;
		}

		public List<Vector2> Vertices => vertices;

		public float MinX => vertices.Min((Vector2 point) => point.x);

		public float MinY => vertices.Min((Vector2 point) => point.y);

		public float MaxX => vertices.Max((Vector2 point) => point.x);

		public float MaxY => vertices.Max((Vector2 point) => point.y);

		public Polygon()
		{
		}

		public Polygon(List<Vector2> verts)
		{
			vertices = verts;
			Initialize();
		}

		public Polygon(Rect bounds)
		{
			vertices = new List<Vector2>();
			vertices.Add(new Vector2(bounds.x, bounds.y));
			vertices.Add(new Vector2(bounds.x + bounds.width, bounds.y));
			vertices.Add(new Vector2(bounds.x + bounds.width, bounds.y + bounds.height));
			vertices.Add(new Vector2(bounds.x, bounds.y + bounds.height));
			Initialize();
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			vertices = new List<Vector2>();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			Initialize();
		}

		public void Add(Vector2 newVert)
		{
			if (vertices == null)
			{
				vertices = new List<Vector2>();
			}
			vertices.Add(newVert);
		}

		public void Initialize()
		{
			Vector2 vector = new Vector2(3.40282347E+38f, 3.40282347E+38f);
			Vector2 vector2 = new Vector2(-3.40282347E+38f, -3.40282347E+38f);
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector3 = vertices[i];
				if (vector3.y < vector.y)
				{
					Vector2 vector4 = vertices[i];
					vector.y = vector4.y;
				}
				Vector2 vector5 = vertices[i];
				if (vector5.x < vector.x)
				{
					Vector2 vector6 = vertices[i];
					vector.x = vector6.x;
				}
				Vector2 vector7 = vertices[i];
				if (vector7.y > vector2.y)
				{
					Vector2 vector8 = vertices[i];
					vector2.y = vector8.y;
				}
				Vector2 vector9 = vertices[i];
				if (vector9.x > vector2.x)
				{
					Vector2 vector10 = vertices[i];
					vector2.x = vector10.x;
				}
			}
			bounds = Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
		}

		public float Area()
		{
			return Mathf.Abs(SignedDoubleArea() * 0.5f);
		}

		public Winding Winding()
		{
			float num = SignedDoubleArea();
			if (!(num < 0f))
			{
				if (!(num > 0f))
				{
					return Delaunay.Geo.Winding.NONE;
				}
				return Delaunay.Geo.Winding.COUNTERCLOCKWISE;
			}
			return Delaunay.Geo.Winding.CLOCKWISE;
		}

		public void ForceWinding(Winding wind)
		{
			if (Winding() != wind)
			{
				vertices.Reverse();
			}
		}

		private float SignedDoubleArea()
		{
			int count = vertices.Count;
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				int index = (i + 1) % count;
				Vector2 vector = vertices[i];
				Vector2 vector2 = vertices[index];
				num += vector.x * vector2.y - vector2.x * vector.y;
			}
			return num;
		}

		public Vector2 Centroid()
		{
			Vector2? nullable = centroid;
			if (!nullable.HasValue)
			{
				centroid = Vector2.zero;
				if (vertices.Count > 1)
				{
					float num = Area();
					int num2 = 1;
					for (int i = 0; i < vertices.Count; i++)
					{
						Vector2 vector = vertices[i];
						float x = vector.x;
						Vector2 vector2 = vertices[num2];
						float num3 = x * vector2.y;
						Vector2 vector3 = vertices[num2];
						float x2 = vector3.x;
						Vector2 vector4 = vertices[i];
						float num4 = num3 - x2 * vector4.y;
						Vector2? nullable2 = centroid;
						Vector2? obj;
						if (nullable2.HasValue)
						{
							Vector2 valueOrDefault = nullable2.GetValueOrDefault();
							Vector2 vector5 = vertices[i];
							float x3 = vector5.x;
							Vector2 vector6 = vertices[num2];
							float x4 = (x3 + vector6.x) * num4;
							Vector2 vector7 = vertices[i];
							float y = vector7.y;
							Vector2 vector8 = vertices[num2];
							obj = valueOrDefault + new Vector2(x4, (y + vector8.y) * num4);
						}
						else
						{
							obj = null;
						}
						centroid = obj;
						num2 = (num2 + 1) % vertices.Count;
					}
					centroid /= (float?)(6f * num);
				}
			}
			Vector2? nullable3 = centroid;
			return nullable3.Value;
		}

		public bool PointInPolygon(Vector2I point)
		{
			return PointInPolygon(new Vector2((float)point.x, (float)point.y));
		}

		public bool Contains(Vector2 point)
		{
			return PointInPolygon(point);
		}

		public bool PointInPolygon(Vector2 point)
		{
			if (bounds.Contains(point))
			{
				int index = vertices.Count - 1;
				bool flag = false;
				for (int num = 0; num < vertices.Count; index = num++)
				{
					Vector2 vector = vertices[num];
					if (vector.y <= point.y)
					{
						float y = point.y;
						Vector2 vector2 = vertices[index];
						if (y < vector2.y)
						{
							goto IL_00bc;
						}
					}
					Vector2 vector3 = vertices[index];
					if (!(vector3.y <= point.y))
					{
						continue;
					}
					float y2 = point.y;
					Vector2 vector4 = vertices[num];
					if (!(y2 < vector4.y))
					{
						continue;
					}
					goto IL_00bc;
					IL_00bc:
					float x = point.x;
					Vector2 vector5 = vertices[index];
					float x2 = vector5.x;
					Vector2 vector6 = vertices[num];
					float num2 = x2 - vector6.x;
					float y3 = point.y;
					Vector2 vector7 = vertices[num];
					float num3 = num2 * (y3 - vector7.y);
					Vector2 vector8 = vertices[index];
					float y4 = vector8.y;
					Vector2 vector9 = vertices[num];
					float num4 = num3 / (y4 - vector9.y);
					Vector2 vector10 = vertices[num];
					if (x < num4 + vector10.x)
					{
						flag = !flag;
					}
				}
				return flag;
			}
			return false;
		}

		public LineSegment GetEdge(int edgeIndex)
		{
			return new LineSegment(vertices[edgeIndex], vertices[(edgeIndex + 1) % vertices.Count]);
		}

		public Commonality SharesEdgeClosest(Polygon other)
		{
			Commonality result = Commonality.None;
			float timeOnEdge = 0f;
			MathUtil.Pair<Vector2, Vector2> closestEdge = GetClosestEdge(other.Centroid(), ref timeOnEdge);
			MathUtil.Pair<Vector2, Vector2> closestEdge2 = other.GetClosestEdge(Centroid(), ref timeOnEdge);
			if (!(Vector2.Distance(closestEdge.First, closestEdge2.First) < 1E-05f) && !(Vector2.Distance(closestEdge.First, closestEdge2.Second) < 1E-05f))
			{
				return result;
			}
			if (!(Vector2.Distance(closestEdge.Second, closestEdge2.First) < 1E-05f) && !(Vector2.Distance(closestEdge.Second, closestEdge2.Second) < 1E-05f))
			{
				return Commonality.Point;
			}
			return Commonality.Edge;
		}

		public Commonality SharesEdge(Polygon other, ref int edgeIdx)
		{
			Commonality result = Commonality.None;
			int num = vertices.Count - 1;
			int num2 = 0;
			while (num2 < vertices.Count)
			{
				Vector2 b = vertices[num];
				Vector2 b2 = vertices[num2];
				int index = other.vertices.Count - 1;
				int num3 = 0;
				while (num3 < other.vertices.Count)
				{
					Vector2 a = other.vertices[index];
					Vector2 a2 = other.vertices[num3];
					int num4 = 0;
					num4 += ((Vector2.Distance(a2, b2) < 0.001f) ? 1 : 0);
					num4 += ((Vector2.Distance(a2, b) < 0.001f) ? 1 : 0);
					num4 += ((Vector2.Distance(a, b2) < 0.001f) ? 1 : 0);
					num4 += ((Vector2.Distance(a, b) < 0.001f) ? 1 : 0);
					if (num4 == 1)
					{
						result = Commonality.Point;
					}
					if (num4 > 1)
					{
						edgeIdx = num;
						return Commonality.Edge;
					}
					index = num3++;
				}
				num = num2++;
			}
			return result;
		}

		public float DistanceToClosestEdge(Vector2? point = default(Vector2?))
		{
			if (!point.HasValue)
			{
				point = Centroid();
			}
			float timeOnEdge = 0f;
			MathUtil.Pair<Vector2, Vector2> closestEdge = GetClosestEdge(point.Value, ref timeOnEdge);
			Vector2 a = closestEdge.Second - closestEdge.First;
			Vector2 a2 = closestEdge.First + a * timeOnEdge;
			return Vector2.Distance(a2, point.Value);
		}

		public MathUtil.Pair<Vector2, Vector2> GetClosestEdge(Vector2 point, ref float timeOnEdge)
		{
			MathUtil.Pair<Vector2, Vector2> result = null;
			float closest_point = 0f;
			timeOnEdge = 0f;
			float num = 3.40282347E+38f;
			int index = vertices.Count - 1;
			int num2 = 0;
			while (num2 < vertices.Count)
			{
				MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(vertices[index], vertices[num2]);
				float num3 = Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, point, ref closest_point));
				if (num3 < num)
				{
					num = num3;
					result = pair;
					timeOnEdge = closest_point;
				}
				index = num2++;
			}
			return result;
		}

		public List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> GetEdgesWithinDistance(Vector2 point, float distance = float.MaxValue)
		{
			List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> list = new List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>>();
			float closest_point = 0f;
			int index = vertices.Count - 1;
			int num = 0;
			while (num < vertices.Count)
			{
				MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(vertices[index], vertices[num]);
				MathUtil.Pair<float, float> pair2 = new MathUtil.Pair<float, float>();
				float num2 = Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, point, ref closest_point));
				if (num2 < distance)
				{
					pair2.First = num2;
					pair2.Second = closest_point;
					list.Add(new KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>(pair2, pair));
				}
				index = num++;
			}
			list.Sort((KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> a, KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> b) => a.Key.First.CompareTo(b.Key.First));
			return list;
		}

		public bool IsConvex()
		{
			if (vertices.Count >= 4)
			{
				bool flag = false;
				int count = vertices.Count;
				for (int i = 0; i < count; i++)
				{
					Vector2 vector = vertices[(i + 2) % count];
					float x = vector.x;
					Vector2 vector2 = vertices[(i + 1) % count];
					double num = (double)(x - vector2.x);
					Vector2 vector3 = vertices[(i + 2) % count];
					float y = vector3.y;
					Vector2 vector4 = vertices[(i + 1) % count];
					double num2 = (double)(y - vector4.y);
					Vector2 vector5 = vertices[i];
					float x2 = vector5.x;
					Vector2 vector6 = vertices[(i + 1) % count];
					double num3 = (double)(x2 - vector6.x);
					Vector2 vector7 = vertices[i];
					float y2 = vector7.y;
					Vector2 vector8 = vertices[(i + 1) % count];
					double num4 = (double)(y2 - vector8.y);
					double num5 = num * num4 - num2 * num3;
					if (i == 0)
					{
						flag = (num5 > 0.0);
					}
					else if (flag != num5 > 0.0)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private List<IntPoint> GetPath()
		{
			List<IntPoint> list = new List<IntPoint>();
			for (int i = 0; i < vertices.Count; i++)
			{
				List<IntPoint> list2 = list;
				Vector2 vector = vertices[i];
				double x = (double)(vector.x * 10000f);
				Vector2 vector2 = vertices[i];
				list2.Add(new IntPoint(x, (double)(vector2.y * 10000f)));
			}
			return list;
		}

		public Polygon Clip(Polygon clippingPoly, ClipType type = ClipType.ctIntersection)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Add(GetPath());
			List<List<IntPoint>> list2 = new List<List<IntPoint>>();
			list2.Add(clippingPoly.GetPath());
			Clipper clipper = new Clipper(0);
			PolyTree polytree = new PolyTree();
			clipper.AddPaths(list, PolyType.ptSubject, true);
			clipper.AddPaths(list2, PolyType.ptClip, true);
			clipper.Execute(type, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
			List<List<IntPoint>> list3 = Clipper.PolyTreeToPaths(polytree);
			if (list3.Count <= 0)
			{
				return null;
			}
			List<Vector2> list4 = new List<Vector2>();
			for (int i = 0; i < list3[0].Count; i++)
			{
				List<Vector2> list5 = list4;
				IntPoint intPoint = list3[0][i];
				float x = (float)intPoint.X * 0.0001f;
				IntPoint intPoint2 = list3[0][i];
				list5.Add(new Vector2(x, (float)intPoint2.Y * 0.0001f));
			}
			return new Polygon(list4);
		}

		private int CrossingNumber(Vector2 point)
		{
			int num = 0;
			for (int i = 0; i < vertices.Count; i++)
			{
				int index = i;
				int index2 = (i < vertices.Count - 1) ? (i + 1) : 0;
				Vector2 vector = vertices[index];
				if (vector.y <= point.y)
				{
					Vector2 vector2 = vertices[index2];
					if (vector2.y > point.y)
					{
						goto IL_00ae;
					}
				}
				Vector2 vector3 = vertices[index];
				if (!(vector3.y > point.y))
				{
					continue;
				}
				Vector2 vector4 = vertices[index2];
				if (!(vector4.y <= point.y))
				{
					continue;
				}
				goto IL_00ae;
				IL_00ae:
				float y = point.y;
				Vector2 vector5 = vertices[index];
				float num2 = y - vector5.y;
				Vector2 vector6 = vertices[index2];
				float y2 = vector6.y;
				Vector2 vector7 = vertices[index];
				float num3 = num2 / (y2 - vector7.y);
				float x = point.x;
				Vector2 vector8 = vertices[index];
				float x2 = vector8.x;
				float num4 = num3;
				Vector2 vector9 = vertices[index2];
				float x3 = vector9.x;
				Vector2 vector10 = vertices[index];
				if (x < x2 + num4 * (x3 - vector10.x))
				{
					num++;
				}
			}
			return num & 1;
		}

		private float perp(Vector2 u, Vector2 v)
		{
			return u.x * v.y - u.y * v.x;
		}

		public bool ClipSegment(LineSegment segment, ref LineSegment intersectingSegment)
		{
			Vector2 normNear = Vector2.zero;
			Vector2 normFar = Vector2.zero;
			return ClipSegment(segment, ref intersectingSegment, ref normNear, ref normFar);
		}

		public bool ClipSegment(LineSegment segment, ref LineSegment intersectingSegment, ref Vector2 normNear, ref Vector2 normFar)
		{
			normNear = Vector2.zero;
			normFar = Vector2.zero;
			Vector2? p = segment.p0;
			bool hasValue = p.HasValue;
			Vector2? p2 = segment.p1;
			if (hasValue != p2.HasValue || (p.HasValue && !(p.GetValueOrDefault() == p2.GetValueOrDefault())))
			{
				float num = 0f;
				float num2 = 1f;
				Vector2 vector = segment.Direction();
				for (int i = 0; i < vertices.Count; i++)
				{
					int index = i;
					int index2 = (i < vertices.Count - 1) ? (i + 1) : 0;
					Vector2 u = vertices[index2] - vertices[index];
					Vector2 vector2 = new Vector2(u.y, 0f - u.x);
					float num3 = perp(u, segment.p0.Value - vertices[index]);
					float num4 = 0f - perp(u, vector);
					if (Mathf.Abs(num4) < Mathf.Epsilon)
					{
						if (num3 < 0f)
						{
							return false;
						}
					}
					else
					{
						float num5 = num3 / num4;
						if (num4 < 0f)
						{
							if (num5 > num)
							{
								num = num5;
								normNear = vector2;
								if (num > num2)
								{
									return false;
								}
							}
						}
						else if (num5 < num2)
						{
							num2 = num5;
							normFar = vector2;
							if (num2 < num)
							{
								return false;
							}
						}
					}
				}
				LineSegment obj = intersectingSegment;
				Vector2? p3 = segment.p0;
				obj.p0 = ((!p3.HasValue) ? null : new Vector2?(p3.GetValueOrDefault() + num * vector));
				LineSegment obj2 = intersectingSegment;
				Vector2? p4 = segment.p0;
				obj2.p1 = ((!p4.HasValue) ? null : new Vector2?(p4.GetValueOrDefault() + num2 * vector));
				normFar.Normalize();
				normNear.Normalize();
				return true;
			}
			intersectingSegment = segment;
			return CrossingNumber(segment.p0.Value) == 1;
		}

		public bool ClipSegmentSAT(LineSegment segment, ref LineSegment intersectingSegment, ref Vector2 normNear, ref Vector2 normFar)
		{
			normNear = Vector2.zero;
			normFar = Vector2.zero;
			float num = 0f;
			float num2 = 1f;
			Vector2 vector = segment.Direction();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector2 = vertices[i];
				Vector2 a = vertices[(i < vertices.Count - 1) ? (i + 1) : 0];
				Vector2 vector3 = a - vector2;
				Vector2 vector4 = new Vector2(vector3.y, 0f - vector3.x);
				Vector2 u = vector2 - segment.p0.Value;
				float num3 = perp(u, vector4);
				float num4 = perp(vector, vector4);
				if (Mathf.Abs(num4) < Mathf.Epsilon)
				{
					if (num3 < 0f)
					{
						return false;
					}
				}
				else
				{
					float num5 = num3 / num4;
					if (num4 < 0f)
					{
						if (num5 > num2)
						{
							return false;
						}
						if (num5 > num)
						{
							num = num5;
							normNear = vector4;
						}
					}
					else
					{
						if (num5 < num)
						{
							return false;
						}
						if (num5 < num2)
						{
							num2 = num5;
							normFar = vector4;
						}
					}
				}
			}
			LineSegment obj = intersectingSegment;
			Vector2? p = segment.p0;
			obj.p0 = ((!p.HasValue) ? null : new Vector2?(p.GetValueOrDefault() + num * vector));
			LineSegment obj2 = intersectingSegment;
			Vector2? p2 = segment.p0;
			obj2.p1 = ((!p2.HasValue) ? null : new Vector2?(p2.GetValueOrDefault() + num2 * vector));
			normFar.Normalize();
			normNear.Normalize();
			return true;
		}

		public void DebugDraw(Color colour, bool drawCentroid = false, float duration = 1f, float inset = 0f)
		{
			Vector2 b = Centroid();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 a = vertices[i];
				Vector2 a2 = vertices[(i < vertices.Count - 1) ? (i + 1) : 0];
				if (inset != 0f)
				{
					Vector2 vector = (a - b).normalized * (0f - inset);
					Vector2 vector2 = (a2 - b).normalized * (0f - inset);
				}
			}
			if (!drawCentroid)
			{
				return;
			}
		}
	}
}
