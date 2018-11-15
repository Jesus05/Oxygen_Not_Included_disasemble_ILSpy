using UnityEngine;

public static class GameTagExtensions
{
	public static GameObject Prefab(this Tag tag)
	{
		return Assets.GetPrefab(tag);
	}

	public static string ProperName(this Tag tag)
	{
		return TagManager.GetProperName(tag);
	}

	public static Tag Create(SimHashes id)
	{
		return TagManager.Create(id.ToString());
	}

	public static Tag CreateTag(this SimHashes id)
	{
		return TagManager.Create(id.ToString());
	}
}
