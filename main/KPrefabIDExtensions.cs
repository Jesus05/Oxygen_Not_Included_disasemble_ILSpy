using UnityEngine;

public static class KPrefabIDExtensions
{
	public static Tag PrefabID(this Component cmp)
	{
		return cmp.gameObject.PrefabID();
	}

	public static Tag PrefabID(this GameObject go)
	{
		return go.GetComponent<KPrefabID>().PrefabTag;
	}

	public static bool HasTag(this Component cmp, Tag tag)
	{
		return cmp.gameObject.HasTag(tag);
	}

	public static bool HasTag(this GameObject go, Tag tag)
	{
		return go.GetComponent<KPrefabID>().HasTag(tag);
	}

	public static void AddTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().AddTag(tag, false);
	}

	public static void AddTag(this Component cmp, Tag tag)
	{
		cmp.gameObject.AddTag(tag);
	}

	public static void RemoveTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().RemoveTag(tag);
	}

	public static void RemoveTag(this Component cmp, Tag tag)
	{
		cmp.gameObject.RemoveTag(tag);
	}
}
