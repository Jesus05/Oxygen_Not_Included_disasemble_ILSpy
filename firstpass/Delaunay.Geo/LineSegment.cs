using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay.Geo
{
	public class LineSegment
	{
		public Vector2? p0;

		public Vector2? p1;

		public LineSegment(Vector2? p0, Vector2? p1)
		{
			this.p0 = p0;
			this.p1 = p1;
		}

		public static int CompareLengths_MAX(LineSegment segment0, LineSegment segment1)
		{
			Vector2? nullable = segment0.p0;
			Vector2 value = nullable.Value;
			Vector2? nullable2 = segment0.p1;
			float num = Vector2.Distance(value, nullable2.Value);
			Vector2? nullable3 = segment1.p0;
			Vector2 value2 = nullable3.Value;
			Vector2? nullable4 = segment1.p1;
			float num2 = Vector2.Distance(value2, nullable4.Value);
			if (!(num < num2))
			{
				if (!(num > num2))
				{
					return 0;
				}
				return -1;
			}
			return 1;
		}

		public static int CompareLengths(LineSegment edge0, LineSegment edge1)
		{
			return -CompareLengths_MAX(edge0, edge1);
		}

		public Vector2? Center()
		{
			Vector2? nullable = p0;
			if (nullable.HasValue)
			{
				Vector2? nullable2 = p1;
				if (nullable2.HasValue)
				{
					return p0.Value + 0.5f * Direction();
				}
				return p0;
			}
			return p1;
		}

		public Vector2 Direction()
		{
			Vector2? nullable = p0;
			if (nullable.HasValue)
			{
				Vector2? nullable2 = p1;
				if (nullable2.HasValue)
				{
					return p1.Value - p0.Value;
				}
			}
			return Vector2.zero;
		}

		private static float[] OverlapIntervals(float ub1, float ub2)
		{
			float val = Math.Min(ub1, ub2);
			float val2 = Math.Max(ub1, ub2);
			float num = Math.Max(0f, val);
			float num2 = Math.Min(1f, val2);
			if (!(num > num2))
			{
				if (num != num2)
				{
					return new float[2]
					{
						num,
						num2
					};
				}
				return new float[1]
				{
					num
				};
			}
			return new float[0];
		}

		private static Vector2[] OneD_Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float num = a2.x - a1.x;
			float num2 = a2.y - a1.y;
			float ub;
			float ub2;
			if (Math.Abs(num) > Math.Abs(num2))
			{
				ub = (b1.x - a1.x) / num;
				ub2 = (b2.x - a1.x) / num;
			}
			else
			{
				ub = (b1.y - a1.y) / num2;
				ub2 = (b2.y - a1.y) / num2;
			}
			List<Vector2> list = new List<Vector2>();
			float[] array = OverlapIntervals(ub, ub2);
			float[] array2 = array;
			foreach (float num3 in array2)
			{
				float x = a2.x * num3 + a1.x * (1f - num3);
				float y = a2.y * num3 + a1.y * (1f - num3);
				Vector2 item = new Vector2(x, y);
				list.Add(item);
			}
			return list.ToArray();
		}

		private static bool PointOnLine(Vector2 p, Vector2 a1, Vector2 a2)
		{
			float u = 0f;
			double num = DistFromSeg(p, a1, a2, (double)Mathf.Epsilon, ref u);
			return num < (double)Mathf.Epsilon;
		}

		private static double DistFromSeg(Vector2 p, Vector2 q0, Vector2 q1, double radius, ref float u)
		{
			double num = (double)(q1.x - q0.x);
			double num2 = (double)(q1.y - q0.y);
			double num3 = (double)(q0.x - p.x);
			double num4 = (double)(q0.y - p.y);
			double num5 = Math.Sqrt(num * num + num2 * num2);
			if (num5 < (double)Mathf.Epsilon)
			{
				throw new Exception("Expected line segment, not point.");
			}
			double num6 = Math.Abs(num * num4 - num3 * num2);
			return num6 / num5;
		}

		public bool DoesIntersect(LineSegment other)
		{
			return DoesIntersect(this, other);
		}

		public static bool DoesIntersect(LineSegment a, LineSegment b)
		{
			Vector2[] array = Intersection(a.p0.Value, a.p1.Value, b.p0.Value, b.p1.Value);
			if (array.Length <= 0)
			{
				return false;
			}
			return true;
		}

		public static LineSegment Intersection(LineSegment a, LineSegment b)
		{
			Vector2[] array = Intersection(a.p0.Value, a.p1.Value, b.p0.Value, b.p1.Value);
			if (array.Length != 1)
			{
				if (array.Length != 2)
				{
					return new LineSegment(null, null);
				}
				return new LineSegment(array[0], array[1]);
			}
			return new LineSegment(array[0], null);
		}

		public static Vector2[] Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			if (a1.Equals(a2) && b1.Equals(b2))
			{
				if (!a1.Equals(b1))
				{
					return new Vector2[0];
				}
				return new Vector2[1]
				{
					a1
				};
			}
			if (!b1.Equals(b2))
			{
				if (!a1.Equals(a2))
				{
					float num = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
					float num2 = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);
					float num3 = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
					if (0f - Mathf.Epsilon < num3 && num3 < Mathf.Epsilon)
					{
						if ((!(0f - Mathf.Epsilon < num) || !(num < Mathf.Epsilon)) && (!(0f - Mathf.Epsilon < num2) || !(num2 < Mathf.Epsilon)))
						{
							return new Vector2[0];
						}
						if (!a1.Equals(a2))
						{
							return OneD_Intersection(a1, a2, b1, b2);
						}
						return OneD_Intersection(b1, b2, a1, a2);
					}
					float num4 = num / num3;
					float num5 = num2 / num3;
					if (0f <= num4 && num4 <= 1f && 0f <= num5 && num5 <= 1f)
					{
						return new Vector2[1]
						{
							new Vector2(a1.x + num4 * (a2.x - a1.x), a1.y + num4 * (a2.y - a1.y))
						};
					}
					return new Vector2[0];
				}
				if (!PointOnLine(a1, b1, b2))
				{
					return new Vector2[0];
				}
				return new Vector2[1]
				{
					a1
				};
			}
			if (!PointOnLine(b1, a1, a2))
			{
				return new Vector2[0];
			}
			return new Vector2[1]
			{
				b1
			};
		}
	}
}
