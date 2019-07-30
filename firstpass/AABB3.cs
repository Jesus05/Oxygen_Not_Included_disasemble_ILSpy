using System;
using UnityEngine;

[Serializable]
public struct AABB3
{
	public Vector3 min;

	public Vector3 max;

	public Vector3 Center => (min + max) * 0.5f;

	public Vector3 Range => max - min;

	public float Width => max.x - min.x;

	public float Height => max.y - min.y;

	public float Depth => max.z - min.z;

	public AABB3(Vector3 pt)
	{
		min = pt;
		max = pt;
	}

	public AABB3(Vector3 min, Vector3 max)
	{
		this.min = min;
		this.max = max;
	}

	public bool IsValid()
	{
		return min.Min(max) == min;
	}

	public void Expand(float amount)
	{
		Vector3 vector = new Vector3(amount * 0.5f, amount * 0.5f, amount * 0.5f);
		min -= vector;
		max += vector;
	}

	public void ExpandToFit(Vector3 pt)
	{
		min = min.Min(pt);
		max = max.Max(pt);
	}

	public void ExpandToFit(AABB3 aabb)
	{
		min = min.Min(aabb.min);
		max = max.Max(aabb.max);
	}

	public bool Contains(Vector3 pt)
	{
		return min.LessEqual(pt) && pt.Less(max);
	}

	public bool Contains(AABB3 aabb)
	{
		return Contains(aabb.min) && Contains(aabb.max);
	}

	public bool Intersects(AABB3 aabb)
	{
		return min.LessEqual(aabb.max) && aabb.min.Less(max);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		AABB3 aABB = (AABB3)obj;
		return min == aABB.min && max == aABB.max;
	}

	public override int GetHashCode()
	{
		return min.GetHashCode() ^ max.GetHashCode();
	}

	public unsafe void Transform(Matrix4x4 t)
	{
		Vector3* ptr = stackalloc Vector3[8];
		*ptr = min;
		ptr[1] = new Vector3(min.x, min.y, max.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 2) = new Vector3(min.x, max.y, min.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 3) = new Vector3(max.x, min.y, min.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 4) = new Vector3(min.x, max.y, max.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 5) = new Vector3(max.x, min.y, max.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 6) = new Vector3(max.x, max.y, min.z);
		*(Vector3*)((byte*)ptr + sizeof(Vector3) * 7) = max;
		min = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
		max = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
		for (int i = 0; i < 8; i++)
		{
			ExpandToFit(t * (Vector4)ptr[i]);
		}
	}
}
