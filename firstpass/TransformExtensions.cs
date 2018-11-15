using UnityEngine;

public static class TransformExtensions
{
	public static Vector3 GetPosition(this Transform transform)
	{
		return transform.position;
	}

	public static Vector3 SetPosition(this Transform transform, Vector3 position)
	{
		transform.position = position;
		if (Singleton<CellChangeMonitor>.Instance != null)
		{
			Singleton<CellChangeMonitor>.Instance.MarkDirty(transform);
		}
		return position;
	}

	public static Vector3 GetLocalPosition(this Transform transform)
	{
		return transform.localPosition;
	}

	public static Vector3 SetLocalPosition(this Transform transform, Vector3 position)
	{
		transform.localPosition = position;
		if (Singleton<CellChangeMonitor>.Instance != null)
		{
			Singleton<CellChangeMonitor>.Instance.MarkDirty(transform);
		}
		return position;
	}
}
