using UnityEngine;

public static class TracesExtesions
{
	public static void DeleteObject(this GameObject go)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if ((Object)component != (Object)null)
		{
			component.Trigger(1502190696, go);
		}
		Object.Destroy(go);
	}

	public static void DeleteObject(this Component cmp)
	{
		cmp.gameObject.DeleteObject();
	}
}
