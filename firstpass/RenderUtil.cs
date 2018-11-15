using UnityEngine;

public static class RenderUtil
{
	public static void EnableRenderer(Transform node, bool is_enabled)
	{
		if ((Object)node != (Object)null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if ((Object)component != (Object)null)
			{
				component.enabled = is_enabled;
			}
			for (int i = 0; i < node.childCount; i++)
			{
				EnableRenderer(node.GetChild(i), is_enabled);
			}
		}
	}
}
