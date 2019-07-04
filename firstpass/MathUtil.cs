using UnityEngine;

public static class MathUtil
{
	public struct MinMax
	{
		public float min
		{
			get;
			private set;
		}

		public float max
		{
			get;
			private set;
		}

		public MinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float Get(SeededRandom rnd)
		{
			return rnd.RandomRange(min, max);
		}

		public float Get()
		{
			return Random.Range(min, max);
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(min, max, t);
		}

		public override string ToString()
		{
			return $"[{min}:{max}]";
		}
	}

	public class Pair<T, U>
	{
		public T First
		{
			get;
			set;
		}

		public U Second
		{
			get;
			set;
		}

		public Pair()
		{
		}

		public Pair(T first, U second)
		{
			First = first;
			Second = second;
		}
	}

	public static float Clamp(float min, float max, float val)
	{
		return Mathf.Max(min, Mathf.Min(max, val));
	}

	public static int Clamp(int min, int max, int val)
	{
		return Mathf.Max(min, Mathf.Min(max, val));
	}

	public static float ReRange(float val, float in_a, float in_b, float out_a, float out_b)
	{
		return (val - in_a) / (in_b - in_a) * (out_b - out_a) + out_a;
	}

	public static float Wrap(float min, float max, float val)
	{
		while (val < min)
		{
			val += max - min;
		}
		while (val > max)
		{
			val -= max - min;
		}
		return val;
	}

	public static float ApproachConstant(float target, float current, float speed)
	{
		float num = target - current;
		if (!(num > speed))
		{
			if (!(num < 0f - speed))
			{
				return target;
			}
			return current - speed;
		}
		return current + speed;
	}

	public static Vector3 ApproachConstant(Vector3 target, Vector3 current, float speed)
	{
		Vector3 vector = target - current;
		float magnitude = vector.magnitude;
		if (!(magnitude > speed))
		{
			return target;
		}
		return current + vector.normalized * speed;
	}

	public static Vector3 Round(this Vector3 v)
	{
		return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
	}

	public static Vector3 Min(this Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
	}

	public static Vector3 Max(this Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
	}

	public static Vector3[] RaySphereIntersection(Ray ray, Vector3 sphereCenter, float sphereRadius)
	{
		ray.direction.Normalize();
		Vector3 vector = sphereCenter - ray.origin;
		float num = Vector3.Dot(ray.direction, vector);
		float num2 = Vector3.Dot(vector, vector);
		float num3 = num * num - num2 + sphereRadius * sphereRadius;
		if (!(num3 < 0f))
		{
			if (num3 != 0f)
			{
				Vector3 vector2 = (num - Mathf.Sqrt(num3)) * ray.direction + ray.origin;
				Vector3 vector3 = (num + Mathf.Sqrt(num3)) * ray.direction + ray.origin;
				return new Vector3[2]
				{
					vector2,
					vector3
				};
			}
			Vector3 vector4 = num * ray.direction + ray.origin;
			return new Vector3[1]
			{
				vector4
			};
		}
		return new Vector3[0];
	}

	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}

	public static float GetClosestPointBetweenPointAndLineSegment(Pair<Vector2, Vector2> segment, Vector2 point, ref float closest_point)
	{
		Vector2 second = segment.Second;
		float x = second.x;
		Vector2 first = segment.First;
		float num = x - first.x;
		Vector2 second2 = segment.Second;
		float x2 = second2.x;
		Vector2 first2 = segment.First;
		float num2 = num * (x2 - first2.x);
		Vector2 second3 = segment.Second;
		float y = second3.y;
		Vector2 first3 = segment.First;
		float num3 = y - first3.y;
		Vector2 second4 = segment.Second;
		float y2 = second4.y;
		Vector2 first4 = segment.First;
		float num4 = num2 + num3 * (y2 - first4.y);
		if (!(num4 <= 0f))
		{
			float x3 = point.x;
			Vector2 first5 = segment.First;
			float num5 = x3 - first5.x;
			Vector2 second5 = segment.Second;
			float x4 = second5.x;
			Vector2 first6 = segment.First;
			float num6 = num5 * (x4 - first6.x);
			float y3 = point.y;
			Vector2 first7 = segment.First;
			float num7 = y3 - first7.y;
			Vector2 second6 = segment.Second;
			float y4 = second6.y;
			Vector2 first8 = segment.First;
			float num8 = num6 + num7 * (y4 - first8.y);
			closest_point = Mathf.Max(0f, Mathf.Min(1f, num8 / num4));
			Vector2 a = segment.First + (segment.Second - segment.First) * closest_point;
			return Vector2.Distance(a, point);
		}
		closest_point = 0f;
		return Vector2.Distance(segment.First, point);
	}
}
