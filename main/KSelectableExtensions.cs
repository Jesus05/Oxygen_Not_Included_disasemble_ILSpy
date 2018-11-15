using UnityEngine;

public static class KSelectableExtensions
{
	public static string GetProperName(this Component cmp)
	{
		if ((Object)cmp != (Object)null && (Object)cmp.gameObject != (Object)null)
		{
			return cmp.gameObject.GetProperName();
		}
		return string.Empty;
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
		return string.Empty;
	}

	public static string GetProperName(this KSelectable cmp)
	{
		if ((Object)cmp != (Object)null)
		{
			return cmp.GetName();
		}
		return string.Empty;
	}
}
