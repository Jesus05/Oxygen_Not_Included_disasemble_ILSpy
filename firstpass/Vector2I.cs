using KSerialization;
using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
[DebuggerDisplay("{x}, {y}")]
[SerializationConfig(MemberSerialization.OptIn)]
public struct Vector2I : IComparable<Vector2I>, IEquatable<Vector2I>
{
	public static readonly Vector2I zero = new Vector2I(0, 0);

	public static readonly Vector2I one = new Vector2I(1, 1);

	public static readonly Vector2I minusone = new Vector2I(-1, -1);

	[Serialize]
	public int x;

	[Serialize]
	public int y;

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int magnitudeSqr => x * x + y * y;

	public Vector2I(int a, int b)
	{
		x = a;
		y = b;
	}

	public static Vector2I operator +(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x + v.x, u.y + v.y);
	}

	public static Vector2I operator -(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x - v.x, u.y - v.y);
	}

	public static Vector2I operator *(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x * v.x, u.y * v.y);
	}

	public static Vector2I operator /(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x / v.x, u.y / v.y);
	}

	public static Vector2I operator *(Vector2I v, int s)
	{
		return new Vector2I(v.x * s, v.y * s);
	}

	public static Vector2I operator /(Vector2I v, int s)
	{
		return new Vector2I(v.x / s, v.y / s);
	}

	public static Vector2I operator +(Vector2I u, int scalar)
	{
		return new Vector2I(u.x + scalar, u.y + scalar);
	}

	public static Vector2I operator -(Vector2I u, int scalar)
	{
		return new Vector2I(u.x - scalar, u.y - scalar);
	}

	public static bool operator ==(Vector2I u, Vector2I v)
	{
		return u.x == v.x && u.y == v.y;
	}

	public static bool operator !=(Vector2I u, Vector2I v)
	{
		return u.x != v.x || u.y != v.y;
	}

	public static Vector2I Min(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x >= w.x) ? w.x : v.x, (v.y >= w.y) ? w.y : v.y);
	}

	public static Vector2I Max(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x <= w.x) ? w.x : v.x, (v.y <= w.y) ? w.y : v.y);
	}

	public static bool operator <(Vector2I u, Vector2I v)
	{
		return u.x < v.x && u.y < v.y;
	}

	public static bool operator >(Vector2I u, Vector2I v)
	{
		return u.x > v.x && u.y > v.y;
	}

	public static bool operator <=(Vector2I u, Vector2I v)
	{
		return u.x <= v.x && u.y <= v.y;
	}

	public static bool operator >=(Vector2I u, Vector2I v)
	{
		return u.x >= v.x && u.y >= v.y;
	}

	public static implicit operator Vector2(Vector2I v)
	{
		return new Vector2((float)v.x, (float)v.y);
	}

	public override bool Equals(object obj)
	{
		Vector2I vector2I = (Vector2I)obj;
		return vector2I.x == x && vector2I.y == y;
	}

	public bool Equals(Vector2I v)
	{
		return v.x == x && v.y == y;
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public static bool operator <=(Vector2I u, Vector2 v)
	{
		return (float)u.x <= v.x && (float)u.y <= v.y;
	}

	public static bool operator >=(Vector2I u, Vector2 v)
	{
		return (float)u.x >= v.x && (float)u.y >= v.y;
	}

	public static bool operator <=(Vector2 u, Vector2I v)
	{
		return u.x <= (float)v.x && u.y <= (float)v.y;
	}

	public static bool operator >=(Vector2 u, Vector2I v)
	{
		return u.x >= (float)v.x && u.y >= (float)v.y;
	}

	public override string ToString()
	{
		return $"{x}, {y}";
	}

	public int CompareTo(Vector2I other)
	{
		int result = y - other.y;
		if (other.y == 0)
		{
			return x - other.x;
		}
		return result;
	}
}
