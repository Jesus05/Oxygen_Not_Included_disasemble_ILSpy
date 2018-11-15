using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{x}, {y}, {z}")]
public struct Vector3F
{
	public float x;

	public float y;

	public float z;

	public Vector3F(float _x, float _y, float _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}

	public static Vector3F operator -(Vector3F v1, Vector3F v2)
	{
		return new Vector3F(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
	}

	public static Vector3F operator +(Vector3F v1, Vector3F v2)
	{
		return new Vector3F(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
	}

	public static Vector3F operator *(Vector3F v, float scalar)
	{
		return new Vector3F(v.x * scalar, v.y * scalar, v.z * scalar);
	}

	public static Vector3F operator *(float scalar, Vector3F v)
	{
		return new Vector3F(v.x * scalar, v.y * scalar, v.z * scalar);
	}

	public static Vector3F operator /(Vector3F v, float scalar)
	{
		return new Vector3F(v.x / scalar, v.y / scalar, v.z / scalar);
	}

	public static Vector3F operator /(float scalar, Vector3F v)
	{
		return new Vector3F(v.x / scalar, v.y / scalar, v.z / scalar);
	}

	public static bool operator <(Vector3F v1, Vector3F v2)
	{
		return v1.x < v2.x && v1.y < v2.y && v1.z < v2.z;
	}

	public static bool operator >(Vector3F v1, Vector3F v2)
	{
		return v1.x > v2.x && v1.y > v2.y && v1.z > v2.z;
	}

	public static bool operator <=(Vector3F v1, Vector3F v2)
	{
		return v1.x <= v2.x && v1.y <= v2.y && v1.z <= v2.z;
	}

	public static bool operator >=(Vector3F v1, Vector3F v2)
	{
		return v1.x >= v2.x && v1.y >= v2.y && v1.z >= v2.z;
	}

	public static bool operator ==(Vector3F v1, Vector3F v2)
	{
		return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
	}

	public static bool operator !=(Vector3F v1, Vector3F v2)
	{
		return !(v1 == v2);
	}

	public override bool Equals(object o)
	{
		return ((ValueType)this).Equals(o);
	}

	public override int GetHashCode()
	{
		return ((ValueType)this).GetHashCode();
	}

	public override string ToString()
	{
		return $"{x}, {y}, {z}";
	}

	public static float Dot(Vector3F v1, Vector3F v2)
	{
		return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
	}

	public static implicit operator Vector3(Vector3F v)
	{
		return new Vector3(v.x, v.y, v.z);
	}
}
