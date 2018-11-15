using UnityEngine;

public static class UIUtil
{
	public static Vector3[] corners = new Vector3[4];

	public static float worldHeight(this RectTransform rt)
	{
		rt.GetWorldCorners(corners);
		return corners[2].y - corners[0].y;
	}
}
