using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	public static class Util
	{
		public static HashSet<Vector2> GetPointsOnHermiteCurve(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1, int numberOfPoints)
		{
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			Vector2 a = t0 - p0;
			Vector2 a2 = t1 - p1;
			float num = 1f / (float)numberOfPoints;
			for (int i = 0; i < numberOfPoints; i++)
			{
				float num2 = (float)i * num;
				Vector2 item = (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * p0 + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * a + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * p1 + (num2 * num2 * num2 - num2 * num2) * a2;
				hashSet.Add(item);
			}
			return hashSet;
		}

		public static HashSet<Vector2> GetPointsOnCatmullRomSpline(List<Vector2> controlPoints, int numberOfPoints)
		{
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			float num = 1f / (float)numberOfPoints;
			for (int i = 0; i < controlPoints.Count - 1; i++)
			{
				Vector2 a = controlPoints[i];
				Vector2 a2 = controlPoints[i + 1];
				Vector2 a3 = (i <= 0) ? (controlPoints[i + 1] - controlPoints[i]) : (0.5f * (controlPoints[i + 1] - controlPoints[i - 1]));
				Vector2 a4 = (i >= controlPoints.Count - 2) ? (controlPoints[i + 1] - controlPoints[i]) : (0.5f * (controlPoints[i + 2] - controlPoints[i]));
				if (i == controlPoints.Count - 2)
				{
					num = 1f / ((float)numberOfPoints - 1f);
				}
				for (int j = 0; j < numberOfPoints; j++)
				{
					float num2 = (float)j * num;
					Vector2 item = (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * a + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * a3 + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * a2 + (num2 * num2 * num2 - num2 * num2) * a4;
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		public static List<Vector2I> StaggerLine(Vector2 p0, Vector2 p1, int numberOfBreaks, SeededRandom rand, float staggerRange = 3f)
		{
			List<Vector2I> list = new List<Vector2I>();
			if (numberOfBreaks == 0)
			{
				return GetLine(p0, p1);
			}
			Vector2 a = p1 - p0;
			Vector2 p2 = p0;
			Vector2 vector = p1;
			for (int i = 0; i < numberOfBreaks; i++)
			{
				vector = p0 + a * (1f / (float)numberOfBreaks) * (float)i + Vector2.one * rand.RandomRange(0f - staggerRange, staggerRange);
				list.AddRange(GetLine(p2, vector));
				p2 = vector;
			}
			list.AddRange(GetLine(vector, p1));
			return list;
		}

		public static List<Vector2I> GetLine(Vector2 p0, Vector2 p1)
		{
			List<Vector2I> list = new List<Vector2I>();
			Vector2 vector = p1 - p0;
			float num = Mathf.Abs(vector.x);
			float num2 = Mathf.Abs(vector.y);
			int num3 = -1;
			if (p0.x < p1.x)
			{
				num3 = 1;
			}
			int num4 = -1;
			if (p0.y < p1.y)
			{
				num4 = 1;
			}
			float num5 = 0f;
			for (int i = 0; (float)i < num + num2; i++)
			{
				list.Add(new Vector2I(Mathf.FloorToInt(p0.x), Mathf.FloorToInt(p0.y)));
				float num6 = num5 + num2;
				float num7 = num5 - num;
				if (Mathf.Abs(num6) < Mathf.Abs(num7))
				{
					p0.x += (float)num3;
					num5 = num6;
				}
				else
				{
					p0.y += (float)num4;
					num5 = num7;
				}
			}
			return list;
		}

		public static List<Vector2> GetCircle(Vector2 center, int radius)
		{
			int num = radius;
			int num2 = 0;
			int num3 = 1 - num;
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			while (num >= num2)
			{
				hashSet.Add(new Vector2((float)num + center.x, (float)num2 + center.y));
				hashSet.Add(new Vector2((float)num2 + center.x, (float)num + center.y));
				hashSet.Add(new Vector2((float)(-num) + center.x, (float)num2 + center.y));
				hashSet.Add(new Vector2((float)(-num2) + center.x, (float)num + center.y));
				hashSet.Add(new Vector2((float)(-num) + center.x, (float)(-num2) + center.y));
				hashSet.Add(new Vector2((float)(-num2) + center.x, (float)(-num) + center.y));
				hashSet.Add(new Vector2((float)num + center.x, (float)(-num2) + center.y));
				hashSet.Add(new Vector2((float)num2 + center.x, (float)(-num) + center.y));
				num2++;
				if (num3 < 0)
				{
					num3 += 2 * num2 + 1;
				}
				else
				{
					num--;
					num3 += 2 * (num2 - num) + 1;
				}
			}
			return new List<Vector2>(hashSet);
		}

		private static void get8points(Vector2 c, float x, float y, List<Vector2I> points)
		{
			Vector2 p = new Vector2(c.x - x, c.y + y);
			Vector2 p2 = new Vector2(c.x + x, c.y + y);
			List<Vector2I> line = GetLine(p, p2);
			points.AddRange(line);
			Vector2 p3 = new Vector2(c.x - x, c.y - y);
			Vector2 p4 = new Vector2(c.x + x, c.y - y);
			line = GetLine(p3, p4);
			points.AddRange(line);
			if (x != y)
			{
				p = new Vector2(c.x - y, c.y + x);
				p2 = new Vector2(c.x + y, c.y + x);
				line = GetLine(p, p2);
				points.AddRange(line);
				p3 = new Vector2(c.x - y, c.y - x);
				p4 = new Vector2(c.x + y, c.y - x);
				line = GetLine(p3, p4);
				points.AddRange(line);
			}
		}

		public static List<Vector2I> GetFilledCircle(Vector2 center, float radius)
		{
			radius = Mathf.Floor(radius);
			List<Vector2I> list = new List<Vector2I>();
			float num = 0f - radius;
			float num2 = radius;
			float num3 = 0f;
			while (num2 >= num3)
			{
				get8points(center, num2, num3, list);
				num += num3;
				num3 += 1f;
				num += num3;
				if (num >= 0f)
				{
					num -= num2;
					num2 -= 1f;
					num -= num2;
				}
			}
			return list;
		}

		public static Vector2 RandomInUnitCircle(System.Random rng = null)
		{
			if (rng == null)
			{
				return UnityEngine.Random.insideUnitCircle;
			}
			double d = rng.NextDouble();
			double num = rng.NextDouble();
			double num2 = Math.Sqrt(d);
			return new Vector2((float)(num2 * Math.Cos(num)), (float)(num2 * Math.Sin(num)));
		}

		public static List<Vector2I> GetBlob(Vector2 center, float radius, System.Random rng)
		{
			List<Vector2> circle = GetCircle(center, (int)Mathf.Ceil(radius + 0.5f));
			circle.ShuffleSeeded(rng);
			for (int i = 0; i < circle.Count; i++)
			{
				List<Vector2> list;
				int index;
				(list = circle)[index = i] = list[index] + RandomInUnitCircle(rng) * radius;
			}
			HashSet<Vector2> pointsOnCatmullRomSpline = GetPointsOnCatmullRomSpline(circle, (int)(2f * radius * radius));
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			foreach (Vector2 item in pointsOnCatmullRomSpline)
			{
				Vector2 current = item;
				hashSet.Add(new Vector2I((int)current.x, (int)current.y));
			}
			return new List<Vector2I>(hashSet);
		}

		public static List<Vector2I> GetSplat(Vector2 center, float radius, System.Random rng)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			float num = 6.28318548f * radius;
			int num2 = Mathf.RoundToInt(num * 1f);
			for (int i = 0; i < num2; i++)
			{
				float num3 = (float)rng.NextDouble();
				num3 *= num3;
				float num4 = num3 * radius;
				float f = 6.28318548f * ((float)i / (float)num2);
				float x = Mathf.Sin(f) * num4;
				float y = Mathf.Cos(f) * num4;
				foreach (Vector2I item in GetLine(center, new Vector2(x, y) + center))
				{
					hashSet.Add(item);
				}
			}
			return new List<Vector2I>(hashSet);
		}

		public static List<Vector2I> GetBorder(HashSet<Vector2I> sourcePoints, int radius)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			IEnumerator<Vector2I> enumerator = sourcePoints.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				Vector2I current = enumerator.Current;
				int x = current.x;
				Vector2I current2 = enumerator.Current;
				int y = current2.y;
				for (int i = x - radius; i <= x + radius; i++)
				{
					for (int j = y - radius; j <= y + radius; j++)
					{
						if (i != x || j != y)
						{
							Vector2I item = new Vector2I(i, j);
							if (!sourcePoints.Contains(item))
							{
								hashSet.Add(item);
							}
						}
					}
				}
				num++;
			}
			return new List<Vector2I>(hashSet);
		}

		public static List<Vector2I> GetFilledRectangle(Vector2 center, float width, float height, SeededRandom rand, float jitterMaxStep = 2f, float jitterRange = 2f)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			if (width < 1f)
			{
				width = 1f;
			}
			if (height < 1f)
			{
				height = 1f;
			}
			float num = 0f;
			float num2 = 0f;
			int num3 = (int)(center.x - width / 2f);
			int num4 = (int)(center.x + width / 2f);
			int num5 = (int)(center.y - height / 2f);
			int num6 = (int)(center.y + height / 2f);
			for (int i = num5; i < num6; i++)
			{
				num = Mathf.Max(0f - jitterRange, Mathf.Min(num + rand.RandomRange(0f - jitterMaxStep, jitterMaxStep), jitterRange));
				num2 = Mathf.Max(0f - jitterRange, Mathf.Min(num2 + rand.RandomRange(0f - jitterMaxStep, jitterMaxStep), jitterRange));
				for (int j = (int)((float)num3 - num); (float)j < (float)num4 + num2; j++)
				{
					hashSet.Add(new Vector2I(j, i));
				}
			}
			float num7 = 0f;
			float num8 = 0f;
			for (int k = num3; k < num4; k++)
			{
				num7 = Mathf.Max(0f - jitterRange, Mathf.Min(num7 + rand.RandomRange(0f - jitterMaxStep, jitterMaxStep), jitterRange));
				num8 = Mathf.Max(0f - jitterRange, Mathf.Min(num8 + rand.RandomRange(0f - jitterMaxStep, jitterMaxStep), jitterRange));
				for (int l = (int)((float)num5 - num7); l < num5; l++)
				{
					hashSet.Add(new Vector2I(k, l));
				}
				for (int m = num6; (float)m < (float)num6 + num8; m++)
				{
					hashSet.Add(new Vector2I(k, m));
				}
			}
			return new List<Vector2I>(hashSet);
		}

		public static T GetRandom<T>(this T[] tArray, SeededRandom rand)
		{
			return tArray[rand.RandomRange(0, tArray.Length)];
		}

		public static T GetRandom<T>(this List<T> tList, SeededRandom rand)
		{
			return tList[rand.RandomRange(0, tList.Count)];
		}
	}
}
