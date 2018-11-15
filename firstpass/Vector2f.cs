using KSerialization;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{x}, {y}")]
public struct Vector2f
{
	[Serialize]
	public float x;

	[Serialize]
	public float y;

	public float X
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

	public float Y
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

	public Vector2f(int a, int b)
	{
		x = (float)a;
		y = (float)b;
	}

	public Vector2f(float a, float b)
	{
		x = a;
		y = b;
	}

	public Vector2f(Vector2 src)
	{
		x = src.x;
		y = src.y;
	}

	public static bool operator ==(Vector2f u, Vector2f v)
	{
		return u.x == v.x && u.y == v.y;
	}

	public static bool operator !=(Vector2f u, Vector2f v)
	{
		return u.x != v.x || u.y != v.y;
	}

	public static implicit operator Vector2(Vector2f v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector2f(Vector2 v)
	{
		return new Vector2f(v.x, v.y);
	}

	public bool Equals(Vector2 v)
	{
		return v.x == x && v.y == y;
	}

	public override bool Equals(object obj)
	{
		try
		{
			Vector2f vector2f = (Vector2f)obj;
			return vector2f.x == x && vector2f.y == y;
		}
		catch
		{
			return false;
		}
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode();
	}

	public override string ToString()
	{
		return $"{x}, {y}";
	}
}
