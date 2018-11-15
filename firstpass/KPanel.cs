using UnityEngine;
using UnityEngine.UI;

public class KPanel : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Image image = GetComponent<Image>();
		if ((Object)image == (Object)null)
		{
			image = base.gameObject.AddComponent<Image>();
		}
		image.type = Image.Type.Sliced;
	}
}
