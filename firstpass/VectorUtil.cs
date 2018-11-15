using UnityEngine;

public static class VectorUtil
{
	public static bool Less(this Vector2 v, Vector2 other)
	{
		return v.x < other.x && v.y < other.y;
	}

	public static bool LessEqual(this Vector2 v, Vector2 other)
	{
		return v.x <= other.x && v.y <= other.y;
	}

	public static bool Less(this Vector3 v, Vector3 other)
	{
		return v.x < other.x && v.y < other.y && v.z < other.z;
	}

	public static bool LessEqual(this Vector3 v, Vector3 other)
	{
		return v.x <= other.x && v.y <= other.y && v.z <= other.z;
	}
}
