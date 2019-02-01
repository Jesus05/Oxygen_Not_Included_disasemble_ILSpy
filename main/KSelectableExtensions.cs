using UnityEngine;

public static class KSelectableExtensions
{
	public static string GetProperName(this Component cmp)
	{
		if ((Object)cmp != (Object)null && (Object)cmp.gameObject != (Object)null)
		{
			return cmp.gameObject.GetProperName();
		}
		return "";
	}

	public static string GetProperName(this GameObject go)
	{
		if ((Object)go != (Object)null)
		{
			KSelectable component = go.GetComponent<KSelectable>();
			if ((Object)component != (Object)null)
			{
				return component.GetName();
			}
		}
		return "";
	}

	public static string GetProperName(this KSelectable cmp)
	{
		if (!((Object)cmp != (Object)null))
		{
			return "";
		}
		return cmp.GetName();
	}
}
