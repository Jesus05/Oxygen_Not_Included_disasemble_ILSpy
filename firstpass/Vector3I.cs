using System;
using System.Diagnostics;

[DebuggerDisplay("{x}, {y}, {z}")]
public struct Vector3I
{
	public int x;

	public int y;

	public int z;

	public Vector3I(int a, int b, int c)
	{
		x = a;
		y = b;
		z = c;
	}

	public static bool operator ==(Vector3I v1, Vector3I v2)
	{
		return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
	}

	public static bool operator !=(Vector3I v1, Vector3I v2)
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
}
