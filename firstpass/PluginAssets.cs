using UnityEngine;

public class PluginAssets : MonoBehaviour
{
	public static PluginAssets Instance;

	public TextStyleSetting defaultTextStyleSetting;

	private void Awake()
	{
		Instance = this;
	}
}
